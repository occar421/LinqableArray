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
	}
}
