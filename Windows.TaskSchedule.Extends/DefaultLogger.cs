using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using System.Xml;
using NLog.Config;

namespace Windows.TaskSchedule.Extends
{
    public class DefaultLogger<T>  // where T : IJob
    {
        public static string currentassemblyName; ///typeof(DefaultLogger).Assembly.GetName().Name;
        //static Logger logger;
        //public static Configuration appConfig; //dll.config
        static object lockObj = new object();

        public DefaultLogger()
        {
            //if (string.IsNullOrWhiteSpace(currentassemblyName))
            //{
            //    currentassemblyName= AppDomain.CurrentDomain.FriendlyName;
            //}
            InitConfig();
        }
        /// <summary>
        /// 日志记录器
        /// </summary>
        public Logger Logger
        {
            get
            {
                return LoggerStatic.logger;
            }
            private set
            {
                LoggerStatic.logger = value;
            }
        }

        public Configuration AppConfig
        {
            get
            {
                return LoggerStatic.appConfig;
            }
            private set
            {
                LoggerStatic.appConfig = value;
            }
        }
        /// <summary>
        /// 初始化Nlog配置
        /// </summary>
        private void InitConfig()
        {
            try
            {
                currentassemblyName = typeof(T).Module.ToString();
                string LOG_FILE_PATH = ConfigurationManager.AppSettings["LOG_FILE_PATH"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "NLog.config");
                if (!File.Exists(xmlPath)) throw new FileNotFoundException("配置文件不存在！位于：" + xmlPath);
                XmlLoggingConfiguration config = new XmlLoggingConfiguration(xmlPath, false);
                LogManager.Configuration = config;
                LogManager.Configuration.Variables.Add(new KeyValuePair<string, NLog.Layouts.SimpleLayout>("LOG_FILE_PATH", new NLog.Layouts.SimpleLayout(LOG_FILE_PATH)));
                lock (lockObj)
                {
                    if (null == Logger)
                        Logger = LogManager.GetCurrentClassLogger();
                    if (null == AppConfig)
                    {
                        var map = new System.Configuration.ExeConfigurationFileMap();
                        map.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentassemblyName + ".config");
                        AppConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(map, System.Configuration.ConfigurationUserLevel.None);
                        Logger.Info(map.ExeConfigFilename);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
