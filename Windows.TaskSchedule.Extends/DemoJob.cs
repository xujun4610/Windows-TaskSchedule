using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule.Extends
{
    public class DemoJob : DefaultLogger, IJob
    {
        
        public void Excute()
        {
            DateTime date = new DateTime();
            date = DateTime.Now;
            Logger.Info(date);
            var str =  System.Configuration.ConfigurationManager.AppSettings["test"].ToString();
            var rand = new Random();
            var a = Convert.ToInt64(rand.NextDouble() * Math.Pow(10,15));
            Logger.Info(str+"-"+a.ToString());
        }
    }
}
