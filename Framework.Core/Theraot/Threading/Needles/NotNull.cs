#if FAT
using System;

namespace Theraot.Threading.Needles
{
    public sealed class NotNull<T> : Needle<T>
    {
        public NotNull(T target)
            : base(target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "NotNull cannot have a null value.");
            }
        }

        public override T Value
        {
            get => base.Value;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "NotNull cannot have a null value.");
                }
                SetTargetValue(value);
            }
        }
    }
}

#endif