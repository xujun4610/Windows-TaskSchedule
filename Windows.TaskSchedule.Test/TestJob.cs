using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.TaskSchedule.Extends;

namespace Windows.TaskSchedule.Test
{
    public class TestJob : DefaultLogger, IJob
    {
        public void Excute()
        {
            Logger.Info(DateTime.Now.ToShortDateString());
        }
    }
}
