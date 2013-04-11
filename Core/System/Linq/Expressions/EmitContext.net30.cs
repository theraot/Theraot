#if NET20 || NET30

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal class EmitContext
    {
        public readonly ILGenerator ILGenerator;

        private CompilationContext _context;
        private List<ParameterExpression> _hoisted;
        private LocalBuilder _hoistedStore;
        private LambdaExpression _lambda;
        private DynamicMethod _method;
        private EmitContext _parent;

        public EmitContext(CompilationContext context, EmitContext parent, LambdaExpression lambda)
        {
            this._context = context;
            this._parent = parent;
            this._lambda = lambda;
            _hoisted = context.GetHoistedLocals(lambda);
            _method = new DynamicMethod
            (
                "lambda_method",
                lambda.GetReturnType(),
                CreateParameterTypes(lambda.Parameters),
                typeof(ExecutionScope),
                true
            );
            ILGenerator = _method.GetILGenerator();
        }

        public bool HasHoistedLocals
        {
            get
            {
                return _hoisted != null && _hoisted.Count > 0;
            }
        }

        public LambdaExpression Lambda
        {
            get
            {
                return _lambda;
            }
        }

        public Delegate CreateDelegate(ExecutionScope scope)
        {
            return _method.CreateDelegate(_lambda.Type, scope);
        }

        public void Emit()
        {
            if (HasHoistedLocals)
            {
                EmitStoreHoistedLocals();
            }
            _lambda.EmitBody(this);
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
            {
                EmitLoadSubject(expression);
            }
            EmitCall(method);
        }

        public void EmitCall(Expression expression, IList<Expression> arguments, MethodInfo method)
        {
            if (!method.IsStatic)
            {
                EmitLoadSubject(expression);
            }
            EmitArguments(method, arguments);
            EmitCall(method);
        }

        public void EmitCall(MethodInfo method)
        {
            ILGenerator.Emit(
                method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                method);
        }

        public void EmitCollection<T>(IEnumerable<T> collection)
            where T : Expression
        {
            foreach (var expression in collection)
            {
                expression.Emit(this);
            }
        }

        public void EmitCollection(IEnumerable<ElementInit> initializers, LocalBuilder local)
        {
            foreach (var initializer in initializers)
            {
                initializer.Emit(this, local);
            }
        }

        public void EmitCollection(IEnumerable<MemberBinding> bindings, LocalBuilder local)
        {
            foreach (var binding in bindings)
            {
                binding.Emit(this, local);
            }
        }

        public void EmitCreateDelegate(LambdaExpression lambda)
        {
            EmitScope();
            ILGenerator.Emit(OpCodes.Ldc_I4, AddChildContext(lambda));
            if (_hoistedStore != null)
            {
                ILGenerator.Emit(OpCodes.Ldloc, _hoistedStore);
            }
            else
            {
                ILGenerator.Emit(OpCodes.Ldnull);
            }
            ILGenerator.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateDelegate"));
            ILGenerator.Emit(OpCodes.Castclass, lambda.Type);
        }

        public void EmitIsInst(Expression expression, Type candidate)
        {
            expression.Emit(this);
            var type = expression.Type;
            if (type.IsValueType)
            {
                ILGenerator.Emit(OpCodes.Box, type);
            }
            ILGenerator.Emit(OpCodes.Isinst, candidate);
        }

        public void EmitIsolateExpression()
        {
            ILGenerator.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("IsolateExpression"));
        }

        public void EmitLoad(LocalBuilder local)
        {
            ILGenerator.Emit(OpCodes.Ldloc, local);
        }

        public void EmitLoadAddress(Expression expression)
        {
            ILGenerator.Emit(OpCodes.Ldloca, EmitStored(expression));
        }

        public void EmitLoadAddress(LocalBuilder local)
        {
            ILGenerator.Emit(OpCodes.Ldloca, local);
        }

        public void EmitLoadEnum(Expression expression)
        {
            expression.Emit(this);
            ILGenerator.Emit(OpCodes.Box, expression.Type);
        }

        public void EmitLoadEnum(LocalBuilder local)
        {
            ILGenerator.Emit(OpCodes.Ldloc, local);
            ILGenerator.Emit(OpCodes.Box, local.LocalType);
        }

        public void EmitLoadGlobals()
        {
            EmitScope();
            ILGenerator.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Globals"));
        }

        public void EmitLoadHoistedLocalsStore()
        {
            ILGenerator.Emit(OpCodes.Ldloc, _hoistedStore);
        }

        public void EmitLoadLocals()
        {
            ILGenerator.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Locals"));
        }

        public void EmitLoadStrongBoxValue(Type type)
        {
            var strongbox = type.MakeStrongBoxType();
            ILGenerator.Emit(OpCodes.Isinst, strongbox);
            ILGenerator.Emit(OpCodes.Ldfld, strongbox.GetField("Value"));
        }

        public void EmitLoadSubject(Expression expression)
        {
            if (expression.Type.IsEnum)
            {
                EmitLoadEnum(expression);
                return;
            }
            else if (expression.Type.IsValueType)
            {
                EmitLoadAddress(expression);
                return;
            }
            else
            {
                Emit(expression);
            }
        }

        public void EmitLoadSubject(LocalBuilder local)
        {
            if (local.LocalType.IsEnum)
            {
                EmitLoadEnum(local);
                return;
            }
            else if (local.LocalType.IsValueType)
            {
                EmitLoadAddress(local);
                return;
            }
            else
            {
                EmitLoad(local);
            }
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
            ILGenerator.Emit(OpCodes.Ldloca, local);
            ILGenerator.Emit(OpCodes.Initobj, local.LocalType);
            ILGenerator.Emit(OpCodes.Ldloc, local);
        }

        public void EmitNullableNew(Type of)
        {
            ILGenerator.Emit(OpCodes.Newobj, of.GetConstructor(new[] { of.GetFirstGenericArgument() }));
        }

        public void EmitParentScope()
        {
            ILGenerator.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Parent"));
        }

        public void EmitReadGlobal(object global)
        {
            EmitReadGlobal(global, global.GetType());
        }

        public void EmitReadGlobal(object global, Type type)
        {
            EmitLoadGlobals();
            ILGenerator.Emit(OpCodes.Ldc_I4, AddGlobal(global, type));
            ILGenerator.Emit(OpCodes.Ldelem, typeof(object));
            EmitLoadStrongBoxValue(type);
        }

        public void EmitScope()
        {
            ILGenerator.Emit(OpCodes.Ldarg_0);
        }

        public LocalBuilder EmitStored(Expression expression)
        {
            var local = ILGenerator.DeclareLocal(expression.Type);
            expression.Emit(this);
            ILGenerator.Emit(OpCodes.Stloc, local);
            return local;
        }

        public int IndexOfHoistedLocal(ParameterExpression parameter)
        {
            if (!HasHoistedLocals)
            {
                return -1;
            }
            else
            {
                return _hoisted.IndexOf(parameter);
            }
        }

        public bool IsHoistedLocal(ParameterExpression parameter, ref int level, ref int position)
        {
            if (_parent == null)
            {
                return false;
            }
            else
            {
                if (_parent._hoisted != null)
                {
                    position = _parent._hoisted.IndexOf(parameter);
                    if (position > -1)
                    {
                        return true;
                    }
                }
                level++;
                return _parent.IsHoistedLocal(parameter, ref level, ref position);
            }
        }

        public bool IsLocalParameter(ParameterExpression parameter, ref int position)
        {
            position = _lambda.Parameters.IndexOf(parameter);
            if (position > -1)
            {
                position++;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Type[] CreateParameterTypes(IList<ParameterExpression> parameters)
        {
            var types = new Type[parameters.Count + 1];
            types[0] = typeof(ExecutionScope);
            for (int i = 0; i < parameters.Count; i++)
            {
                types[i + 1] = parameters[i].Type;
            }
            return types;
        }

        private static object CreateStrongBox(object value, Type type)
        {
            return Activator.CreateInstance(type.MakeStrongBoxType(), value);
        }

        private int AddChildContext(LambdaExpression lambda)
        {
            return _context.AddCompilationUnit(this, lambda);
        }

        private int AddGlobal(object value, Type type)
        {
            return _context.AddGlobal(CreateStrongBox(value, type));
        }

        private void EmitArguments(MethodInfo method, IList<Expression> arguments)
        {
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.ParameterType.IsByRef)
                {
                    var argument = arguments[i];
                    ILGenerator.Emit(OpCodes.Ldloca, EmitStored(argument));
                    continue;
                }
                Emit(arguments[i]);
            }
        }

        private void EmitCall(LocalBuilder local, string methodName)
        {
            EmitCall(local, local.LocalType.GetMethod(methodName, Type.EmptyTypes));
        }

        private void EmitCreateStrongBox(Type type)
        {
            ILGenerator.Emit(OpCodes.Newobj, type.MakeStrongBoxType().GetConstructor(new[] { type }));
        }

        private void EmitHoistedLocalsStore()
        {
            EmitScope();
            _hoistedStore = ILGenerator.DeclareLocal(typeof(object[]));
            ILGenerator.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateHoistedLocals"));
            ILGenerator.Emit(OpCodes.Stloc, _hoistedStore);
        }

        private void EmitStoreHoistedLocal(int position, Expression parameter)
        {
            ILGenerator.Emit(OpCodes.Ldloc, _hoistedStore);
            ILGenerator.Emit(OpCodes.Ldc_I4, position);
            parameter.Emit(this);
            EmitCreateStrongBox(parameter.Type);
            ILGenerator.Emit(OpCodes.Stelem, typeof(object));
        }

        private void EmitStoreHoistedLocals()
        {
            EmitHoistedLocalsStore();
            for (int i = 0; i < _hoisted.Count; i++)
            {
                EmitStoreHoistedLocal(i, _hoisted[i]);
            }
        }
    }
}

#endif