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
    
    /// <summary>
    /// NcFile is the class for netCDF files, providing methods for
    /// netCDF file operations
    /// </summary>
    public class NcFile
    {

        #region Enumerations

        public enum FillMode
        {
            Fill,
            NoFill
        }

        public enum NcFileMode
        {
            /// <summary>
            /// Open an existing file for reading
            /// </summary>
            ReadOnly,
            /// <summary>
            /// Open an existing file for reading or writing
            /// </summary>
            Write,
            /// <summary>
            /// Create a new empty file even if the named file already exists
            /// </summary>
            Replace,
            /// <summary>
            /// Create a new file only if the named file does not already exist
            /// </summary>
            New
        }

        #endregion

        #region Constants (Tags)
        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of 8 bit signed integer
        /// </summary>
        const int NC_BYTE = 1;

        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of ISO/ASCII characters 
        /// </summary>
        const int NC_CHAR = 2;

        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of 16 bit signed integer
        /// </summary>
        const int NC_SHORT = 3;

        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of 32 bit signed integer
        /// </summary>
        const int NC_INT = 4;

        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of IEEE single precision floating point number
        /// </summary>
        const int NC_FLOAT = 5;

        /// <summary>
        /// 32-bit Integer value indicating that data coming after it is an array of IEEE double precision float
        /// </summary>
        const int NC_DOUBLE = 6;


        const int NC_DIMENSION = 10;
        const int NC_VARIABLE = 11;
        const int NC_ATTRIBUTE = 12;
        #endregion

        #region Member Variables

        private FillMode fillMode = FillMode.Fill;

        /// <summary>
        /// Format of file
        /// </summary>
        private NetCDFFileFormat fileFormat = NetCDFFileFormat.NetCDF4;

        /// <summary>
        /// File is isvalid or not
        /// </summary>
        private bool isValid;

        /// <summary>
        /// Number of Dimensions in nc file
        /// </summary>
        private int numDims = 0;

        /// <summary>
        /// Number of Global Attributes in nc file
        /// </summary>
        private int numAtts = 0;

        /// <summary>
        /// Number of Variables in nc file
        /// </summary>
        private int numVars = 0;

        /// <summary>
        /// Metadata of the nc file
        /// </summary>
        private NcMetaData metadata = null;

        /// <summary>
        /// Stream to read/write file
        /// </summary>
        private FileStream fs;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathName"></param>
        public NcFile(string pathName) : this(pathName, NcFileMode.ReadOnly) { }

        /// <summary>
        /// Opens the files from the specified path for reading 
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="mode"></param>
        public NcFile(string pathName, NcFileMode mode)
        {
            try
            {
                switch (mode)
                {
                    case NcFileMode.ReadOnly:
                        fs = new FileStream(pathName, FileMode.Open, FileAccess.Read);
                        break;
                }
                this.metadata = new NcMetaData();
                this.ReadMetadata();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ~NcFile() { }
        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the file is a valid netCDF file
        /// </summary>
        public bool IsValid
        {
            get { return this.isValid; }
        }

        /// <summary>
        /// Gets or sets the fill mode of the file
        /// </summary>
        public FillMode Fill
        {
            get
            {
                return this.fillMode;
            }
            set
            {
                this.fillMode = value;
            }
        }

        /// <summary>
        /// Gets the netCDF file format version
        /// </summary>
        public NetCDFFileFormat Format
        {
            get { return fileFormat; }
        }

        /// <summary>
        /// Gets the MetaData of netCDF file. It contains Dimensions, Variables and Global attributes.
        /// </summary>
        public NcMetaData MetaData
        {
            get
            {
                return this.metadata;
            }
        }

        /// <summary>
        /// Gets the number of dimensions in the netCDF file.
        /// </summary>
        public int NumDims
        {
            get
            {
                return this.numDims;
            }
        }

        /// <summary>
        /// Gets the number of variables in the netCDF file.
        /// </summary>
        public int NumVars
        {
            get
            {
                return numVars;
            }
        }

        /// <summary>
        /// Gets the number of global attributes in the netCDF file.
        /// </summary>
        public int NumAtts
        {
            get
            {
                return numAtts;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Close netCDF file earlier than it would be closed by the desctructor
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Synchronizes file to disk. This flushes buffers so that readers of
        /// the file will see recent changes.
        /// </summary>
        /// <returns></returns>
        public bool Sync() { return false; }

        /// <summary>
        /// Either just closes file (if recently it has been in data mode as the
        /// result of accessing data), or backs out of the most recent sequence
        /// of changes to the file schema (dimensions, variables, and attributes). 
        /// </summary>
        /// <returns></returns>
        public bool Abort() { return false; }

        /// <summary>
        /// Get a dimension by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NcDim GetDim(string name)
        {
            return this.metadata.GetDimension(name);
        }

        /// <summary>
        /// Get the nth dimension (beginning with the 0th).
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public NcDim GetDim(int n)
        {
            return this.metadata.Dimensions[n];

        }

        /// <summary>
        /// Get a variable by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NcVar GetVar(string name)
        {
            return this.metadata.GetVariable(name);
        }

        /// <summary>
        /// Get the nth variable (beginning with the 0th).
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public NcVar GetVar(int n)
        {
            return this.metadata.GetVariable(n);
        }

        /// <summary>
        /// Get an attribute by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public NcAtt GetAtt(string name)
        {
            return this.metadata.GetGlobalAttribute(name);
        }

        /// <summary>
        /// Get the nth global attribute (beginning with the 0th).
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public NcAtt GetAtt(int n)
        {
            return this.metadata.GetGlobalAttribute(n);
        }

        /// <summary>
        /// Get the unlimited dimension if any
        /// </summary>
        /// <returns></returns>
        public NcDim RecDim()
        {
            if (metadata != null)
            {
                foreach (NcDim dim in metadata.Dimensions)
                {
                    if (dim.IsUnlimited)
                        return dim;
                }
            }
            return null;
        }

        /// <summary>
        /// Add an unlimited dimension named dimname to the netCDF file.
        /// </summary>
        /// <param name="dimName"></param>
        /// <returns></returns>
        public NcDim AddDim(string dimName) { return null; }

        /// <summary>
        /// Add a dimension named dimname of size dimsize.
        /// </summary>
        /// <param name="dimName"></param>
        /// <param name="dimSize"></param>
        /// <returns></returns>
        public NcDim AddDim(string dimName, int dimSize)
        {
            return null;
        }

        /// <summary>
        /// Add a variable named varname of the specified type (ncByte, ncChar, ncShort,
        /// ncInt, ncFloat, ncDouble) to the open netCDF file. The variable is defined
        /// with a shape that depends on how many dimension arguments are provided. A
        /// scalar variable would have 0 dimensions, a vector would have 1 dimension,
        /// and so on. Supply as many dimensions as needed, up to 5. If more than 5
        /// dimensions are required, use the n-dimensional version of this member
        /// function instead. 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="type"></param>
        /// <param name="dims"></param>
        /// <returns></returns>
        public NcVar AddVar(string varName, NcType type, params NcDim[] dims) { return null; }

        /// <summary>
        /// Add a variable named varname of numDims dimensions and of the specified type.
        /// This method must be used when dealing with variables of more than 5 dimensions. 
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="type"></param>
        /// <param name="numDims"></param>
        /// <param name="dims"></param>
        /// <returns></returns>
        public NcVar AddVar(string varName, NcType type, int numDims, params NcDim[] dims) { return null; }

        public bool AddAtt(string name, byte val) { return false; }
        public bool AddAtt(string name, char val) { return false; }
        public bool AddAtt(string name, short val) { return false; }
        public bool AddAtt(string name, int val) { return false; }
        public bool AddAtt(string name, float val) { return false; }
        public bool AddAtt(string name, double val) { return false; }
        public bool AddAtt(string name, string val) { return false; }

        public bool AddAtt(string name, byte[] val) { return false; }
        public bool AddAtt(string name, char[] val) { return false; }
        public bool AddAtt(string name, short[] val) { return false; }
        public bool AddAtt(string name, int[] val) { return false; }
        public bool AddAtt(string name, float[] val) { return false; }
        public bool AddAtt(string name, double[] val) { return false; }
        #endregion

        #region Private Methods

        #region MetaData Reading
        /// <summary>
        /// Reads the file metadata such as Dimensions, Attributes and Variables
        /// </summary>
        private void ReadMetadata()
        {
            //First check valid file
            this.isValid = ReadMagicNumber();
            if (!isValid)
            {
                return;
            }

            //Now get the No of record variable. i.e. 
            //variable with "Unlimited" dimension. it will be either 0 or 1.
            string str = ReadBytes(0, 4);
            int numRecs = ReadInteger(str);

            //Now read no of dimensions
            this.numDims = ReadDimensionCount();
            //Now read all the dimensions 
            ReadDimensions(numDims);

            //Now Read the Global attributes count
            this.numAtts = ReadGlobalAttributeCount();
            ReadGlobalAttributes(this.numAtts);

            // Read variables
            this.numVars = ReadVariableCount();
            ReadVariables(this.numVars);
        }

        /// <summary>
        /// Reads the first four bytes of the file to validate it
        /// </summary>
        private bool ReadMagicNumber()
        {
            string str = ReadBytes(0, 4); // First 4 bytes should return "CDF1"
            if (str[0].Equals('C') && str[1].Equals('D')
                && str[2].Equals('F'))
            {
                if (str[3] == 1)
                {
                    fileFormat = NetCDFFileFormat.Classic;
                    return true;
                }
                else if (str[3] == 2)   //We are not considering this format right now
                {
                    fileFormat = NetCDFFileFormat.Offset64Bits;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reads the bytes and return its equivalent string
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        private string ReadBytes(int offset, int byteCount)
        {
            try
            {
                byte[] array = new byte[byteCount];
                fs.Read(array, offset, byteCount);

                StringBuilder data = new StringBuilder();
                foreach (byte da in array)
                {
                    data.Append(Convert.ToChar(da));
                }
                return data.ToString();
            }
            catch (Exception)
            {
                //Handle exception here
            }
            return string.Empty;
        }

        /// <summary>
        /// Reads the double value from the offset
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        private double ReadDouble(int offset)
        {
            try
            {
                int byteCount = 8; // read 8 bytes for double
                byte[] array = new byte[byteCount];
                fs.Read(array, offset, byteCount);

                // Reverse the array
                byte[] arrayRev = new byte[byteCount];
                for (int i = 0; i < array.Length; ++i)
                    arrayRev[(array.Length - 1) - i] = array[i];
                double val = BitConverter.ToDouble(arrayRev, 0);
                return val;
            }
            catch (Exception)
            {
                //handle exception here. 
            }
            return 0;
        }

        /// <summary>
        /// Reads the float values from the offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private float ReadFloat(int offset)
        {
            try
            {
                int byteCount = 4;  // 4 bytes for float 
                byte[] array = new byte[byteCount];
                fs.Read(array, offset, byteCount);

                // Reverse the array
                byte[] arrayRev = new byte[byteCount];
                for (int i = 0; i < array.Length; ++i)
                    arrayRev[(array.Length - 1) - i] = array[i];
                float val = BitConverter.ToSingle(arrayRev, 0);

                return val;
            }
            catch (Exception)
            {
                // handle exception here. 
            }
            return 0;
        }

        /// <summary>
        /// Reads the integer from the offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int ReadInteger(int offset)
        {
            try
            {
                int byteCount = 4;  // 4 bytes for int32
                byte[] array = new byte[byteCount];
                fs.Read(array, offset, byteCount);

                // Reverse the array
                byte[] arrayRev = new byte[byteCount];
                for (int i = 0; i < array.Length; ++i)
                    arrayRev[(array.Length - 1) - i] = array[i];
                int val = BitConverter.ToInt32(arrayRev, 0);

                return val;
            }
            catch (Exception)
            {
                // handle exception here. 
            }
            return 0;
        }


        /// <summary>
        /// Converts the bytes read into hex and then convert it into integer.
        /// This can convert value for string lenght upto 4 only
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int ReadInteger(string str)
        {
            if (str.Length != 4)
            {
                //System.Console.WriteLine("Error");
                //validate for only 4 bytes read
                return 0;
            }
            string numberString = "";
            string tempStr = "";

            for (int counter = 0; counter < str.Length; ++counter)
            {
                int a = str[counter];
                tempStr = String.Format("{0:x}", a);
                numberString = numberString + tempStr;
            }
            return int.Parse(numberString, System.Globalization.NumberStyles.HexNumber, null);
        }


        /// <summary>
        /// Reads the count of dimensions
        /// </summary>
        /// <returns></returns>
        private int ReadDimensionCount()
        {
            string str = ReadBytes(0, 4);
            int num = ReadInteger(str);
            if (num.Equals(NC_DIMENSION))
            {
                str = ReadBytes(0, 4);
                num = ReadInteger(str);
                return num;
            }
            return 0;  // no dimenstion available
        }

        /// <summary>
        /// Read the dimensions values and add it to meta data
        /// </summary>
        /// <param name="count"></param>
        private void ReadDimensions(int count)
        {
            //1. First 4 bytes indicates the length of the string of dimension name
            //2. Next n bytes contains name string characters, followed by null bytes to 
            //   make is multiple of 4 bytes
            //3. Next four bytes contains the size of dimension
            //4. Next 4 bytes contains values for next dimension if present else contains attributes
            for (int i = 0; i < count; ++i)
            {
                //Read dimension name
                string str = ReadBytes(0, 4);
                int stringLength = ReadInteger(str);  // Get the length of the name of the dimension
                string dimName = ReadStringName(stringLength);
                //Now read the dimension size
                str = ReadBytes(0, 4);
                int dimSize = ReadInteger(str);   // if size is 0 then its a record dimenasion. i.e. Unlimited dimension

                //NcDim dimension = new NcDim(dimName, dimSize);
                this.metadata.AddDimension(new NcDim(dimName, dimSize));
            }
        }

        /// <summary>
        /// Reads the no of bytes in multiple of 4. i.e if the length is 5 then read
        /// it and jump to 5 bytes to read next value
        /// </summary>
        /// <param name="lenght"></param>
        /// <returns></returns>
        private string ReadStringName(int length)
        {
            int jump = 0; // 4 - (length % 4);
            if (length % 4 != 0)
            {
                jump = 4 - (length % 4);
            }
            string str = ReadBytes(0, length);
            fs.Position += jump;  //Jump the null bytes if available 
            return str;
        }

        /// <summary>
        /// Get the count of global attributes
        /// </summary>
        /// <returns></returns>
        private int ReadGlobalAttributeCount()
        {
            string str = ReadBytes(0, 4);
            int num = ReadInteger(str);
            if (num.Equals(NC_ATTRIBUTE))
            {
                str = ReadBytes(0, 4);
                num = ReadInteger(str);
                return num;
            }
            return 0;  // no attributes available
        }

        /// <summary>
        /// Read the global attributes and add them to meta data
        /// </summary>
        /// <param name="count"></param>
        private void ReadGlobalAttributes(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                string str = ReadBytes(0, 4);
                int numval = ReadInteger(str);  // Get the length of the name of the global attributes
                string attName = ReadStringName(numval);

                // the nane and value of the attributes are separated by control char 2
                int controlChar = ReadInteger(ReadBytes(0, 4));
                NcType type = NcType.NcByte; //default nc type
                object attributeValue = null;
                switch (controlChar)
                {
                    case NC_BYTE:
                        type = NcType.NcByte;
                        break;
                    case NC_CHAR:
                        type = NcType.NcChar;
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str);  // length of the attribute values
                        attributeValue = ReadStringName(numval);  // attribute value
                        break;
                    case NC_SHORT:
                        type = NcType.NcShort;
                        break;
                    case NC_INT:
                        type = NcType.NcInt;
                        break;
                    case NC_FLOAT:
                        type = NcType.NcFloat;
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str);  // length of the attribute values
                        float[] fArr = new float[numval];
                        for (int j = 0; j < numval; j++)
                            fArr[j] = ReadFloat(0);

                        attributeValue = fArr;  //pass array of float as object
                        break;
                    case NC_DOUBLE:
                        type = NcType.NcDouble;
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str);  // length of the attribute values
                        double[] dArr = new double[numval];
                        for (int j = 0; j < numval; j++)
                            dArr[j] = ReadDouble(0);

                        attributeValue = dArr;  //pass array of double ad object
                        break;
                }

                NcAtt ncAtt = new NcAtt(attName, type, attributeValue);

                this.metadata.AddGlobalAttribute(ncAtt);
            }
        }


        /// <summary>
        /// Reads the count of the variables
        /// </summary>
        /// <returns></returns>
        private int ReadVariableCount()
        {
            string str = ReadBytes(0, 4);
            int num = ReadInteger(str);
            if (num.Equals(NC_VARIABLE))
            {
                str = ReadBytes(0, 4);
                num = ReadInteger(str);
                return num;
            }
            return 0;  // no variables available
        }

        /// <summary>
        /// Reads the variables and add them to the meta data
        /// </summary>
        /// <param name="count"></param>
        private void ReadVariables(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                string str = ReadBytes(0, 4);
                int stringLength = ReadInteger(str);  //get then string lenght of the variable name
                str = ReadStringName(stringLength);

                NcVar var = new NcVar(str);

                //Now read the dimension of the variable
                str = ReadBytes(0, 4);
                int dimCount = ReadInteger(str);
                List<int> dimList = new List<int>();
                for (int dCounter = 0; dCounter < dimCount; ++dCounter)
                {
                    // Read the ID of the dimension. This indicates the 
                    // sequence in which the dimensions are declared and that is
                    // used for this variable. 
                    str = ReadBytes(0, 4);
                    int dimID = ReadInteger(str);
                    dimList.Add(dimID);
                    //If variable has only one dimension , check for co-ordinate variable
                    if (dimCount == 1)
                    {
                        //If Variable Name is same as name of Dimension
                        if (this.metadata.GetDimension(var.Name) != null)
                            var.IsCoordinate = true;
                    }
                    var.SetDimensions(dCounter, this.metadata.Dimensions[dimID]);
                }
                var.Edges = dimList;

                int varAttCount = ReadVariableAttributeCount();  // Get the variable's attribute count

                if (varAttCount > 0)
                {
                    ReadVariableAttributes(varAttCount, ref var);
                }

                // After reading all the attribs read the type, size and offset
                stringLength = ReadInteger(ReadBytes(0, 4));
                switch (stringLength)
                {
                    case 1: var.Type = NcType.NcByte; break;
                    case 2: var.Type = NcType.NcChar; break;
                    case 3: var.Type = NcType.NcShort; break;
                    case 4: var.Type = NcType.NcInt; break;
                    case 5: var.Type = NcType.NcFloat; break;
                    case 6: var.Type = NcType.NcDouble; break;
                }
                int varSize = ReadInteger(ReadBytes(0, 4));
                int offset = ReadInteger(ReadBytes(0, 4));
                var.Offset = offset;
                int bCount = BytesOf(var.Type);
                if (bCount == 0)
                {
                    var.NumValues = 0;
                }
                else
                {
                    var.NumValues = varSize / BytesOf(var.Type);
                }
                // If the variable is a record variable then set the length of the
                // record var
                var.SetRecordDimSize();

                this.metadata.AddVariable(var);
            }
        }

        private int BytesOf(NcType type)
        {
            if (type.Equals(NcType.NcByte) ||
                type.Equals(NcType.NcChar) ||
                type.Equals(NcType.NcShort) ||
                type.Equals(NcType.NcFloat) ||
                type.Equals(NcType.NcInt))
            {
                return 4;
            }
            else if (type.Equals(NcType.NcDouble))
            {
                return 8;
            }
            return 0;
        }


        private int ReadVariableAttributeCount()
        {
            string str = ReadBytes(0, 4);
            int num = ReadInteger(str);
            if (num.Equals(NC_ATTRIBUTE))
            {
                str = ReadBytes(0, 4);
                num = ReadInteger(str);
                return num;
            }
            return 0;  // no variables available
        }

        /// <summary>
        /// Read the variable attributes and fill it to Ncvar object passed as parameter
        /// </summary>
        /// <param name="count"></param>
        /// <param name="var"></param>
        private void ReadVariableAttributes(int count, ref NcVar var)
        {
            for (int i = 0; i < count; ++i)
            {
                string str = ReadBytes(0, 4);
                int numval = ReadInteger(str);  // Get the length of the name of the variable attribute
                string attrName = ReadStringName(numval);

                int controlNum = ReadInteger(ReadBytes(0, 4));
                object attrVal = null;
                NcType type = NcType.NcByte;
                switch (controlNum)
                {
                    case NC_BYTE:
                        break;
                    case NC_CHAR:
                        //Read variable name length
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str);  // Get the length of the attribute value
                        attrVal = ReadStringName(numval);  // Get the attribute value
                        type = NcType.NcChar;
                        break;
                    case NC_INT:
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str); //length of arrary
                        break;
                    case NC_FLOAT:
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str); //length of arrary 
                        float[] arrF = new float[numval];
                        for (int j = 0; j < numval; j++)
                            arrF[j] = ReadFloat(0);
                        attrVal = arrF;
                        type = NcType.NcFloat;
                        break;
                    case NC_DOUBLE:
                        str = ReadBytes(0, 4);
                        numval = ReadInteger(str); //length of arrary 
                        double[] arrD = new double[numval];
                        for (int j = 0; j < numval; j++)
                            arrD[j] = ReadDouble(0);
                        attrVal = arrD;
                        type = NcType.NcDouble;
                        break;
                }
                // Add variable attributes to the varible object here.
                NcAtt varAtt = new NcAtt(attrName, type, attrVal);

                var.VariableAttributes.Add(varAtt);
            }
        }

