using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using MasuqatNet.Collections;

namespace MasuqatNet.Collections.Tests
{
	[TestClass]
	public class LinqableArray2DTests
	{
		[TestMethod]
		[TestCategory("Ctor"), TestCategory("Fail")]
		public void CtorWithRange_ArgumentsInvalidRange_Test()
		{
			try
			{
				var array = new LinqableArray2D<int>(-1, 4);
				Assert.Fail("An exception should have been thrown.");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("length0", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				var array = new LinqableArray2D<int>(4, -1);
				Assert.Fail("An exception should have been thrown.");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("length1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				var array = new LinqableArray2D<int>(-1, -1);
				Assert.Fail("An exception should have been thrown.");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("length0", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		[TestCategory("Ctor")]
		public void CtorWith2DRectangularArray_Test()
		{
			var rectangularArray = new int[,] {	{ 0, 1 },
												{ 2, 3 },
												{ 4, 5 }};
			LinqableArray2D<int> array = new LinqableArray2D<int>(rectangularArray);

			Assert.AreEqual(array[0, 0], 0);
			Assert.AreEqual(array[0, 1], 1);
			Assert.AreEqual(array[1, 0], 2);
			Assert.AreEqual(array[1, 1], 3);
			Assert.AreEqual(array[2, 0], 4);
			Assert.AreEqual(array[2, 1], 5);
		}

		[TestMethod]
		[TestCategory("Ctor"), TestCategory("Fail")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CtorWith2DRectangularArray_ArgumentNull_Test()
		{
			var array = new LinqableArray2D<int>(rectangularArray: null);
		}

		[TestMethod]
		[TestCategory("Ctor")]
		public void CtorWithJuggedArray_Test()
		{
			var juggedArray = new int[][] { new int[] { 0, 1 },
											new int[] { 2, 3 },
											new int[] { 4, 5 }};
			LinqableArray2D<int> array = new LinqableArray2D<int>(juggedArray);

			Assert.AreEqual(array[0, 0], 0);
			Assert.AreEqual(array[0, 1], 1);
			Assert.AreEqual(array[1, 0], 2);
			Assert.AreEqual(array[1, 1], 3);
			Assert.AreEqual(array[2, 0], 4);
			Assert.AreEqual(array[2, 1], 5);
		}

		[TestMethod]
		[TestCategory("Ctor"), TestCategory("Fail")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CtorWith2DJuggedArray_ArgumentNull_Test()
		{
			var array = new LinqableArray2D<int>(juggedArray: null);
		}

		[TestMethod]
		[TestCategory("Ctor"), TestCategory("Fail")]
		[ExpectedException(typeof(ArgumentException))]
		public void CtorWith2DJuggedArray_ArgumentUnflatted_Test()
		{
			var juggedArray = new int[][] { new int[] { 0, 1 },
											new int[] { 2, 3 },
											new int[] { 4, 5, 6 }};
			var array = new LinqableArray2D<int>(juggedArray);
		}

		[TestMethod]
		public void DataOrder_And_Indexer_Test()
		{
			var actualArray = new[] { 0, 1, 2, 3, 4, 5 };

			LinqableArray2D<int> array = new LinqableArray2D<int>(3, 2);

			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			PrivateObject po = new PrivateObject(array);
			var items = (int[])po.GetField("_items");

			Assert.IsTrue(actualArray.SequenceEqual(items));


			array = new LinqableArray2D<int>(3, 2);
			po = new PrivateObject(array);
			po.SetField("_items", actualArray);

			Assert.AreEqual(array[0, 0], 0);
			Assert.AreEqual(array[0, 1], 1);
			Assert.AreEqual(array[1, 0], 2);
			Assert.AreEqual(array[1, 1], 3);
			Assert.AreEqual(array[2, 0], 4);
			Assert.AreEqual(array[2, 1], 5);
		}

		[TestMethod]
		public void IndexOf_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			var retIn = array.IndexOf(3);
			Assert.IsTrue(new Tuple<int, int>(1, 1).Equals(retIn));

			var retOut = array.IndexOf(99);
			Assert.IsTrue(new Tuple<int, int>(-1, -1).Equals(retOut));
		}

		[TestMethod]
		public void GetAll1DEnumerables_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			//reverseItem = false, bool reverseEnumerate = false
			using (var e = array.GetAll1DEnumerables(0, reverseItem: false, reverseEnumerate: false).GetEnumerator())
			{
				for (int i = 0; e.MoveNext(); i++)
				{
					Assert.IsTrue(Enumerable.Range(i * 2, 2).SequenceEqual(e.Current));
				}
			}
			using (var e = array.GetAll1DEnumerables(1, reverseItem: false, reverseEnumerate: false).GetEnumerator())
			{
				for (int i = 0; e.MoveNext(); i++)
				{
					Assert.IsTrue(Enumerable.Range(0, 3).Select(x => 2 * x + i).SequenceEqual(e.Current));
				}
			}

			//reverseItem = true, bool reverseEnumerate = false
			using (var e = array.GetAll1DEnumerables(0, reverseItem: true, reverseEnumerate: false).GetEnumerator())
			{
				for (int i = 0; e.MoveNext(); i++)
				{
					Assert.IsTrue(Enumerable.Range(i * 2, 2).Reverse().SequenceEqual(e.Current));
				}
			}
			using (var e = array.GetAll1DEnumerables(1, reverseItem: true, reverseEnumerate: false).GetEnumerator())
			{
				for (int i = 0; e.MoveNext(); i++)
				{
					Assert.IsTrue(Enumerable.Range(0, 3).Select(x => 2 * x + i).Reverse().SequenceEqual(e.Current));
				}
			}

			//reverseItem = false, bool reverseEnumerate = true
			using (var e = array.GetAll1DEnumerables(0, reverseItem: false, reverseEnumerate: true).GetEnumerator())
			{
				for (int i = 2; e.MoveNext(); i--)
				{
					Assert.IsTrue(Enumerable.Range(i * 2, 2).SequenceEqual(e.Current));
				}
			}
			using (var e = array.GetAll1DEnumerables(1, reverseItem: false, reverseEnumerate: true).GetEnumerator())
			{
				for (int i = 1; e.MoveNext(); i--)
				{
					Assert.IsTrue(Enumerable.Range(0, 3).Select(x => 2 * x + i).SequenceEqual(e.Current));
				}
			}

			//reverseItem = true, bool reverseEnumerate = true
			using (var e = array.GetAll1DEnumerables(0, reverseItem: true, reverseEnumerate: true).GetEnumerator())
			{
				for (int i = 2; e.MoveNext(); i--)
				{
					Assert.IsTrue(Enumerable.Range(i * 2, 2).Reverse().SequenceEqual(e.Current));
				}
			}
			using (var e = array.GetAll1DEnumerables(1, reverseItem: true, reverseEnumerate: true).GetEnumerator())
			{
				for (int i = 1; e.MoveNext(); i--)
				{
					Assert.IsTrue(Enumerable.Range(0, 3).Select(x => 2 * x + i).Reverse().SequenceEqual(e.Current));
				}
			}
		}

		[TestMethod]
		public void ToArray2D_Test()
		{
			var linqableArray = new LinqableArray2D<int>(3, 2);
			linqableArray[0, 0] = 0;
			linqableArray[0, 1] = 1;
			linqableArray[1, 0] = 2;
			linqableArray[1, 1] = 3;
			linqableArray[2, 0] = 4;
			linqableArray[2, 1] = 5;

			var array2D = linqableArray.ToArray2D();
			Assert.AreEqual(array2D[0, 0], 0);
			Assert.AreEqual(array2D[0, 1], 1);
			Assert.AreEqual(array2D[1, 0], 2);
			Assert.AreEqual(array2D[1, 1], 3);
			Assert.AreEqual(array2D[2, 0], 4);
			Assert.AreEqual(array2D[2, 1], 5);
		}

		[TestMethod]
		public void ToJuggedArray2D_Test()
		{
			var linqableArray = new LinqableArray2D<int>(3, 2);
			linqableArray[0, 0] = 0;
			linqableArray[0, 1] = 1;
			linqableArray[1, 0] = 2;
			linqableArray[1, 1] = 3;
			linqableArray[2, 0] = 4;
			linqableArray[2, 1] = 5;

			var array2D = linqableArray.ToJuggedArray2D();
			Assert.AreEqual(array2D[0][0], 0);
			Assert.AreEqual(array2D[0][1], 1);
			Assert.AreEqual(array2D[1][0], 2);
			Assert.AreEqual(array2D[1][1], 3);
			Assert.AreEqual(array2D[2][0], 4);
			Assert.AreEqual(array2D[2][1], 5);
		}

		[TestMethod]
		public void GetLength_Test()
		{
			var array = new LinqableArray2D<int>(1, 2);
			Assert.AreEqual(array.GetLength(0), 1);
			Assert.AreEqual(array.GetLength(1), 2);
		}

		[TestMethod]
		[TestCategory("Fail")]
		public void GetLength_ArgumentInvalidRange_Test()
		{
			var array = new LinqableArray2D<int>(1, 2);

			try
			{
				array.GetLength(-1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("dimension", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.GetLength(2);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("dimension", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public void ReshapeWithTuple_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			var reshapedArray = array.Reshape(Tuple.Create(1, 6));

			Assert.AreEqual(reshapedArray.GetLength(0), 1);
			Assert.AreEqual(reshapedArray.GetLength(1), 6);

			Assert.AreEqual(reshapedArray[0, 0], 0);
			Assert.AreEqual(reshapedArray[0, 1], 1);
			Assert.AreEqual(reshapedArray[0, 2], 2);
			Assert.AreEqual(reshapedArray[0, 3], 3);
			Assert.AreEqual(reshapedArray[0, 4], 4);
			Assert.AreEqual(reshapedArray[0, 5], 5);
		}

		[TestMethod]
		[TestCategory("Fail")]
		public void ReshapeWithTuple_ArgumentError_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			try
			{
				array.Reshape(null);
			}
			catch (ArgumentNullException ex)
			{
				Assert.AreEqual("sizes", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.Reshape(Tuple.Create(-1, 6));
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("sizes.Item1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.Reshape(Tuple.Create(6, -1));
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("sizes.Item2", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.Reshape(Tuple.Create(4, 5));
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("sizes", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public void ReshapeWithRange_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			var reshapedArray = array.Reshape(1, 6);

			Assert.AreEqual(reshapedArray.GetLength(0), 1);
			Assert.AreEqual(reshapedArray.GetLength(1), 6);

			Assert.AreEqual(reshapedArray[0, 0], 0);
			Assert.AreEqual(reshapedArray[0, 1], 1);
			Assert.AreEqual(reshapedArray[0, 2], 2);
			Assert.AreEqual(reshapedArray[0, 3], 3);
			Assert.AreEqual(reshapedArray[0, 4], 4);
			Assert.AreEqual(reshapedArray[0, 5], 5);
		}

		[TestMethod]
		[TestCategory("Fail")]
		public void ReshapeWithRange_ArgumentError_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 0;
			array[0, 1] = 1;
			array[1, 0] = 2;
			array[1, 1] = 3;
			array[2, 0] = 4;
			array[2, 1] = 5;

			try
			{
				array.Reshape(-1, 6);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("size0", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.Reshape(6, -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("size1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			try
			{
				array.Reshape(4, 5);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("size0, size1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public void CopyTo2DRectangularArray_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 10;
			array[0, 1] = 11;
			array[1, 0] = 12;
			array[1, 1] = 13;
			array[2, 0] = 14;
			array[2, 1] = 15;

			int[,] dest;


			dest = new int[4, 3];
			array.CopyTo(dest, 0, 0);

			Assert.AreEqual(dest[0, 0], 10);
			Assert.AreEqual(dest[0, 1], 11);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 12);
			Assert.AreEqual(dest[1, 1], 13);
			Assert.AreEqual(dest[1, 2], 0);
			Assert.AreEqual(dest[2, 0], 14);
			Assert.AreEqual(dest[2, 1], 15);
			Assert.AreEqual(dest[2, 2], 0);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 0);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new int[4, 3];
			array.CopyTo(dest, 1, 0);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 0);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 10);
			Assert.AreEqual(dest[1, 1], 11);
			Assert.AreEqual(dest[1, 2], 0);
			Assert.AreEqual(dest[2, 0], 12);
			Assert.AreEqual(dest[2, 1], 13);
			Assert.AreEqual(dest[2, 2], 0);
			Assert.AreEqual(dest[3, 0], 14);
			Assert.AreEqual(dest[3, 1], 15);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new int[4, 3];
			array.CopyTo(dest, 0, 1);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 10);
			Assert.AreEqual(dest[0, 2], 11);
			Assert.AreEqual(dest[1, 0], 0);
			Assert.AreEqual(dest[1, 1], 12);
			Assert.AreEqual(dest[1, 2], 13);
			Assert.AreEqual(dest[2, 0], 0);
			Assert.AreEqual(dest[2, 1], 14);
			Assert.AreEqual(dest[2, 2], 15);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 0);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new int[4, 3];
			array.CopyTo(dest, 1, 1);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 0);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 0);
			Assert.AreEqual(dest[1, 1], 10);
			Assert.AreEqual(dest[1, 2], 11);
			Assert.AreEqual(dest[2, 0], 0);
			Assert.AreEqual(dest[2, 1], 12);
			Assert.AreEqual(dest[2, 2], 13);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 14);
			Assert.AreEqual(dest[3, 2], 15);
		}

		[TestMethod]
		[TestCategory("Fail")]
		public void CopyTo2DRectangularArray_ArgumentError_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);

			try
			{
				array.CopyTo(rectangularArray: null);
			}
			catch (ArgumentNullException ex)
			{
				Assert.AreEqual("rectangularArray", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var emptyDest = new int[0, 0];
			try
			{
				array.CopyTo(emptyDest, index0: -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("index0", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			try
			{
				array.CopyTo(emptyDest, index1: -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("index1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest22 = new int[2, 2];
			try
			{
				array.CopyTo(dest22);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 0.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest31 = new int[3, 1];
			try
			{
				array.CopyTo(dest31);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 1.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest32 = new int[3, 2];
			try
			{
				array.CopyTo(dest32, index0: 1);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 0.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			try
			{
				array.CopyTo(dest32, index1: 1);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 1.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod]
		public void CopyTo2DLinqableArray_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);
			array[0, 0] = 10;
			array[0, 1] = 11;
			array[1, 0] = 12;
			array[1, 1] = 13;
			array[2, 0] = 14;
			array[2, 1] = 15;

			LinqableArray2D<int> dest;


			dest = new LinqableArray2D<int>(4, 3);
			array.CopyTo(dest, 0, 0);

			Assert.AreEqual(dest[0, 0], 10);
			Assert.AreEqual(dest[0, 1], 11);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 12);
			Assert.AreEqual(dest[1, 1], 13);
			Assert.AreEqual(dest[1, 2], 0);
			Assert.AreEqual(dest[2, 0], 14);
			Assert.AreEqual(dest[2, 1], 15);
			Assert.AreEqual(dest[2, 2], 0);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 0);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new LinqableArray2D<int>(4, 3);
			array.CopyTo(dest, 1, 0);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 0);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 10);
			Assert.AreEqual(dest[1, 1], 11);
			Assert.AreEqual(dest[1, 2], 0);
			Assert.AreEqual(dest[2, 0], 12);
			Assert.AreEqual(dest[2, 1], 13);
			Assert.AreEqual(dest[2, 2], 0);
			Assert.AreEqual(dest[3, 0], 14);
			Assert.AreEqual(dest[3, 1], 15);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new LinqableArray2D<int>(4, 3);
			array.CopyTo(dest, 0, 1);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 10);
			Assert.AreEqual(dest[0, 2], 11);
			Assert.AreEqual(dest[1, 0], 0);
			Assert.AreEqual(dest[1, 1], 12);
			Assert.AreEqual(dest[1, 2], 13);
			Assert.AreEqual(dest[2, 0], 0);
			Assert.AreEqual(dest[2, 1], 14);
			Assert.AreEqual(dest[2, 2], 15);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 0);
			Assert.AreEqual(dest[3, 2], 0);


			dest = new LinqableArray2D<int>(4, 3);
			array.CopyTo(dest, 1, 1);

			Assert.AreEqual(dest[0, 0], 0);
			Assert.AreEqual(dest[0, 1], 0);
			Assert.AreEqual(dest[0, 2], 0);
			Assert.AreEqual(dest[1, 0], 0);
			Assert.AreEqual(dest[1, 1], 10);
			Assert.AreEqual(dest[1, 2], 11);
			Assert.AreEqual(dest[2, 0], 0);
			Assert.AreEqual(dest[2, 1], 12);
			Assert.AreEqual(dest[2, 2], 13);
			Assert.AreEqual(dest[3, 0], 0);
			Assert.AreEqual(dest[3, 1], 14);
			Assert.AreEqual(dest[3, 2], 15);
		}

		[TestMethod]
		[TestCategory("Fail")]
		public void CopyTo2DLinqableArray_ArgumentError_Test()
		{
			var array = new LinqableArray2D<int>(3, 2);

			try
			{
				array.CopyTo(linqableArray: null);
			}
			catch (ArgumentNullException ex)
			{
				Assert.AreEqual("linqableArray", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var emptyDest = new LinqableArray2D<int>(0, 0);
			try
			{
				array.CopyTo(emptyDest, index0: -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("index0", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			try
			{
				array.CopyTo(emptyDest, index1: -1);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Assert.AreEqual("index1", ex.ParamName);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest22 = new LinqableArray2D<int>(2, 2);
			try
			{
				array.CopyTo(dest22);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 0.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest31 = new LinqableArray2D<int>(3, 1);
			try
			{
				array.CopyTo(dest31);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 1.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			var dest32 = new LinqableArray2D<int>(3, 2);
			try
			{
				array.CopyTo(dest32, index0: 1);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 0.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			try
			{
				array.CopyTo(dest32, index1: 1);
			}
			catch (ArgumentException ex)
			{
				Assert.AreEqual("Out of array size with dimension 1.", ex.Message);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}
