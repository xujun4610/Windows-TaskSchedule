using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.TaskSchedule.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                TestJob t = new TestJob();
                t.Excute();
            }
            catch (Exception ex)
            {
                
                throw;
            }

        }
    }
}
