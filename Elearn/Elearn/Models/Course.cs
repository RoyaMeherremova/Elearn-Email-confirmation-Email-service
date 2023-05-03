using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearn.Models
{
    public class Course:BaseEntity
    {
        public string? Name { get; set; }

        public string? Descriprion { get; set; }

        public decimal Price { get; set; }

        public int Sale { get; set; }

        public ICollection<CourseImage> Images { get; set; }

        public int CourseHostId { get; set; }

        public CourseHost CourseHost { get; set; }




    }
}
