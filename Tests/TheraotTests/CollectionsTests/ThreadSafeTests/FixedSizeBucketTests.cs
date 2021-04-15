#if TARGETS_NET || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Theraot.Collections.ThreadSafe;

namespace Tests.TheraotTests.CollectionsTests.ThreadSafeTests
{
    [TestFixture]
    public static class FixedSizeBucketTests
    {
        [Test]
        public static void SerializationWorks()
        {
            var random = new Random();
            var values = new List<int>();
            var bucket = new FixedSizeBucket<int>(1000);
            FixedSizeBucket<int> result;

            for (int i = 0; i < 1000; i++)
            {
                var value = random.Next();
                values.Add(value);
                bucket.Set(i, value);
            }

            var stream = new MemoryStream();

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, bucket);
            }
            catch (SerializationException e)
            {
                Console.WriteLine($"Failed to serialize. Reason: {e.Message}");
                throw;
            }

            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                result = (FixedSizeBucket<int>)formatter.Deserialize(stream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine($"Failed to deserialize. Reason: {e.Message}");
                throw;
            }

            var index = 0;
            foreach (var item in result)
            {
                Assert.AreEqual(values[index], item);
                index++;
            }
        }
    }
}

#endif