using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Reflection;

namespace TestRunner
{
    public static class Program
    {
        public static void ExceptionReport(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            while (true)
            {
                if (exception is TargetInvocationException targetInvocationException && targetInvocationException.InnerException != null)
                {
                    exception = targetInvocationException.InnerException;
                    continue;
                }
                if (exception is AggregateException aggregateException && aggregateException.InnerException != null)
                {
                    exception = aggregateException.InnerException;
                    continue;
                }
                break;
            }
            if (exception is AssertionFailedException)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(StringHelper.Implode("\r\n", exception.StackTrace.Split('\r', '\n').Skip(1)));
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
            Console.WriteLine(report);
        }

        public static void Main()
        {
            var ignoredCategories = new[]
            {
                "Performance"
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
            Console.ReadKey();
        }

        private static IEnumerable<Test> GetAllTests(string[] ignoredCategories)
        {
            return TypeDiscoverer.GetAllTypes()
                .Where(type => type.HasAttribute<TestFixtureAttribute>())
                .Select(type => new TestFixture(type))
                .Where(testFixture => testFixture.TestFixtureAttribute != null && !testFixture.Categories.Overlaps(ignoredCategories))
                .SelectMany(testFixture => testFixture.Type.GetTypeInfo().GetMethods())
                .Select(method => new TestMethod(method))
                .Where(testMethod => testMethod.TestAttribute != null && !testMethod.Categories.Overlaps(ignoredCategories))
                .Select(testMethod => new Test(testMethod));
        }

        private sealed class Test : IDisposable
        {
            private Delegate _delegate;
            private object _instance;
            private readonly bool _isolatedThread;
            private readonly ParameterInfo[] _parameterInfos;
            private readonly Type[] _preferredGenerators;
            private readonly int _repeat;

            public Test(TestMethod testMethod)
            {
                var method = testMethod.Method;
                var type = method.DeclaringType;
                if (type == null)
                {
                    throw new ArgumentException();
                }
                _instance = method.IsStatic ? null : Activator.CreateInstance(type);
                _delegate = DelegateBuilder.BuildDelegate(method, _instance);
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
                object capturedResult = null;
                if (_isolatedThread)
                {
                    Exception capturedException = null;
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
                            catch (Exception exception)
                            {
                                capturedException = exception;
                            }
                        }
                    );
                    thread.Start();
                    thread.Join();
                    if (capturedException != null)
                    {
                        throw capturedException;
                    }
                }
                else
                {
                    for (var iteration = 0; iteration < _repeat; iteration++)
                    {
                        capturedResult = _delegate.DynamicInvoke(parameters);
                    }
                }

                if (!(capturedResult is Task task))
                {
                    return capturedResult;
                }

                capturedResult = null;
                task.Wait();
                capturedResult = task.GetType().GetTypeInfo().GetProperty("Result")?.GetValue(task, ArrayReservoir<object>.EmptyArray);
                return capturedResult;
            }
        }

        private sealed class TestFixture
        {
            public TestFixture(Type type)
            {
                Type = type;
                var testFixtureAttributes = type.GetAttributes<TestFixtureAttribute>(true);
                if (testFixtureAttributes == null || testFixtureAttributes.Length <= 0)
                {
                    return;
                }
                TestFixtureAttribute = testFixtureAttributes[0];
                Categories = type.GetAttributes<CategoryAttribute>(false).Select(category => category.Name);
            }

            public IEnumerable<string> Categories { get; }
            public TestFixtureAttribute TestFixtureAttribute { get; }
            public Type Type { get; }
        }

        private sealed class TestMethod
        {
            public TestMethod(MethodInfo method)
            {
                Method = method;
                var testAttributes = method.GetAttributes<TestAttribute>(true);
                if (testAttributes == null || testAttributes.Length <= 0 || testAttributes[0].Ignore)
                {
                    return;
                }
                TestAttribute = testAttributes[0];
                Categories = method.GetAttributes<CategoryAttribute>(false).Select(category => category.Name);
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
            // Empty
        }

        public AssertionFailedException(string message)
            : base(message)
        {
            // Empty
        }

        public AssertionFailedException(string message, Exception inner)
            : base(message, inner)
        {
            // Empty
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public sealed class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class TestAttribute : Attribute
    {
        public TestAttribute()
        {
            Repeat = 1;
        }

        public bool Ignore { get; set; }

        public bool IsolatedThread { get; set; }

        public int Repeat { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TestFixtureAttribute : Attribute
    {
        // Empty
    }
}