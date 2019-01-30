namespace System.Collections.Generic
{
    internal sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public static readonly HashSetEqualityComparer<T> Instance = new HashSetEqualityComparer<T>();

        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            foreach (var item in x)
            {
                if (!y.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(HashSet<T> obj)
        {
            try
            {
                var comparer = EqualityComparer<T>.Default;
                var hash = 0;
                foreach (var item in obj)
                {
                    hash ^= comparer.GetHashCode(item);
                }

                return hash;
            }
            catch (NullReferenceException)
            {
                return 0;
            }
        }
    }
}