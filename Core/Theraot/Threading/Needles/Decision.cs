using System;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public class Decision : INeedle<bool>
    {
        private readonly Action _onFalse;
        private readonly Action _onTrue;

        private bool _target;
        private Action _toDo;

        public Decision(Action onTrue, Action onFalse)
            : this(onTrue, onFalse, false)
        {
            //Empty
        }

        public Decision(Action onTrue, Action onFalse, bool target)
        {
            _onFalse = onFalse ?? ActionHelper.GetNoopAction();
            _onTrue = onTrue ?? ActionHelper.GetNoopAction();
            _target = target;
            Decide();
        }

        public bool Value
        {
            get
            {
                return _target;
            }
            set
            {
                if (_target != value)
                {
                    _target = value;
                    Decide();
                }
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns True")]
        bool IReadOnlyNeedle<bool>.IsAlive
        {
            get
            {
                return true;
            }
        }

        public void Execute()
        {
            _toDo();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Empty")]
        void INeedle<bool>.Release()
        {
            //Empty
        }

        private void Decide()
        {
            _toDo = _target ? _onTrue : _onFalse;
        }
    }
}