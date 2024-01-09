using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using System.Activities.Expressions;
using BT_KimMex.Class;

namespace BT_KimMex.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetInvoiceListDataTable(string dateRange)
        {
            return Json(new {data=POInvoiceModel.GetInvoiceListByDateRange(dateRange)},JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveInvoicePayment(tb_po_invoice_payment model)
        {
            model.payment_by = User.Identity.GetUserId();
            return Json(new { data = POInvoicePaymentModel.SaveInvoicePayment(model) }, JsonRequestBehavior.AllowGet);
        }
        // GET: Invoice/Details/5
        public ActionResult Detail(string id)
        {
            return View(POInvoiceModel.GetInvoiceItem(id));
        }

        // GET: Invoice/Create
        public ActionResult Create(string id="")
        {
            POInvoiceModel model = new POInvoiceModel();
            model =POInvoiceModel.GetInvoiceItem(id);
            return View(model);
        }

        // POST: Invoice/Create
        [HttpPost]
        public ActionResult Create(POInvoiceModel model)
        {
            try
            {
                if(!ModelState.IsValid)
                    return View(model);
                kim_mexEntities db=new kim_mexEntities();

                if (string.IsNullOrEmpty(model.invoice_id))
                {
                    tb_po_invoice invoice = new tb_po_invoice();
                    invoice.invoice_id = Guid.NewGuid().ToString();
                    invoice.po_report_id = model.po_report_id;
                    invoice.invoice_date = model.invoice_date;
                    invoice.invoice_amount = model.invoice_amount;
                    invoice.invoice_vat_amount = model.invoice_vat_amount;
                    invoice.invoice_number_shipper = model.invoice_number_shipper;
                    invoice.invoice_number_forwarder = model.invoice_number_forwarder;
                    invoice.transporation_fee = model.transporation_fee;
                    invoice.import_duty = model.import_duty;
                    invoice.is_active = true;
                    invoice.created_at = CommonClass.ToLocalTime(DateTime.Now);
                    invoice.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                    invoice.craeted_by = User.Identity.GetUserId();
                    invoice.updated_by = User.Identity.GetUserId();
                    invoice.paid_amount = model.paid_amount;
                    invoice.balance = model.invoice_amount - invoice.paid_amount;
                    db.tb_po_invoice.Add(invoice);
                    db.SaveChanges();

                    //tb_po_invoice_payment payment = new tb_po_invoice_payment();
                    //payment.payment_id = Guid.NewGuid().ToString();
                    //payment.invoice_id=invoice.invoice_id;
                    //payment.paid_amount = model.paid_amount;
                    //payment.payment_date=CommonClass.ToLocalTime(DateTime.Now);
                    //payment.is_active=true;
                    //db.tb_po_invoice_payment.Add(payment);
                    //db.SaveChanges();

                    /* Update PO Outstanding Amount */
                    tb_purchase_request_detail pod = db.tb_purchase_request_detail.Where(s => (string.Compare(s.status, Status.Approved) == 0 || string.Compare(s.status, Status.Completed) == 0 || string.Compare(s.status, Status.MREditted) == 0) && string.Compare(s.po_report_id, model.po_report_id) == 0).FirstOrDefault();
                    if (pod != null)
                    {
                        pod.outstanding_amount = pod.outstanding_amount - invoice.invoice_amount;
                        db.SaveChanges();
                        if (pod.outstanding_amount == 0)
                        {
                            tb_po_report poReport = db.tb_po_report.Find(invoice.po_report_id);
                            if (poReport != null)
                            {
                                poReport.is_invoice = true;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    //Edit Invoice 
                    //Check validation
                    tb_po_invoice invoice = db.tb_po_invoice.Find(model.invoice_id);
                    tb_purchase_request_detail pod = db.tb_purchase_request_detail.Where(s => (string.Compare(s.status, Status.Approved) == 0 || string.Compare(s.status,Status.Completed)==0 || string.Compare(s.status,Status.MREditted)==0 )&& string.Compare(s.po_report_id, invoice.po_report_id) == 0).FirstOrDefault();
                    if (invoice != null && pod!=null)
                    {
                        decimal totalOutstanding = Convert.ToDecimal(invoice.invoice_amount) + Convert.ToDecimal(pod.outstanding_amount);
                        if (model.invoice_amount > totalOutstanding)
                        {
                            ViewBag.Message = "Invalid Amount";
                            return View(model);
                        }
                        else
                        {
                            invoice.invoice_date = model.invoice_date;
                            invoice.invoice_amount = model.invoice_amount;
                            invoice.invoice_vat_amount = model.invoice_vat_amount;
                            invoice.invoice_number_shipper = model.invoice_number_shipper;
                            invoice.invoice_number_forwarder = model.invoice_number_forwarder;
                            invoice.transporation_fee = model.transporation_fee;
                            invoice.import_duty = model.import_duty;
                            invoice.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                            invoice.updated_by = User.Identity.GetUserId();
                            invoice.paid_amount = model.paid_amount;
                            invoice.balance = model.invoice_amount - invoice.paid_amount;
                            db.SaveChanges();

                            pod.outstanding_amount = totalOutstanding - invoice.invoice_amount;
                            db.SaveChanges();
                            tb_po_report poReport = db.tb_po_report.Find(invoice.po_report_id);
                            if (pod.outstanding_amount == 0)
                            {
                                if (poReport != null)
                                {
                                    poReport.is_invoice = true;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                
                                if (poReport != null)
                                {
                                    poReport.is_invoice = false;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                

                if(Convert.ToBoolean(model.is_save_addnew))
                {
                    
                    return RedirectToAction("Create", new {id=""});
                }
                else
                    return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult VoidInvoice(string invoiceId,string reason)
        {
            bool isSuccess = true;
            string message = string.Empty;

            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_invoice invoice = db.tb_po_invoice.Find(invoiceId);
                if (invoice != null)
                {

                    invoice.is_active = false;
                    invoice.remark = reason;
                    invoice.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                    invoice.updated_by = User.Identity.GetUserId();
                    db.SaveChanges();

                    /* Update PO Outstanding Amount */
                    tb_purchase_request_detail pod = db.tb_purchase_request_detail.Where(s => (string.Compare(s.status, Status.Approved) == 0 || string.Compare(s.status, Status.Completed) == 0 || string.Compare(s.status, Status.MREditted) == 0) && string.Compare(s.po_report_id, invoice.po_report_id) == 0).FirstOrDefault();
                    if (pod != null)
                    {
                        pod.outstanding_amount = pod.outstanding_amount + invoice.invoice_amount;
                        db.SaveChanges();

                        tb_po_report poReport = db.tb_po_report.Find(invoice.po_report_id);
                        if (poReport != null)
                        {
                            poReport.is_invoice = false;
                            db.SaveChanges();
                        }
                    }

                }

            }catch(Exception ex)
            {

            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetPOAmountByPOReportIDAJAX(string id)
        {
            return Json(new { 
                data = PurchaseOrderReportViewModel.GetPOReportAmountByPOReportId(id),
                reference = TransactionReferenceModel.GetTransactionReferenceByPOReportId(id),
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInvoiceDetailAJAX(string id)
        {
            AJAXResultModel result = new AJAXResultModel();
            POInvoiceModel data = new POInvoiceModel();
            tb_purchase_request_detail poAmount=new tb_purchase_request_detail();
            TransactionReferenceModel reference = new TransactionReferenceModel();
            try
            {
                data = POInvoiceModel.GetInvoiceItem(id);
                data.invoice_date_text = CommonClass.ToLocalTime(Convert.ToDateTime(data.invoice_date)).ToString("MM/dd/yyyy");
                poAmount = PurchaseOrderReportViewModel.GetPOReportAmountByPOReportId(data.po_report_id);
                reference = TransactionReferenceModel.GetTransactionReferenceByPOReportId(data.po_report_id);
            }
            catch(Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.InnerException.Message;
            }
            return Json(new { result = result, data = data,amount=poAmount, reference= reference }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SubmitInvoiceDocument(tb_po_invoice_document model)
        {
            AJAXResultModel result = new AJAXResultModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_invoice_document doc = new tb_po_invoice_document();
                doc.document_id = Guid.NewGuid().ToString();
                doc.po_invoice_id = model.po_invoice_id;
                doc.fa_received_date = model.fa_received_date;
                doc.submit_via = model.submit_via;
                doc.note = model.note;
                doc.is_active = true;
                doc.created_at = CommonClass.ToLocalTime(DateTime.Now);
                doc.created_by = User.Identity.GetUserId();
                doc.updated_by = User.Identity.GetUserId();
                doc.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                db.tb_po_invoice_document.Add(doc);
                db.SaveChanges();

            }catch(Exception ex)
            {
                result.message = ex.InnerException.Message;
                result.isSuccess = false;
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInvoiceDocumentListByInvoiceId(string id)
        {
            AJAXResultModel result = new AJAXResultModel();
            List<POInvoiceDocumentModel> data = new List<POInvoiceDocumentModel>();
            try
            {
                data = POInvoiceModel.GetInvoiceDocumentListByInvoiceID(id);
            }catch(Exception ex)
            {
                result.isSuccess=false;
                result.message=ex.InnerException.Message;
            }
            return Json(new {result=result,data=data}, JsonRequestBehavior.AllowGet);
        }
    }
}
