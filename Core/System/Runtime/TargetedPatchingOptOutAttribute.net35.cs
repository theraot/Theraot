#if NET20 || NET30 || NET35

namespace System.Runtime
{
    /// <summary>Indicates that the .NET Framework class library method to which this attribute is applied is unlikely to be affected by servicing releases, and therefore is eligible to be inlined across Native Image Generator (NGen) images.</summary>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
    public sealed class TargetedPatchingOptOutAttribute : Attribute
    {
        private readonly string _reason;

        /// <summary>Gets the reason why the method to which this attribute is applied is considered to be eligible for inlining across Native Image Generator (NGen) images.</summary>
        /// <returns>The reason why the method is considered to be eligible for inlining across NGen images.</returns>
        public string Reason
        {
            get
            {
                return _reason;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.TargetedPatchingOptOutAttribute" /> class.</summary>
        /// <param name="reason">The reason why the method to which the <see cref="T:System.Runtime.TargetedPatchingOptOutAttribute" /> attribute is applied is considered to be eligible for inlining across Native Image Generator (NGen) images.</param>
        public TargetedPatchingOptOutAttribute(string reason)
        {
            _reason = reason;
        }
    }
}

#endif