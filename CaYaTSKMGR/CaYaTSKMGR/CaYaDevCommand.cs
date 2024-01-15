using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaYaTSKMGR
{
    public partial class CaYaDevCommand : Form
    {

        private List<string> commandHistory = new List<string>();
        private int currentHistoryIndex = -1;
        private string currentDirectory = Environment.CurrentDirectory; // Başlangıçta mevcut dizini al

        public CaYaDevCommand()
        {
            InitializeComponent();
            txtCommand.KeyDown += textBox1_KeyDown;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Visible = false;
            CaYaDevTask form1 = new CaYaDevTask();
            form1.Visible = true;
        }

        private void CaYaDevCommand_Load(object sender, EventArgs e)
        {

        }

        private void CaYaDevCommand_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
        }








        private void RunCommand(string command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {command}";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false; // Shell kullanımını devre dışı bırak
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                // Çıktıyı ListBox'a ve geçmişe ekle
                lstCommandOutput.Items.Add($"Command: {command}");
                lstCommandOutput.Items.Add("Output: Running...");

                commandHistory.Add(command);
                currentHistoryIndex = -1;

                UpdateCurrentDirectory(command);
                txtCommand.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error running the command: {ex.Message}", "Error");
            }
        }

        private void UpdateCurrentDirectory(string command)
        {
            // "cd" komutunu içeriyorsa ve ardından bir dizin belirtilmişse
            if (command.StartsWith("cd ") && command.Length > 3)
            {
                string newDirectory = command.Substring(3).Trim();

                // Önceki dizini kaydet
                string previousDirectory = currentDirectory;

                // Yeni dizini mevcut dizin olarak güncelle
                if (Path.IsPathRooted(newDirectory))
                {
                    currentDirectory = newDirectory;
                }
                else
                {
                    currentDirectory = Path.Combine(previousDirectory, newDirectory);
                }

                // Mevcut dizini güncelle ve göster
                Environment.CurrentDirectory = currentDirectory;
                lblCurrentDirectory.Text = $"Current Directory: {currentDirectory}";
            }
        }



        private async void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RunCommand(txtCommand.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
                await Task.Delay(200);
                int itemCount = lstCommandOutput.Items.Count;
                if (itemCount >= 2)
                {
                    lstCommandOutput.Items.RemoveAt(itemCount - 2);
                }
            }
            // Yukarı veya aşağı ok tuşlarına basıldığında geçmişteki komutları dolaş
            else if (e.KeyCode == Keys.Up && currentHistoryIndex < commandHistory.Count - 1)
            {
                currentHistoryIndex++;
                txtCommand.Text = commandHistory[currentHistoryIndex];
            }
            else if (e.KeyCode == Keys.Down && currentHistoryIndex > 0)
            {
                currentHistoryIndex--;
                txtCommand.Text = commandHistory[currentHistoryIndex];
            }

        }

        private void Clean_Click(object sender, EventArgs e)
        {
            lstCommandOutput.Items.Clear();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Join(Environment.NewLine, lstCommandOutput.Items.OfType<string>()));
        }
    }
}
