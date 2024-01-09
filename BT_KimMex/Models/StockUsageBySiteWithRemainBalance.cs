using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class StockUsageBySiteWithRemainBalance
    {
    }

    public class StockUsageBySiteWithRemainBalanceViewModel
    {
        public string dateFrom { get; internal set; }
        public string dateTo { get; internal set; }
        public string site_id { get; set; }
        public string Site_name { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string Status { get; set; }
        public string Project_id { get; set; }
        public string Site_manager_projecu_id { get; set; }
        public string Project_name { get; set; }
        public string Inventory_id { get; set; }
        public string inventory_ref_id { get; set; }
        public Nullable<decimal> Quatity { get; set; }
        public string Product_id { get; set; }
        public string Product_Name { get; set; }
        public string Product_Unit { get; set; }

        public string product_code { get; set; }

        public string Stock_adjustment_id { get; set; }

        public string stock_issue_id { get; set; }
        public string stock_issue_number { get; set; }
    }
}