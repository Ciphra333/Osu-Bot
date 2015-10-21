using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ReplayReader
{
    partial class Menu
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            this.btnRead = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.LTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkToVk = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(12, 41);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(215, 23);
            this.btnRead.TabIndex = 0;
            this.btnRead.Text = "Read Replay";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 20;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Location = new System.Drawing.Point(12, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(24, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // LTitle
            // 
            this.LTitle.AutoSize = true;
            this.LTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LTitle.ForeColor = System.Drawing.Color.Red;
            this.LTitle.Location = new System.Drawing.Point(58, 15);
            this.LTitle.Name = "LTitle";
            this.LTitle.Size = new System.Drawing.Size(129, 20);
            this.LTitle.TabIndex = 23;
            this.LTitle.Text = "Osu!ReplayBot";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(12, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "v0.079";
            // 
            // linkToVk
            // 
            this.linkToVk.ActiveLinkColor = System.Drawing.Color.DarkOrange;
            this.linkToVk.AutoSize = true;
            this.linkToVk.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkToVk.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.linkToVk.Location = new System.Drawing.Point(173, 80);
            this.linkToVk.Name = "linkToVk";
            this.linkToVk.Size = new System.Drawing.Size(55, 17);
            this.linkToVk.TabIndex = 25;
            this.linkToVk.TabStop = true;
            this.linkToVk.Text = "Creator";
            this.linkToVk.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkToVk_LinkClicked);
            // 
            // Menu
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(240, 104);
            this.Controls.Add(this.linkToVk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LTitle);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRead);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Menu";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnRead;
        private Timer timer1;
        private Timer timer2;
        private Button btnClose;
        private Label LTitle;
        private Label label1;
        private LinkLabel linkToVk;
    }
}

