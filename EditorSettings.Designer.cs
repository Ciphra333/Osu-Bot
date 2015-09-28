namespace ReplayReader
{
    partial class EditorSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Mouse_check = new System.Windows.Forms.CheckBox();
            this.inversion_check = new System.Windows.Forms.CheckBox();
            this.rightBox = new System.Windows.Forms.ComboBox();
            this.leftBox = new System.Windows.Forms.ComboBox();
            this.titleBox = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(70, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Config Editor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Lime;
            this.label2.Location = new System.Drawing.Point(14, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Title:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Lime;
            this.label3.Location = new System.Drawing.Point(14, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Left key:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.ForeColor = System.Drawing.Color.Lime;
            this.label4.Location = new System.Drawing.Point(13, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 3;
            this.label4.Text = "Right key:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.Color.Lime;
            this.label5.Location = new System.Drawing.Point(13, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Use mouse clicks:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.Color.Lime;
            this.label6.Location = new System.Drawing.Point(14, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Inversion:";
            // 
            // Mouse_check
            // 
            this.Mouse_check.AutoSize = true;
            this.Mouse_check.Location = new System.Drawing.Point(140, 152);
            this.Mouse_check.Name = "Mouse_check";
            this.Mouse_check.Size = new System.Drawing.Size(15, 14);
            this.Mouse_check.TabIndex = 6;
            this.Mouse_check.UseVisualStyleBackColor = true;
            // 
            // inversion_check
            // 
            this.inversion_check.AutoSize = true;
            this.inversion_check.Location = new System.Drawing.Point(90, 189);
            this.inversion_check.Name = "inversion_check";
            this.inversion_check.Size = new System.Drawing.Size(15, 14);
            this.inversion_check.TabIndex = 7;
            this.inversion_check.UseVisualStyleBackColor = true;
            // 
            // rightBox
            // 
            this.rightBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rightBox.FormattingEnabled = true;
            this.rightBox.Items.AddRange(new object[] {
            "q",
            "w",
            "e",
            "r",
            "t",
            "y",
            "u",
            "i",
            "o",
            "p",
            "a",
            "s",
            "d",
            "f",
            "g",
            "h",
            "j",
            "k",
            "l",
            "z",
            "x",
            "c",
            "v",
            "b",
            "n",
            "m"});
            this.rightBox.Location = new System.Drawing.Point(90, 112);
            this.rightBox.Name = "rightBox";
            this.rightBox.Size = new System.Drawing.Size(40, 21);
            this.rightBox.TabIndex = 8;
            // 
            // leftBox
            // 
            this.leftBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.leftBox.FormattingEnabled = true;
            this.leftBox.Items.AddRange(new object[] {
            "q",
            "w",
            "e",
            "r",
            "t",
            "y",
            "u",
            "i",
            "o",
            "p",
            "a",
            "s",
            "d",
            "f",
            "g",
            "h",
            "j",
            "k",
            "l",
            "z",
            "x",
            "c",
            "v",
            "b",
            "n",
            "m"});
            this.leftBox.Location = new System.Drawing.Point(83, 76);
            this.leftBox.Name = "leftBox";
            this.leftBox.Size = new System.Drawing.Size(40, 21);
            this.leftBox.TabIndex = 9;
            // 
            // titleBox
            // 
            this.titleBox.Location = new System.Drawing.Point(55, 43);
            this.titleBox.Name = "titleBox";
            this.titleBox.Size = new System.Drawing.Size(195, 20);
            this.titleBox.TabIndex = 10;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(15, 212);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(232, 23);
            this.Save.TabIndex = 11;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // EditorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(262, 251);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.titleBox);
            this.Controls.Add(this.leftBox);
            this.Controls.Add(this.rightBox);
            this.Controls.Add(this.inversion_check);
            this.Controls.Add(this.Mouse_check);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EditorSettings";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EditorSettings_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox Mouse_check;
        private System.Windows.Forms.CheckBox inversion_check;
        private System.Windows.Forms.ComboBox rightBox;
        private System.Windows.Forms.ComboBox leftBox;
        private System.Windows.Forms.TextBox titleBox;
        private System.Windows.Forms.Button Save;
    }
}