using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Class
{
    public class ClsItemReceive
    {
        public static void RollBackPurchaseOrder(string recieveId,string orderId,bool isRollback,string status=null)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                List<Models.PurchaseOrderDetailViewModel> purchaseOrderItems = new List<Models.PurchaseOrderDetailViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();
                purchaseOrderItems = db.tb_purchase_order_detail
                    .Where(x => string.Compare(x.purchase_order_id, orderId) == 0 && (string.Compare(x.item_status, "approved") == 0 || string.Compare(x.item_status, "Pending") == 0))
                    .Select(x => new Models.PurchaseOrderDetailViewModel() { po_detail_id = x.po_detail_id, item_id = x.item_id }).ToList();
                if (string.IsNullOrEmpty(status))
                {
                    receivedItems = db.tb_received_item_detail
                    .Where(x => string.Compare(x.ri_ref_id, recieveId) == 0)
                    .Select(x => new Models.ItemReceivedDetailViewModel() { ri_item_id = x.ri_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                }
                else
                {
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, recieveId) == 0 && string.Compare(x.item_status, status) == 0)
                        .Select(x => new Models.ItemReceivedDetailViewModel() { ri_item_id = x.ri_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                }

                foreach(Models.ItemReceivedDetailViewModel receiveItem in receivedItems)
                {
                    string orderDetailId = purchaseOrderItems.Where(x => string.Compare(receiveItem.ri_item_id, x.item_id) == 0).Select(x => x.po_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(orderDetailId))
                    {
                        Entities.tb_purchase_order_detail purchaseOrderDetail = db.tb_purchase_order_detail.Find(orderDetailId);
                        if (isRollback)
                            purchaseOrderDetail.remain_quantity = purchaseOrderDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(receiveItem.ri_item_id, receiveItem.unit,Convert.ToDecimal(receiveItem.quantity));
                        else
                            purchaseOrderDetail.remain_quantity = purchaseOrderDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(receiveItem.ri_item_id, receiveItem.unit, Convert.ToDecimal(receiveItem.quantity));
                        db.SaveChanges();
                    }
                }
                Class.ClsItemReceive.UpdateCompletedPurchaseOrder(orderId);
            }
        }
        public static void UpdateCompletedPurchaseOrder(string id)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                var countNotCompleteItem = 0;
                var items= db.tb_purchase_order_detail
                    .Where(x => string.Compare(x.purchase_order_id, id) == 0 && (string.Compare(x.item_status, "approved") == 0 || string.Compare(x.item_status, "Pending") == 0))
                    .Select(x => new { x }).ToList();
                foreach(var item in items)
                {
                    if (item.x.remain_quantity > 0) countNotCompleteItem++;
                }
                Entities.tb_purchase_order dPo = db.tb_purchase_order.Find(id);
                dPo.is_completed = countNotCompleteItem == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static void RollbackStockTransferbyItemReceived(string receiveId,string transferId,bool isRollback,string itemStatus = null)
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                List<Models.StockTransferItemViewModel> transferItems = new List<Models.StockTransferItemViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();
                transferItems = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, transferId) == 0).Select(x => new Models.StockTransferItemViewModel()
                {
                    st_detail_id=x.st_detail_id,
                    st_item_id=x.st_item_id
                }).ToList();
                if (string.IsNullOrEmpty(itemStatus))
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_item_id = x.ri_item_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                else
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_ref_id=x.ri_ref_id,
                        quantity=x.quantity,
                        unit=x.unit
                    }).ToList();
                foreach(Models.ItemReceivedDetailViewModel item in receivedItems)
                {
                    string transferDetailId = transferItems.Where(x => string.Compare(x.st_item_id, item.ri_item_id) == 0).Select(x => x.st_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(transferDetailId))
                    {
                        Entities.tb_stock_transfer_detail transferDetail = db.tb_stock_transfer_detail.Find(transferDetailId);
                        if (isRollback)
                            transferDetail.remain_quantity = transferDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        else
                            transferDetail.remain_quantity = transferDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        db.SaveChanges();
                    }
                }
                Class.ClsItemReceive.UpdateCompletedStockTransfer(transferId);
            }
        }
        public static void RollbackWorkshopTransferbyItemReceived(string receiveId, string transferId, bool isRollback, string itemStatus = null)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<Models.StockTransferItemViewModel> transferItems = new List<Models.StockTransferItemViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();
                transferItems = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, transferId) == 0).Select(x => new Models.StockTransferItemViewModel()
                {
                    st_detail_id = x.st_detail_id,
                    st_item_id = x.st_item_id
                }).ToList();
                if (string.IsNullOrEmpty(itemStatus))
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_item_id = x.ri_item_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                else
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_ref_id = x.ri_ref_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                foreach (Models.ItemReceivedDetailViewModel item in receivedItems)
                {
                    string transferDetailId = transferItems.Where(x => string.Compare(x.st_item_id, item.ri_item_id) == 0).Select(x => x.st_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(transferDetailId))
                    {
                        Entities.tb_transfer_frommain_stock_detail transferDetail = db.tb_transfer_frommain_stock_detail.Find(transferDetailId);
                        if (isRollback)
                            transferDetail.received_remain_quantity = transferDetail.received_remain_quantity + Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        else
                            transferDetail.received_remain_quantity = transferDetail.received_remain_quantity - Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        db.SaveChanges();
                    }
                }
                Class.ClsItemReceive.UpdateCompletedWorkshopTransfer(transferId);
            }
        }
        public static void RollbackStockReturnbyItemReceived(string receiveId, string returnId, bool isRollback, string itemStatus = null)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<Models.StockTransferItemViewModel> returnItems = new List<Models.StockTransferItemViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();
                returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).Select(x => new Models.StockTransferItemViewModel()
                {
                    st_detail_id = x.inventory_detail_id,
                    st_item_id = x.inventory_item_id
                }).ToList();
                if (string.IsNullOrEmpty(itemStatus))
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_item_id = x.ri_item_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                else
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_ref_id = x.ri_ref_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                foreach (Models.ItemReceivedDetailViewModel item in receivedItems)
                {
                    string returnDetailId = returnItems.Where(x => string.Compare(x.st_item_id, item.ri_item_id) == 0).Select(x => x.st_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(returnDetailId))
                    {
                        Entities.tb_inventory_detail transferDetail = db.tb_inventory_detail.Find(returnDetailId);
                        if (isRollback)
                            transferDetail.remain_quantity = transferDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        else
                            transferDetail.remain_quantity = transferDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        db.SaveChanges();
                    }
                }
                Class.ClsItemReceive.UpdateCompletedStockReturn(returnId);
            }
        }
        public static void RollbackReturnWorkshopbyItemReceived(string receiveId, string returnId, bool isRollback, string itemStatus = null)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<Models.StockTransferItemViewModel> returnItems = new List<Models.StockTransferItemViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();
                returnItems = db.tb_return_mainstock_detail.Where(x => string.Compare(x.main_stock_detail_ref, returnId) == 0).Select(x => new Models.StockTransferItemViewModel()
                {
                    st_detail_id = x.main_stock_detail_id,
                    st_item_id = x.main_stock_detail_item
                }).ToList();
                if (string.IsNullOrEmpty(itemStatus))
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_item_id = x.ri_item_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                else
                    receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receiveId) == 0).Select(x => new Models.ItemReceivedDetailViewModel()
                    {
                        ri_ref_id = x.ri_ref_id,
                        quantity = x.quantity,
                        unit = x.unit
                    }).ToList();
                foreach (Models.ItemReceivedDetailViewModel item in receivedItems)
                {
                    string returnDetailId = returnItems.Where(x => string.Compare(x.st_item_id, item.ri_item_id) == 0).Select(x => x.st_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(returnDetailId))
                    {
                        Entities.tb_return_mainstock_detail transferDetail = db.tb_return_mainstock_detail.Find(returnDetailId);
                        if (isRollback)
                            transferDetail.remain_quantity = transferDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        else
                            transferDetail.remain_quantity = transferDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(item.ri_item_id, item.unit, Convert.ToDecimal(item.quantity));
                        db.SaveChanges();
                    }
                }
                Class.ClsItemReceive.UpdateCompletedReturnWorkshop(returnId);
            }
        }
        public static void UpdateCompletedStockTransfer(string id)
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                int countNotCompletedItem = 0;
                var items = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, id) == 0).Select(x => new { x }).ToList();
                foreach (var item in items)
                    if (item.x.remain_quantity > 0) countNotCompletedItem++;
                Entities.tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(id);
                stockTransfer.is_completed = countNotCompletedItem == 0 ? true : false;
                db.SaveChanges();
                

            }
        }
        public static void UpdateCompletedWorkshopTransfer(string id)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                int countNotCompletedItem = 0;
                var items = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, id) == 0).Select(x => new { x }).ToList();
                foreach (var item in items)
                    if (item.x.received_remain_quantity > 0) countNotCompletedItem++;
                Entities.transferformmainstock stockTransfer = db.transferformmainstocks.Find(id);
                stockTransfer.is_receive_completed = countNotCompletedItem == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static List<Models.GoodReceivedNoteStatusViewModel> GetReceivedNoteStatusbyRefId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_received_status.OrderBy(s => s.created_date).Where(s=>string.Compare(s.received_ref_id,id)==0).Select(s => new Models.GoodReceivedNoteStatusViewModel()
                {
                    received_status_id=s.received_status_id,
                    received_id=s.received_id,
                    received_ref_id=s.received_ref_id,
                    status=s.status
                }).ToList();
            }
        }
        public static void UpdateCompletedStockReturn(string id)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                int countNotCompletedItem = 0;
                var items = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).Select(x => new { x }).ToList();
                foreach (var item in items)
                    if (item.x.remain_quantity > 0) countNotCompletedItem++;
                Entities.tb_stock_issue_return stockTransfer = db.tb_stock_issue_return.Find(id);
                stockTransfer.is_receive_completed = countNotCompletedItem == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static void UpdateCompletedReturnWorkshop(string id)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                int countNotCompletedItem = 0;
                var items = db.tb_return_mainstock_detail.Where(x => string.Compare(x.main_stock_detail_ref, id) == 0).Select(x => new { x }).ToList();
                foreach (var item in items)
                    if (item.x.remain_quantity > 0) countNotCompletedItem++;
                Entities.tb_return_main_stock stockTransfer = db.tb_return_main_stock.Find(id);
                stockTransfer.is_receive_completed = countNotCompletedItem == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static Models.ItemReceiveViewModel GetItemReceivedDetail(string id)
        {
            Models.ItemReceiveViewModel itemReceived = new Models.ItemReceiveViewModel();
            List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                itemReceived = db.tb_receive_item_voucher.Where(x => string.Compare(x.receive_item_voucher_id, id) == 0).Select(x => new Models.ItemReceiveViewModel() { receive_item_voucher_id=x.receive_item_voucher_id,received_number=x.received_number,received_type=x.received_type,ref_id=x.ref_id,created_date=x.created_date,supplier_id=x.supplier_id,po_report_number=x.po_report_number}).FirstOrDefault();
                if (string.Compare(itemReceived.received_type, "Stock Transfer") == 0)
                {
                    itemReceived.ref_number = db.tb_stock_transfer_voucher.Where(x => string.Compare(x.stock_transfer_id, itemReceived.ref_id) == 0).Select(x => x.stock_transfer_no).FirstOrDefault().ToString();
                    itemReceived.inventories = Inventory.GetStockTransferItems(itemReceived.ref_id);
                    itemReceived.mr_ref_number = (from mr in db.tb_item_request

                                                  join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                  where string.Compare(st.stock_transfer_id, itemReceived.ref_id) == 0
                                                  select mr.ir_no).FirstOrDefault().ToString();
                    itemReceived.project_full_name= (from pro in db.tb_project
                                                     join mr in db.tb_item_request on pro.project_id equals mr.ir_project_id
                                                     join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                     where string.Compare(st.stock_transfer_id, itemReceived.ref_id) == 0
                                                     select pro.project_full_name).FirstOrDefault().ToString();
                }
                else if (string.Compare(itemReceived.received_type, "Purchase Order") == 0)
                {
                    //itemReceived.ref_number = db.tb_purchase_order.Where(x => string.Compare(x.purchase_order_id, itemReceived.ref_id) == 0).Select(x => x.purchase_oder_number).FirstOrDefault().ToString();
                    itemReceived.ref_number = itemReceived.po_report_number;
                    itemReceived.mr_ref_number = (from mr in db.tb_item_request
                                                  join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                  join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                  join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                  where string.Compare(po.pruchase_request_id, itemReceived.ref_id) == 0
                                                  select mr.ir_no).FirstOrDefault();
                    itemReceived.inventories = Inventory.ConvertFromPOtoInventory(itemReceived.ref_id, id,itemReceived.po_report_number,itemReceived.supplier_id);
                    itemReceived.project_full_name = (from proj in db.tb_project
                                                  join mr in db.tb_item_request on proj.project_id equals mr.ir_project_id
                                                  join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                  join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                  join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                  where string.Compare(po.pruchase_request_id, itemReceived.ref_id) == 0
                                                  select proj.project_full_name).FirstOrDefault();
                }
                else if (string.Compare(itemReceived.received_type, "Transfer Workshop") == 0)
                {
                    var transferRef = (from transfer in db.transferformmainstocks
                                       join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                       join project in db.tb_project on mr.ir_project_id equals project.project_id
                                       join site in db.tb_site on project.site_id equals site.site_id
                                       join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                       where string.Compare(transfer.stock_transfer_id, itemReceived.ref_id) == 0
                                       select new
                                       {
                                           transfer,
                                           project,
                                           mr,
                                           wh
                                       }).FirstOrDefault();
                    itemReceived.ref_number = transferRef.transfer.stock_transfer_no;
                    itemReceived.mr_ref_number = transferRef.mr.ir_no;
                    itemReceived.project_full_name = transferRef.project.project_full_name;
                    itemReceived.inventories = Inventory.GetTransferfromWorkshopItems(itemReceived.ref_id);

                }
                else if(string.Compare(itemReceived.received_type,"Stock Return") == 0)
                {
                    var returnRef = (from sreturn in db.tb_stock_issue_return
                                     join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                                     join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                     join project in db.tb_project on mr.ir_project_id equals project.project_id
                                     join site in db.tb_site on project.site_id equals site.site_id
                                     join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                     where string.Compare(sreturn.stock_issue_return_id, itemReceived.ref_id) == 0
                                     select new
                                     {
                                         sreturn,
                                         transfer,
                                         mr,
                                         project,
                                         wh
                                     }).FirstOrDefault();
                    itemReceived.ref_number = returnRef.sreturn.issue_return_number;
                    itemReceived.mr_ref_number = returnRef.mr.ir_no;
                    itemReceived.project_full_name = returnRef.project.project_full_name;
                    itemReceived.inventories = Inventory.GetStockReturnItems(itemReceived.ref_id);
                }
                else if (string.Compare(itemReceived.received_type, "Return Workshop") == 0)
                {
                    var returnRef = (
                        from wreturn in db.tb_return_main_stock
                        join transfer in db.transferformmainstocks on wreturn.return_ref_id equals transfer.stock_transfer_id
                        join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                        join project in db.tb_project on mr.ir_project_id equals project.project_id
                        join site in db.tb_site on project.site_id equals site.site_id
                        join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                        where string.Compare(wreturn.return_main_stock_id, itemReceived.ref_id) == 0
                        select new
                        {
                            wreturn,
                            transfer,
                            project,
                            mr,
                            wh
                        }).FirstOrDefault();
                    itemReceived.ref_number = returnRef.wreturn.return_main_stock_no;
                    itemReceived.mr_ref_number = returnRef.mr.ir_no;
                    itemReceived.project_full_name = returnRef.project.project_full_name;
                    itemReceived.inventories = Inventory.GetReturnWorkshopItems(itemReceived.ref_id);
                }

                var receivedItems = (from rItem in db.tb_received_item_detail
                                     join item in db.tb_product on rItem.ri_item_id equals item.product_id
                                     join warehouse in db.tb_warehouse on rItem.ri_warehouse_id equals warehouse.warehouse_id
                                     orderby rItem.ordering_number
                                     where string.Compare(rItem.ri_ref_id, itemReceived.receive_item_voucher_id) == 0
                                     select new { rItem, item, warehouse }).ToList();
                foreach(var receivedItem in receivedItems)
                {
                    Models.InventoryViewModel item = new Models.InventoryViewModel();
                    item.inventory_id = receivedItem.rItem.ri_detail_id;
                    item.product_id = receivedItem.rItem.ri_item_id;
                    item.itemCode = receivedItem.item.product_code;
                    item.itemName = receivedItem.item.product_name;
                    item.itemUnit = receivedItem.item.product_unit;
                    item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                    item.warehouse_id = receivedItem.rItem.ri_warehouse_id;
                    item.warehouseName = receivedItem.warehouse.warehouse_name;
                    item.total_quantity = receivedItem.rItem.quantity;
                    item.unit = receivedItem.rItem.unit;
                    item.unitName = db.tb_unit.Find(item.unit).Name;
                    item.invoice_date = receivedItem.rItem.invoice_date;
                    item.invoice_number = receivedItem.rItem.invoice_number;
                    item.item_status = receivedItem.rItem.item_status;
                    item.supplier_id = receivedItem.rItem.supplier_id;
                    item.remark = receivedItem.rItem.remark;
                    item.completed = receivedItem.rItem.completed;
                    if (string.Compare(itemReceived.received_type, "Stock Transfer") == 0)
                        item.supplier_name = db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, item.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString();
                    else if (string.Compare(itemReceived.received_type, "Purchase Order") == 0)
                        item.supplier_name = db.tb_supplier.Where(x => string.Compare(x.supplier_id, item.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString();
                    else if (string.Compare(itemReceived.received_type, "Transfer Workshop") == 0 || string.Compare(itemReceived.received_type,"Return Workshop")==0)
                        item.supplier_name = "Workshop";
                    items.Add(item);
                }
                itemReceived.receivedItems = items;
                itemReceived.receivedHistories = Inventory.GetReceivedHistory(itemReceived.ref_id, itemReceived.receive_item_voucher_id);
            }
            return itemReceived;
        }
        public static void UpdatePurchaseOrderReceivedComplete(string poId,string itemId,string supplierId,string poReportNumber)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                Entities.tb_purchase_order_detail purchaseorderDetail = db.tb_purchase_order_detail.Where(x => string.Compare(x.purchase_order_id, poId) == 0 && string.Compare(x.item_id, itemId) == 0).FirstOrDefault();
                purchaseorderDetail.remain_quantity = 0;
                db.SaveChanges();
                //UpdateCompletedPurchaseOrder(poId);
                UpdateCompltedPurchaseOrderReport(poId, poReportNumber, supplierId);
            }
        }
        public static void UpdateStockTransferReceivedComplete(string transferId,string itemId)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                Entities.tb_stock_transfer_detail transferDetail = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, transferId) == 0 && string.Compare(x.st_item_id, itemId) == 0).FirstOrDefault();
                transferDetail.remain_quantity = 0;
                db.SaveChanges();
                UpdateCompletedStockTransfer(transferId);
            }
        }
        public static void UpdateTransferWorkshopReceivedComplete(string transferId, string itemId)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                Entities.tb_transfer_frommain_stock_detail transferDetail = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, transferId) == 0 && string.Compare(x.st_item_id, itemId) == 0).FirstOrDefault();
                transferDetail.received_remain_quantity = 0;
                db.SaveChanges();
                UpdateCompletedWorkshopTransfer(transferId);
            }
        }
        public static void UpdateStockReturnReceivedComplete(string transferId, string itemId)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                Entities.tb_inventory_detail transferDetail = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, transferId) == 0 && string.Compare(x.inventory_item_id, itemId) == 0).FirstOrDefault();
                transferDetail.remain_quantity = 0;
                db.SaveChanges();
                UpdateCompletedStockReturn(transferId);
            }
        }
        public static List<Models.PurchaseOrderDetailViewModel> GetReceivedRemainItembyPurchaseOrder(string poId, string supplierId,string purchaseOrderNumber, string receivedId = null)
        {
            List<Models.PurchaseOrderDetailViewModel> inventories = new List<Models.PurchaseOrderDetailViewModel>();
            try
            {
                using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
                {
                    bool isVAT = isVATPurchaseOrder(poId, purchaseOrderNumber, supplierId);
                    List<Models.ItemReceivedDetailViewModel> itemReceived = new List<Models.ItemReceivedDetailViewModel>();
                    string quoteId = db.tb_purchase_request.Find(poId).purchase_order_id;
                    if (string.IsNullOrEmpty(receivedId))
                    {
                        
                        var purchaseOrderDetails = (from dPo in db.tb_purchase_order_detail
                                                    join poSup in db.tb_po_supplier on dPo.po_detail_id equals poSup.po_detail_id
                                                    join product in db.tb_product on dPo.item_id equals product.product_id
                                                    orderby dPo.ordering_number
                                                    where string.Compare(dPo.purchase_order_id, quoteId) == 0 && dPo.remain_quantity > 0 && string.Compare(poSup.supplier_id, supplierId) == 0 && poSup.is_selected == true && dPo.item_vat == isVAT
                                                    select new { dPo, product }).ToList();
                        foreach (var po in purchaseOrderDetails)
                        {
                            Models.PurchaseOrderDetailViewModel inv = new Models.PurchaseOrderDetailViewModel();

                            inv.po_detail_id = po.dPo.po_detail_id;
                            inv.purchase_order_id = po.dPo.purchase_order_id;
                            inv.item_id = po.dPo.item_id;
                            inv.product_name = po.product.product_name;
                            inv.product_code = po.product.product_code;
                            inv.product_unit = po.product.product_unit;
                            inv.product_unit_name = db.tb_unit.Find(po.product.product_unit).Name;
                            //add 
                            inv.unit = po.dPo.po_unit;
                            inv.unit_name = db.tb_unit.Find(inv.unit).Name;
                            //  inv.PO_Unit = po.dPo.po_unit;
                            inv.item_unit = po.dPo.po_unit;
                            inv.item_unit_name = db.tb_unit.Find(inv.item_unit).Name;
                            inv.unitLevel = Class.CommonClass.GetItemUnitLevel(inv.item_id, inv.item_unit);
                            inv.po_quantity = po.dPo.po_quantity;
                            inv.quantity = po.dPo.remain_quantity;
                            inv.remain_quantity = po.dPo.remain_quantity;
                            inv.ordering_number = po.dPo.ordering_number;
                            inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new Models.ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            inv.supplier_id = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_id).FirstOrDefault().ToString();
                            inv.supplier_name = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_name).FirstOrDefault().ToString();
                            
                            inventories.Add(inv);
                        }
                    }
                    else
                    {
                        var purchaseOrderDetails = (from dPo in db.tb_purchase_order_detail
                                                    join poSup in db.tb_po_supplier on dPo.po_detail_id equals poSup.po_detail_id
                                                    join product in db.tb_product on dPo.item_id equals product.product_id
                                                    orderby dPo.ordering_number
                                                    where string.Compare(dPo.purchase_order_id, quoteId) == 0 && string.Compare(poSup.supplier_id, supplierId) == 0 && poSup.is_selected == true && dPo.item_vat == isVAT
                                                    select new { dPo, product }).ToList();
                        itemReceived = (from rItem in db.tb_received_item_detail
                                        where string.Compare(rItem.ri_ref_id, receivedId) == 0
                                        select new Models.ItemReceivedDetailViewModel() { ri_item_id = rItem.ri_item_id, quantity = rItem.quantity, unit = rItem.unit }).ToList();
                        foreach (var po in purchaseOrderDetails)
                        {
                            decimal receivedQuantity = 0;
                            var isReceived = itemReceived.Where(m => string.Compare(m.ri_item_id, po.dPo.item_id) == 0).FirstOrDefault();
                            if (isReceived != null)
                            {
                                receivedQuantity = Class.CommonClass.ConvertMultipleUnit(isReceived.ri_item_id, isReceived.unit, Convert.ToDecimal(isReceived.quantity));
                            }
                            Models.PurchaseOrderDetailViewModel inv = new Models.PurchaseOrderDetailViewModel();
                            inv.po_detail_id = po.dPo.po_detail_id;
                            inv.purchase_order_id = po.dPo.purchase_order_id;
                            inv.item_id = po.dPo.item_id;
                            inv.product_name = po.product.product_name;
                            inv.ordering_number = po.dPo.ordering_number;
                            inv.product_code = po.product.product_code;
                            inv.product_unit = po.product.product_unit;
                            inv.product_unit_name = db.tb_unit.Find(po.product.product_unit).Name;
                            //add 
                            inv.unit = po.dPo.po_unit;
                            inv.unit_name = db.tb_unit.Find(inv.unit).Name;
                            //  inv.PO_Unit = po.dPo.po_unit;
                            inv.item_unit = po.dPo.po_unit;
                            inv.item_unit_name = db.tb_unit.Find(inv.item_unit).Name;
                            inv.po_quantity = po.dPo.po_quantity;
                            inv.unitLevel = Class.CommonClass.GetItemUnitLevel(inv.item_id, inv.item_unit);
                            inv.quantity = po.dPo.remain_quantity+receivedQuantity;
                            inv.remain_quantity = po.dPo.remain_quantity;
                            inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new Models.ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            inv.supplier_id = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_id).FirstOrDefault().ToString();
                            inv.supplier_name = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_name).FirstOrDefault().ToString();
                            inventories.Add(inv);
                        }
                    }
                    //inventories = inventories.ToList();
                    inventories = inventories.OrderBy(x => x.ordering_number).ToList();
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "GetReceivedRemainItembyPurchaseOrder", ex.StackTrace, ex.Message);
            }
            finally { }
            return inventories;
        }
        public static bool isVATPurchaseOrder(string purchaseOrderId,string purchaseOrderNumber,string supplierId)
        {
            bool isVAT = false;
            try
            {
                using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
                {
                    string quoteID = db.tb_purchase_request.Find(purchaseOrderId).purchase_order_id;
                    isVAT =Convert.ToBoolean(db.tb_po_report.Where(w => string.Compare(w.po_ref_id, quoteID) == 0 && string.Compare(w.po_supplier_id, supplierId) == 0 && string.Compare(w.po_report_number, purchaseOrderNumber) == 0).Select(s => s.vat_status).FirstOrDefault());
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "isVATPurchaseOrder", ex.StackTrace, ex.Message);
            }
            finally { }
            return isVAT;
        }
        public static bool isVATPurchaseOrder(string poReportNumber)
        {
            bool isVAT = false;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_report po_Report = db.tb_po_report.Where(s => string.Compare(s.po_report_number, poReportNumber) == 0).FirstOrDefault();
                isVAT = Convert.ToBoolean(po_Report.vat_status);
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "isVATPurchaseOrder", ex.StackTrace, ex.Message);
            }
            return isVAT;
        }
        public static void TrackPurchaseOrderItembyItemReceived(string receivedId,string pooRefId, string poReportNumber,string supplierId,bool isRollback,string status = null)
        {
            try
            {
                using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
                {
                    List<Models.PurchaseOrderDetailViewModel> purchaseOrderItems = new List<Models.PurchaseOrderDetailViewModel>();
                    List<Models.ItemReceivedDetailViewModel> receivedItems = new List<Models.ItemReceivedDetailViewModel>();

                    string poRefId = db.tb_purchase_request.Find(pooRefId).purchase_order_id;
                    bool isVAT = isVATPurchaseOrder(poReportNumber);

                    purchaseOrderItems = (from dPo in db.tb_purchase_order_detail
                                          join poSup in db.tb_po_supplier on dPo.po_detail_id equals poSup.po_detail_id
                                          where string.Compare(dPo.purchase_order_id, poRefId) == 0 && string.Compare(poSup.supplier_id, supplierId) == 0 && poSup.is_selected == true && dPo.item_vat == isVAT
                                          select new Models.PurchaseOrderDetailViewModel() { po_detail_id= dPo.po_detail_id, item_id=dPo.item_id }).ToList();
                    if (string.IsNullOrEmpty(status))
                    {
                        receivedItems = db.tb_received_item_detail
                        .Where(x => string.Compare(x.ri_ref_id, receivedId) == 0)
                        .Select(x => new Models.ItemReceivedDetailViewModel() { ri_item_id = x.ri_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                    }
                    else
                    {
                        receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receivedId) == 0 && string.Compare(x.item_status, status) == 0)
                            .Select(x => new Models.ItemReceivedDetailViewModel() { ri_item_id = x.ri_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                    }

                    foreach (Models.ItemReceivedDetailViewModel receiveItem in receivedItems)
                    {
                        string orderDetailId = purchaseOrderItems.Where(x => string.Compare(receiveItem.ri_item_id, x.item_id) == 0).Select(x => x.po_detail_id).FirstOrDefault().ToString();
                        if (!string.IsNullOrEmpty(orderDetailId))
                        {
                            Entities.tb_purchase_order_detail purchaseOrderDetail = db.tb_purchase_order_detail.Find(orderDetailId);
                            if (isRollback)
                                purchaseOrderDetail.remain_quantity = purchaseOrderDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(receiveItem.ri_item_id, receiveItem.unit, Convert.ToDecimal(receiveItem.quantity));
                            else
                                purchaseOrderDetail.remain_quantity = purchaseOrderDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(receiveItem.ri_item_id, receiveItem.unit, Convert.ToDecimal(receiveItem.quantity));
                            db.SaveChanges();
                        }
                    }

                    Class.ClsItemReceive.UpdateCompltedPurchaseOrderReport(poRefId, poReportNumber, supplierId);

                }

            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "TrackPurchaseOrderItembyItemReceived", ex.StackTrace, ex.Message);
            }
            finally { }
        }

        //public static void UpdateCompltedPurchaseOrderReport(string purchasesOrderId, string poReportNumber,string supplierId)
        //{
        //    try
        //    {
        //        using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
        //        {
        //            int countNotCompletedItem = 0;
        //            bool isVAT = isVATPurchaseOrder(purchasesOrderId, poReportNumber, supplierId);
        //            var items = (from dPo in db.tb_purchase_order_detail
        //                                  join poSup in db.tb_po_supplier on dPo.po_detail_id equals poSup.po_detail_id
        //                                  where string.Compare(dPo.purchase_order_id, purchasesOrderId) == 0 && string.Compare(poSup.supplier_id, supplierId) == 0 && poSup.is_selected == true && dPo.item_vat == isVAT
        //                                  select new { dPo}).ToList();
        //            foreach (var item in items)
        //            {
        //                if (item.dPo.remain_quantity > 0) countNotCompletedItem++;
        //            }
        //            Entities.tb_po_report dPO = db.tb_po_report.Where(w => string.Compare(w.po_ref_id, purchasesOrderId) == 0 && string.Compare(w.po_supplier_id, supplierId) == 0 && string.Compare(w.po_report_number, poReportNumber) == 0).FirstOrDefault();
        //            dPO.is_completed = countNotCompletedItem == 0 ? true : false;
        //            db.SaveChanges();
        //        }
        //    }catch(Exception ex)
        //    {
        //        ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "UpdateCompltedPurchaseOrderReport", ex.StackTrace, ex.Message);
        //    }
        //    finally { }
        //}

        public static bool UpdateCompltedPurchaseOrderReport(string purchasesOrderId, string poReportNumber, string supplierId)
        {
            try
            {
                using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    int countNotCompletedItem = 0;
                    bool isVAT = isVATPurchaseOrder(poReportNumber);
                    var items = (from dPo in db.tb_purchase_order_detail
                                 join poSup in db.tb_po_supplier on dPo.po_detail_id equals poSup.po_detail_id
                                 where string.Compare(dPo.purchase_order_id, purchasesOrderId) == 0 && string.Compare(poSup.supplier_id, supplierId) == 0 && poSup.is_selected == true && dPo.item_vat == isVAT
                                 select new { dPo }).ToList();
                    foreach (var item in items)
                    {
                        if (item.dPo.remain_quantity > 0) countNotCompletedItem++;
                    }
                    Entities.tb_po_report dPO = db.tb_po_report.Where(w => string.Compare(w.po_ref_id, purchasesOrderId) == 0 && string.Compare(w.po_supplier_id, supplierId) == 0 && string.Compare(w.po_report_number, poReportNumber) == 0).FirstOrDefault();
                    dPO.is_completed = countNotCompletedItem == 0 ? true : false;
                    db.SaveChanges();
                    return dPO.is_completed;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.submitLogEntry(EnumConstants.ErrorType.Error, "ClsItemReceive.cs", "UpdateCompltedPurchaseOrderReport", ex.StackTrace, ex.Message);
            }
            finally { }
            return false;
        }

        public static void UpdateGoodReceivedNoteReferenceStatus(string receivedId,string refId,string status)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                tb_received_status receivedStatus = new tb_received_status();
                receivedStatus.received_status_id = Guid.NewGuid().ToString();
                receivedStatus.received_id = receivedId;
                receivedStatus.received_ref_id = refId;
                receivedStatus.status = status;
                receivedStatus.created_date = CommonClass.ToLocalTime(DateTime.Now);
                db.tb_received_status.Add(receivedStatus);
                db.SaveChanges();
            }
        }
        public static void UpdateStockTransferStatusbyStockReturn(string returnId,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue_return stockReturn = db.tb_stock_issue_return.Find(returnId);
                if (stockReturn != null)
                {
                    string transferId = stockReturn.stock_issue_ref;
                    tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(transferId);
                    stockTransfer.sr_status = status;
                    db.SaveChanges();
                    stockReturn.received_status = status;
                    db.SaveChanges();
                }
            }
        }
        public static void UpdateMaterialRequestStatusbyPurchaseOrderId(string poId,string status)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string mrId = (from mr in db.tb_item_request
                               join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                               join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                               join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                               where string.Compare(po.pruchase_request_id,poId) == 0
                               select mr.ir_id).FirstOrDefault().ToString();
                tb_item_request materialRequest = db.tb_item_request.Find(mrId);
                materialRequest.po_status = status;
                db.SaveChanges();
            }
        }
    }
}