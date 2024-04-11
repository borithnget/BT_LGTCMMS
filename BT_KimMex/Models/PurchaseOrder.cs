using BT_KimMex.Class;
using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BT_KimMex.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using MoreLinq;


namespace BT_KimMex.Models
{
    public class PurchaseOrderViewModel
    {
        [Key]
        public string purchase_order_id { get; set; }
        [Display(Name ="Quote List No. :")]
        public string purchase_oder_number { get; set; }
        
        [Display(Name ="PR Ref :")]
        [Required(ErrorMessage ="Item Request Reference is required.")]
        public string item_request_id { get; set; }
        public string purchase_order_status { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<bool> vat { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<System.DateTime> checked_date { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
        public string created_by { get; set; }
        public string created_by_fullname { get; set; }
        public string checked_by { get; set; }
        public string approved_by { get; set; }
        public string ir_no { get; set; }
        public string ir_noMR { get; set; }
        [Display(Name ="Project Name :")]
        public string ir_project_id { get; set; }
        public string project_short_name { get; set; }
        public string project_full_name { get; set; }
        public string soNo { get; set; }
        /// Rathana Add 10/04/2019
        [Display(Name = "Quote Project Number :")]
        public string POLNumber { get; set; }

        [Display(Name = "Shipping To :")]
        public string ShippingTo { get; set; }
        /// End Rathana Add
        public string warehouseID { get; set; }
        public string strReqeustedDate { get; set; }
        public Nullable<bool> is_po_checked { get; set; }
        public bool is_completed { get; set; }
        public string purchase_order_show_status { get; set; }
        public List<PurchaseOrderDetailViewModel> poDetails { get; set; }
        public List<PurchaseOrderAttachment> poAttachments { get; set; }
        public List<PurchaseOrderItemSupplier> poSuppliers { get; set; }
        public List<PurchaseOrderVsItemReceivedViewModel> poVsIRe { get; set; }
        public List<string> ReportNumberVAT { get; set; }
        public List<string> ReportNumberNonVAT { get; set; }
        public List<RejectViewModel> rejects { get; set; }
        public List<PurchaseOrderReportViewModel> poReports { get; set; }
        public List<Entities.tb_procress_workflow> processFlows { get; set; }
        public List<PurchaseOrderDetailViewModel> groupSuppliers { get; set; }
        public List<SupplierViewModel> supplierQuotes { get; set; }
        public string mr_id { get; set; }
        public string mr_no { get; set; }
        public string created_at_text { get; set; }
        //public string created_at_text
        //{
        //    get { return Convert.ToDateTime(created_date).ToString("dd/MM/yyyy"); }
        //    set { created_at_text = value; }
        //}
        public string created_by_text
        {
            get { return CommonFunctions.GetUserFullnamebyUserId(created_by); }
            set { created_by_text = value; }
        }
        public string show_status_text
        {
            get { return ShowStatus.GetQuoteShowStatusFull(purchase_order_status); }
            set { show_status_text = value; }
        }
        public List<ProcessWorkflowModel> processWorkFlows { get; set; }
        public List<tb_purchase_order> quoteHistories { get; set; }
        public string created_by_signature { get; set; }
        public string approved_by_signature { get; set; }
        public string checked_by_signature { get; set; }
        public PurchaseOrderViewModel()
        {
            poDetails = new List<PurchaseOrderDetailViewModel>();
            poSuppliers = new List<PurchaseOrderItemSupplier>();
            rejects = new List<RejectViewModel>();
            poVsIRe = new List<PurchaseOrderVsItemReceivedViewModel>();
            ReportNumberVAT = new List<string>();
            ReportNumberNonVAT = new List<string>();
            poReports = new List<PurchaseOrderReportViewModel>();
            groupSuppliers = new List<PurchaseOrderDetailViewModel>();
            supplierQuotes = new List<SupplierViewModel>();
            processWorkFlows = new List<ProcessWorkflowModel>();
            quoteHistories = new List<tb_purchase_order>();
        }

        public static List<QuoteSupplierItemModel> GetQuoteItembySupplier(string quoteId,SupplierViewModel supplier)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                 return (from pod in db.tb_purchase_order_detail
                            join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                            where string.Compare(pod.purchase_order_id, quoteId) == 0 && string.Compare(pos.supplier_id, supplier.supplier_id) == 0 && pos.is_check == true
                            select new QuoteSupplierItemModel{ pod=pod,pos= pos }).ToList();
            }
        }
        public static QuoteBySupplierAmountModel QuoteAmountBySupplier(string quoteId, SupplierViewModel supplier,string quoteStatus)
        {
            List<QuoteSupplierItemModel> items = PurchaseOrderViewModel.GetQuoteItembySupplier(quoteId, supplier);
            if(string.Compare(quoteStatus, "Pending") != 0 && string.Compare(quoteStatus, Status.Rejected) != 0 && string.Compare(quoteStatus, Status.RequestCancelled) != 0 && string.Compare(quoteStatus, Status.cancelled) != 0)
            {
               
                decimal VATAmount = (decimal)items.Where(s => s.pos.vat == true).Sum(g => g.pos.unit_price * g.pod.po_quantity);
                decimal NONVATAmount = (decimal)items.Where(s => s.pos.vat == false).Sum(g => g.pos.unit_price * g.pod.po_quantity);
                VATAmount = (decimal)(VATAmount + (VATAmount * (supplier.discount / 100)));
                return new QuoteBySupplierAmountModel()
                {
                    VATAmount = VATAmount,
                    NONVATAmount = NONVATAmount
                };
            }
            else
            {
                decimal VATAmount = (decimal)items.Where(s => s.pos.vat == true).Sum(g => g.pos.unit_price * g.pod.quantity);
                decimal NONVATAmount = (decimal)items.Where(s => s.pos.vat == false).Sum(g => g.pos.unit_price * g.pod.quantity);
                VATAmount = (decimal)(VATAmount + (VATAmount * (supplier.discount / 100)));
                return new QuoteBySupplierAmountModel()
                {
                    VATAmount = VATAmount,
                    NONVATAmount = NONVATAmount
                };
            }
            
        }
        public static List<tb_purchase_order> GetQuoteHistoriesbyIdAndPR(string quoteId,string prId)
        {
            using (kim_mexEntities db = new kim_mexEntities()) {
                var mr_id = db.tb_purchase_requisition.Find(prId).material_request_id;
                return (from quote in db.tb_purchase_order
                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                        orderby quote.created_date descending
                        where quote.status == true && string.Compare(quote.purchase_order_id, quoteId) != 0 && string.Compare(pr.material_request_id, mr_id) == 0
                        select quote).ToList();
            }
        }
        public static PurchaseOrderViewModel GetPurchaseOrderDetail(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from po in db.tb_purchase_order
                         join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                         join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                         join proj in db.tb_project on ir.ir_project_id equals proj.project_id
                         where po.purchase_order_id == id
                         select new PurchaseOrderViewModel()
                         {
                             purchase_order_id = po.purchase_order_id,
                             item_request_id = po.item_request_id,
                             ir_no = pr.purchase_requisition_number,
                             ir_project_id = ir.ir_project_id,
                             project_full_name = proj.project_full_name,
                             purchase_oder_number = po.purchase_oder_number,
                             purchase_order_status = po.purchase_order_status,
                             created_date = po.created_date,
                             POLNumber = po.pol_project_short_name_number,
                             ShippingTo = po.shipping_to,
                             created_by = po.created_by,
                             approved_by= po.approved_by,
                             approved_date= po.approved_date,
                             checked_by= po.checked_by,
                             checked_date= po.checked_date,
                             is_completed = po.is_completed,
                             is_po_checked = po.is_po_checked,
                         }).FirstOrDefault();

                if (model != null)
                {
                    string host = HttpContext.Current.Request.Url.Authority;
                    //host = "http://vuthytep-001-site7.atempurl.com";
                    model.created_by_signature =string.IsNullOrEmpty(CommonClass.GetUserSignature(model.created_by)) ?string.Empty: string.Format("{0}{1}",EnumConstants.domainName, CommonClass.GetUserSignature(model.created_by));
                    model.approved_by_signature =string.IsNullOrEmpty(CommonClass.GetUserSignature(model.approved_by)) ?string.Empty:  string.Format("{0}{1}", EnumConstants.domainName, CommonClass.GetUserSignature(model.approved_by));
                    model.checked_by_signature = string.IsNullOrEmpty(CommonClass.GetUserSignature(model.checked_by)) ?string.Empty: string.Format("{0}{1}", EnumConstants.domainName, CommonClass.GetUserSignature(model.checked_by));

                    List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                    poDetails = (from dPO in db.tb_purchase_order_detail
                                 join item in db.tb_product on dPO.item_id equals item.product_id
                                 join unit in db.tb_unit on item.product_unit equals unit.Id
                                 orderby dPO.ordering_number
                                 where dPO.purchase_order_id == model.purchase_order_id
                                 select new PurchaseOrderDetailViewModel()
                                 {
                                     po_detail_id = dPO.po_detail_id,
                                     purchase_order_id = dPO.purchase_order_id,
                                     item_id = dPO.item_id,
                                     quantity = dPO.quantity,
                                     item_unit = dPO.item_unit,
                                     product_code = item.product_code,
                                     product_name = item.product_name,
                                     product_unit = item.product_unit,
                                     product_unit_name = unit.Name,
                                     ordering_number = dPO.ordering_number,
                                     unit_price = dPO.unit_price,
                                     item_status = dPO.item_status,
                                     po_quantity = dPO.po_quantity,
                                     po_unit = dPO.po_unit,
                                     uom = db.tb_multiple_uom.Where(x => x.product_id == dPO.item_id).Select(x => new ProductViewModel() { uom1_id = x.uom1_id, uom1_qty = x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault(),
                                     lump_sum_discount_amount = dPO.lump_sum_discount_amount,
                                 }).ToList();
                    foreach (var pod in poDetails)
                    {
                        List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                        //pois = (from pos in db.tb_po_supplier
                        //        join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                        //        orderby pos.sup_number
                        //        where pos.po_detail_id == pod.po_detail_id
                        //        select new PurchaseOrderItemSupplier() {
                        //            po_supplier_id = pos.po_supplier_id,
                        //            po_detail_id = pos.po_detail_id,
                        //            unit_price = pos.unit_price,
                        //            supplier_id = sup.supplier_id,
                        //            supplier_name = sup.supplier_name,
                        //            sup_number = pos.sup_number,
                        //            is_selected = pos.is_selected,
                        //            Reason = pos.Reason,
                        //            po_quantity = pos.po_qty,
                        //            vat =pos.vat,
                        //            is_check =pos.is_check,
                        //        }).ToList();
                        var quoteSuppiers = (from pos in db.tb_po_supplier
                                             join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                             orderby pos.sup_number
                                             where pos.po_detail_id == pod.po_detail_id
                                             select new
                                             {
                                                 pos,
                                                 sup
                                             }).ToList();
                        foreach (var quoteSupplier in quoteSuppiers)
                        {
                            PurchaseOrderItemSupplier poi = new PurchaseOrderItemSupplier();
                            poi.po_supplier_id = quoteSupplier.pos.po_supplier_id;
                            poi.po_detail_id = quoteSupplier.pos.po_detail_id;
                            poi.unit_price = quoteSupplier.pos.unit_price;
                            poi.supplier_id = quoteSupplier.sup.supplier_id;
                            poi.supplier_name = quoteSupplier.sup.supplier_name;
                            poi.sup_number = quoteSupplier.pos.sup_number;
                            poi.is_selected = quoteSupplier.pos.is_selected;
                            poi.Reason = quoteSupplier.pos.Reason;
                            poi.po_quantity = quoteSupplier.pos.po_qty;
                            poi.vat = quoteSupplier.pos.vat;
                            poi.is_check = quoteSupplier.pos.is_check;
                            poi.discount = quoteSupplier.sup.discount;

                            if (quoteSupplier.pos.original_price.HasValue)
                            {
                                poi.original_price = quoteSupplier.pos.original_price;
                                poi.discount_percentage = quoteSupplier.pos.discount_percentage;
                            }
                            else
                            {
                                poi.original_price = quoteSupplier.pos.unit_price;
                                poi.discount_percentage = 0;
                            }
                            poi.discount_amount = quoteSupplier.pos.discount_amount ?? 0;
                            poi.lump_sum_discount_amount = quoteSupplier.pos.lumpsum_discount_amount;

                            var supTC = db.tb_quote.OrderByDescending(s => s.created_date).Where(s => s.status == true && string.Compare(s.supplier_id, poi.supplier_id) == 0 && string.Compare(s.mr_id, model.item_request_id) == 0).FirstOrDefault();
                            if (supTC != null)
                            {
                                poi.incoterm = supTC.incoterm;
                                poi.payment = supTC.payment;
                                poi.delivery = supTC.delivery;
                                poi.shipment = supTC.shipment;
                                poi.warranty = supTC.warranty;
                                poi.vendor_ref = supTC.vendor_ref;
                            }

                            pois.Add(poi);
                        }

                        pod.poSuppliers = pois;
                    }
                    model.poDetails = poDetails;

                    List<PurchaseOrderAttachment> poAttachments = new List<PurchaseOrderAttachment>();
                    poAttachments = (from poAtt in db.tb_po_attachment
                                     where poAtt.po_id == model.purchase_order_id
                                     select new PurchaseOrderAttachment()
                                     {
                                         po_id = poAtt.po_id,
                                         po_attachment_id = poAtt.po_attachment_id,
                                         po_attachment_name = poAtt.po_attachment_name,
                                         po_attachment_extension = poAtt.po_attachment_extension,
                                         file_path = poAtt.file_path
                                     }).ToList();
                    model.poAttachments = poAttachments;
                    model.rejects = CommonClass.GetRejectByRequest(id);
                    model.processFlows = db.tb_procress_workflow.OrderBy(s => s.created_at).Where(s => string.Compare(s.ref_id, model.purchase_order_id) == 0).ToList();

                    //model.groupSuppliers = model.poDetails.DistinctBy(s => s.supplier_id).ToList();
                    var supplierQuotes = (from pod in db.tb_purchase_order_detail
                                          join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                                          join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                          where string.Compare(pod.purchase_order_id, model.purchase_order_id) == 0
                                          select sup).ToList().DistinctBy(s => s);
                    foreach (var sup in supplierQuotes)
                    {
                        var supplierLumpsum = (from pod in db.tb_purchase_order_detail
                                               join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                                               where string.Compare(pod.purchase_order_id, model.purchase_order_id) == 0 && string.Compare(pos.supplier_id, sup.supplier_id) == 0
                                               select pos).FirstOrDefault();
                        SupplierViewModel supplier = SupplierViewModel.ConvertEntityToModel(sup);
                        supplier.lump_sum_discount_amount = supplierLumpsum.lumpsum_discount_amount;
                        supplier.is_quote_selected = supplierLumpsum.is_selected;
                        model.supplierQuotes.Add(supplier);
                    }
                    model.processWorkFlows = ProcessWorkflowModel.GetProcessWorkflowByRefId(model.purchase_order_id);
                }
            }
            return model;
        }
    }
  public class PurchaseOrderDetailViewModel
    {
        [Key]
        public string po_detail_id { get; set; }
        public string ir_detail2_id { get; set; }
        public string purchase_order_id { get; set; }
        public string item_id { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public Nullable<decimal> quantity { get; set; }
        public string item_unit { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public string item_status { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<bool> item_vat { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
        public string po_unit { get; set; }
        public string brand_id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public string unit { get; set; }
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
        public int unitLevel { get; set; }
        public ProductViewModel uom { get; set; }
        public string warehouseID { get; set; }
        public string product_unit_name { get; set; }
        public string unit_name { get; set; }
        public string item_unit_name { get; set; }
        public Nullable<decimal> remain_quantity { get; set; }
        public Nullable<decimal> original_price { get; set; }
        public Nullable<decimal> discount_percentage { get; set; }
        public List<PurchaseOrderItemSupplier> poSuppliers { get; set; }
        public Nullable<decimal> discount_amount { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
        public Nullable<int> ordering_number { get; set; }
        //  public string PO_Unit { get; set; }
        public PurchaseOrderDetailViewModel()
        {
            poSuppliers = new List<Models.PurchaseOrderItemSupplier>();
            uom = new ProductViewModel();
        }
    }
    public class PurchaseOrderItemSupplier
    {
        public string po_supplier_id { get; set; }
        public string po_detail_id { get; set; }
        public string supplier_id { get; set; }
        public string supplier_name { get; set; }
        public string supplier_address { get; set; }
        public string supplier_email { get; set; }
        public string supplier_phone { get; set; }
        public Nullable<decimal> unit_price { get; set; }
        public Nullable<decimal> original_unit_price { get; set; }
        public string sup_number { get; set; }
        public Nullable<bool> is_selected { get; set; }
        public Nullable<bool> vat { get; set; }
        public string Reason { get; set; }
        public Nullable<decimal> po_quantity { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string po_unit { get; set; }
        public Nullable<bool> is_check { get; set; }
        public Nullable<bool> item_vat { get; set; }
        public string incoterm { get; set; }
        public string payment { get; set; }
        public string delivery { get; set; }
        public string shipment { get; set; }
        public string warranty { get; set; }
        public string vendor_ref { get; set; }
        public Nullable<decimal> original_price { get; set; }
        public Nullable<decimal> discount_percentage { get; set; }
        public Nullable<decimal> discount_amount { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
        public List<PurchaseOrderDetailViewModel> items { get; set; }
        public List<PurchaseOrderReportViewModel> reports { get; set; }
        public PurchaseOrderItemSupplier()
        {
            items = new List<PurchaseOrderDetailViewModel>();
        }

        public static implicit operator List<object>(PurchaseOrderItemSupplier v)
        {
            throw new NotImplementedException();
        }
    }

    public class PurchaseOrderVsItemReceivedViewModel
    {
        public string purchaseOrder_id { get; set; }
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
        public Nullable<decimal> item_received { get; set; }
        
        public List<PurchaseOrderDetailViewModel> items { get; set; }
        public PurchaseOrderVsItemReceivedViewModel()
        {
            items = new List<PurchaseOrderDetailViewModel>();
        }
    }

    public class PurchaseOrderAttachment
    {
        public string po_attachment_id { get; set; }
        public string po_attachment_name { get; set; }
        public string po_attachment_extension { get; set; }
        public string file_path { get; set; }
        public string po_id { get; set; }
    }

    public class PurchaseOrderSupplier
    {
        public string purchaseOrderId { get; set; }
        public string purchaseOrderNumber { get; set; }
        public string poSupplierId { get; set; }
        public bool isVAT { get; set; }
        public string warehouseId { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public string itemRequestNumber { get; set; }
        public string poReportNumber { get; set; }
    }
    public class PurchaseOrderReportViewModel
    {
        [Key]
        public string po_report_id { get; set; }
        public string po_report_number { get; set; }
        public string po_ref_id { get; set; }
        public string po_supplier_id { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        public Nullable<bool> vat_status { get; set; }
        public Nullable<bool> is_invoice { get; set; }
        public bool is_completed { get; set; }
        public Nullable<decimal> lump_sum_discount_amount { get; set; }
        public SupplierViewModel supplier { get; set; }
        public PurchaseOrderViewModel purchaseOrder { get; set; }
        public List<PurchaseOrderDetailViewModel> purchaseOrderDetails { get; set; }
        public tb_supplier supplier_entity { get; set; }
        public tb_purchase_order quote_entity { get; set; }
        public tb_purchase_requisition pr { get; set; }
        public tb_item_request mr { get; set; }
        public tb_purchase_request po { get; set; }
        public tb_purchase_request_detail pod { get; set; }
        public tb_project project_entity { get; set; }
        public string created_at_text { get { return Convert.ToDateTime(created_date).ToString("yyyy/MM/dd"); } set { created_at_text = value; } }
        public string created_by { get; set; }
        public string po_status { get; set; }
        public List<PurchaseOrderDetailItemEntityModel> items { get; set; }
        
        public PurchaseOrderReportViewModel()
        {
            purchaseOrderDetails = new List<PurchaseOrderDetailViewModel>() ;
            supplier_entity = new tb_supplier();
            quote_entity = new tb_purchase_order();
            mr = new tb_item_request();
            pr = new tb_purchase_requisition();
            items = new List<PurchaseOrderDetailItemEntityModel>();
            project_entity = new tb_project();
            
        }

        public static List<PurchaseOrderReportViewModel> GetPORequestByDateRange(string dateRange)
        {
            List<PurchaseOrderReportViewModel> models = new List<PurchaseOrderReportViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                models = (from por in db.tb_po_report
                          join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                          join suplier in db.tb_supplier on por.po_supplier_id equals suplier.supplier_id
                          orderby por.created_date descending
                          where por.created_date >= startDate && por.created_date <= endDate
                          select new PurchaseOrderReportViewModel()
                          {
                              po_report_id=por.po_report_id,
                              po_report_number=por.po_report_number,
                              po_ref_id=por.po_ref_id,
                              po_supplier_id=por.po_supplier_id,
                              created_date=por.created_date,
                              vat_status=por.vat_status,
                              is_completed=por.is_completed,
                              supplier_entity=suplier,
                              quote_entity=quote,
                              //created_at_text=Convert.ToDateTime(por.created_date).ToString("yyyy/MM/dd"),
                          }).ToList();
                       

            }
            catch(Exception ex)
            {

            }

            return models;
        }

        public static PurchaseOrderReportViewModel GetPOReportItemDetail(string poReportId)
        {
            PurchaseOrderReportViewModel model = new PurchaseOrderReportViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                model = (from por in db.tb_po_report
                                 join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                 join suplier in db.tb_supplier on por.po_supplier_id equals suplier.supplier_id
                                  join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                  join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                  join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                 orderby por.created_date descending
                                 where string.Compare(por.po_report_id,poReportId)==0
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
                                     mr=mr,
                                     pr=pr,
                                     project_entity= proj,
                                     //created_at_text=Convert.ToDateTime(por.created_date).ToString("yyyy/MM/dd"),
                                 }).FirstOrDefault();
                model.items = (from purd in db.tb_purchase_order_detail
                               
                               join purs in db.tb_po_supplier on purd.po_detail_id equals purs.po_detail_id
                               join prod in db.tb_product on purd.item_id equals prod.product_id
                               join unit in db.tb_unit on purd.po_unit equals unit.Id
                               orderby purd.ordering_number
                               where string.Compare(purd.purchase_order_id, model.po_ref_id) == 0 && string.Compare(purd.item_status, "approved") == 0
                               && purs.is_selected == true && string.Compare(purs.supplier_id, model.po_supplier_id) == 0 && purd.item_vat == model.vat_status
                               select new PurchaseOrderDetailItemEntityModel  {
                                   purd= purd,purs= purs,prod=prod,unit=unit
                               }).ToList();
            }
            catch(Exception ex)
            {

            }
            return model;
        }

        public static List<PurchaseOrderReportViewModel> GetPORequestbyDateRange(DateTime? dateFrom, DateTime? dateTo)
        {
            DateTime dd = Convert.ToDateTime(dateFrom);
            DateTime startDate = new DateTime(dd.Year, dd.Month, dd.Day);
            DateTime endDate = Convert.ToDateTime(dateTo).AddDays(1).AddMilliseconds(-1);
            List<PurchaseOrderReportViewModel> models = new List<PurchaseOrderReportViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                var results = (from por in db.tb_po_report
                          join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                          join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                          orderby por.created_date descending
                          where po.status == true && (po.purchase_request_status == Status.Completed || string.Compare(po.purchase_request_status, Status.Approved) == 0) 
                          && po.created_date >= startDate && po.created_date <= endDate
                               select new 
                          {
                              por,quote,po
                          }).ToList();

                foreach(var rs in results)
                {
                    models.Add(PurchaseOrderReportViewModel.GetPOReportItemDetail(rs.por.po_report_id));
                }

            }
            catch (Exception ex)
            {

            }

            return models;
        }

        public static List<PurchaseOrderReportViewModel> GetPONotYetIssueInvoice()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from por in db.tb_po_report
                        join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                        join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                        orderby por.created_date descending
                        where po.status == true && (po.purchase_request_status == Status.Completed || string.Compare(po.purchase_request_status, Status.Approved) == 0)
                        && (por.is_invoice == false || por.is_invoice==null)
                        select new PurchaseOrderReportViewModel()
                        {
                            po_report_id= por.po_report_id,
                            po_report_number= por.po_report_number,

                        }).ToList();
            }
        }
        public static List<PurchaseRequestDetailViewModel> GetPOReportbyPurchaseOrderId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<PurchaseRequestDetailViewModel> models = new List<PurchaseRequestDetailViewModel>();
                models = (from pod in db.tb_purchase_request_detail
                                   join por in db.tb_po_report on pod.po_report_id equals por.po_report_id
                                   join quote in db.tb_purchase_order on por.po_ref_id equals quote.purchase_order_id
                                   join supplier in db.tb_supplier on por.po_supplier_id equals supplier.supplier_id
                                   join quote_sup in db.tb_quote on quote.item_request_id equals quote_sup.mr_id
                                   where string.Compare(pod.purchase_request_id, id) == 0 && string.Compare(quote_sup.supplier_id, por.po_supplier_id) == 0
                                   select new PurchaseRequestDetailViewModel()
                                   {
                                       pr_detail_id = pod.pr_detail_id,
                                       purchase_request_id = pod.purchase_request_id,
                                       po_report_id = pod.po_report_id,
                                       po_report_number = por.po_report_number,
                                       status = pod.status,
                                       amount = pod.amount,
                                       supplier_id = supplier.supplier_id,
                                       supplier_name = supplier.supplier_name,
                                       supplier_quote = quote_sup
                                   }).DistinctBy(s => s.pr_detail_id).ToList();
                return models;
            }
        }
        public static tb_purchase_request_detail GetPOReportAmountByPOReportId(string poReportId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from pod in db.tb_purchase_request_detail
                        where string.Compare(pod.po_report_id, poReportId) == 0
                        select pod).FirstOrDefault();
            }
        }
    
