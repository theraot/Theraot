﻿#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD12

using NUnit.Framework;
using System;
using System.IO;
using Theraot;
using Theraot.Core;

namespace Tests.Theraot.Threading
{
    internal class PathHelperTest
    {
        [Test]
        public void Combine()
        {
            Assert.AreEqual("a", PathHelper.Combine(new[] { "a", "" }), "Combine #08");
        }

        [Test]
        public void Combine_3Params()
        {
            var sep = Path.DirectorySeparatorChar.ToString();

            try
            {
                PathHelper.Combine(null, "two", "three");
                Assert.Fail("#A1-1");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", null, "three");
                Assert.Fail("#A1-2");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", null);
                Assert.Fail("#A1-3");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            Assert.AreEqual(StringEx.Join(sep, "one", "two", "three"), PathHelper.Combine("one", "two", "three"), "#A2-1");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three"), PathHelper.Combine($"{sep}one", "two", "three"),
                "#A2-2");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three"),
                PathHelper.Combine($"{sep}one{sep}", "two", "three"), "#A2-3");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}two", "three"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", "three"), "#A2-4");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}three"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three"), "#A2-5");

            Assert.AreEqual(StringEx.Join(sep, $"{sep}one{sep}", "two", "three"),
                PathHelper.Combine($"{sep}one{sep}{sep}", "two", "three"), "#A3");

            Assert.AreEqual("", PathHelper.Combine("", "", ""), "#A4");
        }

        [Test]
        public void Combine_4Params()
        {
            var sep = Path.DirectorySeparatorChar.ToString();

            try
            {
                PathHelper.Combine(null, "two", "three", "four");
                Assert.Fail("#A1-1");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", null, "three", "four");
                Assert.Fail("#A1-2");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", null, "four");
                Assert.Fail("#A1-3");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", "three", null);
                Assert.Fail("#A1-4");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            Assert.AreEqual(StringEx.Join(sep, "one", "two", "three", "four"),
                PathHelper.Combine("one", "two", "three", "four"), "#A2-1");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three", "four"),
                PathHelper.Combine($"{sep}one", "two", "three", "four"), "#A2-2");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three", "four"),
                PathHelper.Combine($"{sep}one{sep}", "two", "three", "four"), "#A2-3");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}two", "three", "four"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", "three", "four"), "#A2-4");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}three", "four"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three", "four"), "#A2-5");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}four"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three", $"{sep}four"), "#A2-6");

            Assert.AreEqual(StringEx.Join(sep, $"{sep}one{sep}", "two", "three", "four"),
                PathHelper.Combine($"{sep}one{sep}{sep}", "two", "three", "four"), "#A3");

            Assert.AreEqual("", PathHelper.Combine("", "", "", ""), "#A4");
        }

        [Test]
        public void Combine_Extra()
        {
            var source = new[] { "test", "/test", "\\test" };
            const string start = @"C:\test";
            // tested this on Linux and Windows using Path.Combine...
            foreach (var combination in source)
            {
                var result = PathHelper.Combine(start, combination);
                if
                (
                    combination.StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                    || combination.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                )
                {
                    Assert.AreEqual(combination, result);
                }
                else
                {
                    Assert.AreEqual(StringEx.Join(Path.DirectorySeparatorChar.ToString(), start, combination), result);
                }
            }
        }

        [Test]
        public void Combine_ManyParams()
        {
            var sep = Path.DirectorySeparatorChar.ToString();

            try
            {
                PathHelper.Combine(null, "two", "three", "four", "five");
                Assert.Fail("#A1-1");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", null, "three", "four", "five");
                Assert.Fail("#A1-2");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", null, "four", "five");
                Assert.Fail("#A1-3");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", "three", null, "five");
                Assert.Fail("#A1-4");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            try
            {
                PathHelper.Combine("one", "two", "three", "four", null);
                Assert.Fail("#A1-5");
            }
            catch (Exception exception)
            {
                No.Op(exception);
            }

            Assert.AreEqual(StringEx.Join(sep, "one", "two", "three", "four", "five"),
                PathHelper.Combine("one", "two", "three", "four", "five"), "#A2-1");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three", "four", "five"),
                PathHelper.Combine($"{sep}one", "two", "three", "four", "five"), "#A2-2");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}one", "two", "three", "four", "five"),
                PathHelper.Combine($"{sep}one{sep}", "two", "three", "four", "five"), "#A2-3");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}two", "three", "four", "five"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", "three", "four", "five"), "#A2-4");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}three", "four", "five"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three", "four", "five"), "#A2-5");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}four", "five"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three", $"{sep}four", "five"), "#A2-6");
            Assert.AreEqual(StringEx.Join(sep, $"{sep}five"),
                PathHelper.Combine($"{sep}one{sep}", $"{sep}two", $"{sep}three", $"{sep}four", $"{sep}five"), "#A2-6");

            Assert.AreEqual(StringEx.Join(sep, $"{sep}one{sep}", "two", "three", "four", "five"),
                PathHelper.Combine($"{sep}one{sep}{sep}", "two", "three", "four", "five"), "#A3");

            Assert.AreEqual("", PathHelper.Combine("", "", "", "", ""), "#A4");
        }
    }
}

#endif