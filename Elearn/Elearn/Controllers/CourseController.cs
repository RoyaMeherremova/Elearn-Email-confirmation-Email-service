using Elearn.Data;
using Elearn.Models;
using Elearn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearn.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int count = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).Where(m => !m.SoftDelete).CountAsync();

            ViewBag.Count = count;
            IEnumerable<Course> courses = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).Where(m => !m.SoftDelete).Take(3).OrderByDescending(m => m.Id).ToListAsync();

           
            return View(courses);
        }
        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Course> courses = await _context.Courses
                                                .Include(m => m.Images)
                                                .Include(m => m.CourseHost)
                                                .Where(m => !m.SoftDelete)
                                                .Skip(skip)
                                                .Take(3)
                                                .ToListAsync();


            return PartialView("_CoursesPartial",courses);
        }
    }
}