        public static tb_po_report_cancelled GetPOReportCancelled(bool isVAT, bool isLOP)
        {
            tb_po_report_cancelled result = new tb_po_report_cancelled();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                result = db.tb_po_report_cancelled.Where(s => s.IsReuse == false && s.IsLPO == isLOP && s.IsVAT == isVAT).FirstOrDefault();
            }catch(Exception ex)
            {

            }
            return result;
        }
    }
    public class PurchaseOrderReportGenerateModel
    {
        public string purchaseOrderId { get; set; }
        public string purchaseOrderDate { get; set; }
        public string purchaseOrderNumber { get; set; }
        public string ItemRequestNumber { get; set; }
        public string SMFullName { get; set; }
        public string SMTelephone { get; set; }
        public string SMEmail { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierTelephone { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierVATIN { get; set; }
        public string ProjectFullName { get; set; }
        public string PreparedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string VAT { get; set; }
        public string Status { get; set; }
        
        public string PRRefId { get; set; }
        public string PRRefNo { get; set; }
        public string CreatedAt { get; set; }
        public string CFOApprovedAt { get; set; }
        public string DirectorApprovedAt { get; set; }
        public string createdBySignature { get; set; }
        public string approvedBySignature { get; set; }
        public string checkedBySignature { get; set; }
        public string CFOSignature { get; set; }
        public string DirectorSignature { get; set; }
        public string SubTotal
        {
            get;
            set;
        }
        public string GrandTotal { get; set; }
        public string AmountBeforeDiscount { get; set; }
        public string LumpSumDiscountAmount { get; set; }
        public string incoterm { get; set; }
        public string payment { get; set; }
        public string delivery { get; set; }
        public string shipment { get; set; }
        public string warranty { get; set; }
        public string vendor_ref { get; set; }      
        public string ir_noMR { get; set; }
        public string projectshortname { get; set; }
        public string soNo { get; set; }
        public bool is_local { get; set; }
        public bool is_vat { get; set; }
        public string vat_number { get; set; }
        public bool isPO { get; set; }
        public bool isQuote { get; set; }
        public bool isMR { get; set; }
        public bool isSignature { get; set; }

        public List<PurchaseOrderReportGenerateItemsModel> poItems { get; set; }

        public ItemRequestViewModel materialRequest { get; set; }
        public PurchaseOrderViewModel quote { get; set; }
        public PurchaseOrderReportGenerateModel()
        {
            poItems = new List<PurchaseOrderReportGenerateItemsModel>();
            materialRequest = new ItemRequestViewModel();
            quote = new PurchaseOrderViewModel();
        }
        public static PurchaseOrderReportGenerateModel GetPurchaseOrderGenerate(string poId, string supplierId,bool isVAT, bool isPO = false, bool isQuote = false, bool isMR = false, bool isSignature = false)
        {
            PurchaseOrderReportGenerateModel model = new PurchaseOrderReportGenerateModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                PurchaseOrderViewModel obj = Class.CommonClass.GetPOSupplierItem(poId);
                if (obj != null)
                {
                    string PRRefNo = string.Empty;
                    string poSupplierNumber = string.Empty;
                    string supplierName = string.Empty;
                    string incoterm = string.Empty;
                    string payment = string.Empty;
                    string delivery = string.Empty;
                    string shipment = string.Empty;
                    string warranty = string.Empty;
                    string vendor_ref = string.Empty;
                    string brand = string.Empty;
                    string ir_noMR = string.Empty;
                    string projectshortname = string.Empty;
                    string soNo = string.Empty;
                    //string is_local = string.Empty;
                    string supplierAddress = string.Empty;
                    string supplierPhone = string.Empty;
                    string supplierEmail = string.Empty;
                    string siteManagerId = string.Empty;
                    string siteManagerName = string.Empty;
                    string siteAddress = string.Empty;
                    string siteManagerTelephone = string.Empty;
                    string siteManagerEmail = string.Empty;
                    string preparedBy = string.Empty;
                    string checkedBy = string.Empty;
                    string approvedBy = string.Empty;
                    decimal amount = 0;
                    double totalAmount = 0,totalAmountBeforeDiscount=0,lumpSumDiscountAmount=0;
                    decimal vat = 0;
                    string status = string.Empty;
                    bool is_local = false;
                    poSupplierNumber = db.tb_po_report.Where(x => x.po_supplier_id == supplierId && x.po_ref_id == obj.purchase_order_id && x.vat_status == isVAT).Select(x => x.po_report_number).FirstOrDefault();
                    supplierName = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_name).FirstOrDefault();
                    var stype=db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.is_local).FirstOrDefault();
                    if (stype != null)
                        is_local =(bool) stype;

                    supplierAddress = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_address).FirstOrDefault();
                    supplierPhone = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_phone).FirstOrDefault();
                    supplierEmail = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_email).FirstOrDefault();
                    vat = Convert.ToDecimal(obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.discount).FirstOrDefault());

                    //Rathana Edit Here
                    var IsHasShippingTo = db.tb_purchase_order.FirstOrDefault(x => x.purchase_order_id == poId);
                    if (string.IsNullOrEmpty(IsHasShippingTo.shipping_to))
                    {
                        siteManagerId = (from po in db.tb_purchase_order
                                         join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                         join ir in db.tb_item_request
                                         on pr.material_request_id
                                         equals ir.ir_id

                                         join pro in db.tb_project
                                         on ir.ir_project_id
                                         equals pro.project_id

                                         join sm in db.tb_site_manager_project
                                         on pro.project_id
                                         equals sm.project_id
                                         where po.purchase_order_id == poId
                                         select sm.site_manager).FirstOrDefault();

                        var siteManager = db.tb_user_detail.Where(x => x.user_detail_id == siteManagerId).FirstOrDefault();
                        if (siteManager != null)
                        {
                            siteManagerName = siteManager.user_first_name + " " + siteManager.user_last_name;
                            siteManagerTelephone = siteManager.user_telephone;
                            siteManagerEmail = siteManager.user_email;
                        }
                    }
                    else
                    {
                        var shippingToUser = db.tb_user_detail.FirstOrDefault(x => x.user_detail_id == IsHasShippingTo.shipping_to);
                        siteManagerName = shippingToUser.user_first_name + " " + shippingToUser.user_last_name;
                        siteManagerTelephone = shippingToUser.user_telephone;
                        siteManagerEmail = shippingToUser.user_email;
                    }
                    //End Rathana Edit

                    siteAddress = (from po in db.tb_purchase_order
                                   join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                   join ir in db.tb_item_request
                                   on pr.material_request_id
                                   equals ir.ir_id
                                   join pro in db.tb_project
                                   on ir.ir_project_id
                                   equals pro.project_id
                                   join si in db.tb_site
                                   on pro.site_id
                                   equals si.site_id
                                   where po.purchase_order_id == poId
                                   select si.site_name).FirstOrDefault();

                    PRRefNo = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request
                               on pr.material_request_id
                               equals ir.ir_id
                               where po.purchase_order_id == poId
                               select pr.purchase_requisition_number
                               ).FirstOrDefault();

                    var mr = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request
                               on pr.material_request_id
                               equals ir.ir_id
                               where po.purchase_order_id == poId
                               select ir
                               ).FirstOrDefault();
                    ir_noMR = mr.ir_no;

                    projectshortname = (from po in db.tb_purchase_order
                                        join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                        join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                                        join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                        where po.purchase_order_id == poId
                                        select pro.project_short_name
                               ).FirstOrDefault();

                    soNo = (from po in db.tb_purchase_order
                            join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                            join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                            join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                            where po.purchase_order_id == poId
                            select pro.project_no
                               ).FirstOrDefault();

                    //var purchase = db.tb_purchase_request.Where(x => x.purchase_order_id == poId).FirstOrDefault();
                    var purchase = db.tb_purchase_request.OrderBy(s => s.approved_date).Where(x => x.purchase_order_id == poId && string.Compare(x.purchase_request_status, BT_KimMex.Class.Status.RequestCancelled) != 0).FirstOrDefault();
                    preparedBy = db.AspNetUsers.Where(x => x.Id == purchase.created_by).Select(x => x.UserName).FirstOrDefault();
                    // checkedBy = db.AspNetUsers.Where(x => x.Id == purchase.checked_by).Select(x => x.UserName).FirstOrDefault();
                    preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);
                    checkedBy = CommonClass.GetUserFullname(purchase.checked_by);
                    approvedBy = CommonClass.GetUserFullname(purchase.approved_by);
                    string createdBySignature= string.IsNullOrEmpty(purchase.updated_by) ? db.tb_user_detail.Where(s=>string.Compare(purchase.created_by,s.user_id)==0).FirstOrDefault().user_signature : db.tb_user_detail.Where(s => string.Compare(purchase.updated_by, s.user_id) == 0).FirstOrDefault().user_signature; 
                    

                    status = string.Compare(obj.purchase_order_status, "Completed") == 0 ? "Approved by Director" : string.Empty;

                    //Get Quote Supplier Term and Condition
                    tb_quote sup_quote = db.tb_quote.Where(s => s.status == true && string.Compare(s.supplier_id, supplierId) == 0 && string.Compare(s.mr_id, IsHasShippingTo.item_request_id) == 0).FirstOrDefault();
                    if (sup_quote != null)
                    {
                        incoterm = sup_quote.incoterm;
                        payment = sup_quote.payment;
                        delivery = sup_quote.delivery;
                        shipment = sup_quote.shipment;
                        warranty = sup_quote.warranty;
                        vendor_ref = sup_quote.vendor_ref;
                    }

                    var itemSuppliers = obj.poSuppliers.Where(x => x.supplier_id == supplierId).ToList();
                    if (itemSuppliers.Any())
                    {
                        foreach (var item in obj.poDetails)
                        {
                            var sup = item.poSuppliers;
                            decimal unitPrice = 0,originalPrice=0,discountPercentage=0;

                            if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                            {
                                unitPrice = Convert.ToDecimal(item.unit_price);
                                originalPrice = item.original_price.HasValue ? Convert.ToDecimal(item.original_price) : Convert.ToDecimal(item.unit_price);
                                discountPercentage = item.discount_percentage.HasValue ? Convert.ToDecimal(item.discount_percentage) : 0;

                            }
                                
                            else if (string.Compare(obj.purchase_order_status, "Approved") == 0)
                            {
                                var posup = db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).FirstOrDefault();
                                unitPrice = Convert.ToDecimal(posup.unit_price);
                                originalPrice = posup.unit_price.HasValue ? Convert.ToDecimal(posup.original_price) : Convert.ToDecimal(posup.unit_price);
                                discountPercentage = posup.discount_percentage.HasValue ? Convert.ToDecimal(posup.discount_percentage) : 0;
                            }
                            foreach (var ssup in sup)
                            {
                                if (ssup.supplier_id == supplierId)
                                {
                                    amount = amount + Convert.ToDecimal(item.po_quantity * unitPrice);
                                }
                            }
                        }

                        List<PurchaseOrderReportGenerateItemsModel> items = new List<PurchaseOrderReportGenerateItemsModel>();
                        double vat1 = vat>0? (double) vat/100: 0;
                        if (!isVAT)
                            vat1 = 0;

                        foreach (var item in obj.poDetails)
                        {

                            brand = db.tb_brand.Where(x => x.brand_id == item.brand_id).Select(x => x.brand_name).FirstOrDefault();

                            var sup = item.poSuppliers;
                            decimal unitPrice = 0, originalPrice = 0,discountPercentage=0,discountAmount=0 ;
                            if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                            {
                                unitPrice = Convert.ToDecimal(item.unit_price);
                                originalPrice = item.original_price.HasValue ? Convert.ToDecimal(item.original_price) : Convert.ToDecimal(item.unit_price);
                                discountPercentage = item.discount_percentage.HasValue ? Convert.ToDecimal(item.discount_percentage) : 0;
                                discountAmount = item.discount_amount.HasValue ? Convert.ToDecimal(item.discount_amount) : 0;
                            }
                                
                            else if (string.Compare(obj.purchase_order_status, "Approved") == 0 || string.Compare(obj.purchase_order_status, "mr edited") == 0)
                            {
                                var posup = db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).FirstOrDefault();
                                unitPrice = Convert.ToDecimal(posup.unit_price);
                                originalPrice = posup.unit_price.HasValue ? Convert.ToDecimal(posup.original_price) : Convert.ToDecimal(posup.unit_price);
                                discountPercentage = posup.discount_percentage.HasValue ? Convert.ToDecimal(posup.discount_percentage) : 0;
                                discountAmount = posup.discount_amount.HasValue ? Convert.ToDecimal(posup.discount_percentage) : 0;
                            }

                            //discountAmount = originalPrice * (discountPercentage / 100);

                            foreach (var ssup in sup)
                            {
                                if (ssup.supplier_id == supplierId)
                                {
                                    var show = Convert.ToBoolean(item.item_vat);
                                    if (show == isVAT)
                                    {
                                        
                                        double Amount = Convert.ToDouble(item.po_quantity * unitPrice);
                                        totalAmount = totalAmount + Amount;

                                        double amountBeforeDiscount = 0;
                                        if (discountAmount==0)
                                            amountBeforeDiscount = Convert.ToDouble(item.po_quantity * unitPrice);
                                        else
                                            amountBeforeDiscount = Convert.ToDouble(item.po_quantity * (originalPrice - discountAmount));
                                        totalAmountBeforeDiscount = totalAmountBeforeDiscount + amountBeforeDiscount;
                                        
                                        PurchaseOrderReportGenerateItemsModel poItem = new PurchaseOrderReportGenerateItemsModel();                                       

                                        poItem.ItemCode = item.product_code;
                                        poItem.ItemName = item.product_name;
                                        poItem.ItemUnit = db.tb_unit.Find(item.po_unit).Name;
                                        poItem.itemvat = Convert.ToBoolean(item.item_vat).ToString();
                                        poItem.podetailid = item.po_detail_id;
                                        poItem.POQquantity = Convert.ToDouble(item.po_quantity).ToString();
                                        //poItem.UnitPrice=string.Format("{0:N3}", unitPrice);
                                        poItem.UnitPrice = string.Format("{0:N3}", originalPrice);
                                        poItem.Discount = string.Format("{0:N3}", discountPercentage);
                                        poItem.brand = brand;

                                        //poItem.Amount = String.Format("{0:N3}", Convert.ToDouble(item.po_quantity * unitPrice));
                                        poItem.Amount = String.Format("{0:N3}",amountBeforeDiscount);

                                        items.Add(poItem);

                                    }
                                }

                            }

                        }
                        var lumpsumDiscount = obj.poDetails.GroupBy(s => s.lump_sum_discount_amount).Sum(s => s.Key);
                        totalAmount = totalAmount -Convert.ToDouble(lumpsumDiscount);

                        double vatAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(totalAmount) * vat1), 2);
                        double grandTotal = Convert.ToDouble(totalAmount) + vatAmount;

                        model.purchaseOrderId = obj.purchase_order_id;
                        model.purchaseOrderDate = Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy");
                        model.purchaseOrderNumber = poSupplierNumber;
                        model.PRRefId = "";
                        model.PRRefNo = PRRefNo;
                        model.SupplierName = supplierName;
                        model.SupplierAddress = supplierAddress;
                        model.SupplierTelephone = supplierPhone;
                        model.SupplierEmail = supplierEmail;
                        model.SMFullName = siteManagerName;
                        model.SMTelephone = siteManagerTelephone;
                        model.SMEmail = siteManagerEmail;
                        model.ProjectFullName = siteAddress;
                        model.PreparedBy = preparedBy;
                        model.CheckedBy = checkedBy;
                        model.ApprovedBy = approvedBy;                      
                        model.SubTotal = String.Format("{0:N4}", Convert.ToDouble(totalAmount));
                        model.GrandTotal = String.Format("{0:N4}", Convert.ToDouble(grandTotal));
                        model.VAT = String.Format("{0:N4}", Convert.ToDouble(vatAmount));
                        model.AmountBeforeDiscount = String.Format("{0:N4}", Convert.ToDouble(totalAmountBeforeDiscount));
                        model.LumpSumDiscountAmount = String.Format("{0:N4}", Convert.ToDouble(totalAmountBeforeDiscount - totalAmount));
                        model.Status = status;
                        model.CreatedAt = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                        model.CFOApprovedAt = Convert.ToDateTime(obj.checked_date).ToString("dd-MMM-yyyy");
                        model.DirectorApprovedAt = obj.approved_date == null ? Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy") : Convert.ToDateTime(obj.approved_date).ToString("dd-MMM-yyyy");
                        model.incoterm = incoterm;
                        model.payment = payment;
                        model.delivery = delivery;
                        model.shipment = shipment;
                        model.warranty = warranty;
                        model.vendor_ref = vendor_ref; 
                        model.ir_noMR = ir_noMR;
                        model.projectshortname = projectshortname;
                        model.soNo = soNo;
                        model.is_local = is_local;
                        model.is_vat = isVAT;
                        model.vat_number = Convert.ToDecimal(vat).ToString("G2");

                        string host = HttpContext.Current.Request.Url.Authority;
                        //host = "http://vuthytep-001-site7.atempurl.com";

                        model.createdBySignature =string.IsNullOrEmpty(createdBySignature)?string.Empty: string.Format("{0}{1}",EnumConstants.domainName,createdBySignature);
                        model.approvedBySignature = string.IsNullOrEmpty(CommonClass.GetUserSignature(purchase.approved_by)) ? string.Empty : string.Format("{0}{1}", EnumConstants.domainName, CommonClass.GetUserSignature(purchase.approved_by));
                        model.checkedBySignature= string.IsNullOrEmpty(CommonClass.GetUserSignature(purchase.checked_by)) ? string.Empty : string.Format("{0}{1}", EnumConstants.domainName, CommonClass.GetUserSignature(purchase.checked_by));

                        if (string.IsNullOrEmpty(purchase.approved_signature))
                        {
                            model.approvedBySignature = string.Empty;
                        }
                        else
                        {
                            model.approvedBySignature = string.Format("{0}{1}", EnumConstants.domainName, CommonClass.getUserSignaturebyAttachmentId(purchase.approved_signature)); 
                        }

                        model.isPO = isPO;
                        model.isQuote = isQuote;
                        model.isMR=isMR;
                        model.isSignature=isSignature;

                        model.poItems = items;

                    }


                    #region Get MR Data
                    model.materialRequest = ItemRequestViewModel.GetItemRequestDetail(mr.ir_id);
                    #endregion
                    model.quote = PurchaseOrderViewModel.GetPurchaseOrderDetail(poId);
                    #region Get Quote Data

                    #endregion
                }



            }
            catch (Exception ex)
            {

            }
            return model;
        }
    }
    public class PurchaseOrderVSGRNReportResponseModel
    {
        public List<PurchaseOrderReportViewModel> poGRNs { get; set; }
        public List<DetailPOReceivingReportResponseModel> poDetails { get; set; }

        public PurchaseOrderVSGRNReportResponseModel()
        {
            poGRNs = new List<PurchaseOrderReportViewModel>();
            poDetails = new List<DetailPOReceivingReportResponseModel>();
        }
    }
    public class PurchaseOrderReportGenerateItemsModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemUnit { get; set; }
        public string POQquantity { get; set; }
        public string UnitPrice { get; set; }
        public string Discount { get; set; }
        public string brand { get; set; }
        public string podetailid { get; set; }
        public string itemvat { get; set; }
        public string Amount { get; set; }

    }
    public class QuoteSupplierItemModel
    {
        public tb_po_supplier pos { get; set; }
        public tb_purchase_order_detail pod { get; set; }
    }
    public class QuoteBySupplierAmountModel
    {
        public decimal VATAmount { get; set; }
        public decimal NONVATAmount { get; set; }
        public decimal TotalAmount { get { return VATAmount + NONVATAmount; } set { TotalAmount = value; } }
    }
    public class PurchaseOrderDetailItemEntityModel
    {
        public tb_purchase_order_detail purd { get; set; }
        public tb_po_supplier purs { get; set; }
        public tb_product prod { get; set; }
        public tb_unit unit { get; set; }
    }
    public class TransactionReferenceModel
    {
        public string project { get; set; }
        public string mr { get; set; }
        public string pr { get; set; }
        public string quote { get; set; }
        public string po { get; set; }

        public static TransactionReferenceModel GetTransactionReferenceByPOReportId(string id)
        {
            TransactionReferenceModel model = new TransactionReferenceModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                model = (from pod in db.tb_purchase_request_detail
                         join po in db.tb_purchase_request on pod.purchase_request_id equals po.pruchase_request_id
                         join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                         join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         where string.Compare(pod.status, Status.Approved) == 0 && string.Compare(pod.po_report_id, id) == 0
                         select new TransactionReferenceModel()
                         {
                             project=project.project_full_name,
                             mr=mr.ir_no,
                             pr =pr.purchase_requisition_number,
                             quote=quote.purchase_oder_number,
                             po=po.purchase_request_number,
                             
                         }).FirstOrDefault();

            }catch(Exception ex)
            {

            }
            return model;
        }
    }
    public class QuoteListResultModel
    {
        public tb_purchase_order quote { get; set; }
        public tb_purchase_requisition pr { get; set; }
        public tb_item_request mr { get; set; }
        public tb_project pro { get; set; }
    }
}