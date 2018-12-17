using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TeamHGS_SFDCLookup.Services
{
    public interface IAzureStorageService
    {
        Task<string> StoreAndGetFile(string filename, string containerName, IFormFile image);
        Task StoreFile(string filename, IFormFile image);
        Task StoreFile(string filename, string containerName, IFormFile image);
        Task<string> GetFile(string filename, string containerName);
        Task DeleteFile(string filename, string containerName);
    }
}