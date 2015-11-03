using System;
using System.Collections.Generic;
using Theraot.Collections;

namespace Theraot.Core
{
    /// <summary>
    /// A class to extract information from strings.
    /// </summary>
    /// <remarks>
    /// An alternative for StringReader for all your parsing needs.
    /// StringProcessor is NOT thread safe. Do not share intances without locking.
    /// </remarks>
    [Serializable]
    public class StringProcessor
    {
        // Note: it is assumed that _length = _string.Length - if this weren't true extra check would be needed in every case of IndexOf or IndexOfAny
        private readonly int _length;

        private readonly string _string;
        private int _position;

        /// <summary>
        /// Creates a new instance of StringProcessor.
        /// </summary>
        /// <param name="str">The string to process.</param>
        /// <exception cref="ArgumentNullException">The string is null.</exception>
        public StringProcessor(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str", "The string is null.");
            }
            _string = str;
            _length = str.Length;
            _position = 0;
        }

        /// <summary>
        /// Gets the number of characters yet to process.
        /// </summary>
        public int Count
        {
            get
            {
                return _length - _position;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the current position is at the end of the string.
        /// </summary>
        public bool EndOfString
        {
            get
            {
                return _position == _length;
            }
        }

        /// <summary>
        /// Gets or sets the current position withing the underlying string.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The position must be greater than zero and less or equal to the length of the underlying string.</exception>
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value > 0 && value <= _length)
                {
                    _position = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "The position must be greater than zero and less or equal to the length of the underlying string.");
                }
            }
        }

        /// <summary>
        /// Gets the underlying string.
        /// </summary>
        public string String
        {
            get
            {
                return _string;
            }
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The string to look for. The delimiter.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntil(out string found, string target)
        {
            return ExtractUntil(out found, target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The string to look for. The delimiter.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntil(out string found, string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    found = PrivateReadToPosition(position);
                    return true;
                }
            }
            found = null;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The character to look for. The delimiter.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntil(out string found, char target)
        {
            var position = _string.IndexOf(target, _position);
            if (position != -1)
            {
                found = PrivateReadToPosition(position);
                return true;
            }
            found = null;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The string to look for. The delimiter.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntilAfter(out string found, string target)
        {
            return ExtractUntilAfter(out found, target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The string to look for. The delimiter.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntilAfter(out string found, string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    found = PrivateReadToPosition(position + target.Length);
                    return true;
                }
            }
            found = null;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found.
        /// </summary>
        /// <param name="found">The found string.</param>
        /// <param name="target">The character to look for. The delimiter.</param>
        /// <returns><c>true</c> if the target was found; otherwise <c>false</c></returns>
        public bool ExtractUntilAfter(out string found, char target)
        {
            var position = _string.IndexOf(target, _position);
            if (position != -1)
            {
                found = PrivateReadToPosition(position + 1);
                return true;
            }
            found = null;
            return false;
        }

        /// <summary>
        /// Reads the next character from the underlying string.
        /// </summary>
        /// <returns>The next character from the underlying string, or -1 if no more characters are available.</returns>
        public int Peek()
        {
            if (_position == _length)
            {
                return -1;
            }
            var character = _string[_position];
            return character;
        }

        /// <summary>
        /// Checks if the next characters from underlying string matches the input string.
        /// </summary>
        /// <param name="target">The string to check against.</param>
        /// <returns>
        ///   <c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool Peek(string target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            var length = target.Length;
            if (_position + length <= _length)
            {
                var result = _string.Substring(_position, length);
                if (result == target)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the next character from the string.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Reached the end of the string.</exception>
        /// <returns>
        ///   <c>true</c> if the character was recovered; otherwise, <c>false</c>.
        /// </returns>
        public char PeekChar()
        {
            if (_position == _length)
            {
                throw new IndexOutOfRangeException("Reached the end of the string.");
            }
            var character = _string[_position];
            return character;
        }

        /// <summary>
        /// Checks if the next character from underlying string matches the input character.
        /// </summary>
        /// <param name="target">The character to check against.</param>
        /// <returns>
        ///   <c>true</c> if the characters match; otherwise, <c>false</c>.
        /// </returns>
        public bool Peek(char target)
        {
            if (_position == _length)
            {
                return false;
            }
            var result = _string[_position];
            if (result == target)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the next character from theunderlying string passes the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns>
        ///   <c>true</c> if the sucessful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public bool Peek(Func<char, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate", "The predicate is null.");
            }
            if (_position == _length)
            {
                return false;
            }
            var character = _string[_position];
            if (predicate(character))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the next character from theunderlying string. If sucessful advances the character position by one character.
        /// </summary>
        /// <returns>The next character from the underlying string, or -1 if no more characters are available.</returns>
        public int Read()
        {
            if (_position == _length)
            {
                return -1;
            }
            var character = _string[_position];
            _position++;
            return character;
        }

        /// <summary>Reads a block of characters from the input string and advances the character position by <paramref name="count" />.</summary>
        /// <returns>The total number of characters read into the buffer. This can be less than the number of characters requested if that many characters are not currently available, or zero if the end of the underlying string has been reached.</returns>
        /// <param name="destination">When this method returns, contains the specified character array with the values between <paramref name="destinationIndex" /> and (<paramref name="destinationIndex" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source. </param>
        /// <param name="destinationIndex">The starting index in the buffer. </param>
        /// <param name="count">The number of characters to read. </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="destination" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="destinationIndex" /> is less than <paramref name="count" />. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="destinationIndex" /> or <paramref name="count" /> is negative. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The current reader is closed. </exception>
        public int Read(char[] destination, int destinationIndex, int count)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination", "Buffer cannot be null.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            }
            var readed = _length - _position;
            if (readed > 0)
            {
                if (readed > count)
                {
                    readed = count;
                }
                _string.CopyTo(_position, destination, destinationIndex, readed);
                _position += readed;
            }
            return readed;
        }

        /// <summary>
        /// Checks if the next characters from underlying string matches the input string, if so advances the character position by the length of the string.
        /// </summary>
        /// <param name="target">The string to check against.</param>
        /// <returns>
        ///   <c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool Read(string target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            var length = target.Length;
            if (_position + length <= _length)
            {
                var result = _string.Substring(_position, length);
                if (result == target)
                {
                    _position += length;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the next character from underlying string matches the input character, if so advances the character position one character.
        /// </summary>
        /// <param name="target">The character to check against.</param>
        /// <returns>
        ///   <c>true</c> if the characters match; otherwise, <c>false</c>.
        /// </returns>
        public bool Read(char target)
        {
            if (_position == _length)
            {
                return false;
            }
            var result = _string[_position];
            if (result == target)
            {
                _position++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the next characters from the underlying string, if there are enough. If sucessful advances the character position by the given length.
        /// </summary>
        /// <param name="length">The number of characters to read.</param>
        /// <returns>The readed string if there was enough chracaters left; otherwise null.</returns>
        public string Read(int length)
        {
            if (_position + length <= _length)
            {
                var result = _string.Substring(_position, length);
                _position += length;
                return result;
            }
            return null;
        }

        /// <summary>
        /// Checks if the next characters from underlying string matches any of the input strings, if so advances the character position by the length of the string.
        /// </summary>
        /// <remarks>The strings are tested in the order they are provided. Sorting the strings from longer to shorter is suggested.</remarks>
        /// <param name="targets">The list of string to check against.</param>
        /// <returns> The matched string if any; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string Read(IEnumerable<string> targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets", "The targets collection is null.");
            }
            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentException("Found nulls in the targets collection.", "targets");
                }
                var length = target.Length;
                if (_position + length <= _length)
                {
                    var result = _string.Substring(_position, length);
                    if (result == target)
                    {
                        _position += length;
                        return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the next character from theunderlying string if it passes the predicate. If sucessful advances the character position by one character.
        /// </summary>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns>
        ///   <c>true</c> if the sucessful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public bool Read(Func<char, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate", "The predicate is null.");
            }
            if (_position == _length)
            {
                return false;
            }
            var character = _string[_position];
            if (predicate(character))
            {
                _position++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to read the next character from the string. If sucessful advances the character position by one character.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException">Reached the end of the string.</exception>
        /// <returns>
        ///   <c>true</c> if the character was recovered; otherwise, <c>false</c>.
        /// </returns>
        public char ReadChar()
        {
            if (_position == _length)
            {
                throw new IndexOutOfRangeException("Reached the end of the string.");
            }
            var character = _string[_position];
            _position++;
            return character;
        }

        /// <summary>Reads a line from the underlying string.</summary>
        /// <returns>The next line from the underlying string, or null if the end of the underlying string is reached.</returns>
        /// <remarks>The string that is returned does not contain the terminating carriage return or line feed.</remarks>
        /// <exception cref="T:System.ObjectDisposedException">The current reader is closed. </exception>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception>
        public string ReadLine()
        {
            var result = PrivateReadUntil(new[] { '\r', '\n' });
            Read('\r');
            Read('\n');
            return result;
        }

        /// <summary>Reads the underlying string either in its entirety or from the current position to the end.</summary>
        /// <returns>The content from the current position to the end of the underlying string.</returns>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception>
        /// <exception cref="T:System.ObjectDisposedException">The current reader is closed. </exception>
        public string ReadToEnd()
        {
            var result = (_position != 0 ? _string.Substring(_position, _length - _position) : _string);
            _position = _length;
            return result;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position to the given position.
        /// </summary>
        /// <param name="position">To position to which to read to.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The new position must be greater than the current position.</exception>
        public string ReadToPosition(int position)
        {
            if (position == _position)
            {
                return string.Empty;
            }
            if (position < _position)
            {
                throw new ArgumentOutOfRangeException("position", "The new position must be greater than the current position.");
            }
            return PrivateReadToPosition(position);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided string will be next thing to be read afterwards. The provided string is not included in the returned string.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public string ReadUntil(string target)
        {
            return ReadUntil(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided string will be next thing to be read afterwards. The provided string is not included in the returned string.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public string ReadUntil(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    return PrivateReadToPosition(position);
                }
            }
            return ReadToEnd();
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until the provided character is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided character will be the next thing to be read afterwards. The provided character is not included in the returned string.</remarks>
        /// <param name="target">The character to look for.</param>
        /// <returns>The readed string.</returns>
        public string ReadUntil(char target)
        {
            var position = _string.IndexOf(target, _position);
            if (position != -1)
            {
                return PrivateReadToPosition(position);
            }
            return ReadToEnd();
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards. The string found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(IEnumerable<string> targets)
        {
            return ReadUntil(targets, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards. The string found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(IEnumerable<string> targets, StringComparison stringComparison)
        {
            var oldPosition = _position;
            SkipUntil(targets, stringComparison);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards. The string found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="found">The found string.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(IEnumerable<string> targets, out string found)
        {
            return ReadUntil(targets, out found, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards. The string found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="found">The found string.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(IEnumerable<string> targets, out string found, StringComparison stringComparison)
        {
            var oldPosition = _position;
            SkipUntil(targets, out found, stringComparison);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided characters is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character found will be the next thing to be read afterwards. The character found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(IEnumerable<char> targets)
        {
            var oldPosition = _position;
            SkipUntil(targets);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until any of the provided characters is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character found will be the next thing to be read afterwards. The character found is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadUntil(char[] targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }
            return PrivateReadUntil(targets);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until a character passes the provided predicate or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that passes the predicate will be the next thing to be read afterwards. The character that passes the predicate is not included in the returned string.</remarks>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public string ReadUntil(Func<char, bool> predicate)
        {
            var oldPosition = _position;
            SkipUntil(predicate);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>The provided string is included in the returned string.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public string ReadUntilAfter(string target)
        {
            return ReadUntilAfter(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>The provided string is included in the returned string.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public string ReadUntilAfter(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    return PrivateReadToPosition(position + target.Length);
                }
            }
            return ReadToEnd();
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided character will be the next thing to be read afterwards. The provided character is not included in the returned string.</remarks>
        /// <param name="target">The character to look for.</param>
        /// <returns>The readed string.</returns>
        public string ReadUntilAfter(char target)
        {
            var position = _string.IndexOf(target, _position);
            if (position != -1)
            {
                return PrivateReadToPosition(position + 1);
            }
            return ReadToEnd();
        }

        /// <summary>
        /// Reads the underlying string advancing the current position as long as all the readed characters match the provided character.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that doesn't match the provided character will be the next thing to be read afterwards. The character that doesn't match the provider character is not included in the returned string.</remarks>
        /// <param name="target">The character to look for.</param>
        /// <returns>The readed string.</returns>
        public string ReadWhile(char target)
        {
            var oldPosition = _position;
            SkipWhile(target);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position as long as the readed characters match the provided characters.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the first character not found in the provided characters will be the the next thing to be read afterwards. The first character not found in the provided characters is not included in the returned string.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public string ReadWhile(IEnumerable<char> targets)
        {
            var oldPosition = _position;
            SkipWhile(targets);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position as long as the characters pass the provided predicate.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that doesn't pass the predicate will be the next thing to be read afterwards. The character that doesn't pass the predicate is not included in the returned string.</remarks>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns>The readed string.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public string ReadWhile(Func<char, bool> predicate)
        {
            var oldPosition = _position;
            SkipWhile(predicate);
            return _string.Substring(oldPosition, _position - oldPosition);
        }

        /// <summary>
        /// Checks if the next characters from underlying string matches any of the input strings, if so advances the character position by the length of the string.
        /// </summary>
        /// <remarks>The strings are tested in the order they are provided. Sorting the strings from longer to shorter is suggested.</remarks>
        /// <param name="targets">The list of string to check against.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool Skip(IEnumerable<string> targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets", "The targets collection is null.");
            }
            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentException("Found nulls in the targets collection.", "targets");
                }
                var length = target.Length;
                if (_position + length <= _length)
                {
                    var result = _string.Substring(_position, length);
                    if (result == target)
                    {
                        _position += length;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipBackBafore(string target)
        {
            return SkipBackBafore(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipBackBafore(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.LastIndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    _position = position;
                    return true;
                }
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The character to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        public bool SkipBackBafore(char target)
        {
            var position = _string.LastIndexOf(target, _position);
            if (position != -1)
            {
                _position = position;
                return true;
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipBackTo(string target)
        {
            return SkipBackTo(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipBackTo(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.LastIndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    _position = position + target.Length;
                    return true;
                }
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The character to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        public bool SkipBackTo(char target)
        {
            var position = _string.LastIndexOf(target, _position);
            if (position != -1)
            {
                _position = position + 1;
                return true;
            }
            _position = _length;
            return false;
        }

        /// <summary>Skips a line from the underlying string.</summary>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The current reader is closed. </exception>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception>
        public bool SkipLine()
        {
            return PrivateSkipUntil(new[] { '\r', '\n' }) || Read('\r') || Read('\n');
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided string will be next thing to be read afterwards.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipUntil(string target)
        {
            return SkipUntil(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until the provided string is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided string will be next thing to be read afterwards.</remarks>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipUntil(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    _position = position;
                    return true;
                }
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until the provided character is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the provided character will be the next thing to be read afterwards.</remarks>
        /// <param name="target">The character to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        public bool SkipUntil(char target)
        {
            var position = _string.IndexOf(target, _position);
            var result = position != -1;
            _position = result ? position : _length;
            return result;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(IEnumerable<string> targets)
        {
            return SkipUntil(targets, StringComparison.Ordinal);
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(IEnumerable<string> targets, StringComparison stringComparison)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets", "The targets collection is null.");
            }
            var bestPosition = 0;
            var result = false;
            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentException("Found nulls in the targets collection.", "targets");
                }
                if (target.Length != 0)
                {
                    var position = _string.IndexOf(target, _position, stringComparison);
                    if (position != -1)
                    {
                        if (!result || position < bestPosition)
                        {
                            bestPosition = position;
                            result = true;
                        }
                    }
                }
            }
            _position = result ? bestPosition : _length;
            return result;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="found">The found string.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(IEnumerable<string> targets, out string found)
        {
            return SkipUntil(targets, out found, StringComparison.Ordinal);
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided strings is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the string found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <param name="found">The found string.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(IEnumerable<string> targets, out string found, StringComparison stringComparison)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets", "The targets collection is null.");
            }
            found = null;
            var bestPosition = 0;
            var result = false;
            foreach (var target in targets)
            {
                if (target == null)
                {
                    throw new ArgumentException("Found nulls in the targets collection.", "targets");
                }
                if (target.Length != 0)
                {
                    var position = _string.IndexOf(target, _position, stringComparison);
                    if (position != -1)
                    {
                        if (!result || position < bestPosition)
                        {
                            found = target;
                            bestPosition = position;
                            result = true;
                        }
                    }
                }
            }
            _position = result ? bestPosition : _length;
            return result;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided characters is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(IEnumerable<char> targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }
            var bestPosition = 0;
            var result = false;
            foreach (var target in targets)
            {
                var position = _string.IndexOf(target, _position);
                if (position != -1)
                {
                    if (!result || position < bestPosition)
                    {
                        bestPosition = position;
                        result = true;
                    }
                }
            }
            _position = result ? bestPosition : _length;
            return result;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until any of the provided characters is found or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character found will be the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipUntil(char[] targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }
            return PrivateSkipUntil(targets);
        }

        /// <summary>
        /// Skips the underlying string advancing the current position until a character passes the provided predicate or the end of the string is reached.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that passes the predicate will be the next thing to be read afterwards.</remarks>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public bool SkipUntil(Func<char, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate", "The predicate is null.");
            }
            if (_position == _length)
            {
                return false;
            }
            var result = false;
            while (true)
            {
                if (_position == _length)
                {
                    return result;
                }
                var character = _string[_position];
                if (predicate(character))
                {
                    return result;
                }
                _position++;
                result = true;
            }
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipUntilAfter(string target)
        {
            return SkipUntilAfter(target, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided string is found or the end of the string is reached.
        /// </summary>
        /// <param name="target">The string to look for.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The target string is null.</exception>
        public bool SkipUntilAfter(string target, StringComparison stringComparison)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The target string is null.");
            }
            if (target.Length != 0)
            {
                var position = _string.IndexOf(target, _position, stringComparison);
                if (position != -1)
                {
                    _position = position + target.Length;
                    return true;
                }
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Reads the underlying string advancing the current position until afterwards the provided character is found or the end of the string is reached.
        /// </summary>
        /// <returns><c>true</c>if the target was found; otherwise <c>false</c>.</returns>
        /// <param name="target">The character to look for.</param>
        public bool SkipUntilAfter(char target)
        {
            var position = _string.IndexOf(target, _position);
            if (position != -1)
            {
                _position = position + 1;
                return true;
            }
            _position = _length;
            return false;
        }

        /// <summary>
        /// Skips the underlying string advancing the current position as long as all the readed characters match the provided character.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that doesn't match the provided character will be the next thing to be read afterwards.</remarks>
        /// <param name="target">The character to look for.</param>
        /// <returns><c>true</c>if the character position advanced; otherwise <c>false</c>.</returns>
        public bool SkipWhile(char target)
        {
            if (_position == _length)
            {
                return false;
            }
            var result = false;
            while (true)
            {
                if (_position == _length)
                {
                    return result;
                }
                var character = _string[_position];
                if (character != target)
                {
                    return result;
                }
                _position++;
                result = true;
            }
        }

        /// <summary>
        /// Skips the underlying string advancing the current position as long as the readed characters match the provided characters.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the first character not found will be the the next thing to be read afterwards.</remarks>
        /// <param name="targets">The string to look for.</param>
        /// <returns><c>true</c>if the character position advanced; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The targets collection is null.</exception>
        /// <exception cref="ArgumentException">Found nulls in the targets collection.</exception>
        public bool SkipWhile(IEnumerable<char> targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }
            if (_position == _length)
            {
                return false;
            }
            var container = Extensions.AsCollection(targets);
            var result = false;
            while (true)
            {
                if (_position == _length)
                {
                    return result;
                }
                var character = _string[_position];
                if (!container.Contains(character))
                {
                    return result;
                }
                _position++;
                result = true;
            }
        }

        /// <summary>
        /// Skips the underlying string advancing the current position as long as the characters pass the provided predicate.
        /// </summary>
        /// <remarks>If the end of the string is not reached, the character that doesn't pass the predicate will be the next thing to be read afterwards.</remarks>
        /// <param name="predicate">The predicate to test the characters.</param>
        /// <returns><c>true</c>if the character position advanced; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public bool SkipWhile(Func<char, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (_position == _length)
            {
                return false;
            }
            var result = false;
            while (true)
            {
                if (_position == _length)
                {
                    return result;
                }
                var character = _string[_position];
                if (!predicate(character))
                {
                    return result;
                }
                _position++;
                result = true;
            }
        }

        /// <summary>
        /// Attempts to read the next character from the string.
        /// </summary>
        /// <param name="character">The recovered character.</param>
        /// <returns>
        ///   <c>true</c> if the character was recovered; otherwise, <c>false</c>.
        /// </returns>
        public bool TryPeek(out char character)
        {
            if (_position == _length)
            {
                character = default(char);
                return false;
            }
            character = _string[_position];
            return true;
        }

        /// <summary>
        /// Attempts to read the next character from the string. If sucessful advances the character position by one character.
        /// </summary>
        /// <param name="character">The recovered character.</param>
        /// <returns>
        ///   <c>true</c> if the character was recovered; otherwise, <c>false</c>.
        /// </returns>
        public bool TryTake(out char character)
        {
            if (_position == _length)
            {
                character = default(char);
                return false;
            }
            character = _string[_position];
            _position++;
            return true;
        }

        private string PrivateReadToPosition(int position)
        {
            var result = _string.Substring(_position, position - _position);
            _position = position;
            return result;
        }

        private string PrivateReadUntil(char[] targets)
        {
            var position = _string.IndexOfAny(targets, _position);
            if (position != -1)
            {
                return PrivateReadToPosition(position);
            }
            return ReadToEnd();
        }

        private bool PrivateSkipUntil(char[] targets)
        {
            var position = _string.IndexOfAny(targets, _position);
            var result = position != -1;
            _position = result ? position : _length;
            return result;
        }
    }
}