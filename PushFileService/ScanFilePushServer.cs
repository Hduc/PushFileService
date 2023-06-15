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
using PushFileService.Extensions;

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
            GetProduct();
            //productModels = new List<ProductModel>()
            //{
            //    new ProductModel() {
            //        Id="460905000",
            //        HoaVan="Ivory White",
            //        Bo="Daisy IFP",
            //        ThuongHieu="Minh Long I"
            //    },
            //    new ProductModel()
            //    {
            //        Id="461028000",
            //        Name="Set of 10 pcs",
            //        HoaVan="Ivory White",
            //        Bo="Daisy IFP",
            //        ThuongHieu="Minh Long I"
            //    },
            //    new ProductModel()
            //    {
            //        Id="461128000",
            //        HoaVan="Ivory White",
            //        Bo="Daisy IFP",
            //        ThuongHieu="Minh Long I"
            //    }
            //};

        }
        private void GetProduct()
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectDB"];
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            string query = @"
            select 
                Distinct(sp.MAKH),
                sp.Name,
                b.Name Bo,
                hv.Name HoaVan,
                (SELECT top 1 TenThuongHieu from [Y_B_DMThuongHieu] where MaThuongHieu = b.MaThuongHieu) ThuongHieu
                from [Y_B_DMHH] sp
                join [Y_B_DM hoa van] hv on hv.MaHoaVan = sp.MaHoaVan
                join [Y_B_DMNHOMCT] b on sp.MaNhomCt = b.MaNhomCt
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
            Helper.WriteLog("ProdocutModel Count:"+productModels.Count());
            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        protected override void OnStart(string[] args)
        {
            string pathWatcher = ConfigurationManager.AppSettings["PathWatcher"];
            Helper.WriteLog("start ScanFilePushServer");
            client.Inital();
            ProcessFiles(pathWatcher);
        }


        private void ProcessFiles(string directory)
        {

            string[] files = Directory.GetFiles(directory, "*.jpg");

            foreach (string file in files)
            {
                Helper.WriteLog(file);
                string fileName = Path.GetFileName(file);
                string idSanPham = fileName.Split('.').First().Split(' ').First();
                var product = productModels.FirstOrDefault(x => x.Id == idSanPham);
                product.Id = idSanPham;
                product.FileName = fileName;

                try
                {
                    Task.Run(() => client.UploadFile(file, product));
                }
                catch (Exception ex)
                {
                    Helper.WriteLog($"Error:{file}: {ex.Message}");
                }
            }

            string[] subDirectories = Directory.GetDirectories(directory);

            foreach (string subDirectory in subDirectories)
            {
                ProcessFiles(subDirectory); // Đệ quy vào các thư mục con
            }

            Helper.WriteLog("stop ScanFilePushServer");
            //this.OnStop();
            //this.Dispose();
        }

        protected override void OnStop()
        {
            Helper.WriteLog("stop ScanFilePushServer");
        }
    }
}
