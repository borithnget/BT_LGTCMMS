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
    
    public partial class tb_receive_item_voucher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_receive_item_voucher()
        {
            this.tb_ire_attachment = new HashSet<tb_ire_attachment>();
            this.tb_received_item_detail = new HashSet<tb_received_item_detail>();
        }
    
        public string receive_item_voucher_id { get; set; }
        public string received_number { get; set; }
        public string received_status { get; set; }
        public string received_type { get; set; }
        public string ref_id { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_by { get; set; }
        public string supplier_id { get; set; }
        public string po_report_number { get; set; }
        public Nullable<bool> is_received_partial { get; set; }
        public Nullable<System.DateTime> sending_date { get; set; }
        public Nullable<System.DateTime> returning_date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_ire_attachment> tb_ire_attachment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_received_item_detail> tb_received_item_detail { get; set; }
    }
}