using HDCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zzzzzzzzz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string savePath = "";
        private void btn1_Click(object sender, EventArgs e)
        {
            bsAd.List.Add(new View.AdSequence
            {
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                lstAddObj = new View.lstAdd
                {
                    adObj = new View.AdObject
                    {
                        FileName ="Chien Dang",
                        Duration = 3000,
                        State ="DONE"
                    }
                }                
            });
            (bsAd.List as BindingList<View.AdSequence>).ToList().SaveObject(savePath);
            System.Diagnostics.Process.Start(savePath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            savePath = Path.Combine(Application.StartupPath, "AdList.xml");
        }
    }
}
