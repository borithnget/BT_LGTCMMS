using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class StockBalanceByDateandWarehouse
    {
    }
    public class StockBalanceByDateandWarehouseViewModel
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
        public string project_id { get; set; }
        public string projectName { get; set; }
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

        public Nullable<System.DateTime> dateinventory { get; set; }
        public Nullable<System.DateTime> date { get; set; }

        public Decimal bigbalance { get; set; }
        public Decimal in_receive { get; set; }
        public Decimal in_issue_return { get; set; }
        public Decimal out_return { get; set; }
        public Decimal out_transfer { get; set; }
        public Decimal out_damage { get; set; }
        public Decimal out_issue { get; set; }
        public Decimal total_in { get; set; }
        public Decimal total_out { get; set; }
        public Decimal ending_balance { get; set; }
        public string stock_status_id { get; set; }




        public List<StockBalanceByDateandWarehouseInventoryDetialViewModel> StoBbWDetials { get; set; }
        public StockBalanceByDateandWarehouseViewModel()
        {
            StoBbWDetials = new List<StockBalanceByDateandWarehouseInventoryDetialViewModel>();
            //stomWarehouse = new List<StockMovementWarehouseDetialViewModel>();
        }

    }
    public class StockBalanceByDateandWarehouseInventoryDetialViewModel
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

        public Nullable<System.DateTime> dateinventory { get; set; }

        public Decimal bigbalance { get; set; }
        public Decimal in_receive { get; set; }
        public Decimal in_issue_return { get; set; }
        public Decimal out_return { get; set; }
        public Decimal out_transfer { get; set; }
        public Decimal out_damage { get; set; }
        public Decimal out_issue { get; set; }
        public Decimal total_in { get; set; }
        public Decimal total_out { get; set; }
        public Decimal ending_balance { get; set; }
        public string inventory_status_id { get; set; }
        public string stock_status_id { get; set; }
        public string warehouse_id { get; set; }
    }

    public class StockBalanceMonthyViewModel
    {
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }
        public double BigBalance { get; set; }
        public double ReceivedBalance { get; set; }
        public double IssueReturnBalance { get; set; }
        public double ReturnBalance { get; set; }
        public double TransferBalance { get; set; }
        public double DamangeBalance { get; set; }
        public double IssueBalance { get; set; }
        public double EndingBalance { get; set; }
        public double TotalInBalance() { return ReceivedBalance + IssueBalance; }
        public double TotalOutBalance() { return ReturnBalance + TransferBalance + DamangeBalance + IssueBalance; }

        //Rathana Add 25.04.2019
        public double TransferIn { get; set; }
        public double IN { get; set; }
        public double UPrice { get; set; }
        //End Rathana Add
        public Nullable<decimal> ItemNumber { get; set; }
        public double TotalIn { get; set; }
        public double TotalOut { get; set; }
    }
}