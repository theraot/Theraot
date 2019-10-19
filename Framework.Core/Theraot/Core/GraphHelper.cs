// Needed for Workaround

#pragma warning disable CC0031 // Check for null before calling a delegate

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class GraphHelper
    {
        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next, IEqualityComparer<T> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = initial;
            return ExploreBreadthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>(), comparer ?? EqualityComparer<T>.Default);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = initial;
            return ExploreBreadthFirstGraphExtracted(branches, next, resultSelector, comparer ?? EqualityComparer<TInput>.Default);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next, IEqualityComparer<T> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = new[] { initial };
            return ExploreBreadthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>(), comparer ?? EqualityComparer<T>.Default);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            var branches = new[] { initial };
            return ExploreBreadthFirstGraphExtracted(branches, next, resultSelector, comparer ?? EqualityComparer<TInput>.Default);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = initial;
            return ExploreBreadthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = initial;
            return ExploreBreadthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = new[] { initial };
            return ExploreBreadthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = new[] { initial };
            return ExploreBreadthFirstTreeExtracted(branches, next, resultSelector);
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstGraphExtracted<TInput, TOutput>(IEnumerable<TInput>? branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            // NOTICE this method has no null check
            var known = new HashSet<TInput>(comparer);
            var queue = new Queue<TInput>();
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next(found);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    foreach (var found in branches)
                    {
                        if (known.Contains(found))
                        {
                            continue;
                        }

                        known.Add(found);
                        yield return resultSelector(found);
                        queue.Enqueue(found);
                    }

                    branches = null;
                }
            }
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstTreeExtracted<TInput, TOutput>(IEnumerable<TInput>? branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            // NOTICE this method has no null check
            var queue = new Queue<TInput>();
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next(found);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    foreach (var found in branches)
                    {
                        yield return resultSelector(found);
                        queue.Enqueue(found);
                    }

                    branches = null;
                }
            }
        }
    }

    public static partial class GraphHelper
    {
        public static IEnumerable<T> ExploreDepthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next, IEqualityComparer<T> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = initial;
            return ExploreDepthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>(), comparer ?? EqualityComparer<T>.Default);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = initial;
            return ExploreDepthFirstGraphExtracted(branches, next, resultSelector, comparer ?? EqualityComparer<TInput>.Default);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next, IEqualityComparer<T> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = new[] { initial };
            return ExploreDepthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>(), comparer ?? EqualityComparer<T>.Default);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = new[] { initial };
            return ExploreDepthFirstGraphExtracted(branches, next, resultSelector, comparer ?? EqualityComparer<TInput>.Default);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = initial;
            return ExploreDepthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = initial;
            return ExploreDepthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var branches = new[] { initial };
            return ExploreDepthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }

            var branches = new[] { initial };
            return ExploreDepthFirstTreeExtracted(branches, next, resultSelector);
        }

        private static IEnumerable<TOutput> ExploreDepthFirstGraphExtracted<TInput, TOutput>(IEnumerable<TInput>? branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector, IEqualityComparer<TInput> comparer)
        {
            // NOTICE this method has no null check
            var known = new HashSet<TInput>(comparer);
            var stack = new Stack<TInput>();
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
                        branches = next(found);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    foreach (var found in branches)
                    {
                        if (known.Contains(found))
                        {
                            continue;
                        }

                        known.Add(found);
                        yield return resultSelector(found);
                        stack.Push(found);
                    }

                    branches = null;
                }
            }
        }

        private static IEnumerable<TOutput> ExploreDepthFirstTreeExtracted<TInput, TOutput>(IEnumerable<TInput>? branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            // NOTICE this method has no null check
            var stack = new Stack<TInput>();
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
                        branches = next(found);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    foreach (var found in branches)
                    {
                        yield return resultSelector(found);
                        stack.Push(found);
                    }

                    branches = null;
                }
            }
        }
    }
}