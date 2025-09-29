using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerINformatie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            informationForm.Clear();
            try
            {
                GetMemoryInstalled();
                GetProcessorCount();
                GetProcessorSpeed();
                GetDiskInformation();
            }
            catch (Exception) { }

        }

        private void GetMemoryInstalled()
        {
            try
            {
                GetPhysicallyInstalledSystemMemory(out long memKb);
                informationForm.Text += ((memKb / 1024 / 1024) + " GB RAM geïnstalleerd.\r\n");
            }
            catch (Exception) { informationForm.Text += "Kon hoeveelheid RAM niet opvragen.\r\n"; }
        }

        private void GetProcessorCount()
        {
            try {
                informationForm.Text += $"{Environment.ProcessorCount} processors op deze computer.\r\n";
            }
            catch(Exception) { informationForm.Text += "Kon aantal processors niet opvragen.\r\n"; }
        }

        private void GetProcessorSpeed()
        {
            try {
                var key = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor");
                var subkey = key.GetSubKeyNames().First();
                key = Registry.LocalMachine.OpenSubKey($@"Hardware\Description\System\CentralProcessor\{subkey}");
                var names = key.GetValueNames();
                foreach (var name in names) {
                    var v = key.GetValue(name);
                    if (v == null || v.GetType() != typeof(string))
                    {
                        continue;
                    }
                    informationForm.Text += $"{v}\r\n";
                }

            }
            catch(Exception) { informationForm.Text += "Kon snelheid processor niet opvragen.\r\n"; }
        }

        private void GetDiskInformation()
        {
            try
            {
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives) {
                    try {
                        var totalSize = drive.TotalSize / 1024 / 1024 / 1024;
                        var freeSize = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
                        informationForm.Text += $"{drive.RootDirectory.Name}, totale ruimte: {totalSize} GB, vrije ruimte: {freeSize} GB\r\n";
                    }
                    catch (Exception) { /* ignore */ }
                }
            }
            catch (Exception) {
                informationForm.Text += "Kon geen informatie over schijven (opslag) opvragen.\r\n";
            }       
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

    }
}
