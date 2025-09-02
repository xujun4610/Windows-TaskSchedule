using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Quartz;
using Windows.TaskSchedule.Extends;
using System.Threading;

namespace Windows.TaskSchedule.Utility
{
    public class ScheduleFactory : DefaultLogger
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Jobs.config");
        private static readonly XDocument Doc = XDocument.Load(ConfigPath);
        /// <summary>
        /// windows 服务名称
        /// </summary>
        public static readonly string ServerName = Doc.Element("Jobs").Attribute("serverName").Value;
        /// <summary>
        /// Windows 服务友好显示名称
        /// </summary>
        public static readonly string DisplayName = Doc.Element("Jobs").Attribute("displayName").Value;
        /// <summary>
        /// windows 服务描述
        /// </summary>
        public static readonly string Description = Doc.Element("Jobs").Attribute("description").Value;

        #region const 专区
        private const string P_WORK_DIR = "workDir";

        private const string P_JOB_TYPE = "type";
        private const string P_EXE_PATH = "exePath";
        /// <summary>
        /// 有效期（单位：sec）,过期停止任务
        /// </summary>
        private const string P_EXP_SEC = "expireSecond";
        /// <summary>
        /// 是否运行于沙河中
        /// </summary>
        private const string P_RUN_SAND = "runInSandbox";
        /// <summary>
        /// xml中 true false
        /// </summary>
        private const string P_FALSE_STR = "false", P_TRUE_STR = "true";
        /// <summary>
        /// enabled 是否启用
        /// </summary>
        private const string P_ENABLED_FLAG = "enabled";


        #endregion

        private static List<JobObject> _jobs = new List<JobObject>();

        public void Start()
        {
            Logger.Debug("服务开始启动.");
            ThreadPool.SetMinThreads(50, 50);
            _jobs = GetJobs();
            BatchProcess(_jobs);
            int enabledJobs = _jobs.Count(c => c.Enabled == true);
            int disabledJobs = _jobs.Count(c => c.Enabled == false);
            Logger.Debug("共找到【{0}】个任务,启用【{1}】个任务，停用【{2}】个任务", _jobs.Count, enabledJobs, disabledJobs);
            Logger.Debug("当前服务运行目录:【{0}】.", AppDomain.CurrentDomain.BaseDirectory);
            Logger.Debug("服务启动成功.");
        }

        public void Stop()
        {
            foreach (var job in _jobs)
            {
                if (job.Sandbox != null)
                {
                    job.Sandbox.Dispose();
                }
            }

            Logger.Debug("服务停止.");
        }

        #region Private Methods

        /// <summary>
        /// 启动轮询任务
        /// </summary>
        /// <param name="jobs"></param>
        private void BatchProcess(List<JobObject> jobs)
        {
            foreach (var job in jobs)
            {
                Task jobTask = new Task(() =>
                {
                    while (true)
                    {
                        if (!job.Running)
                        {
                            job.Running = true;
                            if (job.Enabled) //当启用的时候（配置文件设置为Enabled = "true"时）
                            {
                                RunJob(job);
                            }
                            job.Running = false;
                        }
                        System.Threading.Thread.Sleep(800);
                    }
                });

                jobTask.Start();
            }
        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <returns></returns>
        private List<JobObject> GetJobs()
        {
            List<JobObject> result = new List<JobObject>();
            var jobs = Doc.Element("Jobs").Elements("Job");
            foreach (var p in jobs)
            {
                JobObject job = new JobObject();
                if (p.Attributes().Any(o => o.Name.ToString() == P_JOB_TYPE) && p.Attributes().Any(o => o.Name.ToString() == P_EXE_PATH))
                {
                    throw new Exception("job中不能同时配制“type”与“exePath”");
                }
                if (p.Attributes().Any(o => o.Name.ToString() == P_JOB_TYPE))
                {
                    job.JobType = JobTypeEnum.Assembly;
                    string assembly = p.Attribute(P_JOB_TYPE).Value.Split(',')[1];
                    string className = p.Attribute(P_JOB_TYPE).Value.Split(',')[0];
                    //沙盒模式（true 开启，false 关闭-默认值）
                    string runInSandbox = P_FALSE_STR;
                    if (p.Attributes().Any(o => o.Name.ToString() == P_RUN_SAND))
                    {
                        runInSandbox = p.Attribute(P_RUN_SAND).Value;
                    }

                    string config = assembly + ".dll.config";
                    string workDir = "Bin";
                    if (p.Attributes().Any(o => o.Name.ToString() == P_WORK_DIR))
                    {
                        workDir = p.Attribute(P_WORK_DIR).Value;
                        if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, workDir, config)))
                        {
                            config = null;
                        }
                    }

                    //创建sandbox
                    job.Sandbox = Sandbox.Create(config, workDir);
                    job.AssemblyName = assembly;
                    job.TypeName = className;

                }
                if (p.Attributes().Any(o => o.Name.ToString() == P_EXP_SEC))
                {
                    job.ExpireSecond = int.Parse(p.Attribute(P_EXP_SEC).Value);
                }
                if (p.Attributes().Any(o => o.Name.ToString() == P_EXE_PATH))
                {
                    job.JobType = JobTypeEnum.Exe;
                    job.ExePath = p.Attribute(P_EXE_PATH).Value.Replace("${basedir}", AppDomain.CurrentDomain.BaseDirectory);
                    if (p.Attributes().Any(o => o.Name.ToString() == "arguments"))
                    {
                        job.Arguments = p.Attribute("arguments").Value;
                    }
                }

