using System;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace PushFileService
{
    public partial class PushFile : ServiceBase
    {
        Timer Timer = new Timer();
        int Interval = 10000;
        public PushFile()
        {
            InitializeComponent();
            this.ServiceName = "Minhlong-PushFileService";
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service has been started");
            Timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            Timer.Interval = Interval;// cấu hình thời gian chạy
            Timer.Enabled = true;
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            // 1 tiếng chạy 1 lần
            // kiểm tra đúng giờ được chạy thì mới run
            int hour = int.Parse( System.Configuration.ConfigurationManager.AppSettings["HourRun"] ?? "18");
            if(DateTime.Now.Hour == hour)
            {

            }
            WriteLog(String.Format("{0} ms elapsed.", Interval));
        }

        protected override void OnStop()
        {
            Timer.Stop();
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