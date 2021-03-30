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
    public class CDFWriter
    {
        class DimensionWr
        {
            public string valName;
            public bool isRecord;
            public int dimIndex;
            public int size;

            public static int indexCounter = 0;
        }

        class VariableWr
        {
            public string varName;
            public int varType = 0;
            public long varOffset;
            public List<DimensionWr> dimList;
        }

        public CDFWriter(string fileName)
        {
            this.writer = new FileStream(fileName, FileMode.Create);
            this.bw = new BinaryWriter(this.writer);
            this.dimList = new List<DimensionWr>();
            this.varList = new List<VariableWr>();
            this.isRecordWritten = false;
        }

        private void WriteIntNumber(int num)
        {
            byte[] arr = BitConverter.GetBytes(num);
            this.bw.Write(arr[3]);
            this.bw.Write(arr[2]);
            this.bw.Write(arr[1]);
            this.bw.Write(arr[0]);
        }

        private void WriteFloatNumber(float num)
        {
            byte[] arr = BitConverter.GetBytes(num);
            this.bw.Write(arr[3]);
            this.bw.Write(arr[2]);
            this.bw.Write(arr[1]);
            this.bw.Write(arr[0]);
        }

        private void WriteDoubleNumber(double num)
        {
            byte[] arr = BitConverter.GetBytes(num);
            this.bw.Write(arr[7]);
            this.bw.Write(arr[6]);
            this.bw.Write(arr[5]);
            this.bw.Write(arr[4]);
            
            this.bw.Write(arr[3]);
            this.bw.Write(arr[2]);
            this.bw.Write(arr[1]);
            this.bw.Write(arr[0]);
        }

        private void WriteStringVal(string strVal)
        {
            // The number of bytes to write should be a multiple of four.
            // Read the first four bytes
            for (int i = 0; i < strVal.Length; ++i)
            {
                this.bw.Write(strVal[i]);
            }

            if (0 == (strVal.Length % 4))
                return;

            byte b = 0;
            for (int i = 0; i < (4 - (strVal.Length % 4)); ++i)
            {
                this.bw.Write(b);
            }
        }

        private void WriteVal(int type, object val)
        {
            switch (type)
            {
                case 1: // NC_BYTE
                    {
                        //WriteIntNumber(1);
                        break;
                    }
                case 2: // NC_CHAR
                    {
                        WriteIntNumber(((string)val).Length);
                        WriteStringVal((string)val);
                        break;
                    }
                case 3: //NC_SHORT
                    {
                        //WriteIntNumber(3);
                        break;
                    }
                case 4: // NC_INT
                    {
                        WriteIntNumber((int)val);
                        break;
                    }
                case 5: // NC_FLOAT
                    {
                        WriteFloatNumber((float)val);
                        break;
                    }
                case 6: // NC_DOUBLE
                    {
                        WriteDoubleNumber((double)val);
                        break;
                    }
                default:
                    // Raise some error;
                    break;
            }
        }

        private void WriteType(int typeVal)
        {
            switch (typeVal)
            {
                case 1: // NC_BYTE
                    {
                        WriteIntNumber(1);
                        break;
                    }
                case 2: // NC_CHAR
                    {
                        WriteIntNumber(2);
                        break;
                    }
                case 3: //NC_SHORT
                    {
                        WriteIntNumber(3);
                        break;
                    }
                case 4: // NC_INT
                    {
                        WriteIntNumber(4);
                        break;
                    }
                case 5: // NC_FLOAT
                    {
                        WriteIntNumber(5);
                        break;
                    }
                case 6: // NC_DOUBLE
                    {
                        WriteIntNumber(6);
                        break;
                    }
                default:
                    // Raise some error;
                    break;
            }
        }

        private int GetByteSize(int variableType)
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
                default:
                    // Raise some error;
                    break;
            }
            return 0;
        }

        public void WriteHeader()
        {
            uint header = 21382211;
            this.bw.Write(header);
        }

        public void AddRecordVarCount(int recVarNum)
        {
            this.WriteIntNumber(recVarNum);
        }

        public void AddDimensionHeader(int dimNumber)
        {
            this.WriteIntNumber(0xA);
            this.WriteIntNumber(dimNumber);
        }

        public void AddDimension(string dimName, int dimVal)
        {
            // Write the string length first
            this.WriteIntNumber(dimName.Length);
            this.WriteStringVal(dimName);

            this.WriteIntNumber(dimVal);

            DimensionWr d = new DimensionWr();
            d.valName = dimName;
            d.isRecord = (0 == dimVal) ? true : false;
            d.dimIndex = DimensionWr.indexCounter;
            d.size = dimVal;
            DimensionWr.indexCounter += 1;
            this.dimList.Add(d);
        }

        // As of now we are not supporting adding global
        // variables
        public void AddGlobalAttHeader(int globablAttCount)
        {
            if (0 == globablAttCount)
                this.WriteIntNumber(0);
            else
                this.WriteIntNumber(0xC);
            this.WriteIntNumber(globablAttCount);
        }

        public void WriteAttribute(int attribType, string attribName, object attribVal)
        {
            if (attribType < 1 && attribType > 6)
            {
                // Raise error
            }

            // Start with writing the attribue name
            this.WriteIntNumber(attribName.Length);
            this.WriteStringVal(attribName);

            WriteType(attribType);

            this.WriteVal(attribType, attribVal);
        }

        public void AddVariableHeader(int varCount)
        {
            if (0 == varCount)
                this.WriteIntNumber(0);
            else
                this.WriteIntNumber(0xB);
            this.WriteIntNumber(varCount);

            //this.varNameList = new string[varCount];
            //this.varOffset = new long[varCount];
        }

        public void WriteVariable(int variableType, string varName, List<NcDim> dimArray,
            int[] attribTypeArray, string[] attribNamrArr, string[] AttribVal)
        {
            // Start with writing the variable name
            this.WriteIntNumber(varName.Length);
            this.WriteStringVal(varName);

            VariableWr varWr = new VariableWr();
            varWr.varName = varName;
            varWr.dimList = new List<DimensionWr>();

            // Write the dimensions
            this.WriteIntNumber(dimArray.Count);
            for (int i = 0; i < dimArray.Count; ++i)
            {
                foreach(DimensionWr d in this.dimList)
                {
                    if (d.valName == dimArray[i].Name)
                    {
                        this.WriteIntNumber(d.dimIndex);
                        varWr.dimList.Add(d);
                    }
                }
            }

            this.AddGlobalAttHeader(attribNamrArr.Length);

            for (int i = 0; i < attribNamrArr.Length; ++i)
            {
                this.WriteAttribute(attribTypeArray[i], attribNamrArr[i],
                    AttribVal[i]);
            }

            // Write the variable type. Write two bytes with values zero
            // for size and offset. They will be overwritten when writing values
            // for this variable.
            this.WriteType(variableType);

            // Add this to the internal var list
            varWr.varOffset = this.bw.BaseStream.Length;

            this.WriteIntNumber(0);
            this.WriteIntNumber(0);
            this.varList.Add(varWr);
        }

        private void UpdateRecordCount(ref VariableWr var, int dataLength)
        {
            int divisor = 1;
            for (int i = 0; i < var.dimList.Count; ++i)
            {
                if (false == var.dimList[i].isRecord)
                {
                    divisor = divisor * var.dimList[i].size;
                }
            }
            int recCount = dataLength / divisor;
            this.bw.Seek(4, SeekOrigin.Begin);
            this.WriteIntNumber(recCount);
        }

        public void WriteVariableVal(int variableType, string varName, object[] valArray)
        {
            VariableWr var = null;
            for (int j = 0; j < this.varList.Count; ++j)
            {
                if (varName == this.varList[j].varName)
                {
                    var = this.varList[j];
                    break;
                }
            }

            // Get the offset
            this.bw.Seek((int)var.varOffset, SeekOrigin.Begin);
            this.WriteIntNumber(this.GetByteSize(variableType));
            this.WriteIntNumber((int)this.bw.BaseStream.Length);

            // If this is a record var then update record count
            if (false == this.isRecordWritten)
            {
                UpdateRecordCount(ref var, valArray.Length);
                this.isRecordWritten = true;
            }

            // Now move to the end of the file and start writing values
            this.bw.Seek((int)this.bw.BaseStream.Length, SeekOrigin.Begin);
            for (int i = 0; i < valArray.Length; ++i)
            {
                this.WriteVal(variableType, valArray[i]);
            }
        }


        FileStream writer;
        BinaryWriter bw;
        //string[] varNameList;
        //long[] varOffset;
        List<DimensionWr> dimList;
        List<VariableWr> varList;
        bool isRecordWritten;
    }
}
