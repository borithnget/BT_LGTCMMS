using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ColorViewModel
    {

        public string color_id { get; set; }
        public string color_name { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
    }
}
