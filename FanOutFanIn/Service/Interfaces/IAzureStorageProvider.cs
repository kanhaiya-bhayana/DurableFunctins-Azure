using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanOutFanIn.Service.Interfaces
{
    public interface IAzureStorageProvider
    {
        Task<Uri> UploadBlobFromStreamAsync(Stream stream, string blobName, string containerName);
    }
}
