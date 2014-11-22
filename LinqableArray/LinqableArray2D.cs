using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasuqatNet.Collections
{
	public class LinqableArray2D<T> : IStructuralEquatable, IStructuralComparable, IEnumerable<T>, IEnumerable
	{
		private T[] _items;
		private int[] _lengths = new int[2];

		public LinqableArray2D(int length1, int length2)
		{
			if (length1 < 0)
			{
				throw new ArgumentOutOfRangeException("length1");
			}
			if (length2 < 0)
			{
				throw new ArgumentOutOfRangeException("length2");
			}

			_lengths[0] = length1;
			_lengths[1] = length2;
			_items = new T[length1 * length2];
		}

		public LinqableArray2D(T[,] rectangularArray)
		{
			if (rectangularArray == null)
			{
				throw new ArgumentNullException("rectangularArray");
			}

			_lengths[0] = rectangularArray.GetLength(0);
			_lengths[1] = rectangularArray.GetLength(1);
			_items = new T[_lengths[0] * _lengths[1]];

			var i = 0;
			foreach (var item in rectangularArray)
			{
				_items[i] = item;
				i++;
			}
		}

		public LinqableArray2D(T[][] juggedArray)
		{
			if (juggedArray == null)
			{
				throw new ArgumentNullException("juggedArray");
			}

			_lengths[0] = juggedArray.Length;
			_lengths[1] = juggedArray.Length <= 0 ? 0 : juggedArray.First().Length;
			if (juggedArray.Any(x => x.Length != _lengths[1]))
			{
				throw new ArgumentException("array contains different length sub-array", "array");
			}

			_items = new T[_lengths[0] * _lengths[1]];

			var i = 0;
			foreach (var subArray in juggedArray)
			{
				foreach (var item in subArray)
				{
					_items[i] = item;
					i++;
				}
			}
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

				return Get(index1, index2);
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

				Set(value, index1, index2);
			}
		}

		//reliable range only
		private T Get(int index1, int index2)
		{
			var realIndex = index1 * _lengths[1] + index2;
			return _items[realIndex];
		}

		//reliable range only
		private void Set(T value, int index1, int index2)
		{
			var realIndex = index1 * _lengths[1] + index2;
			_items[realIndex] = value;
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

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;

			}
			var otherArray = other as LinqableArray2D<T>;
			if (otherArray == null)
			{
				throw new ArgumentException("Incorrect type", "other");
			}

			return comparer.Compare(this._lengths, otherArray._lengths);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)_items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// Searching the item from entire array. This returns indices begin from 0.
		/// </summary>
		/// <returns>Tuple of index numbers. If array does not contains item, returns (-1, -1)</returns>
		public Tuple<int, int> IndexOf(T item)
		{
			var index = Array.IndexOf(_items, item);
			if (index < 0)
			{
				return new Tuple<int, int>(-1, -1);
			}
			return new Tuple<int, int>(index / _lengths[1], index % _lengths[1]);
		}

		/// <summary>
		/// Get enumeration of indexed one.
		/// </summary>
		/// <returns>One IEnumerable&lt;T&gt; at index.</returns>
		public IEnumerable<T> Get1DEnumerable(int dimension, int index, bool reverse = false)
		{
			int stride, initIndex, length;

			switch (dimension)
			{
				case 0:
					stride = reverse ? -1 : 1;
					initIndex = reverse ? ((index + 1) * _lengths[1] - 1) : (index * _lengths[1]);
					length = _lengths[1];
					break;
				case 1:
					stride = reverse ? -_lengths[1] : _lengths[1];
					initIndex = reverse ? ((_lengths[0] - 1) * _lengths[1] + index) : index;
					length = _lengths[0];
					break;
				default:
					throw new ArgumentOutOfRangeException("dimension");
			}

			for (int itr = initIndex, i = 0; i < length; itr += stride, i++)
			{
				yield return _items[itr];
			}
		}

		/// <summary>
		/// Get all enumeration.
		/// </summary>
		/// <returns>All IEnumerable&lt;T&gt;</returns>
		public IEnumerable<IEnumerable<T>> GetAll1DEnumerables(int dimension, bool reverseItem = false, bool reverseEnumerate = false)
		{
			if (dimension < 0 || 2 < dimension)
			{
				throw new ArgumentOutOfRangeException("dimension");
			}

			if (reverseEnumerate)
			{
				for (int i = _lengths[dimension] - 1; i >= 0; i--)
				{
					yield return Get1DEnumerable(dimension, i, reverseItem);
				}
			}
			else
			{
				for (int i = 0; i < _lengths[dimension]; i++)
				{
					yield return Get1DEnumerable(dimension, i, reverseItem);
				}
			}
		}
	}
}
