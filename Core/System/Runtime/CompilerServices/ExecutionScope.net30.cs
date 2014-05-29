#if NET20 || NET30

using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
    public class ExecutionScope
    {
        private readonly int _compilationUnit;
        private CompilationContext _context;
        private object[] _globals;
        private object[] _locals;
        private ExecutionScope _parent;

        internal ExecutionScope(CompilationContext context)
            : this(context, 0)
        {
        }

        internal ExecutionScope(CompilationContext context, int compilationUnit, ExecutionScope parent, object[] locals)
            : this(context, compilationUnit)
        {
            _parent = parent;
            _locals = locals;
        }

        private ExecutionScope(CompilationContext context, int compilationUnit)
        {
            _context = context;
            _compilationUnit = compilationUnit;
            _globals = context.GetGlobals();
        }

        public object[] Globals
        {
            get
            {
                return _globals;
            }
            set
            {
                _globals = value;
            }
        }

        public object[] Locals
        {
            get
            {
                return _locals;
            }
            set
            {
                _locals = value;
            }
        }

        public ExecutionScope Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
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