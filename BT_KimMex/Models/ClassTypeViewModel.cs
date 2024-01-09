using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ClassTypeViewModel
    {
        internal string product_class_name;
        internal DateTime? update_at;

        [Key]
        public string class_type_id { get; set; }
        [Required(ErrorMessage = "Class Type is required.")]
        [Display(Name = "Class Type Name : ")]
        public string class_type_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}