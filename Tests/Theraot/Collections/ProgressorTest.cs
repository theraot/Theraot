using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Theraot.Collections;

namespace Tests.Theraot.Collections
{
    [TestFixture]
    public class ProgressorTest
    {
        [Test]
        public void EnumerableProgressor()
        {
            var source = new [] {0, 1, 2, 3, 4, 5};
            var progresor = new Progressor<int>(source);
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void TryTakeProgressor()
        {
            var source = new Queue<int>();
            source.Enqueue(0);
            source.Enqueue(1);
            source.Enqueue(2);
            source.Enqueue(3);
            source.Enqueue(4);
            source.Enqueue(5);
            var progresor = new Progressor<int>
            (
                (out int value) =>
                {
                    try
                    {
                        value = source.Dequeue();
                        return true;
                    }
                    catch (Exception)
                    {
                        value = 0;
                        return false;
                    }
                }
            );
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }
        
        [Test]
        public void ProgressorProgressor()
        {
            var source = new Progressor<int>(new[] {0, 1, 2, 3, 4, 5});
            var progresor = new Progressor<int>(source);
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void ObservableProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progresor = new Progressor<int>((source as IObservable<int>));
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            source.AsEnumerable().Consume();
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void ConvertedProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 1, 2, 3, 4, 5 });
            var progresor = Progressor<string>.CreateConverted(source, input => input.ToString(CultureInfo.InvariantCulture));
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB.ToString(CultureInfo.InvariantCulture));
                    indexB++;
                }
            );
            string item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA.ToString(CultureInfo.InvariantCulture));
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void FilteredProgressor()
        {
            var source = new Progressor<int>(new[] { 0, 8, 1, 8, 2, 3, 4, 8, 5 });
            var progresor = Progressor<int>.CreatedFiltered(source, input => input != 8);
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB);
                    indexB++;
                }
            );
            int item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA);
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }

        [Test]
        public void CreatedFilteredConverted()
        {
            var source = new Progressor<int>(new[] { 0, 8, 1, 8, 2, 3, 4, 8, 5 });
            var progresor = Progressor<string>.CreatedFilteredConverted(source, input => input != 8, input => input.ToString(CultureInfo.InvariantCulture));
            int indexA = 0;
            int indexB = 0;
            progresor.SubscribeAction
            (
                value =>
                {
                    Assert.AreEqual(value, indexB.ToString(CultureInfo.InvariantCulture));
                    indexB++;
                }
            );
            string item;
            while (progresor.TryTake(out item))
            {
                Assert.AreEqual(item, indexA.ToString(CultureInfo.InvariantCulture));
                indexA++;
            }
            Assert.AreEqual(indexA, 6);
            Assert.AreEqual(indexA, indexB);
        }
    }
}
