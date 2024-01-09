using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using System.IO;
using System.Net;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;
using WECPOFLogic;
using System.Drawing;
using System.Web.Services.Description;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace BT_KimMex.Controllers
{
   [Authorize]
    public class ItemReceiveController : Controller
    {
        // GET: ItemReceive
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create( string id)
        {
             using(kim_mexEntities db = new kim_mexEntities())
            {
              //  var type = this.GetTypeList();
            }

            //ViewBag.IReID = this.GetItemReceiveNumber();
            //ViewBag.IReID = Class.CommonClass.GenerateProcessNumber("IRe");
            return View();
        }
        public ActionResult Print(string id)
        {
            return Json(new { result = "success", id = id }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GRNReport()
        {
            return View();
        }
        public ActionResult CreateItemReceived(ItemReceiveViewModel model,List<InventoryViewModel> inventories,List<string> Attachment, List<string> DOAttachment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                int countInvalid = 0;
                string message = string.Empty;
                
                if (string.Compare(model.received_type, "Stock Transfer") == 0)
                {
                    List<InventoryViewModel> remainQuantities = new List<InventoryViewModel>();
                    remainQuantities = this.GetRemainByStockTransfer(model.ref_id,model.receive_item_voucher_id);
                    foreach (InventoryViewModel item in remainQuantities)
                    {
                        foreach (InventoryViewModel receive in inventories)
                        {
                            if (item.product_id == receive.product_id && item.total_quantity < CommonClass.ConvertMultipleUnit(receive.product_id, receive.unit, Convert.ToDecimal(receive.in_quantity)))
                            {
                                countInvalid++;
                                if (item.total_quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.itemName, string.Format("{0:G29}", Decimal.Parse(item.total_quantity.ToString())));
                            }
                        }
                    }
                }
                //Change quality unit

                else if (string.Compare(model.received_type, "Purchase Order") ==0)
                {
                    List<PurchaseOrderDetailViewModel> remainQuantities = new List<PurchaseOrderDetailViewModel>();
                    //remainQuantities = this.GetRemainItemByPurchaseOrder(model.ref_id);
                    remainQuantities = Class.ClsItemReceive.GetReceivedRemainItembyPurchaseOrder(model.ref_id, model.supplier_id, model.po_report_number);
                    foreach(PurchaseOrderDetailViewModel item in remainQuantities)
                    {
                        foreach (InventoryViewModel receive in inventories)
                        {
                            if (item.item_id == receive.product_id && item.quantity < CommonClass.ConvertMultipleUnit(receive.product_id, receive.unit, Convert.ToDecimal(receive.in_quantity)))
                                
                                {
                                countInvalid++;
                                if (item.quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.product_name, string.Format("{0:G29}", Decimal.Parse(item.quantity.ToString())));
                            }
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);

                tb_receive_item_voucher itemReceive = new tb_receive_item_voucher();
                itemReceive.receive_item_voucher_id = Guid.NewGuid().ToString();
                itemReceive.received_number = Class.CommonClass.GenerateProcessNumber("IRe");
                itemReceive.received_type = model.received_type;
                itemReceive.ref_id = model.ref_id;
                itemReceive.received_status = Status.Pending;
                itemReceive.supplier_id = model.supplier_id;               
                itemReceive.po_report_number = model.po_report_number;
                itemReceive.status = true;
                itemReceive.created_by = User.Identity.GetUserId();
                itemReceive.created_date =CommonClass.ToLocalTime(DateTime.Now);
                itemReceive.sending_date = CommonClass.ToLocalTime(Convert.ToDateTime(model.sending_date));
                itemReceive.returning_date = CommonClass.ToLocalTime(Convert.ToDateTime(model.returning_date));
                db.tb_receive_item_voucher.Add(itemReceive);
                db.SaveChanges();
                Class.CommonClass.AutoGenerateStockInvoiceNumber(itemReceive.receive_item_voucher_id, inventories);

                foreach(var inv in inventories)
                {
                    /* update workflow
                    tb_inventory inventory = new tb_inventory();
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.product_id && m.warehouse_id == inv.warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.inventory_status_id = "7";
                    inventory.warehouse_id = inv.warehouse_id;
                    inventory.product_id = inv.product_id;
                    inventory.total_quantity = totalQty + inv.in_quantity;
                    inventory.in_quantity = inv.in_quantity;
                    inventory.out_quantity = 0;
                    inventory.ref_id = itemReceive.receive_item_voucher_id;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                    */


                    tb_received_item_detail temp = new tb_received_item_detail();
                    temp.ri_detail_id = Guid.NewGuid().ToString();
                    temp.ri_ref_id = itemReceive.receive_item_voucher_id;
                    temp.ri_item_id = inv.product_id;
                    temp.ri_warehouse_id = inv.warehouse_id;
                    temp.quantity = inv.in_quantity;
                    temp.unit = inv.unit;
                    temp.item_status = Status.Pending;
                    temp.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                    temp.invoice_number = CommonClass.GetInvoiceNumber(itemReceive.receive_item_voucher_id, temp.ri_warehouse_id, temp.invoice_date);
                    temp.supplier_id = inv.supplier_id;
                    db.tb_received_item_detail.Add(temp);
                    db.SaveChanges();
                }

                bool isReceivedPartial = false;
                string status = string.Empty;

                if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                {
                    //Class.ClsItemReceive.RollBackPurchaseOrder(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                    Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, itemReceive.po_report_number, itemReceive.supplier_id, false);
                    #region add on sep 07 2020
                    string quoteId = db.tb_purchase_request.Find(itemReceive.ref_id).purchase_order_id;
                    bool isPOCompleted = db.tb_po_report.Where(w => string.Compare(w.po_ref_id, quoteId) == 0 && string.Compare(w.po_supplier_id, itemReceive.supplier_id) == 0 && string.Compare(w.po_report_number, itemReceive.po_report_number) == 0).FirstOrDefault().is_completed;
                    if (isPOCompleted)
                    {
                        status = ShowStatus.GRNCreated;
                        isReceivedPartial = false;
                    }else
                    {
                        status = ShowStatus.GRNPartialCreated;
                        isReceivedPartial = true;
                    }
                    string mrId = (from mr in db.tb_item_request
                                   join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                   join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                   where string.Compare(quote.purchase_order_id, quoteId) == 0
                                   select mr.ir_id).FirstOrDefault().ToString();
                    tb_item_request materialRequest = db.tb_item_request.Find(mrId);
                    materialRequest.po_status = status;
                    db.SaveChanges();
                    #endregion
                }
                else if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                {
                    Class.ClsItemReceive.RollbackStockTransferbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                    tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(itemReceive.ref_id);
                    //update stock transfer and MR status
                    
                    if (stockTransfer.is_completed)
                    {
                        status = ShowStatus.GRNCreated;
                        isReceivedPartial = false;
                    }
                    else
                    {
                        isReceivedPartial = true;
                        status = ShowStatus.GRNPartialCreated;
                    }
                        
                    //CommonClass.UpdateMaterialRequestStatus(stockTransfer.item_request_id, status);
                    StockTransfer.UpdateStockTransferProcessStatusById(stockTransfer.stock_transfer_id, status);

                }         
                else if(string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                {
                    Class.ClsItemReceive.RollbackWorkshopTransferbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);

                    transferformmainstock transferMainStock = db.transferformmainstocks.Find(itemReceive.ref_id);
                    if(Convert.ToBoolean(transferMainStock.is_receive_completed))
                    {
                        isReceivedPartial = false;
                        status = ShowStatus.GRNCreated;
                    }else
                    {
                        isReceivedPartial = true;
                        status = ShowStatus.GRNPartialCreated;
                    }

                }
                else if(string.Compare(itemReceive.received_type,"Stock Return") == 0)
                {
                    Class.ClsItemReceive.RollbackStockReturnbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                    tb_stock_issue_return stockReturn = db.tb_stock_issue_return.Find(itemReceive.ref_id);
                    if (Convert.ToBoolean(stockReturn.is_receive_completed))
                    {
                        status = ShowStatus.GRNCreated;
                        isReceivedPartial = false;
                    }
                    else
                    {
                        isReceivedPartial = true;
                        status = ShowStatus.GRNPartialCreated;
                    }
                    
                    //ClsItemReceive.UpdateGoodReceivedNoteReferenceStatus(itemReceive.receive_item_voucher_id, itemReceive.ref_id, status);
                    ClsItemReceive.UpdateStockTransferStatusbyStockReturn(itemReceive.ref_id, status);
                }
                else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    Class.ClsItemReceive.RollbackReturnWorkshopbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);

                if (Attachment != null && Attachment.Count > 0)
                {
                    foreach (string att in Attachment)
                    {
                        tb_ire_attachment attachment = db.tb_ire_attachment.Where(m => m.ire_attachment_id == att).FirstOrDefault();
                        attachment.ire_id = itemReceive.receive_item_voucher_id;
                        db.SaveChanges();
                    }
                }

                if(DOAttachment !=null && DOAttachment.Count > 0)
                {
                    foreach(string att in DOAttachment)
                    {
                        tb_attachment attachment = db.tb_attachment.Find(att);
                        if (attachment != null)
                        {
                            attachment.attachment_ref_id = itemReceive.receive_item_voucher_id;
                            db.SaveChanges();

                            var file_path = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), attachment.attachment_id + attachment.attachment_extension);
                            Image image = Image.FromFile(file_path);
                            Graphics imageGraphics = Graphics.FromImage(image);

                            /* CREATED Section */
                            string checkedBySignature = CommonClass.GetUserSignature(itemReceive.created_by);
                            if (!string.IsNullOrEmpty(checkedBySignature))
                            {
                                var checked_signature_path = Server.MapPath("~" + checkedBySignature);
                                string created_by_name = CommonClass.GetUserFullnameByUserId(itemReceive.created_by);
                                Image watermarkImage = Image.FromFile(checked_signature_path);
                                TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                //int y = (image.Height - (watermarkImage.Height + 40));
                                int y = (image.Height - (watermarkImage.Height + 50));
                                //int x = watermarkImage.Width/3;
                                int x = 20;
                                //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                watermarkBrush.TranslateTransform(x, y);
                                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                                Brush brush = new SolidBrush(Color.Black);
                                Font font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                                SizeF textSize = new SizeF();
                                textSize = imageGraphics.MeasureString(created_by_name, font);
                                int textY = image.Height - 50;
                                int textX = watermarkImage.Width / 2;
                                Point position = new Point(textX, textY);
                                imageGraphics.DrawString(created_by_name, font, brush, position);

                                textSize = new SizeF();
                                textSize = imageGraphics.MeasureString(CommonClass.ToLocalTime(Convert.ToDateTime(itemReceive.created_date)).ToString("dd-MMM-yyyy"),font);
                                textY = image.Height - 30;
                                position = new Point(textX, textY);
                                imageGraphics.DrawString(CommonClass.ToLocalTime(Convert.ToDateTime(itemReceive.created_date)).ToString("dd-MMM-yyyy"), font, brush, position);

                            }
                            var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), attachment.attachment_id + "_watermark" + attachment.attachment_extension);
                            image.Save(file_path_after_watermark);
                        }

                    }
                }

                tb_receive_item_voucher grn = db.tb_receive_item_voucher.Find(itemReceive.receive_item_voucher_id);
                grn.is_received_partial = isReceivedPartial;
                db.SaveChanges();

                ClsItemReceive.UpdateGoodReceivedNoteReferenceStatus(itemReceive.receive_item_voucher_id, itemReceive.ref_id, status);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            ItemReceiveViewModel itemReceive = new ItemReceiveViewModel();
            itemReceive = Inventory.GetItemReceivedViewDetail(id);
            itemReceive.histories = GetListItemReceivedByRefId(itemReceive.ref_id);
            
            return View(itemReceive);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            ItemReceiveViewModel itemReceive = new ItemReceiveViewModel();
            itemReceive = Inventory.GetItemReceivedViewDetail(id);
            ViewBag.Suppliers = Class.CommonClass.Suppliers();
            ViewBag.PurchaseOrderRefs= Class.CommonClass.GetPurchaseOrderbySupplier(itemReceive.supplier_id,itemReceive.ref_id,itemReceive.po_report_number);
            if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                ViewBag.WarehouseID = Inventory.GetWarehouseListByStockTransferID(itemReceive.ref_id);
            else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                ViewBag.WarehouseID = Inventory.GetWarehouseListByPurchaseOrderID(itemReceive.ref_id);
            return View(itemReceive);
        }
        public ActionResult EditItemReceived(ItemReceiveViewModel model, List<InventoryViewModel> inventories, List<string> Attachment)
        {
            using (kim_mexEntities db=new kim_mexEntities())
            {
                #region check received item duplicate
                /*
                decimal requestedQty = 0, totalReceivedQty = 0;
                var dupItem = inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                foreach(var Item in dupItem)
                {
                    totalReceivedQty = 0;
                    requestedQty = Convert.ToDecimal(inventories.Where(x => x.product_id == Item.product_id && x.warehouse_id == Item.warehouse_id).Select(x => x.out_quantity).FirstOrDefault());
                    var receivedQty = inventories.Where(x => x.product_id == Item.product_id && x.warehouse_id == Item.warehouse_id).Select(x => x.in_quantity).ToList();
                    foreach(var rQty in receivedQty)
                    {
                        totalReceivedQty = totalReceivedQty + Convert.ToDecimal(rQty);
                    }
                    if (totalReceivedQty > requestedQty)
                    {
                        return Json(new { result = "fail",message="Total Received quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                    }
                }
                */
                #endregion
                int countInvalid = 0;
                string message = string.Empty;

                if (string.Compare(model.received_type, "Stock Transfer") == 0)
                {
                    List<InventoryViewModel> remainQuantities = new List<InventoryViewModel>();
                    remainQuantities = this.GetRemainByStockTransfer(model.ref_id);
                    foreach (InventoryViewModel item in remainQuantities)
                    {
                        foreach (InventoryViewModel receive in inventories)
                        {
                            if (item.product_id == receive.product_id && item.total_quantity < CommonClass.ConvertMultipleUnit(receive.product_id, receive.unit, Convert.ToDecimal(receive.in_quantity)))
                            {
                                countInvalid++;
                                if (item.total_quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.itemName, string.Format("{0:G29}", Decimal.Parse(item.total_quantity.ToString())));
                            }
                        }
                    }
                }
                else if (string.Compare(model.received_type, "Purchase Order") == 0)
                {
                    List<PurchaseOrderDetailViewModel> remainQuantities = new List<PurchaseOrderDetailViewModel>();
                    //remainQuantities = this.GetRemainItemByPurchaseOrder(model.ref_id,model.receive_item_voucher_id);
                    remainQuantities = Class.ClsItemReceive.GetReceivedRemainItembyPurchaseOrder(model.ref_id, model.supplier_id, model.po_report_number, model.receive_item_voucher_id);
                    foreach (PurchaseOrderDetailViewModel item in remainQuantities)
                    {
                        foreach (InventoryViewModel receive in inventories)
                        {
                            if (item.item_id == receive.product_id && item.quantity < CommonClass.ConvertMultipleUnit(receive.product_id, receive.unit, Convert.ToDecimal(receive.in_quantity)))
                            {
                                countInvalid++;
                                if (item.quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.product_name, string.Format("{0:G29}", Decimal.Parse(item.quantity.ToString())));
                            }
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                #region update stcok inventory

                if (!string.IsNullOrEmpty(model.receive_item_voucher_id))
                {
                    string receivedId = model.receive_item_voucher_id;
                    //this.UpdateInventory(receivedId);
                    //this.DeleteInventory(receivedId);
                    tb_receive_item_voucher Receive = db.tb_receive_item_voucher.Where(m => m.receive_item_voucher_id == model.receive_item_voucher_id).FirstOrDefault();
                    if (string.Compare(Receive.received_type, "Stock Transfer") == 0)
                        Class.ClsItemReceive.RollbackStockTransferbyItemReceived(Receive.receive_item_voucher_id, Receive.ref_id, true);
                    else if (string.Compare(Receive.received_type, "Purchase Order") == 0)
                        //Class.ClsItemReceive.RollBackPurchaseOrder(Receive.receive_item_voucher_id, Receive.ref_id, true);
                        Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(Receive.receive_item_voucher_id, Receive.ref_id, Receive.po_report_number, Receive.supplier_id, true);
                    this.DeleteReceivedItemDetail(receivedId);
                    CommonClass.DeleteInvoiceNumber(receivedId);
                    CommonClass.AutoGenerateStockInvoiceNumber(receivedId, inventories);
                }
                #endregion

                tb_receive_item_voucher itemReceive = db.tb_receive_item_voucher.Where(m => m.receive_item_voucher_id == model.receive_item_voucher_id).FirstOrDefault();
                itemReceive.received_number = model.received_number;
                itemReceive.received_type = model.received_type;
                itemReceive.ref_id = model.ref_id;
                itemReceive.supplier_id = model.supplier_id;
                itemReceive.po_report_number = model.po_report_number;
                itemReceive.received_status = "Pending";
                itemReceive.status = true;
                itemReceive.updated_by = User.Identity.GetUserId();
                itemReceive.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                foreach (var inv in inventories)
                {
                    //tb_inventory inventory = new tb_inventory();
                    //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.product_id && m.warehouse_id == inv.warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //inventory.inventory_id = Guid.NewGuid().ToString();
                    //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //inventory.inventory_status_id = "7";
                    //inventory.warehouse_id = inv.warehouse_id;
                    //inventory.product_id = inv.product_id;
                    //inventory.total_quantity = totalQty + inv.in_quantity;
                    //inventory.in_quantity = inv.in_quantity;
                    //inventory.out_quantity = 0;
                    //inventory.ref_id = itemReceive.receive_item_voucher_id;
                    //db.tb_inventory.Add(inventory);
                    //db.SaveChanges();
                    tb_received_item_detail temp = new tb_received_item_detail();
                    temp.ri_detail_id = Guid.NewGuid().ToString();
                    temp.ri_ref_id = itemReceive.receive_item_voucher_id;
                    temp.ri_item_id = inv.product_id;
                    temp.ri_warehouse_id = inv.warehouse_id;
                    temp.quantity = inv.in_quantity;
                    temp.unit = inv.unit;
                    temp.item_status = "pending";
                    temp.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                    temp.invoice_number = CommonClass.GetInvoiceNumber(itemReceive.receive_item_voucher_id,temp.ri_warehouse_id,temp.invoice_date);
                    temp.supplier_id = inv.supplier_id;
                    db.tb_received_item_detail.Add(temp);
                    db.SaveChanges();
                }
                if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    //Class.ClsItemReceive.RollBackPurchaseOrder(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                    Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, itemReceive.po_report_number, itemReceive.supplier_id, false);
                else if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    Class.ClsItemReceive.RollbackStockTransferbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                if (Attachment != null && Attachment.Count > 0)
                {
                    foreach (string att in Attachment)
                    {
                        tb_ire_attachment attachment = db.tb_ire_attachment.Where(m => m.ire_attachment_id == att).FirstOrDefault();
                        attachment.ire_id = itemReceive.receive_item_voucher_id;
                        //db.SaveChanges();
                    }
                }

                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approve(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_receive_item_voucher receivedItem = db.tb_receive_item_voucher.Find(id);
                var inventories = db.tb_received_item_detail.Where(m => m.ri_ref_id == id).ToList();
                int countInvalid = 0;
                string message = string.Empty;

                if (string.Compare(receivedItem.received_type, "Stock Transfer") == 0)
                {
                    List<InventoryViewModel> remainQuantities = new List<InventoryViewModel>();
                    remainQuantities = this.GetRemainByStockTransfer(receivedItem.ref_id);
                    foreach (InventoryViewModel item in remainQuantities)
                    {
                        foreach (var receive in inventories)
                        {
                            if (item.product_id == receive.ri_item_id && item.total_quantity < CommonClass.ConvertMultipleUnit(receive.ri_item_id, receive.unit, Convert.ToDecimal(receive.quantity)))
                            {
                                countInvalid++;
                                if (item.total_quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.itemName, string.Format("{0:G29}", Decimal.Parse(item.total_quantity.ToString())));
                            }
                        }
                    }
                }
                else if (string.Compare(receivedItem.received_type, "Purchase Order") == 0)
                {
                    List<PurchaseOrderDetailViewModel> remainQuantities = new List<PurchaseOrderDetailViewModel>();
                    remainQuantities = this.GetRemainItemByPurchaseOrder(receivedItem.ref_id);
                    foreach (PurchaseOrderDetailViewModel item in remainQuantities)
                    {
                        foreach (var receive in inventories)
                        {
                            if (item.item_id == receive.ri_item_id && item.quantity < CommonClass.ConvertMultipleUnit(receive.ri_item_id, receive.unit, Convert.ToDecimal(receive.quantity)))
                            {
                                countInvalid++;
                                if (item.quantity < 0)
                                    message = message + string.Format("{0} is completed to receive.\n");
                                else
                                    message = message + string.Format("{0} remain quantity is {1} to receive.\n", item.product_name, string.Format("{0:G29}", Decimal.Parse(item.quantity.ToString())));
                            }
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                receivedItem.received_status = "Approved";
                receivedItem.approved_by = User.Identity.GetUserId();
                receivedItem.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                this.InsertItemInventory(id);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Reject(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_receive_item_voucher receivedItem = db.tb_receive_item_voucher.Find(id);
                receivedItem.received_status = "Rejected";
                receivedItem.approved_by = User.Identity.GetUserId();
                receivedItem.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_receive_item_voucher receivedItem = db.tb_receive_item_voucher.Find(id);
                receivedItem.status = false;
                receivedItem.updated_by = User.Identity.GetUserId();
                receivedItem.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            Models.ItemReceiveViewModel model = new ItemReceiveViewModel();
            model = Class.ClsItemReceive.GetItemReceivedDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<ItemReceivedDetailViewModel> models)
        {
            try
            {
                int countItemApproved = 0;
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_receive_item_voucher receive = db.tb_receive_item_voucher.Find(id);
                    foreach (ItemReceivedDetailViewModel model in models)
                    {
                        var iid = model.ri_detail_id;
                        Entities.tb_received_item_detail itemDetail= db.tb_received_item_detail.Find(iid);
                        itemDetail.item_status = model.item_status;
                        itemDetail.remark = model.remark;
                        itemDetail.completed = model.completed;
                        db.SaveChanges();
                        if(string.Compare(itemDetail.item_status,"approved")==0 || itemDetail.completed)
                        {
                            countItemApproved++;
                            #region added new wf sep 04 2020
                            //decimal stockBalance = 0;
                            //if(string.Compare(receive.received_type,"Stock Return")==0 || string.Compare(receive.received_type,"Return Workshop")==0)
                            //    stockBalance = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => string.Compare(x.product_id, itemDetail.ri_item_id) == 0 && string.Compare(x.warehouse_id, itemDetail.supplier_id) == 0).Select(x => x.total_quantity).FirstOrDefault());
                            //else
                            //    stockBalance = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => string.Compare(x.product_id, itemDetail.ri_item_id) == 0 && string.Compare(x.warehouse_id, itemDetail.ri_warehouse_id) == 0).Select(x => x.total_quantity).FirstOrDefault());
                            //tb_inventory inventory = new tb_inventory();
                            //inventory.inventory_id = Guid.NewGuid().ToString();
                            //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                            //inventory.ref_id = id;
                            //inventory.inventory_status_id = "7";
                            //inventory.product_id = itemDetail.ri_item_id;
                            //if (string.Compare(receive.received_type, "Stock Return") == 0 || string.Compare(receive.received_type, "Return Workshop") == 0)
                            //    inventory.warehouse_id = itemDetail.supplier_id;
                            //else
                            //    inventory.warehouse_id = itemDetail.ri_warehouse_id;
                            //inventory.out_quantity = 0;
                            //inventory.in_quantity = Class.CommonClass.ConvertMultipleUnit(itemDetail.ri_item_id, itemDetail.unit, Convert.ToDecimal(itemDetail.quantity));
                            //inventory.total_quantity = stockBalance + inventory.in_quantity;
                            //db.tb_inventory.Add(inventory);
                            //db.SaveChanges();
                            #endregion
                        }
                        if (itemDetail.completed)
                            if (string.Compare(receive.received_type, "Stock Transfer") == 0)
                            {
                                Class.ClsItemReceive.UpdateStockTransferReceivedComplete(receive.ref_id, itemDetail.ri_item_id);
                            }else if (string.Compare(receive.received_type, "Purchase Order") == 0)
                            {
                                Class.ClsItemReceive.UpdatePurchaseOrderReceivedComplete(receive.ref_id, itemDetail.ri_item_id,receive.supplier_id,receive.po_report_number);
                            }else if(string.Compare(receive.received_type,"Transfer Workshop") == 0)
                            {
                                Class.ClsItemReceive.UpdateTransferWorkshopReceivedComplete(receive.ref_id, itemDetail.ri_item_id);
                            }
                            else if(string.Compare(receive.received_type,"Stock Return") == 0)
                            {
                                Class.ClsItemReceive.UpdateStockReturnReceivedComplete(receive.ref_id, itemDetail.ri_item_id);
                            }
                    }
                    receive.received_status = countItemApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                    receive.approved_by = User.Identity.GetUserId();
                    receive.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    //start update reference show status
                    string status = string.Empty;
                    if (Convert.ToBoolean(receive.is_received_partial))
                        status = ShowStatus.GRNPartialApproved;
                    else
                        status = ShowStatus.GRNApproved;

                    if (string.Compare(receive.received_type,"Stock Transfer") == 0)
                    {
                        //tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(receive.ref_id);
                        ////update stock transfer and MR status
                        
                        //if (stockTransfer.is_completed)
                        //    status = ShowStatus.STCompleted;
                        //else
                        //    status = ShowStatus.RemainGRNPending;
                        ////CommonClass.UpdateMaterialRequestStatus(stockTransfer.item_request_id, status);
                        //StockTransfer.UpdateStockTransferProcessStatusById(stockTransfer.stock_transfer_id, status);

                        StockTransfer.UpdateStockTransferProcessStatusById(receive.ref_id, status);
                    }
                    else if(string.Compare(receive.received_type,"Stock Return") == 0)
                    {
                        //tb_stock_issue_return stockReturn = db.tb_stock_issue_return.Find(receive.ref_id);
                        //string status = string.Empty;
                        //if (Convert.ToBoolean(stockReturn.is_receive_completed))
                        //    status = ShowStatus.SRCompleted;
                        //else
                        //    status = ShowStatus.RemainGRNPending;
                        
                        //ClsItemReceive.UpdateGoodReceivedNoteReferenceStatus(receive.receive_item_voucher_id, receive.ref_id, status);
                        ClsItemReceive.UpdateStockTransferStatusbyStockReturn(receive.ref_id, status);
                    }else if(string.Compare(receive.received_type, "Purchase Order") == 0)
                    {
                        ClsItemReceive.UpdateMaterialRequestStatusbyPurchaseOrderId(receive.ref_id, status);
                    }

                    ClsItemReceive.UpdateGoodReceivedNoteReferenceStatus(receive.receive_item_voucher_id, receive.ref_id, status);

                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PrepareFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            Models.ItemReceiveViewModel model = new ItemReceiveViewModel();
            model = Class.ClsItemReceive.GetItemReceivedDetail(id);
            if (string.Compare(model.received_type, "Stock Transfer") == 0)
                ViewBag.WarehouseID = Inventory.GetWarehouseListByStockTransferID(model.ref_id);
            else if (string.Compare(model.received_type, "Purchase Order") == 0)
                ViewBag.WarehouseID = Inventory.GetWarehouseListByPurchaseOrderID(model.ref_id);
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<Models.InventoryViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_receive_item_voucher itemReceived = db.tb_receive_item_voucher.Find(id);
                itemReceived.received_status = "Feedbacked";
                itemReceived.updated_by = User.Identity.GetUserId();
                itemReceived.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                if (string.Compare(itemReceived.received_type, "Stock Transfer") == 0)
                {
                    Class.ClsItemReceive.RollbackStockTransferbyItemReceived(itemReceived.receive_item_voucher_id, itemReceived.ref_id, true, "feedbacked");
                }else if (string.Compare(itemReceived.received_type, "Purchase Order") == 0)
                {
                    //Class.ClsItemReceive.RollBackPurchaseOrder(itemReceived.receive_item_voucher_id, itemReceived.ref_id, true, "feedbacked");
                    Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(itemReceived.receive_item_voucher_id, itemReceived.ref_id, itemReceived.po_report_number, itemReceived.supplier_id, true, "feedbacked");
                }
                //delete old feedbacked item
                var feedbackedItemReceived = db.tb_received_item_detail.Where(x => string.Compare(x.item_status, "feedbacked") == 0 && string.Compare(x.ri_ref_id, itemReceived.receive_item_voucher_id) == 0).Select(x=>x.ri_detail_id).ToList();
                foreach(var feedbackedItem in feedbackedItemReceived)
                {
                    var iid = feedbackedItem;
                    tb_received_item_detail receivedDetail = db.tb_received_item_detail.Find(iid);
                    db.tb_received_item_detail.Remove(receivedDetail);
                    db.SaveChanges();
                }
                foreach(InventoryViewModel item in models)
                {
                    tb_received_item_detail receivedDetail = new tb_received_item_detail();
                    receivedDetail.ri_detail_id = Guid.NewGuid().ToString();
                    receivedDetail.ri_ref_id = itemReceived.receive_item_voucher_id;
                    receivedDetail.ri_item_id = item.product_id;
                    receivedDetail.ri_warehouse_id = item.warehouse_id;
                    receivedDetail.quantity = item.in_quantity;
                    receivedDetail.unit = item.unit;
                    receivedDetail.invoice_number = item.invoice_number;
                    receivedDetail.invoice_date = item.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : item.invoice_date;
                    receivedDetail.supplier_id = item.supplier_id;
                    receivedDetail.item_status = "pending";
                    receivedDetail.remark = item.remark;
                    db.tb_received_item_detail.Add(receivedDetail);
                    db.SaveChanges();
                }
                if (string.Compare(itemReceived.received_type, "Purchase Order") == 0)
                    //Class.ClsItemReceive.RollBackPurchaseOrder(itemReceived.receive_item_voucher_id, itemReceived.ref_id, false,"pending");
                    Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(itemReceived.receive_item_voucher_id, itemReceived.ref_id, itemReceived.po_report_number, itemReceived.supplier_id, false, "pending");
                else if (string.Compare(itemReceived.received_type, "Stock Transfer") == 0)
                    Class.ClsItemReceive.RollbackStockTransferbyItemReceived(itemReceived.receive_item_voucher_id, itemReceived.ref_id, false,"pending");
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error",mesage=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approval(string id,string status)
        {
            try
            {
                string showStatus = string.Empty;
                kim_mexEntities db = new kim_mexEntities();
                tb_receive_item_voucher itemReceive = db.tb_receive_item_voucher.Find(id);
                if (itemReceive!=null)
                {
                    itemReceive.received_status = status;
                    itemReceive.checked_by = User.Identity.GetUserId().ToString();
                    itemReceive.checked_date = CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    if (string.Compare(status, Status.Completed) == 0)
                    {
                        var itemDetails = db.tb_received_item_detail.Where(s => string.Compare(s.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).ToList();
                        foreach(var itemDetail in itemDetails)
                        {
                            decimal stockBalance = 0;
                            if (string.Compare(itemReceive.received_type, "Stock Return") == 0 || string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                                stockBalance = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => string.Compare(x.product_id, itemDetail.ri_item_id) == 0 && string.Compare(x.warehouse_id, itemDetail.supplier_id) == 0).Select(x => x.total_quantity).FirstOrDefault());
                            else
                                stockBalance = Convert.ToDecimal(db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => string.Compare(x.product_id, itemDetail.ri_item_id) == 0 && string.Compare(x.warehouse_id, itemDetail.ri_warehouse_id) == 0).Select(x => x.total_quantity).FirstOrDefault());
                            tb_inventory inventory = new tb_inventory();
                            inventory.inventory_id = Guid.NewGuid().ToString();
                            inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                            inventory.ref_id = id;
                            inventory.inventory_status_id = "7";
                            inventory.product_id = itemDetail.ri_item_id;
                            if (string.Compare(itemReceive.received_type, "Stock Return") == 0 || string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                                inventory.warehouse_id = itemDetail.supplier_id;
                            else
                                inventory.warehouse_id = itemDetail.ri_warehouse_id;
                            inventory.out_quantity = 0;
                            inventory.in_quantity = Class.CommonClass.ConvertMultipleUnit(itemDetail.ri_item_id, itemDetail.unit, Convert.ToDecimal(itemDetail.quantity));
                            inventory.total_quantity = stockBalance + inventory.in_quantity;
                            db.tb_inventory.Add(inventory);
                            db.SaveChanges();
                        }
                        string mrShowStatus = string.Empty;
                        if (Convert.ToBoolean(itemReceive.is_received_partial))
                        {
                            
                            showStatus = ShowStatus.RemainGRNPending;
                        }
                            
                        else
                        {
                            if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                            {
                                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(itemReceive.ref_id);
                                if (Convert.ToBoolean(stockTransfer.is_return))
                                {
                                    showStatus = ShowStatus.SRPending;
                                }else
                                {
                                    showStatus = ShowStatus.STCompleted;
                                }
                                
                            }
                            else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                            {
                                showStatus = ShowStatus.MRCompleted;
                            }
                            else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                            {
                                showStatus = ShowStatus.TWCompleted;
                            }
                            else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                            {
                                showStatus = ShowStatus.SRCompleted;
                            }
                        }

                    }else
                    {
                        if (Convert.ToBoolean(itemReceive.is_received_partial))
                            showStatus = ShowStatus.GRNPartialRejected;
                        else
                            showStatus = ShowStatus.GRNRejected;
                    }
                    ClsItemReceive.UpdateGoodReceivedNoteReferenceStatus(itemReceive.receive_item_voucher_id, itemReceive.ref_id, showStatus);

                    #region update reference show status
                    if(string.Compare(itemReceive.received_type,"Purchase Order") == 0)
                    {
                        ClsItemReceive.UpdateMaterialRequestStatusbyPurchaseOrderId(itemReceive.ref_id, showStatus);

                    }else if(string.Compare(itemReceive.received_type,"Stock Return") == 0)
                    {
                        ClsItemReceive.UpdateStockTransferStatusbyStockReturn(itemReceive.ref_id, showStatus);
                    }
                    else if(string.Compare(itemReceive.received_type,"Stock Transfer") == 0)
                    {
                        StockTransfer.UpdateStockTransferProcessStatusById(itemReceive.ref_id, showStatus);
                    }
                    #endregion


                    //#region update signature on Delivery Order
                    //var doAttachment = db.tb_attachment.Where(s => string.Compare(s.attachment_ref_id, itemReceive.receive_item_voucher_id) == 0 && string.Compare(s.attachment_ref_type, "DO") == 0).FirstOrDefault();
                    //if (doAttachment != null)
                    //{
                    //    var file_path = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"),doAttachment.attachment_id + doAttachment.attachment_extension);
                    //    Image image = Image.FromFile(file_path);
                    //    Graphics imageGraphics = Graphics.FromImage(image);
                        

                    //    /* CREATED Section */
                    //    string createdBySignature = CommonClass.GetUserSignature(itemReceive.created_by);
                    //    if (!string.IsNullOrEmpty(createdBySignature))
                    //    {
                    //        var created_signature_path = Server.MapPath("~" + createdBySignature);
                    //        Image watermarkImage = Image.FromFile(created_signature_path);
                    //        TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //        int y = (image.Height - (watermarkImage.Height + 20));
                    //        //int xLast = (image.Width - watermarkImage.Width - (watermarkImage.Width / 4)); //Last
                    //        int xLast = (image.Width - watermarkImage.Width);
                    //        //watermarkBrush = new TextureBrush(watermarkImage);
                    //        watermarkBrush.TranslateTransform(xLast, y);
                    //        imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(xLast, y), new Size(watermarkImage.Width, watermarkImage.Height)));
                    //    }

                    //    /* APPROVED Section */
                    //    string approvedBySignature = CommonClass.GetUserSignature(itemReceive.approved_by);
                    //    if (!string.IsNullOrEmpty(approvedBySignature))
                    //    {
                    //        var approved_signature_path = Server.MapPath("~" + approvedBySignature);
                    //        Image watermarkImage = Image.FromFile(approved_signature_path);
                    //        TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //        int y = (image.Height - (watermarkImage.Height + 20));
                    //        int x = (image.Width / 2 - watermarkImage.Width / 2);
                    //        //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //        watermarkBrush.TranslateTransform(x, y);
                    //        imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                    //    }

                    //    /* CHECKED Section */
                    //    string checkedBySignature = CommonClass.GetUserSignature(itemReceive.checked_by);
                    //    if (!string.IsNullOrEmpty(checkedBySignature))
                    //    {
                    //        var checked_signature_path = Server.MapPath("~" + checkedBySignature);
                    //        Image watermarkImage = Image.FromFile(checked_signature_path);
                    //        TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //        //int y = (image.Height - (watermarkImage.Height + 40));
                    //        int y = (image.Height - (watermarkImage.Height +20));
                    //        //int x = watermarkImage.Width/3;
                    //        int x = 20;
                    //        //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //        watermarkBrush.TranslateTransform(x, y);
                    //        imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                    //    }


                    //    //var watermark_path = Server.MapPath("~/Assets/Images/Lotus_Logo.png");
                    //    //Image watermarkImage = Image.FromFile(watermark_path);


                    //    //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //    //int x = (image.Width / 2 - watermarkImage.Width / 2); //middle 
                    //    //int y = (image.Height - (watermarkImage.Height + 40));
                    //    //watermarkBrush.TranslateTransform(x, y);
                    //    //imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                    //    //int xLast = (image.Width - watermarkImage.Width - (watermarkImage.Width / 2)); //Last
                    //    //watermarkBrush = new TextureBrush(watermarkImage);
                    //    //watermarkBrush.TranslateTransform(xLast, y);
                    //    //imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(xLast, y), new Size(watermarkImage.Width, watermarkImage.Height)));

                    //    //int xFirst = watermarkImage.Width / 3; //First
                    //    //watermarkBrush = new TextureBrush(watermarkImage);
                    //    //watermarkBrush.TranslateTransform(xFirst, y);
                    //    //imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(xFirst, y), new Size(watermarkImage.Width, watermarkImage.Height)));

                    //    var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), doAttachment.attachment_id + "_watermark" + doAttachment.attachment_extension);
                    //    image.Save(file_path_after_watermark);

                    //    //}
                    //}

                    //#endregion  

                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UploadAttachment()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_ire_attachment ire_attachment = new tb_ire_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    ire_attachment.ire_attachment_id = file_id;
                    ire_attachment.ire_attachment_name = file_name;
                    ire_attachment.ire_attachment_extension = file_extension;
                    ire_attachment.ire_file_path = file_path;
                    db.tb_ire_attachment.Add(ire_attachment);
                    db.SaveChanges();
                }
                return Json(new { result = "success", attachment_id = ire_attachment.ire_attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadDOAttachment()
        {
            kim_mexEntities db = new kim_mexEntities();
            tb_attachment ire_attachment = new tb_attachment();
            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    ire_attachment.attachment_id = file_id;
                    ire_attachment.attachment_name = file_name;
                    ire_attachment.attachment_extension = file_extension;
                    ire_attachment.attachment_path = file_path;
                    ire_attachment.attachment_ref_type = "DO";
                    db.tb_attachment.Add(ire_attachment);
                    db.SaveChanges();


                    

                    //Image image = Image.FromFile(file_path);
                    //using (Bitmap bitmap = new Bitmap(file.InputStream, false))
                    //{
                    //    using(Graphics graphics = Graphics.FromImage(bitmap))
                    //    {
                    //        Brush brush = new SolidBrush(Color.Black);
                    //        Font font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                    //        SizeF textSize = new SizeF();
                    //        textSize = graphics.MeasureString("Bitches", font);
                    //        Point position = new Point(10, image.Height-20);
                    //        graphics.DrawString("Bitches", font, brush, position);
                    //        var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), file_id + "_watermark_text" + file_extension);
                    //        bitmap.Save(file_path_after_watermark);
                    //        //using (MemoryStream mStream = new MemoryStream())
                    //        //{
                    //        //    bitmap.Save(mStream, ImageFormat.Png);
                    //        //    mStream.Position = 0;
                                
                    //        //}

                    //    }
                    //}



                    //var watermark_path = Server.MapPath("~/Assets/Images/Lotus_Logo.png");
                    //Image image = Image.FromFile(file_path);

                    //Image watermarkImage = Image.FromFile(watermark_path);
                    //Graphics imageGraphics = Graphics.FromImage(image);
                    ////using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
                    ////{
                    //    TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                    //    int x = (image.Width / 2 - watermarkImage.Width / 2); //middle 
                    //    int y = (image.Height - (watermarkImage.Height + 40));
                    //    watermarkBrush.TranslateTransform(x, y);
                    //    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                    //    int xLast = (image.Width - watermarkImage.Width-(watermarkImage.Width/2)); //Last
                    //    watermarkBrush = new TextureBrush(watermarkImage);
                    //    watermarkBrush.TranslateTransform(xLast, y);
                    //    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(xLast, y), new Size(watermarkImage.Width, watermarkImage.Height)));

                    //    int xFirst = watermarkImage.Width/3; //First
                    //    watermarkBrush = new TextureBrush(watermarkImage);
                    //    watermarkBrush.TranslateTransform(xFirst, y);
                    //    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(xFirst, y), new Size(watermarkImage.Width, watermarkImage.Height)));

                    //    var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), file_id + "_watermark" + file_extension);
                    //    image.Save(file_path_after_watermark);
                    ////}
                }
                catch(Exception ex)
                {
                    
                }
                

            }
            return Json(new { result = "success", attachment_id = ire_attachment.attachment_id }, JsonRequestBehavior.AllowGet);
        }

        public FileResult Download(String p,String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        [HttpPost]
        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_ire_attachment att = db.tb_ire_attachment.Find(id);
                if (att == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_ire_attachment.Remove(att);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/IRe Attachment/"), att.ire_attachment_id + att.ire_attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public string GetItemReceiveNumber()
        {
            string itemReceiveNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "";
                string number = (from tbl in db.tb_receive_item_voucher orderby tbl.created_date descending select tbl.received_number).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    number = number.Substring(number.Length - 3, 3);
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1) last_no = "00" + num;
                    else if (num.ToString().Length == 2) last_no = "0" + num;
                    else if (num.ToString().Length == 3) last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                itemReceiveNo = "IRe-" + yy + "-" + mm + "-" + last_no;
            }
            return itemReceiveNo;
        }
        public ActionResult GetPurchaseOrderNumberDropdownlist()
        {
            
            /*
            List<tb_purchase_order> purchaseOrders = new List<tb_purchase_order>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                purchaseOrders = db.tb_purchase_order.OrderBy(m => m.purchase_oder_number).Where(m => m.status == true && String.Compare(m.purchase_order_status, "Completed") == 0).ToList();
            }
            */
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            List<PurchaseOrderViewModel> purchaseOrders1 = new List<PurchaseOrderViewModel>();
            // purchaseOrders = Inventory.GetItemReceivedPurchaseOrderList();
            try
            {
                if (User.IsInRole("Admin"))
                    purchaseOrders = PurchaseOrder.GetPurchaseOrderDropdownList();
                else if (User.IsInRole("Stock Keeper"))
                {
                    kim_mexEntities db = new kim_mexEntities();
                    string userID = Class.CommonClass.GetUserId(User.Identity.Name);
                    var warehouses = db.tb_stock_keeper_warehouse.Where(w => string.Compare(w.stock_keeper, userID) == 0).Select(s => s.warehouse_id).ToList();
                    foreach (var warehouse in warehouses)
                    {
                        purchaseOrders1 = PurchaseOrder.GetPurchaseOrderDropdownList().Where(w => string.Compare(w.warehouseID, warehouse) == 0).ToList();
                        if (purchaseOrders1.Any())
                        {
                            foreach (var po in purchaseOrders1)
                            {
                                purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number });
                            }
                        }
                    }

                    //purchaseOrders = PurchaseOrder.GetPurchaseOrderDropdownList();
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemReceiveController.cs", "GetPurchaseOrderNumberDropdownlist", ex.StackTrace, ex.Message);
            }
            return Json(new { data = purchaseOrders },JsonRequestBehavior.AllowGet);
        }
        public ActionResult RequestCancel(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_receive_item_voucher itemReceive = db.tb_receive_item_voucher.Find(id);
                itemReceive.received_status = Status.cancelled;
                itemReceive.updated_by = User.Identity.GetUserId();
                itemReceive.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    //Class.ClsItemReceive.RollBackPurchaseOrder(itemReceive.receive_item_voucher_id, itemReceive.ref_id, false);
                    Class.ClsItemReceive.TrackPurchaseOrderItembyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, itemReceive.po_report_number, itemReceive.supplier_id, true);
                else if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    Class.ClsItemReceive.RollbackStockTransferbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, true);
                else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    Class.ClsItemReceive.RollbackWorkshopTransferbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, true);
                else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                    Class.ClsItemReceive.RollbackStockReturnbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, true);
                else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    Class.ClsItemReceive.RollbackReturnWorkshopbyItemReceived(itemReceive.receive_item_voucher_id, itemReceive.ref_id, true);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetStockTransferNumberDropdownlist(string transferId=null)
        {
            List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    //stockTransfers = db.tb_stock_transfer_voucher.OrderBy(m => m.stock_transfer_no).Where(m => m.status == true&&string.Compare(m.stock_transfer_status,"Approved")==0).ToList();
                    //stockTransfers = db.tb_stock_transfer_voucher.OrderBy(m => m.stock_transfer_no).Where(x => x.status == true && x.is_completed == false && string.Compare(x.stock_transfer_status, "Completed") == 0).ToList();
                    stockTransfers = (from st in db.tb_stock_transfer_voucher
                                      join pr in db.tb_item_request on st.item_request_id equals pr.ir_id
                                      join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                      orderby st.stock_transfer_no descending
                                      where st.status == true && st.is_completed == false && string.Compare(st.stock_transfer_status, Status.Completed) == 0
                                      select new StockTransferViewModel() { stock_transfer_id=st.stock_transfer_id,stock_transfer_no=st.stock_transfer_no,item_request_id=wh.warehouse_id }).ToList();
                    if (!string.IsNullOrEmpty(transferId))
                    {
                        //string stockTransferId = db.tb_receive_item_voucher.Where(x => string.Compare(x.receive_item_voucher_id, receivedId) == 0).Select(x => x.ref_id).FirstOrDefault().ToString();
                        bool isExist = Convert.ToBoolean(stockTransfers.Where(x => string.Compare(x.stock_transfer_id, transferId) == 0).FirstOrDefault());
                        if (!isExist)
                        {
                            string stockTransferNo = db.tb_stock_transfer_voucher.Where(x => string.Compare(x.stock_transfer_id, transferId) == 0).Select(x => x.stock_transfer_no).FirstOrDefault();
                            string warehouseId = Class.CommonClass.GetWarehouseIdbyStockTransfer(transferId);
                            stockTransfers.Add(new StockTransferViewModel() { stock_transfer_id = transferId, stock_transfer_no = stockTransferNo,item_request_id=warehouseId });
                        }
                    }

                    if(User.IsInRole(Role.SiteStockKeeper))
                    {
                        List<StockTransferViewModel> transfer1 = new List<StockTransferViewModel>();
                        transfer1 = stockTransfers;
                        stockTransfers = new List<StockTransferViewModel>();
                        string userID = Class.CommonClass.GetUserId(User.Identity.Name);
                        var warehouses = db.tb_stock_keeper_warehouse.Where(w => string.Compare(w.stock_keeper, userID) == 0).Select(s => s.warehouse_id).ToList();
                        foreach (var warehouse in warehouses)
                        {
                            var stockTransferss = transfer1.Where(w => string.Compare(warehouse, w.item_request_id) == 0).ToList();
                            foreach(var stockTransfer in stockTransferss)
                            {
                                stockTransfers.Add(new StockTransferViewModel() { stock_transfer_id = stockTransfer.stock_transfer_id, stock_transfer_no = stockTransfer.stock_transfer_no });
                            }
                        }
                    }

                    stockTransfers = stockTransfers.OrderBy(x => x.stock_transfer_no).ToList();
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemReceiveController.cs", "GetStockTransferNumberDropdownlist", ex.StackTrace, ex.Message);
            }
            return Json(new { data = stockTransfers },JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockTransferItem(string id,string receivedId=null)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            //inventories = Inventory.GetStockTransferItems(id);
            inventories = Inventory.GetStockTransferRemainItems(id,receivedId);
            return Json(new { result = "success", data = inventories }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPurchaseOrderItem(string id,string supplierid,string purchaseOrderNumber,string receivedId=null)
        {
            List<PurchaseOrderDetailViewModel> purchaseOrders = new List<PurchaseOrderDetailViewModel>();
            //purchaseOrders = Inventory.GetPurchaseOrderItems(id);

            //purchaseOrders = this.GetRemainItemByPurchaseOrder(id,receivedId);
            purchaseOrders = ClsItemReceive.GetReceivedRemainItembyPurchaseOrder(id, supplierid,purchaseOrderNumber, receivedId);
            return Json(new { result = "success", data = purchaseOrders }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWarehouseByStockTransfer(string id)
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            warehouses = Inventory.GetWarehouseListByStockTransferID(id);
            return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWarehouseByPurchaseOrder(string id)
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            warehouses = Inventory.GetWarehouseListByPurchaseOrderID(id);
            return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        }
        #region added on Jun 04 2019
        public ActionResult GetPurchaseOrderbySupplier(string id)
        {
            List<Models.PurchaseOrderSupplier> purchaseOrders = new List<PurchaseOrderSupplier>();
            purchaseOrders = Class.CommonClass.GetPurchaseOrderbySupplier(id);
            if (User.IsInRole(Role.SiteStockKeeper))
            {
                List<Models.PurchaseOrderSupplier> skPurchaseOrders = new List<PurchaseOrderSupplier>();
                kim_mexEntities db = new kim_mexEntities();
                string userId = User.Identity.GetUserId();
                var stockKeeperWarehouses = db.tb_stock_keeper_warehouse.Where(x => string.Compare(x.stock_keeper, userId) == 0).Select(x => new { x.warehouse_id }).ToList();
                foreach(var wh in stockKeeperWarehouses)
                {
                    var POs = purchaseOrders.Where(x => string.Compare(x.warehouseId, wh.warehouse_id) == 0).ToList();
                    foreach (var po in POs)
                        skPurchaseOrders.Add(new PurchaseOrderSupplier()
                        {
                            purchaseOrderId=po.purchaseOrderId,
                            purchaseOrderNumber=po.purchaseOrderNumber,
                            poSupplierId=po.poSupplierId,
                            warehouseId=po.warehouseId,
                            created_date=po.created_date,
                            itemRequestNumber=po.itemRequestNumber,
                        });
                }
                skPurchaseOrders = skPurchaseOrders.OrderByDescending(o => o.created_date).ToList();
                return Json(new { result = "success", data = skPurchaseOrders }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "success", data = purchaseOrders }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult GetItemReceivedDataTable(string status)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
            //itemReceives = Inventory.GetReceiveItemsList();
            IQueryable<ItemReceiveViewModel> itemReceiveList;
            if (string.Compare(status, "All") == 0)
            {
                //if(User.IsInRole("Admin"))
                //    itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true).Select(x=>new ItemReceiveViewModel() {receive_item_voucher_id=x.receive_item_voucher_id,received_number=x.received_number,received_status=x.received_status,received_type=x.received_type ,ref_id=x.ref_id,created_date=x.created_date, po_report_number=x.po_report_number,supplier_id=x.supplier_id, });
                //else if(User.IsInRole("Stock Keeper"))
                //    itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && string.Compare(m.created_by,User.Identity.GetUserId())==0).Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id,  });
                //else
                //    itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true).Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id,  });

                itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true).Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id,created_by=x.created_by });
                //itemReceiveList = (from x in db.tb_receive_item_voucher
                //                   where x.status == true
                //                   select new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id });
            }
            else
            {
                itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true&&m.received_status==status).Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id ,created_date=x.created_date,po_report_number=x.po_report_number,supplier_id=x.supplier_id,created_by=x.created_by});
            }
            //var itemReceiveList = (from ri in db.tb_receive_item_voucher orderby ri.received_number where ri.status == true select ri).ToList();
            if (itemReceiveList.Count() > 0)
            {
                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();
                    string ref_number = string.Empty;
                    string supplier = string.Empty;
                    string invoiceNumber = string.Empty;
                    string invoiceDate = string.Empty;

                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    {
                        //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                        ref_number = itemReceive.po_report_number;
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, sup) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, recItem.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    var duplicateInoviceNumbers = itemDetails.GroupBy(x => x.invoice_number).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInoviceNumbers.Any())
                    {
                        foreach (var inv in duplicateInoviceNumbers)
                            invoiceNumber = invoiceNumber + inv + ", ";
                    }
                    var duplicateInovicedate = itemDetails.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInovicedate.Any())
                    {
                        foreach (var invDate in duplicateInovicedate)
                            invoiceDate = invoiceDate + Convert.ToDateTime(invDate).ToString("dd/MM/yyyy") + ", ";
                    }
                    foreach (var recItem in itemDetails)
                    {
                        bool isDupInvNo = duplicateInoviceNumbers.Where(x => string.Compare(x, recItem.invoice_number) == 0).ToList().Count() > 0 ? true : false;
                        bool isDupInvDate = duplicateInovicedate.Where(x => x == recItem.invoice_date).ToList().Count() > 0 ? true : false;
                        if (!isDupInvNo) invoiceNumber = invoiceNumber + recItem.invoice_number + ", ";
                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(recItem.invoice_date).ToString("dd/MM/yyyy") + ", ";
                    }

                    itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                    itemRec.received_number = itemReceive.received_number;
                    itemRec.received_status = itemReceive.received_status;
                    itemRec.received_type = itemReceive.received_type;
                    itemRec.ref_id = itemReceive.ref_id;
                    itemRec.ref_number = ref_number;
                    itemRec.created_date = itemReceive.created_date;
                    //itemRec.received_status = itemReceive.received_status;
                    itemRec.supplier = supplier;
                    itemRec.invoiceDate = invoiceDate;
                    itemRec.invoiceNumber = invoiceNumber;
                    itemRec.created_by = itemReceive.created_by;
                    itemReceives.Add(itemRec);
                }
            }
            

            return Json(new { data = itemReceives }, JsonRequestBehavior.AllowGet);
        }
        public void UpdateInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory.Where(m => m.ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var item = (from tbl in db.tb_inventory orderby tbl.inventory_date descending where tbl.product_id == inventory.product_id && tbl.warehouse_id == inventory.warehouse_id select tbl).ToList();
                    if (string.Compare(inventory.inventory_id, item[0].inventory_id) != 0)
                    {
                        string inventoryID = item[0].inventory_id;
                        tb_inventory inv = db.tb_inventory.Where(m => m.inventory_id == inventoryID).FirstOrDefault();
                        inv.total_quantity = (inv.total_quantity - inventory.in_quantity) + inventory.out_quantity;
                        db.SaveChanges();
                    }
                }
            }
        }
        public void DeleteInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory.Where(m => m.ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_id;
                    tb_inventory inv = db.tb_inventory.Where(m => m.inventory_id == inventoryID).FirstOrDefault();
                    db.tb_inventory.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void DeleteReceivedItemDetail(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_received_item_detail.Where(m => m.ri_ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.ri_detail_id;
                    tb_received_item_detail inv = db.tb_received_item_detail.Where(m => m.ri_detail_id == inventoryID).FirstOrDefault();
                    db.tb_received_item_detail.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void InsertItemInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_received_item_detail.Where(m => m.ri_ref_id == id).ToList();
                foreach (var inv in inventories)
                {
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.ri_item_id && m.warehouse_id == inv.ri_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "7";
                    inventory.warehouse_id = inv.ri_warehouse_id;
                    inventory.product_id = inv.ri_item_id;
                    inventory.out_quantity = 0;
                    inventory.in_quantity =CommonClass.ConvertMultipleUnit(inv.ri_item_id,inv.unit,Convert.ToDecimal(inv.quantity));
                    inventory.total_quantity = totalQty + inventory.in_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }
        }
        //<StockTransfer>>
        public List<InventoryViewModel> GetRemainByStockTransfer(string tranferId,string receivedId=null)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<Models.StockTransferItemViewModel> transferItems = new List<StockTransferItemViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItems = new List<ItemReceivedDetailViewModel>();
                if (string.IsNullOrEmpty(receivedId))
                {
                    transferItems = (from transfer in db.tb_stock_transfer_detail
                                     where string.Compare(transfer.st_ref_id, tranferId) == 0 && transfer.remain_quantity > 0
                                     select new Models.StockTransferItemViewModel{ st_detail_id=transfer.st_detail_id,st_item_id=transfer.st_item_id,quantity=transfer.remain_quantity,itemUnit=transfer.unit }).ToList();
                }
                else
                {
                    transferItems = (from transfer in db.tb_stock_transfer_detail
                                     where string.Compare(transfer.st_ref_id, tranferId) == 0
                                     select new Models.StockTransferItemViewModel() { st_detail_id = transfer.st_detail_id, st_item_id = transfer.st_item_id, quantity = transfer.remain_quantity, itemUnit = transfer.unit }).ToList();
                    receivedItems = (from received in db.tb_received_item_detail
                                     where string.Compare(received.ri_ref_id, receivedId) == 0 && string.Compare(received.item_status, "approved") != 0
                                     select new Models.ItemReceivedDetailViewModel() {ri_detail_id=received.ri_detail_id,ri_item_id=received.ri_item_id,quantity=received.quantity,unit=received.unit }).ToList();
                }
                foreach (StockTransferItemViewModel transferItem in transferItems)
                {
                    decimal receivedQuantity = 0;
                    var receivedTransfer = receivedItems.Where(x => string.Compare(x.ri_item_id, transferItem.st_item_id) == 0).FirstOrDefault();
                    if (receivedTransfer != null)
                        receivedQuantity = Class.CommonClass.ConvertMultipleUnit(receivedTransfer.ri_item_id, receivedTransfer.unit,Convert.ToDecimal(receivedTransfer.quantity));
                   
                    Models.InventoryViewModel inventory = new InventoryViewModel();
                    inventory.product_id = transferItem.st_item_id;
                    inventory.itemName = db.tb_product.Where(x => string.Compare(x.product_id, inventory.product_id) == 0).Select(x => x.product_name).FirstOrDefault().ToString();
                    inventory.total_quantity = transferItem.quantity + receivedQuantity;
                    inventories.Add(inventory);
                }

                #region old process
                /*
                List<InventoryViewModel> itemReceived = new List<InventoryViewModel>();
                var itemTransfers = (from inv in db.tb_inventory
                                     join product in db.tb_product on inv.product_id equals product.product_id
                                where string.Compare(inv.ref_id, tranferId) == 0
                                select new { inv,product }).ToList();
                var receiveTransfers = (from ret in db.tb_receive_item_voucher
                                        where ret.ref_id == tranferId && ret.status==true && ret.received_status=="Approved"
                                        select ret.receive_item_voucher_id).ToList();
                if (receiveTransfers.Any())
                {
                    foreach(var rt in receiveTransfers)
                    {
                        var received = (from inv in db.tb_inventory
                                        where inv.ref_id == rt
                                        select new { inv }).ToList();
                        if (received.Any())
                            foreach (var re in received)
                                itemReceived.Add(new InventoryViewModel() { product_id = re.inv.product_id, in_quantity = re.inv.in_quantity });
                    }
                }
                if (itemTransfers.Any())
                {
                    foreach(var tranfer in itemTransfers)
                    {
                        decimal remainQuantity = Convert.ToDecimal(tranfer.inv.out_quantity);
                        if (itemReceived.Count() > 0)
                        {
                            foreach(InventoryViewModel ire in itemReceived)
                            {
                                if (tranfer.inv.product_id == ire.product_id)
                                    remainQuantity = remainQuantity - Convert.ToDecimal(ire.in_quantity);
                            }
                        }
                        if(remainQuantity>0)
                        inventories.Add(new InventoryViewModel() { product_id = tranfer.inv.product_id,itemName=tranfer.product.product_name, total_quantity = remainQuantity });
                    }
                }
                */
                #endregion
            }
            return inventories;
        }
        public List<PurchaseOrderDetailViewModel> GetRemainItemByPurchaseOrder(string poId,string receivedId=null)
        {
            List<PurchaseOrderDetailViewModel> inventories = new List<PurchaseOrderDetailViewModel>();
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    List<InventoryViewModel> itemReceived = new List<InventoryViewModel>();
                    if (string.IsNullOrEmpty(receivedId))
                    {
                        var purchaseOrderDetails = (from dPo in db.tb_purchase_order_detail
                                                    join product in db.tb_product on dPo.item_id equals product.product_id
                                                    where string.Compare(dPo.purchase_order_id, poId) == 0 && (string.Compare(dPo.item_status, "approved") == 0 || string.Compare(dPo.item_status, "Pending") == 0) && dPo.remain_quantity > 0
                                                    select new { dPo, product }).ToList();
                        foreach (var po in purchaseOrderDetails)
                        {
                            PurchaseOrderDetailViewModel inv = new PurchaseOrderDetailViewModel();

                            inv.po_detail_id = po.dPo.po_detail_id;
                            inv.purchase_order_id = po.dPo.purchase_order_id;
                            inv.item_id = po.dPo.item_id;
                            inv.product_name = po.product.product_name;
                            inv.product_code = po.product.product_code;
                            inv.product_unit = po.product.product_unit;
                            //add 
                            inv.unit = po.dPo.po_unit;
                            //  inv.PO_Unit = po.dPo.po_unit;
                            inv.item_unit = po.dPo.po_unit;
                            inv.unitLevel = Class.CommonClass.GetItemUnitLevel(inv.item_id, inv.item_unit);

                            inv.quantity = po.dPo.remain_quantity;
                            inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            inv.supplier_id = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_id).FirstOrDefault().ToString();
                            inv.supplier_name = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_name).FirstOrDefault().ToString();
                            inventories.Add(inv);
                        }
                    }
                    else
                    {
                        var purchaseOrderDetails = (from dPo in db.tb_purchase_order_detail
                                                    join product in db.tb_product on dPo.item_id equals product.product_id
                                                    where string.Compare(dPo.purchase_order_id, poId) == 0 && (string.Compare(dPo.item_status, "approved") == 0 || string.Compare(dPo.item_status, "Pending") == 0)
                                                    select new { dPo, product }).ToList();
                        var receivedItems = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, receivedId) == 0).ToList();
                        foreach (var po in purchaseOrderDetails)
                        {
                            decimal receivedQuantity = 0;
                            var isExist = receivedItems.Where(x => string.Compare(x.ri_item_id, po.dPo.item_id) == 0).FirstOrDefault();
                            if (isExist != null)
                            {
                                receivedQuantity = Class.CommonClass.ConvertMultipleUnit(isExist.ri_item_id, isExist.unit, Convert.ToDecimal(isExist.quantity));

                            }
                            PurchaseOrderDetailViewModel inv = new PurchaseOrderDetailViewModel();
                            inv.po_detail_id = po.dPo.po_detail_id;
                            inv.purchase_order_id = po.dPo.purchase_order_id;
                            inv.item_id = po.dPo.item_id;
                            inv.product_name = po.product.product_name;
                            inv.product_code = po.product.product_code;
                            inv.product_unit = po.product.product_unit;
                            inv.item_unit = po.dPo.po_unit;//add
                            inv.unit = po.dPo.po_unit;//add
                            inv.quantity = po.dPo.remain_quantity + receivedQuantity;
                            inv.unitLevel = Class.CommonClass.GetItemUnitLevel(inv.item_id, po.dPo.po_unit);
                            inv.uom = db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            inv.supplier_id = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_id).FirstOrDefault().ToString();
                            inv.supplier_name = (from spo in db.tb_po_supplier join sup in db.tb_supplier on spo.supplier_id equals sup.supplier_id where string.Compare(spo.po_detail_id, inv.po_detail_id) == 0 && spo.is_selected == true select sup.supplier_name).FirstOrDefault().ToString();
                            inventories.Add(inv);
                        }
                    }
                    #region blocked cause new process
                    /*
                    var purchaseOrderItems = (from dPo in db.tb_purchase_order_detail
                                              join product in db.tb_product on dPo.item_id equals product.product_id
                                              orderby product.product_code
                                              where string.Compare(dPo.purchase_order_id, poId) == 0 && string.Compare(dPo.item_status, "approved") == 0
                                              select new { dPo,product}).ToList();
                    var purchaseOrderReceives = (from re in db.tb_receive_item_voucher
                                                 where string.Compare(re.ref_id, poId) == 0 && re.status == true && string.Compare(re.received_status, "Approved") == 0
                                                 select re.receive_item_voucher_id).ToList();
                    if (purchaseOrderReceives.Any())
                    {
                        foreach(var pore in purchaseOrderReceives)
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
                        foreach(var po in purchaseOrderItems)
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
                            if (remainPOQuantity > 0)
                            {
                                PurchaseOrderDetailViewModel inv = new PurchaseOrderDetailViewModel();
                                inv.po_detail_id = po.dPo.po_detail_id;
                                inv.purchase_order_id = po.dPo.purchase_order_id;
                                inv.item_id = po.dPo.item_id;
                                inv.product_name = po.product.product_name;
                                inv.product_code = po.product.product_code;
                                inv.product_unit = po.product.product_unit;
                                inv.quantity = remainPOQuantity;
                                inv.uom= db.tb_multiple_uom.Where(x => x.product_id == inv.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                                inventories.Add(inv);
                            }
                        }
                    }
                    */
                    #endregion
                    inventories = inventories.OrderBy(x => x.product_code).ToList();
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemReceiveController.cs", "GetRemainItemByPurchaseOrder", ex.StackTrace, ex.Message);
            }
            
            return inventories;
        }
        public ActionResult MyRequest()
        {
            //return View(GetItemReceiveRequestListitems());
            return View();
        }
        public ActionResult MyApproval()
        {
            //return View(GetItemReceiveApprovalListitems());
            return View();
        }
        #region Item Receive
        private List<ItemReceiveViewModel> GetItemReceiveRequestListitems(string dateRange)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                string userid = User.Identity.GetUserId();
                List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
                List<ItemReceiveViewModel> itemReceiveList = new List<ItemReceiveViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                    itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && m.created_date>=startDate && m.created_date<=endDate)
                        .Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id,is_received_partial=x.is_received_partial }).ToList();
                if (User.IsInRole(Role.SiteStockKeeper))
                    itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && string.Compare(m.created_by, userid) == 0 && m.created_date >= startDate && m.created_date <= endDate)
                        .Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id, is_received_partial = x.is_received_partial }).ToList();

                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();
                    string ref_number = string.Empty;
                    string supplier = string.Empty;
                    string invoiceNumber = string.Empty;
                    string invoiceDate = string.Empty;

                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    {
                        ref_number = db.transferformmainstocks.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    {
                        ref_number = db.tb_return_main_stock.Where(m => m.status == true && m.return_main_stock_id == itemReceive.ref_id).Select(m => m.return_main_stock_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                    {
                        ref_number = db.tb_stock_issue_return.Where(m => m.status == true && m.stock_issue_return_id == itemReceive.ref_id).Select(m => m.issue_return_number).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    {
                        //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                        ref_number = itemReceive.po_report_number;
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, sup) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, recItem.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    var duplicateInoviceNumbers = itemDetails.GroupBy(x => x.invoice_number).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInoviceNumbers.Any())
                    {
                        foreach (var inv in duplicateInoviceNumbers)
                            invoiceNumber = invoiceNumber + inv + ", ";
                    }
                    var duplicateInovicedate = itemDetails.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInovicedate.Any())
                    {
                        foreach (var invDate in duplicateInovicedate)
                            invoiceDate = invoiceDate + Convert.ToDateTime(invDate).ToString("dd/MM/yyyy") + ", ";
                    }
                    foreach (var recItem in itemDetails)
                    {
                        bool isDupInvNo = duplicateInoviceNumbers.Where(x => string.Compare(x, recItem.invoice_number) == 0).ToList().Count() > 0 ? true : false;
                        bool isDupInvDate = duplicateInovicedate.Where(x => x == recItem.invoice_date).ToList().Count() > 0 ? true : false;
                        if (!isDupInvNo) invoiceNumber = invoiceNumber + recItem.invoice_number + ", ";
                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(recItem.invoice_date).ToString("dd/MM/yyyy") + ", ";
                    }

                    itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                    itemRec.received_number = itemReceive.received_number;
                    itemRec.received_status = itemReceive.received_status;
                    itemRec.received_type = itemReceive.received_type;
                    itemRec.ref_id = itemReceive.ref_id;
                    itemRec.ref_number = ref_number;
                    itemRec.created_date = itemReceive.created_date;
                    //itemRec.received_status = itemReceive.received_status;
                    itemRec.supplier = supplier;
                    itemRec.invoiceDate = invoiceDate;
                    itemRec.invoiceNumber = invoiceNumber;
                    itemRec.is_received_partial = itemReceive.is_received_partial;
                    itemRec.created_date_text = Convert.ToDateTime(itemReceive.created_date).ToString("dd/MM/yyyy HH:mm");
                    itemRec.created_by_text = CommonClass.GetUserFullnameByUserId(itemReceive.created_by);
                    itemRec.show_status_html = ShowStatus.GetGRNShowStatusHTML(itemRec.received_status, Convert.ToBoolean(itemRec.is_received_partial));
                    itemReceives.Add(itemRec);
                }
                itemReceives = itemReceives.OrderByDescending(o => o.created_date).ToList();
                return itemReceives;
            }
        }
        private List<ItemReceiveViewModel> GetItemReceiveApprovalListitems(string dateRange)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                string userid = User.Identity.GetUserId();
                List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
                List<ItemReceiveViewModel> itemReceiveList = new List<ItemReceiveViewModel>();

                itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && m.created_date>=startDate && m.created_date<=endDate ).Select(x => new ItemReceiveViewModel()
                {
                    receive_item_voucher_id = x.receive_item_voucher_id,
                    received_number = x.received_number,
                    received_status = x.received_status,
                    received_type = x.received_type,
                    ref_id = x.ref_id,
                    created_date = x.created_date,
                    po_report_number = x.po_report_number,
                    supplier_id = x.supplier_id,
                    is_received_partial = x.is_received_partial,
                    created_by=x.created_by
                }).ToList();
            
                

                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();
                    string ref_number = string.Empty;
                    string supplier = string.Empty;
                    string invoiceNumber = string.Empty;
                    string invoiceDate = string.Empty;

                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                        supplier_id = x.supplier_id,
                        invoice_date = x.invoice_date,
                        invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    {
                        ref_number = db.transferformmainstocks.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                    {
                        ref_number = db.tb_stock_issue_return.Where(m => m.status == true && m.stock_issue_return_id == itemReceive.ref_id).Select(m => m.issue_return_number).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    {
                        ref_number = db.tb_return_main_stock.Where(m => m.status == true && m.return_main_stock_id == itemReceive.ref_id).Select(m => m.return_main_stock_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    {
                        //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                        ref_number = itemReceive.po_report_number;
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, 
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();

                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, sup) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, recItem.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    var duplicateInoviceNumbers = itemDetails.GroupBy(x => x.invoice_number).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInoviceNumbers.Any())
                    {
                        foreach (var inv in duplicateInoviceNumbers)
                            invoiceNumber = invoiceNumber + inv + ", ";
                    }
                    var duplicateInovicedate = itemDetails.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInovicedate.Any())
                    {
                        foreach (var invDate in duplicateInovicedate)
                            invoiceDate = invoiceDate + Convert.ToDateTime(invDate).ToString("dd/MM/yyyy") + ", ";
                    }
                    foreach (var recItem in itemDetails)
                    {
                        bool isDupInvNo = duplicateInoviceNumbers.Where(x => string.Compare(x, recItem.invoice_number) == 0).ToList().Count() > 0 ? true : false;
                        bool isDupInvDate = duplicateInovicedate.Where(x => x == recItem.invoice_date).ToList().Count() > 0 ? true : false;
                        if (!isDupInvNo) invoiceNumber = invoiceNumber + recItem.invoice_number + ", ";
                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(recItem.invoice_date).ToString("dd/MM/yyyy") + ", ";
                    }

                    string userId = User.Identity.GetUserId();
                    if (User.IsInRole(Role.SystemAdmin)  )
                    {
                        itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                        itemRec.received_number = itemReceive.received_number;
                        itemRec.received_status = itemReceive.received_status;
                        itemRec.received_type = itemReceive.received_type;
                        itemRec.ref_id = itemReceive.ref_id;
                        itemRec.ref_number = ref_number;
                        itemRec.created_date = itemReceive.created_date;
                        itemRec.created_by = itemReceive.created_by;
                        //itemRec.received_status = itemReceive.received_status;
                        itemRec.supplier = supplier;
                        itemRec.invoiceDate = invoiceDate;
                        itemRec.invoiceNumber = invoiceNumber;
                        itemRec.is_received_partial = itemReceive.is_received_partial;
                        itemRec.created_date_text = Convert.ToDateTime(itemReceive.created_date).ToString("dd/MM/yyyy HH:mm");
                        itemRec.created_by_text = CommonClass.GetUserFullnameByUserId(itemReceive.created_by);
                        itemRec.show_status_html = ShowStatus.GetGRNShowStatusHTML(itemRec.received_status,Convert.ToBoolean(itemRec.is_received_partial));
                        itemReceives.Add(itemRec);
                    }
                    else
                    {

                        if(string.Compare(itemReceive.received_status,Status.Pending)==0 || string.Compare(itemReceive.received_status, Status.Feedbacked) == 0)
                        {
                            if((string.Compare(itemReceive.received_type, "Transfer Workshop") ==0 && CommonFunctions.isQCQAbyWorkshopTransfer(itemReceive.ref_id, userid))
                                || (string.Compare(itemReceive.received_type, "Stock Transfer") == 0 && CommonFunctions.isQAQCbyStockTransfer(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Return") == 0 && CommonFunctions.isQAQCbyStockReturn(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Purchase Order") == 0 && CommonFunctions.isQAQCbyPurchaseOrder(itemReceive.ref_id, userId)))
                            {
                                itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                                itemRec.received_number = itemReceive.received_number;
                                itemRec.received_status = itemReceive.received_status;
                                itemRec.received_type = itemReceive.received_type;
                                itemRec.ref_id = itemReceive.ref_id;
                                itemRec.ref_number = ref_number;
                                itemRec.created_date = itemReceive.created_date;
                                //itemRec.received_status = itemReceive.received_status;
                                itemRec.created_by = itemReceive.created_by;
                                itemRec.supplier = supplier;
                                itemRec.invoiceDate = invoiceDate;
                                itemRec.invoiceNumber = invoiceNumber;
                                itemRec.is_received_partial = itemReceive.is_received_partial;
                                itemRec.created_date_text = Convert.ToDateTime(itemReceive.created_date).ToString("dd/MM/yyyy HH:mm");
                                itemRec.created_by_text = CommonClass.GetUserFullnameByUserId(itemReceive.created_by);
                                itemRec.show_status_html = ShowStatus.GetGRNShowStatusHTML(itemRec.received_status, Convert.ToBoolean(itemRec.is_received_partial));
                                itemReceives.Add(itemRec);
                            }
                        }

                        if (string.Compare(itemReceive.received_status, Status.Approved) == 0)
                        {
                            if ((string.Compare(itemReceive.received_type, "Purchase Order") == 0 && CommonFunctions.isSMinSitebyPurchaseRequisition(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Transfer") == 0 && CommonFunctions.isSMinSitebyStockTransfer(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Stock Return") == 0 && CommonFunctions.isSMinSitebyStockReturn(itemReceive.ref_id, userId))
                            || (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0 && CommonFunctions.isSMinSitebyWorkShopTransfer(itemReceive.ref_id, userId)))
                            {
                                itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                                itemRec.received_number = itemReceive.received_number;
                                itemRec.received_status = itemReceive.received_status;
                                itemRec.received_type = itemReceive.received_type;
                                itemRec.ref_id = itemReceive.ref_id;
                                itemRec.ref_number = ref_number;
                                itemRec.created_date = itemReceive.created_date;
                                itemRec.created_by = itemReceive.created_by;
                                //itemRec.received_status = itemReceive.received_status;
                                itemRec.supplier = supplier;
                                itemRec.invoiceDate = invoiceDate;
                                itemRec.invoiceNumber = invoiceNumber;
                                itemRec.is_received_partial = itemReceive.is_received_partial;
                                itemRec.created_date_text = Convert.ToDateTime(itemReceive.created_date).ToString("dd/MM/yyyy HH:mm");
                                itemRec.created_by_text = CommonClass.GetUserFullnameByUserId(itemReceive.created_by);
                                itemRec.show_status_html = ShowStatus.GetGRNShowStatusHTML(itemRec.received_status, Convert.ToBoolean(itemRec.is_received_partial));
                                itemReceives.Add(itemRec);
                            }
                        }
                        
                    }
                }
                itemReceives = itemReceives.OrderByDescending(o => o.created_date).ToList();
                return itemReceives;
            }
        }
        #endregion
        public ActionResult GetItemReceivedByRefId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var result = GetListItemReceivedByRefId(id);
                return Json(new { result = "success", data = result }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<ItemReceiveHistoryViewModel> GetListItemReceivedByRefId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from rec in db.tb_receive_item_voucher
                        join de in db.tb_received_item_detail on rec.receive_item_voucher_id equals de.ri_ref_id
                        where rec.ref_id == id && rec.status == true
                        orderby de.invoice_date
                        select new ItemReceiveHistoryViewModel
                        {
                            PurchaseOrderId = rec.ref_id,
                            ItemId = de.ri_item_id,
                            Quantity = de.quantity ?? 0m
                        }).ToList();
            }
        }
        public ActionResult List()
        {
            //return View(this.GetGoodReceiveNoteListItems());
            return View();
        }
        public List<ItemReceiveViewModel> GetGoodReceiveNoteListItems()
        {
            kim_mexEntities db = new kim_mexEntities();
            List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
            //itemReceives = Inventory.GetReceiveItemsList();
            IQueryable<ItemReceiveViewModel> itemReceiveList;
            itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true).Select(x => new ItemReceiveViewModel() { receive_item_voucher_id = x.receive_item_voucher_id, received_number = x.received_number, received_status = x.received_status, received_type = x.received_type, ref_id = x.ref_id, created_date = x.created_date, po_report_number = x.po_report_number, supplier_id = x.supplier_id, created_by = x.created_by,is_received_partial=x.is_received_partial });
            if (itemReceiveList.Count() > 0)
            {
                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();
                    string ref_number = string.Empty;
                    string supplier = string.Empty;
                    string invoiceNumber = string.Empty;
                    string invoiceDate = string.Empty;
                    string warehouseId = string.Empty;

                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        warehouseId = (from tr in db.tb_stock_transfer_voucher
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    {
                        ref_number = db.transferformmainstocks.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        warehouseId = (from tr in db.transferformmainstocks
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                    {
                        ref_number = db.tb_stock_issue_return.Where(m => m.status == true && m.stock_issue_return_id == itemReceive.ref_id).Select(m => m.issue_return_number).FirstOrDefault();
                        warehouseId = (from re in db.tb_stock_issue_return
                                       join tr in db.tb_stock_transfer_voucher on re.stock_issue_ref equals tr.stock_transfer_id
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    {
                        ref_number = db.tb_return_main_stock.Where(m => m.status == true && m.return_main_stock_id == itemReceive.ref_id).Select(m => m.return_main_stock_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    {
                        //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                        ref_number = itemReceive.po_report_number;
                        warehouseId = (from po in db.tb_purchase_request
                                       join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                                       join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                       join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();

                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, sup) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, recItem.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    var duplicateInoviceNumbers = itemDetails.GroupBy(x => x.invoice_number).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInoviceNumbers.Any())
                    {
                        foreach (var inv in duplicateInoviceNumbers)
                            invoiceNumber = invoiceNumber + inv + ", ";
                    }
                    var duplicateInovicedate = itemDetails.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInovicedate.Any())
                    {
                        foreach (var invDate in duplicateInovicedate)
                            invoiceDate = invoiceDate + Convert.ToDateTime(invDate).ToString("dd/MM/yyyy") + ", ";
                    }
                    foreach (var recItem in itemDetails)
                    {
                        bool isDupInvNo = duplicateInoviceNumbers.Where(x => string.Compare(x, recItem.invoice_number) == 0).ToList().Count() > 0 ? true : false;
                        bool isDupInvDate = duplicateInovicedate.Where(x => x == recItem.invoice_date).ToList().Count() > 0 ? true : false;
                        if (!isDupInvNo) invoiceNumber = invoiceNumber + recItem.invoice_number + ", ";
                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(recItem.invoice_date).ToString("dd/MM/yyyy") + ", ";
                    }



                    itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                    itemRec.received_number = itemReceive.received_number;
                    itemRec.received_status = itemReceive.received_status;
                    itemRec.received_type = itemReceive.received_type;
                    itemRec.ref_id = itemReceive.ref_id;
                    itemRec.ref_number = ref_number;
                    itemRec.created_date = itemReceive.created_date;
                    //itemRec.received_status = itemReceive.received_status;
                    itemRec.supplier = supplier;
                    itemRec.invoiceDate = invoiceDate;
                    itemRec.invoiceNumber = invoiceNumber;
                    itemRec.created_by = itemReceive.created_by;
                    itemRec.warehouse_id = warehouseId;
                    itemRec.is_received_partial = itemReceive.is_received_partial;
                    itemReceives.Add(itemRec);
                }
                string userid = User.Identity.GetUserId();

                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.OperationDirector))
                {

                }else
                {
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_stock_keeper_warehouse
                                            join wh in db.tb_warehouse on keeper.warehouse_id equals wh.warehouse_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.stock_keeper, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                                itemReceives.Add(obj);
                        }
                    }

                    if (User.IsInRole(Role.QAQCOfficer))
                    {

                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_warehouse_qaqc
                                            join wh in db.tb_warehouse on keeper.warehouse_id equals wh.warehouse_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.qaqc_id, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_site_manager_project
                                            join wh in db.tb_warehouse on keeper.project_id equals wh.warehouse_project_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_manager, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }

                    if (User.IsInRole(Role.ProjectManager))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_project_pm
                                            join wh in db.tb_warehouse on keeper.project_id equals wh.warehouse_project_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.project_manager_id, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }

                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tbSiteSiteSupervisors
                                                //join site in db.tb_site on keeper.site_id equals site.site_id
                                            join pro in db.tb_project on keeper.site_id equals pro.project_id
                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_supervisor_id, userid) == 0
                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_site_site_admin
                                                //join site in db.tb_site on keeper.site_id equals site.site_id
                                            join pro in db.tb_project on keeper.site_id equals pro.project_id
                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_admin_id, userid) == 0
                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                }
            
            }


            return itemReceives;
        }
        public List<ItemReceiveViewModel> GetGoodReceiveNoteListItemsByDateRange(string dateRange)
        {
            string[] splitDateRanges = dateRange.Split('-');
            DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
            DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
            kim_mexEntities db = new kim_mexEntities();
            List<ItemReceiveViewModel> itemReceives = new List<ItemReceiveViewModel>();
            //itemReceives = Inventory.GetReceiveItemsList();
            IQueryable<ItemReceiveViewModel> itemReceiveList;
            itemReceiveList = db.tb_receive_item_voucher.Where(m => m.status == true && m.created_date>=startDate && m.created_date<=endDate)
                .Select(x => new ItemReceiveViewModel() { 
                    receive_item_voucher_id = x.receive_item_voucher_id, 
                    received_number = x.received_number, 
                    received_status = x.received_status,
                    received_type = x.received_type, 
                    ref_id = x.ref_id, 
                    created_date = x.created_date, 
                    po_report_number = x.po_report_number, 
                    supplier_id = x.supplier_id,
                    created_by = x.created_by, 
                    is_received_partial = x.is_received_partial,
                    sending_date=x.sending_date,
                    returning_date=x.returning_date,
                });
            if (itemReceiveList.Count() > 0)
            {
                foreach (var itemReceive in itemReceiveList)
                {
                    ItemReceiveViewModel itemRec = new ItemReceiveViewModel();
                    List<Models.ItemReceivedDetailViewModel> itemDetails = new List<ItemReceivedDetailViewModel>();
                    string ref_number = string.Empty;
                    string supplier = string.Empty;
                    string invoiceNumber = string.Empty;
                    string invoiceDate = string.Empty;
                    string warehouseId = string.Empty;

                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                    {
                        ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        warehouseId = (from tr in db.tb_stock_transfer_voucher
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                    {
                        ref_number = db.transferformmainstocks.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                        warehouseId = (from tr in db.transferformmainstocks
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                    {
                        ref_number = db.tb_stock_issue_return.Where(m => m.status == true && m.stock_issue_return_id == itemReceive.ref_id).Select(m => m.issue_return_number).FirstOrDefault();
                        warehouseId = (from re in db.tb_stock_issue_return
                                       join tr in db.tb_stock_transfer_voucher on re.stock_issue_ref equals tr.stock_transfer_id
                                       join mr in db.tb_item_request on tr.item_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(tr.stock_transfer_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                    {
                        ref_number = db.tb_return_main_stock.Where(m => m.status == true && m.return_main_stock_id == itemReceive.ref_id).Select(m => m.return_main_stock_no).FirstOrDefault();
                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id,
                        itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel()
                        {
                            supplier_id = x.supplier_id,
                            invoice_date = x.invoice_date,
                            invoice_number = x.invoice_number
                        }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, sup) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_warehouse.Where(m => string.Compare(m.warehouse_id, recItem.supplier_id) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                    {
                        //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                        ref_number = itemReceive.po_report_number;
                        warehouseId = (from po in db.tb_purchase_request
                                       join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                                       join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                       join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                       join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                       where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                       select wh.warehouse_id).FirstOrDefault();

                        itemDetails = db.tb_received_item_detail.Where(x => string.Compare(x.ri_ref_id, itemReceive.receive_item_voucher_id) == 0).Select(x => new ItemReceivedDetailViewModel() { supplier_id = x.supplier_id, invoice_date = x.invoice_date, invoice_number = x.invoice_number }).ToList();
                        var duplicateSupplier = itemDetails.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateSupplier.Any())
                        {
                            foreach (var sup in duplicateSupplier)
                                supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, sup) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                        foreach (var recItem in itemDetails)
                        {
                            bool isDupSupplier = duplicateSupplier.Where(x => string.Compare(x, recItem.supplier_id) == 0).ToList().Count() > 0 ? true : false;
                            if (!isDupSupplier) supplier = supplier + db.tb_supplier.Where(m => string.Compare(m.supplier_id, recItem.supplier_id) == 0).Select(x => x.supplier_name).FirstOrDefault().ToString() + ", ";
                        }
                    }
                    var duplicateInoviceNumbers = itemDetails.GroupBy(x => x.invoice_number).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInoviceNumbers.Any())
                    {
                        foreach (var inv in duplicateInoviceNumbers)
                            invoiceNumber = invoiceNumber + inv + ", ";
                    }
                    var duplicateInovicedate = itemDetails.GroupBy(x => x.invoice_date).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicateInovicedate.Any())
                    {
                        foreach (var invDate in duplicateInovicedate)
                            invoiceDate = invoiceDate + Convert.ToDateTime(invDate).ToString("dd/MM/yyyy") + ", ";
                    }
                    foreach (var recItem in itemDetails)
                    {
                        bool isDupInvNo = duplicateInoviceNumbers.Where(x => string.Compare(x, recItem.invoice_number) == 0).ToList().Count() > 0 ? true : false;
                        bool isDupInvDate = duplicateInovicedate.Where(x => x == recItem.invoice_date).ToList().Count() > 0 ? true : false;
                        if (!isDupInvNo) invoiceNumber = invoiceNumber + recItem.invoice_number + ", ";
                        if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(recItem.invoice_date).ToString("dd/MM/yyyy") + ", ";
                    }

                    itemRec.receive_item_voucher_id = itemReceive.receive_item_voucher_id;
                    itemRec.received_number = itemReceive.received_number;
                    itemRec.received_status = itemReceive.received_status;
                    itemRec.received_type = itemReceive.received_type;
                    itemRec.ref_id = itemReceive.ref_id;
                    itemRec.ref_number = ref_number;
                    itemRec.created_date = itemReceive.created_date;
                    //itemRec.received_status = itemReceive.received_status;
                    itemRec.supplier = supplier;
                    itemRec.invoiceDate = invoiceDate;
                    itemRec.invoiceNumber = invoiceNumber;
                    itemRec.created_by = itemReceive.created_by;
                    itemRec.warehouse_id = warehouseId;
                    itemRec.is_received_partial = itemReceive.is_received_partial;
                    itemRec.created_date_text = CommonClass.ToLocalTime(Convert.ToDateTime(itemRec.created_date)).ToString("dd/MM/yyyy");
                    itemRec.created_by_text = BT_KimMex.Class.CommonFunctions.GetUserFullnamebyUserId(itemRec.created_by);
                    itemRec.sending_date = itemReceive.sending_date;
                    itemRec.returning_date = itemReceive.returning_date;
                    itemRec.sending_date_str = itemReceive.sending_date.HasValue ? CommonClass.ToLocalTime(Convert.ToDateTime(itemRec.sending_date)).ToString("dd/MM/yyyy") : string.Empty;
                    itemRec.returning_date_str= itemReceive.returning_date.HasValue ? CommonClass.ToLocalTime(Convert.ToDateTime(itemRec.returning_date)).ToString("dd/MM/yyyy") : string.Empty;
                    itemReceives.Add(itemRec);
                }
                string userid = User.Identity.GetUserId();

                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.OperationDirector))
                {

                }
                else
                {
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_stock_keeper_warehouse
                                            join wh in db.tb_warehouse on keeper.warehouse_id equals wh.warehouse_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.stock_keeper, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                                itemReceives.Add(obj);
                        }
                    }

                    if (User.IsInRole(Role.QAQCOfficer))
                    {

                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_warehouse_qaqc
                                            join wh in db.tb_warehouse on keeper.warehouse_id equals wh.warehouse_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.qaqc_id, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_site_manager_project
                                            join wh in db.tb_warehouse on keeper.project_id equals wh.warehouse_project_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_manager, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }

                    if (User.IsInRole(Role.ProjectManager))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_project_pm
                                            join wh in db.tb_warehouse on keeper.project_id equals wh.warehouse_project_id
                                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.project_manager_id, userid) == 0

                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }

                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tbSiteSiteSupervisors
                                                //join site in db.tb_site on keeper.site_id equals site.site_id
                                            join pro in db.tb_project on keeper.site_id equals pro.project_id
                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_supervisor_id, userid) == 0
                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        List<ItemReceiveViewModel> models = new List<ItemReceiveViewModel>();
                        models = itemReceives;
                        itemReceives = new List<ItemReceiveViewModel>();
                        var siteStockWhs = (from keeper in db.tb_site_site_admin
                                                //join site in db.tb_site on keeper.site_id equals site.site_id
                                            join pro in db.tb_project on keeper.site_id equals pro.project_id
                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                            where pro.project_status == true && wh.warehouse_status == true && string.Compare(keeper.site_admin_id, userid) == 0
                                            select wh).ToList();
                        foreach (var wh in siteStockWhs)
                        {
                            var objs = models.Where(s => string.Compare(s.warehouse_id, wh.warehouse_id) == 0).ToList();
                            foreach (var obj in objs)
                            {
                                var isexist = itemReceives.Where(s => string.Compare(s.receive_item_voucher_id, obj.receive_item_voucher_id) == 0).FirstOrDefault();
                                if (isexist == null)
                                    itemReceives.Add(obj);
                            }

                        }
                    }
                }

            }


            return itemReceives;
        }

        public ActionResult GetGoodReceiveNoteListItemsByDateRangeAJAX(string dateRange)
        {
            return Json(new { data = this.GetGoodReceiveNoteListItemsByDateRange(dateRange) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGRNApprovalListItemsByDateRangeAJAX(string dateRange)
        {
            return Json(new { data = this.GetItemReceiveApprovalListitems(dateRange) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGRNMyRequestListItemsByDateRangeAJAX(string dateRange)
        {
            return Json(new { data = this.GetItemReceiveRequestListitems(dateRange) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExporttoExcel(string id,string type,bool is_grn=false,bool is_do=false,bool is_signature=false)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            ItemReceiveViewModel itemReceive = new ItemReceiveViewModel();
            itemReceive = Inventory.GetItemReceivedViewDetail(id);
            itemReceive.histories = GetListItemReceivedByRefId(itemReceive.ref_id);
            itemReceive.process_type = type;
            itemReceive.is_export_grn = is_grn;
            itemReceive.is_export_do = is_do;
            itemReceive.is_export_signature=is_signature;
            
            return View(itemReceive);
        }

        public ActionResult CheckPurchaseOrderReportbyPOReportId(string grnId)
        {
            AJAXResultModel result = new AJAXResultModel();
            try
            {
                var poReportId = string.Empty;
                kim_mexEntities db = new kim_mexEntities();
                var grn = db.tb_receive_item_voucher.Find(grnId);
                tb_po_report poReport = db.tb_po_report.Where(s => string.Compare(s.po_report_number, grn.po_report_number) == 0).FirstOrDefault();
                if (poReport != null)
                {
                    var poItems = (from purd in db.tb_purchase_order_detail
                                   join purs in db.tb_po_supplier on purd.po_detail_id equals purs.po_detail_id
                                   where string.Compare(purd.purchase_order_id, poReport.po_ref_id) == 0 && string.Compare(purd.item_status, "approved") == 0
                                   && purs.is_selected == true && string.Compare(purs.supplier_id, poReport.po_supplier_id) == 0 && purd.item_vat == poReport.is_lpo
                                   select new { purd, purs }).ToList();
                    bool isPOCompleteReceived = poItems.Sum(s => s.purd.remain_quantity) > 0 ? false : true;
                    poReport.is_completed = isPOCompleteReceived;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.Message;
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
} 