// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;
using System.Security.Permissions;
using NUnit.Framework;

namespace Tests.Helpers
{
    internal static class Utilities
    {
        internal static void AssertThrowsContractException(Action action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                action();
                Assert.Fail("Did not throw");
            }
            catch (Exception exc)
            {
                Assert.AreEqual("ContractException", exc.GetType().Name);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        internal static IDisposable WithContractFailed(EventHandler<ContractFailedEventArgs> handler)
        {
            Contract.ContractFailed += handler;
            return new UnregisterContractFailed {Handler = handler};
        }

        private class UnregisterContractFailed : IDisposable
        {
            internal EventHandler<ContractFailedEventArgs> Handler;

            public void Dispose()
            {
                Contract.ContractFailed -= Handler;
            }
        }
    }
}