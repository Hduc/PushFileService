using PushFileService.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PushFileService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            bool runScanService = ConfigurationManager.AppSettings["runScanService"] == "true";
            if (runScanService)
            {
                ServicesToRun =new ServiceBase[]{ new ScanFilePushServer() };
                Helper.WriteLog("Not run PushFile => app.config key 'runScanService' =false");
            }
            else {
                ServicesToRun = new ServiceBase[]{ new ScanFilePushServer() };
                Helper.WriteLog("Not run ScanFilePushServer => app.config key 'runScanService' =true");
            }
            ServiceBase.Run(ServicesToRun);
        }
    }
}
