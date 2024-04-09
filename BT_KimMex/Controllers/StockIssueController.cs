using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Eco.UmlCodeAttributes;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class StockIssueController : Controller
    {
        // GET: StockIssue
        public ActionResult Index()
        {
            return View(StockIssueViewModel.GetStockIssueList(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.SiteSupervisor), User.IsInRole(Role.SiteStockKeeper), User.IsInRole(Role.SiteManager), User.Identity.GetUserId().ToString()));
            //return View();
        }

        // GET: StockIssue/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueViewModel model = new StockIssueViewModel();
            model = StockIssueViewModel.GetWorkOrderIssueDetail(id);
            
            
            return View(model);
        }
        // GET: StockIssue/Details/5
        public ActionResult EditNew(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueViewModel model = new StockIssueViewModel();
            model = this.GetStockIssueEditNew(id);
            kim_mexEntities db = new kim_mexEntities();
            foreach(var ress in model.inventoryDetails)
            {
                model.inventorieshistoryqty = (from h in db.tb_history_issue_qty
                                               where ress.inventory_detail_id == h.inventory_detail_id
                                               select new InventoryViewModel()
                                               {
                                                   history_quantity = h.issue_qty,
                                                   inventory_detail_id = h.inventory_detail_id
                                               }).ToList();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditNew(StockIssueViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                foreach (var inv in model.inventories)
                {
                    tb_inventory inventory = db.tb_inventory.Find(inv.inventory_id);
                    //if(inv.history_quantity > inventory.total_quantity)
                    //{
                    //    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                    //}
                    //inventory.total_quantity = inventory.total_quantity - inv.history_quantity;
                    //db.SaveChanges();.

                    tb_inventory_detail inventory_Detail = db.tb_inventory_detail.Find(inv.inventory_detail_id);
                    //if (inventory.total_quantity < (inventory_Detail.quantity + inv.history_quantity))
                    //{
                    //    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                    //}
                    //inventory_Detail.quantity = inventory_Detail.quantity + inv.history_quantity;
                    if (inventory.total_quantity < inv.history_quantity)
                    {
                        return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                    }
                    inventory_Detail.quantity =inventory_Detail.quantity+inv.history_quantity;
                    inventory_Detail.remain_quantity = inventory_Detail.remain_quantity + inv.history_quantity;
                    inventory_Detail.item_status = Status.Pending;
                    inventory_Detail.inventory_type = "2";
                    db.SaveChanges();
                    tb_stock_issue stockIssue = db.tb_stock_issue.Find(model.stock_issue_id);
                    stockIssue.stock_issue_status = Status.Pending;
                    db.SaveChanges();

                    tb_history_issue_qty HistoryQty = new tb_history_issue_qty();
                    HistoryQty.history_issue_qty_id = Guid.NewGuid().ToString();
                    HistoryQty.inventory_detail_id = inv.inventory_detail_id;
                    HistoryQty.issue_qty = inv.history_quantity;
                    HistoryQty.create_at = DateTime.Now;
                    HistoryQty.status = true;
                    HistoryQty.updated_by = User.Identity.GetUserId().ToString();
                    db.tb_history_issue_qty.Add(HistoryQty);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public StockIssueViewModel GetStockIssueEditNew(string id)
        {
            StockIssueViewModel stockIssue = new StockIssueViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                stockIssue = db.tb_stock_issue.Where(x => x.stock_issue_id == id).Select(x => new StockIssueViewModel() { stock_issue_id = x.stock_issue_id, stock_issue_number = x.stock_issue_number, created_date = x.created_date, stock_issue_status = x.stock_issue_status, created_by = x.created_by, project_id = x.project_id }).FirstOrDefault();
                stockIssue.inventories = Inventory.GetInventoryItem(stockIssue.stock_issue_id);
                stockIssue.inventoryDetails = Inventory.GetInventoryDetail(stockIssue.stock_issue_id);
                //foreach (var nn in stockIssue.inventoryDetails)
                //{
                //    stockIssue.inventorieshistoryqty = (from h in db.tb_history_issue_qty
                //                                            //join invD in db.tb_inventory_detail on h.inventory_detail_id equals invD.inventory_detail_id
                //                                            //join inv in db.tb_inventory on invD.inventory_ref_id equals inv.inventory_id
                //                                        where nn.inventory_detail_id == h.inventory_detail_id
                //                                        select new InventoryViewModel()
                //                                        {
                //                                            history_quantity = h.issue_qty,
                //                                            inventory_detail_id = h.inventory_detail_id
                //                                        }
                //                                        ).ToList();
                //}
                stockIssue.attachments = Inventory.GetAttachments(stockIssue.stock_issue_id);
                stockIssue.rejects = CommonClass.GetRejectByRequest(id);
                stockIssue.project_name = db.tb_project.Find(stockIssue.project_id).project_full_name;
                stockIssue.warehouse = (from pro in db.tb_project
                                            //join site in db.tb_site on pro.site_id equals site.site_id
                                        join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                        from wh in pwh.DefaultIfEmpty()
                                        where string.Compare(pro.project_id, stockIssue.project_id) == 0
                                        select new WareHouseViewModel() { warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name }).FirstOrDefault();
            }
            return stockIssue;
        }
   


        // GET: StockIssue/Create
        public ActionResult Create()
        {
            //ViewBag.StockIssueNumber = this.GetStockIssueNumber();
            ViewBag.StockIssueNumber = CommonClass.GenerateProcessNumber("WOI");
            ViewBag.Projects = ProjectViewModel.GetProjectListItemsBySiteSupervisor(User.IsInRole(Role.SystemAdmin),User.Identity.GetUserId());
            return View();
        }

        // POST: StockIssue/Create
        [HttpPost]
        public ActionResult Create(StockIssueViewModel model,List<string> Attachment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                if (model.inventories.Count == 0)
                    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error" ,message="Duplicate Item and Warehouse."},JsonRequestBehavior.AllowGet);
                }
                tb_stock_issue stockIssue = new tb_stock_issue();
                stockIssue.stock_issue_id = Guid.NewGuid().ToString();
                stockIssue.stock_issue_number = CommonClass.GenerateProcessNumber("WOI") ;
                stockIssue.project_id = model.project_id;
                stockIssue.status = true;
                stockIssue.stock_issue_status = Status.Pending;
                stockIssue.created_by = User.Identity.GetUserId().ToString();
                stockIssue.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                stockIssue.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_stock_issue.Add(stockIssue);
                db.SaveChanges();
                CommonClass.AutoGenerateStockInvoiceNumber(stockIssue.stock_issue_id, model.inventories);
                foreach(var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id)) //&& inv.total_quantity > inv.out_quantity
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockIssue.stock_issue_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(stockIssue.stock_issue_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.remain_quantity = Class.CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit, Convert.ToDecimal(inv.out_quantity));
                        inventoryDetail.item_status = Status.Pending;
                        inventoryDetail.inventory_type = "2";
                        inventoryDetail.ordering_number = inventoryDetail.ordering_number;
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();

                        //kosal
                        //block by sissy
                        //var invid = (from p in db.tb_product
                        //             join invv in db.tb_inventory on p.product_id equals invv.product_id
                        //             where p.product_id == inv.product_id

                        //             select new {p ,invv }).FirstOrDefault();

                        //tb_inventory inventory = db.tb_inventory.Find(invid.invv.inventory_id);
                        //inventory.total_quantity = inventory.total_quantity - inventoryDetail.quantity;
                        //db.SaveChanges();
                        //add stock issue block qty
                        //this.InsertItemInventory(inv.product_id, "9", true);

                        tb_history_issue_qty HistoryQty = new tb_history_issue_qty();
                        HistoryQty.history_issue_qty_id = Guid.NewGuid().ToString();
                        HistoryQty.inventory_detail_id = inventoryDetail.inventory_detail_id;
                        HistoryQty.issue_qty = inventoryDetail.quantity;
                        HistoryQty.create_at =CommonClass.ToLocalTime(DateTime.Now);
                        HistoryQty.status = true;
                        HistoryQty.updated_by = User.Identity.GetUserId().ToString();
                        db.tb_history_issue_qty.Add(HistoryQty);
                        db.SaveChanges();
                        //end
                    }
                }
                //add stock issue block qty
                this.InsertItemInventory(stockIssue.stock_issue_id, "9", true);

                if (Attachment != null && Attachment.Count() > 0)
                {
                    foreach(string att in Attachment)
                    {
                        tb_attachment attachment = db.tb_attachment.Where(x => x.attachment_id == att).FirstOrDefault();
                        attachment.attachment_ref_id = stockIssue.stock_issue_id;
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error",message=ex.Message },JsonRequestBehavior.AllowGet);
            }
        }

        // GET: StockIssue/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueViewModel model = new StockIssueViewModel();
            model = StockIssueViewModel.GetWorkOrderIssueDetail(id);
            ViewBag.WarehouseID = Inventory.GetWarehousesList();
            return View(model);
        }

        // POST: StockIssue/Edit/5
        [HttpPost]
        public ActionResult Edit(StockIssueViewModel model,List<string> Attachment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                if (model.inventories.Count == 0)
                    return Json(new { result = "error", message = "No Item Issue." }, JsonRequestBehavior.AllowGet);
                var dupItem = model.inventories.GroupBy(x => new { x.product_id, x.warehouse_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplicate Item and Warehouse." }, JsonRequestBehavior.AllowGet);
                }
                string id = model.stock_issue_id;
                tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                stockIssue.stock_issue_number = model.stock_issue_number;
                stockIssue.status = true;
                stockIssue.stock_issue_status = Status.Pending;
                stockIssue.updated_by = User.Identity.GetUserId().ToString();
                stockIssue.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(model.stock_issue_id))
                {
                    string damageId = model.stock_issue_id;
                    this.DeleteStockIssueDetail(damageId);
                    CommonClass.DeleteInvoiceNumber(damageId);
                    CommonClass.AutoGenerateStockInvoiceNumber(damageId, model.inventories);
                }
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockIssue.stock_issue_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(stockIssue.stock_issue_id,inventoryDetail.inventory_warehouse_id,inventoryDetail.invoice_date);
                        inventoryDetail.remain_quantity =Class.CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id,inventoryDetail.unit,Convert.ToDecimal(inv.out_quantity));
                        inventoryDetail.item_status = Status.Pending;
                        inventoryDetail.inventory_type = "2";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                if (Attachment != null && Attachment.Count > 0)
                {
                    foreach(string att in Attachment)
                    {
                        tb_attachment attachment = db.tb_attachment.Where(x => x.attachment_id == att).FirstOrDefault();
                        attachment.attachment_ref_id = stockIssue.stock_issue_id;
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet) ;
            }
            catch
            {
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: StockIssue/Delete/5
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                    stockIssue.status = false;
                    stockIssue.updated_by = User.Identity.GetUserId();
                    stockIssue.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approve(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                stockIssue.stock_issue_status = Status.Completed;
                stockIssue.completed_by = User.Identity.GetUserId();
                stockIssue.completed_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                this.InsertItemInventory(id, "10", false);
                this.InsertItemInventory(id,"2");
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }        
        public ActionResult Reject(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue issue = db.tb_stock_issue.Find(id);
                issue.stock_issue_status = Status.Rejected;
                issue.completed_by = User.Identity.GetUserId();
                issue.completed_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                issue.completed_comment = comment;
                db.SaveChanges();
                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = issue.stock_issue_id;
                reject.ref_type = "Stock Issue";
                reject.comment = comment;
                reject.rejected_by = User.Identity.GetUserId();
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_reject.Add(reject);
                db.SaveChanges();

                //return back issue block qty back
                this.InsertItemInventory(id, "10", false);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            StockIssueViewModel model = new StockIssueViewModel();
            model = StockIssueViewModel.GetWorkOrderIssueDetail(id);
            kim_mexEntities db = new kim_mexEntities();
            foreach (var ress in model.inventoryDetails)
            {
                model.inventorieshistoryqty = (from h in db.tb_history_issue_qty
                                               where ress.inventory_detail_id == h.inventory_detail_id
                                               select new InventoryViewModel()
                                               {
                                                   history_quantity = h.issue_qty,
                                                   inventory_detail_id = h.inventory_detail_id
                                               }).ToList();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<InventoryDetailViewModel> models)
        {
            try
            {
                int countApproved = 0;
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
                        //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(x => string.Compare(x.warehouse_id, inventoryDetail.inventory_warehouse_id) == 0 && string.Compare(x.product_id, inventoryDetail.inventory_item_id) == 0).Select(x => x.total_quantity).FirstOrDefault());
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.ref_id = id;
                        //inventory.inventory_status_id = "1";
                        //inventory.warehouse_id = inventoryDetail.inventory_warehouse_id;
                        //inventory.product_id = inventoryDetail.inventory_item_id;
                        //inventory.out_quantity = CommonClass.ConvertMultipleUnit(inventoryDetail.inventory_item_id, inventoryDetail.unit,Convert.ToDecimal(inventoryDetail.quantity));
                        //inventory.in_quantity = 0;
                        //inventory.total_quantity = totalQty - inventory.out_quantity;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();
                    }
                }
                tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                stockIssue.stock_issue_status = countApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                stockIssue.approved_by = User.Identity.GetUserId();
                stockIssue.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
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
            StockIssueViewModel model = new StockIssueViewModel();
            model = StockIssueViewModel.GetWorkOrderIssueDetail(id);
            ViewBag.WarehouseID = Inventory.GetWarehousesList();
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<InventoryViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                stockIssue.stock_issue_status = models.Count() == 0 ?Status.Approved : Status.Feedbacked;
                stockIssue.updated_by = User.Identity.GetUserId();
                stockIssue.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                var oldStockIssue = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0 && string.Compare(x.item_status, Status.Feedbacked) == 0).Select(x=>x.inventory_detail_id).ToList();
                foreach(var oId in oldStockIssue)
                {
                    this.InsertItemInventoryByInventoryDetail(id, oId, "10", false);

                    tb_inventory_detail detail = db.tb_inventory_detail.Find(oId);
                    db.tb_inventory_detail.Remove(detail);
                    db.SaveChanges();
                }
                foreach (var inv in models)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id))
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = stockIssue.stock_issue_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.out_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.invoice_number = inv.invoice_number;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now).Date : inv.invoice_date;
                        inventoryDetail.remain_quantity = inv.out_quantity;
                        inventoryDetail.item_status = "pending";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();

                        tb_history_issue_qty HistoryQty = new tb_history_issue_qty();
                        HistoryQty.history_issue_qty_id = Guid.NewGuid().ToString();
                        HistoryQty.inventory_detail_id = inventoryDetail.inventory_detail_id;
                        HistoryQty.issue_qty = inventoryDetail.quantity;
                        HistoryQty.create_at = CommonClass.ToLocalTime(DateTime.Now);
                        HistoryQty.status = true;
                        HistoryQty.updated_by = User.Identity.GetUserId().ToString();
                        db.tb_history_issue_qty.Add(HistoryQty);
                        db.SaveChanges();

                        this.InsertItemInventoryByInventoryDetail(id,inventoryDetail.inventory_detail_id, "9");
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetInventoryItem(string itemId, string warehouseId)
        {
           
            InventoryViewModel inventory = new InventoryViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                inventory = (from inv in db.tb_inventory
                             join item in db.tb_product on inv.product_id equals item.product_id
                             join unit in db.tb_unit on item.product_unit equals unit.Id
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
                                 out_quantity = inv.out_quantity,
                                 unitName=unit.Name,
                             }).FirstOrDefault();
            }
            return Json(new { result = "success", data = inventory }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockIssueDataTable(string status)
        {
            List<StockIssueViewModel> stockIssue = new List<StockIssueViewModel>();
            stockIssue = StockIssueViewModel.GetStockIssueList(User.IsInRole(Role.SystemAdmin),User.IsInRole(Role.SiteSupervisor), User.IsInRole(Role.SiteStockKeeper), User.IsInRole(Role.SiteManager),User.Identity.GetUserId().ToString(),status);
            return Json(new { data = stockIssue }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockIssueDataList(string id, string returnId = null)
        {
            List<InventoryViewModel> inventories = new List<InventoryViewModel>();
            //inventories = Inventory.GetInventoryItem(id);
            inventories = StockIssueReturn.GetStockIssueItemsReferences1(id, returnId);
            return Json(new { result = "success", data = inventories }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadAttachment()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_attachment attachment = new tb_attachment();
                var file = Request.Files[0];
                if(file!=null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    attachment.attachment_id = file_id;
                    attachment.attachment_name = file_name;
                    attachment.attachment_extension = file_extension;
                    attachment.attachment_path = file_path;
                    attachment.attachment_ref_type = "Stock Issue";
                    db.tb_attachment.Add(attachment);
                    db.SaveChanges();
                }
                return Json(new { result = "success", attachment_id = attachment.attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        [HttpPost]
        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_attachment attachment = db.tb_attachment.Find(id);
                if (attachment == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_attachment.Remove(attachment);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment.attachment_id + attachment.attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public string GetStockIssueNumber()
        {
            string stockIssueNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_stock_issue orderby tbl.created_date descending select tbl.stock_issue_number).FirstOrDefault();
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
                stockIssueNo = "SI-" + yy + "-" + mm + "-" + last_no;
            }
            return stockIssueNo;
        }
        
        //public StockIssueViewModel GetStockIssueDetail(string id)
        //{
        //    StockIssueViewModel stockIssue = new StockIssueViewModel();
        //    using(kim_mexEntities db=new kim_mexEntities())
        //    {
        //        stockIssue = db.tb_stock_issue.Where(x => x.stock_issue_id == id).Select(x => new StockIssueViewModel() { stock_issue_id=x.stock_issue_id,stock_issue_number=x.stock_issue_number,created_date=x.created_date,stock_issue_status=x.stock_issue_status,created_by=x.created_by,project_id=x.project_id }).FirstOrDefault();
        //        stockIssue.inventories = Inventory.GetInventoryItem(stockIssue.stock_issue_id);
        //        stockIssue.inventoryDetails = Inventory.GetInventoryDetail(stockIssue.stock_issue_id);
        //        stockIssue.attachments = Inventory.GetAttachments(stockIssue.stock_issue_id);
        //        stockIssue.rejects = CommonClass.GetRejectByRequest(id);
        //        stockIssue.project_name = db.tb_project.Find(stockIssue.project_id).project_full_name;
        //        stockIssue.warehouse = (from pro in db.tb_project
        //                                //join site in db.tb_site on pro.site_id equals site.site_id
        //                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
        //                                where string.Compare(pro.project_id, stockIssue.project_id) == 0
        //                                select new WareHouseViewModel() { warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name }).FirstOrDefault();
        //    }
        //    return stockIssue;
        //}
        public void DeleteStockIssueDetail(string id)
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

        public void InsertItemInventory(string id,string inv_status,bool isIssue=true)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inventories = db.tb_inventory_detail.Where(m => m.inventory_ref_id == id).ToList();
                foreach (var inv in inventories)
                {

                    //var hisqtdy = (from hq in db.tb_history_issue_qty
                    //               orderby hq.create_at
                    //               where hq.inventory_detail_id == inv.inventory_detail_id
                    //               select hq).ToList();
                    //foreach (var re in hisqtdy)
                    //{
                    //    IList<string> strList = new List<string>() { re.issue_qty.ToString() };
                    //    var c = strList.Last();
                    //}

                    var hisqty = db.tb_history_issue_qty.OrderBy(m => m.create_at).Where(m => m.inventory_detail_id == inv.inventory_detail_id).Select(b => b.issue_qty).ToList();
                    var qtyhis = hisqty.Last();
                    //inv.quantity = qtyhis;
                    decimal quantity = 0;
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    string inventoryUnit = Regex.Replace(db.tb_product.Where(m => m.product_id == inv.inventory_item_id).Select(m => m.product_unit).FirstOrDefault(), @"\t|\n|\r", "");
                    if (string.Compare(inventoryUnit, inv.unit) == 0)
                        quantity = Convert.ToDecimal(qtyhis);
                    else
                    {
                        var uom = db.tb_multiple_uom.Where(m => m.product_id == inv.inventory_item_id).FirstOrDefault();
                        if (uom != null)
                        {
                            if (uom.uom1_id != null && uom.uom1_qty != null)
                            {
                                string uom1 = Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom1) == 0)
                                {
                                    quantity = Convert.ToDecimal(qtyhis / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom2_id != null && uom.uom2_qty != null)
                            {
                                string uom2 = Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom2) == 0)
                                {
                                    quantity = Convert.ToDecimal((qtyhis / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom3_id != null && uom.uom3_qty != null)
                            {
                                string uom3 = Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom3) == 0)
                                {
                                    quantity = Convert.ToDecimal(((qtyhis / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom4_id != null && uom.uom4_qty != null)
                            {
                                string uom4 = Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom4) == 0)
                                {
                                    quantity = Convert.ToDecimal((((qtyhis / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom5_id != null && uom.uom5_qty != null)
                            {
                                string uom5 = Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom5) == 0)
                                {
                                    quantity = Convert.ToDecimal(((((qtyhis / uom.uom5_qty) / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                        }
                    }
                    if (quantity <= totalQty)
                    {
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = id;
                        inventory.inventory_status_id = inv_status;
                        inventory.warehouse_id = inv.inventory_warehouse_id;
                        inventory.product_id = inv.inventory_item_id;
                        if (isIssue)
                        {
                            inventory.out_quantity = quantity;
                            inventory.in_quantity = 0;
                            inventory.total_quantity = totalQty - quantity;
                        }else
                        {
                            inventory.out_quantity = 0;
                            inventory.in_quantity = quantity;
                            inventory.total_quantity = totalQty + quantity;
                        }
                        
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }
                }
            }
        }

        public void InsertItemInventoryByInventoryDetail(string iid,string id,string inv_status,bool isIssue = true)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var inv = db.tb_inventory_detail.Find(id);
                if (inv != null)
                {
                    var hisqty = db.tb_history_issue_qty.OrderBy(m => m.create_at).Where(m => m.inventory_detail_id == inv.inventory_detail_id).Select(b => b.issue_qty).ToList();
                    var qtyhis = hisqty.Last();
                    //inv.quantity = qtyhis;
                    decimal quantity = 0;
                    decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                    string inventoryUnit = Regex.Replace(db.tb_product.Where(m => m.product_id == inv.inventory_item_id).Select(m => m.product_unit).FirstOrDefault(), @"\t|\n|\r", "");
                    if (string.Compare(inventoryUnit, inv.unit) == 0)
                        quantity = Convert.ToDecimal(qtyhis);
                    else
                    {
                        var uom = db.tb_multiple_uom.Where(m => m.product_id == inv.inventory_item_id).FirstOrDefault();
                        if (uom != null)
                        {
                            if (uom.uom1_id != null && uom.uom1_qty != null)
                            {
                                string uom1 = Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom1) == 0)
                                {
                                    quantity = Convert.ToDecimal(qtyhis / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom2_id != null && uom.uom2_qty != null)
                            {
                                string uom2 = Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom2) == 0)
                                {
                                    quantity = Convert.ToDecimal((qtyhis / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom3_id != null && uom.uom3_qty != null)
                            {
                                string uom3 = Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom3) == 0)
                                {
                                    quantity = Convert.ToDecimal(((qtyhis / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom4_id != null && uom.uom4_qty != null)
                            {
                                string uom4 = Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom4) == 0)
                                {
                                    quantity = Convert.ToDecimal((((qtyhis / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                            else if (uom.uom5_id != null && uom.uom5_qty != null)
                            {
                                string uom5 = Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                                if (string.Compare(inv.unit, uom5) == 0)
                                {
                                    quantity = Convert.ToDecimal(((((qtyhis / uom.uom5_qty) / uom.uom4_qty) / uom.uom3_qty) / uom.uom2_qty) / uom.uom1_qty);
                                }
                            }
                        }
                    }
                    if (quantity <= totalQty)
                    {
                        tb_inventory inventory = new tb_inventory();
                        inventory.inventory_id = Guid.NewGuid().ToString();
                        inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inventory.ref_id = iid;
                        inventory.inventory_status_id = inv_status;
                        inventory.warehouse_id = inv.inventory_warehouse_id;
                        inventory.product_id = inv.inventory_item_id;
                        if (isIssue)
                        {
                            inventory.out_quantity = quantity;
                            inventory.in_quantity = 0;
                            inventory.total_quantity = totalQty - quantity;
                        }
                        else
                        {
                            inventory.out_quantity = 0;
                            inventory.in_quantity = quantity;
                            inventory.total_quantity = totalQty + quantity;
                        }

                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }
                }            
            }
        }

        public ActionResult GetIssueItems(string id, string itemId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<InventoryViewModel> inventories = new List<InventoryViewModel>();
                inventories = Inventory.GetInventoryItemByIID(id,itemId);
                return Json(new { result = "success", data = inventories }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<ProjectViewModel> GetProjectListItemsBySiteSupervisor()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (User.IsInRole(Role.SystemAdmin))
                {
                    return db.tb_project.OrderBy(s => s.project_full_name).Where(s => s.project_status == true && string.Compare(s.p_status, "Active") == 0).Select(s => new ProjectViewModel() { project_id=s.project_id,project_full_name=s.project_full_name }).ToList();
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    return (from pro in db.tb_project
                            //join site in db.tb_site on pro.site_id equals site.site_id
                            join sitesupv in db.tbSiteSiteSupervisors on pro.project_id equals sitesupv.site_id
                            where pro.project_status == true && string.Compare(pro.p_status, "Active") == 0 && string.Compare(sitesupv.site_supervisor_id, userid) == 0
                            select new ProjectViewModel()
                            {
                                project_id=pro.project_id,
                                project_full_name=pro.project_full_name
                            }).ToList();
                }
            }
            
        }
        public ActionResult GetWorkOrderIssuedDetailJson(string id)
        {
            return Json(new { data = StockIssueViewModel.GetWorkOrderIssueDetail(id) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetWorkOrderIssueDataTable(string dateRange, string status,string projectId)
        {
            return Json(new {data=StockIssueViewModel.GetWorkOrderIssueListByDaterangeProjectStatus(dateRange,status,projectId, User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.SiteSupervisor), User.IsInRole(Role.SiteStockKeeper), User.IsInRole(Role.SiteManager), User.Identity.GetUserId().ToString())},JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelRequest(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_stock_issue issue = db.tb_stock_issue.Find(id);
                issue.stock_issue_status = Status.RequestCancelled;
                issue.updated_by = User.Identity.GetUserId();
                issue.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                this.InsertItemInventory(id, "10", false);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult PlanningWorkOrderList()
        {
            return View();
        }
        [Authorize]
        public ActionResult PlanningWorkOrder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SubmitPlanningWorkorderAJAX(PostWorkorderPlanningModel model)
        {
            AJAXResultModel result = new AJAXResultModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var currentOrderNumber = PostWorkorderPlanningModel.GetWorkorderIssuePlanningLatestNumber();
                var dates = new List<DateTime>();

                var duplicationItems = model.planningItems.GroupBy(s => s.item_id).Where(s=>s.ToList().Count()>1).Select(s => new {key=s.Key,list=s.ToList(),item=s.FirstOrDefault()}).ToList();
                if (duplicationItems.Count() > 0)
                {
                    result.isSuccess = false;
                    result.message = "Duplicate item:\n";
                    foreach(var dup in duplicationItems)
                    {
                        result.message = string.Format("{0}-{1}\n", result.message, dup.item.item_description);
                    }
                }
                else
                {
                    for (var dt = model.date_from; dt <= model.date_to; dt = Convert.ToDateTime(dt).AddDays(1))
                    {
                        dates.Add(Convert.ToDateTime(dt));
                    }
                    foreach (var date in dates)
                    {
                        foreach (var item in model.planningItems)
                        {
                            tb_workorder_planning woiPlanning = new tb_workorder_planning();
                            woiPlanning.woi_planning_id = Guid.NewGuid().ToString();
                            woiPlanning.ordering = currentOrderNumber;
                            woiPlanning.project_id = model.project_id;
                            woiPlanning.date_from = date;
                            woiPlanning.date_to = date;
                            woiPlanning.item_id = item.item_id;
                            woiPlanning.item_unit_id = item.item_unit_id;
                            woiPlanning.planning_qty = item.planning_qty;
                            woiPlanning.labour_hour = item.labour_hour;
                            woiPlanning.is_active = true;
                            woiPlanning.created_by = User.Identity.GetUserId();
                            woiPlanning.updated_by = User.Identity.GetUserId();
                            woiPlanning.created_at = CommonClass.ToLocalTime(DateTime.Now);
                            woiPlanning.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                            db.tb_workorder_planning.Add(woiPlanning);
                            db.SaveChanges();
                        }
                    }
                }

                    

            }catch(Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.Message;
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PlanningWorkOrderListAJAX(string dateRange,string projectId)
        {
            return Json(new { data = WorkorderPlanningModel.GetWorkorderPlanningListByDateRangeProject(dateRange, projectId) }, JsonRequestBehavior.AllowGet);
        }
    }
}
