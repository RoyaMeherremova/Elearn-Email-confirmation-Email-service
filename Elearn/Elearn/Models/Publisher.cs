using System.Collections;

namespace Elearn.Models
{
    public class Publisher:BaseEntity
    {
        public string? FullName { get; set; }

        public ICollection<News> News { get; set; }
    }
}
