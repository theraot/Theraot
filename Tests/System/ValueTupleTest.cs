using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

#if NET20 || NET30 || NET35 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4

namespace Tests.System
{
	[TestFixture]
	class ValueTupleTest
	{
		[Test]
		public void SameHashCode()
		{
			var a = ("abc", "cde");
			var b = ("abc", "cde");
			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
		}

		[Test]
		public void Equals()
		{
			var a = ("abc", "cde");
			var b = ("abc", "cde");
			Assert.AreEqual(a, b);
		}
	}
}

#endif