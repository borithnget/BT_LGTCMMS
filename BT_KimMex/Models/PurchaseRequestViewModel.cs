using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class PurchaseRequestViewModel
    {
        [Key]
        public string pruchase_request_id { get; set; }
        public string purchase_order_id { get; set; }
        public string purchase_request_number { get; set; }
        public string purchase_request_status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> updated_date { get; set; }
        public string updated_by { get; set; }
        public string checked_by { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public string approved_by { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string approved_comment { get; set; }
        public string checked_comment { get; set; }
        public Nullable<bool> status { get; set; }
        public string project_short_name { get; set; }
        public Nullable<bool> is_check { get; set; }
        public string str_created_date { get; set; }
        public string mr_id { get; set; }
        public string mr_number { get; set; }
        public string quote_number { get; set; }
        public string created_by_text { get; set; }
        public string purchase_request_status_full { get; set; }
        public List<PurchaseRequestDetailViewModel> poDetails { get; set; }
        public PurchaseOrderViewModel purchaseRequisition { get; set; }
        public List<Entities.tb_procress_workflow> processFlow { get; set; }
        public List<ProcessWorkflowModel> processWorkFlows { get; set; }
        public List<tb_purchase_request> purchaseOrderHistories { get; set; }
        public List<PurchaseOrderReportViewModel> poReports { get; set; }
        public PurchaseRequestViewModel()
        {
            poReports = new List<PurchaseOrderReportViewModel>();
        }
        public static PurchaseRequestViewModel GetPurchaseOrderItem(string id)
        {
            using(BT_KimMex.Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                PurchaseRequestViewModel model = db.tb_purchase_request.Where(s => string.Compare(s.pruchase_request_id, id) == 0)
                    .Select(s => new PurchaseRequestViewModel()
                    {
                        pruchase_request_id=s.pruchase_request_id,
                        purchase_request_number=s.purchase_request_number,
                        purchase_order_id=s.purchase_order_id,
                        purchase_request_status=s.purchase_request_status,
                        updated_date=s.updated_date,
                        created_by=s.created_by,
                        is_check=s.is_check,
                    }).FirstOrDefault();
                model.purchaseRequisition = db.tb_purchase_order.Where(s => string.Compare(s.purchase_order_id, model.purchase_order_id) == 0)
                    .Select(s => new PurchaseOrderViewModel() {purchase_order_id=s.purchase_order_id,purchase_oder_number=s.purchase_oder_number }).FirstOrDefault();

                //model.poDetails = db.tb_purchase_request_detail.Where(w => string.Compare(w.purchase_request_id, model.pruchase_request_id) == 0)
                //    .Select(s => new PurchaseRequestDetailViewModel()
                //    {
                //        pr_detail_id=s.pr_detail_id,
                //        purchase_request_id=s.purchase_request_id,
                //        po_report_id=s.po_report_id,

                //        status=s.status,
                //        amount=s.amount,
                //    }).ToList();

                model.poDetails = (from pod in db.tb_purchase_request_detail
                                   join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                   join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                   join supplier in db.tb_supplier on por.po_supplier_id equals supplier.supplier_id
                                   join quote_sup in db.tb_quote on quote.item_request_id equals quote_sup.mr_id 
                                   where string.Compare(pod.purchase_request_id, model.pruchase_request_id) == 0&& string.Compare(quote_sup.supplier_id,por.po_supplier_id)==0
                                   select new PurchaseRequestDetailViewModel()
                                   {
                                       pr_detail_id=pod.pr_detail_id,
                                       purchase_request_id=pod.purchase_request_id,
                                       po_report_id=pod.po_report_id,
                                       po_report_number=por.po_report_number,
                                       status=pod.status,
                                       amount=pod.amount,
                                       supplier_id=supplier.supplier_id,
                                       supplier_name=supplier.supplier_name,
                                       supplier_quote=quote_sup
                                   }).DistinctBy(s=>s.pr_detail_id).ToList();
                model.processFlow = db.tb_procress_workflow.OrderBy(s => s.created_at).Where(s => string.Compare(s.ref_id, model.pruchase_request_id) == 0).ToList();
                model.processWorkFlows = ProcessWorkflowModel.GetProcessWorkflowByRefId(model.pruchase_request_id);

                return model;
            }
        }
        public static List<tb_purchase_request> GetPurchaseOrderHistorybyPOIdAndQuoteId(string poId,string quoteId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var mr_id = (from quote in db.tb_purchase_order
                             join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                             where string.Compare(quote.purchase_order_id, quoteId) == 0
                             select pr.material_request_id).FirstOrDefault();
                return (from po in db.tb_purchase_request
                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                        where string.Compare(pr.material_request_id, mr_id) == 0 && po.status == true && string.Compare(po.pruchase_request_id, poId) != 0
                        select po).ToList();
            }
        }
    }
    public class PurchaseRequestDetailViewModel
    {
        [Key]
        public string pr_detail_id { get; set; }
        public string purchase_request_id { get; set; }
        public string po_report_id { get; set; }
        public string po_report_number { get; set; }
        public string status { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public Entities.tb_quote supplier_quote { get; set; }
        public PurchaseRequestDetailViewModel()
        {
            supplier_quote = new Entities.tb_quote() ;
        }
    }
    public class PurchaseOrderFilterResult
    {
        public tb_purchase_request po { get; set; }
        public tb_purchase_order quote { get; set; }
        public tb_item_request mr { get; set; }
        public tb_project proj { get; set; }
    }
}