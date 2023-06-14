using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void UploadFile()
        {

        }
    }
}
