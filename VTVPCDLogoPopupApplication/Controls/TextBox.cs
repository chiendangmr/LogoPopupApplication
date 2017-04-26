using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTVPCDLogoPopupApplication.Controls
{
    public class TextBox : TextEdit
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool hasKeyDown = false;

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                var curIndex = this.TabIndex;
                int first = curIndex / 100;
                int last = curIndex % 100;
                int nextIndex = curIndex;
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (this.SelectionStart > 0)
                            goto exit;
                        nextIndex = first * 100 + last - 1;
                        break;

                    case Keys.Right:
                        if (this.SelectionStart < this.Text.Length)
                            goto exit;
                        nextIndex = first * 100 + last + 1;
                        break;

                    case Keys.Up:
                        nextIndex = (first > 0 ? (first - 1) * 100 : 0) + last;
                        break;

                    case Keys.Down:
                        nextIndex = (first + 1) * 100 + last;
                        break;
                }

                System.Windows.Forms.Control nextControl = null;
                foreach (System.Windows.Forms.Control control in this.Parent.Controls)
                {
                    if (control.CanFocus)
                    {
                        if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Up) && control.TabIndex <= nextIndex)
                        {
                            if (nextControl == null || control.TabIndex > nextControl.TabIndex)
                                nextControl = control;
                        }
                        else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.Down) && control.TabIndex >= nextIndex)
                        {
                            if (nextControl == null || control.TabIndex < nextControl.TabIndex)
                                nextControl = control;
                        }
                    }
                }
                if (nextControl != null)
                {
                    nextControl.Focus();
                    if ((nextControl as TextBox) != null)
                        (nextControl as TextBox).SelectAll();
                    hasKeyDown = true;
                }
            }

            exit:
            if (!hasKeyDown)
                base.OnKeyDown(e);
        }
    }
}
