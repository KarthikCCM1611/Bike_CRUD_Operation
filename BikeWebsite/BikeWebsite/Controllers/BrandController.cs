using BikeWebsite.Data;
using BikeWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BikeWebsite.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandController(ApplicationDBContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Brand> brands = _dbContext.Brands.ToList();
            return View(brands);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var files = HttpContext.Request.Form.Files;
            if(files.Count() > 0)
            {
                //string webRootPath = _webHostEnvironment.WebRootPath;
                //string newFileName = Guid.NewGuid().ToString();
                //var upload = Path.Combine(webRootPath, @"Images\BrandLogo");
                //var extension = Path.GetExtension(files[0].FileName);

                //using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                //{
                //    files[0].CopyTo(fileStream);
                //}
                //brand.BrandLogo = @"\Images\BrandLogo\" + newFileName + extension;
                brand.BrandLogo = SaveImageAndGetImagePath(files);
            }
            TempData["Success"] = "Record Created Successfully";
            _dbContext.Brands.Add(brand);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == id);
            return View(brand);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == id);
            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                brand.BrandLogo = UpdateImageAndGetImagePath(files, brand);
            }
            else
            {
                var objFromDb = _dbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);
                if(objFromDb.BrandLogo != null)
                {
                    brand.BrandLogo = objFromDb.BrandLogo;
                }
            }
            _dbContext.Brands.Update(brand);
            _dbContext.SaveChanges();
            TempData["Warning"] = "Record Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var brand = _dbContext.Brands.FirstOrDefault(x => x.Id == id);
            return View(brand);
        }

        [HttpPost]
        public IActionResult Delete(Brand brand)
        {
            if(brand.BrandLogo != null)
            {
                DeletePhysicalImage(brand);
            }
            _dbContext.Brands.Remove(brand);
            _dbContext.SaveChanges();
            TempData["Error"] = "Record Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private string SaveImageAndGetImagePath(IFormFileCollection files)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string newFileName = Guid.NewGuid().ToString();
            var upload = Path.Combine(webRootPath, @"Images\BrandLogo");
            var extension = Path.GetExtension(files[0].FileName);

            using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }
            string imagePath = @"\Images\BrandLogo\" + newFileName + extension;
            return imagePath;
        }

        private string UpdateImageAndGetImagePath(IFormFileCollection files, Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string newFileName = Guid.NewGuid().ToString();
            var upload = Path.Combine(webRootPath, @"Images\BrandLogo");
            var extension = Path.GetExtension(files[0].FileName);

            // delete old image
            var objFromDb = _dbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);
            if (objFromDb.BrandLogo != null)
            {
                var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStream);
            }
            string imagePath = @"\Images\BrandLogo\" + newFileName + extension;
            return imagePath;
        }

        private void DeletePhysicalImage(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            // delete old image
            var objFromDb = _dbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);
            if (objFromDb.BrandLogo != null)
            {
                var oldImagePath = Path.Combine(webRootPath, objFromDb.BrandLogo.Trim('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

        }

    }
}
