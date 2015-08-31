namespace SevenZip.Compression.RangeCoder
{
    internal struct BitEncoder
    {
        public const int KNumBitModelTotalBits = 11;
        public const uint KBitModelTotal = (1 << KNumBitModelTotalBits);
        private const int KNumMoveBits = 5;
        private const int KNumMoveReducingBits = 2;
        public const int KNumBitPriceShiftBits = 6;
        private static readonly uint[] _probPrices = new uint[KBitModelTotal >> KNumMoveReducingBits];
        private uint _prob;

        static BitEncoder()
        {
            const int kNumBits = (KNumBitModelTotalBits - KNumMoveReducingBits);
            for (var i = kNumBits - 1; i >= 0; i--)
            {
                var start = (uint) 1 << (kNumBits - i - 1);
                var end = (uint) 1 << (kNumBits - i);
                for (var j = start; j < end; j++)
                    _probPrices[j] = ((uint) i << KNumBitPriceShiftBits) +
                                     (((end - j) << KNumBitPriceShiftBits) >> (kNumBits - i - 1));
            }
        }

        public void Init()
        {
            _prob = KBitModelTotal >> 1;
        }

        public void UpdateModel(uint symbol)
        {
            if (symbol == 0)
                _prob += (KBitModelTotal - _prob) >> KNumMoveBits;
            else
                _prob -= (_prob) >> KNumMoveBits;
        }

        public void Encode(Encoder encoder, uint symbol)
        {
            // encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
            // UpdateModel(symbol);
            var newBound = (encoder.Range >> KNumBitModelTotalBits) * _prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                _prob += (KBitModelTotal - _prob) >> KNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                _prob -= (_prob) >> KNumMoveBits;
            }
            if (encoder.Range < Encoder.KTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

        public uint GetPrice(uint symbol)
        {
            return _probPrices[(((_prob - symbol) ^ ((-(int) symbol))) & (KBitModelTotal - 1)) >> KNumMoveReducingBits];
        }

        public uint GetPrice0()
        {
            return _probPrices[_prob >> KNumMoveReducingBits];
        }

        public uint GetPrice1()
        {
            return _probPrices[(KBitModelTotal - _prob) >> KNumMoveReducingBits];
        }
    }

    internal struct BitDecoder
    {
        public const int KNumBitModelTotalBits = 11;
        public const uint KBitModelTotal = (1 << KNumBitModelTotalBits);
        private const int KNumMoveBits = 5;
        private uint _prob;

        public void UpdateModel(int numMoveBits, uint symbol)
        {
            if (symbol == 0)
                _prob += (KBitModelTotal - _prob) >> numMoveBits;
            else
                _prob -= (_prob) >> numMoveBits;
        }

        public void Init()
        {
            _prob = KBitModelTotal >> 1;
        }

        public uint Decode(Decoder rangeDecoder)
        {
            var newBound = (rangeDecoder.Range >> KNumBitModelTotalBits) * _prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                _prob += (KBitModelTotal - _prob) >> KNumMoveBits;
                if (rangeDecoder.Range < Decoder.KTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 0;
            }
            rangeDecoder.Range -= newBound;
            rangeDecoder.Code -= newBound;
            _prob -= (_prob) >> KNumMoveBits;
            if (rangeDecoder.Range < Decoder.KTopValue)
            {
                rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte) rangeDecoder.Stream.ReadByte();
                rangeDecoder.Range <<= 8;
            }
            return 1;
        }
    }
}