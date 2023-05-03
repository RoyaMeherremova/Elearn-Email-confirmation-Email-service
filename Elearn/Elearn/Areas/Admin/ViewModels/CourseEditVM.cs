using Elearn.Models;
using System.ComponentModel.DataAnnotations;

namespace Elearn.Areas.Admin.ViewModels
{
    public class CourseEditVM
    {
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public int Sale { get; set; }

        public int CourseHostId { get; set; }
        public List<IFormFile> Photos { get; set; }
        public List<CourseImage> Images { get; set; }
       
    }
}
