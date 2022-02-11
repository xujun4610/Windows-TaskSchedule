using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Timers;

namespace Windows.TaskSchedule.WinForms
{
    public partial class FormSvcConsole : Form
    {

        private System.Timers.Timer serviceTimer;

        /// <summary>
        /// 使用记事本打开
        /// </summary>
        private const string APPNAME = "notepad.exe";

        private ServiceConsoleContents sc;

        public FormSvcConsole()
        {
            try
            {
                InitializeComponent();
                #region 注入信息
                var jsonCfg = ServiceConsoleContents.LoadConfig().LoadServiceName();
                this.sc = jsonCfg;
                //加载jobs
                this.sc.LoadJobs();
                LoadJobs2UI();
                #endregion
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.Message, "\r\n", "堆栈消息：\r\n", ex.StackTrace);
                MessageBox.Show(msg, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void bAbout_Click(object sender, EventArgs e)
        {
            var msg = new StringBuilder();
            msg.AppendLine("WS-ServiceConsole" + " Ver." + ServiceConsoleContents.VersionInfo);
            msg.AppendLine(ServiceConsoleContents.CopyrightInfo);
            msg.AppendLine("");
            msg.AppendLine("Website" + "\t" + ServiceConsoleContents.PersonalHomePage);
            msg.AppendLine("E-mail" + "\t" + ServiceConsoleContents.EmailAddress);


            MessageBox.Show(msg.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sc.LoadLogs();
            this.cbblog.DataSource = null;
            this.cbblog.DataSource = null == sc.LogList ? new List<string>() : sc.LogList;

        }

        private void bopen_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.cbblog.SelectedItem.ToString()))
                {
                    var logFile = Path.Combine(sc.LogFolderPath, this.cbblog.SelectedItem.ToString());
                    Process.Start(APPNAME, logFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = string.Format("Service Console - Ver.{0}", ServiceConsoleContents.VersionInfo);
                if (null != sc)
                {
                    this.txtService.Text = sc.ServiceDisplayName;
                    this.txtService.Tag = sc.ServiceUniqueName;

                    this.WindowsServiceTrigger(sc.ServiceUniqueName, null);
                }


                if (null == serviceTimer)
                {
                    serviceTimer = new System.Timers.Timer();
                    serviceTimer.Enabled = true;
                    serviceTimer.Interval = 30000; //执行间隔时间,单位为毫秒; 这里实际间隔为30seconds  
                    serviceTimer.Start();
                    serviceTimer.Elapsed += new System.Timers.ElapsedEventHandler(ServiceTimer_Tick);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Tick事件异常：{0}", ex.Message));
            }

        }

        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            this.WindowsServiceTrigger(sc.ServiceUniqueName, null);
        }

        private void bViewReport_Click(object sender, EventArgs e)
        {
            //FormReportMain f = new FormReportMain();
            //f.Show();
        }

        private void bstart_Click(object sender, EventArgs e)
        {
            var s = this.txtService.Tag.ToString();
            var s_chs = this.txtService.Text;
            try
            {
                if (this.WindowsServiceTrigger(s, true))
                {
                    this.notifyIcon1.ShowBalloonTip(5000, "WS-ServiceConsole", string.Format("服务【{0}】已启动！", s_chs), ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("服务【{0}】启动时出现问题。\n[{1}]", s_chs, ex.Message));
            }

        }

        private void bstop_Click(object sender, EventArgs e)
        {
            var s = this.txtService.Tag.ToString();
            var s_chs = this.txtService.Text;
            try
            {
                if (this.WindowsServiceTrigger(s, false))
                {
                    this.notifyIcon1.ShowBalloonTip(5000, "WS-ServiceConsole", string.Format("服务【{0}】已停止！", s_chs), ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(string.Format("服务【{0}】停止时出现问题。\n[{1}]", s_chs, ex.Message));
            }

        }

        private bool WindowsServiceTrigger(string serviceName, bool? IsStart)
        {
            try
            {
                var serviceController1 = new ServiceController();
                serviceController1.ServiceName = serviceName;
                serviceController1.MachineName = ".";
                if (serviceController1.Status == ServiceControllerStatus.Running)
                {
                    if (!IsStart.HasValue)
                    {
                        this.SetState("已启动", Color.Green, Color.White);
                    }
                    else
                    {
                        if (!IsStart.Value)
                        {
                            serviceController1.Stop();
                            this.SetState("未启动", Color.Orange, Color.Black);
                        }
                    }


                    return true;
                }
                else
                {
                    if (!IsStart.HasValue)
                    {
                        this.SetState("未启动", Color.Orange, Color.Black);
                    }
                    else
                    {
                        if (IsStart.Value)
                        {
                            serviceController1.Start();
                            this.SetState("已启动", Color.Green, Color.White);
                        }
                    }
                    return true;
                }
            }
            catch (Win32Exception w32ex)
            {
                this.SetState("服务开关异常", Color.Red, Color.White);
                throw w32ex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void SetState(string stateText, Color backcolor, Color forecolor)
        {
            this.txtState.Text = stateText;
            this.txtState.BackColor = backcolor;
            this.txtState.ForeColor = forecolor;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(sc.WSInstallPath))
                {
                    var logFile = Path.Combine( sc.WSInstallPath);
                    Process.Start(logFile);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            this.DoShowOnNofityBar();
        }
        /// <summary>
        /// 在通知区域显示
        /// </summary>
        private void DoShowOnNofityBar()
        {
            this.Hide();
            this.notifyIcon1.ShowBalloonTip(60, "提示", "窗口已最小化到通知栏！", ToolTipIcon.Info);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowUI();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadJobs2UI();
        }

        private void LoadJobs2UI()
        {
            try
            {
                if (null != sc)
                    sc.LoadJobs();
                var j = ServiceConsoleContents.GetJobs();
                this.lvJobs.Items.Clear();
                foreach (var job in j)
                {
                    var lvi = new ListViewItem(new[] { job.JobName, job.Type, job.CronExpress, job.Enabled.ToString() });
                    //lvi.SubItems.Add(job.JobName);
                    //lvi.SubItems.Add(job.Type);
                    //lvi.SubItems.Add(job.CronExpress);
                    this.lvJobs.Items.Add(lvi);
                }

                //this.lvJobs.Items.AddRange()
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowUI();
        }

        private void ShowUI()
        {
            if (System.Windows.Forms.Application.OpenForms.Cast<Form>().Count(c => c.GetType().Equals(typeof(FormSvcConsole))) > 0)
            {
                if (!this.Visible)
                    this.Show();
            }
        }

        /// <summary>
        /// 手工执行,进入维护工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(sc.WSInstallPath))
                {
                    var logFile = Path.Combine( sc.MaintenancePath);
                    Process.Start(logFile);
                }
                else
                {
                    MessageBox.Show("未能设置维护工具路径！");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }
        /// <summary>
        /// 操作说明
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bHelpNote_Click(object sender, EventArgs e)
        {

            try
            {

                if (!string.IsNullOrEmpty(sc.HelpFilePath))
                {
                    var path = Path.Combine(sc.HelpFilePath);
                    Process.Start(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 中间表初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bInitlize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请向开发咨询并索要，如何手工执行SQL脚本！");
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                MessageBox.Show("修改完毕并保存后，请重启服务以生效！！！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (null != sc)
                {
                    var jobFile = Path.Combine(sc.JobFilePath);
                    Process.Start(APPNAME, jobFile);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void FormSvcConsole_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.DoShowOnNofityBar();
            }
        }

        /// <summary>
        /// 打开日志目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(sc.LogFolderPath))
                {
                    var logFile = Path.Combine( sc.LogFolderPath);
                    Process.Start(logFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
