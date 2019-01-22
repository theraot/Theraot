using System;

namespace Theraot.Core
{
    public static class CharHelper
    {
        private static readonly char[] _classicWhitespace =
        {
            '\u0009' /*Tab*/,
            '\u000A' /*NewLine*/,
            '\u000B' /*Vertical Tab*/,
            '\u000C' /*Form Feed*/,
            '\u000D' /*Carriage return*/,
            '\u0020' /*Space*/
        };

        public static bool IsClassicWhitespace(char character)
        {
            return Array.IndexOf(_classicWhitespace, character) >= 0;
        }
    }
}