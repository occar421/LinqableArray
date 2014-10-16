using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasuqatNet.Collections
{
	public class LinqableArray2D<T> : IStructuralEquatable
	{
		private T[] _items;
		private int[] _lengths = new int[2];

		public LinqableArray2D(int length1, int length2)
		{
			_lengths[0] = length1;
			_lengths[1] = length2;
			_items = new T[length1 * length2];
		}

		public T this[int index1, int index2]
		{
			get
			{
				if (index1 < 0 || _lengths[0] <= index1)
				{
					throw new IndexOutOfRangeException("index1");
				}
				if (index2 < 0 || _lengths[1] <= index2)
				{
					throw new IndexOutOfRangeException("index2");
				}

				var realIndex = index1 * _lengths[1] + index2;
				return _items[realIndex];
			}
			set
			{
				if (index1 < 0 || _lengths[0] <= index1)
				{
					throw new IndexOutOfRangeException("index1");
				}
				if (index2 < 0 || _lengths[1] <= index2)
				{
					throw new IndexOutOfRangeException("index2");
				}

				var realIndex = index1 * _lengths[1] + index2;
				_items[realIndex] = value;
			}
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			var otherArray = other as LinqableArray2D<T>;
			if (otherArray == null)
			{
				return false;
			}
			return comparer.Equals(this._lengths, otherArray._lengths);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return comparer.GetHashCode(this._items);
		}
	}
}
