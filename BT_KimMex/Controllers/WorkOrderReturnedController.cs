using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;
using BT_KimMex.Models;

namespace BT_KimMex.Controllers
{
    public class WorkOrderReturnedController : Controller
    {
        // GET: WorkOrderReturned
        public ActionResult Index()
        {
            return View(this.GetWorkOrderReturnListItems());
        }

        // GET: WorkOrderReturned/Details/5
        public ActionResult Details(string id)
        {
            return View(WorkorderReturnedViewModel.GetWorkOrderReturnItem(id));
        }

        // GET: WorkOrderReturned/Create
        public ActionResult Create()
        {
            ViewBag.WorkorderIssuedListItems = this.GetWorkOrderedIssuesListItems();
            ViewBag.WorkorderReturnedNumber= CommonClass.GenerateProcessNumber("WOR");
            return View();
        }

        // POST: WorkOrderReturned/Create
        [HttpPost]
        public ActionResult Create(WorkorderReturnedViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                kim_mexEntities db = new kim_mexEntities();
                tb_workorder_returned orderReturned = new tb_workorder_returned();
                orderReturned.workorder_returned_id = Guid.NewGuid().ToString();
                orderReturned.workorder_returned_number= CommonClass.GenerateProcessNumber("WOR");
                orderReturned.workorder_issued_id = model.workorder_issued_id;
                orderReturned.workorder_returned_status = Status.Pending;
                orderReturned.status = true;
                orderReturned.created_by = User.Identity.GetUserId();
                orderReturned.created_date = DateTime.Now;
                orderReturned.updated_by = User.Identity.GetUserId();
                orderReturned.updated_date = DateTime.Now;
                db.tb_workorder_returned.Add(orderReturned);

                CommonClass.AutoGenerateStockInvoiceNumber(orderReturned.workorder_returned_id, model.inventories);
                foreach (var inv in model.inventories)
                {
                    if (!string.IsNullOrEmpty(inv.warehouse_id) && !string.IsNullOrEmpty(inv.product_id) && inv.total_quantity >= Class.CommonClass.ConvertMultipleUnit(inv.product_id, inv.unit, Convert.ToDecimal(inv.in_quantity))) //&& inv.total_quantity >= inv.in_quantity
                    {
                        tb_inventory_detail inventoryDetail = new tb_inventory_detail();
                        inventoryDetail.inventory_detail_id = Guid.NewGuid().ToString();
                        inventoryDetail.inventory_ref_id = orderReturned.workorder_returned_id;
                        inventoryDetail.inventory_item_id = inv.product_id;
                        inventoryDetail.inventory_warehouse_id = inv.warehouse_id;
                        inventoryDetail.quantity = inv.in_quantity;
                        inventoryDetail.remark = inv.remark;
                        inventoryDetail.unit = inv.unit;
                        inventoryDetail.item_status = Status.Pending;
                        inventoryDetail.invoice_date = inv.invoice_date == null ? Class.CommonClass.ToLocalTime(DateTime.Now) : inv.invoice_date;
                        inventoryDetail.invoice_number = CommonClass.GetInvoiceNumber(orderReturned.workorder_returned_id, inventoryDetail.inventory_warehouse_id, inventoryDetail.invoice_date);
                        inventoryDetail.inventory_type = "5";
                        db.tb_inventory_detail.Add(inventoryDetail);
                        db.SaveChanges();
                    }
                }
                WorkorderReturnedViewModel.RollbackWorkOrderIssueItems(orderReturned.workorder_issued_id, orderReturned.workorder_returned_id, false);

                return Json(new { result = "success" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { result = "error", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: WorkOrderReturned/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WorkOrderReturned/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: WorkOrderReturned/Delete/5
        public ActionResult CancelRequest(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_workorder_returned orderReturned = db.tb_workorder_returned.Find(id);
                orderReturned.workorder_returned_status = Status.RequestCancelled;
                orderReturned.updated_by = User.Identity.GetUserId();
                orderReturned.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                WorkorderReturnedViewModel.RollbackWorkOrderIssueItems(orderReturned.workorder_issued_id, orderReturned.workorder_returned_id, true);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: WorkOrderReturned/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Approval(string id,string status,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {

                tb_workorder_returned orderReturn = db.tb_workorder_returned.Find(id);
                orderReturn.workorder_returned_status =string.Compare(status,Status.Approved)==0?Status.Checked:Status.CheckRejected;
                orderReturn.checked_by = User.Identity.GetUserId();
                orderReturn.checked_date = DateTime.Now;
                orderReturn.checked_comment = comment;
                db.SaveChanges();
                if (string.Compare(status, Status.Rejected) == 0)
                {
                    WorkorderReturnedViewModel.RollbackWorkOrderIssueItems(orderReturn.workorder_issued_id, orderReturn.workorder_returned_id, true);
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = orderReturn.workorder_returned_id;
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
        public ActionResult Completed(string id,string status,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_workorder_returned orderReturn = db.tb_workorder_returned.Find(id);
                orderReturn.workorder_returned_status = string.Compare(status, Status.Approved) ==0? Status.Completed : Status.Rejected;
                orderReturn.completed_comment = comment;
                orderReturn.completed_by = User.Identity.GetUserId();
                orderReturn.completed_date = DateTime.Now;
                db.SaveChanges();

                if (string.Compare(orderReturn.workorder_returned_status, Status.Completed) == 0)
                {
                    //insert to inventory here

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
                        inventory.in_quantity = CommonClass.ConvertMultipleUnit(inv.inventory_item_id, inv.unit, Convert.ToDecimal(inv.quantity));
                        inventory.total_quantity = totalQty + inventory.in_quantity;
                        db.tb_inventory.Add(inventory);
                        db.SaveChanges();
                    }

                }
                else
                {
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = orderReturn.workorder_returned_id;
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
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            return View(WorkorderReturnedViewModel.GetWorkOrderReturnItem(id));
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<InventoryDetailViewModel> models)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                int countApproved = 0;
                foreach(InventoryDetailViewModel item in models)
                {
                    string iid = item.inventory_detail_id;
                    tb_inventory_detail inventoryDetail = db.tb_inventory_detail.Find(iid);
                    inventoryDetail.item_status = item.item_status;
                    inventoryDetail.remark = item.remark;
                    db.SaveChanges();

                    if (string.Compare(item.item_status, Status.Approved) == 0)
                    {
                        countApproved++;
                    }
                }

                tb_workorder_returned workOrderReturn = db.tb_workorder_returned.Find(id);
                workOrderReturn.workorder_returned_status = countApproved == models.Count() ? Status.Approved : Status.PendingFeedback;
                workOrderReturn.approved_by = User.Identity.GetUserId();
                workOrderReturn.approved_date = DateTime.Now;
                db.SaveChanges();

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<StockIssueViewModel> GetWorkOrderedIssuesListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (User.IsInRole(Role.SystemAdmin))
                    return db.tb_stock_issue.OrderByDescending(s => s.created_date).Where(s => s.status == true && string.Compare(s.stock_issue_status, Status.Completed) == 0 && s.is_completed==false).Select(s => new StockIssueViewModel() {stock_issue_id=s.stock_issue_id,stock_issue_number=s.stock_issue_number }).ToList();
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    List<StockIssueViewModel> models = (from issue in db.tb_stock_issue
                                                        join project in db.tb_project on issue.project_id equals project.project_id
                                                        //join site in db.tb_site on project.site_id equals site.site_id
                                                        join sitsup in db.tbSiteSiteSupervisors on project.project_id equals sitsup.site_id
                                                     where issue.status == true && string.Compare(issue.stock_issue_status, Status.Completed) == 0 && string.Compare(sitsup.site_supervisor_id, userid) == 0 && issue.is_completed==false
                                                        select new StockIssueViewModel()
                                                     {
                                                         stock_issue_id=issue.stock_issue_id,
                                                         stock_issue_number=issue.stock_issue_number,
                                                     }).ToList();
                    return models;
                }
            }
            return new List<StockIssueViewModel>();
        }
        public List<WorkorderReturnedViewModel> GetWorkOrderReturnListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<WorkorderReturnedViewModel> models = new List<WorkorderReturnedViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                {
                    models = (from returned in db.tb_workorder_returned
                              join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                              join pro in db.tb_project on issued.project_id equals pro.project_id
                              //join site in db.tb_site on pro.site_id equals site.site_id
                              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                              orderby returned.created_date descending
                              where returned.status == true
                              select new WorkorderReturnedViewModel()
                              {
                                  workorder_returned_id = returned.workorder_returned_id,
                                  workorder_issued_id = returned.workorder_issued_id,
                                  workorder_returned_number = returned.workorder_returned_number,
                                  workorder_issued_number = issued.stock_issue_number,
                                  workorder_returned_status = returned.workorder_returned_status,
                                  created_by = returned.created_by,
                                  created_date = returned.updated_date,
                                  project_id = pro.project_id,
                                  project_fullname = pro.project_full_name,
                                  warehouse_id = wh.warehouse_id,
                                  warehouse_name = wh.warehouse_name
                              }).ToList();
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    List<WorkorderReturnedViewModel> objs = new List<WorkorderReturnedViewModel>();
                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        objs = (from returned in db.tb_workorder_returned
                                  join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                  join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                orderby returned.created_date descending
                                  where returned.status == true && string.Compare(returned.created_by,userid)==0
                                  select new WorkorderReturnedViewModel()
                                  {
                                      workorder_returned_id = returned.workorder_returned_id,
                                      workorder_issued_id = returned.workorder_issued_id,
                                      workorder_returned_number = returned.workorder_returned_number,
                                      workorder_issued_number = issued.stock_issue_number,
                                      workorder_returned_status = returned.workorder_returned_status,
                                      created_by = returned.created_by,
                                      created_date = returned.updated_date,
                                      project_id = pro.project_id,
                                      project_fullname = pro.project_full_name,
                                      warehouse_id = wh.warehouse_id,
                                      warehouse_name = wh.warehouse_name
                                  }).ToList();
                        foreach (var obj in objs)
                            models.Add(obj);
                    }
                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                orderby returned.created_date descending
                                where returned.status == true && ( (string.Compare(qaqc.qaqc_id, userid) == 0 && string.Compare(returned.workorder_returned_status,Status.Pending)==0) || string.Compare(returned.approved_by,userid)==0)
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                    workorder_issued_id = returned.workorder_issued_id,
                                    workorder_returned_number = returned.workorder_returned_number,
                                    workorder_issued_number = issued.stock_issue_number,
                                    workorder_returned_status = returned.workorder_returned_status,
                                    created_by = returned.created_by,
                                    created_date = returned.updated_date,
                                    project_id = pro.project_id,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if(!isExist)
                            models.Add(obj);
                        }
                            
                    }
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                                orderby returned.created_date descending
                                where returned.status == true && ((string.Compare(sitestock.stock_keeper, userid) == 0 && string.Compare(returned.workorder_returned_status, Status.Approved) == 0) || string.Compare(returned.checked_by, userid) == 0)
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                    workorder_issued_id = returned.workorder_issued_id,
                                    workorder_returned_number = returned.workorder_returned_number,
                                    workorder_issued_number = issued.stock_issue_number,
                                    workorder_returned_status = returned.workorder_returned_status,
                                    created_by = returned.created_by,
                                    created_date = returned.updated_date,
                                    project_id = pro.project_id,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }

                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                orderby returned.created_date descending
                                where returned.status == true && ((string.Compare(sm.site_manager, userid) == 0 && string.Compare(returned.workorder_returned_status, Status.Checked) == 0) || string.Compare(returned.completed_by, userid) == 0)
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                    workorder_issued_id = returned.workorder_issued_id,
                                    workorder_returned_number = returned.workorder_returned_number,
                                    workorder_issued_number = issued.stock_issue_number,
                                    workorder_returned_status = returned.workorder_returned_status,
                                    created_by = returned.created_by,
                                    created_date = returned.updated_date,
                                    project_id = pro.project_id,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join sm in db.tb_project_pm on pro.project_id equals sm.project_id
                                orderby returned.created_date descending
                                where returned.status == true && string.Compare(sm.project_manager_id,userid)==0
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                    workorder_issued_id = returned.workorder_issued_id,
                                    workorder_returned_number = returned.workorder_returned_number,
                                    workorder_issued_number = issued.stock_issue_number,
                                    workorder_returned_status = returned.workorder_returned_status,
                                    created_by = returned.created_by,
                                    created_date = returned.updated_date,
                                    project_id = pro.project_id,
                                    project_fullname = pro.project_full_name,
                                    warehouse_id = wh.warehouse_id,
                                    warehouse_name = wh.warehouse_name
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                }
                models = models.OrderByDescending(s => s.created_date).ToList();
                return models;
            }
        }
        
    }
}
