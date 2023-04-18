using System;
using System.Collections.Generic;

namespace StoreFrontSchema.DATA.EF.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;
        public string? OrderDate { get; set; }
        public string? ShipDate { get; set; }
        public string? ShipTo { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipNation { get; set; }
        public string? ShipZip { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
