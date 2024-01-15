namespace CaYaTSKMGR
{
    partial class CaYaDevCommand
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaYaDevCommand));
            button1 = new Button();
            txtCommand = new TextBox();
            lstCommandOutput = new ListBox();
            label1 = new Label();
            Clean = new Button();
            lblCurrentDirectory = new Label();
            Copy = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveBorder;
            button1.Location = new Point(734, 12);
            button1.Name = "button1";
            button1.Size = new Size(96, 25);
            button1.TabIndex = 0;
            button1.Text = "Task Manager";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // txtCommand
            // 
            txtCommand.BackColor = SystemColors.WindowText;
            txtCommand.ForeColor = SystemColors.Window;
            txtCommand.Location = new Point(12, 44);
            txtCommand.Name = "txtCommand";
            txtCommand.Size = new Size(663, 23);
            txtCommand.TabIndex = 1;
            txtCommand.KeyDown += textBox1_KeyDown;
            // 
            // lstCommandOutput
            // 
            lstCommandOutput.BackColor = SystemColors.WindowText;
            lstCommandOutput.ForeColor = SystemColors.Window;
            lstCommandOutput.FormattingEnabled = true;
            lstCommandOutput.HorizontalScrollbar = true;
            lstCommandOutput.ItemHeight = 15;
            lstCommandOutput.Location = new Point(12, 73);
            lstCommandOutput.Name = "lstCommandOutput";
            lstCommandOutput.Size = new Size(818, 394);
            lstCommandOutput.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(767, 55);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 3;
            label1.Text = "CaYa CMD";
            // 
            // Clean
            // 
            Clean.BackColor = Color.CornflowerBlue;
            Clean.ForeColor = Color.OldLace;
            Clean.Location = new Point(12, 15);
            Clean.Name = "Clean";
            Clean.Size = new Size(75, 23);
            Clean.TabIndex = 4;
            Clean.Text = "Clear";
            Clean.UseVisualStyleBackColor = false;
            Clean.Click += Clean_Click;
            // 
            // lblCurrentDirectory
            // 
            lblCurrentDirectory.AutoSize = true;
            lblCurrentDirectory.ForeColor = SystemColors.ButtonHighlight;
            lblCurrentDirectory.Location = new Point(93, 23);
            lblCurrentDirectory.Name = "lblCurrentDirectory";
            lblCurrentDirectory.Size = new Size(108, 15);
            lblCurrentDirectory.TabIndex = 5;
            lblCurrentDirectory.Text = "lblCurrentDirectory";
            // 
            // Copy
            // 
            Copy.BackColor = SystemColors.ButtonHighlight;
            Copy.Location = new Point(600, 12);
            Copy.Name = "Copy";
            Copy.Size = new Size(75, 23);
            Copy.TabIndex = 6;
            Copy.Text = "Copy";
            Copy.UseVisualStyleBackColor = false;
            Copy.Click += Copy_Click;
            // 
            // CaYaDevCommand
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(842, 472);
            Controls.Add(Copy);
            Controls.Add(lblCurrentDirectory);
            Controls.Add(Clean);
            Controls.Add(label1);
            Controls.Add(lstCommandOutput);
            Controls.Add(txtCommand);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CaYaDevCommand";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CaYa DevTool";
            TopMost = true;
            FormClosed += CaYaDevCommand_FormClosed;
            Load += CaYaDevCommand_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox txtCommand;
        private ListBox lstCommandOutput;
        private Label label1;
        private Button Clean;
        private Label lblCurrentDirectory;
        private Button Copy;
    }
}