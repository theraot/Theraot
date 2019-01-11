#if NET20 || NET30 || NET35 || NET40 || NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6

// Needed for Workaround

using System;
using System.IO;
using Theraot.Collections;
using System.Runtime.CompilerServices;

#if NET20 || NET30 || NET35

using System.Collections.Generic;

#endif

namespace Theraot.Core
{
    public static class PathHelper
    {
        public static string AltDirectorySeparatorString { get; } = Path.AltDirectorySeparatorChar.ToString();
        public static string DirectorySeparatorString { get; } = Path.DirectorySeparatorChar.ToString();
        public static string VolumeSeparatorString { get; } = Path.VolumeSeparatorChar.ToString();

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
                ref var current = ref paths[index];
                if (HasInvalidPathChars(current))
                {
                    throw new ArgumentException("invalid characters in path");
                }
                if (current.Length != 0)
                {
                    if (combine.Count > 0)
                    {
                        if (!current.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                            && !current.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                            && !current.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                        {
                            current += DirectorySeparatorString;
                        }
                    }
                    combine.Insert(0, current);
                    if (Path.IsPathRooted(current))
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

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Combine(string path1, string path2)
        {
#if NET20 || NET30 || NET35

            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (path2.Length != 0)
            {
                if (Path.IsPathRooted(path2))
                {
                    return path2;
                }
            }
            if (path1.Length != 0)
            {
                if (!path1.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += DirectorySeparatorString;
                }
            }
            return string.Concat(path1, path2);
#else
            return Path.Combine(path1, path2);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Combine(string path1, string path2, string path3)
        {
#if NET20 || NET30 || NET35
            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2) || HasInvalidPathChars(path3))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (path3.Length != 0)
            {
                if (Path.IsPathRooted(path3))
                {
                    return path3;
                }
            }
            if (path2.Length != 0)
            {
                if (!path2.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path2 += DirectorySeparatorString;
                }
                if (Path.IsPathRooted(path2))
                {
                    return string.Concat(path2, path3);
                }
            }
            if (path1.Length != 0)
            {
                if (!path1.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += DirectorySeparatorString;
                }
            }
            return string.Concat(path1, path2, path3);
#else
            return Path.Combine(path1, path2, path3);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Combine(string path1, string path2, string path3, string path4)
        {
#if NET20 || NET30 || NET35
            if (HasInvalidPathChars(path1) || HasInvalidPathChars(path2) || HasInvalidPathChars(path3) || HasInvalidPathChars(path4))
            {
                throw new ArgumentException("invalid characters in path");
            }
            if (path4.Length != 0)
            {
                if (Path.IsPathRooted(path4))
                {
                    return path4;
                }
            }
            if (path3.Length != 0)
            {
                if (!path3.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path3.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path3.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path3 += DirectorySeparatorString;
                }
                if (Path.IsPathRooted(path3))
                {
                    return string.Concat(path3, path4);
                }
            }
            if (path2.Length != 0)
            {
                if (!path2.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path2.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path2 += DirectorySeparatorString;
                }
                if (Path.IsPathRooted(path2))
                {
                    return string.Concat(path2, path3, path4);
                }
            }
            if (path1.Length != 0)
            {
                if (!path1.EndsWith(DirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(AltDirectorySeparatorString, StringComparison.Ordinal)
                    && !path1.EndsWith(VolumeSeparatorString, StringComparison.Ordinal))
                {
                    path1 += DirectorySeparatorString;
                }
            }
            return string.Concat(path1, path2, path3, path4);
#else
            return Path.Combine(path1, path2, path3, path4);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool HasInvalidFileNameChars(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return fileName.ContainsAny(Path.GetInvalidFileNameChars());
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
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
    }
}

#endif