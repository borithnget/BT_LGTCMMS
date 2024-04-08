using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
//using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using BT_KimMex.Models;
using System;
using BT_KimMex.Class;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProjectList()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ProjectList(string status)
        {
            return RedirectToAction("GenerateProjectList", new { status = status });
        }
        public ActionResult GenerateProjectList()
        {
            return View();
        }
        public ActionResult BOQbyProject()
        {
            List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
            boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
            ViewBag.Projects = new SelectList(boqs, "boq_id", "project_full_name");
            return View();
        }
        [HttpPost]
        public ActionResult BOQbyProject(string boq_id)
        {
            if (!string.IsNullOrEmpty(boq_id))
            {
                return RedirectToAction("GenerateReport",new { reportType = "BOQByProject", boqId = boq_id });
            }
            List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
            boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
            ViewBag.Projects = new SelectList(boqs, "boq_id", "project_full_name");
            return View();
        }
        public ActionResult ItemRequestbyProject()
        {
            List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
            boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
            ViewBag.Projects = new SelectList(boqs, "project_id", "project_full_name");
            return View();
        }
        [HttpPost]
        public ActionResult ItemRequestbyProject(string project_id)
        {
            if (!string.IsNullOrEmpty(project_id))
                return RedirectToAction("GenerateReport", new { reportType = "ItemRequestbyProject", projectId = project_id });
            List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
            boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
            ViewBag.Projects = new SelectList(boqs, "project_id", "project_full_name");
            return View();
        }
        public ActionResult PurchaseOrder(int id)
        {
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id=x.supplier_id,supplier_name=x.supplier_name}).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            ViewBag.Id=id;
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseOrder(Models.PurchaseOrderReport model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("PurchaseOrderResponse", new { fromDate = model.dateFrom, toDate = model.dateTo, status = model.poStatus, supplier = model.poSuppplier, project = model.project_id, isFinance=model.isFinance });
                //return RedirectToAction("GenerateReport", new { reportType = "PurchaseOrder", fromDate = model.dateFrom, toDate = model.dateTo, status = model.poStatus, supplier = model.poSuppplier, project = model.project_id });
            }
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            ViewBag.IsFinance = model.isFinance;
            return View();
        }
        
        public ActionResult PurchaseOrderResponse(DateTime fromDate,DateTime toDate,string status,string supplier,string project,int isFinance)
        {
            Models.PurchaseOrderReport model = new PurchaseOrderReport();
            model.dateFrom= fromDate;
            model.dateTo= toDate;
            model.poStatus= status;
            model.supplierId= supplier;
            model.project_id= project;
            model.isFinance= isFinance;
            ViewBag.isFinance = isFinance;
            return View(PurchaseOrderResponseReport.GeneratePurchaseOrderReportList(model));
        }

        public ActionResult PurchaseOrderVsItemReceived()
        {
            List<Models.PurchaseOrderVsItemReceivedViewModel> poVsIR = new List<Models.PurchaseOrderVsItemReceivedViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseOrderVsItemReceived(Models.PurchaseOrderVsItemReceivedReport model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("PurchaseOrderVSGRNResponse", new { dateFrom = model.dateFrom, dateTo = model.dateTo });
                //return RedirectToAction("GenerateReport", new { reportType = "PurchaseOrderVsItemReceived", dateFrom = model.dateFrom, dateTo = model.dateTo });
            }
                
            List<Models.PurchaseOrderVsItemReceivedViewModel> poVsIR = new List<Models.PurchaseOrderVsItemReceivedViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }

        public ActionResult PurchaseOrderVSGRNResponse(DateTime? dateFrom,DateTime? dateTo)
        {
            PurchaseOrderVSGRNReportResponseModel model = new PurchaseOrderVSGRNReportResponseModel();
            model.poGRNs = PurchaseOrderReportViewModel.GetPORequestbyDateRange(dateFrom, dateTo);
            model.poDetails = DetailPOReceivingReportResponseModel.GetDetailPOReceivingReportsList(dateFrom, dateTo);
            return View(model);
        }

        //add new 10/5/2018
        public ActionResult PurchaseOrderNotVAT()
        {
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseOrderNotVAT(Models.PurchaseOrderReport model)
        {
            if (ModelState.IsValid)
                return RedirectToAction("GenerateReport", new { reportType = "PurchaseOrderNotVAT", fromDate = model.dateFrom, toDate = model.dateTo, status = model.poStatus, supplier = model.poSuppplier });
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            return View();
        }
        //end 10/5/2018

        public ActionResult PurchaseOrderBySupplier()
        {
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel()
            { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseOrderBySupplier(Models.PurchaseOrderSupplierReport model)
        {
            if (ModelState.IsValid)
                return RedirectToAction("GenerateReport", new { reportType = "PurchaseOrderBySupplier", supplier = model.supplier, dateFrom = model.dateFrom, dateTo = model.dateTo,project=model.project_id });
            List<Models.SupplierViewModel> suppliers = new List<Models.SupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new Models.SupplierViewModel() { supplier_id = x.supplier_id, supplier_name = x.supplier_name }).ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "supplier_id", "supplier_name");
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            return View();
        }
        public ActionResult StockUsageBySiteWithRemainBalance()
        {
            List<Models.StockUsageBySiteWithRemainBalanceViewModel> site = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            site = db.tb_site.OrderBy(x => x.site_name).Where(x => x.status == true).Select(x => new Models.StockUsageBySiteWithRemainBalanceViewModel()

            {
                site_id=x.site_id,
                Site_name=x.site_name,
            }).ToList();
            ViewBag.Site = new SelectList(site, "site_id", "site_name");
            return View();
        }
        [HttpPost]
        public ActionResult StockUsageBySiteWithRemainBalance(Models.StockUsageBySiteWithRemainBalanceReport model)
        {
            if (ModelState.IsValid)
                return RedirectToAction("GenerateReport", new { reportType = "StockUsageBySiteWithRemainBalance",site=model.Site_name, dateFrom = model.dateFrom, dateTo = model.dateTo });
            List<Models.StockUsageBySiteWithRemainBalanceViewModel> site = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            site = db.tb_site.OrderBy(x => x.site_name).Where(x => x.status == true).Select(x => new Models.StockUsageBySiteWithRemainBalanceViewModel()

            {
                site_id=x.site_id,
                Site_name=x.site_name,
            }).ToList();
            ViewBag.Site = new SelectList(site, "site_id", "site_name");
            return View();
        }
        public ActionResult StockMovement()
        {
            List<Models.StockMovementViewModel> stockM = new List<Models.StockMovementViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }
        [HttpPost]
        public ActionResult StockMovement(Models.StockMovementReport model)
        {
            if(ModelState.IsValid)
                return RedirectToAction("GenerateReport", new { reportType = "StockMovement",  dateFrom = model.dateFrom, dateTo = model.dateTo });
            //List<Models.StockMovementViewModel> stockM = new List<Models.StockMovementViewModel>();
            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }


        //public ActionResult ItemRequestbyProject()
        //{
        //    List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
        //    boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
        //    ViewBag.Projects = new SelectList(boqs, "project_id", "project_full_name");
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ItemRequestbyProject(string project_id)
        //{
        //    if (!string.IsNullOrEmpty(project_id))
        //        return RedirectToAction("GenerateReport", new { reportType = "ItemRequestbyProject", projectId = project_id });
        //    List<BT_KimMex.Models.BOQViewModel> boqs = new List<BT_KimMex.Models.BOQViewModel>();
        //    boqs = BT_KimMex.Class.CommonClass.GetProjectsDropdownList();
        //    ViewBag.Projects = new SelectList(boqs, "project_id", "project_full_name");
        //    return View();
        //}
        public ActionResult StockBalanceBywarehouse()
        {
            //List<Models.StockBalanceBywarehouseViewModel> stockW = new List<Models.StockBalanceBywarehouseViewModel>();
            List<BT_KimMex.Models.StockBalanceBywarehouseViewModel> stockW = new List<Models.StockBalanceBywarehouseViewModel>();
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByWarehouse();
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");
            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }
        [HttpPost]
        public ActionResult StockBalanceBywarehouse(string Warehouse_id)
        {
            return RedirectToAction("GenerateReport", new { reportType = "StockBalanceBywarehouse", WarehouseId = Warehouse_id });
            /*
            if (!string.IsNullOrEmpty(Warehouse_id))
                return RedirectToAction("GenerateReport",new { reportType= "StockBalanceBywarehouse" ,WarehouseId = Warehouse_id});
            List<BT_KimMex.Models.StockBalanceBywarehouseViewModel> stockW = new List<BT_KimMex.Models.StockBalanceBywarehouseViewModel>();
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByWarehouse();
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");
            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
            */
        }


        //public ActionResult StockMovement()
        //{
        //    List<Models.StockMovementViewModel> stockM = new List<Models.StockMovementViewModel>();
        //    Entities.kim_mexEntities db = new Entities.kim_mexEntities();

        //    return View();

        //}
        //[HttpPost]
        //public ActionResult StockMovement(Models.StockMovementReport model)
        //{
        //    if (ModelState.IsValid)
        //        return RedirectToAction("GenerateReport", new { reportType = "StockMovement", dateFrom = model.dateFrom, dateTo = model.dateTo });
        //    List<Models.StockMovementViewModel> stockM = new List<Models.StockMovementViewModel>();
        //    Entities.kim_mexEntities db = new Entities.kim_mexEntities();

        //    return View();
        //}
        public ActionResult ReturnItemtoSupplier()
        {
            List<Models.ReturnItemtoSupplierViewModel> ReItoSub = new List<Models.ReturnItemtoSupplierViewModel>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            return View();
        }
        [HttpPost]
        public ActionResult ReturnItemtoSupplier(Models.ReturnItemtoSupplierReport model)
        {
            if (ModelState.IsValid)
                return RedirectToAction("GenerateReport",new { reportType = "ReturnItemtoSupplier", dateFrom=model.dateFrom , dateTo=model.dateTo });
            List<Models.ReturnItemtoSupplierViewModel> ReItoSub = new List<Models.ReturnItemtoSupplierViewModel>();
            Entities.kim_mexEntities db=new Entities.kim_mexEntities();
            return View();
        }
        

        


        //public ActionResult StockUsageBySiteWithRemainBalance()
        //{
        //    List<Models.StockUsageBySiteWithRemainBalanceViewModel> stockUSRB = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
        //    Entities.kim_mexEntities db = new Entities.kim_mexEntities();
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult StockUsageBySiteWithRemainBalance(Models.StockUsageBySiteWithRemainBalanceReport model)

        //{
        //    if (ModelState.IsValid)
        //        return RedirectToAction("GenerateReport", new { reportType = "StockUsageBySiteWithRemainBalance", dateFrom = model.dateFrom, dateTo = model.dateTo });
        //    List<Models.StockUsageBySiteWithRemainBalanceViewModel> stockUSRB = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
        //    Entities.kim_mexEntities db = new Entities.kim_mexEntities();
        //    return View();
        //}
        public ActionResult GenerateReport()
        {
            return View();
        }

        public ActionResult ItemReport()
        {
            return View();
        }
        public ActionResult StockBalanceByDateandWarehouse(int id=0)
        {
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockW = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockWP = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouse();
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");

            //stockWP = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouseProject();
            //ViewBag.Projects = new SelectList(stockWP, "project_id", "projectName");

            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            //var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            //ViewBag.Projects = new SelectList(projects, "project_id", "project_name");

            ViewBag.IsStockContoller = id;
            return View();
        }
        //bora 01.07.21
        //public ActionResult MRreport()
        //{
        //    List<BT_KimMex.Models.ItemRequestViewModel> MR = new List<Models.ItemRequestViewModel>();
        //    List<BT_KimMex.Models.ItemRequestViewModel> MRe = new List<Models.ItemRequestViewModel>();
        //    MR = BT_KimMex.Class.CommonClass.GetItemRequestItems();
        //    ViewBag.itemrequest = new SelectList(, "ir_id", "warehouseName");

        //    //stockWP = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouseProject();
        //    //ViewBag.Projects = new SelectList(stockWP, "project_id", "projectName");

        //    Entities.kim_mexEntities db = new Entities.kim_mexEntities();
        //    var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
        //    ViewBag.Projects = new SelectList(projects, "project_id", "project_name");

        //    return View();
        //}
        [HttpPost]
        public ActionResult StockBalanceByDateandWarehouse(Models.StockBalanceBydateAndwarehouseReport model , string Warehouse_id,int IsStockController)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("GenerateReport", new { reportType = "StockBalanceByDateandWarehouse", date = model.date, WarehouseId = Warehouse_id, project = model.project_id, report = IsStockController });
            }
                
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockW = new List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel>();
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockWP = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouse();
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");

            //stockWP = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouseProject();
            //ViewBag.Projects = new SelectList(stockWP, "project_id", "projectName");
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            return View();
        }
        public ActionResult StockBalanceByDateandWarehouseStockKeeper()
        {
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockW = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockWP = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            string userid = string.Empty;
            if (User.IsInRole(Class.Role.SiteStockKeeper))
            {
                userid = User.Identity.GetUserId().ToString();
            }
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouse(userid);
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");

            #region added by TTerd Sep 01 2020
            stockWP = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouseProject(userid);
            ViewBag.Projects = new SelectList(stockWP, "project_id", "projectName");
            #endregion

            #region blocked by TTerd Sep 01 2020
            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            //var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            //ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            #endregion
            return View();
        }
        [HttpPost]
        public ActionResult StockBalanceByDateandWarehouseStockKeeper(Models.StockBalanceBydateAndwarehouseReport model, string Warehouse_id)
        {
            if (ModelState.IsValid)
                return RedirectToAction("GenerateReport", new { reportType = "StockBalanceByDateandWarehouseStockKeeper", date = model.date, WarehouseId = Warehouse_id, project = model.project_id });
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockW = new List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel>();
            List<BT_KimMex.Models.StockBalanceByDateandWarehouseViewModel> stockWP = new List<Models.StockBalanceByDateandWarehouseViewModel>();
            string userid = string.Empty;
            if (User.IsInRole(Class.Role.SiteStockKeeper))
            {
                userid = User.Identity.GetUserId().ToString();
            }
            stockW = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouse(userid);
            ViewBag.StockBalance = new SelectList(stockW, "warehouse_id", "warehouseName");

            #region added by TTerd Sep 01 2020
            stockWP = BT_KimMex.Class.CommonClass.GetStockBalanceByDateandWarehouseProject(userid);
            ViewBag.Projects = new SelectList(stockWP, "project_id", "projectName");
            #endregion

            #region blocked by TTerd Sep 01 2020
            //Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            //var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            //ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            #endregion
            return View();
        }
        public JsonResult getprojectbywarehouse(string UnId)
        {
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            if(UnId == "")
            {
                var pro = (from p in db.tb_project
                           where p.project_status == true && p.p_status == "Active"
                           select new
                           {
                               id = p.project_id,
                               name = p.project_full_name,
                           }
                       ).ToList();

                return Json(pro, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var pro = (from p in db.tb_project
                          // join w in db.tb_warehouse on p.project_id equals w.warehouse_project_id
                           join sw in db.tb_site on p.site_id equals sw.site_id
                           where sw.site_id == UnId && p.site_id == sw.site_id  && p.project_status == true && p.p_status == "Active"
                           select new
                           {
                               id = p.project_id,
                               name = p.project_full_name,
                           }
                       ).ToList();

                return Json(pro, JsonRequestBehavior.AllowGet);

            }
        }



        public ActionResult WorkOrderIsuseVSWorkOrderReturn()
        {
            //List<Models.WorkOrderIsuseVSWorkOrderReturnReport> woiVSwor = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            if(User.IsInRole(Role.SystemAdmin))
            {
                projects = ProjectViewModel.GetProjectListItemsBySiteSupervisor(User.IsInRole(Role.SystemAdmin));
            }
            
            else if (User.IsInRole(Role.ProjectManager))
            {
                projects = ProjectViewModel.GetProjectListItemByProjectManager(User.Identity.GetUserId());
            }else if (User.IsInRole(Role.SiteSupervisor))
            {
                projects = ProjectViewModel.GetProjectListItemsBySiteSupervisor(User.IsInRole(Role.SystemAdmin), User.Identity.GetUserId());

            }else if (User.IsInRole(Role.SiteManager))
            {
                projects=ProjectViewModel.GetProjectListItemBySiteManager(User.Identity.GetUserId());
            }
            ViewBag.Projects = new SelectList(projects, "project_id", "project_full_name");
            return View();
        }
        [HttpPost]
        public ActionResult WorkOrderIsuseVSWorkOrderReturn(Models.WorkOrderIsuseVSWorkOrderReturnReport model)
        {
            if (ModelState.IsValid)
                //return RedirectToAction("GenerateReport", new { reportType = "WorkOrderIsuseVSWorkOrderReturn", dateFrom = model.dateFrom, dateTo = model.dateTo , project = model.project_id });
                return RedirectToAction("WorkOrderIsuseVSWorkOrderReturnReport", new { dateFrom = model.dateFrom, dateTo = model.dateTo, project = model.project_id });
            //List<Models.WorkOrderIsuseVSWorkOrderReturnReport> woiVSwor = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();
            Entities.kim_mexEntities db = new Entities.kim_mexEntities();
            var projects = db.tb_project.OrderBy(x => x.project_full_name).Where(x => x.project_status == true && string.Compare(x.p_status, "Active") == 0).Select(x => new { project_id = x.project_id, project_name = x.project_full_name }).ToList();
            ViewBag.Projects = new SelectList(projects, "project_id", "project_name");
            return View();
        }
        public ActionResult WorkOrderIsuseVSWorkOrderReturnReport(DateTime dateFrom,DateTime dateTo,string project)
        {
            return View(WorkOrderIssueVSWorkOrderRequestReportResult.GenerateWorkOrderIssueVSWorkOrderReturnReport(dateFrom,dateTo,project));
        }

        public ActionResult ReportTest()
        {
            //ReportViewer viewer = new ReportViewer();
            //viewer.ProcessingMode = ProcessingMode.Remote;
            //viewer.SizeToReportContent = true;
            //viewer.AsyncRendering = true;
            //viewer.Width = Unit.Percentage(100);
            //viewer.Height = Unit.Percentage(100);

            ////viewer.ServerReport.ReportServerUrl =;
            //viewer.ServerReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Quote.rdlc";
            ////viewer.LocalReport.DataSources.Clear();
            ////ReportDataSource rds = new ReportDataSource("DSItemsInStock", dt);
            ////rv.LocalReport.DataSources.Add(rds);


            //ViewBag.ReportViewer = viewer;
            return View();
        }
        public ActionResult DetailPurchaseOrderReceiving()
        {
            return View();
        }
        [HttpPost] 
        public ActionResult DetailPurchaseOrderReceiving(DateTime? dateFrom,DateTime? dateTo)
        {
            return RedirectToAction("DetailPurchaseOrderReceivingResponse", new { dateFrom = dateFrom, dateTo = dateTo });
        }
        public ActionResult DetailPurchaseOrderReceivingResponse(DateTime? dateFrom,DateTime? dateTo)
        {
            return View(DetailPOReceivingReportResponseModel.GetDetailPOReceivingReportsList(dateFrom,dateTo));
        }

    }
}