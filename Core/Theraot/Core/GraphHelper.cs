using System;
using System.Collections.Generic;
using Theraot.Collections;

namespace Theraot.Core
{
    public static partial class GraphHelper
    {
        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreBreadthFirstGraphExtracted(new ExtendedQueue<TInput>() { initial }, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreBreadthFirstGraphExtracted(new ExtendedQueue<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreBreadthFirstGraphExtracted(new ExtendedQueue<T>() { initial }, next);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreBreadthFirstGraphExtracted(new ExtendedQueue<T>(initial), next);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreBreadthFirstTreeExtracted(new ExtendedQueue<TInput>() { initial }, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreBreadthFirstTreeExtracted(new ExtendedQueue<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreBreadthFirstTreeExtracted(new ExtendedQueue<T>() { initial }, next);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreBreadthFirstTreeExtracted(new ExtendedQueue<T>(initial), next);
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstGraphExtracted<TInput, TOutput>(ExtendedQueue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    TInput found;
                    if (queue.TryTake(out found))
                    {
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        if (known.Add(found))
                        {
                            yield return resultSelector.Invoke(found);
                            queue.Add(found);
                        }
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreBreadthFirstGraphExtracted<T>(ExtendedQueue<T> queue, Func<T, IEnumerable<T>> next)
        {
            var known = new HashSet<T>();
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    T found;
                    if (queue.TryTake(out found))
                    {
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        if (known.Add(found))
                        {
                            yield return found;
                            queue.Add(found);
                        }
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstTreeExtracted<TInput, TOutput>(ExtendedQueue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    TInput found;
                    if (queue.TryTake(out found))
                    {
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        yield return resultSelector.Invoke(found);
                        queue.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreBreadthFirstTreeExtracted<T>(ExtendedQueue<T> queue, Func<T, IEnumerable<T>> next)
        {
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    T found;
                    if (queue.TryTake(out found))
                    {
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        yield return found;
                        queue.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }
    }

    public static partial class GraphHelper
    {
        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreDepthFirstGraphExtracted(new ExtendedStack<TInput>() { initial }, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreDepthFirstGraphExtracted(new ExtendedStack<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreDepthFirstGraphExtracted(new ExtendedStack<T>() { initial }, next);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreDepthFirstGraphExtracted(new ExtendedStack<T>(initial), next);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreDepthFirstTreeExtracted(new ExtendedStack<TInput>() { initial }, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            Check.NotNullArgument(next, "next");
            Check.NotNullArgument(resultSelector, "next");
            return ExploreDepthFirstTreeExtracted(new ExtendedStack<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreDepthFirstTreeExtracted(new ExtendedStack<T>() { initial }, next);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            Check.NotNullArgument(next, "next");
            return ExploreDepthFirstTreeExtracted(new ExtendedStack<T>(initial), next);
        }

        private static IEnumerable<TOutput> ExploreDepthFirstGraphExtracted<TInput, TOutput>(ExtendedStack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    TInput found;
                    if (stack.TryTake(out found))
                    {
                        if (known.Add(found))
                        {
                            yield return resultSelector.Invoke(found);
                            branches = next.Invoke(found).GetEnumerator();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreDepthFirstGraphExtracted<T>(ExtendedStack<T> stack, Func<T, IEnumerable<T>> next)
        {
            var known = new HashSet<T>();
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    T found;
                    if (stack.TryTake(out found))
                    {
                        if (known.Add(found))
                        {
                            yield return found;
                            branches = next.Invoke(found).GetEnumerator();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<TOutput> ExploreDepthFirstTreeExtracted<TInput, TOutput>(ExtendedStack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    TInput found;
                    if (stack.TryTake(out found))
                    {
                        yield return resultSelector.Invoke(found);
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreDepthFirstTreeExtracted<T>(ExtendedStack<T> stack, Func<T, IEnumerable<T>> next)
        {
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    T found;
                    if (stack.TryTake(out found))
                    {
                        yield return found;
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced = false;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Add(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }
    }
}