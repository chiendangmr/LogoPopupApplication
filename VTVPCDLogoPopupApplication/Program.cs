using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VTVPCDLogoPopupApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (HDControl.Tools.RunOnly())
                Application.Run(new MainForm());
            else
                HDControl.HDMessageBox.Show("Phần mềm đang chạy!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
