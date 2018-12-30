using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
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
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
            }
            if (message != null)
            {
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Message: {message}");
            }
            throw new AssertionFailedException($"Expected: {typeof(TException).Name}");
        }

        public static void Throws<TException, T>(Func<T> func, string message = null)
            where TException : Exception
        {
            try
            {
                GC.KeepAlive(func());
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
                    throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception} - Message: {message}", exception);
                }
                throw new AssertionFailedException($"Expected: {typeof(TException).Name} - Found: {exception}", exception);
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
            if (exception is AssertionFailedException)
            {
                Console.WriteLine(exception.Message);
                return;
            }
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
                report.Append("\r\n\r\n");
            }
            Console.WriteLine(report.ToString());
        }

        public static void Main()
        {
            var ignoredCategories = new string[]
            {
            };
            var tests = GetAllTests(ignoredCategories);
            var stopwatch = new Stopwatch();
            Console.WriteLine();
            foreach (var test in tests)
            {
                using (test)
                {
                    object capturedResult = null;
                    Exception capturedException = null;
                    Console.Write($"{test.Name}");
                    var parameters = test.GenerateParameters();
                    Console.Write($"({StringHelper.Implode(", ", parameters)})");
                    try
                    {
                        stopwatch.Restart();
                        capturedResult = test.Invoke(parameters);
                        stopwatch.Stop();
                    }
                    catch (Exception exception)
                    {
                        stopwatch.Stop();
                        capturedException = exception;
                    }
                    if (capturedException == null)
                    {
                        Console.Write($"-> ok {capturedResult} ({stopwatch.Elapsed})");
                    }
                    else
                    {
                        Console.WriteLine($"-> error ({stopwatch.Elapsed})");
                        ExceptionReport(capturedException);
                    }
                }
                Console.WriteLine();
            }
            Exit();
        }

        [Conditional("DEBUG")]
        private static void Exit()
        {
            Console.WriteLine("[Press any key to exit]");
            var readKey = typeof(Console).GetTypeInfo().GetMethod("ReadKey", ArrayReservoir<Type>.EmptyArray);
            readKey?.Invoke(null, ArrayReservoir<object>.EmptyArray);
        }

        private static IEnumerable<Test> GetAllTests(string[] ignoredCategories)
        {
            return TypeDiscoverer.GetAllTypes()
                .Where(IsTestType)
                .SelectMany(t => t.GetTypeInfo().GetMethods())
                .Where(IsTestMethod)
                .Select(method => new TestMethod(method))
                .Where(testMethod => !testMethod.Categories.Overlaps(ignoredCategories))
                .Select(testMethod => new Test(testMethod));
        }

        private static bool IsTestMethod(MethodInfo methodInfo)
        {
            return methodInfo.HasAttribute<TestAttribute>() && !methodInfo.HasAttribute<IgnoreAttribute>();
        }

        private static bool IsTestType(Type type)
        {
            return type.HasAttribute<TestFixtureAttribute>() && !type.HasAttribute<IgnoreAttribute>();
        }

        private sealed class Test : IDisposable
        {
            private readonly bool _isolatedThread;
            private readonly ParameterInfo[] _parameterInfos;
            private readonly Type[] _preferredGenerators;
            private readonly int _repeat;
            private Delegate _delegate;
            private object _instance;

            public Test(TestMethod testMethod)
            {
                var method = testMethod.Method;
                var type = method.DeclaringType;
                if (type == null)
                {
                    throw new ArgumentException();
                }
                if (method.IsStatic)
                {
                    _instance = null;
                }
                else
                {
                    _instance = Activator.CreateInstance(type);
                }
                _delegate = TypeHelper.BuildDelegate(method, _instance);
                _parameterInfos = method.GetParameters();
                _isolatedThread = testMethod.TestAttribute.IsolatedThread;
                _preferredGenerators = testMethod.PreferredGenerators;
                Name = method.Name;
                _repeat = testMethod.TestAttribute.Repeat;
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

            public object Invoke(object[] parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException(nameof(parameters));
                }
                var @delegate = Volatile.Read(ref _delegate);
                if (@delegate == null)
                {
                    throw new ObjectDisposedException(nameof(Test));
                }
                if (_isolatedThread)
                {
                    Exception capturedException = null;
                    object capturedResult = null;
                    var thread = new Thread
                    (
                        () =>
                        {
                            try
                            {
                                for (var iteration = 0; iteration < _repeat; iteration++)
                                {
                                    capturedResult = _delegate.DynamicInvoke(parameters);
                                }
                            }
                            catch (TargetInvocationException exception)
                            {
                                capturedException = exception.InnerException ?? exception;
                            }
                        }
                    );
                    thread.Start();
                    thread.Join();
                    if (capturedException != null)
                    {
                        throw capturedException;
                    }
                    return capturedResult;
                }
                try
                {
                    object capturedResult = null;
                    for (var iteration = 0; iteration < _repeat; iteration++)
                    {
                        capturedResult = _delegate.DynamicInvoke(parameters);
                    }
                    return capturedResult;
                }
                catch (TargetInvocationException exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw;
                }
            }

            public object[] GenerateParameters()
            {
                var parameterInfos = _parameterInfos;
                var preferredGenerators = _preferredGenerators;
                var parameters = new object[parameterInfos.Length];
                for (var index = 0; index < parameters.Length; index++)
                {
                    var parameterInfo = parameterInfos[index];
                    var preferredGenerator = parameterInfo.GetAttributes<UseGeneratorAttribute>(false).FirstOrDefault();
                    var generators = preferredGenerator == null ? preferredGenerators : new[] { preferredGenerator.GeneratorType };
                    parameters[index] = DataGenerator.Get(parameterInfo.ParameterType, generators);
                }
                return parameters;
            }
        }

        private sealed class TestMethod
        {
            public TestMethod(MethodInfo method)
            {
                Method = method;
                Categories = method.GetAttributes<CategoryAttribute>(false).Select(category => category.Name);
                TestAttribute = method.GetAttributes<TestAttribute>(false).First();
                PreferredGenerators = method.GetAttributes<UseGeneratorAttribute>(false).Select(attribute => attribute.GeneratorType).ToArray();
            }

            public IEnumerable<string> Categories { get; }
            public MethodInfo Method { get; }
            public Type[] PreferredGenerators { get; }
            public TestAttribute TestAttribute { get; }
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
        public TestAttribute()
        {
            Repeat = 1;
        }

        public bool IsolatedThread { get; set; }

        public int Repeat { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestFixtureAttribute : Attribute
    {
        // Empty
    }
}