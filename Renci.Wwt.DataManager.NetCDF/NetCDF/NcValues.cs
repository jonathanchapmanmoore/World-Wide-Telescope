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
    public class NcValues
    {
        #region Member Variables
        /// <summary>
        /// datatype
        /// </summary>
        private NcType type;
        /// <summary>
        /// length of type
        /// </summary>
        private int length;
        /// <summary>
        /// Holds data
        /// </summary>
        private Object data;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public NcValues() { }

        /// <summary>
        /// Constructor for a value block of the specified type and length. 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        public NcValues(NcType type, int length, Object data)
        {
            this.type = type;
            this.length = length;
            this.data = data;
        }
        #endregion

        #region Destructor
        ~NcValues() { }
        #endregion

        #region Properties
        /// <summary>
        /// The number of values in the value block.
        /// </summary>
        public long Num
        {
            get { return 0; }
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }


        public Object DataValue
        {
            get
            {
                return this.data;
            }
        }

        public NcType Type
        {
            get
            {
                return this.type;

            }
        }

        #endregion

        #region Public Methods 

            #region Not Implemented
        /// <summary>
        /// Used to print the comma-delimited sequence of values of the value block. 
        /// </summary>
        /// <param name="sw"></param>
        public void Print(StreamWriter sw) { }

        /// <summary>
        /// Returns a bland pointer to the beginning of the value block.
        /// </summary>
        /// <returns></returns>
        public List<object> BaseValues() { return null; }

        /// <summary>
        /// Returns the number of bytes required for one value.
        /// </summary>
        /// <returns></returns>
        public int BytesForOne() { return 0; }

        public byte AsByte(int index) { return 0; }
        public char AsChar(int index) { return (char)0; }
        public short AsShort(int index) { return 0; }
        public int AsInt(int index) { return 0; }
        public long AsLong(int index) { return 0; }
        public float AsFloat(int index) { return 0; }
        public double AsDouble(int index) { return 0.0; }
        public string AsString(int index) { return null; }
        #endregion
        #endregion
    }

}
