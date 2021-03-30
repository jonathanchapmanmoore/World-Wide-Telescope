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
    /// Stores metadata information such as Dimensions, Gloabal Attributes , Variables and Attributes of variables.
    /// </summary>
    public class NcMetaData
    {
        #region Member Variables
        /// <summary>
        /// List of Dimensions in Nc File
        /// </summary>
        private List<NcDim> dimensionList = null;
        /// <summary>
        /// List of Variables
        /// </summary>
        private List<NcVar> variableList = null;

        /// <summary>
        /// Global Attributes
        /// </summary>
        private List<NcAtt> globalAttributeList = null;

		int numrec = 0;

		#endregion

        #region Constructor
        public NcMetaData()
        {
            this.dimensionList = new List<NcDim>();
            this.variableList = new List<NcVar>();
            this.globalAttributeList = new List<NcAtt>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get all dimensions
        /// </summary>
        public List<NcDim> Dimensions
        {
            get
            {
                return this.dimensionList;
            }
        }
        //Get All variables
        public List<NcVar> Variables
        {
            get
            {
                return this.variableList;
            }
        }

        //Get all global attributes
        public List<NcAtt> GlobalAttributes
        {
            get
            {
                return this.globalAttributeList;
            }
        }

		public int NumberOfRecords
		{
			get
			{
				return this.numrec;
			}
			set
			{
				this.numrec = value;
			}
		}

        #endregion

        #region Public Methods

        #region Fetch Dimension, Variable,Global Attribute based on index/name
        public NcDim GetDimension(int index)
        {
            return this.dimensionList[index];
        }

        public NcVar GetVariable(int index)
        {
            return this.variableList[index];
        }

        public NcAtt GetGlobalAttribute(int index)
        {
            return this.globalAttributeList[index];
        }

        public NcDim GetDimension(string name)
        {
            if (this.dimensionList.Count > 0)
            {
                foreach (NcDim dim in this.dimensionList)
                {
                    if (dim.Name.Equals(name))
                        return dim;
                }
            }
            return null;
        }

        public NcVar GetVariable(string name)
        {
            if (this.variableList.Count > 0)
            {
                foreach (NcVar var in variableList)
                {
                    if (var.Name.Equals(name))
                        return var;
                }
            }
            return null;

        }

        public NcAtt GetGlobalAttribute(string name)
        {
            if (this.globalAttributeList.Count > 0)
            {
                foreach (NcAtt att in this.globalAttributeList)
                {
                    if (att.Name.Equals(name))
                        return att;
                }
            }
            return null;
        }
        #endregion

        #region Add Dimension, Variable and GlobalAttributes
        public void AddDimension(NcDim dim)
        {
            if (!IfExists(dim))
                dimensionList.Add(dim);
        }

        public void AddVariable(NcVar var)
        {
            variableList.Add(var);
        }

        public void AddGlobalAttribute(NcAtt globalAttr)
        {
            globalAttributeList.Add(globalAttr);
        }
        #endregion

        #region Remove Dimensions, Variable and GlobalAttribute
        public void RemoveDimension(int index)
        {
            if (index > this.dimensionList.Count)
                throw new ArgumentOutOfRangeException();

            this.dimensionList.RemoveAt(index);
        }

        public void RemoveDimension(string name)
        {

            foreach (NcDim item in this.dimensionList)
            {
                if (string.Compare(item.Name, name) == 0)
                {
                    this.dimensionList.Remove(item);
                    break;
                }
            }

        }

        public void RemoveVariable(int index)
        {
            if (index > this.variableList.Count)
                throw new ArgumentOutOfRangeException();

            this.variableList.RemoveAt(index);
        }

        public void RemoveVariable(string name)
        {

            foreach (NcVar item in this.variableList)
            {
                if (string.Compare(item.Name, name) == 0)
                {

                    this.variableList.Remove(item);
                    break;
                }
            }

        }

        public void RemoveAttribute(int index)
        {
            if (index > this.globalAttributeList.Count)
                throw new ArgumentOutOfRangeException();

            this.globalAttributeList.RemoveAt(index);
        }

        public void RemoveAttribute(string name)
        {

            foreach (NcAtt item in this.globalAttributeList)
            {
                if (string.Compare(item.Name, name) == 0)
                {

                    this.globalAttributeList.Remove(item);
                    break;
                }
            }

        }

        #endregion

        #endregion

        #region Private Methods

        #region Function to check for existence of Dimension, Variable and Global Attribute
        private bool IfExists(NcDim dim)
        {
            foreach (NcDim myDim in this.dimensionList)
            {
                if (string.Compare(myDim.Name, dim.Name) == 0)
                    return true;
            }

            return false;
        }
        private bool IfExists(NcAtt attr)
        {
            foreach (NcAtt myAttr in this.globalAttributeList)
            {
                if (string.Compare(myAttr.Name, attr.Name) == 0)
                    return true;
            }

            return false;
        }
        private bool IfExists(NcVar var)
        {
            foreach (NcVar myvar in this.variableList)
            {
                if (string.Compare(myvar.Name, var.Name) == 0)
                    return true;
            }

            return false;
        }
        #endregion


        #endregion
    }

}
