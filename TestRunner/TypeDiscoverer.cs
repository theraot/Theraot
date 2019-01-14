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
#if LESSTHAN_NETSTANDARD15
            types = new []
			{
				typeof(Assert),
				typeof(AvailabilityTests),
				typeof(Console),
				typeof(DataGenerator),
				typeof(NumericGenerator),
				typeof(SmallPositiveNumericGenerator),
				typeof(StringGenerator),
				typeof(DataGeneratorAttribute),
				typeof(UseGeneratorAttribute),
				typeof(InterfaceTests),
				typeof(Program),
				typeof(AssertionFailedException),
				typeof(CategoryAttribute),
				typeof(TestAttribute),
				typeof(TestFixtureAttribute),
				typeof(TypeDiscoverer),
				typeof(System.IO.StreamExtensionsTest),
				typeof(System.Linq.EnumerableTest),
				typeof(System.Reflection.IntrospectionExtensionsTest),
				typeof(System.Threading.SemaphoreSlimTestsEx),
				typeof(System.Threading.TaskExFromTest),
				typeof(System.Threading.ThreadPoolTest),
				typeof(System.Threading.ThreadTest),
				typeof(System.Threading.TimerTest),
				typeof(System.Collections.Concurrent.BlockingCollectionTest),
			};
#else
            types = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(TypeDiscoverer)).Assembly.GetTypes();
#endif
		}
    }
}
