using System.ComponentModel;

namespace ReplayReader
{
    public struct ReplayFrame
    {
        public int Time;
        public int TimeDiff;

        [DisplayName(@"Time In Seconds")]
        public float TimeInSeconds => Time / 1000f;

        public float X { get; set; }
        public float Y { get; set; }
        public KeyData Keys { get; set; }
    }
}