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
    
    public partial class tb_inventory_deleted
    {
        public string inventory_id { get; set; }
        public Nullable<System.DateTime> inventory_date { get; set; }
        public string inventory_status_id { get; set; }
        public string warehouse_id { get; set; }
        public string product_id { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> in_quantity { get; set; }
        public Nullable<decimal> out_quantity { get; set; }
        public string ref_id { get; set; }
        public string remark { get; set; }
        public string unit { get; set; }
    }
}
