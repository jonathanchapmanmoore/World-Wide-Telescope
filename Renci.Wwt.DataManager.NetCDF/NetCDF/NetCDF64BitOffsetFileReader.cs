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
using System.IO;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    public class NetCDF64BitOffsetFileReader : NetCDFReader
    {
        
        public NetCDF64BitOffsetFileReader(BinaryReader theReader)
        {
            //this.fileReader = theReader;
        }

        public override T[] readVariable<T>(string aName)
        {
            throw new NotImplementedException();
        }


        public override T[] readVariable<T>(string aName, uint theRecordIndex)
        {
            throw new NotImplementedException();
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
