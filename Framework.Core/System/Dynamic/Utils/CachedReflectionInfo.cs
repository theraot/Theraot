#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic;
using System.Reflection;

namespace System.Linq.Expressions
{
    internal static partial class CachedReflectionInfo
    {
        private static MethodInfo _dynamicObjectTryBinaryOperation;
        private static MethodInfo _dynamicObjectTryConvert;
        private static MethodInfo _dynamicObjectTryCreateInstance;
        private static MethodInfo _dynamicObjectTryDeleteIndex;
        private static MethodInfo _dynamicObjectTryDeleteMember;
        private static MethodInfo _dynamicObjectTryGetIndex;
        private static MethodInfo _dynamicObjectTryGetMember;
        private static MethodInfo _dynamicObjectTryInvoke;
        private static MethodInfo _dynamicObjectTryInvokeMember;
        private static MethodInfo _dynamicObjectTrySetIndex;
        private static MethodInfo _dynamicObjectTrySetMember;
        private static MethodInfo _dynamicObjectTryUnaryOperation;
        private static ConstructorInfo _invalidCastExceptionCtorString;
        private static MethodInfo _stringFormatStringObjectArray;

        public static MethodInfo DynamicObjectTryBinaryOperation =>
            _dynamicObjectTryBinaryOperation ??
            (_dynamicObjectTryBinaryOperation = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryBinaryOperation)));

        public static MethodInfo DynamicObjectTryConvert =>
            _dynamicObjectTryConvert ??
            (_dynamicObjectTryConvert = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryConvert)));

        public static MethodInfo DynamicObjectTryCreateInstance =>
            _dynamicObjectTryCreateInstance ??
            (_dynamicObjectTryCreateInstance = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryCreateInstance)));

        public static MethodInfo DynamicObjectTryDeleteIndex =>
            _dynamicObjectTryDeleteIndex ??
            (_dynamicObjectTryDeleteIndex = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryDeleteIndex)));

        public static MethodInfo DynamicObjectTryDeleteMember =>
            _dynamicObjectTryDeleteMember ??
            (_dynamicObjectTryDeleteMember = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryDeleteMember)));

        public static MethodInfo DynamicObjectTryGetIndex =>
            _dynamicObjectTryGetIndex ??
            (_dynamicObjectTryGetIndex = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryGetIndex)));

        public static MethodInfo DynamicObjectTryGetMember =>
            _dynamicObjectTryGetMember ??
            (_dynamicObjectTryGetMember = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryGetMember)));

        public static MethodInfo DynamicObjectTryInvoke =>
            _dynamicObjectTryInvoke ??
            (_dynamicObjectTryInvoke = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryInvoke)));

        public static MethodInfo DynamicObjectTryInvokeMember =>
            _dynamicObjectTryInvokeMember ??
            (_dynamicObjectTryInvokeMember = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryInvokeMember)));

        public static MethodInfo DynamicObjectTrySetIndex =>
            _dynamicObjectTrySetIndex ??
            (_dynamicObjectTrySetIndex = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TrySetIndex)));

        public static MethodInfo DynamicObjectTrySetMember =>
            _dynamicObjectTrySetMember ??
            (_dynamicObjectTrySetMember = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TrySetMember)));

        public static MethodInfo DynamicObjectTryUnaryOperation =>
            _dynamicObjectTryUnaryOperation ??
            (_dynamicObjectTryUnaryOperation = typeof(DynamicObject).GetMethod(nameof(DynamicObject.TryUnaryOperation)));

        public static ConstructorInfo InvalidCastExceptionCtorString =>
            _invalidCastExceptionCtorString ??
            (_invalidCastExceptionCtorString = typeof(InvalidCastException).GetConstructor(new[] {typeof(string)}));

        public static MethodInfo StringFormatStringObjectArray =>
            _stringFormatStringObjectArray ??
            (_stringFormatStringObjectArray = typeof(string).GetMethod(nameof(string.Format), new[] {typeof(string), typeof(object[])}));
    }
}

#endif