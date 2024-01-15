namespace CaYaTSKMGR
{
    partial class CaYaDevTask
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaYaDevTask));
            button1 = new Button();
            button3 = new Button();
            lstProcesses = new ListBox();
            txtFilter = new TextBox();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            button2 = new Button();
            pictureBox3 = new PictureBox();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            checkBox1 = new CheckBox();
            label1 = new Label();
            button7 = new Button();
            pictureBox4 = new PictureBox();
            label2 = new Label();
            lstStartupApps = new ListBox();
            checkBox2 = new CheckBox();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            button13 = new Button();
            button14 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(84, 72);
            button1.Name = "button1";
            button1.Size = new Size(212, 23);
            button1.TabIndex = 0;
            button1.Text = "End Task For Background Processes";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnEndTaskWithDependencies_Click;
            // 
            // button3
            // 
            button3.Location = new Point(3, 72);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 2;
            button3.Text = "End Task";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnEndTask_Click;
            // 
            // lstProcesses
            // 
            lstProcesses.FormattingEnabled = true;
            lstProcesses.ItemHeight = 15;
            lstProcesses.Location = new Point(3, 101);
            lstProcesses.Name = "lstProcesses";
            lstProcesses.Size = new Size(605, 679);
            lstProcesses.TabIndex = 3;
            lstProcesses.KeyDown += lstProcesses_KeyDown;
            lstProcesses.MouseDoubleClick += lstProcesses_MouseDoubleClick;
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(302, 73);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(100, 23);
            txtFilter.TabIndex = 4;
            txtFilter.Text = "Search";
            txtFilter.Click += txtFilter_Click;
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.reload__Özel_1;
            pictureBox1.Location = new Point(408, 71);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(30, 30);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.pc2;
            pictureBox2.Location = new Point(3, 14);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(50, 50);
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.TabIndex = 6;
            pictureBox2.TabStop = false;
            // 
            // button2
            // 
            button2.BackColor = Color.IndianRed;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Arial Narrow", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(584, 3);
            button2.Name = "button2";
            button2.Size = new Size(25, 25);
            button2.TabIndex = 7;
            button2.Text = "X";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.LightBlue;
            pictureBox3.Image = Properties.Resources.closeviwe1;
            pictureBox3.Location = new Point(553, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(25, 25);
            pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox3.TabIndex = 9;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(59, 43);
            button4.Name = "button4";
            button4.Size = new Size(156, 23);
            button4.TabIndex = 10;
            button4.Text = "Open Program Folder";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btnOpenFileLocation_Click;
            // 
            // button5
            // 
            button5.Location = new Point(221, 43);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 11;
            button5.Text = "Detail";
            button5.UseVisualStyleBackColor = true;
            button5.Click += lstProcesses_SelectedIndexChanged;
            // 
            // button6
            // 
            button6.BackColor = Color.Chocolate;
            button6.Location = new Point(59, 14);
            button6.Name = "button6";
            button6.Size = new Size(112, 23);
            button6.TabIndex = 12;
            button6.Text = "Program Delete";
            button6.UseVisualStyleBackColor = false;
            button6.Click += btnCloseAndDelete_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.ForeColor = SystemColors.ButtonFace;
            checkBox1.Location = new Point(258, 17);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(72, 19);
            checkBox1.TabIndex = 13;
            checkBox1.Text = "TopMost";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(500, 83);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 14;
            label1.Text = "CaYa Task Manager";
            // 
            // button7
            // 
            button7.BackColor = SystemColors.ControlDark;
            button7.Location = new Point(177, 14);
            button7.Name = "button7";
            button7.Size = new Size(75, 23);
            button7.TabIndex = 15;
            button7.Text = "CMD";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.RunAs_Admin;
            pictureBox4.Location = new Point(580, 60);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(25, 25);
            pictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox4.TabIndex = 16;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Coral;
            label2.Location = new Point(500, 65);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 17;
            label2.Text = "Not Admin";
            // 
            // lstStartupApps
            // 
            lstStartupApps.FormattingEnabled = true;
            lstStartupApps.ItemHeight = 15;
            lstStartupApps.Location = new Point(3, 101);
            lstStartupApps.Name = "lstStartupApps";
            lstStartupApps.Size = new Size(606, 679);
            lstStartupApps.TabIndex = 18;
            lstStartupApps.Visible = false;
            lstStartupApps.SelectedIndexChanged += lstStartupApps_SelectedIndexChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.ForeColor = SystemColors.ButtonFace;
            checkBox2.Location = new Point(302, 46);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(126, 19);
            checkBox2.TabIndex = 19;
            checkBox2.Text = "Show Startup Apps";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // button8
            // 
            button8.Location = new Point(3, 72);
            button8.Name = "button8";
            button8.Size = new Size(102, 23);
            button8.TabIndex = 20;
            button8.Text = "End Startup Task";
            button8.UseVisualStyleBackColor = true;
            button8.Visible = false;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.Location = new Point(111, 72);
            button9.Name = "button9";
            button9.Size = new Size(141, 23);
            button9.TabIndex = 21;
            button9.Text = "End Scheduler Task";
            button9.UseVisualStyleBackColor = true;
            button9.Visible = false;
            button9.Click += button9_Click;
            // 
            // button10
            // 
            button10.BackColor = Color.IndianRed;
            button10.Location = new Point(59, 12);
            button10.Name = "button10";
            button10.Size = new Size(141, 23);
            button10.TabIndex = 22;
            button10.Text = "Delete Scheduler Task";
            button10.UseVisualStyleBackColor = false;
            button10.Visible = false;
            button10.Click += button10_Click;
            // 
            // button11
            // 
            button11.BackColor = Color.IndianRed;
            button11.Location = new Point(59, 42);
            button11.Name = "button11";
            button11.Size = new Size(141, 23);
            button11.TabIndex = 23;
            button11.Text = "Delete Startup Task";
            button11.UseVisualStyleBackColor = false;
            button11.Visible = false;
            button11.Click += button11_Click;
            // 
            // button12
            // 
            button12.BackColor = Color.FromArgb(255, 128, 128);
            button12.Location = new Point(472, 5);
            button12.Name = "button12";
            button12.Size = new Size(75, 23);
            button12.TabIndex = 24;
            button12.Text = "Critical End";
            button12.UseVisualStyleBackColor = false;
            button12.Click += button12_Click;
            // 
            // button13
            // 
            button13.BackColor = Color.SandyBrown;
            button13.Location = new Point(379, 5);
            button13.Name = "button13";
            button13.Size = new Size(87, 23);
            button13.TabIndex = 25;
            button13.Text = "End All Task";
            button13.UseVisualStyleBackColor = false;
            button13.Click += button13_Click;
            // 
            // button14
            // 
            button14.BackColor = Color.Goldenrod;
            button14.Location = new Point(553, 34);
            button14.Name = "button14";
            button14.Size = new Size(55, 23);
            button14.TabIndex = 26;
            button14.Text = "More";
            button14.UseVisualStyleBackColor = false;
            button14.Click += button14_Click;
            // 
            // CaYaDevTask
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(613, 782);
            Controls.Add(button14);
            Controls.Add(button13);
            Controls.Add(button12);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(checkBox2);
            Controls.Add(label2);
            Controls.Add(pictureBox4);
            Controls.Add(button7);
            Controls.Add(label1);
            Controls.Add(checkBox1);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(pictureBox3);
            Controls.Add(button2);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(txtFilter);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(lstStartupApps);
            Controls.Add(lstProcesses);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CaYaDevTask";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CaYa DevTool";
            TopMost = true;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button3;
        private ListBox lstProcesses;
        private TextBox txtFilter;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button button2;
        private PictureBox pictureBox3;
        private Button button4;
        private Button button5;
        private Button button6;
        private CheckBox checkBox1;
        private Label label1;
        private Button button7;
        private PictureBox pictureBox4;
        private Label label2;
        private ListBox lstStartupApps;
        private CheckBox checkBox2;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button13;
        private Button button14;
    }
}