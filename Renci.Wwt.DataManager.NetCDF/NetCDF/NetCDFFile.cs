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
using WorkflowData;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    public class NetCDFFile
    {
        private NetCDFReader reader;
        private HyperCube cube;
        private NcMetaData metaData;

		public NetCDFFile()
		{
		}

        public NetCDFFile(NetCDFReader reader, HyperCube cube)
        {
            this.reader = reader;
            this.cube = cube;
            this.metaData = NetCDFFile.GetMetaData(reader);
        }

		public NetCDFReader NetCDFMetadata
        {
            get
            {
                return this.reader;
            }
			set
			{
				this.reader = value;
			}
        }

		public HyperCube HyperCube
        {
            get
            {
                return this.cube;
            }
			set
			{
				this.cube = value;
			}
        }


        public NcMetaData NcMetaData
        {
            get
            {
                return this.metaData;

            }
            set
            {
                this.metaData = value;
            }
        }


        /// <summary>
        /// Fills the NcMetada with file metadata
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
		public static NcMetaData GetMetaData(NetCDFReader reader)
        {
            NcMetaData metadata = new NcMetaData();

            //Add No of records
            metadata.NumberOfRecords = (int)reader.NumberOfRecords;

            //Add Dimensions
            Dictionary<uint, INetCDFDimension>.Enumerator dimEnumerator
                                    = reader.Dimensions.GetEnumerator();
            while (dimEnumerator.MoveNext())
            {
                string name = dimEnumerator.Current.Value.Name;
                int size = (int)dimEnumerator.Current.Value.Length;
                NcDim dim = new NcDim(name, size);
                //dim.IsUnlimited = dimEnumerator.Current.Value.IsRecordDimension;
                
                metadata.AddDimension(dim);
            }

            //Add Global attributes
            Dictionary<string, INetCDFAttribute>.Enumerator attrEnumerator
                                    = reader.Attributes.GetEnumerator();
            while (attrEnumerator.MoveNext())
            {
                NcType attType = (NcType)attrEnumerator.Current.Value.DataType; 
                string attName = attrEnumerator.Current.Key; //Get Attname here
                object attValue = attrEnumerator.Current.Value.Value; 
                NcAtt att = new NcAtt(attName, attType, attValue);
                metadata.AddGlobalAttribute(att);
            }

            //Add Variables
            Dictionary<string, INetCDFVariable>.Enumerator varEnumerator
            = reader.Variables.GetEnumerator();

            while (varEnumerator.MoveNext())
            {
                string name = varEnumerator.Current.Value.Name;
                //object value = varEnumerator.Current.Value;
                NcType type = (NcType)varEnumerator.Current.Value.DataType; 
                NcVar var = new NcVar(name, type);

                //Add dimenstions  to variables
                var.NumValues = (int)varEnumerator.Current.Value.NumValues;
                for (int i = 0; i < varEnumerator.Current.Value.DimensionIDs.Length; i++)
                {
                    uint dimID = varEnumerator.Current.Value.DimensionIDs[i];
                    NcDim dim = new NcDim(reader.Dimensions[dimID].Name, (int)reader.Dimensions[dimID].Length);
                    var.Dimensions.Insert(i,dim);
                }

                // Add variable attributes
                Dictionary<string, INetCDFAttribute>.Enumerator vattEnumerator
                      = varEnumerator.Current.Value.Attributes.GetEnumerator();
                while (vattEnumerator.MoveNext())
                {
                    NcType attType = (NcType)vattEnumerator.Current.Value.DataType; 
                    string attName = vattEnumerator.Current.Key; //Get Attname here
                    object attValue = vattEnumerator.Current.Value.Value; 
                    NcAtt att = new NcAtt(attName, attType, attValue);
                    var.VariableAttributes.Add(att);
                }
                
                metadata.AddVariable(var);
            }

            return metadata;
        }
    }
}
