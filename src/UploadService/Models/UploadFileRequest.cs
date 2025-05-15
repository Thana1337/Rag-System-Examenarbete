using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace UploadService.Models
{
    public class UploadFileRequest
    {
        [Required]
        [Display(Name = "file")]
        public IFormFile File { get; set; } = default!;
    }
}
