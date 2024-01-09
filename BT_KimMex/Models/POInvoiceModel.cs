using BT_KimMex.Class;
using BT_KimMex.Entities;
using Eco.Persistence.Connection;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BT_KimMex.Models
{
    public class POInvoiceModel
    {
        [Key]
        public string invoice_id { get; set; }
        public string po_report_id { get; set; }
        public Nullable<System.DateTime> invoice_date { get; set; }
        public string invoice_number_shipper { get; set; }
        public string invoice_number_forwarder { get; set; }
        public Nullable<decimal> invoice_amount { get; set; }
        public Nullable<decimal> invoice_vat_amount { get; set; }
        public Nullable<decimal> transporation_fee { get; set; }
        public Nullable<decimal> import_duty { get; set; }
        public Nullable<decimal> paid_amount { get; set; }
        public Nullable<decimal> balance { get; set; }
        public Nullable<bool> is_active { get; set; }
        public string remark { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string craeted_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string updated_by { get; set; }
        public Nullable<bool> is_save_addnew { get; set; }
        public tb_po_report po_Report { get; set; }
        public string invoice_date_text { get; set; }
        public string created_at_text { get; set; }
        public string created_by_text { get; set; }
        public List<POInvoicePaymentModel> payments { get; set; }
        public tb_po_invoice_document document { get; set; }
        public List<POInvoiceDocumentModel> documents { get; set; }
        public TransactionReferenceModel poReportTransaction { get; set; }
        public int countPayment
        {
            get { return payments.Count; }
            set { countPayment = value; }
        }
        public POInvoiceModel()
        {
            payments= new List<POInvoicePaymentModel>();
            document=new tb_po_invoice_document();
            documents = new List<POInvoiceDocumentModel>();
        }

        public static POInvoiceModel ConvertEntityToModel(tb_po_invoice entity)
        {
            return new POInvoiceModel()
            {
                invoice_id =    entity.invoice_id,
                po_report_id= entity.po_report_id,
                invoice_date= entity.invoice_date,
                invoice_number_shipper= entity.invoice_number_shipper,
                invoice_number_forwarder= entity.invoice_number_forwarder,
                invoice_amount= entity.invoice_amount,
                invoice_vat_amount= entity.invoice_vat_amount,
                transporation_fee= entity.transporation_fee,
                import_duty= entity.import_duty,
                paid_amount= entity.paid_amount,
                balance= entity.balance,
                is_active= entity.is_active,
                remark= entity.remark,
                created_at= entity.created_at,
                craeted_by = entity.craeted_by,
                updated_at= entity.updated_at,
                updated_by= entity.updated_by,
            };
        }

        public static List<POInvoiceModel> GetInvoiceListByDateRange(string dateRange)
        {
            List<POInvoiceModel> invoices= new List<POInvoiceModel>();
            string[] splitDateRanges = dateRange.Split('-');
            DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
            DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
            kim_mexEntities db= new kim_mexEntities();
            var results = (from inv in db.tb_po_invoice
                           join por in db.tb_po_report on inv.po_report_id equals por.po_report_id
                           orderby inv.created_at descending
                           where inv.is_active == true && inv.created_at >= startDate && inv.created_at <= endDate
                           select new { inv, por }).ToList();
            foreach(var rs in results)
            {
                POInvoiceModel model = ConvertEntityToModel(rs.inv);
                model.invoice_date_text = Convert.ToDateTime(model.invoice_date).ToString("dd-MMM-yyyy");
                model.created_at_text = Convert.ToDateTime(model.created_at).ToString("dd-MMM-yyyy");
                model.created_by_text = CommonFunctions.GetUserFullnamebyUserId(model.craeted_by);
                model.document=db.tb_po_invoice_document.OrderByDescending(s=>s.created_at).Where(s=>s.is_active==true && string.Compare(s.po_invoice_id,model.invoice_id)==0).FirstOrDefault();

                model.po_Report = rs.por;
                invoices.Add(model);
            }

            return invoices;
        }
        

        public static void UpdateInvoiceByPayment(tb_po_invoice_payment model)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_po_invoice invoice = db.tb_po_invoice.Find(model.invoice_id);
                if (invoice != null)
                {
                    invoice.paid_amount = invoice.paid_amount + model.paid_amount;
                    invoice.balance = invoice.invoice_amount - invoice.paid_amount;
                    db.SaveChanges();
                }
            }
        }

        public static List<POInvoiceModel> GetPOInvoiceByPOReportId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<POInvoiceModel> models= new List<POInvoiceModel>();
                var results = db.tb_po_invoice.OrderByDescending(s=>s.invoice_date).Where(s=>string.Compare(s.po_report_id,id)==0 && s.is_active==true ).ToList();
                foreach(var rs in results)
                {
                    POInvoiceModel obj = POInvoiceModel.ConvertEntityToModel(rs);
                    obj.payments = POInvoicePaymentModel.GetPOInvoicePaymentByInvoiceId(obj.invoice_id);
                    obj.document = db.tb_po_invoice_document.OrderByDescending(s => s.created_at).Where(s => string.Compare(s.po_invoice_id, obj.invoice_id) == 0
                    && s.is_active == true).FirstOrDefault();
                    models.Add(obj);
                }
                return models;
            }
        }

        public static POInvoiceModel GetInvoiceItem(string id)
        {
            POInvoiceModel model = new POInvoiceModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_invoice entity = db.tb_po_invoice.Find(id);
                if (entity != null)
                {
                    model = ConvertEntityToModel(entity);
                    model.invoice_date_text = Convert.ToDateTime(model.invoice_date).ToString("dd-MMM-yyyy");
                    model.created_at_text = Convert.ToDateTime(model.created_at).ToString("dd-MMM-yyyy");
                    model.created_by_text = CommonFunctions.GetUserFullnamebyUserId(model.craeted_by);
                    model.po_Report = db.tb_po_report.Find(model.po_report_id);
                    model.documents = GetInvoiceDocumentListByInvoiceID(model.invoice_id);
                    model.payments = POInvoicePaymentModel.GetPOInvoicePaymentByInvoiceId(model.invoice_id);
                    model.poReportTransaction = TransactionReferenceModel.GetTransactionReferenceByPOReportId(model.po_report_id);
                }
            }catch(Exception ex)
            {
                
            }
            return model;
        }

        public static List<POInvoiceDocumentModel> GetInvoiceDocumentListByInvoiceID(string invoiceId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<POInvoiceDocumentModel> models = new List<POInvoiceDocumentModel>();
                var results= db.tb_po_invoice_document.OrderByDescending(s => s.created_at).Where(s => s.is_active == true && string.Compare(s.po_invoice_id, invoiceId) == 0).ToList();
                foreach(var rs in results)
                {
                    POInvoiceDocumentModel model = POInvoiceDocumentModel.ConvertEntityToModel(rs);
                    model.fa_received_date_text = CommonClass.ToLocalTime(Convert.ToDateTime(model.fa_received_date)).ToString("MM/dd/yyyy");
                    model.created_at_text = CommonClass.ToLocalTime(Convert.ToDateTime(model.created_at)).ToString("MM/dd/yyyy");
                    model.created_by_text = CommonClass.GetUserFullnameByUserId(model.created_by);
                    models.Add(model);
                }
                return models;
            }
        }

    }

    public class POInvoicePaymentModel
    {
        [Key]
        public string payment_id { get; set; }
        public string invoice_id { get; set; }
        public Nullable<System.DateTime> payment_date { get; set; }
        public string payment_by { get; set; }
        public Nullable<decimal> paid_amount { get; set; }
        public Nullable<bool> is_active { get; set; }

        public static POInvoicePaymentModel ConvertEnityToModel(tb_po_invoice_payment entity)
        {
            if (entity == null) return null;
            return new POInvoicePaymentModel()
            {
                payment_id = entity.payment_id,
                invoice_id = entity.invoice_id,
                payment_date = entity.payment_date,
                payment_by = entity.payment_by,
                paid_amount= entity.paid_amount,
                is_active = entity.is_active,
            };
        }

        public static AJAXResultModel SaveInvoicePayment(tb_po_invoice_payment model)
        {
            AJAXResultModel response=new AJAXResultModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_invoice_payment payment=new tb_po_invoice_payment();
                payment.payment_id = Guid.NewGuid().ToString();
                payment.invoice_id = model.invoice_id;
                payment.payment_date = CommonClass.ToLocalTime(Convert.ToDateTime(model.payment_date));
                payment.payment_by= model.payment_by;
                payment.paid_amount= model.paid_amount;
                payment.is_active = true;
                db.tb_po_invoice_payment.Add(payment);
                db.SaveChanges();

                POInvoiceModel.UpdateInvoiceByPayment(model);

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.message= ex.Message;
            }
            return response;
        }
        public static List<POInvoicePaymentModel> GetPOInvoicePaymentByInvoiceId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<POInvoicePaymentModel> models = new List<POInvoicePaymentModel>();
                var results = db.tb_po_invoice_payment.OrderBy(s => s.payment_date).Where(s => s.is_active == true && string.Compare(s.invoice_id, id) == 0).ToList();
                foreach( var result in results)
                {
                    models.Add(POInvoicePaymentModel.ConvertEnityToModel(result));
                }
                return models;
            }
        }
    }

    public class POInvoiceDocumentModel
    {
        [Key]
        public string document_id { get; set; }
        public string po_invoice_id { get; set; }
        public Nullable<System.DateTime> fa_received_date { get; set; }
        public string fa_received_date_text { get; set; }
        public string submit_via { get; set; }
        public string note { get; set; }
        public Nullable<bool> is_active { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string created_by { get; set; }
        public string created_by_text { get; set; }
        public string created_at_text { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public string updated_by { get; set; }

        public static POInvoiceDocumentModel ConvertEntityToModel(tb_po_invoice_document entity)
        {
            return new POInvoiceDocumentModel()
            {
                document_id = entity.document_id,
                po_invoice_id = entity.po_invoice_id,
                fa_received_date = entity.fa_received_date,
                submit_via = entity.submit_via,
                note = entity.note,
                is_active = entity.is_active,
                created_at = entity.created_at,
                created_by = entity.created_by,
                updated_at = entity.updated_at,
                updated_by = entity.updated_by,
            };
        }
    }
}