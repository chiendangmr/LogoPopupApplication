using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HDControl;
using System.IO;

namespace VTVPCDLogoPopupApplication
{
    public partial class SettingForm : HDForm
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            txtIP.Text = AppSetting.Default.ServerIP;
            nPort.Value = AppSetting.Default.ServerPort;
            nChannel.Value = AppSetting.Default.Channel;
            txtMediaFolder.Text = AppSetting.Default.MediaFolder;
            txtDatabaseFolder.Text = AppSetting.Default.DatabaseFolder;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dig = new FolderBrowserDialog();
            if (txtMediaFolder.Text != "")
                dig.SelectedPath = txtMediaFolder.Text;
            if (dig.ShowDialog() == DialogResult.OK)
                txtMediaFolder.Text = dig.SelectedPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtIP.Text == "")
            {
                HDMessageBox.Show("Chưa nhập địa chỉ IP", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(txtMediaFolder.Text == "")
            {
                HDMessageBox.Show("Chưa nhập thư mục media", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtDatabaseFolder.Text == "")
            {
                HDMessageBox.Show("Chưa nhập thư mục lưu trữ lịch", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtMediaFolder.Text))
            {
                HDMessageBox.Show("Thư mục media không tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(txtDatabaseFolder.Text))
            {
                HDMessageBox.Show("Thư mục lưu trữ không tồn tại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppSetting.Default.ServerIP = txtIP.Text;
            AppSetting.Default.ServerPort = (int)nPort.Value;
            AppSetting.Default.Channel = (int)nChannel.Value;
            AppSetting.Default.MediaFolder = txtMediaFolder.Text;
            AppSetting.Default.DatabaseFolder = txtDatabaseFolder.Text;
            AppSetting.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dig = new FolderBrowserDialog();
            if (txtDatabaseFolder.Text != "")
                dig.SelectedPath = txtDatabaseFolder.Text;
            if (dig.ShowDialog() == DialogResult.OK)
                txtDatabaseFolder.Text = dig.SelectedPath;
        }
    }
}