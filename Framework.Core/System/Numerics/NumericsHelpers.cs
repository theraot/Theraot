#if LESSTHAN_NET40

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Numerics
{
    internal static class NumericsHelpers
    {
        public static uint CombineHash(uint u1, uint u2)
        {
            return ((u1 << 7) | (u1 >> 25)) ^ u2;
        }

        public static int CombineHash(int n1, int n2)
        {
            return (int)CombineHash((uint)n1, (uint)n2);
        }

        // Do an in-place two's complement. "Dangerous" because it causes
        // a mutation and needs to be used with care for immutable types.
        public static void DangerousMakeTwosComplement(uint[] d)
        {
            if (d?.Length > 0)
            {
                d[0] = ~d[0] + 1;

                var i = 1;
                // first do complement and +1 as long as carry is needed
                for (; d[i - 1] == 0 && i < d.Length; i++)
                {
                    ref var current = ref d[i];
                    current = ~current + 1;
                }

                // now ones complement is sufficient
                for (; i < d.Length; i++)
                {
                    ref var current = ref d[i];
                    current = ~current;
                }
            }
        }
    }
}

#endif