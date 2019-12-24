#if LESSTHAN_NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
**
**
** Purpose: The contract class allows for expressing preconditions,
** postconditions, and object invariants about methods in source
** code for runtime checking & static analysis.
**
** Two classes (Contract and ContractHelper) are split into partial classes
** in order to share the public front for multiple platforms (this file)
** while providing separate implementation details for each platform.
**
===========================================================*/
#define DEBUG // The behavior of this contract library should be consistent regardless of build type.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics.Contracts
{
    /// <summary>
    ///     Contains static methods for representing program contracts such as preconditions, postconditions, and invariants.
    /// </summary>
    /// <remarks>
    ///     WARNING: A binary rewriter must be used to insert runtime enforcement of these contracts.
    ///     Otherwise some contracts like Ensures can only be checked statically and will not throw exceptions during runtime
    ///     when contracts are violated.
    ///     Please note this class uses conditional compilation to help avoid easy mistakes.  Defining the preprocessor
    ///     symbol CONTRACTS_PRECONDITIONS will include all preconditions expressed using Contract.Requires in your
    ///     build.  The symbol CONTRACTS_FULL will include postconditions and object invariants, and requires the binary
    ///     rewriter.
    /// </remarks>
    public static partial class Contract
    {
        /// <summary>
        ///     In debug builds, perform a runtime check that <paramref name="condition" /> is true.
        /// </summary>
        /// <param name="condition">Expression to check to always be true.</param>
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                ReportFailure(ContractFailureKind.Assert, null, null, null);
            }
        }

        /// <summary>
        ///     In debug builds, perform a runtime check that <paramref name="condition" /> is true.
        /// </summary>
        /// <param name="condition">Expression to check to always be true.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assert(bool condition, string userMessage)
        {
            if (!condition)
            {
                ReportFailure(ContractFailureKind.Assert, userMessage, null, null);
            }
        }

        /// <summary>
        ///     Instructs code analysis tools to assume the expression <paramref name="condition" /> is true even if it can not be
        ///     statically proven to always be true.
        /// </summary>
        /// <param name="condition">Expression to assume will always be true.</param>
        /// <remarks>
        ///     At runtime this is equivalent to an <seealso cref="Assert(bool)" />.
        /// </remarks>
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assume(bool condition)
        {
            if (!condition)
            {
                ReportFailure(ContractFailureKind.Assume, null, null, null);
            }
        }

        /// <summary>
        ///     Instructs code analysis tools to assume the expression <paramref name="condition" /> is true even if it can not be
        ///     statically proven to always be true.
        /// </summary>
        /// <param name="condition">Expression to assume will always be true.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     At runtime this is equivalent to an <seealso cref="Assert(bool)" />.
        /// </remarks>
        [Conditional("DEBUG")]
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Assume(bool condition, string userMessage)
        {
            if (!condition)
            {
                ReportFailure(ContractFailureKind.Assume, userMessage, null, null);
            }
        }

        /// <summary>
        ///     Marker to indicate the end of the contract section of a method.
        /// </summary>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static void EndContractBlock()
        {
            // Empty
        }

        /// <summary>
        ///     Specifies a public contract such that the expression <paramref name="condition" /> will be true when the enclosing
        ///     method or property returns normally.
        /// </summary>
        /// <param name="condition">
        ///     Boolean expression representing the contract.  May include <seealso cref="OldValue{T}" /> and
        ///     <seealso cref="Result{T}" />.
        /// </param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     The contract rewriter must be used for runtime enforcement of this postcondition.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Ensures(bool condition)
        {
            _ = condition;
            AssertMustUseRewriter(ContractFailureKind.Postcondition, nameof(Ensures));
        }

        /// <summary>
        ///     Specifies a public contract such that the expression <paramref name="condition" /> will be true when the enclosing
        ///     method or property returns normally.
        /// </summary>
        /// <param name="condition">
        ///     Boolean expression representing the contract.  May include <seealso cref="OldValue{T}" /> and
        ///     <seealso cref="Result{T}" />.
        /// </param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     The contract rewriter must be used for runtime enforcement of this postcondition.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Ensures(bool condition, string userMessage)
        {
            _ = condition;
            _ = userMessage;
            AssertMustUseRewriter(ContractFailureKind.Postcondition, nameof(Ensures));
        }

        /// <summary>
        ///     Specifies a contract such that if an exception of type <typeparamref name="TException" /> is thrown then the
        ///     expression <paramref name="condition" /> will be true when the enclosing method or property terminates abnormally.
        /// </summary>
        /// <typeparam name="TException">Type of exception related to this postcondition.</typeparam>
        /// <param name="condition">
        ///     Boolean expression representing the contract.  May include <seealso cref="OldValue{T}" /> and
        ///     <seealso cref="Result{T}" />.
        /// </param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference types and members at least as visible as the enclosing
        ///     method.
        ///     The contract rewriter must be used for runtime enforcement of this postcondition.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void EnsuresOnThrow<TException>(bool condition)
            where TException : Exception
        {
            _ = typeof(TException);
            _ = condition;
            AssertMustUseRewriter(ContractFailureKind.PostconditionOnException, nameof(EnsuresOnThrow));
        }

        /// <summary>
        ///     Specifies a contract such that if an exception of type <typeparamref name="TException" /> is thrown then the
        ///     expression <paramref name="condition" /> will be true when the enclosing method or property terminates abnormally.
        /// </summary>
        /// <typeparam name="TException">Type of exception related to this postcondition.</typeparam>
        /// <param name="condition">
        ///     Boolean expression representing the contract.  May include <seealso cref="OldValue{T}" /> and
        ///     <seealso cref="Result{T}" />.
        /// </param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference types and members at least as visible as the enclosing
        ///     method.
        ///     The contract rewriter must be used for runtime enforcement of this postcondition.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void EnsuresOnThrow<TException>(bool condition, string userMessage)
            where TException : Exception
        {
            _ = typeof(TException);
            _ = condition;
            _ = userMessage;
            AssertMustUseRewriter(ContractFailureKind.PostconditionOnException, nameof(EnsuresOnThrow));
        }

        /// <summary>
        ///     Returns whether the <paramref name="predicate" /> returns <c>true</c>
        ///     for any integer starting from <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate" />.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate" />.</param>
        /// <param name="predicate">
        ///     Function that is evaluated from <paramref name="fromInclusive" /> to
        ///     <paramref name="toExclusive" /> - 1.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="predicate" /> returns <c>true</c> for any integer
        ///     starting from <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </returns>
        /// <seealso cref="List{T}.Exists" />
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] // Assumes predicate obeys CER rules.
        public static bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
            if (fromInclusive > toExclusive)
            {
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive.");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            EndContractBlock();

            for (var i = fromInclusive; i < toExclusive; i++)
            {
                if (predicate(i))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Returns whether the <paramref name="predicate" /> returns <c>true</c>
        ///     for any element in the <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="T">The type that is contained in collection.</typeparam>
        /// <param name="collection">
        ///     The collection from which elements will be drawn from to pass to <paramref name="predicate" />
        ///     .
        /// </param>
        /// <param name="predicate">Function that is evaluated on elements from <paramref name="collection" />.</param>
        /// <returns>
        ///     <c>true</c> if and only if <paramref name="predicate" /> returns <c>true</c> for an element in
        ///     <paramref name="collection" />.
        /// </returns>
        /// <seealso cref="List&lt;T&gt;.Exists" />
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] // Assumes predicate & collection enumerator obey CER rules.
        public static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            EndContractBlock();

            return collection.Any(t => predicate(t));
        }

        /// <summary>
        ///     Returns whether the <paramref name="predicate" /> returns <c>true</c>
        ///     for all integers starting from <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate" />.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate" />.</param>
        /// <param name="predicate">
        ///     Function that is evaluated from <paramref name="fromInclusive" /> to
        ///     <paramref name="toExclusive" /> - 1.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="predicate" /> returns <c>true</c> for all integers
        ///     starting from <paramref name="fromInclusive" /> to <paramref name="toExclusive" /> - 1.
        /// </returns>
        /// <seealso cref="List&lt;T&gt;.TrueForAll" />
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] // Assumes predicate obeys CER rules.
        public static bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
            if (fromInclusive > toExclusive)
            {
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive.");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            EndContractBlock();

            for (var i = fromInclusive; i < toExclusive; i++)
            {
                if (!predicate(i))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns whether the <paramref name="predicate" /> returns <c>true</c>
        ///     for all elements in the <paramref name="collection" />.
        /// </summary>
        /// <typeparam name="T">The type that is contained in collection.</typeparam>
        /// <param name="collection">
        ///     The collection from which elements will be drawn from to pass to <paramref name="predicate" />
        ///     .
        /// </param>
        /// <param name="predicate">Function that is evaluated on elements from <paramref name="collection" />.</param>
        /// <returns>
        ///     <c>true</c> if and only if <paramref name="predicate" /> returns <c>true</c> for all elements in
        ///     <paramref name="collection" />.
        /// </returns>
        /// <seealso cref="List&lt;T&gt;.TrueForAll" />
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] // Assumes predicate & collection enumerator obey CER rules.
        public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            EndContractBlock();

            return collection.All(t => predicate(t));
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> will be true after every method or
        ///     property on the enclosing class.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <remarks>
        ///     This contact can only be specified in a dedicated invariant method declared on a class.
        ///     This contract is not exposed to clients so may reference members less visible as the enclosing method.
        ///     The contract rewriter must be used for runtime enforcement of this invariant.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Invariant(bool condition)
        {
            _ = condition;
            AssertMustUseRewriter(ContractFailureKind.Invariant, nameof(Invariant));
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> will be true after every method or
        ///     property on the enclosing class.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     This contact can only be specified in a dedicated invariant method declared on a class.
        ///     This contract is not exposed to clients so may reference members less visible as the enclosing method.
        ///     The contract rewriter must be used for runtime enforcement of this invariant.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Invariant(bool condition, string userMessage)
        {
            _ = condition;
            _ = userMessage;
            AssertMustUseRewriter(ContractFailureKind.Invariant, nameof(Invariant));
        }

        /// <summary>
        ///     Represents the value of <paramref name="value" /> as it was at the start of the method or property.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="value" />.  This can be inferred.</typeparam>
        /// <param name="value">Value to represent.  This must be a field or parameter.</param>
        /// <returns>Value of <paramref name="value" /> at the start of the method or property.</returns>
        /// <remarks>
        ///     This method can only be used within the argument to the <seealso cref="Ensures(bool)" /> contract.
        /// </remarks>
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MaybeNull]
        public static T OldValue<T>(T value)
        {
            _ = value;
            return default!;
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> must be true before the enclosing
        ///     method or property is invoked.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     Use this form when backward compatibility does not force you to throw a particular exception.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Requires(bool condition)
        {
            _ = condition;
            AssertMustUseRewriter(ContractFailureKind.Precondition, nameof(Requires));
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> must be true before the enclosing
        ///     method or property is invoked.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     Use this form when backward compatibility does not force you to throw a particular exception.
        /// </remarks>
        [Conditional("CONTRACTS_FULL")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Requires(bool condition, string userMessage)
        {
            _ = condition;
            _ = userMessage;
            AssertMustUseRewriter(ContractFailureKind.Precondition, nameof(Requires));
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> must be true before the enclosing
        ///     method or property is invoked.
        /// </summary>
        /// <typeparam name="TException">The exception to throw if the condition is false.</typeparam>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     Use this form when you want to throw a particular exception.
        /// </remarks>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Requires<TException>(bool condition)
            where TException : Exception
        {
            _ = typeof(TException);
            _ = condition;
            AssertMustUseRewriter(ContractFailureKind.Precondition, "Requires<TException>");
        }

        /// <summary>
        ///     Specifies a contract such that the expression <paramref name="condition" /> must be true before the enclosing
        ///     method or property is invoked.
        /// </summary>
        /// <typeparam name="TException">The exception to throw if the condition is false.</typeparam>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        ///     This call must happen at the beginning of a method or property before any other code.
        ///     This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        ///     Use this form when you want to throw a particular exception.
        /// </remarks>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void Requires<TException>(bool condition, string userMessage)
            where TException : Exception
        {
            _ = typeof(TException);
            _ = condition;
            _ = userMessage;
            AssertMustUseRewriter(ContractFailureKind.Precondition, "Requires<TException>");
        }

        /// <summary>
        ///     Represents the result (a.k.a. return value) of a method or property.
        /// </summary>
        /// <typeparam name="T">Type of return value of the enclosing method or property.</typeparam>
        /// <returns>Return value of the enclosing method or property.</returns>
        /// <remarks>
        ///     This method can only be used within the argument to the <seealso cref="Ensures(bool)" /> contract.
        /// </remarks>
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MaybeNull]
        public static T Result<T>()
        {
            return default!;
        }

        /// <summary>
        ///     Represents the final (output) value of an out parameter when returning from a method.
        /// </summary>
        /// <typeparam name="T">Type of the out parameter.</typeparam>
        /// <param name="value">The out parameter.</param>
        /// <returns>The output value of the out parameter.</returns>
        /// <remarks>
        ///     This method can only be used within the argument to the <seealso cref="Ensures(bool)" /> contract.
        /// </remarks>
        [Pure]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MaybeNull]
        public static T ValueAtReturn<T>(out T value)
        {
            value = default!;
            return value;
        }
    }
}

#endif