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
    
    public partial class tb_job_category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_job_category()
        {
            this.tb_boq_detail1 = new HashSet<tb_boq_detail1>();
            this.tb_ir_detail1 = new HashSet<tb_ir_detail1>();
        }
    
        public string j_category_id { get; set; }
        public string j_category_name { get; set; }
        public string j_description { get; set; }
        public Nullable<bool> j_status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_boq_detail1> tb_boq_detail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_ir_detail1> tb_ir_detail1 { get; set; }
    }
}
