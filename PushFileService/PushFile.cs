using PushFileService.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace PushFileService
{
    public partial class PushFile : ServiceBase
    {
        public PushFile()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service has been started");
            string pathWatcher = System.Configuration.ConfigurationManager.AppSettings["PathWatcher"];

            var watcher = new FileSystemWatcher(pathWatcher);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.txt|*.jpg";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            WriteLog($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            WriteLog(value);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            WriteLog($"Deleted: {e.FullPath}");

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            WriteLog(string.Format("Renamed file: '{0}' to '{1}'", e.OldFullPath, e.FullPath));
        }

        public void OnError(object sender, ErrorEventArgs e)
        {
            WriteLog(String.Format("Message {0} ms elapsed.", e.GetException()));
        }


        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteLog(String.Format("{0} ms elapsed.", 1));
        }

        protected override void OnStop()
        {
            WriteLog("Service has been stopped.");
        }


        public void WriteLog(string logMessage, bool addTimeStamp = true)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = String.Format("{0}\\{1}_{2}.txt",
                path,
                ServiceName,
                DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture)
                );

            if (addTimeStamp)
                logMessage = String.Format("[{0}] - {1}\r\n",
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
                    logMessage);

            File.AppendAllText(filePath, logMessage);
        }
    }
}