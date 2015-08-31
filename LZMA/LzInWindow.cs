// LzInWindow.cs

using System.IO;

namespace SevenZip.Compression.LZ
{
    public class InWindow
    {
        private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos
        private uint _pointerToLastSafePosition;
        private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        private Stream _stream;
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream
        public uint BlockSize; // Size of Allocated memory block
        public byte[] BufferBase; // pointer to buffer with data
        public uint BufferOffset;
        public uint Pos; // offset (from _buffer) of curent byte
        public uint StreamPos; // offset (from _buffer) of first not read byte from Stream

        public void MoveBlock()
        {
            var offset = BufferOffset + Pos - _keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
                offset--;

            var numBytes = BufferOffset + StreamPos - offset;

            // check negative offset ????
            for (uint i = 0; i < numBytes; i++)
                BufferBase[i] = BufferBase[offset + i];
            BufferOffset -= offset;
        }

        public virtual void ReadBlock()
        {
            if (_streamEndWasReached)
                return;
            while (true)
            {
                var size = (int) ((0 - BufferOffset) + BlockSize - StreamPos);
                if (size == 0)
                    return;
                var numReadBytes = _stream.Read(BufferBase, (int) (BufferOffset + StreamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = StreamPos;
                    var pointerToPostion = BufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition)
                        _posLimit = _pointerToLastSafePosition - BufferOffset;

                    _streamEndWasReached = true;
                    return;
                }
                StreamPos += (uint) numReadBytes;
                if (StreamPos >= Pos + _keepSizeAfter)
                    _posLimit = StreamPos - _keepSizeAfter;
            }
        }

        private void Free()
        {
            BufferBase = null;
        }

        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            var blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (BufferBase == null || BlockSize != blockSize)
            {
                Free();
                BlockSize = blockSize;
                BufferBase = new byte[BlockSize];
            }
            _pointerToLastSafePosition = BlockSize - keepSizeAfter;
        }

        public void SetStream(Stream stream)
        {
            _stream = stream;
        }

        public void ReleaseStream()
        {
            _stream = null;
        }

        public void Init()
        {
            BufferOffset = 0;
            Pos = 0;
            StreamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        public void MovePos()
        {
            Pos++;
            if (Pos > _posLimit)
            {
                var pointerToPostion = BufferOffset + Pos;
                if (pointerToPostion > _pointerToLastSafePosition)
                    MoveBlock();
                ReadBlock();
            }
        }

        public byte GetIndexByte(int index)
        {
            return BufferBase[BufferOffset + Pos + index];
        }

        // index + limit have not to exceed _keepSizeAfter;
        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (_streamEndWasReached)
                if ((Pos + index) + limit > StreamPos)
                    limit = StreamPos - (uint) (Pos + index);
            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            var pby = BufferOffset + Pos + (uint) index;

            uint i;
            for (i = 0; i < limit && BufferBase[pby + i] == BufferBase[pby + i - distance]; i++) ;
            return i;
        }

        public uint GetNumAvailableBytes()
        {
            return StreamPos - Pos;
        }

        public void ReduceOffsets(int subValue)
        {
            BufferOffset += (uint) subValue;
            _posLimit -= (uint) subValue;
            Pos -= (uint) subValue;
            StreamPos -= (uint) subValue;
        }
    }
}