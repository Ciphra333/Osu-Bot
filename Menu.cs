using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ReplayReader
{
    public partial class Menu : Form
    {
        public static IntPtr TimerAddress = (IntPtr) 0x000000;
        private static IntPtr _gameHandle = (IntPtr) null;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public static bool IsRun;

        public static int OsuCoordX;
        public static int OsuCoordY;
        private static int _osuSizeX = 800;
        private static int _osuSizeY = 640;
        private static int _pId;
        private static Rect _r;
        public static bool ReplayParsed = false;
        private static readonly int Dc = SystemInformation.CaptionHeight;
        public static int XOffset;
        public static int YOffset;
        public static float XMultiplier;
        public static float YMultiplier;
        private IntPtr _addressSizeX;
        private IntPtr _addressSizeY;

        public Menu()
        {
            InitializeComponent();
            var cfg = new Config();
            cfg.OpenConfig();
            Text = cfg.NewTitle;
            var m = new Thread(BotFunction.BotThread);
            m.Start();
            while (true)
            {
                _gameHandle = FindWindow(null, "osu!");

                if (_gameHandle != IntPtr.Zero)
                    break;
                Thread.Sleep(1);
            }
            TimerSearch();
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public static byte[] ReadMemory(IntPtr address, int size)
        {
            int bytesRead;
            var buffer = new byte[size];
            var hProcess = OpenProcess(0x001F0FFF, false, _pId);
            ReadProcessMemory(hProcess, address, buffer, size, out bytesRead);
            CloseHandle(hProcess);
            return buffer;
        }

        private static int FindSignature(int pid, IList<byte> signature)
        {
            const long maxAddr = 0x7fffffff;
            long addr = 0x00000000;
            MemoryBasicInformation m;
            var handle = OpenProcess(0x001F0FFF, false, pid);
            m.BaseAddress = IntPtr.Zero;
            m.RegionSize = IntPtr.Zero;
            while (addr <= maxAddr)
            {
                VirtualQueryEx(handle, (IntPtr) addr, out m, (uint) Marshal.SizeOf(typeof (MemoryBasicInformation)));
                var buffer = new byte[(uint) m.RegionSize];
                int dammy;
                ReadProcessMemory(handle, m.BaseAddress, buffer, (int) m.RegionSize, out dammy);
                if (addr == (long) m.BaseAddress + (long) m.RegionSize)
                    break;
                var count = 0;
                addr = (long) m.BaseAddress + (long) m.RegionSize;
                if (buffer.Length <= signature.Count) continue;
                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != signature[0]) continue;
                    for (var q = 0; q < signature.Count; q++)
                        try
                        {
                            if (buffer[i + q] == signature[q] || signature[q] == 0)
                            {
                                count++;
                                if (count == signature.Count)
                                    return i + (int) m.BaseAddress;
                            }
                            else
                                count = 0;
                        }
                        catch
                        {
                            addr -= signature.Count;
                        }
                }
            }
            return 0;
        }

        private static void GetProcess()
        {
            var pList = Process.GetProcesses();
            if (!pList.Any()) return;
            foreach (var process in pList.Where(process => process.ProcessName == "osu!"))
            {
                _pId = process.Id;
                return;
            }
        }

        private void TimerSearch()
        {
            //byte[] signature = {0xB8, 0x17, 0, 0, 0x1C, 0x13, 0, 0, 0xB8, 0x17, 0, 0, 0x1C, 0x13, 0, 0}; //old signature
            byte[] signature = { 0xC8, 0x17, 0, 0, 0x24, 0x13, 0, 0, 0xC8, 0x17, 0, 0, 0x24, 0x13, 0, 0};
            GetProcess();
            while (true)
            {
                var signatureAddress = (IntPtr) FindSignature(_pId, signature);
                TimerAddress = (IntPtr) ((int) signatureAddress + 0xCE0); // +0x1200 old
                _addressSizeX = (IntPtr) ((int) TimerAddress + 0x168); // +0x168 old
                _addressSizeY = (IntPtr) ((int) _addressSizeX + 0x4);

                if ((int) signatureAddress > 0)
                    break;
                Thread.Sleep(1);
            }
        }

        private static string Dec2HexL(long input)
        {
            var result = input.ToString("X");
            if (result.Length % 2 != 0) result = '0' + result;
            return result;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            BotFunction.Parse();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            Capture = false;
            var n = Message.Create(Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref n);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var x = ReadMemory(_addressSizeX, 4);
                var y = ReadMemory(_addressSizeY, 4);
                _osuSizeX = BitConverter.ToInt32(x, 0);
                _osuSizeY = BitConverter.ToInt32(y, 0);

                var winInfo = new Windowinfo();
                GetWindowInfo(_gameHandle, ref winInfo);
                _r = winInfo.rcWindow;
                _r.Left += (int) winInfo.cxWindowBorders / 2;
                _r.Top += (int) winInfo.cyWindowBorders;
                if (string.Compare(Dec2HexL(winInfo.dwStyle)[2].ToString(), "C", StringComparison.Ordinal) == 0)
                    _r.Top += Dc;

                if (_osuSizeX != SystemInformation.PrimaryMonitorSize.Width |
                    _osuSizeY != SystemInformation.PrimaryMonitorSize.Height)
                {
                    OsuCoordX = _r.Left;
                    OsuCoordY = _r.Top + 6;
                }
                else
                {
                    OsuCoordX = 0;
                    OsuCoordY = 12;
                }

                var swidth = _osuSizeX;
                var sheight = _osuSizeY;

                if (swidth * 3 > sheight * 4)
                    swidth = sheight * 4 / 3;
                else
                    sheight = swidth * 3 / 4;

                XMultiplier = swidth / 640f;
                YMultiplier = sheight / 480f;
                XOffset = (int) (_osuSizeX - 512 * XMultiplier) / 2;
                YOffset = (int) (_osuSizeY - 384 * YMultiplier) / 2;
            }
            catch
            {
                // ignored
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                var length = GetWindowTextLength(_gameHandle);
                var sb = new StringBuilder(length + 1);
                GetWindowText(_gameHandle, sb, sb.Capacity);
                var title = sb.ToString();

                if ((title != "osu!") && (title != "") && (ReplayParsed))
                {
                    if (IsRun) return;
                    IsRun = true;
                    LTitle.ForeColor = Color.Chartreuse;
                }
                else
                {
                    IsRun = false;
                    LTitle.ForeColor = Color.Red;
                    if (BotFunction.GetAsyncKeyState(Keys.LControlKey) == 0 || BotFunction.GetAsyncKeyState(Keys.O) == 0)
                        return;
                    var cfg = new Config();
                    cfg.OpenConfig();
                    var fpath = Environment.CurrentDirectory + "\\Data\\Settings.cfg";
                    var proc = Process.Start("notepad.exe", fpath);
                    if (proc == null) return;
                    proc.WaitForExit();
                    proc.Close();
                    cfg.OpenConfig();
                    Text = cfg.NewTitle;
                }
            }
            catch
            {
                // ignored
            }
        }

        #region Dll's

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName,
                                                string lpWindowName);

        //OpenProcess // Flag = All = 0x001F0FFF
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        // CloseHandle
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref Windowinfo pwi);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        //ReadProcessMemory
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead
            );

        // VirtualQueryEx
        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryBasicInformation lpBuffer,
                                                 uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryBasicInformation
        {
            public IntPtr BaseAddress;
            private readonly IntPtr AllocationBase;
            private readonly uint AllocationProtect;
            public IntPtr RegionSize;
            private readonly uint State;
            private readonly uint Protect;
            private readonly uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left, Top;
            private readonly int Right;
            private readonly int Bottom;

            private Rect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            private Rect(Rectangle r)
                : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }

            private int Height => Bottom - Top;

            private int Width => Right - Left;

            public static implicit operator Rectangle(Rect r)
            {
                return new Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator Rect(Rectangle r)
            {
                return new Rect(r);
            }

            public static bool operator ==(Rect r1, Rect r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(Rect r1, Rect r2)
            {
                return !r1.Equals(r2);
            }

            private bool Equals(Rect r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is Rect)
                    return Equals((Rect) obj);
                if (obj is Rectangle)
                    return Equals(new Rect((Rectangle) obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((Rectangle) this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top,
                                     Right, Bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Windowinfo
        {
            private readonly uint cbSize;
            public readonly Rect rcWindow;
            private readonly Rect rcClient;
            public readonly uint dwStyle;
            private readonly uint dwExStyle;
            private readonly uint dwWindowStatus;
            public readonly uint cxWindowBorders;
            public readonly uint cyWindowBorders;
            private readonly ushort atomWindowType;
            private readonly ushort wCreatorVersion;
        }

        #endregion Dll's

        private void linkToVk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/id256356348");
        }
    }

}