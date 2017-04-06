// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Diagnostics.Contracts.Tests
{
    internal static class Utilities
    {
        internal static void AssertThrowsContractException(Action action)
        {
            try
            {
                action();
                NUnit.Framework.Assert.Fail("Did not throw");
            }
            catch (Exception exc)
            {
                NUnit.Framework.Assert.AreEqual("ContractException", exc.GetType().Name);
            }
        }

        internal static IDisposable WithContractFailed(EventHandler<ContractFailedEventArgs> handler)
        {
            Contract.ContractFailed += handler;
            return new UnregisterContractFailed { _handler = handler };
        }

        private class UnregisterContractFailed : IDisposable
        {
            internal EventHandler<ContractFailedEventArgs> _handler;

            public void Dispose()
            {
                Contract.ContractFailed -= _handler;
            }
        }
    }
}