namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.ImmutableObject(true)]
    public sealed class DefaultNeedle<T> : INeedle<T>
    {
        private static readonly DefaultNeedle<T> _instance = new DefaultNeedle<T>();

        private DefaultNeedle()
        {
            //Empty
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "By Design")]
        public static DefaultNeedle<T> Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool IsAlive
        {
            get
            {
                return true;
            }
        }

        public T Value
        {
            get
            {
                return default(T);
            }
            set
            {
                //Empty
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is DefaultNeedle<T>)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        void INeedle<T>.Release()
        {
            //Empty
        }
    }
}