using System;
using System.Globalization;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "False Positive")]
        public static bool CanCreateNeedle<T, TNeedle>()
            where TNeedle : INeedle<T>
        {
            return NeedleCreator<T, TNeedle>.CanCreate;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "False Positive")]
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

        public static TNeedle CreateNestedNeedle<T, TNeedle, TNestedNeedle>(INeedle<T> target)
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
        
        public static bool TryGet<T>(this IReadOnlyNeedle<T> needle, out T target)
        {
            var _needle = Check.NotNullArgument(needle, "needle");
            target = _needle.Value;
            return _needle.IsAlive;
        }

        private static class DeferredNeedleCreator<T, TNeedle>
            where TNeedle : INeedle<T>
        {
            private static bool _canCreate;
            private static Func<Func<T>, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static DeferredNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<Func<T>, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<T, TNeedle> tmpA;
                    _canCreate = TypeHelper.TryGetCreate<T, TNeedle>(out tmpA);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            return tmpA(target.Invoke());
                        };
                    }
                    else
                    {
                        Func<TNeedle> tmpB;
                        _canCreate = TypeHelper.TryGetCreate<TNeedle>(out tmpB);
                        if (_canCreate)
                        {
                            _create =
                            (target) =>
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
            private static bool _canCreate;
            private static Func<Func<T>, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static DeferredReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<Func<T>, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<T, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate<T, TNeedle>(out tmp);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            return tmp(target.Invoke());
                        };
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
            private static bool _canCreate;
            private static Func<T, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static NeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<T, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<TNeedle> tmpA;
                    _canCreate = TypeHelper.TryGetCreate<TNeedle>(out tmpA);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            var needle = tmpA.Invoke();
                            needle.Value = target;
                            return needle;
                        };
                    }
                    else
                    {
                        Func<Func<T>, TNeedle> tmpB;
                        _canCreate = TypeHelper.TryGetCreate<Func<T>, TNeedle>(out tmpB);
                        if (_canCreate)
                        {
                            _create =
                            (target) =>
                            {
                                return tmpB(() => target);
                            };
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
            private static bool _canCreate;
            private static Func<INeedle<T>, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static NestedNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<INeedle<T>, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<Func<INeedle<T>>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate<Func<INeedle<T>>, TNeedle>(out tmp);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            return tmp(() => target);
                        };
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
            private static bool _canCreate;
            private static Func<IReadOnlyNeedle<T>, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static NestedReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<IReadOnlyNeedle<T>, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<Func<IReadOnlyNeedle<T>>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate<Func<IReadOnlyNeedle<T>>, TNeedle>(out tmp);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            return tmp(() => target);
                        };
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
            private static bool _canCreate;
            private static Func<T, TNeedle> _create;

            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Expensive Initialization")]
            static ReadOnlyNeedleCreator()
            {
                _canCreate = TypeHelper.TryGetCreate<T, TNeedle>(out _create);
                if (!_canCreate)
                {
                    Func<Func<T>, TNeedle> tmp;
                    _canCreate = TypeHelper.TryGetCreate<Func<T>, TNeedle>(out tmp);
                    if (_canCreate)
                    {
                        _create =
                        (target) =>
                        {
                            return tmp(() => target);
                        };
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