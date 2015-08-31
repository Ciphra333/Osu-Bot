using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using ReplayReader.Properties;

namespace ReplayReader
{
    public static class BotFunction
    {
        private static KeyData _key;
        private static Replay _rep;
        private static int _replayIndex;
        private static int _rTime;
        private static int _trackPosition;
        private static readonly List<KeyData> Btn = new List<KeyData>();
        private static readonly List<int> Time = new List<int>();
        private static readonly List<float> X = new List<float>();
        private static readonly List<float> Y = new List<float>();
        private static int _zero;
        public static string OsuLeftKey;
        public static string OsuRightKey;
        public static VirtualKeyCode OsuLeft;
        public static VirtualKeyCode OsuRight;
        public static bool Inversion = false;
        public static bool UseMouse = false;
        private static readonly InputSimulator Input = new InputSimulator();
        private static GameModes _mode;
        private static float _tempX;
        private static float _tempY;

        private static readonly float AbsX = 65535.0f / SystemInformation.PrimaryMonitorSize.Width;
        private static readonly float AbsY = 65535.0f / SystemInformation.PrimaryMonitorSize.Height;

        // ReSharper disable once NotAccessedField.Local
        private static IKeyboardSimulator _keyboardSimulator;

        // ReSharper disable once NotAccessedField.Local
        private static IMouseSimulator _mouseSimulator;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        public static void BotThread()
        {
            while (true)
            {
                if (Menu.IsRun && _mode == GameModes.Osu)
                    OsuTap();
                else
                // ReSharper disable once RedundantCheckBeforeAssignment
                    if (_replayIndex != _zero)
                        _replayIndex = _zero;
                Thread.Sleep(1);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        //private static void TaikoTap()
        //{
        //    var x = (int)X[_replayIndex];
        //    _rTime = Time[_replayIndex];

        //    if (GetAsyncKeyState(Keys.LControlKey) != 0)
        //        _panic = true;

        //    if (TrackPosition < _rTime) return;
        //    if (_panic) //Водит курсор, но не нажимает кнопки.
        //    {
        //        _replayIndex++;
        //        _panic = false;
        //        if (UseMouse)
        //        {
        //            Input.Mouse.LeftButtonUp();
        //            Input.Mouse.RightButtonUp();
        //        }
        //        Input.Keyboard.KeyUp(OsuLeft);
        //        Input.Keyboard.KeyUp(OsuRight);
        //        return;
        //    }
        //    if (x == 640)
        //    {
        //        Input.Keyboard.KeyDown(VirtualKeyCode.VK_J);
        //    }
        //    else
        //    {
        //        Input.Keyboard.KeyUp(VirtualKeyCode.VK_J);
        //    }
        //    if (x == 320)
        //    {
        //        Input.Keyboard.KeyDown(VirtualKeyCode.VK_F);
        //    }
        //    else
        //    {
        //        Input.Keyboard.KeyUp(VirtualKeyCode.VK_F);
        //    }
        //    _replayIndex++;
        //    _panic = false;
        //}
        private static void OsuTap()
        {
            _key = Btn[_replayIndex];
            _tempX = GetScaledX(X[_replayIndex]) * AbsX;
            _tempY = GetScaledY(Y[_replayIndex]) * AbsY;
            _rTime = Time[_replayIndex];
            var timer = Menu.ReadMemory(Menu.TimerAddress, 4);
            _trackPosition = BitConverter.ToInt32(timer, 0);
            while (_trackPosition < _rTime && Menu.IsRun)
            {
                Thread.Sleep(1);
                timer = Menu.ReadMemory(Menu.TimerAddress, 4);
                _trackPosition = BitConverter.ToInt32(timer, 0);
            }
            Input.Mouse.MoveMouseTo(_tempX, _tempY);

            if (GetAsyncKeyState(Keys.LControlKey) != 0)
            {
                if (UseMouse)
                {
                    Input.Mouse.LeftButtonUp();
                    Input.Mouse.RightButtonUp();
                }
                Input.Keyboard.KeyUp(OsuLeft);
                Input.Keyboard.KeyUp(OsuRight);
                _replayIndex++;
                return;
            }
            if (UseMouse)
            {
                _mouseSimulator = (_key.HasFlag(KeyData.M1) && !_key.HasFlag(KeyData.K1))
                                      ? Input.Mouse.LeftButtonDown()
                                      : Input.Mouse.LeftButtonUp();
                _mouseSimulator = (_key.HasFlag(KeyData.M2) && !_key.HasFlag(KeyData.K2))
                                      ? Input.Mouse.RightButtonDown()
                                      : Input.Mouse.RightButtonUp();
            }
            _keyboardSimulator = _key.HasFlag(KeyData.K1)
                                     ? Input.Keyboard.KeyDown(OsuLeft)
                                     : Input.Keyboard.KeyUp(OsuLeft);
            _keyboardSimulator = _key.HasFlag(KeyData.K2)
                                     ? Input.Keyboard.KeyDown(OsuRight)
                                     : Input.Keyboard.KeyUp(OsuRight);
            _replayIndex++;
        }

        private static int GetScaledX(float x)
        {
             return (int) (x * Menu.XMultiplier + Menu.XOffset + Menu.OsuCoordX);
        }

        private static int GetScaledY(float y)
        {
            if (Inversion) //HR to Normal and Normal to HR
                y = 384 - y;
            return (int) (y * Menu.YMultiplier + Menu.YOffset + Menu.OsuCoordY);
        }

        public static void Parse()
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetPath(),
                Filter = Resources.mouseMoverThread_parse_osr_files____osr____osr,
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            X.Clear();
            Y.Clear();
            Time.Clear();
            Btn.Clear();
            var path = dialog.FileName;
            _rep = new Replay(path, true);
            _mode = _rep.GameMode;
            var index = 0;
            while (index < _rep.ReplayFrames.Count - 2)
            {
                var thisFrame = _rep.ReplayFrames[index];
                var futureFrame = _rep.ReplayFrames[index + 1];
                X.Add(thisFrame.X);
                Y.Add(thisFrame.Y);
                Time.Add(thisFrame.Time);
                Btn.Add(thisFrame.Keys);

                //Smooth moving
                if (thisFrame.Time > 0 && futureFrame.TimeDiff > 19)
                {
                    var steps = futureFrame.TimeDiff / 10;
                    var xMult = (futureFrame.X - thisFrame.X) / steps;
                    var yMult = (futureFrame.Y - thisFrame.Y) / steps;

                    var startX = thisFrame.X;
                    var startY = thisFrame.Y;
                    var startTime = thisFrame.Time;
                    var startBtn = thisFrame.Keys;
                    for (var i = 0; i < steps; i++)
                    {
                        startX = startX + xMult;
                        startY = startY + yMult;
                        startTime = startTime + 10;
                        X.Add(startX);
                        Y.Add(startY);
                        Time.Add(startTime);
                        Btn.Add(startBtn);
                    }
                }

                index++;
            }
            var it = 0;
            while (Time[it] <= 0)
                it++;
            if (Time[it + 1] < 0)
                while (Time[it + 1] <= 0)
                    it++;
            for (var i = 0; i < 4; i++)
            {
                Time.Add(999999999);
                X.Add(0);
                Y.Add(0);
                Btn.Add(KeyData.None);
            }
            _zero = it;
            Menu.ReplayParsed = true;
        }
    }
}