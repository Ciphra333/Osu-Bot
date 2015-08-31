using Microsoft.Win32;

namespace ReplayReader
{
    public static class Path
    {
        public static string GetPath()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\osu!\DefaultIcon");
            if (key == null) return "";
            var path = key.GetValue(null).ToString();
            path = path.Substring(1, path.Length - 13) + "\\Replays";
            return path;
        }
    }
}