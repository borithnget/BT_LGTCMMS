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
    
    public partial class tb_material_request_history
    {
        public string mr_history_id { get; set; }
        public string mr_ref_id { get; set; }
        public string ir_number { get; set; }
        public string new_created_by { get; set; }
        public Nullable<System.DateTime> new_created_at { get; set; }
        public string ir_project_id { get; set; }
        public string ir_purpose_id { get; set; }
        public string ir_status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<bool> is_completed { get; set; }
        public Nullable<bool> is_mr { get; set; }
        public string po_status { get; set; }
        public string st_status { get; set; }
        public string tw_status { get; set; }
        public Nullable<bool> is_cut_off { get; set; }
        public Nullable<System.DateTime> expected_delivery_date { get; set; }
    }
}
