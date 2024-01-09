using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Views
{
    public class PurchaseOrderAandR
    {

       
        public int Purchase_order_id { get; set; }
        public string purchase_oder_number { get; set; }
        public string Item_request_id { get; set; }
        public string item_id { get; set; }
        public string Purchar_Number { get; set; }
        public System.DateTime Create_date { get; set; }
        public string status { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
        public string checked_by { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public System.DateTime checked_date { get; set; }
        public System.DateTime Update_Date { get; set; }
        public DateTime created_date { get; set; }
        public string Update_by { get; set; }
        public string Upprove_by { get; set; }
        public System.DateTime Uprove_date { get; set; }
        public string rejected_by { get; set; }
        public System.DateTime rejected_date { get; set; }
        public string Action { get; set; }

        public string sup_number { get; set; }
        public List<PurchaseOrderDetailViewModel> poDetails { get; set; }
        public List<PurchaseOrderItemSupplier> poSuppliers { get; set; }
        public List<RejectViewModel> rejects { get; set; }

        public PurchaseOrderAandR()
        {
            poDetails = new List<PurchaseOrderDetailViewModel>();
            poSuppliers = new List<PurchaseOrderItemSupplier>();
            rejects = new List<RejectViewModel>();

        }

      
        public class PurchaseOrderVsItemReceivedViewModel
        {
            public string purchaseOrder_id { get; set; }
            public string po_supplier_id { get; set; }
            public string po_detail_id { get; set; }
            public string ref_id { get; set; }
            public string ri_ref_id { get; set; }
            public string ri_item_id { get; set; }
            public string ri_detail_id { get; set; }


            public Nullable<System.DateTime> created_date { get; set; }
            public string purchase_order_number { get; set; }
            public string item_code { get; set; }
            public string item_name { get; set; }
            public string item_id { get; set; }
            public string unit { get; set; }
            //public Nullable<decimal> unit_price { get; set; }
            public Nullable<decimal> po_quantity { get; set; }
            public Nullable<decimal> different_quatity { get; set; }
            public Nullable<decimal> item_received { get; set; }

            public List<PurchaseOrderDetailViewModel> items { get; set; }
            public PurchaseOrderVsItemReceivedViewModel()
            {
                items = new List<PurchaseOrderDetailViewModel>();
            }
        }


        public class PurchaseOrderSupplier
        {
            public string purchaseOrderId { get; set; }
            public string purchaseOrderNumber { get; set; }
            public string poSupplierId { get; set; }
            public bool isVAT { get; set; }
            public string warehouseId { get; set; }
            public string itemRequestNumber { get; set; }
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
            public PurchaseOrderDetailViewModel()
            {
                poSuppliers = new List<Models.PurchaseOrderItemSupplier>();
                uom = new ProductViewModel();
            }
        }
    }

   
}