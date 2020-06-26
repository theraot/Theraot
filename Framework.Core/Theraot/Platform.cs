using System;

namespace Theraot
{
    public static partial class Platform
    {
        public static string Moniker { get; } =
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
#elif NET47 || NET471 || NET472

            "net47"
#elif NET48

            "net48"
#elif NETCOREAPP1_0

            "netcoreapp1.0"
#elif NETCOREAPP1_1

            "netcoreapp1.1"
#elif NETCOREAPP2_0

            "netcoreapp2.0"
#elif NETCOREAPP2_1

            "netcoreapp2.1"
#elif NETCOREAPP2_2

            "netcoreapp2.2"
#elif NETCOREAPP3_0

            "netcoreapp3.0"
#elif NETCOREAPP3_1

            "netcoreapp3.1"
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
#elif NETSTANDARD2_0

            "netstandard2.0"
#elif NETSTANDARD2_1

            "netstandard2.1"
#else

            "unknown"
#endif
            ;
    }

    public static partial class Platform
    {
        public static bool TargetsDotNetCore { get; } =
#if TARGETS_NETCORE
            true
#else
            false
#endif
            ;

        public static bool TargetsDotNetFramework { get; } =
#if TARGETS_NET
            true
#else
            false
#endif
            ;

        public static bool TargetsDotNetStandard { get; } =
#if TARGETS_NETSTANDARD
            true
#else
            false
#endif
            ;
    }

    public static partial class Platform
    {
        public static int MajorVersion { get; } =
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP1_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
            1
#elif NET20 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETSTANDARD2_0 || NETSTANDARD2_1
            2
#elif NET30 || NET35 || NETCOREAPP3_0
            3
#elif NET40 || NET45 || NET46 || NET47 || NET471 || NET472 || NET48
            4
#else
            -1
#endif
            ;

        public static int MinorVersion { get; } =
#if NET20 || NET30 || NET40 || NETCOREAPP1_0 || NETCOREAPP2_0 || NETCOREAPP3_0 || NETSTANDARD1_0 || NETSTANDARD2_0
            0
#elif NETCOREAPP1_1 || NETCOREAPP2_1 || NETSTANDARD1_1 || NETSTANDARD2_1
            1
#elif NETCOREAPP1_2 || NETCOREAPP2_2 || NETSTANDARD1_2
            2
#elif NETSTANDARD1_3
            3
#elif NETSTANDARD1_4
            4
#elif NET35 || NET45 || NETSTANDARD1_5
            5
#elif NET46 || NETSTANDARD1_6
            6
#elif NET47 || NET471 || NET472
            7
#elif NET48
            8
#else
            -1
#endif
            ;

        public static Version Version { get; } = new Version(MajorVersion, MinorVersion);
    }
}