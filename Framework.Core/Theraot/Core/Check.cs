#if FAT
using System;
using System.Diagnostics;

namespace Theraot.Core
{
    [DebuggerNonUserCode]
    public static class Check
    {
        public static TArgument CheckArgument<TArgument>(TArgument argument, Predicate<TArgument> check, string parameterName)
        {
            if (check == null || !check(argument))
            {
                throw new ArgumentException(parameterName, parameterName);
            }
            return argument;
        }

        public static TArgument InRangeArgument<TArgument>(TArgument argument, Predicate<TArgument> isInRange, string parameterName)
        {
            if (isInRange == null || !isInRange(argument))
            {
                throw new ArgumentOutOfRangeException(parameterName, argument, parameterName + " is out of range.");
            }
            return argument;
        }

        public static T NotNullArgument<T>(T argument, string parameterName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, parameterName + " is null.");
            }
            return argument;
        }

        public static string NotNullNotEmpty(string text, string parameterName)
        {
            if (text == null)
            {
                throw new ArgumentNullException(parameterName, parameterName + " is null.");
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentOutOfRangeException(parameterName, parameterName + " is empty.");
            }
            return text;
        }
    }
}

#endif