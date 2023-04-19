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
            //Retrieve cart contents
            var sessionCart = HttpContext.Session.GetString("cart");

            //Create a shell for local version C#
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //Check to see if the session cart was null

            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //The user either has not put things in cart OR they have removed all items
                //So, set the shopping cart to an empty object
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;

                //Deserialize cart contents from Json to C#
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }
            return View(shoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            var sessionCart = HttpContext.Session.GetString("cart");
            //Check to see if the session object exists
            //If not we want to instantiate a new local collection
            if (String.IsNullOrEmpty(sessionCart))
            {
                //If the session didnt exist yet lets instantiate a collection as a new object
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                //Cart already exists -- transfer existing cart items from session into the local cart
                //DeserializeObject() is a method that converts JSON to C#. We MUST tell this method which C# class to convert the JSON into.(here -- Dictionary<int, CIVM>)
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);


            }

            //Add the newly selected product to the cart
            //Retrieve the product from DB
            Weapon weapon = _context.Weapons.Find(id);

            //Instantiate the CIVM object so we can add to the cart
            CartItemViewModel civm = new CartItemViewModel(1, weapon); //Adds ONE of the selected product to the cart

            //If the product was already in the cart, increase the quantity by 1
            //Otherwise, just add new item to cart
            if (shoppingCart.ContainsKey(weapon.ProductId))
            {
                //Update the quantity property
                shoppingCart[weapon.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(weapon.ProductId, civm);
            }

            //Update the session version of the cart
            //Take the local version and this time we need to serialize it as JSON
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);

            //Assign that value to our session
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int id)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert the JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Remove the cart item
            shoppingCart.Remove(id);

            //If there are no remaining items in cart, remove it from session
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            //Otherwise, update the session variable with the new local cart contents
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }

            return RedirectToAction("Index");



        }
        public IActionResult UpdateCart(int productId, int qty)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Deserialize from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart =
                JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Update the qty from the specific dictionary key
            shoppingCart[productId].Qty = qty;

            //Update the session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }
    }
}
