using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using MasuqatNet.Collections;

namespace LinqableArray.Tests
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
				Assert.AreEqual("length1", ex.ParamName);
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
				Assert.AreEqual("length2", ex.ParamName);
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
				Assert.AreEqual("length1", ex.ParamName);
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
	}
}
