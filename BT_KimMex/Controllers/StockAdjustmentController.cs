using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using BT_KimMex.Models;
using MoreLinq;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class StockAdjustmentController : Controller
    {
        // GET: StockAdjustment
        public ActionResult Index()
        {
            List<StockAdjustmentViewModel> models = new List<StockAdjustmentViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.OperationDirector))
                //{
                //    models = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true).Select(x => new Models.StockAdjustmentViewModel()
                //    {
                //        stock_adjustment_id = x.stock_adjustment_id,
                //        stock_adjuctment_code = x.stock_adjuctment_code,
                //        warehouse_id = x.warehouse_id,
                //        warehouse_name = db.tb_warehouse.Where(w => string.Compare(w.warehouse_id, x.warehouse_id) == 0).Select(w => w.warehouse_name).FirstOrDefault().ToString(),
                //        comment = x.comment,
                //        stock_adjustment_status = x.stock_adjustment_status,
                //        created_date = x.updated_date,
                //        created_by = x.created_by
                //    }).ToList();
                //}
                //}else if (User.IsInRole(Role.SiteSupervisor) || User.IsInRole(Role.SiteAdmin))
                //{

                //}
                if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.OperationDirector))
                {
                    string userid = User.Identity.GetUserId().ToString();
                    List<StockAdjustmentViewModel> objs = new List<StockAdjustmentViewModel>();
                    objs = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true).Select(x => new Models.StockAdjustmentViewModel()
                    {
                        stock_adjustment_id = x.stock_adjustment_id,
                        stock_adjuctment_code = x.stock_adjuctment_code,
                        warehouse_id = x.warehouse_id,
                        warehouse_name = db.tb_warehouse.Where(w => string.Compare(w.warehouse_id, x.warehouse_id) == 0).Select(w => w.warehouse_name).FirstOrDefault().ToString(),
                        comment = x.comment,
                        stock_adjustment_status = x.stock_adjustment_status,
                        created_date = x.updated_date,
                        created_by = x.created_by
                    }).ToList();
                    foreach (var obj in objs)
                    {
                        var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                        if (!isExist)
                            models.Add(obj);
                    }


                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    List<StockAdjustmentViewModel> objs = new List<StockAdjustmentViewModel>();
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        models = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true && string.Compare(x.created_by, userid) == 0).Select(x => new Models.StockAdjustmentViewModel()
                        {
                            stock_adjustment_id = x.stock_adjustment_id,
                            stock_adjuctment_code = x.stock_adjuctment_code,
                            warehouse_id = x.warehouse_id,
                            warehouse_name = db.tb_warehouse.Where(w => string.Compare(w.warehouse_id, x.warehouse_id) == 0).Select(w => w.warehouse_name).FirstOrDefault().ToString(),
                            comment = x.comment,
                            stock_adjustment_status = x.stock_adjustment_status,
                            created_date = x.updated_date,
                            created_by = x.created_by
                        }).ToList();
                    }
                    if (User.IsInRole(Role.QAQCOfficer))
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                join qaqc in db.tb_warehouse_qaqc on x.warehouse_id equals qaqc.warehouse_id
                                where x.status == true && (((string.Compare(x.stock_adjustment_status, Status.Pending) == 0 || string.Compare(x.stock_adjustment_status, Status.Feedbacked) == 0) && string.Compare(qaqc.qaqc_id, userid) == 0) || string.Compare(x.approved_by, userid) == 0)
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                    stock_adjuctment_code = x.stock_adjuctment_code,
                                    warehouse_id = x.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                    comment = x.comment,
                                    stock_adjustment_status = x.stock_adjustment_status,
                                    created_date = x.updated_date,
                                    created_by = x.created_by
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }

                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                //join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                                where x.status == true && ((string.Compare(x.stock_adjustment_status, Status.Approved) == 0 && string.Compare(sm.site_manager, userid) == 0) || string.Compare(x.checked_by, userid) == 0)
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                    stock_adjuctment_code = x.stock_adjuctment_code,
                                    warehouse_id = x.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                    comment = x.comment,
                                    stock_adjustment_status = x.stock_adjustment_status,
                                    created_date = x.updated_date,
                                    created_by = x.created_by
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                //join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                                where x.status == true && ((string.Compare(x.stock_adjustment_status, Status.Checked) == 0 && string.Compare(pm.project_manager_id, userid) == 0) || string.Compare(x.completed_by, userid) == 0)
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                    stock_adjuctment_code = x.stock_adjuctment_code,
                                    warehouse_id = x.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                    comment = x.comment,
                                    stock_adjustment_status = x.stock_adjustment_status,
                                    created_date = x.updated_date,
                                    created_by = x.created_by
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }

                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join sup in db.tbSiteSiteSupervisors on proj.project_id equals sup.site_id
                                where x.status == true && string.Compare(sup.site_supervisor_id, userid) == 0
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                    stock_adjuctment_code = x.stock_adjuctment_code,
                                    warehouse_id = x.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                    comment = x.comment,
                                    stock_adjustment_status = x.stock_adjustment_status,
                                    created_date = x.updated_date,
                                    created_by = x.created_by
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join sa in db.tb_site_site_admin on proj.project_id equals sa.site_id
                                where x.status == true && string.Compare(sa.site_admin_id, userid) == 0
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                    stock_adjuctment_code = x.stock_adjuctment_code,
                                    warehouse_id = x.warehouse_id,
                                    warehouse_name = wh.warehouse_name,
                                    comment = x.comment,
                                    stock_adjustment_status = x.stock_adjustment_status,
                                    created_date = x.updated_date,
                                    created_by = x.created_by
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }


                }
            }
            models = models.OrderByDescending(o => o.created_date).ToList();
                return View(models);
        }
        public ActionResult Create()
        {
            kim_mexEntities db = new kim_mexEntities();
            string userid = User.Identity.GetUserId();
            List<Models.WareHouseViewModel> warehouses = new List<Models.WareHouseViewModel>();
            if (User.IsInRole(Role.SystemAdmin))
            {
                //warehouses = Class.CommonClass.Warehouses();
                warehouses = (from wh in db.tb_warehouse
                              join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                              where wh.warehouse_status == true && pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 
                              select new WareHouseViewModel()
                              {
                                  warehouse_id = wh.warehouse_id,
                                  warehouse_name = wh.warehouse_name,
                              }).ToList();
            }
                
            else if (User.IsInRole(Role.SiteStockKeeper))
            {
                warehouses = (from wh in db.tb_warehouse
                              join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                              join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                              where wh.warehouse_status == true && pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(sitestock.stock_keeper, userid) == 0
                              select new WareHouseViewModel()
                              {
                                  warehouse_id=wh.warehouse_id,
                                  warehouse_name=wh.warehouse_name,
                              }).ToList();
            }
            ViewBag.Warehouses =new SelectList(warehouses, "warehouse_id", "warehouse_name");
            //ViewBag.StockAdjustmentCode = this.GetStockAdjustmentNo();
            ViewBag.StockAdjustmentCode = Class.CommonClass.GenerateProcessNumber("SA");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.StockAdjustmentViewModel model)
        {
            try
            {
                var dupItems = model.items.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItems.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplication item to adjust." }, JsonRequestBehavior.AllowGet);
                }
                Entities.kim_mexEntities db = new Entities.kim_mexEntities();
                Entities.tb_stock_adjustment stockAdjustment = new Entities.tb_stock_adjustment();
                stockAdjustment.stock_adjustment_id = Guid.NewGuid().ToString();
                stockAdjustment.stock_adjuctment_code = Class.CommonClass.GenerateProcessNumber("SA");
                stockAdjustment.warehouse_id = model.warehouse_id;
                stockAdjustment.stock_adjustment_status = Class.Status.Pending;
                stockAdjustment.status = true;
                stockAdjustment.created_by = User.Identity.GetUserId();
                stockAdjustment.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                stockAdjustment.updated_by = User.Identity.GetUserId();
                stockAdjustment.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_stock_adjustment.Add(stockAdjustment);
                db.SaveChanges();

                foreach(Models.InventoryViewModel item in model.items)
                {
                    Entities.tb_inventory_detail inv = new Entities.tb_inventory_detail();
                    inv.inventory_detail_id = Guid.NewGuid().ToString();
                    inv.inventory_ref_id = stockAdjustment.stock_adjustment_id;
                    inv.inventory_item_id = item.product_id;
                    inv.inventory_warehouse_id = stockAdjustment.warehouse_id;
                    inv.quantity = item.out_quantity;
                    inv.previous_quantity = item.total_quantity;
                    inv.item_status = "pending";
                    inv.reason = item.remark;
                    db.tb_inventory_detail.Add(inv);
                    db.SaveChanges();
                }

                return Json(new { result = "success" },JsonRequestBehavior.AllowGet);
                    }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            Models.StockAdjustmentViewModel model = new Models.StockAdjustmentViewModel();
            model = this.GetStockAdjustmentDetail(id);
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            Models.StockAdjustmentViewModel model = new Models.StockAdjustmentViewModel();
            model = this.GetStockAdjustmentDetail(id);
            List<Models.WareHouseViewModel> warehouses = new List<Models.WareHouseViewModel>();
            warehouses = Class.CommonClass.Warehouses();
            ViewBag.Warehouses = warehouses;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(string id,Models.StockAdjustmentViewModel model)
        {
            try
            {
                var dupItems = model.items.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItems.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplication item to adjust." }, JsonRequestBehavior.AllowGet);
                }
                Entities.kim_mexEntities db = new Entities.kim_mexEntities();
                Entities.tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.warehouse_id = model.warehouse_id;
                stockAdjustment.updated_by = User.Identity.Name;
                stockAdjustment.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                this.RemoveItemInventoryDetail(id, "pending");
                foreach (Models.InventoryViewModel item in model.items)
                {
                    Entities.tb_inventory_detail inv = new Entities.tb_inventory_detail();
                    inv.inventory_detail_id = Guid.NewGuid().ToString();
                    inv.inventory_ref_id = stockAdjustment.stock_adjustment_id;
                    inv.inventory_item_id = item.product_id;
                    inv.inventory_warehouse_id = stockAdjustment.warehouse_id;
                    inv.quantity = item.out_quantity;
                    inv.previous_quantity = item.total_quantity;
                    inv.item_status = "pending";
                    db.tb_inventory_detail.Add(inv);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(string id)
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                Entities.tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.status = false;
                stockAdjustment.updated_by = User.Identity.Name;
                stockAdjustment.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ApproveFeedback(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            Models.StockAdjustmentViewModel model = new Models.StockAdjustmentViewModel();
            model = this.GetStockAdjustmentDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ApproveFeedback(string id,List<Models.InventoryViewModel> models)
        {
            try
            {
                int countItemApproved = 0;
                Entities.kim_mexEntities db = new Entities.kim_mexEntities();
                
                foreach(Models.InventoryViewModel item in models)
                {
                    string idd = item.inventory_id;
                    Entities.tb_inventory_detail invDetail = db.tb_inventory_detail.Find(idd);
                    invDetail.item_status = item.item_status;
                    invDetail.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    invDetail.remark = item.remark;
                    db.SaveChanges();
                    #region insert to inventory table if status is approved
                    if (string.Compare(invDetail.item_status, "approved") == 0)
                    {
                        countItemApproved++;

                        //Entities.tb_inventory inv = new Entities.tb_inventory();
                        //inv.inventory_id = Guid.NewGuid().ToString();
                        //inv.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inv.inventory_status_id = "1";
                        //inv.warehouse_id = invDetail.inventory_warehouse_id;
                        //inv.product_id = invDetail.inventory_item_id;
                        //inv.total_quantity = invDetail.quantity;
                        //inv.in_quantity = invDetail.quantity;
                        //inv.out_quantity = 0;
                        //inv.ref_id = id;
                        //db.tb_inventory.Add(inv);
                        //db.SaveChanges();
                    }
                    #endregion
                }
                Entities.tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.stock_adjustment_status =countItemApproved==models.Count()?Class.Status.Approved: Class.Status.PendingFeedback;
                stockAdjustment.approved_by = User.Identity.GetUserId();
                stockAdjustment.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
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
            Models.StockAdjustmentViewModel model = new Models.StockAdjustmentViewModel();
            model = this.GetStockAdjustmentDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult PrepareFeedback(string id,List<Models.InventoryViewModel> models)
        {
            try
            {
                Entities.kim_mexEntities db = new Entities.kim_mexEntities();
                var dupItems = models.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItems.Count() > 0)
                {
                    return Json(new { result = "error", message = "Duplication item to adjust." }, JsonRequestBehavior.AllowGet);
                }
                Entities.tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.stock_adjustment_status = Class.Status.Feedbacked;
                stockAdjustment.updated_by = User.Identity.GetUserId();
                stockAdjustment.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                this.RemoveItemInventoryDetail(id, "feedbacked");
                foreach (Models.InventoryViewModel item in models)
                {
                    Entities.tb_inventory_detail inv = new Entities.tb_inventory_detail();
                    inv.inventory_detail_id = Guid.NewGuid().ToString();
                    inv.inventory_ref_id = stockAdjustment.stock_adjustment_id;
                    inv.inventory_item_id = item.product_id;
                    inv.inventory_warehouse_id = stockAdjustment.warehouse_id;
                    inv.quantity = item.out_quantity;
                    inv.previous_quantity = item.total_quantity;
                    inv.item_status = "pending";
                    inv.remark = item.remark;
                    db.tb_inventory_detail.Add(inv);
                    db.SaveChanges();
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approval(string id, string status, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.stock_adjustment_status = string.Compare(status, Status.Approved) == 0 ? Status.Checked : Status.CheckRejected;
                stockAdjustment.checked_by = User.Identity.GetUserId();
                stockAdjustment.checked_date = DateTime.Now;
                stockAdjustment.checked_comment = comment;
                db.SaveChanges();

                if (string.Compare(status, Status.Rejected) == 0)
                {
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = stockAdjustment.stock_adjustment_id;
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
        public ActionResult RequestCancel(string id,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.stock_adjustment_status = Status.RequestCancelled;
                stockAdjustment.updated_by = User.Identity.GetUserId();
                stockAdjustment.updated_date = DateTime.Now;
                stockAdjustment.checked_comment = comment;
                db.SaveChanges();

                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = stockAdjustment.stock_adjustment_id;
                reject.ref_type = "Stock Adjustment";
                reject.comment = comment;
                reject.rejected_by = User.Identity.GetUserId();
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_reject.Add(reject);
                db.SaveChanges();

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Completed(string id, string status, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_stock_adjustment stockAdjustment = db.tb_stock_adjustment.Find(id);
                stockAdjustment.stock_adjustment_status = string.Compare(status, Status.Approved) == 0 ? Status.Completed : Status.Rejected;
                stockAdjustment.completed_by = User.Identity.GetUserId();
                stockAdjustment.completed_date = DateTime.Now;
                stockAdjustment.completed_comment = comment;
                db.SaveChanges();

                if (string.Compare(stockAdjustment.stock_adjustment_status, Status.Completed) == 0)
                {
                    //insert to inventory here

                    var inventories = db.tb_inventory_detail.Where(m => m.inventory_ref_id == id).ToList();
                    foreach (var invDetail in inventories)
                    {
                        //decimal totalQty = Convert.ToDecimal(db.tb_inventory.OrderByDescending(m => m.inventory_date).Where(m => m.product_id == inv.inventory_item_id && m.warehouse_id == inv.inventory_warehouse_id).Select(m => m.total_quantity).FirstOrDefault());
                        //tb_inventory inventory = new tb_inventory();
                        //inventory.inventory_id = Guid.NewGuid().ToString();
                        //inventory.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        //inventory.ref_id = id;
                        //inventory.inventory_status_id = "5";
                        //inventory.warehouse_id = inv.inventory_warehouse_id;
                        //inventory.product_id = inv.inventory_item_id;
                        //inventory.out_quantity = 0;
                        //inventory.in_quantity = CommonClass.ConvertMultipleUnit(inv.inventory_item_id, inv.unit, Convert.ToDecimal(inv.quantity));
                        //inventory.total_quantity = totalQty + inventory.in_quantity;
                        //db.tb_inventory.Add(inventory);
                        //db.SaveChanges();

                        Entities.tb_inventory inv = new Entities.tb_inventory();
                        inv.inventory_id = Guid.NewGuid().ToString();
                        inv.inventory_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        inv.inventory_status_id = "1";
                        inv.warehouse_id = invDetail.inventory_warehouse_id;
                        inv.product_id = invDetail.inventory_item_id;
                        inv.total_quantity = invDetail.quantity;
                        inv.in_quantity = invDetail.quantity;
                        inv.out_quantity = 0;
                        inv.ref_id = id;
                        db.tb_inventory.Add(inv);
                        db.SaveChanges();
                    }

                }
                else
                {
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = stockAdjustment.stock_adjustment_id;
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
        public ActionResult GetAllStockAdjustments(string status)
        {
            List<Models.StockAdjustmentViewModel> models = new List<Models.StockAdjustmentViewModel>();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                if (string.IsNullOrEmpty(status) || string.Compare(status, "All") == 0)
                {
                    models = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true).Select(x => new Models.StockAdjustmentViewModel()
                    {
                        stock_adjustment_id=x.stock_adjustment_id,
                        stock_adjuctment_code=x.stock_adjuctment_code,
                        warehouse_id=x.warehouse_id,
                        warehouse_name=db.tb_warehouse.Where(w=>string.Compare(w.warehouse_id,x.warehouse_id)==0).Select(w=>w.warehouse_name).FirstOrDefault().ToString(),
                        comment=x.comment,
                        stock_adjustment_status=x.stock_adjustment_status,
                        created_date=x.created_date
                    }).ToList();
                }
                else
                {
                    models = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true && string.Compare(x.stock_adjustment_status,status)==0).Select(x => new Models.StockAdjustmentViewModel()
                    {
                        stock_adjustment_id = x.stock_adjustment_id,
                        stock_adjuctment_code = x.stock_adjuctment_code,
                        warehouse_id = x.warehouse_id,
                        warehouse_name = db.tb_warehouse.Where(w => string.Compare(w.warehouse_id, x.warehouse_id) == 0).Select(w => w.warehouse_name).FirstOrDefault().ToString(),
                        comment = x.comment,
                        stock_adjustment_status = x.stock_adjustment_status,
                        created_date = x.created_date
                    }).ToList();
                }
            }
            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllItemsbyWarehouseJson(string id)
        {
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            models = this.GetAllItembyWarehouse(id);
            //return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { data = models }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public string GetStockAdjustmentNo()
        {
            string stockAdjustmentNo = string.Empty;
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_stock_adjustment orderby tbl.created_date descending select tbl.stock_adjuctment_code).FirstOrDefault();
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
                stockAdjustmentNo = "SA-" + yy + "-" + mm + "-" + last_no;
            }
            return stockAdjustmentNo;
        }
        public List<Models.InventoryViewModel> GetAllItembyWarehouse(string id)
        {
            List<Models.InventoryViewModel> models = new List<Models.InventoryViewModel>();
            List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
            using (Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                List<Models.InventoryViewModel> orginalItems = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
                List<Models.InventoryViewModel> items1 = new List<Models.InventoryViewModel>();
                items1 = db.tb_inventory.OrderBy(x => x.product_id).ThenByDescending(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id,id) == 0).Select(x => new Models.InventoryViewModel()
                {
                    inventory_date = x.inventory_date,
                    product_id = x.product_id,
                    warehouse_id = x.warehouse_id,
                    total_quantity = x.total_quantity,
                    in_quantity=x.in_quantity,
                    out_quantity=x.out_quantity,
                    inventory_status_id=x.inventory_status_id,
                }).ToList();

                #region new enhance speed
                var groupItems = items1.GroupBy(s => s.product_id).Select(s => new { key = s.Key, item = s.OrderByDescending(x => x.inventory_date).FirstOrDefault() }).ToList();
                foreach (var item in groupItems)
                {
                    var product = Class.CommonClass.GetProductDetail(item.item.product_id);
                    Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                    iitem.inventory_date = item.item.inventory_date;
                    iitem.product_id = item.item.product_id;
                    iitem.itemCode = product.product_code;
                    iitem.itemName = product.product_name;
                    iitem.itemUnit = product.product_unit;
                    iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                    iitem.warehouse_id = item.item.warehouse_id;
                    iitem.total_quantity = item.item.total_quantity;
                    if (iitem.total_quantity > 0)
                        models.Add(iitem);

                    items1.RemoveAll(s => string.Compare(s.product_id, item.item.product_id) == 0);
                }
                foreach (var item in items1)
                {
                    var product = Class.CommonClass.GetProductDetail(item.product_id);
                    Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                    iitem.inventory_date = item.inventory_date;
                    iitem.product_id = item.product_id;
                    iitem.itemCode = product.product_code;
                    iitem.itemName = product.product_name;
                    iitem.itemUnit = product.product_unit;
                    iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                    iitem.warehouse_id = item.warehouse_id;
                    iitem.total_quantity = item.total_quantity;
                    if (iitem.total_quantity > 0)
                        models.Add(iitem);
                }
                #endregion

                //var dup = items1.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                //if (dup.Any())
                //{
                //    foreach (var ditem in dup)
                //    {
                //        Models.InventoryViewModel item = new Models.InventoryViewModel();
                //        item = items1.Where(x => string.Compare(x.product_id, ditem) == 0).First();
                //        items.Add(item);
                //    }
                //    foreach (var item in items1)
                //    {
                //        var checkItem = dup.Where(x => string.Compare(x, item.product_id) == 0).ToList();
                //        if (checkItem.Any()) { }
                //        else
                //            items.Add(new Models.InventoryViewModel() {
                //                inventory_date = item.inventory_date,
                //                product_id = item.product_id,
                //                warehouse_id = item.warehouse_id,
                //                total_quantity = item.total_quantity,
                //                in_quantity =item.in_quantity,
                //                out_quantity =item.out_quantity,
                //                inventory_status_id=item.inventory_status_id,
                //            });
                //    }
                //}
                //else
                //    items = items1;

                //if (items.Any())
                //{
                //    foreach (var item in items)
                //    {
                //        if (item != null)
                //        {
                //            if (item.product_id != null)
                //            {
                //                Models.ProductViewModel product = Class.CommonClass.GetProductDetail(item.product_id);
                //                if (product != null)
                //                {
                //                    if (item.total_quantity>0)
                //                        orginalItems.Add(new Models.InventoryViewModel()
                //                        {
                //                            inventory_date = item.inventory_date,
                //                            product_id = item.product_id,
                //                            total_quantity = item.total_quantity,
                //                            warehouse_id = item.warehouse_id,
                //                            in_quantity=item.in_quantity,
                //                            out_quantity=item.out_quantity,
                //                            inventory_status_id=item.inventory_status_id,
                //                            itemCode = Class.CommonClass.GetProductDetail(item.product_id).product_code,
                //                            itemName = Class.CommonClass.GetProductDetail(item.product_id).product_name,
                //                            itemUnit = Class.CommonClass.GetProductDetail(item.product_id).product_unit,
                //                            itemUnitName= Class.CommonClass.GetProductDetail(item.product_id).unit_name,
                //                        });
                //                }
                //            }

                //        }

                //    }
                //}

                //var duplicationItems = orginalItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                //if (duplicationItems.Any())
                //{
                //    foreach (var dItem in duplicationItems)
                //    {
                //        decimal totalQuantity = 0;
                //        var itemss = orginalItems.Where(x => string.Compare(x.product_id, dItem) == 0).Select(x => x.total_quantity).ToList();
                //        foreach (var quantiy in itemss)
                //            totalQuantity = totalQuantity + Convert.ToDecimal(quantiy);
                //        foreach (var item in orginalItems)
                //        {
                //            Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                //            iitem.inventory_date = item.inventory_date;
                //            iitem.product_id = item.product_id;
                //            iitem.itemCode = Class.CommonClass.GetProductDetail(iitem.product_id).product_code;
                //            iitem.itemName= Class.CommonClass.GetProductDetail(iitem.product_id).product_name;
                //            iitem.itemUnit = Class.CommonClass.GetProductDetail(iitem.product_id).product_unit;
                //            iitem.itemUnitName = db.tb_unit.Find(iitem.itemUnit).Name;
                //            iitem.warehouse_id = item.warehouse_id;
                //            if (string.Compare(item.product_id, dItem) == 0)
                //            {
                //                iitem.total_quantity = totalQuantity;
                //            }
                //            else
                //            {
                //                iitem.total_quantity = item.total_quantity;
                //            }
                //            if(iitem.total_quantity>0)
                //                models.Add(iitem);
                //        }
                //    }
                //}
                //else
                //    models = orginalItems;

                inventories = models.OrderBy(x => x.itemCode).ToList().DistinctBy(s=>s.product_id).ToList();
            }
            return inventories;
        }
        public Models.StockAdjustmentViewModel GetStockAdjustmentDetail(string id)
        {
            Models.StockAdjustmentViewModel model = new Models.StockAdjustmentViewModel();
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                model = db.tb_stock_adjustment.Where(x => string.Compare(x.stock_adjustment_id, id) == 0).Select(x => new Models.StockAdjustmentViewModel()
                {
                    stock_adjustment_id=x.stock_adjustment_id,
                    stock_adjuctment_code=x.stock_adjuctment_code,
                    stock_adjustment_status=x.stock_adjustment_status,
                    warehouse_id=x.warehouse_id,
                    warehouse_name=db.tb_warehouse.Where(w=>string.Compare(w.warehouse_id,x.warehouse_id)==0).Select(w=>w.warehouse_name).FirstOrDefault().ToString(),
                    created_by=x.created_by,
                    created_date=x.created_date
                }).FirstOrDefault();
                List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
                inventories = (from x in db.tb_inventory_detail
                               join p in db.tb_product on x.inventory_item_id equals p.product_id
                               join u in db.tb_unit on p.product_unit equals u.Id
                               orderby p.product_code 
                               where string.Compare(x.inventory_ref_id, model.stock_adjustment_id) == 0
                               select new Models.InventoryViewModel()
                               {
                                   inventory_id=x.inventory_detail_id,
                                   product_id=x.inventory_item_id,
                                   itemCode=p.product_code,
                                   itemName=p.product_name,
                                   itemUnit=p.product_unit,
                                   unitName=u.Name,
                                   total_quantity=x.previous_quantity,
                                   out_quantity=x.quantity,
                                   item_status=x.item_status,
                                   approved_date=x.approved_date,
                                   remark=x.remark,
                                   reason=x.reason,
                               }).ToList();
                model.items = inventories;
            }
            return model;
        }
        public void RemoveItemInventoryDetail(string id,string status)
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                List<string> arrIds = new List<string>();
                var stockAdjustmentDetails = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0 && string.Compare(x.item_status, status) == 0).ToList();
                if (stockAdjustmentDetails.Any())
                {
                    foreach(var item in stockAdjustmentDetails)
                    {
                        string idd = item.inventory_detail_id;
                        Entities.tb_inventory_detail invDetail = db.tb_inventory_detail.Find(idd);
                        db.tb_inventory_detail.Remove(invDetail);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}