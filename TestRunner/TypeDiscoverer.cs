using System;
using System.Collections.Generic;

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
#if NET20 || NET30 || NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_5 || NETSTANDARD1_6
            types = typeof(TypeDiscoverer).GetTypeInfo().Assembly.GetTypes();
#else
            types =  new Type[]
			{
				typeof(TestRunner.System.Threading.ThreadTest),
			};
#endif
		}
    }
}
