using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HDControl;
using Svt.Caspar;
using System.Threading;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using System.IO;
using HDCore;
using DevExpress.XtraGrid;
using System.Text.RegularExpressions;
using MediaInfoDotNet;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;

namespace VTVPCDLogoPopupApplication
{
    public partial class MainForm : HDForm
    {
        CasparDevice caspar = null;

        string fileListDBPath = "";
        string dbPath = "";

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            AboutHDUltrasoftForm frmAbout = new AboutHDUltrasoftForm();
            frmAbout.lbName.Text = "VTVPCD Logo & Popup Schedule Application";
            frmAbout.lbVersion.Text = "2017.01.17.0";
            frmAbout.ShowDialog();
        }

        private void btnSetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (new SettingForm().ShowDialog() == DialogResult.OK)
            {
                if (caspar.Settings.Hostname != AppSetting.Default.ServerIP || caspar.Settings.Port != AppSetting.Default.ServerPort)
                {
                    try
                    {
                        if (caspar.IsConnected)
                            caspar.Disconnect();
                    }
                    catch { }

                    caspar.Connect(AppSetting.Default.ServerIP, AppSetting.Default.ServerPort, true);
                }
            }
        }

        bool running = false;
        private void MainForm_Load(object sender, EventArgs e)
        {
            LogProcess.LogFolder = Path.Combine(Application.StartupPath, "Logs");
            LogProcess.Start();

            caspar = new CasparDevice();
            caspar.ChannelInfo += Caspar_ChannelInfo;
            caspar.Called += Caspar_Called;
            if (AppSetting.Default.ServerIP != "")
                caspar.Connect(AppSetting.Default.ServerIP, AppSetting.Default.ServerPort, true);

            running = true;
            thrInfo = new Thread(InfoThread);
            thrInfo.IsBackground = true;
            thrInfo.Start();

            dbPath = Path.Combine(Application.StartupPath, "DBs");
            if (!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            fileListDBPath = Path.Combine(dbPath, "FileList.xml");
            if (File.Exists(fileListDBPath))
            {
                try
                {
                    var tins = Utils.GetObject<List<Objects.FileList>>(fileListDBPath);
                    foreach (var tin in tins)
                        bsFileList.Add(tin);
                }
                catch { }
            }

            playListPath = Path.Combine(dbPath, "PlayList.xml");
            if (File.Exists(playListPath))
            {
                try
                {
                    var tins = Utils.GetObject<List<Objects.PlayList>>(playListPath);
                    foreach (var tin in tins)
                        bsPlayList.Add(tin);
                }
                catch { }
            }

            btnStart.PerformClick();
        }

        object lockCall = new object();
        private void Caspar_Called(object sender, EventArgs e)
        {
            lock (lockCall)
            {
                Monitor.PulseAll(lockCall);
            }
        }

        channel casparChannel = null;
        object lockChannel = new object();
        private void Caspar_ChannelInfo(object sender, ChannelEventArgs e)
        {
            lock (lockChannel)
            {
                casparChannel = e.Channel;
            }
        }

        private void tServer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (caspar.IsConnected)
                {
                    lbServer.Caption = "Connected";
                }
                else
                {
                    lbServer.Caption = "Disconnect";
                }
            }
            catch { }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;

            if (thrInfo != null)
                thrInfo.Join();

            LogProcess.Stop();
        }

        Thread thrInfo = null;
        void InfoThread()
        {
            while (running)
            {
                try
                {
                    if (caspar != null && caspar.IsConnected && caspar.Channels.Count > 0)
                    {
                        caspar.Channels[AppSetting.Default.Channel].GetInfo();

                        for (int time = 0; running && time < 5000; time += 100)
                            Thread.Sleep(100);
                    }
                    else
                        for (int time = 0; running && time < 1000; time += 100)
                            Thread.Sleep(100);
                }
                catch { }
            }
        }

        bool cgRunning = false;
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!cgRunning)
            {
                LogProcess.AddLog("Bắt đầu đồ họa");

                cgRunning = true;
                lbStatus.Caption = "Running";

                thrLogoPopup = new Thread(LogoPopupThread);
                thrLogoPopup.IsBackground = true;
                thrLogoPopup.Start();
            }
        }

        RepositoryItemCheckEdit chkedit = new RepositoryItemCheckEdit();
        protected void DrawCheckBox(Graphics g, Rectangle r, bool Checked)
        {
            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo info;
            DevExpress.XtraEditors.Drawing.CheckEditPainter painter;
            DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs args;
            info = chkedit.CreateViewInfo() as DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo;
            painter = chkedit.CreatePainter() as DevExpress.XtraEditors.Drawing.CheckEditPainter;
            info.EditValue = Checked;

            info.Bounds = r;
            info.PaintAppearance.ForeColor = Color.Black;
            info.CalcViewInfo(g);
            args = new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, new DevExpress.Utils.Drawing.GraphicsCache(g), r);
            painter.Draw(args);
            args.Cache.Dispose();
        }

        private void view_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == (sender as GridView).Columns["Use"])
            {
                e.Info.InnerElements.Clear();
                e.Info.Caption = "";
                e.Painter.DrawObject(e.Info);
                DrawCheckBox(e.Graphics, e.Bounds, getCheckedCount(sender as GridView) == (sender as GridView).RowCount);
                e.Handled = true;
            }
        }

        int getCheckedCount(GridView gv)
        {
            int count = 0;
            for (int i = 0; i < gv.RowCount; i++)
            {
                if ((bool)gv.GetRowCellValue(i, gv.Columns["Use"]) == true)
                    count++;
            }
            return count;
        }

        void CheckAll(GridView gv)
        {
            for (int i = 0; i < gv.RowCount; i++)
            {
                gv.SetRowCellValue(i, gv.Columns["Use"], true);
            }
        }

        void UnChekAll(GridView gv)
        {
            for (int i = 0; i < gv.RowCount; i++)
            {
                gv.SetRowCellValue(i, gv.Columns["Use"], false);
            }
        }

        #region File list                

        void SaveFileList()
        {
            try
            {
                (bsFileList.List as BindingList<Objects.FileList>).ToList().SaveObject(fileListDBPath);
            }
            catch { }
        }

        private void gvFileList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            SaveFileList();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(AppSetting.Default.MediaFolder))
                {
                    HDMessageBox.Show("Thư mục video cấu hình không tồn tại", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    openFileDialog1.InitialDirectory = AppSetting.Default.MediaFolder;
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var i in openFileDialog1.FileNames)
                        {
                            bsFileList.List.Add(new Objects.FileList
                            {
                                Use = false,
                                FilePath = i
                            });
                        }
                         (bsFileList.List as BindingList<Objects.FileList>).ToList().SaveObject(fileListDBPath);
                    }
                }
            }
            catch (Exception ex)
            {
                HDMessageBox.Show(ex.ToString());
            }
        }


        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (gvFileList.SelectedRowsCount <= 0 && gvFileList.FocusedRowHandle < 0)
                HDMessageBox.Show("Không chọn tin nào để xóa", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var lstSelected = gvFileList.GetSelectedRows();
                if (lstSelected.Length == 0)
                    lstSelected = new int[1] { gvFileList.FocusedRowHandle };
                if (HDMessageBox.Show("Chắc chắn xóa " + lstSelected.Length + " tin", "Chú ý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (var handle in lstSelected.OrderByDescending(h => h).ToList())
                    {
                        bsFileList.List.RemoveAt(handle);
                    }

                    SaveFileList();
                }
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (gvFileList.SelectedRowsCount <= 0 && gvFileList.FocusedRowHandle < 0)
                HDMessageBox.Show("Chưa chọn tin cần chuyển lên", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var lstHandle = gvFileList.GetSelectedRows().OrderBy(h => h).ToList();
                if (lstHandle.Count == 0)
                    lstHandle.Add(gvFileList.FocusedRowHandle);
                if (lstHandle[0] > 0)
                {
                    var forcusOld = gvFileList.FocusedRowHandle;

                    gvFileList.ClearSelection();
                    foreach (var handle in lstHandle)
                    {
                        var obj = bsFileList.List[handle] as Objects.FileList;
                        bsFileList.List.RemoveAt(handle);
                        bsFileList.List.Insert(handle - 1, obj);
                        gvFileList.SelectRow(handle - 1);
                        if (forcusOld == handle)
                            gvFileList.FocusedRowHandle = handle - 1;
                    }

                    SaveFileList();
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (gvFileList.SelectedRowsCount <= 0 && gvFileList.FocusedRowHandle < 0)
                HDMessageBox.Show("Chưa chọn tin cần chuyển xuống", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var lstHandle = gvFileList.GetSelectedRows().OrderByDescending(h => h).ToList();
                if (lstHandle.Count == 0)
                    lstHandle.Add(gvFileList.FocusedRowHandle);
                if (lstHandle[0] < gvFileList.RowCount - 1)
                {
                    var forcusOld = gvFileList.FocusedRowHandle;

                    gvFileList.ClearSelection();
                    foreach (var handle in lstHandle)
                    {
                        var obj = bsFileList.List[handle] as Objects.FileList;
                        bsFileList.List.RemoveAt(handle);
                        bsFileList.List.Insert(handle + 1, obj);
                        gvFileList.SelectRow(handle + 1);
                        if (forcusOld == handle)
                            gvFileList.FocusedRowHandle = handle - 1;
                    }

                    SaveFileList();
                }
            }
        }

        private void btnTinDocClone_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gvFileList.SelectedRowsCount <= 0 && gvFileList.FocusedRowHandle < 0)
                HDMessageBox.Show("Chưa chọn tin để tạo bản sao", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                var handles = gvFileList.GetSelectedRows().OrderBy(h => h).ToList();
                if (handles.Count == 0)
                    handles.Add(gvFileList.FocusedRowHandle);
                for (int i = 0; i < handles.Count; ++i)
                {
                    var tin = gvFileList.GetRow(handles[i]) as Objects.FileList;
                    var tinNew = new Objects.FileList()
                    {
                        Use = false,
                        FilePath = tin.FilePath,
                    };
                    bsFileList.Add(tinNew);
                }
                SaveFileList();
            }
        }

        private void btnTinDocImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.Filter = "Xml files(*.xml)|*.xml";
            if (dig.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var tins = Utils.GetObject<List<Objects.FileList>>(dig.FileName);
                    if (tins.Count == 0)
                        throw new Exception("File xml không có tin nào");
                    else
                    {
                        foreach (var tin in tins)
                        {
                            bsFileList.Add(tin);
                        }
                        SaveFileList();
                    }
                }
                catch (Exception ex)
                {
                    HDMessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTinDocExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog dig = new SaveFileDialog();
            dig.Filter = "Xml files(*.xml)|*.xml";
            if (dig.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    (bsFileList.List as BindingList<Objects.FileList>).ToList().SaveObject(dig.FileName);
                }
                catch (Exception ex)
                {
                    HDMessageBox.Show("Lỗi:" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #region Show Popup
        private void gvFileList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            popupFileList.ShowPopup(MousePosition);
        }
        private void gvPlayList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            popupPlayList.ShowPopup(MousePosition);
        }
        #endregion
        Thread thrLogoPopup = null;
        bool anyChanged = true;
        List<object> threadlock = null;
        void LogoPopupThread()
        {
            string tempFileName = "";
            int tempTotalDuration = 0;
            int tempCount = 0;
            bool isChanged = false;
            DateTime tempDateTime = new DateTime();
            while (running && cgRunning)
            {
                try
                {
                    if (caspar.IsConnected && caspar.Channels.Count > 0)
                    {
                        LogProcess.AddLog("Bật đồ họa Popup & Logo");
                        int tempTotalSec = 0;
                        DateTime tempTempTime = new DateTime();
                        List<Objects.PlayList> tempLstObj = new List<Objects.PlayList>();
                        foreach (var t in bsPlayList.List as BindingList<Objects.PlayList>)
                        {
                            var tempTime = new DateTime(int.Parse(t.GioPhat.Substring(6, 4)), int.Parse(t.GioPhat.Substring(3, 2)), int.Parse(t.GioPhat.Substring(0, 2)), int.Parse(t.GioPhat.Substring(11, 2)), int.Parse(t.GioPhat.Substring(14, 2)), int.Parse(t.GioPhat.Substring(17, 2)));
                            try
                            {
                                if (isOnTime(tempTime))
                                {
                                    if (tempFileName != t.FileName || tempLstObj.Count == 0)
                                    {
                                        tempTempTime = tempTime;
                                        tempFileName = t.FileName;
                                        tempLstObj.Add(t);
                                        tempTotalSec += int.Parse(getNumber(t.Duration));
                                    }

                                }
                            }
                            catch { }
                        }
                        if (tempTotalSec != tempTotalDuration || tempLstObj.Count != tempCount || tempDateTime != tempTempTime)
                        {
                            tempTotalDuration = tempTotalSec;
                            tempCount = tempLstObj.Count;
                            tempDateTime = tempTempTime;
                            isChanged = true;
                        }
                        else isChanged = false;
                        if (tempLstObj.Count > 0)
                        {
                            List<int> sleepTime = new List<int>();
                            foreach (var temp in tempLstObj)
                            {
                                sleepTime.Add(int.Parse(getNumber(temp.Duration)));
                            }
                            var maxDur = sleepTime.Max();

                            if (isChanged || anyChanged)
                            {
                                threadlock = new List<object>();
                                for (int i = 0; i < tempLstObj.Count; i++)
                                    threadlock.Add(new object());
                                var loopResult = Parallel.For(0, tempLstObj.Count, (i, loopState) =>
                                //for (int i = 0; i < tempLstObj.Count; i++)
                                {
                                    var tempTime = new DateTime(int.Parse(tempLstObj[i].GioPhat.Substring(6, 4)), int.Parse(tempLstObj[i].GioPhat.Substring(3, 2)), int.Parse(tempLstObj[i].GioPhat.Substring(0, 2)), int.Parse(tempLstObj[i].GioPhat.Substring(11, 2)), int.Parse(tempLstObj[i].GioPhat.Substring(14, 2)), int.Parse(tempLstObj[i].GioPhat.Substring(17, 2)));

                                    if (DateTime.Now.TimeOfDay.TotalSeconds - tempTime.TimeOfDay.TotalSeconds <= maxDur)
                                    {
                                        caspar.Channels[AppSetting.Default.Channel].LoadBG(tempLstObj[i].Layer, tempLstObj[i].FileName, true);
                                        caspar.Channels[AppSetting.Default.Channel].Play(tempLstObj[i].Layer);
                                        int tempSleep = int.Parse(getNumber(tempLstObj[i].Duration)) * 1000 - (int)(DateTime.Now.TimeOfDay.TotalMilliseconds - tempTime.TimeOfDay.TotalMilliseconds);
                                        if (tempSleep > 0)
                                        {
                                            anyChanged = false;
                                            lock (threadlock[i])
                                            {
                                                Monitor.Wait(threadlock[i], tempSleep);
                                            }
                                            caspar.Channels[AppSetting.Default.Channel].Stop(tempLstObj[i].Layer);
                                        }
                                    }
                                    else
                                    {
                                        caspar.Channels[AppSetting.Default.Channel].Stop(tempLstObj[i].Layer);
                                        anyChanged = false;
                                        //loopState.Break();
                                        //return;
                                    }
                                });
                            }
                            else Thread.Sleep(500);
                        }
                        else Thread.Sleep(500);

                    }
                    else anyChanged = true;
                }
                catch //(Exception ex)
                {
                    //HDMessageBox.Show(ex.ToString(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    for (int time = 0; running && cgRunning && time < 1000; time += 100)
                        Thread.Sleep(100);
                }

            }

        }
        #region Kiểm tra thời điểm hiện tại thuộc khung giờ nào
        private bool isOnTime(DateTime startTime)
        {
            List<double> timeToSec = new List<double>();
            var nowTime = DateTime.Now;
            if (nowTime.Date != startTime.Date) return false;
            foreach (var i in bsPlayList.List as BindingList<Objects.PlayList>)
            {
                var tempStartTime = new DateTime(int.Parse(i.GioPhat.Substring(6, 4)), int.Parse(i.GioPhat.Substring(3, 2)), int.Parse(i.GioPhat.Substring(0, 2)), int.Parse(i.GioPhat.Substring(11, 2)), int.Parse(i.GioPhat.Substring(14, 2)), int.Parse(i.GioPhat.Substring(17, 2)));
                var tempTime = nowTime.TimeOfDay.TotalSeconds - tempStartTime.TimeOfDay.TotalSeconds;
                if (tempTime >= 0)
                    timeToSec.Add(tempTime);
            }
            if ((nowTime.TimeOfDay.TotalSeconds - startTime.TimeOfDay.TotalSeconds) == timeToSec.Min()) return true;
            return false;
        }
        /// <summary>
        /// Trong trường hợp không có khung giờ nào để phát trong ngày, phát tin trong khung giờ gần nhất của ngày trước đó
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private bool isNoNewNext(DateTime startTime)
        {
            List<double> timeToSec = new List<double>();
            var nowTime = DateTime.Now;
            foreach (var i in bsPlayList.List as BindingList<Objects.PlayList>)
            {
                var tempStartTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, int.Parse(i.GioPhat.Substring(0, 2)), int.Parse(i.GioPhat.Substring(3, 2)), int.Parse(i.GioPhat.Substring(6, 2)));
                var tempTime = nowTime.TimeOfDay.TotalSeconds - tempStartTime.TimeOfDay.TotalSeconds;
                timeToSec.Add(tempTime);
            }
            if ((nowTime.TimeOfDay.TotalSeconds - startTime.TimeOfDay.TotalSeconds) == timeToSec.Min()) return true;
            return false;
        }

        #endregion
        #endregion
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (HDMessageBox.Show("Chắc chắn dừng đồ họa?", "Chú ý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    LogProcess.AddLog("Dừng đồ họa");
                    if (thrLogoPopup != null)
                        thrLogoPopup.Interrupt();
                    cgRunning = false;
                    if (caspar.IsConnected && caspar.Channels.Count > 0)
                    {
                        for (int i = 0; i < threadlock.Count; i++)
                            lock (threadlock[i])
                            {
                                Monitor.Pulse(threadlock[i]);
                            }
                        for (int i = 11; i < 21; i++)
                            caspar.Channels[AppSetting.Default.Channel].Stop(i);
                    }

                    lbStatus.Caption = "Stop";
                    anyChanged = false;
                }
            }
            catch { }
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (cgRunning && HDMessageBox.Show("Chắc chắn khởi động lại?", "Chú ý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LogProcess.AddLog("Khởi động lại đồ họa");

                if (caspar.IsConnected)
                    caspar.SendString("kill");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                var tempPlayListList = new List<object>();
                foreach (var i in gvPlayList.GetSelectedRows())
                {
                    tempPlayListList.Add(gvPlayList.GetRow(i));
                }
                foreach (var j in tempPlayListList)
                {
                    bsPlayList.List.Remove(j);
                }

                (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(playListPath);
            }
            catch { }
        }
        string playListPath = "";
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbLuot.Checked || cbTime.Checked)
                {
                    if ((cbLuot.Checked && numLuot.Value > 0) || (cbTime.Checked && numTime.Value > 0))
                    {
                        for (int j = 0; j < gvFileList.RowCount; j++)
                        {
                            var tempTinDoc = gvFileList.GetRow(j) as Objects.FileList;
                            if (tempTinDoc.Use)
                            {
                                bool isSameLayer = false;
                                foreach (var t in bsPlayList.List as BindingList<Objects.PlayList>)
                                {
                                    if (t.GioPhat == (datePhat.Text + " " + timePhat.Text) && t.Layer == int.Parse(cbLayer.Text))
                                    {
                                        isSameLayer = true;
                                        break;
                                    }
                                }
                                if (!isSameLayer)
                                {
                                    MediaFile mInfo = new MediaFile(Path.GetFullPath(tempTinDoc.FilePath));
                                    bsPlayList.List.Add(new Objects.PlayList()
                                    {
                                        Use = tempTinDoc.Use,
                                        Layer = int.Parse(cbLayer.Text),
                                        GioPhat = datePhat.Text + " " + timePhat.Text,
                                        FileName = Path.GetFileNameWithoutExtension(tempTinDoc.FilePath),
                                        Duration = cbLuot.Checked ? (numLuot.Value * mInfo.duration / 1000).ToString() + " giây" : numTime.Value.ToString() + " giây"
                                    });
                                    tempTinDoc.Use = false;
                                    gvFileList.RefreshData();
                                    SaveFileList();
                                } else
                                    HDMessageBox.Show("Layer bị trùng, chọn layer khác! ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        gvPlayList.ExpandAllGroups();
                        (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(playListPath);
                    }
                    else
                        HDMessageBox.Show("Chưa chọn số lượt hoặc thời gian phát!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    HDMessageBox.Show("Chưa chọn số lượt hoặc thời gian phát!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                HDMessageBox.Show(ex.ToString(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete) btnRemove.PerformClick();
            }
            catch { }
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete) btnRemove.PerformClick();
            }
            catch { }
        }
        #region Drag and Drop
        GridHitInfo downHitInfo = null;
        private void view_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 1 && e.Button == MouseButtons.Left)
            {
                GridHitInfo info;
                Point pt = (sender as GridView).GridControl.PointToClient(Control.MousePosition);
                info = (sender as GridView).CalcHitInfo(pt);

                if (info.InColumn && info.Column.FieldName == "Use")
                {
                    if (getCheckedCount(sender as GridView) == (sender as GridView).RowCount)
                        UnChekAll(sender as GridView);
                    else
                        CheckAll(sender as GridView);
                }

            }
            GridView view = sender as GridView;
            downHitInfo = null;
            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.RowHandle >= 0)
                downHitInfo = hitInfo;
        }
        private void view_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (e.Button == MouseButtons.Left && downHitInfo != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    Rectangle dragRect = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2,
                        downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                    if (!dragRect.Contains(new Point(e.X, e.Y)))
                    {
                        Objects.FileList row = view.GetRow(downHitInfo.RowHandle) as Objects.FileList;
                        Objects.PlayList rowView = view.GetRow(downHitInfo.RowHandle) as Objects.PlayList;
                        if (rowView != null)
                        {
                            view.GridControl.DoDragDrop(rowView, DragDropEffects.Move);
                        }
                        else
                            view.GridControl.DoDragDrop(row, DragDropEffects.Move);
                        downHitInfo = null;
                        DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                    }
                }
            }
            catch { }
        }

        private void grid_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Objects.FileList)) || e.Data.GetDataPresent(typeof(Objects.PlayList)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void grid_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                GridView view = grid.MainView as GridView;
                Objects.FileList row = e.Data.GetData(typeof(Objects.FileList)) as Objects.FileList;
                Objects.PlayList rowView = e.Data.GetData(typeof(Objects.PlayList)) as Objects.PlayList;
                GridHitInfo hitInfo = view.CalcHitInfo(grid.PointToClient(new Point(e.X, e.Y)));
                int targetRow = hitInfo.RowHandle;
                Objects.PlayList tempObj = gvPlayList.GetRow(targetRow) as Objects.PlayList;
                if (row != null)
                {
                    row.Use = true;
                    gvFileList.RefreshData();
                    bsPlayList.List.Add(new Objects.PlayList
                    {
                        Use = row.Use,
                        Layer = int.Parse(cbLayer.Text),
                        GioPhat = tempObj.GioPhat,
                        FileName = Path.GetFileNameWithoutExtension(row.FilePath),
                        Duration = cbLuot.Checked ? numLuot.Value.ToString() + " lượt" : numTime.Value.ToString() + " giây"
                    });
                }
                else
                {
                    bsPlayList.List.Add(new Objects.PlayList
                    {
                        Use = rowView.Use,
                        Layer = rowView.Layer,
                        GioPhat = tempObj.GioPhat,
                        FileName = rowView.FileName,
                        Duration = rowView.Duration
                    });
                    bsPlayList.List.Remove(rowView);
                }
                (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(playListPath);
            }
            catch { }
        }
        #endregion

        private void barBtnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRemove.PerformClick();
        }

        private void cbLuot_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLuot.Checked)
            {
                cbTime.Checked = false;
                numLuot.Enabled = true;
            }
            else numLuot.Enabled = false;
        }

        private void cbTime_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTime.Checked)
            {
                cbLuot.Checked = false;
                numTime.Enabled = true;
            }
            else numTime.Enabled = false;
        }
        private string getNumber(string str)
        {
            var number = Regex.Match(str, @"\d+").Value;

            return number;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = AppSetting.Default.DatabaseFolder;
            saveFileDialog1.Filter = "Lịch files (*.lich)|*.lich|Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<Objects.PlayList> lstPlaylist = new List<Objects.PlayList>();
                foreach (var i in bsPlayList.List as BindingList<Objects.PlayList>)
                {
                    lstPlaylist.Add(i);
                }
                var tempPath = Path.Combine(AppSetting.Default.DatabaseFolder, saveFileDialog1.FileName);
                if (tempPath.EndsWith(".xlsx"))
                {
                    Objects.lichXlsx lich = new Objects.lichXlsx();
                    lich.DataSource = lstPlaylist;
                    lich.CreateDocument();
                    lich.ExportToXlsx(tempPath);
                }
                else
                    (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(tempPath);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(AppSetting.Default.DatabaseFolder))
            {
                HDMessageBox.Show("Thư mục lưu trữ lịch không tồn tại", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Lịch files (*.lich)|*.lich|Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                open.InitialDirectory = AppSetting.Default.DatabaseFolder;
                if (open.ShowDialog() == DialogResult.OK)
                    txtTenLich.Text = open.SafeFileName;
                if (open.FileName.EndsWith(".lich"))
                {
                    bsPlayList.Clear();
                    try
                    {
                        var tins = Utils.GetObject<List<Objects.PlayList>>(open.FileName);
                        foreach (var tin in tins)
                            bsPlayList.Add(tin);
                        (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(open.FileName);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        using (var fs = new FileStream(open.FileName, FileMode.Open, FileAccess.Read))
                        {
                            var wb = new XSSFWorkbook(fs);
                            var sh = (XSSFSheet)wb.GetSheetAt(0);

                            bsPlayList.Clear();
                            for (int i = 1; i <= sh.LastRowNum; i++)
                            {
                                try
                                {
                                    Objects.PlayList temp = new Objects.PlayList();
                                    temp.GioPhat = sh.GetRow(i).GetCell(0).StringCellValue;
                                    temp.Layer = (int)sh.GetRow(i).GetCell(1).NumericCellValue;
                                    temp.FileName = sh.GetRow(i).GetCell(2).StringCellValue;
                                    temp.Duration = sh.GetRow(i).GetCell(3).StringCellValue;

                                    bsPlayList.Add(temp);
                                    (bsPlayList.List as BindingList<Objects.PlayList>).ToList().SaveObject(playListPath);

                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch //(Exception ex)
                    {
                        //HDMessageBox.Show(ex.ToString(), "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bsPlayList.List.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}