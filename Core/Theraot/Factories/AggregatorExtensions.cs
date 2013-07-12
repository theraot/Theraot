#if FAT
using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Factories
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class AggregatorExtensions
    {
        public static IAggregator<TExpectedInput, TOutput> Add<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, TActualInput input)
        {
            Check.NotNullArgument(builder, "builder").Process(Check.NotNullArgument(converter, "converter").Invoke(input));
            return builder;
        }

        public static IAggregator<TInput, TOutput> AddFromFactories<TInput, TOutput>(this IAggregator<TInput, TOutput> builder, IEnumerable<IFactory<TInput>> items)
        {
            IAggregator<TInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.AddFromFactory(item);
            }
            return _builder;
        }

        public static IAggregator<TExpectedInput, TOutput> AddFromFactories<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, IEnumerable<IFactory<TActualInput>> items)
        {
            IAggregator<TExpectedInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.AddFromFactory(converter, item);
            }
            return _builder;
        }

        public static IAggregator<TInput, TOutput> AddFromFactory<TInput, TOutput>(this IAggregator<TInput, TOutput> builder, IFactory<TInput> input)
        {
            Check.NotNullArgument(builder, "builder").Process(Check.NotNullArgument(input, "input").Create());
            return builder;
        }

        public static IAggregator<TExpectedInput, TOutput> AddFromFactory<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, IFactory<TActualInput> input)
        {
            Check.NotNullArgument(builder, "builder").Process(Check.NotNullArgument(converter, "converter").Invoke(Check.NotNullArgument(input, "input").Create()));
            return builder;
        }

        public static IAggregator<TInput, TOutput> AddFromFunction<TInput, TOutput>(this IAggregator<TInput, TOutput> builder, Func<TInput> input)
        {
            Check.NotNullArgument(builder, "builder").Process(Check.NotNullArgument(input, "input")());
            return builder;
        }

        public static IAggregator<TExpectedInput, TOutput> AddFromFunction<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, Func<TActualInput> input)
        {
            Check.NotNullArgument(builder, "builder").Process(Check.NotNullArgument(converter, "converter").Invoke(Check.NotNullArgument(input, "input")()));
            return builder;
        }

        public static IAggregator<TInput, TOutput> AddFromFunctions<TInput, TOutput>(this IAggregator<TInput, TOutput> builder, IEnumerable<Func<TInput>> items)
        {
            IAggregator<TInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.Process(item());
            }
            return _builder;
        }

        public static IAggregator<TExpectedInput, TOutput> AddFromFunctions<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, IEnumerable<Func<TActualInput>> items)
        {
            IAggregator<TExpectedInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.Add(converter, item());
            }
            return _builder;
        }

        public static IAggregator<TInput, TOutput> AddItems<TInput, TOutput>(this IAggregator<TInput, TOutput> builder, IEnumerable<TInput> items)
        {
            IAggregator<TInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.Process(item);
            }
            return _builder;
        }

        public static IAggregator<TExpectedInput, TOutput> AddItems<TExpectedInput, TActualInput, TOutput>(this IAggregator<TExpectedInput, TOutput> builder, Converter<TActualInput, TExpectedInput> converter, IEnumerable<TActualInput> items)
        {
            IAggregator<TExpectedInput, TOutput> _builder = Check.NotNullArgument(builder, "builder");
            foreach (var item in Check.NotNullArgument(items, "items"))
            {
                _builder.Add(converter, item);
            }
            return _builder;
        }
    }
}
#endif