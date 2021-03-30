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
    /// Represents global attribute and variable's attributes.
    /// </summary>
    public class NcAtt : NcTypedComponent
    {
        #region Member Variables
        /// <summary>
        /// Holds data type and data of attribute
        /// </summary>
        private NcValues value;
        #endregion

        #region Constructor
        /// <summary>
        /// Sets Attribute name, type and its value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        internal NcAtt(string name, NcType type, object value)
        {
            this.Name = name;
            this.Type = type;
            int length = 0;
            switch(type)
            {
                case NcType.NcByte:
                    byte[] bArr = (byte[])value;
                    length = bArr.Length;
                    break;
                case NcType.NcChar:
                    length = value.ToString().Length;  // string length
                    break;
                case NcType.NcShort:
                    Int16[] iArr = (Int16[])value;
                    length = iArr.Length;
                    break;
                case NcType.NcInt:
                    int[] iArray = (int[])value;
                    length = iArray.Length;
                    break;
                case NcType.NcFloat:
                    float[] fArr = (float[])value;
                    length = fArr.Length;
                    break;
                case NcType.NcDouble:
                    double[] dArr = (double[])value;
                    length = dArr.Length;
                    break;
            }
            this.value = new NcValues(type, length, value);
        }
        #endregion

        #region Property

        /// <summary>
        /// Value of Attribute. Holds data type and data.
        /// </summary>
        public NcValues Value
        {
            get
            {
                return this.value;
            }
        }
        #endregion
                
    }
}
