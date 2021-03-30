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
    public enum NetCDFDataType
    {
        /// <summary>
        /// 8 bit signed integer
        /// </summary>
        NcByte = 1,

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
