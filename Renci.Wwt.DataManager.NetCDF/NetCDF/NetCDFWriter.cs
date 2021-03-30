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
using WorkflowData;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    public class NetCDFWriter
    {
        private static FileStream fs = null;
        private static BinaryWriter writer = null;
        private static HyperCube dataCube = null;
        private static NetCDFReader reader = null;
        static Dictionary<string, int> offsetDic = new Dictionary<string, int>();

        public static void Close()
        {
            try
            {
                fs.Close();
                writer.Close();

            }
            catch (Exception)
            {
                //Handle exception here
            }
        }

        #region Public Methods

        #region COMMENTED: CreateFile with NcMetaData object

        public static void CreateFile(string fileName, NetCDFFile ncFile)
        {
            NetCDFWriter.dataCube = null;
            NetCDFWriter.fs = null;
            NetCDFWriter.offsetDic = null;
            NetCDFWriter.reader = null;
            NetCDFWriter.writer = null;

            using (NetCDFWriter.fs = new FileStream(fileName, FileMode.Create))
            {
                NetCDFWriter.offsetDic = new Dictionary<string, int>();
                NetCDFWriter.writer = new BinaryWriter(fs);
                NetCDFWriter.reader = ncFile.NetCDFMetadata;
                NetCDFWriter.dataCube = ncFile.HyperCube;

                int fileFormat = 0;
                if (reader is ClassicNetCDFFileReader)
                    fileFormat = 1;
                else if (reader is NetCDF64BitOffsetFileReader)
                    fileFormat = 2;

                WriteFileHeader(fileFormat);  //Magic number

                //get the record count
                int recCount = (int)reader.NumberOfRecords;

                WriteRecordCount(recCount);
                //write dimension number
                int dimCount = (int)reader.NumberOfDimensions;
                //int dimCount = ncMD.Dimensions.Count;

                WriteDimensionHeader(dimCount);

                Dictionary<uint, INetCDFDimension>.Enumerator dimEnumerator
                 = reader.Dimensions.GetEnumerator();
                while (dimEnumerator.MoveNext())
                {
                    WriteDimension(dimEnumerator.Current.Value.Name, (int)dimEnumerator.Current.Value.Length);
                }
                //NcDim recDim = null;  //THIS is required while seperating the records variable and no record variable writing below
                //foreach (NcDim dim in ncMD.Dimensions)
                //{
                //    WriteDimension(dim.Name, dim.Size);   //SIZE == LENGTH ?? Check
                //    //if(dim.IsUnlimited)
                //    //    recDim = dim;
                //}

                #region Ignoring Global Attributes

                WriteAttributeHeader(0);

                ////WriteAttributeHeader((int)reader.NumberOfGlobalAttributes);
                //WriteAttributeHeader(ncMD.GlobalAttributes.Count);
                ////Dictionary<string, INetCDFAttribute>.Enumerator attrEnumerator
                ////= reader.Attributes.GetEnumerator();
                ////while (attrEnumerator.MoveNext())
                ////{
                ////    int attType = (int)attrEnumerator.Current.Value.DataType;
                ////    string attName = attrEnumerator.Current.Key; //Get Attname here
                ////    object[] attValue = new object[] { attrEnumerator.Current.Value.Value };
                ////    WriteAttribute(attType, attName, attValue);
                ////    
                ////}
                //foreach (NcAtt gAtt in ncMD.GlobalAttributes)
                //{
                //    NcType type = gAtt.Type;
                //    WriteAttribute(type, gAtt.Name, gAtt.Values);
                //}

                #endregion

                #region Commented : Writing All the Variables defined in Reader

                //WriteVariableHeader((int)reader.NumberOfVariables);

                //Dictionary<string, INetCDFVariable>.Enumerator varEnumerator
                //= reader.Variables.GetEnumerator();

                //#region Write Variable MetaData
                //while (varEnumerator.MoveNext())
                //{
                //    WriteVariable(varEnumerator.Current.Value);
                //}
                //#endregion

                //#region write variable values

                //varEnumerator = reader.Variables.GetEnumerator();

                //while (varEnumerator.MoveNext())
                //{
                //    string variableName = varEnumerator.Current.Value.Name;
                //    int numval = (int)varEnumerator.Current.Value.NumValues;
                //    int variableOffset = offsetDic[variableName];
                //    object[] variableValue = ReadCubeData(variableName, numval);
                //    WriteVariableValues(varEnumerator.Current.Value.DataType, variableOffset, variableName, variableValue);

                //}
                //#endregion
                #endregion

                #region Writing variables which are defined in HyperCube

                WriteVariableHeader(dataCube.Schema.ColumnCount);
                //WriteVariableHeader(ncMD.Variables.Count);   //Add variable count


                string columnname = string.Empty;
                List<INetCDFVariable> varlist = new List<INetCDFVariable>();

                for (int index = 0; index < dataCube.Schema.ColumnCount; index++)
                {
                    columnname = dataCube.Schema.GetColumnName(index);
                    INetCDFVariable myvariable = GetNetCDFvariable(columnname);
                    if (null != myvariable)
                    {
                        varlist.Add(myvariable);
                    }
                }

                //Write metadata for All variables
                foreach (INetCDFVariable myvar in varlist)
                {
                    WriteVariable(myvar);
                }

                #region write variable values

                //First write non record variables
                foreach (INetCDFVariable myVariable1 in varlist)
                {
                    if (!myVariable1.IsRecordVariable)
                    {
                        int variableOffset = offsetDic[myVariable1.Name];
                        object[] variableValue = ReadCubeNonRecordData(myVariable1);
                        int recSize = (int)myVariable1.Size;
                        WriteVariableValues(myVariable1.DataType, variableOffset, myVariable1.Name, recSize, variableValue);
                    }
                }

                //Write Data for Record Variables
                for (int r = 0; r < (int)reader.NumberOfRecords; r++)
                {
                    foreach (INetCDFVariable myVariable1 in varlist)
                    {
                        if (myVariable1.IsRecordVariable)
                        {
                            int variableOffset = offsetDic[myVariable1.Name];
                            if (r > 0)
                            {
                                variableOffset = -1; 
                                // means no need to update the offset for next record of the same variable
                                // This is handled in WriteVariableValues() function called below

                            }

                            //First Dimension is the Record Dimension
                            //uint recordDimensionIndex = (uint)myVariable1.DimensionIDs.GetValue(0);

                            object[] variableValue = ReadCubeRecordData(myVariable1, ncFile, r);
                            int recSize = (int)(myVariable1.Size);
                            WriteVariableValues(myVariable1.DataType, variableOffset, myVariable1.Name, recSize, variableValue);
                        }
                    }
                }
                #endregion

                #endregion

                NetCDFWriter.Close();
            }// end of using
        }


        #endregion

        #region CreateFile with earliear version

        //public static void CreateFile(string fileName, NetCDFFile ncFile)
        //{
        //    NetCDFWriter.dataCube = null;
        //    NetCDFWriter.fs = null;
        //    NetCDFWriter.offsetDic = null;
        //    NetCDFWriter.reader = null;
        //    NetCDFWriter.writer = null;

        //    using (NetCDFWriter.fs = new FileStream(fileName, FileMode.Create))
        //    {
        //        NetCDFWriter.offsetDic = new Dictionary<string, int>();
        //        NetCDFWriter.writer = new BinaryWriter(fs);
        //        NetCDFWriter.reader = ncFile.NetCDFMetadata;
        //        NetCDFWriter.dataCube = ncFile.HyperCube;

        //        int fileFormat = 0;
        //        if (reader is ClassicNetCDFFileReader)
        //            fileFormat = 1;
        //        else if (reader is NetCDF64BitOffsetFileReader)
        //            fileFormat = 2;

        //        WriteFileHeader(fileFormat);  //Magic number

        //        //get the record count
        //        int recCount = (int)reader.NumberOfRecords;
        //        WriteRecordCount(recCount);
        //        //write dimension number
        //        int dimCount = (int)reader.NumberOfDimensions;

        //        WriteDimensionHeader(dimCount);

        //        Dictionary<uint, INetCDFDimension>.Enumerator dimEnumerator
        //         = reader.Dimensions.GetEnumerator();
        //        while (dimEnumerator.MoveNext())
        //        {
        //            WriteDimension(dimEnumerator.Current.Value.Name, (int)dimEnumerator.Current.Value.Length);
        //        }

        //        #region Ignoring Global Attributes

        //        WriteAttributeHeader(0);

        //        //WriteAttributeHeader((int)reader.NumberOfGlobalAttributes);
        //        //Dictionary<string, INetCDFAttribute>.Enumerator attrEnumerator
        //        //= reader.Attributes.GetEnumerator();
        //        //while (attrEnumerator.MoveNext())
        //        //{
        //        //    int attType = (int)attrEnumerator.Current.Value.DataType;
        //        //    string attName = attrEnumerator.Current.Key; //Get Attname here
        //        //    object[] attValue = new object[] { attrEnumerator.Current.Value.Value };
        //        //    WriteAttribute(attType, attName, attValue);
        //        //    
        //        //}

        //        #endregion

        //        #region Commented : Writing All the Variables defined in Reader

        //        //WriteVariableHeader((int)reader.NumberOfVariables);

        //        //Dictionary<string, INetCDFVariable>.Enumerator varEnumerator
        //        //= reader.Variables.GetEnumerator();

        //        //#region Write Variable MetaData
        //        //while (varEnumerator.MoveNext())
        //        //{
        //        //    WriteVariable(varEnumerator.Current.Value);
        //        //}
        //        //#endregion

        //        //#region write variable values

        //        //varEnumerator = reader.Variables.GetEnumerator();

        //        //while (varEnumerator.MoveNext())
        //        //{
        //        //    string variableName = varEnumerator.Current.Value.Name;
        //        //    int numval = (int)varEnumerator.Current.Value.NumValues;
        //        //    int variableOffset = offsetDic[variableName];
        //        //    object[] variableValue = ReadCubeData(variableName, numval);
        //        //    WriteVariableValues(varEnumerator.Current.Value.DataType, variableOffset, variableName, variableValue);

        //        //}
        //        //#endregion
        //        #endregion

        //        #region Writing variables which are defined in HyperCube

        //        WriteVariableHeader(dataCube.Schema.ColumnCount);

        //        string columnName = string.Empty;
        //        List<INetCDFVariable> varList = new List<INetCDFVariable>();

        //        for (int index = 0; index < dataCube.Schema.ColumnCount; index++)
        //        {
        //            columnName = dataCube.Schema.GetColumnName(index);
        //            INetCDFVariable myVariable = GetNetCDFvariable(columnName);
        //            if (null != myVariable)
        //            {
        //                varList.Add(myVariable);
        //            }
        //        }

        //        //Write MetaData for non-record variables
        //        foreach (INetCDFVariable myVar in varList)
        //        {
        //            if (!myVar.IsRecordVariable)
        //            {
        //                WriteVariable(myVar);
        //            }
        //        }

        //        //Write MetaData for record variables
        //        foreach (INetCDFVariable myVar in varList)
        //        {
        //            if (myVar.IsRecordVariable)
        //            {
        //                WriteVariable(myVar);
        //            }
        //        }
        //        #region write variable values

        //        //First write non record variables
        //        foreach (INetCDFVariable myVariable1 in varList)
        //        {
        //            if (!myVariable1.IsRecordVariable)
        //            {
        //                int variableOffset = offsetDic[myVariable1.Name];
        //                object[] variableValue = ReadCubeNonRecordData(myVariable1);
        //                WriteVariableValues(myVariable1.DataType, variableOffset, myVariable1.Name, variableValue);

        //            }
        //        }

        //        //Write Data for Record Variables
        //        foreach (INetCDFVariable myVariable1 in varList)
        //        {
        //            if (myVariable1.IsRecordVariable)
        //            {
        //                int variableOffset = offsetDic[myVariable1.Name];
        //                //First Dimension is the Record Dimension
        //                uint recordDimensionIndex = (uint)myVariable1.DimensionIDs.GetValue(0);

        //                uint productOfDimLength = 1;

        //                for (uint index = recordDimensionIndex + 1; index < myVariable1.DimensionIDs.Length; index++)
        //                {
        //                    productOfDimLength = productOfDimLength * reader.Dimensions[index].Length;
        //                }

        //                int rc = (int)(myVariable1.NumValues / productOfDimLength);
        //                object[] variableValue = ReadCubeRecordData(myVariable1,ncFile, rc);
        //                WriteVariableValues(myVariable1.DataType, variableOffset, myVariable1.Name, variableValue);
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        NetCDFWriter.Close();
        //    }// end of using
        //}

        #endregion

        #endregion


        #region Private Methods

        private static INetCDFVariable GetNetCDFvariable(string variableName)
        {
            return reader.Variables[variableName];
        }

        private static object[] ReadCubeNonRecordData(INetCDFVariable variable)
        {

            int colCount = dataCube.Schema.ColumnCount;
            object[] data = new object[(int)variable.NumValues];
            int colindex = -1;

            try
            {
                colindex = dataCube.Schema.LookupColumnIndex(variable.Name);
            }
            catch //Eat Exception
            {
                return null;
            }
            try
            {
                int counter = 0;

                int xLen = 1; //dataCube.GetAxisLength(HyperCube.Axis.X);  //default lenght will be 1
                int yLen = 1; //dataCube.GetAxisLength(HyperCube.Axis.Y);
                int zLen = 1; //dataCube.GetAxisLength(HyperCube.Axis.Z);
                int tLen = 1;// dataCube.GetAxisLength(HyperCube.Axis.T);

                for (int ii = 0; ii < variable.DimensionIDs.Length; ii++)
                {
                    uint dimId = variable.DimensionIDs[ii];

                    if (dataCube.GetDimensionName(HyperCube.Axis.X) == reader.Dimensions[dimId].Name)
                        xLen = (int)reader.Dimensions[dimId].Length;
                    else if (dataCube.GetDimensionName(HyperCube.Axis.Y) == reader.Dimensions[dimId].Name)
                        yLen = (int)reader.Dimensions[dimId].Length;
                    else if (dataCube.GetDimensionName(HyperCube.Axis.Z) == reader.Dimensions[dimId].Name)
                        zLen = (int)reader.Dimensions[dimId].Length;
                    else if (dataCube.GetDimensionName(HyperCube.Axis.T) == reader.Dimensions[dimId].Name)
                        tLen = (int)reader.Dimensions[dimId].Length;
                }

                for (int i = 0; i < xLen; i++)
                {
                    for (int j = 0; j < yLen; j++)
                    {
                        for (int k = 0; k < zLen; k++)
                        {
                            for (int m = 0; m < tLen; m++)
                            {
                                object val = (object)dataCube[i, j, k, m][colindex];
                                if (val != null)
                                {
                                    data[counter] = val;
                                    counter++;
                                }
                            }
                        }
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        private static object[] ReadCubeRecordData(INetCDFVariable variable, NetCDFFile ncFile, int recordCounter)
        {

            int colCount = dataCube.Schema.ColumnCount;
            object[] data = new object[(int)variable.NumValues];
            int colindex = -1;

            try
            {
                colindex = dataCube.Schema.LookupColumnIndex(variable.Name);
            }
            catch //Eat Exception
            {
                return null;
            }
            try
            {
                int r = recordCounter;
                //for (int r = 0; r < recordCounter; r++)
                {
                    int counter = 0;

                    int xLen = 1; // dataCube.GetAxisLength(HyperCube.Axis.X);  //default lenght will be 1
                    int yLen = 1; // dataCube.GetAxisLength(HyperCube.Axis.Y);
                    int zLen = 1; // dataCube.GetAxisLength(HyperCube.Axis.Z);

                    for (uint ii = 0; ii < variable.DimensionIDs.Length; ii++)
                    {
                        uint dimId = variable.DimensionIDs[ii];

                        if (dataCube.GetDimensionName(HyperCube.Axis.X) == reader.Dimensions[dimId].Name)
                            xLen = (int)reader.Dimensions[dimId].Length;
                        else if (dataCube.GetDimensionName(HyperCube.Axis.Y) == reader.Dimensions[dimId].Name)
                            yLen = (int)reader.Dimensions[dimId].Length;
                        else if (dataCube.GetDimensionName(HyperCube.Axis.Z) == reader.Dimensions[dimId].Name)
                            zLen = (int)reader.Dimensions[dimId].Length;
                    }

                    //Ignore X axis here 
                    for (int j = 0; j < xLen; j++)
                    {
                        for (int k = 0; k < yLen; k++)
                        {
                            for (int m = 0; m < zLen; m++)
                            {
                                object val = (object)dataCube[j, k, m, r][colindex];
                                if (val != null)
                                {
                                    data[counter] = val;
                                    counter++;
                                }
                            }
                        }
                    }
                }

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }



        /// <summary>
        /// Writes the nc file header
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="fileFormat"></param>
        private static void WriteFileHeader(int fileFormat)
        {
            byte[] magicNos = new byte[4];
            magicNos[0] = Convert.ToByte('C');
            magicNos[1] = Convert.ToByte('D');
            magicNos[2] = Convert.ToByte('F');
            byte val = (fileFormat == 1) ? (byte)1 : (byte)2;
            magicNos[3] = val;
            writer.Write(magicNos);
        }

        /// <summary>
        /// Adds the number of records in the file
        /// </summary>
        /// <param name="count"></param>
        private static void WriteRecordCount(int count)
        {
            WriteInteger(count);
        }

        private static void WriteDimensionHeader(int dimensionCount)
        {
            if (dimensionCount < 0)
                throw new ArgumentException("Dimensions cannot be less than zero");

            if (dimensionCount.Equals(0))
            {
                WriteInteger(0);
            }
            else
            {
                WriteInteger((int)NetCDFMetadataType.NcDimension);
            }

            WriteInteger(dimensionCount);
        }

        /// <summary>
        /// Writes the dimesions in to nc file
        /// </summary>
        /// <param name="writer"></param>
        private static void WriteDimension(string name, int dimensionLength)
        {
            //Number of Characters in Dimension Name
            WriteInteger(name.Length);
            //Dimension Name
            WriteString(name);
            //Length Of Dimension
            WriteInteger(dimensionLength);
        }

        /// <summary>
        /// Writes attributes header for both global and variable attributes
        /// </summary>
        /// <param name="writer"></param>
        private static void WriteAttributeHeader(int count)
        {
            // Global attribute tag
            if (count.Equals(0))
            {
                WriteInteger(0);
            }
            else
            {
                WriteInteger((int)NetCDFMetadataType.NcAttribute);
            }
            //Count of global attributes
            WriteInteger(count);
        }

        /// <summary>
        /// Writes attribute 
        /// </summary>
        /// <param name="attribType"></param>
        /// <param name="attribName"></param>
        /// <param name="attribVal"></param>
        public static void WriteAttribute(NetCDFDataType dataType, string attribName, object[] attVal)
        {
            //Number of Characters in name 
            WriteInteger(attribName.Length);
            //write attribute name
            WriteString(attribName);
            //Write data type tag for the attribute
            WriteInteger((int)dataType);
            //Write variable / attribute values as per the data type
            for (int i = 0; i < attVal.Length; i++)
            {
                WriteAttValue(dataType, attVal[i]);
            }

        }

        /// <summary>
        /// Writes variable header
        /// </summary>
        /// <param name="count"></param>
        private static void WriteVariableHeader(int count)
        {
            if (0 == count)
                WriteInteger(0);
            else
                WriteInteger((int)NetCDFMetadataType.NcVariable);

            WriteInteger(count);
        }



        private static void WriteVariable(INetCDFVariable variable)
        {
            //Number of Characters in variable's name
            WriteInteger(variable.Name.Length);

            //write variable name
            WriteString(variable.Name);

            //Write dimension count of the variable
            WriteInteger(variable.DimensionIDs.Length);
            for (uint i = 0; i < variable.DimensionIDs.Length; i++)
            {
                WriteInteger((int)variable.DimensionIDs[i]);
            }
            #region IGNORING Variable Attribute Info
            WriteAttributeHeader(0);
            //WriteAttributeHeader(variable.Attributes.Count);

            //Dictionary<string, INetCDFAttribute>.Enumerator attributeEnumerator
            //= variable.Attributes.GetEnumerator();
            //while (attributeEnumerator.MoveNext())
            //{
            //    
            //int attType = (int)attributeEnumerator.Current.Value.DataType;
            //    string attName = attributeEnumerator.Current.Key;
            //    object[] attValue = new object[] { attributeEnumerator.Current.Value.Value };
            //    WriteAttribute(attType, attName, attValue);
            //}
            #endregion

            //write variable type
            WriteInteger((int)variable.DataType);

            //this is the position at which the nect four bytes contains
            // the bytes used by the variable and the offset value. This needs
            //to be updated everytime the new records is added to the file
            int variablePosition = (int)writer.BaseStream.Length;

            //Store this position in a structure for possible future reference
            offsetDic.Add(variable.Name, variablePosition);

            //write next 8 bytes with zero values of variable size and variable offset values
            //DONT REMOVE THIS TWO LINES.
            WriteInteger(0);  //size
            WriteInteger(0);  //offset value in file for this variable

        }

        
        private static void WriteVariableValues(NetCDFDataType dataType, int offset, string variableName, int recordSize, object[] values)
        {
            //First overwrite the size bytes and offset values for the variable
            //HERE THE SIZE AND OFFSET VALUES ARE OVERWRITTEN.
            if (offset != -1)
            {
                writer.Seek(offset, SeekOrigin.Begin);
                //Size
                //WriteInteger(GetByteSize((int)dataType));
                WriteInteger(GetByteSize(recordSize));

                // update offset value only for the first time
                WriteInteger((int)writer.BaseStream.Length);
            }

            // Seek to the end of the file again if the record is updated.
            writer.Seek((int)writer.BaseStream.Length, SeekOrigin.Begin);
            
            //Now start writing values of the variable
            for (int i = 0; i < values.Length; i++)
            {
                WriteVarValue(dataType, values[i]);
            }
        }




        private static int GetByteSize(int variableType)
        {
            switch (variableType)
            {
                case 1: // NC_BYTE
                    {
                        return 1;
                    }
                case 2: // NC_CHAR
                    {
                        return 1;
                    }
                case 3: //NC_SHORT
                    {
                        return 4;
                    }
                case 4: // NC_INT
                    {
                        return 4;
                    }
                case 5: // NC_FLOAT
                    {
                        return 4;
                    }
                case 6: // NC_DOUBLE
                    {
                        return 8;
                    }
            }
            return 0;
        }

        #region Binary Writer Methods

        private static void WriteAttValue(NetCDFDataType dataType, object value)
        {
            switch (dataType)
            {
                case NetCDFDataType.NcByte:
                    byte byteValue = (byte)value;
                    writer.Write(byteValue);
                    break;
                case NetCDFDataType.NcChar:
                    string[] strValue = (string[])value;
                    for (int i = 0; i < strValue.Length; i++)
                    {
                        WriteInteger(strValue[i].Length);
                        WriteString(strValue[i]);
                    }
                    break;
                case NetCDFDataType.NcShort:
                    short[] shA = (short[])value;
                    WriteInteger(shA.Length);
                    for (int i = 0; i < shA.Length; i++)
                        WriteShort(shA[i]);
                    break;
                case NetCDFDataType.NcInt:
                    int[] intA = (int[])value;
                    WriteInteger(intA.Length);
                    for (int i = 0; i < intA.Length; i++)
                        WriteInteger(intA[i]);
                    break;
                case NetCDFDataType.NcFloat:
                    float[] flA = (float[])value;
                    WriteInteger(flA.Length);
                    for (int i = 0; i < flA.Length; i++)
                        WriteFloat(flA[i]);
                    break;
                case NetCDFDataType.NcDouble:
                    double[] dbA = (double[])value;
                    WriteInteger(dbA.Length);
                    for (int i = 0; i < dbA.Length; i++)
                        WriteDouble(dbA[i]);
                    break;

            }
        }

        private static void WriteVarValue(NetCDFDataType dataType, object value)
        {
            switch (dataType)
            {
                case NetCDFDataType.NcByte:
                    if (null != value)
                    {
                        byte byteValue = (byte)value;
                        writer.Write(byteValue);
                    }
                    else
                    {
                        writer.Write(0);
                    }
                    break;
                case NetCDFDataType.NcChar:

                    if (null != value)
                    {
                        string[] strValue = (string[])value;
                        for (int i = 0; i < strValue.Length; i++)
                        {
                            WriteInteger(strValue[i].Length);
                            WriteString(strValue[i]);
                        }
                    }
                    else
                    {
                        WriteInteger(0);
                        WriteInteger(0);
                    }
                    break;
                case NetCDFDataType.NcShort:
                    if (null != value)
                    {
                        short shA = (short)value;
                        WriteShort(shA);
                    }
                    else
                    {
                        WriteShort(-255);
                    }
                    break;
                case NetCDFDataType.NcInt:
                    if (null != value)
                    {
                        int intA = (int)value;
                        WriteInteger(intA);
                    }
                    else
                    {
                        WriteInteger(int.MinValue);
                    }
                    break;
                case NetCDFDataType.NcFloat:
                    if (null != value)
                    {
                        float flA = (float)value;
                        WriteFloat(flA);
                    }
                    else
                    {
                        WriteFloat(float.MinValue);
                    }
                    break;
                case NetCDFDataType.NcDouble:
                    if (null != value)
                    {
                        double dbA = (double)value;
                        WriteDouble(dbA);
                    }
                    else
                    {
                        WriteDouble(double.MinValue);
                    }
                    break;

            }
        }

        private static void WriteShort(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            //writer.Write(arr[3]);
            //writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        private static void WriteFloat(float value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[3]);
            writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        private static void WriteInteger(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[3]);
            writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        private static void WriteDouble(double value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[7]);
            writer.Write(arr[6]);
            writer.Write(arr[5]);
            writer.Write(arr[4]);

            writer.Write(arr[3]);
            writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);

        }
        /// <summary>
        /// Writes the string in to bytes and pad zero bytes, if required, to make the length
        /// in multiple of four 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        private static void WriteString(string value)
        {
            if (value.Length == 0)
            {
                throw new ArgumentException("string cannot be blank");
            }
            for (int i = 0; i < value.Length; ++i)
            {
                writer.Write(value[i]);
            }

            int nullByteCount = value.Length % 4;
            // No pad zero byte till the length is of multiple of 4 bytes
            if (nullByteCount > 0)
            {
                byte val = 0;
                for (int i = 0; i < 4 - nullByteCount; i++)
                {
                    writer.Write(val);
                }
            }
        }
        #endregion

        #endregion
    }
}
