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
using System.Globalization;

namespace Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI
{
    class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream aStream, Encoding anEncoding)
            : base(aStream, anEncoding)
        {
        }

        public override int ReadInt32()
        {
            int number = 0;
            string hex = string.Empty;
            byte[] bytes = this.ReadBytes(4);
            foreach (byte b in bytes)
            {
                hex = hex + b.ToString("x2");
            }
            number = int.Parse(hex, NumberStyles.HexNumber);
            return number;
        }

        public override uint ReadUInt32()
        {
            uint number = 0;
            string hex = string.Empty;
            
            byte[] bytes = this.ReadBytes(4);
            foreach (byte b in bytes)
            {
                hex = hex + b.ToString("x2");
            }
            number = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return number;
        }

		public override short ReadInt16()
		{
			Int16 number = 0;
			string hex = string.Empty;

			byte[] bytes = this.ReadBytes(2); //<---- Change is Here
			foreach (byte b in bytes)
			{
				hex = hex + b.ToString("x2");
			}
			number = Int16.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			return number;
		}

        public override float ReadSingle() {
            float number = 0;
            int size = 4;
            byte[] bytes = this.ReadBytes(size);
            byte[] bytes_reversed = new byte[size];
            for (uint i = 0; i < size; i++) {
                bytes_reversed[(size - 1) - i] = bytes[i];
            }
            number = BitConverter.ToSingle(bytes_reversed, 0);
            return number;
        }

        public override double ReadDouble() {
            double number = 0;
            int size = 8;
            byte[] bytes = this.ReadBytes(size);
            byte[] bytes_reversed = new byte[size];
            for (uint i = 0; i < size; i++) {
                bytes_reversed[(size - 1) - i] = bytes[i];
            }
            number = BitConverter.ToDouble(bytes_reversed, 0);
            return number;
        }

        public string ReadString(uint count, bool applyPadding)
        {
            char[] characters = this.ReadChars((int)count);
            string str = new String(characters);

            if (applyPadding && ((count % 4) != 0))
            {
                // calculate the padding applied. 
                uint pad = 4 - (count % 4);

                // read through the padded fields to advance the file pointer. 
                this.ReadBytes((int)pad);
            }
            return str;
        }

        public byte[] ReadBytes(uint count, bool applyPadding) {
            byte[] bytes = this.ReadBytes((int)count);

            if (applyPadding && ((count % 4) != 0)) {
                // calculate the padding applied. 
                uint pad = 4 - (count % 4);

                // read through the padded fields to advance the file pointer. 
                this.ReadBytes((int)pad);
            }
            return bytes;
        }

        public short[] ReadShorts(uint count, bool applyPadding)
        {
            short[] shorts = new short[count];
            for (int i = 0; i < count; i++)
            {
                shorts[i] = this.ReadInt16();
            }

            if (applyPadding && (((count * sizeof(short)) % 4) != 0))
            {  
                // calculate the padding applied. 
                uint pad = 4 - ((count * sizeof(short)) % 4); 

                // read through the padded fields to advance the file pointer. 
                this.ReadBytes((int)pad);
            }
            return shorts;
        }

        public int[] ReadIntegers(uint count) {
            int[] integers = new int[count];
            for (int i = 0; i < count; i++) {
                integers[i] = this.ReadInt32();
            }
            return integers;
        }

        public float[] ReadSingles(uint count) {
            float[] floats = new float[count];
            for (uint i = 0; i < count; i++) {
                floats[i] = this.ReadSingle();
            }
            return floats;
        }

        public double[] ReadDoubles(uint count) {
            double[] doubles = new double[count];
            for (uint i = 0; i < count; i++) {
                doubles[i] = this.ReadDouble();
            }
            return doubles;            
        }
    }
}
