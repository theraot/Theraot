#if LESSTHAN_NET35
extern alias nunitlinq;
#endif

#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using NUnit.Framework;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;

namespace APIList
{
    [TestFixture]
    public static class ApiListTest
    {
        [Test]
        [Category("Performance")]
        public static void Test()
        {
            var version = "2.1";
            var skippedNamespaces = new []
            {
                "System.Xml",
                "System.Data",
                "System.Net",
                "System.IO",
                "System.Drawing",
                "System.Runtime.Serialization",
                "System.Security.Cryptography"
            };
            // ---
            var url = $"https://raw.githubusercontent.com/dotnet/standard/master/docs/versions/netstandard{version}_ref.md";
            var text = new WebClient().DownloadString(url);
            var parser = new StringProcessor(text);
            var keywords = new []
            {
                "class",
                "static class",
                "abstract class",
                "sealed class",
                "struct",
                "enum",
                "interface"
            };
            var entryAssembly = typeof(ApiListTest).Assembly;
            var entryAssemblyName = entryAssembly.GetName();
            var assemblyNameComparer = new CustomEqualityComparer<AssemblyName>
            (
                (left, right) => left.FullName == right.FullName,
                name => name.FullName.GetHashCode()
            );
            var assemblyDictionary = new Dictionary<AssemblyName, Assembly>(assemblyNameComparer)
            {
                {entryAssemblyName, entryAssembly}
            };
            var loadedAssemblies = GraphHelper.ExploreBreadthFirstGraph
            (
                entryAssemblyName,
                assemblyName =>
                {
                    Assembly assembly = GetAssembly(assemblyName, assemblyDictionary);
                    return assembly.GetReferencedAssemblies();
                },
                assemblyNameComparer
            );
            var types = loadedAssemblies.SelectMany
            (
                assemblyName => GetAssembly(assemblyName, assemblyDictionary).GetTypes()
            );
            var lookup = new ProgressiveLookup<string, Type>(types.GroupProgressiveBy(type => type.FullName));
            parser.SkipUntilAfter("```C#");
            while (!parser.EndOfString)
            {
                parser.SkipWhile(CharHelper.IsClassicWhitespace);
                if (parser.Read("namespace"))
                {
                    parser.SkipWhile(CharHelper.IsClassicWhitespace);
                    var @namespace = parser.ReadUntil(CharHelper.IsClassicWhitespace);
                    var includeNamespace = true;
                    foreach (var skippedNamespace in skippedNamespaces)
                    {
                        if (@namespace.StartsWith(skippedNamespace))
                        {
                            includeNamespace = false;
                            break;
                        }
                    }
                    parser.SkipWhile(CharHelper.IsClassicWhitespace);
                    parser.Read("{");
                    parser.SkipWhile(CharHelper.IsClassicWhitespace);
                    while (true)
                    {
                        if (parser.Read("public"))
                        {
                            parser.SkipWhile(CharHelper.IsClassicWhitespace);
                            if (parser.Read("delegate"))
                            {
                                parser.SkipWhile(CharHelper.IsClassicWhitespace);
                                parser.SkipUntil(CharHelper.IsClassicWhitespace);
                                parser.SkipWhile(CharHelper.IsClassicWhitespace);
                                ReadType(parser, @namespace, lookup, includeNamespace);
                                parser.ReadUntil(new[] {'\n', '\r', '{'});
                                if (parser.Peek('\n') || parser.Peek('\r'))
                                {
                                    continue;
                                }
                            }
                            else if (parser.Read(keywords) != null)
                            {
                                parser.SkipWhile(CharHelper.IsClassicWhitespace);
                                ReadType(parser, @namespace, lookup, includeNamespace);
                            }
                            var count = 0;
                            do
                            {
                                if (parser.ReadUntil(new[] {'{', '}'}) == null)
                                {
                                    break;
                                }
                                if (parser.Read() == '{')
                                {
                                    count++;
                                }
                                else
                                {
                                    count--;
                                }
                            } while (count != 0);
                        }
                        parser.SkipWhile(CharHelper.IsClassicWhitespace);
                        if (parser.Read("}"))
                        {
                            break;
                        }
                    }
                }
                parser.Read("```");
            }
        }

        private static void ReadType(StringProcessor parser, string @namespace, ProgressiveLookup<string, Type> lookup, bool includeNamespace)
        {
            var typeName = string.Empty;
            while (true)
            {
                typeName += parser.ReadWhile(char.IsLetterOrDigit);
                if (parser.Read("_"))
                {
                    typeName += "_";
                }
                else
                {
                    break;
                }
            }
            var generic = parser.Read("<");
            if (generic)
            {
                var genericArgsCount = parser.ReadUntil(">").CountItemsWhere(chr => chr == ',') + 1;
                parser.Read(">");
                if (includeNamespace)
                {
                    ProcessType($"{@namespace}.{typeName}`{genericArgsCount}", lookup);
                }
            }
            else
            {
                if (includeNamespace)
                {
                    ProcessType($"{@namespace}.{typeName}", lookup);
                }
            }
        }

        private static Assembly GetAssembly(AssemblyName assemblyName, Dictionary<AssemblyName, Assembly> assemblyDictionary)
        {
            if (!assemblyDictionary.TryGetValue(assemblyName, out var assembly))
            {
                assembly = Assembly.Load(assemblyName);
                assemblyDictionary.Add(assemblyName, assembly);
            }
            return assembly;
        }

        private static void ProcessType(string typeName, ProgressiveLookup<string, Type> lookup)
        {
            var types = lookup[typeName];
            if (!types.Any())
            {
                Console.WriteLine(typeName);
            }
        }
    }
}

#endif