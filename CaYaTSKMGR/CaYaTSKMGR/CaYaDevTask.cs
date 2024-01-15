using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Timers;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using System.ServiceProcess;
using WindowsInput;
using System.Net.NetworkInformation;

namespace CaYaTSKMGR
{
    public partial class CaYaDevTask : Form
    {




        private void CheckStartupApplications()
        {
            lstStartupApps.Items.Clear();
            lstStartupApps.Items.Add("-------------------------AllStartupApplicationsRegedit-------------------------");

            // Yerel Makine (Local Machine) Kay�t Defteri - 64-bit
            string registryPathLocalMachine64Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var startupAppsLocalMachine64Bit = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPathLocalMachine64Bit)?.GetValueNames();
            foreach (var app in startupAppsLocalMachine64Bit)
            {
                lstStartupApps.Items.Add($"Registry Startup (Local Machine 64-bit): {app}");
            }

            // Yerel Makine (Local Machine) Kay�t Defteri - 32-bit
            string registryPathLocalMachine32Bit = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";
            var startupAppsLocalMachine32Bit = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPathLocalMachine32Bit)?.GetValueNames();
            foreach (var app in startupAppsLocalMachine32Bit)
            {
                lstStartupApps.Items.Add($"Registry Startup (Local Machine 32-bit): {app}");
            }

