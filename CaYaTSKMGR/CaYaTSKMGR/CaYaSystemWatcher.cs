using Microsoft.Win32;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace CaYaTSKMGR
{
    public partial class CaYaSystemWatcher : Form
    {
        static FileSystemWatcher fileWatcher;
        static string htmlLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.html");
        static System.Timers.Timer logTimer;
        static string logBuffer = "";
        static RegistryWatcher registryWatcher;

        public CaYaSystemWatcher()
        {
            InitializeComponent();
        }

        private async void CaYaSystemWatcher_Load(object sender, EventArgs e)
        {
            await StartupS();
        }

        private async Task StartupS()
        {
            await Task.Run(() =>
            {
                InitializeLog();
                StartFileSystemWatcher();
                StartProcessWatcher();
                StartLogTimer();
            });
            await Task.Run(() =>
            {
                StartRegistryWatcher();
            });
        }

        static void InitializeLog()
        {
            using (StreamWriter writer = new StreamWriter(htmlLogPath, false, Encoding.UTF8))
            {
                writer.WriteLine("<html><head><title>İzleme Logları</title><style>");
                writer.WriteLine("body { font-family: 'Segoe UI', sans-serif; }");
                writer.WriteLine(".tab { display: none; }");
                writer.WriteLine(".tab-header { cursor: pointer; padding: 10px; background-color: #ccc; border: 1px solid #aaa; }");
                writer.WriteLine(".tab-header:hover { background-color: #bbb; }");
                writer.WriteLine(".tab-content { padding: 10px; border: 1px solid #aaa; }");
                writer.WriteLine("</style></head><body>");
                writer.WriteLine("<h1>İzleme Logları</h1>");
                writer.WriteLine("<div class='tab-header' onclick='toggleTab(\"process-tab\")'>Program Açılış/Kapanış</div>");
                writer.WriteLine("<div id='process-tab' class='tab'><div class='tab-content'><ul id='process-log'></ul></div></div>");
                writer.WriteLine("<div class='tab-header' onclick='toggleTab(\"registry-tab\")'>Kayıt Defteri Değişiklikleri</div>");
                writer.WriteLine("<div id='registry-tab' class='tab'><div class='tab-content'><ul id='registry-log'></ul></div></div>");
                writer.WriteLine("<div class='tab-header' onclick='toggleTab(\"file-tab\")'>Dosya İşlemleri</div>");
                writer.WriteLine("<div id='file-tab' class='tab'><div class='tab-content'><ul id='file-log'></ul></div></div>");
                writer.WriteLine("<script>");
                writer.WriteLine("function toggleTab(tabId) { var tab = document.getElementById(tabId); if (tab.style.display === 'none') { tab.style.display = 'block'; } else { tab.style.display = 'none'; } }");
                writer.WriteLine("</script>");
                writer.WriteLine("</body></html>");
            }
        }

        static void AppendToLog(string category, string log)
        {
            log = log.Replace("\\", "\\\\");
            logBuffer += $"<script>document.getElementById('{category}-log').innerHTML += '<li>{DateTime.Now}: {EscapeHtml(log)}</li>';</script>\n";
        }

        static string EscapeHtml(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        static void WriteLogToFile()
        {
            if (!string.IsNullOrEmpty(logBuffer))
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(htmlLogPath, true, Encoding.UTF8))
                    {
                        writer.Write(logBuffer);
                    }
                    logBuffer = "";
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Log yazma hatası: {ex.Message}");
                }
            }
        }

        static void StartLogTimer()
        {
            logTimer = new System.Timers.Timer(5000); // 5 saniyede bir çalışır
            logTimer.Elapsed += (sender, e) => WriteLogToFile();
            logTimer.AutoReset = true;
            logTimer.Enabled = true;
        }

        static void StartFileSystemWatcher()
        {
            fileWatcher = new FileSystemWatcher
            {
                Path = @"C:\", // Tüm bilgisayarı izler
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                Filter = "*.*",
                IncludeSubdirectories = true
            };

            fileWatcher.Changed += OnChanged;
            fileWatcher.Created += OnCreated;
            fileWatcher.Deleted += OnDeleted;
            fileWatcher.Renamed += OnRenamed;
            fileWatcher.EnableRaisingEvents = true;
        }

        static void OnChanged(object source, FileSystemEventArgs e) =>
            AppendToLog("file", $"Değiştirilen dosya: {EscapeHtml(e.FullPath)}");

        static void OnCreated(object source, FileSystemEventArgs e) =>
            AppendToLog("file", $"Oluşturulan dosya: {EscapeHtml(e.FullPath)}");

        static void OnDeleted(object source, FileSystemEventArgs e) =>
            AppendToLog("file", $"Silinen dosya: {EscapeHtml(e.FullPath)}");

        static void OnRenamed(object source, RenamedEventArgs e) =>
            AppendToLog("file", $"Yeniden adlandırılan dosya: {EscapeHtml(e.OldFullPath)} -> {EscapeHtml(e.FullPath)}");







        static void StartRegistryWatcher()
        {


            registryWatcher = new RegistryWatcher(@"Software\Microsoft\Windows\CurrentVersion\Run");
            registryWatcher.RegistryChanged += OnRegistryChanged;


            //AppendToLog("registry", $" ");
        }
        static void RegistryKeyChanged(object sender, EventArgs e)
        {
            // Değişikliğin olduğu Registry anahtarını alın
            RegistryKey regKey = (RegistryKey)sender;

            // Değişikliğin olduğu anahtarın konumunu ve değişikliğin ne olduğunu konsola yazdırın
            Console.WriteLine("Değişiklik algılandı:");
            Console.WriteLine("Anahtar: " + regKey.Name);
            AppendToLog("registry", $"Değişiklik algılandı: Anahtar: {regKey.Name}");
        }


        //static void StartRegistryWatcher()
        //{
        //    registryWatcher = new RegistryWatcher(@"Software\Microsoft\Windows\CurrentVersion\Run");
        //    registryWatcher.RegistryChanged += OnRegistryChanged;
        //}

        static void OnRegistryChanged(object sender, RegistryChangedEventArgs e)
        {
            string valueMessage = e.Value != null ? $"Yeni Değer: {e.Value}" : "Değer Silindi";
            AppendToLog("registry", $"Kayıt defteri değişikliği: {EscapeHtml(e.RegistryKeyPath)} - Değer: {EscapeHtml(e.ValueName)} - {EscapeHtml(valueMessage)}");
        }

        static void StartProcessWatcher()
        {
            ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(ProcessStarted);
            startWatch.Start();

            ManagementEventWatcher stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(ProcessStopped);
            stopWatch.Start();
        }

        static void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            AppendToLog("process", $"Başlatılan Uygulama: {processName}");
        }

        static void ProcessStopped(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            AppendToLog("process", $"Durdurulan Uygulama: {processName}");
        }

        static void CloseLog()
        {
            WriteLogToFile();
            using (StreamWriter writer = new StreamWriter(htmlLogPath, true, Encoding.UTF8))
            {
                writer.WriteLine("</body></html>");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CloseLog();
        }
    }




   



}
