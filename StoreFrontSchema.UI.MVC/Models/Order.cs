using System;
using System.Collections.Generic;

namespace StoreFrontSchema.UI.MVC.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string? OrderDate { get; set; }
        public string? ShipDate { get; set; }
        public string? ShipTo { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipNation { get; set; }
        public string? ShipZip { get; set; }

        public virtual UserDetail? User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
