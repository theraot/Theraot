// Needed for NET35 (BigInteger)

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

        private static readonly char[] _newLine =
        {
            '\u000A' /*NewLine*/,
            '\u000D' /*Carriage return*/,
        };

        public static char[] GetClassicWhitespace()
        {
            return _classicWhitespace;
        }

        public static char[] GetNewLine()
        {
            return _newLine;
        }

        public static bool IsClassicWhitespace(char character)
        {
            return Array.IndexOf(_classicWhitespace, character) >= 0;
        }

        public static bool IsNewLine(char character)
        {
            return Array.IndexOf(_newLine, character) >= 0;
        }
    }
}