using APICatalogo.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] ICollection<IFormFile> files)
        {
            var (success, message) = await _fileService.ProcessFilesAsync(files);

            if (success)
            {
                return Ok(message);
            }
            else
            {
                return BadRequest(message);
            }
        }
    }
}
