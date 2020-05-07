namespace UR5Tool
{
    partial class AxisEndowment
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
            this.components = new System.ComponentModel.Container();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RunBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.baseAngle = new System.Windows.Forms.Label();
            this.wrist3Angle = new System.Windows.Forms.Label();
            this.wrist2Angle = new System.Windows.Forms.Label();
            this.wrist1Angle = new System.Windows.Forms.Label();
            this.shoulderAngle = new System.Windows.Forms.Label();
            this.elbowAngle = new System.Windows.Forms.Label();
            this.shoulderText = new System.Windows.Forms.TextBox();
            this.elbowText = new System.Windows.Forms.TextBox();
            this.w3Text = new System.Windows.Forms.TextBox();
            this.w2Text = new System.Windows.Forms.TextBox();
            this.w1Text = new System.Windows.Forms.TextBox();
            this.baseText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.InfoTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.startAfreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.protectivestop = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.running = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.status = new System.Windows.Forms.Label();
            this.FileText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.OpenAxis = new System.Windows.Forms.OpenFileDialog();
            this.offToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox12.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.baseAngle);
            this.groupBox2.Controls.Add(this.wrist3Angle);
            this.groupBox2.Controls.Add(this.wrist2Angle);
            this.groupBox2.Controls.Add(this.wrist1Angle);
            this.groupBox2.Controls.Add(this.shoulderAngle);
            this.groupBox2.Controls.Add(this.elbowAngle);
            this.groupBox2.Controls.Add(this.shoulderText);
            this.groupBox2.Controls.Add(this.elbowText);
            this.groupBox2.Controls.Add(this.w3Text);
            this.groupBox2.Controls.Add(this.w2Text);
            this.groupBox2.Controls.Add(this.w1Text);
            this.groupBox2.Controls.Add(this.baseText);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(28, 223);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(347, 325);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Six Axis";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RunBtn);
            this.panel1.Location = new System.Drawing.Point(0, 281);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(347, 44);
            this.panel1.TabIndex = 47;
            // 
            // RunBtn
            // 
            this.RunBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RunBtn.Font = new System.Drawing.Font("FZCuHeiSongS-B-GB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunBtn.Location = new System.Drawing.Point(96, 0);
            this.RunBtn.Name = "RunBtn";
            this.RunBtn.Size = new System.Drawing.Size(161, 44);
            this.RunBtn.TabIndex = 31;
            this.RunBtn.Text = "Run";
            this.RunBtn.UseVisualStyleBackColor = true;
            this.RunBtn.Click += new System.EventHandler(this.RunBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(160, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 20);
            this.label3.TabIndex = 46;
            this.label3.Text = "Set";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(257, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 20);
            this.label7.TabIndex = 12;
            this.label7.Text = "Now";
            // 
            // baseAngle
            // 
            this.baseAngle.AutoSize = true;
            this.baseAngle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.baseAngle.Location = new System.Drawing.Point(254, 52);
            this.baseAngle.Name = "baseAngle";
            this.baseAngle.Size = new System.Drawing.Size(45, 20);
            this.baseAngle.TabIndex = 45;
            this.baseAngle.Text = "null";
            // 
            // wrist3Angle
            // 
            this.wrist3Angle.AutoSize = true;
            this.wrist3Angle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.wrist3Angle.Location = new System.Drawing.Point(254, 251);
            this.wrist3Angle.Name = "wrist3Angle";
            this.wrist3Angle.Size = new System.Drawing.Size(45, 20);
            this.wrist3Angle.TabIndex = 44;
            this.wrist3Angle.Text = "null";
            // 
            // wrist2Angle
            // 
            this.wrist2Angle.AutoSize = true;
            this.wrist2Angle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.wrist2Angle.Location = new System.Drawing.Point(254, 211);
            this.wrist2Angle.Name = "wrist2Angle";
            this.wrist2Angle.Size = new System.Drawing.Size(45, 20);
            this.wrist2Angle.TabIndex = 43;
            this.wrist2Angle.Text = "null";
            // 
            // wrist1Angle
            // 
            this.wrist1Angle.AutoSize = true;
            this.wrist1Angle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.wrist1Angle.Location = new System.Drawing.Point(254, 171);
            this.wrist1Angle.Name = "wrist1Angle";
            this.wrist1Angle.Size = new System.Drawing.Size(45, 20);
            this.wrist1Angle.TabIndex = 42;
            this.wrist1Angle.Text = "null";
            // 
            // shoulderAngle
            // 
            this.shoulderAngle.AutoSize = true;
            this.shoulderAngle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.shoulderAngle.Location = new System.Drawing.Point(254, 91);
            this.shoulderAngle.Name = "shoulderAngle";
            this.shoulderAngle.Size = new System.Drawing.Size(45, 20);
            this.shoulderAngle.TabIndex = 40;
            this.shoulderAngle.Text = "null";
            // 
            // elbowAngle
            // 
            this.elbowAngle.AutoSize = true;
            this.elbowAngle.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.elbowAngle.Location = new System.Drawing.Point(254, 131);
            this.elbowAngle.Name = "elbowAngle";
            this.elbowAngle.Size = new System.Drawing.Size(45, 20);
            this.elbowAngle.TabIndex = 41;
            this.elbowAngle.Text = "null";
            // 
            // shoulderText
            // 
            this.shoulderText.Location = new System.Drawing.Point(136, 89);
            this.shoulderText.Name = "shoulderText";
            this.shoulderText.Size = new System.Drawing.Size(86, 22);
            this.shoulderText.TabIndex = 1;
            // 
            // elbowText
            // 
            this.elbowText.Location = new System.Drawing.Point(136, 129);
            this.elbowText.Name = "elbowText";
            this.elbowText.Size = new System.Drawing.Size(86, 22);
            this.elbowText.TabIndex = 2;
            // 
            // w3Text
            // 
            this.w3Text.Location = new System.Drawing.Point(136, 249);
            this.w3Text.Name = "w3Text";
            this.w3Text.Size = new System.Drawing.Size(86, 22);
            this.w3Text.TabIndex = 5;
            // 
            // w2Text
            // 
            this.w2Text.Location = new System.Drawing.Point(136, 209);
            this.w2Text.Name = "w2Text";
            this.w2Text.Size = new System.Drawing.Size(86, 22);
            this.w2Text.TabIndex = 4;
            // 
            // w1Text
            // 
            this.w1Text.Location = new System.Drawing.Point(136, 171);
            this.w1Text.Name = "w1Text";
            this.w1Text.Size = new System.Drawing.Size(86, 22);
            this.w1Text.TabIndex = 3;
            // 
            // baseText
            // 
            this.baseText.Location = new System.Drawing.Point(136, 49);
            this.baseText.Name = "baseText";
            this.baseText.Size = new System.Drawing.Size(86, 22);
            this.baseText.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(18, 251);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 20);
            this.label6.TabIndex = 11;
            this.label6.Text = "Wrist3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(18, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Wrist2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(18, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Wrist1";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label33.ForeColor = System.Drawing.Color.Red;
            this.label33.Location = new System.Drawing.Point(18, 131);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(54, 20);
            this.label33.TabIndex = 8;
            this.label33.Text = "Elbow";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(18, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Shoulder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(18, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Base";
            // 
            // InfoTimer
            // 
            this.InfoTimer.Interval = 2000;
            this.InfoTimer.Tick += new System.EventHandler(this.InfoTimer_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startAfreshToolStripMenuItem,
            this.offToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(405, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // startAfreshToolStripMenuItem
            // 
            this.startAfreshToolStripMenuItem.Name = "startAfreshToolStripMenuItem";
            this.startAfreshToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.startAfreshToolStripMenuItem.Text = "On";
            this.startAfreshToolStripMenuItem.Click += new System.EventHandler(this.startAfreshToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.groupBox12);
            this.groupBox8.Controls.Add(this.groupBox13);
            this.groupBox8.Controls.Add(this.groupBox14);
            this.groupBox8.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.groupBox8.Location = new System.Drawing.Point(71, 68);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox8.Size = new System.Drawing.Size(267, 147);
            this.groupBox8.TabIndex = 30;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Robot Status";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.protectivestop);
            this.groupBox12.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.groupBox12.Location = new System.Drawing.Point(13, 83);
            this.groupBox12.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox12.Size = new System.Drawing.Size(241, 51);
            this.groupBox12.TabIndex = 24;
            this.groupBox12.TabStop = false;
            // 
            // protectivestop
            // 
            this.protectivestop.AutoSize = true;
            this.protectivestop.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.protectivestop.ForeColor = System.Drawing.Color.Black;
            this.protectivestop.Location = new System.Drawing.Point(52, 22);
            this.protectivestop.Name = "protectivestop";
            this.protectivestop.Size = new System.Drawing.Size(118, 21);
            this.protectivestop.TabIndex = 50;
            this.protectivestop.Text = "Function well";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.running);
            this.groupBox13.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.groupBox13.Location = new System.Drawing.Point(134, 20);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox13.Size = new System.Drawing.Size(120, 51);
            this.groupBox13.TabIndex = 22;
            this.groupBox13.TabStop = false;
            // 
            // running
            // 
            this.running.AutoSize = true;
            this.running.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.running.Location = new System.Drawing.Point(12, 22);
            this.running.Name = "running";
            this.running.Size = new System.Drawing.Size(40, 21);
            this.running.TabIndex = 49;
            this.running.Text = "null";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.status);
            this.groupBox14.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.groupBox14.Location = new System.Drawing.Point(13, 20);
            this.groupBox14.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox14.Size = new System.Drawing.Size(105, 51);
            this.groupBox14.TabIndex = 21;
            this.groupBox14.TabStop = false;
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.status.Location = new System.Drawing.Point(9, 22);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(40, 21);
            this.status.TabIndex = 19;
            this.status.Text = "null";
            // 
            // FileText
            // 
            this.FileText.Location = new System.Drawing.Point(164, 36);
            this.FileText.Name = "FileText";
            this.FileText.Size = new System.Drawing.Size(174, 22);
            this.FileText.TabIndex = 48;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 12.75F);
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(67, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 20);
            this.label8.TabIndex = 49;
            this.label8.Text = "File name";
            // 
            // OpenAxis
            // 
            this.OpenAxis.FileName = "openFileDialog1";
            // 
            // offToolStripMenuItem
            // 
            this.offToolStripMenuItem.Name = "offToolStripMenuItem";
            this.offToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.offToolStripMenuItem.Text = "Off";
            this.offToolStripMenuItem.Click += new System.EventHandler(this.offToolStripMenuItem_Click);
            // 
            // AxisEndowment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 572);
            this.Controls.Add(this.FileText);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AxisEndowment";
            this.Text = "AxisEndowment";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AxisEndowment_FormClosing);
            this.Load += new System.EventHandler(this.AxisEndowment_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox shoulderText;
        private System.Windows.Forms.TextBox elbowText;
        private System.Windows.Forms.TextBox w3Text;
        private System.Windows.Forms.TextBox w2Text;
        private System.Windows.Forms.TextBox w1Text;
        private System.Windows.Forms.TextBox baseText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer InfoTimer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startAfreshToolStripMenuItem;
        private System.Windows.Forms.Label baseAngle;
        private System.Windows.Forms.Label wrist3Angle;
        private System.Windows.Forms.Label wrist2Angle;
        private System.Windows.Forms.Label wrist1Angle;
        private System.Windows.Forms.Label shoulderAngle;
        private System.Windows.Forms.Label elbowAngle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label protectivestop;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.Label running;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button RunBtn;
        private System.Windows.Forms.TextBox FileText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.OpenFileDialog OpenAxis;
        private System.Windows.Forms.ToolStripMenuItem offToolStripMenuItem;
    }
}