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
    internal class NetCDFDimension : INetCDFDimension
    {
        private string _name;
        private uint _size;

        public NetCDFDimension(string name, uint size) {
            this._name = name;
            this._size = size;
        }

        #region INetCDFDimension Members

        public string Name 
        {
            get { return this._name; }
        }

        public uint Length 
        {
            get { return this._size; }
        }

        /// <summary>
        /// Returns true if the Dimension is a Record Dimension.
        /// </summary>
        public bool IsRecordDimension
        {
            get
            {
                return (this.Length == 0) ? true : false;
            }
        }

        #endregion
    }
}
