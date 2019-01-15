using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestRunner
{
    public static class TypeDiscoverer
    {
        private static Type[] _types;

        public static IEnumerable<Type> GetAllTypes()
        {
            GetAllTypesPrivate(ref _types);
            return _types;
        }

        private static void GetAllTypesPrivate(ref Type[] types)
        {
            if (_types != null)
            {
                return;
            }
            var assembly = typeof(TypeDiscoverer).GetTypeInfo().Assembly;
            types = assembly.GetTypes();
        }
    }
}
