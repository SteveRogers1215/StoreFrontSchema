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
        [AllowAnonymous]
        public async Task<IActionResult> TiledWeapons()
        {
            if (User.IsInRole("Admin"))
            {
                var weapons = _context.Weapons
                .Include(w => w.Category).Include(w => w.Smith).Include(w => w.OrderProducts);
                return View(await weapons.ToListAsync());
            }
            else
            {
                var weapons = _context.Weapons.Where(w => w.IsDiscontinued == false && w.InStock > 0)

                .Include(w => w.Category).Include(w => w.Smith).Include(w => w.OrderProducts);
                return View(await weapons.ToListAsync());
                
            }
            
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
