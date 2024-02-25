#if LESSTHAN_NETSTANDARD15

#pragma warning disable CA1815 // Override equals and operator equals on value types

using System.Runtime.InteropServices;

namespace System.Reflection
{
    [ComVisible(true)]
    [Serializable]
    public struct ParameterModifier
    {
        private readonly bool[] _internal;

        public ParameterModifier(int parameterCount)
        {
            if (parameterCount <= 0)
            {
                throw new ArgumentException("Must specify one or more parameters.");
            }

            _internal = new bool[parameterCount];
        }


        public bool this[int index]
        {
            get
            {
                return _internal[index];
            }
            set
            {
                _internal[index] = value;
            }
        }
    }
}

#endif