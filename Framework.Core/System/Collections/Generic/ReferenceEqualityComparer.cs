namespace System.Collections.Generic
{
    public sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        private ReferenceEqualityComparer()
        {
            // Empty
        }

        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object? obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}