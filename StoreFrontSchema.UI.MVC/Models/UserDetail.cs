using System;
using System.Collections.Generic;

namespace StoreFrontSchema.UI.MVC.Models
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Nation { get; set; }
        public string? Zip { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
