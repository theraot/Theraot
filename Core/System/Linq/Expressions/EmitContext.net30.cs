#if NET20 || NET30

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal class CompilationContext
    {
        private List<object> globals = new List<object>();

        private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

        private List<EmitContext> units = new List<EmitContext>();

        public int AddCompilationUnit(LambdaExpression lambda)
        {
            DetectHoistedVariables(lambda);
            return AddCompilationUnit(null, lambda);
        }

        public int AddCompilationUnit(EmitContext parent, LambdaExpression lambda)
        {
            var context = new EmitContext(this, parent, lambda);
            var unit = AddItemToList(context, units);
            context.Emit();
            return unit;
        }

        public int AddGlobal(object global)
        {
            return AddItemToList(global, globals);
        }

        public Delegate CreateDelegate()
        {
            return CreateDelegate(0, new ExecutionScope(this));
        }

        public Delegate CreateDelegate(int unit, ExecutionScope scope)
        {
            return units[unit].CreateDelegate(scope);
        }

        public object[] CreateHoistedLocals(int unit)
        {
            var hoisted = GetHoistedLocals(units[unit].Lambda);
            return new object[hoisted == null ? 0 : hoisted.Count];
        }

        public object[] GetGlobals()
        {
            return globals.ToArray();
        }

        public List<ParameterExpression> GetHoistedLocals(LambdaExpression lambda)
        {
            if (hoisted_map == null)
                return null;

            List<ParameterExpression> hoisted;
            hoisted_map.TryGetValue(lambda, out hoisted);
            return hoisted;
        }

        public Expression IsolateExpression(ExecutionScope scope, object[] locals, Expression expression)
        {
            return new ParameterReplacer(this, scope, locals).Transform(expression);
        }

        private static int AddItemToList<T>(T item, IList<T> list)
        {
            list.Add(item);
            return list.Count - 1;
        }

        private void DetectHoistedVariables(LambdaExpression lambda)
        {
            hoisted_map = new HoistedVariableDetector().Process(lambda);
        }

        private class HoistedVariableDetector : ExpressionVisitor
        {
            private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

            private LambdaExpression lambda;

            private Dictionary<ParameterExpression, LambdaExpression> parameter_to_lambda =
                                        new Dictionary<ParameterExpression, LambdaExpression>();
            public Dictionary<LambdaExpression, List<ParameterExpression>> Process(LambdaExpression lambda)
            {
                Visit(lambda);
                return hoisted_map;
            }

            protected override void VisitLambda(LambdaExpression lambda)
            {
                this.lambda = lambda;
                foreach (var parameter in lambda.Parameters)
                    parameter_to_lambda[parameter] = lambda;
                base.VisitLambda(lambda);
            }

            protected override void VisitParameter(ParameterExpression parameter)
            {
                if (lambda.Parameters.Contains(parameter))
                    return;

                Hoist(parameter);
            }

            private void Hoist(ParameterExpression parameter)
            {
                LambdaExpression lambda;
                if (!parameter_to_lambda.TryGetValue(parameter, out lambda))
                    return;

                if (hoisted_map == null)
                    hoisted_map = new Dictionary<LambdaExpression, List<ParameterExpression>>();

                List<ParameterExpression> hoisted;
                if (!hoisted_map.TryGetValue(lambda, out hoisted))
                {
                    hoisted = new List<ParameterExpression>();
                    hoisted_map[lambda] = hoisted;
                }

                hoisted.Add(parameter);
            }
        }

        private class ParameterReplacer : ExpressionTransformer
        {
            private CompilationContext context;
            private object[] locals;
            private ExecutionScope scope;
            public ParameterReplacer(CompilationContext context, ExecutionScope scope, object[] locals)
            {
                this.context = context;
                this.scope = scope;
                this.locals = locals;
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                var scope = this.scope;
                var locals = this.locals;

                while (scope != null)
                {
                    int position = IndexOfHoistedLocal(scope, parameter);
                    if (position != -1)
                        return ReadHoistedLocalFromArray(locals, position);

                    locals = scope.Locals;
                    scope = scope.Parent;
                }

                return parameter;
            }

            private int IndexOfHoistedLocal(ExecutionScope scope, ParameterExpression parameter)
            {
                return context.units[scope.compilation_unit].IndexOfHoistedLocal(parameter);
            }

            private Expression ReadHoistedLocalFromArray(object[] locals, int position)
            {
                return Expression.Field(
                    Expression.Convert(
                        Expression.ArrayIndex(
                            Expression.Constant(locals),
                            Expression.Constant(position)),
                        locals[position].GetType()),
                    "Value");
            }
        }
    }

    internal class EmitContext
    {
        public readonly ILGenerator ig;
        private CompilationContext context;
        private List<ParameterExpression> hoisted;
        private LocalBuilder hoisted_store;
        private LambdaExpression lambda;
        private DynamicMethod method;
        private EmitContext parent;
        public EmitContext(CompilationContext context, EmitContext parent, LambdaExpression lambda)
        {
            this.context = context;
            this.parent = parent;
            this.lambda = lambda;
            this.hoisted = context.GetHoistedLocals(lambda);

            method = new DynamicMethod(
                "lambda_method",
                lambda.GetReturnType(),
                CreateParameterTypes(lambda.Parameters),
                typeof(ExecutionScope),
                true);

            ig = method.GetILGenerator();
        }

        public bool HasHoistedLocals
        {
            get { return hoisted != null && hoisted.Count > 0; }
        }

        public LambdaExpression Lambda
        {
            get { return lambda; }
        }
        public Delegate CreateDelegate(ExecutionScope scope)
        {
            return method.CreateDelegate(lambda.Type, scope);
        }

        public void Emit()
        {
            if (HasHoistedLocals)
                EmitStoreHoistedLocals();

            lambda.EmitBody(this);
        }

        public void Emit(Expression expression)
        {
            expression.Emit(this);
        }

        public void EmitCall(LocalBuilder local, IList<Expression> arguments, MethodInfo method)
        {
            EmitLoadSubject(local);
            EmitArguments(method, arguments);
            EmitCall(method);
        }

        public void EmitCall(LocalBuilder local, MethodInfo method)
        {
            EmitLoadSubject(local);
            EmitCall(method);
        }

        public void EmitCall(Expression expression, MethodInfo method)
        {
            if (!method.IsStatic)
                EmitLoadSubject(expression);

            EmitCall(method);
        }

        public void EmitCall(Expression expression, IList<Expression> arguments, MethodInfo method)
        {
            if (!method.IsStatic)
                EmitLoadSubject(expression);

            EmitArguments(method, arguments);
            EmitCall(method);
        }

        public void EmitCall(MethodInfo method)
        {
            ig.Emit(
                method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                method);
        }

        public void EmitCollection<T>(IEnumerable<T> collection) where T : Expression
        {
            foreach (var expression in collection)
                expression.Emit(this);
        }

        public void EmitCollection(IEnumerable<ElementInit> initializers, LocalBuilder local)
        {
            foreach (var initializer in initializers)
                initializer.Emit(this, local);
        }

        public void EmitCollection(IEnumerable<MemberBinding> bindings, LocalBuilder local)
        {
            foreach (var binding in bindings)
                binding.Emit(this, local);
        }

        public void EmitCreateDelegate(LambdaExpression lambda)
        {
            EmitScope();

            ig.Emit(OpCodes.Ldc_I4, AddChildContext(lambda));
            if (hoisted_store != null)
                ig.Emit(OpCodes.Ldloc, hoisted_store);
            else
                ig.Emit(OpCodes.Ldnull);

            ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateDelegate"));

            ig.Emit(OpCodes.Castclass, lambda.Type);
        }

        public void EmitIsInst(Expression expression, Type candidate)
        {
            expression.Emit(this);

            var type = expression.Type;

            if (type.IsValueType)
                ig.Emit(OpCodes.Box, type);

            ig.Emit(OpCodes.Isinst, candidate);
        }

        public void EmitIsolateExpression()
        {
            ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("IsolateExpression"));
        }

        public void EmitLoad(LocalBuilder local)
        {
            ig.Emit(OpCodes.Ldloc, local);
        }

        public void EmitLoadAddress(Expression expression)
        {
            ig.Emit(OpCodes.Ldloca, EmitStored(expression));
        }

        public void EmitLoadAddress(LocalBuilder local)
        {
            ig.Emit(OpCodes.Ldloca, local);
        }

        public void EmitLoadEnum(Expression expression)
        {
            expression.Emit(this);
            ig.Emit(OpCodes.Box, expression.Type);
        }

        public void EmitLoadEnum(LocalBuilder local)
        {
            ig.Emit(OpCodes.Ldloc, local);
            ig.Emit(OpCodes.Box, local.LocalType);
        }

        public void EmitLoadGlobals()
        {
            EmitScope();

            ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Globals"));
        }

        public void EmitLoadHoistedLocalsStore()
        {
            ig.Emit(OpCodes.Ldloc, hoisted_store);
        }

        public void EmitLoadLocals()
        {
            ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Locals"));
        }

        public void EmitLoadStrongBoxValue(Type type)
        {
            var strongbox = type.MakeStrongBoxType();

            ig.Emit(OpCodes.Isinst, strongbox);
            ig.Emit(OpCodes.Ldfld, strongbox.GetField("Value"));
        }

        public void EmitLoadSubject(Expression expression)
        {
            if (expression.Type.IsEnum)
            {
                EmitLoadEnum(expression);
                return;
            }

            if (expression.Type.IsValueType)
            {
                EmitLoadAddress(expression);
                return;
            }

            Emit(expression);
        }

        public void EmitLoadSubject(LocalBuilder local)
        {
            if (local.LocalType.IsEnum)
            {
                EmitLoadEnum(local);
                return;
            }

            if (local.LocalType.IsValueType)
            {
                EmitLoadAddress(local);
                return;
            }

            EmitLoad(local);
        }

        public void EmitNullableGetValue(LocalBuilder local)
        {
            EmitCall(local, "get_Value");
        }

        public void EmitNullableGetValueOrDefault(LocalBuilder local)
        {
            EmitCall(local, "GetValueOrDefault");
        }

        public void EmitNullableHasValue(LocalBuilder local)
        {
            EmitCall(local, "get_HasValue");
        }

        public void EmitNullableInitialize(LocalBuilder local)
        {
            ig.Emit(OpCodes.Ldloca, local);
            ig.Emit(OpCodes.Initobj, local.LocalType);
            ig.Emit(OpCodes.Ldloc, local);
        }

        public void EmitNullableNew(Type of)
        {
            ig.Emit(OpCodes.Newobj, of.GetConstructor(new[] { of.GetFirstGenericArgument() }));
        }

        public void EmitParentScope()
        {
            ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Parent"));
        }

        public void EmitReadGlobal(object global)
        {
            EmitReadGlobal(global, global.GetType());
        }

        public void EmitReadGlobal(object global, Type type)
        {
            EmitLoadGlobals();

            ig.Emit(OpCodes.Ldc_I4, AddGlobal(global, type));
            ig.Emit(OpCodes.Ldelem, typeof(object));

            EmitLoadStrongBoxValue(type);
        }

        public void EmitScope()
        {
            ig.Emit(OpCodes.Ldarg_0);
        }

        public LocalBuilder EmitStored(Expression expression)
        {
            var local = ig.DeclareLocal(expression.Type);
            expression.Emit(this);
            ig.Emit(OpCodes.Stloc, local);

            return local;
        }

        public int IndexOfHoistedLocal(ParameterExpression parameter)
        {
            if (!HasHoistedLocals)
                return -1;

            return hoisted.IndexOf(parameter);
        }

        public bool IsHoistedLocal(ParameterExpression parameter, ref int level, ref int position)
        {
            if (parent == null)
                return false;

            if (parent.hoisted != null)
            {
                position = parent.hoisted.IndexOf(parameter);
                if (position > -1)
                    return true;
            }

            level++;

            return parent.IsHoistedLocal(parameter, ref level, ref position);
        }

        public bool IsLocalParameter(ParameterExpression parameter, ref int position)
        {
            position = lambda.Parameters.IndexOf(parameter);
            if (position > -1)
            {
                position++;
                return true;
            }

            return false;
        }

        private static Type[] CreateParameterTypes(IList<ParameterExpression> parameters)
        {
            var types = new Type[parameters.Count + 1];
            types[0] = typeof(ExecutionScope);

            for (int i = 0; i < parameters.Count; i++)
                types[i + 1] = parameters[i].Type;

            return types;
        }
        private static object CreateStrongBox(object value, Type type)
        {
            return Activator.CreateInstance(
                type.MakeStrongBoxType(), value);
        }

        private int AddChildContext(LambdaExpression lambda)
        {
            return context.AddCompilationUnit(this, lambda);
        }

        private int AddGlobal(object value, Type type)
        {
            return context.AddGlobal(CreateStrongBox(value, type));
        }

        private void EmitArguments(MethodInfo method, IList<Expression> arguments)
        {
            var parameters = method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];

                if (parameter.ParameterType.IsByRef)
                {
                    ig.Emit(OpCodes.Ldloca, EmitStored(argument));
                    continue;
                }

                Emit(arguments[i]);
            }
        }
        private void EmitCall(LocalBuilder local, string method_name)
        {
            EmitCall(local, local.LocalType.GetMethod(method_name, Type.EmptyTypes));
        }
        private void EmitCreateStrongBox(Type type)
        {
            ig.Emit(OpCodes.Newobj, type.MakeStrongBoxType().GetConstructor(new[] { type }));
        }

        private void EmitHoistedLocalsStore()
        {
            EmitScope();
            hoisted_store = ig.DeclareLocal(typeof(object[]));
            ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateHoistedLocals"));
            ig.Emit(OpCodes.Stloc, hoisted_store);
        }

        private void EmitStoreHoistedLocal(int position, ParameterExpression parameter)
        {
            ig.Emit(OpCodes.Ldloc, hoisted_store);
            ig.Emit(OpCodes.Ldc_I4, position);
            parameter.Emit(this);
            EmitCreateStrongBox(parameter.Type);
            ig.Emit(OpCodes.Stelem, typeof(object));
        }

        private void EmitStoreHoistedLocals()
        {
            EmitHoistedLocalsStore();
            for (int i = 0; i < hoisted.Count; i++)
                EmitStoreHoistedLocal(i, hoisted[i]);
        }
    }
}

#endif