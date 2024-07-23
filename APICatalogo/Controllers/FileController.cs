﻿using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract; 

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string _tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
        private readonly string _tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
        private readonly AppDbContext _context;

        public FileController(AppDbContext context)
        {
            _context = context;

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }

            if (!Directory.Exists(_tessdataPath))
            {
                Directory.CreateDirectory(_tessdataPath);
             
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] ICollection<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("Nenhum arquivo enviado.");
                }

                // Contar o número de arquivos existentes na pasta
                var existingFilesCount = Directory.GetFiles(_tempFolder).Length;

                foreach (var file in files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExtension) || !_permittedExtensions.Contains(fileExtension))
                    {
                        return BadRequest($"O arquivo {file.FileName} não é uma imagem válida.");
                    }

                    // Incrementar o número de arquivos para definir o ArchiveId
                    var archiveId = existingFilesCount + 1;
                    existingFilesCount++;

                    var fileName = Path.GetRandomFileName() + fileExtension;
                    var filePath = Path.Combine(_tempFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Processamento do OCR usando Tesseract
                    string ocrText = string.Empty;
                    string ocrError = null;

                    try
                    {
                        using (var ocrEngine = new TesseractEngine(_tessdataPath, "por", EngineMode.Default))
                        {
                            using (var img = Pix.LoadFromFile(filePath))
                            {
                                using (var page = ocrEngine.Process(img))
                                {
                                    ocrText = page.GetText();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ocrError = $"Erro no OCR: {ex.Message}";
                    }

                    var arquivo = new Arquivos
                    {
                        Id = Guid.NewGuid(), // Mantém o ID como Guid
                        ArchiveId = archiveId,
                        CreateAt = DateTime.Now,
                        ProcessAt = DateTime.Now,
                        ArchivePath = filePath,
                        Content = ocrText,
                        Erro = ocrError
                    };

                    _context.Arquivos.Add(arquivo);
                }

                await _context.SaveChangesAsync();

                return Ok("Arquivos de imagem recebidos e processados com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro durante o upload: {ex.Message}");
            }
        }
    }
}