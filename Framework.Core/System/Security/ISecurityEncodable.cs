#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20

#pragma warning disable CA1041 // Provide ObsoleteAttribute message

#if TARGETS_NETSTANDARD

#pragma warning disable S927 // parameter names should match base declaration and other partial definitions

#endif

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security
{
    [Runtime.InteropServices.ComVisible(true)]
    public interface ISecurityEncodable
    {
        void FromXml(SecurityElement e);

        SecurityElement? ToXml();
    }

    internal interface ISecurityElementFactory
    {
        string? Attribute(string attributeName);

        object Copy();

        SecurityElement CreateSecurityElement();

        string GetTag();
    }

    public abstract class CodeAccessPermission : IPermission, IStackWalk
    {
        public static void RevertAll()
        {
            // Empty
        }

        public static void RevertAssert()
        {
            // Empty
        }

        [Obsolete]
        public static void RevertDeny()
        {
            // Empty
        }

        public static void RevertPermitOnly()
        {
            // Empty
        }

        public void Assert()
        {
            // Empty
        }

        public abstract IPermission Copy();

        public void Demand()
        {
            // Empty
        }

        [Obsolete]
        public void Deny() => throw new PlatformNotSupportedException();

        public abstract void FromXml(SecurityElement elem);

        public abstract IPermission? Intersect(IPermission target);

        public abstract bool IsSubsetOf(IPermission target);

        public void PermitOnly()
        {
            throw new PlatformNotSupportedException();
        }

        public abstract SecurityElement? ToXml();

        public virtual IPermission? Union(IPermission target)
        {
            return null;
        }
    }
}

#endif