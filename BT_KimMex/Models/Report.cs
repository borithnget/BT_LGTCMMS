using BT_KimMex.Class;
using BT_KimMex.Entities;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
//using System.Web;
//using static System.Data.Entity.Infrastructure.Design.Executor;

namespace BT_KimMex.Models
{
    public class ItemRequestReport
    {
        public string project_full_name { get; set; }
        public string item_request_number { get; set; }
        public string item_request_date { get; set; }
        public string job_category_id { get; set; }
        public string job_category_code { get; set; }
        public string job_category_name { get; set; }
        public string item_id { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_unit { get; set; }
        public decimal item_quantity { get; set; }
        public decimal boq_quantity { get; set; }
        public string remark { get; set; }
    }
    public class PurchaseOrderReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        [Required(ErrorMessage ="From Date is required.")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        [Required(ErrorMessage = "To Date is required.")]
        public Nullable<System.DateTime> dateTo { get; set; }
        [Display(Name ="Status: ")]
        public string poStatus { get; set; }
        [Display(Name = "Supplier: ")]
        public string poSuppplier { get; set; }
        [Display(Name ="Project: ")]
        public string project_id { get; set; }
        public DateTime purchase_order_date { get; set; }
        public string project_full_name { get; set; }
        public string prepared_by { get; set; }
        public string description { get; set; }
        public string purchase_order_number { get; set; }
        public string supplierId { get; set; }
        public string purchaseOrderId { get; set; }
        public Nullable<int> isFinance { get; set; }

       
    }

    public class PurchaseOrderResponseReport
    {
        
        public PurchaseOrderReportViewModel poReport { get; set; }
        public ProjectViewModel project { get; set; }
        public tb_quote supplierQuote { get; set; }
        public List<POInvoiceModel> invoices { get; set; }
        public int countPayment { get; set; }
        public List<tb_receive_item_voucher> received { get; set; }
        public tb_po_invoice_document document { get; set; }

        public PurchaseOrderResponseReport()
        {
            supplierQuote = new tb_quote();
            invoices = new List<POInvoiceModel>();
            received = new List<tb_receive_item_voucher>();
            document= new tb_po_invoice_document();
        }
        

