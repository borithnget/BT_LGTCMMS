using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class StockIssueReturnController : Controller
    {
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        // GET: StockIssueReturn
        public ActionResult Index()
        {
            //return View(this.GetStockIssueReturnList());
            return View();
        }

        // GET: StockIssueReturn/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueReturnViewModel model = new StockIssueReturnViewModel();
            model = this.GetIssueReturnDetail(id);
            return View(model);
        }

        // GET: StockIssueReturn/Create
        public ActionResult Create()
        {
            List<StockTransferViewModel> issues = new List<StockTransferViewModel>();
            issues = this.GetIssueNumberReferences();
            //ViewBag.IssueReturnNumber = this.GetIssueReturnNumber();
            ViewBag.IssueReturnNumber = CommonClass.GenerateProcessNumber("STR");
            ViewBag.TransferReference = new SelectList(issues, "stock_transfer_id", "stock_transfer_no");
            return View();

        }


        // POST: StockIssueReturn/Create
        //[HttpPost]
        //public ActionResult Create(StockIssueReturnViewModel model)
        //{
        //    try
        //    {
        //        //List<InventoryDetailViewModel> remain = new List<InventoryDetailViewModel>();
        //        List<InventoryViewModel> remain = new List<InventoryViewModel>();
        //        kim_mexEntities db = new kim_mexEntities();
        //        string msg = string.Empty;
        //        int countInvalid = 0;
        //        if(model.inventories.Count()==0)
        //            return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
        //        var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //        if (dupItem.Count() > 0)
        //        {
        //            return Json(new { result = "error", message = "Duplicate Item and Warehouse." }, JsonRequestBehavior.AllowGet);
        //        }

        //        //remain= this.isStockIssueExist(model.stock_issue_ref);
        //        remain = Class.StockIssueReturn.GetStockIssueItemsReferences(model.stock_issue_ref);
        //        foreach(InventoryViewModel item in remain)
        //        {
        //            foreach(InventoryViewModel issue in model.inventories)
        //            {
        //                if (item.product_id == issue.product_id && item.warehouse_id == issue.warehouse_id && item.out_quantity < CommonClass.ConvertMultipleUnit(issue.product_id,issue.unit,Convert.ToDecimal(issue.in_quantity)))
        //                {
        //                    countInvalid++;
        //                    if(item.out_quantity==0)
        //                        msg= msg + " " + issue.itemName + " in " + issue.warehouseName + " is completed to issue return\n";
        //                    else
        //                        msg = msg + " " + issue.itemName + " in " + issue.warehouseName + " remain quantity=" + item.out_quantity + " to issue return\n";
        //                    break;
        //                }
        //            }
        //        }

        //        if (countInvalid > 0)
        //            return Json(new { result = "error", message = msg }, JsonRequestBehavior.AllowGet);
        //        tb_stock_issue_return issueReturn = new tb_stock_issue_return();

        //        issueReturn.stock_issue_return_id = Guid.NewGuid().ToString();
        //        issueReturn.issue_return_number = CommonClass.GenerateProcessNumber("SIR");
        //        issueReturn.stock_issue_ref = model.stock_issue_ref;
        //        issueReturn.issue_return_status = "Pending";
        //        issueReturn.status = true;
        //        issueReturn.created_by = User.Identity.Name;
        //        issueReturn.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
        //        db.tb_stock_issue_return.Add(issueReturn);
        //        db.SaveChanges();
        //        CommonClass.AutoGenerateStockInvoiceNumber(issueReturn.stock_issue_return_id, model.inventories);
        //        foreach (var inv in model.inventories)
        //        {
        //            if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >=Class.CommonClass.ConvertMultipleUnit(inv.product_id,inv.unit,Convert.ToDecimal(inv.in_quantity))) //&& inv.total_quantity >= inv.in_quantity
        //            {
        //                tb_inventory_detail inventoryDetail = new tb_inventory_detail();
        //                inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
        //                inventoryDetail.inventory_ref_id = issueReturn.stock_issue_return_id;
        //                inventoryDetail.inventory_item_id = inv.product_id;
        //                inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
        //                inventoryDetail.quantity = inv.in_quantity;
        //                inventoryDetail.remark = inv.remark;
        //                inventoryDetail.unit = inv.unit;
        //                inventoryDetail.item_status = "pending";
        //                inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
        //                inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(issueReturn.stock_issue_return_id, inventoryDetail.inventory_warehouse_id, inventoryDetail.invoice_date);
        //                inventoryDetail.inventory_type = "5";
        //                db.tb_inventory_detail.Add(inventoryDetail);
        //                db.SaveChanges();
        //            }
        //        }
        //        StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, false);
        //        return Json(new { result = "success" },JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json(new { result = "error" },JsonRequestBehavior.AllowGet);
        //    }
        //}
        [HttpPost]
        public ActionResult Create(StockIssueReturnViewModel model)
        {
            try
            {
                //List<InventoryDetailViewModel> remain = new List<InventoryDetailViewModel>();
                List<InventoryViewModel> remain = new List<InventoryViewModel>();
                kim_mexEntities db = new kim_mexEntities();
                string msg = string.Empty;
                int countInvalid = 0;
                if (model.inventories.Count() == 0)
                    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplicate Item and Warehouse." }, JsonRequestBehavior.AllowGet);
                }

                //remain= this.isStockIssueExist(model.stock_issue_ref);
                remain = Class.StockIssueReturn.GetStockIssueItemsReferences(model.stock_issue_ref);
                foreach (InventoryViewModel item in remain)
                {
                    foreach (InventoryViewModel issue in model.inventories)
                    {
                        if (item.product_id == issue.product_id && item.warehouse_id == issue.warehouse_id && item.out_quantity < CommonClass.ConvertMultipleUnit(issue.product_id, issue.unit, Convert.ToDecimal(issue.in_quantity)))
                        {
                            countInvalid++;
                            if (item.out_quantity == 0)
                                msg = msg + " " + issue.itemName + " in " + issue.warehouseName + " is completed to issue return\n";
                            else
                                msg = msg + " " + issue.itemName + " in " + issue.warehouseName + " remain quantity=" + item.out_quantity + " to issue return\n";
                            break;
                        }
                    }
                }

                if (countInvalid > 0)
                    return Json(new { result = "error", message = msg }, JsonRequestBehavior.AllowGet);

                tb_stock_issue_return issueReturn = new tb_stock_issue_return();
                issueReturn.stock_issue_return_id = Guid.NewGuid().ToString();
                issueReturn.issue_return_number = CommonClass.GenerateProcessNumber("STR");
                issueReturn.stock_issue_ref = model.stock_issue_ref;
                issueReturn.issue_return_status = Status.Pending;
                issueReturn.status = true;
                issueReturn.created_by = User.Identity.GetUserId();
                issueReturn.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                issueReturn.is_receive_completed = false;
                db.tb_stock_issue_return.Add(issueReturn);
                db.SaveChanges();
                CommonClass.AutoGenerateStockInvoiceNumber(issueReturn.stock_issue_return_id, model.inventories);
                foreach (var inv in model.inventories)
                {
                    var remains = remain.FirstOrDefault(item => item.product_id == inv.product_id);
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >= Class.CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.in_quantity))) //&& inv.total_quantity >= inv.in_quantity
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = issueReturn.stock_issue_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.in_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = Status.Pending;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(issueReturn.stock_issue_return_id, inventoryDetail.inventory_warehouse_id, inventoryDetail.invoice_date);
                        inventoryDetail.inventory_type = "5";
                        inventoryDetail.remain_quantity = inv.in_quantity;
                        if (remains != null)
                            inventoryDetail.ordering_number = remains.ordering_number;
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, false);
                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(issueReturn.stock_issue_ref);
                stockTransfer.sr_status = ShowStatus.SRCreated;
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), issueReturn.stock_issue_return_id, issueReturn.issue_return_status, issueReturn.created_by, issueReturn.created_date, string.Empty);


                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CancelRequest(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue_return issueReturn = db.tb_stock_issue_return.Find(id);
                issueReturn.issue_return_status = Status.RequestCancelled;
                issueReturn.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                issueReturn.update_by = User.Identity.GetUserId();
                db.SaveChanges();
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, true);
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), issueReturn.stock_issue_return_id, issueReturn.issue_return_status, issueReturn.update_by, issueReturn.updated_date, string.Empty);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: StockIssueReturn/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueReturnViewModel model = new StockIssueReturnViewModel();
            model = this.GetIssueReturnDetail(id);
            List<StockTransferViewModel> issues = new List<StockTransferViewModel>();
            ViewBag.IssueReference = this.GetIssueNumberReferences1(model.stock_issue_ref);
            //ViewBag.IssueReference = new SelectList(issues, "stock_issue_id", "stock_issue_number");
            return View(model);
        }

        // POST: StockIssueReturn/Edit/5
        [HttpPost]
        public ActionResult Edit(StockIssueReturnViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                //List<InventoryDetailViewModel> remain = new List<InventoryDetailViewModel>();
                List<InventoryViewModel> remain = new List<InventoryViewModel>();
                int countInvalid = 0;
                string msg = string.Empty;
                if (model.inventories.Count() == 0)
                    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplicate Item and Warehouse." }, JsonRequestBehavior.AllowGet);
                }
                //remain = this.isStockIssueExist(model.stock_issue_ref);
                remain = Class.StockIssueReturn.GetStockIssueItemsReferences(model.stock_issue_ref);
                foreach (InventoryViewModel item in remain)
                {
                    var a = (from fd in db.tb_inventory_detail where fd.inventory_ref_id == model.stock_issue_return_id && fd.inventory_warehouse_id == item.warehouse_id && fd.inventory_item_id == item.product_id select fd.quantity).FirstOrDefault();

                    foreach (InventoryViewModel issue in model.inventories)
                    {
                        
                        if (item.product_id == issue.product_id && item.warehouse_id == issue.warehouse_id && (item.out_quantity + a) < CommonClass.ConvertMultipleUnit(issue.product_id,issue.unit,Convert.ToDecimal(issue.in_quantity)))
                        {
                            countInvalid++;
                            if (item.out_quantity == 0)
                                msg = msg + " " + issue.itemName + " in " + issue.warehouseName + " is completed to issue return\n";
                            else
                                msg = msg + " " + issue.itemName + " in " + issue.warehouseName + " remain quantity=" + item.out_quantity + " to issue return\n";
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return Json(new { result = "error", message = msg }, JsonRequestBehavior.AllowGet);
                string id = model.stock_issue_return_id;
                tb_stock_issue_return issueReturn = db.tb_stock_issue_return.Find(id);
                issueReturn.stock_issue_ref = model.stock_issue_ref;
                issueReturn.issue_return_status = "Pending";
                issueReturn.status = true;
                issueReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                issueReturn.update_by = User.Identity.Name;
                db.SaveChanges();
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, true);
                this.DeleteIssueReturnDetail(id);
                CommonClass.DeleteInvoiceNumber(issueReturn.stock_issue_return_id);
                CommonClass.AutoGenerateStockInvoiceNumber(issueReturn.stock_issue_return_id, model.inventories);
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >= CommonClass.ConvertMultipleUnit(inv.product_id,inv.unit,Convert.ToDecimal(inv.in_quantity)))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = issueReturn.stock_issue_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.in_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = "pending";
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(issueReturn.stock_issue_return_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.inventory_type = "5";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, false);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: StockIssueReturn/Delete/5
        public ActionResult Delete(string id)
        {
            
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string idstv = "";
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_issue_return issueReturn = db.tb_stock_issue_return.Find(id);
                    idstv = issueReturn.stock_issue_ref;
                    var sumstock = (from sir in db.tb_stock_issue_return
                                    join stv in db.tb_stock_transfer_voucher
                                    on sir.stock_issue_ref equals stv.stock_transfer_id
                                    join std in db.tb_stock_transfer_detail on stv.stock_transfer_id equals std.st_ref_id
                                    where sir.stock_issue_return_id == id && sir.status == true && std.status == true
                                    select new { std,stv}).ToList();
                    var searchstock = (from sir in db.tb_stock_issue_return
                                       join invd in db.tb_inventory_detail
                                       on sir.stock_issue_return_id equals invd.inventory_ref_id
                                       where sir.stock_issue_return_id == id && sir.status == true
                                       select invd
                                       ).ToList();
                    
                    //var returnstock = (from sst in sumstock
                    //                   join st in searchstock on sst.st_item_id equals st.inventory_item_id
                    //                   where sst.st_item_id == st.inventory_item_id && sst.st_warehouse_id == st.inventory_warehouse_id
                    //                   select sst
                    //                   ).ToList();
                    foreach(var remaininstd in sumstock)
                    {
                       
                        foreach(var remainininvd in searchstock)
                        {
                            if(remaininstd.std.st_item_id == remainininvd.inventory_item_id && remaininstd.std.st_warehouse_id == remainininvd.inventory_warehouse_id)
                            {
                                remaininstd.std.remain_quantity = remaininstd.std.remain_quantity + remainininvd.quantity;                              
                            }
                        }
                     
                    }
                   
                    
                    issueReturn.status = false;
                    issueReturn.update_by = User.Identity.Name;
                    issueReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }
                StockIssueReturn.UpdateStockIssueCompleted(idstv);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
      
        public ActionResult Approve(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue_return issueReturn = db.tb_stock_issue_return.Find(id);
                issueReturn.issue_return_status = Status.Completed;
                issueReturn.checked_by = User.Identity.GetUserId();
                issueReturn.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(issueReturn.stock_issue_ref);
                stockTransfer.sr_status = ShowStatus.GRNPending ;
                db.SaveChanges();
                string warehouseId = (from st in db.tb_stock_transfer_voucher
                                      join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                                      join wh in db.tb_warehouse on mr.ir_project_id equals wh.warehouse_project_id
                                      where string.Compare(st.stock_transfer_id, issueReturn.stock_issue_ref) == 0
                                      select wh.warehouse_id).FirstOrDefault().ToString();

                var inventories = db.tb_inventory_detail.Where(m => m.inventory_ref_id == id).ToList();
                foreach (var inv in inventories)
                {
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == warehouseId).Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "5";
                    inventory.warehouse_id = warehouseId;
                    inventory.product_id = inv.inventory_item_id;
                    inventory.out_quantity = CommonClass.ConvertMultipleUnit(inv.inventory_item_id, inv.unit, Convert.ToDecimal(inv.quantity));
                    inventory.in_quantity = 0;
                    inventory.total_quantity = totalQty - inventory.out_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), issueReturn.stock_issue_return_id, issueReturn.issue_return_status, issueReturn.checked_by, issueReturn.checked_date, string.Empty);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Reject(string id,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_stock_issue_return issue = db.tb_stock_issue_return.Find(id);
                issue.issue_return_status = Status.CheckRejected;
                issue.checked_by = User.Identity.GetUserId();
                issue.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                issue.checked_comment = comment;
                db.SaveChanges();
                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = issue.stock_issue_return_id;
                reject.ref_type = "Stock Return";
                reject.comment = comment;
                reject.rejected_by = User.Identity.GetUserId();
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_reject.Add(reject);
                db.SaveChanges();
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), issue.stock_issue_return_id, issue.issue_return_status, issue.checked_by, issue.checked_date, issue.checked_comment);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueReturnViewModel model = new StockIssueReturnViewModel();
            model = this.GetIssueReturnDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<InventoryDetailViewModel> models)
        {
            try
            {
                int countApproved = 0;
                int countAppend = 0;
                kim_mexEntities db = new kim_mexEntities();
                foreach(InventoryDetailViewModel item in models)
                {
                    string iid = item.inventory_detail_id;
                    tb_inventory_detail inventoryDetail = db.tb_inventory_detail.Find(iid);
                    inventoryDetail.item_status = item.item_status;
                    inventoryDetail.remark = item.remark;
                    db.SaveChanges();

                    if (string.Compare(item.item_status, "approved") == 0)
                    {
                        countApproved++;

                        //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.ref_id = id;
                        //inventory.inventory_status_id = "5";
                        //inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                        //inventory.product_id = inventoryDetail.inventory_item_id;
                        //inventory.out_quantity = 0;
                        //inventory.in_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inventoryDetail.quantity));
                        //inventory.total_quantity = totalQty + inventory.in_quantity;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();

                    }

                    //if (string.Compare(item.item_status, "firstapproved") == 0)
                    //{
                    //    countAppend++;
                    //    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //    tb_inventory inventory = new tb_inventory();
                    //    inventory.inventory_id = Guid.NewGuid().ToString();
                    //    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //    inventory.ref_id = id;
                    //    inventory.inventory_status_id = "5";
                    //    inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                    //    inventory.product_id = inventoryDetail.inventory_item_id;
                    //    inventory.out_quantity = 0;
                    //    inventory.in_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inventoryDetail.quantity));
                    //    inventory.total_quantity = totalQty + inventory.in_quantity;
                    //    db.tb_inventory.Add(inventory);
                    //    db.SaveChanges();
                    //}
                    //else if (string.Compare(item.item_status, "approved") == 0)
                    //{
                    //    countApproved++;

                    //    //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //    //tb_inventory inventory = new tb_inventory();
                    //    //inventory.inventory_id = Guid.NewGuid().ToString();
                    //    //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //    //inventory.ref_id = id;
                    //    //inventory.inventory_status_id = "5";
                    //    //inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                    //    //inventory.product_id = inventoryDetail.inventory_item_id;
                    //    //inventory.out_quantity = 0;
                    //    //inventory.in_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inventoryDetail.quantity));
                    //    //inventory.total_quantity = totalQty + inventory.in_quantity;
                    //    //db.tb_inventory.Add(inventory);
                    //    //db.SaveChanges();

                    //}

                    //tb_stock_issue_return stockIssueReturn = db.tb_stock_issue_return.Find(id);
                    ////if(countAppend != 0)
                    ////{
                    ////    stockIssueReturn.issue_return_status = countAppend == models.Count() ? "Complete" : "Pending Feedback";
                    ////    stockIssueReturn.approved_by = User.Identity.Name;
                    ////}
                    ////else if(countApproved != 0)
                    ////{
                    ////    stockIssueReturn.issue_return_status = countApproved == models.Count() ? "Completed" : "Pending Feedback";
                    ////    stockIssueReturn.last_approved_by = User.Identity.Name;
                    ////}else if(countAppend == 0 && countApproved == 0)
                    ////{
                    ////    stockIssueReturn.issue_return_status = "Pending Feedback";
                    ////    stockIssueReturn.approved_by = User.Identity.Name;
                    ////}
                    //stockIssueReturn.issue_return_status = countApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                    //stockIssueReturn.last_approved_by = User.Identity.GetUserId();
                    //stockIssueReturn.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //db.SaveChanges();
                    //stockIssueReturn.issue_return_status = countApproved == models.Count() ? "Complete" : "Pending Feedback";

                    //}else if (string.Compare(item.item_status, "approved")== 0)
                    //{
                    //    countApproved++;
                    //    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inventoryDetail.inventory_item_id && m.warehouse_id == inventoryDetail.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    //    tb_inventory inventory = new tb_inventory();
                    //    inventory.inventory_id = Guid.NewGuid().ToString();
                    //    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //    inventory.ref_id = id;
                    //    inventory.inventory_status_id = "5";
                    //    inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                    //    inventory.product_id = inventoryDetail.inventory_item_id;
                    //    inventory.out_quantity = 0;
                    //    inventory.in_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inventoryDetail.quantity));
                    //    inventory.total_quantity = totalQty + inventory.in_quantity;
                    //    db.tb_inventory.Add(inventory);
                    //    db.SaveChanges();

                    //    tb_stock_issue_return stockIssueReturn = db.tb_stock_issue_return.Find(id);
                    //    stockIssueReturn.issue_return_status = countApproved == models.Count() ? "Completed" : "Pending Feedback";
                    //    stockIssueReturn.approved_by = User.Identity.Name;
                    //    stockIssueReturn.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    //    db.SaveChanges();
                    //}
                }
                tb_stock_issue_return stockIssueReturn = db.tb_stock_issue_return.Find(id);
                stockIssueReturn.issue_return_status = countApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                stockIssueReturn.last_approved_by = User.Identity.GetUserId();
                stockIssueReturn.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                tb_stock_transfer_voucher stockTransfer = db.tb_stock_transfer_voucher.Find(stockIssueReturn.stock_issue_ref);
                stockTransfer.sr_status = ShowStatus.SRChecked;
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), stockIssueReturn.stock_issue_return_id, stockIssueReturn.issue_return_status, stockIssueReturn.last_approved_by, stockIssueReturn.approved_date, string.Empty);

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
            StockIssueReturnViewModel model = new StockIssueReturnViewModel();
            model = this.GetIssueReturnDetail(id);
            return View(model);
        }
        [HttpPost] 
        public ActionResult PrepareFeedback(string id,List<InventoryViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_stock_issue_return issueReturn = db.tb_stock_issue_return.Find(id);
                issueReturn.issue_return_status = models.Count() == 0 ? "Completed" : "Feedbacked";
                issueReturn.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                issueReturn.update_by = User.Identity.Name;
                db.SaveChanges();
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, true,"feedbacked");
                var inventories = db.tb_inventory_detail.Where(x => x.inventory_ref_id == id && string.Compare(x.item_status,"feedbacked")==0).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_detail_id;
                    tb_inventory_detail inv = db.tb_inventory_detail.Where(x => x.inventory_detail_id == inventoryID).FirstOrDefault();
                    db.tb_inventory_detail.Remove(inv);
                    db.SaveChanges();
                }
                foreach (var inv in models)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >= CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.in_quantity)))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = issueReturn.stock_issue_return_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.in_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        //inventoryDetail.item_status = "pending";
                        inventoryDetail.item_status = "feedbacked";
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = inv.invoice_number;
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                StockIssueReturn.RollbackStockIssueItems(issueReturn.stock_issue_ref, issueReturn.stock_issue_return_id, false, "feedbacked");
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), issueReturn.stock_issue_return_id, issueReturn.issue_return_status, issueReturn.update_by, issueReturn.updated_date, string.Empty);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetIssueReturnDataTable(string status)
        {
            List<StockIssueReturnViewModel> issueReturns = new List<StockIssueReturnViewModel>();
            issueReturns = this.GetStockIssueReturnList(status);
            return Json(new { data = issueReturns }, JsonRequestBehavior.AllowGet);
        }
        public List<StockTransferViewModel> GetIssueNumberReferences(string id = null)
        {
            List<StockTransferViewModel> issues = new List<StockTransferViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {

                //var item11 = (from a in db.tb_stock_transfer_voucher join b in db.tb_stock_transfer_detail on a.stock_transfer_id equals b.st_ref_id where b.status == true && a.status == true && a.stock_transfer_status == "Completed" && b.item_status == "approved" && b.remain_quantity != 0 select new { a }).ToList();
                
                //issues = item11.Select(x => new StockTransferViewModel() { stock_transfer_id = x.a.stock_transfer_id, stock_transfer_no = x.a.stock_transfer_no }).ToList();
                issues = db.tb_stock_transfer_voucher.Where(x => x.status == true && x.is_return_complete == false && string.Compare(x.stock_transfer_status, Status.Completed) == 0 ).Select(x => new StockTransferViewModel() { stock_transfer_id = x.stock_transfer_id, stock_transfer_no = x.stock_transfer_no }).ToList();

                if (!string.IsNullOrEmpty(id))
                {
                    bool isExist = issues.Where(x => string.Compare(x.stock_transfer_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExist)
                    {
                        issues.Add(new StockTransferViewModel()
                        {
                            stock_transfer_id = id,
                            stock_transfer_no = db.tb_stock_transfer_voucher.Where(x => string.Compare(x.stock_transfer_id, id) == 0).Select(x => x.stock_transfer_no).FirstOrDefault().ToString(),
                        });
                    }
                }
                issues = issues.OrderBy(x => x.stock_transfer_no).ToList();

            }
            return issues;
        }
        public List<StockTransferViewModel> GetIssueNumberReferences1(string id = null)
        {
            List<StockTransferViewModel> issues = new List<StockTransferViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {

                var item11 = (from a in db.tb_stock_transfer_voucher join b in db.tb_stock_transfer_detail on a.stock_transfer_id equals b.st_ref_id where b.status == true && a.status == true && a.stock_transfer_status == "Completed" && b.item_status == "approved" select new { a.stock_transfer_id, b.st_ref_id, a.stock_transfer_no }).ToList();

                issues = item11.Select(x => new StockTransferViewModel() { stock_transfer_id = x.stock_transfer_id, stock_transfer_no = x.stock_transfer_no }).ToList();
                //issues = db.tb_stock_transfer_voucher.Where(x => x.status == true && string.Compare(x.stock_transfer_status, "Completed") == 0 ).Select(x => new StockTransferViewModel() { stock_transfer_id = x.stock_transfer_id, stock_transfer_no = x.stock_transfer_no }).ToList();


                if (!string.IsNullOrEmpty(id))
                {
                    bool isExist = issues.Where(x => string.Compare(x.stock_transfer_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExist)
                    {
                        issues.Add(new StockTransferViewModel()
                        {
                            stock_transfer_id = id,
                            stock_transfer_no = db.tb_stock_transfer_voucher.Where(x => string.Compare(x.stock_transfer_id, id) == 0).Select(x => x.stock_transfer_no).FirstOrDefault().ToString(),
                        });
                    }
                }
                issues = issues.OrderBy(x => x.stock_transfer_no).ToList();

            }
            return issues;
        }

        public List<StockIssueReturnViewModel> GetStockIssueReturnList(string status)
        {
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;
            List<StockIssueReturnViewModel> issueReturns = new List<StockIssueReturnViewModel>();
            List<StockIssueReturnViewModel> objs = new List<StockIssueReturnViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (string.Compare(status, "All") == 0)
                    //objs = (from isr in db.tb_stock_issue_return
                    //                join iss in db.tb_stock_issue on isr.stock_issue_ref equals iss.stock_issue_id
                    //                orderby isr.issue_return_number
                    //                where isr.status == true
                    //                select new StockIssueReturnViewModel() {stock_issue_return_id=isr.stock_issue_return_id,issue_return_number=isr.issue_return_number,stock_issue_ref=isr.stock_issue_ref,stock_issue_number=iss.stock_issue_number,issue_return_status=isr.issue_return_status,created_date=isr.created_date }).ToList();
                    objs = (from isr in db.tb_stock_issue_return
                            join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                            orderby isr.issue_return_number
                            where isr.status == true
                            select new StockIssueReturnViewModel() { stock_issue_return_id = isr.stock_issue_return_id, issue_return_number = isr.issue_return_number, stock_issue_ref = isr.stock_issue_ref, stock_issue_number = iss.stock_transfer_no, issue_return_status = isr.issue_return_status, created_date = isr.created_date }).ToList();
                else
                    //objs = (from isr in db.tb_stock_issue_return
                    //                join iss in db.tb_stock_issue on isr.stock_issue_ref equals iss.stock_issue_id
                    //                orderby isr.issue_return_number
                    //                where isr.status == true && string.Compare(isr.issue_return_status,status)==0
                    //                select new StockIssueReturnViewModel() { stock_issue_return_id = isr.stock_issue_return_id, issue_return_number = isr.issue_return_number, stock_issue_ref = isr.stock_issue_ref, stock_issue_number = iss.stock_issue_number, issue_return_status = isr.issue_return_status, created_date = isr.created_date }).ToList();
                    objs = (from isr in db.tb_stock_issue_return
                            join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                            orderby isr.issue_return_number
                            where isr.status == true && string.Compare(isr.issue_return_status, status) == 0
                            select new StockIssueReturnViewModel() { stock_issue_return_id = isr.stock_issue_return_id, issue_return_number = isr.issue_return_number, stock_issue_ref = isr.stock_issue_ref, stock_issue_number = iss.stock_transfer_no, issue_return_status = isr.issue_return_status, created_date = isr.created_date }).ToList();
                foreach (var obj in objs)
                {
                    strWarehouse = string.Empty;
                    strInvoiceDate = string.Empty;
                    strInvoiceNumber = string.Empty;
                    var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                    var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    foreach (var wh in dupWarehouse)
                        strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                    foreach (var ivn in dupInvoiceNumber)
                        strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                    foreach (var ivd in dupInvoiceDate)
                        strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                    foreach(var returnItem in returnItems)
                    {
                        bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                        if(!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                        if(!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                        if(!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                    }
                    StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                    issueReturn.stock_issue_return_id = obj.stock_issue_return_id;
                    issueReturn.issue_return_number = obj.issue_return_number;
                    issueReturn.stock_issue_ref = obj.stock_issue_ref;
                    issueReturn.stock_issue_number = obj.stock_issue_number;
                  
                    issueReturn.issue_return_status = obj.issue_return_status;
                    issueReturn.created_date = obj.created_date;
                    issueReturn.strWarehouse = strWarehouse;
                    issueReturn.strInvoiceNumber = strInvoiceNumber;
                    issueReturn.strInvoiceDate = strInvoiceDate;
                    issueReturns.Add(issueReturn);
                }
            }
            return issueReturns;
        }

        public List<StockIssueReturnViewModel> GetStockIssueReturnList()
        {
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;
            List<StockIssueReturnViewModel> issueReturns = new List<StockIssueReturnViewModel>();
            List<StockIssueReturnViewModel> objs = new List<StockIssueReturnViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.OperationDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteStockKeeper))
                {
                    objs = (from isr in db.tb_stock_issue_return
                            join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                            orderby isr.issue_return_number
                            where isr.status == true
                            select new StockIssueReturnViewModel()
                            {
                                stock_issue_return_id = isr.stock_issue_return_id,
                                issue_return_number = isr.issue_return_number,
                                stock_issue_ref = isr.stock_issue_ref,
                                stock_issue_number = iss.stock_transfer_no,
                                issue_return_status = isr.issue_return_status,
                                created_date = isr.created_date,
                                created_by = isr.created_by,
                                received_status=isr.received_status,
                            }).ToList();

                    foreach (var obj in objs)
                    {
                        strWarehouse = string.Empty;
                        strInvoiceDate = string.Empty;
                        strInvoiceNumber = string.Empty;
                        var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                        var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        foreach (var wh in dupWarehouse)
                            strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                        foreach (var ivn in dupInvoiceNumber)
                            strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                        foreach (var ivd in dupInvoiceDate)
                            strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                        foreach (var returnItem in returnItems)
                        {
                            bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                            bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                            bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                            if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                            if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                            if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                        }
                        StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                        issueReturn.stock_issue_return_id = obj.stock_issue_return_id;
                        issueReturn.issue_return_number = obj.issue_return_number;
                        issueReturn.stock_issue_ref = obj.stock_issue_ref;
                        issueReturn.stock_issue_number = obj.stock_issue_number;
                        issueReturn.created_by = obj.created_by;
                        issueReturn.issue_return_status = obj.issue_return_status;
                        issueReturn.created_date = obj.created_date;
                        issueReturn.strWarehouse = strWarehouse;
                        issueReturn.strInvoiceNumber = strInvoiceNumber;
                        issueReturn.strInvoiceDate = strInvoiceDate;
                        issueReturn.received_status = obj.received_status;
                        issueReturn.receivedStatus = ClsItemReceive.GetReceivedNoteStatusbyRefId(issueReturn.stock_issue_return_id);
                        issueReturns.Add(issueReturn);
                    }
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    if (User.IsInRole(Role.Purchaser))
                    {
                        
                        objs = (from isr in db.tb_stock_issue_return
                                join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                                orderby isr.issue_return_number
                                where isr.status == true && string.Compare(isr.created_by,userid)==0
                                select new StockIssueReturnViewModel()
                                {
                                    stock_issue_return_id = isr.stock_issue_return_id,
                                    issue_return_number = isr.issue_return_number,
                                    stock_issue_ref = isr.stock_issue_ref,
                                    stock_issue_number = iss.stock_transfer_no,
                                    issue_return_status = isr.issue_return_status,
                                    created_date = isr.created_date,
                                    created_by = isr.created_by,
                                    received_status=isr.received_status,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            strWarehouse = string.Empty;
                            strInvoiceDate = string.Empty;
                            strInvoiceNumber = string.Empty;
                            var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                            var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            foreach (var wh in dupWarehouse)
                                strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                            foreach (var ivn in dupInvoiceNumber)
                                strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                            foreach (var ivd in dupInvoiceDate)
                                strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                            foreach (var returnItem in returnItems)
                            {
                                bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                                bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                                bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                                if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                                if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                                if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                            }
                            StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                            issueReturn.stock_issue_return_id = obj.stock_issue_return_id;
                            issueReturn.issue_return_number = obj.issue_return_number;
                            issueReturn.stock_issue_ref = obj.stock_issue_ref;
                            issueReturn.stock_issue_number = obj.stock_issue_number;
                            issueReturn.created_by = obj.created_by;
                            issueReturn.issue_return_status = obj.issue_return_status;
                            issueReturn.created_date = obj.created_date;
                            issueReturn.strWarehouse = strWarehouse;
                            issueReturn.strInvoiceNumber = strInvoiceNumber;
                            issueReturn.strInvoiceDate = strInvoiceDate;
                            issueReturn.received_status = obj.received_status;
                            issueReturn.receivedStatus = ClsItemReceive.GetReceivedNoteStatusbyRefId(issueReturn.stock_issue_return_id);
                            issueReturns.Add(issueReturn);
                        }
                    }

                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        objs = (from isr in db.tb_stock_issue_return
                                join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                                join mr in db.tb_item_request on iss.item_request_id equals mr.ir_id
                                join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                join wh in db.tb_warehouse on pro.site_id equals wh.warehouse_site_id
                                join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                orderby isr.issue_return_number
                                where isr.status == true &&(string.Compare(isr.last_approved_by, userid) == 0 || ((string.Compare(isr.issue_return_status,Status.Feedbacked)==0 ||string.Compare(isr.issue_return_status,Status.Pending)==0 )&& string.Compare(qaqc.qaqc_id,userid)==0))
                                select new StockIssueReturnViewModel()
                                {
                                    stock_issue_return_id = isr.stock_issue_return_id,
                                    issue_return_number = isr.issue_return_number,
                                    stock_issue_ref = isr.stock_issue_ref,
                                    stock_issue_number = iss.stock_transfer_no,
                                    issue_return_status = isr.issue_return_status,
                                    created_date = isr.created_date,
                                    created_by = isr.created_by,
                                    received_status=isr.received_status,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = issueReturns.Where(s => string.Compare(s.stock_issue_return_id, obj.stock_issue_return_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                            {
                                strWarehouse = string.Empty;
                                strInvoiceDate = string.Empty;
                                strInvoiceNumber = string.Empty;
                                var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                                var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                foreach (var wh in dupWarehouse)
                                    strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                                foreach (var ivn in dupInvoiceNumber)
                                    strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                                foreach (var ivd in dupInvoiceDate)
                                    strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                                foreach (var returnItem in returnItems)
                                {
                                    bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                                    bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                                    bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                                    if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                                    if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                                    if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                                }
                                StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                                issueReturn.stock_issue_return_id = obj.stock_issue_return_id;
                                issueReturn.issue_return_number = obj.issue_return_number;
                                issueReturn.stock_issue_ref = obj.stock_issue_ref;
                                issueReturn.stock_issue_number = obj.stock_issue_number;
                                issueReturn.created_by = obj.created_by;
                                issueReturn.issue_return_status = obj.issue_return_status;
                                issueReturn.created_date = obj.created_date;
                                issueReturn.strWarehouse = strWarehouse;
                                issueReturn.strInvoiceNumber = strInvoiceNumber;
                                issueReturn.strInvoiceDate = strInvoiceDate;
                                issueReturn.received_status = obj.received_status;
                                issueReturn.receivedStatus = ClsItemReceive.GetReceivedNoteStatusbyRefId(issueReturn.stock_issue_return_id);
                                issueReturns.Add(issueReturn);
                            }
                            
                        }
                    }

                    if (User.IsInRole(Role.SiteManager))
                    {
                        objs = (from isr in db.tb_stock_issue_return
                                join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                                join mr in db.tb_item_request on iss.item_request_id equals mr.ir_id
                                join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                orderby isr.issue_return_number
                                where isr.status == true && (string.Compare(isr.checked_by, userid) == 0 || (string.Compare(isr.issue_return_status, Status.Approved) == 0&& string.Compare(sm.site_manager, userid) == 0))
                                select new StockIssueReturnViewModel()
                                {
                                    stock_issue_return_id = isr.stock_issue_return_id,
                                    issue_return_number = isr.issue_return_number,
                                    stock_issue_ref = isr.stock_issue_ref,
                                    stock_issue_number = iss.stock_transfer_no,
                                    issue_return_status = isr.issue_return_status,
                                    created_date = isr.created_date,
                                    created_by = isr.created_by,
                                    received_status=isr.received_status,

                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = issueReturns.Where(s => string.Compare(s.stock_issue_return_id, obj.stock_issue_return_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                            {
                                strWarehouse = string.Empty;
                                strInvoiceDate = string.Empty;
                                strInvoiceNumber = string.Empty;
                                var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                                var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                foreach (var wh in dupWarehouse)
                                    strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                                foreach (var ivn in dupInvoiceNumber)
                                    strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                                foreach (var ivd in dupInvoiceDate)
                                    strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                                foreach (var returnItem in returnItems)
                                {
                                    bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                                    bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                                    bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                                    if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                                    if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                                    if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                                }
                                StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                                issueReturn.stock_issue_return_id = obj.stock_issue_return_id;
                                issueReturn.issue_return_number = obj.issue_return_number;
                                issueReturn.stock_issue_ref = obj.stock_issue_ref;
                                issueReturn.stock_issue_number = obj.stock_issue_number;
                                issueReturn.created_by = obj.created_by;
                                issueReturn.issue_return_status = obj.issue_return_status;
                                issueReturn.created_date = obj.created_date;
                                issueReturn.strWarehouse = strWarehouse;
                                issueReturn.strInvoiceNumber = strInvoiceNumber;
                                issueReturn.strInvoiceDate = strInvoiceDate;
                                issueReturn.received_status = obj.received_status;
                                issueReturn.receivedStatus = ClsItemReceive.GetReceivedNoteStatusbyRefId(issueReturn.stock_issue_return_id);
                                issueReturns.Add(issueReturn);
                            }

                        }
                    }

                }
            }
            issueReturns = issueReturns.OrderByDescending(s => s.created_date).ToList();
            return issueReturns;
        }
        public List<StockIssueReturnViewModel> GetStockReturnListItemsDaterangeAndStatus(string dateRange,string status)
        {
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;

            string[] splitDateRanges = dateRange.Split('-');
            DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
            DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);


            List<StockIssueReturnViewModel> issueReturns = new List<StockIssueReturnViewModel>();
            List<StockIssueReturnViewModel> objs = new List<StockIssueReturnViewModel>();
            List<StockReturnFilterResultModel> results = new List<StockReturnFilterResultModel>();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.OperationDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin) || User.IsInRole(Role.ProjectManager) || User.IsInRole(Role.SiteStockKeeper) || User.IsInRole(Role.Purchaser))
                {
                    var oobjs = (from isr in db.tb_stock_issue_return
                            join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                            orderby isr.issue_return_number
                            where isr.status == true && isr.created_date>=startDate && isr.created_date<=endDate
                            select new StockReturnFilterResultModel()
                            {
                                iss=iss,
                                isr=isr
                            }).ToList();
                    results.AddRange(oobjs);
                    
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    //if (User.IsInRole(Role.Purchaser))
                    //{

                    //    var oobjs = (from isr in db.tb_stock_issue_return
                    //            join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                    //            orderby isr.issue_return_number
                    //            where isr.status == true && string.Compare(isr.created_by, userid) == 0 && isr.created_date >= startDate && isr.created_date <= endDate
                    //                 select new StockReturnFilterResultModel()
                    //            {
                    //                isr=isr,
                    //                iss=iss
                    //            }).ToList();
                    //    results.AddRange(oobjs);
                    //}

                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        var oobjs = (from isr in db.tb_stock_issue_return
                                join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                                join mr in db.tb_item_request on iss.item_request_id equals mr.ir_id
                                join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                join wh in db.tb_warehouse on pro.site_id equals wh.warehouse_site_id
                                join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                orderby isr.issue_return_number
                                where isr.status == true && (string.Compare(isr.last_approved_by, userid) == 0 || ((string.Compare(isr.issue_return_status, Status.Feedbacked) == 0 || string.Compare(isr.issue_return_status, Status.Pending) == 0) && string.Compare(qaqc.qaqc_id, userid) == 0))
                                && isr.created_date >= startDate && isr.created_date <= endDate
                                     select new StockReturnFilterResultModel()
                                {
                                    isr=isr,
                                    iss=iss,
                                }).ToList();
                        results.AddRange(oobjs);

                    }

                    if (User.IsInRole(Role.SiteManager))
                    {
                        var obs = (from isr in db.tb_stock_issue_return
                                join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                                join mr in db.tb_item_request on iss.item_request_id equals mr.ir_id
                                join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                orderby isr.issue_return_number
                                where isr.status == true && (string.Compare(isr.checked_by, userid) == 0 || (string.Compare(isr.issue_return_status, Status.Approved) == 0 && string.Compare(sm.site_manager, userid) == 0))
                                && isr.created_date >= startDate && isr.created_date <= endDate
                                   select new StockReturnFilterResultModel()
                                {
                                    isr=isr,
                                    iss=iss

                                }).ToList();
                        results.AddRange(obs);
    
                    }

                }
                results = results.DistinctBy(s => s.isr).ToList();
                if (string.Compare(status,"0")!=0)
                {
                    results = results.Where(s => string.Compare(s.isr.issue_return_status, status) == 0).ToList();
                }

                foreach (var obj in results.DistinctBy(s => s.isr).ToList())
                {
                    strWarehouse = string.Empty;
                    strInvoiceDate = string.Empty;
                    strInvoiceNumber = string.Empty;
                    var returnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, obj.isr.stock_issue_return_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceNumber = x.invoice_number, invoiceDate = x.invoice_date }).ToList();
                    var dupWarehouse = returnItems.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceNumber = returnItems.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    var dupInvoiceDate = returnItems.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    foreach (var wh in dupWarehouse)
                        strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, wh) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                    foreach (var ivn in dupInvoiceNumber)
                        strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                    foreach (var ivd in dupInvoiceDate)
                        strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                    foreach (var returnItem in returnItems)
                    {
                        bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, returnItem.warehouseId) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, returnItem.invoiceNumber) == 0).Count() > 0 ? true : false;
                        bool isDupInvoiceDate = dupInvoiceDate.Where(x => x == returnItem.invoiceDate).Count() > 0 ? true : false;
                        if (!isDupWarehouse) strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, returnItem.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault());
                        if (!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, returnItem.invoiceNumber);
                        if (!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(returnItem.invoiceDate).ToString("dd/MM/yyyy"));
                    }
                    StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                    issueReturn.stock_issue_return_id = obj.isr.stock_issue_return_id;
                    issueReturn.issue_return_number = obj.isr.issue_return_number;
                    issueReturn.stock_issue_ref = obj.isr.stock_issue_ref;
                    issueReturn.stock_issue_number = obj.iss.stock_transfer_no;
                    issueReturn.created_by = obj.isr.created_by;
                    issueReturn.issue_return_status = obj.isr.issue_return_status;
                    issueReturn.created_date = obj.isr.created_date;
                    issueReturn.strWarehouse = strWarehouse;
                    issueReturn.strInvoiceNumber = strInvoiceNumber;
                    issueReturn.strInvoiceDate = strInvoiceDate;
                    issueReturn.received_status = obj.isr.received_status==null?string.Empty:string.Format("<span class='label label-danger'>{0}</span>", obj.isr.received_status);
                    issueReturn.receivedStatus = ClsItemReceive.GetReceivedNoteStatusbyRefId(issueReturn.stock_issue_return_id);
                    issueReturn.created_at_text = CommonClass.ToLocalTime(Convert.ToDateTime(issueReturn.created_date)).ToString("dd/MM/yyyy");
                    issueReturn.created_by_text = CommonClass.GetUserFullnameByUserId(issueReturn.created_by);
                    issueReturn.show_status = ShowStatus.GetStockReturnShowStatus(issueReturn.issue_return_status);
                    issueReturns.Add(issueReturn);
                }

            }

            

            issueReturns = issueReturns.OrderByDescending(s => s.created_date).ToList();
            return issueReturns;
        }

        public ActionResult GetStockReturnListItemsDaterangeAndStatusAJAX(string dateRange,string status)
        {
            return Json(new { data = GetStockReturnListItemsDaterangeAndStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }

        public string GetIssueReturnNumber()
        {
            string issueReturnNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_stock_issue_return orderby tbl.created_date descending select tbl.issue_return_number).FirstOrDefault();
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
                issueReturnNo = "SIR-" + yy + "-" + mm + "-" + last_no;
            }
            return issueReturnNo;
        }
        public StockIssueReturnViewModel GetIssueReturnDetail(string id)
        {
            StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //issueReturn = (from isr in db.tb_stock_issue_return
                //               join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_issue_id
                //               where string.Compare(isr.stock_issue_return_id, id) == 0
                //               select new StockIssueReturnViewModel() { stock_issue_return_id=isr.stock_issue_return_id,issue_return_number=isr.issue_return_number,stock_issue_ref=isr.stock_issue_ref,stock_issue_number=iss.stock_issue_number,issue_return_status=isr.issue_return_status,created_date=isr.created_date }).FirstOrDefault();
                //issueReturn.inventories = Inventory.GetInventoryItem(issueReturn.stock_issue_return_id);
                //issueReturn.inventoryDetails = this.GetInventoryDetail(issueReturn.stock_issue_return_id,issueReturn.stock_issue_ref);
                //issueReturn.rejects = CommonClass.GetRejectByRequest(id);

                issueReturn = (from isr in db.tb_stock_issue_return
                               join iss in db.tb_stock_transfer_voucher on isr.stock_issue_ref equals iss.stock_transfer_id
                               where string.Compare(isr.stock_issue_return_id, id) == 0
                               select new StockIssueReturnViewModel() { stock_issue_return_id = isr.stock_issue_return_id, issue_return_number = isr.issue_return_number, stock_issue_ref = isr.stock_issue_ref, stock_issue_number = iss.stock_transfer_no, issue_return_status = isr.issue_return_status, created_date = isr.created_date }).FirstOrDefault();
                issueReturn.inventories = Inventory.GetInventoryItem(issueReturn.stock_issue_return_id);
                issueReturn.inventoryDetails = this.GetInventoryDetail(issueReturn.stock_issue_return_id, issueReturn.stock_issue_ref);
                issueReturn.rejects = CommonClass.GetRejectByRequest(id);
                issueReturn.processWorkflows = ProcessWorkflowModel.GetProcessWorkflowByRefId(issueReturn.stock_issue_return_id);

            }
            return issueReturn;
        }
        public ActionResult GetIssueItems(string id, string itemId, string returnId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                StockIssueReturnViewModel issueReturn = new StockIssueReturnViewModel();
                List<InventoryViewModel> inventories = new List<InventoryViewModel>();
                

                inventories = Inventory.GetInventoryItemByIID(id, itemId);
                var search = (from ivd in db.tb_inventory_detail where ivd.inventory_ref_id == returnId select ivd).ToList();
                inventories = (from s in search join i in inventories on s.inventory_item_id equals i.product_id where s.inventory_warehouse_id != i.warehouse_id select new InventoryViewModel {
                    inventory_id = i.inventory_id,
                    product_id = i.product_id,
                    itemUnitName = db.tb_product.Find(i.product_id).product_name,
                    itemCode = db.tb_product.Find(i.product_id).product_code,
                    in_quantity = i.in_quantity,
                    out_quantity = i.out_quantity,
                    invoice_date = Convert.ToDateTime(i.invoice_date),
                    invoice_number = i.invoice_number,
                    remark = i.remark,
                    unit = i.itemUnit,
                    itemName = i.itemName,
                    warehouse_id = i.warehouse_id,
                    warehouseName = db.tb_warehouse.Find(i.warehouse_id).warehouse_name,
                    itemUnit= i.itemUnit,
                    unitName = db.tb_unit.Find(i.itemUnit).Name,
                    uom = db.tb_multiple_uom.Where(x => x.product_id == i.product_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),

            }).ToList();
                if (inventories.Count() == 0)
                {
                    inventories = Inventory.GetInventoryItemByIID(id, itemId);
                   
                    if (inventories.Count() == 0)
                    {
                        return Json(new { result = "error", data = inventories }, JsonRequestBehavior.AllowGet);
                    }
                }
              
                return Json(new { result = "success", data = inventories }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<InventoryDetailViewModel> GetInventoryDetail(string id,string refId)
        {
            List<InventoryDetailViewModel> inventoryItemDetails = new List<InventoryDetailViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //var find = (from str in db.tb_stock_issue_return join stv in db.tb_stock_transfer_voucher on str.stock_issue_ref equals stv.stock_transfer_id
                //            join std in db.tb_stock_transfer_detail on stv.stock_transfer_id equals std.st_ref_id
                //            join ivtd in db.tb_inventory_detail on str.stock_issue_return_id equals ivtd.inventory_ref_id
                //            where 


                //            )
                //detail editted
                //var search = (from ivd in db.tb_inventory_detail where ivd.inventory_ref_id == id select ivd).ToList();
                ////stock transfer
                //var find = (from str in db.tb_stock_issue_return
                //            join stv in db.tb_stock_transfer_voucher on str.stock_issue_ref equals stv.stock_transfer_id
                //            join std in db.tb_stock_transfer_detail on stv.stock_transfer_id equals std.st_ref_id
                //            where str.stock_issue_return_id == id && str.stock_issue_ref == refId
                //            select new { std }).ToList();
                //foreach (var s in search)
                //{
                //    foreach (var f in find)
                //    {
                //        if (s.inventory_item_id == f.std.st_item_id && s.inventory_warehouse_id == f.std.st_warehouse_id && s.quantity != 0 && s.quantity != s.remain_quantity)
                //        {
                //            InventoryDetailViewModel inventoryDetail = new InventoryDetailViewModel();
                //            inventoryDetail.inventory_detail_id = s.inventory_detail_id;
                //            inventoryDetail.inventory_ref_id = s.inventory_ref_id;
                //            inventoryDetail.inventory_item_id = s.inventory_item_id;
                //            inventoryDetail.itemCode = db.tb_product.Find(s.inventory_item_id).product_code;
                //            inventoryDetail.itemName = db.tb_product.Find(s.inventory_item_id).product_name;
                //            inventoryDetail.inventory_item_id = s.inventory_item_id;
                //            inventoryDetail.warehouseName = db.tb_warehouse.Find(s.inventory_warehouse_id).warehouse_name;
                //            inventoryDetail.inventory_warehouse_id = s.inventory_warehouse_id;
                //            inventoryDetail.quantity = s.quantity;
                //            inventoryDetail.stock_balance = s.quantity;
                //            inventoryDetail.unit = s.unit;
                //            inventoryDetail.itemunitname = db.tb_unit.Find(s.unit).Name;
                //            inventoryDetail.item_status = s.item_status;
                //            inventoryDetail.invoice_date = s.invoice_date;
                //            inventoryDetail.invoice_number = s.invoice_number;
                //            inventoryDetail.uom = db.tb_multiple_uom.Where(x => x.product_id == s.inventory_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            
                //            inventoryItemDetails.Add(inventoryDetail);

                //        }

                //    }
                //}


                var inventoryDetails = (from invd in db.tb_inventory_detail
                                        join item in db.tb_product on invd.inventory_item_id equals item.product_id
                                        join warehouse in db.tb_warehouse on invd.inventory_warehouse_id equals warehouse.warehouse_id
                                        orderby invd.ordering_number
                                        where invd.inventory_ref_id == id
                                        select new InventoryDetailViewModel()
                                        {
                                            inventory_detail_id = invd.inventory_detail_id,
                                            inventory_ref_id = invd.inventory_ref_id,
                                            inventory_item_id = invd.inventory_item_id,
                                            ordering_number = invd.ordering_number,
                                            itemCode = item.product_code,
                                            itemName = item.product_name,
                                            itemUnit = item.product_unit,
                                            //itemunitname = db.tb_unit.Find(item.product_unit).Name,
                                            inventory_warehouse_id = invd.inventory_warehouse_id,
                                            warehouseName = warehouse.warehouse_name,
                                            quantity = invd.quantity,
                                            remark = invd.remark,
                                            unit = invd.unit,
                                            item_status = invd.item_status,
                                            invoice_date = invd.invoice_date,
                                            invoice_number = invd.invoice_number
                                        }).ToList();
                foreach (var inventory in inventoryDetails)
                {
                    //decimal issueQty = Convert.ToDecimal(db.tb_inventory_detail.Where(x => x.inventory_item_id == inventory.inventory_item_id && x.inventory_warehouse_id == inventory.inventory_warehouse_id && x.inventory_ref_id==refId).Select(x => x.quantity).FirstOrDefault());
                    decimal issueQty = Convert.ToDecimal(db.tb_inventory.Where(x => x.product_id == inventory.inventory_item_id && x.warehouse_id == inventory.inventory_warehouse_id && x.ref_id == refId).Select(x => x.out_quantity).FirstOrDefault());
                    issueQty = issueQty == null ? 0 : issueQty;
                    InventoryDetailViewModel inventoryDetail = new InventoryDetailViewModel();
                    inventoryDetail.inventory_detail_id = inventory.inventory_detail_id;
                    inventoryDetail.inventory_ref_id = inventory.inventory_ref_id;
                    inventoryDetail.inventory_item_id = inventory.inventory_item_id;
                    inventoryDetail.itemCode = inventory.itemCode;
                    inventoryDetail.itemName = inventory.itemName;
                    inventoryDetail.ordering_number = inventory.ordering_number;
                    inventoryDetail.itemUnit = inventory.itemUnit;
                    inventoryDetail.inventory_warehouse_id = inventory.inventory_warehouse_id;
                    inventoryDetail.warehouseName = inventory.warehouseName;
                    inventoryDetail.quantity = inventory.quantity;
                    inventoryDetail.remark = inventory.remark;
                    inventoryDetail.stock_balance = issueQty;
                    inventoryDetail.unit = inventory.unit;
                    inventoryDetail.itemunitname = db.tb_unit.Find(inventory.unit).Name;
                    inventoryDetail.uom = db.tb_multiple_uom.Where(x => x.product_id == inventory.inventory_item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                    inventoryDetail.item_status = inventory.item_status;
                    inventoryDetail.invoice_number = inventory.invoice_number;
                    inventoryDetail.invoice_date = inventory.invoice_date;
                    inventoryItemDetails.Add(inventoryDetail);
                }
            }
            return inventoryItemDetails;
        }
    
        public List<InventoryDetailViewModel> isStockIssueExist(string issueId)
        {
            List<InventoryDetailViewModel> remain = new List<InventoryDetailViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var issueReturns = db.tb_stock_issue_return.Where(x => x.stock_issue_ref == issueId && x.issue_return_status=="Completed").ToList();
                if (issueReturns.Any())
                {
                    
                    List<InventoryDetailViewModel> stockIssues = new List<InventoryDetailViewModel>();
                    List<InventoryViewModel> stockIssueReturns = new List<InventoryViewModel>();
                    stockIssues = Inventory.GetInventoryDetail(issueId);
                    foreach (var issueReturn in issueReturns) { 
                        List<InventoryViewModel> issueItems = new List<InventoryViewModel>();
                        issueItems = Inventory.GetInventoryItem(issueReturn.stock_issue_return_id);
                        foreach(InventoryViewModel item in issueItems)
                        {
                            stockIssueReturns.Add(item);
                        }
                    }
                    foreach(InventoryDetailViewModel issue in stockIssues)
                    {
                        foreach(InventoryViewModel returnn in stockIssueReturns)
                        {
                            if (issue.inventory_item_id == returnn.product_id && issue.inventory_warehouse_id == returnn.warehouse_id)
                            {
                                issue.quantity = issue.quantity - returnn.in_quantity;
                            }
                        }
                        remain.Add(new InventoryDetailViewModel() { inventory_item_id = issue.inventory_item_id, inventory_warehouse_id = issue.inventory_warehouse_id, quantity = issue.quantity });
                    }
                }
            }
            return remain;
        }
        public void DeleteIssueReturnDetail(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory_detail.Where(x => x.inventory_ref_id == id).ToList();
                foreach (var inventory in inventories)
                {
                    var inventoryID = inventory.inventory_detail_id;
                    tb_inventory_detail inv = db.tb_inventory_detail.Where(x => x.inventory_detail_id == inventoryID).FirstOrDefault();
                    db.tb_inventory_detail.Remove(inv);
                    db.SaveChanges();
                }
            }
        }
        public string InsertItemInventory(string id)
        {
            List<InventoryDetailViewModel> remain = new List<InventoryDetailViewModel>();
            int countInvalid = 0;
            string msg = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var invs = (from dInv in db.tb_inventory_detail
                                   join pro in db.tb_product on dInv.inventory_item_id equals pro.product_id
                                   join wh in db.tb_warehouse on dInv.inventory_warehouse_id equals wh.warehouse_id
                                   where dInv.inventory_ref_id == id
                                   select new { dInv, pro, wh }).ToList();
                string stockIssueId = db.tb_stock_issue_return.Where(x => x.stock_issue_return_id == id).Select(m => m.stock_issue_ref).FirstOrDefault().ToString();
                remain = this.isStockIssueExist(stockIssueId);
                foreach (InventoryDetailViewModel item in remain)
                {
                    foreach (var issue in invs)
                    {
                        if (item.inventory_item_id == issue.dInv.inventory_item_id && item.inventory_warehouse_id == issue.dInv.inventory_warehouse_id && item.quantity < CommonClass.ConvertMultipleUnit(issue.dInv.inventory_item_id, issue.dInv.unit, Convert.ToDecimal(issue.dInv.quantity)))
                        {
                            countInvalid++;
                            if (item.quantity == 0)
                                msg = msg + " " + issue.pro.product_name + " in " + issue.wh.warehouse_name + " is completed to issue return\n";
                            else
                                msg = msg + " " + issue.pro.product_name + " in " + issue.wh.warehouse_name + " remain quantity=" + item.quantity + " to issue return\n";
                            break;
                        }
                    }
                }
                if (countInvalid > 0)
                    return msg;

                var inventories = db.tb_inventory_detail.Where(m => m.inventory_ref_id == id).ToList();
                foreach (var inv in inventories)
                {
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    tb_inventory inventory = new tb_inventory();
                    inventory.inventory_id = Guid.NewGuid().ToString();
                    inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    inventory.ref_id = id;
                    inventory.inventory_status_id = "5";
                    inventory.warehouse_id = inv.inventory_warehouse_id;
                    inventory.product_id = inv.inventory_item_id;
                    
                    inventory.out_quantity = 0;
                    inventory.in_quantity =CommonClass.ConvertMultipleUnit(inv.inventory_item_id,inv.unit,Convert.ToDecimal(inv.quantity));
                    inventory.total_quantity = totalQty + inventory.in_quantity;
                    db.tb_inventory.Add(inventory);
                    db.SaveChanges();
                }
            }
            return msg;
        }
    }
}
