//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BT_KimMex.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_transfer_frommain_stock_detail
    {
        public string st_detail_id { get; set; }
        public Nullable<bool> status { get; set; }
        public string st_ref_id { get; set; }
        public string st_item_id { get; set; }
        public string st_warehouse_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string unit { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public string item_status { get; set; }
        public string remark { get; set; }
        public Nullable<decimal> received_remain_quantity { get; set; }
        public Nullable<int> ordering_number { get; set; }
    
        public virtual transferformmainstock transferformmainstock { get; set; }
    }
}
