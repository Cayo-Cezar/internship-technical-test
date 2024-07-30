using APICatalogo.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileProcessingApi.Core.Interfaces
{
    public interface IArquivoRepository
    {
        Task<IEnumerable<Arquivos>> GetUnprocessedArquivosAsync();
        Task AddArquivoAsync(Arquivos arquivo);
        Task UpdateArquivoAsync(Arquivos arquivo);
    }
}
