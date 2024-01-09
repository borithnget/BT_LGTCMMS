using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class PurchaseRequisitionDetailViewModel
    {
        [Key]
        public string purchase_requisition_detail_id { get; set; }
        public string purchase_requisition_id { get; set; }
        public string item_id { get; set; }
        public string item_unit { get; set; }
        public Nullable<decimal> approved_qty { get; set; }
        public Nullable<decimal> remain_qty { get; set; }
        public string reason { get; set; }
        public string remark { get; set; }
        public string item_unit_name { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_status { get; set; }
    }
}