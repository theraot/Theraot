#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
    /// <inheritdoc />
    /// <summary>
    ///     <para>Emits or clears a sequence point for debug information.</para>
    ///     <para>
    ///         This allows the debugger to highlight the correct source code when
    ///         debugging.
    ///     </para>
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugInfoExpressionProxy))]
    public class DebugInfoExpression : Expression
    {
        internal DebugInfoExpression(SymbolDocumentInfo document)
        {
            Document = document;
        }

        /// <summary>
        ///     Gets the <see cref="SymbolDocumentInfo" /> that represents the source file.
        /// </summary>
        public SymbolDocumentInfo Document { get; }

        /// <summary>
        ///     Gets the end column of this <see cref="DebugInfoExpression" />.
        /// </summary>
        public virtual int EndColumn => throw ContractUtils.Unreachable;

        /// <summary>
        ///     Gets the end line of this <see cref="DebugInfoExpression" />.
        /// </summary>
        public virtual int EndLine => throw ContractUtils.Unreachable;

        /// <summary>
        ///     Gets the value to indicate if the <see cref="DebugInfoExpression" /> is for clearing a sequence point.
        /// </summary>
        public virtual bool IsClear => throw ContractUtils.Unreachable;

        /// <inheritdoc />
        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from
        ///     <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.DebugInfo;

        /// <summary>
        ///     Gets the start column of this <see cref="DebugInfoExpression" />.
        /// </summary>
        public virtual int StartColumn => throw ContractUtils.Unreachable;

        /// <summary>
        ///     Gets the start line of this <see cref="DebugInfoExpression" />.
        /// </summary>
        public virtual int StartLine => throw ContractUtils.Unreachable;

        /// <inheritdoc />
        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        ///     (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public sealed override Type Type => typeof(void);

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitDebugInfo(this);
        }
    }

    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="DebugInfoExpression" /> for clearing a sequence point.
        /// </summary>
        /// <param name="document">The <see cref="SymbolDocumentInfo" /> that represents the source file.</param>
        /// <returns>An instance of <see cref="DebugInfoExpression" /> for clearing a sequence point.</returns>
        public static DebugInfoExpression ClearDebugInfo(SymbolDocumentInfo document)
        {
            ContractUtils.RequiresNotNull(document, nameof(document));

            return new ClearDebugInfoExpression(document);
        }

        /// <summary>
        ///     Creates a <see cref="DebugInfoExpression" /> with the specified span.
        /// </summary>
        /// <param name="document">The <see cref="SymbolDocumentInfo" /> that represents the source file.</param>
        /// <param name="startLine">The start line of this <see cref="DebugInfoExpression" />. Must be greater than 0.</param>
        /// <param name="startColumn">The start column of this <see cref="DebugInfoExpression" />. Must be greater than 0.</param>
        /// <param name="endLine">
        ///     The end line of this <see cref="DebugInfoExpression" />. Must be greater or equal than the start
        ///     line.
        /// </param>
        /// <param name="endColumn">
        ///     The end column of this <see cref="DebugInfoExpression" />. If the end line is the same as the
        ///     start line, it must be greater or equal than the start column. In any case, must be greater than 0.
        /// </param>
        /// <returns>An instance of <see cref="DebugInfoExpression" />.</returns>
        public static DebugInfoExpression DebugInfo(SymbolDocumentInfo document, int startLine, int startColumn, int endLine, int endColumn)
        {
            ContractUtils.RequiresNotNull(document, nameof(document));
            if (startLine == 0xfeefee && startColumn == 0 && endLine == 0xfeefee && endColumn == 0)
            {
                return new ClearDebugInfoExpression(document);
            }

            ValidateSpan(startLine, startColumn, endLine, endColumn);
            return new SpanDebugInfoExpression(document, startLine, startColumn, endLine, endColumn);
        }

        private static void ValidateSpan(int startLine, int startColumn, int endLine, int endColumn)
        {
            if (startLine < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startLine), $"{nameof(startLine)} must be greater than or equal to {1}");
            }

            if (startColumn < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startColumn), $"{nameof(startColumn)} must be greater than or equal to {1}");
            }

            if (endLine < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(endLine), $"{nameof(endLine)} must be greater than or equal to {1}");
            }

            if (endColumn < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(endColumn), $"{nameof(endColumn)} must be greater than or equal to {1}");
            }

            if (startLine > endLine)
            {
                throw new ArgumentException("Start and End must be well ordered");
            }

            if (startLine == endLine && startColumn > endColumn)
            {
                throw new ArgumentException("Start and End must be well ordered");
            }
        }
    }

    internal sealed class ClearDebugInfoExpression : DebugInfoExpression
    {
        internal ClearDebugInfoExpression(SymbolDocumentInfo document)
            : base(document)
        {
            // Empty
        }

        public override int EndColumn => 0;
        public override int EndLine => 0xfeefee;
        public override bool IsClear => true;

        public override int StartColumn => 0;
        public override int StartLine => 0xfeefee;
    }

    internal sealed class SpanDebugInfoExpression : DebugInfoExpression
    {
        internal SpanDebugInfoExpression(SymbolDocumentInfo document, int startLine, int startColumn, int endLine, int endColumn)
            : base(document)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        public override int EndColumn { get; }

        public override int EndLine { get; }

        public override bool IsClear => false;
        public override int StartColumn { get; }

        public override int StartLine { get; }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitDebugInfo(this);
        }
    }
}

#endif