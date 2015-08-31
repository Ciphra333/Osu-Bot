// LzBinTree.cs

using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
    public class BinTree : InWindow, IMatchFinder
    {
        private const uint KHash2Size = 1 << 10;
        private const uint KHash3Size = 1 << 16;
        private const uint KBt2HashSize = 1 << 16;
        private const uint KStartMaxLen = 1;
        private const uint KHash3Offset = KHash2Size;
        private const uint KEmptyHashValue = 0;
        private const uint KMaxValForNormalize = ((uint) 1 << 31) - 1;
        private uint _cutValue = 0xFF;
        private uint _cyclicBufferPos;
        private uint _cyclicBufferSize;
        private uint[] _hash;
        private bool _hashArray = true;
        private uint _hashMask;
        private uint _hashSizeSum;
        private uint _kFixHashSize = KHash2Size + KHash3Size;
        private uint _kMinMatchCheck = 4;
        private uint _kNumHashDirectBytes;
        private uint _matchMaxLen;
        private uint[] _son;

        public new void SetStream(Stream stream)
        {
            base.SetStream(stream);
        }

        public new void ReleaseStream()
        {
            base.ReleaseStream();
        }

        public new void Init()
        {
            base.Init();
            for (uint i = 0; i < _hashSizeSum; i++)
                _hash[i] = KEmptyHashValue;
            _cyclicBufferPos = 0;
            ReduceOffsets(-1);
        }

        public new byte GetIndexByte(int index)
        {
            return base.GetIndexByte(index);
        }

        public new uint GetMatchLen(int index, uint distance, uint limit)
        {
            return base.GetMatchLen(index, distance, limit);
        }

        public new uint GetNumAvailableBytes()
        {
            return base.GetNumAvailableBytes();
        }

        public void Create(uint historySize, uint keepAddBufferBefore,
                           uint matchMaxLen, uint keepAddBufferAfter)
        {
            if (historySize > KMaxValForNormalize - 256)
                throw new Exception();
            _cutValue = 16 + (matchMaxLen >> 1);

            var windowReservSize = (historySize + keepAddBufferBefore +
                                    matchMaxLen + keepAddBufferAfter) / 2 + 256;

            base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

            _matchMaxLen = matchMaxLen;

            var cyclicBufferSize = historySize + 1;
            if (_cyclicBufferSize != cyclicBufferSize)
                _son = new uint[(_cyclicBufferSize = cyclicBufferSize) * 2];

            var hs = KBt2HashSize;

            if (_hashArray)
            {
                hs = historySize - 1;
                hs |= (hs >> 1);
                hs |= (hs >> 2);
                hs |= (hs >> 4);
                hs |= (hs >> 8);
                hs >>= 1;
                hs |= 0xFFFF;
                if (hs > (1 << 24))
                    hs >>= 1;
                _hashMask = hs;
                hs++;
                hs += _kFixHashSize;
            }
            if (hs != _hashSizeSum)
                _hash = new uint[_hashSizeSum = hs];
        }

        public uint GetMatches(uint[] distances)
        {
            uint lenLimit;
            if (Pos + _matchMaxLen <= StreamPos)
                lenLimit = _matchMaxLen;
            else
            {
                lenLimit = StreamPos - Pos;
                if (lenLimit < _kMinMatchCheck)
                {
                    MovePos();
                    return 0;
                }
            }

            uint offset = 0;
            var matchMinPos = (Pos > _cyclicBufferSize) ? (Pos - _cyclicBufferSize) : 0;
            var cur = BufferOffset + Pos;
            var maxLen = KStartMaxLen; // to avoid items for len < hashSize;
            uint hashValue, hash2Value = 0, hash3Value = 0;

            if (_hashArray)
            {
                var temp = Crc.Table[BufferBase[cur]] ^ BufferBase[cur + 1];
                hash2Value = temp & (KHash2Size - 1);
                temp ^= ((uint) (BufferBase[cur + 2]) << 8);
                hash3Value = temp & (KHash3Size - 1);
                hashValue = (temp ^ (Crc.Table[BufferBase[cur + 3]] << 5)) & _hashMask;
            }
            else
                hashValue = BufferBase[cur] ^ ((uint) (BufferBase[cur + 1]) << 8);

            var curMatch = _hash[_kFixHashSize + hashValue];
            if (_hashArray)
            {
                var curMatch2 = _hash[hash2Value];
                var curMatch3 = _hash[KHash3Offset + hash3Value];
                _hash[hash2Value] = Pos;
                _hash[KHash3Offset + hash3Value] = Pos;
                if (curMatch2 > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch2] == BufferBase[cur])
                    {
                        distances[offset++] = maxLen = 2;
                        distances[offset++] = Pos - curMatch2 - 1;
                    }
                if (curMatch3 > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch3] == BufferBase[cur])
                    {
                        if (curMatch3 == curMatch2)
                            offset -= 2;
                        distances[offset++] = maxLen = 3;
                        distances[offset++] = Pos - curMatch3 - 1;
                        curMatch2 = curMatch3;
                    }
                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = KStartMaxLen;
                }
            }

            _hash[_kFixHashSize + hashValue] = Pos;

            var ptr0 = (_cyclicBufferPos << 1) + 1;
            var ptr1 = (_cyclicBufferPos << 1);

            uint len0, len1;
            len0 = len1 = _kNumHashDirectBytes;

            if (_kNumHashDirectBytes != 0)
                if (curMatch > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch + _kNumHashDirectBytes] !=
                        BufferBase[cur + _kNumHashDirectBytes])
                    {
                        distances[offset++] = maxLen = _kNumHashDirectBytes;
                        distances[offset++] = Pos - curMatch - 1;
                    }

            var count = _cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    _son[ptr0] = _son[ptr1] = KEmptyHashValue;
                    break;
                }
                var delta = Pos - curMatch;
                var cyclicPos = ((delta <= _cyclicBufferPos)
                                     ? (_cyclicBufferPos - delta)
                                     : (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                var pby1 = BufferOffset + curMatch;
                var len = Math.Min(len0, len1);
                if (BufferBase[pby1 + len] == BufferBase[cur + len])
                {
                    while (++len != lenLimit)
                        if (BufferBase[pby1 + len] != BufferBase[cur + len])
                            break;
                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                }
                if (BufferBase[pby1 + len] < BufferBase[cur + len])
                {
                    _son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = _son[ptr1];
                    len1 = len;
                }
                else
                {
                    _son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = _son[ptr0];
                    len0 = len;
                }
            }
            MovePos();
            return offset;
        }

        public void Skip(uint num)
        {
            do
            {
                uint lenLimit;
                if (Pos + _matchMaxLen <= StreamPos)
                    lenLimit = _matchMaxLen;
                else
                {
                    lenLimit = StreamPos - Pos;
                    if (lenLimit < _kMinMatchCheck)
                    {
                        MovePos();
                        continue;
                    }
                }

                var matchMinPos = (Pos > _cyclicBufferSize) ? (Pos - _cyclicBufferSize) : 0;
                var cur = BufferOffset + Pos;

                uint hashValue;

                if (_hashArray)
                {
                    var temp = Crc.Table[BufferBase[cur]] ^ BufferBase[cur + 1];
                    var hash2Value = temp & (KHash2Size - 1);
                    _hash[hash2Value] = Pos;
                    temp ^= ((uint) (BufferBase[cur + 2]) << 8);
                    var hash3Value = temp & (KHash3Size - 1);
                    _hash[KHash3Offset + hash3Value] = Pos;
                    hashValue = (temp ^ (Crc.Table[BufferBase[cur + 3]] << 5)) & _hashMask;
                }
                else
                    hashValue = BufferBase[cur] ^ ((uint) (BufferBase[cur + 1]) << 8);

                var curMatch = _hash[_kFixHashSize + hashValue];
                _hash[_kFixHashSize + hashValue] = Pos;

                var ptr0 = (_cyclicBufferPos << 1) + 1;
                var ptr1 = (_cyclicBufferPos << 1);

                uint len0, len1;
                len0 = len1 = _kNumHashDirectBytes;

                var count = _cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        _son[ptr0] = _son[ptr1] = KEmptyHashValue;
                        break;
                    }

                    var delta = Pos - curMatch;
                    var cyclicPos = ((delta <= _cyclicBufferPos)
                                         ? (_cyclicBufferPos - delta)
                                         : (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                    var pby1 = BufferOffset + curMatch;
                    var len = Math.Min(len0, len1);
                    if (BufferBase[pby1 + len] == BufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                            if (BufferBase[pby1 + len] != BufferBase[cur + len])
                                break;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                    if (BufferBase[pby1 + len] < BufferBase[cur + len])
                    {
                        _son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = _son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        _son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = _son[ptr0];
                        len0 = len;
                    }
                }
                MovePos();
            } while (--num != 0);
        }

        public void SetType(int numHashBytes)
        {
            _hashArray = (numHashBytes > 2);
            if (_hashArray)
            {
                _kNumHashDirectBytes = 0;
                _kMinMatchCheck = 4;
                _kFixHashSize = KHash2Size + KHash3Size;
            }
            else
            {
                _kNumHashDirectBytes = 2;
                _kMinMatchCheck = 2 + 1;
                _kFixHashSize = 0;
            }
        }

        public new void MovePos()
        {
            if (++_cyclicBufferPos >= _cyclicBufferSize)
                _cyclicBufferPos = 0;
            base.MovePos();
            if (Pos == KMaxValForNormalize)
                Normalize();
        }

        private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
        {
            for (uint i = 0; i < numItems; i++)
            {
                var value = items[i];
                if (value <= subValue)
                    value = KEmptyHashValue;
                else
                    value -= subValue;
                items[i] = value;
            }
        }

        private void Normalize()
        {
            var subValue = Pos - _cyclicBufferSize;
            NormalizeLinks(_son, _cyclicBufferSize * 2, subValue);
            NormalizeLinks(_hash, _hashSizeSum, subValue);
            ReduceOffsets((int) subValue);
        }

        public void SetCutValue(uint cutValue)
        {
            _cutValue = cutValue;
        }
    }
}