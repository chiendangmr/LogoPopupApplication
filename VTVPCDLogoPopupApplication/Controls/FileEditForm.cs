using System;
using System.Windows.Forms;
using HDControl;

namespace VTVPCDLogoPopupApplication.Controls
{
    public partial class FileEditForm : HDForm
    {
        public FileEditForm()
        {
            InitializeComponent();
        }

        private void FileEditForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lkType.GetSelectedDataRow() == null)
                HDMessageBox.Show("Chưa chọn loại tin", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}