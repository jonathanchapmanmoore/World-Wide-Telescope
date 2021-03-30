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
    /// NcTypedComponent is an abstract base class for NcVar and NcAtt
    /// that captures the similarities between netCDF variables and
    /// attributes. We list here the member functions that variables
    /// and attributes inherit from NcTypedComponent, but these member
    /// functions are also documented under the NcVar and NcAtt classes
    /// for convenience.
    /// </summary>
    public class NcTypedComponent
    {
        #region Member Variables
        private string name;
        private NcType type = NcType.NcByte;
        private int numValues = 0;
        #endregion

        #region Properties
        /// <summary>
        /// The name of the variable or attribute.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        
        /// <summary>
        /// the type of the variable or attribute. The type will be one of
        /// NcByte, NcChar, NcShort, NcInt, NcFloat, or NcDouble.
        /// </summary>
        public NcType Type
        {
            get { return type; }
            set { type = value; }

        }

       
        /// <summary>
        /// The number of values for an attribute or variable. For an attribute,
        /// this is just 1 for a scalar attribute, the number of values for a
        /// vector-valued attribute, and the number of characters for a string-valued
        /// attribute. For a variable, this is the product of the dimension sizes for
        /// all the variable's dimensions.
        /// </summary>
        public virtual int NumValues
        {
            get { return numValues; }
            set { numValues = value; }
        }

        public virtual NcValues Values
        {
            get { return null; }
        }
        #endregion

        #region Public Methods
            #region Not Implemented
        /// <summary>
        /// Renames the variable or attribute.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool Rename(string newName)
        {
            Name = newName;
            return true;
        }

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
   