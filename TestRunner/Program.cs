using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.Specialized;
using Theraot.Core;
using Theraot.Reflection;

namespace TestRunner
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T found, string message = null)
        {
            if (Equals(expected, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {expected} - Found: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {expected} - Found: {found}");
        }

        public static void AreNotEqual<T>(T expected, T found, string message = null)
        {
            if (!Equals(expected, found))
            {
                return;
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Unexpected: {found} - Message: {message}");
            }
            throw new AssertionFailedException($"Unexpected: {found}");
        }

        public static void Fail(string message = null)
        {
            if (message != null)
            {
                throw new AssertionFailedException($"Failed - Message: {message}");
            }
            throw new AssertionFailedException("Failed");
        }

        public static void IsFalse(bool found, string message = null)
        {
            AreEqual(true, found, message);
        }

        public static void IsNotNull<T>(T found, string message = null)
            where T : class
        {
            AreNotEqual(null, found, message);
        }

        public static void IsNull<T>(T found, string message = null)
            where T : class
        {
            AreEqual(null, found, message);
        }

        public static void IsTrue(bool found, string message = null)
        {
            AreEqual(true, found, message);
        }

        public static void Throws<TException>(Action action, string message = null)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException exception)
            {
                GC.KeepAlive(exception);
                return;
            }
            catch (Exception exception)
            {
                if (message != null)
                {
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}");
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}");
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name}");
        }
    }

    public static class Program
    {
        public static void ExceptionReport(Exception exception)
        {
            var report = new StringBuilder();
            report.Append("Exception");
            report.Append("\r\n\r\n");
            var current = exception;
            for (; current != null; current = current.InnerException)
            {
                report.Append
                (
                    StringHelper.Join
                    (
                        "\r\n\r\n",
                        "== Exception Type ==",
                        current.GetType().Name,
                        "== Exception Message ==",
                        current.Message,
                        "== Source ==",
                        current.Source,
                        "== Stacktrace ==",
                        current.StackTrace
                    )
                );
            }
            report.Append("\r\n\r\n");
            var extendedStackTrace = Environment.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            report.Append
            (
                StringHelper.Join
                (
                    "\r\n",
                    new ExtendedEnumerable<string>(Extensions.AsUnaryIEnumerable("== Reconstructed StackTrace ==\r\n"), extendedStackTrace.SkipItems(4))
                )
            );
            Console.WriteLine(report.ToString());
        }

        public static void Main()
        {
            var ignoredCategories = new string[]
            {
                
            };
            var tests = GetAllTests(ignoredCategories);
            var stopwatch = new Stopwatch();
            foreach (var test in tests)
            {
                stopwatch.Reset();
                using (test)
                {
                    try
                    {
                        var result = test.Invoke();
                        stopwatch.Stop();
                        Console.WriteLine($"{test.Name}: ok {result}");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"{test.Name}: error");
                        ExceptionReport(exception);
                    }
                    Console.WriteLine(stopwatch.Elapsed);
                }
                Console.WriteLine();
            }
            Exit();
        }

        [Conditional("DEBUG")]
        private static void Exit()
        {
            Console.WriteLine("[Press any key to exit]");
            Console.ReadKey();
        }

        private static IEnumerable<Test> GetAllTests(string[] ignoredCategories)
        {
            var programType = typeof(Program);
            var assembly = programType.GetTypeInfo().Assembly;
            return assembly.GetExportedTypes()
                .Where(IsTestType)
                .SelectMany(t => t.GetTypeInfo().GetMethods())
                .Where(IsTestMethod)
                .Select(method => new CategorizedMethod(method))
                .Where(categorizedMethod => !categorizedMethod.Categories.Overlaps(ignoredCategories))
                .Select(categorizedMethod => new Test(categorizedMethod.Method));
        }

        private static bool IsTestMethod(MethodInfo methodInfo)
        {
            return methodInfo.HasAttribute<TestAttribute>() && !methodInfo.HasAttribute<IgnoreAttribute>();
        }

        private static bool IsTestType(Type type)
        {
            return type.HasAttribute<TestFixtureAttribute>() && !type.HasAttribute<IgnoreAttribute>();
        }

        private sealed class CategorizedMethod
        {
            public CategorizedMethod(MethodInfo method)
            {
                Method = method;
                Categories = method.GetAttributes<CategoryAttribute>(false).Select(category => category.Name);
            }

            public IEnumerable<string> Categories { get; }
            public MethodInfo Method { get; }
        }

        private sealed class Test : IDisposable
        {
            private readonly Type[] _parameterTypes;
            private Delegate _delegate;
            private object _instance;

            public Test(MethodInfo methodInfo)
            {
                var type = methodInfo.DeclaringType;
                if (type == null)
                {
                    throw new ArgumentException();
                }
                if (methodInfo.IsStatic)
                {
                    _instance = null;
                }
                else
                {
                    _instance = Activator.CreateInstance(type);
                }
                _delegate = TypeHelper.BuildDelegate(methodInfo, _instance);
                _parameterTypes = methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();
                Name = methodInfo.Name;
            }
            public string Name { get; }

            public void Dispose()
            {
                var instance = Interlocked.Exchange(ref _instance, null);
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                Interlocked.Exchange(ref _delegate, null);
            }

            public object Invoke()
            {
                var @delegate = Volatile.Read(ref _delegate);
                if (@delegate == null)
                {
                    throw new ObjectDisposedException(nameof(Test));
                }
                var parameters = new object[_parameterTypes.Length];
                for (int index = 0; index < parameters.Length; index++)
                {
                    parameters[index] = DataGenerator.Get(_parameterTypes[index]);
                }
                return _delegate.DynamicInvoke(parameters);
            }
        }
    }

    public sealed class AssertionFailedException : Exception
    {

        public AssertionFailedException()
        {
        }

        public AssertionFailedException(string message)
            : base(message)
        {
        }

        public AssertionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        // Empty
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestAttribute : Attribute
    {
        // Empty
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestFixtureAttribute : Attribute
    {
        // Empty
    }
}
