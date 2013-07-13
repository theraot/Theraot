namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class Capsule<T> : IReadOnlyNeedle<T>
    {
        private bool _isAlive;
        private T _target;

        public Capsule()
        {
            _isAlive = false;
        }

        public Capsule(T target)
        {
            _isAlive = true;
            _target = target;
        }

        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
        }

        private void Kill()
        {
            _isAlive = false;
            _target = default(T);
        }
    }
}