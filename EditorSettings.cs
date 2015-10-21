using System;
using System.Windows.Forms;

namespace ReplayReader
{
    public partial class EditorSettings : Form
    {
        public EditorSettings()
        {
            InitializeComponent();
            var cfg = new Config();
            cfg.ReadeFile();
            titleBox.Text = cfg.NewTitle;
            leftBox.SelectedIndex = leftBox.Items.IndexOf(BotFunction.OsuLeftKey);
            rightBox.SelectedIndex = rightBox.Items.IndexOf(BotFunction.OsuRightKey);
            Mouse_check.Checked = BotFunction.UseMouse;
            inversion_check.Checked = BotFunction.Inversion;
            boxSizeX.Text = ReplayReader.Menu.OsuSizeX.ToString();
            boxSizeY.Text = ReplayReader.Menu.OsuSizeY.ToString();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var cfg = new Config();

            int xSize, ySize;
            int.TryParse(boxSizeX.Text, out xSize);
            int.TryParse(boxSizeY.Text, out ySize);

            xSize = xSize == -1 ? 800 : xSize; // if x/ySize NaN then used 800x600 resolution 
            ySize = ySize == -1 ? 600 : ySize;

            cfg.CreateFile(titleBox.Text, leftBox.Text, rightBox.Text, Mouse_check.Checked ? "1" : "0", inversion_check.Checked ? "1" : "0", xSize, ySize);
            cfg.ReadeFile();
            ReplayReader.Menu.SettingOpen = false;
            Close();
        }

        private void EditorSettings_MouseDown(object sender, MouseEventArgs e)
        {
            Capture = false;
            var n = Message.Create(Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            WndProc(ref n);
        }
    }
}
