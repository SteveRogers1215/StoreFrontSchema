﻿using System;
using System.Collections.Generic;

namespace StoreFrontSchema.DATA.EF.Models
{
    public partial class UserDetail
    {
        public UserDetail()
        {
            Orders = new HashSet<Order>();
        }

        public string UserId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Nation { get; set; }
        public string? Zip { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
