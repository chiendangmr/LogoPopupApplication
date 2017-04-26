namespace VTVPCDLogoPopupApplication.Controls
{
    partial class FileEditForm
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
            this.components = new System.ComponentModel.Container();
            this.lkType = new DevExpress.XtraEditors.LookUpEdit();
            this.tinDocBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.lkType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tinDocBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lkType
            // 
            this.lkType.Location = new System.Drawing.Point(12, 12);
            this.lkType.Name = "lkType";
            this.lkType.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lkType.Properties.Appearance.Options.UseFont = true;
            this.lkType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lkType.Properties.AppearanceDropDown.Options.UseFont = true;
            this.lkType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lkType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TypeString", "Loại")});
            this.lkType.Properties.DataSource = this.tinDocBindingSource;
            this.lkType.Properties.DisplayMember = "TypeString";
            this.lkType.Properties.NullText = "Chưa chọn loại";
            this.lkType.Properties.NullValuePrompt = "Chưa chọn loại";
            this.lkType.Properties.NullValuePromptShowForEmptyValue = true;
            this.lkType.Properties.ValueMember = "Type";
            this.lkType.Size = new System.Drawing.Size(317, 30);
            this.lkType.TabIndex = 0;
            // 
            // tinDocBindingSource
            // 
            this.tinDocBindingSource.DataSource = typeof(VTVPCDLogoPopupApplication.Objects.FileList);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(62, 62);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(101, 31);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Chọn";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(179, 62);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Thôi";
            // 
            // SelectTinDocTypeForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(341, 105);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lkType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTinDocTypeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loại tin dọc";
            this.Load += new System.EventHandler(this.FileEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lkType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tinDocBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource tinDocBindingSource;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        public DevExpress.XtraEditors.LookUpEdit lkType;
    }
}