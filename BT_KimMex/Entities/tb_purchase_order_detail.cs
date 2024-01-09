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
    
    public partial class tb_purchase_order_detail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_purchase_order_detail()
        {
            this.tb_po_supplier = new HashSet<tb_po_supplier>();
        }
    
        public string po_detail_id { get; set; }
        public string purchase_order_id { get; set; }
        public string item_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string item_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public string item_status { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
        public string po_unit { get; set; }
        public Nullable<bool> item_vat { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public Nullable<decimal> original_price { get; set; }
        public Nullable<decimal> discount_percentage { get; set; }
        public Nullable<decimal> discount_amount { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_po_supplier> tb_po_supplier { get; set; }
        public virtual tb_purchase_order tb_purchase_order { get; set; }
    }
}
