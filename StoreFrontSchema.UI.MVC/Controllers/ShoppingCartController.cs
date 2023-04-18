using Microsoft.AspNetCore.Mvc;
using StoreFrontSchema.DATA.EF.Models;//Grants access to GadgetStoreContext and Product classes
using Microsoft.AspNetCore.Identity;//Grants access to identity classes and methods
using StoreFrontSchema.UI.MVC.Models;//Grants access to CartItemViewModel class
using Newtonsoft.Json;//Grants access to JSON classes and methods (Serialization & Deserialization)

namespace StoreFrontSchema.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        //Fields/Props
        private readonly StoreFrontSchemaContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //CTOR

        public ShoppingCartController(StoreFrontSchemaContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult AddToCart(int id)
        //{
        //    Dictionary<int, CartItemViewModel> shoppingCart = null;

        //    var sessionCart = HttpContext.Session.GetString("cart");
        //}


    }
}
