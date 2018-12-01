#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal static partial class CachedReflectionInfo
    {
        private static FieldInfo _closureConstants;
        private static FieldInfo _closureLocals;

        // Closure and RuntimeOps helpers are used only in the compiler.
        private static ConstructorInfo _closureObjectArrayObjectArray;

        private static FieldInfo _dateTimeMinValue;
        private static ConstructorInfo _decimalCtorInt32;
        private static ConstructorInfo _decimalCtorInt32Int32Int32BoolByte;
        private static ConstructorInfo _decimalCtorInt64;
        private static ConstructorInfo _decimalCtorUInt32;
        private static ConstructorInfo _decimalCtorUInt64;
        private static FieldInfo _decimalMaxValue;
        private static FieldInfo _decimalMinusOne;
        private static FieldInfo _decimalMinValue;
        private static FieldInfo _decimalOne;
        private static MethodInfo _decimalOpImplicitByte;
        private static MethodInfo _decimalOpImplicitChar;
        private static MethodInfo _decimalOpImplicitInt16;
        private static MethodInfo _decimalOpImplicitInt32;
        private static MethodInfo _decimalOpImplicitInt64;
        private static MethodInfo _decimalOpImplicitSByte;
        private static MethodInfo _decimalOpImplicitUInt16;
        private static MethodInfo _decimalOpImplicitUInt32;
        private static MethodInfo _decimalOpImplicitUInt64;
        private static FieldInfo _decimalZero;
        private static MethodInfo _dictionaryOfStringInt32AddStringInt32;
        private static ConstructorInfo _dictionaryOfStringInt32CtorInt32;
        private static MethodInfo _mathPowDoubleDouble;
        private static MethodInfo _methodBaseGetMethodFromHandleRuntimeMethodHandle;
        private static MethodInfo _methodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle;
        private static ConstructorInfo _nullableBooleanCtor;

        private static MethodInfo _objectGetType;

        private static MethodInfo _runtimeOpsCreateRuntimeVariables;

        private static MethodInfo _runtimeOpsCreateRuntimeVariablesObjectArrayInt64Array;

        private static MethodInfo _runtimeOpsMergeRuntimeVariables;

        private static MethodInfo _runtimeOpsQuote;

        private static MethodInfo _stringEqualsStringString;

        private static MethodInfo _stringOpEqualityStringString;

        private static MethodInfo _typeGetTypeFromHandle;

        public static FieldInfo ClosureConstants =>
                                 _closureConstants ??
                                (_closureConstants = typeof(Closure).GetField(nameof(Closure.Constants)));

        public static FieldInfo ClosureLocals =>
                                 _closureLocals ??
                                (_closureLocals = typeof(Closure).GetField(nameof(Closure.Locals)));

        public static ConstructorInfo ClosureObjectArrayObjectArray =>
                                       _closureObjectArrayObjectArray ??
                                      (_closureObjectArrayObjectArray = typeof(Closure).GetConstructor(new[] { typeof(object[]), typeof(object[]) }));

        public static FieldInfo DateTimeMinValue
            => _dateTimeMinValue ?? (_dateTimeMinValue = typeof(DateTime).GetField(nameof(DateTime.MinValue)));

        public static ConstructorInfo DecimalCtorInt32 =>
                                       _decimalCtorInt32 ??
                                      (_decimalCtorInt32 = typeof(decimal).GetConstructor(new[] { typeof(int) }));

        public static ConstructorInfo DecimalCtorInt32Int32Int32BoolByte =>
                                       _decimalCtorInt32Int32Int32BoolByte ??
                                      (_decimalCtorInt32Int32Int32BoolByte = typeof(decimal).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));

        public static ConstructorInfo DecimalCtorInt64 =>
                                       _decimalCtorInt64 ??
                                      (_decimalCtorInt64 = typeof(decimal).GetConstructor(new[] { typeof(long) }));

        public static ConstructorInfo DecimalCtorUInt32 =>
                                       _decimalCtorUInt32 ??
                                      (_decimalCtorUInt32 = typeof(decimal).GetConstructor(new[] { typeof(uint) }));

        public static ConstructorInfo DecimalCtorUInt64 =>
                                       _decimalCtorUInt64 ??
                                      (_decimalCtorUInt64 = typeof(decimal).GetConstructor(new[] { typeof(ulong) }));

        public static FieldInfo DecimalMaxValue
            => _decimalMaxValue ?? (_decimalMaxValue = typeof(decimal).GetField(nameof(decimal.MaxValue)));

        public static FieldInfo DecimalMinusOne
            => _decimalMinusOne ?? (_decimalMinusOne = typeof(decimal).GetField(nameof(decimal.MinusOne)));

        public static FieldInfo DecimalMinValue
            => _decimalMinValue ?? (_decimalMinValue = typeof(decimal).GetField(nameof(decimal.MinValue)));

        public static FieldInfo DecimalOne
            => _decimalOne ?? (_decimalOne = typeof(decimal).GetField(nameof(decimal.One)));

        public static MethodInfo DecimalOpImplicitByte =>
                                  _decimalOpImplicitByte ??
                                 (_decimalOpImplicitByte = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(byte) }));

        public static MethodInfo DecimalOpImplicitChar =>
                                  _decimalOpImplicitChar ??
                                 (_decimalOpImplicitChar = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(char) }));

        public static MethodInfo DecimalOpImplicitInt16 =>
                                  _decimalOpImplicitInt16 ??
                                 (_decimalOpImplicitInt16 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(short) }));

        public static MethodInfo DecimalOpImplicitInt32 =>
                                  _decimalOpImplicitInt32 ??
                                 (_decimalOpImplicitInt32 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(int) }));

        public static MethodInfo DecimalOpImplicitInt64 =>
                                  _decimalOpImplicitInt64 ??
                                 (_decimalOpImplicitInt64 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(long) }));

        public static MethodInfo DecimalOpImplicitSByte =>
                                  _decimalOpImplicitSByte ??
                                 (_decimalOpImplicitSByte = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(sbyte) }));

        public static MethodInfo DecimalOpImplicitUInt16 =>
                                  _decimalOpImplicitUInt16 ??
                                 (_decimalOpImplicitUInt16 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(ushort) }));

        public static MethodInfo DecimalOpImplicitUInt32 =>
                                  _decimalOpImplicitUInt32 ??
                                 (_decimalOpImplicitUInt32 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(uint) }));

        public static MethodInfo DecimalOpImplicitUInt64 =>
                                  _decimalOpImplicitUInt64 ??
                                 (_decimalOpImplicitUInt64 = typeof(decimal).GetMethod("op_Implicit", new[] { typeof(ulong) }));

        public static FieldInfo DecimalZero
            => _decimalZero ?? (_decimalZero = typeof(decimal).GetField(nameof(decimal.Zero)));

        public static MethodInfo DictionaryOfStringInt32AddStringInt32 =>
                                  _dictionaryOfStringInt32AddStringInt32 ??
                                 (_dictionaryOfStringInt32AddStringInt32 = typeof(Dictionary<string, int>).GetMethod(nameof(Dictionary<string, int>.Add), new[] { typeof(string), typeof(int) }));

        public static ConstructorInfo DictionaryOfStringInt32CtorInt32 =>
                                       _dictionaryOfStringInt32CtorInt32 ??
                                      (_dictionaryOfStringInt32CtorInt32 = typeof(Dictionary<string, int>).GetConstructor(new[] { typeof(int) }));

        public static MethodInfo MathPowDoubleDouble =>
                                  _mathPowDoubleDouble ??
                                 (_mathPowDoubleDouble = typeof(Math).GetMethod(nameof(Math.Pow), new[] { typeof(double), typeof(double) }));

        public static MethodInfo MethodBaseGetMethodFromHandleRuntimeMethodHandle =>
                                  _methodBaseGetMethodFromHandleRuntimeMethodHandle ??
                                 (_methodBaseGetMethodFromHandleRuntimeMethodHandle = typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) }));

        public static MethodInfo MethodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle =>
                                  _methodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle ??
                                 (_methodBaseGetMethodFromHandleRuntimeMethodHandleRuntimeTypeHandle = typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }));

        public static ConstructorInfo NullableBooleanCtor
                                                                                                                                                                                                                                                                                                            => _nullableBooleanCtor ?? (_nullableBooleanCtor = typeof(bool?).GetConstructor(new[] { typeof(bool) }));

        /*private static MethodInfo _methodInfoCreateDelegateTypeObject;

        public static MethodInfo MethodInfo_CreateDelegate_Type_Object =>
                                  s_MethodInfo_CreateDelegate_Type_Object ??
                                 (s_MethodInfo_CreateDelegate_Type_Object = typeof(MethodInfo).GetMethod(nameof(MethodInfo.CreateDelegate), new[] { typeof(Type), typeof(object) }));*/

        public static MethodInfo ObjectGetType =>
                                  _objectGetType ??
                                 (_objectGetType = typeof(object).GetMethod(nameof(GetType)));

        public static MethodInfo RuntimeOpsCreateRuntimeVariables =>
                                  _runtimeOpsCreateRuntimeVariables ??
                                 (_runtimeOpsCreateRuntimeVariables = typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.CreateRuntimeVariables), Type.EmptyTypes));

        public static MethodInfo RuntimeOpsCreateRuntimeVariablesObjectArrayInt64Array =>
                                  _runtimeOpsCreateRuntimeVariablesObjectArrayInt64Array ??
                                 (_runtimeOpsCreateRuntimeVariablesObjectArrayInt64Array = typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.CreateRuntimeVariables), new[] { typeof(object[]), typeof(long[]) }));

        public static MethodInfo RuntimeOpsMergeRuntimeVariables =>
                                  _runtimeOpsMergeRuntimeVariables ??
                                 (_runtimeOpsMergeRuntimeVariables = typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.MergeRuntimeVariables)));

        public static MethodInfo RuntimeOpsQuote =>
                                  _runtimeOpsQuote ??
                                 (_runtimeOpsQuote = typeof(RuntimeOps).GetMethod(nameof(RuntimeOps.Quote)));

        public static MethodInfo StringEqualsStringString =>
                                  _stringEqualsStringString ??
                                 (_stringEqualsStringString = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }));

        public static MethodInfo StringOpEqualityStringString =>
                                                                                  _stringOpEqualityStringString ??
                                 (_stringOpEqualityStringString = typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }));

        public static MethodInfo TypeGetTypeFromHandle =>
                                  _typeGetTypeFromHandle ??
                                 (_typeGetTypeFromHandle = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle)));
    }
}

#endif