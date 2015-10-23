
namespace ReplayReader
{
    public struct ReplayFrame
    {
        public int Time;
        public int TimeDiff;

        public float X { get; set; }
        public float Y { get; set; }
        public KeyData Keys { get; set; }
    }
}