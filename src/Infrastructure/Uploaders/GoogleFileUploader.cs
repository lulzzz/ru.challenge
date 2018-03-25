using Google.Cloud.Storage.V1;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Uploaders
{
    public class GoogleFileUploader : IFileUploader
    {
        private readonly string _songsBucket;
        private readonly string _imagesBucket;
        private readonly StorageClient _storageClient;

        public GoogleFileUploader(
            string songsBucket,
            string imagesBucket,
            StorageClient storageClient)
        {
            _songsBucket = songsBucket;
            _imagesBucket = imagesBucket;
            _storageClient = storageClient;
        }

        public async Task<string> UploadFileAsync(string fileName, string contentType, Stream content)
        {
            var bucket = contentType.Contains("image")
                ? _imagesBucket
                : _songsBucket;

            var res = await _storageClient.UploadObjectAsync(bucket, fileName, contentType, content);
            return res.MediaLink;
        }
    }
}