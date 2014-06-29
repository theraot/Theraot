#if NET20 || NET30

using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
    public partial class ExecutionScope
    {
        //These fields are accessed via Reflection
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public object[] Globals;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public object[] Locals;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Microsoft's Design")]
        public ExecutionScope Parent;
    }

    public partial class ExecutionScope
    {
        private readonly int _compilationUnit;
        private CompilationContext _context;

        internal ExecutionScope(CompilationContext context)
            : this(context, 0)
        {
        }

        internal ExecutionScope(CompilationContext context, int compilationUnit, ExecutionScope parent, object[] locals)
            : this(context, compilationUnit)
        {
            Parent = parent;
            Locals = locals;
        }

        private ExecutionScope(CompilationContext context, int compilationUnit)
        {
            _context = context;
            _compilationUnit = compilationUnit;
            Globals = context.GetGlobals();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Mono compatibility")]
        internal int compilation_unit
        {
            get
            {
                return _compilationUnit;
            }
        }

        internal CompilationContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        public Delegate CreateDelegate(int indexLambda, object[] locals)
        {
            return _context.CreateDelegate(
                       indexLambda,
                       new ExecutionScope(_context, indexLambda, this, locals));
        }

        public object[] CreateHoistedLocals()
        {
            return _context.CreateHoistedLocals(_compilationUnit);
        }

        public Expression IsolateExpression(Expression expression, object[] locals)
        {
            return _context.IsolateExpression(this, locals, expression);
        }
    }
}

#endif