#if LESSTHAN_NET45

namespace System.Reflection
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Linq;

    public class TypeInfo : Type, IReflectableType
    {
        readonly Type type;

        readonly static MethodInfo MethodGetAttributeFlagsImpl =
            typeof(Type).GetMethod(nameof(GetAttributeFlagsImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodGetConstructorImpl =
            typeof(Type).GetMethod(nameof(GetConstructorImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodGetMethodImpl =
            typeof(Type).GetMethod(nameof(GetMethodImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodGetPropertyImpl =
            typeof(Type).GetMethod(nameof(GetPropertyImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodHasElementTypeImpl =
            typeof(Type).GetMethod(nameof(HasElementTypeImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsArrayImpl =
            typeof(Type).GetMethod(nameof(IsArrayImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsByRefImpl =
            typeof(Type).GetMethod(nameof(IsByRefImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsCOMObjectImpl =
            typeof(Type).GetMethod(nameof(IsCOMObjectImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsPointerImpl =
            typeof(Type).GetMethod(nameof(IsPointerImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsPrimitiveImpl =
            typeof(Type).GetMethod(nameof(IsPrimitiveImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        //string name, MemberTypes type, BindingFlags bindingAttr
        readonly static MethodInfo MethodGetMember =
            typeof(Type).GetMethod(nameof(GetMember)
                    , BindingFlags.Instance | BindingFlags.NonPublic
                    , default(Binder)
                    , new[] { typeof(string), typeof(MemberTypes), typeof(BindingFlags) }
                    , null);
        readonly static MethodInfo MethodIsContextfulImpl =
            typeof(Type).GetMethod(nameof(IsContextfulImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        readonly static MethodInfo MethodIsMarshalByRefImpl =
            typeof(Type).GetMethod(nameof(IsMarshalByRefImpl), BindingFlags.Instance | BindingFlags.NonPublic);
        //IsValueTypeImpl
        readonly static MethodInfo MethodIsValueTypeImpl =
            typeof(Type).GetMethod(nameof(IsValueTypeImpl), BindingFlags.Instance | BindingFlags.NonPublic);

        internal TypeInfo(Type type)
        {
            this.type = type;
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
                .Select(nt=>new TypeInfo(nt))
                .ToList()
                .AsReadOnly();
            DeclaredProperties = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            GenericTypeParameters = type
                .GetGenericArguments();
            ImplementedInterfaces = type
                .GetInterfaces();
        }


        public override Guid GUID => type.GUID;

        public override Module Module => type.Module;

        public override Assembly Assembly => type.Assembly;

        public override string FullName => type.FullName;

        public override string Namespace => type.Namespace;

        public override string AssemblyQualifiedName => type.AssemblyQualifiedName;

        public override Type BaseType => type.BaseType;

        public override Type UnderlyingSystemType => type.UnderlyingSystemType;

        public override string Name => type.Name;

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => type.GetConstructors(bindingAttr);

        public override object[] GetCustomAttributes(bool inherit) => type.GetCustomAttributes(inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => type.GetCustomAttributes(attributeType, inherit);

        public override Type GetElementType() => type.GetElementType();

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => type.GetEvent(name, bindingAttr);

        public override EventInfo[] GetEvents(BindingFlags bindingAttr) => type.GetEvents(bindingAttr);

        public override FieldInfo GetField(string name, BindingFlags bindingAttr) => GetField(name, bindingAttr);

        public override FieldInfo[] GetFields(BindingFlags bindingAttr) => GetFields(bindingAttr);

        public override Type GetInterface(string name, bool ignoreCase) => type.GetInterface(name, ignoreCase);

        public override Type[] GetInterfaces() => type.GetInterfaces();

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => type.GetMembers(bindingAttr);

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => type.GetMethods(bindingAttr);

        public override Type GetNestedType(string name, BindingFlags bindingAttr) => type.GetNestedType(name, bindingAttr);

        public override Type[] GetNestedTypes(BindingFlags bindingAttr) => type.GetNestedTypes(bindingAttr);

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => GetProperties(bindingAttr);

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => type.InvokeMember(name, invokeAttr, binder, target, args);

        public override bool IsDefined(Type attributeType, bool inherit) => type.IsDefined(attributeType, inherit);

        protected override TypeAttributes GetAttributeFlagsImpl() => (TypeAttributes)MethodGetAttributeFlagsImpl.Invoke(type, null);

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => (ConstructorInfo)MethodGetConstructorImpl.Invoke(type, new object[] { bindingAttr, binder, callConvention, type, modifiers });

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => (MethodInfo)MethodGetMethodImpl.Invoke(type, new object[] { name, bindingAttr, binder, callConvention, type, modifiers });

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => (PropertyInfo)MethodGetPropertyImpl.Invoke(type, new object[] { name, bindingAttr, binder, returnType, type, modifiers });

        protected override bool HasElementTypeImpl() => (bool)MethodHasElementTypeImpl.Invoke(type, null);

        protected override bool IsArrayImpl() => (bool)MethodIsArrayImpl.Invoke(type, null);

        protected override bool IsByRefImpl() => (bool)MethodIsByRefImpl.Invoke(type, null);

        protected override bool IsCOMObjectImpl() => (bool)MethodIsCOMObjectImpl.Invoke(type, null);

        protected override bool IsPointerImpl() => (bool)MethodIsPointerImpl.Invoke(type, null);

        protected override bool IsPrimitiveImpl() => (bool)MethodIsPrimitiveImpl.Invoke(type, null);

        public IEnumerable<Type> GenericTypeArguments => type.GetGenericArguments();

        public override bool IsGenericType => type.IsGenericType;

        public override bool ContainsGenericParameters => type.ContainsGenericParameters;

        public override MethodBase DeclaringMethod => type.DeclaringMethod;

        public override Type DeclaringType => type.DeclaringType;

        public override bool Equals(object o) => type.Equals(o);

        public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria) => type.FindInterfaces(filter, filterCriteria);

        public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria) => type.FindMembers(memberType, bindingAttr, filter, filterCriteria);

        public override GenericParameterAttributes GenericParameterAttributes => type.GenericParameterAttributes;

        public override int GenericParameterPosition => type.GenericParameterPosition;

        public override int GetArrayRank() => type.GetArrayRank();

        public override MemberInfo[] GetDefaultMembers() => type.GetDefaultMembers();

        public override EventInfo[] GetEvents() => type.GetEvents();

        public override Type[] GetGenericArguments() => type.GetGenericArguments();

        public override Type[] GetGenericParameterConstraints() => type.GetGenericParameterConstraints();

        public override Type GetGenericTypeDefinition() => type.GetGenericTypeDefinition();

        public override int GetHashCode() => type.GetHashCode();

        public override InterfaceMapping GetInterfaceMap(Type interfaceType) => type.GetInterfaceMap(interfaceType);

        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr) => type.GetMember(name, bindingAttr);

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) => (MemberInfo[])MethodGetMember.Invoke(type, new object[] { name, type, bindingAttr });

        public override bool IsAssignableFrom(Type c) => type.IsAssignableFrom(c);

        protected override bool IsContextfulImpl() => (bool)MethodIsContextfulImpl.Invoke(type, null);

        public override bool IsGenericParameter => type.IsGenericParameter;

        public override bool IsGenericTypeDefinition => type.IsGenericTypeDefinition;

        public override bool IsInstanceOfType(object o) => type.IsInstanceOfType(o);

        protected override bool IsMarshalByRefImpl() => (bool)MethodIsMarshalByRefImpl.Invoke(type, null);

        public override bool IsSubclassOf(Type c) => type.IsSubclassOf(c);

        protected override bool IsValueTypeImpl() => (bool)MethodIsValueTypeImpl.Invoke(type, null);

        public override Type MakeArrayType() => type.MakeArrayType();

        public override Type MakeArrayType(int rank) => type.MakeArrayType(rank);

        public override Type MakeByRefType() => type.MakeByRefType();

        public override Type MakeGenericType(params Type[] typeArguments) => type.MakeGenericType(typeArguments);

        public override Type MakePointerType() => type.MakePointerType();

        public override MemberTypes MemberType => type.MemberType;

        public override int MetadataToken => type.MetadataToken;

        public override Type ReflectedType => type.ReflectedType;

        public override StructLayoutAttribute StructLayoutAttribute => type.StructLayoutAttribute;

        public override string ToString() => type.ToString();

        public TypeInfo GetTypeInfo() => this;

        public override RuntimeTypeHandle TypeHandle => type.TypeHandle;

        public new ConstructorInfo TypeInitializer => type.TypeInitializer;

        public virtual IEnumerable<ConstructorInfo> DeclaredConstructors { get; }
        public virtual IEnumerable<EventInfo> DeclaredEvents { get; }
        public virtual IEnumerable<FieldInfo> DeclaredFields { get; }
        public virtual IEnumerable<MemberInfo> DeclaredMembers { get; }
        public virtual IEnumerable<MethodInfo> DeclaredMethods { get; }
        public virtual IEnumerable<TypeInfo> DeclaredNestedTypes { get; }
        public virtual IEnumerable<PropertyInfo> DeclaredProperties { get; }
        public virtual Type[] GenericTypeParameters { get; }
        public virtual IEnumerable<Type> ImplementedInterfaces { get; }

        public virtual Type AsType() 
            => type;
        public virtual EventInfo? GetDeclaredEvent(string name) =>
            DeclaredEvents.FirstOrDefault(e=>e.Name == name);
        public virtual FieldInfo? GetDeclaredField(string name) =>
            DeclaredFields.FirstOrDefault(f => f.Name == name);
        public virtual MethodInfo? GetDeclaredMethod(string name) =>
            DeclaredMethods.FirstOrDefault(m => m.Name == name);
        public virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name) =>
            DeclaredMethods.Where(m => m.Name == name);
        public virtual TypeInfo? GetDeclaredNestedType(string name) =>
            type.GetNestedTypes()
                .Where(nt => nt.Name == name)
                .Select(nt => new TypeInfo(nt))
                .FirstOrDefault();
        public virtual PropertyInfo? GetDeclaredProperty(string name) =>
            DeclaredProperties.FirstOrDefault(p => p.Name == name);
        public virtual bool IsAssignableFrom(TypeInfo? typeInfo) =>
           typeInfo is null
            ? false
            : type.IsAssignableFrom(typeInfo.type);
    }
}


#endif