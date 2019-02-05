#if FAT
namespace Theraot.Threading
{
    /// <summary>
    /// Provides a mechanism to get an object that represents a version of a mutable resource
    /// </summary>
    public sealed class VersionProvider
    {
        internal VersionTarget Target { get; private set; }

        private TryAdvance _tryAdvance;

        /// <summary>
        /// Creates a new VersionProvider
        /// </summary>
        public VersionProvider()
        {
            Target = new VersionTarget(out _tryAdvance);
        }

        /// <summary>
        /// Advances the current up to date version
        /// </summary>
        public void Advance()
        {
            if (!_tryAdvance.Invoke(out _))
            {
                Target = new VersionTarget(out _tryAdvance);
            }
        }

        /// <summary>
        /// Advances the current up to date version and returns a VersionToken for the new version
        /// </summary>
        /// <returns>A VersionToken representing the advanced version</returns>
        public VersionToken AdvanceNewToken()
        {
            if (!_tryAdvance.Invoke(out var number))
            {
                Target = new VersionTarget(out _tryAdvance);
            }
            return new VersionToken(this, Target, number);
        }

        /// <summary>
        /// Creates a new VersionToken, it should be considered to be out of date
        /// </summary>
        /// <returns>A new VersionToken</returns>
        public VersionToken NewToken()
        {
            return new VersionToken(this);
        }
    }
}

#endif