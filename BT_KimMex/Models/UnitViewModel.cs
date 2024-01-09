using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class UnitViewModel
    {
        [Key]
        public string Id { get; set; }
        [Required(ErrorMessage ="Unit is required.")]
        [Display(Name="Unit:")]
        public string Name { get; set; }
        [Display(Name ="Description:")]
        public string unit_description { get; set; }
        [Display(Name ="Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
    }
}