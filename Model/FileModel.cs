using System.ComponentModel.DataAnnotations;

namespace net.Model
{
    public class FileModel
    {
        [Required]
        public required string FileName { get; set; }
        
        [Required]
        public required IFormFile file { get; set; } 
    }
}
