#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
    internal static partial class CachedReflectionInfo
    {
        private static MethodInfo _callSiteOpsAddRule;
        private static MethodInfo _callSiteOpsBind;
        private static MethodInfo _callSiteOpsClearMatch;
        private static MethodInfo _callSiteOpsCreateMatchmaker;
        private static MethodInfo _callSiteOpsGetCachedRules;
        private static MethodInfo _callSiteOpsGetMatch;
        private static MethodInfo _callSiteOpsGetRuleCache;
        private static MethodInfo _callSiteOpsGetRules;
        private static MethodInfo _callSiteOpsMoveRule;
        private static MethodInfo _callSiteOpsSetNotMatched;
        private static MethodInfo _callSiteOpsUpdateRules;
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

        public static MethodInfo CallSiteOpsAddRule =>
                                  _callSiteOpsAddRule ??
                                 (_callSiteOpsAddRule = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.AddRule)));

        public static MethodInfo CallSiteOpsBind =>
                                  _callSiteOpsBind ??
                                 (_callSiteOpsBind = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.Bind)));

        public static MethodInfo CallSiteOpsClearMatch =>
                                  _callSiteOpsClearMatch ??
                                 (_callSiteOpsClearMatch = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.ClearMatch)));

        public static MethodInfo CallSiteOpsCreateMatchmaker =>
                                  _callSiteOpsCreateMatchmaker ??
                                 (_callSiteOpsCreateMatchmaker = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.CreateMatchmaker)));

        public static MethodInfo CallSiteOpsGetCachedRules =>
                                  _callSiteOpsGetCachedRules ??
                                 (_callSiteOpsGetCachedRules = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.GetCachedRules)));

        public static MethodInfo CallSiteOpsGetMatch =>
                                  _callSiteOpsGetMatch ??
                                 (_callSiteOpsGetMatch = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.GetMatch)));

        public static MethodInfo CallSiteOpsGetRuleCache =>
                                  _callSiteOpsGetRuleCache ??
                                 (_callSiteOpsGetRuleCache = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.GetRuleCache)));

        public static MethodInfo CallSiteOpsGetRules =>
                                  _callSiteOpsGetRules ??
                                 (_callSiteOpsGetRules = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.GetRules)));

        public static MethodInfo CallSiteOpsMoveRule =>
                                  _callSiteOpsMoveRule ??
                                 (_callSiteOpsMoveRule = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.MoveRule)));

        public static MethodInfo CallSiteOpsSetNotMatched =>
                                  _callSiteOpsSetNotMatched ??
                                 (_callSiteOpsSetNotMatched = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.SetNotMatched)));

        public static MethodInfo CallSiteOpsUpdateRules =>
                                  _callSiteOpsUpdateRules ??
                                 (_callSiteOpsUpdateRules = typeof(CallSiteOps).GetMethod(nameof(CallSiteOps.UpdateRules)));

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
                                      (_invalidCastExceptionCtorString = typeof(InvalidCastException).GetConstructor(new[] { typeof(string) }));

        public static MethodInfo StringFormatStringObjectArray =>
                                                                                                                                                                                                                                  _stringFormatStringObjectArray ??
                                 (_stringFormatStringObjectArray = typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object[]) }));
    }
}

#endif