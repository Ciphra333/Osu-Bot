using System;
using System.IO;
using System.Text;
using WindowsInput.Native;

namespace ReplayReader
{
    public class Config
    {
        public string NewTitle = "";
        string path = Environment.CurrentDirectory + "\\Data\\Settings.cfg";
        public void CreateFile(string title, string left, string right, string mouse, string inversion)
        {
            var dataPath = path.Substring(0, path.IndexOf("\\Settings", StringComparison.Ordinal));
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            using (var fs = File.Create(path))
            {
                var line1 =
                    new UTF8Encoding(true).GetBytes("*" + title + "*" + " ");
                var line2 =
                    new UTF8Encoding(true).GetBytes("*" + left + "*" + " ");
                var line3 =
                    new UTF8Encoding(true).GetBytes("*" + right + "*" + " ");
                var line4 =
                    new UTF8Encoding(true).GetBytes("*" + mouse + "*" + " ");
                var line5 =
                    new UTF8Encoding(true).GetBytes("*" + inversion + "*" + " ");
                fs.Write(line1, 0, line1.Length);
                fs.Write(line2, 0, line2.Length);
                fs.Write(line3, 0, line3.Length);
                fs.Write(line4, 0, line4.Length);
                fs.Write(line5, 0, line5.Length);
            }
        }

        public void readeFile()
        {
            var lines = File.ReadAllLines(path);
            NewTitle = lines[0].Split('*')[1];
            BotFunction.OsuLeftKey = lines[0].Split('*')[3].ToLower();
            BotFunction.OsuRightKey = lines[0].Split('*')[5].ToLower();
            BotFunction.OsuLeft = CharToVirtualKeyCode(BotFunction.OsuLeftKey);
            BotFunction.OsuRight = CharToVirtualKeyCode(BotFunction.OsuRightKey);
            BotFunction.UseMouse = lines[0].Split('*')[7] == "1";
            BotFunction.Inversion = lines[0].Split('*')[9] == "1";
        }

        private static VirtualKeyCode CharToVirtualKeyCode(string key)
        {
            switch (key)
            {
                case "a":
                    return VirtualKeyCode.VK_A;

                case "b":
                    return VirtualKeyCode.VK_B;

                case "c":
                    return VirtualKeyCode.VK_C;

                case "d":
                    return VirtualKeyCode.VK_D;

                case "e":
                    return VirtualKeyCode.VK_E;

                case "f":
                    return VirtualKeyCode.VK_F;

                case "g":
                    return VirtualKeyCode.VK_G;

                case "h":
                    return VirtualKeyCode.VK_H;

                case "i":
                    return VirtualKeyCode.VK_I;

                case "j":
                    return VirtualKeyCode.VK_J;

                case "k":
                    return VirtualKeyCode.VK_K;

                case "l":
                    return VirtualKeyCode.VK_L;

                case "m":
                    return VirtualKeyCode.VK_M;

                case "n":
                    return VirtualKeyCode.VK_N;

                case "o":
                    return VirtualKeyCode.VK_O;

                case "p":
                    return VirtualKeyCode.VK_P;

                case "q":
                    return VirtualKeyCode.VK_Q;

                case "r":
                    return VirtualKeyCode.VK_R;

                case "s":
                    return VirtualKeyCode.VK_S;

                case "t":
                    return VirtualKeyCode.VK_T;

                case "u":
                    return VirtualKeyCode.VK_U;

                case "v":
                    return VirtualKeyCode.VK_V;

                case "w":
                    return VirtualKeyCode.VK_W;

                case "x":
                    return VirtualKeyCode.VK_X;

                case "y":
                    return VirtualKeyCode.VK_Y;

                case "z":
                    return VirtualKeyCode.VK_Z;
                default:
                    return VirtualKeyCode.VK_Z;
            }
        }
    }
}