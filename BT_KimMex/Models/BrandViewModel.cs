using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class BrandViewModel
    {
        internal string product_size_id;

        [Key]
        public string brand_id { get; set; }
        [Required(ErrorMessage = "Product Type is required.")]
        [Display(Name = "Product Type : ")]
        public string product_type_id { get; set; }
        [Required(ErrorMessage = "Brand Name is required.")]
        [Display(Name = "Brand Name : ")]
        public string brand_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string product_type_name { get; set; }
        public object Brand { get; internal set; }
        public string DisplayMode { get; internal set; }
        public object SelectedBrand { get; internal set; }
    }
}