namespace VTVPCDLogoPopupApplication
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtIP = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.nPort = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtMediaFolder = new DevExpress.XtraEditors.TextEdit();
            this.btnBrowse = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.nChannel = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.txtDatabaseFolder = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtIP.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediaFolder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nChannel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseFolder.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Location = new System.Drawing.Point(45, 15);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(58, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Server IP:";
            // 
            // txtIP
            // 
            this.txtIP.EditValue = "127.0.0.1";
            this.txtIP.Location = new System.Drawing.Point(109, 12);
            this.txtIP.Name = "txtIP";
            this.txtIP.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIP.Properties.Appearance.Options.UseFont = true;
            this.txtIP.Properties.Mask.EditMask = "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(" +
    "25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";
            this.txtIP.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.txtIP.Size = new System.Drawing.Size(97, 22);
            this.txtIP.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Location = new System.Drawing.Point(33, 45);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(70, 16);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Server port:";
            // 
            // nPort
            // 
            this.nPort.EditValue = new decimal(new int[] {
            5250,
            0,
            0,
            0});
            this.nPort.Location = new System.Drawing.Point(109, 42);
            this.nPort.Name = "nPort";
            this.nPort.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nPort.Properties.Appearance.Options.UseFont = true;
            this.nPort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.nPort.Properties.DisplayFormat.FormatString = "n0";
            this.nPort.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.nPort.Properties.EditFormat.FormatString = "n0";
            this.nPort.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.nPort.Properties.Mask.EditMask = "n0";
            this.nPort.Properties.MaxValue = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nPort.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPort.Size = new System.Drawing.Size(97, 22);
            this.nPort.TabIndex = 3;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Location = new System.Drawing.Point(8, 110);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(95, 16);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "Thư mục media:";
            // 
            // txtMediaFolder
            // 
            this.txtMediaFolder.Location = new System.Drawing.Point(109, 107);
            this.txtMediaFolder.Name = "txtMediaFolder";
            this.txtMediaFolder.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMediaFolder.Properties.Appearance.Options.UseFont = true;
            this.txtMediaFolder.Size = new System.Drawing.Size(216, 22);
            this.txtMediaFolder.TabIndex = 5;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Appearance.Options.UseFont = true;
            this.btnBrowse.Location = new System.Drawing.Point(331, 107);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Chọn";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(109, 203);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 37);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "Lưu";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(228, 203);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 37);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Thoát";
            // 
            // nChannel
            // 
            this.nChannel.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nChannel.Location = new System.Drawing.Point(109, 76);
            this.nChannel.Name = "nChannel";
            this.nChannel.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nChannel.Properties.Appearance.Options.UseFont = true;
            this.nChannel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.nChannel.Properties.DisplayFormat.FormatString = "n0";
            this.nChannel.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.nChannel.Properties.EditFormat.FormatString = "n0";
            this.nChannel.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.nChannel.Properties.Mask.EditMask = "n0";
            this.nChannel.Properties.MaxValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nChannel.Size = new System.Drawing.Size(97, 22);
            this.nChannel.TabIndex = 10;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Location = new System.Drawing.Point(52, 79);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(51, 16);
            this.labelControl4.TabIndex = 9;
            this.labelControl4.Text = "Channel:";
            // 
            // btnChoose
            // 
            this.btnChoose.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChoose.Appearance.Options.UseFont = true;
            this.btnChoose.Location = new System.Drawing.Point(331, 135);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(75, 23);
            this.btnChoose.TabIndex = 13;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // txtDatabaseFolder
            // 
            this.txtDatabaseFolder.Location = new System.Drawing.Point(109, 135);
            this.txtDatabaseFolder.Name = "txtDatabaseFolder";
            this.txtDatabaseFolder.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDatabaseFolder.Properties.Appearance.Options.UseFont = true;
            this.txtDatabaseFolder.Size = new System.Drawing.Size(216, 22);
            this.txtDatabaseFolder.TabIndex = 12;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Location = new System.Drawing.Point(2, 138);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(101, 16);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "Thư mục lưu lịch:";
            // 
            // SettingForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(418, 252);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.txtDatabaseFolder);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.nChannel);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtMediaFolder);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.nPort);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cấu hình";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtIP.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediaFolder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nChannel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabaseFolder.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtIP;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SpinEdit nPort;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtMediaFolder;
        private DevExpress.XtraEditors.SimpleButton btnBrowse;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SpinEdit nChannel;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.TextEdit txtDatabaseFolder;
        private DevExpress.XtraEditors.LabelControl labelControl5;
    }
}