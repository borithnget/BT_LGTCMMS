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
    
    public partial class tb_purchase_requisition_detail
    {
        public string purchase_requisition_detail_id { get; set; }
        public string purchase_requisition_id { get; set; }
        public string item_id { get; set; }
        public string item_unit { get; set; }
        public Nullable<decimal> approved_qty { get; set; }
        public Nullable<decimal> remain_qty { get; set; }
        public string reason { get; set; }
        public string remark { get; set; }
        public string item_status { get; set; }
    
        public virtual tb_product tb_product { get; set; }
        public virtual tb_purchase_requisition tb_purchase_requisition { get; set; }
        public virtual tb_unit tb_unit { get; set; }
    }
}