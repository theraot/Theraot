#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal static partial class CachedReflectionInfo
    {
        private static FieldInfo? _closureConstants;
        private static FieldInfo? _closureLocals;
        private static ConstructorInfo? _closureObjectArrayObjectArray;
        private static FieldInfo? _dateTimeMinValue;
        private static ConstructorInfo? _decimalCtorInt32;
        private static ConstructorInfo? _decimalCtorInt32Int32Int32BoolByte;
        private static ConstructorInfo? _decimalCtorInt64;
        private static ConstructorInfo? _decimalCtorUInt32;
        private static ConstructorInfo? _decimalCtorUInt64;
        private static FieldInfo? _decimalMaxValue;
        private static FieldInfo? _decimalMinusOne;
        private static FieldInfo? _decimalMinValue;
        private static FieldInfo? _decimalOne;
        private static MethodInfo? _decimalOpImplicitByte;
        private static MethodInfo? _decimalOpImplicitChar;
        private static MethodInfo? _decimalOpImplicitInt16;
        private static MethodInfo? _decimalOpImplicitInt32;
        private static MethodInfo? _decimalOpImplicitInt64;
        private static MethodInfo? _decimalOpImplicitSByte;
        private static MethodInfo? _decimalOpImplicitUInt16;
        private static MethodInfo? _decimalOpImplicitUInt32;
        private static MethodInfo? _decimalOpImplicitUInt64;
        private static FieldInfo? _decimalZero;
        private static MethodInfo? _dictionaryOfStringInt32AddStringInt32;
        private static ConstructorInfo? _dictionaryOfStringInt32CtorInt32;
        private static MethodInfo? _mathPowDoubleDouble;
        private static MethodInfo? _methodBaseGetMethodFromHandleRuntimeMethodHandle;
        private static MethodInfo? _methodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle;
        private static ConstructorInfo? _nullableBooleanCtor;
        private static MethodInfo? _objectGetType;
        private static MethodInfo? _runtimeOpsCreateRuntimeVariables;
        private static MethodInfo? _runtimeOpsCreateRuntimeVariablesObjectArrayInt64Array;
        private static MethodInfo? _runtimeOpsMergeRuntimeVariables;
        private static MethodInfo? _runtimeOpsQuote;
        private static MethodInfo? _stringEqualsStringString;
        private static MethodInfo? _stringOpEqualityStringString;
        private static MethodInfo? _typeGetTypeFromHandle;

        public static FieldInfo ClosureConstants =>
            _closureConstants ??= typeof(Closure).GetField(nameof(Closure.Constants));

        public static FieldInfo ClosureLocals =>
            _closureLocals ??= typeof(Closure).GetField(nameof(Closure.Locals));

        public static ConstructorInfo ClosureObjectArrayObjectArray =>
            _closureObjectArrayObjectArray ??= typeof(Closure).GetConstructor(new[] { typeof(object[]), typeof(object[]) });

        public static FieldInfo DateTimeMinValue
            => _dateTimeMinValue ??= typeof(DateTime).GetField(nameof(DateTime.MinValue));

        public static ConstructorInfo DecimalCtorInt32 =>
            _decimalCtorInt32 ??= typeof(decimal).GetConstructor(new[] { typeof(int) });

        public static ConstructorInfo DecimalCtorInt32Int32Int32BoolByte =>
            _decimalCtorInt32Int32Int32BoolByte ??= typeof(decimal).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) });

        public static ConstructorInfo DecimalCtorInt64 =>
            _decimalCtorInt64 ??= typeof(decimal).GetConstructor(new[] { typeof(long) });

        public static ConstructorInfo DecimalCtorUInt32 =>
            _decimalCtorUInt32 ??= typeof(decimal).GetConstructor(new[] { typeof(uint) });

        public static ConstructorInfo DecimalCtorUInt64 =>
            _decimalCtorUInt64 ??= typeof(decimal).GetConstructor(new[] { typeof(ulong) });

        public static FieldInfo DecimalMaxValue
            => _decimalMaxValue ??= typeof(decimal).GetField(nameof(decimal.MaxValue));

        public static FieldInfo DecimalMinusOne
            => _decimalMinusOne ??= typeof(decimal).GetField(nameof(decimal.MinusOne));

        public static FieldInfo DecimalMinValue
            => _decimalMinValue ??= typeof(decimal).GetField(nameof(decimal.MinValue));

        public static FieldInfo DecimalOne
            => _decimalOne ??= typeof(decimal).GetField(nameof(decimal.One));

        public static MethodInfo DecimalOpImplicitByte =>
            _decimalOpImplicitByte ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(byte) });

        public static MethodInfo DecimalOpImplicitChar =>
            _decimalOpImplicitChar ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(char) });

        public static MethodInfo DecimalOpImplicitInt16 =>
            _decimalOpImplicitInt16 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(short) });

        public static MethodInfo DecimalOpImplicitInt32 =>
            _decimalOpImplicitInt32 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(int) });

        public static MethodInfo DecimalOpImplicitInt64 =>
            _decimalOpImplicitInt64 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(long) });

        public static MethodInfo DecimalOpImplicitSByte =>
            _decimalOpImplicitSByte ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(sbyte) });

        public static MethodInfo DecimalOpImplicitUInt16 =>
            _decimalOpImplicitUInt16 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(ushort) });

        public static MethodInfo DecimalOpImplicitUInt32 =>
            _decimalOpImplicitUInt32 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(uint) });

        public static MethodInfo DecimalOpImplicitUInt64 =>
            _decimalOpImplicitUInt64 ??= typeof(decimal).GetMethod("op_Implicit", new[] { typeof(ulong) });

        public static FieldInfo DecimalZero
            => _decimalZero ??= typeof(decimal).GetField(nameof(decimal.Zero));

        public static MethodInfo DictionaryOfStringInt32AddStringInt32 =>
            _dictionaryOfStringInt32AddStringInt32 ??= typeof(Dictionary<string, int>).GetMethod(nameof(Dictionary<string, int>.Add), new[] { typeof(string), typeof(int) });

        public static ConstructorInfo DictionaryOfStringInt32CtorInt32 =>
            _dictionaryOfStringInt32CtorInt32 ??= typeof(Dictionary<string, int>).GetConstructor(new[] { typeof(int) });

        public static MethodInfo MathPowDoubleDouble =>
            _mathPowDoubleDouble ??= typeof(Math).GetMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) });

        public static MethodInfo MethodBaseGetMethodFromHandleRuntimeMethodHandle =>
            _methodBaseGetMethodFromHandleRuntimeMethodHandle ??= typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) });

        public static MethodInfo MethodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle =>
            _methodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle ??= typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });

        public static ConstructorInfo NullableBooleanCtor =>
            _nullableBooleanCtor ??= typeof(bool?).GetConstructor(new[] { typeof(bool) });

        public static MethodInfo ObjectGetType =>
            _objectGetType ??= typeof(object).GetMethod(nameof(GetType));

        public static MethodInfo RuntimeOpsCreateRuntimeVariables =>
            _runtimeOpsCreateRuntimeVariables ??= typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.CreateRuntimeVariables), Type.EmptyTypes);

        public static MethodInfo RuntimeOpsCreateRuntimeVariablesObjectArrayInt64Array =>
            _runtimeOpsCreateRuntimeVariablesObjectArrayInt64Array ??= typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.CreateRuntimeVariables), new[] { typeof(object[]), typeof(long[]) });

        public static MethodInfo RuntimeOpsMergeRuntimeVariables =>
            _runtimeOpsMergeRuntimeVariables ??= typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.MergeRuntimeVariables));

        public static MethodInfo RuntimeOpsQuote =>
            _runtimeOpsQuote ??= typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.Quote));

        public static MethodInfo StringEqualsStringString =>
            _stringEqualsStringString ??= typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) });

        public static MethodInfo StringOpEqualityStringString =>
            _stringOpEqualityStringString ??= typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) });

        public static MethodInfo TypeGetTypeFromHandle =>
            _typeGetTypeFromHandle ??= typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle));
    }

    internal static partial class CachedReflectionInfo
    {
        private static MethodInfo? _dynamicObjectTryBinaryOperation;
        private static MethodInfo? _dynamicObjectTryConvert;
        private static MethodInfo? _dynamicObjectTryCreateInstance;
        private static MethodInfo? _dynamicObjectTryDeleteIndex;
        private static MethodInfo? _dynamicObjectTryDeleteMember;
        private static MethodInfo? _dynamicObjectTryGetIndex;
        private static MethodInfo? _dynamicObjectTryGetMember;
        private static MethodInfo? _dynamicObjectTryInvoke;
        private static MethodInfo? _dynamicObjectTryInvokeMember;
        private static MethodInfo? _dynamicObjectTrySetIndex;
        private static MethodInfo? _dynamicObjectTrySetMember;
        private static MethodInfo? _dynamicObjectTryUnaryOperation;
        private static ConstructorInfo? _invalidCastExceptionCtorString;
        private static MethodInfo? _stringFormatStringObjectArray;

        public static MethodInfo DynamicObjectTryBinaryOperation =>
            _dynamicObjectTryBinaryOperation ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryBinaryOperation));

        public static MethodInfo DynamicObjectTryConvert =>
            _dynamicObjectTryConvert ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryConvert));

        public static MethodInfo DynamicObjectTryCreateInstance =>
            _dynamicObjectTryCreateInstance ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryCreateInstance));

        public static MethodInfo DynamicObjectTryDeleteIndex =>
            _dynamicObjectTryDeleteIndex ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryDeleteIndex));

        public static MethodInfo DynamicObjectTryDeleteMember =>
            _dynamicObjectTryDeleteMember ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryDeleteMember));

        public static MethodInfo DynamicObjectTryGetIndex =>
            _dynamicObjectTryGetIndex ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryGetIndex));

        public static MethodInfo DynamicObjectTryGetMember =>
            _dynamicObjectTryGetMember ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryGetMember));

        public static MethodInfo DynamicObjectTryInvoke =>
            _dynamicObjectTryInvoke ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryInvoke));

        public static MethodInfo DynamicObjectTryInvokeMember =>
            _dynamicObjectTryInvokeMember ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryInvokeMember));

        public static MethodInfo DynamicObjectTrySetIndex =>
            _dynamicObjectTrySetIndex ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TrySetIndex));

        public static MethodInfo DynamicObjectTrySetMember =>
            _dynamicObjectTrySetMember ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TrySetMember));

        public static MethodInfo DynamicObjectTryUnaryOperation =>
            _dynamicObjectTryUnaryOperation ??= typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryUnaryOperation));

        public static ConstructorInfo InvalidCastExceptionCtorString =>
            _invalidCastExceptionCtorString ??= typeof(InvalidCastException).GetConstructor(new[] { typeof(string) });

        public static MethodInfo StringFormatStringObjectArray =>
            _stringFormatStringObjectArray ??= typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object[]) });
    }
}

#endif