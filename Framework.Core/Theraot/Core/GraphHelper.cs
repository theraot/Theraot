// Needed for Workaround

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class GraphHelper
    {
        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var branches = new[] { initial };
            return ExploreBreadthFirstGraphExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = initial;
            return ExploreBreadthFirstGraphExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = new[] { initial };
            return ExploreBreadthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = initial;
            return ExploreBreadthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = new[] { initial };
            return ExploreBreadthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = initial;
            return ExploreBreadthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = new[] { initial };
            return ExploreBreadthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = initial;
            return ExploreBreadthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstGraphExtracted<TInput, TOutput>(IEnumerable<TInput> branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
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
                        if (!known.Contains(found))
                        {
                            known.Add(found);
                            yield return resultSelector(found);
                        }
                        queue.Enqueue(found);
                    }
                    branches = null;
                }
            }
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstTreeExtracted<TInput, TOutput>(IEnumerable<TInput> branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
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
        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = new[] { initial };
            return ExploreDepthFirstGraphExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = initial;
            return ExploreDepthFirstGraphExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = new[] { initial };
            return ExploreDepthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = initial;
            return ExploreDepthFirstGraphExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = new[] { initial };
            return ExploreDepthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException("resultSelector");
            }
            var branches = initial;
            return ExploreDepthFirstTreeExtracted(branches, next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = new[] { initial };
            return ExploreDepthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var branches = initial;
            return ExploreDepthFirstTreeExtracted(branches, next, FuncHelper.GetIdentityFunc<T>());
        }

        private static IEnumerable<TOutput> ExploreDepthFirstGraphExtracted<TInput, TOutput>(IEnumerable<TInput> branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
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
                        if (!known.Contains(found))
                        {
                            known.Add(found);
                            yield return resultSelector(found);
                        }
                        stack.Push(found);
                    }
                    branches = null;
                }
            }
        }

        private static IEnumerable<TOutput> ExploreDepthFirstTreeExtracted<TInput, TOutput>(IEnumerable<TInput> branches, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
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