            // Ge�erli Kullan�c� (Current User) Kay�t Defteri - 64-bit
            string registryPathCurrentUser64Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var startupAppsCurrentUser64Bit = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPathCurrentUser64Bit)?.GetValueNames();
            foreach (var app in startupAppsCurrentUser64Bit)
            {
                lstStartupApps.Items.Add($"Registry Startup (Current User 64-bit): {app}");
            }

            // Ge�erli Kullan�c� (Current User) Kay�t Defteri - 32-bit
        }






        private void lstProcesses_KeyDown(object sender, KeyEventArgs e)
        {
            // DELETE tu�una bas�ld���nda se�ili i�lemi sonland�r
            if (e.KeyCode == Keys.Delete)
            {
                EndSelectedProcess();
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // DELETE tu�una bas�ld���nda se�ili i�lemi sonland�r
            if (e.KeyCode == Keys.Delete)
            {
                EndSelectedProcess();
            }
        }



        private void txtFilter_Click(object sender, EventArgs e)
        {
            txtFilter.Clear();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ListRunningProcesses();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Run();


            if (IsRunAsAdmin())
            {
                label2.Text = "Administrator";
                label2.ForeColor = Color.Orange;
                pictureBox4.Visible = false;
                label2.Location = new Point(528, 65);
            }
        }

        private async void Run()
        {
            await System.Threading.Tasks.Task.Delay(100);
            ListRunningProcesses();
        }

        public CaYaDevTask()
        {
            InitializeComponent();


            ListRunningProcesses();
            CheckStartupApplications();
            CheckTaskSchedulerApplications();

            // Form y�klenirken renkleri ayarla
            SetFormColors();

            // ListBox ��elerini �izmek i�in etkinle�tir
            lstProcesses.DrawMode = DrawMode.OwnerDrawFixed;

            // ListBox ��elerini �izecek olan olay� tan�mla
            lstProcesses.DrawItem += new DrawItemEventHandler(lstProcesses_DrawItem);

        }

        private void SetFormColors()
        {



        }

        private void ListRunningProcesses()
        {
            Process[] processes = Process.GetProcesses();

            lstProcesses.Items.Clear();

            foreach (Process process in processes)
            {
                bool isMainProcess = IsMainProcess(process);
                bool isSystemProcess = IsSystemProcess(process);

                string mainProcessInfo = isMainProcess ? " (Main Process)" : string.Empty;

                // Ana s�re�lerin adlar�n� a��k mavi, sistem s�re�lerini k�rm�z� olarak, di�er s�re�leri siyah olarak g�ster
                Color textColor = isMainProcess ? Color.Blue : (isSystemProcess ? Color.Red : Color.Black);

                // ListBox ��esine eklenecek metni olu�tur
                string processInfo = $"{process.ProcessName} (ID: {process.Id}){mainProcessInfo}";

                // S�recin mevcut pencere ba�l���n� al
                string windowTitle = GetProcessWindowTitle(process);

                // E�er mevcut pencere ba�l��� varsa, ListBox ��esine ekle
                if (!string.IsNullOrEmpty(windowTitle))
                {
                    processInfo += $" [{windowTitle}]";
                }

                lstProcesses.Items.Add(new ListBoxItem(processInfo, textColor));
            }
        }

        private string GetProcessWindowTitle(Process process)
        {
            try
            {
                // E�er s�recin bir ana penceresi varsa, pencere ba�l���n� al
                return process.MainWindowTitle;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void btnEndTask_Click(object sender, EventArgs e)
        {
            EndSelectedProcess();
        }

        private void btnEndTaskWithDependencies_Click(object sender, EventArgs e)
        {
            EndSelectedProcessWithDependencies();
        }

        private void EndSelectedProcess()
        {
            if (lstProcesses.SelectedIndex != -1)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            Process process = Process.GetProcessById(processId);
                            process.Kill();
                            MessageBox.Show($"Process {process.ProcessName} (ID: {processId}) has been terminated.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error terminating the process: {ex.Message}");
                        }

                        ListRunningProcesses();
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a process to terminate.");
            }
        }

        private void EndSelectedProcessWithDependencies()
        {
            if (lstProcesses.SelectedItem != null)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            // Se�ilen s�recin ba��ml�l�klar�n� al
                            List<int> dependencies = GetProcessDependencies(processId);

                            // Se�ilen s�reci sonland�r
                            Process process = Process.GetProcessById(processId);
                            process.Kill();
                            MessageBox.Show($"Process {process.ProcessName} (ID: {processId}) has been terminated.");

                            // Ba��ml�l�klar� sonland�r
                            foreach (int dependencyId in dependencies)
                            {
                                try
                                {
                                    Process dependencyProcess = Process.GetProcessById(dependencyId);
                                    dependencyProcess.Kill();
                                    MessageBox.Show($"Process {dependencyProcess.ProcessName} (ID: {dependencyId}) has been terminated.");
                                }
                                catch (Exception ex)
                                {
                                    // Ba��ml�l��� sonland�rmada hata olursa, devam et
                                    MessageBox.Show($"Error terminating dependency process (ID: {dependencyId}): {ex.Message}");
                                }
                            }

                            // Ba�latan uygulamay� sonland�r
                            EndParentApplication(processId);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error terminating the process and its dependencies: {ex.Message}");
                        }

                        ListRunningProcesses();
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a process to terminate with dependencies.");
            }
        }

        private void EndParentApplication(int processId)
        {
            // Ba�latan uygulaman�n ID'sini al
            int parentProcessId = GetParentProcessId(processId);

            if (parentProcessId != -1)
            {
                try
                {
                    Process parentProcess = Process.GetProcessById(parentProcessId);
                    parentProcess.Kill();
                    MessageBox.Show($"Parent process {parentProcess.ProcessName} (ID: {parentProcessId}) has been terminated.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error terminating the parent process: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Unable to find parent process.");
            }
        }


        private List<int> GetProcessDependencies(int processId)
        {
            List<int> dependencies = new List<int>();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ParentProcessId={processId}"))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    int dependentProcessId = Convert.ToInt32(mo["ProcessId"]);
                    dependencies.Add(dependentProcessId);
                }
            }

            return dependencies;
        }

        private int GetParentProcessId(int processId)
        {
            try
            {
                int parentProcessId;
                Process process = Process.GetProcessById(processId);
                GetParentProcessId(process.Handle, out parentProcessId);
                return parentProcessId;
            }
            catch
            {
                return -1;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetParentProcessId(IntPtr handle, out int parentProcessId);

        private bool IsMainProcess(Process process)
        {
            try
            {
                // E�er ba�lat�c� (ana) s�recin bir ba�lant� noktas� varsa, bu s�reci ana s�re� olarak kabul ederiz.
                return process.MainWindowHandle != IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        }

        private bool IsSystemProcess(Process process)
        {
            try
            {
                // E�er s�recin �nceden belirlenmi� bir sistem s�reci olup olmad���n� kontrol et
                return process.ProcessName.ToLowerInvariant() == "svchost" || process.ProcessName.ToLowerInvariant() == "system";
            }
            catch
            {
                return false;
            }
        }

        private void FilterProcessesByKeyword(string keyword)
        {
            Process[] processes = Process.GetProcesses();

            lstProcesses.Items.Clear();

            foreach (Process process in processes)
            {
                bool isMainProcess = IsMainProcess(process);
                bool isSystemProcess = IsSystemProcess(process);
                string mainProcessInfo = isMainProcess ? " (Main Process)" : string.Empty;

                // Ana s�re�lerin adlar�n� a��k mavi, sistem s�re�lerini k�rm�z� olarak, di�er s�re�leri siyah olarak g�ster
                Color textColor = isMainProcess ? Color.Blue : (isSystemProcess ? Color.Red : Color.Black);

                // ListBox ��esine eklenecek metni olu�tur
                string processInfo = $"{process.ProcessName} (ID: {process.Id}){mainProcessInfo}";

                // S�recin mevcut pencere ba�l���n� al
                string windowTitle = GetProcessWindowTitle(process);

                // E�er mevcut pencere ba�l��� varsa, ListBox ��esine ekle
                if (!string.IsNullOrEmpty(windowTitle))
                {
                    processInfo += $" [{windowTitle}]";
                }

                // TextBox'tan al�nan kelime ile uygulama ad�n� kar��la�t�r
                if (string.IsNullOrEmpty(keyword) || processInfo.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    lstProcesses.Items.Add(new ListBoxItem(processInfo, textColor));
                }
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            // TextBox i�eri�i de�i�ti�inde uygulamalar� filtrele
            FilterProcessesByKeyword(txtFilter.Text);
        }

        private void lstProcesses_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ListBoxItem item = lstProcesses.Items[e.Index] as ListBoxItem;

            if (item != null)
            {
                e.DrawBackground();
                e.Graphics.DrawString(item.Text, lstProcesses.Font, new SolidBrush(item.ForeColor), e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                e.DrawBackground();
                e.Graphics.DrawString(lstProcesses.Items[e.Index].ToString(), lstProcesses.Font, Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private class ListBoxItem
        {
            public string Text { get; set; }
            public Color ForeColor { get; set; }

            public ListBoxItem(string text, Color foreColor)
            {
                Text = text;
                ForeColor = foreColor;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        bool mousedown;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                int mousex = MousePosition.X - 306;
                int mousey = MousePosition.Y - 20;
                this.SetDesktopLocation(mousex, mousey);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        private void btnOpenFileLocation_Click(object sender, EventArgs e)
        {
            OpenFileLocation();
        }




        private void ShowProcessDetails()
        {
            if (lstProcesses.SelectedIndex != -1)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            Process process = Process.GetProcessById(processId);

                            // ��lemin dosya konumunu al
                            string processFilePath = process.MainModule.FileName;

                            // Dosya konumunu g�ster
                            MessageBox.Show($"Process: {process.ProcessName}\nProcess ID: {processId}\nFile Path: {processFilePath}", "Process Details");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error getting process details: {ex.Message}", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
        }

        private void OpenFileLocation()
        {
            if (lstProcesses.SelectedIndex != -1)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            Process process = Process.GetProcessById(processId);

                            // ��lemin dosya konumunu al
                            string processFilePath = process.MainModule.FileName;

                            // Dosya konumunu a�
                            Process.Start("explorer.exe", $"/select,\"{processFilePath}\"");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error opening file location: {ex.Message}", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
        }

        private void lstProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ShowProcessDetails();
            if (lstProcesses.SelectedItem != null)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        ShowProcessDetails(processId);
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
        }

        private void btnCloseAndDelete_Click(object sender, EventArgs e)
        {
            // ListBox'ta se�ilen ��eyi al
            if (lstProcesses.SelectedItem != null)
            {
                // Se�ilen ��e bir string oldu�undan, bunu bir s�re� ID'sine d�n��t�rmelisiniz.
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            Process process = Process.GetProcessById(processId);
                            // DeleteProcessFile metodunu �a��r
                            DeleteProcessFile(process);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error getting process details: {ex.Message}", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a process to delete.", "Error");
            }
        }





        private string tempFilePathToDelete;

        private void DeleteProcessFile(Process process)
        {
            try
            {
                string processFilePath = process.MainModule.FileName;

                // Dosyan�n var olup olmad���n� kontrol et
                if (File.Exists(processFilePath))
                {
                    // Dosyay� silme i�lemi s�ras�nda dosya yolunu ge�ici bir de�i�kende sakla
                    tempFilePathToDelete = processFilePath;

                    // S�reci sonland�r
                    process.Kill();
                    process.WaitForExit(); // S�recin tamamen sonlanmas�n� bekleyin
                                           // Kullan�c�ya dosyay� silip silmemek istedi�ini sor
                    if (MessageBox.Show($"Do you want to delete the file associated with {tempFilePathToDelete}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Dosyay� sil
                        File.Delete(tempFilePathToDelete);
                        MessageBox.Show($"File {tempFilePathToDelete} has been deleted.", "File Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Ge�ici dosya yolunu temizle
                        tempFilePathToDelete = null;
                    }
                    else
                    {
                        MessageBox.Show("Deleteing UnSuccessful");
                    }

                }
                else
                {
                    MessageBox.Show($"File {processFilePath} does not exist.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting the file: {ex.Message}", "Error");
            }
        }


        private void PromptAndEndSelectedProcess()
        {
            if (lstProcesses.SelectedIndex != -1)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        try
                        {
                            Process process = Process.GetProcessById(processId);
                            string processName = process.ProcessName;

                            // ��lemi sonland�r
                            process.Kill();
                            MessageBox.Show($"Process {processName} (ID: {processId}) has been terminated.");

                            // Kullan�c�ya dosyay� silip silmemek istedi�ini sor
                            if (MessageBox.Show($"Do you want to delete the file associated with {processName}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // Dosyay� sil
                                DeleteProcessFile(process);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error terminating the process: {ex.Message}");
                        }

                        ListRunningProcesses();
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a process to terminate.");
            }
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Visible = false;
            CaYaDevCommand cmd = new CaYaDevCommand();
            cmd.ShowDialog();
        }





        private void RunUac()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Verb = "runas", // Uygulamay� y�netici olarak ba�lat
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                Application.Exit(); // �u anki uygulamay� kapat
            }
            catch (Exception ex)
            {
                //MessageBox.Show("UYARI! ��inize Devam Etmek ��in Onaylaman�z Gerekmektedir.");
            }
        }

        private bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (!IsRunAsAdmin())
            {
                RunUac();
                return; // ��lemi sonland�r
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                lstStartupApps.Visible = true;
                button8.Visible = true;
                button9.Visible = true;
                button10.Visible = true;
                button11.Visible = true;
                button13.Visible = false;
                button12.Visible = false;
                button3.Visible = false;
                button1.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                txtFilter.Visible = false;
                pictureBox1.Visible = false;
            }
            else
            {
                lstStartupApps.Visible = false;
                button8.Visible = false;
                button9.Visible = false;
                button10.Visible = false;
                button11.Visible = false;
                button13.Visible = true;
                button12.Visible = true;
                button3.Visible = true;
                button1.Visible = true;
                button4.Visible = true;
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                txtFilter.Visible = true;
                pictureBox1.Visible = true;
            }
        }

        private void CheckTaskSchedulerApplications()
        {
            //lstStartupApps.Items.Clear();
            lstStartupApps.Items.Add("-------------------------TaskSchedulerApplications-------------------------");
            using (TaskService taskService = new TaskService())
            {
                foreach (var task in taskService.AllTasks)
                {
                    lstStartupApps.Items.Add($"Task Scheduler: {task.Path}");
                }
            }
        }


        private void EndStartupTask()
        {
            if (lstStartupApps.SelectedItem != null)
            {
                string selectedStartupApp = lstStartupApps.SelectedItem.ToString();

                // Se�ilen ba�lang�� uygulamas�n�n ad�n� alma
                string startupAppName;

                if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 32-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Local Machine 32-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 64-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Local Machine 64-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Current User 32-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Current User 32-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Current User 64-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Current User 64-bit): ".Length);
                }
                else
                {
                    MessageBox.Show("Invalid startup application selected.", "Error");
                    return;
                }

                try
                {
                    // Ba�lang�� uygulamas�n�n exe dosyas�n�n yolunu kay�t defterinden al
                    string registryPathLocalMachine64Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                    string registryPathLocalMachine32Bit = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";
                    string registryPathCurrentUser64Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                    string registryPathCurrentUser32Bit = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run";

                    string exePath = null;

                    if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 64-bit):"))
                    {
                        exePath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPathLocalMachine64Bit)?.GetValue(startupAppName)?.ToString();
                    }
                    else if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 32-bit):"))
                    {
                        exePath = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPathLocalMachine32Bit)?.GetValue(startupAppName)?.ToString();
                    }
                    else if (selectedStartupApp.StartsWith("Registry Startup (Current User 64-bit):"))
                    {
                        exePath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPathCurrentUser64Bit)?.GetValue(startupAppName)?.ToString();
                    }
                    else if (selectedStartupApp.StartsWith("Registry Startup (Current User 32-bit):"))
                    {
                        exePath = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPathCurrentUser32Bit)?.GetValue(startupAppName)?.ToString();
                    }

                    if (!string.IsNullOrEmpty(exePath))
                    {
                        // Ba�lang�� uygulamas�n� sonland�rmak i�in exe dosyas�n� bul ve sonland�r
                        Process[] processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(exePath));

                        foreach (var process in processes)
                        {
                            process.Kill();
                        }

                        MessageBox.Show($"Startup application '{startupAppName}' terminated successfully.", "Success");
                    }
                    else
                    {
                        MessageBox.Show($"Could not find the path for startup application '{startupAppName}'.", "Error");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error terminating startup application: {ex.Message}", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a startup application to terminate.", "Error");
            }
        }













        private void button8_Click(object sender, EventArgs e)
        {
            EndStartupTask();
        }


        private void EndTaskSchedulerTask()
        {
            if (lstStartupApps.SelectedItem != null)
            {
                string selectedTaskSchedulerTask = lstStartupApps.SelectedItem.ToString();

                // Se�ilen g�rev zamanlay�c�s� g�revinin ad�n� alma
                string taskSchedulerTaskName = selectedTaskSchedulerTask.Substring("Task Scheduler: ".Length);

                // G�rev zamanlay�c�s� g�revini sonland�rma
                try
                {
                    // G�rev zamanlay�c�s� g�revini sonland�rmak i�in g�rev ad�n� kullanabilirsiniz.
                    using (TaskService taskService = new TaskService())
                    {
                        var task = taskService.AllTasks.FirstOrDefault(t => t.Path == taskSchedulerTaskName);

                        if (task != null)
                        {
                            task.Stop();
                            MessageBox.Show($"Task Scheduler task '{taskSchedulerTaskName}' terminated successfully.", "Success");
                        }
                        else
                        {
                            MessageBox.Show($"Task Scheduler task '{taskSchedulerTaskName}' not found.", "Error");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error terminating Task Scheduler task: {ex.Message}", "Error");
                }

                // Yeniden kontrol etmek ve g�ncellemek i�in g�rev zamanlay�c�s� g�revlerini listele
                //CheckTaskSchedulerApplications();
            }
            else
            {
                MessageBox.Show("Please select a Task Scheduler task to terminate.", "Error");
            }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            EndTaskSchedulerTask();
        }


        private void DeleteStartupTask()
        {
            if (lstStartupApps.SelectedItem != null)
            {
                string selectedStartupApp = lstStartupApps.SelectedItem.ToString();

                // Se�ilen ba�lang�� uygulamas�n�n ad�n� alma
                string startupAppName;

                if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 32-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Local Machine 32-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 64-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Local Machine 64-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Current User 32-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Current User 32-bit): ".Length);
                }
                else if (selectedStartupApp.StartsWith("Registry Startup (Current User 64-bit):"))
                {
                    startupAppName = selectedStartupApp.Substring("Registry Startup (Current User 64-bit): ".Length);
                }
                else
                {
                    MessageBox.Show("Invalid startup application selected.", "Error");
                    return;
                }

                try
                {
                    // Ba�lang�� uygulamas�n� silmek i�in uygulama ad�n� kullanabilirsiniz.
                    Microsoft.Win32.RegistryKey registryKey64BitLocalMachine = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    Microsoft.Win32.RegistryKey registryKey32BitLocalMachine = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run", true);
                    Microsoft.Win32.RegistryKey registryKey64BitCurrentUser = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                    Microsoft.Win32.RegistryKey registryKey32BitCurrentUser = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run", true);

                    // E�er 64-bit kay�t defterinde bulunamazsa 32-bit kay�t defterinde ara
                    if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 64-bit):"))
                    {
                        if (registryKey64BitLocalMachine == null)
                        {
                            registryKey64BitLocalMachine = registryKey32BitLocalMachine;
                        }
                    }
                    else if (selectedStartupApp.StartsWith("Registry Startup (Current User 64-bit):"))
                    {
                        if (registryKey64BitCurrentUser == null)
                        {
                            registryKey64BitCurrentUser = registryKey32BitCurrentUser;
                        }
                    }

                    if (MessageBox.Show($"Do you want to delete the registry entry for {startupAppName}?", "Delete Registry Entry", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 64-bit):"))
                        {
                            registryKey64BitLocalMachine?.DeleteValue(startupAppName, false);
                        }
                        else if (selectedStartupApp.StartsWith("Registry Startup (Local Machine 32-bit):"))
                        {
                            registryKey32BitLocalMachine?.DeleteValue(startupAppName, false);
                        }
                        else if (selectedStartupApp.StartsWith("Registry Startup (Current User 64-bit):"))
                        {
                            registryKey64BitCurrentUser?.DeleteValue(startupAppName, false);
                        }
                        else if (selectedStartupApp.StartsWith("Registry Startup (Current User 32-bit):"))
                        {
                            registryKey32BitCurrentUser?.DeleteValue(startupAppName, false);
                        }

                        MessageBox.Show($"Startup application '{startupAppName}' deleted successfully.", "Success");

                        // Yeniden kontrol etmek ve g�ncellemek i�in ba�lang�� uygulamalar�n� listele
                    }
                    else
                    {
                        MessageBox.Show("Deleting Unsuccessful");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting startup application: {ex.Message}", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a startup application to delete.", "Error");
            }
        }



        private void DeleteTaskSchedulerTask()
        {
            if (lstStartupApps.SelectedItem != null)
            {
                string selectedTaskSchedulerTask = lstStartupApps.SelectedItem.ToString();

                // Se�ilen g�rev zamanlay�c�s� g�revinin ad�n� alma
                string taskSchedulerTaskName = selectedTaskSchedulerTask.Substring("Task Scheduler: ".Length);

                // G�rev zamanlay�c�s� g�revini silme
                try
                {
                    // G�rev zamanlay�c�s� g�revini silmek i�in g�rev ad�n� kullanabilirsiniz.
                    using (TaskService taskService = new TaskService())
                    {
                        var task = taskService.AllTasks.FirstOrDefault(t => t.Path == taskSchedulerTaskName);
                        if (MessageBox.Show($"Do you want to delete the file associated with {tempFilePathToDelete}?", "Delete File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (task != null)
                            {
                                taskService.RootFolder.DeleteTask(task.Name, false);
                                MessageBox.Show($"Task Scheduler task '{taskSchedulerTaskName}' deleted successfully.", "Success");

                                // Yeniden kontrol etmek ve g�ncellemek i�in g�rev zamanlay�c�s� g�revlerini listele
                                //CheckTaskSchedulerApplications();
                            }
                            else
                            {
                                MessageBox.Show($"Task Scheduler task '{taskSchedulerTaskName}' not found.", "Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Deleteing UnSuccessful");
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting Task Scheduler task: {ex.Message}", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a Task Scheduler task to delete.", "Error");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DeleteTaskSchedulerTask();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DeleteStartupTask();
        }

















        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetInformationJobObject(IntPtr hJob, int JobObjectInfoClass, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        const int JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;
        const int JOB_OBJECT_LIMIT_NETWORK_JOB_LIMIT = 0x00001000;

        public static void ApplyIoRestrictions(int processId)
        {
            try
            {
                IntPtr hJob = CreateJobObject(IntPtr.Zero, null);

                if (hJob == IntPtr.Zero)
                {
                    throw new Exception("Failed to create job object.");
                }

                var jobInfo = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                {
                    LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE | JOB_OBJECT_LIMIT_NETWORK_JOB_LIMIT
                };

                var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
                {
                    BasicLimitInformation = jobInfo
                };

                int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
                IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
                Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

                if (!SetInformationJobObject(hJob, 9, extendedInfoPtr, (uint)length))
                {
                    throw new Exception("Failed to set information on job object.");
                }

                Process process = Process.GetProcessById(processId);
                if (!AssignProcessToJobObject(hJob, process.Handle))
                {
                    throw new Exception("Failed to assign process to job object.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying I/O restrictions: {ex.Message}");
            }
        }

        public static void RemoveIoRestrictions(int processId)
        {
            // �lgili i�lemin I/O k�s�tlamalar�n� kald�rma
            // Bu b�l�m� i�letim sistemine ve ihtiyac�n�za ba�l� olarak �zelle�tirebilirsiniz
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // �rnek olarak se�ilen ListBox ��esinden processId al�n�yor.
            if (lstProcesses.SelectedItem != null)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        ApplyIoRestrictions(processId);
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a process to apply I/O restrictions.", "Error");
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public int LimitFlags;
            public IntPtr MinimumWorkingSetSize;
            public IntPtr MaximumWorkingSetSize;
            public int ActiveProcessLimit;
            public IntPtr Affinity;
            public int PriorityClass;
            public int SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public IntPtr ProcessMemoryLimit;
            public IntPtr JobMemoryLimit;
            public IntPtr PeakProcessMemoryUsed;
            public IntPtr PeakJobMemoryUsed;
        }



        private void lstProcesses_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstProcesses.SelectedItem != null)
            {
                string selectedProcess = lstProcesses.SelectedItem.ToString();
                int startIndex = selectedProcess.IndexOf("(ID: ") + "(ID: ".Length;
                int endIndex = selectedProcess.IndexOf(")", startIndex);

                if (startIndex != -1 && endIndex != -1)
                {
                    string processIdStr = selectedProcess.Substring(startIndex, endIndex - startIndex);
                    int processId;

                    if (int.TryParse(processIdStr, out processId))
                    {
                        ShowProcessDetails(processId);
                    }
                    else
                    {
                        MessageBox.Show("Invalid process ID.", "Error");
                    }
                }
            }
        }

        private void ShowProcessDetails(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
                PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName);

                // �lk de�erleri almak i�in bir s�re bekleyin
                cpuCounter.NextValue();
                ramCounter.NextValue();
                System.Threading.Thread.Sleep(1000);  // 1 saniye bekleyin, bu s�reyi ihtiyaca g�re ayarlayabilirsiniz

                // Di�er performans saya�lar�n� ekleyebilirsiniz (disk I/O, vb.)

                // �lgili bilgileri kullan�c� aray�z�ne g�ster
                MessageBox.Show($"Process Name: {process.ProcessName}\n" +
                                $"CPU Usage: {cpuCounter.NextValue()}%\n" +
                                $"RAM Usage: {ramCounter.NextValue() / (1024 * 1024):N2} MB\n" +  // Byte cinsinden RAM kullan�m�n� MB'ye �evirir
                                $"Start Time: {process.StartTime}\n" +
                                $"File Path: {GetProcessFilePath(processId)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing process details: {ex.Message}", "Error");
            }
        }



        private string GetProcessFilePath(int processId)
        {
            try
            {
                string filePath = Process.GetProcessById(processId).MainModule.FileName;
                return filePath;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }






        private const uint PROCESS_TERMINATE = 0x0001;

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private static bool TryTerminateProcess(Process process)
        {
            try
            {
                // Try to close the main window first
                if (!process.CloseMainWindow())
                {
                    // If the main window doesn't respond, forcefully terminate
                    IntPtr processHandle = OpenProcess(PROCESS_TERMINATE, false, process.Id);

                    if (processHandle == IntPtr.Zero)
                    {
                        Console.WriteLine($"Failed to open process {process.ProcessName} with error: {Marshal.GetLastWin32Error()}");
                        return false;
                    }

                    if (!TerminateProcess(processHandle, 0))
                    {
                        Console.WriteLine($"Failed to terminate process {process.ProcessName} with error: {Marshal.GetLastWin32Error()}");
                        return false;
                    }
                }

                Console.WriteLine($"Terminated process: {process.ProcessName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error terminating process {process.ProcessName}: {ex.Message}");
                return false;
            }
        }




        //private static string[] essentialSystemServices = { "lsass", "wininit", "services", "csrss", "smss", "svchost", "winlogon", "explorer", "spoolsv", "lsaiso", "dwm", "taskhostw", "taskeng", "lsass", "lsm", "msiexec", "sihost", "audiodg", "ctfmon", "logonui", "wudfhost", "dllhost", "wmpnetwk", "fontdrvhost", "conhost", "searchprotocolhost", "searchfilterhost" };


        //private static string[] essentialSystemServices = {
        //"lsass", "wininit", "services", "csrss", "smss", "svchost", "winlogon",
        //"explorer", "spoolsv", "lsaiso", "dwm", "taskhostw", "taskeng", "lsass",
        //"lsm", "msiexec", "sihost", "audiodg", "ctfmon", "logonui", "wudfhost",
        //"dllhost", "wmpnetwk", "fontdrvhost", "conhost", "searchprotocolhost",
        //"searchfilterhost", "System", "Registry", "RuntimeBroker", "WmiPrvSE",
        //"csrss", "lsass", "smss", "wininit", "winlogon", "WmiPrvSE", "wininit",
        //"spoolsv", "svchost", "winlogon", "wininit", "lsass", "dwm", "svchost",
        //"explorer", "ctfmon", "SearchUI", "SearchIndexer", "MicrosoftEdge",
        //"MicrosoftEdgeCP", "SecurityHealthService", "RuntimeBroker", "SearchApp",
        //"TextInputHost", "ShellExperienceHost", "taskhostw", "backgroundTaskHost",
        //"LockApp", "Microsoft.Photos", "Microsoft.Windows.Cortana", "OneDrive",
        //"SystemSettings", "NVIDIA Share", "NVIDIA Web Helper", "audiodg", "sihost",
        //"fontdrvhost", "SecurityHealthSystray", "dllhost", "GameBar", "GameBarFTServer",
        //"MicrosoftEdgeUpdate", "MicrosoftEdgeElevationService", "MSASCuiL",
        //"SearchFilterHost", "SearchProtocolHost", "OfficeClickToRun", "RuntimeBroker",
        //"WmiPrvSE", "ShellExperienceHost", "taskhostw", "backgroundTaskHost", "SearchApp",
        //"TextInputHost", "LockApp", "Microsoft.Photos", "Microsoft.Windows.Cortana",
        //"OneDrive", "SystemSettings", "NVIDIA Share", "NVIDIA Web Helper", "audiodg",
        //"sihost", "fontdrvhost", "SecurityHealthSystray", "dllhost", "GameBar",
        //"GameBarFTServer", "MicrosoftEdgeUpdate", "MicrosoftEdgeElevationService",
        //"MSASCuiL", "SearchFilterHost", "SearchProtocolHost", "OfficeClickToRun",
        //"WindowsShellExperienceHost", "RuntimeBroker", "SecurityHealthService",
        //"ShellExperienceHost", "SystemSettings", "NVIDIA Share", "NVIDIA Web Helper",
        //"audiodg", "sihost", "fontdrvhost", "SecurityHealthSystray", "dllhost",
        //"GameBar", "GameBarFTServer", "MicrosoftEdgeUpdate",
        //"MicrosoftEdgeElevationService", "MSASCuiL", "SearchFilterHost",
        //"SearchProtocolHost", "OfficeClickToRun", "RuntimeBroker", "WmiPrvSE",
        //"ShellExperienceHost", "taskhostw", "backgroundTaskHost", "SearchApp",
        //"TextInputHost", "LockApp", "Microsoft.Photos", "Microsoft.Windows.Cortana",
        //"OneDrive", "SystemSettings", "NVIDIA Share", "NVIDIA Web Helper", "audiodg",
        //"sihost", "fontdrvhost", "SecurityHealthSystray", "dllhost", "GameBar",
        //"GameBarFTServer", "MicrosoftEdgeUpdate", "MicrosoftEdgeElevationService",
        //"MSASCuiL", "SearchFilterHost", "SearchProtocolHost", "OfficeClickToRun"
        //};

        //private static string[] essentialSystemServices = { "lsass", "csrss", "smss", "svchost", "winlogon", "dwm", "fontdrvhost", "RuntimeBroker", "explorer", "Idle", "System", "Registry", "wininit", "services", "memory compression", "msmpenq", "Nissvr", "sihost", "wmiprvse", "dllhost", "searchIndexer", "UserOOBEBroker", "FileCoAuth", "widgets", "ctfmon" };

        private static string[] essentialSystemServices = { "lsass", "csrss", "smss", "svchost", "winlogon", "dwm", "fontdrvhost", "RuntimeBroker", "explorer" };


        private async void CloseEssentialSystemServices()
        {
            const int maxAttempts = 3; // You can adjust the number of attempts as needed
            const int delayBetweenAttempts = 2000; // Delay in milliseconds between attempts

            try
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    Process[] processes = Process.GetProcesses();

                    int currentProcessId = Process.GetCurrentProcess().Id;

                    foreach (Process process in processes)
                    {
                        // E�er i�lem kendi i�leminizse, kapatma i�lemini es ge�
                        if (process.Id == currentProcessId)
                        {
                            continue;
                        }

                        string processName = process.ProcessName.ToLower();

                        if (essentialSystemServices.Any(service => processName.Contains(service)))
                        {
                            // Temel sistem hizmetlerini kapatma
                            continue;
                        }

                        if (!TryTerminateProcess(process))
                        {
                            // E�er terminasyon ba�ar�s�z olursa, bu durumu rapor edebilir veya i�lemi es ge�ebilirsiniz.
                            Console.WriteLine($"Skipped terminating process {process.ProcessName}");
                        }
                    }

                    await Task.Delay(delayBetweenAttempts);

                    // Close Explorer after waiting
                    try
                    {
                        //foreach (var process in Process.GetProcessesByName("explorer"))
                        //{
                            //process.Kill();
                        //}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Recheck for remaining processes
                    if (Process.GetProcesses().Any(process => !essentialSystemServices.Any(service => process.ProcessName.ToLower().Contains(service))))
                    {
                        // There are still open processes after the attempt
                        if (attempt == maxAttempts)
                        {
                            MessageBox.Show("Some processes could not be closed.", "Warning");
                        }
                    }
                    else
                    {
                        // All processes are closed
                        MessageBox.Show("All processes except Basic System Services are closed.", "Success");
                        break; // Exit the loop if all processes are closed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing All processes except Basic System Services: {ex.Message}", "Error");
            }
        }



        private void button13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"CAUTION! If you are using the Windows Insider version, your computer may crash. WARNING! Are you sure you want to close all non-system applications?", "close all non-system applications", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (MessageBox.Show($"LAST WARNING! Are you sure you want to close all non-system applications? There is a possibility of your computer crashing.", "WARNING! ACTION REQUIRED!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    CloseEssentialSystemServices();
                }
                else
                {
                    MessageBox.Show("Operation canceled.");
                }
            }
            else
            {
                MessageBox.Show("Operation canceled.");
            }
        }



























        public static void StopNonSystemServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                // Sistem ve �zel hizmetleri filtreleme
                var nonSystemServices = services.Where(service => !IsSystemService(service.ServiceName)).ToList();

                Parallel.ForEach(nonSystemServices, service =>
                {
                    try
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                            Console.WriteLine($"Stopped service: {service.ServiceName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error stopping service {service.ServiceName}: {ex.Message}");
                    }
                });

                Console.WriteLine("Non-system services stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping non-system services: {ex.Message}");
            }
        }

        // Verilen hizmet ad�n�n sistem hizmeti olup olmad���n� kontrol eden fonksiyon
        private static bool IsSystemService(string serviceName)
        {
            // Sistem hizmeti adlar�n� buraya ekleyin
            string[] systemServiceNames = {
            "wuauserv",   // Windows Update hizmeti
            "spooler",    // Yazd�rma kuyru�u hizmeti
            "winmgmt",    // Windows Y�netim Enstr�mantasyon hizmeti
            "sens",       // Sistem Olay Bildirimleri hizmeti
            "wudfsvc",    // Windows S�r�c� Temel Kav�ak hizmeti
            "wlansvc",    // Kablosuz Konfig�rasyon hizmeti
            "ehrecvr",    // Windows Media Center al�c� hizmeti
            // ... Di�er sistem hizmetleri

            // A� ve ses hizmeti adlar�n� ekleyin
            "lanmanserver",  // Sunucu
            "lanmanworkstation",  // �stemci
            "iphlpsvc",  // IP Yard�mc� Hizmeti
            "audiosrv",  // Windows Ses
            "AudioEndpointBuilder",  // Windows Ses Yap�land�rma
            // ... Di�er a� ve ses hizmetleri
        };

            return systemServiceNames.Contains(serviceName.ToLower());
        }



        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        private bool Servicesclosed = true;
        private async void button14_Click(object sender, EventArgs e)
        {
            
            CYServices services = new CYServices();

            // FormClosed olay�na abone oluyoruz.
            services.FormClosed += Services_FormClosed;

            // Form'un kapat�lma durumu kontrol�
            if (Servicesclosed)
            {
                Servicesclosed = false;
                services.Show();
            }
            else
            {
                if (checkBox1.Checked)
                {
                    TopMost = false;
                    services.TopMost = false;
                    await Task.Delay(100);
                    TopMost = true;
                    await Task.Delay(500);
                    services.TopMost = true;
                }
                else
                {
                    services.TopMost = false;
                    await Task.Delay(200);
                    services.TopMost = true;
                }
                
                //MessageBox.Show(Text = "Already opened!");
                //SetTopMost(services.Handle, true);
            }
        }

        private static void SetTopMost(IntPtr handle, bool topMost)
        {
            int hWndInsertAfter = topMost ? HWND_TOPMOST : HWND_NOTOPMOST;
            SetWindowPos(handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }



        // FormClosed olay� i�in bir olay dinleyicisi metodu
        private void Services_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Burada Form kapat�ld���nda yap�lacak i�lemleri ekleyebilirsiniz.
            // �rne�in, bu metod i�inde ba�ka bir formu a�abilir veya ba�ka i�lemler ger�ekle�tirebilirsiniz.
            Servicesclosed = true;
        }


        private void lstStartupApps_SelectedIndexChanged(object sender, EventArgs e)
        {

        }






    }


}
