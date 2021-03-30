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
//using System.Linq;
using System.Text;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    /// <summary>
    /// This class provides control for netCDF error handling. Declaring
    /// an NcError object temporarily changes the error-handling behavior
    /// for all netCDF classes until the NcError object is destroyed
    /// (typically by going out of scope), at which time the previous
    /// error-handling behavior is restored. 
    /// </summary>
    public class NcError
    {
        public enum Behavior
        {
            SilentNonfatal,
            VerboseNonfatal,
            SilentFatal,
            VerboseFatal
        }

        /// <summary>
        /// The constructor saves the previous error state for restoration
        /// when the destructor is invoked, and sets a new specified state.
        /// To control whether error messages are output from the underlying
        /// library and whether such messages are fatal or nonfatal. 
        /// </summary>
        public NcError() : this (Behavior.VerboseFatal) { }

        public NcError(Behavior b) { }

        /// <summary>
        /// Destructor, restores previous error state. 
        /// </summary>
        ~NcError() { }

        /// <summary>
        /// Returns most recent error, as enumerated in netcdf.h.
        /// </summary>
        /// <returns></returns>
        public int GetErr() { return 0; }
    }
}
