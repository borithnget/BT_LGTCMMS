using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using BT_KimMex.Class;
using Microsoft.Ajax.Utilities;


namespace BT_KimMex.Controllers
{
    public class TransferFromMainStockController : Controller
    {
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        // GET: StockTransfer
        public ActionResult Index()
        {
            //return View(this.GetTransferFromWorkshopListItems());
            return View();
        }
        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Create()
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            itemRequests = this.GetItemRequestDropdownList();
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList();

            ViewBag.STDate = Class.CommonClass.ToLocalTime(DateTime.Now).ToString("dd/MM/yyyy");
            ViewBag.STID = Class.CommonClass.GenerateProcessNumber("TW");
            ViewBag.IRID = new SelectList(itemRequestss, "ir_id", "ir_no");
            return View();
        }
        public ActionResult CreateTransferFromMainStock(TransferFromMainStockViewModel model, List<InventoryViewModel> inventories)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countInvalid = 0;
                string message = string.Empty;
                List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                //remainRequestItems = Class.ItemRequest.GetAllAvailableItem(model.item_request_id);
                remainRequestItems = TransferFromMainStockViewModel.GetAllAvailableMaterailRequestItembyTransferWorkshop(model.item_request_id);

                #region check reamin transfer balance
                foreach (Models.ItemRequestDetail2ViewModel item in remainRequestItems)
                {
                    foreach (InventoryViewModel transfer in inventories)
                    {
                        if (item.ir_item_id == transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                        {
                            countInvalid++;
                            if (item.remain_qty < 0)
                                message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                            else
                                message = message + string.Format("{0} remain quantity {1} to transfer.\n", item.product_name, string.Format("{0:G29}", Double.Parse(item.remain_qty.ToString())));
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                #endregion
                #region check with total transfer quantity
                decimal requestQty = 0, totalTransferQty = 0;
                var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                foreach (var Id in dupId)
                {
                    totalTransferQty = 0;
                    requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                    var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                    foreach (var tQty in transferQty)
                    {
                        totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                    }
                    if (totalTransferQty > requestQty)
                    {
                        return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                transferformmainstock sTransfer = new transferformmainstock();
                sTransfer.stock_transfer_id = Guid.NewGuid().ToString();
                sTransfer.stock_transfer_no = Class.CommonClass.GenerateProcessNumber("TW");
                sTransfer.item_request_id = model.item_request_id;
                sTransfer.stock_transfer_status = Status.Pending;
                sTransfer.status = true;
                sTransfer.created_by = User.Identity.GetUserId();
                sTransfer.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                sTransfer.is_completed = false;
                sTransfer.is_receive_completed = false;
                db.transferformmainstocks.Add(sTransfer);
                db.SaveChanges();

                tb_item_request mr = db.tb_item_request.Find(sTransfer.item_request_id);
                mr.tw_status = ShowStatus.TWCreated;
                db.SaveChanges();

                Class.CommonClass.AutoGenerateStockInvoiceNumber(sTransfer.stock_transfer_id, inventories);
                foreach (var inv in inventories)
                {
                    #region updated when have workflow
                    #endregion

                    

                    tb_transfer_frommain_stock_detail stDetail = new tb_transfer_frommain_stock_detail();
                    stDetail.st_detail_id = Guid.NewGuid().ToString();
                    stDetail.st_ref_id = sTransfer.stock_transfer_id;
                    stDetail.st_item_id = inv.product_id;
                    stDetail.status = Convert.ToBoolean(inv.status);
                    stDetail.st_warehouse_id = inv.warehouse_id;
                    stDetail.quantity = inv.out_quantity;
                    stDetail.unit = inv.unit;
                    stDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.inventory_date;
                    stDetail.invoice_number = Class.CommonClass.GetInvoiceNumber(sTransfer.stock_transfer_id, stDetail.st_warehouse_id, stDetail.invoice_date); 
                    stDetail.remain_quantity = Class.CommonClass.ConvertMultipleUnit(stDetail.st_item_id, stDetail.unit, Convert.ToDecimal(inv.out_quantity));
                    stDetail.received_remain_quantity = Class.CommonClass.ConvertMultipleUnit(stDetail.st_item_id, stDetail.unit, Convert.ToDecimal(inv.out_quantity));
                    stDetail.item_status = "pending";
                    db.tb_transfer_frommain_stock_detail.Add(stDetail);
                    db.SaveChanges();
                }

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), sTransfer.stock_transfer_id, sTransfer.stock_transfer_status, sTransfer.created_by, sTransfer.created_date, string.Empty);
                Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock(model.item_request_id, sTransfer.stock_transfer_id, false);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Detail(string id)
        {
            TransferFromMainStockViewModel sTransfer = new TransferFromMainStockViewModel();
            sTransfer = this.GetTransferFromMainStockDetail(id);
            return View(sTransfer);
        }
        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Edit(string id)
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            TransferFromMainStockViewModel sTransfer = new TransferFromMainStockViewModel();
            sTransfer = this.GetTransferFromMainStockDetail(id);
            itemRequests = this.GetItemRequestDropdownList();
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList(sTransfer.item_request_id);
            ViewBag.IRID = itemRequestss;
            return View(sTransfer);
        }
        public ActionResult EditTransferFromMainStock(TransferFromMainStockViewModel model, List<InventoryViewModel> inventories)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                int countInvalid = 0;
                string message = string.Empty;
                List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                remainRequestItems = Class.ItemRequest.GetAllAvailableItembyTransferFromMainStock(model.item_request_id, model.stock_transfer_id);
                #region check reamin transfer balance
                foreach (ItemRequestDetail2ViewModel item in remainRequestItems)
                {
                    foreach (InventoryViewModel transfer in inventories)
                    {
                        if (item.ir_item_id == transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                        {
                            countInvalid++;
                            if (item.remain_qty < 0)
                                message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                            else
                                message = message + string.Format("{0} is remain quantity {1} to transfer.", item.product_name, string.Format("{0:G29}", Double.Parse(item.remain_qty.ToString())));
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                #endregion
                #region check with total transfer quantity
                decimal requestQty = 0, totalTransferQty = 0;
                var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                foreach (var Id in dupId)
                {
                    totalTransferQty = 0;
                    requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                    var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                    foreach (var tQty in transferQty)
                    {
                        totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                    }
                    if (totalTransferQty > requestQty)
                    {
                        return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                #region update inventory
                if (!string.IsNullOrEmpty(model.stock_transfer_id))
                {
                    string transferFromMainStockID = model.stock_transfer_id;
                    Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock(model.item_request_id, model.stock_transfer_id, true);
                    this.DeleteTransferFromMainStockDetail(transferFromMainStockID);
                    Class.CommonClass.DeleteInvoiceNumber(transferFromMainStockID);
                    Class.CommonClass.AutoGenerateStockInvoiceNumber(transferFromMainStockID ,inventories);
                }
                #endregion


                transferformmainstock sTransfer = db.transferformmainstocks.Where(m => m.stock_transfer_id == model.stock_transfer_id).FirstOrDefault();


                sTransfer.stock_transfer_no = model.stock_transfer_no;
                sTransfer.item_request_id = model.item_request_id;
                sTransfer.stock_transfer_status = "Pending";
                sTransfer.status = true;
                sTransfer.updated_by = User.Identity.GetUserId();
                sTransfer.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                foreach (var inv in inventories)
                {

                    tb_transfer_frommain_stock_detail stDetail = new tb_transfer_frommain_stock_detail();
                    if (inv.status == "true")
                    {
                        inv.status = "true";
                    }
                    else
                    {
                        inv.status = "false";
                    }

                    stDetail.st_detail_id = Guid.NewGuid().ToString();
                    stDetail.st_ref_id = sTransfer.stock_transfer_id;
                    stDetail.st_item_id = inv.product_id;
                    stDetail.st_warehouse_id = inv.warehouse_id;
                    stDetail.quantity = inv.out_quantity;
                    stDetail.unit = inv.unit;
                    stDetail.status = Convert.ToBoolean(inv.status);
                    stDetail.invoice_date = inv.invoice_date != null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.inventory_date;
                    stDetail.invoice_number = Class.CommonClass.GetInvoiceNumber(sTransfer.stock_transfer_id, stDetail.st_warehouse_id, stDetail.invoice_date); 
                    stDetail.item_status = "pending";
                    db.tb_transfer_frommain_stock_detail.Add(stDetail);
                    db.SaveChanges();
                }
                Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock(sTransfer.item_request_id, sTransfer.stock_transfer_id, false);
            }
            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    transferformmainstock transferStock = db.transferformmainstocks.Find(id);
                    transferStock.status = false;
                    transferStock.updated_by = User.Identity.Name;
                    transferStock.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Approve(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int countInvalid = 0;
                    string message = string.Empty;
                    List<InventoryViewModel> remainRequestItems = new List<InventoryViewModel>();
                    remainRequestItems = this.getRemainItems(id);
                    var inventories = db.tb_stock_transfer_detail.Where(m => m.st_ref_id == id).ToList();
                    #region check reamin transfer balance
                    foreach (InventoryViewModel item in remainRequestItems)
                    {
                        foreach (var transfer in inventories)
                        {
                            if (item.product_id == transfer.st_item_id && item.total_quantity < Class.CommonClass.ConvertMultipleUnit(transfer.st_item_id, transfer.unit, Convert.ToDecimal(transfer.quantity)))
                            {
                                countInvalid++;
                                if (item.total_quantity < 0)
                                    message = message + string.Format("{0} is completed to tranfer.\n", item.itemName);
                                else
                                    message = message + string.Format("{0} is remain quantity {1} to transfer.", item.itemName, string.Format("{0:G29}", Double.Parse(item.total_quantity.ToString())));
                                break;
                            }
                        }
                    }
                    if (countInvalid > 0)
                        return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                    #endregion

                    transferformmainstock transferStock = db.transferformmainstocks.Find(id);
                    transferStock.stock_transfer_status = "Approved";
                    transferStock.approved_by = User.Identity.GetUserId();
                    transferStock.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    InsertItemInventory(id);
                    string irId = transferStock.item_request_id;
                    updateItemRequestStatus(transferStock.item_request_id, transferStock.stock_transfer_id);

                    tb_item_request mr = db.tb_item_request.Find(irId);
                    mr.tw_status = ShowStatus.GRNPending;
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Reject(string id, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    transferformmainstock transferStock = db.transferformmainstocks.Find(id);
                    transferStock.stock_transfer_status = "Rejected";
                    transferStock.approved_by = User.Identity.Name;
                    transferStock.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = transferStock.stock_transfer_id;
                    reject.ref_type = "Transfer From Main Stock";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]

        public ActionResult Approval(string id, string status, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                transferformmainstock transfer = db.transferformmainstocks.Find(id);
                transfer.stock_transfer_status = String.Compare(status, Status.Approved) ==0? Status.Checked : Status.CheckRejected;
                transfer.checked_by = User.Identity.GetUserId();
                transfer.checked_date = DateTime.Now;
                transfer.checked_comment = comment;
                db.SaveChanges();

                if (string.Compare(status, Status.Rejected) == 0)
                {
                    Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock(transfer.item_request_id, transfer.stock_transfer_id, true);
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = transfer.stock_transfer_id;
                    reject.ref_type = "Work Order Return";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Completed(string id, string status, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                transferformmainstock transfer = db.transferformmainstocks.Find(id);
                transfer.stock_transfer_status = string.Compare(status, Status.Approved) == 0 ? Status.Completed : Status.Rejected;
                transfer.completed_by = User.Identity.GetUserId();
                transfer.completed_date = DateTime.Now;
                transfer.completed_comment = comment;
                db.SaveChanges();

                if (string.Compare(transfer.stock_transfer_status, Status.Completed) == 0)
                {
                    //insert to inventory here

                    InsertItemInventory(id);

                }
                else
                {
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = transfer.stock_transfer_id;
                    reject.ref_type = "Work Order Return";
                    reject.comment = comment;
                    reject.rejected_by = User.Identity.GetUserId();
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            TransferFromMainStockViewModel model = new TransferFromMainStockViewModel();
            model = this.GetTransferFromMainStockDetail(id);
            return View(model);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult ApproveFeedback(string id, List<Models.InventoryViewModel> models)
        {
            try
            {
                int countItemApproved = 0;
                Entities.kim_mexEntities db = new kim_mexEntities();
                foreach (InventoryViewModel item in models)
                {
                    string idd = item.inventory_id;
                    Entities.tb_transfer_frommain_stock_detail transferDetail = db.tb_transfer_frommain_stock_detail.Find(idd);
                    transferDetail.item_status = item.item_status;
                    transferDetail.remark = item.remark;
                    transferDetail.invoice_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    transferDetail.remain_quantity = transferDetail.quantity;
                    db.SaveChanges();
                    if (string.Compare(transferDetail.item_status, "approved") == 0)
                    {
                        countItemApproved++;

                        decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == transferDetail.st_item_id && m.warehouse_id == "1").Select(m => m.total_quantity).FirstOrDefault());
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = id;
                        inventory.inventory_status_id = "6";
                        inventory.warehouse_id = "1";
                        inventory.product_id = transferDetail.st_item_id;
                        inventory.out_quantity = Class.CommonClass.ConvertMultipleUnit1(transferDetail.st_item_id, transferDetail.unit, Convert.ToDecimal(transferDetail.quantity));
                        inventory.in_quantity = 0;
                        inventory.total_quantity = totalQty - inventory.out_quantity;
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }
                }

                transferformmainstock transferStock = db.transferformmainstocks.Find(id);
                transferStock.stock_transfer_status = countItemApproved == models.Count() ? Status.Completed : Status.PendingFeedback;
                transferStock.approved_by = User.Identity.GetUserId();
                transferStock.approved_date = DateTime.Now;
                db.SaveChanges();

                tb_item_request mr = db.tb_item_request.Find(transferStock.item_request_id);
                mr.tw_status = ShowStatus.GRNPending;
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), transferStock.stock_transfer_id, transferStock.stock_transfer_status, transferStock.approved_by, transferStock.approved_date, string.Empty);

                //if (transferStock.stock_transfer_status == "Pending" || transferStock.stock_transfer_status == "Feedbacked")
                //{
                //    var pendingItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "pending") == 0).ToList();
                //    if (pendingItems.Count() == 0)
                //    {
                //        var totalItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0).ToList();
                //        var approvedItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "approved") == 0).ToList();
                //        if (totalItems.Count() == approvedItems.Count())
                //            transferStock.stock_transfer_status = "Complete";
                //        else
                //            transferStock.stock_transfer_status = "Pending Feedback";
                //    }
                //    transferStock.approved_by = User.Identity.GetUserId();
                //    transferStock.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //    db.SaveChanges();
                //}
                //else if (transferStock.stock_transfer_status == "Complete")
                //{
                //    var pendingItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "pending") == 0).ToList();
                //    if (pendingItems.Count() == 0)
                //    {
                //        var totalItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0).ToList();
                //        var approvedItems = db.tb_transfer_frommain_stock_detail.Where(s => string.Compare(s.st_ref_id, id) == 0 && string.Compare(s.item_status, "approved") == 0).ToList();
                //        if (totalItems.Count() == approvedItems.Count())
                //            transferStock.stock_transfer_status = "Completed";
                //        else
                //            transferStock.stock_transfer_status = "Pending Feedback";
                //    }
                //    transferStock.approved_by = User.Identity.GetUserId();
                //    transferStock.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //    db.SaveChanges();

                //}

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "Admin,Main Stock Controller,Purchaser")]
        public ActionResult PrepareFeedback(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            TransferFromMainStockViewModel model = new TransferFromMainStockViewModel();
            model = this.GetTransferFromMainStockDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id, List<InventoryViewModel> inventories)
        {
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    int countInvalid = 0;
                    string message = string.Empty;
                    transferformmainstock model = db.transferformmainstocks.Find(id);
                    List<Models.ItemRequestDetail2ViewModel> remainRequestItems = new List<ItemRequestDetail2ViewModel>();
                    remainRequestItems = Class.ItemRequest.GetAllAvailableItembyTransferFromMainStock(model.item_request_id, id);
                    #region check reamin transfer balance
                    foreach (ItemRequestDetail2ViewModel item in remainRequestItems)
                    {
                        foreach (InventoryViewModel transfer in inventories)
                        {
                            if (item.ir_item_id == transfer.product_id && item.remain_qty < Class.CommonClass.ConvertMultipleUnit(transfer.product_id, transfer.unit, Convert.ToDecimal(transfer.out_quantity)))
                            {
                                countInvalid++;
                                if (item.remain_qty < 0)
                                    message = message + string.Format("{0} is completed to tranfer.\n", item.product_name);
                                else
                                    message = message + string.Format("{0} is remain quantity {1} to transfer.", item.product_name, string.Format("{0:G29}", Double.Parse(item.remain_qty.ToString())));
                                break;
                            }
                        }
                    }
                    if (countInvalid > 0)
                        return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
                    #endregion
                    #region check with total transfer quantity
                    decimal requestQty = 0, totalTransferQty = 0;
                    var dupId = inventories.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    foreach (var Id in dupId)
                    {
                        totalTransferQty = 0;
                        requestQty = Convert.ToDecimal(remainRequestItems.Where(m => m.ir_item_id == Id).Select(m => m.remain_qty).FirstOrDefault());
                        var transferQty = inventories.Where(x => x.product_id == Id).ToList();
                        foreach (var tQty in transferQty)
                        {
                            totalTransferQty = totalTransferQty + Class.CommonClass.ConvertMultipleUnit(tQty.product_id, tQty.unit, Convert.ToDecimal(tQty.out_quantity));
                        }
                        if (totalTransferQty > requestQty)
                        {
                            return Json(new { result = "fail", message = "Total transfer quantity must be smaller than Request quantity." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    #endregion
                    Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock(model.item_request_id, id, true, "feedbacked");
                    var transferFromMainStockDetail = db.tb_transfer_frommain_stock_detail.Where(x => string.Compare(x.st_ref_id, id) == 0 && string.Compare(x.item_status, "feedbacked") == 0).Select(x => x.st_detail_id).ToList();
                    foreach (var idd in transferFromMainStockDetail)
                    {
                        tb_transfer_frommain_stock_detail transferDetails = db.tb_transfer_frommain_stock_detail.Find(idd);
                        db.tb_transfer_frommain_stock_detail.Remove(transferDetails);
                        db.SaveChanges();
                    }
                    model.stock_transfer_status = "Feedbacked";
                    model.updated_by = User.Identity.GetUserId();
                    model.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    foreach (var inv in inventories)
                    {
                        tb_transfer_frommain_stock_detail stDetail = new tb_transfer_frommain_stock_detail();
                        stDetail.st_detail_id = Guid.NewGuid().ToString();
                        stDetail.st_ref_id = id;
                        stDetail.st_item_id = inv.product_id;
                        stDetail.st_warehouse_id = inv.warehouse_id;
                        stDetail.quantity = inv.out_quantity;
                        stDetail.remain_quantity = Class.CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.out_quantity));
                        stDetail.status = Convert.ToBoolean(inv.status);
                        stDetail.unit = inv.unit;
                        stDetail.invoice_date = inv.invoice_date != null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.inventory_date;
                        stDetail.invoice_number = inv.invoice_number; 
                        stDetail.item_status = "pending";
                        db.tb_transfer_frommain_stock_detail.Add(stDetail);
                        db.SaveChanges();
                    }
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), model.stock_transfer_id, model.stock_transfer_status, model.updated_by, model.updated_date, string.Empty);
                    Class.TransferFromMainStock.RollBackItemQuantitybyTransferFromMainStock (model.item_request_id, id, false, "pending");
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransferFromMainStockDataTable(string status)
        {
            List<TransferFromMainStockViewModel> transferStocks = new List<TransferFromMainStockViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferStocks.Add(transferstock);
                        }
                    }
                   
                }
                else
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_stock_transfer_detail
                                                        join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferStocks.Add(transferstock);
                        }
                    }
                }

                return Json(new { data = transferStocks }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransferFromMainStockDataTableForQA(string status)
        {
            List<TransferFromMainStockViewModel> transferStocks = new List<TransferFromMainStockViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending" || st.stock_transfer_status == "Complete"
                                             || st.stock_transfer_status == "Feedbacked" || st.stock_transfer_status == "Pending Feedback"
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                           TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferStocks.Add(transferstock);
                        }
                    }
                }
                else
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferStocks.Add(transferstock);
                        }
                    }
                }

                return Json(new { data = transferStocks }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransferFromMainStockDataTableForPCH(string status)
        {
            List<TransferFromMainStockViewModel> transferstocks = new List<TransferFromMainStockViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending"
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferstocks.Add(transferstock);
                        }
                    }
                   
                }
                else
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Pending" && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferstocks.Add(transferstock);
                        }
                    }
                }

                return Json(new { data = transferstocks }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetTransferFromMainStockDataTableForCFO(string status)
        {
            List<TransferFromMainStockViewModel> transferstocks = new List<TransferFromMainStockViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && st.stock_transfer_status == "Complete" || st.stock_transfer_status == "Completed"
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferstocks.Add(transferstock);
                        }
                    }
                  
                }
                else
                {
                    var allTransferStocks = (from st in db.transferformmainstocks
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             where st.status == true && string.Compare(st.stock_transfer_status, status) == 0
                                             select new { st, ir }).ToList();
                    if (allTransferStocks.Any())
                    {
                        foreach (var item in allTransferStocks)
                        {
                            string warehouse = string.Empty;
                            string invoiceNo = string.Empty;
                            string invoiceDate = string.Empty;
                            var transferFromMainStockDetails = (from std in db.tb_transfer_frommain_stock_detail
                                                                join wh in db.tb_warehouse on std.st_warehouse_id equals wh.warehouse_id
                                                        where string.Compare(std.st_ref_id, item.st.stock_transfer_id) == 0
                                                        select new { warehouseName = wh.warehouse_name, invoiceNo = std.invoice_number, invoiceDate = std.invoice_date }).ToList();
                            var dupwarehouses = transferFromMainStockDetails.GroupBy(x => x.warehouseName).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupwarehouses.Any())
                            {
                                foreach (var wh in dupwarehouses)
                                    warehouse = warehouse + wh + ", ";
                            }
                            var dupinvoiceno = transferFromMainStockDetails.GroupBy(x => x.invoiceNo).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invno in dupinvoiceno)
                                invoiceNo = invoiceNo + invno + ", ";
                            var dupinvoicedate = transferFromMainStockDetails.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var invdate in dupinvoicedate)
                                invoiceDate = invoiceDate + invdate == null ? "" : Convert.ToDateTime(invdate).ToString("dd/MM/yyy") + ", ";

                            foreach (var std in transferFromMainStockDetails)
                            {
                                bool isDupWH = dupwarehouses.Where(x => string.Compare(std.warehouseName, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvNo = dupinvoiceno.Where(x => string.Compare(std.invoiceNo, x) == 0).ToList().Count() > 0 ? true : false;
                                bool isDupInvDate = dupinvoicedate.Where(x => x == std.invoiceDate).ToList().Count() > 0 ? true : false;
                                if (!isDupWH) warehouse = warehouse + std.warehouseName + ", ";
                                if (!isDupInvNo) invoiceNo = invoiceNo + std.invoiceNo + ", ";
                                if (!isDupInvDate) invoiceDate = invoiceDate + std.invoiceDate == null ? "" : Convert.ToDateTime(std.invoiceDate).ToString("dd/MM/yyy") + ", ";
                            }
                            TransferFromMainStockViewModel transferstock = new TransferFromMainStockViewModel();
                            transferstock.stock_transfer_id = item.st.stock_transfer_id;
                            transferstock.stock_transfer_no = item.st.stock_transfer_no;
                            transferstock.item_request_id = item.st.item_request_id;
                            transferstock.item_request_no = item.ir.ir_no;
                            transferstock.created_date = item.st.created_date;
                            transferstock.stock_transfer_status = item.st.stock_transfer_status;
                            transferstock.strWarehouse = warehouse;
                            transferstock.strInvoiceDate = invoiceDate;
                            transferstock.strInvoiceNo = invoiceNo;
                            transferstocks.Add(transferstock);
                        }
                    }
                }

                return Json(new { data = transferstocks }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<tb_item_request> GetItemRequestDropdownList()
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                itemRequests = db.tb_item_request.OrderBy(m => m.ir_no).Where(m => m.status == true && m.ir_status == "Approved").ToList();
            }
            return itemRequests;
        }
        public string GetTransferFromMainStockNo()
        {
            string transferFromMainStockNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.transferformmainstocks orderby tbl.created_date descending select tbl.stock_transfer_no).FirstOrDefault();
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
                transferFromMainStockNo = "ST-" + yy + "-" + mm + "-" + last_no;
            }
            return transferFromMainStockNo;
        }
        public ActionResult GetItemByIRID(string id, string stId = null)
        {
            List<STItemViewModel1> stItems = new List<STItemViewModel1>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<Models.ItemRequestDetail2ViewModel> itemIDs = Class.ItemRequest.GetAllAvailableItembyTransferFromMainStock(id, stId);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true).Select(m => m.warehouse_id).ToList();
                foreach (var it in itemIDs)
                {
                    foreach (var wh in warehouses)
                    {

                        var irItem = (from inv in db.tb_inventory
                                      join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                      join pro in db.tb_product on inv.product_id equals pro.product_id
                                      orderby inv.inventory_date descending
                                      where inv.warehouse_id == wh && inv.product_id == it.ir_item_id
                                      select new { inv, wah, pro }
                               
                                  ).FirstOrDefault();
                        if (irItem != null)
                        {
                            STItemViewModel1 item = new STItemViewModel1();
                            item.itemID = irItem.inv.product_id;
                            item.itemCode = irItem.pro.product_code;
                            item.itemName = irItem.pro.product_name;
                            item.itemUnit = irItem.pro.product_unit;
                            item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                            item.Unit = it.ir_item_unit;//add new
                            item.unitName = db.tb_unit.Find(item.Unit).Name;
                            item.warehouseID = irItem.inv.warehouse_id;
                            item.warehouseName = irItem.wah.warehouse_name;
                            item.stockBalance = irItem.inv.total_quantity;
                            item.requestQty = it.approved_qty;
                            item.requestUnit = it.ir_item_unit;
                            item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                            item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                            item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                            item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                            item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                            item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                            item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                            item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                            item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                            item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                            item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                            stItems.Add(item);
                        }


                    }
                }
                return Json(new { result = "success", data = stItems }, JsonRequestBehavior.AllowGet);
            }
        }
        public TransferFromMainStockViewModel GetTransferFromMainStockDetail(string id)
        {
            TransferFromMainStockViewModel sTransfer = new TransferFromMainStockViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                sTransfer = (from str in db.transferformmainstocks
                             join ir in db.tb_item_request on str.item_request_id equals ir.ir_id
                             join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                             //join site in db.tb_site on pro.site_id equals site.site_id
                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                             where str.stock_transfer_id == id
                             select new TransferFromMainStockViewModel()
                             {
                                 stock_transfer_id = str.stock_transfer_id,
                                 stock_transfer_no = str.stock_transfer_no,
                                 item_request_id = str.item_request_id,
                                 item_request_no = ir.ir_no,
                                 created_date = str.created_date,
                                 stock_transfer_status = str.stock_transfer_status,
                                 project_id=pro.project_id,
                                 project_fullname=pro.project_full_name,
                                 warehouse_id=wh.warehouse_id,
                                 warehouse_name=wh.warehouse_name
                             }).FirstOrDefault();
              
                var inventories = (from inv in db.tb_transfer_frommain_stock_detail
                                   join wah in db.tb_warehouse on inv.st_warehouse_id equals wah.warehouse_id
                                   join pro in db.tb_product on inv.st_item_id equals pro.product_id
                                   where inv.st_ref_id == sTransfer.stock_transfer_id
                                   select new InventoryViewModel()
                                   {
                                       inventory_id = inv.st_detail_id,
                                       product_id = inv.st_item_id,
                                       itemCode = pro.product_code,
                                       itemName = pro.product_name,
                                       itemUnit = pro.product_unit,
                                       warehouse_id = inv.st_warehouse_id,
                                       warehouseName = wah.warehouse_name,
                                       in_quantity = 0,
                                       out_quantity = inv.quantity,
                                       unit = inv.unit,
                                       total_quantity = 0m,
                                       //total_quantity = (from invv in db.tb_inventory orderby invv.inventory_date descending where invv.product_id == inv.st_item_id && invv.warehouse_id == inv.st_warehouse_id select invv.total_quantity).FirstOrDefault(),
                                       invoice_number = inv.invoice_number,
                                       invoice_date = inv.invoice_date,
                                       item_status = inv.item_status,
                                     
                                       remark = inv.remark,
                                   }).ToList();
                sTransfer.inventoryDetails = inventories;
                sTransfer.itemTransfers = this.GetItemRequest(sTransfer.item_request_id, sTransfer.stock_transfer_id);
                //sTransfer.itemTransfers = TransferFromMainStockViewModel.GetAllAvailableMaterailRequestItembyTransferWorkshop(sTransfer.item_request_id, sTransfer.stock_transfer_id);
                sTransfer.rejects = Class.CommonClass.GetRejectByRequest(id);
                sTransfer.processWorkflows = ProcessWorkflowModel.GetProcessWorkflowByRefId(sTransfer.stock_transfer_id);
                return sTransfer;
            }
        }
        public List<STItemViewModel1> GetItemRequest(string id, string stID = null)
        {
            List<STItemViewModel1> stItems = new List<STItemViewModel1>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemRequestDetail2ViewModel> itemIDs = new List<ItemRequestDetail2ViewModel>();
             
                itemIDs = Class.ItemRequest.GetAllAvailableItembyTransferFromMainStock(id, stID);
                var warehouses = db.tb_warehouse.Where(m => m.warehouse_status == true).Select(m => m.warehouse_id).ToList();
                foreach (var it in itemIDs)
                {
                    var irItem = (from inv in db.tb_inventory
                                  join wah in db.tb_warehouse on inv.warehouse_id equals wah.warehouse_id
                                  join pro in db.tb_product on inv.product_id equals pro.product_id
                                  orderby inv.inventory_date descending
                                  where inv.warehouse_id == EnumConstants.WORKSHOP && inv.product_id == it.ir_item_id
                                  select new { inv, wah, pro }).FirstOrDefault();
                    if (irItem != null)
                    {
                        STItemViewModel1 item = new STItemViewModel1();
                        item.itemID = irItem.inv.product_id;
                        item.itemCode = irItem.pro.product_code;
                        item.itemName = irItem.pro.product_name;
                        item.itemUnit = irItem.pro.product_unit;
                        item.itemUnitName = db.tb_unit.Find(item.itemUnit).Name;
                        item.warehouseID = irItem.inv.warehouse_id;
                        item.warehouseName = irItem.wah.warehouse_name;
                        item.stockBalance = irItem.inv.total_quantity;
                        item.requestQty = Convert.ToDecimal(it.approved_qty);
                        item.requestUnit = it.ir_item_unit;
                        item.requestUnitName = db.tb_unit.Find(item.requestUnit).Name;
                        item.remain_qty = it.remain_qty;
                        item.uom1_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_id).FirstOrDefault();
                        item.uom2_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_id).FirstOrDefault();
                        item.uom3_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_id).FirstOrDefault();
                        item.uom4_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_id).FirstOrDefault();
                        item.uom5_id = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_id).FirstOrDefault();
                        item.uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom1_qty).FirstOrDefault();
                        item.uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom2_qty).FirstOrDefault();
                        item.uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom3_qty).FirstOrDefault();
                        item.uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom4_qty).FirstOrDefault();
                        item.uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == irItem.inv.product_id).Select(x => x.uom5_qty).FirstOrDefault();
                        item.projectSiteManagers = (from dwh in db.tb_warehouse
                                                    join dsite in db.tb_site on dwh.warehouse_site_id equals dsite.site_id
                                                    join dpro in db.tb_project on dsite.site_id equals dpro.site_id
                                                    join dprosm in db.tb_site_manager_project on dpro.project_id equals dprosm.project_id
                                                    where string.Compare(dwh.warehouse_id, item.warehouseID) == 0
                                                    select dprosm.site_manager).ToList();
                        stItems.Add(item);
                    }
                }
            }
            return stItems;
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
        public void DeleteTransferFromMainStockDetail(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_transfer_frommain_stock_detail.Where(m => m.st_ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.st_detail_id;
                    tb_transfer_frommain_stock_detail inv = db.tb_transfer_frommain_stock_detail.Where(m => m.st_detail_id == inventoryID).FirstOrDefault();
                    db.tb_transfer_frommain_stock_detail.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public void InsertItemInventory(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_transfer_frommain_stock_detail.Where(m => m.st_ref_id == id).ToList();
                foreach (var inv in inventories)
                {
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.st_item_id && m.warehouse_id == "1").Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "6";
                    inventory.warehouse_id = "1";
                    inventory.product_id = inv.st_item_id;
                    inventory.out_quantity = Class.CommonClass.ConvertMultipleUnit1(inv.st_item_id, inv.unit, Convert.ToDecimal(inv.quantity));
                    inventory.in_quantity = 0;
                    inventory.total_quantity = totalQty - inventory.out_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }
        }
        public void updateItemRequestStatus(string irId, string stId)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<InventoryViewModel> invs = new List<InventoryViewModel>();
            int count = 0;
            var requestItems = (from ir in db.tb_item_request
                                join dIr in db.tb_ir_detail1 on ir.ir_id equals dIr.ir_id
                                join ddIr in db.tb_ir_detail2 on dIr.ir_detail1_id equals ddIr.ir_detail1_id
                                where ir.ir_id == irId && ddIr.is_approved == true
                                select new { itemId = ddIr.ir_item_id, approvedQty = ddIr.approved_qty }).ToList();
            var inventories = (from inv in db.tb_inventory
                               where inv.ref_id == stId
                               select new { itemId = inv.product_id, qty = inv.out_quantity }).ToList();
            var dupItem = inventories.GroupBy(x => x.itemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            double totalInventoryQty = 0;
            if (dupItem.Any())
            {
                foreach (var item in dupItem)
                {
                    totalInventoryQty = 0;
                    var dd = inventories.Where(m => m.itemId == item).ToList();
                    if (dd.Any())
                    {
                        foreach (var d in dd)
                        {
                            totalInventoryQty = totalInventoryQty + Convert.ToDouble(d.qty);
                        }
                    }
                    invs.Add(new InventoryViewModel() { product_id = item, total_quantity = Convert.ToDecimal(totalInventoryQty) });
                }
            }
            else
            {
                foreach (var inv in inventories)
                {
                    invs.Add(new InventoryViewModel() { product_id = inv.itemId, total_quantity = inv.qty });
                }
            }

            foreach (var ri in requestItems)
            {
                foreach (var inv in invs)
                {
                    if (string.Compare(ri.itemId, inv.product_id) == 0 && ri.approvedQty == inv.total_quantity)
                        count++;
                }
            }
            if (count == requestItems.Count())
            {
                tb_item_request itemRequest = db.tb_item_request.Where(m => m.ir_id == irId).FirstOrDefault();
                itemRequest.ir_status = "Completed";
                itemRequest.checked_by = User.Identity.Name;
                itemRequest.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
            }
        }
        public List<InventoryViewModel> getRemainItems(string requestId)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<InventoryViewModel> itemTransfers = new List<InventoryViewModel>();
                var itemRequests = (from request in db.tb_item_request
                                    join dRequest in db.tb_ir_detail1 on request.ir_id equals dRequest.ir_id
                                    join ddRequest in db.tb_ir_detail2 on dRequest.ir_detail1_id equals ddRequest.ir_detail1_id
                                    join product in db.tb_product on ddRequest.ir_item_id equals product.product_id
                                    where request.ir_id == requestId && ddRequest.is_approved == true
                                    select new { request, dRequest, ddRequest, product }).ToList();

                var stIDs = (from st in db.transferformmainstocks
                             where st.item_request_id == requestId && st.status == true && st.stock_transfer_status == "Approved"
                             select st.stock_transfer_id).ToList();
                if (stIDs.Any())
                {
                    foreach (var stId in stIDs)
                    {
                        var transers = (from inv in db.tb_inventory
                                        where inv.ref_id == stId
                                        select new InventoryViewModel() { product_id = inv.product_id, out_quantity = inv.out_quantity }).ToList();
                        if (transers.Any())
                        {
                            foreach (var t in transers)
                            {
                                itemTransfers.Add(new InventoryViewModel() { product_id = t.product_id, out_quantity = t.out_quantity });
                            }
                        }

                    }
                }

                if (itemRequests.Any())
                {
                    foreach (var item in itemRequests)
                    {
                        decimal remainApprovedQty = Convert.ToDecimal(Class.CommonClass.ConvertMultipleUnit1(item.ddRequest.ir_item_id, item.ddRequest.ir_item_unit, Convert.ToDecimal(item.ddRequest.approved_qty)));
                        if (itemTransfers.Count() > 0)
                        {
                            foreach (InventoryViewModel st in itemTransfers)
                            {
                                if (item.ddRequest.ir_item_id == st.product_id)
                                    remainApprovedQty = remainApprovedQty - Convert.ToDecimal(st.out_quantity);
                            }
                        }
                        inventories.Add(new InventoryViewModel() { product_id = item.ddRequest.ir_item_id, itemName = item.product.product_name, total_quantity = remainApprovedQty });
                    }
                }
            }
            return inventories;
        }
        public ActionResult GetWarehousFromPR(string id)
        {
            string warehouseId = Class.CommonClass.GetWarehouseIDbyPurchaseRequisition(id);
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(warehouseId))
                {
                    var getWarehouse = db.tb_warehouse.Where(x => x.warehouse_id == warehouseId).FirstOrDefault();
                    if (getWarehouse == null)
                    {
                        return Json(JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { result = "success", data = getWarehouse }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { result = "error", message = "No warehouse available for this request." }, JsonRequestBehavior.AllowGet);
            }

        }
           
        //#region added by TTerd Apr 07 2020
        public ActionResult GetTransferListItemsJson(string materialRequestId,string transferId = null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<STItemViewModel> itemInventories = new List<STItemViewModel>();
                List<Models.ItemRequestDetail2ViewModel> materialRequestItems = TransferFromMainStockViewModel.GetAllAvailableMaterailRequestItembyTransferWorkshop(materialRequestId, transferId);
                foreach(var item in materialRequestItems)
                {
                    var itemBalance = (from inv in db.tb_inventory
                                       join material in db.tb_product on inv.product_id equals material.product_id
                                       orderby inv.inventory_date descending
                                       where string.Compare(inv.product_id, item.ir_item_id) == 0 && string.Compare(inv.warehouse_id,EnumConstants.WORKSHOP)==0
                                       select new { inv, material }).FirstOrDefault();
                    if (itemBalance != null)
                    {
                        STItemViewModel inv = new STItemViewModel();
                        inv.itemID = itemBalance.inv.product_id;
                        inv.itemCode = itemBalance.material.product_code;
                        inv.itemName = itemBalance.material.product_name;
                        inv.itemUnit = itemBalance.material.product_unit;
                        inv.itemUnitName = db.tb_unit.Find(inv.itemUnit).Name;
                        inv.stockBalance = itemBalance.inv.total_quantity;
                        inv.requestQty = item.approved_qty;
                        inv.requestUnit = item.ir_item_unit;
                        inv.requestUnitName = db.tb_unit.Find(inv.requestUnit).Name;
                        itemInventories.Add(inv);
                    }
                    
                }
                return Json(new { data = itemInventories }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<TransferFromMainStockViewModel> GetTransferFromWorkshopListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<TransferFromMainStockViewModel> items = new List<TransferFromMainStockViewModel>();
                List<TransferFromMainStockViewModel> objs = new List<TransferFromMainStockViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                {
                    items = (from tw in db.transferformmainstocks
                             join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                             join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                             //join site in db.tb_site on pro.site_id equals site.site_id
                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                             orderby tw.created_date descending
                             where tw.status == true
                             select new TransferFromMainStockViewModel()
                             {
                                 stock_transfer_id = tw.stock_transfer_id,
                                 stock_transfer_no = tw.stock_transfer_no,
                                 item_request_id = tw.item_request_id,
                                 item_request_no = mr.ir_no,
                                 stock_transfer_status = tw.stock_transfer_status,
                                 created_by = tw.created_by,
                                 created_date = tw.created_date,
                                 project_id = pro.project_id,
                                 project_fullname = pro.project_full_name,
                                 warehouse_id = wh.warehouse_id,
                                 warehouse_name = wh.warehouse_name
                             }).ToList();
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    if (User.IsInRole(Role.Purchaser))
                    {
                        items = (from tw in db.transferformmainstocks
                                 join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                 from wh in pwh.DefaultIfEmpty()
                                 orderby tw.created_date descending
                                 where tw.status == true && string.Compare(tw.created_by,userid)==0
                                 select new TransferFromMainStockViewModel()
                                 {
                                     stock_transfer_id = tw.stock_transfer_id,
                                     stock_transfer_no = tw.stock_transfer_no,
                                     item_request_id = tw.item_request_id,
                                     item_request_no = mr.ir_no,
                                     stock_transfer_status = tw.stock_transfer_status,
                                     created_by = tw.created_by,
                                     created_date = tw.created_date,
                                     project_id = pro.project_id,
                                     project_fullname = pro.project_full_name,
                                     warehouse_id = wh.warehouse_id,
                                     warehouse_name = wh.warehouse_name
                                 }).ToList();
                    }
                    if (User.IsInRole(Role.WorkshopController))
                    {
                        objs = (from tw in db.transferformmainstocks
                                join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                orderby tw.created_date descending
                                where tw.status == true &&( string.Compare(tw.approved_by, userid) == 0  ||string.Compare(tw.stock_transfer_status,Status.Pending)==0 || string.Compare(tw.stock_transfer_status,Status.Feedbacked)==0)
                                select new TransferFromMainStockViewModel()
                                {
                                     stock_transfer_id = tw.stock_transfer_id,
                                     stock_transfer_no = tw.stock_transfer_no,
                                     item_request_id = tw.item_request_id,
                                     item_request_no = mr.ir_no,
                                     stock_transfer_status = tw.stock_transfer_status,
                                     created_by = tw.created_by,
                                     created_date = tw.created_date,
                                     project_id = pro.project_id,
                                     project_fullname = pro.project_full_name,
                                     warehouse_id = wh.warehouse_id,
                                     warehouse_name = wh.warehouse_name
                                 }).ToList();
                        foreach(var obj in objs)
                        {
                            var isExist = items.Where(s => string.Compare(s.stock_transfer_id, obj.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                items.Add(obj);
                        }
                    }
                }
                return items;
            }
        }
        public List<TransferFromMainStockViewModel> GetTransferFromWorkshopListItemsbyDateRangeandStatus(string dateRange,string status)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<TransferFromMainStockViewModel> items = new List<TransferFromMainStockViewModel>();
                List<TransferFromMainStockViewModel> objs = new List<TransferFromMainStockViewModel>();
                List<FilterTransferWorkshopModel> results = new List<FilterTransferWorkshopModel>();

                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                if (string.Compare(status, "0") == 0)
                {
                    if (User.IsInRole(Role.SystemAdmin)||User.IsInRole(Role.Purchaser))
                    {
                        results = (from tw in db.transferformmainstocks
                                 join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                 join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                 from wh in pwh.DefaultIfEmpty()
                                 orderby tw.created_date descending
                                 where tw.status == true && tw.created_date>=startDate && tw.created_date<=endDate
                                 select new FilterTransferWorkshopModel() {tw=tw,mr=mr,pro=pro,wh=wh }).ToList();
                    }
                    else
                    {
                        string userid = User.Identity.GetUserId().ToString();
                        //if (User.IsInRole(Role.Purchaser))
                        //{
                        //    var rs = (from tw in db.transferformmainstocks
                        //             join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                        //             join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                        //             //join site in db.tb_site on pro.site_id equals site.site_id
                        //             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                        //             from wh in pwh.DefaultIfEmpty()
                        //             orderby tw.created_date descending
                        //             where tw.status == true && string.Compare(tw.created_by, userid) == 0 && tw.created_date >= startDate && tw.created_date <= endDate
                        //              select new FilterTransferWorkshopModel() { tw = tw, mr = mr, pro = pro, wh = wh }).ToList();
                        //    results.AddRange(rs);
                        //}
                        if (User.IsInRole(Role.WorkshopController))
                        {
                            var rs = (from tw in db.transferformmainstocks
                                    join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                    join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                    //join site in db.tb_site on pro.site_id equals site.site_id
                                    join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                    from wh in pwh.DefaultIfEmpty()
                                    orderby tw.created_date descending
                                    where tw.status == true && (string.Compare(tw.approved_by, userid) == 0 || string.Compare(tw.stock_transfer_status, Status.Pending) == 0 || string.Compare(tw.stock_transfer_status, Status.Feedbacked) == 0)
                                    && tw.created_date >= startDate && tw.created_date <= endDate
                                    select new FilterTransferWorkshopModel() { tw = tw, mr = mr, pro = pro, wh = wh }).ToList();
                            results.AddRange(rs);
                        }
                    }
                }
                else
                {
                    if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser))
                    {
                        results = (from tw in db.transferformmainstocks
                                   join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                   join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                   //join site in db.tb_site on pro.site_id equals site.site_id
                                   join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                   from wh in pwh.DefaultIfEmpty()
                                   orderby tw.created_date descending
                                   where tw.status == true && tw.created_date >= startDate && tw.created_date <= endDate && string.Compare(tw.stock_transfer_status,status)==0
                                   select new FilterTransferWorkshopModel() { tw = tw, mr = mr, pro = pro, wh = wh }).ToList();
                    }
                    else
                    {
                        string userid = User.Identity.GetUserId().ToString();
                        //if (User.IsInRole(Role.Purchaser))
                        //{
                        //    var rs = (from tw in db.transferformmainstocks
                        //              join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                        //              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                        //              //join site in db.tb_site on pro.site_id equals site.site_id
                        //              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                        //              from wh in pwh.DefaultIfEmpty()
                        //              orderby tw.created_date descending
                        //              where tw.status == true && string.Compare(tw.created_by, userid) == 0 && tw.created_date >= startDate && tw.created_date <= endDate&&string.Compare(tw.stock_transfer_status, status) == 0
                        //              select new FilterTransferWorkshopModel() { tw = tw, mr = mr, pro = pro, wh = wh }).ToList();
                        //    results.AddRange(rs);
                        //}
                        if (User.IsInRole(Role.WorkshopController))
                        {
                            var rs = (from tw in db.transferformmainstocks
                                      join mr in db.tb_item_request on tw.item_request_id equals mr.ir_id
                                      join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                      //join site in db.tb_site on pro.site_id equals site.site_id
                                      join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                      from wh in pwh.DefaultIfEmpty()
                                      orderby tw.created_date descending
                                      where tw.status == true && (string.Compare(tw.approved_by, userid) == 0 || string.Compare(tw.stock_transfer_status, Status.Pending) == 0 || string.Compare(tw.stock_transfer_status, Status.Feedbacked) == 0)
                                      && tw.created_date >= startDate && tw.created_date <= endDate&&string.Compare(tw.stock_transfer_status, status) == 0
                                      select new FilterTransferWorkshopModel() { tw = tw, mr = mr, pro = pro, wh = wh }).ToList();
                            results.AddRange(rs);
                        }
                    }
                }

                foreach(var rs in results.DistinctBy(s=>s.tw).ToList())
                {
                    string created_by_text = CommonClass.GetUserFullnameByUserId(rs.tw.created_by);
                    string created_at_text = Convert.ToDateTime(rs.tw.created_date).ToString("dd/MM/yyyy");
                    string show_status_text = ShowStatus.GetTransferFromWorkshopShowStatus(rs.tw.stock_transfer_status);
                    string grn_show_status_text = CommonFunctions.GetLatestGRNStatusbyRefId(rs.tw.stock_transfer_id);

                    items.Add(new TransferFromMainStockViewModel()
                    {
                        stock_transfer_id = rs.tw.stock_transfer_id,
                        stock_transfer_no = rs.tw.stock_transfer_no,
                        item_request_id = rs.tw.item_request_id,
                        item_request_no = rs.mr.ir_no,
                        stock_transfer_status = rs.tw.stock_transfer_status,
                        created_by = rs.tw.created_by,
                        created_date = rs.tw.created_date,
                        project_id = rs.pro.project_id,
                        project_fullname = rs.pro.project_full_name,
                        warehouse_id = rs.wh.warehouse_id,
                        warehouse_name = rs.wh.warehouse_name,
                        created_at_text=created_at_text,
                        created_by_text=created_by_text,
                        show_status=show_status_text,
                        grn_show_status=grn_show_status_text,
                    });
                }

                return items;
            }
            
        }
        public ActionResult GetTransferFromWorkshopbgyDateRangeandStatusAJAX(string dateRange,string status)
        {
            return Json(new { data = GetTransferFromWorkshopListItemsbyDateRangeandStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }
        //#endregion
        public ActionResult GetTransferFromWorkshopDatabyMR(string materialRequestId,string transferId=null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<STItemViewModel> itemInventories = new List<STItemViewModel>();
                //get detail information
                ItemRequestViewModel materialRequest = (from pr in db.tb_item_request
                                       join pro in db.tb_project on pr.ir_project_id equals pro.project_id
                                                        //join site in db.tb_site on pro.site_id equals site.site_id
                                                        join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                        from wh in pwh.DefaultIfEmpty()
                                                        where string.Compare(pr.ir_id, materialRequestId) == 0
                                       select new ItemRequestViewModel {
                                           ir_id=pr.ir_id,
                                           ir_no=pr.ir_no,
                                           ir_project_id=pr.ir_project_id,
                                           project_full_name=pro.project_full_name,
                                           warehouse_id=wh.warehouse_id,
                                           warehouse_name=wh.warehouse_name,
                                       }).FirstOrDefault();
                //material request list by project
                List<ItemRequestViewModel>  itemRequests = ItemRequest.GetAllItemRequestDropdownList().Where(w => string.Compare(w.ir_project_id, materialRequest.ir_project_id) == 0).ToList();

                //get item in stock workshop
                List<Models.ItemRequestDetail2ViewModel> materialRequestItems = TransferFromMainStockViewModel.GetAllAvailableMaterailRequestItembyTransferWorkshop(materialRequestId, transferId);
                foreach (var item in materialRequestItems)
                {
                    var itemBalance = (from inv in db.tb_inventory
                                       join material in db.tb_product on inv.product_id equals material.product_id
                                       orderby inv.inventory_date descending
                                       where string.Compare(inv.product_id, item.ir_item_id) == 0 && string.Compare(inv.warehouse_id,EnumConstants.WORKSHOP)==0
                                       select new { inv, material }).FirstOrDefault();
                    if (itemBalance != null)
                    {
                        STItemViewModel inv = new STItemViewModel();
                        inv.itemID = itemBalance.inv.product_id;
                        inv.itemCode = itemBalance.material.product_code;
                        inv.itemName = itemBalance.material.product_name;
                        inv.itemUnit = itemBalance.material.product_unit;
                        inv.itemUnitName = db.tb_unit.Find(inv.itemUnit).Name;
                        inv.stockBalance = itemBalance.inv.total_quantity;
                        inv.requestQty = item.approved_qty;
                        inv.requestUnit = item.ir_item_unit;
                        inv.requestUnitName = db.tb_unit.Find(inv.requestUnit).Name;
                        itemInventories.Add(inv);
                    }

                }
                return Json(new { materialRequest=materialRequest, itemRequests= itemRequests, data = itemInventories }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult RequestCancel(string id, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                transferformmainstock transferFromMS = db.transferformmainstocks.Find(id);
                transferFromMS.stock_transfer_status = Status.RequestCancelled;
                transferFromMS.updated_by = User.Identity.GetUserId();
                transferFromMS.updated_date = DateTime.Now;
                transferFromMS.checked_comment = comment;
                db.SaveChanges();

                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = transferFromMS.stock_transfer_id;
                reject.ref_type = "Transfer From Workshop";
                reject.comment = comment;
                reject.rejected_by = User.Identity.GetUserId();
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_reject.Add(reject);
                db.SaveChanges();

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

