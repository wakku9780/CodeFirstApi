using CodeFirstApi.Interfaces;
using CodeFirstApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CodeFirstApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentDBContext studentDB;
        private readonly IRedisCache _redisCache;

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(StudentDBContext studentDB,IRedisCache redisCache)
        {
            this.studentDB = studentDB;
           _redisCache=redisCache;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _redisCache.GetCacheData<List<Student>>("student");
            if (result == null)
            {
                result = await studentDB.Students.ToListAsync();
                bool isCached = await _redisCache.SetCacheData("student", result, DateTimeOffset.Now.AddMinutes(2));
            }
            return View(result);
            //var stdData = await studentDB.Students.ToListAsync();

            //return View(stdData);
        }

        public IActionResult Create()//HttpGet ke liye chalega
        {
            List<SelectListItem> Gender = new List<SelectListItem>()
            {
                new SelectListItem{Value= "Male",Text ="Male"},
                new SelectListItem{Value="Female",Text ="female"},
            };
            ViewBag.Gender = Gender;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student std)
        {
            if (ModelState.IsValid)
            {
                await studentDB.Students.AddAsync(std);
                await studentDB.SaveChangesAsync();
                TempData["Insert_Success"]="Inserted";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || studentDB==null)
            {
                return NotFound();

            }
            var stdData = await studentDB.Students.FirstOrDefaultAsync(x=>x.Id==id );
            if (stdData == null)
            {
                return NotFound();
            }

            return View(stdData);
        }

        public async Task<IActionResult> Edit(int? id)//HttpGet ke liye chalega
        {
            List<SelectListItem> Gender = new List<SelectListItem>()
            {
                new SelectListItem{Value= "Male",Text ="Male"},
                new SelectListItem{Value="Female",Text ="female"},
            };
            ViewBag.Gender = Gender;

            if (id == null || studentDB == null)
            {
                return NotFound();

            }
            var stdData = await studentDB.Students.FindAsync(id);
            if (stdData == null)
            {
                return NotFound();
            }
            return View(stdData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Student std)//HttpGet ke liye chalega
        {
            if (id != std.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                studentDB.Update(std);
                await studentDB.SaveChangesAsync();
                TempData["Update_Success"] = "Updated";
                return RedirectToAction("Index", "Home");
            }
            return View(std);
        }

        public async Task<IActionResult> Delete(int? id)//HttpGet ke liye chalega
        {
            if (id == null || studentDB == null)
            {
                return NotFound();

            }
            var stdData = await studentDB.Students.FirstOrDefaultAsync(x => x.Id == id);
            if (stdData == null)
            {
                return NotFound();
                
            }
            return View(stdData);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deleteconfirmed(int? id)
        {
            var stdData = await studentDB.Students.FindAsync(id);
            if(stdData != null)
            {
                studentDB.Students.Remove(stdData);
            }
            await studentDB.SaveChangesAsync();
            TempData["Deleted_Success"] = "Deleted";
            return RedirectToAction("Index", "Home");

        }

        public IActionResult Privacy()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
