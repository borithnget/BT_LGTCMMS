using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;
using System.IO;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class WareHouseController : Controller
    {
        // GET: WareHouse
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            List<tb_site> sites = new List<tb_site>();
            sites = this.GetSiteDropdownList();
            ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
            ViewBag.QAQCOfficer = CommonFunctions.GetUserListItemsByRole(Role.QAQCOfficer);
            return View();
        }
        [HttpPost]
        public ActionResult Create(WareHouseViewModel model)
        {
            if (ModelState.IsValid)
            {
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_warehouse warehouse = new tb_warehouse();
                    warehouse.warehouse_id = Guid.NewGuid().ToString();
                    warehouse.warehouse_name = model.warehouse_name;
                    warehouse.warehouse_telephone = model.warehouse_telephone;
                    warehouse.warehouse_address = model.warehouse_address;
                    warehouse.warehouse_site_id = model.warehouse_site_id;
                    warehouse.warehouse_project_id = model.warehouse_project_id;
                    warehouse.warehouse_status = true;
                    warehouse.created_by = User.Identity.Name;
                    warehouse.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_warehouse.Add(warehouse);
                    db.SaveChanges();
                    //save_stock_keeper
                    foreach (var i in model.Stock_keeper_id) {
                        tb_stock_keeper_warehouse Stock_keeper = new tb_stock_keeper_warehouse();
                        Stock_keeper.stock_keeper_warehouse_id = Guid.NewGuid().ToString();
                        Stock_keeper.warehouse_id = warehouse.warehouse_id;
                        Stock_keeper.stock_keeper = i;
                        db.tb_stock_keeper_warehouse.Add(Stock_keeper);
                        db.SaveChanges();
                    }
                    //if (!string.IsNullOrEmpty(model.qaqc_id))
                    //{
                    //    tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                    //    qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                    //    qaqc.warehouse_id = warehouse.warehouse_id;
                    //    qaqc.qaqc_id = model.qaqc_id;
                    //    db.tb_warehouse_qaqc.Add(qaqc);
                    //    db.SaveChanges();
                    //}
                    foreach(var qaqcID in model.qaqcs)
                    {
                        if (!string.IsNullOrEmpty(qaqcID))
                        {
                            tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                            qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                            qaqc.warehouse_id = warehouse.warehouse_id;
                            qaqc.qaqc_id = qaqcID;
                            db.tb_warehouse_qaqc.Add(qaqc);
                            db.SaveChanges();
                        }
                    }
                           
                    return Redirect("Index");
                }
                  
               
            }
            List<tb_site> sites = new List<tb_site>();
            sites = this.GetSiteDropdownList();
            ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
            ViewBag.QAQCOfficer = CommonFunctions.GetUserListItemsByRole(Role.QAQCOfficer);
            return View(model);
        }
        //public ActionResult CreateJson()
        //{
        //    List<tb_site> sites = new List<tb_site>();
        //    sites = this.GetSiteDropdownList();
        //    ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
        //    ViewBag.QAQCOfficer = CommonFunctions.GetUserListItemsByRole(Role.QAQCOfficer);
        //    return View();
        //}
        [HttpPost]
        public ActionResult CreateJson(string warehouse_name,string warehouse_telephone,string warehouse_address,string qaqc_id,string warehouse_site_id,List<string> Stock_keeper_id,List<string> qaqcs)
        {
            try {
                kim_mexEntities db = new kim_mexEntities();
                tb_warehouse warehouse = new tb_warehouse();
                warehouse.warehouse_id = Guid.NewGuid().ToString();
                warehouse.warehouse_name = warehouse_name;
                warehouse.warehouse_telephone = warehouse_telephone;
                warehouse.warehouse_address = warehouse_address;
                warehouse.warehouse_site_id = warehouse_site_id;
                //warehouse.warehouse_project_id = model.warehouse_project_id;
                warehouse.warehouse_status = true;
                warehouse.created_by = User.Identity.Name;
                warehouse.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_warehouse.Add(warehouse);
                db.SaveChanges();
                //save_stock_keeper
                foreach (var i in Stock_keeper_id)
                {
                    tb_stock_keeper_warehouse Stock_keeper = new tb_stock_keeper_warehouse();
                    Stock_keeper.stock_keeper_warehouse_id = Guid.NewGuid().ToString();
                    Stock_keeper.warehouse_id = warehouse.warehouse_id;
                    Stock_keeper.stock_keeper = i;
                    db.tb_stock_keeper_warehouse.Add(Stock_keeper);
                    db.SaveChanges();
                }
                //if (!string.IsNullOrEmpty(qaqc_id))
                //{
                //    tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                //    qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                //    qaqc.warehouse_id = warehouse.warehouse_id;
                //    qaqc.qaqc_id = qaqc_id;
                //    db.tb_warehouse_qaqc.Add(qaqc);
                //    db.SaveChanges();
                //}

                foreach (var qaqcID in qaqcs)
                {
                    if (!string.IsNullOrEmpty(qaqcID))
                    {
                        tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                        qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                        qaqc.warehouse_id = warehouse.warehouse_id;
                        qaqc.qaqc_id = qaqcID;
                        db.tb_warehouse_qaqc.Add(qaqc);
                        db.SaveChanges();
                    }
                }

                //return Redirect("Index");
                return Json(new { result = "success", warehouse_id = warehouse.warehouse_id }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                List<tb_site> sites = new List<tb_site>();
                sites = this.GetSiteDropdownList();
                ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
                return Json(new { result = "fail",mesage=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Detail(string id)
        {
            WareHouseViewModel model = new WareHouseViewModel();
            model = this.GetWarehouseDetail(id);
            return View(model);
        }
        public ActionResult Edit(string id)
        {
            WareHouseViewModel model = new WareHouseViewModel();
            model = this.GetWarehouseDetail(id);
            List<tb_site> sites = new List<tb_site>();
            sites = this.GetSiteDropdownList();
            ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(WareHouseViewModel model,string id)
        {
            if (ModelState.IsValid)
            {
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_warehouse warehouse = db.tb_warehouse.Find(id);
                    warehouse.warehouse_name = model.warehouse_name;
                    warehouse.warehouse_telephone = model.warehouse_telephone;
                    warehouse.warehouse_address = model.warehouse_address;
                    warehouse.warehouse_site_id = model.warehouse_site_id;
                    warehouse.warehouse_status = true;
                    warehouse.updated_by = User.Identity.Name;
                    warehouse.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    var getwarehouse_id = db.tb_stock_keeper_warehouse.Where(m => m.warehouse_id == warehouse.warehouse_id).ToList();
                    if (getwarehouse_id!=null) { 
                    foreach (var i in getwarehouse_id)
                    {
                            var stk = db.tb_stock_keeper_warehouse.Find(i.stock_keeper_warehouse_id);
                            db.tb_stock_keeper_warehouse.Remove(stk);
                            db.SaveChanges();  
                    }

                    }
                    foreach(var x in model.Stock_keeper_id)
                    {
                        tb_stock_keeper_warehouse Stock_Keeper = new tb_stock_keeper_warehouse();
                        Stock_Keeper.stock_keeper_warehouse_id = Guid.NewGuid().ToString();
                        Stock_Keeper.warehouse_id = warehouse.warehouse_id;
                        Stock_Keeper.stock_keeper = x;
                        db.tb_stock_keeper_warehouse.Add(Stock_Keeper);
                        db.SaveChanges();

                    }

                    //if (!string.IsNullOrEmpty(model.qaqc_id))
                    //{
                    //    tb_warehouse_qaqc qaqcc = db.tb_warehouse_qaqc.Where(s => string.Compare(s.warehouse_id, warehouse.warehouse_id) == 0).FirstOrDefault();
                    //    if (qaqcc == null)
                    //    {
                    //        tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                    //        qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                    //        qaqc.warehouse_id = warehouse.warehouse_id;
                    //        qaqc.qaqc_id = model.qaqc_id;
                    //        db.tb_warehouse_qaqc.Add(qaqc);
                    //        db.SaveChanges();
                    //    }
                    //    else
                    //    {
                    //        qaqcc.qaqc_id = model.qaqc_id;
                    //        db.SaveChanges();
                    //    }
                    //}

                    var oldQAQCs = db.tb_warehouse_qaqc.Where(s => string.Compare(s.warehouse_id, warehouse.warehouse_id) == 0).ToList();
                    foreach(var oqaqc in oldQAQCs)
                    {
                        tb_warehouse_qaqc obj = db.tb_warehouse_qaqc.Find(oqaqc.warehouse_qaqc_id);
                        db.tb_warehouse_qaqc.Remove(obj);
                        db.SaveChanges();
                    }

                    foreach (var qaqcId in model.qaqcs)
                    {
                        tb_warehouse_qaqc qaqc = new tb_warehouse_qaqc();
                        qaqc.warehouse_qaqc_id = Guid.NewGuid().ToString();
                        qaqc.warehouse_id = warehouse.warehouse_id;
                        qaqc.qaqc_id = qaqcId;
                        db.tb_warehouse_qaqc.Add(qaqc);
                        db.SaveChanges();

                    }
                    

                    return RedirectToAction("Index", "WareHouse");
                }
            }
            List<tb_site> sites = new List<tb_site>();
            sites = this.GetSiteDropdownList();
            ViewBag.SiteID = new SelectList(sites, "site_id", "site_name");
            //kim_mexEntities db1 = new kim_mexEntities();
            //model.list_stock_keeper = db1.tb_stock_keeper_warehouse.Where(m => m.warehouse_id == model.warehouse_id).Select(m => m.stock_keeper);
            return View(model);
        }
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_warehouse warehouse = db.tb_warehouse.Find(id);
                warehouse.warehouse_status = false;
                warehouse.updated_by = User.Identity.Name;
                warehouse.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<tb_site> GetSiteDropdownList()
        {
            List<tb_site> sites = new List<tb_site>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                sites = db.tb_site.OrderBy(m=>m.site_name).Where(m => m.status == true).ToList();
            }
            return sites;
        }
        public ActionResult GetWarehouseDataTable()
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                warehouses = (from wh in db.tb_warehouse
                              join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                              join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id into proj
                              from pro in proj.DefaultIfEmpty()
                              orderby wh.warehouse_name
                              where wh.warehouse_status == true
                              select new WareHouseViewModel
                              {
                                  warehouse_id=wh.warehouse_id,
                                  warehouse_name=wh.warehouse_name,
                                  warehouse_telephone=wh.warehouse_telephone,
                                  warehouse_address=wh.warehouse_address,
                                  warehouse_site_id=wh.warehouse_site_id,
                                  //created_date= Convert.ToDateTime(wh.created_date).ToString("dd/MM/yyyy"),
                                  site_name=site.site_name,
                                  warehouse_project_id=wh.warehouse_project_id,
                                  warehouse_project_name=pro.project_full_name,
                                  warehouse_project_status=pro.p_status,
                              }).ToList();
                return Json(new { data = warehouses }, JsonRequestBehavior.AllowGet);
            }
        }
        public WareHouseViewModel GetWarehouseDetail(string id)
        {
            WareHouseViewModel warehouse = new WareHouseViewModel();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                warehouse = (from wh in db.tb_warehouse
                             join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                             join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id into proj from pro in proj.DefaultIfEmpty()
                             where wh.warehouse_status == true && wh.warehouse_id == id
                             select new WareHouseViewModel
                             {
                                 warehouse_id = wh.warehouse_id,
                                 warehouse_name = wh.warehouse_name,
                                 warehouse_telephone = wh.warehouse_telephone,
                                 warehouse_address = wh.warehouse_address,
                                 warehouse_site_id = wh.warehouse_site_id,
                                 created_date = wh.created_date,
                                 site_name = site.site_name,
                                 warehouse_project_id=wh.warehouse_project_id,
                                 warehouse_project_name=pro.project_full_name,
                                list_stock_keeper = db.tb_stock_keeper_warehouse.Where(m => m.warehouse_id == wh.warehouse_id).Select(m => m.stock_keeper).ToList(),
                                
                             }).FirstOrDefault();
                warehouse.Stock_keeper_id = db.tb_stock_keeper_warehouse.Where(m => m.warehouse_id == warehouse.warehouse_id).Select(m => m.stock_keeper).ToArray();
                warehouse.date = Convert.ToDateTime(warehouse.created_date).ToString("dd/MM/yyyy");
                warehouse.warehouseQAQC = db.tb_warehouse_qaqc.Where(s => string.Compare(s.warehouse_id, warehouse.warehouse_id) == 0).Select(s => new WarehouseQAQCViewModel() { warehouse_qaqc_id = s.warehouse_qaqc_id, warehouse_id = s.warehouse_id, qaqc_id = s.qaqc_id }).FirstOrDefault();
                if (warehouse.warehouseQAQC != null)
                    warehouse.warehouseQAQC.qaqc_name = CommonFunctions.GetUserFullnamebyUserId(warehouse.warehouseQAQC.qaqc_id);
                warehouse.siteProjects = db.tb_project.Where(s => s.project_status == true && string.Compare(s.site_id, warehouse.warehouse_site_id) == 0 && string.Compare(s.p_status, ProjectStatus.Active) == 0).Select(s => new ProjectViewModel()
                {
                    project_id = s.project_id,
                    project_full_name = s.project_full_name
                }).ToList();
               
                warehouse.warehouseQAQCs = (from qaqc in db.tb_warehouse_qaqc
                                            join user in db.tb_user_detail on qaqc.qaqc_id equals user.user_id into puser
                                            from user in puser.DefaultIfEmpty()
                                            where string.Compare(qaqc.warehouse_id, warehouse.warehouse_id) == 0
                                            select new WarehouseQAQCViewModel()
                                            {
                                                warehouse_qaqc_id = qaqc.warehouse_qaqc_id,
                                                warehouse_id = qaqc.warehouse_id,
                                                qaqc_id = qaqc.qaqc_id,
                                                qaqc_name=user.user_first_name+" "+user.user_last_name
                                            }).ToList();
            }
            return warehouse;
        }
        public List<tb_warehouse> GetWarehouseList(int iid = 0 )
        {
            string id = User.Identity.GetUserId();

            List<tb_warehouse> warehouses = new List<tb_warehouse>();
          
            using (kim_mexEntities db =new kim_mexEntities())
            {
                if (iid == 0)
                {
                    if (User.IsInRole(Role.SiteStockKeeper))
                    {
                        warehouses = (from wh in db.tb_warehouse
                                      join stock_keeper in db.tb_stock_keeper_warehouse on wh.warehouse_id equals stock_keeper.warehouse_id
                                      where wh.warehouse_status == true && stock_keeper.stock_keeper == id
                                      select wh).ToList();
                        return warehouses;
                    }
                    else if (User.IsInRole(Role.SiteSupervisor))
                    {
                        warehouses = (from wh in db.tb_warehouse
                                     join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                                     join sitesupt in db.tbSiteSiteSupervisors on site.site_id equals sitesupt.site_id
                                     where wh.warehouse_status==true && sitesupt.site_supervisor_id==id
                                     select wh).ToList();
                        return warehouses;
                    }
                    warehouses = db.tb_warehouse.OrderBy(m => m.warehouse_name).Where(m => m.warehouse_status == true).ToList();
                }
                else
                    warehouses = db.tb_warehouse.OrderBy(m => m.warehouse_name).Where(m => m.warehouse_status == true).ToList();
            }
            return warehouses;
        }
        public ActionResult GetWarehouseDropdownList(int iid=0)
        {
                List<tb_warehouse> warehouses = new List<tb_warehouse>();
                warehouses = this.GetWarehouseList(iid);

                return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllWarehouseDropdown()
        {
            List<WareHouseViewModel> warehouses = new List<WareHouseViewModel>();
            warehouses = this.GetAllWarehouse();
            return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StockBalance(string warehouse_id)
        {
            List<tb_warehouse> warehouses = new List<tb_warehouse>();
            warehouses = this.GetWarehouseList();
            ViewBag.Warehouse = new SelectList(warehouses, "warehouse_id", "warehouse_name");
            List<InventoryDetailViewModel> inventories = new List<InventoryDetailViewModel>();
            //inventories = Inventory.GetStockBalance("All");
            //return View(inventories);
            return View();
        }
        public ActionResult StockBalananceDataTable(string warehouse_id)
        {
            List<InventoryDetailViewModel> inventories = new List<InventoryDetailViewModel>();
            inventories = Inventory.GetStockBalance(warehouse_id);
            return Json(new { data = inventories }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles= "Admin")]
        public ActionResult ImportStockBalance()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportStockBalance(string filename)
        {
            ImportStockBalance obj = new Class.ImportStockBalance();
            StockBalancedImportReturn ImportReturn = new StockBalancedImportReturn();
            if (Request.Files.Count > 0)
            {
                var file0 = Request.Files[0];
                if(file0 !=null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/Stock Balance/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    ImportReturn = obj.GetDataFromExcelContent(path, true);

                    return View(ImportReturn);
                }
            }
            return RedirectToAction("ImportStockBalance");
        }
        public List<Models.WareHouseViewModel> GetAllWarehouse()
        {
            string id = User.Identity.GetUserId();
            List<Models.WareHouseViewModel> models = new List<WareHouseViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if(User.IsInRole("Stock Keeper"))
                {
                    models = (from wh in db.tb_warehouse
                                  join stock_keeper in db.tb_stock_keeper_warehouse on wh.warehouse_id equals stock_keeper.warehouse_id
                                  where wh.warehouse_status == true && stock_keeper.stock_keeper == id
                                  select new WareHouseViewModel
                                  {
                                      warehouse_id = wh.warehouse_id,
                                      warehouse_name = wh.warehouse_name,
                                      warehouse_site_id = wh.warehouse_site_id
                                  }
                                  ).ToList();

                    return models;
                }
                models = db.tb_warehouse.OrderBy(m => m.warehouse_name).Where(m => m.warehouse_status == true).Select(m => new WareHouseViewModel() { warehouse_id=m.warehouse_id,warehouse_name=m.warehouse_name,warehouse_site_id=m.warehouse_site_id}).ToList();
            }
            return models;
        }
        public ActionResult StockKeeperDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users                     
                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,                          
                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where role.Name=="Site Stock Keeper"
                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,
                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p => p.Role == "Site Stock Keeper").OrderBy(p => p.Username);
                return Json(new { data = userRoles }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                return null;
            }
             
        }
        public ActionResult GetWarehouseBySiteJSON(string id,bool isCreate=false)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<WareHouseViewModel> models = new List<WareHouseViewModel>();
                if(isCreate)
                    models = db.tb_warehouse.Where(s => s.warehouse_status == true && string.Compare(s.warehouse_site_id, id) == 0 && string.IsNullOrEmpty(s.warehouse_project_id)).Select(s => new WareHouseViewModel() { warehouse_id = s.warehouse_id, warehouse_name = s.warehouse_name }).ToList();
                else
                    models = db.tb_warehouse.Where(s => s.warehouse_status == true && string.Compare(s.warehouse_site_id, id) == 0).Select(s => new WareHouseViewModel() { warehouse_id = s.warehouse_id, warehouse_name = s.warehouse_name }).ToList();
                return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            }
        }
        public void saveWarehouseJSON()
        {

        }

    }
}