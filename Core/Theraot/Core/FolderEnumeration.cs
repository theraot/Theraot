// Needed for Workadound

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class FolderEnumeration
    {
        public static IEnumerable<string> GetFiles(string folder, string pattern)
        {
            // TODO handle exceptions
            IEnumerable<string> fileEntries = null;
            try
            {
#if NET20 || NET30 || NET35
                fileEntries = Directory.GetFiles(folder, pattern, SearchOption.TopDirectoryOnly);
#else
                fileEntries = Directory.EnumerateFiles(folder, pattern, SearchOption.TopDirectoryOnly);
#endif
            }
            catch (DirectoryNotFoundException)
            {
                // Empty
            }
            catch (UnauthorizedAccessException)
            {
                // Empty
            }
            return fileEntries ?? (IEnumerable<string>)ArrayReservoir<string>.EmptyArray;
        }

        public static IEnumerable<string> GetFilesAndFoldersRecursive(string folder, string pattern)
        {
            var enumerable = GraphHelper.ExploreBreadthFirstTree
                (
                    folder,
                    GetFolders,
                    current => current.AsUnaryEnumerable().Concat(GetFiles(current, pattern))
                );
            return GetFiles(folder, pattern).Concat(enumerable.Flatten());
        }

        public static IEnumerable<string> GetFilesRecursive(string folder, string pattern)
        {
            var enumerable = GraphHelper.ExploreBreadthFirstTree
                (
                    folder,
                    GetFolders,
                    current => GetFiles(current, pattern)
                );
            return GetFiles(folder, pattern).Concat(enumerable.Flatten());
        }

        public static IEnumerable<string> GetFolders(string folder)
        {
            // TODO handle exceptions
            try
            {
#if NET20 || NET30 || NET35
                var directories = Directory.GetDirectories(folder);
#else
            var directories = Directory.EnumerateDirectories(folder);
#endif
                return
                    directories.Where(
                        subFolder =>
                            ((File.GetAttributes(subFolder) & FileAttributes.ReparsePoint)
                             != FileAttributes.ReparsePoint));
            }
            catch
            {
                // Pokemon
                return ArrayReservoir<string>.EmptyArray;
            }
        }

        public static IEnumerable<string> GetFoldersRecursive(string folder)
        {
            return GraphHelper.ExploreBreadthFirstTree(folder, GetFolders);
        }
    }
}