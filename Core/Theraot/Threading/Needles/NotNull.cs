using System;

namespace Theraot.Threading.Needles
{
    public sealed class NotNull<T> : Needle<T>
    {
        public NotNull(T target)
            : base(target)
        {
            if (ReferenceEquals(target, null))
            {
                throw new ArgumentNullException("NotNull cannot have a null value.");
            }
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (ReferenceEquals(value, null))
                {
                    throw new ArgumentNullException("NotNull cannot have a null value.");
                }
                else
                {
                    SetTarget(value);
                }
            }
        }
    }
}