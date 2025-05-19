using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UploadService.Models
{
    public class UploadFileRequest
    {
        [Required]
        [FromForm(Name = "files")]
        public List<IFormFile> Files { get; set; } = default!;
    }
}
