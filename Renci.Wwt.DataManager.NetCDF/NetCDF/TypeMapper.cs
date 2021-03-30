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
    public static class TypeMapper
    {
		public static NetCDFDataType GetNcType(Type clrType)
        {
			NetCDFDataType ncType;

            if(clrType == typeof(byte))
            {
				ncType = NetCDFDataType.NcByte;
            }
            else if(clrType == typeof(char))
            {
				ncType = NetCDFDataType.NcChar;
            }
            else if(clrType == typeof(double))
            {
				ncType = NetCDFDataType.NcDouble;
            }
            else if(clrType == typeof(float))
            {
				ncType = NetCDFDataType.NcFloat;
            }
            else if(clrType == typeof(int))
            {
				ncType = NetCDFDataType.NcInt;
            }
			//else if(clrType == typeof(long))
			//{
			//    ncType = NetCDFDataType.NcLong;
			//}
            else if(clrType == typeof(short))
            {
				ncType = NetCDFDataType.NcShort;
            }
            else 
            {
                throw new ArgumentException("The given type cannot be mapped to a valid NcType.");
            }
            return ncType;
        }

        public static NcType GetNcFileType(Type clrType)
        {
            NcType ncType;

            if (clrType == typeof(byte))
            {
                ncType = NcType.NcByte;
            }
            else if (clrType == typeof(char))
            {
                ncType = NcType.NcChar;
            }
            else if (clrType == typeof(double))
            {
                ncType = NcType.NcDouble;
            }
            else if (clrType == typeof(float))
            {
                ncType = NcType.NcFloat;
            }
            else if (clrType == typeof(int))
            {
                ncType = NcType.NcInt;
            }
            else if (clrType == typeof(short))
            {
                ncType = NcType.NcShort;
            }
            else
            {
                throw new ArgumentException("The given type cannot be mapped to a valid NcType.");
            }
            return ncType;
        }

        public static Type GetClrType(NetCDFDataType ncType)
        {
            Type clrType = null;

            switch (ncType)
            {
				case NetCDFDataType.NcByte:
                    clrType = typeof(byte);
                    break;
				case NetCDFDataType.NcChar:
                    clrType = typeof(char);
                    break;
				case NetCDFDataType.NcDouble:
                    clrType = typeof(double);
                    break;
				case NetCDFDataType.NcInt:
                    clrType = typeof(Int32);
                    break;
				//case NetCDFDataType.NcLong:
				//    clrType = typeof(long);
				//    break;
				case NetCDFDataType.NcShort:
                    clrType = typeof(short);
                    break;
				case NetCDFDataType.NcFloat:
                    clrType = typeof(float);
                    break;
            }

            return clrType;
        }

        public static Type GetClrType(string type)
        {
            Type clrType = null;

            switch (type)
            {
                case "NcByte":
                    clrType = typeof(byte);
                    break;
                case "NcChar":
                    clrType = typeof(char);
                    break;
                case "NcDouble":
                    clrType = typeof(double);
                    break;
                case "NcInt":
                    clrType = typeof(Int32);
                    break;
                //case NetCDFDataType.NcLong:
                //    clrType = typeof(long);
                //    break;
                case "NcShort":
                    clrType = typeof(short);
                    break;
                case "NcFloat":
                    clrType = typeof(float);
                    break;
                default:
                    clrType = typeof(byte);
                    break;
            }

            return clrType;
        }
    }
}
