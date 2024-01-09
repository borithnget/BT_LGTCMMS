using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Class;
namespace BT_KimMex.Models
{
    public class StockAdjustmentViewModel
    {
        [Key]
        public string stock_adjustment_id { get; set; }
        [Display(Name ="Stock Adjustment No.:")]
        public string stock_adjuctment_code { get; set; }
        [Display(Name ="Warehouse:")]
        public string warehouse_id { get; set; }
        [Display(Name ="Comment:")]
        public string comment { get; set; }
        [Display(Name ="Status:")]
        public string stock_adjustment_status { get; set; }
        public Nullable<bool> status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string warehouse_name { get; set; }
        public List<Models.InventoryViewModel> items { get; set; }
        public string project_short_name { get; set; }
        public StockAdjustmentViewModel()
        {
            items = new List<InventoryViewModel>();
        }

        public static int CountStockAdjustmntApproval(bool isAdmin,bool isQAQC,bool isSiteManger,bool isProjectManager,string userid)
        {
            List<StockAdjustmentViewModel> models = new List<StockAdjustmentViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                if (isAdmin)
                {
                    List<StockAdjustmentViewModel> objs = new List<StockAdjustmentViewModel>();

                    objs = db.tb_stock_adjustment.OrderByDescending(x => x.created_date).Where(x => x.status == true &&( string.Compare(x.stock_adjustment_status, Status.Pending)==0 || string.Compare(x.stock_adjustment_status,Status.Feedbacked)==0 || string.Compare(x.stock_adjustment_status,Status.Approved)==0 || string.Compare(x.stock_adjustment_status,Status.Checked)==0))
                        .Select(x => new Models.StockAdjustmentViewModel()
                    {
                        stock_adjustment_id = x.stock_adjustment_id,
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
                    List<StockAdjustmentViewModel> objs = new List<StockAdjustmentViewModel>();

                    if (isQAQC)
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                join qaqc in db.tb_warehouse_qaqc on x.warehouse_id equals qaqc.warehouse_id
                                where x.status == true && (((string.Compare(x.stock_adjustment_status, Status.Pending) == 0 || string.Compare(x.stock_adjustment_status, Status.Feedbacked) == 0) && string.Compare(qaqc.qaqc_id, userid) == 0))
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,

                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }

                    }
                    if (isSiteManger)
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                //join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                                where x.status == true && ((string.Compare(x.stock_adjustment_status, Status.Approved) == 0 && string.Compare(sm.site_manager, userid) == 0))
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (isProjectManager)
                    {
                        objs = (from x in db.tb_stock_adjustment
                                join wh in db.tb_warehouse on x.warehouse_id equals wh.warehouse_id
                                //join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                                join proj in db.tb_project on wh.warehouse_project_id equals proj.project_id
                                join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                                where x.status == true && ((string.Compare(x.stock_adjustment_status, Status.Checked) == 0 && string.Compare(pm.project_manager_id, userid) == 0))
                                select new StockAdjustmentViewModel()
                                {
                                    stock_adjustment_id = x.stock_adjustment_id,
                                }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.stock_adjustment_id, obj.stock_adjustment_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                }      
            }catch(Exception ex)
            {

            }
            return models.Count();
        }

    }
}