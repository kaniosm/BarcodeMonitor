using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BarcodeMonitor
{
    public partial class frmBarcodes : Form
    {
        private BmDataSet bmDataSet = new BmDataSet();
        public frmBarcodes()
        {
            InitializeComponent();
        }

        private void frmBarcodes_Load(object sender, EventArgs e)
        {
            try
            {
                string barcodesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeMonitor");
                if (!Directory.Exists(barcodesPath)) Directory.CreateDirectory(barcodesPath);
                if (File.Exists($"{barcodesPath}\\barcodes.xml"))
                    bmDataSet.ReadXml($"{barcodesPath}\\barcodes.xml");
                dataGridView1.DataSource = bmDataSet.Barcode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string barcodesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BarcodeMonitor");
                if (!Directory.Exists(barcodesPath)) Directory.CreateDirectory(barcodesPath);
                bmDataSet.WriteXml($"{barcodesPath}\\barcodes.xml");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
