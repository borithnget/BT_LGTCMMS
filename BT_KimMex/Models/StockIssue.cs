using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using System.Drawing;

namespace BT_KimMex.Models
{
    public class StockIssueViewModel
    {
        [Key]
        public string stock_issue_id { get; set; }
        [Display(Name = "Work Order Issued No.:")]
        public string stock_issue_number { get; set; }
        public string stock_issue_status { get; set; }
        public Nullable<bool> status { get; set; }
        [Display(Name = "Date:")]
        public Nullable<System.DateTime> created_date { get; set; }
        public string created_by { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceDate { get; set; }
        public string strInoviceNumber { get; set; }
        public string project_id { get; set; }
        public string project_name { get; set; }
        public string created_at_str { get; set; }
        public string created_by_str { get; set; }
        public string show_status_html { get; set; }
        public WareHouseViewModel warehouse { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<InventoryViewModel> inventorieshistoryqty { get; set; }
        public List<InventoryDetailViewModel> inventoryDetails { get; set; }
        public List<Entities.tb_attachment> attachments { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public List<WorkorderReturnedViewModel> workorderReturns { get; set; }
        public StockIssueViewModel()
        {
            inventories = new List<InventoryViewModel>();
            inventorieshistoryqty = new List<InventoryViewModel>();
            inventoryDetails = new List<InventoryDetailViewModel>();
            attachments = new List<Entities.tb_attachment>();
            rejects = new List<RejectViewModel>();
            workorderReturns = new List<WorkorderReturnedViewModel>();
        }

        public static List<StockIssueViewModel> GetStockIssueList(bool isAdmin,bool isSiteSupervisor,bool isSiteStockKeeper,bool isSiteManager,string userid, string status = null)
        {
            List<StockIssueViewModel> stockIssues = new List<StockIssueViewModel>();
            List<StockIssueViewModel> objs = new List<StockIssueViewModel>();
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //if (string.Compare(status, "All") == 0)
                //    objs = db.tb_stock_issue.OrderBy(m => m.stock_issue_number).Where(m => m.status == true).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.created_date }).ToList();
                //else
                //    objs = db.tb_stock_issue.OrderBy(m => m.stock_issue_number).Where(m => m.status == true&&string.Compare(m.stock_issue_status,status)==0).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.created_date }).ToList();



                if (isAdmin)
                {
                    objs = db.tb_stock_issue.OrderBy(m => m.stock_issue_number).Where(m => m.status == true).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.updated_date, created_by = m.created_by, project_id = m.project_id }).ToList();
                }
                else
                {
                    if (isSiteSupervisor)
                    {
                        List<StockIssueViewModel> models = db.tb_stock_issue.Where(m => m.status == true && string.Compare(m.created_by, userid) == 0).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.updated_date, created_by = m.created_by, project_id = m.project_id }).ToList();
                        foreach (var model in models)
                            objs.Add(model);
                    }
                    if (isSiteStockKeeper)
                    {
                        List<StockIssueViewModel> models = (from si in db.tb_stock_issue
                                                            join pro in db.tb_project on si.project_id equals pro.project_id
                                                            //join site in db.tb_site on pro.site_id equals site.site_id
                                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                                            join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                                                            where si.status == true && (((string.Compare(si.stock_issue_status, Status.Pending) == 0 || string.Compare(si.stock_issue_status, Status.Feedbacked) == 0) && string.Compare(sitestock.stock_keeper, userid) == 0) || string.Compare(si.approved_by, userid) == 0)
                                                            select new StockIssueViewModel()
                                                            {
                                                                stock_issue_id = si.stock_issue_id,
                                                                stock_issue_number = si.stock_issue_number,
                                                                stock_issue_status = si.stock_issue_status,
                                                                created_date = si.updated_date,
                                                                created_by = si.created_by,
                                                                project_id = si.project_id
                                                            }).ToList();

                        //var oobjs = models.GroupBy(s => s.stock_issue_id).Where(s => s.Count() > 1).Select(s => s.Key).ToList();
                        //foreach (var model in oobjs)
                        //{

                        //    objs.Add(models.Where(s=>string.Compare(s.stock_issue_id,model)==0).FirstOrDefault());
                        //}

                        //models = db.tb_stock_issue.Where(m => m.status == true && string.Compare(m.approved_by, userid) == 0).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.updated_date, created_by = m.created_by, project_id = m.project_id }).ToList();
                        //foreach (var model in models)
                        //    objs.Add(model);
                        foreach (var model in models)
                        {
                            var isexist = objs.Where(s => string.Compare(s.stock_issue_id, model.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                objs.Add(model);
                        }


                    }
                    if (isSiteManager)
                    {
                        List<StockIssueViewModel> models = (from si in db.tb_stock_issue
                                                            join pro in db.tb_project on si.project_id equals pro.project_id
                                                            join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                                            where si.status == true && (string.Compare(si.stock_issue_status, Status.Approved) == 0) && string.Compare(sm.site_manager, userid) == 0
                                                            select new StockIssueViewModel()
                                                            {
                                                                stock_issue_id = si.stock_issue_id,
                                                                stock_issue_number = si.stock_issue_number,
                                                                stock_issue_status = si.stock_issue_status,
                                                                created_date = si.updated_date,
                                                                created_by = si.created_by,
                                                                project_id = si.project_id
                                                            }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = objs.Where(s => string.Compare(model.stock_issue_id, s.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                objs.Add(model);
                        }
                        models = db.tb_stock_issue.Where(m => m.status == true && string.Compare(m.completed_by, userid) == 0).Select(m => new StockIssueViewModel { stock_issue_id = m.stock_issue_id, stock_issue_number = m.stock_issue_number, stock_issue_status = m.stock_issue_status, created_date = m.updated_date, created_by = m.created_by, project_id = m.project_id }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = objs.Where(s => string.Compare(model.stock_issue_id, s.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                objs.Add(model);
                        }
                    }
                }

                foreach (var obj in objs)
                {
                    //strWarehouse = string.Empty;
                    //strInvoiceNumber = string.Empty;
                    //strInvoiceDate = string.Empty;
                    //var oobjs = db.tb_inventory_detail.Where(m => string.Compare(m.inventory_ref_id, obj.stock_issue_id) == 0).Select(x => new { warehouseId = x.inventory_warehouse_id, invoiceDate = x.invoice_date, invoiceNumber = x.invoice_number }).ToList();
                    //var dupWarehouse = oobjs.GroupBy(x => x.warehouseId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    //var dupInvoiceNumber = oobjs.GroupBy(x => x.invoiceNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    //var dupInoviceDate = oobjs.GroupBy(x => x.invoiceDate).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    //foreach (var w in dupWarehouse)
                    //    strWarehouse = string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, w) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString());
                    //foreach (var ivn in dupInvoiceNumber)
                    //    strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, ivn);
                    //foreach (var ivd in dupInoviceDate)
                    //    strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(ivd).ToString("dd/MM/yyyy"));
                    //foreach(var oobj in oobjs)
                    //{
                    //    bool isDupWarehouse = dupWarehouse.Where(x => string.Compare(x, oobj.warehouseId)==0).Count()>0?true :false;
                    //    bool isDupInvoiceNumber = dupInvoiceNumber.Where(x => string.Compare(x, oobj.invoiceNumber) == 0).Count() > 0 ? true : false;
                    //    bool isDupInvoiceDate = dupInoviceDate.Where(x => x == oobj.invoiceDate).Count() > 0 ? true : false;
                    //    if(!isDupWarehouse) strWarehouse= string.Format("{0} {1},", strWarehouse, db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, oobj.warehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString());
                    //    if(!isDupInvoiceNumber) strInvoiceNumber = string.Format("{0} {1},", strInvoiceNumber, oobj.invoiceNumber);
                    //    if(!isDupInvoiceDate) strInvoiceDate = string.Format("{0} {1},", strInvoiceDate, Convert.ToDateTime(oobj.invoiceDate).ToString("dd/MM/yyyy"));
                    //}

                    StockIssueViewModel stockIssue = new StockIssueViewModel();
                    stockIssue.stock_issue_id = obj.stock_issue_id;
                    stockIssue.stock_issue_number = obj.stock_issue_number;
                    stockIssue.stock_issue_status = obj.stock_issue_status;
                    stockIssue.created_date = obj.created_date;
                    if (!string.IsNullOrEmpty(obj.project_id))
                    {
                        stockIssue.project_id = db.tb_project.Find(obj.project_id).project_full_name;
                        //stockIssue.strWarehouse = CommonFunctions.GetWarehousebyProject(obj.project_id).warehouse_name;
                        var wh = db.tb_warehouse.Where(s => string.Compare(s.warehouse_project_id, obj.project_id) == 0).FirstOrDefault();
                        if (wh != null)
                            stockIssue.strWarehouse = wh.warehouse_name;
                    }
                    //stockIssue.strWarehouse = strWarehouse;
                    //stockIssue.strInoviceNumber = strInvoiceNumber;
                    //stockIssue.strInvoiceDate = strInvoiceDate;
                    stockIssue.created_by = obj.created_by;
                    stockIssues.Add(stockIssue);
                }
                stockIssues = stockIssues.OrderByDescending(s => s.created_date).ToList();
            }
            return stockIssues;
        }
        public static int CountStockIssueApproval(bool isAdmin,bool isSiteStockKeeper, bool isSiteManager,string userid)
        {
            int totalItem = 0;
            List<StockIssueViewModel> stockIssues = new List<StockIssueViewModel>();
            List<StockIssueViewModel> objs = new List<StockIssueViewModel>();
            string strWarehouse = string.Empty;
            string strInvoiceNumber = string.Empty;
            string strInvoiceDate = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (isAdmin)
                {
                    objs = db.tb_stock_issue
                        .Where(s => s.status == true && (string.Compare(s.stock_issue_status, Status.Pending) == 0 || string.Compare(s.stock_issue_status, Status.Feedbacked) == 0 || string.Compare(s.stock_issue_status, Status.Approved) == 0))
                        .Select(s => new StockIssueViewModel()
                        {

                        }).ToList();
                }
                else
                {
                    if (isSiteStockKeeper)
                    {
                        List<StockIssueViewModel> models = (from si in db.tb_stock_issue
                                                            join pro in db.tb_project on si.project_id equals pro.project_id
                                                            //join site in db.tb_site on pro.site_id equals site.site_id
                                                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                                            join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                                                            where si.status == true && (string.Compare(si.stock_issue_status, Status.Pending) == 0 || string.Compare(si.stock_issue_status, Status.Feedbacked) == 0) && string.Compare(sitestock.stock_keeper, userid) == 0
                                                            select new StockIssueViewModel()
                                                            {
                                                                stock_issue_id = si.stock_issue_id,
                                                                stock_issue_number = si.stock_issue_number,
                                                                stock_issue_status = si.stock_issue_status,
                                                                created_date = si.updated_date,
                                                                created_by = si.created_by,
                                                                project_id = si.project_id
                                                            }).ToList();

                        foreach (var model in models)
                        {
                            var isexist = objs.Where(s => string.Compare(s.stock_issue_id, model.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                objs.Add(model);
                        }


                    }
                    if (isSiteManager)
                    {
                        List<StockIssueViewModel> models = (from si in db.tb_stock_issue
                                                            join pro in db.tb_project on si.project_id equals pro.project_id
                                                            join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                                            where si.status == true && (string.Compare(si.stock_issue_status, Status.Approved) == 0) && string.Compare(sm.site_manager, userid) == 0
                                                            select new StockIssueViewModel()
                                                            {
                                                                stock_issue_id = si.stock_issue_id,
                                                                stock_issue_number = si.stock_issue_number,
                                                                stock_issue_status = si.stock_issue_status,
                                                                created_date = si.updated_date,
                                                                created_by = si.created_by,
                                                                project_id = si.project_id
                                                            }).ToList();
                        foreach (var model in models)
                        {
                            var isExist = objs.Where(s => string.Compare(model.stock_issue_id, s.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                objs.Add(model);
                        }

                    }
                }
                

                totalItem = objs.Count();
            }
            return totalItem;
        }
        public static List<StockIssueViewModel> GetWorkOrderIssueListByDaterangeProjectStatus(string dateRange, string status,string projectId, bool isAdmin, bool isSiteSupervisor, bool isSiteStockKeeper, bool isSiteManager, string userid)
        {
            List<StockIssueViewModel> models = new List<StockIssueViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                List<tb_stock_issue> results = new List<tb_stock_issue>();

                if (isAdmin)
                {
                    results = db.tb_stock_issue.OrderBy(m => m.stock_issue_number).Where(m => m.status == true && m.created_date >= startDate && m.created_date <= endDate).ToList();
                }
                else
                {
                    if (isSiteSupervisor)
                    {
                        results = db.tb_stock_issue.Where(m => m.status == true && string.Compare(m.created_by, userid) == 0 && m.created_date >= startDate && m.created_date <= endDate).ToList();

                    }
                    if (isSiteStockKeeper)
                    {
                        var resultss = (from si in db.tb_stock_issue
                                        join pro in db.tb_project on si.project_id equals pro.project_id
                                        //join site in db.tb_site on pro.site_id equals site.site_id
                                        join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                                        join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                                        where si.status == true && (((string.Compare(si.stock_issue_status, Status.Pending) == 0 || string.Compare(si.stock_issue_status, Status.Feedbacked) == 0) && string.Compare(sitestock.stock_keeper, userid) == 0) || string.Compare(si.approved_by, userid) == 0 && si.created_date >= startDate && si.created_date <= endDate)
                                        select si).ToList();
                        foreach (var model in resultss)
                        {
                            var isexist = results.Where(s => string.Compare(s.stock_issue_id, model.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                results.Add(model);
                        }


                    }
                    if (isSiteManager)
                    {
                        var resultss = (from si in db.tb_stock_issue
                                        join pro in db.tb_project on si.project_id equals pro.project_id
                                        join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                        where si.status == true && (string.Compare(si.stock_issue_status, Status.Approved) == 0) && string.Compare(sm.site_manager, userid) == 0
                                        && si.created_date >= startDate && si.created_date <= endDate
                                        select si).ToList();
                        foreach (var model in resultss)
                        {
                            var isexist = results.Where(s => string.Compare(s.stock_issue_id, model.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                results.Add(model);
                        }

                        resultss = db.tb_stock_issue.Where(m => m.status == true && string.Compare(m.completed_by, userid) == 0 && m.created_date >= startDate && m.created_date <= endDate).ToList();
                        foreach (var model in resultss)
                        {
                            var isexist = results.Where(s => string.Compare(s.stock_issue_id, model.stock_issue_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                                results.Add(model);
                        }
                    }
                }

                if (string.Compare(status, "0") != 0)
                {
                    results=results.Where(s=>string.Compare(s.stock_issue_status, status) == 0).ToList();
                }
                if(string.Compare(projectId, "0") != 0)
                {
                    results=results.Where(s=>string.Compare(s.project_id,projectId) == 0).ToList();
                }

                foreach(var obj in results)
                {
                    StockIssueViewModel stockIssue = new StockIssueViewModel();
                    stockIssue.stock_issue_id = obj.stock_issue_id;
                    stockIssue.stock_issue_number = obj.stock_issue_number;
                    stockIssue.stock_issue_status = obj.stock_issue_status;
                    stockIssue.created_date = obj.created_date;
                    stockIssue.created_at_str = CommonClass.ToLocalTime(Convert.ToDateTime(stockIssue.created_date)).ToString("dd/MM/yyyy HH:mm");
                    if (!string.IsNullOrEmpty(obj.project_id))
                    {
                        stockIssue.project_id = db.tb_project.Find(obj.project_id).project_full_name;
                       
                        var wh = db.tb_warehouse.Where(s => string.Compare(s.warehouse_project_id, obj.project_id) == 0).FirstOrDefault();
                        if (wh != null)
                            stockIssue.strWarehouse = wh.warehouse_name;
                    }
                    
                    stockIssue.created_by = obj.created_by;
                    stockIssue.created_by_str = CommonFunctions.GetUserFullnamebyUserId(stockIssue.created_by);
                    stockIssue.show_status_html = ShowStatus.GetWOIShowStatusHTML(stockIssue.stock_issue_status);
                    models.Add(stockIssue);
                }
                models = models.OrderByDescending(s => s.created_date).ToList();
            }
            catch (Exception ex)
            {

            }
            return models;
        }
        public static StockIssueViewModel GetWorkOrderIssueDetail(string id)
        {
            StockIssueViewModel stockIssue = new StockIssueViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                stockIssue = db.tb_stock_issue.Where(x => x.stock_issue_id == id)
                    .Select(x => new StockIssueViewModel() { 
                        stock_issue_id = x.stock_issue_id, 
                        stock_issue_number = x.stock_issue_number, 
                        created_date = x.created_date, 
                        stock_issue_status = x.stock_issue_status, 
                        created_by = x.created_by, 
                        project_id = x.project_id 
                    }).FirstOrDefault();
                stockIssue.inventories = Inventory.GetInventoryItem(stockIssue.stock_issue_id);
                stockIssue.inventoryDetails = Inventory.GetInventoryDetail(stockIssue.stock_issue_id);
                stockIssue.attachments = Inventory.GetAttachments(stockIssue.stock_issue_id);
                stockIssue.rejects = CommonClass.GetRejectByRequest(id);
                stockIssue.project_name = db.tb_project.Find(stockIssue.project_id).project_full_name;
                stockIssue.warehouse = (from pro in db.tb_project
                                            //join site in db.tb_site on pro.site_id equals site.site_id
                                        join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                        from wh in pwh.DefaultIfEmpty()
                                        where string.Compare(pro.project_id, stockIssue.project_id) == 0
                                        select new WareHouseViewModel() { warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name }).FirstOrDefault();
                foreach (var ress in stockIssue.inventoryDetails)
                {
                    stockIssue.inventorieshistoryqty = (from h in db.tb_history_issue_qty
                                                   where ress.inventory_detail_id == h.inventory_detail_id
                                                   select new InventoryViewModel()
                                                   {
                                                       history_quantity = h.issue_qty,
                                                       inventory_detail_id = h.inventory_detail_id
                                                   }).ToList();
                }
            }
            catch (Exception ex) { }
            return stockIssue;
        }
    }
    public class StockIssueHistoryViewModel
    {
        [Key]
        public string history_issue_qty_id { get; set; }
        public string inventory_detail_id { get; set; }
        public Nullable<decimal> issue_qty { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<bool> status { get; set; }
        public string updated_by { get; set; }
    }
    public class PostWorkorderPlanningModel
    {
        public string project_id { get; set; }
        public Nullable<DateTime> date_from { get; set; }
        public Nullable<System.DateTime> date_to { get; set; }
        public List<WorkorderPlanningItemModel> planningItems { get; set; }

        public PostWorkorderPlanningModel()
        {
            planningItems=new List<WorkorderPlanningItemModel>();
        }

        public class WorkorderPlanningItemModel
        {
            public string item_id { get; set; }
            public string item_unit_id { get; set; }
            public Nullable<decimal> planning_qty { get; set; }
            public Nullable<decimal> labour_hour { get; set; }
            public string item_description { get; set; }

            
        }

        public static int GetWorkorderIssuePlanningLatestNumber()
        {
            int latestNumber = 1;
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var lastestObj = db.tb_workorder_planning.OrderByDescending(s => s.ordering).Where(s=>s.is_active==true).FirstOrDefault();
                if (lastestObj != null)
                {
                    latestNumber = Convert.ToInt32(lastestObj.ordering) + 1;
                }
            }
            return latestNumber;
        }

        public static decimal GetPlanningQuatityByDateProjectItem(DateTime issueDate, string projectId, string itemId, string unitId)
        {
            decimal planningQTY = 0;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                DateTime date = new DateTime(issueDate.Year, issueDate.Month, issueDate.Day, 0, 0,0);
                var planning = db.tb_workorder_planning.OrderByDescending(s => s.ordering).Where(s => s.is_active == true && s.date_from == date && string.Compare(s.project_id, projectId) == 0
                 && string.Compare(s.item_id, itemId) == 0 && string.Compare(s.item_unit_id, unitId) == 0).FirstOrDefault();
                if (planning != null)
                {
                    planningQTY = Convert.ToDecimal(planning.planning_qty);
                }
            }
            return planningQTY;
        }

    }

    public class WorkorderPlanningModel
    {
        public Nullable<DateTime> planning_date { get; set; }
        public string planning_date_str { 
            get { return Convert.ToDateTime(planning_date).ToString("dd/MM/yyyy"); }
            set { planning_date_str = value; } 
        }
        public string project_fullname { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_unit_name { get; set; }
        public string item_description { 
            get { return string.Format("{0}-{1}", item_code, item_name); } 
            set { item_description = value; } 
        }
        public Nullable<decimal> planning_qty { get; set; }
        public Nullable<decimal> labour_hour { get; set; }

        public static List<WorkorderPlanningModel> GetWorkorderPlanningListByDateRangeProject(string dateRange,string projectId)
        {
            List<WorkorderPlanningModel> models = new List<WorkorderPlanningModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                models = (from planning in db.tb_workorder_planning
                          join item in db.tb_product on planning.item_id equals item.product_id
                          join unit in db.tb_unit on planning.item_unit_id equals unit.Id
                          join project in db.tb_project on planning.project_id equals project.project_id

                          orderby item.product_number
                          where planning.is_active == true && string.Compare(planning.project_id, projectId) == 0 && planning.date_from >= startDate && planning.date_to <= endDate
                          select new WorkorderPlanningModel()
                          {
                              planning_date=planning.date_from,
                              project_fullname=project.project_full_name,
                              item_code=item.product_code,
                              item_name=item.product_name,
                              item_unit_name=unit.Name,
                              planning_qty=planning.planning_qty,
                              labour_hour=item.labour_hour,


                          }).ToList();

            }
            return models;
        }

    }

    public class IssueHistoryModel
    {
        public decimal totalIssueQty { get; set; }
    }

}