using PushFileService.Extensions;
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
using System.Threading.Tasks;

namespace PushFileService
{
    partial class ScanFilePushServer : ServiceBase
    {
        public ScanFilePushServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string pathWatcher = System.Configuration.ConfigurationManager.AppSettings["PathWatcher"];
            WriteLog("start ScanFilePushServer");
            ProcessFiles(pathWatcher);
        }


        private void ProcessFiles(string directory)
        {

            string[] files = Directory.GetFiles(directory);

            foreach (string file in files)
            {
                WriteLog(file);
            }

            string[] subDirectories = Directory.GetDirectories(directory);

            foreach (string subDirectory in subDirectories)
            {
                ProcessFiles(subDirectory); // Đệ quy vào các thư mục con
            }

        }

        protected override void OnStop()
        {
            WriteLog("stop ScanFilePushServer");
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
