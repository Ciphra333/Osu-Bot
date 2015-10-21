using System;
using System.IO;
using System.Text;
using WindowsInput.Native;

namespace ReplayReader
{
    public class Config
    {
        public string NewTitle = "";
        readonly string _path = Environment.CurrentDirectory + "\\Data\\Settings.cfg";
        public void CreateFile(string title, string left, string right, string mouse, string inversion, int sizeX, int sizeY)
        {
            var dataPath = _path.Substring(0, _path.IndexOf("\\Settings", StringComparison.Ordinal));
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            using (var fs = File.Create(_path))
            {
                var parametr1 =
                    new UTF8Encoding(true).GetBytes("*" + title + "*" + " ");
                var parametr2 =
                    new UTF8Encoding(true).GetBytes("*" + left + "*" + " ");
                var parametr3 =
                    new UTF8Encoding(true).GetBytes("*" + right + "*" + " ");
                var parametr4 =
                    new UTF8Encoding(true).GetBytes("*" + mouse + "*" + " ");
                var parametr5 =
                    new UTF8Encoding(true).GetBytes("*" + inversion + "*" + " ");
                var parametr6 =
                    new UTF8Encoding(true).GetBytes("*" + sizeX + "*" + " ");
                var parametr7 =
                    new UTF8Encoding(true).GetBytes("*" + sizeY + "*" + " ");
                fs.Write(parametr1, 0, parametr1.Length);
                fs.Write(parametr2, 0, parametr2.Length);
                fs.Write(parametr3, 0, parametr3.Length);
                fs.Write(parametr4, 0, parametr4.Length);
                fs.Write(parametr5, 0, parametr5.Length);
                fs.Write(parametr6, 0, parametr6.Length);
                fs.Write(parametr7, 0, parametr7.Length);
            }
        }

        public void ReadeFile()
        {
            if (!File.Exists(_path))
            {
                CreateFile("Osu!ReplayBot", "z", "x", "0", "0", 800, 600);
            }
            var lines = File.ReadAllLines(_path);
            NewTitle = lines[0].Split('*')[1];
            BotFunction.OsuLeftKey = lines[0].Split('*')[3].ToLower();
            BotFunction.OsuRightKey = lines[0].Split('*')[5].ToLower();
            BotFunction.OsuLeft = CharToVirtualKeyCode(BotFunction.OsuLeftKey);
            BotFunction.OsuRight = CharToVirtualKeyCode(BotFunction.OsuRightKey);
            BotFunction.UseMouse = lines[0].Split('*')[7] == "1";
            BotFunction.Inversion = lines[0].Split('*')[9] == "1";
            Menu.OsuSizeX = int.Parse(lines[0].Split('*')[11]);
            Menu.OsuSizeY = int.Parse(lines[0].Split('*')[13]);
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