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
            bool eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetHandled();
            };
            using (Utilities.WithContractFailed(handler))
            {
                Contract.Assert(true);
                NUnit.Framework.Assert.False(eventRaised, "ContractFailed event was raised");
            }
        }

        [Test]
        public static void AssertFalseRaisesEvent()
        {
            bool eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetHandled();
            };
            using (Utilities.WithContractFailed(handler))
            {
                Contract.Assert(false);
                NUnit.Framework.Assert.True(eventRaised, "ContractFailed event not raised");
            }
        }

        [Test]
        public static void AssertFalseThrows()
        {
            bool eventRaised = false;
            EventHandler<ContractFailedEventArgs> handler = (s, e) =>
            {
                eventRaised = true;
                e.SetUnwind();
            };
            using (Utilities.WithContractFailed(handler))
            {
#if DEBUG
                Utilities.AssertThrowsContractException(() => Contract.Assert(false, "Some kind of user message"));
                NUnit.Framework.Assert.True(eventRaised, "ContractFailed was not raised");
#else
                Contract.Assert(false, "Some kind of user message");
                NUnit.Framework.Assert.False(eventRaised, "ContractFailed event was raised");
#endif
            }
        }
    }
}