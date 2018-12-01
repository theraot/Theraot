// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Security.Permissions;

namespace System.Diagnostics.Contracts.Tests
{
    [TestFixture]
    public static class AssertTests
    {
        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
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
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
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
                Assert.False(eventRaised, "ContractFailed event not raised");
#endif
            }
        }

        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
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
                Assert.False(eventRaised, "ContractFailed event was raised");
#endif
            }
        }
    }
}