                job.Name = p.Attribute("name").Value;
                job.CornExpress = p.Attribute("cornExpress").Value;
                if (!CronExpression.IsValidExpression(job.CornExpress))
                {
                    throw new Exception(string.Format("Job名称：{0}，corn表达式：{1}不正确。", job.Name, job.CornExpress));
                }
                if (p.Attributes().Any(c => c.Name.ToString().Equals(P_ENABLED_FLAG)))
                {
                    string enableValue = p.Attribute(P_ENABLED_FLAG).Value ?? string.Empty;
                    job.Enabled = enableValue.ToLower() == P_TRUE_STR.ToLower() ? true : false;
                }
                /*
                if (p.Attributes().Any(c => c.Name.ToString().Equals(P_ENABLED_FLAG)))
                {
                    string enableValue = p.Attribute(P_ENABLED_FLAG).Value;
                    job.Enabled = enableValue.ToLower() == P_TRUE_STR.ToLower() ? true : false;
                }
                */
                result.Add(job);
            }
            return result;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="job">任务信息</param>
        private void RunJob(JobObject job)
        {
            try
            {
                if (CornUtility.Trigger(job.CornExpress, DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))))
                {
                    if (!job.Triggering)
                    {
                        job.Triggering = true;
                        switch (job.JobType)
                        {
                            case JobTypeEnum.Assembly:
                                job.Sandbox.Execute(job.AssemblyName, job.TypeName, "Excute", null);
                                break;
                            case JobTypeEnum.Exe:
                                using (var process = new Process())
                                {
                                    bool hasExpireSetting = job.ExpireSecond.HasValue;
                                    if (string.IsNullOrWhiteSpace(job.Arguments))
                                    {
                                        process.StartInfo = new ProcessStartInfo(job.ExePath);
                                    }
                                    else
                                    {
                                        process.StartInfo = new ProcessStartInfo(job.ExePath, job.Arguments);
                                    }
                                    process.Start();
                                    if (hasExpireSetting) //如果设置了最长运行时间，到达时间时，自动中止进程
                                    {
                                        bool result = process.WaitForExit(job.ExpireSecond.Value * 1000);
                                        if (!result)
                                        {
                                            Logger.Info("任务【{0}】因长时间：{1}秒未返回运行状态，程序已自动将其Kill.", job.Name, job.ExpireSecond);
                                            process.Kill();
                                        }
                                    }
                                    else
                                    {
                                        process.WaitForExit();
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    job.Triggering = false;
                }
            }
            catch (Exception ex) //不处理错误，防止日志爆长
            {
                Logger.Error(string.Format("加载依赖时候出现异常!\r\n消息：{0}\r\n堆栈：{1}", ex.Message, ex.StackTrace));
                try
                {
                    if (job.JobType == JobTypeEnum.Assembly)
                    {
                        job.Sandbox.Execute(job.AssemblyName, job.TypeName, "OnError", ex);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                    Logger.Error(string.Format("加载处理异常依赖时候出现异常!\r\n消息：{0}\r\n堆栈：{1}", e.Message, e.StackTrace));
                }
            }
        }
        #endregion
    }
}
