using Elearn.Data;
using Elearn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;

        public NewsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<News> news = await _context.News.Include(m=>m.Publisher).Where(m => !m.SoftDelete).ToListAsync();

            return View(news);
        }
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();

            News? news = await _context.News.Include(m => m.Publisher).FirstOrDefaultAsync(m => m.Id == id);

            if (news == null) return NotFound();
            return View(news);
        }



        [HttpGet]

        public IActionResult Create()
        {

            return View();
        }


     
    }
}
