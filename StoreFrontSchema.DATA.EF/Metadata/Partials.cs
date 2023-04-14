using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFrontSchema.DATA.EF.Models/*.Metadata*/
{
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { }
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }
    [ModelMetadataType(typeof(OrderProductMetadata))]

    public partial class OrderProduct { }
    [ModelMetadataType(typeof(UserDetailMetadata))]

    public partial class UserDetail { }
    [ModelMetadataType(typeof(VendorMetadata))]

    public partial class Vendor { }
    [ModelMetadataType(typeof(WeaponMetadata))]

    public partial class Weapon 
    {
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
