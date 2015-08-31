using System;
using System.IO;
using SevenZip;
using SevenZip.Compression.LZMA;

namespace ReplayReader
{
    public static class LzmaCoder
    {
        public static MemoryStream Compress(MemoryStream inStream)
        {
            inStream.Position = 0;

            CoderPropId[] propIDs =
            {
                CoderPropId.DictionarySize,
                CoderPropId.PosStateBits,
                CoderPropId.LitContextBits,
                CoderPropId.LitPosBits,
                CoderPropId.Algorithm
            };

            object[] properties =
            {
                (1 << 16),
                2,
                3,
                0,
                2
            };

            var outStream = new MemoryStream();
            var encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            for (var i = 0; i < 8; i++)
                outStream.WriteByte((byte) (inStream.Length >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            outStream.Flush();
            outStream.Position = 0;

            return outStream;
        }

        public static MemoryStream Decompress(FileStream inStream)
        {
            var decoder = new Decoder();

            var properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            decoder.SetDecoderProperties(properties);

            long outSize = 0;
            for (var i = 0; i < 8; i++)
            {
                var v = inStream.ReadByte();
                if (v < 0)
                    break;
                outSize |= ((long) (byte) v) << (8 * i);
            }
            var compressedSize = inStream.Length - inStream.Position;

            var outStream = new MemoryStream();
            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            outStream.Flush();
            outStream.Position = 0;
            return outStream;
        }
    }
}