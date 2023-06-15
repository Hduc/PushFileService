using Minio;
using PushFileService.Extensions;
using PushFileService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace PushFileService.MinIO
{
    public class Client
    {
        private MinioClient minioClient;
        private string bucketName = "san-pham";
        public void Inital(string server, int port, string accessKey, string secretKey, bool ssl)
        {
            minioClient = new MinioClient()
                .WithEndpoint(server, port)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(ssl)
                .Build();
        }

        public void Inital()
        {
            string server = ConfigurationManager.AppSettings["server"];
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            string accessKey = ConfigurationManager.AppSettings["accessKey"];
            string secretKey = ConfigurationManager.AppSettings["secretKey"];
            bool ssl = ConfigurationManager.AppSettings["ssl"] == "true";

            minioClient = new MinioClient()
               .WithEndpoint(server, port)
               .WithCredentials(accessKey, secretKey)
               .WithSSL(ssl)
               .Build();
        }
        public async Task UploadFile(string path, ProductModel product)
        {
            string savePath = StringExtension
                .GenerateFilePath(product.ThuongHieu, product.Bo, product.HoaVan, product.Name, product.Id, "image", product.FileName);
            Helper.WriteLog(savePath);
            bool found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                Helper.WriteLog("Create bucketName:san-pham");
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(savePath)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType("application/octet-stream");
            var result = await minioClient.PutObjectAsync(args);
            Helper.WriteLog($"success url new:{result.ObjectName}");
        }
    }
}
