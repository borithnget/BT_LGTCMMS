using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ReturnItemtoSupplier
    {
    }

    public class ReturnItemtoSupplierViewModel
    {
        public Nullable<System.DateTime> created_date { get; set; }
        public string item_return_number { get; set; }
        public string item_return_id { get; set; }
        public string suppliler_id { get; set; }
        public string supplier_name { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string dateFrom { get; internal set; }
        public string dateTo { get; internal set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }

    }
}