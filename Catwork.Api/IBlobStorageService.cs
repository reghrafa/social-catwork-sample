using System.IO;
using System.Threading.Tasks;

namespace Catwork.Api
{
    public interface IBlobStorageService
    {
        Task<string> WriteProfileImageToBlob(Stream stream, string catId);
    }
}