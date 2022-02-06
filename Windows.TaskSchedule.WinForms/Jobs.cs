using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.TaskSchedule.WinForms
{
    public class Jobs
    {
        public string JobName { get; set; }
        public string Type { get; set; }
        public string CronExpress { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
