#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

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
        private string _tag;
        private string _text;
        private ArrayList _children;
        private ArrayList _attributes;

        private const int _attributesTypical = 4 * 2;  // 4 attributes, times 2 strings per attribute
        private const int _childrenTypical = 1;

        private static readonly char[] _tagIllegalCharacters = new char[] { ' ', '<', '>' };
        private static readonly char[] _textIllegalCharacters = new char[] { '<', '>' };
        private static readonly char[] _valueIllegalCharacters = new char[] { '<', '>', '\"' };
        private static readonly char[] _escapeChars = new char[] { '<', '>', '\"', '\'', '&' };

        private static readonly string[] _escapeStringPairs = new string[]
        {
            // these must be all once character escape sequences or a new escaping algorithm is needed
            "<", "&lt;",
            ">", "&gt;",
            "\"", "&quot;",
            "\'", "&apos;",
            "&", "&amp;"
        };

        //-------------------------- Constructors ---------------------------

        internal SecurityElement()
        {
            // Empty
        }

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

        public SecurityElement(string tag, string text)
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

        //-------------------------- Properties -----------------------------

        public string Tag
        {
            get => _tag;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Tag));
                }

                if (!IsValidTag(value))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid element tag '{0}'.", value));
                }

                _tag = value;
            }
        }

        public Hashtable Attributes
        {
            get
            {
                if (_attributes == null || _attributes.Count == 0)
                {
                    return null;
                }
                else
                {
                    var hashtable = new Hashtable(_attributes.Count / 2);

                    var iMax = _attributes.Count;
                    Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                    for (var i = 0; i < iMax; i += 2)
                    {
                        hashtable.Add(_attributes[i], _attributes[i + 1]);
                    }

                    return hashtable;
                }
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

        public string Text
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

        public ArrayList Children
        {
            get
            {
                ConvertSecurityElementFactories();
                return _children;
            }

            set
            {
                if (value != null && value.Contains(null))
                {
                    throw new ArgumentException("Cannot have a null child.");
                }
                _children = value;
            }
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

        //-------------------------- Public Methods -----------------------------

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
                    var strAttrName = (string)_attributes[i];

                    if (string.Equals(strAttrName, name))
                    {
                        throw new ArgumentException("Attribute names must be unique.");
                    }
                }
            }

            _attributes.Add(name);
            _attributes.Add(value);
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

            if (_children == null)
            {
                _children = new ArrayList(_childrenTypical);
            }

            _children.Add(child);
        }

        public bool Equal(SecurityElement other)
        {
            if (other == null)
            {
                return false;
            }

            // Check if the tags are the same
            if (!string.Equals(_tag, other._tag))
            {
                return false;
            }

            // Check if the text is the same
            if (!string.Equals(_text, other._text))
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
                    var lhs = (string)_attributes[i];
                    var rhs = (string)other._attributes[i];

                    if (!string.Equals(lhs, rhs))
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
                    if (e1 == null || !e1.Equal(e2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public SecurityElement Copy()
        {
            var element = new SecurityElement(_tag, _text)
            {
                _children = _children == null ? null : new ArrayList(_children),
                _attributes = _attributes == null ? null : new ArrayList(_attributes)
            };

            return element;
        }

        public static bool IsValidTag(string tag)
        {
            if (tag == null)
            {
                return false;
            }

            return tag.IndexOfAny(_tagIllegalCharacters) == -1;
        }

        public static bool IsValidText(string text)
        {
            if (text == null)
            {
                return false;
            }

            return text.IndexOfAny(_textIllegalCharacters) == -1;
        }

        public static bool IsValidAttributeName(string name)
        {
            return IsValidTag(name);
        }

        public static bool IsValidAttributeValue(string value)
        {
            if (value == null)
            {
                return false;
            }

            return value.IndexOfAny(_valueIllegalCharacters) == -1;
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

            Debug.Assert(false, "Unable to find escape sequence for this character");
            return c.ToString();
        }

        public static string Escape(string str)
        {
            if (str == null)
            {
                return null;
            }

            StringBuilder sb = null;

            var strLen = str.Length;
            int index; // Pointer into the string that indicates the location of the current '&' character
            var newIndex = 0; // Pointer into the string that indicates the start index of the "remaining" string (that still needs to be processed).

            while (true)
            {
                index = str.IndexOfAny(_escapeChars, newIndex);

                if (index == -1)
                {
                    if (sb == null)
                    {
                        return str;
                    }
                    else
                    {
                        sb.Append(str, newIndex, strLen - newIndex);
                        return sb.ToString();
                    }
                }
                else
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(str, newIndex, index - newIndex);
                    sb.Append(GetEscapeSequence(str[index]));

                    newIndex = index + 1;
                }
            }

            // no normal exit is possible
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

                if (length <= maxCompareLength && string.Compare(strEscValue, 0, str, index, length, StringComparison.Ordinal) == 0)
                {
                    newIndex = index + strEscValue.Length;
                    return strEscSeq;
                }
            }

            newIndex = index + 1;
            return str[index].ToString();
        }

        private static string Unescape(string str)
        {
            if (str == null)
            {
                return null;
            }

            StringBuilder sb = null;

            var strLen = str.Length;
            int index; // Pointer into the string that indicates the location of the current '&' character
            var newIndex = 0; // Pointer into the string that indicates the start index of the "remainging" string (that still needs to be processed).

            do
            {
                index = str.IndexOf('&', newIndex);

                if (index == -1)
                {
                    if (sb == null)
                    {
                        return str;
                    }
                    else
                    {
                        sb.Append(str, newIndex, strLen - newIndex);
                        return sb.ToString();
                    }
                }
                else
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(str, newIndex, index - newIndex);
                    sb.Append(GetUnescapeSequence(str, index, out newIndex)); // updates the newIndex too
                }
            }
            while (true);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            ToString(sb, (obj, str) => ((StringBuilder)obj).Append(str));

            return sb.ToString();
        }

        private void ToString(object obj, Action<object, string> write)
        {
            write(obj, "<");
            write(obj, _tag);

            // If there are any attributes, plop those in.
            if (_attributes != null && _attributes.Count > 0)
            {
                write(obj, " ");

                var iMax = _attributes.Count;
                Debug.Assert(iMax % 2 == 0, "Odd number of strings means the attr/value pairs were not added correctly");

                for (var i = 0; i < iMax; i += 2)
                {
                    var strAttrName = (string)_attributes[i];
                    var strAttrValue = (string)_attributes[i + 1];

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

                    for (var i = 0; i < _children.Count; ++i)
                    {
                        ((SecurityElement)_children[i]).ToString(obj, write);
                    }
                }

                // Output the closing tag
                write(obj, "</");
                write(obj, _tag);
                write(obj, ">");
                write(obj, Environment.NewLine);
            }
        }

        public string Attribute(string name)
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
                var strAttrName = (string)_attributes[i];

                if (string.Equals(strAttrName, name))
                {
                    var strAttrValue = (string)_attributes[i + 1];

                    return Unescape(strAttrValue);
                }
            }

            // In the case where we didn't find it, we are expected to
            // return null
            return null;
        }

        public SecurityElement SearchForChildByTag(string tag)
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
                if (current != null && string.Equals(current.Tag, tag))
                {
                    return current;
                }
            }
            return null;
        }

        public string SearchForTextOfTag(string tag)
        {
            // Search on each child in order and each
            // child's child, depth-first
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            // Note: we don't check for a valid tag here because
            // an invalid tag simply won't be found.
            if (string.Equals(_tag, tag))
            {
                return Unescape(_text);
            }

            if (_children == null)
            {
                return null;
            }

            foreach (SecurityElement child in Children)
            {
                var text = child.SearchForTextOfTag(tag);
                if (text != null)
                {
                    return text;
                }
            }
            return null;
        }

        public static SecurityElement FromString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            return default;
        }

        //--------------- ISecurityElementFactory implementation -----------------

        SecurityElement ISecurityElementFactory.CreateSecurityElement()
        {
            return this;
        }

        string ISecurityElementFactory.GetTag()
        {
            return Tag;
        }

        object ISecurityElementFactory.Copy()
        {
            return Copy();
        }

        string ISecurityElementFactory.Attribute(string attributeName)
        {
            return Attribute(attributeName);
        }
    }
}

#endif