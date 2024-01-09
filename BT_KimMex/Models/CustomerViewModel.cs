using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class CustomerViewModel
    {
        [Key]
        public string customer_id { get; set; }
        [Required(ErrorMessage ="Customer name is required.")]
        [Display(Name ="Customer Name:")]
        public string customer_name { get; set; }
        [Display(Name ="Address:")]
        public string customer_address { get; set; }
        [EmailAddress]
        [Display(Name ="Email:")]
        public string customer_email { get; set; }
        [Display(Name ="Telephone:")]
        public string customer_phone { get; set; }
        [Display(Name ="Date:")]
        public System.DateTime customer_created_date { get; set; }
    }
}