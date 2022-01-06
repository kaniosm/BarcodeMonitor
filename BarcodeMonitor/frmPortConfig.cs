using System.IO.Ports;

namespace BarcodeMonitor
{
    public partial class frmPortConfig : Form
    {
        public frmPortConfig()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                cmbPorts.Items.Clear();
                var ports = SerialPort.GetPortNames();
                foreach (var port in ports)
                    cmbPorts.Items.Add(port.ToString());
                cmbPorts.SelectedItem = Properties.Settings.Default.SelectedPort;
                cmbBaudRate.SelectedItem = Properties.Settings.Default.SelectedBaudRate.ToString();
                cmbDataBits.SelectedItem = Properties.Settings.Default.SelectedDataBits.ToString();
                cmbStopBits.SelectedItem = Properties.Settings.Default.SelectedStopBits;
                cmbParity.SelectedItem = Properties.Settings.Default.SelectedParity;

                cmbPorts.Enabled = !Properties.Settings.Default.UseUsbScanner;
                cmbBaudRate.Enabled = !Properties.Settings.Default.UseUsbScanner;
                cmbDataBits.Enabled = !Properties.Settings.Default.UseUsbScanner;
                cmbStopBits.Enabled = !Properties.Settings.Default.UseUsbScanner;
                cmbParity.Enabled = !Properties.Settings.Default.UseUsbScanner;
                btnSelect.Enabled = !Properties.Settings.Default.UseUsbScanner;

                btnSave.Enabled = Properties.Settings.Default.UseUsbScanner;
                textBox1.Text = Properties.Settings.Default.UsbScanner;
                chReplaceBarcode.Checked = Properties.Settings.Default.ReplaceBarcode;

                if (textBox1.Text == "") textBox1.Text = "Use your scanner to detect the device";
                var rawinput = new RawInput.RawInput(this.Handle, true);
                rawinput.AddMessageFilter();
                rawinput.KeyPressed += (s, e) => { textBox1.Text = e.KeyPressEvent.DeviceName; };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedPort = (string)cmbPorts.SelectedItem;
            Properties.Settings.Default.SelectedBaudRate = int.Parse((string)cmbBaudRate.SelectedItem);
            Properties.Settings.Default.SelectedDataBits = int.Parse((string)cmbDataBits.SelectedItem);
            Properties.Settings.Default.SelectedStopBits = (string)cmbStopBits.SelectedItem;
            Properties.Settings.Default.SelectedParity = (string)cmbParity.SelectedItem;
            Properties.Settings.Default.UseUsbScanner = checkBox1.Checked;
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cmbPorts.Enabled = !Properties.Settings.Default.UseUsbScanner;
            cmbBaudRate.Enabled = !Properties.Settings.Default.UseUsbScanner;
            cmbDataBits.Enabled = !Properties.Settings.Default.UseUsbScanner;
            cmbStopBits.Enabled = !Properties.Settings.Default.UseUsbScanner;
            cmbParity.Enabled = !Properties.Settings.Default.UseUsbScanner;
            btnSelect.Enabled = !Properties.Settings.Default.UseUsbScanner;

            btnSave.Enabled = Properties.Settings.Default.UseUsbScanner;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UsbScanner = textBox1.Text;
            Properties.Settings.Default.UseUsbScanner = checkBox1.Checked;
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chReplaceBarcode_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReplaceBarcode = chReplaceBarcode.Checked;
        }
    }
}