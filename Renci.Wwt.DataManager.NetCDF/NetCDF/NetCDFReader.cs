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
	public abstract class NetCDFReader : IDisposable
	{
		protected Dictionary<uint, INetCDFDimension> dimensions = new Dictionary<uint, INetCDFDimension>();
		protected Dictionary<string, INetCDFAttribute> globalAttributes = new Dictionary<string, INetCDFAttribute>();
		protected Dictionary<string, INetCDFVariable> variables = new Dictionary<string, INetCDFVariable>();

		protected uint numberOfRecords;
		protected uint numberOfDimensions;
		protected uint numberOfGlobalAttributes;
		protected uint numberOfVariables;
		protected string unlimitedDimension;

		protected string filename;

		/// <summary>
		/// Creates the Reader. 
		/// </summary>
		/// <param name="theFilePath">Path of the file to read</param>
		/// <returns></returns>
		public static NetCDFReader Create(string theFilePath)
		{
			NetCDFReader reader;
			const int default_blocksize = 8192;
			FileStream fileStream = new FileStream(theFilePath, FileMode.Open, FileAccess.Read,
					FileShare.Read, default_blocksize, FileOptions.RandomAccess);

			BigEndianBinaryReader binaryFileReader = new BigEndianBinaryReader(fileStream, Encoding.ASCII);

			string format = binaryFileReader.ReadString(3, false);
			if (String.Compare(format, "CDF", false) == 0)
			{
				int format_id = binaryFileReader.ReadByte();
				if (format_id == 1)
				{
					reader = new ClassicNetCDFFileReader(binaryFileReader);
				}
				//else if (format_id == 2)
				//{
				//    reader = new NetCDF64BitOffsetFileReader(binaryFileReader);
				//}
				else
				{
					string message = String.Format("Format ID: {0} not supported", format_id);
					throw new FileFormatNotSupportedException(message);
				}
			}
			else
			{
				string message = String.Format("{0} is not a valid netCDF file", theFilePath);
				throw new InvalidFileTypeException(message);
			}
			return reader;
		}

        public abstract T[] ReadVariable<T>(string aName);

        public abstract IEnumerable<decimal> ReadDecimalVariable(string aName);

        public abstract T[] ReadVariable<T>(string aName, uint theRecordIndex);

		public uint NumberOfRecords
		{
			get { return this.numberOfRecords; }
			set { this.numberOfRecords = value; }
		}

		public uint NumberOfDimensions {
			get { return this.numberOfDimensions; }
		}

		public uint NumberOfGlobalAttributes {
			get { return this.numberOfGlobalAttributes; }
		}

		public uint NumberOfVariables {
			get { return this.numberOfVariables; }
		}

		public string UnlimitedDimension {
			get { return this.unlimitedDimension; }
		}

		public Dictionary<uint, INetCDFDimension> Dimensions {
			get { return this.dimensions; }
		}

		public Dictionary<string, INetCDFAttribute> Attributes {
			get { return this.globalAttributes; }
		}

		public Dictionary<string, INetCDFVariable> Variables
		{
			get { return this.variables; }
		}		
		public NcMetaData GetMetadata()
		{
			NcMetaData metadata = new NcMetaData();

			Dictionary<uint, INetCDFDimension>.Enumerator allDims = this.Dimensions.GetEnumerator();
			while (allDims.MoveNext())
			{
				INetCDFDimension iDim = allDims.Current.Value;
				NcDim dim = new NcDim(iDim.Name, (int)iDim.Length);
			}

			return metadata;
		}

		#region IDisposable Members

		abstract public void Dispose();

		#endregion
	}
}
