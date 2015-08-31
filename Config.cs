using System;
using System.IO;
using System.Text;
using WindowsInput.Native;

namespace ReplayReader
{
    public class Config
    {
        public string NewTitle = "";

        public void OpenConfig()
        {
            var path = Environment.CurrentDirectory + "\\Data\\Settings.cfg";
            var dataPath = path.Substring(0, path.IndexOf("\\Settings", StringComparison.Ordinal));
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            if (!File.Exists(path))
                using (var fs = File.Create(path))
                {
                    var line1 =
                        new UTF8Encoding(true).GetBytes("WindowTitle       *Osu!ReplayBot* //Title for window" +
                                                        Environment.NewLine);
                    var line2 =
                        new UTF8Encoding(true).GetBytes("LeftKey           *Z* //Left key for clicking" +
                                                        Environment.NewLine);
                    var line3 =
                        new UTF8Encoding(true).GetBytes("RightKey          *X* //Right key for clicking" +
                                                        Environment.NewLine);
                    var line4 =
                        new UTF8Encoding(true).GetBytes(
                                                        "MouseClicks       *0* //0 - no use mouse clicks, 1 - use mouse clicks" +
                                                        Environment.NewLine);
                    var line5 =
                        new UTF8Encoding(true).GetBytes(
                                                        "InversionMode     *0* //Convert HardRock to Normal and Normal to HardRock(1 - true, 0 - false)");
                    fs.Write(line1, 0, line1.Length);
                    fs.Write(line2, 0, line2.Length);
                    fs.Write(line3, 0, line3.Length);
                    fs.Write(line4, 0, line4.Length);
                    fs.Write(line5, 0, line5.Length);
                }
            var lines = File.ReadAllLines(path);
            NewTitle = lines[0].Split('*')[1];
            BotFunction.OsuLeftKey = lines[1].Split('*')[1].ToLower();
            BotFunction.OsuRightKey = lines[2].Split('*')[1].ToLower();
            BotFunction.OsuLeft = CharToVirtualKeyCode(BotFunction.OsuLeftKey);
            BotFunction.OsuRight = CharToVirtualKeyCode(BotFunction.OsuRightKey);
            BotFunction.UseMouse = lines[3].Split('*')[1] == "1";
            BotFunction.Inversion = lines[4].Split('*')[1] == "1";
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