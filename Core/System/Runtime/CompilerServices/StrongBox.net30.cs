#if NET20 || NET30

namespace System.Runtime.CompilerServices
{
    public class StrongBox<T> : IStrongBox
    {
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public T Value;

        public StrongBox()
        {
            // Empty
        }

        public StrongBox(T value)
        {
            Value = value;
        }

        object IStrongBox.Value
        {
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
            get
            {
                return Value;
            }
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
            set
            {
                Value = (T)value;
            }
        }
    }
}

#endif