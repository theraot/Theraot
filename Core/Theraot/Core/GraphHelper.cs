// Needed for Workaround

using System;
using System.Collections.Generic;

namespace Theraot.Core
{
    public static partial class GraphHelper
    {
        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            var queue = new Queue<TInput>();
            queue.Enqueue(initial);
            return ExploreBreadthFirstGraphExtracted(queue, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            return ExploreBreadthFirstGraphExtracted(new Queue<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            var queue = new Queue<T>();
            queue.Enqueue(initial);
            return ExploreBreadthFirstGraphExtracted(queue, next);
        }

        public static IEnumerable<T> ExploreBreadthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            return ExploreBreadthFirstGraphExtracted(new Queue<T>(initial), next);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            var queue = new Queue<TInput>();
            queue.Enqueue(initial);
            return ExploreBreadthFirstTreeExtracted(queue, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreBreadthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            return ExploreBreadthFirstTreeExtracted(new Queue<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            var queue = new Queue<T>();
            queue.Enqueue(initial);
            return ExploreBreadthFirstTreeExtracted(queue, next);
        }

        public static IEnumerable<T> ExploreBreadthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            return ExploreBreadthFirstTreeExtracted(new Queue<T>(initial), next);
        }

        private static IEnumerable<TOutput> ExploreBreadthFirstGraphExtracted<TInput, TOutput>(Queue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        if (known.Add(found))
                        {
                            yield return resultSelector.Invoke(found);
                            queue.Enqueue(found);
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

        private static IEnumerable<T> ExploreBreadthFirstGraphExtracted<T>(Queue<T> queue, Func<T, IEnumerable<T>> next)
        {
            var known = new HashSet<T>();
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        if (known.Add(found))
                        {
                            yield return found;
                            queue.Enqueue(found);
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

        private static IEnumerable<TOutput> ExploreBreadthFirstTreeExtracted<TInput, TOutput>(Queue<TInput> queue, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        yield return resultSelector.Invoke(found);
                        queue.Enqueue(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreBreadthFirstTreeExtracted<T>(Queue<T> queue, Func<T, IEnumerable<T>> next)
        {
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (queue.Count > 0)
                    {
                        var found = queue.Dequeue();
                        branches = next.Invoke(found).GetEnumerator();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        yield return found;
                        queue.Enqueue(found);
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
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            var stack = new Stack<TInput>();
            stack.Push(initial);
            return ExploreDepthFirstGraphExtracted(stack, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstGraph<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            return ExploreDepthFirstGraphExtracted(new Stack<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            var stack = new Stack<T>();
            stack.Push(initial);
            return ExploreDepthFirstGraphExtracted(stack, next);
        }

        public static IEnumerable<T> ExploreDepthFirstGraph<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            return ExploreDepthFirstGraphExtracted(new Stack<T>(initial), next);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(TInput initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            var stack = new Stack<TInput>();
            stack.Push(initial);
            return ExploreDepthFirstTreeExtracted(stack, next, resultSelector);
        }

        public static IEnumerable<TOutput> ExploreDepthFirstTree<TInput, TOutput>(IEnumerable<TInput> initial, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            if (resultSelector == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp1 = resultSelector;
            return ExploreDepthFirstTreeExtracted(new Stack<TInput>(initial), next, resultSelector);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(T initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            var stack = new Stack<T>();
            stack.Push(initial);
            return ExploreDepthFirstTreeExtracted(stack, next);
        }

        public static IEnumerable<T> ExploreDepthFirstTree<T>(IEnumerable<T> initial, Func<T, IEnumerable<T>> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            var temp = next;
            return ExploreDepthFirstTreeExtracted(new Stack<T>(initial), next);
        }

        private static IEnumerable<TOutput> ExploreDepthFirstGraphExtracted<TInput, TOutput>(Stack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            var known = new HashSet<TInput>();
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
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
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Push(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreDepthFirstGraphExtracted<T>(Stack<T> stack, Func<T, IEnumerable<T>> next)
        {
            var known = new HashSet<T>();
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
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
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Push(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<TOutput> ExploreDepthFirstTreeExtracted<TInput, TOutput>(Stack<TInput> stack, Func<TInput, IEnumerable<TInput>> next, Func<TInput, TOutput> resultSelector)
        {
            IEnumerator<TInput> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
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
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Push(found);
                    }
                    else
                    {
                        branches.Dispose();
                        branches = null;
                    }
                }
            }
        }

        private static IEnumerable<T> ExploreDepthFirstTreeExtracted<T>(Stack<T> stack, Func<T, IEnumerable<T>> next)
        {
            IEnumerator<T> branches = null;
            while (true)
            {
                if (branches == null)
                {
                    if (stack.Count > 0)
                    {
                        var found = stack.Pop();
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
                    bool advanced;
                    try
                    {
                        advanced = branches.MoveNext();
                    }
                    catch
                    {
                        // Regardless of what exception it is
                        branches.Dispose();
                        throw;
                    }
                    if (advanced)
                    {
                        var found = branches.Current;
                        stack.Push(found);
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