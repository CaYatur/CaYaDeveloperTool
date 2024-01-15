using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaYaTSKMGR
{
    public partial class CYServices : Form
    {
        public CYServices()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Dosya konumu
            string dosyaYolu = @"C:\Windows\explorer.exe";

            // Dosya açma işlemi
            ExplorerIleAc(dosyaYolu);
        }
        static void ExplorerIleAc(string dosyaYolu)
        {
            try
            {
                // Dosya veya klasörü varsayılan uygulama ile açmayı dene
                Process.Start($"\"{dosyaYolu}\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dosya veya klasörü açma hatası: " + ex.Message);
            }
        }


        //////Exit/
        private void label5_Click(object sender, EventArgs e)
        {
            Close();
        }
        //////Back/
        private void back_Click(object sender, EventArgs e)
        {
            back.Visible = false;
            label1.Visible = true;

            //Label1 WINDOWS SERVICES
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            panel1.Visible = false;
            panel2.Visible = false;
            panelst1.Visible = false;
            panelst2.Visible = false;
            //////////////////////////
            ///


        }
        //////////////////







        ///////////////////////////////////////////Services For Windows//////////////////////>>>>>>>>>
        private void label1_Click(object sender, EventArgs e)
        {
            back.Visible = true;
            label1.Visible = false;

            //
            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label13.Visible = true;
            panel1.Visible = true;
            panel2.Visible = true;
            panelst1.Visible = true;
            panelst2.Visible = true;
        }

        private async void ServiceCloseDefault()
        {
            label12.Text = "Processing Stoping Services";
            panelst1.BackColor = Color.Blue;
            await Task.Delay(1000);
            // Ses Hizmeti
            string audioServiceName = "AudioSrv";
            StopService(audioServiceName);

            // İnternet Hizmeti (Windows Güncelleştirmeleri Hizmeti örneği)
            string internetServiceName = "wuauserv";
            string internetServiceName2 = "Ethernet";
            StopService(internetServiceName);
            StopService(internetServiceName2);

            DisableNetwork();

            label12.Text = "Successfully! Stopped Services";
            panelst1.BackColor = Color.Green;
        }

        private async void ServiceStartDefault()
        {
            label12.Text = "Processing Starting Services";
            panelst1.BackColor = Color.Blue;
            await Task.Delay(1000);
            // Ses Hizmeti
            string audioServiceName = "AudioSrv";
            StartService(audioServiceName);

            // İnternet Hizmeti (Windows Güncelleştirmeleri Hizmeti örneği)
            string internetServiceName = "wuauserv";
            string internetServiceName2 = "Ethernet";
            StartService(internetServiceName);
            StartService(internetServiceName2);

            EnableNetwork();

            label12.Text = "Successfully! Started Services";
            panelst1.BackColor = Color.Green;
        }

        static void StartService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    Console.WriteLine($"Service '{serviceName}' started successfully.");
                }
                else
                {
                    Console.WriteLine($"Service '{serviceName}' is already running.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting service '{serviceName}': {ex.Message}");
                CYServices myService = new CYServices();
                myService.label12.Text = "ERROR! Starting Services";
                myService.panelst1.BackColor = Color.Red;
            }
        }

        static void StopService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    Console.WriteLine($"Service '{serviceName}' stopped successfully.");
                }
                else
                {
                    Console.WriteLine($"Service '{serviceName}' is already stopped.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping service '{serviceName}': {ex.Message}");
                CYServices myService = new CYServices();
                myService.label12.Text = "ERROR! Stoping Services";
                myService.panelst1.BackColor = Color.Red;
            }
        }



        static void DisableNetwork()
        {
            string[] bağdaştırıcılar = GetNetworkInterfaces();

            foreach (string bağdaştırıcı in bağdaştırıcılar)
            {
                RunCommand($"netsh interface set interface name=\"{bağdaştırıcı}\" admin=DISABLED");
            }
        }

        static void EnableNetwork()
        {
            string[] bağdaştırıcılar = GetNetworkInterfaces();

            foreach (string bağdaştırıcı in bağdaştırıcılar)
            {
                RunCommand($"netsh interface set interface name=\"{bağdaştırıcı}\" admin=ENABLED");
            }
        }

        static string[] GetNetworkInterfaces()
        {
            Process process = new Process();
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = "Get-NetAdapter | Select-Object -ExpandProperty Name";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Çıktıdaki boşlukları temizleyerek diziye dönüştür
            string[] bağdaştırıcılar = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return bağdaştırıcılar;
        }

        static void RunCommand(string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}"; // /c komutu çalıştırdıktan sonra kapatır.
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            Console.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Hata: {error}");
            }

            process.WaitForExit();
        }


        private void label8_Click(object sender, EventArgs e)
        {
            ServiceStartDefault();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            ServiceCloseDefault();
        }

        /////////
        private async void label9_Click(object sender, EventArgs e)
        {
            label14.Visible = true;
            label13.Text = "Processing Starting Services";
            panelst2.BackColor = Color.Blue;
            await Task.Delay(1000);

            EnableNetwork();
            EnableAllServices();

            label14.Visible = false;
            label13.Text = "Successfully! Started Services";
            panelst2.BackColor = Color.Green;
        }

        private async void label11_Click(object sender, EventArgs e)
        {
            label14.Visible = true;
            label13.Text = "Processing Stoping Services";
            panelst2.BackColor = Color.Blue;
            await Task.Delay(1000);

            DisableNetwork();
            DisableAllServices();
            //DisableNonSystemServices();

            label13.Text = "Successfully! Stoped Services";
            panelst2.BackColor = Color.Green;
            label14.Visible = false;
        }


        static void DisableAllServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                Parallel.ForEach(services, service =>
                {
                    try
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            int timeoutMilliseconds = 10000; // 10 saniye
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                            Console.WriteLine($"Service '{service.DisplayName}' stopped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error stopping service '{service.DisplayName}': {ex.Message}");
                        // Burada hata es geçiliyor, loglamak veya başka bir işlem yapmak istiyorsanız ekleyebilirsiniz.
                    }
                });

                Console.WriteLine("All services stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping services: {ex.Message}");
                CYServices myService = new CYServices();
                myService.label13.Text = "ERROR! Stopping Services";
                myService.panelst2.BackColor = Color.Red;
            }
        }

        static void EnableAllServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                Parallel.ForEach(services, service =>
                {
                    try
                    {
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            int timeoutMilliseconds = 10000; // 20 saniye
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                            Console.WriteLine($"Service '{service.DisplayName}' started successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error starting service '{service.DisplayName}': {ex.Message}");
                        // Burada hata es geçiliyor, loglamak veya başka bir işlem yapmak istiyorsanız ekleyebilirsiniz.
                    }
                });

                Console.WriteLine("All services started successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting services: {ex.Message}");
                CYServices myService = new CYServices();
                myService.label13.Text = "ERROR! Starting Services";
                myService.panelst2.BackColor = Color.Red;
            }
        }

        static void DisableNonSystemServices()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                Parallel.ForEach(services, service =>
                {
                    try
                    {
                        // Sadece Windows sistem hizmetleri hariç diğer hizmetleri kapat
                        if (!service.ServiceName.ToLower().StartsWith("wuauserv") && // Örnek: Windows Update
                            !service.ServiceName.ToLower().StartsWith("winmgmt") &&   // Örnek: Windows Management Instrumentation
                            service.Status == ServiceControllerStatus.Running)
                        {
                            int timeoutMilliseconds = 10000; // 10 saniye
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                            Console.WriteLine($"Service '{service.DisplayName}' stopped successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error stopping service '{service.DisplayName}': {ex.Message}");
                        // Burada hata es geçiliyor, loglamak veya başka bir işlem yapmak istiyorsanız ekleyebilirsiniz.
                    }
                });

                Console.WriteLine("Non-system services stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping non-system services: {ex.Message}");
                CYServices myService = new CYServices();
                myService.label13.Text = "ERROR! Stopping Non-system Services";
                myService.panelst2.BackColor = Color.Red;
            }
        }




        //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<//////////Services For Windows/////////////////////////////////<<<<<<






    }
}
