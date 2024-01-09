using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class PurchaseOrderAandRViewModel
    {
       
        public int Purchase_order_id { get; set; }
        public string purchase_oder_number { get; set; }
        public string Item_request_id { get; set; }
        public string item_id { get; set; }
        public string Purchar_Number { get; set; }
        public string purchase_order_status { get; set; }
        public Nullable<System.DateTime> Create_date { get; set; }
        public string status { get; set; }
        public Nullable<bool> active { get; set; }
        public string Approve { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
       
        public string checked_by { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public System.DateTime checked_date { get; set; }
        public System.DateTime Update_Date { get; set; }
        public string Update_by { get; set; }
        public string Upprove_by { get; set; }
        public System.DateTime Uprove_date { get; set; }
        public string rejected_by { get; set; }
        public System.DateTime rejected_date { get; set; }
        public string Action { get; set; }
        public string ir_no { get; set; }
        public List<PurchaseOrderDetailViewModel> poDetails { get; set; }
        //private List<PurchaseOrderItemSupplier> poSuppliers;

        public List<PurchaseOrderItemSupplier> poSuppliers { get; set; }
        public List<RejectViewModel> rejects { get; set; }


        public PurchaseOrderAandRViewModel()
        {
            poDetails = new List<PurchaseOrderDetailViewModel>();
            poSuppliers = new List<PurchaseOrderItemSupplier>();
            rejects = new List<RejectViewModel>();
           
        }

        public class PurchaseOrderDetailViewModel
        {
          
            public string po_detail_id { get; set; }
            public string ir_detail2_id { get; set; }
            public string purchase_order_id { get; set; }
            public string item_id { get; set; }
            public string supplier_id { get; set; }
            public string supplier_name { get; set; }
            public Nullable<decimal> quantity { get; set; }
            public string item_unit { get; set; }
            public Nullable<decimal> unit_price { get; set; }
            public string item_status { get; set; }
            public Nullable<bool> status { get; set; }
            public Nullable<bool> item_vat { get; set; }
            public Nullable<decimal> po_quantity { get; set; }
            public string po_unit { get; set; }
            public string product_code { get; set; }
            public string product_name { get; set; }
            public string product_unit { get; set; }
            public string unit { get; set; }
            public string uom1_id { get; set; }
            public Nullable<decimal> uom1_qty { get; set; }
            public string uom2_id { get; set; }
            public Nullable<decimal> uom2_qty { get; set; }
            public string uom3_id { get; set; }
            public Nullable<decimal> uom3_qty { get; set; }
            public string uom4_id { get; set; }
            public Nullable<decimal> uom4_qty { get; set; }
            public string uom5_id { get; set; }
            public Nullable<decimal> uom5_qty { get; set; }
            public int unitLevel { get; set; }
            public ProductViewModel uom { get; set; }
            public string warehouseID { get; set; }
            public string product_unit_name { get; set; }
            public string unit_name { get; set; }
            public string item_unit_name { get; set; }
            public List<PurchaseOrderItemSupplier> poSuppliers { get; set; }
            //  public string PO_Unit { get; set; }
            //public PurchaseOrderDetailViewModel()
        //    {
        //        poSuppliers = new List<Models.PurchaseOrderItemSupplier>();
        //        uom = new ProductViewModel();
        //}
    }
        public class PurchaseOrderItemSupplier
        {
            public string po_supplier_id { get; set; }
            public string po_detail_id { get; set; }
            public string supplier_id { get; set; }
            public string supplier_name { get; set; }
            public string supplier_address { get; set; }
            public string supplier_email { get; set; }
            public string supplier_phone { get; set; }
            public Nullable<decimal> unit_price { get; set; }
            public string sup_number { get; set; }
            public Nullable<bool> is_selected { get; set; }
            public Nullable<bool> vat { get; set; }
            public string Reason { get; set; }
            public Nullable<decimal> po_quantity { get; set; }
            public Nullable<decimal> discount { get; set; }
            public string po_unit { get; set; }
            public Nullable<bool> is_check { get; set; }
            public Nullable<bool> item_vat { get; set; }

            //public List<PurchaseOrderDetailViewModel> items { get; set; }
            //public PurchaseOrderItemSupplier()
            //{
            //    items = new List<PurchaseOrderDetailViewModel>();
            //}

            public static implicit operator List<object>(PurchaseOrderItemSupplier v)
            {
                throw new NotImplementedException();
            }
        }

    }


}