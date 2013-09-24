#if FAT

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Theraot.Collections.Specialized
{
    public sealed class EnumCollection<TEnum> : ProgressiveCollection<TEnum>
    {
        private static EnumCollection<TEnum> instance = new EnumCollection<TEnum>();

        private EnumCollection()
            : base(GetEnumerable())
        {
            //Empty
        }

        public static EnumCollection<TEnum> Instance
        {
            get
            {
                return instance;
            }
        }

        public static IEnumerable<TEnum> GetEnumerable()
        {
            var type = CheckType();
            foreach (var item in Enum.GetValues(type))
            {
                yield return (TEnum)item;
            }
        }

        public static string ToString(TEnum value)
        {
            var type = CheckType();
            return Enum.GetName(type, value);
        }

        private static Type CheckType()
        {
            var type = typeof(TEnum);
            if (type.IsEnum)
            {
                return type;
            }
            else
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} is not an Enum", type.Name));
            }
        }
    }
}

#endif