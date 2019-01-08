#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Expression switchValue, params SwitchCase[] cases)
        {
            return Switch(switchValue, null, null, (IEnumerable<SwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="defaultBody">The result of the switch if no cases are matched.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, params SwitchCase[] cases)
        {
            return Switch(switchValue, defaultBody, null, (IEnumerable<SwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="defaultBody">The result of the switch if no cases are matched.</param>
        /// <param name="comparison">The equality comparison method to use.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
        {
            return Switch(switchValue, defaultBody, comparison, (IEnumerable<SwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="type">The result type of the switch.</param>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="defaultBody">The result of the switch if no cases are matched.</param>
        /// <param name="comparison">The equality comparison method to use.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, params SwitchCase[] cases)
        {
            return Switch(type, switchValue, defaultBody, comparison, (IEnumerable<SwitchCase>)cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="defaultBody">The result of the switch if no cases are matched.</param>
        /// <param name="comparison">The equality comparison method to use.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
        {
            return Switch(null, switchValue, defaultBody, comparison, cases);
        }

        /// <summary>
        /// Creates a <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="type">The result type of the switch.</param>
        /// <param name="switchValue">The value to be tested against each case.</param>
        /// <param name="defaultBody">The result of the switch if no cases are matched.</param>
        /// <param name="comparison">The equality comparison method to use.</param>
        /// <param name="cases">The valid cases for this switch.</param>
        /// <returns>The created <see cref="SwitchExpression"/>.</returns>
        public static SwitchExpression Switch(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, IEnumerable<SwitchCase> cases)
        {
            ExpressionUtils.RequiresCanRead(switchValue, nameof(switchValue));
            if (switchValue.Type == typeof(void))
            {
                throw Error.ArgumentCannotBeOfTypeVoid(nameof(switchValue));
            }

            var caseArray = Theraot.Collections.Extensions.AsArray(cases);
            ContractUtils.RequiresNotNullItems(caseArray, nameof(cases));

            // Type of the result. Either provided, or it is type of the branches.
            Type resultType;
            if (type != null)
            {
                resultType = type;
            }
            else if (caseArray.Length != 0)
            {
                resultType = caseArray[0].Body.Type;
            }
            else if (defaultBody != null)
            {
                resultType = defaultBody.Type;
            }
            else
            {
                resultType = typeof(void);
            }

            var customType = type != null;

            if (comparison != null)
            {
                ValidateMethodInfo(comparison, nameof(comparison));
                var pms = comparison.GetParameters();
                if (pms.Length != 2)
                {
                    throw Error.IncorrectNumberOfMethodCallArguments(comparison, nameof(comparison));
                }
                // Validate that the switch value's type matches the comparison method's
                // left hand side parameter type.
                var leftParam = pms[0];
                var liftedCall = false;
                if (!ParameterIsAssignable(leftParam, switchValue.Type))
                {
                    liftedCall = ParameterIsAssignable(leftParam, switchValue.Type.GetNonNullable());
                    if (!liftedCall)
                    {
                        throw Error.SwitchValueTypeDoesNotMatchComparisonMethodParameter(switchValue.Type, leftParam.ParameterType);
                    }
                }

                var rightParam = pms[1];
                foreach (var c in caseArray)
                {
                    ContractUtils.RequiresNotNull(c, nameof(cases));
                    ValidateSwitchCaseType(c.Body, customType, resultType, nameof(cases));
                    for (int i = 0, n = c.TestValues.Count; i < n; i++)
                    {
                        // When a comparison method is provided, test values can have different type but have to
                        // be reference assignable to the right hand side parameter of the method.
                        var rightOperandType = c.TestValues[i].Type;
                        if (liftedCall)
                        {
                            if (!rightOperandType.IsNullable())
                            {
                                throw Error.TestValueTypeDoesNotMatchComparisonMethodParameter(rightOperandType, rightParam.ParameterType);
                            }
                            rightOperandType = rightOperandType.GetNonNullable();
                        }
                        if (!ParameterIsAssignable(rightParam, rightOperandType))
                        {
                            throw Error.TestValueTypeDoesNotMatchComparisonMethodParameter(rightOperandType, rightParam.ParameterType);
                        }
                    }
                }

                // if we have a non-boolean user-defined equals, we don't want it.
                if (comparison.ReturnType != typeof(bool))
                {
                    throw Error.EqualityMustReturnBoolean(comparison, nameof(comparison));
                }
            }
            else if (caseArray.Length != 0)
            {
                // When comparison method is not present, all the test values must have
                // the same type. Use the first test value's type as the baseline.
                var firstTestValue = caseArray[0].TestValues[0];
                foreach (var c in caseArray)
                {
                    ContractUtils.RequiresNotNull(c, nameof(cases));
                    ValidateSwitchCaseType(c.Body, customType, resultType, nameof(cases));
                    // When no comparison method is provided, require all test values to have the same type.
                    for (int i = 0, n = c.TestValues.Count; i < n; i++)
                    {
                        if (!TypeUtils.AreEquivalent(firstTestValue.Type, c.TestValues[i].Type))
                        {
                            throw Error.AllTestValuesMustHaveSameType(nameof(cases));
                        }
                    }
                }

                // Now we need to validate that switchValue.Type and testValueType
                // make sense in an Equal node. Fortunately, Equal throws a
                // reasonable error, so just call it.
                var equal = Equal(switchValue, firstTestValue, false, null);

                // Get the comparison function from equals node.
                comparison = equal.Method;
            }

            if (defaultBody == null)
            {
                if (resultType != typeof(void))
                {
                    throw Error.DefaultBodyMustBeSupplied(nameof(defaultBody));
                }
            }
            else
            {
                ValidateSwitchCaseType(defaultBody, customType, resultType, nameof(defaultBody));
            }

            return new SwitchExpression(resultType, switchValue, defaultBody, comparison, caseArray);
        }

        /// <summary>
        /// If custom type is provided, all branches must be reference assignable to the result type.
        /// If no custom type is provided, all branches must have the same type - resultType.
        /// </summary>
        private static void ValidateSwitchCaseType(Expression @case, bool customType, Type resultType, string parameterName)
        {
            if (customType)
            {
                if (resultType != typeof(void))
                {
                    if (!resultType.IsReferenceAssignableFromInternal(@case.Type))
                    {
                        throw Error.ArgumentTypesMustMatch(parameterName);
                    }
                }
            }
            else
            {
                if (resultType != @case.Type)
                {
                    throw Error.AllCaseBodiesMustHaveSameType(parameterName);
                }
            }
        }
    }

    /// <summary>
    /// Represents a control expression that handles multiple selections by passing control to a <see cref="SwitchCase"/>.
    /// </summary>
    [DebuggerTypeProxy(typeof(SwitchExpressionProxy))]
    public sealed class SwitchExpression : Expression
    {
        private readonly SwitchCase[] _cases;
        private readonly TrueReadOnlyCollection<SwitchCase> _casesAsReadOnlyCollection;

        internal SwitchExpression(Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, SwitchCase[] cases)
        {
            Type = type;
            SwitchValue = switchValue;
            DefaultBody = defaultBody;
            Comparison = comparison;
            _cases = cases;
            _casesAsReadOnlyCollection = new TrueReadOnlyCollection<SwitchCase>(_cases);
        }

        /// <summary>
        /// Gets the collection of <see cref="SwitchCase"/> objects for the switch.
        /// </summary>
        public ReadOnlyCollection<SwitchCase> Cases => _casesAsReadOnlyCollection;

        /// <summary>
        /// Gets the equality comparison method, if any.
        /// </summary>
        public MethodInfo Comparison { get; }

        /// <summary>
        /// Gets the test for the switch.
        /// </summary>
        public Expression DefaultBody { get; }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Switch;

        /// <summary>
        /// Gets the test for the switch.
        /// </summary>
        public Expression SwitchValue { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression"/> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type"/> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        internal bool IsLifted
        {
            get
            {
                if (SwitchValue.Type.IsNullable())
                {
                    return Comparison == null ||
                        !TypeUtils.AreEquivalent(SwitchValue.Type, Comparison.GetParameters()[0].ParameterType.GetNonRefTypeInternal());
                }
                return false;
            }
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="switchValue">The <see cref="SwitchValue"/> property of the result.</param>
        /// <param name="cases">The <see cref="Cases"/> property of the result.</param>
        /// <param name="defaultBody">The <see cref="DefaultBody"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public SwitchExpression Update(Expression switchValue, IEnumerable<SwitchCase> cases, Expression defaultBody)
        {
            if (switchValue == SwitchValue & defaultBody == DefaultBody & cases != null)
            {
                if (ExpressionUtils.SameElements(ref cases, _cases))
                {
                    return this;
                }
            }
            return Switch(Type, switchValue, defaultBody, Comparison, cases);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitSwitch(this);
        }
    }
}

#endif