using Elearn.Data;
using Elearn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;

        public SliderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Slider.Where(m => !m.SoftDelete).ToListAsync();

            return View(sliders);
        }
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            Slider? slider = await _context.Slider.FirstOrDefaultAsync(m => m.Id == id);
            if (slider == null) return NotFound();
            return View(slider);
        }



        [HttpGet]

        public IActionResult Create()
        {

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            Slider? slider = await _context.Slider.FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null) return NotFound();
            return View(slider);

        }
    }
}
