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
            var str = System.Configuration.ConfigurationManager.AppSettings["hello"];
            Logger.Info(string.Format("{0}-{1}[{2}]", str, "新GUID", Guid.NewGuid().ToString()));
        }
    }
}
