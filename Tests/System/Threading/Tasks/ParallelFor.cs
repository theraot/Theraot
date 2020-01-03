// BASEDON: https://github.com/dotnet/corefx/blob/7ae1a252d7e68c5513d2658de7a401c37e9b0504/src/System.Threading.Tasks.Parallel/tests/ParallelFor.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace System.Threading.Tasks.Tests
{
    public static class ParallelForUnitTests
    {
        [Test]
        [TestCase(Api.For64, StartIndexBase.Int32, 0, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For64, StartIndexBase.Int32, 10, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.For64, StartIndexBase.Int32, 10, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For64, StartIndexBase.Int32, 1, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.For64, StartIndexBase.Int32, 1, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.For64, StartIndexBase.Int32, 2, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For64, StartIndexBase.Int32, 2, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.For64, StartIndexBase.Int32, 97, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.For64, StartIndexBase.Int32, 97, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 0, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.For, StartIndexBase.Zero, 10, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 1, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 1, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.For, StartIndexBase.Zero, 2, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 2, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.For, StartIndexBase.Zero, 97, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.For, StartIndexBase.Zero, 97, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 0, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 10, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 1, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 1, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 2, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 2, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 97, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnArray, StartIndexBase.Zero, 97, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 0, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 10, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 1, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 1, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 2, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 2, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 97, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.ForeachOnList, StartIndexBase.Zero, 97, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 0, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 10, WithParallelOption.None, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 10, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 1, WithParallelOption.None, ActionWithState.None, ActionWithLocal.None)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 1, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 2, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 2, WithParallelOption.WithDop, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 2, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 97, WithParallelOption.None, ActionWithState.None, ActionWithLocal.HasFinally)]
        [TestCase(Api.Foreach, StartIndexBase.Zero, 97, WithParallelOption.WithDop, ActionWithState.Stop, ActionWithLocal.None)]
        public static void ParallelFor(Api api, StartIndexBase startIndexBase, int count, WithParallelOption parallelOption, ActionWithState stateOption, ActionWithLocal localOption)
        {
            var parameters = new TestParameters(api, startIndexBase)
            {
                Count = count,
                ParallelOption = parallelOption,
                StateOption = stateOption,
                LocalOption = localOption
            };
            var test = new ParallelForTest(parameters);
            test.RealRun();
        }
    }
}