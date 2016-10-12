using System.Runtime.InteropServices;
using System.Collections;

namespace System.Security
{
    [ComVisible(true)]
    [Serializable]
    public sealed class SecurityElement
    {
        public Hashtable Attributes
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public ArrayList Children
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public string Tag
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public string Text
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private SecurityElement()
        {
            // Empty
        }

        public SecurityElement(string tag)
        {
            throw new NotSupportedException();
        }

        public SecurityElement(string tag, string text)
        {
            throw new NotSupportedException();
        }

        public void AddAttribute(string name, string value)
        {
            throw new NotSupportedException();
        }

        public void AddChild(SecurityElement child)
        {
            throw new NotSupportedException();
        }

        public string Attribute(string name)
        {
            throw new NotSupportedException();
        }

        [ComVisible(false)]
        public SecurityElement Copy()
        {
            throw new NotSupportedException();
        }

        public bool Equal(SecurityElement other)
        {
            throw new NotSupportedException();
        }

        public static string Escape(string str)
        {
            throw new NotImplementedException();
        }

        public static SecurityElement FromString(string xml)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidAttributeName(string name)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidAttributeValue(string value)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidTag(string tag)
        {
            throw new NotImplementedException();
        }

        public static bool IsValidText(string text)
        {
            throw new NotImplementedException();
        }

        public SecurityElement SearchForChildByTag(string tag)
        {
            throw new NotSupportedException();
        }

        public string SearchForTextOfTag(string tag)
        {
            throw new NotSupportedException();
        }
    }
}