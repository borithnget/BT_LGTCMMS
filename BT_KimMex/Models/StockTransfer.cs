using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Class;

namespace BT_KimMex.Models
{
    public class StockTransferViewModel
    {
        [Key]
        public string stock_transfer_id { get; set; }
        [Display(Name ="Transfer No.:")]
        public string stock_transfer_no { get; set; }
        [Display(Name ="Purchase Requisition No.:")]
        [Required(ErrorMessage ="Purchase Requisition Number is required.")]
        public string item_request_id { get; set; }
        public Nullable<bool> status { get; set; }
        public string stock_transfer_status { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public List<InventoryViewModel> inventoryDetails { get; set; }
        public List<STItemViewModel> itemTransfers { get; set; }
        [Display(Name = "Material Request No.:")]
        public string item_request_no { get; set; }
        [Display(Name ="Date:")]
        public string date { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public string strWarehouse { get; set; }
        public string strInvoiceNo { get; set; }
        public string strInvoiceDate { get; set; }
        public string create_by { get; set; }
        public string project_id { get; set; }
        public string project_fullname { get; set; }
        public string warehouse_id { get; set; }
        public string warehouse_name { get; set; }
        public string from_warehouse_id { get; set; }
        public string from_warehouse_name { get; set; }
        public string sr_status { get; set; }
        public string mr_status { get; set; }
        public Nullable<bool> is_return { get; set; }
        public string received_status { get; set; }
        public string project_short_name { get; set; }
        public string created_at_text { get; set; }
        public string created_by_text { get; set; }
        public string show_status { get; set; }
        public List<ProcessWorkflowModel> processWorkFlows { get; set; }
        public StockTransferViewModel()
        {
            inventoryDetails = new List<InventoryViewModel>();
            itemTransfers = new List<STItemViewModel>();
            rejects = new List<RejectViewModel>();
            processWorkFlows = new List<ProcessWorkflowModel>();
        }
        public static int CountApproval(bool isAdmin,bool isQAQC,bool isSM,string userid)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<StockTransferViewModel> stockTransfers = new List<StockTransferViewModel>();
                if (isAdmin)
                {
                    var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                             join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                             join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                             //join site in db.tb_site on pro.site_id equals site.site_id
                                             join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                             from wh in pwh.DefaultIfEmpty()
                                             join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                             from fwh in ffwh.DefaultIfEmpty()
                                             where st.status == true
                                             select new { st, ir, pro, wh, fwh }).ToList();
                    return allStockTransfers.Count();
                }
                else
                {

                    if (isQAQC)
                    {
                        //Pending and Feedback Request
                        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                 from wh in pwh.DefaultIfEmpty()
                                                 join qaqc in db.tb_warehouse_qaqc on st.from_warehouse_id equals qaqc.warehouse_id
                                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                 from fwh in ffwh.DefaultIfEmpty()
                                                 where st.status == true && string.Compare(qaqc.qaqc_id, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Pending) == 0 || string.Compare(st.stock_transfer_status, Status.Feedbacked) == 0)
                                                 select new { st, ir, pro, wh, fwh }).ToList();
                        foreach (var item in allStockTransfers)
                        {
                            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                            {


                                StockTransferViewModel stocktransfer = new StockTransferViewModel();
                                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                                stockTransfers.Add(stocktransfer);
                            }
                        }
                        //Approved History
                        var allStockTransfers1 = (from st in db.tb_stock_transfer_voucher
                                                  join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                  join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                  //join site in db.tb_site on pro.site_id equals site.site_id
                                                  join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                  from wh in pwh.DefaultIfEmpty()
                                                      //join qaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqc.warehouse_id
                                                  join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                  from fwh in ffwh.DefaultIfEmpty()
                                                  where st.status == true && string.Compare(st.approved_by, userid) == 0
                                                  select new { st, ir, pro, wh, fwh }).ToList();
                        foreach (var item in allStockTransfers1)
                        {
                            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                            {
                                StockTransferViewModel stocktransfer = new StockTransferViewModel();
                                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                                stockTransfers.Add(stocktransfer);
                            }
                        }
                    }
                    if (isSM)
                    {
                        //Pending and Feedback Request
                        var allStockTransfers = (from st in db.tb_stock_transfer_voucher
                                                 join ir in db.tb_item_request on st.item_request_id equals ir.ir_id
                                                 join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                                 //join site in db.tb_site on pro.site_id equals site.site_id
                                                 join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh
                                                 from wh in pwh.DefaultIfEmpty()

                                                 join fwh in db.tb_warehouse on st.from_warehouse_id equals fwh.warehouse_id into ffwh
                                                 from fwh in ffwh.DefaultIfEmpty()
                                                 join fsite in db.tb_site on fwh.warehouse_site_id equals fsite.site_id
                                                 join fpro in db.tb_project on fsite.site_id equals fpro.site_id
                                                 join sitem in db.tb_site_manager_project on fpro.project_id equals sitem.project_id


                                                 where st.status == true && string.Compare(sitem.site_manager, userid) == 0 && (string.Compare(st.stock_transfer_status, Status.Approved) == 0)
                                                 select new { st, ir, pro, wh, fwh }).ToList();
                        foreach (var item in allStockTransfers)
                        {
                            var isexist = stockTransfers.Where(s => string.Compare(s.stock_transfer_id, item.st.stock_transfer_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isexist)
                            {
                                StockTransferViewModel stocktransfer = new StockTransferViewModel();
                                stocktransfer.stock_transfer_id = item.st.stock_transfer_id;
                                stockTransfers.Add(stocktransfer);
                            }
                        }             
                    }
                    return stockTransfers.Count();
                }
            }
            catch(Exception ex)
            {

            }
            return 0;
        }
    }

    public class STItemViewModel
    {
        public string itemID { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string status { get; set; }
        public Nullable<decimal> stockBalance { get; set; }
        public Nullable<decimal> requestQty { get; set; }
        public string requestUnit { get; set; }
        public Nullable<decimal> approved_qty { get; set; }
        public string warehouseID { get; set; }
        public string warehouseName { get; set; }
        public string uom1_id { get; set; }
        public Nullable<decimal> uom1_qty { get; set; }
        public string uom2_id { get; set; }
        public Nullable<decimal> uom2_qty { get; set; }
        public string uom3_id { get; set; }
        public Nullable<decimal> uom3_qty { get; set; }
        public string uom4_id { get; set; }
        public Nullable<decimal> uom4_qty { get; set; }
        public string uom5_id { get; set; }
        public Nullable<decimal> uom5_qty { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string invoice_number { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public string item_status { get; set; }
        public string remark { get; set; }
        public Nullable<decimal> remain_qty { get; set; }
        public string Unit { get; set; }
        public string itemUnitName { get; set; }
        public string unitName { get; set; }
        public string requestUnitName { get; set; }
        public List<string> projectSiteManagers { get; set; }
        public Nullable<bool> is_return { get; set; }
    }

    public class StockTransferItemViewModel
    {
        public string st_detail_id { get; set; }
        public string st_ref_id { get; set; }
        public string status { get; set; }
        public string st_item_id { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string st_warehouse_id { get; set; }
        public string warehouseName { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string ir_no { get; set; }
        public string ir_status { get; set; }
        public string ir_id { get; set; }
        public Nullable<bool> is_return { get; set; }
        public Nullable<int> ordering_number { get; set; }

    }
    public class StockTransferFilterResult
    {
        public tb_stock_transfer_voucher st { get; set; }
        public tb_item_request ir { get; set; }
        public tb_project pro { get; set; }
        public tb_warehouse wh { get; set; }
        public tb_warehouse fwh { get; set; }
    }

}