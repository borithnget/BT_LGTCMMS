using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class AttachmentViewModel
    {
        public string attachment_id { get; set; }
        public string attachment_name { get; set; }
        public string attachment_extension { get; set; }
        public string attachment_path { get; set; }
        public string attachment_ref_id { get; set; }
        public string attachment_ref_type { get; set; }
    }
}