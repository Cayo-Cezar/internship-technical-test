using System.Threading.Tasks;

namespace FileProcessingApi.Core.Interfaces
{
    public interface IFileProcessingService
    {
        Task ProcessFilesAsync();
    }
}
