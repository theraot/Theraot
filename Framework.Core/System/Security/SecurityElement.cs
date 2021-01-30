#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable CA2227 // Collection properties should be read only
#pragma warning disable S1168 // Return empty collection

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Security
{
    public sealed class SecurityElement : ISecurityElementFactory
    {
        private const int _attributesTypical = 4 * 2; // 4 attributes, times 2 strings per attribute
        private const int _childrenTypical = 1;
        private static readonly char[] _escapeChars = { '<', '>', '\"', '\'', '&' };

        private static readonly string[] _escapeStringPairs =
        {
            // these must be all once character escape sequences or a new escaping algorithm is needed
            "<", "&lt;",
            ">", "&gt;",
            "\"", "&quot;",
            "\'", "&apos;",
            "&", "&amp;"
        };

        private static readonly char[] _tagIllegalCharacters = { ' ', '<', '>' };
        private static readonly char[] _textIllegalCharacters = { '<', '>' };
        private static readonly char[] _valueIllegalCharacters = { '<', '>', '\"' };
        private ArrayList? _attributes;
        private ArrayList? _children;
        private string _tag;
        private string? _text;

        public SecurityElement(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (!IsValidTag(tag))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element tag '{0}'.", tag));
            }

            _tag = tag;
            _text = null;
        }

        public SecurityElement(string? tag, string? text)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (!IsValidTag(tag))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element tag '{0}'.", tag));
            }

            if (text != null && !IsValidText(text))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element text '{0}'.", text));
            }

            _tag = tag;
            _text = text;
        }

        public Hashtable? Attributes
        {
            get
            {
                if (_attributes == null || _attributes.Count == 0)
                {
                    return null;
                }

                var hashtable = new Hashtable(_attributes.Count / 2);

                var iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (var i = 0; i < iMax; i += 2)
                {
                    hashtable.Add(_attributes[i]!, _attributes[i + 1]);
                }

                return hashtable;
            }

            set
            {
                if (value == null || value.Count == 0)
                {
                    _attributes = null;
                }
                else
                {
                    var list = new ArrayList(value.Count);
                    var enumerator = value.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var attrName = (string)enumerator.Key;
                        var attrValue = (string)enumerator.Value;

                        if (!IsValidAttributeName(attrName))
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element name '{0}'.", attrName));
                        }

                        if (!IsValidAttributeValue(attrValue))
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element value '{0}'.", attrValue));
                        }

                        list.Add(attrName);
                        list.Add(attrValue);
                    }

                    _attributes = list;
                }
            }
        }

        public ArrayList? Children
        {
            get
            {
                ConvertSecurityElementFactories();
                return _children;
            }

            set
            {
                if (value?.Contains(null) == true)
                {
                    throw new ArgumentException("Cannot have a null child.");
                }

                _children = value;
            }
        }

        public string Tag
        {
            get => _tag;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!IsValidTag(value))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element tag '{0}'.", value));
                }

                _tag = value;
            }
        }

        public string? Text
        {
            get => Unescape(_text);

            set
            {
                if (value == null)
                {
                    _text = null;
                }
                else
                {
                    if (!IsValidText(value))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element tag '{0}'.", value));
                    }

                    _text = value;
                }
            }
        }

        public static string? Escape(string? str)
        {
            if (str == null)
            {
                return null;
            }

            StringBuilder? sb = null;

            var strLen = str.Length;
            var newIndex = 0; // Pointer into the string that indicates the start index of the "remaining" string (that still needs to be processed).

            while (true)
            {
                var index = str.IndexOfAny(_escapeChars, newIndex); // Pointer into the string that indicates the location of the current '&' character

                if (index == -1)
                {
                    if (sb == null)
                    {
                        return str;
                    }

                    sb.Append(str, newIndex, strLen - newIndex);
                    return sb.ToString();
                }

                (sb ??= new StringBuilder()).Append(str, newIndex, index - newIndex);
                sb.Append(GetEscapeSequence(str[index]));

                newIndex = index + 1;
            }

            // no normal exit is possible
        }

        public static SecurityElement? FromString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            return default;
        }

        public static bool IsValidAttributeName(string name)
        {
            return IsValidTag(name);
        }

        public static bool IsValidAttributeValue(string value)
        {
            return value?.IndexOfAny(_valueIllegalCharacters) == -1;
        }

        public static bool IsValidTag(string tag)
        {
            return tag?.IndexOfAny(_tagIllegalCharacters) == -1;
        }

        public static bool IsValidText(string text)
        {
            return text?.IndexOfAny(_textIllegalCharacters) == -1;
        }

        public void AddAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!IsValidAttributeName(name))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element name '{0}'.", name));
            }

            if (!IsValidAttributeValue(value))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element value '{0}'.", value));
            }

            AddAttributeSafe(name, value);
        }

        public void AddChild(SecurityElement child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            (_children ??= new ArrayList(_childrenTypical)).Add(child);
        }

        public string? Attribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            // Note: we don't check for validity here because an
            // if an invalid name is passed we simply won't find it.
            if (_attributes == null)
            {
                return null;
            }

            // Go through all the attribute and see if we know about
            // the one we are asked for
            var iMax = _attributes.Count;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (var i = 0; i < iMax; i += 2)
            {
                var strAttrName = (string?)_attributes[i];

                if (!string.Equals(strAttrName, name, StringComparison.Ordinal))
                {
                    continue;
                }

                var strAttrValue = (string?)_attributes[i + 1];

                return Unescape(strAttrValue);
            }

            // In the case where we didn't find it, we are expected to
            // return null
            return null;
        }

        string? ISecurityElementFactory.Attribute(string attributeName)
        {
            return Attribute(attributeName);
        }

        public SecurityElement Copy()
        {
            return new SecurityElement(_tag, _text)
            {
                _children = _children == null ? null : new ArrayList(_children),
                _attributes = _attributes == null ? null : new ArrayList(_attributes)
            };
        }

        object ISecurityElementFactory.Copy()
        {
            return Copy();
        }

        SecurityElement ISecurityElementFactory.CreateSecurityElement()
        {
            return this;
        }

        public bool Equal(SecurityElement other)
        {
            if (other == null)
            {
                return false;
            }

            // Check if the tags are the same
            if (!string.Equals(_tag, other._tag, StringComparison.Ordinal))
            {
                return false;
            }

            // Check if the text is the same
            if (!string.Equals(_text, other._text, StringComparison.Ordinal))
            {
                return false;
            }

            // Check if the attributes are the same and appear in the same
            // order.
            if (_attributes == null || other._attributes == null)
            {
                if (_attributes != other._attributes)
                {
                    return false;
                }
            }
            else
            {
                var iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                // Maybe we can get away by only checking the number of attributes
                if (iMax != other._attributes.Count)
                {
                    return false;
                }

                for (var i = 0; i < iMax; i++)
                {
                    var lhs = (string?)_attributes[i];
                    var rhs = (string?)other._attributes[i];

                    if (!string.Equals(lhs, rhs, StringComparison.Ordinal))
                    {
                        return false;
                    }
                }
            }

            // Finally we must check the child and make sure they are
            // equal and in the same order
            if (_children == null || other._children == null)
            {
                if (_children != other._children)
                {
                    return false;
                }
            }
            else
            {
                // Maybe we can get away by only checking the number of children
                if (_children.Count != other._children.Count)
                {
                    return false;
                }

                ConvertSecurityElementFactories();
                other.ConvertSecurityElementFactories();

                var lhs = _children.GetEnumerator();
                var rhs = other._children.GetEnumerator();

                while (lhs.MoveNext())
                {
                    rhs.MoveNext();
                    var e1 = (SecurityElement)lhs.Current;
                    var e2 = (SecurityElement)rhs.Current;
                    if (e1?.Equal(e2) != true)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        string ISecurityElementFactory.GetTag()
        {
            return Tag;
        }

        public SecurityElement? SearchForChildByTag(string tag)
        {
            // Go through all the children and see if we can
            // find the one are are asked for (matching tags)
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            // Note: we don't check for a valid tag here because
            // an invalid tag simply won't be found.
            if (_children == null)
            {
                return null;
            }

            foreach (SecurityElement current in _children)
            {
                if (current != null && string.Equals(current.Tag, tag, StringComparison.Ordinal))
                {
                    return current;
                }
            }

            return null;
        }

        public string? SearchForTextOfTag(string tag)
        {
            // Search on each child in order and each
            // child's child, depth-first
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            // Note: we don't check for a valid tag here because
            // an invalid tag simply won't be found.
            if (string.Equals(_tag, tag, StringComparison.Ordinal))
            {
                return Unescape(_text);
            }

            if (_children == null)
            {
                return null;
            }

            ConvertSecurityElementFactories();
            foreach (SecurityElement child in _children)
            {
                var text = child.SearchForTextOfTag(tag);
                if (text != null)
                {
                    return text;
                }
            }

            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            ToString(sb, (obj, str) => ((StringBuilder)obj).Append(str));

            return sb.ToString();
        }

        internal void AddAttributeSafe(string name, string value)
        {
            if (_attributes == null)
            {
                _attributes = new ArrayList(_attributesTypical);
            }
            else
            {
                var iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (var i = 0; i < iMax; i += 2)
                {
                    var strAttrName = (string?)_attributes[i];

                    if (string.Equals(strAttrName, name, StringComparison.Ordinal))
                    {
                        throw new ArgumentException("Attribute names must be unique.");
                    }
                }
            }

            _attributes.Add(name);
            _attributes.Add(value);
        }

        internal void ConvertSecurityElementFactories()
        {
            if (_children == null)
            {
                return;
            }

            for (var i = 0; i < _children.Count; ++i)
            {
                if (_children[i] is ISecurityElementFactory iseFactory && !(_children[i] is SecurityElement))
                {
                    _children[i] = iseFactory.CreateSecurityElement();
                }
            }
        }

        private static string GetEscapeSequence(char c)
        {
            var iMax = _escapeStringPairs.Length;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (var i = 0; i < iMax; i += 2)
            {
                var strEscSeq = _escapeStringPairs[i];
                var strEscValue = _escapeStringPairs[i + 1];

                if (strEscSeq[0] == c)
                {
                    return strEscValue;
                }
            }

            DebugEx.Fail("Unable to find escape sequence for this character");
            return c.ToString();
        }

        private static string GetUnescapeSequence(string str, int index, out int newIndex)
        {
            var maxCompareLength = str.Length - index;

            var iMax = _escapeStringPairs.Length;
            Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

            for (var i = 0; i < iMax; i += 2)
            {
                var strEscSeq = _escapeStringPairs[i];
                var strEscValue = _escapeStringPairs[i + 1];

                var length = strEscValue.Length;

                if (length > maxCompareLength || string.Compare(strEscValue, 0, str, index, length, StringComparison.Ordinal) != 0)
                {
                    continue;
                }

                newIndex = index + strEscValue.Length;
                return strEscSeq;
            }

            newIndex = index + 1;
            return str[index].ToString();
        }

        private static string? Unescape(string? str)
        {
            if (str == null)
            {
                return null;
            }

            StringBuilder? sb = null;

            var strLen = str.Length;
            var newIndex = 0; // Pointer into the string that indicates the start index of the remaining string (that still needs to be processed).

            while (true)
            {
                var index = str.IndexOf('&', newIndex); // Pointer into the string that indicates the location of the current '&' character

                if (index == -1)
                {
                    if (sb == null)
                    {
                        return str;
                    }

                    sb.Append(str, newIndex, strLen - newIndex);
                    return sb.ToString();
                }

                (sb ??= new StringBuilder()).Append(str, newIndex, index - newIndex);
                sb.Append(GetUnescapeSequence(str, index, out newIndex)); // updates the newIndex too
            }
        }

        private void ToString(object obj, Action<object, string?> write)
        {
            write(obj, "<");
            write(obj, _tag);

            // If there are any attributes, plop those in.
            if (_attributes?.Count > 0)
            {
                write(obj, " ");

                var iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (var i = 0; i < iMax; i += 2)
                {
                    var strAttrName = (string?)_attributes[i];
                    var strAttrValue = (string?)_attributes[i + 1];

                    write(obj, strAttrName);
                    write(obj, "=\"");
                    write(obj, strAttrValue);
                    write(obj, "\"");

                    if (i != _attributes.Count - 2)
                    {
                        write(obj, Environment.NewLine);
                    }
                }
            }

            if (_text == null && (_children == null || _children.Count == 0))
            {
                // If we are a single tag with no children, just add the end of tag text.
                write(obj, "/>");
                write(obj, Environment.NewLine);
            }
            else
            {
                // Close the current tag.
                write(obj, ">");

                // Output the text
                write(obj, _text);

                // Output any children.
                if (_children != null)
                {
                    ConvertSecurityElementFactories();

                    write(obj, Environment.NewLine);

                    foreach (var child in _children)
                    {
                        ((SecurityElement)child).ToString(obj, write);
                    }
                }

                // Output the closing tag
                write(obj, "</");
                write(obj, _tag);
                write(obj, ">");
                write(obj, Environment.NewLine);
            }
        }
    }
}

#endif