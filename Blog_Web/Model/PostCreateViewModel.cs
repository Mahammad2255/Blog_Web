using BlogApp.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_Web.Model
{
    public class PostCreateViewModel
    {
        public int PostId{ get; set; }
        public bool isActive { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content{ get; set; }

        [Required]
        public string? Description{ get; set; }
        [Required]

        public IFormFile? BlogImageFile { get; set; }
        [Required]

        public string? Url { get; set; }
        [Required]

        public List<Tag> Tags { get; set; } = new();
        public string? GetImage { get; set; }
    }
}
