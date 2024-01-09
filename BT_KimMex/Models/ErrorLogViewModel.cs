using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ErrorLogViewModel
    {
        [Key]
        public string error_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string class_name { get; set; }
        public string method_name { get; set; }
        public string information_type { get; set; }
        public string exception { get; set; }
        public string message { get; set; }
    }

}