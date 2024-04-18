using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class TransferFromMainStockViewModel
    {
        [Key]
        public string stock_transfer_id { get; set; }
        public string stock_transfer_no { get; set; }
        public string stock_transfer_status { get; set; }
        public string item_request_id { get; set; }
        public string item_request_no { get; set; }
        public Nullable<bool> status { get; set; }
        [Display(Name = "Date:")]
        public string date { get; set; }
        public string created_by { get; set; }
        public string last_approved_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public List<InventoryViewModel> inventoryDetails { get; set; }

        public List<STItemViewModel1> itemTransfers { get; set; }

        public List<RejectViewModel> rejects { get; set; }
        public string checked_by { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceDate { get; set; }
        public string strInvoiceNo { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<bool> is_completed { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string project_short_name { get; set; }
        public string created_at_text { get; set; }
        public string created_by_text { get; set; }
        public string show_status { get; set; }
        public string grn_show_status { get; set; }
        public List<ProcessWorkflowModel> processWorkflows { get; set; }

        public TransferFromMainStockViewModel()
        {
            inventoryDetails = new List<InventoryViewModel>();
            itemTransfers = new List<STItemViewModel1>();
            rejects = new List<RejectViewModel>();
            processWorkflows = new List<ProcessWorkflowModel>();
        }

        //internal void Add(TransferFromMainStockViewModel transferstock)
        //{
        //    throw new NotImplementedException();
        //}

        public static List<Models.ItemRequestDetail2ViewModel> GetAllAvailableMaterailRequestItembyTransferWorkshop(string materialRequestId,string transferId = null)
        {
            List<Models.ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                if (string.IsNullOrEmpty(transferId))
                {
                    items = (from item in db.tb_ir_detail2
                             join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                             join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                             join pro in db.tb_product on item.ir_item_id equals pro.product_id
                             orderby item.ordering_number, pro.product_code
                             where string.Compare(pr.ir_id, materialRequestId) == 0 && item.is_approved == true && item.remain_qty > 0 && item.remain_qty != null
                             select new Models.ItemRequestDetail2ViewModel()
                             {
                                 ir_detail2_id = item.ir_detail2_id,
                                 ir_item_id = item.ir_item_id,
                                 product_code = pro.product_code,
                                 product_name = pro.product_name,
                                 product_unit = pro.product_unit,
                                 ir_qty = item.ir_qty,
                                 ir_item_unit = item.ir_item_unit,
                                 approved_qty = item.approved_qty,
                                 remain_qty = item.remain_qty,
                                 ordering_number = item.ordering_number
                             }).ToList();
                }
            }
            return items;
        }
        
    }

    public class FilterTransferWorkshopModel
    {
        public transferformmainstock tw { get; set; }
        public tb_item_request mr { get; set; }
        public tb_project pro { get; set; }
        public tb_warehouse wh { get; set; }
    }

    public class STItemViewModel1
    {
        public string itemID { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public Nullable<decimal> stockBalance { get; set; }
        public Nullable<decimal> requestQty { get; set; }
        public string requestUnit { get; set; }
        public Nullable<decimal> approved_qty { get; set; }
        public string warehouseID { get; set; }
        public string warehouseName { get; set; }
        public string warehouseid { get; set; }
       
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
        public Nullable<decimal> quantity { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public string item_status { get; set; }
        public string remark { get; set; }
        public Nullable<decimal> remain_qty { get; set; }
        public string Unit { get; set; }
        public string itemUnitName { get; set; }
        public string unitName { get; set; }
        public string requestUnitName { get; set; }
        public List<string> projectSiteManagers { get; set; }
    }

    public class TransferFromMainStockItemViewModel
    {
        public string st_detail_id { get; set; }
        public string st_ref_id { get; set; }
        public string st_item_id { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string itemUnitName { get; set; }
        public string st_warehouse_id { get; set; }
        public string warehouseName { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public string unit { get; set; }
        public string unitName { get; set; }
        public string ir_no { get; set; }
        public string ir_status { get; set; }
        public string ir_id { get; set; }
    }
}