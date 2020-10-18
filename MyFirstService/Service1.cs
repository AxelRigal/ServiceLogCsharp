using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MyFirstService
{
    public partial class Service1 : ServiceBase
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Output";
        System.Timers.Timer Timer = new System.Timers.Timer();
        int Interval = 1000;

        public Service1()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("{0} ms elapsed");
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at" + DateTime.Now);
            Timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            Timer.Interval = Interval;
            Timer.Enabled = true;
            
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at" + DateTime.Now);
        }

        private void WriteToFile(string logMessage, bool addTimeStamp = true)
        {

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = String.Format("{0}\\{1}_{2}.txt",
                path,
                ServiceName,
                DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture));
            if(addTimeStamp)
                logMessage = String.Format("[{0}] - {1}",
                    DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture));
            File.AppendAllText(filePath, logMessage);
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Copies file to another directory.
        }

        private void watch()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }
    }
}
