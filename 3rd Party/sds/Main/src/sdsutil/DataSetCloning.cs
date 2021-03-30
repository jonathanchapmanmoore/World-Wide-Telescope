// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Research.Science.Data;

namespace sdsutil
{
    public static class DataSetCloning
    {
        public static DataSet Clone(DataSet src, DataSetUri dstUri)
        {
            if (src == null) throw new ArgumentNullException("src");

            // Maximum memory capacity in bytes
            int N = 200 * 1024 * 1024;
            // Estimated size of a single string in bytes
            int sizeofString = 100 * 1024;

            /***********************************************************************************
             * Preparing output
            ***********************************************************************************/
            DataSet dst = DataSet.Open(dstUri);
            if (dst.IsReadOnly)
                throw new NotSupportedException("Output DataSet is read-only");
            dst.IsAutocommitEnabled = false;

            DataSetSchema srcSchema = src.GetSchema();
            Dictionary<int, int> IDs = new Dictionary<int, int>();

            // Creating empty variables and copying global metadata and scalar variables
            Console.Out.Write("\n\nCreating structure and copying global metadata and scalar variables... ");
            foreach (VariableSchema v in srcSchema.Variables)
            {
                if (v.ID == DataSet.GlobalMetadataVariableID)
                {
                    // Copying global metadata
                    var dstGlobalMetadata = dst.Metadata;
                    foreach (var attr in v.Metadata)
                        dstGlobalMetadata[attr.Key] = attr.Value;
                    continue;
                }

                Variable t = dst.AddVariable(v.TypeOfData, v.Name, null, v.Dimensions.AsNamesArray());
                IDs.Add(v.ID, t.ID);

                foreach (var attr in v.Metadata)
                    t.Metadata[attr.Key] = attr.Value;

                if (t.Rank == 0) // scalar
                    t.PutData(src.Variables.GetByID(v.ID).GetData());
            }
            dst.Commit();
            Console.Out.WriteLine("Done.\n");
            /***********************************************************************************
             * Adjusting dimensions deltas
            ***********************************************************************************/
            Dimension[] srcDims = srcSchema.GetDimensions();
            Dictionary<string, int> deltas = new Dictionary<string, int>(srcDims.Length);
            foreach (var d in srcDims)
                deltas[d.Name] = d.Length;

            Console.Out.WriteLine("Total memory capacity: " + (N / 1024.0 / 1024.0).ToString("F2") + " Mb");
            int totalSize;
            do
            {
                totalSize = 0;
                foreach (var var in srcSchema.Variables)
                {
                    if (var.Rank == 0) continue; // scalar
                    int typeSize = SizeOf(var.TypeOfData, sizeofString);

                    int count = 0;
                    foreach (var vdim in var.Dimensions)
                    {
                        int dimDelta = deltas[vdim.Name];
                        if (count == 0) count = dimDelta;
                        else count *= dimDelta;
                    }
                    totalSize += typeSize * count;
                }
                if (totalSize > N)
                {
                    string maxDim = null;
                    int max = int.MinValue;
                    foreach (var dim in deltas)
                        if (dim.Value > max)
                        {
                            max = dim.Value;
                            maxDim = dim.Key;
                        }
                    if (maxDim == null || max <= 1)
                        throw new NotSupportedException("Cannot copy the DataSet: it is too large to be copied entirely by the utility for the provided memory capacity");
                    deltas[maxDim] = max >> 1;
                }
            } while (totalSize > N);

            // Printing deltas
            Console.Out.WriteLine("Deltas for the dimensions adjusted (max iteration capacity: " + (totalSize / 1024.0 / 1024.0).ToString("F2") + " Mb):");
            foreach (var delta in deltas)
                Console.Out.WriteLine(" Dimension " + delta.Key + ": " + delta.Value);

            /***********************************************************************************
             * Copying data
            ***********************************************************************************/
            Console.WriteLine();
            UpdateProgress(0);
            Dictionary<int, int[]> origins = new Dictionary<int, int[]>(srcSchema.Variables.Length);
            Dictionary<int, int[]> shapes = new Dictionary<int, int[]>(srcSchema.Variables.Length);
            List<VariableSchema> copyVars = srcSchema.Variables.Where(vs =>
                (vs.Rank > 0 && vs.ID != DataSet.GlobalMetadataVariableID)).ToList();

            Dictionary<string, int> dimOrigin = new Dictionary<string, int>(srcDims.Length);
            foreach (var d in srcDims)
                dimOrigin[d.Name] = 0;

            Array.Sort(srcDims, (d1, d2) => d1.Length - d2.Length);
            int totalDims = srcDims.Length;

            do
            {
                // for each variable:
                for (int varIndex = copyVars.Count; --varIndex >= 0; )
                {
                    VariableSchema var = copyVars[varIndex];
                    bool hasChanged = false;
                    // Getting its origin
                    int[] origin;
                    if (!origins.TryGetValue(var.ID, out origin))
                    {
                        origin = new int[var.Rank];
                        origins[var.ID] = origin;
                        hasChanged = true;
                    }
                    // Getting its shape
                    int[] shape;
                    if (!shapes.TryGetValue(var.ID, out shape))
                    {
                        shape = new int[var.Rank];
                        for (int i = 0; i < var.Dimensions.Count; i++)
                            shape[i] = deltas[var.Dimensions[i].Name];
                        shapes.Add(var.ID, shape);
                    }

                    // Updating origin for the variable:
                    if (!hasChanged)
                        for (int i = 0; i < shape.Length; i++)
                        {
                            int o = dimOrigin[var.Dimensions[i].Name];
                            if (origin[i] != o)
                            {
                                hasChanged = true;
                                origin[i] = o;
                            }
                        }
                    if (!hasChanged) // this block is already copied
                        continue;

                    bool doCopy = false;
                    bool shapeUpdated = false;
                    for (int i = 0; i < shape.Length; i++)
                    {
                        int s = origin[i] + shape[i];
                        int len = var.Dimensions[i].Length;
                        if (s > len)
                        {
                            if (!shapeUpdated)
                            {
                                shapeUpdated = true;
                                shape = (int[])shape.Clone();
                            }
                            shape[i] = len - origin[i];
                        }
                        if (shape[i] > 0) doCopy = true;
                    }

                    if (doCopy)
                    {
                        Array data = src.Variables.GetByID(var.ID).GetData(origin, shape);
                        // Compute real size here for strings
                        dst.Variables.GetByID(IDs[var.ID]).PutData(origin, data);
                    }
                    else // variable is copied
                    {
                        copyVars.RemoveAt(varIndex);
                    }
                }
                dst.Commit();

                // Updating dimensions origin
                bool isOver = true;
                for (int i = 0; i < totalDims; i++)
                {
                    Dimension dim = srcDims[i];
                    int origin = dimOrigin[dim.Name] + deltas[dim.Name];
                    if (origin < dim.Length)
                    {
                        dimOrigin[dim.Name] = origin;
                        isOver = false;
                        // Progress indicator
                        if (i == totalDims - 1)
                        {
                            double perc = (double)origin / dim.Length * 100.0;
                            UpdateProgress(perc);
                        }
                        break;
                    }
                    dimOrigin[dim.Name] = 0;
                }
                if (isOver) break;
            } while (copyVars.Count > 0);

            UpdateProgress(100.0);

            Console.Out.WriteLine();
            return dst;
        }

        private static void UpdateProgress(double perc)
        {
            Console.Write("\rCopying data... {0,3:F0}%", perc);
        }

        private static int SizeOf(Type type, int sizeofString)
        {
            if (type == typeof(Double)) return sizeof(Double);
            if (type == typeof(Single)) return sizeof(Single);
            if (type == typeof(Int16)) return sizeof(Int16);
            if (type == typeof(Int32)) return sizeof(Int32);
            if (type == typeof(Int64)) return sizeof(Int64);
            if (type == typeof(UInt64)) return sizeof(UInt64);
            if (type == typeof(UInt32)) return sizeof(UInt32);
            if (type == typeof(UInt16)) return sizeof(UInt16);
            if (type == typeof(Byte)) return sizeof(Byte);
            if (type == typeof(SByte)) return sizeof(SByte);
            if (type == typeof(String)) return sizeofString;
            if (type == typeof(Boolean)) return sizeof(Boolean);
            if (type == typeof(DateTime)) return sizeof(long);
            if (type == typeof(EmptyValueType)) return 1;
            return Marshal.SizeOf(type);
        }
    }
}

