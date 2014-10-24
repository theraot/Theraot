using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Theraot.Collections;

namespace Theraot.Core
{
    [global::System.Diagnostics.DebuggerNonUserCode]
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
            if (fileEntries != null)
            {
                return fileEntries;
            }
            else
            {
                return EmptySet<string>.Instance;
            }
        }

        public static IEnumerable<string> GetFilesAndFoldersRecursive(string folder, string pattern)
        {
            return GetFiles(folder, pattern).Concat
                (
                    GraphHelper.ExploreBreadthFirstTree
                        (
                            folder,
                            GetFolders,
                            current => GetFiles(current, pattern).Prepend(current)
                        ).SelectMany(input => input)
                );
        }

        public static IEnumerable<string> GetFilesRecursive(string folder, string pattern)
        {
            return
                GetFiles(folder, pattern).Concat
                    (
                        GraphHelper.ExploreBreadthFirstTree
                            (
                                folder,
                                GetFolders,
                                current => GetFiles(current, pattern)
                            ).SelectMany(input => input)
                    );
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
                            ((File.GetAttributes(subFolder) & FileAttributes.ReparsePoint) !=
                             FileAttributes.ReparsePoint));
            }
            catch
            {
                // Pokemon
                return EmptySet<string>.Instance;
            }
        }

        public static IEnumerable<string> GetFoldersRecursive(string folder)
        {
            return GraphHelper.ExploreBreadthFirstTree(folder, GetFolders);
        }
    }
}