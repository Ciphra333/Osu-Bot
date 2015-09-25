﻿using ReplayReader.Properties;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace ReplayReader
{
    public static class BotFunction
    {
        private static Replay _originalReplay;
        private static List<ReplayFrame> _rep;
        private static int _replayIndex;
        public static string OsuLeftKey;
        public static string OsuRightKey;
        public static VirtualKeyCode OsuLeft;
        public static VirtualKeyCode OsuRight;
        public static bool Inversion = false;
        public static bool UseMouse = false;
        private static readonly InputSimulator Input = new InputSimulator();
        private static GameModes _mode;

        private static readonly float AbsX = 65535.0f / SystemInformation.PrimaryMonitorSize.Width;
        private static readonly float AbsY = 65535.0f / SystemInformation.PrimaryMonitorSize.Height;

        private static IKeyboardSimulator _keyboardSimulator;

        private static IMouseSimulator _mouseSimulator;

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        public static void BotThread() // it very stupid idea... But it works o_O
        {
            while (true)
            {
                if (Menu.IsRun && _mode == GameModes.Osu)
                    OsuTap(_rep[_replayIndex]);
                else
                    if (_replayIndex != 0)
                        _replayIndex = 0;
                Thread.Sleep(1);
            }
        }

        private static void OsuTap(ReplayFrame frame)
        {
            var timer = Menu.ReadMemory(Menu.TimerAddress, 4);
            var _trackPosition = BitConverter.ToInt32(timer, 0);
            var frameTime = frame.Time;
            while (_trackPosition < frameTime && Menu.IsRun)
            {
                Thread.Sleep(1);
                timer = Menu.ReadMemory(Menu.TimerAddress, 4);
                _trackPosition = BitConverter.ToInt32(timer, 0);
            }
            Input.Mouse.MoveMouseTo(GetScaledX(frame.X), GetScaledY(frame.Y));

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
                _mouseSimulator = (frame.Keys.HasFlag(KeyData.M1) && !frame.Keys.HasFlag(KeyData.K1))
                                      ? Input.Mouse.LeftButtonDown()
                                      : Input.Mouse.LeftButtonUp();
                _mouseSimulator = (frame.Keys.HasFlag(KeyData.M2) && !frame.Keys.HasFlag(KeyData.K2))
                                      ? Input.Mouse.RightButtonDown()
                                      : Input.Mouse.RightButtonUp();
            }
            _keyboardSimulator = frame.Keys.HasFlag(KeyData.K1)
                                     ? Input.Keyboard.KeyDown(OsuLeft)
                                     : Input.Keyboard.KeyUp(OsuLeft);
            _keyboardSimulator = frame.Keys.HasFlag(KeyData.K2)
                                     ? Input.Keyboard.KeyDown(OsuRight)
                                     : Input.Keyboard.KeyUp(OsuRight);
            _replayIndex++;
        }

        private static int GetScaledX(float x)
        {
             return (int) ((x * Menu.XMultiplier + Menu.XOffset + Menu.OsuCoordX) * AbsX);
        }

        private static int GetScaledY(float y)
        {
            if (Inversion) //HR to Normal and Normal to HR
                y = 384 - y;
            return (int) ((y * Menu.YMultiplier + Menu.YOffset + Menu.OsuCoordY) * AbsY);
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
            var path = dialog.FileName;
            _rep.Clear();
            _originalReplay = new Replay(path, true);
            _mode = _originalReplay.GameMode;
            var index = 0;
            while (index < _originalReplay.ReplayFrames.Count - 2)
            {
                var thisFrame = _originalReplay.ReplayFrames[index];

                if (thisFrame.Time < 0) { index++; continue; } // i don't like negative time :)

                var futureFrame = _originalReplay.ReplayFrames[index + 1];
                var frame = new ReplayFrame
                {
                    X = thisFrame.X,
                    Y = thisFrame.Y,
                    Time = thisFrame.Time,
                    Keys = thisFrame.Keys
                };
                _rep.Add(frame);

                //Smooth linear moving
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
                        var smoothFrame = new ReplayFrame
                        {
                            X = startX,
                            Y = startY,
                            Time = startTime,
                            Keys = startBtn
                        };
                    }
                }

                index++;
            }

            var speedFrame = new ReplayFrame
            {
                X = 0,
                Y = 0,
                Keys = KeyData.None,
                Time = 999999999
            };

            for (var i = 0; i < 4; i++) // I'm use it for speed up... Really, it don't good...
            { 
                _rep.Add(speedFrame);
            }
            Menu.ReplayParsed = true;
        }
    }
}