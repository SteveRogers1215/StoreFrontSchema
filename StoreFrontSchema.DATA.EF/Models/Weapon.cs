using System;
using System.Collections.Generic;

namespace StoreFrontSchema.DATA.EF.Models
{
    public partial class Weapon
    {
        public Weapon()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public short? InStock { get; set; }
        public short? BackOrder { get; set; }
        public bool IsDiscontinued { get; set; }
        public int CategoryId { get; set; }
        public int SmithId { get; set; }
        public string? ProductImage { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Vendor? Smith { get; set; } 
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
