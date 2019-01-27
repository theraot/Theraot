#if FAT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Theraot.Collections;

namespace Theraot.Core
{
    public sealed class RangeSet<T> : ICollection<Range<T>>
        where T : IComparable<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly List<Range<T>> _ranges;

        public RangeSet()
        {
            _comparer = Comparer<T>.Default;
            _ranges = new List<Range<T>>();
        }

        public RangeSet(IEnumerable<Range<T>> ranges)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException(nameof(ranges));
            }
            _comparer = Comparer<T>.Default;
            _ranges = new List<Range<T>>();
            foreach (var range in ranges)
            {
                Add(range);
            }
        }

        public RangeSet(IComparer<T> comparer)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _ranges = new List<Range<T>>();
        }

        public RangeSet(IComparer<T> comparer, IEnumerable<Range<T>> ranges)
        {
            if (ranges == null)
            {
                throw new ArgumentNullException(nameof(ranges));
            }
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            foreach (var range in ranges)
            {
                Add(range);
            }
        }

        int ICollection<Range<T>>.Count => _ranges.Count;

        bool ICollection<Range<T>>.IsReadOnly => false;

        public void Add(Range<T> item)
        {
            Add(_ranges, item, _comparer);
        }

        public void Clear()
        {
            _ranges.Clear();
        }

        public bool Contains(T value)
        {
            foreach (var stored in _ranges)
            {
                var situation = stored.CompareTo(value, _comparer);
                if (situation < 0)
                {
                    continue;
                }
                return situation == 0;
            }
            return false;
        }

        public bool Contains(Range<T> item)
        {
            foreach (var stored in _ranges)
            {
                switch (stored.CompareTo(item, _comparer))
                {
                    case RangeSituation.BeforeSeparated:
                    case RangeSituation.BeforeTouching:
                    case RangeSituation.BeforeOverlapped:
                        continue;
                    case RangeSituation.Contained:
                        return false;

                    case RangeSituation.Equals:
                    case RangeSituation.Contains:
                        return true;

                    case RangeSituation.AfterOverlapped:
                    case RangeSituation.AfterTouching:
                    case RangeSituation.AfterSeparated:
                        return false;

                    default:
                        // Shouldn't happen
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

        public bool Contains(T value, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            foreach (var stored in _ranges)
            {
                var situation = stored.CompareTo(value, comparer);
                if (situation < 0)
                {
                    continue;
                }
                return situation == 0;
            }
            return false;
        }

        public bool Contains(Range<T> range, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            foreach (var stored in _ranges)
            {
                switch (stored.CompareTo(range, comparer))
                {
                    case RangeSituation.BeforeSeparated:
                    case RangeSituation.BeforeTouching:
                    case RangeSituation.BeforeOverlapped:
                        continue;
                    case RangeSituation.Contained:
                        return false;

                    case RangeSituation.Equals:
                    case RangeSituation.Contains:
                        return true;

                    case RangeSituation.AfterOverlapped:
                    case RangeSituation.AfterTouching:
                    case RangeSituation.AfterSeparated:
                        return false;

                    default:
                        // Shouldn't happen
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

        public void CopyTo(Range<T>[] array, int arrayIndex)
        {
            _ranges.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Range<T>> GetEnumerator()
        {
            return _ranges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsContainedBy(Range<T> range)
        {
            return IsContainedBy(range, _comparer);
        }

        public bool IsContainedBy(Range<T> range, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            if (_ranges.Count == 0)
            {
                return true;
            }
            return range.CompareTo(_ranges[0], comparer) == RangeSituation.Contains && range.CompareTo(_ranges[_ranges.Count - 1], comparer) == RangeSituation.Contains;
        }

        public bool Overlaps(Range<T> range)
        {
            return !Overlapped(_ranges, range, _comparer).IsEmpty();
        }

        public bool Overlaps(Range<T> range, IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }
            return !Overlapped(_ranges, range, comparer).IsEmpty();
        }

        public bool Remove(Range<T> item)
        {
            return Remove(_ranges, item, _comparer);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var first = true;
            sb.Append('{');
            foreach (var item in _ranges)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append(item.ToString());
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static void Add(List<Range<T>> ranges, Range<T> range, IComparer<T> comparer)
        {
            var remove = new List<int>();
            Range<T>? add = null;
            int? insert = null;
            var justAdd = true;
            for (var index = 0; index < ranges.Count; index++)
            {
                var currentRange = ranges[index];
                switch (currentRange.CompareTo(range, comparer))
                {
                    case RangeSituation.BeforeSeparated:
                        continue;

                    case RangeSituation.BeforeTouching:
                    case RangeSituation.BeforeOverlapped:
                        remove.Add(index);
                        if (!insert.HasValue)
                        {
                            insert = index;
                        }
                        add = new Range<T>
                        (
                            currentRange.Minimum,
                            currentRange.ClosedMinimum,
                            range.Maximum,
                            range.ClosedMaximum
                        );
                        justAdd = false;
                        continue;

                    case RangeSituation.Contained:
                        remove.Add(index);
                        if (!insert.HasValue)
                        {
                            insert = index;
                        }
                        add = add.HasValue ? new Range<T>
                            (
                                add.Value.Minimum,
                                add.Value.ClosedMinimum,
                                range.Maximum,
                                range.ClosedMaximum
                            ) : range;
                        justAdd = false;
                        continue;

                    case RangeSituation.Equals:
                    case RangeSituation.Contains:
                        justAdd = false;
                        break;

                    case RangeSituation.AfterOverlapped:
                    case RangeSituation.AfterTouching:
                        remove.Add(index);
                        if (!insert.HasValue)
                        {
                            insert = index;
                        }
                        add = add.HasValue ? new Range<T>
                            (
                                add.Value.Minimum,
                                add.Value.ClosedMinimum,
                                range.Maximum,
                                range.ClosedMaximum
                            ) : new Range<T>
                            (
                                currentRange.Minimum,
                                currentRange.ClosedMinimum,
                                range.Maximum,
                                range.ClosedMaximum
                            );
                        justAdd = false;
                        continue;

                    case RangeSituation.AfterSeparated:
                        if (!insert.HasValue)
                        {
                            insert = index;
                        }
                        if (!add.HasValue)
                        {
                            add = range;
                        }
                        justAdd = false;
                        break;

                    default:
                        // Shouldn't happen
                        throw new ArgumentOutOfRangeException();
                }
                break;
            }
            if (justAdd)
            {
                add = range;
            }
            var removeStart = -1;
            var removeCount = 0;
            for (var index = remove.Count - 1; index >= 0; index--)
            {
                if (remove[index] == removeStart - removeCount)
                {
                    removeStart--;
                    removeCount++;
                }
                else
                {
                    if (removeStart >= 0)
                    {
                        ranges.RemoveRange(removeStart, removeCount);
                    }
                    removeStart = remove[index];
                    removeCount = 1;
                }
            }
            if (removeStart >= 0)
            {
                ranges.RemoveRange(removeStart, removeCount);
            }
            if (insert.HasValue)
            {
                ranges.Insert(insert.Value, add.Value);
            }
            else if (add.HasValue)
            {
                ranges.Add(add.Value);
            }
        }

        private static IEnumerable<Range<T>> Overlapped(List<Range<T>> ranges, Range<T> range, IComparer<T> comparer)
        {
            foreach (var item in ranges)
            {
                if ((item.CompareTo(range, comparer) & (RangeSituation.AfterOverlapped | RangeSituation.BeforeOverlapped)) != RangeSituation.Invalid)
                {
                    yield return item;
                }
            }
        }

        private static bool Remove(List<Range<T>> ranges, Range<T> range, IComparer<T> comparer)
        {
            var replacements = new List<Tuple<int, Range<T>?, Range<T>?>>();
            for (var index = 0; index < ranges.Count; index++)
            {
                var currentRange = ranges[index];
                switch (currentRange.CompareTo(range, comparer))
                {
                    case RangeSituation.BeforeSeparated:
                    case RangeSituation.BeforeTouching:
                        continue;
                    case RangeSituation.BeforeOverlapped:
                        replacements.Add
                        (
                            new Tuple<int, Range<T>?, Range<T>?>(
                                index,
                                new Range<T>
                                (
                                    currentRange.Minimum,
                                    currentRange.ClosedMinimum,
                                    range.Minimum,
                                    !range.ClosedMinimum
                                ),
                                null
                            )
                        );
                        continue;
                    case RangeSituation.Contained:
                        replacements.Add
                        (
                            new Tuple<int, Range<T>?, Range<T>?>(
                                index,
                                null,
                                null
                            )
                        );
                        continue;
                    case RangeSituation.Equals:
                        replacements.Add
                        (
                            new Tuple<int, Range<T>?, Range<T>?>(
                                index,
                                null,
                                null
                            )
                        );
                        break;

                    case RangeSituation.Contains:
                        replacements.Add
                        (
                            new Tuple<int, Range<T>?, Range<T>?>(
                                index,
                                new Range<T>
                                (
                                    currentRange.Minimum,
                                    currentRange.ClosedMinimum,
                                    range.Minimum,
                                    !range.ClosedMinimum
                                ),
                                new Range<T>
                                (
                                    range.Maximum,
                                    !range.ClosedMaximum,
                                    currentRange.Maximum,
                                    currentRange.ClosedMaximum
                                )
                            )
                        );
                        break;

                    case RangeSituation.AfterOverlapped:
                        replacements.Add
                        (
                            new Tuple<int, Range<T>?, Range<T>?>(
                                index,
                                null,
                                new Range<T>
                                (
                                    range.Maximum,
                                    !range.ClosedMaximum,
                                    currentRange.Maximum,
                                    currentRange.ClosedMaximum
                                )
                            )
                        );
                        break;

                    case RangeSituation.AfterTouching:
                    case RangeSituation.AfterSeparated:
                        break;

                    default:
                        // Shouldn't happen
                        throw new ArgumentOutOfRangeException();
                }
                break;
            }
            foreach (var tuple in replacements)
            {
                var index = tuple.Item1;
                var before = tuple.Item2;
                var after = tuple.Item3;
                if (after.HasValue && !after.Value.IsEmpty(comparer))
                {
                    ranges[index] = after.Value;
                    if (before.HasValue && !before.Value.IsEmpty(comparer))
                    {
                        ranges.Insert(index, before.Value);
                    }
                }
                else
                {
                    if (before.HasValue && !before.Value.IsEmpty(comparer))
                    {
                        ranges[index] = before.Value;
                    }
                    else
                    {
                        ranges.RemoveAt(index);
                    }
                }
            }
            return replacements.Count > 0;
        }
    }
}

#endif