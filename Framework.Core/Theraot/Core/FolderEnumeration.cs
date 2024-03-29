﻿#if TARGETS_NET || TARGETS_NETCORE || GREATERTHAN_NETSTANDARD12

// Needed for Workaround

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Theraot.Collections;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static class FolderEnumeration
    {
        public static IEnumerable<string> GetFiles(string folder, string pattern)
        {
            // TODO handle exceptions
            IEnumerable<string>? fileEntries = null;
            try
            {
#if LESSTHAN_NET40
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

            return fileEntries ?? ArrayEx.Empty<string>();
        }

        public static IEnumerable<string> GetFilesAndFoldersRecursive(string folder, string pattern)
        {
            var enumerable = GraphHelper.ExploreBreadthFirstTree
            (
                folder,
                GetFolders,
                current => current.AsUnaryIEnumerable().Concat(GetFiles(current, pattern))
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
#if LESSTHAN_NET40
            var directories = Directory.GetDirectories(folder);
#else
            var directories = Directory.EnumerateDirectories(folder);
#endif
            return
                directories.Where
                (
                    subFolder =>
                        (File.GetAttributes(subFolder) & FileAttributes.ReparsePoint)
                        != FileAttributes.ReparsePoint
                );
        }

        public static IEnumerable<string> GetFoldersRecursive(string folder)
        {
            return GraphHelper.ExploreBreadthFirstTree(folder, GetFolders);
        }
    }
}

#endif