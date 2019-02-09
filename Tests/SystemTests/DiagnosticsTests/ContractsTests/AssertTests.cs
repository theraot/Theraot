// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Contracts;
using System.Diagnostics.Contracts.Tests;
using System.Security.Permissions;
using NUnit.Framework;

namespace Tests.SystemTests.DiagnosticsTests.ContractsTests
{
    [TestFixture]
    public static class AssertTests
    {
        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static void AssertFalseRaisesEvent()
        {
            var eventRaised = false;

            void Handler(object s, ContractFailedEventArgs e)
            {
                eventRaised = true;
                e.SetHandled();
            }

            using (Utilities.WithContractFailed(Handler))
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

            void Handler(object s, ContractFailedEventArgs e)
            {
                eventRaised = true;
                e.SetUnwind();
            }

            using (Utilities.WithContractFailed(Handler))
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

        [Test]
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public static void AssertTrueDoesNotRaiseEvent()
        {
            var eventRaised = false;

            void Handler(object s, ContractFailedEventArgs e)
            {
                eventRaised = true;
                e.SetHandled();
            }

            using (Utilities.WithContractFailed(Handler))
            {
                Contract.Assert(true);
                Assert.False(eventRaised, "ContractFailed event was raised");
            }
        }
    }
}