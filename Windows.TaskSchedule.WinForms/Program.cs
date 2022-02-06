using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Windows.TaskSchedule.WinForms
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormSvcConsole());
            }
            catch (Exception ex)
            {
                var msg = string.Concat(ex.Message, "\r\n", "堆栈消息：\r\n", ex.StackTrace);

                MessageBox.Show(msg, "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }
    }
}
