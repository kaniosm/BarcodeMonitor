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
                if(File.Exists("barcodes.xml"))
                    bmDataSet.ReadXml("barcodes.xml");
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
                bmDataSet.WriteXml("barcodes.xml");
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
