// Needed for Workaround

using System.Runtime.CompilerServices;
using System.Text;

namespace Theraot.Core
{
    public static class StringBuilderExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void Clear(this StringBuilder stringBuilder)
        {
            stringBuilder.Length = 0;
        }
    }
}