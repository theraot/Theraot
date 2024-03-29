﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Linq.Expressions
{
    internal sealed class ExpressionStringBuilder : ExpressionVisitor
    {
        private readonly StringBuilder _out;

        // Associate every unique label or anonymous parameter in the tree with an integer.
        // Labels are displayed as UnnamedLabel_#; parameters are displayed as Param_#.
        private Dictionary<object, int>? _ids;

        private ExpressionStringBuilder()
        {
            _out = new StringBuilder();
        }

        public override string ToString()
        {
            return _out.ToString();
        }

        internal static string CatchBlockToString(CatchBlock node)
        {
            var esb = new ExpressionStringBuilder();
            esb.VisitCatchBlock(node);
            return esb.ToString();
        }

        internal static string ElementInitBindingToString(ElementInit node)
        {
            var esb = new ExpressionStringBuilder();
            esb.VisitElementInit(node);
            return esb.ToString();
        }

        internal static string ExpressionToString(Expression node)
        {
            var esb = new ExpressionStringBuilder();
            esb.Visit(node);
            return esb.ToString();
        }

        internal static string MemberBindingToString(MemberBinding node)
        {
            var esb = new ExpressionStringBuilder();
            esb.VisitMemberBinding(node);
            return esb.ToString();
        }

        internal static string SwitchCaseToString(SwitchCase node)
        {
            var esb = new ExpressionStringBuilder();
            esb.VisitSwitchCase(node);
            return esb.ToString();
        }

        protected internal override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                Visit(node.Left);
                Out('[');
                Visit(node.Right);
                Out(']');
            }
            else
            {
                string op;
                switch (node.NodeType)
                {
                    // AndAlso and OrElse were unintentionally changed in
                    // CLR 4. We changed them to "AndAlso" and "OrElse" to
                    // be 3.5 compatible, but it turns out 3.5 shipped with
                    // "&&" and "||". Oops.
                    case ExpressionType.AndAlso:
                        op = "AndAlso";
                        break;

                    case ExpressionType.OrElse:
                        op = "OrElse";
                        break;

                    case ExpressionType.Assign:
                        op = "=";
                        break;

                    case ExpressionType.Equal:
                        op = "==";
                        break;

                    case ExpressionType.NotEqual:
                        op = "!=";
                        break;

                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;

                    case ExpressionType.LessThan:
                        op = "<";
                        break;

                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;

                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;

                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        op = "+";
                        break;

                    case ExpressionType.AddAssign:
                    case ExpressionType.AddAssignChecked:
                        op = "+=";
                        break;

                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        op = "-";
                        break;

                    case ExpressionType.SubtractAssign:
                    case ExpressionType.SubtractAssignChecked:
                        op = "-=";
                        break;

                    case ExpressionType.Divide:
                        op = "/";
                        break;

                    case ExpressionType.DivideAssign:
                        op = "/=";
                        break;

                    case ExpressionType.Modulo:
                        op = "%";
                        break;

                    case ExpressionType.ModuloAssign:
                        op = "%=";
                        break;

                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        op = "*";
                        break;

                    case ExpressionType.MultiplyAssign:
                    case ExpressionType.MultiplyAssignChecked:
                        op = "*=";
                        break;

                    case ExpressionType.LeftShift:
                        op = "<<";
                        break;

                    case ExpressionType.LeftShiftAssign:
                        op = "<<=";
                        break;

                    case ExpressionType.RightShift:
                        op = ">>";
                        break;

                    case ExpressionType.RightShiftAssign:
                        op = ">>=";
                        break;

                    case ExpressionType.And:
                        op = IsBool(node) ? "And" : "&";
                        break;

                    case ExpressionType.AndAssign:
                        op = IsBool(node) ? "&&=" : "&=";
                        break;

                    case ExpressionType.Or:
                        op = IsBool(node) ? "Or" : "|";
                        break;

                    case ExpressionType.OrAssign:
                        op = IsBool(node) ? "||=" : "|=";
                        break;

                    case ExpressionType.ExclusiveOr:
                        op = "^";
                        break;

                    case ExpressionType.ExclusiveOrAssign:
                        op = "^=";
                        break;

                    case ExpressionType.Power:
                        op = "^";
                        break; // This was changed in CoreFx from ^ to **

                    case ExpressionType.PowerAssign:
                        op = "^=";
                        break; // This was changed in CoreFx from ^= to **=

                    case ExpressionType.Coalesce:
                        op = "??";
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                Out('(');
                Visit(node.Left);
                Out(' ');
                Out(op);
                Out(' ');
                Visit(node.Right);
                Out(')');
            }

            return node;
        }

        protected internal override Expression VisitBlock(BlockExpression node)
        {
            Out('{');
            foreach (var v in node.Variables)
            {
                Out("var ");
                Visit(v);
                Out(';');
            }

            Out(" ... }");
            return node;
        }

        protected internal override Expression VisitConditional(ConditionalExpression node)
        {
            Out("IIF(");
            Visit(node.Test);
            Out(", ");
            Visit(node.IfTrue);
            Out(", ");
            Visit(node.IfFalse);
            Out(')');
            return node;
        }

        protected internal override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                var sValue = node.Value.ToString();
                if (node.Value is string)
                {
                    Out('\"');
                    Out(sValue);
                    Out('\"');
                }
                else if (string.Equals(sValue, node.Value.GetType().ToString(), StringComparison.Ordinal))
                {
                    Out("value(");
                    Out(sValue);
                    Out(')');
                }
                else
                {
                    Out(sValue);
                }
            }
            else
            {
                Out("null");
            }

            return node;
        }

        protected internal override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            var s = string.Format
            (
                CultureInfo.CurrentCulture,
                "<DebugInfo({0}: {1}, {2}, {3}, {4})>",
                node.Document.FileName,
                node.StartLine,
                node.StartColumn,
                node.EndLine,
                node.EndColumn
            );
            Out(s);
            return node;
        }

        protected internal override Expression VisitDefault(DefaultExpression node)
        {
            Out("default(");
            Out(node.Type.Name);
            Out(')');
            return node;
        }

        protected internal override Expression VisitExtension(Expression node)
        {
            // Prefer an overridden ToString, if available.
            var toString = node.GetType().GetMethod(nameof(ToString), Type.EmptyTypes);
            if (toString != null && toString.DeclaringType != typeof(Expression) && !toString.IsStatic)
            {
                Out(node.ToString());
                return node;
            }

            Out('[');
            // For 3.5 subclasses, print the NodeType.
            // For Extension nodes, print the class name.
            Out(node.NodeType == ExpressionType.Extension ? node.GetType().FullName : node.NodeType.ToString());
            Out(']');
            return node;
        }

        protected internal override Expression VisitGoto(GotoExpression node)
        {
            string op;
            switch (node.Kind)
            {
                case GotoExpressionKind.Goto:
                    op = "goto";
                    break;

                case GotoExpressionKind.Break:
                    op = "break";
                    break;

                case GotoExpressionKind.Continue:
                    op = "continue";
                    break;

                case GotoExpressionKind.Return:
                    op = "return";
                    break;

                default:
                    throw new InvalidOperationException();
            }

            Out(op);
            Out(' ');
            DumpLabel(node.Target);
            if (node.Value == null)
            {
                return node;
            }

            Out(" (");
            Visit(node.Value);
            Out(")");
            return node;
        }

        protected internal override Expression VisitIndex(IndexExpression node)
        {
            if (node.Object != null)
            {
                Visit(node.Object);
            }
            else
            {
                if (node.Indexer?.DeclaringType != null)
                {
                    Out(node.Indexer.DeclaringType.Name);
                }
            }

            if (node.Indexer != null)
            {
                Out('.');
                Out(node.Indexer.Name);
            }

            Out('[');
            for (int i = 0, n = node.ArgumentCount; i < n; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                Visit(node.GetArgument(i));
            }

            Out(']');

            return node;
        }

        protected internal override Expression VisitInvocation(InvocationExpression node)
        {
            Out("Invoke(");
            Visit(node.Expression);
            for (int i = 0, n = node.ArgumentCount; i < n; i++)
            {
                Out(", ");
                Visit(node.GetArgument(i));
            }

            Out(')');
            return node;
        }

        protected internal override Expression VisitLabel(LabelExpression node)
        {
            Out("{ ... } ");
            DumpLabel(node.Target);
            Out(':');
            return node;
        }

        protected internal override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.ParameterCount == 1)
            {
                // p => body
                Visit(node.GetParameter(0));
            }
            else
            {
                // (p1, p2, ..., pn) => body
                Out('(');
                for (int i = 0, n = node.ParameterCount; i < n; i++)
                {
                    if (i > 0)
                    {
                        Out(", ");
                    }

                    Visit(node.GetParameter(i));
                }

                Out(')');
            }

            Out(" => ");
            Visit(node.Body);
            return node;
        }

        protected internal override Expression VisitListInit(ListInitExpression node)
        {
            Visit(node.NewExpression);
            Out(" {");
            for (int i = 0, n = node.Initializers.Count; i < n; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                VisitElementInit(node.Initializers[i]);
            }

            Out('}');
            return node;
        }

        protected internal override Expression VisitLoop(LoopExpression node)
        {
            Out("loop { ... }");
            return node;
        }

        protected internal override Expression VisitMember(MemberExpression node)
        {
            OutMember(node.Expression, node.Member);
            return node;
        }

        protected internal override Expression VisitMemberInit(MemberInitExpression node)
        {
            if (node.NewExpression.ArgumentCount == 0 && node.NewExpression.Type.Name.Contains('<'))
            {
                // anonymous type constructor
                Out("new");
            }
            else
            {
                Visit(node.NewExpression);
            }

            Out(" {");
            for (int i = 0, n = node.Bindings.Count; i < n; i++)
            {
                var b = node.Bindings[i];
                if (i > 0)
                {
                    Out(", ");
                }

                VisitMemberBinding(b);
            }

            Out('}');
            return node;
        }

        protected internal override Expression VisitMethodCall(MethodCallExpression node)
        {
            var start = 0;
            var ob = node.Object;

            if (node.Method.GetCustomAttributes(typeof(ExtensionAttribute), inherit: true).Length > 0)
            {
                start = 1;
                ob = node.GetArgument(0);
            }

            if (ob != null)
            {
                Visit(ob);
                Out('.');
            }

            Out(node.Method.Name);
            Out('(');
            for (int i = start, n = node.ArgumentCount; i < n; i++)
            {
                if (i > start)
                {
                    Out(", ");
                }

                Visit(node.GetArgument(i));
            }

            Out(')');
            return node;
        }

        protected internal override Expression VisitNew(NewExpression node)
        {
            Out("new ");
            Out(node.Type.Name);
            Out('(');
            var members = node.Members;
            for (var i = 0; i < node.ArgumentCount; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                if (members != null)
                {
                    var name = members[i].Name;
                    Out(name);
                    Out(" = ");
                }

                Visit(node.GetArgument(i));
            }

            Out(')');
            return node;
        }

        protected internal override Expression VisitNewArray(NewArrayExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    Out("new ");
                    Out(node.Type.ToString());
                    VisitExpressions('(', node.Expressions, ')');
                    break;

                case ExpressionType.NewArrayInit:
                    Out("new [] ");
                    VisitExpressions('{', node.Expressions, '}');
                    break;

                default:
                    break;
            }

            return node;
        }

        protected internal override Expression VisitParameter(ParameterExpression node)
        {
            if (node.IsByRef)
            {
                Out("ref ");
            }

            var name = node.Name;
            if (string.IsNullOrEmpty(name))
            {
                Out($"Param_{GetParamId(node)}");
            }
            else
            {
                Out(name!);
            }

            return node;
        }

        protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            VisitExpressions('(', node.Variables, ')');
            return node;
        }

        protected internal override Expression VisitSwitch(SwitchExpression node)
        {
            Out("switch ");
            Out('(');
            Visit(node.SwitchValue);
            Out(") { ... }");
            return node;
        }

        protected internal override Expression VisitTry(TryExpression node)
        {
            Out("try { ... }");
            return node;
        }

        protected internal override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            Out('(');
            Visit(node.Expression);
            switch (node.NodeType)
            {
                case ExpressionType.TypeIs:
                    Out(" Is ");
                    break;

                case ExpressionType.TypeEqual:
                    Out(" TypeEqual ");
                    break;

                default:
                    break;
            }

            Out(node.TypeOperand.Name);
            Out(')');
            return node;
        }

        protected internal override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    Out('-');
                    break;

                case ExpressionType.Not:
                    Out("Not(");
                    break;

                case ExpressionType.IsFalse:
                    Out("IsFalse(");
                    break;

                case ExpressionType.IsTrue:
                    Out("IsTrue(");
                    break;

                case ExpressionType.OnesComplement:
                    Out("~(");
                    break;

                case ExpressionType.ArrayLength:
                    Out("ArrayLength(");
                    break;

                case ExpressionType.Convert:
                    Out("Convert(");
                    break;

                case ExpressionType.ConvertChecked:
                    Out("ConvertChecked(");
                    break;

                case ExpressionType.Throw:
                    Out("throw(");
                    break;

                case ExpressionType.TypeAs:
                    Out('(');
                    break;

                case ExpressionType.UnaryPlus:
                    Out('+');
                    break;

                case ExpressionType.Unbox:
                    Out("Unbox(");
                    break;

                case ExpressionType.Increment:
                    Out("Increment(");
                    break;

                case ExpressionType.Decrement:
                    Out("Decrement(");
                    break;

                case ExpressionType.PreIncrementAssign:
                    Out("++");
                    break;

                case ExpressionType.PreDecrementAssign:
                    Out("--");
                    break;

                case ExpressionType.Quote:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                    break;

                default:
                    throw new InvalidOperationException();
            }

            Visit(node.Operand);

            switch (node.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.Quote:
                    break;

                case ExpressionType.TypeAs:
                    Out(" As ");
                    Out(node.Type.Name);
                    Out(')');
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    Out(')');
                    break; // These were changed in CoreFx to add the type name

                case ExpressionType.PostIncrementAssign:
                    Out("++");
                    break;

                case ExpressionType.PostDecrementAssign:
                    Out("--");
                    break;

                default:
                    Out(')');
                    break;
            }

            return node;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Out("catch (");
            Out(node.Test.Name);
            if (!string.IsNullOrEmpty(node.Variable?.Name))
            {
                Out(' ');
                Out(node.Variable!.Name!);
            }

            Out(") { ... }");
            return node;
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            Out(node.AddMethod.ToString());
            Out('(');
            for (int i = 0, n = node.ArgumentCount; i < n; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                Visit(node.GetArgument(i));
            }

            Out(')');
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Out(node.Member.Name);
            Out(" = ");
            Visit(node.Expression);
            return node;
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            Out(node.Member.Name);
            Out(" = {");
            for (int i = 0, n = node.Initializers.Count; i < n; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                VisitElementInit(node.Initializers[i]);
            }

            Out('}');
            return node;
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            Out(node.Member.Name);
            Out(" = {");
            for (int i = 0, n = node.Bindings.Count; i < n; i++)
            {
                if (i > 0)
                {
                    Out(", ");
                }

                VisitMemberBinding(node.Bindings[i]);
            }

            Out('}');
            return node;
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            Out("case ");
            VisitExpressions('(', node.TestValues, ')');
            Out(": ...");
            return node;
        }

        private static bool IsBool(Expression node)
        {
            return node.Type == typeof(bool) || node.Type == typeof(bool?);
        }

        private void DumpLabel(LabelTarget target)
        {
            var name = target.Name;
            if (string.IsNullOrEmpty(name))
            {
                var labelId = GetLabelId(target);
                Out($"UnnamedLabel_{labelId}");
            }
            else
            {
                Out(name!);
            }
        }

        private int GetId(object o)
        {
            if (_ids == null)
            {
                _ids = new Dictionary<object, int>();
            }

            if (_ids.TryGetValue(o, out var id))
            {
                return id;
            }

            id = _ids.Count;
            _ids.Add(o, id);

            return id;
        }

        private int GetLabelId(LabelTarget label)
        {
            return GetId(label);
        }

        private int GetParamId(ParameterExpression p)
        {
            return GetId(p);
        }

        private void Out(string s)
        {
            _out.Append(s);
        }

        private void Out(char c)
        {
            _out.Append(c);
        }

        // Prints ".instanceField" or "declaringType.staticField"
        private void OutMember(Expression? instance, MemberInfo member)
        {
            if (instance != null)
            {
                Visit(instance);
                Out('.');
            }
            else if (member.DeclaringType != null)
            {
                // For static members, include the type name
                Out(member.DeclaringType.Name);
                Out('.');
            }

            Out(member.Name);
        }

        private void VisitExpressions<T>(char open, ReadOnlyCollection<T> expressions, char close, string separator = ", ") where T : Expression
        {
            Out(open);
            if (expressions != null)
            {
                var isFirst = true;
                foreach (var e in expressions)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        Out(separator);
                    }

                    Visit(e);
                }
            }

            Out(close);
        }
    }
}

#endif