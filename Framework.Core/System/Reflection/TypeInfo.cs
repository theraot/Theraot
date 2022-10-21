#if LESSTHAN_NET45

#pragma warning disable CA1819 //Properties should not return arrays

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Linq;

namespace System.Reflection
{
    public class TypeInfo : Type, IReflectableType
    {
        private static readonly MethodInfo _methodGetAttributeFlagsImpl =
            typeof(Type).GetMethod(nameof(GetAttributeFlagsImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodGetConstructorImpl =
            typeof(Type).GetMethod(nameof(GetConstructorImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        //string name, MemberTypes type, BindingFlags bindingAttr
        private static readonly MethodInfo _methodGetMember =
            typeof(Type).GetMethod(nameof(GetMember)
                , BindingFlags.Instance | BindingFlags.NonPublic
                , default
                , new[] { typeof(string), typeof(MemberTypes), typeof(BindingFlags) }
                , modifiers: null);

        private static readonly MethodInfo _methodGetMethodImpl =
            typeof(Type).GetMethod(nameof(GetMethodImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodGetPropertyImpl =
            typeof(Type).GetMethod(nameof(GetPropertyImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodHasElementTypeImpl =
            typeof(Type).GetMethod(nameof(HasElementTypeImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsArrayImpl =
            typeof(Type).GetMethod(nameof(IsArrayImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsByRefImpl =
            typeof(Type).GetMethod(nameof(IsByRefImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsComObjectImpl =
            typeof(Type).GetMethod(nameof(IsCOMObjectImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsContextfulImpl =
            typeof(Type).GetMethod(nameof(IsContextfulImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsMarshalByRefImpl =
            typeof(Type).GetMethod(nameof(IsMarshalByRefImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsPointerImpl =
            typeof(Type).GetMethod(nameof(IsPointerImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _methodIsPrimitiveImpl =
            typeof(Type).GetMethod(nameof(IsPrimitiveImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        //IsValueTypeImpl
        private static readonly MethodInfo _methodIsValueTypeImpl =
            typeof(Type).GetMethod(nameof(IsValueTypeImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly Type _type;

        internal TypeInfo(Type type)
        {
            this._type = type;
            DeclaredConstructors = type
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            DeclaredEvents = type
                .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            DeclaredFields = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            DeclaredMembers = type
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            DeclaredMethods = type
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            DeclaredNestedTypes = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Select(nt => new TypeInfo(nt))
                .ToList()
                .AsReadOnly();
            DeclaredProperties = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            GenericTypeParameters = type
                .GetGenericArguments();
            ImplementedInterfaces = type
                .GetInterfaces();
        }

        public override Assembly Assembly => _type.Assembly;
        public override string AssemblyQualifiedName => _type.AssemblyQualifiedName;
        public override Type BaseType => _type.BaseType;
        public override bool ContainsGenericParameters => _type.ContainsGenericParameters;
        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors { get; }
        public virtual IEnumerable<EventInfo> DeclaredEvents { get; }
        public virtual IEnumerable<FieldInfo> DeclaredFields { get; }
        public virtual IEnumerable<MemberInfo> DeclaredMembers { get; }
        public virtual IEnumerable<MethodInfo> DeclaredMethods { get; }
        public virtual IEnumerable<TypeInfo> DeclaredNestedTypes { get; }
        public virtual IEnumerable<PropertyInfo> DeclaredProperties { get; }
        public override MethodBase DeclaringMethod => _type.DeclaringMethod;
        public override Type DeclaringType => _type.DeclaringType;
        public override string FullName => _type.FullName;
        public override GenericParameterAttributes GenericParameterAttributes => _type.GenericParameterAttributes;
        public override int GenericParameterPosition => _type.GenericParameterPosition;
        public IEnumerable<Type> GenericTypeArguments => _type.GetGenericArguments();
        public virtual Type[] GenericTypeParameters { get; }
        public override Guid GUID => _type.GUID;

        public virtual IEnumerable<Type> ImplementedInterfaces { get; }
        public override bool IsGenericParameter => _type.IsGenericParameter;
        public override bool IsGenericType => _type.IsGenericType;
        public override bool IsGenericTypeDefinition => _type.IsGenericTypeDefinition;
        public override MemberTypes MemberType => _type.MemberType;
        public override int MetadataToken => _type.MetadataToken;
        public override Module Module => _type.Module;
        public override string Name => _type.Name;
        public override string Namespace => _type.Namespace;
        public override Type ReflectedType => _type.ReflectedType;
        public override StructLayoutAttribute StructLayoutAttribute => _type.StructLayoutAttribute;
        public override RuntimeTypeHandle TypeHandle => _type.TypeHandle;
        public new ConstructorInfo TypeInitializer => _type.TypeInitializer;
        public override Type UnderlyingSystemType => _type.UnderlyingSystemType;

        public virtual Type AsType()
            => _type;

        public override bool Equals(object o) => _type.Equals(o);

        public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria) => _type.FindInterfaces(filter, filterCriteria);

        public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria) => _type.FindMembers(memberType, bindingAttr, filter, filterCriteria);

        public override int GetArrayRank() => _type.GetArrayRank();

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => _type.GetConstructors(bindingAttr);

        public override object[] GetCustomAttributes(bool inherit) => _type.GetCustomAttributes(inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _type.GetCustomAttributes(attributeType, inherit);

        public virtual EventInfo? GetDeclaredEvent(string name) =>
            DeclaredEvents.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.Ordinal));

        public virtual FieldInfo? GetDeclaredField(string name) =>
            DeclaredFields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.Ordinal));

        public virtual MethodInfo? GetDeclaredMethod(string name) =>
            DeclaredMethods.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.Ordinal));

        public virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name) =>
            DeclaredMethods.Where(m => string.Equals(m.Name, name, StringComparison.Ordinal));

        public virtual TypeInfo? GetDeclaredNestedType(string name) =>
            _type.GetNestedTypes()
                .Where(nt => string.Equals(nt.Name, name, StringComparison.Ordinal))
                .Select(nt => new TypeInfo(nt))
                .FirstOrDefault();

        public virtual PropertyInfo? GetDeclaredProperty(string name) =>
            DeclaredProperties.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.Ordinal));

        public override MemberInfo[] GetDefaultMembers() => _type.GetDefaultMembers();

        public override Type GetElementType() => _type.GetElementType();

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => _type.GetEvent(name, bindingAttr);

        public override EventInfo[] GetEvents(BindingFlags bindingAttr) => _type.GetEvents(bindingAttr);

        public override EventInfo[] GetEvents() => _type.GetEvents();

        public override FieldInfo GetField(string name, BindingFlags bindingAttr) => _type.GetField(name, bindingAttr);

        public override FieldInfo[] GetFields(BindingFlags bindingAttr) => _type.GetFields(bindingAttr);

        public override Type[] GetGenericArguments() => _type.GetGenericArguments();

        public override Type[] GetGenericParameterConstraints() => _type.GetGenericParameterConstraints();

        public override Type GetGenericTypeDefinition() => _type.GetGenericTypeDefinition();

        public override int GetHashCode() => _type.GetHashCode();

        public override Type GetInterface(string name, bool ignoreCase) => _type.GetInterface(name, ignoreCase);

        public override InterfaceMapping GetInterfaceMap(Type interfaceType) => _type.GetInterfaceMap(interfaceType);

        public override Type[] GetInterfaces() => _type.GetInterfaces();

        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr) => _type.GetMember(name, bindingAttr);

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) => (MemberInfo[])_methodGetMember.Invoke(type, new object[] { name, type, bindingAttr });

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => _type.GetMembers(bindingAttr);

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => _type.GetMethods(bindingAttr);

        public override Type GetNestedType(string name, BindingFlags bindingAttr) => _type.GetNestedType(name, bindingAttr);

        public override Type[] GetNestedTypes(BindingFlags bindingAttr) => _type.GetNestedTypes(bindingAttr);

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => _type.GetProperties(bindingAttr);

        public TypeInfo GetTypeInfo() => this;

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => _type.InvokeMember(name, invokeAttr, binder, target, args, CultureInfo.CurrentCulture);

        public override bool IsAssignableFrom(Type c) => _type.IsAssignableFrom(c);

        public virtual bool IsAssignableFrom(TypeInfo? typeInfo) =>
            typeInfo is not null && _type.IsAssignableFrom(typeInfo._type);

        public override bool IsDefined(Type attributeType, bool inherit) => _type.IsDefined(attributeType, inherit);

        public override bool IsInstanceOfType(object o) => _type.IsInstanceOfType(o);

        public override bool IsSubclassOf(Type c) => _type.IsSubclassOf(c);

        public override Type MakeArrayType() => _type.MakeArrayType();

        public override Type MakeArrayType(int rank) => _type.MakeArrayType(rank);

        public override Type MakeByRefType() => _type.MakeByRefType();

        public override Type MakeGenericType(params Type[] typeArguments) => _type.MakeGenericType(typeArguments);

        public override Type MakePointerType() => _type.MakePointerType();

        public override string ToString() => _type.ToString();

        protected override TypeAttributes GetAttributeFlagsImpl() => (TypeAttributes)_methodGetAttributeFlagsImpl.Invoke(_type, parameters: null);

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => (ConstructorInfo)_methodGetConstructorImpl.Invoke(_type, new object[] { bindingAttr, binder, callConvention, _type, modifiers });

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => (MethodInfo)_methodGetMethodImpl.Invoke(_type, new object[] { name, bindingAttr, binder, callConvention, _type, modifiers });

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => (PropertyInfo)_methodGetPropertyImpl.Invoke(_type, new object[] { name, bindingAttr, binder, returnType, _type, modifiers });

        protected override bool HasElementTypeImpl() => (bool)_methodHasElementTypeImpl.Invoke(_type, parameters: null);

        protected override bool IsArrayImpl() => (bool)_methodIsArrayImpl.Invoke(_type, parameters: null);

        protected override bool IsByRefImpl() => (bool)_methodIsByRefImpl.Invoke(_type, parameters: null);

        protected override bool IsCOMObjectImpl() => (bool)_methodIsComObjectImpl.Invoke(_type, parameters: null);

        protected override bool IsContextfulImpl() => (bool)_methodIsContextfulImpl.Invoke(_type, parameters: null);

        protected override bool IsMarshalByRefImpl() => (bool)_methodIsMarshalByRefImpl.Invoke(_type, parameters: null);

        protected override bool IsPointerImpl() => (bool)_methodIsPointerImpl.Invoke(_type, parameters: null);

        protected override bool IsPrimitiveImpl() => (bool)_methodIsPrimitiveImpl.Invoke(_type, parameters: null);

        protected override bool IsValueTypeImpl() => (bool)_methodIsValueTypeImpl.Invoke(_type, parameters: null);
    }
}

#endif