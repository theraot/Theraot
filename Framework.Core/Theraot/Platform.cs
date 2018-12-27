namespace Theraot.Core
{
    public static class Platform
    {
        public const string Version =
#if NET20
                "net20"
#elif NET30
                "net30"
#elif NET35
                "net35"
#elif NET40
                "net40"
#elif NET45
                "net45"
#elif NET46
                "net46"
#elif NET47
                "net47"
#elif NETCOREAPP1_0
                "netcoreapp1.0"
#elif NETCOREAPP1_1
                "netcoreapp1.1"
#elif NETCOREAPP2_0
                "netcoreapp2.0"
#elif NETCOREAPP2_1
                "netcoreapp2.1"
#elif NETSTANDARD1_0
                "netstandard1.0"
#elif NETSTANDARD1_1
                "netstandard1.1"
#elif NETSTANDARD1_2
                "netstandard1.2"
#elif NETSTANDARD1_3
                "netstandard1.3"
#elif NETSTANDARD1_4
                "netstandard1.4"
#elif NETSTANDARD1_5
                "netstandard1.5"
#elif NETSTANDARD1_6
                "netstandard1.6"
#else
                "unknown"
#endif
            ;
    }
}