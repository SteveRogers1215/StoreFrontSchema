using System;
using System.Collections.Generic;

namespace StoreFrontSchema.UI.MVC.Models
{
    public partial class Category
    {
        public Category()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryDescription { get; set; }

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
