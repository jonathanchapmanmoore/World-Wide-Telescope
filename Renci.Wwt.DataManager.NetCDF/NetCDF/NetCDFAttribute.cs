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
    internal class NetCDFAttribute<T> : INetCDFAttribute
    {
        private T[] _values;

        private string _name;

        private NetCDFDataType _dataType;

        public NetCDFAttribute(string name, T[] values, NetCDFDataType dataType) 
        {
            this._values = values;
            this._name = name;
            this._dataType = dataType;
        }

        #region INetCDFAttribute Members

        public uint Length 
        {
            get { return (uint)this._values.Length; }
        }

        public NetCDFDataType DataType 
        {
            get { return this._dataType; }
        }

        public object Value 
        {
            get { return this._values; }
        }

        #endregion
    }
}
