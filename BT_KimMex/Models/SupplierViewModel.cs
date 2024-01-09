using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class SupplierViewModel
    {
        [Key]
        public string supplier_id { get; set; }
        [Required(ErrorMessage = "Supplier Name is required.")]
        [Display(Name = "Supplier Name:")]
        public string supplier_name { get; set; }
        [Display(Name ="Address:")]
        public string supplier_address { get; set; }
        [EmailAddress]
        [Display(Name ="Email:")]
        public string supplier_email { get; set; }
        [Display(Name ="Contact Person")]
        public string supplier_contact_person { get; set; }
        [Display(Name ="Telephone:")]
        public string supplier_phone { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        [Display(Name ="VAT (%):")]
        public Nullable<decimal> discount { get; set; }
        public Nullable<bool> is_local { get; set; }
        [Required(ErrorMessage ="Supplier's Type is required.")]
        public string supplier_type { get; set; }
        public string supplier_fax { get; set; }
        public string incoterm { get; set; }
        public string payment { get; set; }
        public string delivery { get; set; }
        public string shipment { get; set; }
        public string warranty { get; set; }
        public string vendor_ref { get; set; }
        public Nullable<bool> is_quote_selected { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
        public static SupplierViewModel ConvertEntityToModel(Entities.tb_supplier entity)
        {
            return new SupplierViewModel()
            {
                supplier_id=entity.supplier_id,
                supplier_name=entity.supplier_name,
                supplier_address=entity.supplier_address,
                supplier_contact_person=entity.supplier_contact_person,
                supplier_email=entity.supplier_email,
                supplier_phone=entity.supplier_phone,
                created_date=entity.created_date,
                discount=entity.discount,
                is_local=entity.is_local,
                supplier_type=entity.supplier_type,
                supplier_fax=entity.supplier_fax,
                incoterm=entity.incoterm,
                payment=entity.payment,
                delivery=entity.delivery,
                shipment=entity.shipment,
                warranty=entity.warranty,
                vendor_ref=entity.vendor_ref,
            };
        }
    }

}