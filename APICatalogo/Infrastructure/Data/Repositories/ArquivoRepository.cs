using APICatalogo.Core.Entities;
using APICatalogo.Infrastructure.Data;
using FileProcessingApi.Core.Entities;
using FileProcessingApi.Core.Interfaces;
using FileProcessingApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileProcessingApi.Infrastructure.Repositories
{
    public class ArquivoRepository : IArquivoRepository
    {
        private readonly AppDbContext _context;

        public ArquivoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Arquivos>> GetUnprocessedArquivosAsync()
        {
            return await _context.Arquivos
                .Where(a => string.IsNullOrEmpty(a.Content) && string.IsNullOrEmpty(a.Erro))
                .ToListAsync();
        }

        public async Task AddArquivoAsync(Arquivos arquivo)
        {
            await _context.Arquivos.AddAsync(arquivo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArquivoAsync(Arquivos arquivo)
        {
            _context.Arquivos.Update(arquivo);
            await _context.SaveChangesAsync();
        }
    }
}
