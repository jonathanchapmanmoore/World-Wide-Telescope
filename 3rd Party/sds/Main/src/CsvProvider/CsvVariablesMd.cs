// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;

namespace Microsoft.Research.Science.Data.CSV
{
    /// <summary>Class that represents multidimensional (>=2) variable in CSV files</summary>
    /// <typeparam name="DataType">Type of variable. For list of supported types see DataSet specification.</typeparam>
    /// <remarks>Instances of this call cannot be constructed directly. See 
    /// <see cref="M:Microsoft.Research.Science.Data.DataSet.AddVariable"/> for details about creation
    /// of new variables</remarks>
    internal sealed class CsvVariableMd<DataType> : CsvVariable<DataType>, ICsvVariableMd
    {
        internal CsvVariableMd(CsvDataSet dataSet, CsvColumn column)
            : base(dataSet, column)
        {
            if (column.Rank <= 2)
                throw new Exception("This is multidimensional variable and is being created with different rank.");

            data = new ArrayWrapper(column.Rank, typeof(DataType));

            Initialize();
        }

        private CsvVariableMd(CsvDataSet dataSet, int id, MetadataDictionary metadata, ArrayWrapper data, string[] dims)
            : base(dataSet, id, metadata, dims)
        {
            this.data = data;
            Initialize();
        }

        public override Variable CloneAndRenameDims(string[] newDims)
        {
            if (newDims == null || Rank != newDims.Length)
                throw new Exception("New dimensions are wrong");
            Variable var = new CsvVariableMd<DataType>((CsvDataSet)DataSet, ID, Metadata, data, newDims);
            return var;
        }

        protected override Array GetInnerData()
        {
            Array array = data.Data;
            if (array == null)
                return new DataType[0] { };

            int dimCount = Rank;
            int[] indices = new int[dimCount]; // Zero values by default
            int n = array.Length;
            DataType[] innerData = new DataType[n];
            for (int i = 0; i < n; i++)
            {
                innerData[i] = (DataType)array.GetValue(indices);
                int j = dimCount - 1;
                while (j >= 0)
                {
                    indices[j]++;
                    if (indices[j] >= array.GetLength(j))
                        indices[j--] = 0;
                    else
                        break;
                }
            }

            return innerData;
        }

        protected override void InnerInitialize(Array data, int[] shape)
        {
            if (data == null) return;

            int dimCount = Rank;
            int[] indices = new int[dimCount]; // Zero values by default
            int n = data.Length;
            DataType[] typedData = (DataType[])data;
            Array array = Array.CreateInstance(TypeOfData, shape);

            for (int i = 0; i < n; i++)
            {
                array.SetValue(typedData[i], indices);
                int j = dimCount - 1;
                while (j >= 0)
                {
                    indices[j]++;
                    if (indices[j] >= shape[j])
                        indices[j--] = 0;
                    else
                        break;
                }
            }
            base.data.PutData(null, array);
            ChangesUpdateShape(this.changes, ReadShape());
        }

        Array ICsvVariableMd.GetColumnData(int col)
        {
            Array array = data.Data;
            if (array == null)
                return new DataType[0] { };

            int rank = Rank;
            int n = array.Length / array.GetLength(rank - 1);

            DataType[] colData = new DataType[n];
            int[] indices = new int[rank];
            indices[rank - 1] = col; // column ~ last index
            for (int i = 0; i < n; i++)
            {
                colData[i] = (DataType)array.GetValue(indices);
                int j = rank - 2;
                while (j >= 0)
                {
                    indices[j]++;
                    if (indices[j] >= array.GetLength(j))
                        indices[j--] = 0;
                    else
                        break;
                }
            }
            return colData;
        }

        void ICsvVariableMd.Initialize(Array data)
        {
            base.data.PutData(null, data);
            ChangesUpdateShape(this.changes, ReadShape());
        }

        void ICsvVariableMd.FastCopyColumn(Array entireArray, Array column, int index)
        {
            DataType[] colData = (DataType[])column;

            int rank = Rank;
            int n = column.Length;

            int[] indices = new int[rank];
            indices[rank - 1] = index; // column ~ last index

            for (int i = 0; i < n; i++)
            {
                entireArray.SetValue(colData[i], indices);
                int j = rank - 2;
                while (j >= 0)
                {
                    indices[j]++;
                    if (indices[j] >= entireArray.GetLength(j))
                        indices[j--] = 0;
                    else
                        break;
                }
            }
        }
    }
}

