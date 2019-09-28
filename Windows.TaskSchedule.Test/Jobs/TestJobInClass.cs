using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.TaskSchedule.Extends;

namespace Windows.TaskSchedule.Test.Jobs
{
    public class TestJobInClass  :DefaultLogger, IJob
    {
        public void Excute()
        {
            var date1 = new DateTime();
            DoMuchJobs();
            var date2 = new DateTime();

            Logger.Info(string.Format("运行秒数：{0}", (date2-date1).TotalSeconds.ToString() ));

        }

        private void DoMuchJobs(BoTest t = null)
        {
            var bo = default(BoTest);
            if (null == t)
            {
                bo = new BoTest();
                bo.TestProperty1 = "fuck!";
                bo.TestProperty2 = 10086;
                bo.TestProperty3 = "test";
                    
            }
            Logger.Info("执行很多工作……"+ bo.TestProperty2 );
        }
    }
}
