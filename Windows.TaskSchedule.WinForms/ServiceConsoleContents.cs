using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Reflection;

namespace Windows.TaskSchedule.WinForms
{
    internal class ServiceConsoleContents
    {
        private static string JsonFileName = "ServiceConsoleConfig.json";
        public static string AuthorName = "Niko Studio";
        public static string EmailAddress = "xujun4610@outlook.com";
        public static string PersonalHomePage = "https://github.com/xujun4610";
        public static string CopyrightInfo = string.Format("Copyright © {0} {1}", DateTime.Today.Year, AuthorName);
        public static string VersionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static string _ServiceUniqueName;
        public string ServiceUniqueName { get { return _ServiceUniqueName; } set { _ServiceUniqueName = value; } }

        private static string _ServiceDisplayName;
        public string ServiceDisplayName { get { return _ServiceDisplayName; } set { _ServiceDisplayName = value; } }

        private static string _LogFolderPath;
        public string LogFolderPath { get { return _LogFolderPath; } set { _LogFolderPath = value; } }

        private static string _JobFilePath;
        public string JobFilePath { get { return _JobFilePath; } set { _JobFilePath = value; } }

        private static string _MaintenancePath;
        /// <summary>
        /// 维护工具执行路径
        /// </summary>
        public string MaintenancePath { get { return _MaintenancePath; } set { _MaintenancePath = value; } }

        private static string _HelpFilePath;
        /// <summary>
        /// 帮助文件执行路径
        /// </summary>
        public string HelpFilePath { get { return _HelpFilePath; } set { _HelpFilePath = value; } }
        
        private static string _WSInstallPath;
        /// <summary>
        /// windows服务安装路径
        /// </summary>
        public string WSInstallPath { get { return _WSInstallPath; } set { _WSInstallPath = value; } }


        private static IList<string> _LogList;

        public IList<string> LogList { get { return _LogList; } set { _LogList = value; } }


        private static IList<Jobs> _Jobs;
        public IList<Jobs> Jobs { get { return _Jobs; } set { _Jobs = value; } }


        public ServiceConsoleContents LoadServiceName()
        {
            try
            {
                if (!string.IsNullOrEmpty(_JobFilePath))
                {
                    var doc = XDocument.Load(Path.Combine(BaseDirectory, _JobFilePath));
                    this.ServiceUniqueName = doc.Element("Jobs").Attribute("serverName").Value;
                    this.ServiceDisplayName = doc.Element("Jobs").Attribute("displayName").Value;
                }
                return this;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void LoadJobs()
        {
            try
            {
                if (!string.IsNullOrEmpty(_JobFilePath))
                {
                    var doc = XDocument.Load(Path.Combine(BaseDirectory, _JobFilePath));
                    var ele_jobs = doc.Element("Jobs").Elements("Job");
                    var jobs_list = ele_jobs.ToList().Select(c => new Jobs()
                    {
                        JobName = c.Attribute("name").Value,
                        Type = c.Attribute("type").Value,
                        CronExpress = c.Attribute("cornExpress").Value,
                        Enabled = bool.Parse(string.IsNullOrEmpty(c.Attribute("enabled").Value) ? true.ToString() : c.Attribute("enabled").Value)
                    }).ToList();
                    if (null != this.Jobs)
                        this.Jobs = jobs_list;
                    else
                        this.Jobs = new List<Jobs>();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void LoadLogs()
        {
            try
            {
                if (!string.IsNullOrEmpty(_LogFolderPath))
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(_LogFolderPath));
                    var listInfo = di.GetFiles("*.log", SearchOption.TopDirectoryOnly).ToList();
                    if (listInfo.Count <= 0)
                    {
                        this.LogList = new List<string>();
                    }
                    else
                    {
                        this.LogList = listInfo.Select(c => c.Name).OrderByDescending(a => a).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static ServiceConsoleContents LoadConfig()
        {
            try
            {
                var cfgContent = string.Empty;
                using (var s = new StreamReader(JsonFileName))
                {
                    cfgContent = s.ReadToEnd();
                }
                var replace = "%basedir%\\";
                var basedir = AppDomain.CurrentDomain.BaseDirectory;
                var obj = JsonConvert.DeserializeObject<ServiceConsoleContents>(cfgContent);
                obj.LogFolderPath = obj.LogFolderPath.Replace(replace, basedir);
                obj.HelpFilePath =  obj.HelpFilePath.Replace(replace, basedir );
                obj.JobFilePath =   obj.JobFilePath.Replace(replace, basedir);
                obj.HelpFilePath =  obj.HelpFilePath.Replace(replace, basedir);
                obj.MaintenancePath = obj.MaintenancePath.Replace(replace, basedir);

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IList<Jobs> GetJobs()
        {
            if (null == _Jobs)
            {
                return new List<Jobs>();
            }
            return _Jobs;
        }

    }
}
