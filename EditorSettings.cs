using System;
using System.Windows.Forms;

namespace ReplayReader
{
    public partial class EditorSettings : Form
    {
        public EditorSettings()
        {
            InitializeComponent();
            Config cfg = new Config();
            cfg.readeFile();
            titleBox.Text = cfg.NewTitle;
            leftBox.SelectedIndex = leftBox.Items.IndexOf(BotFunction.OsuLeftKey);
            rightBox.SelectedIndex = rightBox.Items.IndexOf(BotFunction.OsuRightKey);
            Mouse_check.Checked = BotFunction.UseMouse;
            inversion_check.Checked = BotFunction.Inversion;
        }

        private void Save_Click(object sender, System.EventArgs e)
        {
            var cfg = new Config();
            cfg.CreateFile(titleBox.Text, leftBox.Text, rightBox.Text, Mouse_check.Checked ? "1" : "0", inversion_check.Checked ? "1" : "0");
            cfg.readeFile();
            ReplayReader.Menu.settingOpen = false;
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
