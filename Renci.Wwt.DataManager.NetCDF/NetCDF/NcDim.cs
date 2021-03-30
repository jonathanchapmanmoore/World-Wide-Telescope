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
using System.Collections;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    /// <summary>
    /// A netCDF dimension has a name and a size. Dimensions are
    /// only created and destroyed by NcFile member functions,
    /// because they cannot exist independently of an open netCDF
    /// file. Hence there are no public constructors or destructors.
    /// </summary>
    public class NcDim
    {
        #region Member Variables

        private string name;  // Dimension name
        private int size;     // Dimension size
        private bool isUnlimited;
        private bool isValid = false; // Property value
        #endregion

        #region Constructors
        internal NcDim(string name)
        {
            this.name = name;
            this.size = 0;
            this.isUnlimited = true;
            isValid = true;

        }

        internal NcDim(string name, int size)
        {
            this.name = name;
            this.size = size;
            this.isUnlimited = (size == 0) ? true : false;
            isValid=true;
        }
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the name of the dimension
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The dimension size
        /// </summary>
        public int Size
        {
            get { return this.size; }
            set { this.size = value; }
         
        }

        /// <summary>
        /// Indicates if the dimension is valid
        /// </summary>
        public bool IsValid
        {
            get 
            { 
                return this.isValid;
            }
        }

        /// <summary>
        /// Indicates if it is unlimited dimension. i.e. size is 0
        /// </summary>
        public bool IsUnlimited
        {
            // If the dimension length is 0 then it is unlimited dimension.
            get 
            {
				return (size == 0) ? true : false; 
            }
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Renames the dimension to newName.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool Rename(string newName)
        {
            this.name = newName;
            return true;
        }

        /// <summary>
        /// If the dimension may have been renamed, make sure its name is updated.
        /// (not implemented).
        /// </summary>
        /// <returns></returns>
        public bool Sync() { return false; }

        #endregion
    }   
}
