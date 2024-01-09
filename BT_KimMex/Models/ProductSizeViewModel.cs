using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ProductSizeViewModel
    {
        [Key]
        public string product_size_id { get; set; }
        [Required(ErrorMessage ="Class is required.")]
        [Display(Name ="Class")]
        public string class_id { get; set; }
        [Required(ErrorMessage ="Product Size Name is required.")]
        [Display(Name ="Product Size")]
        public string product_size_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string product_category_id { get; set; }
        public string p_category_name { get; set; }
        public string brand_id { get; set; }
        public string brand_name { get; set; }
            public string created_by { get; set; }
        public string updated_by { get; set; }
        [Display(Name ="Date")]
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string class_name { get; set; }
    }
}