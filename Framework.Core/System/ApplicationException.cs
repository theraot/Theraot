#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: The base class for all "less serious" exceptions that must be
**          declared or caught.
**
**
=============================================================================*/

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Theraot;

namespace System
{
    [ComVisible(true)]
    [Serializable]
    public class ApplicationException : Exception
    {
        // Creates a new ApplicationException with its message string set to
        // the empty string, its HRESULT set to COR_E_APPLICATION,
        // // and its ExceptionInfo reference set to null.
        public ApplicationException()
            : base("Error in the application.")
        {
            HResult = unchecked((int)0x80131600);
        }

        // Creates a new ApplicationException with its message string set to
        // message, its HRESULT set to COR_E_APPLICATION,
        // and its ExceptionInfo reference set to null.
        public ApplicationException(string message)
            : base(message)
        {
            HResult = unchecked((int)0x80131600);
        }

        public ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = unchecked((int)0x80131600);
        }

#if GREATERTHAN_NETCOREAPP20 || NETSTANDARD2_0 || TARGETS_NET
        protected ApplicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
#else

        [Obsolete("This target platform does not support binary serialization.")]
        protected ApplicationException(SerializationInfo info, StreamingContext context)
#endif
        {
            No.Op(info);
            No.Op(context);
        }
    }
}

#endif