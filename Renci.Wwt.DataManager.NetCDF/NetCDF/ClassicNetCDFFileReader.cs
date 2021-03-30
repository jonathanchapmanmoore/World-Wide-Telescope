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
using System.Linq;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    /// <summary>
    /// This class provides an interface to read a NetCDF file in 
    /// the Classic Format. 
    /// </summary>
    public class ClassicNetCDFFileReader : NetCDFReader
    {
        private int _recordSize;
        private BigEndianBinaryReader _reader;

        /// <summary>
        /// Creates an instance of the class by reading all the 
        /// dimensions, attributes and variable metadata from
        /// the NetCDF file. 
        /// </summary>
        /// <param name="reader">
        /// A binary reader object which has already opened the NetCDF file
        /// and read the magic number "CDF\001". 
        /// <remarks>
        /// The reader's pointer should be positioned at the 4th byte in the 
        /// file when this constructor is called. 
        /// </remarks>
        /// </param>
        /// <exception cref="Microsoft.NetCDF.CSharpAPI.ParserException"></exception>
        internal ClassicNetCDFFileReader(BigEndianBinaryReader reader)
        {
            this._reader = reader;

            // Read 4 bytes to get the number of records present in the 
            // NetCDF file. 
            this.numberOfRecords = this._reader.ReadUInt32();
            this.numberOfDimensions = this.ReadDimensions();
            this.numberOfGlobalAttributes = this.ReadAttributes(this.globalAttributes);
            this.numberOfVariables = this.ReadVariables();
        }

        public override IEnumerable<decimal> ReadDecimalVariable(string name)
        {
            IEnumerable<decimal> values = null;
            INetCDFVariable var = this.Variables[name];
            int offset = var.DataOffset;
            uint dimension_product = GetDimensionProduct(var.DimensionIDs);
            _reader.BaseStream.Position = offset;
            uint length = var.Size;

            switch (var.DataType)
            {
                case NetCDFDataType.NcFloat:
                    //if (typeof(T) == typeof(float))
                    {
                        values = _reader.ReadSingles(length / sizeof(float)).Select((v) => new decimal(v));
                    }
                    break;
                case NetCDFDataType.NcDouble:
                    //if (typeof(T) == typeof(double))
                    {

                        values = _reader.ReadDoubles(length / sizeof(double)).Select((v) => new decimal(v));
                    }
                    break;
                //case NetCDFDataType.NcChar:
                //    if (typeof(T) == typeof(char))
                //    {
                //        values = _reader.ReadChars((int)length / sizeof(char)) as T[];
                //    }
                //    break;
                case NetCDFDataType.NcByte:
                    //if (typeof(T) == typeof(byte))
                    {
                        if (length == dimension_product * sizeof(byte))
                        {
                            values = _reader.ReadBytes(length / sizeof(byte), false).Select((v) => new decimal(v));
                        }
                        else
                        {
                            values = _reader.ReadBytes(dimension_product, true).Select((v) => new decimal(v));
                        }
                    }
                    break;
                case NetCDFDataType.NcInt:
                    //if (typeof(T) == typeof(int))
                    {
                        values = _reader.ReadIntegers(length / sizeof(int)).Select((v) => new decimal(v));
                    }
                    break;
                case NetCDFDataType.NcShort:
                    //if (typeof(T) == typeof(short))
                    {
                        if (length == dimension_product * sizeof(byte))
                        {
                            //No Padding
                            values = _reader.ReadShorts(length / sizeof(short), false).Select((v) => new decimal(v));
                        }
                        else
                        {
                            //Apply Padding
                            values = _reader.ReadShorts(dimension_product, true).Select((v) => new decimal(v));
                        }
                    }
                    break;
            }

            return values;
        }


        /// <summary>
        /// Fetches the data values for a non-record variable. 
        /// </summary>
        /// <typeparam name="T">Any of float, int, double, char, byte or short </typeparam>
        /// <param name="name">Name of a non-record variable</param>
        /// <returns>Array of data values of the specified type</returns>
        public override T[] ReadVariable<T>(string name)
        {
            T[] values = default(T[]);
            INetCDFVariable var = this.Variables[name];
            int offset = var.DataOffset;
            uint dimension_product = GetDimensionProduct(var.DimensionIDs);
            _reader.BaseStream.Position = offset;
            uint length = var.Size;

            switch (var.DataType)
            {
                case NetCDFDataType.NcFloat:
                    if (typeof(T) == typeof(float))
                    {
                        values = _reader.ReadSingles(
                            length / sizeof(float)) as T[];
                    }
                    break;
                case NetCDFDataType.NcDouble:
                    if (typeof(T) == typeof(double))
                    {

                        values = _reader.ReadDoubles(
                            length / sizeof(double)) as T[];
                    }
                    break;
                case NetCDFDataType.NcChar:
                    if (typeof(T) == typeof(char))
                    {
                        values = _reader.ReadChars((int)length / sizeof(char)) as T[];
                    }
                    break;
                case NetCDFDataType.NcByte:
                    if (typeof(T) == typeof(byte))
                    {
                        if (length == dimension_product * sizeof(byte))
                        {
                            values = _reader.ReadBytes(length / sizeof(byte), false) as T[];
                        }
                        else
                        {
                            values = _reader.ReadBytes(dimension_product, true) as T[];
                        }
                    }
                    break;
                case NetCDFDataType.NcInt:
                    if (typeof(T) == typeof(int))
                    {
                        values = _reader.ReadIntegers(
                            length / sizeof(int)) as T[];
                    }
                    break;
                case NetCDFDataType.NcShort:
                    if (typeof(T) == typeof(short))
                    {
                        if (length == dimension_product * sizeof(byte))
                        {
                            //No Padding
                            values = _reader.ReadShorts(length / sizeof(short), false) as T[];
                        }
                        else
                        {
                            //Apply Padding
                            values = _reader.ReadShorts(dimension_product, true) as T[];
                        }
                    }
                    break;
            }
            return values;
        }

        /// <summary>
        /// Fetches the data values for a record variable. 
        /// </summary>
        /// <typeparam name="T">Any of float, int, double, char, byte or short</typeparam>
        /// <param name="name">Name of a non-record variable</param>
        /// <param name="recordIndex">Index into the Record data</param>
        /// <returns>Array of data values of the specified type</returns>
        public override T[] ReadVariable<T>(string name, uint recordIndex)
        {
            T[] values = default(T[]);
            INetCDFVariable var = this.Variables[name];
            int offset = var.DataOffset;
            uint dimension_product = GetDimensionProduct(var.DimensionIDs);
            _reader.BaseStream.Position = var.DataOffset + this._recordSize * recordIndex;

            switch (var.DataType)
            {
                case NetCDFDataType.NcFloat:
                    if (typeof(T) == typeof(float))
                    {
                        values = _reader.ReadSingles(dimension_product) as T[];
                    }
                    break;
                case NetCDFDataType.NcDouble:
                    if (typeof(T) == typeof(double))
                    {

                        values = _reader.ReadDoubles(dimension_product) as T[];
                    }
                    break;
                case NetCDFDataType.NcChar:
                    if (typeof(T) == typeof(char))
                    {
                        values = _reader.ReadChars((int)dimension_product) as T[];
                    }
                    break;
                case NetCDFDataType.NcByte:
                    if (typeof(T) == typeof(byte))
                    {
                        if (((dimension_product * sizeof(byte)) % 4) == 0)
                        {
                            values = _reader.ReadBytes(dimension_product, false) as T[];
                        }
                        else
                        {
                            values = _reader.ReadBytes(dimension_product, true) as T[];
                        }
                    }
                    break;
                case NetCDFDataType.NcInt:
                    if (typeof(T) == typeof(int))
                    {
                        values = _reader.ReadIntegers(dimension_product) as T[];
                    }
                    break;
                case NetCDFDataType.NcShort:
                    if (typeof(T) == typeof(short))
                    {
                        if (((dimension_product * sizeof(short)) % 4) == 0)
                        {
                            values = _reader.ReadShorts(dimension_product, false) as T[];
                        }
                        else
                        {
                            values = _reader.ReadShorts(dimension_product, true) as T[];
                        }
                    }
                    break;
            }
            return values;
        }

        /// <summary>
        /// Disposes the stream associated with the reader object. 
        /// </summary>
        public override void Dispose()
        {
            if (this._reader != null)
            {
                this._reader.BaseStream.Dispose();
            }
        }

        /// <summary>
        /// Reads metadata of the variables defined in the NetCDF file. 
        /// </summary>
        /// <exception cref="Microsoft.NetCDF.CSharpAPI.ParserException"></exception>
        /// <returns>Count of variables</returns>
        private uint ReadVariables()
        {
            int metadata_type_flag = _reader.ReadInt32();
            uint variables_count = 0;
            if ((NetCDFMetadataType)metadata_type_flag == NetCDFMetadataType.NcVariable)
            {
                variables_count = _reader.ReadUInt32();

                for (uint i = 0; i < variables_count; i++)
                {
                    uint name_length = _reader.ReadUInt32();
                    string name = _reader.ReadString(name_length, true);
                    uint dimensionality = _reader.ReadUInt32();
                    uint[] dimension_ids = new uint[dimensionality];

                    for (uint j = 0; j < dimensionality; j++)
                    {
                        dimension_ids[j] = _reader.ReadUInt32();
                    }

                    bool is_record_variable = this.Dimensions[dimension_ids[0]].IsRecordDimension;
                    Dictionary<string, INetCDFAttribute> variable_attributes =
                        new Dictionary<string, INetCDFAttribute>();
                    this.ReadAttributes(variable_attributes);
                    NetCDFDataType data_type = (NetCDFDataType)_reader.ReadInt32();
                    uint variable_size = _reader.ReadUInt32();
                    _recordSize += is_record_variable ? (int)variable_size : 0;
                    int data_offset = _reader.ReadInt32();

                    switch (data_type)
                    {
                        case NetCDFDataType.NcByte:
                            NetCDFVariable<byte> var_bytes =
                                new NetCDFVariable<byte>(name,
                                    dimension_ids, variable_attributes,
                                    data_type, variable_size, data_offset,
                                    is_record_variable);
                            this.variables.Add(name, var_bytes);
                            break;

                        case NetCDFDataType.NcChar:
                            NetCDFVariable<char> var_chars =
                                new NetCDFVariable<char>(name,
                                    dimension_ids, variable_attributes,
                                    data_type, variable_size, data_offset,
                                    is_record_variable);
                            this.variables.Add(name, var_chars);
                            break;

                        case NetCDFDataType.NcShort:
                            NetCDFVariable<short> var_shorts =
                                new NetCDFVariable<short>(name,
                                    dimension_ids, variable_attributes,
                                    data_type, variable_size, data_offset,
                                    is_record_variable);
                            this.variables.Add(name, var_shorts);
                            break;

                        case NetCDFDataType.NcInt:
                            NetCDFVariable<int> var_ints =
                                new NetCDFVariable<int>(name,
                                    dimension_ids, variable_attributes,
                                    data_type, variable_size, data_offset,
                                    is_record_variable);
                            this.variables.Add(name, var_ints);
                            break;

                        case NetCDFDataType.NcFloat:
                            NetCDFVariable<float> var_floats =
                                 new NetCDFVariable<float>(name,
                                     dimension_ids, variable_attributes,
                                     data_type, variable_size, data_offset,
                                     is_record_variable);
                            this.variables.Add(name, var_floats);
                            break;

                        case NetCDFDataType.NcDouble:
                            NetCDFVariable<double> var_doubles =
                                 new NetCDFVariable<double>(name,
                                     dimension_ids, variable_attributes,
                                     data_type, variable_size, data_offset,
                                     is_record_variable);
                            this.variables.Add(name, var_doubles);
                            break;
                    }
                }
            }
            else if (metadata_type_flag == 0)
            {
                // read the next 4 bytes to position the pointer at the start
                // of the data section. 
                variables_count = (uint)_reader.ReadInt32();

                // If this condition is met, raise a parser exception.  
                if (variables_count != 0)
                {
                    string message =
                        String.Format("Expected ZERO value for variables count. But actual value is {0}",
                        variables_count);
                    throw new ParserException(message);
                }
            }
            // If we reach here, raise a parser exception. 
            else
            {
                string message = "Variable metadata type token expected, but not found.";
                throw new ParserException(message);
            }
            return variables_count;
        }

        /// <summary>
        /// Computes the product of all dimensions.
        /// </summary>
        /// <param name="dimensionIDs">Array of values indicating the size of each dimension.
        /// </param>
        /// <returns>Product of all dimensions</returns>
        private uint GetDimensionProduct(uint[] dimensionIDs)
        {
            uint product = 1;
            foreach (uint dimensionID in dimensionIDs)
            {
                product *= this.Dimensions[dimensionID].Length != 0 ? this.Dimensions[dimensionID].Length : 1;
            }
            return product;
        }

        /// <summary>
        /// Reads the attributes array from NetCDF file and places them in the 
        /// Dictionary. 
        /// </summary>
        /// <param name="attributes">Dictionary into which attributes are placed.</param>
        /// <exception cref="Microsoft.NetCDF.CSharpAPI.ParserException"></exception>
        /// <returns>Number of attributes read from the NetCDF file</returns>
        private uint ReadAttributes(Dictionary<string, INetCDFAttribute> attributes)
        {
            int metadata_type_flag = _reader.ReadInt32();
            uint attributes_count = 0;
            if ((NetCDFMetadataType)metadata_type_flag == NetCDFMetadataType.NcAttribute)
            {
                attributes_count = _reader.ReadUInt32();

                for (int i = 0; i < attributes_count; i++)
                {
                    uint name_length = _reader.ReadUInt32();
                    string name = _reader.ReadString(name_length, true);
                    NetCDFDataType data_type = (NetCDFDataType)_reader.ReadInt32();

                    uint size = _reader.ReadUInt32();
                    switch (data_type)
                    {
                        case NetCDFDataType.NcByte:
                            byte[] bytes = _reader.ReadBytes(size, true);
                            attributes.Add(name,
                                new NetCDFAttribute<byte>(name, bytes, data_type));
                            break;
                        case NetCDFDataType.NcChar:
                            string chars = _reader.ReadString(size, true);
                            string[] str = new string[1];
                            str[0] = chars;
                            attributes.Add(name,
                                new NetCDFAttribute<string>(name, str, data_type));
                            break;
                        case NetCDFDataType.NcShort:
                            short[] shorts = _reader.ReadShorts(size, true);
                            attributes.Add(name,
                                new NetCDFAttribute<short>(name, shorts, data_type));
                            break;
                        case NetCDFDataType.NcInt:
                            int[] integers = _reader.ReadIntegers(size);
                            attributes.Add(name,
                                new NetCDFAttribute<int>(name, integers, data_type));
                            break;
                        case NetCDFDataType.NcFloat:
                            float[] floats = _reader.ReadSingles(size);
                            attributes.Add(name,
                                new NetCDFAttribute<float>(name, floats, data_type));
                            break;
                        case NetCDFDataType.NcDouble:
                            double[] doubles = _reader.ReadDoubles(size);
                            attributes.Add(name,
                                new NetCDFAttribute<double>(name, doubles, data_type));
                            break;
                    }
                }
            }
            else if (metadata_type_flag == 0)
            {
                // read the next 4 bytes to position the pointer at the start
                // of the variables array. 
                attributes_count = (uint)_reader.ReadInt32();

                // If this condition is met, raise a parser exception.  
                if (attributes_count != 0)
                {
                    string message =
                        String.Format("Expected ZERO value for attribute count. But actual value is {0}",
                        attributes_count);
                    throw new ParserException(message);
                }
            }
            // If we reach here, raise a parser exception. 
            else
            {
                string message = "Attribute metadata type token expected, but not found.";
                throw new ParserException(message);
            }
            return attributes_count;
        }

        /// <summary>
        /// Reads Dimension information from the NetCDF file. 
        /// This function assumes that fileReader's pointer is positioned at 
        /// the 8th byte.
        /// If dimension data exists, it reads the data into a 
        /// Dictionary(uint, INetCDFDimension) object and sets the dimension 
        /// count.
        /// </summary>
        /// <exception cref="Microsoft.NetCDF.CSharpAPI.ParserException"></exception>
        /// <returns>Number of dimensions defined in the file</returns>
        private uint ReadDimensions()
        {
            // Read 4 bytes starting from the 8th position. 
            int metadata_type_flag = _reader.ReadInt32();
            uint dimensions_count = 0;

            // check if dimensions exist.
            if ((NetCDFMetadataType)metadata_type_flag == NetCDFMetadataType.NcDimension)
            {
                // Read next 4 bytes to get the count of dimensions.
                dimensions_count = _reader.ReadUInt32();

                // for each dimension get the name and its size
                for (uint i = 0; i < dimensions_count; i++)
                {
                    // get the length of the name. 
                    uint name_length = _reader.ReadUInt32();
                    string name = _reader.ReadString(name_length, true);
                    uint dimension_size = _reader.ReadUInt32();
                    dimensions.Add(i, new NetCDFDimension(name, dimension_size));
                }
            }
            // Check if dimensions have not been defined. 
            else if (metadata_type_flag == 0)
            {
                // read the next 4 bytes to position the pointer at the start
                // of the global attributes array. 
                dimensions_count = (uint)_reader.ReadInt32();

                // If this condition is met, raise a parser exception.  
                if (dimensions_count != 0)
                {
                    string message =
                        String.Format("Expected ZERO value for dimension count. But actual value is {0}",
                        dimensions_count);
                    throw new ParserException(message);
                }
            }
            // If we reach here, raise a parser exception. 
            else
            {
                string message = "Dimension metadata type token expected, but not found.";
                throw new ParserException(message);
            }
            return dimensions_count;
        }
    }
}
