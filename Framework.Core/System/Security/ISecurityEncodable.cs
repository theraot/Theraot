#if LESSTHAN_NETCOREAPP20 || LESSTHAN_NETSTANDARD20
#pragma warning disable CA1041 // Provide ObsoleteAttribute message

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security
{
    [Runtime.InteropServices.ComVisible(true)]
    public interface ISecurityEncodable
    {
        SecurityElement ToXml();

        void FromXml(SecurityElement e);
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

        public abstract void FromXml(SecurityElement securityElement);

        public abstract IPermission Intersect(IPermission target);

        public abstract bool IsSubsetOf(IPermission target);

        public void PermitOnly()
        {
            throw new PlatformNotSupportedException();
        }

        public abstract SecurityElement ToXml();

        public virtual IPermission Union(IPermission target)
        {
            return null;
        }
    }

    internal interface ISecurityElementFactory
    {
        SecurityElement CreateSecurityElement();

        object Copy();

        string GetTag();

        string Attribute(string attributeName);
    }
}

#endif