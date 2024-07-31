using APICatalogo.Core.Entities;
using FileProcessingApi.Core.Interfaces;

namespace APICatalogo.Application.Services
{
    public class FileService
    {
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _tempFolder;
        private readonly IArquivoRepository _arquivoRepository;

        public FileService(
            IArquivoRepository arquivoRepository)
        {
            _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            _arquivoRepository = arquivoRepository;

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }
        }

        public async Task<(bool success, string message)> ProcessFilesAsync(ICollection<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return (false, "Nenhum arquivo enviado.");
                }

                var existingFilesCount = Directory.GetFiles(_tempFolder).Length;

                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension))
                    {
                        return (false, $"O arquivo {file.FileName} não é uma imagem válida.");
                    }

                    var archiveId = existingFilesCount + 1;
                    existingFilesCount++;

                    var fileName = Path.GetRandomFileName() + fileExtension;
                    var filePath = Path.Combine(_tempFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var arquivo = new Arquivos
                    {
                        Id = Guid.NewGuid(),
                        ArchiveId = archiveId,
                        CreateAt = DateTime.Now,
                        ProcessAt = DateTime.Now,
                        ArchivePath = filePath,
                        Content = null,
                        Erro = null
                    };

                    await _arquivoRepository.AddArquivoAsync(arquivo);
                }

                return (true, "Arquivos de imagem recebidos e aguardando processamento.");
            }
            catch (Exception ex)
            {
                return (false, $"Erro durante o upload: {ex.Message}");
            }
        }
    }
}
