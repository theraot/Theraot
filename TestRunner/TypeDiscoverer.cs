namespace TestRunner
{
    public static class TypeDiscoverer
    {
        private static global::System.Type[] _types;

        public static global::System.Collections.Generic.IEnumerable<global::System.Type> GetAllTypes()
        {
            GetAllTypesPrivate(ref _types);
            return _types;
        }

        private static void GetAllTypesPrivate(ref global::System.Type[] types)
        {
            if (_types != null)
            {
                return;
            }
#if NET20 || NET30 || NET35 || NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_5 || NETSTANDARD1_6
            types = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(TypeDiscoverer)).Assembly.GetTypes();
#else
            types = new []
			{
				typeof(Console),
				typeof(DataGenerator),
				typeof(NumericGenerator),
				typeof(SmallPositiveNumericGenerator),
				typeof(StringGenerator),
				typeof(DataGeneratorAttribute),
				typeof(UseGeneratorAttribute),
				typeof(Assert),
				typeof(Program),
				typeof(AssertionFailedException),
				typeof(CategoryAttribute),
				typeof(TestAttribute),
				typeof(TestFixtureAttribute),
				typeof(TypeDiscoverer),
				typeof(System.Reflection.IntrospectionExtensionsTest),
				typeof(System.Threading.ThreadTest),
				typeof(System.Threading.TimerTest),
			};
#endif
		}
    }
}
