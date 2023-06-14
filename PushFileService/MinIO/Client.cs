using Minio;
using System.Configuration;

namespace PushFileService.MinIO
{
    public class Client
    {
        private MinioClient minioClient;
        public Client(string server, int port, string accessKey, string secretKey, bool ssl)
        {
            minioClient = new MinioClient()
                .WithEndpoint(server, port)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(ssl)
                .Build();
        }

        public Client()
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
        public void UploadFile(string path)
        {

        }
    }
}
