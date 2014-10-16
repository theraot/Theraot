#if NET20 || NET30

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal class CompilationContext
    {
        private readonly List<object> _globals = new List<object>();
        private Dictionary<LambdaExpression, List<ParameterExpression>> _hoistedMap;
        private readonly List<EmitContext> _units = new List<EmitContext>();

        public int AddCompilationUnit(LambdaExpression lambda)
        {
            DetectHoistedVariables(lambda);
            return AddCompilationUnit(null, lambda);
        }

        public int AddCompilationUnit(EmitContext parent, LambdaExpression lambda)
        {
            var context = new EmitContext(this, parent, lambda);
            var unit = AddItemToList(context, _units);
            context.Emit();
            return unit;
        }

        public int AddGlobal(object global)
        {
            return AddItemToList(global, _globals);
        }

        public Delegate CreateDelegate()
        {
            return CreateDelegate(0, new ExecutionScope(this));
        }

        public Delegate CreateDelegate(int unit, ExecutionScope scope)
        {
            return _units[unit].CreateDelegate(scope);
        }

        public object[] CreateHoistedLocals(int unit)
        {
            var hoisted = GetHoistedLocals(_units[unit].Lambda);
            return new object[hoisted == null ? 0 : hoisted.Count];
        }

        public object[] GetGlobals()
        {
            return _globals.ToArray();
        }

        public List<ParameterExpression> GetHoistedLocals(LambdaExpression lambda)
        {
            if (_hoistedMap == null)
            {
                return null;
            }
            else
            {
                List<ParameterExpression> hoisted;
                _hoistedMap.TryGetValue(lambda, out hoisted);
                return hoisted;
            }
        }

        public Expression IsolateExpression(ExecutionScope scope, object[] locals, Expression expression)
        {
            return new ParameterReplacer(this, scope, locals).Transform(expression);
        }

        private static int AddItemToList<T>(T item, ICollection<T> list)
        {
            list.Add(item);
            return list.Count - 1;
        }

        private void DetectHoistedVariables(LambdaExpression lambda)
        {
            _hoistedMap = new HoistedVariableDetector().Process(lambda);
        }

        private class HoistedVariableDetector : ExpressionVisitor
        {
            private Dictionary<LambdaExpression, List<ParameterExpression>> _hoisted_map;
            private LambdaExpression _lambda;
            private readonly Dictionary<ParameterExpression, LambdaExpression> _parameter_to_lambda = new Dictionary<ParameterExpression, LambdaExpression>();

            public Dictionary<LambdaExpression, List<ParameterExpression>> Process(LambdaExpression lambda)
            {
                Visit(lambda);
                return _hoisted_map;
            }

            protected override void VisitLambda(LambdaExpression lambda)
            {
                _lambda = lambda;
                foreach (var parameter in lambda.Parameters)
                {
                    _parameter_to_lambda[parameter] = lambda;
                }
                base.VisitLambda(lambda);
            }

            protected override void VisitParameter(ParameterExpression parameter)
            {
                if (_lambda.Parameters.Contains(parameter))
                {
                    return;
                }
                Hoist(parameter);
            }

            private void Hoist(ParameterExpression parameter)
            {
                LambdaExpression lambda;
                if (!_parameter_to_lambda.TryGetValue(parameter, out lambda))
                {
                    return;
                }
                if (_hoisted_map == null)
                {
                    _hoisted_map = new Dictionary<LambdaExpression, List<ParameterExpression>>();
                }
                List<ParameterExpression> hoisted;
                if (!_hoisted_map.TryGetValue(lambda, out hoisted))
                {
                    hoisted = new List<ParameterExpression>();
                    _hoisted_map[lambda] = hoisted;
                }
                hoisted.Add(parameter);
            }
        }

        private class ParameterReplacer : ExpressionTransformer
        {
            private readonly CompilationContext _context;
            private readonly object[] _locals;
            private readonly ExecutionScope _scope;

            public ParameterReplacer(CompilationContext context, ExecutionScope scope, object[] locals)
            {
                _context = context;
                _scope = scope;
                _locals = locals;
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                var scope = _scope;
                var locals = _locals;
                while (scope != null)
                {
                    int position = IndexOfHoistedLocal(scope, parameter);
                    if (position != -1)
                    {
                        return ReadHoistedLocalFromArray(locals, position);
                    }
                    locals = scope.Locals;
                    scope = scope.Parent;
                }
                return parameter;
            }

            private int IndexOfHoistedLocal(ExecutionScope scope, ParameterExpression parameter)
            {
                return _context._units[scope.CompilationUnit].IndexOfHoistedLocal(parameter);
            }

            private Expression ReadHoistedLocalFromArray(object[] locals, int position)
            {
                return Expression.Field
                (
                    Expression.Convert
                    (
                        Expression.ArrayIndex
                        (
                            Expression.Constant(locals),
                            Expression.Constant(position)
                        ),
                        locals[position].GetType()
                    ),
                    "Value"
                );
            }
        }
    }
}

#endif