#endregion

        #region Data Reading
        /// <summary>
        /// This will return the variable values arrays
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object ReadVariableValue(string name)
        {
            try
            {
                NcVar var = GetVar(name);
                if (var != null)
                {
                    // Get the number of values of the variables, This would be multiply of each dimension of the variable
                    int numrecs = (int)var.NumValues;
                    object[] values = new object[numrecs];
                    fs.Seek(var.Offset, SeekOrigin.Begin);
                    for (int i = 0; i < numrecs; ++i)
                    {
                        values[i] = ReadValue(var.Type);
                    }
                    return (object)values;
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }

            return null; // if variable of the name provided is not found then return null
        }

        /// <summary>
        /// Reads one value for the variable from the nc file.
        /// </summary>
        /// <param name="varName">variable for which value is to be read</param>
        /// <param name="dimNames">name of the dimensions</param>
        /// <param name="dimValues">index for the dimension value. The indexes match with the
        /// dimNames array</param>
        /// <returns></returns>
        public object ReadVariableValue(string varName, string[] dimNames, int[] dimValues)
        {
            try
            {
                NcVar var = GetVar(varName);
                if (null == var)
                {
                    return null;
                }

                // We want to get the dimension values for dimenstions which exist for the
                // variable.
                int[] dimIndexValArray = new int[var.Dimensions.Count];

                // Check if the value requested is within the dimensions. If the value
                // requested is for a dimension for which this variable does not vary
                // then return null.
                for (int i = 0; i < dimNames.Length; ++i)
                {
                    bool found = false;
                    for (int j = 0; j < var.Dimensions.Count; ++j)
                    {
                        // If the dimension exists move to the next
                        if (dimNames[i] == var.Dimensions[j].Name)
                        {
                            found = true;
                            dimIndexValArray[j] = dimValues[i];
                            break;
                        }
                    }
                    // If the dimension does not exist for the variable and a value is 
                    // requested for this dimension then return null
                    if (!found && dimValues[i] > 0)
                        return null;
                }

                if (null == var.ValueArr)
                {
                    object[] values = new object[var.NumValues];
                    fs.Seek(var.Offset, SeekOrigin.Begin);
                    for (int i = 0; i < var.NumValues; ++i)
                    {
                        values[i] = ReadValue(var.Type);
                    }
                    var.ValueArr = values;
                }

                return var.GetValue(ref dimIndexValArray);
            }
            catch (Exception)
            {
            }
            return null; 
        }

        /// <summary>
        /// Read the values accrding to the values type of the variable
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        private object ReadValue(NcType varType)
        {
            object value = null;
            switch (varType)
            {
                case NcType.NcByte:
                    value = ReadBytes(0, 4);
                    break;
                case NcType.NcChar:
                    value = ReadBytes(0, 4);
                    break;
                case NcType.NcShort:
                    value = ReadBytes(0, 4);
                    break;
                case NcType.NcInt:
                    value = ReadInteger(ReadBytes(0, 4));
                    break;
                case NcType.NcFloat:
                    value = ReadFloat(0);
                    break;
                case NcType.NcDouble:
                    value = ReadDouble(0);
                    break;
                default:
                    return null;
            }
            return value;
        }

        #endregion

        #endregion

    }

    #region Enumeration for DataType

    public enum NcType
    {
		NcByte = 1,
		NcChar,
		NcShort,
		NcInt,
		NcFloat,
		NcDouble
	}

    #endregion
}