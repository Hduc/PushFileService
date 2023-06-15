using PushFileService.MinIO;
using System;
using System.Globalization;
using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Collections.Generic;
using PushFileService.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using System.Net.NetworkInformation;

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
            //GetProduct();
            productModels = new List<ProductModel>()
            {
                new ProductModel() {
                    Id="460905000",
                    HoaVan="Ivory White",
                    Bo="Daisy IFP",
                    ThuongHieu="ML"
                },
                new ProductModel()
                {
                    Id="461028000",
                    Name="Set of 10 pcs",
                    HoaVan="Ivory White",
                    Bo="Daisy IFP",
                    ThuongHieu="ML"
                },
                new ProductModel()
                {
                    Id="461128000",
                    HoaVan="Ivory White",
                    Bo="Daisy IFP",
                    ThuongHieu="ML"
                }
            };
                
        }
        private void GetProduct()
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectDB"];
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string query = @"
            select Distinct(sp.MAKH) ,sp.Name,hv.Name,b.Name,b.MaThuongHieu 
            from [Y_B_DMHH] sp
            join [Y_B_DM hoa van] hv on hv.MaHoaVan = sp.MaHoaVan
            join [Y_B_DMNHOMCT] b on sp.MaNhomCt = b.MaNhomCtF
                ";
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
            bool runScanService = ConfigurationManager.AppSettings["runScanService"] == "true";
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

            string[] files = Directory.GetFiles(directory, "*.jpg");

            foreach (string file in files)
            {
                WriteLog(file);
                string fileName = Path.GetFileName(file);
                string idSanPham = fileName.Split('.').First().Split(' ').First();
                var product = productModels.FirstOrDefault(x => x.Id == idSanPham);
                product.Id = idSanPham;
                product.FileName= fileName;

                client.UploadFile(file, product).RunSynchronously();
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
