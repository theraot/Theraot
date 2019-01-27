#if FAT
namespace Theraot.Threading
{
    /// <summary>
    /// Provides a mechanism to get an object that represents a version of a mutable resource
    /// </summary>
    public sealed partial class VersionProvider
    {
        private Target _target;
        private TryAdvance _tryAdvance;

        /// <summary>
        /// Creates a new VersionProvider
        /// </summary>
        public VersionProvider()
        {
            _target = new Target(out _tryAdvance);
        }

        internal delegate bool TryAdvance(out long number);

        /// <summary>
        /// Advances the current up to date version
        /// </summary>
        public void Advance()
        {
            if (!_tryAdvance.Invoke(out _))
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
            if (!_tryAdvance.Invoke(out var number))
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