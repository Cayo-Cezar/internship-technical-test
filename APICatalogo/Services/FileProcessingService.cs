using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;

namespace APICatalogo.Services
{
    public class FileProcessingService
    {
        private readonly AppDbContext _context;
        private readonly string _tempFolder;
        private readonly string _tessdataPath;

        public FileProcessingService(AppDbContext context)
        {
            _context = context;
            _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            _tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
        }

        public async Task ProcessFiles()
        {
            var arquivos = await _context.Arquivos
                .Where(a => string.IsNullOrEmpty(a.Content) && string.IsNullOrEmpty(a.Erro))
                .ToListAsync();

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

                _context.Arquivos.Update(arquivo);
            }

            await _context.SaveChangesAsync();
        }
    }
}
