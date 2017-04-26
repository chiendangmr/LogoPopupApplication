namespace zzzzzzzzz
{
    partial class Form1
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
            this.rtb1 = new System.Windows.Forms.RichTextBox();
            this.btn1 = new System.Windows.Forms.Button();
            this.bsAd = new System.Windows.Forms.BindingSource();
            ((System.ComponentModel.ISupportInitialize)(this.bsAd)).BeginInit();
            this.SuspendLayout();
            // 
            // rtb1
            // 
            this.rtb1.Location = new System.Drawing.Point(37, 80);
            this.rtb1.Name = "rtb1";
            this.rtb1.Size = new System.Drawing.Size(549, 96);
            this.rtb1.TabIndex = 0;
            this.rtb1.Text = "";
            // 
            // btn1
            // 
            this.btn1.Location = new System.Drawing.Point(226, 243);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(160, 81);
            this.btn1.TabIndex = 1;
            this.btn1.Text = "button1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btn1_Click);
            // 
            // bsAd
            // 
            this.bsAd.DataSource = typeof(zzzzzzzzz.View.AdSequence);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 335);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.rtb1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bsAd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb1;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.BindingSource bsAd;
    }
}

