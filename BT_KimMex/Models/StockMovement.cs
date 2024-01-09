using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    //public class StockMovement
    //{
    //    [Key]
    //    public string product_id { get; set; }
    //    public string product_Name { get; set; }
    //    public string product_Unit { get; set; }
    //    public string product_Category { get; set; }
    //    public string Qty_Of_Item_Return { get; set; }
    //    public string Warehouse { get; set; }
    //    public string TotalStockCost { get; set; }
    //}
    public class StockMovementViewModel
    {
        [Key]
        public string stock_transfer_id { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public string P_Category_Name { get; set; }
        //public string Create_date { get; set; }
        //public string Inventory_Date { get; set; }
        public string Warehouse_Name { get; set; }
        public string Project_Name { get; set; }
        public string Stock_transfer_status { get; set; }
        public Nullable<decimal> Out_Quantity { get; set; }
        public Nullable<decimal> Total_Quantity { get; set; }
        public string Item_Request_no { get; set; }
        public string DateFrom { get; internal set; }
        public string DateTo { get; internal set; }

        [Display(Name = "IR ID:")]
      
        public Nullable<System.DateTime> Inventory_Date { get; set; }
        public List<StockMovementTransferDetialViewModel> stomTDetials { get; set; }
        public List<StockMovementWarehouseViewModel> stomWarehouse { get; set; }
        public StockMovementViewModel()
        {
            stomTDetials = new List<StockMovementTransferDetialViewModel>();
            stomWarehouse = new List<StockMovementWarehouseViewModel>();
        }
    }
    public class StockMovementTransferDetialViewModel
    {
        public string Stock_transfer_no { get; set; }
        public string Stock_transfer_status { get; set; }
        public string Item_Request_Id { get; set; }

        public List<StockMovementWarehouseViewModel> stomWarehouse { get; set; }
        public StockMovementTransferDetialViewModel()
        {
            stomWarehouse = new List<StockMovementWarehouseViewModel>();
        }

    }
    public class StockMovementWarehouseViewModel
    {
        public string Warehouse_name { get; set; }
        public string Create_Date { get; set; }
        public Nullable<decimal> Total_Quantity { get; set; }


    }

}