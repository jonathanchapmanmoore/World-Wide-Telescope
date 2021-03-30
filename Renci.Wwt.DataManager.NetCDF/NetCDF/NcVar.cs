//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Apache License, Version 2.0.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    /// <summary>
    /// NcVar is derived from NcTypedComponent, and represents a netCDF
    /// variable. A netCDF variable has a name, a type, a shape, zero or
    /// more attributes, and a block of values associated with it. Because
    /// variables are only associated with open netCDF files, there are no
    /// public constructors for this class. Use member functions of NcFile
    /// to get variables or add new variables.
    /// </summary>
    public class NcVar : NcTypedComponent
    {
        #region Member varibles
        private bool isCoordinate = false;
        private int offset = 0;
        private List<NcDim> dimensions = new List<NcDim>();
        private List<NcAtt> varAttributes = new List<NcAtt>();
        private object[] valArray = null;
        private int[] productArray = null;

        /// <summary>
        /// Use  ???
        /// </summary>
        private List<int> edges;
       
        #endregion

        #region Constructors
        internal NcVar(string name)
        {
            this.Name = name;
        }

        internal NcVar(string name, NcType type)
        {
            this.Name = name;
            this.Type = type;

        }
        #endregion

        #region Properties

        /// <summary>
        /// Dimensions of variable
        /// </summary>
        public List<NcDim> Dimensions
        {
            get
            {
                return this.dimensions;
            }
        }

        /// <summary>
        /// Checks if variable is co-ordinate variable.
        /// If variable has only one dimension and name of variable is same as of dimension,
        /// then Variable is termed as Coordinate variable.
        /// </summary>
        public bool IsCoordinate
        {
            get
            {
                return this.isCoordinate;
            }
            set
            {
                this.isCoordinate = value;
            }
        }

       
        /// <summary>
        /// Gets or Sets the Variable offset value from which the data is writtent in the file
        /// </summary>
        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        /// <summary>
        /// The variable number. (Not Implemented)
        /// </summary>
        public int ID
        {
            get { return 0; }
        }


        /// <summary>
        /// The number of dimensions for this variable.
        /// </summary>
        public int NumDims
        {
            get { return this.dimensions.Count; }
        }

        /// <summary>
        /// The number of attributes attached to the variable.
        /// </summary>
        public int NumAtts
        {
            get { return this.varAttributes.Count; }
        }

       /// <summary>
       /// Attributes of variable.
       /// </summary>
        public List<NcAtt> VariableAttributes
        {
            get
            {

                return varAttributes;
            }
            set
            {
                varAttributes = value;
            }
        }

       
        /// <summary>
        /// The shape of the variable, in the form of a vector containing the
        /// sizes of the dimensions of the variable.
        /// </summary>
        public List<int> Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        /// <summary>
        /// True if the variable is valid, False otherwise. (Not Implemented
        /// </summary>
        public bool IsValid
        {
            get { return true; }
        }


        /// <summary>
        /// The number of values for a variable. This is just 1 for a scalar variable,
        /// or the product of the dimension sizes for all the variable's dimensions.
        /// </summary>
        public override int NumValues
        {
            get { return base.NumValues; }
            set { base.NumValues = value; }
        }

        /// <summary>
        /// The block of all values for the variable. The caller is responsible for
        /// deleting this block of values when no longer needed. Note that this is not a
        /// good way to read selected values of a variable; use the get member function
        /// instead, to get single values or selected cross-sections of values.
        /// </summary>
        public override NcValues Values
        {
            get { return base.Values; }
        }

        public object[] ValueArr
        {
            get
            {
                return this.valArray;
            }
            set
            {
                this.valArray = value;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// This method computes the value of the record variable. A variable
        /// is a record var if the first dimension of the variable has unlimited
        /// size
        /// </summary>
        public void SetRecordDimSize()
        {
            int totalSize = 1;
            bool isRecordVar = false;
            int dimIndex = 0;   // As per the current understanding this will always be zero.
                                // but we are doing this for safekeeping
            for (int i = 0; i < this.Dimensions.Count; ++i)
            {
                if (true == this.Dimensions[i].IsUnlimited)
                {
                    isRecordVar = true;
                    dimIndex = i;
                    totalSize = totalSize * 
                        (0 == this.Dimensions[i].Size? 1 : this.Dimensions[i].Size);
                }
            }
            Debug.Assert(0 == dimIndex);
            if (isRecordVar)
            {
                this.Dimensions[dimIndex].Size = (this.NumValues) / totalSize;
            }
        }

        public void SetDimensions(int index, NcDim dim)
        {
            this.dimensions.Insert(index, dim);
        }

        public NcDim GetDim(int index)
        {

            return this.dimensions[index];
        }

        public NcAtt GetAtt(string name)
        {
            foreach (NcAtt attribute in this.varAttributes)
            {
                if (string.Compare(attribute.Name, name) == 0)
                    return attribute;
            }
            return null;
        }

        public object GetValue(ref int[] dimIndexValArray)
        {
            if (dimIndexValArray.Length != this.Dimensions.Count)
                return null;

            if (null == this.productArray)
                ComputeProductArray();

            int indexVal = 0;
            for (int i = 0; i < dimIndexValArray.Length; ++i)
            {
                indexVal += dimIndexValArray[i] * this.productArray[i];
            }
            return this.valArray[indexVal];
        }

        /// <summary>
        /// This method walks the dimension list for this variable and matches
        /// the name passed with the dimension names to find if the variable
        /// varies accross this dimension
        /// </summary>
        /// <param name="dimensionName">Name of the dimension</param>
        /// <returns>True: If the dimnesion exists. False if it does not</returns>
        public bool HasDimension(string dimensionName)
        {
            foreach (NcDim d in this.Dimensions)
            {
                if (dimensionName == d.Name)
                    return true;
            }
            return false;
        }
        
        public NcAtt GetAtt(int index) { return null; }

        public bool Put(byte[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(char[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(short[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(int[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(long[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(float[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Put(double[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }

        public bool Put(byte[] vals, long[] counts) { return false; }
        public bool Put(char[] vals, long[] counts) { return false; }
        public bool Put(short[] vals, long[] counts) { return false; }
        public bool Put(int[] vals, long[] counts) { return false; }
        public bool Put(long[] vals, long[] counts) { return false; }
        public bool Put(float[] vals, long[] counts) { return false; }
        public bool Put(double[] vals, long[] counts) { return false; }

        public bool Get(byte[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(char[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(short[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(int[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(long[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(float[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }
        public bool Get(double[] vals, long c0, long c1, long c2, long c3, long c4) { return false; }

        public bool Get(byte[] vals, long[] counts) { return false; }
        public bool Get(char[] vals, long[] counts) { return false; }
        public bool Get(short[] vals, long[] counts) { return false; }
        public bool Get(int[] vals, long[] counts) { return false; }
        public bool Get(long[] vals, long[] counts) { return false; }
        public bool Get(float[] vals, long[] counts) { return false; }
        public bool Get(double[] vals, long[] counts) { return false; }

        /// <summary>
        /// Resets the starting corner to the values supplied. The first form works
        /// for a variable of dimensionality from scalar to 5 dimensions. For more than
        /// five dimensions, use the second form, in which the number of longfs supplied
        /// must match the rank of the variable. The method returns FALSE if any argument
        /// is greater than the size of the corresponding dimension.
        /// </summary>
        /// <param name="cur"></param>
        /// <returns></returns>
        public bool SetCur(long[] cur) { return false; }

        /// <summary>
        /// If the variable may have been renamed, make sure its name is updated.
        /// </summary>
        public void Sync() { }

        public long RecSize() { return 0; }
        public long RecSize(NcDim dim) { return 0; }

        public NcValues GetRec() { return null; }
        public NcValues GetRec(long index) { return null; }
        public NcValues GetRec(NcDim dim) { return null; }
        public NcValues GetRec(NcDim dim, long index) { return null; }

        /// <summary>
        /// Set the current record for this variable.
        /// </summary>
        /// <param name="rec"></param>
        public void SetRec(long rec) { }

        /// <summary>
        /// Set the current dimension slice for the specified dimension for this variable.
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="rec"></param>
        public void SetRec(NcDim dim, long rec) { }

        public bool PutRec(byte[] vals) { return false; }
        public bool PutRec(short[] vals) { return false; }
        public bool PutRec(int[] vals) { return false; }
        public bool PutRec(long[] vals) { return false; }
        public bool PutRec(float[] vals) { return false; }
        public bool PutRec(char[] vals) { return false; }
        public bool PutRec(double[] vals) { return false; }

        public bool PutRec(NcDim dim, byte[] vals) { return false; }
        public bool PutRec(NcDim dim, short[] vals) { return false; }
        public bool PutRec(NcDim dim, int[] vals) { return false; }
        public bool PutRec(NcDim dim, long[] vals) { return false; }
        public bool PutRec(NcDim dim, float[] vals) { return false; }
        public bool PutRec(NcDim dim, char[] vals) { return false; }
        public bool PutRec(NcDim dim, double[] vals) { return false; }

        public bool PutRec(byte[] vals, long rec) { return false; }
        public bool PutRec(short[] vals, long rec) { return false; }
        public bool PutRec(int[] vals, long rec) { return false; }
        public bool PutRec(long[] vals, long rec) { return false; }
        public bool PutRec(float[] vals, long rec) { return false; }
        public bool PutRec(char[] vals, long rec) { return false; }
        public bool PutRec(double[] vals, long rec) { return false; }

        public bool PutRec(NcDim dim, byte[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, short[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, int[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, long[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, float[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, char[] vals, long rec) { return false; }
        public bool PutRec(NcDim dim, double[] vals, long rec) { return false; }
        #endregion

        #region Private Methods

        private void ComputeProductArray()
        {
            this.productArray = new int[this.dimensions.Count];
            int lastVal = 1;
            for (int i = 0; i < this.productArray.Length; ++i)
            {
                this.productArray[productArray.Length - i - 1] = lastVal;
                lastVal = lastVal * this.dimensions[productArray.Length - i - 1].Size;
                //this.productArray[productArray.Length - i - 1] = lastVal;
            }
        }

        #endregion

    }
}
