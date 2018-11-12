// Needed for Workaround

using System;
using System.IO;
using Theraot.Collections;

#if NET20 || NET30 || NET35 || NET40

using System.Collections.Generic;

#endif

namespace Theraot.Core
{
    public static class PathHelper
    {
        private static readonly string _directorySeparatorString = Path.DirectorySeparatorChar.ToString();
        private static readonly string _altDirectorySeparatorString = Path.AltDirectorySeparatorChar.ToString();
        private static readonly string _volumeSeparatorString = Path.VolumeSeparatorChar.ToString();

        public static string DirectorySeparatorString
        {
            get { return _directorySeparatorString; }
        }

        public static string AltDirectorySeparatorString
        {
            get { return _altDirectorySeparatorString; }
        }

        public static string VolumeSeparatorString
        {
            get { return _volumeSeparatorString; }
        }

        public static string Combine(params string[] paths)
        {
#if NET20 || NET30 || NET35
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }
            var combine = new List<string>();
            for (var index = paths.Length - 1; index >= 0; index--)
            {
                if (HasInvalidPathChars(paths[index]))
                {
                    throw new ArgumentException("invalid characters in path");
                }
                if (string.Empty != paths[index])
                {
                    if (combine.Count > 0)
                    {
                        if (!paths[index].EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                            && !paths[index].EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                            && !paths[index].EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                        {
                            paths[index] += _directorySeparatorString;
                        }
                    }
                    combine.Insert(0, paths[index]);
                    if (Path.IsPathRooted(paths[index]))
                    {
                        break;
                    }
                }
            }
            return StringHelper.Concat(combine);
#else
            return Path.Combine(paths);
#endif
        }

        public static string Combine(string path1, string path2)
        {
#if NET20 || NET30 || NET35

            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (string.Empty != path2)
            {
                if (Path.IsPathRooted(path2))
                {
                    return path2;
                }
            }
            if (string.Empty != path1)
            {
                if (!path1.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += _directorySeparatorString;
                }
            }
            return string.Concat(path1, path2);
#else
            return Path.Combine(path1, path2);
#endif
        }

        public static string Combine(string path1, string path2, string path3)
        {
#if NET20 || NET30 || NET35
            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2) || HasInvalidPathChars(path3))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (string.Empty != path3)
            {
                if (Path.IsPathRooted(path3))
                {
                    return path3;
                }
            }
            if (string.Empty != path2)
            {
                if (!path2.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path2 += _directorySeparatorString;
                }
                if (Path.IsPathRooted(path2))
                {
                    return string.Concat(path2, path3);
                }
            }
            if (string.Empty != path1)
            {
                if (!path1.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += _directorySeparatorString;
                }
            }
            return string.Concat(path1, path2, path3);
#else
            return Path.Combine(path1, path2, path3);
#endif
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
#if NET20 || NET30 || NET35
            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2) || HasInvalidPathChars(path3) || HasInvalidPathChars(path4))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (string.Empty != path4)
            {
                if (Path.IsPathRooted(path4))
                {
                    return path4;
                }
            }
            if (string.Empty != path3)
            {
                if (!path3.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path3.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path3.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path3 += _directorySeparatorString;
                }
                if (Path.IsPathRooted(path3))
                {
                    return string.Concat(path3, path4);
                }
            }
            if (string.Empty != path2)
            {
                if (!path2.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path2 += _directorySeparatorString;
                }
                if (Path.IsPathRooted(path2))
                {
                    return string.Concat(path2, path3, path4);
                }
            }
            if (string.Empty != path1)
            {
                if (!path1.EndsWith(_directorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_altDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(_volumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += _directorySeparatorString;
                }
            }
            return string.Concat(path1, path2, path3, path4);
#else
            return Path.Combine(path1, path2, path3, path4);
#endif
        }

        public static bool HasInvalidPathChars(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (path.Length == 0)
            {
                return false;
            }
            return path.ContainsAny(Path.GetInvalidPathChars());
        }

        public static bool HasInvalidFileNameChars(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return fileName.ContainsAny(Path.GetInvalidFileNameChars());
        }
    }
}