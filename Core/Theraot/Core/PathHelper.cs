using System.IO;

namespace Theraot.Core
{
    public static class PathHelper
    {
        private static readonly string _separator = Path.DirectorySeparatorChar.ToString();

        public static string Combine(params string[] paths)
        {
#if NET20 || NET30 || NET35
            return string.Join(_separator, paths);
#else
            return Path.Combine (paths);
#endif
        }

        public static string Combine(string path1, string path2)
        {
#if NET20 || NET30 || NET35
            return StringHelper.Join(_separator, path1, path2);
#else
            return Path.Combine (path1, path2);
#endif
        }

        public static string Combine(string path1, string path2, string path3)
        {
#if NET20 || NET30 || NET35
            return StringHelper.Join(_separator, path1, path2, path3);
#else
            return Path.Combine (path1, path2, path3);
#endif
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
#if NET20 || NET30 || NET35
            return StringHelper.Join(_separator, path1, path2, path3, path4);
#else
            return Path.Combine (path1, path2, path3, path4);
#endif
        }

        public static string DirectorySeparatorString
        {
            get
            {
                return _separator;
            }
        }
    }
}
