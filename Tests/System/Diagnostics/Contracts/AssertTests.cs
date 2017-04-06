// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace System.Diagnostics.Contracts.Tests
{
    [TestFixture]
    public class AssertTests
    {
        [Test]
        public static void AssertTrueDoesNotRaiseEvent()
        {
            var eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetHandled();
            };
            using (Utilities.WithContractFailed(handler))
            {
                Contract.Assert(true);
                Assert.False(eventRaised, "ContractFailed event was raised");
            }
        }

        [Test]
        public static void AssertFalseRaisesEvent()
        {
            var eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetHandled();
            };
            using (Utilities.WithContractFailed(handler))
            {
                Contract.Assert(false);
#if DEBUG
                Assert.True(eventRaised, "ContractFailed event not raised");
#else
                NUnit.Framework.Assert.False(eventRaised, "ContractFailed event not raised");
#endif
            }
        }

        [Test]
        public static void AssertFalseThrows()
        {
            var eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetUnwind();
            };
            using (Utilities.WithContractFailed(handler))
            {
#if DEBUG
                Utilities.AssertThrowsContractException(() => Contract.Assert(false, "Some kind of user message"));
                Assert.True(eventRaised, "ContractFailed was not raised");
#else
                Contract.Assert(false, "Some kind of user message");
                NUnit.Framework.Assert.False(eventRaised, "ContractFailed event was raised");
#endif
            }
        }
    }
}