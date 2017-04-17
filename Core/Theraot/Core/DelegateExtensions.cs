#if FAT

using System;
using System.Threading;

namespace Theraot.Core
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class DelegateExtensions
    {
        public static int SafeInvoke<T>(this Comparison<T> method, T x, T y, int def)
        {
            if (method != null)
            {
                return method.Invoke(x, y);
            }
            else
            {
                return def;
            }
        }

        public static int SafeInvoke<T>(this Comparison<T> method, T x, T y, Func<int> alternative, int def)
        {
            if (method != null)
            {
                return method.Invoke(x, y);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static bool SafeInvoke<T>(this Predicate<T> method, T obj, bool def)
        {
            if (method != null)
            {
                return method.Invoke(obj);
            }
            else
            {
                return def;
            }
        }

        public static bool SafeInvoke<T>(this Predicate<T> method, T obj, Func<bool> alternative, bool def)
        {
            if (method != null)
            {
                return method.Invoke(obj);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TOutput SafeInvoke<TInput, TOutput>(this Func<TInput, TOutput> method, TInput input, TOutput def)
        {
            if (method != null)
            {
                return method.Invoke(input);
            }
            else
            {
                return def;
            }
        }

        public static TOutput SafeInvoke<TInput, TOutput>(this Func<TInput, TOutput> method, TInput input, Func<TOutput> alternative, TOutput def)
        {
            if (method != null)
            {
                return method.Invoke(input);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static void SafeInvoke(this ThreadStart method)
        {
            if (method != null)
            {
                method.Invoke();
            }
        }

        public static void SafeInvoke(this ThreadStart method, Action alternative)
        {
            if (method != null)
            {
                method.Invoke();
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke(this ParameterizedThreadStart method, object obj)
        {
            if (method != null)
            {
                method.Invoke(obj);
            }
        }

        public static void SafeInvoke(this ParameterizedThreadStart method, object obj, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(obj);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke(this EventHandler method, object sender, EventArgs e)
        {
            if (method != null)
            {
                method.Invoke(sender, e);
            }
        }

        public static void SafeInvoke(this EventHandler method, object sender, EventArgs e, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(sender, e);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> method, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (method != null)
            {
                method.Invoke(sender, e);
            }
        }

        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> method, object sender, TEventArgs e, Action alternative)
            where TEventArgs : EventArgs
        {
            if (method != null)
            {
                method.Invoke(sender, e);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke(this Action method)
        {
            if (method != null)
            {
                method.Invoke();
            }
        }

        public static void SafeInvoke(this Action method, Action alternative)
        {
            if (method != null)
            {
                method.Invoke();
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T>(this Action<T> method, T obj)
        {
            if (method != null)
            {
                method.Invoke(obj);
            }
        }

        public static void SafeInvoke<T>(this Action<T> method, T obj, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(obj);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2>(this Action<T1, T2> method, T1 arg1, T2 arg2)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2);
            }
        }

        public static void SafeInvoke<T1, T2>(this Action<T1, T2> method, T1 arg1, T2 arg2, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3);
            }
        }

        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, Action alternative)
        {
            if (method != null)
            {
                method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
            else
            {
                SafeInvoke(alternative);
            }
        }

        public static TResult SafeInvoke<TResult>(this Func<TResult> method, TResult def)
        {
            if (method != null)
            {
                return method.Invoke();
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<TResult>(this Func<TResult> method, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke();
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, TResult>(this Func<T1, T2, TResult> method, T1 arg1, T2 arg2, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, TResult>(this Func<T1, T2, TResult> method, T1 arg1, T2 arg2, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
            else
            {
                return def;
            }
        }

        public static TResult SafeInvoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, Func<TResult> alternative, TResult def)
        {
            if (method != null)
            {
                return method.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            }
            else
            {
                return SafeInvoke(alternative, def);
            }
        }
    }
}

#endif