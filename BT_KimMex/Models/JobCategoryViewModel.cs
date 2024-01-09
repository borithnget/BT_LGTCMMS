using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class JobCategoryViewModel
    {
        [Key]
        public string j_category_id { get; set; }
        [Display(Name ="Category Name:")]
        [Required(ErrorMessage ="Category Name is required.")]
        public string j_category_name { get; set; }
        [Display(Name ="Description:")]
        public string j_description { get; set; }
        [Display(Name ="Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
    }
}