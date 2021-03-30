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
    public interface INetCDFVariable
    {
        string Name { get; }

        uint[] DimensionIDs { get; }

        Dictionary<string, INetCDFAttribute> Attributes { get; }

        NetCDFDataType DataType { get; }

        /// <summary>
        /// If not a record variable,
        /// the amount of space, in bytes, allocated to
        /// that variable’s data. This number is the
        /// product of the dimension lengths times the
        /// size of the type, padded to a four byte
        /// boundary. If a record variable, it is the
        /// amount of space per record. The netCDF
        /// "record size" is calculated as the sum of
        /// the vsize’s of the record variables.
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// Returns true if and only if the first dimension of 
        /// the variable is a record dimension (i.e. unlimited dimension
        /// )
        /// </summary>
        bool IsRecordVariable { get; }

        int DataOffset { get; }

        uint NumValues { get; }
    }
}
