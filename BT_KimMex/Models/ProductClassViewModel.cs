using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BT_KimMex.Models
{
    public class ProductClassViewModel
    {
        [Key]
        public string product_class_id { get; set; }
        [Display(Name = "Class Type : ")]
        [Required(ErrorMessage = "Class Type is required.")]
        public string class_type_id { get; set; }
        [Display(Name = "Product Class : ")]
        [Required(ErrorMessage = "Product Class is required.")]
        public string product_class_name { get; set; }
        public Nullable<bool> active { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string class_type_name { get; set; }
        [Display(Name = "Site Name:")]
        //[Required(ErrorMessage = "Site Name is required.")]
        public string productclass_site_id { get; set; }
        [Display(Name = "Stock Keeper:")]
        //[Required(ErrorMessage = "Stock Keeper is required.")]
        public string[] Stock_keeper_id { get; set; }
        [Display(Name = "Stock Keeper:")]

        public IEnumerable<String> list_stock_keeper { set; get; }


        public static string GetStock_keeper_name(String id)
        {
            BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return db.tb_user_detail.Where(m => m.user_id == id).Select(m => m.user_first_name + " " + m.user_last_name).FirstOrDefault();
        }




    }
}