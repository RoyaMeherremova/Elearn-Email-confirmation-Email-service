using Elearn.Models;
using System.Collections;

namespace Elearn.ViewModel
{
    public class HomeVM
    {
        public IEnumerable<Slider> Slider { get; set; }

        public IEnumerable<Course> Courses { get; set; }

        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<News> News { get; set; }
    }
}


