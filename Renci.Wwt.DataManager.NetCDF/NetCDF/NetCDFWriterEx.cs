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
    public class NetCDFWriterEx
    {

        #region Member Variable Declarations

        private static FileStream fs = null;
        private static BinaryWriter writer = null;
        private static HyperCube dataCube = null;
        //private static NetCDFReader reader = null;
        //private static NcMetaData metadata = null;
        //Temporarily stores the offset values for each variable while
        //writing variable values in file
        static Dictionary<string, int> offsetDic = new Dictionary<string, int>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Closes the file stream and binary writer 
        /// </summary>
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

        public static void CreateFile(string fileName, HyperCube outCube)
        {
            NetCDFWriterEx.dataCube = null;
            NetCDFWriterEx.fs = null;
            NetCDFWriterEx.offsetDic = null;
            //NetCDFWriterEx.reader = null;
            NetCDFWriterEx.writer = null;
            //NetCDFWriterEx.metadata = null;

            using (NetCDFWriterEx.fs = new FileStream(fileName, FileMode.Create))
            {
                NetCDFWriterEx.offsetDic = new Dictionary<string, int>();
                NetCDFWriterEx.writer = new BinaryWriter(fs);
                NetCDFWriterEx.dataCube = outCube;

                // For now the writer supports creation of Classic Format Files only.
                // This would be an optional input argument to the property.
                int fileFormat = 1;
                WriteFileHeader(fileFormat);  //Magic number

                //Write nunmer of records
                // T Axis. The HC Shecma object would be modified to have a property as record
                // dimension.
                WriteRecordCount(outCube.GetAxisLength(HyperCube.Axis.T));

                // If the axes are mapped to a dimension then the dimension name will not be
                // empty.
                int dimCount = 0;
                dimCount = dimCount + (true == String.IsNullOrEmpty(outCube.Schema.XDimensionName) ? 0 : 1);
                dimCount = dimCount + (true == String.IsNullOrEmpty(outCube.Schema.YDimensionName) ? 0 : 1);
                dimCount = dimCount + (true == String.IsNullOrEmpty(outCube.Schema.ZDimensionName) ? 0 : 1);
                dimCount = dimCount + (true == String.IsNullOrEmpty(outCube.Schema.TDimensionName) ? 0 : 1);

                WriteDimensionHeader(dimCount);
                // The dimensions are mapped to the HyperCube Axes. Hence writing the dimensions
                // is just writing the axis name and the length.
                if (!String.IsNullOrEmpty(outCube.Schema.TDimensionName))
                    WriteDimension(outCube.Schema.TDimensionName, 0);// As the record var is always 0
                if (!String.IsNullOrEmpty(outCube.Schema.XDimensionName))
                    WriteDimension(outCube.Schema.XDimensionName, outCube.XDimensionValues.Count);
                if (!String.IsNullOrEmpty(outCube.Schema.YDimensionName))
                    WriteDimension(outCube.Schema.YDimensionName, outCube.YDimensionValues.Count);
                if (!String.IsNullOrEmpty(outCube.Schema.ZDimensionName))
                    WriteDimension(outCube.Schema.ZDimensionName, outCube.ZDimensionValues.Count);

                #region Ignoring Global Attributes
                //Write globle attribute header with 0 number of global attribute to ignore the global attributes
                WriteAttributeHeader(0);
                #endregion

                #region Writing variables which are defined in HyperCube

                #region Write variable header and metadata
                // Write variable header with number of variables present in the HyperCube
                // The number of varaibles is the number of dimension variables and the 
                // column count in the schema
                WriteVariableHeader(outCube.Schema.ColumnCount + dimCount);

                // First write the dimension varaibles mapped to the HyperCube axes
                WriteDimensionVariables(outCube);

                //First write metadata for all variables
                for (int k = 0; k < outCube.Schema.ColumnCount; ++k)
                {
                    string varName = outCube.Schema.GetColumnName(k);
                    WriteVariable(varName, k, outCube);
                }

                #endregion

                #region Write Non Record Dimension Variables

                if ((null != outCube.XDimensionValues) && (outCube.XDimensionValues.Count > 0))
                    WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.XDimensionType),
                        offsetDic[outCube.Schema.XDimensionName], outCube.Schema.XDimensionName,
                        outCube.XDimensionValues.Count * GetByteSize(outCube.Schema.XDimensionType),
                        outCube.XDimensionValues.ToArray());

                if ((null != outCube.YDimensionValues) && (outCube.YDimensionValues.Count > 0))
                    WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.YDimensionType),
                        offsetDic[outCube.Schema.YDimensionName], outCube.Schema.YDimensionName,
                        outCube.YDimensionValues.Count * GetByteSize(outCube.Schema.YDimensionType),
                        outCube.YDimensionValues.ToArray());

                if ((null != outCube.ZDimensionValues) && (outCube.ZDimensionValues.Count > 0))
                    WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.ZDimensionType),
                        offsetDic[outCube.Schema.ZDimensionName], outCube.Schema.ZDimensionName,
                        outCube.ZDimensionValues.Count * GetByteSize(outCube.Schema.ZDimensionType),
                        outCube.ZDimensionValues.ToArray());

                #endregion

                #region Write variable values

                //Write data for NON RECORD variables
                for (int k = 0; k < outCube.Schema.ColumnCount; ++k)
                {
                    string varName = outCube.Schema.GetColumnName(k);
                    // Check if the varaible is a non record varaible i.e. it does
                    // not vary accross t dimension
                    if (!outCube.Schema.HasDimension(k, HyperCube.Axis.T))
                    {
                        int variableOffset = offsetDic[varName];
                        //Get the variable values from hypercube in scalar format then write into file
                        object[] variableValue = ReadCubeNonRecordData(outCube, varName);
                        //Get the size in multiple of 4 bytes to accomodate the number of values of the variable
                        int recSize = CalculateRecordSize(k, outCube);
                        //Write all the values.
                        WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.GetColumnType(k)), 
                            variableOffset, varName, recSize, variableValue);
                    }
                }

                //Write Data for Record Variables
                for (int r = 0; r < outCube.GetAxisLength(HyperCube.Axis.T); r++)
                {
                    // First write one value for the dimension variable mapped to the
                    // T axis
                    if (outCube.TDimensionValues.Count > 0)
                    {
                        List<object> tDimTemp = new List<object>();
                        tDimTemp.Insert(0, outCube.TDimensionValues[r]);

                        WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.TDimensionType),
                            (r == 0)? offsetDic[outCube.Schema.TDimensionName] : -1, outCube.Schema.TDimensionName,
                            GetByteSize(outCube.Schema.TDimensionType), tDimTemp.ToArray());
                    }

                    for (int k = 0; k < outCube.Schema.ColumnCount; ++k)
                    {
                        string varName = outCube.Schema.GetColumnName(k);
                        if (outCube.Schema.HasDimension(k, HyperCube.Axis.T))
                        {
                            int variableOffset = offsetDic[varName];
                            if (r > 0)
                            {
                                variableOffset = -1;
                                // means no need to update the offset for next record of the same variable
                                // This is handled in WriteVariableValues() function called below
                            }

                            //Get the variable values from hypercube in scalar format then write into file
                            object[] variableValue = ReadCubeRecordData(outCube, varName, k, r);
                            //Get the size in multiple of 4 bytes to accomodate the number of values of the variable
                            int recSize = CalculateRecordSize(k, outCube);
                            //Write all the values.
                            WriteVariableValues(TypeMapper.GetNcFileType(outCube.Schema.GetColumnType(k)), 
                                variableOffset, varName, recSize, variableValue);
                        }
                    }
                }

                #endregion

                #endregion

                //Close the file stream and binary writer once writing is done
                NetCDFWriterEx.Close();
            } // end of using
        }
        #endregion

        #region Private Methods

        private static int GetByteSize(Type typ)
        {
            int byteSize = 1;
            
            if ((typ.ToString() == "System.Byte") || (typ.ToString() == "System.Single") ||
            (typ.ToString() == "System.Int32") || (typ.ToString() == "System.Int16"))
                byteSize = 4;
            else if ((typ.ToString() == "System.Double"))
                byteSize = 8;
            
            return byteSize;
        }

        /// <summary>
        /// Provides the actual record size padded to multiple of 4
        /// </summary>
        /// <param name="variableValue"></param>
        /// <returns></returns>
        private static int CalculateRecordSize(int colIndex, HyperCube cube)
        {
            int recordSize = 1;
            int byteSize = GetByteSize(cube.Schema.GetColumnType(colIndex));

            if (cube.Schema.HasDimension(colIndex, HyperCube.Axis.X))
                recordSize *= cube.XDimensionValues.Count;
            if (cube.Schema.HasDimension(colIndex, HyperCube.Axis.Y))
                recordSize *= cube.YDimensionValues.Count;
            if (cube.Schema.HasDimension(colIndex, HyperCube.Axis.Z))
                recordSize *= cube.ZDimensionValues.Count;

            return (recordSize*byteSize);
        }

        /// <summary>
        /// Convert the HyperCube data for the selected Non record variable in to scalar form to provide
        /// it to write in the file.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private static object[] ReadCubeNonRecordData(HyperCube outcube, string varName)
        {
            int colCount = dataCube.Schema.ColumnCount;
            //object[] data = new object[variable.NumValues];
            object[] data = null;
            int colindex = -1;

            try
            {
                colindex = outcube.Schema.LookupColumnIndex(varName);
            }
            catch //Eat Exception
            {
                return null;
            }
            try
            {
                int counter = 0;

                int xLen = outcube.Schema.HasDimension(colindex, HyperCube.Axis.X) ?
                    outcube.XDimensionValues.Count : 1;
                int yLen = outcube.Schema.HasDimension(colindex, HyperCube.Axis.Y) ?
                    outcube.YDimensionValues.Count : 1;
                int zLen = outcube.Schema.HasDimension(colindex, HyperCube.Axis.Z) ?
                    outcube.ZDimensionValues.Count : 1;
                int tLen = outcube.Schema.HasDimension(colindex, HyperCube.Axis.T) ?
                    outcube.TDimensionValues.Count : 1; 


                data = new object[xLen * yLen * zLen * tLen];

                for (int i = 0; i < xLen; i++)
                {
                    for (int j = 0; j < yLen; j++)
                    {
                        for (int k = 0; k < zLen; k++)
                        {
                            for (int m = 0; m < tLen; m++)
                            {
                                object val = (object)outcube[i, j, k, m][colindex];
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

        /// <summary>
        /// Convert the HyperCube data for the selected record variable in to scalar form to provide
        /// it to write in the file.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="ncFile"></param>
        /// <param name="recordCounter"></param>
        /// <returns></returns>
        private static object[] ReadCubeRecordData(HyperCube outcube, string varName, int colIndex, int recordCounter)
        {
            object[] data = null;

            try
            {
                int r = recordCounter;
                int counter = 0;

                int xLen = outcube.Schema.HasDimension(colIndex, HyperCube.Axis.X) ?
                    outcube.XDimensionValues.Count : 1;
                int yLen = outcube.Schema.HasDimension(colIndex, HyperCube.Axis.Y) ?
                    outcube.YDimensionValues.Count : 1;
                int zLen = outcube.Schema.HasDimension(colIndex, HyperCube.Axis.Z) ?
                    outcube.ZDimensionValues.Count : 1;
                
                data = new object[xLen * yLen * zLen];

                //Ignore X axis here 
                for (int j = 0; j < xLen; j++)
                {
                    for (int k = 0; k < yLen; k++)
                    {
                        for (int m = 0; m < zLen; m++)
                        {
                            object val = (object)dataCube[j, k, m, r][colIndex];
                            if (val != null)
                            {
                                data[counter] = val;
                                counter++;
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

        /// <summary>
        /// Writes the dimension tag (NC_DIMENSION) and number of dimensions 
        /// </summary>
        /// <param name="dimensionCount"></param>
        private static void WriteDimensionHeader(int dimensionCount)
        {
            if (dimensionCount < 0)
                throw new ArgumentException("Dimensions cannot be less than zero");

            if (dimensionCount.Equals(0))
            {
                //this means the Dimesions are absent in the file
                WriteInteger(0);
            }
            else
            {
                //Dimensions are present in the file
                WriteInteger((int)NetCDFMetadataType.NcDimension);
            }

            //Write number of dimension even if it is zero.
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
        /// Writes attributes tag (NC_ATTRIBUTE) and number of Attributes. This function
        /// can be used wot writer header for both global and varible attributes
        /// </summary>
        /// <param name="writer"></param>
        private static void WriteAttributeHeader(int count)
        {
            if (count.Equals(0))
            {
                // Attribute is absent                
                WriteInteger(0);
            }
            else
            {
                // Attribute is present
                WriteInteger((int)NetCDFMetadataType.NcAttribute);
            }
            //Number of attributes
            WriteInteger(count);
        }

        /// <summary>
        /// Writes attribute name and values
        /// </summary>
        /// <param name="attribType"></param>
        /// <param name="attribName"></param>
        /// <param name="attribVal"></param>

        public static void WriteAttribute(NcType dataType, string attribName, object[] attVal)
        {
            //Number of Characters in name 
            WriteInteger(attribName.Length);
            //write attribute name
            WriteString(attribName);
            //Write data type tag for the attribute
            WriteInteger(GetTypeValue(dataType));
            //Write variable / attribute values as per the data type
            for (int i = 0; i < attVal.Length; i++)
            {
                WriteAttValue(dataType, attVal[i]);
            }

        }

        /// <summary>
        /// Writes variable tag (NC_VARIABLE) and number of variables
        /// </summary>
        /// <param name="count"></param>
        private static void WriteVariableHeader(int count)
        {
            if (0 == count)
                //Varibale is absent file
                WriteInteger(0);
            else
                //Varable is present in the file
                WriteInteger((int)NetCDFMetadataType.NcVariable);

            //Number of variables
            WriteInteger(count);
        }

        /// <summary>
        /// This method writes the dimension variables to the nc file
        /// </summary>
        private static void WriteDimensionVariables(HyperCube cube)
        {
            // To write the dimension indexes just check if the variable varies across the 
            // dimensions Y, Y, Z and T in that order. This is bcoz the dimensions were written
            // in that order.
            int dimCounter = 0;
            int tIndexVal = !string.IsNullOrEmpty(cube.Schema.TDimensionName) ? dimCounter++ : 0;
            int xIndexVal = !string.IsNullOrEmpty(cube.Schema.XDimensionName) ? dimCounter++ : 0;
            int yIndexVal = !string.IsNullOrEmpty(cube.Schema.YDimensionName) ? dimCounter++ : 0;
            int zIndexVal = !string.IsNullOrEmpty(cube.Schema.ZDimensionName) ? dimCounter++ : 0;

            if (!string.IsNullOrEmpty(cube.Schema.TDimensionName))
            {
                WriteVariable1(cube.Schema.TDimensionName, tIndexVal,
                    GetTypeValue(TypeMapper.GetNcFileType(cube.Schema.TDimensionType)));
            }
            if (!string.IsNullOrEmpty(cube.Schema.XDimensionName))
            {
                WriteVariable1(cube.Schema.XDimensionName, xIndexVal,
                    GetTypeValue(TypeMapper.GetNcFileType(cube.Schema.XDimensionType)));
            }
            if (!string.IsNullOrEmpty(cube.Schema.YDimensionName))
            {
                WriteVariable1(cube.Schema.YDimensionName, yIndexVal,
                    GetTypeValue(TypeMapper.GetNcFileType(cube.Schema.YDimensionType)));
            }
            if (!string.IsNullOrEmpty(cube.Schema.ZDimensionName))
            {
                WriteVariable1(cube.Schema.ZDimensionName, zIndexVal,
                    GetTypeValue(TypeMapper.GetNcFileType(cube.Schema.ZDimensionType)));
            }
        }

        /// <summary>
        /// This method writes the dimension variable. Later the method WriteVariable
        /// will be modified to have just one method.
        /// </summary>
        private static void WriteVariable1(string varName, int dimIndex, int typeVal)
        {
            //Number of Characters in variable's name
            WriteInteger(varName.Length);

            //write variable name
            WriteString(varName);

            // Write dimension count of the variable, always 1 
            // as these are dimension variables.
            WriteInteger(1);

            // Write the dimension index
            WriteInteger(dimIndex);

            WriteAttributeHeader(0);

            //write variable type
            WriteInteger(typeVal);

            //this is the position at which the nect four bytes contains
            // the bytes used by the variable and the offset value. This needs
            //to be updated everytime the new records is added to the file
            int variablePosition = (int)writer.BaseStream.Length;

            //Store this position in a structure for possible future reference
            offsetDic.Add(varName, variablePosition);

            //write next 8 bytes with zero values of variable size and variable offset values
            //DONT REMOVE THIS TWO LINES.
            WriteInteger(0);  //size
            WriteInteger(0);  //offset value in file for this variable
        }

        /// <summary>
        /// Writes the variable infomration without its values. 
        /// </summary>
        private static void WriteVariable(string varName, int colIndex, HyperCube outcube)
        {
            //Number of Characters in variable's name
            WriteInteger(outcube.Schema.GetColumnName(colIndex).Length);

            //write variable name
            WriteString(outcube.Schema.GetColumnName(colIndex));

            //Write dimension count of the variable
            WriteInteger(outcube.Schema.GetDimensionCount(colIndex));

            // To write the dimension indexes just check if the variable varies across the 
            // dimensions Y, Y, Z and T in that order. This is bcoz the dimensions were written
            // in that order.
            int dimCounter = 0;
            int tIndexVal = !string.IsNullOrEmpty(outcube.Schema.TDimensionName) ? dimCounter++ : 0;
            int xIndexVal = !string.IsNullOrEmpty(outcube.Schema.XDimensionName) ? dimCounter++ : 0;
            int yIndexVal = !string.IsNullOrEmpty(outcube.Schema.YDimensionName) ? dimCounter++ : 0;
            int zIndexVal = !string.IsNullOrEmpty(outcube.Schema.ZDimensionName) ? dimCounter++ : 0;


            if (outcube.Schema.HasDimension(colIndex, HyperCube.Axis.T))
                WriteInteger(tIndexVal);
            if (outcube.Schema.HasDimension(colIndex, HyperCube.Axis.X))
                WriteInteger(xIndexVal);
            if (outcube.Schema.HasDimension(colIndex, HyperCube.Axis.Y))
                WriteInteger(yIndexVal);
            if (outcube.Schema.HasDimension(colIndex, HyperCube.Axis.Z))
                WriteInteger(zIndexVal);
            
            #region IGNORING Variable Attribute Info
            WriteAttributeHeader(0);
            #endregion

            //write variable type
            WriteInteger(GetTypeValue(TypeMapper.GetNcFileType(outcube.Schema.GetColumnType(colIndex))));

            //this is the position at which the nect four bytes contains
            // the bytes used by the variable and the offset value. This needs
            //to be updated everytime the new records is added to the file
            int variablePosition = (int)writer.BaseStream.Length;

            //Store this position in a structure for possible future reference
            offsetDic.Add(varName, variablePosition);

            //write next 8 bytes with zero values of variable size and variable offset values
            //DONT REMOVE THIS TWO LINES.
            WriteInteger(0);  //size
            WriteInteger(0);  //offset value in file for this variable

        }

        /// <summary>
        /// Converts the NcType to Net CDF data type values. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int GetTypeValue(NcType type)
        {
            //This function can be removed later.

            switch (type)
            {
                case NcType.NcByte: return 1;
                case NcType.NcChar: return 2;
                case NcType.NcShort: return 3;
                case NcType.NcInt: return 4;
                case NcType.NcFloat: return 5;
                case NcType.NcDouble: return 6;
            }
            return 0;
        }

        /// <summary>
        /// Write variable values for each variable name and type provided. If updates the offset value for
        /// the first record of the variable.
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="offset"></param>
        /// <param name="variableName"></param>
        /// <param name="recordSize"></param>
        /// <param name="values"></param>
        private static void WriteVariableValues(NcType dataType, int offset, string variableName, int recordSize, object[] values)
        {
            //First overwrite the size bytes and offset values for the variable
            //HERE THE SIZE AND OFFSET VALUES ARE OVERWRITTEN.
            if (offset != -1)
            {
                writer.Seek(offset, SeekOrigin.Begin);
                //Size
                WriteInteger(recordSize);
                //WriteInteger(GetByteSize(recordSize));

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
                        
            #region PATCH: Apply padding for short and bytes values
            
            int paddingLen = 0;
            switch (dataType)
            {
                case NcType.NcShort:
                    // 2 because each short value occupies 2 bytes
                    if (values.Length < 2)
                    {
                        paddingLen = 2 - values.Length;
                    }
                    else
                    {
                        paddingLen = 2 - values.Length % 2;
                    }
                    //Pad minimum values for short if required
                    for (int i = 0; i < paddingLen; i++)
                    {
                        WriteShort(short.MinValue);
                    }
                    break;

                case NcType.NcByte:
                    
                    if (values.Length < 4)
                    {
                        paddingLen = 4 - values.Length;
                    }
                    else
                    {
                        paddingLen = 4 - values.Length % 4;
                    }
                    //Pad minimum values for short if required
                    for (int i = 0; i < paddingLen; i++)
                    {
                        writer.Write(short.MinValue);
                    }
                    break;
            }

            #endregion
            
        }


        #region Write Attribute values and Write Variable values code 

        /// <summary>
        /// Write attribute value. 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        private static void WriteAttValue(NcType dataType, object value)
        {
            //This function is NOT USED AT PRESENT as attribute writing are ingored
            
            switch (dataType)
            {
                case NcType.NcByte:
                    if (null != value)
                    {
                        byte byteValue = (byte)value;
                        writer.Write(byteValue);
                    }
                    else
                    {
                        writer.Write(byte.MinValue);
                    }
                    break;
                case NcType.NcChar:
                    string[] strValue = (string[])value;
                    for (int i = 0; i < strValue.Length; i++)
                    {
                        WriteInteger(strValue[i].Length);
                        WriteString(strValue[i]);
                    }
                    break;
                case NcType.NcShort:
                    short[] shA = (short[])value;
                    WriteInteger(shA.Length);
                    for (int i = 0; i < shA.Length; i++)
                        WriteShort(shA[i]);
                    break;
                case NcType.NcInt:
                    int[] intA = (int[])value;
                    WriteInteger(intA.Length);
                    for (int i = 0; i < intA.Length; i++)
                        WriteInteger(intA[i]);
                    break;
                case NcType.NcFloat:
                    float[] flA = (float[])value;
                    WriteInteger(flA.Length);
                    for (int i = 0; i < flA.Length; i++)
                        WriteFloat(flA[i]);
                    break;
                case NcType.NcDouble:
                    double[] dbA = (double[])value;
                    WriteInteger(dbA.Length);
                    for (int i = 0; i < dbA.Length; i++)
                        WriteDouble(dbA[i]);
                    break;

            }
        }

        /// <summary>
        /// Writes the variable values. For numm values it writes the minimum values of its datatype
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="value"></param>
        private static void WriteVarValue(NcType dataType, object value)
        {
            switch (dataType)
            {
                case NcType.NcByte:
                    if (null != value)
                    {
                        byte byteValue = (byte)value;
                        writer.Write(byteValue);
                    }
                    else
                    {
                        writer.Write(byte.MinValue);
                    }
                    break;
                case NcType.NcChar:
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
                case NcType.NcShort:
                    if (null != value)
                    {
                        short shA = (short)value;
                        WriteShort(shA);
                    }
                    else
                    {
                        WriteShort(short.MinValue);
                    }
                    break;
                case NcType.NcInt:
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
                case NcType.NcFloat:
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
                case NcType.NcDouble:
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

        #endregion


        #region Binary Writer Methods

        /// <summary>
        /// Write short values
        /// </summary>
        /// <param name="value"></param>
        private static void WriteShort(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        /// <summary>
        /// Writes all the short values and pad minimum short values
        /// if required to make the length multiple of 4 bytes
        /// </summary>
        /// <param name="values"></param>
        private static void WriteShort(short[] values)
        {
            int paddingLen = 0; // values.Length;

            // 2 because each short value occupies 2 bytes
            if (values.Length < 2)
            {
                paddingLen = 2 - values.Length;
            }
            else
            {
                paddingLen = 2 - values.Length % 2;
            }
            //Now write each element one by one
            for (int i = 0; i < values.Length; i++)
            {
                WriteShort(values[i]);
            }
            //Pad minimum values for short if required
            for (int i = 0; i < paddingLen; i++)
            {
                WriteShort(short.MinValue);
            }
        }

        /// <summary>
        /// Write float value
        /// </summary>
        /// <param name="value"></param>
        private static void WriteFloat(float value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[3]);
            writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        /// <summary>
        /// Write integer values
        /// </summary>
        /// <param name="value"></param>
        private static void WriteInteger(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            writer.Write(arr[3]);
            writer.Write(arr[2]);
            writer.Write(arr[1]);
            writer.Write(arr[0]);
        }

        /// <summary>
        /// Write double values
        /// </summary>
        /// <param name="value"></param>
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
