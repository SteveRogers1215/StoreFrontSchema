using StoreFrontSchema.DATA.EF.Models;//Grants access to Product
namespace StoreFrontSchema.UI.MVC.Models
{
    public class CartItemViewModel
    {
        public int Qty { get; set; }

        public Weapon Weapon { get; set; }
        
        public CartItemViewModel(int qty, Weapon weapon)
        {
            //Assignment
            Qty = qty;
            Weapon = weapon;
        }
    }
}
