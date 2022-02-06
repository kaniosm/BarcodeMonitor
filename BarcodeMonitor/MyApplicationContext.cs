using BarcodeMonitor.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeMonitor
{
    public class MyApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private SerialPort? port;
        private BmDataSet bmDataSet = new BmDataSet();
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "BarcodeMonitor";


        public MyApplicationContext()
        {
            string barcodesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeMonitor");
            if (!Directory.Exists(barcodesPath)) Directory.CreateDirectory(barcodesPath);
            if (File.Exists($"{barcodesPath}\\barcodes.xml")) bmDataSet.ReadXml($"{barcodesPath}\\barcodes.xml");
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.BarcodeMonitor,
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };
            trayIcon.ContextMenuStrip.Items.Add("Configure Port", null, ConfigurePort);
            trayIcon.ContextMenuStrip.Items.Add("Barcodes", null, EditBarcodes);
            var mnu = (ToolStripMenuItem)trayIcon.ContextMenuStrip.Items.Add("Add to Startup", null, AddToStartUp);
            mnu.CheckOnClick = true;
            mnu.Checked = IsInStartup();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);
            if(!Settings.Default.UseUsbScanner)
                InitializeScanner();
            else
            {
                string line = ""; 
                var rawInput = new RawInput.RawInput(new Form().Handle, false);
                rawInput.AddMessageFilter();
                DateTime lastMsg = DateTime.Now;
                HookManager.KeyPress += (s, e) =>
                {
                    //Thread.Sleep(1000);
                    if((DateTime.Now - lastMsg) < TimeSpan.FromMilliseconds(1000) && e.KeyChar == (char)13)
                        e.Handled = true;
                };
                rawInput.KeyPressed += (s, e) => 
                {
                    if( e.KeyPressEvent.DeviceName == Settings.Default.UsbScanner && e.KeyPressEvent.KeyPressState == "MAKE")
                    {
                        lastMsg = DateTime.Now;
                        char c = (char)e.KeyPressEvent.VKey;
                        if (c == '\b') return;
                        if (c != '\r')
                        {
                            line += c;
                            if (Settings.Default.ReplaceBarcode) SendKeys.SendWait("{BACKSPACE}");
                        }
                        else
                        {
                            if (Settings.Default.ReplaceBarcode) SendKeys.SendWait("{BACKSPACE}");
                            var mapp = bmDataSet.Barcode.AsEnumerable().FirstOrDefault(b => b.Barcode == line.TrimEnd());
                            Clipboard.SetText(mapp?.ItemCode ?? "NA");
                            if (Settings.Default.ReplaceBarcode)
                            {
                                SendKeys.SendWait("ff{ESC}");
                                SendKeys.SendWait(mapp?.ItemCode);
                            }
                            line = "";
                        }
                    }
                };
            }
        }

        private bool IsInStartup()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            return key?.GetValue(StartupValue) != null;
        }

        private void AddToStartUp(object? sender, EventArgs e)
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            if (!IsInStartup())
                key?.SetValue(StartupValue, Application.ExecutablePath.ToString());
            else
                key?.DeleteValue(StartupValue);
        }

        private void ConfigurePort(object? sender, EventArgs e)
        {
            frmPortConfig frm = new frmPortConfig();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                InitializeScanner();
            }
        }

        private void EditBarcodes(object? sender, EventArgs e)
        {
            string barcodesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeMonitor");
            if (!Directory.Exists(barcodesPath)) Directory.CreateDirectory(barcodesPath);
            frmBarcodes frm = new frmBarcodes();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                bmDataSet.Clear();
                bmDataSet.ReadXml($"{barcodesPath}\\barcodes.xml");
            }
        }

        private void InitializeScanner()
        {
            try
            {
                if (port != null)
                {
                    port.DataReceived -= OnScan;
                    port.Close();
                }
                string portNum = Settings.Default.SelectedPort;
                int baudRate = Settings.Default.SelectedBaudRate;
                Parity parity = (Parity)Enum.Parse(typeof(Parity), Settings.Default.SelectedParity);
                int dataBits = Settings.Default.SelectedDataBits;
                StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), Settings.Default.SelectedStopBits);

                port = new SerialPort(portNum, baudRate, parity, dataBits, stopBits);
                port.Close();
                if (!port.IsOpen)
                    port.Open();
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                port.DataReceived += OnScan;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnScan(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort port = (SerialPort)sender;

            string line = port.ReadExisting();

            int idx = line.IndexOf('\r');
            if (idx != -1)
            {
                line = line.Substring(0, idx);
                var mapp = bmDataSet.Barcode.AsEnumerable().FirstOrDefault(b => b.Barcode == line.TrimEnd());
                Clipboard.SetText(mapp?.ItemCode);
                if (Settings.Default.ReplaceBarcode)
                {
                    foreach(var c in line) SendKeys.SendWait("{BACKSPACE}");
                    SendKeys.SendWait(mapp?.ItemCode);
                }
                //_scanBuffer += line;
                //Invoke((MethodInvoker)delegate { OnScan(_scanBuffer); });
                //_scanBuffer = "";
            }
            else
            {
                //_scanBuffer += line;
            }
        }

        private void Exit(object? sender, EventArgs e)
        {
            trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
