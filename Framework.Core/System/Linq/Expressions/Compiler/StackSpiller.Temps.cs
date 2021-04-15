#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Expressions.Compiler
{
    internal partial class StackSpiller
    {
        /// <summary>
        ///     The source of temporary variables introduced during stack spilling.
        /// </summary>
        private readonly TempMaker _tm = new();

        /// <summary>
        ///     Frees temporaries created since the last marking using <see cref="Mark" />.
        /// </summary>
        /// <param name="mark">The watermark value up to which to recycle used temporary variables.</param>
        /// <remarks>
        ///     This is a performance optimization to lower the overall number of temporaries needed.
        /// </remarks>
        private void Free(int mark)
        {
            _tm.Free(mark);
        }

        /// <summary>
        ///     Creates a temporary variable of the specified <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type for the temporary variable to create.</param>
        /// <returns>
        ///     A temporary variable of the specified <paramref name="type" />. When the temporary
        ///     variable is no longer used, it should be returned by using the <see cref="Mark" />
        ///     and <see cref="Free" /> mechanism provided.
        /// </returns>
        [return: NotNull]
        private ParameterExpression MakeTemp(Type type)
        {
            return _tm.Temp(type);
        }

        /// <summary>
        ///     Gets a watermark into the stack of used temporary variables. The returned
        ///     watermark value can be passed to <see cref="Free" /> to free all variables
        ///     below the watermark value, allowing them to be reused.
        /// </summary>
        /// <returns>
        ///     A watermark value indicating the number of temporary variables currently in use.
        /// </returns>
        /// <remarks>
        ///     This is a performance optimization to lower the overall number of temporaries needed.
        /// </remarks>
        private int Mark()
        {
            return _tm.Mark();
        }

        /// <summary>
        ///     Creates and returns a temporary variable to store the result of evaluating
        ///     the specified <paramref name="expression" />.
        /// </summary>
        /// <param name="expression">The expression to store in a temporary variable.</param>
        /// <param name="save">An expression that assigns the <paramref name="expression" /> to the created temporary variable.</param>
        /// <param name="byRef">Indicates whether the <paramref name="expression" /> represents a ByRef value.</param>
        /// <returns>The temporary variable holding the result of evaluating <paramref name="expression" />.</returns>
        private ParameterExpression ToTemp(Expression expression, out Expression save, bool byRef)
        {
            var tempType = byRef ? expression.Type.MakeByRefType() : expression.Type;
            var temp = MakeTemp(tempType);
            save = AssignBinaryExpression.Make(temp, expression, byRef);
            return temp;
        }

        /// <summary>
        ///     Verifies that all temporary variables get properly returned to the free list
        ///     after stack spilling for a lambda expression has taken place. This is used
        ///     to detect misuse of the <see cref="Mark" /> and <see cref="Free" /> methods.
        /// </summary>
        [Conditional("DEBUG")]
        private void VerifyTemps()
        {
            _tm.VerifyTemps();
        }

        /// <summary>
        ///     Utility to create and recycle temporary variables.
        /// </summary>
        private sealed class TempMaker
        {
            /// <summary>
            ///     List of free temporary variables. These can be recycled for new temporary variables.
            /// </summary>
            private List<ParameterExpression>? _freeTemps;

            /// <summary>
            ///     Index of the next temporary variable to create.
            ///     This value is used for naming temporary variables using an increasing index.
            /// </summary>
            private int _temp;

            /// <summary>
            ///     Stack of temporary variables that are currently in use.
            /// </summary>
            private Stack<ParameterExpression>? _usedTemps;

            /// <summary>
            ///     List of all temporary variables created by the stack spiller instance.
            /// </summary>
            internal List<ParameterExpression> Temps { get; } = new List<ParameterExpression>();

            /// <summary>
            ///     Frees temporaries created since the last marking using <see cref="Mark" />.
            /// </summary>
            /// <param name="mark">The watermark value up to which to recycle used temporary variables.</param>
            /// <remarks>
            ///     This is a performance optimization to lower the overall number of temporaries needed.
            /// </remarks>
            internal void Free(int mark)
            {
                // (_usedTemps != null) ==> (mark <= _usedTemps.Count)
                Debug.Assert(_usedTemps == null || mark <= _usedTemps.Count);
                // (_usedTemps == null) ==> (mark == 0)
                Debug.Assert(mark == 0 || _usedTemps != null);

                if (_usedTemps == null)
                {
                    return;
                }

                while (mark < _usedTemps.Count)
                {
                    FreeTemp(_usedTemps.Pop());
                }
            }

            /// <summary>
            ///     Gets a watermark into the stack of used temporary variables. The returned
            ///     watermark value can be passed to <see cref="Free" /> to free all variables
            ///     below the watermark value, allowing them to be reused.
            /// </summary>
            /// <returns>
            ///     A watermark value indicating the number of temporary variables currently in use.
            /// </returns>
            /// <remarks>
            ///     This is a performance optimization to lower the overall number of temporaries needed.
            /// </remarks>
            internal int Mark()
            {
                return _usedTemps?.Count ?? 0;
            }

            /// <summary>
            ///     Creates a temporary variable of the specified <paramref name="type" />.
            /// </summary>
            /// <param name="type">The type for the temporary variable to create.</param>
            /// <returns>
            ///     A temporary variable of the specified <paramref name="type" />. When the temporary
            ///     variable is no longer used, it should be returned by using the <see cref="Mark" />
            ///     and <see cref="Free" /> mechanism provided.
            /// </returns>
            [return: NotNull]
            internal ParameterExpression Temp(Type type)
            {
                ParameterExpression temp;

                if (_freeTemps != null)
                {
                    // Recycle from the free-list if possible.
                    for (var i = _freeTemps.Count - 1; i >= 0; i--)
                    {
                        temp = _freeTemps[i];
                        if (temp.Type != type)
                        {
                            continue;
                        }

                        _freeTemps.RemoveAt(i);
                        return UseTemp(temp);
                    }
                }

                // Not on the free-list, create a brand new one.
                temp = ParameterExpression.Make(type, $"$temp${_temp++}", isByRef: false);
                Temps.Add(temp);

                return UseTemp(temp);
            }

            /// <summary>
            ///     Verifies that all temporary variables get properly returned to the free list
            ///     after stack spilling for a lambda expression has taken place. This is used
            ///     to detect misuse of the <see cref="Mark" /> and <see cref="Free" /> methods.
            /// </summary>
            [Conditional("DEBUG")]
            internal void VerifyTemps()
            {
                Debug.Assert(_usedTemps == null || _usedTemps.Count == 0);
            }

            /// <summary>
            ///     Puts the temporary variable on the free list which is used by the
            ///     <see cref="Temp" /> method to reuse temporary variables.
            /// </summary>
            /// <param name="temp">The temporary variable to mark as no longer in use.</param>
            private void FreeTemp(ParameterExpression temp)
            {
                Debug.Assert(_freeTemps?.Contains(temp) != true);

                (_freeTemps ??= new List<ParameterExpression>()).Add(temp);
            }

            /// <summary>
            ///     Registers the temporary variable in the stack of used temporary variables.
            ///     The <see cref="Mark" /> and <see cref="Free" /> methods use a watermark index
            ///     into this stack to enable recycling temporary variables in bulk.
            /// </summary>
            /// <param name="temp">The temporary variable to mark as used.</param>
            /// <returns>The original temporary variable.</returns>
            [return: NotNullIfNotNull("temp")]
            private ParameterExpression UseTemp(ParameterExpression temp)
            {
                Debug.Assert(_freeTemps?.Contains(temp) != true);
                Debug.Assert(_usedTemps?.Contains(temp) != true);

                (_usedTemps ??= new Stack<ParameterExpression>()).Push(temp);

                return temp;
            }
        }
    }
}

#endif