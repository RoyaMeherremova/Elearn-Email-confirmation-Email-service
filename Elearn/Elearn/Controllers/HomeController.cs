using Elearn.Data;
using Elearn.Models;
using Elearn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Elearn.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> slider = await _context.Slider.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Course> courses = await _context.Courses.Include(m=>m.Images).Include(m => m.CourseHost).Where(m => !m.SoftDelete).OrderByDescending(m=>m.Id).ToListAsync();

            IEnumerable<Event> events = await _context.Events.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<News> news = await _context.News.Include(m => m.Publisher).Where(m => !m.SoftDelete).ToListAsync();
            HomeVM model = new()
            {
                Slider = slider,
                Courses= courses,
                Events= events,
                News= news
            };
            return View(model);
        }

       

   
    }
}