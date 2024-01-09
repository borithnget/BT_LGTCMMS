using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class InventoryViewModel
    {
        [Key]
        public string inventory_id { get; set; }
        public string inventory_detail_id { get; set; }
        public Nullable<decimal> history_quantity { get; set; }
        public Nullable<System.DateTime> inventory_date { get; set; }
        public string inventory_status_id { get; set; }
        public string warehouse_id { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public string unit { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
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
        public ProductViewModel uom { get; set; }
        public string item_status { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public bool completed { get; set; }
        public string itemUnitName { get; set; }
        public string unitName { get; set; }
        public int totalReceived { get; set; }
        public string from_warehouse_id { get; set; }
        public string from_warehouse_name { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public List<InventoryViewModel> histories { get; set; }
        public InventoryViewModel history { get; set; }
        public string brand { get; set; }
        public string reason { get; set; }
        public tb_inventory inventory { get; set; }
        public tb_product product { get; set; }
        public tb_unit unitEntity { get; set; }
        public InventoryViewModel()
        {
            uom = new ProductViewModel();
            histories = new List<InventoryViewModel>();
            inventory = new tb_inventory();
            product = new tb_product();
            unitEntity = new tb_unit();
            
        }

        public static List<InventoryViewModel> GetInventoryItemsByRefId(string id)
        {
            List<InventoryViewModel> models = new List<InventoryViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                models = (from inv in db.tb_inventory
                          join pro in db.tb_product on inv.product_id equals pro.product_id
                          join un in db.tb_unit on pro.product_unit equals un.Id
                          where string.Compare(inv.ref_id,id) == 0
                          select new InventoryViewModel { 
                              inventory= inv,
                              product= pro,
                              unitEntity= un 
                          }).ToList();
            }
            catch(Exception ex)
            {

            }
            return models;
        }
    }
    public class InventoryDetailViewModel
    {
        [Key]
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
        public decimal unit_price { get; set; }
        public string unit { get; set; }
        public string unitName { get; set; }
        public string itemunitname { get; set; }
        public ProductViewModel uom { get; set; }
        public string item_status { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public Nullable<decimal> total_quantity { get; set; }
        public Nullable<decimal> item_labour_hour { get; set; }
        public Nullable<DateTime> inventory_date { get; set; }
        public InventoryDetailViewModel()
        {
            uom = new ProductViewModel();
            
        }

        public static List<InventoryDetailViewModel> GetInventoryDetailByRefId(string refId)
        {
            List<InventoryDetailViewModel> models = new List<InventoryDetailViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

            }catch(Exception ex)
            {

            }
            return models;
        }
    }
}