using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFrontSchema.DATA.EF.Models/*.Metadata*/
{
    public class CategoryMetadata
    {
        public int CategoryId { get; set; }
        [Required]
        [Display(Name ="Category")]
        [StringLength(40)]
        [Range(0, 40, ErrorMessage = "X")]
        public string CategoryName { get; set; } = null!;
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [Display(Name = "Description")]
        [StringLength(500)]
        [Range(0, 500, ErrorMessage = "X")]
        public string CategoryDescription {get; set;}
    }

    public class OrderMetadata
    {
        public int OrderId { get; set; } 
        public int UserId { get; set; }

        [Required]
        [DisplayFormat(NullDisplayText ="NULL")]
        [StringLength(500)]
        [Display(Name = "Date Ordered")]
        public string OrderDate { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(500)]
        [Display(Name = "Date Shipped")]

        public string ShipDate { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(500)]
        [Display(Name = "Shipped To")]

        public string ShipTo { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]
        [Display(Name = "City")]

        public string ShipCity { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]
        [Display(Name = "Nation")]

        public string ShipNation { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(5)]
        [Display(Name = "Zip Code")]

        public string ShipZip { get; set; }


    }
    public class OrderProductMetadata
    {
        public int OrderProductId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        [Required]
        public short Quantity { get; set; }
        [Required]
        [Display(Name = "Price")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:c}")]
        public decimal ProductPrice { get; set; } 
    }
    public class UserDetailMetadata
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(40)]

        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(40)]

        public string LastName { get; set; } = null!;
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(10)]

        public string? Address { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]

        public string? City { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]

        public string? Nation { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(5)]

        public string? Zip { get; set; }
        
    }
    public class VendorMetadata
    {
        public int SmithId { get; set; }
        [Required]
        [StringLength(40)]
        [Display(Name = "Weapon Smith")]

        public string SmithName { get; set; } = null!;
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(50)]


        public string? Address { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]


        public string? City { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(25)]


        public string? Nation { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(5)]

        public string? Zip { get; set; }
        
    }
    public class WeaponMetadata
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(40)]
        [Display(Name = "Product")]

        public string ProductName { get; set; } = null!;
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [Display(Name = "Price")]
        


        public decimal? ProductPrice { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(500)]
        [Display(Name = "Description")]

        public string? ProductDescription { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [Display(Name = "In Stock")]

        public short? InStock { get; set; }
        [Required]
        [DisplayFormat(NullDisplayText = "NULL")]
        [Display(Name = "On Backorder")]

        public short? BackOrder { get; set; }
        [Required]
        [Display(Name = "Discontinued?")]

        public bool IsDiscontinued { get; set; } 
        public int CategoryId { get; set; }
        public int SmithId { get; set; }

        
        [DisplayFormat(NullDisplayText = "NULL")]
        [StringLength(75)]

        public string? ProductImage { get; set; }
    }
}
