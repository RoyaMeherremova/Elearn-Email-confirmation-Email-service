using Elearn.Areas.Admin.ViewModels;
using Elearn.Data;
using Elearn.Helpers;
using Elearn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
namespace Elearn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CourseController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Course> courses = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).Where(m => !m.SoftDelete).ToListAsync();

            return View(courses);
        }
       
        
        [HttpGet]

        public async Task<IActionResult> Create()
        {
            ViewBag.owners = await GetOwnersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CourseCreateVM model)
        {
            try
            {
                ViewBag.owners = await GetOwnersAsync(); //categorileri gonderirik viewa

                if (!ModelState.IsValid) //bos gelirse inputda nese  seyfeye qaytar
                {
                    return View(model);
                }
                foreach (var photo in model.Photos)     //sekil type yoxla
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }
                    //if (!photo.CheckFileSize(200))  //sekil size yoxla
                    //{
                    //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    //    return View();
                    //}
                }

                List<CourseImage> courseImages = new();  //bir productun bir nece sekli olar deye Sekiler tipinden Liste yiq
                foreach (var photo in model.Photos)  //her gelen seklilleri foreacha sal
                {

                    string fileName = Guid.NewGuid().ToString() + " " + photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);
                    await FileHelper.SaveFileAsync(newPath, photo);

                    CourseImage newCourseImage = new()
                    {
                        Name = fileName 
                    };
                    courseImages.Add(newCourseImage); 

                }
                decimal convertedPrice = decimal.Parse(model.Price); 
                Course course = new()                                                  //ProductCreateVM-de propertini stringden qoymusuqki Replace methodu islesin
                {
                    Name = model.Name,
                    Price = convertedPrice, 
                    Descriprion = model.Description, 
                    CourseHostId = model.CourseHostId, 
                    Images = courseImages, 
                    Sale= model.Sale
                };

                await _context.CourseImages.AddRangeAsync(courseImages); //Listin icinde List elave edende Addrange methodun istifade edirik(Product Image Table-Listdi,butun tablar Listdi!)
                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }


        private async Task<SelectList> GetOwnersAsync()
        {
            IEnumerable<CourseHost> owners = await _context.CourseHosts.Where(m => !m.SoftDelete).ToListAsync();
            return new SelectList(owners, "Id", "FullName");
        }




        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            Course? course = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).FirstOrDefaultAsync(m => m.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Course course = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]  //eyni parametr ve adda actiondu deye ayirmaq ucun basqa ad qoyub ama actiona Name-eyni geyd edirik

        //---------Delete product------------
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            try
            {
               Course course = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).FirstOrDefaultAsync(m => m.Id == id); ;
            foreach (var item in course.Images)   //bir productun bir nece sekli var deye Listi foreache salib ele silirik bir bir
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", item.Name);
                FileHelper.DeleteFile(path);
            }

            _context.Courses.Remove(course); //databazadan silirik.product silinir deye databazadan ona aid olan sekiler ve categoriler silinir avtomatik
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ViewBag.error = ex.Message;
                return View();
            }
         

        }




        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();

            bool result = false;

            CourseImage courseImage = await _context.CourseImages.Where(m => m.Id == id).FirstOrDefaultAsync();


            if (courseImage == null) return NotFound();

            var data = await _context.Courses.Include(m => m.Images).FirstOrDefaultAsync(m => m.Id == courseImage.CourseId);

            if (data.Images.Count > 1)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", courseImage.Name);

                FileHelper.DeleteFile(path);

                _context.CourseImages.Remove(courseImage);

                await _context.SaveChangesAsync();

                result = true;
            }


          

            return Ok(result);

        }







        //-----------EDIT VIEW------------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();
            ViewBag.owners = await GetOwnersAsync();
            Course course = await _context.Courses.Include(m => m.Images).Include(m => m.CourseHost).FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            CourseEditVM model = new()
            {
               Name= course.Name,
               Description = course.Descriprion,
               Price = course.Price.ToString("0.#####"),
                Sale = course.Sale,
               Images = course.Images.ToList(),
                CourseHostId = course.CourseHostId

            };
            return View(model);

        }
        //------------EDIT--------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CourseEditVM updatedCourse)
        {

            try
            {
                if (id == null) return BadRequest();
                ViewBag.owners = await GetOwnersAsync();
                Course dbCourse = await _context.Courses.AsNoTracking().Include(m => m.Images).Include(m => m.CourseHost).FirstOrDefaultAsync(m => m.Id == id);

                if (dbCourse == null) return NotFound();

                if (!ModelState.IsValid) //bos gelirse inputda nese  seyfeye qaytar
                {

                    updatedCourse.Images = dbCourse.Images.ToList();
                    return View(updatedCourse);
                }


                List<CourseImage> courseImages = new();

                if (updatedCourse.Photos is not null)
                {
                    foreach (var photo in updatedCourse.Photos)
                    {
                        if (!photo.CheckFileType("image/"))
                        {
                            ModelState.AddModelError("Photo", "File type must be image");
                            updatedCourse.Images = dbCourse.Images.ToList();
                            return View(updatedCourse);
                        }

                        //if (!photo.CheckFileSize(200))
                        //{
                        //    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        //    updatedCourse.Images = dbCourse.Images.ToList();
                        //    return View(updatedCourse);
                        //}
                    }



                    foreach (var photo in updatedCourse.Photos)
                    {
                        string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                        string path = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);

                        await FileHelper.SaveFileAsync(path, photo);

                        CourseImage courseImage = new()
                        {
                            Name = fileName
                        };

                        courseImages.Add(courseImage);
                    }

                    await _context.CourseImages.AddRangeAsync(courseImages);
                }

                decimal convertedPrice = decimal.Parse(updatedCourse.Price.Replace(".", ","));

                Course newCourse = new()
                {
                    Id = dbCourse.Id,
                    Name = updatedCourse.Name,
                    Price = convertedPrice,
                    Sale = updatedCourse.Sale,
                    Descriprion = updatedCourse.Description,
                    CourseHostId = updatedCourse.CourseHostId,
                    Images = courseImages.Count == 0 ? dbCourse.Images : courseImages

                };


                _context.Courses.Update(newCourse);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                throw;
            }

        }

    }

}

