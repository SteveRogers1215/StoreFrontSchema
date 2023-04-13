using System;
using System.Collections.Generic;

namespace StoreFrontSchema.DATA.EF.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Weapons = new HashSet<Weapon>();
        }

        public int SmithId { get; set; }
        public string SmithName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Nation { get; set; }
        public string? Zip { get; set; }

        public virtual ICollection<Weapon> Weapons { get; set; }
    }
}
