namespace Elearn.Models
{
    public class CourseHost:BaseEntity
    {

        public string? Image { get; set; }
        public string? FullName { get; set; }

        public ICollection<Course> Course { get; set; }
    }
}
