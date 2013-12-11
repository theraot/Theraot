using System;
using System.Collections.Generic;

using Theraot.Core;

namespace Theraot.Collections
{
    public static class ProgressorBuilder
    {
        public static IProgressor<T> CreateConversionProgressor<T, TInput>(IEnumerable<TInput> wrapped, Converter<TInput, T> converter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                           if (enumerator.MoveNext())
                           {
                               item = _converter.Invoke(enumerator.Current);
                               return true;
                           }
                           else
                           {
                               enumerator.Dispose();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateConversionProgressor<T, TInput>(IProgressor<TInput> wrapped, Converter<TInput, T> converter)
        {
            Check.NotNullArgument(wrapped, "wrapped");
            var _converter = Check.NotNullArgument(converter, "converter");
            var proxy = new ProxyObservable<T>();
            wrapped.Subscribe(new ConvertedObserver<TInput, T>(proxy, _converter));
            return new SilentProgressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                           if (wrapped.TryTake(out _item))
                           {
                               item = _converter.Invoke(_item);
                               return true;
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       },
            proxy
                   );
        }

        public static IProgressor<T> CreateConversionProgressor<T, TInput>(TryTake<TInput> tryTake, Converter<TInput, T> converter)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                           if (_tryTake.Invoke(out _item))
                           {
                               item = _converter.Invoke(_item);
                               return true;
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateConversionProgressor<T, TInput>(TryTake<TInput> tryTake, Action done, Converter<TInput, T> converter)
        {
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            var _done = Check.NotNullArgument(done, "done");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                           if (_tryTake.Invoke(out _item))
                           {
                               item = _converter.Invoke(_item);
                               return true;
                           }
                           else
                           {
                               _done.Invoke();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T>(IEnumerable<T> wrapped, Predicate<T> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                       again:
                           if (enumerator.MoveNext())
                           {
                               item = enumerator.Current;
                               if (_predicate.Invoke(item))
                               {
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               enumerator.Dispose();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T>(IProgressor<T> wrapped, Predicate<T> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            var proxy = new ProxyObservable<T>();
            wrapped.Subscribe(new FilteredObserver<T>(proxy, _predicate));
            return new SilentProgressor<T>
                   (
                       (out T item) =>
                       {
                       again:
                           if (_wrapped.TryTake(out item))
                           {
                               if (_predicate.Invoke(item))
                               {
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       },
            proxy
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T>(TryTake<T> tryTake, Action done, Predicate<T> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _done = Check.NotNullArgument(done, "done");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                       again:
                           if (_tryTake.Invoke(out item))
                           {
                               if (_predicate.Invoke(item))
                               {
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               _done.Invoke();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T>(TryTake<T> tryTake, Predicate<T> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                       again:
                           if (_tryTake.Invoke(out item))
                           {
                               if (_predicate.Invoke(item))
                               {
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T, TInput>(IEnumerable<TInput> wrapped, Predicate<TInput> predicate, Converter<TInput, T> converter)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _converter = Check.NotNullArgument(converter, "converter");
            var enumerator = Check.CheckArgument(Check.NotNullArgument(wrapped, "wrapped").GetEnumerator(), arg => arg != null, "wrapped.GetEnumerator()");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                       again:
                           if (enumerator.MoveNext())
                           {
                               TInput _item = enumerator.Current;
                               if (_predicate.Invoke(_item))
                               {
                                   item = _converter.Invoke(_item);
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               enumerator.Dispose();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T, TInput>(IProgressor<TInput> wrapped, Predicate<TInput> predicate, Converter<TInput, T> converter)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            var _converter = Check.NotNullArgument(converter, "converter");
            var proxy = new ProxyObservable<T>();
            wrapped.Subscribe(new FilteredConvertedObserver<TInput, T>(proxy, _converter, _predicate));
            return new SilentProgressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                       again:
                           if (_wrapped.TryTake(out _item))
                           {
                               if (_predicate.Invoke(_item))
                               {
                                   item = _converter.Invoke(_item);
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       },
            proxy
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T, TInput>(TryTake<TInput> tryTake, Action done, Predicate<TInput> predicate, Converter<TInput, T> converter)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            var _done = Check.NotNullArgument(done, "done");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                       again:
                           if (_tryTake.Invoke(out _item))
                           {
                               if (_predicate.Invoke(_item))
                               {
                                   item = _converter.Invoke(_item);
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               _done.Invoke();
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateFilterProgressor<T, TInput>(TryTake<TInput> tryTake, Predicate<TInput> predicate, Converter<TInput, T> converter)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _tryTake = Check.NotNullArgument(tryTake, "tryTake");
            var _converter = Check.NotNullArgument(converter, "converter");
            return new Progressor<T>
                   (
                       (out T item) =>
                       {
                           TInput _item;
                       again:
                           if (_tryTake.Invoke(out _item))
                           {
                               if (_predicate.Invoke(_item))
                               {
                                   item = _converter.Invoke(_item);
                                   return true;
                               }
                               else
                               {
                                   goto again;
                               }
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       }
                   );
        }

        public static IProgressor<T> CreateProgressor<T>(IProgressor<T> wrapped)
        {
            var _wrapped = Check.NotNullArgument(wrapped, "wrapped");
            var proxy = new ProxyObservable<T>();
            wrapped.Subscribe(proxy);
            return new SilentProgressor<T>
                   (
                       (out T item) =>
                       {
                           if (_wrapped.TryTake(out item))
                           {
                               return true;
                           }
                           else
                           {
                               item = default(T);
                               return false;
                           }
                       },
            proxy
                   );
        }
    }
}