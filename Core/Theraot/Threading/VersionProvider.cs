#if FAT

namespace Theraot.Threading
{
    /// <summary>
    /// Provides a mechanism to get an object that represents a version of a mutable resource
    /// </summary>
    public sealed partial class VersionProvider
    {
        private Target _target;
        private Advancer _tryAdvance;

        /// <summary>
        /// Creates a new VersionProvider
        /// </summary>
        public VersionProvider()
        {
            _target = new Target(out _tryAdvance);
        }

        internal delegate bool Advancer(out long number);

        /// <summary>
        /// Advances the current up to date version
        /// </summary>
        public void Advance()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
        }

        /// <summary>
        /// Advances the current up to date version and returns a VersionToken for the new version
        /// </summary>
        /// <returns>A VersionToken representing the advanced version</returns>
        public VersionToken AdvanceNewToken()
        {
            long number;
            if (!_tryAdvance.Invoke(out number))
            {
                _target = new Target(out _tryAdvance);
            }
            return new VersionToken(this, _target, number);
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