using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class ReturnMainStockViewModel
    {
        [Key]
        public string return_main_stock_id { get; set; }
        public string return_main_stock_no { get; set; }
        public string return_type { get; set; }
        public string return_ref_id { get; set; }
        public string return_main_stock_status { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public string create_by { get; set; }
        public Nullable<System.DateTime> update_date { get; set; }
        public string update_by { get; set; }
        public string approved_by { get; set; }
        public string last_approved_by { get; set; }
        
        public Nullable<System.DateTime> approved_date { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string checked_comment { get; set; }
        public string completed_by { get; set; }
        public Nullable<System.DateTime> completed_date { get; set; }
        public string completed_comment { get; set; }
        public string return_ref_number { get; set; }
        public string mr_id { get; set; }
        public string mr_number { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public List<TransferFromMainStockItemViewModel> itemTransfers { get; set; }
        public List<InventoryViewModel> inventoryDetails { get; set; }
        public string rmsWarehouse { get; set; }
        public string rmsInvoiceNo { get; set; }
        public string rmsInvoiceDate { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public WareHouseViewModel warehouse { get; set; }
        public string project_short_name { get; set; }
        public ReturnMainStockViewModel()
        {
            inventoryDetails = new List<InventoryViewModel>();
            itemTransfers = new List<TransferFromMainStockItemViewModel>();
            inventories = new List<InventoryViewModel>();
            warehouse = new WareHouseViewModel();
        }


    }

    public class ReturnMainStockItemViewModel
    {
        public string st_detail_id { get; set; }
        public string st_ref_id { get; set; }
        public string st_item_id { get; set; }
        public string st_warehouse_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string unit { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public string item_status { get; set; }
        public string remark { get; set; }
        public Nullable<bool> status { get; set; }

        public virtual tb_product tb_product { get; set; }
        public virtual tb_stock_transfer_voucher tb_stock_transfer_voucher { get; set; }
        public virtual tb_warehouse tb_warehouse { get; set; }
    }

}