using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Uploaders
{
    public interface IFileUploader
    {
        Task<string> UploadFileAsync(string fileName, string contentType, Stream content);
    }
}