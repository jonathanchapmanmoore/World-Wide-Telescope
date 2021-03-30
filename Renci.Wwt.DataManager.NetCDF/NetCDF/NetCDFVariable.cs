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

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    public class NetCDFVariable<T> : INetCDFVariable
    {
        private int _dataOffsetInFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="dimensions"></param>
        /// <param name="attributes"></param>
        /// <param name="offset"></param>
        public NetCDFVariable(string name, uint[] dimensions, Dictionary<string, INetCDFAttribute> attributes, NetCDFDataType dataType, uint variableSize, int dataOffset, bool isARecordVariable)
        {
            this.Name = name;
            this.DimensionIDs = dimensions;
            this.Attributes = attributes;
            this.DataType = dataType;
            this.Size = variableSize;
            this._dataOffsetInFile = dataOffset;
            this.IsRecordVariable = isARecordVariable;
        }

        public int DataOffset
        {
            get { return this._dataOffsetInFile; }
        }

        #region INetCDFVariable Members

        public string Name {get; private set;}

        public uint[] DimensionIDs {get; private set;}

        public Dictionary<string, INetCDFAttribute> Attributes {get; private set;}

        public NetCDFDataType DataType {get; private set;}

        public uint Size {get; private set;}

        public bool IsRecordVariable {get; private set;}

        public uint NumValues
        {
            get
            {
                uint typeSize = sizeof(int);
                switch (this.DataType)
                {
                    case NetCDFDataType.NcByte:
                        typeSize = sizeof(byte);
                        break;
                    case NetCDFDataType.NcChar:
                        typeSize = sizeof(char);
                        break;
                    case NetCDFDataType.NcDouble:
                        typeSize = sizeof(double);
                        break;
                    case NetCDFDataType.NcFloat:
                        typeSize = sizeof(float);
                        break;
                    case NetCDFDataType.NcInt:
                        typeSize = sizeof(int);
                        break;
                    case NetCDFDataType.NcShort:
                        typeSize = sizeof(short);
                        break;
                }

                return (this.Size / typeSize);
            }
        }
        #endregion
    }
}
