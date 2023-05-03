using Elearn.Models;
using System.ComponentModel.DataAnnotations;

namespace Elearn.Areas.Admin.ViewModels
{
    public class CourseCreateVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Price { get; set; }
        [Required]
        public int Sale { get; set; }
        [Required]
        public List<IFormFile> Photos { get; set; }

        public int CourseHostId { get; set; }



    }
}
