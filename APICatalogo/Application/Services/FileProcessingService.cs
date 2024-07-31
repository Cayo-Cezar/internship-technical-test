using FileProcessingApi.Core.Interfaces;
using Tesseract;


namespace APICatalogo.Application.Services
{
    public class FileProcessingService
    {
        private readonly IArquivoRepository _arquivoRepository;
        private readonly string _tempFolder;
        private readonly string _tessdataPath;

        public FileProcessingService(
            IArquivoRepository arquivoRepository)
        {
            _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            _tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            _arquivoRepository = arquivoRepository;
        }

        public async Task ProcessFiles()
        {
            var arquivos = await _arquivoRepository
                .GetUnprocessedArquivosAsync();

            foreach (var arquivo in arquivos)
            {
                try
                {
                    string ocrText = string.Empty;

                    using (var ocrEngine = new TesseractEngine(_tessdataPath, "por", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(arquivo.ArchivePath))
                        {
                            using (var page = ocrEngine.Process(img))
                            {
                                ocrText = page.GetText();
                            }
                        }
                    }

                    arquivo.Content = ocrText;
                }
                catch (Exception ex)
                {
                    arquivo.Erro = $"Erro no OCR: {ex.Message}";
                }

                await _arquivoRepository.UpdateArquivoAsync(arquivo);
            }
        }
    }
}
