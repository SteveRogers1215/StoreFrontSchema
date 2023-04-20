using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFrontSchema.DATA.EF.Models;
using StoreFrontSchema.UI.MVC.Utilities;
using X.PagedList;

namespace StoreFrontSchema.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WeaponsController : Controller
    {
        private readonly StoreFrontSchemaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public WeaponsController(StoreFrontSchemaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Weapons
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var storeFrontSchemaContext = _context.Weapons.Include(w => w.Category).Include(w => w.Smith);
            return View(await storeFrontSchemaContext.ToListAsync());
        }

        #region Implementing Paged List

        //1) Install package for X.PagedList.Mvc.Core
        //      - Open Package Manger Console -> select the UI project -> install-package x.pagedlist.mvc.core
        //2) Add using statements and update model declaration in the View
        //3) Add using statement to Controller
        //4) Add param to Controller Action
        //5) Add page size variable in Action
        //6) Update return statement in Controller Action
        //7) Add Counter in View
        // 8) Create a new CSS file in wwwroot/css named 'PagedList.css'
        //      - NOTE: We may need to go to the package's NuGet page and copy the CSS directly
        //      - X.PagedList CSS file link (go here to copy the code):https://github.com/dncuug/X.PagedList/blob/master/examples/Example.Website/wwwroot/css/PagedList.css
        // 9) Add a <link> to the _Layout referencing 'PagedList.css'

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> TiledWeapons(string searchTerm, int categoryId = 0, int page = 1)
        {
            //if (User.IsInRole("Admin"))
            //{
            //    var weapons = _context.Weapons
            //    .Include(w => w.Category)
            //    .Include(w => w.Smith)
            //    .Include(w => w.OrderProducts);
            //    return View(await weapons.ToListAsync());
            //}
            //else
            //{
            //    var weapons = _context.Weapons.Where(w => w.IsDiscontinued == false && w.InStock > 0)
            //    .Include(w => w.Category)
            //    .Include(w => w.Smith)
            //    .Include(w => w.OrderProducts);
            //    return View(await weapons.ToListAsync());

            //}
            int pageSize = 6;


            var weapons = _context.Weapons.Where(p => !p.IsDiscontinued)
               .Include(p => p.Category)
               .Include(p => p.Smith)
               .Include(p => p.OrderProducts).ToList();

            #region Optional Category Filter

            //Create a ViewData object to send a list of categories to View. This is the same as we see in products/create 4/20/23

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");


            //Create a ViewBag variable to persist selected category
            ViewBag.Category = 0;

            if (categoryId != 0)
            {
                weapons = weapons.Where(p => p.CategoryId == categoryId).ToList();

                //Repopulate the dropdown with current category selected.
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);

                ViewBag.Category = categoryId;

            }

            #endregion



            #region Optional Search Filter

            if (!String.IsNullOrEmpty(searchTerm))
            {
                weapons = weapons.Where(p =>
                    p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Smith.SmithName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                    p.ProductDescription.ToLower().Contains(searchTerm.ToLower())).ToList();

                //ViewBag for the total number of results
                ViewBag.NbrResults = weapons.Count;

                //ViewBag for search term
                ViewBag.SearchTerm = searchTerm;

            }
            else
            {
                ViewBag.NbrResults = null;
                ViewBag.SearchTerm = null;
            }

            #endregion

            //return View(weapons);
            return View(weapons.ToPagedList(page, pageSize));

        }

        // GET: Weapons/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .Include(w => w.Category)
                .Include(w => w.Smith)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // GET: Weapons/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SmithId"] = new SelectList(_context.Vendors, "SmithId", "SmithName");
            return View();
        }

        // POST: Weapons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,InStock,BackOrder,IsDiscontinued,CategoryId,SmithId,ProductImage,ImageFile")] Weapon weapon)
        {
            if (ModelState.IsValid)
            {
                #region File Upload - Create w/Image Utility
                if (weapon.ImageFile != null)
                {
                    // process the file

                    //check the file type
                    // - retrieve extension of file
                    string ext = Path.GetExtension(weapon.ImageFile.FileName);

                    // - Create a list of valid extensions
                    string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    // - Check the extension versus list of valid extensions
                    if (validExts.Contains(ext.ToLower()) && weapon.ImageFile.Length < 4_194_303)
                    {
                        // Generate unique file name
                        weapon.ProductImage = Guid.NewGuid() + ext;

                        //Save to web server(here its wwwroot/images)
                        // - retrieve the path to web root
                        string webRootPath = _webHostEnvironment.WebRootPath;

                        // - Create a variable for image path
                        string fullImagePath = webRootPath + "/images/";

                        //Create a memory stream to read the image into the web server memory
                        using (var memoryStream = new MemoryStream())
                        {
                            await weapon.ImageFile.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                // Now , send the img to ImageUtiltiy to be resized and saved
                                //Need 5 args for this to resize our image
                                //1- int max img size
                                //2- int max thumbnail
                                //3-string full path where file will be saved
                                //4-(Image) an image
                                //5-(string) filename

                                int maxImageSize = 500;
                                int maxThumbSize = 100;

                                ImageUtility.ResizeImage(fullImagePath, weapon.ProductImage, img, maxImageSize, maxThumbSize);
                            }
                        }
                    }

                }
                else
                {
                    // assign a default image
                    // IF no image uploaded, assing default file name
                    //Will also need to DL default image and name it "noImage.png" place it in wwwroot/images.
                    weapon.ProductImage = "noImage.jpg";
                }
                #endregion

                _context.Add(weapon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TiledWeapons));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", weapon.CategoryId);
            ViewData["SmithId"] = new SelectList(_context.Vendors, "SmithId", "SmithName", weapon.SmithId);
            return View(weapon);
        }

        // GET: Weapons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", weapon.CategoryId);
            ViewData["SmithId"] = new SelectList(_context.Vendors, "SmithId", "SmithName", weapon.SmithId);
            return View(weapon);
        }

        // POST: Weapons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,InStock,BackOrder,IsDiscontinued,CategoryId,SmithId,ProductImage,ImageFile")] Weapon weapon)
        {
            if (id != weapon.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region File Upload - Edit w/Image Utility
                //retain old image file name and reuse or delete if new file is uploaded
                string oldImageName = weapon.ProductImage;

                //Check user uploaded a file
                if (weapon.ImageFile != null)
                {
                    // Check file extension
                    string ext = Path.GetExtension(weapon.ImageFile.FileName);

                    // List valid extensions
                    string[] validExts = { ".jpg", ".png", ".jpeg", ".gif" };

                    // Check file extension against valid extensions and file size
                    if (validExts.Contains(ext.ToLower()) && weapon.ImageFile.Length < 4_194_303)
                    {
                        // generate a unique file name
                        weapon.ProductImage = Guid.NewGuid() + ext;

                        // define file path and save image
                        string fullPath = _webHostEnvironment.WebRootPath + "/images/";

                        //delete old image
                        if (oldImageName != "noImage.jpg" && oldImageName != null)
                        {
                            ImageUtility.Delete(fullPath, oldImageName);
                        }

                        //Save new image to  webroot
                        using (var memoryStream = new MemoryStream())
                        {
                            await weapon.ImageFile.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(fullPath, weapon.ProductImage, img, maxImageSize, maxThumbSize);
                            }
                        }
                    }
                }
                #endregion


                try
                {
                    _context.Update(weapon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeaponExists(weapon.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", weapon.CategoryId);
            ViewData["SmithId"] = new SelectList(_context.Vendors, "SmithId", "SmithName", weapon.SmithId);
            return View(weapon);
        }

        // GET: Weapons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Weapons == null)
            {
                return NotFound();
            }

            var weapon = await _context.Weapons
                .Include(w => w.Category)
                .Include(w => w.Smith)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (weapon == null)
            {
                return NotFound();
            }

            return View(weapon);
        }

        // POST: Weapons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Weapons == null)
            {
                return Problem("Entity set 'StoreFrontSchemaContext.Weapons'  is null.");
            }
            var weapon = await _context.Weapons.FindAsync(id);
            if (weapon != null)
            {
                _context.Weapons.Remove(weapon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeaponExists(int id)
        {
          return (_context.Weapons?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
