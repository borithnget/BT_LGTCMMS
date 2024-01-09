using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using BT_KimMex.Controllers;

namespace BT_KimMex.Models
{
    public class WorkorderReturnedViewModel
    {
        [Key]
        public string workorder_returned_id { get; set; }
        public string workorder_issued_id { get; set; }
        public string workorder_returned_number { get; set; }
        public string workorder_returned_status { get; set; }
        public Nullable<bool> status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_comment { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string checked_comment { get; set; }
        public string completed_by { get; set; }
        public Nullable<System.DateTime> completed_date { get; set; }
        public string completed_comment { get; set; }
        public List<InventoryViewModel> inventories { get; set; }
        public List<InventoryDetailViewModel> inventoryDetails { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public string workorder_issued_number { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public List<InventoryViewModel> issueItems { get; set; }
        public WorkorderReturnedViewModel()
        {
            inventoryDetails = new List<InventoryDetailViewModel>();
            issueItems = new List<InventoryViewModel>();
        }

        public static void RollbackWorkOrderIssueItems(string id, string returnId, bool isRollBack, string status = null)
        {
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                List<InventoryDetailViewModel> issueItems = new List<InventoryDetailViewModel>();
                List<InventoryDetailViewModel> issueReturnItems = new List<InventoryDetailViewModel>();
                issueItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).Select(x => new InventoryDetailViewModel() { inventory_detail_id = x.inventory_detail_id, inventory_item_id = x.inventory_item_id }).ToList();
                if (string.IsNullOrEmpty(status))
                {
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0).Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit }).ToList();
                }
                else
                    issueReturnItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, returnId) == 0 && string.Compare(x.item_status, status) == 0).Select(x => new InventoryDetailViewModel() { inventory_item_id = x.inventory_item_id, quantity = x.quantity, unit = x.unit }).ToList();

                foreach (InventoryDetailViewModel returnItem in issueReturnItems)
                {
                    string issueItemId = issueItems.Where(x => string.Compare(x.inventory_item_id, returnItem.inventory_item_id) == 0).Select(x => x.inventory_detail_id).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(issueItemId))
                    {
                        tb_inventory_detail issueDetail = db.tb_inventory_detail.Find(issueItemId);
                        if (isRollBack)
                            issueDetail.remain_quantity= issueDetail.remain_quantity + Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                        else
                            issueDetail.remain_quantity = issueDetail.remain_quantity - Class.CommonClass.ConvertMultipleUnit(returnItem.inventory_item_id, returnItem.unit, Convert.ToDecimal(returnItem.quantity));
                        db.SaveChanges();
                    }
                }
                UpdateStockIssueCompleted(id);
            }
        }
        public static void UpdateStockIssueCompleted(string id)
        {
            using (BT_KimMex.Entities.kim_mexEntities db = new kim_mexEntities())
            {
                int countNotCompleted = 0;
                var issueItems = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, id) == 0).ToList();

                foreach (var issueItem in issueItems)
                    if (issueItem.remain_quantity > 0) countNotCompleted++;
                tb_stock_issue stockIssue = db.tb_stock_issue.Find(id);
                stockIssue.is_completed = countNotCompleted == 0 ? true : false;
                db.SaveChanges();
            }
        }
        public static int CountWorkOrderReturnApproval(bool isAdmin,bool isQAQC,bool isSiteStockKeeper,bool isSiteManager,string userid)
        {
            int totalItems = 0;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<WorkorderReturnedViewModel> models = new List<WorkorderReturnedViewModel>();
                if (isAdmin)
                {
                    models = (from returned in db.tb_workorder_returned
                              where returned.status == true
                              && (string.Compare(returned.workorder_returned_status, Status.Pending) == 0 || string.Compare(returned.workorder_returned_status, Status.Feedbacked) == 0 || string.Compare(returned.workorder_returned_status, Status.Approved) == 0 || string.Compare(returned.workorder_returned_status, Status.Checked) == 0)
                              select new WorkorderReturnedViewModel()
                              {
                                  workorder_returned_id = returned.workorder_returned_id
                              }).ToList();
                }
                else
                {
                    List<WorkorderReturnedViewModel> objs = new List<WorkorderReturnedViewModel>();

                    if (isQAQC)
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                orderby returned.created_date descending
                                where returned.status == true && string.Compare(qaqc.qaqc_id, userid) == 0 && (string.Compare(returned.workorder_returned_status, Status.Pending) == 0 || string.Compare(Status.Feedbacked, returned.workorder_returned_status) == 0)
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }

                    }
                    if (isSiteStockKeeper)
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                                orderby returned.created_date descending
                                where returned.status == true && string.Compare(sitestock.stock_keeper, userid) == 0 && string.Compare(returned.workorder_returned_status, Status.Approved) == 0
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }

                    }
                    if (isSiteManager)
                    {
                        objs = (from returned in db.tb_workorder_returned
                                join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                                join pro in db.tb_project on issued.project_id equals pro.project_id
                                //join site in db.tb_site on pro.site_id equals site.site_id
                                join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                from wh in pwh.DefaultIfEmpty()
                                join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                orderby returned.created_date descending
                                where returned.status == true && string.Compare(sm.site_manager, userid) == 0 && string.Compare(returned.workorder_returned_status, Status.Checked) == 0
                                select new WorkorderReturnedViewModel()
                                {
                                    workorder_returned_id = returned.workorder_returned_id,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.workorder_returned_id, obj.workorder_returned_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                }
                return models.Count();
            }
            return totalItems;
                
        }

        public static List<WorkorderReturnedViewModel> GetWorkOrderReturnedByWorkorderIssueId(string id)
        {
            List<WorkorderReturnedViewModel> models = new List<WorkorderReturnedViewModel>();
            try
            {
                kim_mexEntities db= new kim_mexEntities();
                List<Models.InventoryDetailViewModel> workOrderReturnItems = new List<Models.InventoryDetailViewModel>();
                var issueReturns = db.tb_workorder_returned.Where(s => s.status == true && string.Compare(s.workorder_issued_id, id) == 0).ToList();
                foreach (var rs in issueReturns)
                {
                    models.Add(GetWorkOrderReturnItem(rs.workorder_returned_id));
                }
            }
            catch(Exception ex)
            {

            }
            return models;
        }

        public static WorkorderReturnedViewModel GetWorkOrderReturnItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                WorkorderReturnedViewModel model = new WorkorderReturnedViewModel();
                model = (from returned in db.tb_workorder_returned
                         join issued in db.tb_stock_issue on returned.workorder_issued_id equals issued.stock_issue_id
                         join pro in db.tb_project on issued.project_id equals pro.project_id
                         //join site in db.tb_site on pro.site_id equals site.site_id
                         join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                         from wh in pwh.DefaultIfEmpty()
                         where string.Compare(returned.workorder_returned_id, id) == 0
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
                         }).FirstOrDefault();
                var inventoryDetails = db.tb_inventory_detail.Where(s => string.Compare(s.inventory_ref_id, id) == 0).ToList();
                foreach (var item in inventoryDetails)
                {
                    InventoryDetailViewModel inventoryDetail = new InventoryDetailViewModel();
                    tb_product product = db.tb_product.Find(item.inventory_item_id);
                    inventoryDetail.inventory_detail_id = item.inventory_detail_id;
                    inventoryDetail.inventory_item_id = item.inventory_item_id;
                    inventoryDetail.itemCode = product.product_code;
                    inventoryDetail.itemName = product.product_name;
                    inventoryDetail.itemUnit = product.product_unit;
                    inventoryDetail.itemunitname = db.tb_unit.Find(inventoryDetail.itemUnit).Name;
                    inventoryDetail.quantity = item.quantity;
                    inventoryDetail.unit = item.unit;
                    inventoryDetail.unitName = db.tb_unit.Find(item.unit).Name;
                    inventoryDetail.item_status = item.item_status;
                    inventoryDetail.invoice_date = item.invoice_date;
                    inventoryDetail.invoice_number = item.invoice_number;
                    inventoryDetail.remark = item.remark;
                    model.inventoryDetails.Add(inventoryDetail);
                }
                StockIssueController obj = new StockIssueController();
                model.issueItems = StockIssueViewModel.GetWorkOrderIssueDetail(model.workorder_issued_id).inventories;
                return model;
            }
        }

    }
}