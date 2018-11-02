// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Dynamic.Utils
{
    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static class Error
    {
        /// <summary>
        /// InvalidOperationException with message like "Enumeration has either not started or has already finished."
        /// </summary>
        internal static Exception EnumerationIsDone()
        {
            return new InvalidOperationException(Strings.EnumerationIsDone);
        }

        /// <summary>
        /// InvalidOperationException with message like "Collection was modified; enumeration operation may not execute."
        /// </summary>
        internal static Exception CollectionModifiedWhileEnumerating()
        {
            return new InvalidOperationException(Strings.CollectionModifiedWhileEnumerating);
        }

        internal static Exception TypeContainsGenericParameters(object p0)
        {
            return new ArgumentException(Strings.TypeContainsGenericParameters(p0));
        }

        internal static Exception TypeIsGeneric(object p0)
        {
            return new ArgumentException(Strings.TypeIsGeneric(p0));
        }

        /// <summary>
        /// ArgumentException with message like "Incorrect number of arguments for constructor"
        /// </summary>
        internal static Exception IncorrectNumberOfConstructorArguments()
        {
            return new ArgumentException(Strings.IncorrectNumberOfConstructorArguments);
        }

        internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2));
        }

        internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchParameter(p0, p1));
        }

        /// <summary>
        /// InvalidOperationException with message like "Incorrect number of arguments supplied for lambda invocation"
        /// </summary>
        internal static Exception IncorrectNumberOfLambdaArguments()
        {
            return new InvalidOperationException(Strings.IncorrectNumberOfLambdaArguments);
        }

        internal static Exception IncorrectNumberOfMethodCallArguments(object p0)
        {
            return new ArgumentException(Strings.IncorrectNumberOfMethodCallArguments(p0));
        }

        internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
        {
            return new ArgumentException(Strings.ExpressionTypeDoesNotMatchConstructorParameter(p0, p1));
        }
    }
}