// Needed for NET40

using System;
using System.Globalization;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static class NeedleHelper
    {
        public static bool CanCreateDeferredNeedle<T, TNeedle>()
            where TNeedle : INeedle<T>
        {
            return DeferredNeedleCreator<T, TNeedle>.CanCreate;
        }

        public static bool CanCreateDeferredReadOnlyNeedle<T, TNeedle>()
            where TNeedle : IReadOnlyNeedle<T>
        {
            return DeferredReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
        }

        public static bool CanCreateNeedle<T, TNeedle>()
            where TNeedle : INeedle<T>
        {
            return NeedleCreator<T, TNeedle>.CanCreate;
        }

        public static bool CanCreateNestedNeedle<T, TNeedle>()
            where TNeedle : INeedle<T>
        {
            return NestedNeedleCreator<T, TNeedle>.CanCreate;
        }

        public static bool CanCreateNestedReadOnlyNeedle<T, TNeedle>()
            where TNeedle : IReadOnlyNeedle<T>
        {
            return NestedReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
        }

        public static bool CanCreateReadOnlyNeedle<T, TNeedle>()
            where TNeedle : IReadOnlyNeedle<T>
        {
            return ReadOnlyNeedleCreator<T, TNeedle>.CanCreate;
        }

        public static TNeedle CreateDeferredNeedle<T, TNeedle>(Func<T> target)
            where TNeedle : INeedle<T>
        {
            return DeferredNeedleCreator<T, TNeedle>.Create(target);
        }

        public static TNeedle CreateDeferredReadOnlyNeedle<T, TNeedle>(Func<T> target)
            where TNeedle : IReadOnlyNeedle<T>
        {
            return DeferredReadOnlyNeedleCreator<T, TNeedle>.Create(target);
        }

        public static TNeedle CreateNeedle<T, TNeedle>(T target)
            where TNeedle : INeedle<T>
        {
            return NeedleCreator<T, TNeedle>.Create(target);
        }

        public static TNeedle CreateNestedNeedle<T, TNeedle>(INeedle<T> target)
           where TNeedle : INeedle<T>
        {
            return NestedNeedleCreator<T, TNeedle>.Create(target);
        }

        public static TNeedle CreateReadOnlyNeedle<T, TNeedle>(T target)
            where TNeedle : IReadOnlyNeedle<T>
        {
            return ReadOnlyNeedleCreator<T, TNeedle>.Create(target);
        }

        public static TNeedle CreateReadOnlyNestedNeedle<T, TNeedle>(IReadOnlyNeedle<T> target)
            where TNeedle : IReadOnlyNeedle<T>
        {
            return NestedReadOnlyNeedleCreator<T, TNeedle>.Create(target);
        }

        public static bool Retrieve<T, TNeedle>(this TNeedle needle, out T target)
            where TNeedle : IRecyclableNeedle<T>
        {
            if (ReferenceEquals(needle, null))
            {
                target = default(T);
                return false;
            }
            bool done;
            var cacheNeedle = needle as ICacheNeedle<T>;
            if (cacheNeedle == null)
            {
                target = ((INeedle<T>)needle).Value;
                done = needle.IsAlive;
            }
            else
            {
                done = cacheNeedle.TryGetValue(out target);
            }
            if (done)
            {
                needle.Free();
            }
            return done;
        }

        public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
        {
            if (needle == null)
            {
                target = default(T);
                return false;
            }
            var cacheNeedle = needle as ICacheNeedle<T>;
            if (cacheNeedle != null)
            {
                return cacheNeedle.TryGetValue(out target);
            }
            target = ((INeedle<T>)needle).Value;
            return needle.IsAlive;
        }

        private static class DeferredNeedleCreator<T, TNeedle>
            where TNeedle : INeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<Func<T>, TNeedle> _create;

            static DeferredNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<T, TNeedle> tmpA;
                    _canCreate = TypeHelper.TryGetCreate(out tmpA);
                    if (_canCreate)
                    {
                        _create = target => tmpA(target.Invoke());
                    }
                    else
                    {
                        Func<TNeedle> tmpB;
                        _canCreate = TypeHelper.TryGetCreate(out tmpB);
                        if (_canCreate)
                        {
                            _create =
                            target =>
                            {
                                var needle = tmpB.Invoke();
                                needle.Value = target.Invoke();
                                return needle;
                            };
                        }
                    }
                }
                if (!_canCreate)
                {
                    _create =
                    _ =>
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    };
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(Func<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class DeferredReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<Func<T>, TNeedle> _create;

            static DeferredReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<T, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate(out tmp);
                    if (_canCreate)
                    {
                        _create = target => tmp(target.Invoke());
                    }
                }
                if (!_canCreate)
                {
                    _create =
                    _ =>
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    };
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(Func<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NeedleCreator<T, TNeedle>
            where TNeedle : INeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<T, TNeedle> _create;

            static NeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<TNeedle> tmpA;
                    _canCreate = TypeHelper.TryGetCreate(out tmpA);
                    if (_canCreate)
                    {
                        _create =
                        target =>
                        {
                            var needle = tmpA.Invoke();
                            needle.Value = target;
                            return needle;
                        };
                    }
                    else
                    {
                        Func<Func<T>, TNeedle> tmpB;
                        _canCreate = TypeHelper.TryGetCreate(out tmpB);
                        if (_canCreate)
                        {
                            _create = target => tmpB(() => target);
                        }
                    }
                    if (!_canCreate)
                    {
                        _create =
                        _ =>
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                        };
                    }
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(T target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NestedNeedleCreator<T, TNeedle>
        where TNeedle : INeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<INeedle<T>, TNeedle> _create;

            static NestedNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<Func<INeedle<T>>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate(out tmp);
                    if (_canCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                    if (!_canCreate)
                    {
                        _create =
                        _ =>
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                        };
                    }
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(INeedle<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NestedReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<IReadOnlyNeedle<T>, TNeedle> _create;

            static NestedReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<Func<IReadOnlyNeedle<T>>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate(out tmp);
                    if (_canCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                }
                if (!_canCreate)
                {
                    _create =
                    _ =>
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    };
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(IReadOnlyNeedle<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class ReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly bool _canCreate;
            private static readonly Func<T, TNeedle> _create;

            static ReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate(out _create);
                if (!_canCreate)
                {
                    Func<Func<T>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate(out tmp);
                    if (_canCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                }
                if (!_canCreate)
                {
                    _create =
                    _ =>
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    };
                }
            }

            public static bool CanCreate
            {
                get
                {
                    return _canCreate;
                }
            }

            public static TNeedle Create(T target)
            {
                return _create.Invoke(target);
            }
        }
    }
}