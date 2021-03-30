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
    /// <summary>
    /// Used to indicate exceptions that can occur when parsing a NetCDF file. 
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// Instantiates an object of this class. 
        /// </summary>
        /// <param name="message">User friendly message for the exception</param>
        public ParserException(string message)
            : base(message)
        {

        }
    }
}
