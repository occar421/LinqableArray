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

		private LinqableArray2D() { }

		public LinqableArray2D(int length0, int length1)
		{
			if (length0 < 0)
			{
				throw new ArgumentOutOfRangeException("length0");
			}
			if (length1 < 0)
			{
				throw new ArgumentOutOfRangeException("length1");
			}

			_lengths[0] = length0;
			_lengths[1] = length1;
			_items = new T[length0 * length1];
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

		public T this[int index0, int index1]
		{
			get
			{
				if (index0 < 0 || _lengths[0] <= index0)
				{
					throw new IndexOutOfRangeException("index0");
				}
				if (index1 < 0 || _lengths[1] <= index1)
				{
					throw new IndexOutOfRangeException("index1");
				}

				return Get(index0, index1);
			}
			set
			{
				if (index0 < 0 || _lengths[0] <= index0)
				{
					throw new IndexOutOfRangeException("index0");
				}
				if (index1 < 0 || _lengths[1] <= index1)
				{
					throw new IndexOutOfRangeException("index1");
				}

				Set(value, index0, index1);
			}
		}

		//reliable range only
		private T Get(int index0, int index1)
		{
			var realIndex = index0 * _lengths[1] + index1;
			return _items[realIndex];
		}

		//reliable range only
		private void Set(T value, int index0, int index1)
		{
			var realIndex = index0 * _lengths[1] + index1;
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

		public T[,] ToArray2D()
		{
			var array2D = new T[_lengths[0], _lengths[1]];
			for (int i = 0; i < _lengths[0]; i++)
			{
				for (int j = 0; j < _lengths[1]; j++)
				{
					array2D[i, j] = this.Get(i, j);
				}
			}
			return array2D;
		}

		public T[][] ToJuggedArray2D()
		{
			var array2D = new T[_lengths[0]][];
			for (int i = 0; i < _lengths[0]; i++)
			{
				array2D[i] = new T[_lengths[1]];
				for (int j = 0; j < _lengths[1]; j++)
				{
					array2D[i][j] = this.Get(i, j);
				}
			}
			return array2D;
		}

		/// <summary>
		/// Get length of a dimension
		/// </summary>
		/// <returns>length of a dimension</returns>
		public int GetLength(int dimension)
		{
			if (dimension < 0 || 1 < dimension)
			{
				throw new ArgumentOutOfRangeException("dimension");
			}
			return _lengths[dimension];
		}

		/// <summary>
		/// Returns a new shaped 2d-array without changing data.
		/// </summary>
		/// <returns>reshaped 2D array</returns>
		public LinqableArray2D<T> Reshape(Tuple<int, int> sizes)
		{
			if (sizes == null)
			{
				throw new ArgumentNullException("sizes");
			}

			if (sizes.Item1 < 0)
			{
				throw new ArgumentOutOfRangeException("sizes.Item1");
			}
			if (sizes.Item2 < 0)
			{
				throw new ArgumentOutOfRangeException("sizes.Item2");
			}

			if (_lengths[0] * _lengths[1] != sizes.Item1 * sizes.Item2)
			{
				throw new ArgumentException("total size of new array must be unchanged", "sizes");
			}

			return ReshapeInPrivate(sizes.Item1, sizes.Item2);
		}

		/// <summary>
		/// Returns a new shaped 2d-array without changing data.
		/// </summary>
		/// <returns>reshaped 2D array</returns>
		public LinqableArray2D<T> Reshape(int size0, int size1)
		{
			if (size0 < 0)
			{
				throw new ArgumentOutOfRangeException("size0");
			}
			if (size1 < 0)
			{
				throw new ArgumentOutOfRangeException("size1");
			}

			if (_lengths[0] * _lengths[1] != size0 * size1)
			{
				throw new ArgumentException("total size of new array must be unchanged", "size0, size1");
			}

			return ReshapeInPrivate(size0, size1);
		}

		//reliable size only
		private LinqableArray2D<T> ReshapeInPrivate(int size0, int size1)
		{
			var newArray = new LinqableArray2D<T>();
			newArray._items = new T[size0 * size1];
			newArray._lengths = new[] { size0, size1 };
			this._items.CopyTo(newArray._items, 0);
			return newArray;
		}
	}
}