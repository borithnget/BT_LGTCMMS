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
    
    public partial class tb_item_blocking
    {
        public string item_blocking_id { get; set; }
        public string item_blocking_number { get; set; }
        public string warehouse_id { get; set; }
        public Nullable<bool> active { get; set; }
        public string item_blocking_status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string updated_by { get; set; }
    }
}
