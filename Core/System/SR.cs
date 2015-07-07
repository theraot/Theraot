namespace System
{
    internal class SR
    {
        internal static string CollectionModifiedWhileEnumerating { get { return "Collection was modified; enumeration operation may not execute"; } }

        internal static string EnumerationIsDone { get { return "Enumeration has either not started or has already finished."; } }

        internal static string ExpressionMustBeReadable { get { return "Expression must be readable"; } }

        internal static string ExpressionTypeDoesNotMatchConstructorParameter { get { return "Expression of type '{0}' cannot be used for constructor parameter of type '{1}'"; } }

        internal static string ExpressionTypeDoesNotMatchMethodParameter { get { return "Expression of type '{0}' cannot be used for parameter of type '{1}' of method '{2}'"; } }

        internal static string ExpressionTypeDoesNotMatchParameter { get { return "Expression of type '{0}' cannot be used for parameter of type '{1}'"; } }

        internal static string IncorrectNumberOfConstructorArguments { get { return "Incorrect number of arguments for constructor"; } }

        internal static string IncorrectNumberOfLambdaArguments { get { return "Incorrect number of arguments supplied for lambda invocation"; } }

        internal static string IncorrectNumberOfMethodCallArguments { get { return "Incorrect number of arguments supplied for call to method '{0}'"; } }

        internal static string InvalidArgumentValue { get { return "Invalid argument value"; } }

        internal static string InvalidNullValue { get { return "The value null is not of type '{0}' and cannot be used in this collection."; } }

        internal static string InvalidObjectType { get { return "The value '{0}' is not of type '{1}' and cannot be used in this collection."; } }

        internal static string NonEmptyCollectionRequired { get { return "Non-empty collection required"; } }

        internal static string TypeContainsGenericParameters { get { return "Type {0} contains generic parameters"; } }

        internal static string TypeIsGeneric { get { return "Type {0} is a generic type definition"; } }

        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                return string.Format(resourceFormat, args);
            }
            return resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            return string.Format(resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            return string.Format(resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            return string.Format(resourceFormat, p1, p2, p3);
        }
    }
}