        public static List<PurchaseOrderResponseReport> GeneratePurchaseOrderReportList(PurchaseOrderReport model)
        {
            List<PurchaseOrderResponseReport> models = new List<PurchaseOrderResponseReport>();
            
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                DateTime dateFromConvert=Convert.ToDateTime(model.dateFrom);
                DateTime dateToConvert = Convert.ToDateTime(model.dateTo);

                DateTime startDate = new DateTime(dateFromConvert.Year, dateFromConvert.Month, dateFromConvert.Day, 0, 0, 0);
                DateTime endDate = dateToConvert.AddDays(1).AddMilliseconds(-1);

                var poReports = (from pod in db.tb_purchase_request_detail 
                                 join po in db.tb_purchase_request on pod.purchase_request_id equals po.pruchase_request_id
                                 join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                join suplier in db.tb_supplier on por.po_supplier_id equals suplier.supplier_id
                                join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                orderby por.created_date
                                  where por.created_date >= startDate && por.created_date <= endDate && quote.status==true
                                  && string.Compare(pod.status,Status.Approved)==0 && po.status==true
                                  select new PurchaseOrderReportViewModel()
                                  {
                                      po_report_id = por.po_report_id,
                                      po_report_number = por.po_report_number,
                                      po_ref_id = por.po_ref_id,
                                      po_supplier_id = por.po_supplier_id,
                                      created_date = por.created_date,
                                      vat_status = por.vat_status,
                                      is_completed = por.is_completed,
                                      supplier_entity = suplier,
                                      quote_entity = quote,
                                      pr=pr,
                                      mr=mr,
                                      po=po,
                                      pod=pod
                                  }).ToList();

                if (!string.IsNullOrEmpty(model.project_id))
                {
                    poReports = poReports.Where(s => string.Compare(s.mr.ir_project_id, model.project_id) == 0).ToList();
                }
                if (!string.IsNullOrEmpty(model.supplierId))
                {
                    poReports = poReports.Where(s => string.Compare(s.po_supplier_id, model.supplierId) == 0).ToList();
                }
                
                foreach(var por in poReports)
                {
                    int maxPayment = 0;
                    PurchaseOrderResponseReport obj = new PurchaseOrderResponseReport();
                    obj.poReport = por;
                    if (obj.poReport != null)
                    {
                        if(obj.poReport.mr!=null)
                            obj.project = ProjectViewModel.GetProjectItem(obj.poReport.mr.ir_project_id);
                        obj.poReport.created_by = CommonClass.GetUserFullnameByUserId(por.po.created_by);
                        obj.poReport.po_status = por.po.purchase_request_status;
                    }
                    //Get Quote Supplier Term and Condition
                    tb_quote sup_quote = db.tb_quote.OrderByDescending(s=>s.created_date).Where(s => s.status == true && string.Compare(s.supplier_id, obj.poReport.supplier_entity.supplier_id) == 0 
                    && string.Compare(s.mr_id, obj.poReport.pr.purchase_requisition_id) == 0).FirstOrDefault();
                    if (sup_quote != null)
                    {
                        obj.supplierQuote = sup_quote;
                    }
                    obj.invoices = POInvoiceModel.GetPOInvoiceByPOReportId(obj.poReport.po_report_id).OrderByDescending(s=>s.countPayment).ToList();

                    int countPayment = obj.invoices.Select(s => s.countPayment).FirstOrDefault();
                    if(countPayment>maxPayment)
                        maxPayment= countPayment;
                    obj.countPayment = maxPayment;

                    #region receiving slip 
                    obj.received = (from grn in db.tb_receive_item_voucher
                                     orderby grn.approved_date descending
                                     //join po in db.tb_purchase_request on grn.ref_id equals po.pruchase_request_id
                                     where string.Compare(grn.po_report_number, por.po_report_number) == 0 && string.Compare(grn.received_status, Status.Completed) == 0
                                     && grn.status == true
                                     select grn).ToList();

                    #endregion
                    

                    models.Add(obj);
                }
            }
            catch(Exception ex)
            {

            }
            return models;
        }

    }
    public class PurchaseOrderSupplierReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }
        [Display(Name = "Supplier: ")]
        //[Required(ErrorMessage ="Supplier is required.")]
        public string supplier { get; set; }
        [Display(Name ="Project: ")]
        public string project_id { get; set; }
    }

    public class StockMovementReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        [Required(ErrorMessage ="Date from is required.")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage ="Date to is required.")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }
        public string poSuppplier { get; set; }
        public DateTime purchase_order_date { get; set; }
        public string project_full_name { get; set; }
        public string prepared_by { get; set; }
        public string description { get; set; }
        public string purchase_order_number { get; set; }
    }

    public class ReturnItemtoSupplierReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string item_return_number { get; set; }
        public string supplier_name { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string warehouse_name { get; set; }
        public string item_return_id { get; set; }
    }
    public class StockBalanceBywarehouseReport
    {
        public string item_id { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string itemTypeName { get; set; }
        public string warehouseName { get; set; }
        public Decimal unit_price { get; set; }
        public Decimal total_quantity { get; set; }
        public Decimal total { get; set; }
    }
    //add 10/24/2018
    public class StockBalanceBydateAndwarehouseReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date:")]
        [Required(ErrorMessage = "Date is required.")]
        public Nullable<System.DateTime> date { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateinventory { get; set; }

        [Display(Name = "warehouseName:")]
        public string warehouseName { get; set; }

        public string item_id { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemUnit { get; set; }
        public string itemTypeName { get; set; }
        public Decimal unit_price { get; set; }
        public Decimal total_quantity { get; set; }
        public Decimal total { get; set; }
        public Decimal bigbalance { get; set; }
        public Decimal in_receive { get; set; }
        public Decimal in_issue_return { get; set; }
        public Decimal out_return { get; set; }
        public Decimal out_transfer { get; set; }
        public Decimal out_damage { get; set; }
        public Decimal out_issue { get; set; }
        public Decimal total_in { get; set; }
        public Decimal total_out { get; set; }
        public Decimal ending_balance { get; set; }
        public string inventory_status_id { get; set; }
        public string stock_status_id { get; set; }
        public string warehouse_id { get; set; }
        public string project_id { get; set; }
    }
    //end
    public class PurchaseOrderVsItemReceivedReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }
        public string purchase_order_id { get; set; }
        public string po_supplier_id { get; set; }
        public string po_detail_id { get; set; }
        public string ref_id { get; set; }
        public string ri_ref_id { get; set; }
        public string ri_item_id { get; set; }
        public string ri_detail_id { get; set; }
        
        public Nullable<System.DateTime> created_date { get; set; }
        public string purchase_order_number { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string item_id { get; set; }
        
        public string unit { get; set; }
        //public Nullable<decimal> unit_price { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
        public Nullable<decimal> different_quatity { get; set; }
        public Nullable<decimal> received_quantity { get; set; }
    }
    public class StockUsageBySiteWithRemainBalanceReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }
        public string site_id { get; set; }
        public string Site_name { get; set; }
        public string Created_date { get; set; }
        public string Status { get; set; }
        public string Project_id { get; set; }
        public string Site_manager_projecu_id { get; set; }
        public string Project_name { get; set; }
        public string Inventory_id { get; set; }
        public string inventory_ref_id { get; set; }
        public Nullable<decimal> Quatity { get; set; }
        public string Product_id { get; set; }
        public string Product_Name { get; set; }
        public string Product_Unit { get; set; }

        public string product_code { get; set; }

        public string Stock_adjustment_id { get; set; }

        public string stock_issue_id { get; set; }
        public string stock_issue_number { get; set; }

    }

    public class WorkOrderIssueVSWorkOrderRequestReportResult
    {
        public Nullable<System.DateTime> dateFrom { get; set; }
        public Nullable<DateTime> dateTo { get; set; }
        public string projectFullname { get; set; }
        public string warehouseName { get; set; }
        public List<WorkOrderIsuseVSWorkOrderReturnReport> items { get; set; }
        public List<StockIssueViewModel> workOrderIssues { get; set; }
        public WorkOrderIssueVSWorkOrderRequestReportResult()
        {
            items = new List<WorkOrderIsuseVSWorkOrderReturnReport>();
            workOrderIssues=new List<StockIssueViewModel>();
        }

        public static WorkOrderIssueVSWorkOrderRequestReportResult GenerateWorkOrderIssueVSWorkOrderReturnReport(DateTime dateFrom, DateTime dateTo, String project)
        {
            WorkOrderIssueVSWorkOrderRequestReportResult model = new WorkOrderIssueVSWorkOrderRequestReportResult();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                model.dateFrom = dateFrom;
                model.dateTo = dateTo;
                model.projectFullname = project;
                model.warehouseName = string.Empty;

                List<tb_stock_issue> workOrderIssues = new List<tb_stock_issue>();
                if (string.IsNullOrEmpty(project))
                {
                    workOrderIssues= (from woi in db.tb_stock_issue
                                      where woi.status == true 
                                      && string.Compare(woi.stock_issue_status, Status.Completed) == 0 
                                      && woi.created_date >= dateFrom && woi.created_date <= dateTo
                                      select woi).ToList();
                }
                else
                {
                    workOrderIssues = (from woi in db.tb_stock_issue
                                       where woi.status == true && string.Compare(woi.project_id, project) == 0
                                       && string.Compare(woi.stock_issue_status, Status.Completed) == 0
                                       && woi.created_date >= dateFrom && woi.created_date <= dateTo
                                       && string.Compare(woi.project_id,project)==0
                                       select woi).ToList();
                }
                
                
                foreach(var ws in workOrderIssues)
                {
                    StockIssueViewModel workOrder = StockIssueViewModel.GetWorkOrderIssueDetail(ws.stock_issue_id);
                    workOrder.workorderReturns = WorkorderReturnedViewModel.GetWorkOrderReturnedByWorkorderIssueId(workOrder.stock_issue_id);


                    model.workOrderIssues.Add(workOrder);
                }
                

            }
            catch (Exception ex)
            {

            }
            return model;
        }
    }
    public class WorkOrderIsuseVSWorkOrderReturnReport
    {
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date From:")]
        public Nullable<System.DateTime> dateFrom { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date To:")]
        public Nullable<System.DateTime> dateTo { get; set; }

        public string project_name { get; set; }
        public string project_id { get; set; }
        public string issue_id { get; set; }
        public string return_id { get; set; }
        public string issue_no { get; set; }
        public string issue_item_code { get; set; }
        public string issue_description { get; set; }
        public string issue_unit { get; set; }
        public string issue_qty { get; set; }
        public string return_no { get; set; }
        public string return_item_code { get; set; }
        public string return_description { get; set; }
        public string return_unit { get; set; }
        public string return_qty { get; set; }
        public string item_id { get; set; }
        public string woi_id { get; set; }
        public string wrs { get; set; }
        public string status_r { get; set; }
        public string wis { get; set; }
        public string status_i { get; set; }
        public string inventory_type { get; set; }


        

    }
    public class WorkOrderIssueReportInformationModel
    {
        public Nullable<decimal> quantity { get; set; }
        public Nullable<decimal> labour_unit { get; set; }
        public Nullable<decimal> price { get;}
    }
    public class DetailPOReceivingReportResponseModel
    {
        public tb_receive_item_voucher grn { get; set; }
        public PurchaseOrderReportViewModel poReport { get; set; }
        public List<ItemReceiveViewModel> receivedHistories { get; set; }
        public DetailPOReceivingReportResponseModel()
        {
            grn = new tb_receive_item_voucher();
            poReport = new PurchaseOrderReportViewModel();
            receivedHistories = new List<ItemReceiveViewModel>();
        }

        public static List<DetailPOReceivingReportResponseModel> GetDetailPOReceivingReportsList(DateTime? dateFrom,DateTime? dateTo)
        {
            List<DetailPOReceivingReportResponseModel> models = new List<DetailPOReceivingReportResponseModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                DateTime newDateTo = Convert.ToDateTime(dateTo).AddDays(1).AddMilliseconds(-1);
                var grnEntities = db.tb_receive_item_voucher.OrderByDescending(s => s.created_date)
                    .Where(s => s.status == true && string.Compare(s.received_type, "Purchase Order") == 0 
                    //&& String.Compare(s.received_status, Status.Completed) == 0 
                    && s.created_date>=dateFrom && s.created_date<=newDateTo)
                    .ToList();
                foreach(var obj in grnEntities)
                {
                    tb_po_report poReport = db.tb_po_report.Where(s => string.Compare(s.po_report_number, obj.po_report_number) == 0).FirstOrDefault();
                    if(poReport!= null)
                    {
                        DetailPOReceivingReportResponseModel model = new DetailPOReceivingReportResponseModel();
                        model.grn = obj;
                        model.poReport = PurchaseOrderReportViewModel.GetPOReportItemDetail(poReport.po_report_id);
                        /* Received History */
                        var histories = db.tb_receive_item_voucher.OrderBy(s => s.created_date).Where(s => s.status == true && string.Compare(s.ref_id, obj.ref_id) == 0).ToList();
                        foreach(var his in histories)
                        {
                            model.receivedHistories.Add(Inventory.GetItemReceivedViewDetail(his.receive_item_voucher_id));
                        }

                        models.Add(model);
                    }
                }
            }catch(Exception ex)
            {

            }
            return models;
        }

    }
}