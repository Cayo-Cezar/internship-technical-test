using APICatalogo.Core.Entities;

namespace FileProcessingApi.Core.Interfaces
{
    public interface IArquivoRepository
    {
        Task<IEnumerable<Arquivos>> GetUnprocessedArquivosAsync();
        Task AddArquivoAsync(Arquivos arquivo);
        Task UpdateArquivoAsync(Arquivos arquivo);
    }
}
