using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Windows.TaskSchedule.Extends
{
    internal static class LoggerStatic
    {
        public static Logger logger;
        public static Configuration appConfig; //dll.config
    }
}
