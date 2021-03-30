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

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    public enum NetCDFFileFormat
    {
        /// <summary>
        /// NetCDF classic is the default format for all versions of netCDF
        /// </summary>
        Classic,
        /// <summary>
        /// Nearly identical to netCDF classic format, it uses 64-bit offsets 
        /// (hence the name), and allows users to create far larger datasets
        /// </summary>
        Offset64Bits,
        /// <summary>
        /// NetCDF-4 is not yet ready for operational use
        /// </summary>
        NetCDF4,
        /// <summary>
        /// NetCDF-4 is not yet ready for operational use
        /// </summary>
        NetCDF4Classic
    }
}
