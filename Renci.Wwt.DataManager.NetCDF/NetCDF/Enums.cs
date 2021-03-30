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
    public class NetCDFConstants
    {
        public const byte NC_FILL_BYTE = 255;
        public const byte NC_FILL_CHAR = 0;
        public const Int16 NC_FILL_SHORT = -32767;
        public const Int32 NC_FILL_INT = -2147483647;
        public const float NC_FILL_FLOAT = 9.96921E+36F;
    }

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

    public enum NetCDFMetadataType
    {
        NcDimension = 10,
        NcVariable,
        NcAttribute
    }

    public enum NetCDFDataType
    {
        /// <summary>
        /// 8 bit signed integer
        /// </summary>
        NcByte=1,

        /// <summary>
        /// ISO/ASCII character
        /// </summary>
        NcChar,

        /// <summary>
        /// 16 bit signed integer
        /// </summary>
        NcShort,

        /// <summary>
        /// 32 bit signed integer
        /// </summary>
        NcInt,

        /// <summary>
        /// IEEE single precision floating point number
        /// </summary>
        NcFloat,

        /// <summary>
        /// IEEE double precision float
        /// </summary>
        NcDouble
    }
}