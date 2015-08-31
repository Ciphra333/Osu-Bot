using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReplayAPI;

namespace ReplayReader
{
    public class Replay : IDisposable
    {
        private readonly CultureInfo _culture = new CultureInfo("en-US", false);
        private readonly BinaryReader _replayReader;
        private bool _fullLoaded;
        private bool _headerLoaded;
        public ushort Count100;
        public ushort Count300;
        public ushort Count50;
        public ushort CountGeki;
        public ushort CountKatu;
        public ushort CountMiss;
        public int FileFormat;
        public string Filename;
        public GameModes GameMode;
        public bool IsPerfect;
        public List<LifeFrame> LifeFrames = new List<LifeFrame>();
        public string MapHash;
        public ushort MaxCombo;
        public Mods Mods;
        public string PlayerName;
        public DateTime PlayTime;
        public List<ReplayFrame> ReplayFrames = new List<ReplayFrame>();
        public string ReplayHash;
        public int ReplayLength;
        public int Seed;
        public uint TotalScore;

        public Replay()
        {
        }

        public Replay(string replayFile, bool fullLoad = false)
        {
            Filename = replayFile;
            _replayReader = new BinaryReader(new FileStream(replayFile, FileMode.Open, FileAccess.Read, FileShare.Read));

            LoadHeader();
            if (fullLoad)
                Load();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Loads Metadata if not already loaded and loads Lifedata, Timestamp, Playtime and Clicks.
        /// </summary>
        public void Load()
        {
            if (!_headerLoaded)
                LoadHeader();
            if (_fullLoaded)
                return;

            //Life
            var lifeData = _replayReader.ReadNullableString();
            if (!string.IsNullOrEmpty(lifeData))
                foreach (
                    var split in
                        lifeData.Split(',').Select(lifeBlock => lifeBlock.Split('|')).Where(split => split.Length >= 2))
                    LifeFrames.Add(new LifeFrame
                    {
                        Time = int.Parse(split[0], _culture),
                        Percentage = float.Parse(split[1], _culture)
                    });

            var ticks = _replayReader.ReadInt64();
            PlayTime = new DateTime(ticks, DateTimeKind.Utc);

            ReplayLength = _replayReader.ReadInt32();

            //Data
            if (ReplayLength > 0)
            {
                var lastTime = 0;
                using (var codedStream = LzmaCoder.Decompress(_replayReader.BaseStream as FileStream))
                using (var sr = new StreamReader(codedStream))
                    foreach (
                        var split in
                            sr.ReadToEnd()
                              .Split(',')
                              .Where(frame => !string.IsNullOrEmpty(frame))
                              .Select(frame => frame.Split('|'))
                              .Where(split => split.Length >= 4))
                    {
                        if (split[0] == "-12345")
                        {
                            Seed = int.Parse(split[3], _culture);
                            continue;
                        }

                        ReplayFrames.Add(new ReplayFrame
                        {
                            TimeDiff = int.Parse(split[0], _culture),
                            Time = int.Parse(split[0], _culture) + lastTime,
                            X = float.Parse(split[1], _culture),
                            Y = float.Parse(split[2], _culture),
                            Keys = (KeyData) Enum.Parse(typeof (KeyData), split[3])
                        });
                        lastTime = ReplayFrames[ReplayFrames.Count - 1].Time;
                    }
            }

            //Todo: There are some extra bytes here

            _fullLoaded = true;
        }

        public void Save(string file)
        {
            using (var bw = new BinaryWriter(new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                //Header
                bw.Write((byte) GameMode);
                bw.Write(FileFormat);
                bw.WriteNullableString(MapHash);
                bw.WriteNullableString(PlayerName);
                bw.WriteNullableString(ReplayHash);
                bw.Write(Count300);
                bw.Write(Count100);
                bw.Write(Count50);
                bw.Write(CountGeki);
                bw.Write(CountKatu);
                bw.Write(CountMiss);
                bw.Write(TotalScore);
                bw.Write(MaxCombo);
                bw.Write(IsPerfect);
                bw.Write((int) Mods);

                //Life
                var sb = new StringBuilder();
                for (var i = 0; i < LifeFrames.Count; i++)
                    sb.AppendFormat("{0}|{1},", LifeFrames[i].Time.ToString(_culture),
                                    LifeFrames[i].Percentage.ToString(_culture));
                bw.WriteNullableString(sb.ToString());

                bw.Write(PlayTime.ToUniversalTime().Ticks);

                //Data
                if (ReplayFrames.Count == 0)
                    bw.Write(0);
                else
                {
                    sb.Clear();
                    for (var i = 0; i < ReplayFrames.Count; i++)
                        sb.AppendFormat("{0}|{1}|{2}|{3},", ReplayFrames[i].TimeDiff.ToString(_culture),
                                        ReplayFrames[i].X.ToString(_culture), ReplayFrames[i].Y.ToString(_culture),
                                        (int) ReplayFrames[i].Keys);
                    sb.AppendFormat("{0}|{1}|{2}|{3},", -12345, 0, 0, Seed);
                    var rawBytes = Encoding.ASCII.GetBytes(sb.ToString());
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(rawBytes, 0, rawBytes.Length);

                        var codedStream = LzmaCoder.Compress(ms);
                        var rawBytesCompressed = new byte[codedStream.Length];
                        codedStream.Read(rawBytesCompressed, 0, rawBytesCompressed.Length);

                        bw.Write(rawBytesCompressed.Length);
                        bw.Write(rawBytesCompressed);
                    }
                }

                //Todo: There are some extra bytes here
            }
        }

        protected void Dispose(bool state)
        {
            if (_replayReader != null)
                _replayReader.Close();
            ReplayFrames.Clear();
            LifeFrames.Clear();
        }

        private void LoadHeader()
        {
            GameMode = (GameModes) Enum.Parse(typeof (GameModes), _replayReader.ReadByte().ToString(_culture));
            FileFormat = _replayReader.ReadInt32();
            MapHash = _replayReader.ReadNullableString();
            PlayerName = _replayReader.ReadNullableString();
            ReplayHash = _replayReader.ReadNullableString();
            Count300 = _replayReader.ReadUInt16();
            Count100 = _replayReader.ReadUInt16();
            Count50 = _replayReader.ReadUInt16();
            CountGeki = _replayReader.ReadUInt16();
            CountKatu = _replayReader.ReadUInt16();
            CountMiss = _replayReader.ReadUInt16();
            TotalScore = _replayReader.ReadUInt32();
            MaxCombo = _replayReader.ReadUInt16();
            IsPerfect = _replayReader.ReadBoolean();
            Mods = (Mods) _replayReader.ReadInt32();
            _headerLoaded = true;
        }
    }
}