#if FAT

using System;
using System.Collections.Generic;
using System.IO;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Core
{
    public sealed class RamStream : Stream
    {
        private Bucket<KeyValuePair<long, byte[]>> _bytes;
        private readonly int _sectorBits;
        private long _length;
        private long _position;

        public RamStream()
        {
            _sectorBits = 6;
            _length = 0;
            _bytes = new Bucket<KeyValuePair<long, byte[]>>();
        }

        public RamStream(int clusterSize)
        {
            if (clusterSize < 0)
            {
                throw new ArgumentOutOfRangeException("clusterSize");
            }
            _sectorBits = NumericHelper.Log2(NumericHelper.PopulationCount(clusterSize) == 1 ? clusterSize : NumericHelper.NextPowerOf2(clusterSize));
            _length = 0;
            _bytes = new Bucket<KeyValuePair<long, byte[]>>();
        }

        public override bool CanRead
        {
            get { return _position != -1; }
        }

        public override bool CanSeek
        {
            get { return _position != -1; }
        }

        public override bool CanWrite
        {
            get { return _position != -1; }
        }

        public override long Length
        {
            get
            {
                if (_position == -1)
                {
                    throw new ObjectDisposedException(typeof(RamStream).Name);
                }
                return _length;
            }
        }

        public override long Position
        {
            get { return _position; }

            set
            {
                if (_position == -1)
                {
                    throw new ObjectDisposedException(typeof(RamStream).Name);
                }
                if (value < 0)
                {
                    throw new ArgumentException("Negative position");
                }
                if (value > (long)int.MaxValue << _sectorBits)
                {
                    throw new ArgumentOutOfRangeException("value", "Overflow");
                }
                _position = (int)value;
            }
        }

        public override void Flush()
        {
            // NoOp
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytes = _bytes;
            if (_position == -1)
            {
                throw new ObjectDisposedException(typeof(RamStream).Name);
            }
            Extensions.CanCopyTo(buffer, offset, count);
            if (_position <= _length && _position + count > _length)
            {
                count = (int)(_length - _position);
            }
            var progress = 0;
            foreach (var node in bytes.EnumerateRange((int)(_position >> _sectorBits), int.MaxValue))
            {
                var pair = node;
                long contribution = 0;
                var intContribution = 0;
                if (pair.Key > _position)
                {
                    // The pair is beyond the current position
                    var diff = pair.Key - _position;
                    contribution = diff > count ? count : diff;
                    if (contribution + _position > _length)
                    {
                        contribution = _length - _position;
                    }
                    if (contribution > int.MaxValue)
                    {
                        contribution = int.MaxValue;
                    }
                    intContribution = (int)contribution;
                    Array.Clear(buffer, offset, intContribution);
                    progress += intContribution;
                    offset += intContribution;
                    count -= intContribution;
                    _position += intContribution;
                    if (count == 0)
                    {
                        break;
                    }
                }
                // The pair is prior to the current position
                var index = _position - pair.Key;
                if (pair.Value.Length > index)
                {
                    // The position is in the middle of the pair
                    var source = pair.Value;
                    var diff = pair.Value.Length - index;
                    contribution = diff > count ? count : diff;
                    if (contribution + _position > _length)
                    {
                        contribution = _length - _position;
                    }
                    if (contribution > int.MaxValue)
                    {
                        contribution = int.MaxValue;
                    }
                    intContribution = (int)contribution;
                    Array.Copy(source, (int)index, buffer, offset, intContribution);
                }
                progress += intContribution;
                offset += intContribution;
                count -= intContribution;
                _position += contribution;
                if (count == 0)
                {
                    break;
                }
            }
            return progress;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_position == -1)
            {
                throw new ObjectDisposedException(typeof(RamStream).Name);
            }
            var position = _position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;

                case SeekOrigin.Current:
                    position += offset;
                    break;

                case SeekOrigin.End:
                    position += _length + offset;
                    break;

                default:
                    throw new ArgumentException("SeekOrigin not valid");
            }
            if (position < 0)
            {
                throw new IOException("Negative position");
            }
            if (position > (long)int.MaxValue << _sectorBits)
            {
                throw new ArgumentOutOfRangeException("offset", "Overflow");
            }
            _position = position;
            return _position;
        }

        public override void SetLength(long value)
        {
            var bytes = _bytes;
            if (_position == -1)
            {
                throw new ObjectDisposedException(typeof(RamStream).Name);
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", "Negative length");
            }
            if (value > (long)int.MaxValue << _sectorBits)
            {
                throw new ArgumentOutOfRangeException("value", "Overflow");
            }
            _length = (int)value;
            foreach (var node in bytes.EnumerateRange(int.MaxValue, (int)value >> _sectorBits))
            {
                bytes.RemoveAt((int)(node.Key >> _sectorBits));
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytes = _bytes;
            if (_position == -1)
            {
                throw new ObjectDisposedException(typeof(RamStream).Name);
            }
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("The sum of offset and count is greater than the buffer length.");
            }
            again:
            foreach (var node in bytes.EnumerateRange((int)(_position >> _sectorBits), int.MaxValue))
            {
                var pair = node;
                long contribution;
                int intContribution;
                while (pair.Key > _position)
                {
                    var left = new byte[1 << _sectorBits];
                    contribution = (1 << _sectorBits) > count ? count : 1 << _sectorBits;
                    intContribution = (int)contribution;
                    Array.Copy(buffer, offset, left, 0, intContribution);
                    if (!Add(bytes, left, out pair))
                    {
                        break;
                    }
                    pair = node;
                    offset += intContribution;
                    count -= intContribution;
                    _position += intContribution;
                    if (_position > _length)
                    {
                        _length = _position;
                    }
                    if (count == 0)
                    {
                        break;
                    }
                }
                var index = _position - pair.Key;
                if (pair.Value.Length > index)
                {
                    // The position is in the middle of the pair
                    var source = pair.Value;
                    var diff = pair.Value.Length - index;
                    contribution = diff > count ? count : diff;
                    if (contribution == 0)
                    {
                        break;
                    }
                    intContribution = (int)contribution;
                    Array.Copy(buffer, offset, source, (int)index, intContribution);
                    offset += intContribution;
                    count -= intContribution;
                    _position += intContribution;
                    if (_position > _length)
                    {
                        _length = _position;
                    }
                    if (count == 0)
                    {
                        break;
                    }
                }
            }
            while (count > 0)
            {
                KeyValuePair<long, byte[]> pair;
                var left = new byte[1 << _sectorBits];
                var index = _position - ((_position >> _sectorBits) << _sectorBits);
                var diff = (1 << _sectorBits) - index;
                var contribution = diff > count ? count : diff;
                var intContribution = (int)contribution;
                Array.Copy(buffer, offset, left, (int)index, intContribution);
                if (!Add(bytes, left, out pair))
                {
                    goto again;
                }
                offset += intContribution;
                count -= intContribution;
                _position += intContribution;
                if (_position > _length)
                {
                    _length = _position;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _length = -1;
            _position = -1;
            _bytes = null;
            base.Dispose(disposing);
        }

        private bool Add(Bucket<KeyValuePair<long, byte[]>> bytes, byte[] left, out KeyValuePair<long, byte[]> previous)
        {
            return bytes.Insert((int)(_position >> _sectorBits), new KeyValuePair<long, byte[]>((_position >> _sectorBits) << _sectorBits, left), out previous);
        }
    }
}

#endif