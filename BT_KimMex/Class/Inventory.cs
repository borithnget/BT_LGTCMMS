using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Class
{
    public class Inventory
    {
        private static kim_mexEntities db = new kim_mexEntities();
        string ID = HttpContext.Current.User.Identity.GetUserId(); //Get user Loggin ID 
        public static List<InventoryViewModel> GetInventoryItem(string id)
        {
            string ID = HttpContext.Current.User.Identity.GetUserId();

            List<InventoryViewModel> inventories = new List<InventoryViewModel>();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                /*
                //add new 
                if(HttpContext.Current.User.IsInRole("Stock Keeper"))
                {
                    inventories = (from inv in db.tb_stock_transfer_detail
                                   join wh in db.tb_stock_keeper_warehouse on inv.st_warehouse_id equals wh.warehouse_id
                                   join item in db.tb_product on inv.st_item_id equals item.product_id
                                   join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                   join type in db.tb_product_category on item.p_category_id equals type.p_category_id
                                   orderby item.product_code
                                   where string.Compare(inv.st_ref_id, id) == 0 && wh.stock_keeper==ID
                                   //where wh.stock_keeper == ID
                                   select new InventoryViewModel()
                                   {
                                       inventory_id = inv.st_detail_id,
                                       //inventory_date = inv.inventory_date,
                                       product_id = inv.st_item_id,
                                       itemName = item.product_name,
                                       itemCode = item.product_code,
                                       itemUnit = item.product_unit,
                                       warehouse_id = inv.st_warehouse_id,
                                       warehouseName = wah.warehouse_name,
                                       //total_quantity = inv.total_quantity,
                                       //in_quantity = inv.in_quantity,
                                       out_quantity = inv.quantity,
                                       remark = inv.remark,
                                       itemTypeId = type.p_category_id,
                                       uom = db.tb_multiple_uom.Where(x => x.product_id == inv.st_warehouse_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                   }).ToList();
                    //inventories = (from inv in db.tb_inventory
                    //               join wh in db.tb_stock_keeper_warehouse on inv.warehouse_id equals wh.warehouse_id
                    //               join item in db.tb_product on inv.product_id equals item.product_id
                    //               join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                    //               join type in db.tb_product_category on item.p_category_id equals type.p_category_id
                    //               orderby item.product_code
                    //               where string.Compare(inv.ref_id, id) == 0
                    //               where wh.stock_keeper==ID
                    //               select new InventoryViewModel()
                    //               {
                    //                   inventory_id = inv.inventory_id,
                    //                   inventory_date = inv.inventory_date,
                    //                   product_id = inv.product_id,
                    //                   itemName = item.product_name,
                    //                   itemCode = item.product_code,
                    //                   itemUnit = item.product_unit,
                    //                   warehouse_id = inv.warehouse_id,
                    //                   warehouseName = wah.warehouse_name,
                    //                   total_quantity = inv.total_quantity,
                    //                   in_quantity = inv.in_quantity,
                    //                   out_quantity = inv.out_quantity,
                    //                   remark = inv.remark,
                    //                   itemTypeId = type.p_category_id,
                    //                   uom = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                    //               }).ToList();
                    return inventories;
                }
                */
                //add new 
                inventories = (from inv in db.tb_inventory
                               join item in db.tb_product on inv.product_id equals item.product_id
                               //join std in db.tb_stock_transfer_detail on item.product_id equals std.st_item_id //new add
                               join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                               //join type in db.tb_brand on item.brand_id equals type.brand_id
                               //orderby item.product_code
                               where string.Compare(inv.ref_id, id) == 0
                               select new InventoryViewModel()
                               {
                                   inventory_id = inv.inventory_id,
                                   inventory_date = inv.inventory_date,
                                   product_id = inv.product_id,
                                   itemName = item.product_name,
                                   itemCode = item.product_code,
                                   itemUnit = item.product_unit,
                                   warehouse_id = inv.warehouse_id,
                                   warehouseName = wah.warehouse_name,
                                   total_quantity = inv.total_quantity,
                                   in_quantity = inv.in_quantity,
                                   out_quantity = inv.out_quantity,
                                   remark = inv.remark,
                                   //itemTypeId = type.brand_id,
                                   //unit = std.unit,
                                   uom = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                               }).ToList();
            }
            return inventories;
        }
        public static List<InventoryViewModel> GetInventoryItemByIID(string id, string itemId)
        { 
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var fd = (from std in db.tb_stock_transfer_detail join stv in db.tb_stock_transfer_voucher on std.st_ref_id equals stv.stock_transfer_id 
                          join pro in db.tb_product on std.st_item_id equals pro.product_id
                          join war in db.tb_warehouse on std.st_warehouse_id equals war.warehouse_id
                          join uni in db.tb_unit on pro.product_unit equals uni.Id                         
                          where std.st_item_id == itemId && std.item_status == "approved" && std.status == true && stv.stock_transfer_id == id && std.quantity != 0 select new { std, stv, pro, war, uni }).ToList();

                inventories = fd.Select(x => new InventoryViewModel() { 
                           itemCode = x.std.st_item_id,
                           unitName = x.uni.Name,
                           itemName = x.pro.product_name,
                           warehouseName = x.war.warehouse_name,
                           out_quantity = x.std.quantity,
                           in_quantity = x.std.quantity,
                           itemUnit = x.std.unit,
                           warehouse_id = x.std.st_warehouse_id,
                           itemUnitName = x.uni.Name,
                           product_id = x.std.st_item_id,
                           invoice_number = x.std.invoice_number,
                           invoice_date = x.std.invoice_date,
                           remark = x.std.remark,
                            uom = db.tb_multiple_uom.Where(std => std.product_id == x.pro.product_id).Select(b=>new ProductViewModel() { uom1_id = b.uom1_id, uom1_qty = b.uom1_qty, uom2_id = b.uom2_id, uom2_qty = b.uom2_qty, uom3_id = b.uom3_id, uom3_qty = b.uom3_qty, uom4_id = b.uom4_id, uom4_qty = b.uom4_qty, uom5_id = b.uom5_id, uom5_qty = b.uom5_qty }).FirstOrDefault(),

                }).ToList();
               // issueItems = item.Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();

                //var fd = (from std in db.tb_stock_transfer_detail
                //          join stv in db.tb_stock_transfer_voucher on std.st_ref_id equals stv.stock_transfer_id
                //          join pro in db.tb_product on std.st_item_id equals pro.product_id
                //          join war in db.tb_warehouse on std.st_warehouse_id equals war.warehouse_id
                //          join uni in db.tb_unit on pro.product_unit equals uni.Id
                //          where std.st_item_id == itemId && std.item_status == "approved" && std.status == true && stv.stock_transfer_id == id && std.quantity != 0
                //          select new { std, stv, pro, war, uni }).ToList();

                //inventories = fd.Select(x => new InventoryViewModel()
                //{
                //    itemCode = x.std.st_item_id,
                //    unitName = x.uni.Name,
                //    itemName = x.pro.product_name,
                //    warehouseName = x.war.warehouse_name,
                //    out_quantity = x.std.quantity,
                //    in_quantity = x.std.quantity,
                //    itemUnit = x.std.unit,
                //    warehouse_id = x.std.st_warehouse_id,
                //    itemUnitName = x.uni.Name,
                //    product_id = x.std.st_item_id,
                //    invoice_number = x.std.invoice_number,
                //    invoice_date = x.std.invoice_date,
                //    remark = x.std.remark,
                //    uom = db.tb_multiple_uom.Where(std => std.product_id == x.pro.product_id).Select(b => new ProductViewModel() { uom1_id = b.uom1_id, uom1_qty = b.uom1_qty, uom2_id = b.uom2_id, uom2_qty = b.uom2_qty, uom3_id = b.uom3_id, uom3_qty = b.uom3_qty, uom4_id = b.uom4_id, uom4_qty = b.uom4_qty, uom5_id = b.uom5_id, uom5_qty = b.uom5_qty }).FirstOrDefault(),

                //}).ToList();
                // issueItems = item.Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();

            }
            return inventories;
        }
        //public static List<InventoryViewModel> GetInventoryItemByIID(string id, string itemId)
        //{
        //    List<InventoryViewModel> inventories = new List<InventoryViewModel>();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        inventories = (from inv in db.tb_inventory
        //                       join item in db.tb_product on inv.product_id equals item.product_id
        //                       join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
        //                       join type in db.tb_product_category on item.brand_id equals type.p_category_id
        //                       orderby item.product_code
        //                       where string.Compare(inv.ref_id, id) == 0 && string.Compare(inv.product_id, itemId) == 0
        //                       select new InventoryViewModel()
        //                       {
        //                           inventory_id = inv.inventory_id,
        //                           inventory_date = inv.inventory_date,
        //                           product_id = inv.product_id,
        //                           itemName = item.product_name,
        //                           itemCode = item.product_code,
        //                           itemUnit = item.product_unit,
        //                           warehouse_id = inv.warehouse_id,
        //                           warehouseName = wah.warehouse_name,
        //                           total_quantity = inv.total_quantity,
        //                           in_quantity = inv.in_quantity,
        //                           out_quantity = inv.out_quantity,
        //                           remark = inv.remark,
        //                           itemTypeId = type.p_category_id,
        //                           uom = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
        //                       }).ToList();
        //    }
        //    return inventories;
        //}
        //public static List<InventoryViewModel> GetInventoryItemByIID(string id, string itemId)
        //{
        //    List<InventoryViewModel> inventories = new List<InventoryViewModel>();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        inventories = (from inv in db.tb_inventory
        //                       join item in db.tb_product on inv.product_id equals item.product_id
        //                       join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
        //                       join type in db.tb_product_category on item.brand_id equals type.p_category_id
        //                       orderby item.product_code
        //                       where string.Compare(inv.ref_id, id) == 0 && string.Compare(inv.product_id, itemId) == 0
        //                       select new InventoryViewModel()
        //                       {
        //                           inventory_id = inv.inventory_id,
        //                           inventory_date = inv.inventory_date,
        //                           product_id = inv.product_id,
        //                           itemName = item.product_name,
        //                           itemCode = item.product_code,
        //                           itemUnit = item.product_unit,
        //                           warehouse_id = inv.warehouse_id,
        //                           warehouseName = wah.warehouse_name,
        //                           total_quantity = inv.total_quantity,
        //                           in_quantity = inv.in_quantity,
        //                           out_quantity = inv.out_quantity,
        //                           remark = inv.remark,
        //                           itemTypeId = type.p_category_id,
        //                           uom = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
        //                       }).ToList();
        //    }
        //    return inventories;
        //}
        public static InventoryViewModel GetInvetoryItemDetail(string itemId,string warehouseId)
        {
//    List<InventoryViewModel> inventories = new List<InventoryViewModel>();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        inventories = (from inv in db.tb_inventory
        //                       join item in db.tb_product on inv.product_id equals item.product_id
        //                       join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
        //                       join type in db.tb_product_category on item.brand_id equals type.p_category_id
        //                       orderby item.product_code
        //                       where string.Compare(inv.ref_id, id) == 0 && string.Compare(inv.product_id, itemId) == 0
        //                       select new InventoryViewModel()
        //                       {
        //                           inventory_id = inv.inventory_id,
        //                           inventory_date = inv.inventory_date,
        //                           product_id = inv.product_id,
        //                           itemName = item.product_name,
        //                           itemCode = item.product_code,
        //                           itemUnit = item.product_unit,
        //                           warehouse_id = inv.warehouse_id,
        //                           warehouseName = wah.warehouse_name,
        //                           total_quantity = inv.total_quantity,
        //                           in_quantity = inv.in_quantity,
        //                           out_quantity = inv.out_quantity,
        //                           remark = inv.remark,
        //                           itemTypeId = type.p_category_id,
        //                           uom = db.tb_multiple_uom.Where(x => x.product_id == inv.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
        //                       }).ToList();
        //    }
        //    return inventories;
        //}
                           InventoryViewModel inventory = new InventoryViewModel();
            inventory = (from inv in db.tb_inventory
                         join item in db.tb_product on inv.product_id equals item.product_id
                         join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                         orderby inv.inventory_date descending
                         where string.Compare(inv.warehouse_id, warehouseId) == 0 && string.Compare(inv.product_id, itemId) == 0
                         select new InventoryViewModel()
                         {
                             inventory_id = inv.inventory_id,
                             inventory_date = inv.inventory_date,
                             product_id = inv.product_id,
                             itemName = item.product_name,
                             itemCode = item.product_code,
                             itemUnit = item.product_unit,
                             warehouse_id = inv.warehouse_id,
                             warehouseName = wah.warehouse_name,
                             total_quantity = inv.total_quantity,
                             in_quantity = inv.in_quantity,
                             out_quantity = inv.out_quantity
                         }).FirstOrDefault();
            return inventory;
        }
        public static List<InventoryDetailViewModel> GetInventoryDetail(string id)
        {
            List<InventoryDetailViewModel> inventoryItemDetails = new List<InventoryDetailViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventoryDetails = (from invd in db.tb_inventory_detail
                                        join item in db.tb_product on invd.inventory_item_id equals item.product_id
                                        //join unit in db.tb_unit on item.product_unit equals unit.Id
                                        join warehouse in db.tb_warehouse on invd.inventory_warehouse_id equals warehouse.warehouse_id
                                        orderby invd.ordering_number
                                        where invd.inventory_ref_id == id
                                        select new InventoryDetailViewModel()
                                        {
                                            inventory_detail_id = invd.inventory_detail_id,
                                            inventory_ref_id = invd.inventory_ref_id,
                                            inventory_item_id = invd.inventory_item_id,
                                            itemCode = item.product_code,
                                            itemName = item.product_name,
                                            itemUnit = item.product_unit,
                                            inventory_warehouse_id = invd.inventory_warehouse_id,
                                            warehouseName = warehouse.warehouse_name,
                                            quantity = invd.quantity,
                                            remark = invd.remark,
                                            supplier_id = invd.supplier_id,
                                            unit = invd.unit,
                                            item_status = invd.item_status,
                                            invoice_date = invd.invoice_date,
                                            invoice_number = invd.invoice_number,
                                            remain_quantity = invd.remain_quantity,
                                            item_labour_hour=item.labour_hour,
                                            unit_price=(decimal)item.unit_price,
                                            ordering_number = invd.ordering_number,

                                        }).ToList();
                foreach (var inventory in inventoryDetails)
                {
                    decimal sb = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => x.product_id == inventory.inventory_item_id && x.warehouse_id == inventory.inventory_warehouse_id).Select(x => x.total_quantity).FirstOrDefault());
                    sb = sb == null ? 0 : sb;
                    InventoryDetailViewModel inventoryDetail = new InventoryDetailViewModel();
                    inventoryDetail.inventory_detail_id = inventory.inventory_detail_id;
                    inventoryDetail.inventory_ref_id = inventory.inventory_ref_id;
                    inventoryDetail.inventory_item_id = inventory.inventory_item_id;
                    inventoryDetail.itemCode = inventory.itemCode;
                    inventoryDetail.itemName = inventory.itemName;
                    inventoryDetail.itemUnit = inventory.itemUnit;
                    inventoryDetail.itemunitname = db.tb_unit.Find(inventoryDetail.itemUnit).Name;
                    inventoryDetail.inventory_warehouse_id = inventory.inventory_warehouse_id;
                    inventoryDetail.warehouseName = inventory.warehouseName;
                    inventoryDetail.quantity = inventory.quantity;
                    inventoryDetail.remark = inventory.remark;
                    inventoryDetail.ordering_number = inventory.ordering_number;
                    inventoryDetail.stock_balance = sb;
                    inventoryDetail.supplier_id = inventory.supplier_id;
                    inventoryDetail.supplier_name = db.tb_supplier.Where(x => x.supplier_id == inventoryDetail.supplier_id).Select(x => x.supplier_name).FirstOrDefault();
                    inventoryDetail.unit = inventory.unit;
                    inventoryDetail.unitName = db.tb_unit.Find(inventoryDetail.unit).Name;
                    inventoryDetail.uom = db.tb_multiple_uom.Where(x => x.product_id == inventory.inventory_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                    inventoryDetail.item_status = inventory.item_status;
                    inventoryDetail.invoice_number = inventory.invoice_number;
                    inventoryDetail.invoice_date = inventory.invoice_date;
                    inventoryDetail.remain_quantity = inventory.remain_quantity;
                    inventoryDetail.total_quantity = db.tb_history_issue_qty.Where(s => string.Compare(s.inventory_detail_id, inventoryDetail.inventory_detail_id) == 0).Select(s => s.issue_qty).Sum();
                    inventoryDetail.item_labour_hour = inventory.item_labour_hour;
                    inventoryDetail.unit_price = inventory.unit_price;
                    inventoryItemDetails.Add(inventoryDetail);
                }
            }
            return inventoryItemDetails;
        }
        public static List<WareHouseViewModel> GetWarehouseListByStockTransferID(string id)
        {

            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            //new add
            //if(HttpContext.Current.User.IsInRole("Stock Keeper"))
            //{
            //    warehouses=(from wah in db.tb_warehouse
            //                join stk in db.tb_stock_keeper_warehouse on wah.warehouse_id equals stk.warehouse_id
            //                join site in db.tb_site on wah.warehouse_site_id equals site.site_id
            //                join proj in db.tb_project on site.site_id equals proj.site_id
            //                join it in db.tb_item_request on proj.project_id equals it.ir_project_id
            //                join st in db.tb_stock_transfer_voucher on it.ir_id equals st.item_request_id
            //                where stk.stock_keeper==ID
            //                where st.stock_transfer_id == id
            //                select new WareHouseViewModel
            //                {
            //                    warehouse_id = wah.warehouse_id,
            //                    warehouse_name = wah.warehouse_name

            //                }).ToList();
            //    return warehouses;
            //new add 

            //  }
            warehouses = (from wah in db.tb_warehouse
                          join site in db.tb_site on wah.warehouse_site_id equals site.site_id
                          join proj in db.tb_project on site.site_id equals proj.site_id
                          join it in db.tb_item_request on proj.project_id equals it.ir_project_id
                          join st in db.tb_stock_transfer_voucher on it.ir_id equals st.item_request_id
                          where st.stock_transfer_id == id
                          select new WareHouseViewModel
                          {
                              warehouse_id = wah.warehouse_id,
                              warehouse_name = wah.warehouse_name

                          }).ToList();
            return warehouses;
        }
        public static List<WareHouseViewModel> GetWarehouseListByPurchaseOrderID(string id)
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            warehouses = (from wah in db.tb_warehouse
                          join site in db.tb_site on wah.warehouse_site_id equals site.site_id
                          join proj in db.tb_project on site.site_id equals proj.site_id
                          join it in db.tb_item_request on proj.project_id equals it.ir_project_id
                          join pr in db.tb_purchase_requisition on it.ir_id equals pr.material_request_id
                          join po in db.tb_purchase_order on pr.purchase_requisition_id equals po.item_request_id
                          join poo in db.tb_purchase_request on po.purchase_order_id equals poo.purchase_order_id
                          where poo.pruchase_request_id == id
                          select new WareHouseViewModel
                          {
                              warehouse_id = wah.warehouse_id,
                              warehouse_name = wah.warehouse_name
                          }).ToList();
            return warehouses;
        }
        public static List<WareHouseViewModel> GetWarehousesList()
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            warehouses = (from wah in db.tb_warehouse
                          join site in db.tb_site on wah.warehouse_site_id equals site.site_id
                          where wah.warehouse_status == true
                          select new WareHouseViewModel
                          {
                              warehouse_id = wah.warehouse_id,
                              warehouse_name = wah.warehouse_name
                          }).ToList();
            
            return warehouses;
        }
        #region Purchase Order
        public static List<PurchaseOrderDetailViewModel> GetPurchaseOrderItems(string id)
        {
            List<PurchaseOrderDetailViewModel> purchaseOrders = new List<PurchaseOrderDetailViewModel>();
            purchaseOrders = (from tbl in db.tb_purchase_order_detail
                              join pro in db.tb_product on tbl.item_id equals pro.product_id
                              orderby pro.product_code
                              where tbl.purchase_order_id == id && tbl.item_status == "approved"
                              select new PurchaseOrderDetailViewModel()
                              {
                                  po_detail_id = tbl.po_detail_id,
                                  purchase_order_id = tbl.purchase_order_id,
                                  item_id = tbl.item_id,
                                  product_code = pro.product_code,
                                  product_name = pro.product_name,
                                  product_unit = pro.product_unit,
                                  //quantity = tbl.quantity
                                  quantity = tbl.po_quantity,
                                  po_unit = tbl.po_unit,
                                  uom1_id = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom1_id).FirstOrDefault(),
                                  uom2_id = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom2_id).FirstOrDefault(),
                                  uom3_id = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom3_id).FirstOrDefault(),
                                  uom4_id = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom4_id).FirstOrDefault(),
                                  uom5_id = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom5_id).FirstOrDefault(),
                                  uom1_qty = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom1_qty).FirstOrDefault(),
                                  uom2_qty = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom2_qty).FirstOrDefault(),
                                  uom3_qty = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom3_qty).FirstOrDefault(),
                                  uom4_qty = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom4_qty).FirstOrDefault(),
                                  uom5_qty = db.tb_multiple_uom.Where(m => m.product_id == tbl.item_id).Select(x => x.uom5_qty).FirstOrDefault(),
                              }).ToList();


            return purchaseOrders;
        }
        public static decimal GetPurchaseOrderItemQty(string id, string itemID)
        {
            decimal quantity = 0;
            quantity = Convert.ToDecimal(db.tb_purchase_order_detail.Where(m => m.purchase_order_id == id && m.item_id == itemID).Select(m => m.quantity).FirstOrDefault());
            return quantity;
        }
        #endregion
        #region Stock Transfer
        public static List<InventoryViewModel> GetStockTransferItems(string id)
        {
            List<InventoryViewModel> stockTransferItems = new List<InventoryViewModel>();
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            inventories = Inventory.GetInventoryItem(id);
            decimal totalTransferQty = 0, transferQty = 0;
            var dupItem = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (dupItem.Any())
            {
                foreach (var Item in dupItem)
                {
                    totalTransferQty = 0;
                    var transferedItem = inventories.Where(x => x.product_id == Item).Select(x => x.out_quantity).ToList();
                    foreach (var tQty in transferedItem)
                    {
                        totalTransferQty = totalTransferQty + Convert.ToDecimal(tQty);
                    }
                    foreach (var inv in inventories)
                    {
                        InventoryViewModel inventory = new InventoryViewModel();
                        if (string.Compare(inv.product_id, Item) == 0)
                        {
                            inventory.inventory_id = inv.inventory_id;
                            inventory.inventory_date = inv.inventory_date;
                            inventory.product_id = inv.product_id;
                            inventory.itemName = inv.itemName;
                            inventory.itemCode = inv.itemCode;
                            inventory.itemUnit = inv.itemUnit;
                            inventory.warehouse_id = inv.warehouse_id;
                            inventory.warehouseName = inv.warehouseName;
                            inventory.total_quantity = inv.total_quantity;
                            inventory.in_quantity = inv.in_quantity;
                            inventory.out_quantity = totalTransferQty;
                        }
                        else
                        {
                            inventory.inventory_id = inv.inventory_id;
                            inventory.inventory_date = inv.inventory_date;
                            inventory.product_id = inv.product_id;
                            inventory.itemName = inv.itemName;
                            inventory.itemCode = inv.itemCode;
                            inventory.itemUnit = inv.itemUnit;
                            inventory.warehouse_id = inv.warehouse_id;
                            inventory.warehouseName = inv.warehouseName;
                            inventory.total_quantity = inv.total_quantity;
                            inventory.in_quantity = inv.in_quantity;
                            inventory.out_quantity = inv.out_quantity;
                        }

                        ///////

                        inventory.unit = db.tb_stock_transfer_detail.FirstOrDefault(x => x.st_ref_id == id).unit;

                        ////
                        inventory.uom = inv.uom;
                        stockTransferItems.Add(inventory);
                    }
                }
            }
            else
            {
                stockTransferItems = inventories;
            }
            List<InventoryViewModel> noDupST = new List<InventoryViewModel>();
            noDupST = stockTransferItems.DistinctBy(i => i.product_id).ToList();
            return noDupST;
        }
        public static decimal GetStockTransferItemQty(string id, string itemID)
        {
            decimal quantity = 0;
            quantity = Convert.ToDecimal(db.tb_inventory.Where(m => m.ref_id == id && m.product_id == itemID).Select(m => m.out_quantity).FirstOrDefault());
            return quantity;
        }
        public static List<InventoryViewModel> GetStockTransferRemainItems(string id, string receivedId = null)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(receivedId))
                {
                    var models = (from transfer in db.tb_stock_transfer_detail
                                  join item in db.tb_product on transfer.st_item_id equals item.product_id
                                  join wh in db.tb_warehouse on transfer.st_warehouse_id equals wh.warehouse_id
                                  orderby transfer.ordering_number
                                  //where string.Compare(transfer.st_ref_id, id) == 0 && transfer.remain_quantity > 0
                                  where string.Compare(transfer.st_ref_id, id) == 0
                                  select new Models.InventoryViewModel()
                                  {
                                      inventory_id = transfer.st_detail_id,
                                      //inventory_date = inv.inventory_date,
                                      product_id = transfer.st_item_id,
                                      itemName = item.product_name,
                                      itemCode = item.product_code,
                                      itemUnit = item.product_unit,
                                      warehouse_id = transfer.st_warehouse_id,
                                      warehouseName = wh.warehouse_name,
                                      unit = transfer.unit,
                                      total_quantity = transfer.quantity,
                                      //in_quantity = inv.in_quantity,
                                      out_quantity = transfer.remain_quantity,
                                      remark = transfer.remark,
                                      //itemTypeId = type.p_category_id,
                                      uom = db.tb_multiple_uom.Where(x => x.product_id == transfer.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                  }).ToList();
                    foreach (var item in models)
                    {
                        InventoryViewModel inv = new InventoryViewModel();
                        inv.inventory_id = item.inventory_id;
                        inv.product_id = item.product_id;
                        inv.itemName = item.itemName;
                        inv.itemCode = item.itemCode;
                        inv.itemUnit = item.itemUnit;
                        inv.itemUnitName = db.tb_unit.Find(inv.itemUnit).Name;
                        inv.warehouse_id = item.warehouse_id;
                        inv.warehouseName = item.warehouseName;
                        inv.unit = item.unit;
                        inv.unitName = db.tb_unit.Find(inv.unit).Name;
                        inv.out_quantity = item.out_quantity;
                        inv.remark = item.remark;
                        inv.uom = item.uom;
                        inv.total_quantity = item.total_quantity;

                        var receivedHistory = db.tb_receive_item_voucher.Where(s => s.status == true && string.Compare(s.ref_id, id) == 0).ToList();
                        inv.totalReceived = receivedHistory.Count();
                        if (receivedHistory.Any())
                        {
                            foreach(var rh in receivedHistory)
                            {
                                var receivedItem = db.tb_received_item_detail.Where(s => string.Compare(s.ri_ref_id, rh.receive_item_voucher_id) == 0 && string.Compare(s.ri_item_id, inv.product_id) == 0 && string.Compare(s.supplier_id, inv.warehouse_id) == 0).FirstOrDefault();
                                if (receivedItem != null)
                                    inv.histories.Add(new InventoryViewModel() { total_quantity = receivedItem.quantity });
                            }
                        }

                        inventories.Add(inv);
                    }
                }
                else
                {
                    List<InventoryViewModel> transferItems = new List<InventoryViewModel>();
                    List<Models.ItemReceivedDetailViewModel> receivedItems = new List<ItemReceivedDetailViewModel>();
                    transferItems = (from transfer in db.tb_stock_transfer_detail
                                     join item in db.tb_product on transfer.st_item_id equals item.product_id
                                     join wh in db.tb_warehouse on transfer.st_warehouse_id equals wh.warehouse_id
                                     orderby transfer.ordering_number
                                     //orderby item.product_code
                                     where string.Compare(transfer.st_ref_id, id) == 0
                                     select new Models.InventoryViewModel()
                                     {
                                         inventory_id = transfer.st_detail_id,
                                         product_id = transfer.st_item_id,
                                         itemName = item.product_name,
                                         itemCode = item.product_code,
                                         itemUnit = item.product_unit,
                                         unit = transfer.unit,
                                         warehouse_id = transfer.st_warehouse_id,
                                         warehouseName = wh.warehouse_name,
                                         out_quantity = transfer.remain_quantity,
                                         remark = transfer.remark,
                                         uom = db.tb_multiple_uom.Where(x => x.product_id == transfer.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                     }).ToList();
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receivedId) == 0).Select(x => new ItemReceivedDetailViewModel()
                    {
                        ri_item_id = x.ri_item_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                    foreach (InventoryViewModel item in transferItems)
                    {
                        decimal receivedQuantity = 0;
                        var isExist = receivedItems.Where(x => string.Compare(x.ri_item_id, item.product_id) == 0).FirstOrDefault();
                        if (isExist != null)
                        {
                            receivedQuantity = Class.CommonClass.ConvertMultipleUnit(isExist.ri_item_id, isExist.unit, Convert.ToDecimal(isExist.quantity));
                        }
                        InventoryViewModel inv = new InventoryViewModel();
                        inv.inventory_id = item.inventory_id;
                        //inventory_date = inv.inventory_date,
                        inv.product_id = item.product_id;
                        inv.itemName = item.itemName;
                        inv.itemCode = item.itemCode;
                        inv.itemUnit = item.itemUnit;
                        inv.itemUnitName = db.tb_unit.Find(inv.itemUnit).Name;
                        inv.unit = item.unit;
                        inv.unitName = db.tb_unit.Find(inv.unit).Name;
                        inv.warehouse_id = item.warehouse_id;
                        inv.warehouseName = item.warehouseName;
                        inv.out_quantity = item.out_quantity + receivedQuantity;
                        inv.remark = item.remark;
                        inv.uom = item.uom;
                        inventories.Add(inv);
                    }
                }
            }
            return inventories;
        }
        #endregion
        #region Receive Item
        public static List<ItemReceiveViewModel> GetReceiveItemsList()
        {
            List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
            var itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true).ToList();
            //var itemReceiveList = (from ri in db.tb_receive_item_voucher orderby ri.received_number where ri.status == true select ri).ToList();
            foreach (var itemReceive in itemReceiveList)
            {
                ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                string ref_number = string.Empty;
                if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                {
                    ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                }
                else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                {
                    ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                }
                itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                itemRec.received_number = itemReceive.received_number;
                itemRec.received_status = itemReceive.received_status;
                itemRec.received_type = itemReceive.received_type;
                itemRec.ref_id = itemReceive.ref_id;
                itemRec.ref_number = ref_number;
                //itemRec.received_status = itemReceive.received_status;
                itemReceives.Add(itemRec);
            }
            return itemReceives;
        }
        public static ItemReceiveViewModel GetItemReceivedViewDetail(string id)
        {
            ItemReceiveViewModel itemReceive = new ItemReceiveViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string ref_number = string.Empty;
                itemReceive = (from ire in db.tb_receive_item_voucher
                               where string.Compare(ire.receive_item_voucher_id, id) == 0
                               select new ItemReceiveViewModel()
                               {
                                   receive_item_voucher_id = ire.receive_item_voucher_id,
                                   received_number = ire.received_number,
                                   received_status = ire.received_status,
                                   received_type = ire.received_type,
                                   po_report_number = ire.po_report_number,
                                   supplier_id = ire.supplier_id,
                                   ref_id = ire.ref_id,
                                   created_date = ire.created_date,
                                   created_by=ire.created_by,
                                   approved_by=ire.approved_by,
                                   checked_by=ire.checked_by,
                                    approved_date=ire.approved_date,
                                    checked_date=ire.checked_date,
                                    sending_date=ire.sending_date,
                                    returning_date=ire.returning_date,
                               }).FirstOrDefault();
                itemReceive.created_by_signature = string.IsNullOrEmpty(itemReceive.created_by) ? string.Empty : CommonClass.GetUserSignature(itemReceive.created_by);
                itemReceive.approved_by_signature = string.IsNullOrEmpty(itemReceive.approved_by) ? string.Empty : CommonClass.GetUserSignature(itemReceive.approved_by);
                itemReceive.checked_by_signature = string.IsNullOrEmpty(itemReceive.checked_by) ? string.Empty : CommonClass.GetUserSignature(itemReceive.checked_by);
                itemReceive.created_by = CommonClass.GetUserFullname(itemReceive.created_by);
                itemReceive.approved_by = CommonClass.GetUserFullname(itemReceive.approved_by);
                itemReceive.checked_by = CommonClass.GetUserFullname(itemReceive.checked_by);
                itemReceive.approved_date_text = itemReceive.approved_date.HasValue ? CommonClass.ToLocalTime(Convert.ToDateTime(itemReceive.approved_date)).ToString("dd-MMM-yyyy") : string.Empty;
                itemReceive.checked_date_text = itemReceive.checked_date.HasValue ? CommonClass.ToLocalTime(Convert.ToDateTime(itemReceive.checked_date)).ToString("dd-MMM-yyyy") : string.Empty;
                

                if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                {
                    var transfer= db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).FirstOrDefault();
                    ref_number = transfer.stock_transfer_no;
                    itemReceive.ref_number_date = Convert.ToDateTime(transfer.created_date).ToString("dd-MMM-yy");
                    //itemReceive.receivedItems = Inventory.GetInventoryItem(itemReceive.ref_id);
                    itemReceive.receivedItems = Inventory.GetStockTransferItems(itemReceive.ref_id);
                    itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                 join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                 where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                 select mr.ir_no).FirstOrDefault().ToString();

                    var project= (from pro in db.tb_project
                                                    join mr in db.tb_item_request on pro.project_id equals mr.ir_project_id
                                                    join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                    where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                    select pro).FirstOrDefault();
                    itemReceive.project_full_name = project.project_full_name;
                    itemReceive.project_so_number = project.project_no;

                }
                else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                {
                    //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                    ref_number = itemReceive.po_report_number;
                    itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                 join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                 join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                 join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                 where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                 select mr.ir_no).FirstOrDefault();
                    var project= (from mr in db.tb_item_request
                                                    join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                    join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                    join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                    join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                    where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                    select pro).FirstOrDefault();
                    itemReceive.project_full_name = project.project_full_name;
                    itemReceive.project_so_number = project.project_no;

                    itemReceive.receivedItems = Inventory.ConvertFromPOtoInventory(itemReceive.ref_id, id, itemReceive.po_report_number, itemReceive.supplier_id);

                    var poreport = db.tb_po_report.Where(s => string.Compare(s.po_report_number, ref_number) == 0).FirstOrDefault();
                    if (poreport != null)
                        itemReceive.ref_number_date = Convert.ToDateTime(poreport.created_date).ToString("dd-MMM-yy");
                    
                }
                else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                {
                    var transferRef= (from transfer in db.transferformmainstocks
                                      join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                      join project in db.tb_project on mr.ir_project_id equals project.project_id
                                      join site in db.tb_site on project.site_id equals site.site_id
                                      join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                      where string.Compare(transfer.stock_transfer_id, itemReceive.ref_id) == 0
                                      select new 
                                      {
                                          transfer,project,mr,wh
                                      }).FirstOrDefault();
                    ref_number = transferRef.transfer.stock_transfer_no;
                    itemReceive.mr_ref_number = transferRef.mr.ir_no;
                    itemReceive.project_full_name = transferRef.project.project_full_name;
                    itemReceive.project_so_number = transferRef.project.project_no;
                    itemReceive.receivedItems = Inventory.GetTransferfromWorkshopItems(itemReceive.ref_id);
                    itemReceive.ref_number_date = Convert.ToDateTime(transferRef.transfer.created_date).ToString("dd-MMM-yy");
                    
                }
                else if(string.Compare(itemReceive.received_type,"Stock Return") == 0)
                {
                    var returnRef = (from sreturn in db.tb_stock_issue_return
                                     join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                                     join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                     join project in db.tb_project on mr.ir_project_id equals project.project_id
                                     join site in db.tb_site on project.site_id equals site.site_id
                                     join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                     where string.Compare(sreturn.stock_issue_return_id, itemReceive.ref_id) == 0
                                     select new 
                             {
                                 sreturn,transfer,mr,project,wh
                             }).FirstOrDefault();
                    ref_number = returnRef.sreturn.issue_return_number;
                    itemReceive.mr_ref_number = returnRef.mr.ir_no;
                    itemReceive.project_full_name = returnRef.project.project_full_name;
                    itemReceive.project_so_number = returnRef.project.project_no;
                    itemReceive.receivedItems = Inventory.GetStockReturnItems(itemReceive.ref_id);
                    itemReceive.ref_number_date = Convert.ToDateTime(returnRef.sreturn.created_date).ToString("dd-MMM-yyyy");
                }
                else if(string.Compare(itemReceive.received_type,"Return Workshop") == 0)
                {
                    var returnRef = (
                        from wreturn in db.tb_return_main_stock
                        join transfer in db.transferformmainstocks on wreturn.return_ref_id equals transfer.stock_transfer_id
                                       join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                       join project in db.tb_project on mr.ir_project_id equals project.project_id
                                       join site in db.tb_site on project.site_id equals site.site_id
                                       join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                       where string.Compare(wreturn.return_main_stock_id, itemReceive.ref_id) == 0
                                       select new
                                       {
                                           wreturn,
                                           transfer,
                                           project,
                                           mr,
                                           wh
                                       }).FirstOrDefault();
                    ref_number = returnRef.wreturn.return_main_stock_no;
                    itemReceive.mr_ref_number = returnRef.mr.ir_no;
                    itemReceive.project_full_name = returnRef.project.project_full_name;
                    itemReceive.project_so_number = returnRef.project.project_no;
                    itemReceive.receivedItems = Inventory.GetReturnWorkshopItems(itemReceive.ref_id);
                    itemReceive.ref_number_date =Convert.ToDateTime(returnRef.wreturn.create_date).ToString("dd-MMM-yy");
                }
                itemReceive.ref_number = ref_number;
                //itemReceive.inventories = Inventory.GetInventoryItem(itemReceive.receive_item_voucher_id);
                itemReceive.receivedHistories = Inventory.GetReceivedHistory(itemReceive.ref_id, itemReceive.receive_item_voucher_id);
                itemReceive.inventories = Inventory.GetReceivedItemList(itemReceive);
                itemReceive.attachments = Inventory.GetItemReceiveAttachment(itemReceive.receive_item_voucher_id);
                itemReceive.rejects = CommonClass.GetRejectByRequest(id);
                itemReceive.doAttachments = db.tb_attachment.Where(s => string.Compare(s.attachment_ref_id, itemReceive.receive_item_voucher_id) == 0).ToList();
            }
            return itemReceive;
        }
        public static List<InventoryViewModel> GetReceivedItemList(ItemReceiveViewModel itemReceive)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventoryList = (from inv in db.tb_received_item_detail
                                     join item in db.tb_product on inv.ri_item_id equals item.product_id
                                     join wah in db.tb_warehouse on inv.ri_warehouse_id equals wah.warehouse_id
                                     orderby item.product_code
                                     where string.Compare(inv.ri_ref_id, itemReceive.receive_item_voucher_id) == 0
                                     select new
                                     {
                                         inventory_id = inv.ri_detail_id,
                                         //inventory_date = inv.inventory_date,
                                         product_id = inv.ri_item_id,
                                         itemName = item.product_name,
                                         itemCode = item.product_code,
                                         itemUnit = item.product_unit,
                                         warehouse_id = inv.ri_warehouse_id,
                                         warehouseName = wah.warehouse_name,
                                         unit = inv.unit,
                                         //total_quantity = inv.total_quantity,
                                         in_quantity = inv.quantity,
                                         inovice_date = inv.invoice_date,
                                         invoice_number = inv.invoice_number,
                                         supplier_id = inv.supplier_id,
                                         item_status = inv.item_status,
                                         remark = inv.remark,
                                         completed = inv.completed,
                                         //out_quantity = inv.out_quantity
                                     }
                             ).ToList();
                foreach (var inv in inventoryList)
                {
                    InventoryViewModel inventory = new InventoryViewModel();
                    inventory.inventory_id = inv.inventory_id;
                    //inventory.inventory_date = inv.inventory_date;
                    inventory.product_id = inv.product_id;
                    inventory.itemName = inv.itemName;
                    inventory.itemCode = inv.itemCode;
                    inventory.itemUnit = inv.itemUnit;
                    inventory.warehouse_id = inv.warehouse_id;
                    inventory.warehouseName = inv.warehouseName;
                    inventory.in_quantity = inv.in_quantity;
                    inventory.unit = inv.unit;
                    inventory.unitName = db.tb_unit.Find(inventory.unit).Name;
                    inventory.item_status = inv.item_status;
                    inventory.invoice_number = inv.invoice_number;
                    inventory.invoice_date = inv.inovice_date;
                    inventory.supplier_id = inv.supplier_id;
                    //if(!string.IsNullOrEmpty(inventory.supplier_id))
                    //    inventory.supplier_name = db.tb_supplier.Where(x => x.supplier_id == inventory.supplier_id).Select(x => x.supplier_name).FirstOrDefault().ToString();
                    //inventory.out_quantity = inv.out_quantity;
                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        inventory.total_quantity = Inventory.GetStockTransferItemQty(itemReceive.ref_id, inventory.product_id);
                        inventory.supplier_name = db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, inventory.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString();

                        var receivedHistory = db.tb_receive_item_voucher.Where(s => s.status == true && string.Compare(s.ref_id, itemReceive.ref_id) == 0).ToList();
                        inventory.totalReceived = receivedHistory.Count();
                        if (receivedHistory.Any())
                        {
                            foreach (var rh in receivedHistory)
                            {
                                var receivedItem = db.tb_received_item_detail.Where(s => string.Compare(s.ri_ref_id, rh.receive_item_voucher_id) == 0 && string.Compare(s.ri_item_id, inventory.product_id) == 0 && string.Compare(s.supplier_id, inventory.supplier_id) == 0).FirstOrDefault();
                                if (receivedItem != null)
                                    inventory.histories.Add(new InventoryViewModel() { total_quantity = receivedItem.quantity });
                            }
                        }
                    }else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    {
                        inventory.supplier_name = "Workshop";
                    }
                    else if(string.Compare(itemReceive.received_type,"Stock Return") == 0)
                    {
                        inventory.supplier_name = db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, inventory.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString();
                    }
                    else
                    {
                        inventory.total_quantity = Inventory.GetPurchaseOrderItemQty(itemReceive.ref_id, inventory.product_id);
                        inventory.supplier_name = db.tb_supplier.Where(x => string.Compare(x.supplier_id, inventory.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString();
                    }
                    inventory.remark = inv.remark;
                    inventory.completed = inv.completed;
                    inventories.Add(inventory);
                }
            }
            return inventories;
        }
        public static List<tb_ire_attachment> GetItemReceiveAttachment(string id)
        {
            List<tb_ire_attachment> attachments = new List<tb_ire_attachment>();
            attachments = db.tb_ire_attachment.OrderBy(m => m.ire_attachment_name).Where(m => m.ire_id == id).ToList();
            return attachments;
        }
        #endregion
        public static List<tb_attachment> GetAttachments(string id)
        {
            List<tb_attachment> attachments = new List<tb_attachment>();
            attachments = db.tb_attachment.OrderBy(x => x.attachment_name).Where(x => x.attachment_ref_id == id).ToList();
            return attachments;
        }
        public static List<InventoryViewModel> ConvertFromPOtoInventory(string id, string receivedId, string poReportNumber = null, string supplierId = null)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            List<PurchaseOrderDetailViewModel> purchaseOrders = new List<PurchaseOrderDetailViewModel>();
            //purchaseOrders = Inventory.GetPurchaseOrderItems(id);
            if (string.IsNullOrEmpty(poReportNumber) && string.IsNullOrEmpty(poReportNumber))
                purchaseOrders = Inventory.GetRemainItemByPurchaseOrder(id, receivedId);
            else
                purchaseOrders = Class.ClsItemReceive.GetReceivedRemainItembyPurchaseOrder(id, supplierId, poReportNumber, receivedId);
            if (purchaseOrders != null)
            {
                foreach (var po in purchaseOrders)
                {
                    InventoryViewModel inventory = new InventoryViewModel();
                    inventory.product_id = po.item_id;
                    inventory.itemName = po.product_name;
                    inventory.itemCode = po.product_code;
                    inventory.itemUnit = po.product_unit;
                    inventory.itemUnitName = db.tb_unit.Find(inventory.itemUnit).Name;
                    inventory.total_quantity = po.po_quantity;
                    inventory.out_quantity = po.quantity;
                    inventory.remain_quantity = po.remain_quantity;
                    inventory.uom1_id = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom1_id).FirstOrDefault();
                    inventory.uom2_id = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom2_id).FirstOrDefault();
                    inventory.uom3_id = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom3_id).FirstOrDefault();
                    inventory.uom4_id = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom4_id).FirstOrDefault();
                    inventory.uom5_id = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom5_id).FirstOrDefault();
                    inventory.uom1_qty = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom1_qty).FirstOrDefault();
                    inventory.uom2_qty = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom2_qty).FirstOrDefault();
                    inventory.uom3_qty = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom3_qty).FirstOrDefault();
                    inventory.uom4_qty = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom4_qty).FirstOrDefault();
                    inventory.uom5_qty = db.tb_multiple_uom.Where(m => m.product_id == po.item_id).Select(x => x.uom5_qty).FirstOrDefault();
                    inventory.supplier_id = po.supplier_id;
                    inventory.supplier_name = po.supplier_name;
                    inventories.Add(inventory);
                }
            }
            inventories = inventories.ToList();
            return inventories;
        }
        public static List<PurchaseOrderDetailViewModel> GetRemainItemByPurchaseOrder(string poId, string receivedId)
        {
            List<PurchaseOrderDetailViewModel> inventories = new List<PurchaseOrderDetailViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //List<InventoryViewModel> itemReceived = new List<InventoryViewModel>();
                string quoteid = db.tb_purchase_request.Find(poId).purchase_order_id;
                List<Models.ItemReceivedDetailViewModel> itemReceived = new List<ItemReceivedDetailViewModel>();
                var purchaseOrderDetails = (from dPo in db.tb_purchase_order_detail
                                            join product in db.tb_product on dPo.item_id equals product.product_id
                                            orderby dPo.ordering_number
                                            where string.Compare(dPo.purchase_order_id, quoteid) == 0 && (string.Compare(dPo.item_status, "approved") == 0 || string.Compare(dPo.item_status, "Pending") == 0|| string.Compare(dPo.item_status, "pending") == 0)
                                            select new { dPo, product }).ToList();
                itemReceived = (from rItem in db.tb_received_item_detail
                                where string.Compare(rItem.ri_ref_id, receivedId) == 0
                                select new ItemReceivedDetailViewModel() { ri_item_id = rItem.ri_item_id, quantity = rItem.quantity, unit = rItem.unit }).ToList();
                foreach (var po in purchaseOrderDetails)
                {
                    decimal receivedQuantity = 0;
                    var isReceived = itemReceived.Where(m => string.Compare(m.ri_item_id, po.dPo.item_id) == 0).FirstOrDefault();
                    if (isReceived != null)
                    {
                        receivedQuantity = Class.CommonClass.ConvertMultipleUnit(isReceived.ri_item_id, isReceived.unit, Convert.ToDecimal(isReceived.quantity));
                    }
                    PurchaseOrderDetailViewModel inv = new PurchaseOrderDetailViewModel();
                    inv.po_detail_id = po.dPo.po_detail_id;
                    inv.purchase_order_id = po.dPo.purchase_order_id;
                    inv.item_id = po.dPo.item_id;
                    inv.product_name = po.product.product_name;
                    inv.product_code = po.product.product_code;
                    inv.product_unit = po.product.product_unit;
                    inv.quantity = po.dPo.remain_quantity + receivedQuantity;
                    inv.po_quantity = po.dPo.po_quantity;
                    inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                    inv.supplier_id = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_id).FirstOrDefault().ToString();
                    inv.supplier_name = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_name).FirstOrDefault().ToString();
                    inventories.Add(inv);
                }
                #region old process
                /*
                var purchaseOrderItems = (from dPo in db.tb_purchase_order_detail
                                          join product in db.tb_product on dPo.item_id equals product.product_id
                                          orderby product.product_code
                                          where string.Compare(dPo.purchase_order_id, poId) == 0 && string.Compare(dPo.item_status, "approved") == 0
                                          select new { dPo, product }).ToList();
                var purchaseOrderReceives = (from re in db.tb_receive_item_voucher
                                             where string.Compare(re.ref_id, poId) == 0 && re.status == true && string.Compare(re.received_status, "Approved") == 0
                                             select re.receive_item_voucher_id).ToList();
                if (purchaseOrderReceives.Any())
                {
                    foreach (var pore in purchaseOrderReceives)
                    {
                        var received = (from inv in db.tb_inventory
                                        where inv.ref_id == pore
                                        select new { inv }).ToList();
                        if (received.Any())
                            foreach (var re in received)
                                itemReceived.Add(new InventoryViewModel() { product_id = re.inv.product_id, in_quantity = re.inv.in_quantity });
                    }
                }
                if (purchaseOrderItems.Any())
                {
                    foreach (var po in purchaseOrderItems)
                    {
                        decimal remainPOQuantity = CommonClass.ConvertMultipleUnit(po.dPo.item_id, po.dPo.po_unit, Convert.ToDecimal(po.dPo.po_quantity));
                        if (itemReceived.Count() > 0)
                        {
                            foreach (InventoryViewModel ire in itemReceived)
                            {
                                if (po.dPo.item_id == ire.product_id)
                                    remainPOQuantity = remainPOQuantity - Convert.ToDecimal(ire.in_quantity);
                            }
                        }
                        //if (remainPOQuantity > 0)
                        //{
                            PurchaseOrderDetailViewModel inv = new PurchaseOrderDetailViewModel();
                            inv.po_detail_id = po.dPo.po_detail_id;
                            inv.purchase_order_id = po.dPo.purchase_order_id;
                            inv.item_id = po.dPo.item_id;
                            inv.product_name = po.product.product_name;
                            inv.product_code = po.product.product_code;
                            inv.product_unit = po.product.product_unit;
                            inv.quantity = remainPOQuantity;
                            inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            inventories.Add(inv);
                        //}
                    }
                }
                */
                #endregion
            }
            return inventories;
        }
        public static List<InventoryDetailViewModel> GetStockBalance(string id)
        {
            List<InventoryDetailViewModel> inventories = new List<InventoryDetailViewModel>();
            List<ProductViewModel> items = new List<ProductViewModel>();
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();

            items = (from item in db.tb_product
                         //join type in db.tb_product_category on item.brand_id equals type.p_category_id
                     join unit in db.tb_unit on item.product_unit equals unit.Id into punit
                     from unit in punit.DefaultIfEmpty()
                     join brand in db.tb_brand on item.brand_id equals brand.brand_id into pbrand
                     from brand in pbrand.DefaultIfEmpty()
                     orderby item.product_code
                     where item.status == true
                     select new ProductViewModel()
                     {
                         product_id = item.product_id,
                         p_category_id = item.brand_id,
                         //p_category_name=type.p_category_name,
                         brand_name = brand.brand_name,
                         product_code = item.product_code,
                         product_name = item.product_name,
                         product_unit = item.product_unit,
                         unit_name = unit.Name,
                         unit_price = item.unit_price
                     }).ToList();
            warehouses = db.tb_warehouse.OrderBy(x => x.warehouse_name).Where(x => x.warehouse_status == true).Select(x => new WareHouseViewModel() { warehouse_id = x.warehouse_id, warehouse_name = x.warehouse_name }).ToList();
            if (id == "All")
            {
                foreach (var item in items)
                {
                    decimal stockBalance = 0;
                    foreach (var warehouse in warehouses)
                    {
                        var itemInv = db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => x.product_id == item.product_id && x.warehouse_id == warehouse.warehouse_id).FirstOrDefault();
                        if (itemInv != null)
                        {
                            decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => x.product_id == item.product_id && x.warehouse_id == warehouse.warehouse_id).Select(x => x.total_quantity).FirstOrDefault());
                            stockBalance += totalQty;
                        }
                    }
                    if (stockBalance > 0)
                        inventories.Add(new InventoryDetailViewModel() { itemTypeId = item.p_category_id, itemTypeName = item.p_category_name, inventory_item_id = item.product_id, itemCode = item.product_code, itemName = item.product_name, itemUnit = item.product_unit, quantity = stockBalance, unit_price = Convert.ToDecimal(item.unit_price) });
                }

            }
            else
            {
                foreach (var item in items)
                {
                    decimal stockBalance = 0;
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => x.product_id == item.product_id && x.warehouse_id == id).Select(x => x.total_quantity).FirstOrDefault());
                    stockBalance += totalQty;
                    if (stockBalance > 0)
                        inventories.Add(new InventoryDetailViewModel() { itemTypeId = item.p_category_id, itemTypeName = item.p_category_name, inventory_item_id = item.product_id, itemCode = item.product_code, itemName = item.product_name, itemUnit = item.product_unit, quantity = stockBalance,unitName=item.unit_name ,unit_price = Convert.ToDecimal(item.unit_price) });
                }
            }
            return inventories;
        }
        #region General 
        public static List<Models.PurchaseOrderViewModel> GetItemReceivedPurchaseOrderList()
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            var objs = db.tb_purchase_order.OrderBy(x => x.created_date).Where(x => x.status == true && (string.Compare(x.purchase_order_status, "Approved") == 0 || string.Compare(x.purchase_order_status, "Completed") == 0)).Select(x => new PurchaseOrderViewModel()
            {
                purchase_order_id = x.purchase_order_id,
                purchase_oder_number = x.purchase_oder_number,
                purchase_order_status = x.purchase_order_status
            }).ToList();
            if (objs.Any())
            {
                foreach (var obj in objs)
                {
                    List<PurchaseOrderDetailViewModel> purchaseOrderItems = new List<PurchaseOrderDetailViewModel>();
                    if (string.Compare(obj.purchase_order_status, "Approved") == 0)
                    {
                        purchaseOrderItems = (from pod in db.tb_purchase_order_detail
                                              where string.Compare(pod.purchase_order_id, obj.purchase_order_id) == 0 && string.Compare(pod.item_status, "Pending") == 0
                                              select new PurchaseOrderDetailViewModel()
                                              {
                                                  item_id = pod.item_id,
                                                  po_quantity = pod.quantity,
                                                  po_unit = pod.item_unit
                                              }).ToList();
                    }
                    else if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                    {
                        purchaseOrderItems = (from pod in db.tb_purchase_order_detail
                                              where string.Compare(pod.purchase_order_id, obj.purchase_order_id) == 0 && string.Compare(pod.item_status, "approved") == 0
                                              select new PurchaseOrderDetailViewModel()
                                              {
                                                  item_id = pod.item_id,
                                                  po_quantity = pod.po_quantity,
                                                  po_unit = pod.po_unit
                                              }).ToList();

                    }
                    List<InventoryViewModel> itemReceiveds = new List<InventoryViewModel>();
                    var POItemReceiveds = db.tb_receive_item_voucher.Where(x => x.status == true && (string.Compare(x.received_status, "Pending") == 0 || string.Compare(x.received_status, "Approved") == 0) && string.Compare(x.ref_id, obj.purchase_order_id) == 0).ToList();
                    if (POItemReceiveds.Any())
                    {

                        foreach (var POItemReceived in POItemReceiveds)
                        {
                            if (string.Compare(POItemReceived.received_status, "Pending") == 0)
                            {
                                var objReceiveds = db.tb_received_item_detail.Where(x => string.Compare(POItemReceived.receive_item_voucher_id, x.ri_ref_id) == 0).ToList();
                                foreach (var objR in objReceiveds)
                                {
                                    itemReceiveds.Add(new InventoryViewModel() { product_id = objR.ri_item_id, total_quantity = objR.quantity, unit = objR.unit });
                                }
                            }
                            else if (string.Compare(POItemReceived.received_status, "Approved") == 0)
                            {
                                var objReceiveds = db.tb_inventory.Where(x => string.Compare(x.ref_id, POItemReceived.receive_item_voucher_id) == 0).ToList();
                                foreach (var objR in objReceiveds)
                                {
                                    itemReceiveds.Add(new InventoryViewModel() { product_id = objR.product_id, total_quantity = objR.in_quantity, unit = objR.unit });
                                }
                            }
                        }
                    }
                    else
                        purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = obj.purchase_order_id, purchase_oder_number = obj.purchase_oder_number });

                    if (itemReceiveds.Any())
                    {
                        int count = 0;
                        foreach (var poItem in purchaseOrderItems)
                        {
                            var purchaseOrderQuantity = CommonClass.ConvertMultipleUnit(poItem.item_id, poItem.po_unit, Convert.ToDecimal(poItem.po_quantity));
                            var results = itemReceiveds.Where(x => string.Compare(poItem.item_id, x.product_id) == 0).ToList();
                            if (results.Any())
                            {
                                foreach (var rs in results)
                                    purchaseOrderQuantity = purchaseOrderQuantity - Convert.ToDecimal(CommonClass.ConvertMultipleUnit(rs.product_id, rs.unit, Convert.ToDecimal(rs.total_quantity)));
                                if (purchaseOrderQuantity > 0)
                                    purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = obj.purchase_order_id, purchase_oder_number = obj.purchase_oder_number });
                            }
                            else
                                count++;
                        }
                        if (count > 0)
                            purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = obj.purchase_order_id, purchase_oder_number = obj.purchase_oder_number });
                    }

                }
            }
            return purchaseOrders;
        }
        #endregion
        #region Transfer From Workshop
        public static List<InventoryViewModel> GetTransferfromWorkshopItems(string id)
        {
            List<InventoryViewModel> stockTransferItems = new List<InventoryViewModel>();
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            inventories=(from detail in db.tb_transfer_frommain_stock_detail
             join item in db.tb_product on detail.st_item_id equals item.product_id
             join tunit in db.tb_unit on detail.unit equals tunit.Id
             join unit in db.tb_unit on item.product_unit equals unit.Id
             join wh in db.tb_warehouse on detail.st_warehouse_id equals wh.warehouse_id
             where string.Compare(detail.st_ref_id, id) == 0
             select new InventoryViewModel()
             {
                 inventory_id = detail.st_detail_id,
                 product_id = detail.st_item_id,
                 itemCode = item.product_code,
                 itemName = item.product_name,
                 itemUnit = item.product_unit,
                 itemUnitName = unit.Name,
                 warehouse_id = detail.st_warehouse_id,
                 out_quantity = detail.quantity,
                 remain_quantity = detail.received_remain_quantity,
                 unit = detail.unit,
                 unitName = tunit.Name,
                 warehouseName = wh.warehouse_name,
                 from_warehouse_id = EnumConstants.WORKSHOP,
                 from_warehouse_name = "Workshop",

             }).ToList();
            return inventories;
        }
        public static List<InventoryViewModel> GetReturnWorkshopItems(string id)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            inventories= (from detail in db.tb_return_mainstock_detail
                          join item in db.tb_product on detail.main_stock_detail_item equals item.product_id
                          join tunit in db.tb_unit on detail.unit equals tunit.Id
                          join unit in db.tb_unit on item.product_unit equals unit.Id
                          join wh in db.tb_warehouse on detail.main_stock_warehouse_id equals wh.warehouse_id
                          where string.Compare(detail.main_stock_detail_ref, id) == 0
                          select new InventoryViewModel()
                          {
                              inventory_id = detail.main_stock_detail_id,
                              product_id = detail.main_stock_detail_item,
                              itemCode = item.product_code,
                              itemName = item.product_name,
                              itemUnit = item.product_unit,
                              itemUnitName = unit.Name,
                              warehouse_id = detail.main_stock_warehouse_id,
                              out_quantity = detail.quantity,
                              remain_quantity = detail.remain_quantity,
                              unit = detail.unit,
                              unitName = tunit.Name,
                              warehouseName = wh.warehouse_name,
                              from_warehouse_id = EnumConstants.WORKSHOP,
                              from_warehouse_name = "Workshop",

                          }).ToList();
            return inventories;
        }
        #endregion
        public static List<InventoryViewModel> GetStockReturnItems(string id)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            inventories=(from detail in db.tb_inventory_detail
             join product in db.tb_product on detail.inventory_item_id equals product.product_id
             join punit in db.tb_unit on product.product_unit equals punit.Id
             join iunit in db.tb_unit on detail.unit equals iunit.Id
             join wh in db.tb_warehouse on detail.inventory_warehouse_id equals wh.warehouse_id
             where string.Compare(detail.inventory_ref_id, id) == 0
             select new InventoryViewModel()
             {
                 inventory_id = detail.inventory_detail_id,
                 product_id = detail.inventory_item_id,
                 itemCode = product.product_code,
                 itemName = product.product_name,
                 itemUnit = product.product_unit,
                 itemUnitName = punit.Name,
                 warehouse_id = detail.inventory_warehouse_id,
                 out_quantity = detail.quantity,
                 remain_quantity = detail.remain_quantity,
                 unit = detail.unit,
                 unitName = iunit.Name,
                 warehouseName = wh.warehouse_name,
             }).ToList();
            return inventories;
        }
        public static List<ItemReceiveViewModel> GetReceivedHistory(string poId,string currentReceiveId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                var objs = db.tb_receive_item_voucher.OrderBy(s=>s.created_date).Where(s => s.status == true && string.Compare(s.ref_id, poId) == 0 && string.Compare(s.receive_item_voucher_id,currentReceiveId)!=0).ToList();
                if (objs.Any())
                {
                    foreach(var obj in objs)
                    {
                        ItemReceiveViewModel model = new ItemReceiveViewModel();
                        model.created_date = obj.created_date;
                        model.received_status = obj.received_status;
                        model.inventories = db.tb_received_item_detail.Where(s => string.Compare(s.ri_ref_id, obj.receive_item_voucher_id) == 0).Select(s => new InventoryViewModel()
                        {
                            product_id=s.ri_item_id,
                            in_quantity=s.quantity,
                            unit=s.unit,
                        }).ToList();
                        models.Add(model);
                    }
                }
                return models;
            }
        }
    }
    public class StockTransfer
    {
        public static void RollBackItemQuantitybyStockTransfer(string id, string stId, bool isRollback, string item_status = null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countReturn = 0;
                List<Models.ItemRequestDetail2ViewModel> itemRequests = new List<ItemRequestDetail2ViewModel>();
                List<Models.StockTransferItemViewModel> stockTransferItems = new List<StockTransferItemViewModel>();

                itemRequests = (from item in db.tb_ir_detail2
                                join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                                where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                                select new Models.ItemRequestDetail2ViewModel() { ir_detail2_id = item.ir_detail2_id, ir_item_id = item.ir_item_id, ir_item_unit = item.ir_item_unit }).ToList();
                if (string.IsNullOrEmpty(item_status))
                {
                    stockTransferItems = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, stId) == 0).Select(x => new Models.StockTransferItemViewModel()
                    {
                        st_item_id = x.st_item_id,
                        quantity = x.quantity,
                        itemUnit = x.unit,
                        is_return=x.status
                    }).ToList();
                }
                else
                {
                    stockTransferItems = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, stId) == 0 && string.Compare(x.item_status, item_status) == 0).Select(x => new Models.StockTransferItemViewModel()
                    {
                        st_item_id = x.st_item_id,
                        quantity = x.quantity,
                        itemUnit = x.unit,
                        is_return = x.status
                    }).ToList();
                }

                foreach (StockTransferItemViewModel transferItem in stockTransferItems)
                {
                    if (Convert.ToBoolean(transferItem.is_return))
                        countReturn++;
                    else
                    {
                        string irDetail2Id = itemRequests.Where(x => string.Compare(x.ir_item_id, transferItem.st_item_id) == 0).Select(x => x.ir_detail2_id).FirstOrDefault();
                        if (irDetail2Id != null)
                        {
                            tb_ir_detail2 irDetail2 = db.tb_ir_detail2.Find(irDetail2Id);
                            if (isRollback)
                            {
                                irDetail2.remain_qty = irDetail2.remain_qty + Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.itemUnit, Convert.ToDecimal(transferItem.quantity), irDetail2.ir_item_unit);
                            }
                            else
                            {
                                irDetail2.remain_qty = irDetail2.remain_qty - Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.itemUnit, Convert.ToDecimal(transferItem.quantity), irDetail2.ir_item_unit);
                            }
                            db.SaveChanges();
                        }
                    }
                    
                }

                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(stId);
                stockTransfer.is_return = countReturn == 0 ? false : true;
                db.SaveChanges();

                ItemRequest.UpdateCompletedItemRequest(id,true,countReturn==0?false:true);
            }
        }
        public static string GenerateInvoiceNumber(string warehouseId, DateTime invoiceDate)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string invoiceNumber = string.Empty;
                DateTime date = invoiceNumber == null ? DateTime.Now : invoiceDate;
                string YY = date.Year.ToString().Substring(2, 2);
                string MM = date.Month.ToString().Count() == 1 ? "0" + date.Month.ToString() : date.Month.ToString();
                string invoiceCompare = string.Format("IVN{0}{1}", YY, MM);
                string lastInvoiceNumber = db.tb_invoice.OrderByDescending(x => x.invoice_date).Where(x => x.invoice_number.Contains(invoiceCompare) && string.Compare(x.warehouse_id, warehouseId) == 0).Select(x => x.invoice_number).FirstOrDefault();
                int lastDigit = lastInvoiceNumber == null ? 1 : Convert.ToInt32(lastInvoiceNumber.Substring(lastInvoiceNumber.Count() - 3, 3)) + 1;
                string lastSplit = lastDigit.ToString().Length == 1 ? string.Format("00{0}", lastDigit.ToString()) : lastDigit.ToString().Length == 2 ? string.Format("0{0}", lastDigit.ToString()) : lastDigit.ToString();
                invoiceNumber = string.Format("IVN{0}{1}{2}", YY, MM, lastSplit);
                return invoiceNumber;
            }
        }
        public static void GenerateWarehouseInvoiceNumber(string stockTransferId, List<Models.InventoryViewModel> inventories)
        {
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    List<string> warehouses = new List<string>();
                    var dupWarehouses = inventories.GroupBy(x => x.warehouse_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupWarehouses.Any())
                    {
                        foreach (var dwarehouse in dupWarehouses)
                            warehouses.Add(dwarehouse);
                        foreach (InventoryViewModel inv in inventories)
                        {
                            bool isDupWarehouse = dupWarehouses.Where(x => string.Compare(x, inv.warehouse_id) == 0).Count() > 0 ? true : false;
                            if (!isDupWarehouse) warehouses.Add(inv.warehouse_id);
                        }
                    }
                    else
                    {
                        foreach (InventoryViewModel inv in inventories)
                            warehouses.Add(inv.warehouse_id);
                    }
                    foreach (string warehouse in warehouses)
                    {
                        List<InventoryViewModel> warehouseItems = inventories.Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();
                        if (warehouseItems.Count() == 1)
                        {
                            DateTime invoiceDate = warehouseItems[0].invoice_date == null ? DateTime.Now : Convert.ToDateTime(warehouseItems[0].invoice_date);
                            string invoiceNumber = string.IsNullOrEmpty(warehouseItems[0].invoice_number) ? StockTransfer.GenerateInvoiceNumber(warehouse, invoiceDate) : warehouseItems[0].invoice_number;
                            tb_invoice invoice = new tb_invoice();
                            invoice.invoice_id = Guid.NewGuid().ToString();
                            invoice.ref_id = stockTransferId;
                            invoice.warehouse_id = warehouse;
                            invoice.invoice_date = invoiceDate;
                            invoice.invoice_number = invoiceNumber;
                            db.tb_invoice.Add(invoice);
                            db.SaveChanges();
                        }
                        else
                        {
                            warehouseItems = warehouseItems.OrderByDescending(x => x.invoice_date).ThenByDescending(x => x.invoice_number).ToList();
                            foreach (InventoryViewModel warehouseItem in warehouseItems)
                            {
                                string invoiceNumber = string.Empty;
                                DateTime invoiceDate = warehouseItem.invoice_date == null ? DateTime.Now : Convert.ToDateTime(warehouseItem.invoice_date);
                                if (string.IsNullOrEmpty(warehouseItem.invoice_number))
                                {
                                    var invoice = db.tb_invoice.Where(x => string.Compare(x.ref_id, stockTransferId) == 0 && string.Compare(x.warehouse_id, warehouse) == 0 && x.invoice_date.Value.Date == invoiceDate.Date).Select(x => x.invoice_number).FirstOrDefault();
                                    if (invoice == null) invoiceNumber = StockTransfer.GenerateInvoiceNumber(warehouse, invoiceDate);
                                    else invoiceNumber = invoice;
                                }
                                else
                                    invoiceNumber = warehouseItem.invoice_number;
                                var InvoiceExist = db.tb_invoice.Where(x => string.Compare(x.ref_id, stockTransferId) == 0 && string.Compare(x.warehouse_id, warehouse) == 0 && (x.invoice_date >= invoiceDate && x.invoice_date <= invoiceDate) && string.Compare(x.invoice_number, invoiceNumber) == 0).FirstOrDefault();
                                if (InvoiceExist == null)
                                {
                                    tb_invoice invoice = new tb_invoice();
                                    invoice.invoice_id = Guid.NewGuid().ToString();
                                    invoice.ref_id = stockTransferId;
                                    invoice.warehouse_id = warehouse;
                                    invoice.invoice_date = invoiceDate;
                                    invoice.invoice_number = invoiceNumber;
                                    db.tb_invoice.Add(invoice);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public static void UpdateStockTransferProcessStatus(string materialRequest,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var stockTransfers = db.tb_stock_transfer_voucher.Where(s => s.status == true && string.Compare(s.item_request_id, materialRequest) == 0 && s.is_return == true).ToList();
                foreach (var stockTransfer in stockTransfers)
                {
                    string id = stockTransfer.stock_transfer_id;
                    tb_stock_transfer_voucher st = db.tb_stock_transfer_voucher.Find(id);
                    st.mr_status = status;
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateStockTransferProcessStatusById(string stockTransferId,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var stockTransfer = db.tb_stock_transfer_voucher.Find(stockTransferId);
                if (stockTransfer != null)
                {
                    stockTransfer.received_status = status;
                    if (!Convert.ToBoolean(stockTransfer.is_return))
                    {
                        stockTransfer.mr_status = string.Compare(status, ShowStatus.STCompleted) == 0 ? ShowStatus.MRCompleted : status; 
                        tb_item_request materialRequest = db.tb_item_request.Find(stockTransfer.item_request_id);
                        materialRequest.st_status = string.Compare(status,ShowStatus.STCompleted)==0?ShowStatus.MRCompleted : status;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateTransferMainStockProcessStatusbyId(string transferId, string status)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var transferMainStock = db.transferformmainstocks.Find(transferId);
                
            }
        }
    }
    public class StockIssueReturn
    {
        public static List<InventoryViewModel> GetStockIssueItemsReferences(string id, string returnId = null)
        {
            List<InventoryViewModel> models = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(returnId))
                {
                    var objs = (from inv in db.tb_stock_transfer_detail
                                join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                join item in db.tb_product on inv.st_item_id equals item.product_id
                                orderby inv.ordering_number
                                where string.Compare(inv.st_ref_id, id) == 0 && inv.status == true
                                select new InventoryViewModel()
                                {
                                    inventory_id = inv.st_detail_id,
                                    //inventory_date = inv.inventory_date,
                                    product_id = inv.st_item_id,
                                    itemName = item.product_name,
                                    ordering_number = inv.ordering_number,
                                    itemCode = item.product_code,
                                    itemUnit = item.product_unit,
                                    //itemUnitName = db.tb_unit.Find(item.product_unit).Name,
                                    warehouse_id = inv.st_warehouse_id,
                                    warehouseName = wah.warehouse_name,
                                    total_quantity = inv.quantity,
                                    unit = inv.unit,
                                    //in_quantity = inv.in_quantity,
                                    out_quantity = inv.return_remain_quantity,
                                    remark = inv.remark,
                                    uom = db.tb_multiple_uom.Where(x => x.product_id == inv.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                }).ToList();
                    foreach (var obj in objs)
                    {
                        InventoryViewModel model = new InventoryViewModel();
                        model.inventory_id = obj.inventory_id;
                        model.product_id = obj.product_id;
                        model.itemName = obj.itemName;
                        model.itemCode = obj.itemCode;
                        model.ordering_number = obj.ordering_number;
                        model.itemUnit = obj.itemUnit;
                        model.itemUnitName = db.tb_unit.Find(obj.itemUnit).Name;
                        model.warehouse_id = obj.warehouse_id;
                        model.warehouseName = obj.warehouseName;
                        model.total_quantity = obj.total_quantity;
                        model.unit = obj.unit;
                        model.out_quantity = obj.out_quantity;
                        model.remark = obj.remark;
                        model.uom = obj.uom;
                        models.Add(model);
                    }
                }
                else
                {
                    var issueRef = db.tb_stock_issue_return.Find(returnId);
                    if (string.Compare(issueRef.stock_issue_ref, id) == 0)
                    {
                        var objs = (from inv in db.tb_stock_transfer_detail
                                    join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                    join item in db.tb_product on inv.st_item_id equals item.product_id
                                    orderby inv.ordering_number
                                    where string.Compare(inv.st_ref_id, id) == 0
                                    select new InventoryViewModel()
                                    {
                                        inventory_id = inv.st_detail_id,
                                        //inventory_date = inv.inventory_date,
                                        product_id = inv.st_item_id,
                                        itemName = item.product_name,
                                        itemCode = item.product_code,
                                        ordering_number = inv.ordering_number,
                                        itemUnit = item.product_unit,
                                        itemUnitName = inv.unit,
                                        warehouse_id = inv.st_warehouse_id,
                                        warehouseName = wah.warehouse_name,
                                        total_quantity = inv.quantity,
                                        //in_quantity = inv.in_quantity,
                                        out_quantity = inv.return_remain_quantity,
                                        remark = inv.remark,
                                        uom = db.tb_multiple_uom.Where(x => x.product_id == inv.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                    }).ToList();

                        var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).ToList();
                        foreach (var obj in objs)
                        {
                            decimal returnQty = 0;
                            var returnItem = returnItems.Where(x => string.Compare(obj.product_id, x.inventory_item_id) == 0).FirstOrDefault();
                            if (returnItem != null)
                                returnQty = Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                            InventoryViewModel model = new InventoryViewModel();
                            model.inventory_id = obj.inventory_id;
                            model.product_id = obj.product_id;
                            model.itemName = obj.itemName;
                            model.itemCode = obj.itemCode;
                            model.itemUnit = obj.itemUnit;
                            model.ordering_number = obj.ordering_number;
                            model.warehouse_id = obj.warehouse_id;
                            model.warehouseName = obj.warehouseName;
                            model.total_quantity = obj.total_quantity;
                            model.unit = obj.unit;
                            model.out_quantity = obj.out_quantity + returnQty;
                            model.remark = obj.remark;
                            model.uom = obj.uom;
                            models.Add(model);
                        }
                    }
                    else
                    {
                        var objs = (from inv in db.tb_inventory_detail
                                    join wah in db.tb_warehouse on inv.inventory_warehouse_id equals wah.warehouse_id
                                    join item in db.tb_product on inv.inventory_item_id equals item.product_id
                                    orderby inv.ordering_number
                                    where string.Compare(inv.inventory_ref_id, id) == 0 && inv.remain_quantity > 0
                                    select new InventoryViewModel()
                                    {
                                        inventory_id = inv.inventory_detail_id,
                                        //inventory_date = inv.inventory_date,
                                        product_id = inv.inventory_item_id,
                                        itemName = item.product_name,
                                        ordering_number = inv.ordering_number,
                                        itemCode = item.product_code,
                                        itemUnit = item.product_unit,
                                        warehouse_id = inv.inventory_warehouse_id,
                                        warehouseName = wah.warehouse_name,
                                        total_quantity = inv.quantity,
                                        unit = inv.unit,
                                        //in_quantity = inv.in_quantity,
                                        out_quantity = inv.remain_quantity,
                                        remark = inv.remark,
                                        uom = db.tb_multiple_uom.Where(x => x.product_id == inv.inventory_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            InventoryViewModel model = new InventoryViewModel();
                            model.inventory_id = obj.inventory_id;
                            model.product_id = obj.product_id;
                            model.itemName = obj.itemName;
                            model.ordering_number = obj.ordering_number;
                            model.itemCode = obj.itemCode;
                            model.itemUnit = obj.itemUnit;
                            model.warehouse_id = obj.warehouse_id;
                            model.warehouseName = obj.warehouseName;
                            model.total_quantity = obj.total_quantity;
                            model.unit = obj.unit;
                            model.out_quantity = obj.out_quantity;
                            model.remark = obj.remark;
                            model.uom = obj.uom;
                            models.Add(model);
                        }
                    }
                }
            }
            return models;
        }
        public static void RollbackStockIssueItems(string id, string returnId, bool isRollBack, string status = null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<InventoryDetailViewModel> issueItems = new List<InventoryDetailViewModel>();
                List<InventoryDetailViewModel> issueReturnItems = new List<InventoryDetailViewModel>();
                //issueItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();
                issueItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, id) == 0).Select(s => new InventoryDetailViewModel() { inventory_detail_id = s.st_detail_id, inventory_item_id = s.st_item_id }).ToList();
                if (string.IsNullOrEmpty(status))
                {
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                    //issueReturnItems=db.tb_stock_transfer_detail.Where(s=>string.Compare(s.st_ref_id,returnId)==0).Select(x => new InventoryDetailViewModel() { inventory_item_id = x.st_ref_id, quantity = x.quantity, unit = x.unit }).ToList();
                }
                else
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0 && string.Compare(x.item_status, status) == 0). Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                    //issueReturnItems = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, returnId) == 0 && string.Compare(s.item_status,status)==0).Select(x => new InventoryDetailViewModel() { inventory_item_id = x.st_ref_id, quantity = x.quantity, unit = x.unit }).ToList();

                foreach (InventoryDetailViewModel returnItem in issueReturnItems)
                {
                    string issueItemId = issueItems.Where(x => string.Compare(x.inventory_item_id, returnItem.inventory_item_id) == 0).Select(x => x.inventory_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(issueItemId))
                    {
                        //tb_inventory_detail issueDetail = db.tb_inventory_detail.Find(issueItemId);
                        tb_stock_transfer_detail issueDetail = db.tb_stock_transfer_detail.Find(issueItemId);
                        if (isRollBack)
                            issueDetail.return_remain_quantity = issueDetail.return_remain_quantity + Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                        else
                            issueDetail.return_remain_quantity = issueDetail.return_remain_quantity - Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                        db.SaveChanges();
                    }
                }
                UpdateStockIssueCompleted(id);
            }
        }
        public static List<InventoryViewModel> GetStockIssueItemsReferences1(string id, string returnId = null)
        {
            List<InventoryViewModel> models = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(returnId))
                {
                    var objs = (from inv in db.tb_stock_transfer_detail
                                join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                join item in db.tb_product on inv.st_item_id equals item.product_id
                                orderby inv.ordering_number
                                where string.Compare(inv.st_ref_id, id) == 0 && inv.status == true && inv.return_remain_quantity > 0
                                select new InventoryViewModel()
                                {
                                    inventory_id = inv.st_detail_id,
                                    //inventory_date = inv.inventory_date,
                                    product_id = inv.st_item_id,
                                    itemName = item.product_name,
                                    itemCode = item.product_code,
                                    itemUnit = item.product_unit,
                                    //itemUnitName = db.tb_unit.Find(item.product_unit).Name,
                                    warehouse_id = inv.st_warehouse_id,
                                    warehouseName = wah.warehouse_name,
                                    total_quantity = inv.quantity,
                                    unit = inv.unit,
                                    //in_quantity = inv.in_quantity,
                                    out_quantity = inv.return_remain_quantity,
                                    remark = inv.remark,
                                    uom = db.tb_multiple_uom.Where(x => x.product_id == inv.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                }).ToList();
                    foreach (var obj in objs)
                    {
                        InventoryViewModel model = new InventoryViewModel();
                        model.inventory_id = obj.inventory_id;
                        model.product_id = obj.product_id;
                        model.itemName = obj.itemName;
                        model.itemCode = obj.itemCode;
                        model.itemUnit = obj.itemUnit;
                        model.itemUnitName = db.tb_unit.Find(obj.itemUnit).Name;
                        model.warehouse_id = obj.warehouse_id;
                        model.warehouseName = obj.warehouseName;
                        model.total_quantity = obj.total_quantity;
                        model.unit = obj.unit;
                        model.out_quantity = obj.out_quantity;
                        model.remark = obj.remark;
                        model.uom = obj.uom;
                        models.Add(model);
                    }
                }
                else
                {
                    var issueRef = db.tb_stock_issue_return.Find(returnId);
                    if (string.Compare(issueRef.stock_issue_ref, id) == 0)
                    {
                        var objs = (from inv in db.tb_stock_transfer_detail
                                    join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                    join item in db.tb_product on inv.st_item_id equals item.product_id
                                    orderby inv.ordering_number
                                    where string.Compare(inv.st_ref_id, id) == 0
                                    select new InventoryViewModel()
                                    {
                                        inventory_id = inv.st_detail_id,
                                        //inventory_date = inv.inventory_date,
                                        product_id = inv.st_item_id,
                                        itemName = item.product_name,
                                        itemCode = item.product_code,
                                        itemUnit = item.product_unit,
                                        itemUnitName = inv.unit,
                                        warehouse_id = inv.st_warehouse_id,
                                        warehouseName = wah.warehouse_name,
                                        total_quantity = inv.quantity,
                                        //in_quantity = inv.in_quantity,
                                        out_quantity = inv.return_remain_quantity,
                                        remark = inv.remark,
                                        uom = db.tb_multiple_uom.Where(x => x.product_id == inv.st_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                    }).ToList();

                        var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).ToList();
                        foreach (var obj in objs)
                        {
                            decimal returnQty = 0;
                            var returnItem = returnItems.Where(x => string.Compare(obj.product_id, x.inventory_item_id) == 0).FirstOrDefault();
                            if (returnItem != null)
                                returnQty = Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                            InventoryViewModel model = new InventoryViewModel();
                            model.inventory_id = obj.inventory_id;
                            model.product_id = obj.product_id;
                            model.itemName = obj.itemName;
                            model.itemCode = obj.itemCode;
                            model.itemUnit = obj.itemUnit;
                            model.warehouse_id = obj.warehouse_id;
                            model.warehouseName = obj.warehouseName;
                            model.total_quantity = obj.total_quantity;
                            model.unit = obj.unit;
                            model.out_quantity = obj.out_quantity + returnQty;
                            model.remark = obj.remark;
                            model.uom = obj.uom;
                            models.Add(model);
                        }
                    }
                    else
                    {
                        var objs = (from inv in db.tb_inventory_detail
                                    join wah in db.tb_warehouse on inv.inventory_warehouse_id equals wah.warehouse_id
                                    join item in db.tb_product on inv.inventory_item_id equals item.product_id
                                    orderby inv.ordering_number
                                    where string.Compare(inv.inventory_ref_id, id) == 0 && inv.remain_quantity > 0
                                    select new InventoryViewModel()
                                    {
                                        inventory_id = inv.inventory_detail_id,
                                        //inventory_date = inv.inventory_date,
                                        product_id = inv.inventory_item_id,
                                        itemName = item.product_name,
                                        itemCode = item.product_code,
                                        itemUnit = item.product_unit,
                                        warehouse_id = inv.inventory_warehouse_id,
                                        warehouseName = wah.warehouse_name,
                                        total_quantity = inv.quantity,
                                        unit = inv.unit,
                                        //in_quantity = inv.in_quantity,
                                        out_quantity = inv.remain_quantity,
                                        remark = inv.remark,
                                        uom = db.tb_multiple_uom.Where(x => x.product_id == inv.inventory_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            InventoryViewModel model = new InventoryViewModel();
                            model.inventory_id = obj.inventory_id;
                            model.product_id = obj.product_id;
                            model.itemName = obj.itemName;
                            model.itemCode = obj.itemCode;
                            model.itemUnit = obj.itemUnit;
                            model.warehouse_id = obj.warehouse_id;
                            model.warehouseName = obj.warehouseName;
                            model.total_quantity = obj.total_quantity;
                            model.unit = obj.unit;
                            model.out_quantity = obj.out_quantity;
                            model.remark = obj.remark;
                            model.uom = obj.uom;
                            models.Add(model);
                        }
                    }
                }
            }
            return models;
        }
        public static void RollbackStockIssueItems1(string id, string returnId, bool isRollBack, string status = null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<InventoryDetailViewModel> issueItems = new List<InventoryDetailViewModel>();
                List<InventoryDetailViewModel> issueReturnItems = new List<InventoryDetailViewModel>();
                var item = (from a in db.tb_inventory_detail join b in db.tb_stock_issue_return on a.inventory_ref_id equals b.stock_issue_return_id join c in db.tb_stock_transfer_voucher on b.stock_issue_ref equals c.stock_transfer_id where c.stock_transfer_id == id select new { a.inventory_detail_id, a.inventory_item_id }).ToList();
                //issueItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();
                issueItems = item.Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();

                if (string.IsNullOrEmpty(status))
                {
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).
                        Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit, inventory_warehouse_id = x.inventory_warehouse_id }).ToList();

                }
                else
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0 && string.Compare(x.item_status, status) == 0).
                        Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit, inventory_warehouse_id = x.inventory_warehouse_id }).ToList();
                foreach (InventoryDetailViewModel returnItem in issueReturnItems)
                {
                    string issueItemId = issueItems.Where(x => string.Compare(x.inventory_item_id, returnItem.inventory_item_id) == 0).Select(x => x.inventory_detail_id).FirstOrDefault().ToString();


                    if (!string.IsNullOrEmpty(issueItemId))
                    {

                        if (!string.IsNullOrEmpty(issueItemId))
                        {


                            //                     //var fd = (from a in db.tb_stock_transfer_detail where a.st_item_id == returnItem.inventory_item_id && a.st_warehouse_id == returnItem.inventory_warehouse_id select a.st_detail_id).FirstOrDefault();

                            //                     var fd = (from a in db.tb_stock_transfer_detail join b in db.tb_stock_transfer_voucher on a.st_ref_id equals b.stock_transfer_id
                            //                               join c in db.tb_stock_issue_return on b.stock_transfer_id equals c.stock_issue_ref
                            //                               join d in db.tb_inventory_detail on c.stock_issue_return_id equals d.inventory_ref_id       
                            //                               where d.inventory_detail_id == issueItemId && a.st_item_id == returnItem.inventory_item_id && a.st_warehouse_id == returnItem.inventory_warehouse_id select a.st_detail_id).FirstOrDefault();

                            //                     tb_stock_transfer_detail issueDetail = db.tb_stock_transfer_detail.Find(fd);

                            //                     if (isRollBack)
                            //                             issueDetail.remain_quantity = issueDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                            //                         else
                            //                             issueDetail.remain_quantity = issueDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));


                            db.SaveChanges();
                        }
                    }
                    UpdateStockIssueCompleted(id);
                }
            }
        }
        public static void UpdateStockIssueCompleted(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countNotCompleted = 0;
                //var issueItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).ToList();
                var item = (from a in db.tb_inventory_detail
                            join b in db.tb_stock_issue_return on a.inventory_ref_id equals b.stock_issue_return_id
                            join c in db.tb_stock_transfer_voucher on b.stock_issue_ref equals c.stock_transfer_id
                            join d in db.tb_stock_transfer_detail on c.stock_transfer_id equals d.st_ref_id
                            where c.stock_transfer_id == id && d.status == true
                            select new { a, b, c, d }
                            ).ToList();
                foreach (var issueItem in item)
                    if (issueItem.d.return_remain_quantity > 0) countNotCompleted++;
                tb_stock_transfer_voucher stockIssue = db.tb_stock_transfer_voucher.Find(id);
                stockIssue.is_return_complete = countNotCompleted == 0 ? true : false;
               
                db.SaveChanges();
            }
        }
    }
    public class TransferFromMainStock
    {
        public static void RollBackItemQuantitybyTransferFromMainStock(string id, string stId, bool isRollback, string item_status = null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countReturn = 0;
                List<Models.ItemRequestDetail2ViewModel> itemRequests = new List<ItemRequestDetail2ViewModel>();
                List<Models.TransferFromMainStockItemViewModel> transferFromMainStockItems = new List<TransferFromMainStockItemViewModel>();

                itemRequests = (from item in db.tb_ir_detail2
                                join job in db.tb_ir_detail1 on item.ir_detail1_id equals job.ir_detail1_id
                                join pr in db.tb_item_request on job.ir_id equals pr.ir_id
                                where string.Compare(pr.ir_id, id) == 0 && item.is_approved == true
                                select new Models.ItemRequestDetail2ViewModel() { ir_detail2_id = item.ir_detail2_id, ir_item_id = item.ir_item_id, ir_item_unit = item.ir_item_unit }).ToList();
                if (string.IsNullOrEmpty(item_status))
                {
                    transferFromMainStockItems = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, stId) == 0).Select(x => new Models.TransferFromMainStockItemViewModel()
                    {
                        st_item_id = x.st_item_id,
                        quantity = x.quantity,
                        itemUnit = x.unit,
                        
                    }).ToList();
                }
                else
                {
                    transferFromMainStockItems = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, stId) == 0 && string.Compare(x.item_status, item_status) == 0).Select(x => new Models.TransferFromMainStockItemViewModel()
                    {
                        st_item_id = x.st_item_id,
                        quantity = x.quantity,
                        itemUnit = x.unit,
                    }).ToList();
                }

                foreach (TransferFromMainStockItemViewModel transferItem in transferFromMainStockItems)
                {
                    string irDetail2Id = itemRequests.Where(x => string.Compare(x.ir_item_id, transferItem.st_item_id) == 0).Select(x => x.ir_detail2_id).FirstOrDefault();
                    if (irDetail2Id != null)
                    {
                        tb_ir_detail2 irDetail2 = db.tb_ir_detail2.Find(irDetail2Id);
                        if (isRollback)
                        {
                            irDetail2.remain_qty = irDetail2.remain_qty + Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.itemUnit, Convert.ToDecimal(transferItem.quantity), irDetail2.ir_item_unit);
                        }
                        else
                        {
                            irDetail2.remain_qty = irDetail2.remain_qty - Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.itemUnit, Convert.ToDecimal(transferItem.quantity), irDetail2.ir_item_unit);
                        }
                        db.SaveChanges();
                    }
                }
                ItemRequest.UpdateCompletedItemRequest(id,true,false);
            }
        }


        public static string GenerateInvoiceNumber(string warehouseId, DateTime invoiceDate)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string invoiceNumber = string.Empty;
                DateTime date = invoiceNumber == null ? DateTime.Now : invoiceDate;
                string YY = date.Year.ToString().Substring(2, 2);
                string MM = date.Month.ToString().Count() == 1 ? "0" + date.Month.ToString() : date.Month.ToString();
                string invoiceCompare = string.Format("IVN{0}{1}", YY, MM);
                string lastInvoiceNumber = db.tb_invoice.OrderByDescending(x => x.invoice_date).Where(x => x.invoice_number.Contains(invoiceCompare) && string.Compare(x.warehouse_id, warehouseId) == 0).Select(x => x.invoice_number).FirstOrDefault();
                int lastDigit = lastInvoiceNumber == null ? 1 : Convert.ToInt32(lastInvoiceNumber.Substring(lastInvoiceNumber.Count() - 3, 3)) + 1;
                string lastSplit = lastDigit.ToString().Length == 1 ? string.Format("00{0}", lastDigit.ToString()) : lastDigit.ToString().Length == 2 ? string.Format("0{0}", lastDigit.ToString()) : lastDigit.ToString();
                invoiceNumber = string.Format("IVN{0}{1}{2}", YY, MM, lastSplit);
                return invoiceNumber;
            }
        }
        public static void GenerateWarehouseInvoiceNumber(string transferFromMainStockId, List<Models.InventoryViewModel> inventories)
        {
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    List<string> warehouses = new List<string>();
                    var dupWarehouses = inventories.GroupBy(x => x.warehouse_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupWarehouses.Any())
                    {
                        foreach (var dwarehouse in dupWarehouses)
                            warehouses.Add(dwarehouse);
                        foreach (InventoryViewModel inv in inventories)
                        {
                            bool isDupWarehouse = dupWarehouses.Where(x => string.Compare(x, inv.warehouse_id) == 0).Count() > 0 ? true : false;
                            if (!isDupWarehouse) warehouses.Add(inv.warehouse_id);
                        }
                    }
                    else
                    {
                        foreach (InventoryViewModel inv in inventories)
                            warehouses.Add(inv.warehouse_id);
                    }
                    foreach (string warehouse in warehouses)
                    {
                        List<InventoryViewModel> warehouseItems = inventories.Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();
                        if (warehouseItems.Count() == 1)
                        {
                            DateTime invoiceDate = warehouseItems[0].invoice_date == null ? DateTime.Now : Convert.ToDateTime(warehouseItems[0].invoice_date);
                            string invoiceNumber = string.IsNullOrEmpty(warehouseItems[0].invoice_number) ? TransferFromMainStock.GenerateInvoiceNumber(warehouse, invoiceDate) : warehouseItems[0].invoice_number;
                            tb_invoice invoice = new tb_invoice();
                            invoice.invoice_id = Guid.NewGuid().ToString();
                            invoice.ref_id = transferFromMainStockId;
                            invoice.warehouse_id = warehouse;
                            invoice.invoice_date = invoiceDate;
                            invoice.invoice_number = invoiceNumber;
                            db.tb_invoice.Add(invoice);
                            db.SaveChanges();
                        }
                        else
                        {
                            warehouseItems = warehouseItems.OrderByDescending(x => x.invoice_date).ThenByDescending(x => x.invoice_number).ToList();
                            foreach (InventoryViewModel warehouseItem in warehouseItems)
                            {
                                string invoiceNumber = string.Empty;
                                DateTime invoiceDate = warehouseItem.invoice_date == null ? DateTime.Now : Convert.ToDateTime(warehouseItem.invoice_date);
                                if (string.IsNullOrEmpty(warehouseItem.invoice_number))
                                {
                                    var invoice = db.tb_invoice.Where(x => string.Compare(x.ref_id, transferFromMainStockId) == 0 && string.Compare(x.warehouse_id, warehouse) == 0 && x.invoice_date.Value.Date == invoiceDate.Date).Select(x => x.invoice_number).FirstOrDefault();
                                    if (invoice == null) invoiceNumber = TransferFromMainStock.GenerateInvoiceNumber(warehouse, invoiceDate);
                                    else invoiceNumber = invoice;
                                }
                                else
                                    invoiceNumber = warehouseItem.invoice_number;
                                var InvoiceExist = db.tb_invoice.Where(x => string.Compare(x.ref_id,
                                    transferFromMainStockId) == 0 && string.Compare(x.warehouse_id, warehouse) == 0 && (x.invoice_date >= invoiceDate && x.invoice_date <= invoiceDate) && string.Compare(x.invoice_number, invoiceNumber) == 0).FirstOrDefault();
                                if (InvoiceExist == null)
                                {
                                    tb_invoice invoice = new tb_invoice();
                                    invoice.invoice_id = Guid.NewGuid().ToString();
                                    invoice.ref_id = transferFromMainStockId;
                                    invoice.warehouse_id = warehouse;
                                    invoice.invoice_date = invoiceDate;
                                    invoice.invoice_number = invoiceNumber;
                                    db.tb_invoice.Add(invoice);
                                    db.SaveChanges();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public static void UpdateCompletedTransferWorkshop(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countNotCompleteItem = 0;
                var items = (from item in db.tb_transfer_frommain_stock_detail
                             where string.Compare(item.st_ref_id, id) == 0
                             select new { item }).ToList();
                foreach (var item in items)
                    if (item.item.remain_quantity > 0) countNotCompleteItem++;
                transferformmainstock ir = db.transferformmainstocks.Find(id);
                if (ir!=null){
                    ir.is_completed = countNotCompleteItem == 0 ? true : false;
                    db.SaveChanges();
                }
                
            }
        }
    }
    public class ReturnToWorkshop
    {
        public static void RollBackItemQuantitybyWorkshopTransfer(string transferId, string returnId, bool isRollback, string item_status = null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<Models.TransferFromMainStockItemViewModel> transferItems = new List<TransferFromMainStockItemViewModel>();
                List<Models.ReturnMainStockItemViewModel> returnItems = new List<ReturnMainStockItemViewModel>();

                transferItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, transferId) == 0)
                    .Select(s => new TransferFromMainStockItemViewModel()
                    {
                        st_detail_id=s.st_detail_id,
                        st_item_id=s.st_item_id,
                        unit=s.unit,

                    }).ToList();

                if (string.IsNullOrEmpty(item_status))
                {
                    returnItems = db.tb_return_mainstock_detail.Where(s => string.Compare(s.main_stock_detail_ref, returnId) == 0).Select(s => new ReturnMainStockItemViewModel()
                    {
                        st_item_id=s.main_stock_detail_item,
                        quantity=s.quantity,
                        unit=s.unit
                    }).ToList();
                }
                else
                {
                    returnItems = db.tb_return_mainstock_detail.Where(s => string.Compare(s.main_stock_detail_ref, returnId) == 0 && string.Compare(s.item_status,item_status)==0).Select(s => new ReturnMainStockItemViewModel()
                    {
                        st_item_id = s.main_stock_detail_item,
                        quantity = s.quantity,
                        unit = s.unit
                    }).ToList();
                }

                foreach (ReturnMainStockItemViewModel transferItem in returnItems)
                {
                    string irDetail2Id = transferItems.Where(x => string.Compare(x.st_item_id, transferItem.st_item_id) == 0).Select(x => x.st_detail_id).FirstOrDefault();
                    if (irDetail2Id != null)
                    {
                        tb_transfer_frommain_stock_detail irDetail2 = db.tb_transfer_frommain_stock_detail.Find(irDetail2Id);
                        if (isRollback)
                        {
                            irDetail2.remain_quantity = irDetail2.remain_quantity + Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.unit, Convert.ToDecimal(transferItem.quantity), irDetail2.unit);
                        }
                        else
                        {
                            irDetail2.remain_quantity = irDetail2.remain_quantity - Class.CommonClass.ConvertMultipleUnit(transferItem.st_item_id, transferItem.unit, Convert.ToDecimal(transferItem.quantity), irDetail2.unit);
                        }
                        db.SaveChanges();
                    }
                }
                TransferFromMainStock.UpdateCompletedTransferWorkshop(transferId);
            }
        }
    }
    public class WorkOrderIssue
    {
        public static List<StockIssueHistoryViewModel> GetWorkOrderIssueItembyItemId(string issue_id,string item_id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from his in db.tb_history_issue_qty
                        join inv_d in db.tb_inventory_detail on his.inventory_detail_id equals inv_d.inventory_detail_id
                        where string.Compare(inv_d.inventory_ref_id, issue_id) == 0 && string.Compare(inv_d.inventory_item_id, item_id) == 0 && his.status == true
                        select new StockIssueHistoryViewModel()
                        {
                            history_issue_qty_id=his.history_issue_qty_id,
                            inventory_detail_id=his.inventory_detail_id,
                            issue_qty=his.issue_qty
                        }).ToList();
            }
        }
    }
    
}