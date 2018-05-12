﻿using System;
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
    public class DefaultLogger
    {
        static string assemblyName = typeof(DefaultLogger).Assembly.GetName().Name;
        static Logger logger;
        public static Configuration appConfig; //dll.config
        static object lockObj = new object();

        static DefaultLogger()
        {
            InitConfig();
        }
        /// <summary>
        /// 日志记录器
        /// </summary>
        public Logger Logger
        {
            get
            {
                return logger;
            }
        }
        /// <summary>
        /// 初始化Nlog配置
        /// </summary>
        private static void InitConfig()
        {
            if (logger == null)
            {
                string LOG_FILE_PATH = ConfigurationManager.AppSettings["LOG_FILE_PATH"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "NLog.config");
                XmlLoggingConfiguration config = new XmlLoggingConfiguration(xmlPath, false);
                LogManager.Configuration = config;
                LogManager.Configuration.Variables.Add(new KeyValuePair<string, NLog.Layouts.SimpleLayout>("LOG_FILE_PATH", new NLog.Layouts.SimpleLayout(LOG_FILE_PATH)));
                lock (lockObj)
                {
                    if (null == logger)
                        logger = LogManager.GetCurrentClassLogger();
                    if (null == appConfig)
                    {
                        var map = new System.Configuration.ExeConfigurationFileMap();
                        map.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName + ".dll.config");
                        appConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(map, System.Configuration.ConfigurationUserLevel.None);
                    }
                }
            }
        }
    }
}
