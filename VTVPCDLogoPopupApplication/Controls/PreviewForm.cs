using System;
using System.Windows.Forms;
using HDControl;
using System.Diagnostics;
using System.Threading;
using HDCore;

namespace VTVPCDLogoPopupApplication.Controls
{
    public partial class PreviewForm : HDForm
    {
        int _width, _height;       
        
        public PreviewForm(int width, int height)
        {
            InitializeComponent();

            _width = width;
            _height = height;                     
        }

        Process p = null;       

        private void PreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {          
            try
            {
                if (p != null)
                {
                    try
                    {
                        p.StandardInput.WriteLine("quit");
                    }
                    catch { }

                    new Thread(() =>
                    {
                        try
                        {
                            p.WaitForExit(1000);
                        }
                        catch { }

                        try
                        {
                            if (HDCore.ProcessExtensions.IsRunning(p))
                                p.Kill();
                        }
                        catch { }
                    }).Start();
                }
            }
            catch { }
        }

        private void PreviewForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;           

                p = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "VTVPCDTickerPreview.exe",
                        CreateNoWindow = true,
                        //RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };

                p.OutputDataReceived += P_OutputDataReceived;
                p.Start();
                p.BeginOutputReadLine();              

                p.WaitForExit();
                this.Activate();
                this.Close();
            }
            catch { }
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Console.WriteLine("Preview: " + e.Data);
        }
    }
}