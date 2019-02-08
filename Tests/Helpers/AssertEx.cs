using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Tests.Helpers
{
    public static class AssertEx
    {
        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void AreEqual<T>(T expected, T actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Throws<TException>(Func<object> code)
            where TException : Exception
        {
            Assert.Throws<TException>(() => code());
        }
    }
}
