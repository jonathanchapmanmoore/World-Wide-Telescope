// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.Science.Data
{
	/// <summary>
	/// Keeps array, its rank and type. Empty and zero-rank arrays are supported.
	/// </summary>
    [Obsolete("This class is obsolete and will be removed in the next version.")]
	public class ArrayWrapper
	{
        /// <summary>Array of holding actual data. Can be null if there is no data</summary>
		protected Array array;
        /// <summary>Rank of array. Valid even if <see cref="array"/> field is null</summary>
		protected int rank;
        /// <summary>Type of array elements. Valid even if <see cref="type"/> field is null</summary>
		protected Type type;

        /// <summary>Initializes a new instance of ArrayWrapper with specified rank and type of elements
        /// but without data</summary>
        /// <param name="rank">Rank of array. Zero-ranked arrays are also supported.</param>
        /// <param name="type">Type of array elements</param>
		public ArrayWrapper(int rank, Type type)
		{
			this.rank = rank;
			if (rank < 0)
				throw new ArgumentOutOfRangeException("Rank must be non-negative");
			this.type = type;
			array = null;
			if (rank == 0)
			{
				array = Array.CreateInstance(type, 1);
			}
		}

        /// <summary>Gets or sets data for the array</summary>
        /// <remarks>Array is not copied to the wrapper, so modification of assigned
        /// array means modification of ArrayWrapper</remarks>
		public Array Data
		{
			get
			{
				return array;
			}
			set
			{
				if (value.Rank != (rank == 0 ? 1 : rank))
					throw new ArgumentException("Rank is wrong");
				array = value;
			}
		}

        /// <summary>Returns rank of wrapper array. Note that zero-ranked array are supported</summary>
		public int Rank { get { return rank; } }

        /// <summary>Return type of elements for this array</summary>
		public Type DataType { get { return type; } }

		#region Data access routines

        /// <summary>Return array with shape of wrapper array. Returned array is a copy, so
        /// it can be modified in any way without affecting ArrayWrapper</summary>
        /// <returns>Array of integers containing sizes of dimensions</returns>
		public int[] GetShape()
		{
			int[] shape;
			if (array != null)
			{
				shape = new int[rank];
				for (int i = 0; i < rank; i++)
				{
					shape[i] = array.GetLength(i);
				}
				return shape;
			}

			shape = new int[rank];
			return shape;
		}

        /// <summary>Returns copy of data from part of wrapper array</summary>
        /// <param name="origin">The origin of the window (e.g., the left-bottom corner). Null means all zeros.</param>
        /// <param name="shape">The shape of the region. Null means entire array.</param>
        /// <returns>An array of data from the specified region.</returns>
		public virtual Array GetData(int[] origin, int[] shape)
		{
			if (rank == 0)
			{
				if (array == null)
					throw new Exception("Array is empty");
				return array;
			}

			if ((shape == null || EqualToShape(shape)) &&
				(origin == null || IsZero(origin)))
			{
				return array != null ? (Array)array.Clone() : Array.CreateInstance(type, new int[rank]);
			}

			if (array == null)
				throw new Exception("Array is empty");

			if (origin == null)
				origin = new int[array.Rank];
			else if (origin.Length != rank)
				throw new Exception("Wrong length of origin.");

			if (shape == null)
			{
				shape = new int[array.Rank];
				for (int i = 0; i < array.Rank; i++)
					shape[i] = array.GetLength(i) - origin[i];
			}
			else if (shape.Length != rank)
				throw new Exception("Wrong length of shape.");

			Array res = Array.CreateInstance(type, shape);
			CopyArray(array, origin, res);
			return res;
		}

        /// <summary>Writes the data to the wrapped array starting with the specified origin indices.
        /// </summary>
        /// <param name="origin">Indices to start adding of data. Null means all zeros.</param>
        /// <param name="a">Data to add to the variable.</param>
		public virtual void PutData(int[] origin, Array a)
		{
            if (a == null) return;
			if (rank == 0)
			{
				if (a.Rank != 1)
					throw new Exception("Wrong rank");
				array.SetValue(a.GetValue(0), 0);
				return;
			}

			if (rank != a.Rank)
				throw new Exception("Wrong rank");

			int[] shape = null;

			if (array == null)
			{
				if (origin == null || IsZero(origin))
				{
					array = (Array)a.Clone();
					return;
				}

				shape = new int[rank];
				for (int i = 0; i < rank; i++)
					shape[i] = origin[i] + a.GetLength(i);

				array = Array.CreateInstance(type, shape);
				CopyArray(a, array, origin);
			}
			else
			{
				Array oldArr = array;

				if (origin == null)
					origin = new int[rank];

				shape = new int[array.Rank];
				bool sufficient = true;
				for (int i = 0; i < array.Rank; i++)
				{
					int size = origin[i] + a.GetLength(i);
					sufficient = sufficient && (size <= oldArr.GetLength(i));

					shape[i] = Math.Max(size, oldArr.GetLength(i));
				}

				Array newArr;
				if (sufficient)
				{
					newArr = oldArr;
				}
				else
				{
					newArr = Array.CreateInstance(type, shape);
					CopyArray(oldArr, newArr, new int[array.Rank]);
				}

				if (origin == null)
					origin = new int[array.Rank];
				CopyArray(a, newArr, origin);
				array = newArr;
			}

			return;// shape;
		}

		#region Utilities

		private bool EqualToShape(int[] shape)
		{
			if (array == null)
				return shape == null || IsZero(shape);

			for (int i = 0; i < rank; i++)
			{
				if (shape[i] != array.GetLength(i))
					return false;
			}
			return true;
		}

		private void CopyArray(Array src, Array dst, int[] dstOrigin)
		{
			if (src.Length == 0)
				return;

			if (src.Rank == 1)
			{
				Array.Copy(src, 0, dst, dstOrigin[0], src.Length);
				return;
			}

			int[] getIndex = new int[src.Rank];
			int[] setIndex = new int[src.Rank];
			dstOrigin.CopyTo(setIndex, 0);

			int length = src.Length;

			for (int i = 0; i < length; i++)
			{
				dst.SetValue(src.GetValue(getIndex), setIndex);

				for (int j = 0; j < getIndex.Length; j++)
				{
					getIndex[j]++;
					setIndex[j]++;
					if (getIndex[j] < src.GetLength(j))
						break;
					getIndex[j] = 0;
					setIndex[j] = dstOrigin[j];
				}
			}
		}

		private void CopyArray(Array src, int[] srcOrigin, Array dst)
		{
			if (src.Length == 0)
				return;

			if (src.Rank == 1)
			{
				Array.Copy(src, srcOrigin[0], dst, 0, dst.Length);
				return;
			}

			int[] getIndex = new int[src.Rank];
			int[] setIndex = new int[src.Rank];
			srcOrigin.CopyTo(getIndex, 0);

			int length = dst.Length;

			for (int i = 0; i < length; i++)
			{
				dst.SetValue(src.GetValue(getIndex), setIndex);

				for (int j = 0; j < getIndex.Length; j++)
				{
					getIndex[j]++;
					setIndex[j]++;
					if (setIndex[j] < dst.GetLength(j))
						break;
					setIndex[j] = 0;
					getIndex[j] = srcOrigin[j];
				}
			}
		}

		private bool IsZero(int[] origin)
		{
			for (int i = 0; i < origin.Length; i++)
			{
				if (origin[i] != 0) return false;
			}
			return true;
		}

		#endregion

		#endregion


        /// <summary>Creates a copy of ArrayWrapper instance</summary>
        /// <returns>Full copy of ArrayWrapper</returns>
		public virtual ArrayWrapper Copy()
		{
			ArrayWrapper aw = new ArrayWrapper(rank, type);
			if (array != null)
				aw.PutData(null, array);

			return aw;
		}
	}
}
