using PushFileService.MinIO;
using System;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Collections.Generic;
using PushFileService.Models;
using System.Data.SqlClient;

namespace PushFileService
{
    partial class ScanFilePushServer : ServiceBase
    {
        private Client client;
        private SqlConnection sqlConnection;
        private List<ProductModel> productModels = new List<ProductModel>();
        public ScanFilePushServer()
        {
            InitializeComponent();
            client = new Client();
            string connectionString = ConfigurationManager.AppSettings["ConnectDB"];
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string query = "SELECT Id, Name FROM YourTable";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                ProductModel model = new ProductModel();
                model.Id = reader.GetString(0);
                model.Name = reader.GetString(1);
                model.Bo = reader.GetString(2);
                model.HoaVan = reader.GetString(3);
                model.ThuongHieu = reader.GetString(4);
                productModels.Add(model);
            }
            sqlConnection.Close();
            sqlConnection.Dispose();

        }

        protected override void OnStart(string[] args)
        {
            bool runScanService = ConfigurationManager.AppSettings["runScanService"] =="true";
            if (runScanService)
            {
                string pathWatcher = ConfigurationManager.AppSettings["PathWatcher"];
                WriteLog("start ScanFilePushServer");
                ProcessFiles(pathWatcher);
            }
            else
            WriteLog("Not run ScanFilePushServer => app.config key 'runScanService'=true");
        }


        private void ProcessFiles(string directory)
        {

            string[] files = Directory.GetFiles(directory);

            foreach (string file in files)
            {
                WriteLog(file);
                client.UploadFile(file);
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
