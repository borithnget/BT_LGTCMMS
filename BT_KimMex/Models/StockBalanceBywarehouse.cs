using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class StockBalanceBywarehouse
    {
        
    }
    public class StockBalanceBywarehouseViewModel
    {
        [Key]
        public string inventory_id { get; set; }
        public Nullable<System.DateTime> inventory_date { get; set; }
        public string inventory_status_id { get; set; }
        public string warehouse_id { get; set; }
        public string product_id { get; set; }
        public string unit { get; set; }
      
        public Nullable<decimal> in_quantity { get; set; }
        public Nullable<decimal> out_quantity { get; set; }
        public string ref_id { get; set; }
        public string warehouseName { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string remark { get; set; }
        public string itemTypeId { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }

        public Nullable<decimal> unit_price { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> total { get; set; }



    public List<StockBalanceBywarehouseInventoryDetialViewModel> StoBbWDetials { get; set; }
        public StockBalanceBywarehouseViewModel()
        {
            StoBbWDetials = new List<StockBalanceBywarehouseInventoryDetialViewModel>();
            //stomWarehouse = new List<StockMovementWarehouseDetialViewModel>();
        }

    }
    public class StockBalanceBywarehouseInventoryDetialViewModel
    {

        public string inventory_detail_id { get; set; }
        public string inventory_ref_id { get; set; }
        public string inventory_item_id { get; set; }
        public string inventory_warehouse_id { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> stock_balance { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string warehouseName { get; set; }
        public string remark { get; set; }
        public string itemTypeId { get; set; }
        public string itemTypeName { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        //public decimal unit_price { get; set; }

        public Nullable<decimal> unit_price { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> total { get; set; }


        

    }
    

}