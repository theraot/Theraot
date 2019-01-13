#if FAT

using System;
using System.Globalization;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static partial class NeedleHelper
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

        private static class DeferredNeedleCreator<T, TNeedle>
            where TNeedle : INeedle<T>
        {
            private static readonly Func<Func<T>, TNeedle> _create;

            static DeferredNeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<T, TNeedle> tmpA);
                    if (CanCreate)
                    {
                        _create = target => tmpA(target.Invoke());
                    }
                    else
                    {
                        CanCreate = TypeHelper.TryGetCreate(out Func<TNeedle> tmpB);
                        if (CanCreate)
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
                if (!CanCreate)
                {
                    _create =
                    _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(Func<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class DeferredReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly Func<Func<T>, TNeedle> _create;

            static DeferredReadOnlyNeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<T, TNeedle> tmp);
                    if (CanCreate)
                    {
                        _create = target => tmp(target.Invoke());
                    }
                }
                if (!CanCreate)
                {
                    _create =
                    _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(Func<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NeedleCreator<T, TNeedle>
            where TNeedle : INeedle<T>
        {
            private static readonly Func<T, TNeedle> _create;

            static NeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<TNeedle> tmpA);
                    if (CanCreate)
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
                        CanCreate = TypeHelper.TryGetCreate(out Func<Func<T>, TNeedle> tmpB);
                        if (CanCreate)
                        {
                            _create = target => tmpB(() => target);
                        }
                    }
                    if (!CanCreate)
                    {
                        _create =
                        _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    }
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(T target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NestedNeedleCreator<T, TNeedle>
        where TNeedle : INeedle<T>
        {
            private static readonly Func<INeedle<T>, TNeedle> _create;

            static NestedNeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<Func<INeedle<T>>, TNeedle> tmp);
                    if (CanCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                    if (!CanCreate)
                    {
                        _create =
                        _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                    }
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(INeedle<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class NestedReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly Func<IReadOnlyNeedle<T>, TNeedle> _create;

            static NestedReadOnlyNeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<Func<IReadOnlyNeedle<T>>, TNeedle> tmp);
                    if (CanCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                }
                if (!CanCreate)
                {
                    _create =
                    _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(IReadOnlyNeedle<T> target)
            {
                return _create.Invoke(target);
            }
        }

        private static class ReadOnlyNeedleCreator<T, TNeedle>
            where TNeedle : IReadOnlyNeedle<T>
        {
            private static readonly Func<T, TNeedle> _create;

            static ReadOnlyNeedleCreator()
            {
                CanCreate = TypeHelper.TryGetCreate(out _create);
                if (!CanCreate)
                {
                    CanCreate = TypeHelper.TryGetCreate(out Func<Func<T>, TNeedle> tmp);
                    if (CanCreate)
                    {
                        _create = target => tmp(() => target);
                    }
                }
                if (!CanCreate)
                {
                    _create =
                    _ => throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to find a way to create {0}", typeof(TNeedle).Name));
                }
            }

            public static bool CanCreate { get; }

            public static TNeedle Create(T target)
            {
                return _create.Invoke(target);
            }
        }
    }
}

#endif