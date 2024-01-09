using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class CategoryViewModel
    {
        internal object p_class_name;

        public string p_category_id { get; set; }
        public string p_category_code { get; set; }
        public string p_category_name { get; set; }
        public string p_category_address { get; set; }
        public string chart_account { get; set; }
        public string class_id { get; set; }
        public string class_name { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public System.DateTime productcategory_created_date { get; set; }
        public string sub_group_id { get; set; }
        public string sub_group_name { get; set; }

    }
}