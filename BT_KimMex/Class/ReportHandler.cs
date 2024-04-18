  using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using System.Web.UI.WebControls;
using BT_KimMex.Models;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.Web.Mvc;

namespace BT_KimMex.Class
{
    public class ReportHandler
    {
        private static string ir_id;
        //private static DateTime dataTo;
        public static void GeneratePOSupplier(ReportViewer rv, string reportPath, string poId, string supplierId)
        {

            try
            {
                using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    BT_KimMex.Models.PurchaseOrderViewModel obj = new Models.PurchaseOrderViewModel();
                    obj = BT_KimMex.Class.CommonClass.GetPOSupplierItem(poId);
                    DataTable tb = new DataTable();
                    if (obj != null)
                    {
                        DataRow dtr;
                        DataColumn col = new DataColumn();
                        col.ColumnName = "purchaseOrderId";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "purchaseOrderDate";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "purchaseOrderNumber";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemRequestNumber";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMFullName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMTelephone";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMEmail";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierAddress";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierTelephone";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierEmail";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierVATIN";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemCode";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemUnit";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "POQquantity";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "UnitPrice";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Discount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ProjectFullName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PreparedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CheckedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ApprovedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "AmountInWord";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "VAT";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Status";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "podetailid";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "itemvat";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PRRefId";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PRRefNo";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CreatedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CFOApprovedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "DirectorApprovedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Amount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SubTotal";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "GrandTotal";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "incoterm";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "payment";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "delivery";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "shipment";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "warranty";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "vendor_ref";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "brand";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ir_noMR";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "projectshortname";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "soNo";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "is_local";
                        tb.Columns.Add(col);

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
                        string is_local = string.Empty;
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
                        double totalAmount = 0, totalAmountBeforeDiscount=0, lumpSumDiscountAmount=0;
                        decimal vat = 0;
                        string status = string.Empty;
                        string preparedBySignature = string.Empty;
                        string approvedBySignature = string.Empty;
                        string checkedBySignature = string.Empty;

                        poSupplierNumber = db.tb_po_report.Where(x => x.po_supplier_id == supplierId && x.po_ref_id == obj.purchase_order_id && x.vat_status == true).Select(x => x.po_report_number).FirstOrDefault();
                        supplierName = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_name).FirstOrDefault();


                        //incoterm = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.incoterm).FirstOrDefault();
                        //payment = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.payment).FirstOrDefault();
                        //delivery = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.delivery).FirstOrDefault();
                        //shipment = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.shipment).FirstOrDefault();
                        //warranty = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.warranty).FirstOrDefault();
                        //vendor_ref = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.vendor_ref).FirstOrDefault();
                        is_local = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.is_local.ToString()).FirstOrDefault();

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

                        ir_noMR = (from po in db.tb_purchase_order
                                   join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                   join ir in db.tb_item_request
                                   on pr.material_request_id
                                   equals ir.ir_id
                                   where po.purchase_order_id == poId
                                   select ir.ir_no
                                   ).FirstOrDefault();
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

                        // var purchase = db.tb_purchase_order.Where(x => x.purchase_order_id == poId).FirstOrDefault();
                        var purchase = db.tb_purchase_request.Where(x => x.purchase_order_id == poId).FirstOrDefault();

                        // preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);                      
                        // approvedBy = CommonClass.GetUserFullname(purchase.approved_by);

                        preparedBy = db.AspNetUsers.Where(x => x.Id == purchase.created_by).Select(x => x.UserName).FirstOrDefault();
                        // checkedBy = db.AspNetUsers.Where(x => x.Id == purchase.checked_by).Select(x => x.UserName).FirstOrDefault();
                        preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);
                        checkedBy = CommonClass.GetUserFullname(purchase.checked_by);
                        approvedBy = CommonClass.GetUserFullname(purchase.approved_by);
                        //approvedBy = db.AspNetUsers.Where(x => x.Id == purchase.approved_by).Select(x => x.UserName).FirstOrDefault();

                        status = string.Compare(obj.purchase_order_status, "Completed") == 0 ? "Approved by Director" : string.Empty;

                        preparedBySignature = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserSignature(purchase.created_by) : CommonClass.GetUserSignature(purchase.updated_by);
                        //checkedBySignature = CommonClass.GetUserSignature(purchase.approved_by);
                        approvedBySignature = CommonClass.GetUserSignature(purchase.checked_by);
                        if (string.IsNullOrEmpty(purchase.approved_signature))
                        {
                            checkedBySignature = CommonClass.GetUserSignature(purchase.approved_by);
                            
                        }
                        else
                        {
                            checkedBySignature = CommonClass.getUserSignaturebyAttachmentId(purchase.approved_signature);
                        }

                        //Get Quote Supplier Term and Condition
                        tb_quote sup_quote = db.tb_quote
                            .OrderByDescending(s => s.created_date)
                            .Where(s => s.status == true && string.Compare(s.supplier_id, supplierId) == 0 && string.Compare(s.mr_id, IsHasShippingTo.item_request_id) == 0 && string.Compare(s.quote_status, Status.cancelled) != 0).FirstOrDefault();
                        
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
                                decimal unitPrice = 0;
                                if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                                    unitPrice = Convert.ToDecimal(item.unit_price);
                                else if (string.Compare(obj.purchase_order_status, "Approved") == 0)
                                {
                                    unitPrice = Convert.ToDecimal(db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).Select(x => x.unit_price).FirstOrDefault());
                                }
                                foreach (var ssup in sup)
                                {
                                    if (ssup.supplier_id == supplierId)
                                    {
                                        amount = amount + Convert.ToDecimal(item.po_quantity * unitPrice);
                                    }
                                }
                            }
                            /*
                            decimal vatAmount = 0;
                            vatAmount = vat == 0 ? 0 : amount * (vat / 100);
                            totalAmount = amount - vatAmount;
                            */

                            lumpSumDiscountAmount = Convert.ToDouble(obj.poDetails.GroupBy(s => s.lump_sum_discount_amount).Sum(s => s.Key));

                            foreach (var item in obj.poDetails)
                            {

                                brand = db.tb_brand.Where(x => x.brand_id == item.brand_id).Select(x => x.brand_name).FirstOrDefault();

                                var sup = item.poSuppliers;
                                decimal unitPrice = 0, original_price = 0, discount_percentage = 0 ,discount_amount=0;
                                if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                                {
                                    unitPrice = Convert.ToDecimal(item.unit_price);
                                    original_price =item.original_price.HasValue? Convert.ToDecimal(item.original_price):Convert.ToDecimal(item.unit_price);
                                    discount_percentage =item.discount_percentage.HasValue? Convert.ToDecimal(item.discount_percentage):0;
                                }
                                    
                                else if (string.Compare(obj.purchase_order_status, "Approved") == 0 || string.Compare(obj.purchase_order_status, Status.MREditted) == 0)
                                {
                                    var posup=db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).FirstOrDefault();
                                    unitPrice = Convert.ToDecimal(posup.unit_price);
                                    original_price =posup.original_price.HasValue? Convert.ToDecimal(posup.original_price):Convert.ToDecimal(posup.unit_price);
                                    discount_percentage =posup.discount_percentage.HasValue? Convert.ToDecimal(posup.discount_percentage):0;
                                }
                                discount_amount = original_price * (discount_percentage / 100);
                                foreach (var ssup in sup)
                                {
                                    if (ssup.supplier_id == supplierId)
                                    {
                                        var show = Convert.ToBoolean(item.item_vat);
                                        if (show == true)
                                        {
                                            //double vat1 = 0.1;
                                            double vat1 = vat > 0 ? (double)vat / 100 :0;

                                            double Amount = Convert.ToDouble(item.po_quantity * unitPrice);
                                            totalAmount = totalAmount + Amount;
                                            totalAmountBeforeDiscount = totalAmountBeforeDiscount + Convert.ToDouble(item.po_quantity *(original_price- discount_amount));
                                            //lumpSumDiscountAmount = totalAmountBeforeDiscount - totalAmount;

                                            double vatAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(totalAmount-lumpSumDiscountAmount) * vat1), 2);
                                            double grandTotal = Convert.ToDouble(totalAmount) + vatAmount;

                                            //totalAmount = totalAmount +Convert.ToDecimal(vatAmount);
                                            dtr = tb.NewRow();
                                            dtr["purchaseOrderId"] = obj.purchase_order_id;
                                            dtr["purchaseOrderDate"] = Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy");
                                            dtr["purchaseOrderNumber"] = poSupplierNumber;
                                            dtr["PRRefId"] = "";
                                            dtr["PRRefNo"] = PRRefNo;
                                            dtr["SupplierName"] = supplierName;
                                            dtr["SupplierAddress"] = supplierAddress;
                                            dtr["SupplierTelephone"] = supplierPhone;
                                            dtr["SupplierEmail"] = supplierEmail;
                                            dtr["ItemCode"] = item.product_code;
                                            dtr["ItemName"] = string.IsNullOrEmpty(item.supplier_item_name)?item.product_name:item.supplier_item_name;
                                            dtr["ItemUnit"] = db.tb_unit.Find(item.po_unit).Name;                                          
                                            dtr["podetailid"] = item.po_detail_id;
                                            dtr["POQquantity"] = Convert.ToDouble(item.po_quantity);
                                            dtr["UnitPrice"] =string.Format("{0},{1},", original_price,string.Format("{0:N3}", discount_percentage));
                                            dtr["SMFullName"] = siteManagerName;
                                            dtr["SMTelephone"] = siteManagerTelephone;
                                            dtr["SMEmail"] = siteManagerEmail;
                                            dtr["ProjectFullName"] = siteAddress;
                                            dtr["PreparedBy"] = preparedBy;
                                            dtr["CheckedBy"] = checkedBy;
                                            dtr["ApprovedBy"] = approvedBy;
                                            //dtr["AmountInWord"] = Class.ConvertCurrencyToWord.CurrencyToWord(totalAmount.ToString());
                                            dtr["AmountInWord"] = Class.ConvertCurrencyToWord.CurrencyToWord(grandTotal.ToString()); //Class.Convertion.NumberToWords(grandTotal);
                                            dtr["Amount"] = String.Format("{0:N3}", Convert.ToDouble(item.po_quantity * (original_price- discount_amount)));
                                            dtr["SubTotal"] = String.Format("{0}/{1}/{2}", String.Format("{0:N3}", Convert.ToDouble(totalAmount-lumpSumDiscountAmount)), String.Format("{0:N3}", Convert.ToDouble(totalAmountBeforeDiscount)), String.Format("{0:N3}", Convert.ToDouble(lumpSumDiscountAmount)));
                                            //String.Format("{0:N3}", Convert.ToDouble(totalAmount));
                                            dtr["GrandTotal"] = String.Format("{0:N3}", Convert.ToDouble(grandTotal-lumpSumDiscountAmount));
                                            dtr["VAT"] = String.Format("{0:N3}", Convert.ToDouble(vatAmount));
                                            dtr["Status"] = status;
                                            dtr["CreatedAt"] = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                                            dtr["CFOApprovedAt"] = Convert.ToDateTime(obj.checked_date).ToString("dd-MMM-yyyy");
                                            dtr["DirectorApprovedAt"] = obj.approved_date == null ? Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy") : Convert.ToDateTime(obj.approved_date).ToString("dd-MMM-yyyy");
                                            dtr["incoterm"] = incoterm;
                                            dtr["payment"] = payment;
                                            dtr["delivery"] = delivery;
                                            dtr["shipment"] = shipment;
                                            dtr["warranty"] = warranty;
                                            dtr["vendor_ref"] = vendor_ref;
                                            dtr["brand"] = brand;
                                            dtr["ir_noMR"] = ir_noMR;
                                            dtr["projectshortname"] = projectshortname;
                                            dtr["soNo"] = soNo;
                                            dtr["is_local"] = is_local;
                                            //use as VAT amount
                                            //dtr["itemvat"] = Convert.ToBoolean(item.item_vat);
                                            dtr["itemvat"] =Convert.ToDecimal(vat).ToString("G2");

                                            tb.Rows.Add(dtr);

                                        }
                                    }

                                }

                            }
                        }

                        string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

                        rv.Reset();
                        rv.LocalReport.EnableExternalImages = true;
                        string filePath =new Uri(string.Format("{0}{1}",path,preparedBySignature)).AbsoluteUri;
                        ReportParameter[] param = new ReportParameter[3];
                        param[0] = new ReportParameter("ImgPath", filePath);
                        param[1]= new ReportParameter("ImgPath1", new Uri(string.Format("{0}{1}", path, approvedBySignature)).AbsoluteUri);
                        param[2] = new ReportParameter("ImgPath2", new Uri(string.Format("{0}{1}", path, checkedBySignature)).AbsoluteUri);
                        rv.ProcessingMode = ProcessingMode.Local;

                        LocalReport rep = rv.LocalReport;
                        //rv.LocalReport.ReportPath = reportPath;
                        rep.ReportPath = reportPath;
                        rep.SetParameters(param);

                        rv.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("purchaseOrderToSupplier", tb);
                        rv.LocalReport.DataSources.Add(rdc);

                        


                        rv.LocalReport.Refresh();
                    }
                }
            }
            catch (Exception ex) { }
        }
        public static void GeneratePOSupplierNotVAT(ReportViewer rv, string reportPath, string poId, string supplierId)
        {
            try
            {
                using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    BT_KimMex.Models.PurchaseOrderViewModel obj = new Models.PurchaseOrderViewModel();
                    obj = BT_KimMex.Class.CommonClass.GetPOSupplierItem(poId);
                    DataTable tb = new DataTable();
                    if (obj != null)
                    {
                        DataRow dtr;
                        DataColumn col = new DataColumn();
                        col.ColumnName = "purchaseOrderId";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "purchaseOrderDate";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "purchaseOrderNumber";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemRequestNumber";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMFullName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMTelephone";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SMEmail";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierAddress";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierTelephone";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierEmail";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SupplierVATIN";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemCode";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemUnit";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "POQquantity";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "UnitPrice";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Discount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ProjectFullName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PreparedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CheckedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ApprovedBy";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "AmountInWord";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "VAT";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Status";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "podetailid";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "itemvat";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PRRefId";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "PRRefNo";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CreatedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "CFOApprovedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "DirectorApprovedAt";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "Amount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "SubTotal";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "GrandTotal";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "incoterm";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "payment";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "delivery";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "shipment";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "warranty";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "vendor_ref";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "brand";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ir_noMR";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "projectshortname";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "soNo";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "is_local";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "createdBySignature";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "cratedBySignatureType";
                        tb.Columns.Add(col);

                        string PRRefNo = string.Empty;
                        string poSupplierNumber = string.Empty;
                        string supplierName = string.Empty;
                        string supplierAddress = string.Empty;
                        string supplierPhone = string.Empty;
                        string supplierEmail = string.Empty;
                        string siteManagerId = string.Empty;
                        string siteManagerName = string.Empty;
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
                        string is_local = string.Empty;
                        string preparedBySignature = string.Empty;
                        string approvedBySignature = string.Empty;
                        string checkedBySignature = string.Empty;


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
                        poSupplierNumber = db.tb_po_report.Where(x => x.po_supplier_id == supplierId && x.po_ref_id == obj.purchase_order_id && x.vat_status == false).Select(x => x.po_report_number).FirstOrDefault();
                        supplierName = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_name).FirstOrDefault();

                        //incoterm = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.incoterm).FirstOrDefault();
                        //payment = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.payment).FirstOrDefault();
                        //delivery = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.delivery).FirstOrDefault();
                        //shipment = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.shipment).FirstOrDefault();
                        //warranty = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.warranty).FirstOrDefault();
                        //vendor_ref = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.vendor_ref).FirstOrDefault();

                        is_local = db.tb_supplier.Where(x => x.supplier_id == supplierId).Select(x => x.is_local.ToString()).FirstOrDefault();

                        supplierAddress = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_address).FirstOrDefault();
                        supplierPhone = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_phone).FirstOrDefault();
                        supplierEmail = obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.supplier_email).FirstOrDefault();
                        vat = Convert.ToDecimal(obj.poSuppliers.Where(x => x.supplier_id == supplierId).Select(x => x.discount).FirstOrDefault());

                        //Rathana Edit Here
                        var IsHasShippingTo = db.tb_purchase_order.FirstOrDefault(x => x.purchase_order_id == poId);
                        if (string.IsNullOrEmpty(IsHasShippingTo.shipping_to))
                        {
                            siteManagerId = (from po in db.tb_purchase_order
                                             join ir in db.tb_item_request
                                             on po.item_request_id
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

                        siteAddress = (from po in db.tb_purchase_order
                                       join ir in db.tb_item_request
                                       on po.item_request_id
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
                                   join ir in db.tb_item_request
                                   on po.item_request_id
                                   equals ir.ir_id
                                   where po.purchase_order_id == poId
                                   select ir.ir_no
                                   ).FirstOrDefault();


                        //var purchase = db.tb_purchase_order.Where(x => x.purchase_order_id == poId).FirstOrDefault();
                        //preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);
                        //checkedBy = CommonClass.GetUserFullname(purchase.checked_by);
                        //approvedBy = CommonClass.GetUserFullname(purchase.approved_by);

                        ir_noMR = (from po in db.tb_purchase_order
                                   join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                   join ir in db.tb_item_request
                                   on pr.material_request_id
                                   equals ir.ir_id
                                   where po.purchase_order_id == poId
                                   select ir.ir_no
                                   ).FirstOrDefault();
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

                        // var purchase = db.tb_purchase_order.Where(x => x.purchase_order_id == poId).FirstOrDefault();
                        var purchase = db.tb_purchase_request.OrderBy(s=>s.approved_date).Where(x => x.purchase_order_id == poId && string.Compare(x.purchase_request_status,Status.RequestCancelled)!=0).FirstOrDefault();

                        // preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);                     
                        // approvedBy = CommonClass.GetUserFullname(purchase.approved_by);
                        //preparedBy = db.AspNetUsers.Where(x => x.Id == purchase.created_by).Select(x => x.UserName).FirstOrDefault();
                        // checkedBy = db.AspNetUsers.Where(x => x.Id == purchase.checked_by).Select(x => x.UserName).FirstOrDefault();
                        preparedBy = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserFullname(purchase.created_by) : CommonClass.GetUserFullname(purchase.updated_by);
                        checkedBy = CommonClass.GetUserFullname(purchase.checked_by);
                        approvedBy = CommonClass.GetUserFullname(purchase.approved_by);
                        //approvedBy = db.AspNetUsers.Where(x => x.Id == purchase.approved_by).Select(x => x.UserName).FirstOrDefault();

                        preparedBySignature = string.IsNullOrEmpty(purchase.updated_by) ? CommonClass.GetUserSignature(purchase.created_by) : CommonClass.GetUserSignature(purchase.updated_by);
                        
                        //checkedBySignature = CommonClass.GetUserSignature(purchase.approved_by);
                        approvedBySignature = CommonClass.GetUserSignature(purchase.checked_by);

                        if (string.IsNullOrEmpty(purchase.approved_signature))
                        {
                            checkedBySignature = CommonClass.GetUserSignature(purchase.approved_by);
                        }
                        else
                        {
                            checkedBySignature = CommonClass.getUserSignaturebyAttachmentId(purchase.approved_signature);
                        }

                        //Get Quote Supplier Term and Condition
                        tb_quote sup_quote = db.tb_quote
                            .OrderByDescending(s=>s.created_date)
                            .Where(s => s.status == true && string.Compare(s.supplier_id, supplierId) == 0 && string.Compare(s.mr_id, IsHasShippingTo.item_request_id) == 0 && string.Compare(s.quote_status,Status.cancelled)!=0).FirstOrDefault();
                        if (sup_quote != null)
                        {
                            incoterm = sup_quote.incoterm;
                            payment = sup_quote.payment;
                            delivery = sup_quote.delivery;
                            shipment = sup_quote.shipment;
                            warranty = sup_quote.warranty;
                            vendor_ref = sup_quote.vendor_ref;
                        }


                        status = string.Compare(obj.purchase_order_status, "Completed") == 0 ? "Approved by Director" : string.Empty;
                        lumpSumDiscountAmount =Convert.ToDouble(obj.poDetails.GroupBy(s => s.lump_sum_discount_amount).Sum(s => s.Key));

                        var itemSuppliers = obj.poSuppliers.Where(x => x.supplier_id == supplierId).ToList();
                        if (itemSuppliers.Any())
                        {
                            foreach (var item in obj.poDetails)
                            {


                                var sup = item.poSuppliers;
                                decimal unitPrice = 0;
                                if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                                    unitPrice = Convert.ToDecimal(item.unit_price);
                                else if (string.Compare(obj.purchase_order_status, "Approved") == 0)
                                {
                                    unitPrice = Convert.ToDecimal(db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).Select(x => x.unit_price).FirstOrDefault());
                                }
                                foreach (var ssup in sup)
                                {
                                    if (ssup.supplier_id == supplierId)
                                    {
                                        amount = amount + Convert.ToDecimal(item.po_quantity * unitPrice);
                                    }
                                }
                            }
                            /*
                            decimal vatAmount = 0;
                            vatAmount = vat == 0 ? 0 : amount * (vat / 100);
                            totalAmount = amount - vatAmount;
                            */
                            foreach (var item in obj.poDetails)
                            {
                                brand = db.tb_brand.Where(x => x.brand_id == item.brand_id).Select(x => x.brand_name).FirstOrDefault();

                                var sup = item.poSuppliers;
                                decimal unitPrice = 0,original_price=0,discount_percentage=0, discount_amount=0;
                                if (string.Compare(obj.purchase_order_status, "Completed") == 0)
                                {
                                    unitPrice = Convert.ToDecimal(item.unit_price);
                                    original_price =item.original_price.HasValue? Convert.ToDecimal(item.original_price):Convert.ToDecimal(item.unit_price);
                                    discount_percentage = item.discount_percentage.HasValue ? Convert.ToDecimal(item.discount_percentage) : 0 ;
                                }
                                    
                                else if (string.Compare(obj.purchase_order_status, "Approved") == 0||string.Compare(obj.purchase_order_status,Status.MREditted)==0)
                                {
                                    var posup = db.tb_po_supplier.Where(x => x.po_detail_id == item.po_detail_id && x.is_selected == true).FirstOrDefault();
                                    unitPrice = Convert.ToDecimal(posup.unit_price);
                                    original_price = posup.original_price.HasValue? Convert.ToDecimal(posup.original_price): Convert.ToDecimal(posup.unit_price);
                                    discount_percentage = posup.discount_percentage.HasValue ? Convert.ToDecimal(posup.discount_percentage) : 0 ;

                                }

                                discount_amount = original_price * (discount_percentage / 100);

                                foreach (var ssup in sup)
                                {
                                    if (ssup.supplier_id == supplierId)
                                    {
                                        var show = Convert.ToBoolean(item.item_vat);
                                        if (show == false)
                                        {
                                            totalAmount = totalAmount + Convert.ToDouble(item.po_quantity * unitPrice);
                                            totalAmountBeforeDiscount = totalAmountBeforeDiscount + Convert.ToDouble(item.po_quantity * (original_price-discount_amount));
                                            //lumpSumDiscountAmount = totalAmountBeforeDiscount - totalAmount;
                                            

                                            dtr = tb.NewRow();
                                            dtr["purchaseOrderId"] = obj.purchase_order_id;
                                            dtr["purchaseOrderDate"] = Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy");
                                            dtr["purchaseOrderNumber"] = poSupplierNumber;
                                            dtr["PRRefId"] = "";
                                            dtr["PRRefNo"] = PRRefNo;
                                            dtr["SupplierName"] = supplierName;
                                            dtr["SupplierAddress"] = supplierAddress;
                                            dtr["SupplierTelephone"] = supplierPhone;
                                            dtr["SupplierEmail"] = supplierEmail;
                                            dtr["ItemCode"] = item.product_code;
                                            dtr["ItemName"] = string.IsNullOrEmpty(item.supplier_item_name) ? item.product_name : item.supplier_item_name;
                                            dtr["ItemUnit"] = db.tb_unit.Find(item.po_unit).Name;
                                            dtr["itemvat"] = Convert.ToBoolean(item.item_vat);
                                            dtr["podetailid"] = item.po_detail_id;
                                            dtr["POQquantity"] = Convert.ToDouble(item.po_quantity);
                                            //dtr["UnitPrice"] = unitPrice;
                                            dtr["UnitPrice"] = string.Format("{0},{1}", original_price, string.Format("{0:N3}", discount_percentage));
                                            dtr["SMFullName"] = siteManagerName;
                                            dtr["SMTelephone"] = siteManagerTelephone;
                                            dtr["SMEmail"] = siteManagerEmail;
                                            dtr["ProjectFullName"] = siteAddress;
                                            dtr["PreparedBy"] = preparedBy;
                                            dtr["CheckedBy"] = checkedBy;
                                            dtr["ApprovedBy"] = approvedBy;
                                            dtr["AmountInWord"] = Class.ConvertCurrencyToWord.CurrencyToWord(totalAmount.ToString());
                                            //dtr["AmountInWord"] = Class.Convertion.NumberToWords(Convert.ToDouble(totalAmount));
                                            dtr["Amount"] = String.Format("{0:N3}", Convert.ToDouble(item.po_quantity * (original_price-discount_amount)));
                                            dtr["SubTotal"] = String.Format("{0}/{1}/{2}", String.Format("{0:N3}", Convert.ToDouble(totalAmount-lumpSumDiscountAmount)), String.Format("{0:N3}", Convert.ToDouble(totalAmountBeforeDiscount)), String.Format("{0:N3}", Convert.ToDouble(lumpSumDiscountAmount))); //=SUM(cdbl(Fields!POQquantity.Value*Fields!UnitPrice.Value))
                                            dtr["GrandTotal"] = String.Format("{0:N3}", Convert.ToDouble(totalAmount-lumpSumDiscountAmount)); //=SUM(cdbl(Fields!POQquantity.Value*Fields!UnitPrice.Value))-((SUM(cdbl(Fields!POQquantity.Value*Fields!UnitPrice.Value)))*(0/100))
                                            dtr["VAT"] = Convert.ToDouble(vat);
                                            dtr["Status"] = status;
                                            dtr["CreatedAt"] = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                                            dtr["CFOApprovedAt"] = Convert.ToDateTime(obj.checked_date).ToString("dd-MMM-yyyy");
                                            dtr["DirectorApprovedAt"] = obj.approved_date == null ? Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy") : Convert.ToDateTime(obj.approved_date).ToString("dd-MMM-yyyy");
                                            dtr["incoterm"] = incoterm;
                                            dtr["payment"] = payment;
                                            dtr["delivery"] = delivery;
                                            dtr["shipment"] = shipment;
                                            dtr["warranty"] = warranty;
                                            dtr["vendor_ref"] = vendor_ref;
                                            dtr["brand"] = brand;
                                            dtr["ir_noMR"] = ir_noMR;
                                            dtr["projectshortname"] = projectshortname;
                                            dtr["soNo"] = soNo;
                                            dtr["is_local"] = is_local;
                                            dtr["cratedBySignatureType"] = "";
                                            
                                            dtr["createdBySignature"] = "https://lgtss.com/Assets/images/lotusgreeenteam.png";
                                            tb.Rows.Add(dtr);

                                        }
                                    }
                                }

                            }
                        }

                        //rv.LocalReport.ReportPath = reportPath;
                        string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;

                        rv.Reset();
                        rv.LocalReport.EnableExternalImages = true;
                        string filePath = new Uri(string.Format("{0}{1}", path, preparedBySignature)).AbsoluteUri;
                        ReportParameter[] param = new ReportParameter[3];
                        param[0] = new ReportParameter("ImgPath", filePath);
                        param[1] = new ReportParameter("ImgPath1", new Uri(string.Format("{0}{1}", path, approvedBySignature)).AbsoluteUri);
                        param[2] = new ReportParameter("ImgPath2", new Uri(string.Format("{0}{1}", path, checkedBySignature)).AbsoluteUri);
                        rv.ProcessingMode = ProcessingMode.Local;

                        LocalReport rep = rv.LocalReport;
                        //rv.LocalReport.ReportPath = reportPath;
                        rep.ReportPath = reportPath;
                        rep.SetParameters(param);

                        rv.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("purchaseOrderToSupplier", tb);
                        rv.LocalReport.DataSources.Add(rdc);

                        //string imagePath = new Uri("https://lgtss.com/Assets/images/lotusgreeenteam.png").AbsoluteUri;
                        //ReportParameter parameter = new ReportParameter("SignatureCreator", imagePath);
                        //rv.LocalReport.SetParameters(parameter);
                        
                        rv.LocalReport.Refresh();
                    }
                }
            }
            catch (Exception ex) { }
        }
        public static void GenerateProjectbyStatus(ReportViewer rv, string reportPath, string status)
        {

        }
        public static void GenerateBOQbyProject(ReportViewer rv, string reportPath, string boqId)
        {
            try
            {
                using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    BT_KimMex.Models.BOQViewModel obj = new Models.BOQViewModel();
                    obj = (from boq in db.tb_build_of_quantity
                           join project in db.tb_project on boq.project_id equals project.project_id
                           where boq.boq_id == boqId
                           select new Models.BOQViewModel() { boq_id = boq.boq_id, project_full_name = project.project_full_name }).FirstOrDefault();
                    DataTable tb = new DataTable();
                    if (obj != null)
                    {
                        DataRow dtr;
                        DataColumn col = new DataColumn();
                        col.ColumnName = "ProjectFullname";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "JobCategory";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "JobCategoryAmount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemType";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemTypeAmount";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemTypeRemark";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemCode";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemName";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemUnit";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemQty";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemUnitPrice";
                        tb.Columns.Add(col);
                        col = new DataColumn();
                        col.ColumnName = "ItemAmount";
                        tb.Columns.Add(col);

                        var Jobs = (from bJob in db.tb_boq_detail1
                                    join job in db.tb_job_category on bJob.job_category_id equals job.j_category_id
                                    orderby bJob.job_category_code
                                    where bJob.boq_id == obj.boq_id
                                    select new Models.BOQDetail1() { boq_detail1_id = bJob.boq_detail1_id, job_category_code = bJob.job_category_code, j_category_name = job.j_category_name, amount = bJob.amount, remark = bJob.remark }).ToList();
                        if (Jobs.Any())
                        {
                            foreach (var job in Jobs)
                            {
                                var types = (from bType in db.tb_boq_detail2
                                             join type in db.tb_product_category on bType.item_type_id equals type.p_category_id
                                             orderby type.p_category_code
                                             where bType.boq_detail1_id == job.boq_detail1_id
                                             select new Models.BOQDetail2() { boq_detail2_id = bType.boq_detail2_id, p_category_name = type.p_category_name, amount = bType.amount, remark = bType.remark }).ToList();
                                if (types.Any())
                                {
                                    foreach (var type in types)
                                    {
                                        var items = (from bItem in db.tb_boq_detail3
                                                     join item in db.tb_product on bItem.item_id equals item.product_id
                                                     orderby item.product_code
                                                     where bItem.boq_detail2_id == type.boq_detail2_id
                                                     select new Models.BOQDetail3() { product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, item_qty = bItem.item_qty, unit_price = bItem.item_unit_price }).ToList();
                                        if (items.Any())
                                        {
                                            foreach (var item in items)
                                            {
                                                dtr = tb.NewRow();
                                                dtr["ProjectFullname"] = obj.project_full_name;
                                                dtr["JobCategory"] = job.j_category_name;
                                                dtr["JobCategoryAmount"] = job.amount;
                                                dtr["ItemType"] = type.p_category_name;
                                                dtr["ItemTypeAmount"] = type.amount;
                                                dtr["ItemTypeRemark"] = type.remark;
                                                dtr["ItemCode"] = item.product_code;
                                                dtr["ItemName"] = item.product_name;
                                                dtr["ItemUnit"] = item.product_unit;
                                                dtr["ItemQty"] = item.item_qty;
                                                dtr["ItemUnitPrice"] = item.unit_price;
                                                dtr["ItemAmount"] = Convert.ToDecimal(item.item_qty * item.unit_price);
                                                tb.Rows.Add(dtr);
                                            }
                                        }
                                        else
                                        {
                                            dtr = tb.NewRow();
                                            dtr["ProjectFullname"] = obj.project_full_name;
                                            dtr["JobCategory"] = job.j_category_name;
                                            dtr["JobCategoryAmount"] = job.amount;
                                            dtr["ItemType"] = type.p_category_name;
                                            dtr["ItemTypeAmount"] = type.amount;
                                            dtr["ItemTypeRemark"] = type.remark;
                                            dtr["ItemCode"] = string.Empty;
                                            dtr["ItemName"] = string.Empty;
                                            dtr["ItemUnit"] = string.Empty;
                                            dtr["ItemQty"] = Convert.ToDecimal(0);
                                            dtr["ItemUnitPrice"] = Convert.ToDecimal(0);
                                            dtr["ItemAmount"] = Convert.ToDecimal(0);
                                            tb.Rows.Add(dtr);
                                        }
                                    }
                                }
                                else
                                {
                                    dtr = tb.NewRow();
                                    dtr["ProjectFullname"] = obj.project_full_name;
                                    dtr["JobCategory"] = job.j_category_name;
                                    dtr["JobCategoryAmount"] = job.amount;
                                    dtr["ItemType"] = string.Empty;
                                    dtr["ItemTypeAmount"] = string.Empty;
                                    dtr["ItemTypeRemark"] = string.Empty;
                                    dtr["ItemCode"] = string.Empty;
                                    dtr["ItemName"] = string.Empty;
                                    dtr["ItemUnit"] = string.Empty;
                                    dtr["ItemQty"] = Convert.ToDecimal(0);
                                    dtr["ItemUnitPrice"] = Convert.ToDecimal(0);
                                    dtr["ItemAmount"] = Convert.ToDecimal(0);
                                    tb.Rows.Add(dtr);
                                }
                            }
                        }
                        rv.LocalReport.ReportPath = reportPath;
                        rv.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("BOQ", tb);
                        rv.LocalReport.DataSources.Add(rdc);
                        rv.LocalReport.Refresh();
                    }
                }
            }
            catch (Exception ex) { }
        }
        public static void GenerateItemRequestbyProject(ReportViewer rv, string reportPath, string projectId)
        {
            try
            {
                List<Models.ItemRequestReport> itemRequests = new List<Models.ItemRequestReport>();
                List<Models.ItemRequestReport> itemRequests1 = new List<Models.ItemRequestReport>();
                using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    var objs = db.tb_item_request.Where(x => x.status == true && x.ir_project_id == projectId).ToList();
                    DataTable tb = new DataTable();
                    DataRow dtr;
                    DataColumn col = new DataColumn();
                    col.ColumnName = "project_full_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_request_number";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_request_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "job_category_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_unit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "remark";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "boq_quantity";
                    tb.Columns.Add(col);
                    foreach (var obj in objs)
                    {
                        var jobs = (from ir_job in db.tb_ir_detail1 join job in db.tb_job_category on ir_job.ir_job_category_id equals job.j_category_id where ir_job.ir_id == obj.ir_id select new Models.ItemRequestDetail1ViewModel() { ir_detail1_id = ir_job.ir_detail1_id, ir_job_category_id = job.j_category_id, job_category_description = job.j_category_name }).ToList();
                        if (jobs.Any())
                        {
                            foreach (var job in jobs)
                            {
                                var items = (from ir_item in db.tb_ir_detail2 join item in db.tb_product on ir_item.ir_item_id equals item.product_id orderby item.product_code where ir_item.ir_detail1_id == job.ir_detail1_id && ir_item.is_approved == true select new Models.ItemRequestDetail2ViewModel() { ir_item_id = item.product_id, product_code = item.product_code, product_name = item.product_name, product_unit = ir_item.ir_item_unit, approved_qty = ir_item.approved_qty, remark = ir_item.remark }).ToList();
                                if (items.Any())
                                {
                                    foreach (var item in items)
                                    {
                                        itemRequests.Add(new Models.ItemRequestReport()
                                        {
                                            project_full_name = db.tb_project.Where(x => x.project_id == projectId).Select(x => x.project_full_name).FirstOrDefault(),
                                            job_category_id = job.ir_job_category_id,
                                            job_category_code = CommonClass.GetJobCategoryCode(projectId, job.ir_job_category_id),
                                            job_category_name = job.job_category_description,
                                            item_id = item.ir_item_id,
                                            item_code = item.product_code,
                                            item_name = item.product_name,
                                            item_unit = item.product_unit,
                                            item_quantity = Convert.ToDecimal(item.approved_qty),
                                            remark = item.remark,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    decimal totalRequestQty = 0;
                    var dupItems = itemRequests.GroupBy(x => new { x.item_id, x.job_category_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupItems.Any())
                    {
                        foreach (var itemId in dupItems)
                        {
                            totalRequestQty = 0;
                            var requestQtys = itemRequests.Where(x => x.item_id == itemId.item_id && x.job_category_id == itemId.job_category_id).Select(x => x.item_quantity).ToList();
                            foreach (var quantity in requestQtys)
                            {
                                totalRequestQty = totalRequestQty + Convert.ToDecimal(quantity);
                            }
                            var item = itemRequests.Where(x => x.item_id == itemId.item_id && x.job_category_id == itemId.job_category_id).FirstOrDefault();
                            itemRequests1.Add(new Models.ItemRequestReport()
                            {
                                project_full_name = item.project_full_name,
                                job_category_id = item.job_category_id,
                                job_category_code = item.job_category_code,
                                job_category_name = item.job_category_name,
                                item_id = item.item_id,
                                item_code = item.item_code,
                                item_name = item.item_name,
                                item_unit = item.item_unit,
                                item_quantity = totalRequestQty,
                                remark = item.remark,
                            });
                        }
                        foreach (var item in itemRequests)
                        {
                            foreach (var itemId in dupItems)
                                if (item.item_id == itemId.item_id && item.job_category_id == itemId.job_category_id)
                                    break;
                                else
                                {
                                    itemRequests1.Add(new Models.ItemRequestReport()
                                    {
                                        project_full_name = item.project_full_name,
                                        job_category_id = item.job_category_id,
                                        job_category_code = item.job_category_code,
                                        job_category_name = item.job_category_name,
                                        item_id = item.item_id,
                                        item_code = item.item_code,
                                        item_name = item.item_name,
                                        item_unit = item.item_unit,
                                        item_quantity = item.item_quantity,
                                        remark = item.remark,
                                    });
                                }

                        }
                    }
                    else
                        itemRequests1 = itemRequests;
                    itemRequests1 = itemRequests1.OrderBy(x => x.job_category_code).ThenBy(x => x.item_id).ToList();
                    foreach (var item in itemRequests1)
                    {
                        dtr = tb.NewRow();
                        dtr["project_full_name"] = item.project_full_name;
                        dtr["job_category_name"] = item.job_category_name;
                        dtr["item_code"] = item.item_code;
                        dtr["item_name"] = item.item_name;
                        dtr["item_unit"] = item.item_unit;
                        dtr["item_quantity"] = item.item_quantity;
                        dtr["remark"] = item.remark;
                        dtr["boq_quantity"] = ItemRequest.GetBoqItemQty(projectId, item.job_category_id, item.item_id);
                        tb.Rows.Add(dtr);
                    }

                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("ItemRequest", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }
        }
        public static void GeneratePurchaseOrderReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status, string supplier, string project)
        {
            try
            {
                List<Models.PurchaseOrderViewModel> objs = new List<Models.PurchaseOrderViewModel>();
                List<Models.PurchaseOrderReport> purchaseOrders = new List<Models.PurchaseOrderReport>();
                List<Models.PurchaseOrderReport> purchaseOrders1 = new List<Models.PurchaseOrderReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    if (string.Compare(status, "All") == 0)
                    {
                        if (string.IsNullOrEmpty(project))
                        {
                            objs = (from poo in db.tb_purchase_request
                                    join po in db.tb_purchase_order on poo.purchase_order_id equals po.purchase_order_id
                                    join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                    join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                                    join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                    orderby po.approved_date
                                    where poo.status == true && poo.created_date >= dateFrom && poo.created_date <= toDate
                                    select new Models.PurchaseOrderViewModel()
                                    {
                                        purchase_order_id = po.purchase_order_id,
                                        purchase_oder_number = po.purchase_oder_number,
                                        created_date = po.created_date,
                                        created_by = po.created_by,
                                        project_full_name = pro.project_full_name,
                                        purchase_order_status = po.purchase_order_status
                                    }).ToList();
                        }
                        else
                            objs = (from poo in db.tb_purchase_request
                                    join po in db.tb_purchase_order on poo.purchase_order_id equals po.purchase_order_id
                                    join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                    join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                                    join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                    orderby po.approved_date
                                    where poo.status == true && poo.created_date >= dateFrom && poo.created_date <= toDate && string.Compare(ir.ir_project_id, project) == 0
                                    select new Models.PurchaseOrderViewModel()
                                    {
                                        purchase_order_id = po.purchase_order_id,
                                        purchase_oder_number = po.purchase_oder_number,
                                        created_date = po.created_date,
                                        created_by = po.created_by,
                                        project_full_name = pro.project_full_name,
                                        purchase_order_status = po.purchase_order_status
                                    }).ToList();
                    }
                    else if (string.Compare(status, "All") != 0)
                    {
                        if (string.IsNullOrEmpty(project))
                            objs = (from poo in db.tb_purchase_request
                                    join po in db.tb_purchase_order on poo.purchase_order_id equals po.purchase_order_id
                                    join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                    join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                                    join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                    orderby po.approved_date
                                    where poo.status == true && poo.created_date >= dateFrom && poo.created_date <= toDate && string.Compare(poo.purchase_request_status, status) == 0
                                    select new Models.PurchaseOrderViewModel()
                                    {
                                        purchase_order_id = po.purchase_order_id,
                                        purchase_oder_number = po.purchase_oder_number,
                                        created_date = po.created_date,
                                        created_by = po.created_by,
                                        project_full_name = pro.project_full_name,
                                        purchase_order_status = po.purchase_order_status
                                    }).ToList();
                        else
                            objs = (from poo in db.tb_purchase_request
                                    join po in db.tb_purchase_order on poo.purchase_order_id equals po.purchase_order_id
                                    join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                                    join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                                    join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                    orderby po.approved_date
                                    where poo.status == true && poo.created_date >= dateFrom && poo.created_date <= toDate && string.Compare(poo.purchase_request_status, status) == 0 && string.Compare(ir.ir_project_id, project) == 0
                                    select new Models.PurchaseOrderViewModel()
                                    {
                                        purchase_order_id = po.purchase_order_id,
                                        purchase_oder_number = po.purchase_oder_number,
                                        created_date = po.created_date,
                                        created_by = po.created_by,
                                        project_full_name = pro.project_full_name,
                                        purchase_order_status = po.purchase_order_status
                                    }).ToList();
                    }
                    DataColumn col = new DataColumn();
                    col.ColumnName = "PODate";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ProjectName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "POStatus";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "RequestedBy";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Description";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "DateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "DateTo";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "POSupplier";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "PONumber";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportStatus";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportSupplier";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportProject";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "POVAT";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "PONonVAT";
                    tb.Columns.Add(col);
                    if (objs.Any())
                    {
                        foreach (var obj in objs)
                        {
                            if (string.Compare(obj.purchase_order_status, "Pending") == 0 || string.Compare(obj.purchase_order_status, "Rejected") == 0)
                            {
                                purchaseOrders.Add(new Models.PurchaseOrderReport()
                                {
                                    purchaseOrderId = obj.purchase_order_id,
                                    purchase_order_date = Convert.ToDateTime(obj.created_date),
                                    project_full_name = obj.project_full_name,
                                    poStatus = obj.purchase_order_status,
                                    prepared_by = CommonFunctions.GetUserFullnamebyUserId(obj.created_by),
                                    dateFrom = Convert.ToDateTime(dateFrom),
                                    dateTo = Convert.ToDateTime(dateTo),
                                    purchase_order_number = obj.purchase_oder_number,
                                    poSuppplier = string.Empty,
                                    supplierId = string.Empty,
                                });

                                //dtr = tb.NewRow();
                                //dtr["PODate"] = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                                //dtr["ProjectName"] = obj.project_full_name;
                                //dtr["POStatus"] = obj.purchase_order_status;
                                //dtr["RequestedBy"] = CommonClass.GetUserFullname(obj.created_by);
                                //dtr["Description"] = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                                //dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                //dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                //dtr["PONumber"] = obj.purchase_oder_number;
                                //dtr["POSupplier"] = string.Empty;
                                //tb.Rows.Add(dtr);
                            }
                            else if (string.Compare(obj.purchase_order_status, "Approved") == 0 || string.Compare(obj.purchase_order_status, "Completed") == 0)
                            {
                                var suppliers = (from po in db.tb_purchase_order
                                                 join pod in db.tb_purchase_order_detail on po.purchase_order_id equals pod.purchase_order_id
                                                 join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                                                 join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                                 where po.purchase_order_id == obj.purchase_order_id && pos.is_selected == true
                                                 select sup).ToList();
                                foreach (var sup in suppliers)
                                {
                                    purchaseOrders.Add(new Models.PurchaseOrderReport()
                                    {
                                        purchaseOrderId = obj.purchase_order_id,
                                        purchase_order_date = Convert.ToDateTime(obj.created_date),
                                        project_full_name = obj.project_full_name,
                                        poStatus = obj.purchase_order_status,
                                        prepared_by = CommonFunctions.GetUserFullnamebyUserId(obj.created_by),
                                        dateFrom = Convert.ToDateTime(dateFrom),
                                        dateTo = Convert.ToDateTime(dateTo),
                                        purchase_order_number = obj.purchase_oder_number,
                                        poSuppplier = sup.supplier_name,
                                        supplierId = sup.supplier_id,
                                    });
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(supplier))
                            purchaseOrders = purchaseOrders.Where(x => x.poSuppplier == (db.tb_supplier.Where(m => m.supplier_id == supplier).Select(m => m.supplier_name).FirstOrDefault())).ToList();
                        if (purchaseOrders.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                            dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                            dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                            dtr["ReportStatus"] = status;
                            dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => string.Compare(x.project_id, project) == 0).Select(x => x.project_full_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            var dupItems = purchaseOrders.GroupBy(x => new { x.purchase_order_number, x.poSuppplier }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dupItems.Any())
                            {
                                foreach (var it in dupItems)
                                {
                                    var item = purchaseOrders.Where(x => x.purchase_order_number == it.purchase_order_number && x.poSuppplier == it.poSuppplier).FirstOrDefault();
                                    purchaseOrders1.Add(new Models.PurchaseOrderReport()
                                    {
                                        purchaseOrderId = item.purchaseOrderId,
                                        purchase_order_date = item.purchase_order_date,
                                        project_full_name = item.project_full_name,
                                        poStatus = item.poStatus,
                                        prepared_by = item.prepared_by,
                                        dateFrom = item.dateFrom,
                                        dateTo = item.dateTo,
                                        purchase_order_number = item.purchase_order_number,
                                        poSuppplier = item.poSuppplier,
                                        supplierId = item.supplierId,
                                    });
                                }
                                foreach (var po in purchaseOrders)
                                {
                                    /*
                                    foreach (var item in dupItems)
                                        if (item.purchase_order_number == po.purchase_order_number && po.poSuppplier == item.poSuppplier)
                                            break;
                                        else
                                        {
                                            purchaseOrders1.Add(new Models.PurchaseOrderReport()
                                            {
                                                purchase_order_date = po.purchase_order_date,
                                                project_full_name = po.project_full_name,
                                                poStatus = po.poStatus,
                                                prepared_by = po.prepared_by,
                                                dateFrom = po.dateFrom,
                                                dateTo = po.dateTo,
                                                purchase_order_number = po.purchase_order_number,
                                                poSuppplier = po.poSuppplier
                                            });
                                        }
                                        */
                                    var found = dupItems.Where(x => string.Compare(x.purchase_order_number, po.purchase_order_number) == 0 && string.Compare(x.poSuppplier, po.poSuppplier) == 0).ToList();
                                    if (!found.Any())
                                    {
                                        purchaseOrders1.Add(new Models.PurchaseOrderReport()
                                        {
                                            purchaseOrderId = po.purchaseOrderId,
                                            purchase_order_date = po.purchase_order_date,
                                            project_full_name = po.project_full_name,
                                            poStatus = po.poStatus,
                                            prepared_by = po.prepared_by,
                                            dateFrom = po.dateFrom,
                                            dateTo = po.dateTo,
                                            purchase_order_number = po.purchase_order_number,
                                            poSuppplier = po.poSuppplier,
                                            supplierId = po.supplierId
                                        });
                                    }
                                }
                            }
                            else
                                purchaseOrders1 = purchaseOrders;
                            purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                            foreach (var obj in purchaseOrders1)
                            {
                                dtr = tb.NewRow();
                                dtr["PODate"] = Convert.ToDateTime(obj.purchase_order_date).ToString("dd-MMM-yyyy");
                                dtr["ProjectName"] = obj.project_full_name;
                                dtr["POStatus"] = obj.poStatus;
                                dtr["RequestedBy"] = obj.prepared_by;
                                dtr["Description"] = string.Empty;
                                dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                dtr["PONumber"] = obj.purchase_order_number;
                                dtr["POSupplier"] = obj.poSuppplier;
                                dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                                dtr["ReportStatus"] = status;
                                dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => string.Compare(x.project_id, project) == 0).Select(x => x.project_full_name).FirstOrDefault();
                                dtr["POVAT"] = CommonClass.GetPurchaseOrderNumber(obj.purchaseOrderId, obj.supplierId, true);
                                dtr["PONonVAT"] = CommonClass.GetPurchaseOrderNumber(obj.purchaseOrderId, obj.supplierId, false);
                                tb.Rows.Add(dtr);
                            }
                        }
                    }
                    else
                    {
                        dtr = tb.NewRow();
                        dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                        dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                        dtr["ReportStatus"] = status;
                        dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => string.Compare(x.project_id, project) == 0).Select(x => x.project_full_name).FirstOrDefault();
                        tb.Rows.Add(dtr);
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("PurchaseOrder", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }

        }
        public static void GeneratePurchaseOrderSupplierReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string supplier, string project)
        {
            try
            {
                const decimal VAT = 10 / 100;
                List<Models.PurchaseOrderViewModel> objs = new List<Models.PurchaseOrderViewModel>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "PODate";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ProjectName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "POStatus";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "RequestedBy";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Description";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "DateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "DateTo";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "POSupplier";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "PONumber";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportStatus";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportSupplier";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Amount";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ReportProject";
                    tb.Columns.Add(col);

                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    if (string.IsNullOrEmpty(project))
                    {
                        objs = (from po in db.tb_purchase_order
                                join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
                                join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                orderby po.approved_date
                                where po.status == true && po.purchase_order_status == "Completed" && po.created_date >= dateFrom && po.created_date <= toDate
                                select new Models.PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, created_date = po.created_date, created_by = po.created_by, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status }).ToList();
                    }
                    else
                        objs = (from po in db.tb_purchase_order
                                join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
                                join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                                orderby po.approved_date
                                where po.status == true && po.purchase_order_status == "Completed" && po.created_date >= dateFrom && po.created_date <= toDate && string.Compare(ir.ir_project_id, project) == 0
                                select new Models.PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, created_date = po.created_date, created_by = po.created_by, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status }).ToList();
                    if (objs.Any())
                    {
                        int count = 0;
                        decimal vat = Convert.ToDecimal(db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.discount).FirstOrDefault());
                        foreach (var obj in objs)
                        {
                            Decimal amount = 0;
                            var poDetail = (from pod in db.tb_purchase_order_detail
                                            where pod.purchase_order_id == obj.purchase_order_id && pod.item_status == "approved"
                                            select new Models.PurchaseOrderDetailViewModel() { po_detail_id = pod.po_detail_id, item_id = pod.item_id, po_quantity = pod.po_quantity, unit_price = pod.unit_price, item_vat = pod.item_vat }).ToList();

                            if (poDetail.Any())
                            {
                                decimal totalAmountVAT = 0;
                                decimal totalAmountNonVAT = 0;
                                foreach (var pod in poDetail)
                                {
                                    Entities.tb_po_supplier poSupplier;
                                    if (string.IsNullOrEmpty(supplier))
                                    {
                                        poSupplier = (from sup in db.tb_po_supplier where sup.po_detail_id == pod.po_detail_id && sup.is_selected == true select sup).FirstOrDefault();
                                    }
                                    else
                                    {
                                        poSupplier = (from sup in db.tb_po_supplier
                                                      where sup.po_detail_id == pod.po_detail_id && sup.is_selected == true && sup.supplier_id == supplier
                                                      select sup).FirstOrDefault();
                                    }
                                    //var poSupplier = (from sup in db.tb_po_supplier
                                    //                  where sup.po_detail_id == pod.po_detail_id && sup.is_selected == true && sup.supplier_id == supplier
                                    //                  select sup).FirstOrDefault();
                                    //if (poSupplier != null)
                                    //{
                                    //    amount = amount +Convert.ToDecimal(pod.po_quantity * pod.unit_price);
                                    //    count++;
                                    //}
                                    if (poSupplier != null)
                                    {
                                        if (Convert.ToBoolean(pod.item_vat)) totalAmountVAT = totalAmountVAT + Convert.ToDecimal(pod.po_quantity * pod.unit_price);
                                        else totalAmountNonVAT = totalAmountNonVAT + Convert.ToDecimal(pod.po_quantity * pod.unit_price);
                                        count++;
                                    }
                                }

; amount = (totalAmountVAT + Convert.ToDecimal(totalAmountVAT * VAT)) + totalAmountNonVAT;
                            }

                            if (amount > 0)
                            {
                                dtr = tb.NewRow();
                                dtr["PODate"] = Convert.ToDateTime(obj.created_date).ToString("dd-MMM-yyyy");
                                dtr["ProjectName"] = obj.project_full_name;
                                dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                dtr["PONumber"] = obj.purchase_oder_number;
                                dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                                dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => x.project_id == project).Select(x => x.project_full_name).FirstOrDefault();
                                //dtr["Amount"] =Convert.ToDecimal( amount - (amount * Convert.ToDecimal(vat / 100)));
                                dtr["Amount"] = decimal.Round(amount - (amount * Convert.ToDecimal(vat / 100)), 2, MidpointRounding.AwayFromZero);
                                tb.Rows.Add(dtr);

                            }
                        }
                        if (count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                            dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                            dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                            dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => x.project_id == project).Select(x => x.project_full_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                    }
                    else
                    {
                        dtr = tb.NewRow();
                        dtr["DateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["DateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                        dtr["ReportSupplier"] = string.IsNullOrEmpty(supplier) ? "All" : db.tb_supplier.Where(x => x.supplier_id == supplier).Select(x => x.supplier_name).FirstOrDefault();
                        dtr["ReportProject"] = string.IsNullOrEmpty(project) ? "All" : db.tb_project.Where(x => x.project_id == project).Select(x => x.project_full_name).FirstOrDefault();
                        tb.Rows.Add(dtr);
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("PurchaseOrderSupplier", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }
        }
        public static void GenerateStockMovementReport(ReportViewer rv, string reportPath, DateTime DateFrom, DateTime DateTo, string status)
        {
            try
            {
                List<Models.StockMovementViewModel> stoM = new List<Models.StockMovementViewModel>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(DateTo).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "Product_Code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Product_Name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "P_Category_Name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Inventory_Date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Warehouse_Name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Project_Name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Stock_transfer_status";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Total_Quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Out_Quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Item_Request_no";
                    tb.Columns.Add(col);

                    col = new DataColumn();
                    col.ColumnName = "DateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "DateTo";
                    tb.Columns.Add(col);

                    stoM = (from stoMs in db.tb_stock_transfer_voucher
                                //join inVet in db.tb_inventory on stoM.item_request_id equals inVet.inventory_id
                            join Ir in db.tb_item_request on stoMs.item_request_id equals Ir.ir_id
                            join pro in db.tb_project on Ir.ir_project_id equals pro.project_id
                            //join inVet in db.tb_inventory on stoMs.stock_transfer_status equals inVet.inventory_id
                            where stoMs.status == true && stoMs.stock_transfer_status == "Approved" && stoMs.created_date >= DateFrom && stoMs.created_date <= toDate
                            select new Models.StockMovementViewModel()
                            {
                                stock_transfer_id = stoMs.stock_transfer_id,
                                Stock_transfer_status = stoMs.stock_transfer_status,
                                Item_Request_no = Ir.ir_no,
                                Project_Name = pro.project_full_name,
                            }

                          ).ToList();

                    if (stoM.Any())
                    {
                        foreach (var products in stoM)
                        {
                            var pros = (from inVet in db.tb_inventory
                                            //join stoMT in db.tb_stock_transfer_voucher on inVet.ref_id equals stoMT.stock_transfer_id
                                            //join ir in db.tb_item_request on stoMT.item_request_id equals ir.ir_id
                                        join war in db.tb_warehouse on inVet.warehouse_id equals war.warehouse_id
                                        join pro in db.tb_product on inVet.product_id equals pro.product_id
                                        join proC in db.tb_brand on pro.brand_id equals proC.brand_id
                                        //join proJ in db.tb_project in
                                        where inVet.ref_id == products.stock_transfer_id
                                        select new Models.StockMovementViewModel()
                                        {
                                            Warehouse_Name = war.warehouse_name,

                                            Inventory_Date = inVet.inventory_date,
                                            //Item_Request_no = ir.ir_no,
                                            Stock_transfer_status = products.Stock_transfer_status,
                                            Product_Code = pro.product_code,
                                            Product_Name = pro.product_name,
                                            Out_Quantity = inVet.out_quantity,
                                            Total_Quantity = inVet.total_quantity,
                                            P_Category_Name = proC.brand_name,
                                            Project_Name = products.Project_Name

                                        }
                                      ).ToList();
                            foreach (var product in pros)
                            {
                                dtr = tb.NewRow();
                                dtr["Product_Code"] = product.Product_Code;
                                dtr["Product_Name"] = product.Product_Name;
                                dtr["P_Category_Name"] = product.P_Category_Name;
                                dtr["Inventory_Date"] = product.Inventory_Date;
                                dtr["Warehouse_Name"] = product.Warehouse_Name;
                                dtr["Project_Name"] = product.Project_Name;
                                dtr["Stock_transfer_status"] = product.Stock_transfer_status;
                                dtr["Item_Request_no"] = product.Item_Request_no;
                                dtr["Total_Quantity"] = product.Total_Quantity;
                                dtr["Out_Quantity"] = product.Out_Quantity;
                                dtr["DateFrom"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                                dtr["DateTo"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                                tb.Rows.Add(dtr);
                            }

                        }
                    }

                    else
                    {
                        dtr = tb.NewRow();
                        dtr["DateFrom"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                        dtr["DateTo"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockMovement", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();

                }
            }
            catch (Exception ex) { }
        }
        public static void GenerateStockMovementReport(ReportViewer rv, string reportPath, DateTime DateFrom, DateTime DateTo)
        {
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                DataColumn col = new DataColumn();
                col.ColumnName = "Inventory_Date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Id";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Code";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Unit";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Total_Quantity";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "In_Quantity";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Out_Quantity";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Warehouse_Name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Inventory_Status";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Date_From";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Date_To";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "InvoiceNumber";
                tb.Columns.Add(col);
                DateTime toDate = Convert.ToDateTime(DateTo).AddDays(1);
                List<Models.InventoryViewModel> objs = new List<Models.InventoryViewModel>();
                #region stock adjustment,stock damage,item return, stock issue, stock issue return 
                var stocks = (from inv in db.tb_inventory_detail
                              join status in db.tb_inventory_status on inv.inventory_type equals status.s_status_id
                              join item in db.tb_product on inv.inventory_item_id equals item.product_id
                              join warehouse in db.tb_warehouse on inv.inventory_warehouse_id equals warehouse.warehouse_id
                              orderby inv.invoice_date, item.product_code
                              where inv.invoice_date >= DateFrom && inv.invoice_date <= DateTo && string.Compare(inv.item_status, "approved") == 0
                              select new { inv, status, warehouse, item }).ToList();
                foreach (var stock in stocks)
                {
                    Models.InventoryViewModel inventory = new Models.InventoryViewModel();
                    inventory.invoice_date = stock.inv.invoice_date;
                    inventory.invoice_number = stock.inv.invoice_number;
                    inventory.product_id = stock.inv.inventory_item_id;
                    inventory.itemCode = stock.item.product_code;
                    inventory.itemName = stock.item.product_name;
                    inventory.unit = stock.inv.unit;
                    if (string.Compare(stock.inv.inventory_type, "2") == 0 || string.Compare(stock.inv.inventory_type, "3") == 0 || string.Compare(stock.inv.inventory_type, "8") == 0)
                    {
                        inventory.in_quantity = 0;
                        inventory.out_quantity = stock.inv.quantity;
                    }
                    else if (string.Compare(stock.inv.inventory_type, "5") == 0 || string.Compare(stock.inv.inventory_type, "7") == 0)
                    {
                        inventory.in_quantity = stock.inv.quantity;
                        inventory.out_quantity = 0;
                    }
                    inventory.warehouseName = stock.warehouse.warehouse_name;
                    inventory.inventory_status_id = stock.status.s_status_name;
                    objs.Add(inventory);
                }
                #endregion
                #region item received
                var itemReceives = (from received in db.tb_received_item_detail
                                    join item in db.tb_product on received.ri_item_id equals item.product_id
                                    join warehouse in db.tb_warehouse on received.ri_warehouse_id equals warehouse.warehouse_id
                                    orderby received.invoice_date, item.product_code
                                    where received.invoice_date >= DateFrom && received.invoice_date <= DateTo && string.Compare(received.item_status, "approved") == 0
                                    select new { received, item, warehouse }).ToList();
                foreach (var received in itemReceives)
                {
                    Models.InventoryViewModel inventory = new Models.InventoryViewModel();
                    inventory.invoice_date = received.received.invoice_date;
                    inventory.invoice_number = received.received.invoice_number;
                    inventory.product_id = received.received.ri_item_id;
                    inventory.itemCode = received.item.product_code;
                    inventory.itemName = received.item.product_name;
                    inventory.unit = received.received.unit;
                    inventory.in_quantity = received.received.quantity;
                    inventory.out_quantity = 0;
                    inventory.warehouseName = received.warehouse.warehouse_name;
                    inventory.inventory_status_id = "Receive";
                    objs.Add(inventory);
                }
                #endregion
                #region stock transfer
                var transfers = (from transfer in db.tb_stock_transfer_detail
                                 join item in db.tb_product on transfer.st_item_id equals item.product_id
                                 join warehouse in db.tb_warehouse on transfer.st_warehouse_id equals warehouse.warehouse_id
                                 orderby transfer.invoice_date, item.product_code
                                 where transfer.invoice_date >= DateFrom && transfer.invoice_date <= DateTo && string.Compare(transfer.item_status, "approved") == 0
                                 select new { transfer, item, warehouse }).ToList();
                foreach (var transfer in transfers)
                {
                    Models.InventoryViewModel inventory = new Models.InventoryViewModel();
                    inventory.invoice_date = transfer.transfer.invoice_date;
                    inventory.invoice_number = transfer.transfer.invoice_number;
                    inventory.product_id = transfer.transfer.st_item_id;
                    inventory.itemCode = transfer.item.product_code;
                    inventory.itemName = transfer.item.product_name;
                    inventory.unit = transfer.transfer.unit;
                    inventory.in_quantity = 0;
                    inventory.out_quantity = transfer.transfer.quantity;
                    inventory.warehouseName = transfer.warehouse.warehouse_name;
                    inventory.inventory_status_id = "Transfer";
                    objs.Add(inventory);
                }
                #endregion

                if (objs.Any())
                {
                    objs = objs.OrderBy(m => m.invoice_date).ThenBy(m => m.itemCode).ToList();
                    foreach (var obj in objs)
                    {
                        dtr = tb.NewRow();
                        dtr["Inventory_Date"] = Convert.ToDateTime(obj.invoice_date).ToString("dd-MMM-yyyy");
                        dtr["Item_Id"] = obj.product_id;
                        dtr["Item_Code"] = obj.itemCode;
                        dtr["Item_Name"] = obj.itemName;
                        dtr["Item_Unit"] = obj.unit;
                        dtr["In_Quantity"] = string.Format("{0:G29}", decimal.Parse(obj.in_quantity.ToString()));
                        dtr["Out_Quantity"] = string.Format("{0:G29}", decimal.Parse(obj.out_quantity.ToString())); ;
                        dtr["Warehouse_Name"] = obj.warehouseName;
                        dtr["Inventory_Status"] = obj.inventory_status_id;
                        dtr["InvoiceNumber"] = obj.invoice_number;
                        dtr["Date_From"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                        dtr["Date_To"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }
                }
                else
                {
                    dtr = tb.NewRow();
                    dtr["Date_From"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                    dtr["Date_To"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                    tb.Rows.Add(dtr);
                }

                #region old process
                /*
                var objs = (from inv in db.tb_inventory
                            join status in db.tb_inventory_status on inv.inventory_status_id equals status.s_status_id
                            join item in db.tb_product on inv.product_id equals item.product_id
                            join warehouse in db.tb_warehouse on inv.warehouse_id equals warehouse.warehouse_id
                            orderby inv.inventory_date, item.product_code
                            where inv.inventory_date >= DateFrom && inv.inventory_date <= toDate
                            select new { inv, status, item, warehouse }).ToList();
                if (objs.Any())
                {
                    objs = objs.OrderBy(m => m.inv.inventory_date).ThenBy(m => m.item.product_code).ToList();
                    foreach (var obj in objs)
                    {
                        dtr = tb.NewRow();
                        dtr["Inventory_Date"] = Convert.ToDateTime(obj.inv.inventory_date).ToString("dd-MMM-yyyy");
                        dtr["Item_Id"] = obj.inv.product_id;
                        dtr["Item_Code"] = obj.item.product_code;
                        dtr["Item_Name"] = obj.item.product_name;
                        dtr["Item_Unit"] = obj.item.product_unit;
                        //dtr["Total_Quantity"] = obj.inv.total_quantity;
                        dtr["In_Quantity"] = obj.inv.in_quantity;
                        dtr["Out_Quantity"] = obj.inv.out_quantity;
                        dtr["Warehouse_Name"] = obj.warehouse.warehouse_name;
                        dtr["Inventory_Status"] = obj.status.s_status_name;
                        dtr["Date_From"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                        dtr["Date_To"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }
                }
                else
                {
                    dtr = tb.NewRow();
                    dtr["Date_From"] = Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyyy");
                    dtr["Date_To"] = Convert.ToDateTime(DateTo).ToString("dd-MMM-yyyy");
                    tb.Rows.Add(dtr);
                }
                */
                #endregion
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("InventoryMovement", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();
            }
        }
        public static void GenerateReturnItemtoSupplierReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status)
        {
            try
            {
                List<Models.ReturnItemtoSupplierViewModel> ReItoSub = new List<Models.ReturnItemtoSupplierViewModel>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "created_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_return_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_return_number";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "supplier_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "dateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "dateTo";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "InvoiceDate";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "InvoiceNumber";
                    tb.Columns.Add(col);
                    /*
                    ReItoSub = (from ReItoSubs in db.tb_item_return
                                //join inVetD in db.tb_inventory_detail on ReItoSubs.item_return_id equals inVetD.inventory_ref_id
                                where ReItoSubs.status == true && ReItoSubs.item_return_status == "Completed" && ReItoSubs.created_date >= dateFrom && ReItoSubs.created_date <= dateTo
                                //where ReItoSubs.status==true && string.Compare(ReItoSubs.item_return_status,"Completed")==0 && DateTime.Compare(Convert.ToDateTime(ReItoSubs.created_date), dateFrom)>0 && DateTime.Compare(Convert.ToDateTime(ReItoSubs.created_date),dateTo)<0
                                orderby ReItoSubs.created_date
                                select new Models.ReturnItemtoSupplierViewModel()
                                {
                                    item_return_number = ReItoSubs.item_return_number,
                                    item_return_id = ReItoSubs.item_return_id,
                                    created_date = ReItoSubs.created_date,
                                }).ToList();
                                */

                    //new process filter by invoice date
                    var objs = db.tb_inventory_detail.OrderBy(x => x.invoice_date).Where(x => x.invoice_date >= dateFrom && x.invoice_date <= dateTo && string.Compare(x.inventory_type, "8") == 0).Select(x => new { x }).ToList();
                    if (objs.Any())
                    {
                        var duplicateRefId = objs.GroupBy(x => x.x.inventory_ref_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateRefId.Any())
                        {
                            foreach (var dupId in duplicateRefId)
                            {
                                var itemReturn = db.tb_item_return.Where(x => string.Compare(x.item_return_id, dupId) == 0 && string.Compare(x.item_return_status, "Completed") == 0).FirstOrDefault();
                                if (itemReturn != null)
                                    ReItoSub.Add(new Models.ReturnItemtoSupplierViewModel() { item_return_number = itemReturn.item_return_number, item_return_id = itemReturn.item_return_id, created_date = itemReturn.created_date });
                            }
                        }
                        foreach (var obj in objs)
                        {
                            bool isExist = duplicateRefId.Where(x => string.Compare(x, obj.x.inventory_ref_id) == 0).Count() > 0 ? true : false;
                            if (!isExist)
                            {
                                var itemReturn = db.tb_item_return.Where(x => string.Compare(x.item_return_id, obj.x.inventory_ref_id) == 0 && string.Compare(x.item_return_status, "Completed") == 0).FirstOrDefault();
                                if (itemReturn != null)
                                    ReItoSub.Add(new Models.ReturnItemtoSupplierViewModel() { item_return_number = itemReturn.item_return_number, item_return_id = itemReturn.item_return_id, created_date = itemReturn.created_date });
                            }
                        }
                    }

                    if (ReItoSub.Any())
                    {
                        foreach (var items in ReItoSub)
                        {
                            //var itemReturns = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.ref_id, items.item_return_id) == 0).ToList();
                            //var itemReturns = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, items.item_return_id) == 0).ToList();
                            var itemReturns = (from invd in db.tb_inventory_detail
                                               join un in db.tb_unit on invd.unit equals un.Id
                                               join ir in db.tb_item_return on invd.inventory_ref_id equals ir.item_return_id
                                               where invd.inventory_ref_id == items.item_return_id
                                               select new { invd, un, ir }).ToList();
                            if (itemReturns.Any())
                            {
                                foreach (var item in itemReturns)
                                {
                                    dtr = tb.NewRow();
                                    string supplierId = db.tb_inventory_detail.Where(x => string.Compare(x.inventory_ref_id, items.item_return_id) == 0 && string.Compare(x.inventory_warehouse_id, item.invd.inventory_warehouse_id) == 0 && string.Compare(x.inventory_item_id, item.invd.inventory_item_id) == 0).Select(x => x.supplier_id).FirstOrDefault();
                                    dtr["created_date"] = Convert.ToDateTime(items.created_date).ToString("dd-MMM-yyyy");
                                    dtr["warehouse_name"] = db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, item.invd.inventory_warehouse_id) == 0).Select(x => x.warehouse_name).FirstOrDefault();
                                    dtr["item_return_number"] = items.item_return_number;
                                    dtr["supplier_name"] = db.tb_supplier.Where(x => string.Compare(x.supplier_id, supplierId) == 0).Select(x => x.supplier_name).FirstOrDefault();
                                    dtr["quantity"] = Convert.ToDecimal(item.invd.quantity);
                                    dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                    dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                    dtr["itemCode"] = db.tb_product.Where(x => string.Compare(x.product_id, item.invd.inventory_item_id) == 0).Select(x => x.product_code).FirstOrDefault();
                                    dtr["itemName"] = db.tb_product.Where(x => string.Compare(x.product_id, item.invd.inventory_item_id) == 0).Select(x => x.product_name).FirstOrDefault();
                                    //dtr["itemUnit"]= db.tb_product.Where(x => string.Compare(x.product_id, item.inventory_item_id) == 0).Select(x => x.product_unit).FirstOrDefault();
                                    dtr["itemUnit"] = item.un.Name;
                                    dtr["InvoiceDate"] = Convert.ToDateTime(item.invd.invoice_date).ToString("dd-MM-yyyy");
                                    dtr["InvoiceNumber"] = item.ir.item_return_number;
                                    tb.Rows.Add(dtr);
                                }
                            }
                            /*
                            var itemP = (from inVetD in db.tb_inventory_detail
                                         join sup in db.tb_supplier on inVetD.supplier_id equals sup.supplier_id
                                         join ware in db.tb_warehouse on inVetD.inventory_warehouse_id equals ware.warehouse_id
                                         where inVetD.inventory_ref_id == items.item_return_id
                                         select new Models.ReturnItemtoSupplierViewModel()
                                         {
                                             item_return_id=items.item_return_id,
                                             item_return_number=items.item_return_number,
                                             supplier_name=sup.supplier_name,
                                             quantity = inVetD.quantity,
                                             warehouse_name =ware.warehouse_name,
                                         }).ToList();
                            foreach (var item in itemP)
                            {
                                dtr = tb.NewRow();
                                dtr["created_date"] = Convert.ToDateTime(items.created_date).ToString("dd-MMM-yyyy");
                                //dtr["item_return_id"] = item.item_return_id;
                                dtr["warehouse_name"] = item.warehouse_name;
                                dtr["item_return_number"] = item.item_return_number;
                                dtr["supplier_name"] = item.supplier_name;
                                dtr["quantity"] = item.quantity;
                                dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                tb.Rows.Add(dtr);
                            }
                            */

                        }
                    }
                    else
                    {
                        dtr = tb.NewRow();
                        dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("ReturnItemtoSupplier", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }
        }
        public static void GeneratePurchaseOrderVsItemReceivedReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status)
        {
            try
            {
                //List<Models.PurchaseOrderVsItemReceivedViewModel> poVsIR = new List<Models.PurchaseOrderVsItemReceivedViewModel>();
                //List<Models.PurchaseOrderVsItemReceivedViewModel> poVsIR2 = new List<Models.PurchaseOrderVsItemReceivedViewModel>();
                //List<Models.PurchaseOrderVsItemReceivedViewModel> poVsIR3 = new List<Models.PurchaseOrderVsItemReceivedViewModel>();
                List<Models.PurchaseOrderViewModel> poVsIR = new List<Models.PurchaseOrderViewModel>();
                List<Models.PurchaseOrderDetailViewModel> poItems = new List<Models.PurchaseOrderDetailViewModel>();
                List<Models.ItemReceiveViewModel> itemReceives = new List<Models.ItemReceiveViewModel>();
                List<Models.ItemReceivedDetailViewModel> receivedItemDetails = new List<Models.ItemReceivedDetailViewModel>();
                List<Models.PurchaseOrderVsItemReceivedReport> POReceived1 = new List<Models.PurchaseOrderVsItemReceivedReport>();
                List<Models.PurchaseOrderVsItemReceivedReport> POReceived2 = new List<Models.PurchaseOrderVsItemReceivedReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "po_supplier_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "purchaseOrder_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "po_detail_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ref_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ri_item_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "created_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "purchase_order_number";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "po_quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "item_received";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ri_detail_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "different_quatity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "dateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "dateTo";
                    tb.Columns.Add(col);

                    //poVsIR = (from po in db.tb_purchase_order
                    //          where po.status ==true && (po.purchase_order_status == "Completed"||string.Compare(po.purchase_order_status,"Approved")==0)&& po.created_date >= dateFrom && po.created_date <= dateTo
                    //          orderby po.created_date
                    //          select new Models.PurchaseOrderViewModel()
                    //          {
                    //              purchase_order_id=po.purchase_order_id,
                    //              purchase_oder_number=po.purchase_oder_number,
                    //              purchase_order_status=po.purchase_order_status,
                    //              created_date=po.created_date,
                    //          }).ToList();
                    //new process May 11 2020
                    poVsIR = (from po in db.tb_purchase_request
                              where po.status == true && (po.purchase_request_status == Status.Completed || string.Compare(po.purchase_request_status, Status.Approved) == 0) && po.created_date >= dateFrom && po.created_date <= dateTo
                              orderby po.created_date
                              select new Models.PurchaseOrderViewModel()
                              {
                                  purchase_order_id = po.pruchase_request_id,
                                  purchase_oder_number = po.purchase_request_number,
                                  purchase_order_status = po.purchase_request_status,
                                  created_date = po.created_date,
                                  item_request_id = po.purchase_order_id
                              }).ToList();
                    if (poVsIR.Any())
                    {
                        foreach (var item in poVsIR)
                        {
                            //get po item
                            if (string.Compare(item.purchase_order_status, Status.Completed) == 0)
                                poItems = (from ReCItemD in db.tb_purchase_order_detail
                                           join pro in db.tb_product on ReCItemD.item_id equals pro.product_id
                                           where ReCItemD.purchase_order_id == item.item_request_id && ReCItemD.item_status == "approved"
                                           select new Models.PurchaseOrderDetailViewModel()
                                           {
                                               item_id = ReCItemD.item_id,
                                               product_code = pro.product_code,
                                               product_name = pro.product_name,
                                               unit = pro.product_unit,
                                               po_quantity = ReCItemD.po_quantity,
                                           }).ToList();
                            else
                                poItems = (from ReCItemD in db.tb_purchase_order_detail
                                           join pro in db.tb_product on ReCItemD.item_id equals pro.product_id
                                           where ReCItemD.purchase_order_id == item.item_request_id && ReCItemD.item_status == "pending"
                                           select new Models.PurchaseOrderDetailViewModel()
                                           {
                                               item_id = ReCItemD.item_id,
                                               product_code = pro.product_code,
                                               product_name = pro.product_name,
                                               unit = pro.product_unit,
                                               po_quantity = ReCItemD.po_quantity,
                                           }).ToList();

                            //get item receive
                            itemReceives = (from irs in db.tb_receive_item_voucher
                                            where irs.ref_id == item.purchase_order_id && irs.status == true && irs.received_status == Status.Completed
                                            select new Models.ItemReceiveViewModel()
                                            {
                                                receive_item_voucher_id = irs.receive_item_voucher_id,
                                                received_number = irs.received_number,
                                                ref_id = item.purchase_order_id
                                            }).ToList();

                            if (itemReceives.Any())
                            {
                                foreach (var ir in itemReceives)
                                {
                                    receivedItemDetails = (from irsD in db.tb_received_item_detail
                                                           where irsD.ri_ref_id == ir.receive_item_voucher_id
                                                           select new Models.ItemReceivedDetailViewModel()
                                                           {
                                                               ri_ref_id = ir.receive_item_voucher_id,
                                                               ri_item_id = irsD.ri_item_id,
                                                               quantity = irsD.quantity,
                                                           }).ToList();

                                    foreach(var poItem in poItems)
                                    {
                                        foreach (var receivedItemDetail in receivedItemDetails)
                                        {
                                            if (receivedItemDetail.ri_item_id == poItem.item_id && ir.ref_id == item.purchase_order_id)
                                            {
                                                POReceived1.Add(new Models.PurchaseOrderVsItemReceivedReport()
                                                {
                                                    dateFrom = dateFrom,
                                                    dateTo = dateTo,
                                                    created_date = item.created_date,
                                                    purchase_order_id = item.purchase_order_id,
                                                    purchase_order_number = item.purchase_oder_number,
                                                    item_id = poItem.item_id,
                                                    item_code = poItem.product_code,
                                                    item_name = poItem.product_name,
                                                    unit = poItem.unit,
                                                    po_quantity = poItem.po_quantity,
                                                    received_quantity = receivedItemDetail.quantity,
                                                });
                                            }
                                            //added new May 18 2022
                                            else
                                            {
                                                POReceived1.Add(new Models.PurchaseOrderVsItemReceivedReport()
                                                {
                                                    dateFrom = dateFrom,
                                                    dateTo = dateTo,
                                                    created_date = item.created_date,
                                                    purchase_order_id = item.purchase_order_id,
                                                    purchase_order_number = item.purchase_oder_number,
                                                    item_id = poItem.item_id,
                                                    item_code = poItem.product_code,
                                                    item_name = poItem.product_name,
                                                    unit = poItem.unit,
                                                    po_quantity = poItem.po_quantity,
                                                    received_quantity = 0,
                                                });
                                            }
                                        }
                                    }

                                    //foreach (var receivedItemDetail in receivedItemDetails)
                                    //{
                                    //    foreach (var poItem in poItems)
                                    //    {
                                    //        if (receivedItemDetail.ri_item_id == poItem.item_id && ir.ref_id == item.purchase_order_id)
                                    //        {
                                    //            POReceived1.Add(new Models.PurchaseOrderVsItemReceivedReport()
                                    //            {
                                    //                dateFrom = dateFrom,
                                    //                dateTo = dateTo,
                                    //                created_date = item.created_date,
                                    //                purchase_order_id = item.purchase_order_id,
                                    //                purchase_order_number = item.purchase_oder_number,
                                    //                item_id = poItem.item_id,
                                    //                item_code = poItem.product_code,
                                    //                item_name = poItem.product_name,
                                    //                unit = poItem.unit,
                                    //                po_quantity = poItem.po_quantity,
                                    //                received_quantity = receivedItemDetail.quantity,
                                    //            });
                                    //        }
                                            
                                    //    }
                                        
                                    //}
                                }
                            }
                            //po don't have received item
                            else
                            {
                                foreach (var poItem in poItems)
                                {
                                    POReceived1.Add(new Models.PurchaseOrderVsItemReceivedReport()
                                    {
                                        dateFrom = dateFrom,
                                        dateTo = dateTo,
                                        created_date = item.created_date,
                                        purchase_order_id = item.purchase_order_id,
                                        purchase_order_number = item.purchase_oder_number,
                                        item_id = poItem.item_id,
                                        item_code = poItem.product_code,
                                        item_name = poItem.product_name,
                                        unit = poItem.unit,
                                        po_quantity = poItem.po_quantity,
                                        received_quantity = 0,
                                    });
                                }
                            }
                        }

                    }
                    #region by borith nget
                    decimal totalReceivedQty = 0;
                    var dupItems = POReceived1.GroupBy(x => new { x.purchase_order_id, x.item_id }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupItems.Any())
                    {
                        foreach (var poRecievedItem in dupItems)
                        {
                            totalReceivedQty = 0;
                            var receivedQtys = POReceived1.Where(x => x.purchase_order_id == poRecievedItem.purchase_order_id && x.item_id == poRecievedItem.item_id).Select(x => x.received_quantity).ToList();
                            foreach (var receivedQty in receivedQtys)
                            {
                                totalReceivedQty = totalReceivedQty + Convert.ToDecimal(receivedQty);
                            }
                            var item = POReceived1.Where(x => x.purchase_order_id == poRecievedItem.purchase_order_id && x.item_id == poRecievedItem.item_id).FirstOrDefault();
                            POReceived2.Add(new Models.PurchaseOrderVsItemReceivedReport()
                            {
                                dateFrom = item.dateFrom,
                                dateTo = item.dateTo,
                                created_date = item.created_date,
                                purchase_order_id = item.purchase_order_id,
                                purchase_order_number = item.purchase_order_number,
                                item_id = item.item_id,
                                item_code = item.item_code,
                                item_name = item.item_name,
                                unit = item.unit,
                                po_quantity = item.po_quantity,
                                received_quantity = totalReceivedQty,
                            });
                        }
                        foreach (var item in POReceived1)
                        {
                            foreach (var itemId in dupItems)
                            {
                                if (item.purchase_order_id == itemId.purchase_order_id && item.item_id == itemId.item_id)
                                    break;
                                else
                                {
                                    POReceived2.Add(new Models.PurchaseOrderVsItemReceivedReport()
                                    {
                                        dateFrom = item.dateFrom,
                                        dateTo = item.dateTo,
                                        created_date = item.created_date,
                                        purchase_order_id = item.purchase_order_id,
                                        purchase_order_number = item.purchase_order_number,
                                        item_id = item.item_id,
                                        item_code = item.item_code,
                                        item_name = item.item_name,
                                        unit = item.unit,
                                        po_quantity = item.po_quantity,
                                        received_quantity = item.received_quantity,
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        POReceived2 = POReceived1;
                    }
                    #endregion
                    POReceived2 = POReceived2.OrderBy(x => x.purchase_order_number).ThenBy(x => x.item_code).ToList();

                    foreach (var item in POReceived2)
                    {
                        dtr = tb.NewRow();
                        dtr["created_date"] = Convert.ToDateTime(item.created_date).ToString("dd-MMM-yyyy");
                        dtr["purchase_order_number"] = item.purchase_order_number;
                        dtr["item_code"] = item.item_code;
                        dtr["item_name"] = item.item_name;
                        dtr["unit"] = db.tb_unit.Find(item.unit).Name;
                        dtr["po_quantity"] = Convert.ToDouble(item.po_quantity);
                        dtr["item_received"] = Convert.ToDouble(item.received_quantity);
                        dtr["different_quatity"] = Convert.ToDouble(item.po_quantity) - Convert.ToDouble(item.received_quantity);
                        dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }

                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("PurchaseOrderVsItemReceived", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }
        }

        //public static void WorkOrderIsuseVSWorkOrderReturnReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status, string project)
        //{
        //    try
        //    {
        //        List<Models.WorkOrderIsuseVSWorkOrderReturnReport> objs = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();

        //        DataTable tb = new DataTable();
        //        DataRow dtr;
        //        using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
        //        {
        //            DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
        //            DataColumn col = new DataColumn();
        //            col.ColumnName = "item_id";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "datefrom";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "todate";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "project_name";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_no";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_item_code";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_description";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_unit";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_qty";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_no";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_item_code";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_description";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_unit";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_qty";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "inventory_type";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();


        //            //Added by Terd May 11 2020
        //            if (string.IsNullOrEmpty(project))
        //            {
        //                objs = (from woi in db.tb_stock_issue
        //                        join pro in db.tb_project on woi.project_id equals pro.project_id
        //                        where string.Compare(woi.stock_issue_status, Status.Completed) == 0 && woi.status == true && woi.created_date >= dateFrom && woi.created_date <= dateTo
        //                        select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                        {
        //                            issue_id = woi.stock_issue_id,
        //                            issue_no = woi.stock_issue_number,
        //                            project_id = woi.project_id,
        //                            project_name = "All",
        //                        }).ToList();
        //            }
        //            else
        //            {
        //                objs = (from woi in db.tb_stock_issue
        //                        join pro in db.tb_project on woi.project_id equals pro.project_id
        //                        where woi.status == true && string.Compare(woi.project_id, project) == 0 && string.Compare(woi.stock_issue_status, Status.Completed) == 0 && woi.created_date >= dateFrom && woi.created_date <= dateTo
        //                        select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                        {
        //                            issue_id = woi.stock_issue_id,
        //                            issue_no = woi.stock_issue_number,
        //                            project_id = woi.project_id,
        //                            project_name = pro.project_full_name,
        //                        }).ToList();
        //            }

        //            if (objs.Any())
        //            {
        //                foreach (var obj in objs)
        //                {
        //                    string issueReturnNUmber = string.Empty;
        //                    List<Models.InventoryDetailViewModel> returnItems = new List<Models.InventoryDetailViewModel>();
        //                    var issueReturns = db.tb_workorder_returned.Where(s => s.status == true && string.Compare(s.workorder_issued_id, obj.issue_id) == 0).ToList();
        //                    foreach (var issReturn in issueReturns)
        //                    {
        //                        issueReturnNUmber = string.Format("{0}{1},", issueReturnNUmber, issReturn.workorder_returned_number);
        //                        var oobjs = db.tb_inventory_detail.Where(s => string.Compare(s.inventory_ref_id, issReturn.workorder_returned_id) == 0).Select(s => new Models.InventoryDetailViewModel()
        //                        {
        //                            inventory_item_id = s.inventory_item_id,
        //                            quantity = s.quantity,
        //                            unit = s.unit,
        //                        }).ToList();
        //                        foreach (var oobj in oobjs)
        //                            returnItems.Add(oobj);
        //                    }

        //                    var issueItems = (from inv in db.tb_inventory
        //                                      join pro in db.tb_product on inv.product_id equals pro.product_id
        //                                      join un in db.tb_unit on pro.product_unit equals un.Id
        //                                      where string.Compare(inv.ref_id, obj.issue_id) == 0
        //                                      select new { inv, pro, un }).ToList();
        //                    foreach (var issitem in issueItems)
        //                    {
        //                        dtr = tb.NewRow();
        //                        dtr["item_id"] = issitem.inv.product_id;
        //                        dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                        dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                        dtr["project_name"] = obj.project_name;
        //                        dtr["issue_no"] = obj.issue_no;
        //                        dtr["issue_item_code"] = issitem.pro.product_code;
        //                        dtr["issue_description"] = issitem.pro.product_name;
        //                        dtr["issue_unit"] = issitem.un.Name;
        //                        dtr["issue_qty"] = issitem.inv.out_quantity;
        //                        dtr["return_no"] = issueReturnNUmber;

        //                        dtr["return_qty"] = returnItems.Where(s => string.Compare(issitem.inv.product_id, s.inventory_item_id) == 0).Sum(s => s.quantity);

        //                        //if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                        //{
        //                        // dtr["return_no"] = re.return_no;
        //                        // // dtr["return_qty"] = item.Number[i].return_qty;
        //                        // dtr["return_unit"] = item.return_unit;
        //                        // dtr["return_qty"] = item.return_qty;
        //                        //}
        //                        //else
        //                        //{
        //                        // dtr["return_no"] = "";
        //                        // dtr["return_qty"] = "";
        //                        //}
        //                        tb.Rows.Add(dtr);
        //                    }
        //                }
        //            }

        //            rv.LocalReport.ReportPath = reportPath;
        //            rv.LocalReport.DataSources.Clear();
        //            ReportDataSource rdc = new ReportDataSource("WorkOrderIsuseVSWorkOrderReturn", tb);
        //            rv.LocalReport.DataSources.Add(rdc);
        //            rv.LocalReport.Refresh();
        //        }
        //    }
        //    catch (Exception ex) { }
        //}
        //        public static void WorkOrderIsuseVSWorkOrderReturnReportCopy(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status, string project)
        //        {
        //            try
        //            {
        //                List<Models.WorkOrderIsuseVSWorkOrderReturnReport> objs = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();

        //                DataTable tb = new DataTable();
        //                DataRow dtr;
        //                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
        //                {
        //                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
        //                    DataColumn col = new DataColumn();
        //                    col.ColumnName = "item_id";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "datefrom";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "todate";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "project_name";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "issue_no";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "issue_item_code";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "issue_description";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "issue_unit";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "issue_qty";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "return_no";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "return_item_code";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "return_description";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "return_unit";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "return_qty";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();
        //                    col.ColumnName = "inventory_type";
        //                    tb.Columns.Add(col);
        //                    col = new DataColumn();


        //                    objs = (from woi in db.tb_stock_issue
        //                            join pro in db.tb_project on woi.project_id equals pro.project_id
        //                            where woi.project_id == project
        //                            select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                            ).ToList();



        //                    if (string.Compare(project, "") == 0)
        //                    {
        //                        objs = (from woi in db.tb_stock_issue
        //                                join wor in db.tb_workorder_returned on woi.stock_issue_id equals wor.workorder_issued_id
        //                                where woi.stock_issue_status == "completed" && woi.status == true
        //                                select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                {
        //                                    project_name = "All",
        //                                    issue_no = woi.stock_issue_number,
        //                                    return_no = wor.workorder_returned_number,
        //                                    status_i = wor.workorder_returned_status,
        //                                    wrs = wor.status.ToString(),
        //                                    issue_id = woi.stock_issue_id,
        //                                    return_id = wor.workorder_returned_id,


        //                                }
        //                            ).ToList();
        //                        foreach (var re in objs)
        //                        {





        //                            var itemsR = (from inv in db.tb_inventory
        //                                          join pro in db.tb_product on inv.product_id equals pro.product_id
        //                                          join invD in db.tb_inventory_detail on inv.product_id equals invD.inventory_item_id
        //                                          join unit in db.tb_unit on pro.product_unit equals unit.Id
        //                                          //where inv.ref_id == re.issue_id && invD.inventory_ref_id == re.issue_id
        //                                          where inv.ref_id == re.issue_id && invD.inventory_type == "5" && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)
        //                                          //where inv.ref_id == re.issue_id && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)

        //                                          select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                          {
        //                                              issue_item_code = pro.product_code,
        //                                              issue_description = pro.product_name,
        //                                              issue_unit = unit.Name,
        //                                              issue_qty = invD.quantity.ToString(),
        //                                              return_qty = invD.quantity.ToString(),
        //                                              // return_unit = invD.inventory_type,
        //                                              //inventory_type = invD.inventory_type,
        //                                          }).ToList();

        //                            var items = (from inv in db.tb_inventory
        //                                         join pro in db.tb_product on inv.product_id equals pro.product_id
        //                                         join invD in db.tb_inventory_detail on inv.product_id equals invD.inventory_item_id
        //                                         join unit in db.tb_unit on pro.product_unit equals unit.Id
        //                                         //where inv.ref_id == re.issue_id && invD.inventory_ref_id == re.issue_id
        //                                         //where inv.ref_id == re.issue_id && invD.inventory_type == "2" && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)
        //                                         where inv.ref_id == re.issue_id && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)

        //                                         select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                         {
        //                                             issue_item_code = pro.product_code,
        //                                             issue_description = pro.product_name,
        //                                             issue_unit = unit.Name,
        //                                             issue_qty = invD.quantity.ToString(),
        //                                             return_qty = invD.quantity.ToString(),
        //                                             //return_unit = invD.inventory_type,
        //                                             //inventory_type = invD.inventory_type,

        //                                         }).ToList();


        //                            var numbers = new[] { itemsR };
        //                            var words = new[] { items };


        //                            //var numbers = new[] { 1, 2, 3, 4 };
        //                            //var words = new[] { "one", "two", "three", "four" };

        //                            //var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
        //                            //foreach (var nw in numbersAndWords)
        //                            //{
        //                            //    Console.WriteLine(nw.Number + nw.Word);
        //                            //}


        //                            //var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
        //                            //var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
        //                            foreach (var item in itemsR)
        //                            {
        //                                dtr = tb.NewRow();
        //                                dtr["item_id"] = "";
        //                                dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                                dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                                dtr["project_name"] = re.project_name;
        //                                dtr["issue_no"] = re.issue_no;
        //                                dtr["issue_item_code"] = item.issue_item_code;
        //                                dtr["issue_description"] = item.issue_description;
        //                                dtr["issue_unit"] = item.issue_unit;
        //                                dtr["issue_qty"] = item.issue_qty;
        //                                if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                                {
        //                                    dtr["return_no"] = re.return_no;
        //                                    // dtr["return_qty"] = item.Number[i].return_qty;
        //                                    // dtr["return_unit"] = item.return_unit;
        //                                    //dtr["return_qty"] = item.Number[0].return_qty;
        //                                }
        //                                else
        //                                {
        //                                    dtr["return_no"] = "";
        //                                    dtr["return_qty"] = "";
        //                                }
        //                                tb.Rows.Add(dtr);


        //                            }


        //                            //var numbers = new[] { itemsR };
        //                            //var words = new[] { items };
        //                            ////var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
        //                            //var numbersAndWords = numbers.Zip(words, (n, w) => new { Number = n, Word = w });
        //                            //foreach (var item in numbersAndWords)
        //                            //{



        //                            //    for (var i = 0; i < item.Word.Count ; i++)
        //                            //    {
        //                            //        //for (var ii = 0; ii < item.Number.Count; ii++)
        //                            //        //{
        //                            //        //}
        //                            //        dtr = tb.NewRow();
        //                            //        dtr["item_id"] = "";
        //                            //        dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                            //        dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                            //        dtr["project_name"] = re.project_name;
        //                            //        dtr["issue_no"] = re.issue_no;


        //                            //        dtr["issue_item_code"] = item.Word[i].issue_item_code;
        //                            //        dtr["issue_description"] = item.Word[i].issue_description;
        //                            //        dtr["issue_unit"] = item.Word[i].issue_unit;
        //                            //        dtr["issue_qty"] = item.Word[i].issue_qty;


        //                            //        if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                            //        {
        //                            //            dtr["return_no"] = re.return_no;
        //                            //            for (var ii = 0; ii < item.Number.Count; ii++)
        //                            //            {

        //                            //                dtr["return_qty"] = item.Number[ii].return_qty;

        //                            //            }
        //                            //            // dtr["return_qty"] = item.Number[i].return_qty;
        //                            //            // dtr["return_unit"] = item.return_unit;
        //                            //            //dtr["return_qty"] = item.Number[0].return_qty;
        //                            //        }
        //                            //        else
        //                            //        {
        //                            //            dtr["return_no"] = "";
        //                            //            dtr["return_qty"] = "";
        //                            //        }
        //                            //        tb.Rows.Add(dtr);
        //                            //    }
        //                            //}
        //                        }
        //                    }
        //                    else
        //                    {
        //                        objs = (from proj in db.tb_project
        //                                join woi in db.tb_stock_issue on proj.project_id equals woi.project_id
        //                                join wor in db.tb_workorder_returned on woi.stock_issue_id equals wor.workorder_issued_id

        //                                where proj.project_id == project && woi.stock_issue_status == "completed" && woi.status == true
        //                                select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                {
        //                                    project_name = proj.project_full_name,
        //                                    issue_no = woi.stock_issue_number,
        //                                    return_no = wor.workorder_returned_number,
        //                                    status_i = wor.workorder_returned_status,
        //                                    wrs = wor.status.ToString(),

        //                                }
        //                            ).ToList();
        //                        foreach (var re in objs)
        //                        {

        //                            dtr = tb.NewRow();
        //                            dtr["item_id"] = "";
        //                            dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                            dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                            dtr["project_name"] = re.project_name;
        //                            dtr["issue_no"] = re.issue_no;
        //                            dtr["issue_item_code"] = "";
        //                            dtr["issue_description"] = "";
        //                            dtr["issue_unit"] = "";
        //                            dtr["issue_qty"] = "";
        //                            if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                            {
        //                                dtr["return_no"] = re.return_no;
        //                                dtr["return_item_code"] = "";
        //                                dtr["return_description"] = "";
        //                                dtr["return_unit"] = "";
        //                                dtr["return_qty"] = "";

        //                            }
        //                            else
        //                            {
        //                                dtr["return_no"] = "";
        //                                dtr["return_item_code"] = "";
        //                                dtr["return_description"] = "";
        //                                dtr["return_unit"] = "";
        //                                dtr["return_qty"] = "";

        //                            }

        //                            tb.Rows.Add(dtr);


        //                        }
        //                    }







        //                    rv.LocalReport.ReportPath = reportPath;
        //                    rv.LocalReport.DataSources.Clear();
        //                    ReportDataSource rdc = new ReportDataSource("WorkOrderIsuseVSWorkOrderReturn", tb);
        //                    rv.LocalReport.DataSources.Add(rdc);
        //                    rv.LocalReport.Refresh();
        //                }
        //            }
        //            catch (Exception ex) { }
        //        }

        public static void GenerateStockUsageBySiteWithRemainBalanceReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string site)
        {
            try
            {
                List<Models.StockUsageBySiteWithRemainBalanceViewModel> StockUSRB = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();
                List<Models.StockUsageBySiteWithRemainBalanceViewModel> StockUSRB2 = new List<Models.StockUsageBySiteWithRemainBalanceViewModel>();

                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "Site_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Site_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Create_Date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Status";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Project_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Project_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Site_manager_proect_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_ref_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Quatity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Product_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Product_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Product_unit";
                    tb.Columns.Add(col);

                    col = new DataColumn();
                    col.ColumnName = "product_code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "Stock_adjustment_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_issue_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_issue_number";
                    tb.Columns.Add(col);

                    col = new DataColumn();
                    col.ColumnName = "dateFrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "dateTo";
                    tb.Columns.Add(col);

                    StockUSRB = (from stockIss in db.tb_stock_issue
                                     //join stockD in db.tb_stock_transfer_detail on stockIss.stock_issue_id equals stockD.st_ref_id
                                     //join invet in db.tb_inventory on stockIss.stock_issue_id equals invet.ref_id
                                     //join ware in db.tb_warehouse on invetD.inventory_warehouse_id equals ware.warehouse_id
                                     //join sitess in db.tb_site on ware.warehouse_site_id equals sitess.site_id
                                 where stockIss.status == true && stockIss.stock_issue_status == "Completed" && stockIss.created_date >= dateFrom && stockIss.created_date <= dateTo
                                 orderby stockIss.created_date

                                 select new Models.StockUsageBySiteWithRemainBalanceViewModel()
                                 {

                                     created_date = stockIss.created_date,
                                     stock_issue_id = stockIss.stock_issue_id,
                                     stock_issue_number = stockIss.stock_issue_number,


                                 }
                               ).ToList();


                    if (StockUSRB.Any())
                    {

                        int count = 0;

                        foreach (var items in StockUSRB)
                        {

                            var issueDetails = (from stocD in db.tb_inventory_detail
                                                join wh in db.tb_warehouse on stocD.inventory_warehouse_id equals wh.warehouse_id
                                                join st in db.tb_site on wh.warehouse_site_id equals st.site_id


                                                //join inV in db.tb_inventory on stocD.inventory_detail_id equals inV.inventory_id

                                                join prod in db.tb_product on stocD.inventory_item_id equals prod.product_id
                                                where stocD.inventory_ref_id == items.stock_issue_id && st.site_id == site
                                                //where st.site_id ==site && st.status==true
                                                select new Models.StockUsageBySiteWithRemainBalanceViewModel
                                                {
                                                    inventory_ref_id = stocD.inventory_ref_id,
                                                    site_id = st.site_id,
                                                    Site_name = st.site_name,
                                                    Product_Name = prod.product_name,
                                                    product_code = prod.product_code,
                                                    Product_Unit = prod.product_unit,
                                                    Quatity = stocD.quantity
                                                }).ToList();


                            if (issueDetails.Any())
                            {

                                foreach (var answer in issueDetails)
                                {
                                    dtr = tb.NewRow();
                                    dtr["Create_Date"] = Convert.ToDateTime(items.created_date).ToString("dd-MMM-yyyy");
                                    dtr["stock_issue_number"] = items.stock_issue_number;
                                    dtr["Site_name"] = answer.Site_name;
                                    //dtr["Project_name"] = items.Project_name;
                                    dtr["Quatity"] = answer.Quatity;
                                    dtr["Product_name"] = answer.Product_Name;
                                    dtr["product_code"] = answer.product_code;
                                    dtr["Product_Unit"] = answer.Product_Unit;
                                    dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                    dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                    tb.Rows.Add(dtr);
                                    count++;
                                }
                            }

                            ////if (issueDetails.Any())
                            ////{
                            ////    foreach(var si in issueDetails)
                            ////    {
                            ////        var ste = (from st in db.tb_site
                            ////                   where st.status==true && st.site_id==site

                            ////                   select st
                            ////                   ).FirstOrDefault();
                            ////        if(ste != null)
                            ////        {
                            ////            count++;
                            ////        }

                            ////    }


                            ////}
                            //foreach (var answer in issueDetails)
                            //{

                            //    dtr = tb.NewRow();
                            //    dtr["Create_Date"] = Convert.ToDateTime(items.created_date).ToString("dd-MMM-yyyy");
                            //    dtr["stock_issue_number"] = items.stock_issue_number;
                            //    dtr["Site_name"] = answer.Site_name;
                            //    //dtr["Project_name"] = items.Project_name;
                            //    dtr["Quatity"] = answer.Quatity;
                            //    dtr["Product_name"] = answer.Product_Name;
                            //    dtr["product_code"] = answer.product_code;
                            //    dtr["Product_Unit"] = answer.Product_Unit;
                            //    dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                            //    dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                            //    tb.Rows.Add(dtr);


                            //}

                            ////if (count == 0)
                            ////{
                            ////    dtr = tb.NewRow();
                            ////    dtr["ReportSupplier"] = db.tb_site.Where(x => x.site_id == site).Select(x => x.site_id).FirstOrDefault();
                            ////    dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                            ////    dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                            ////    tb.Rows.Add(dtr);
                            ////}




                        }
                        if (count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["Site_name"] = db.tb_site.Where(x => x.site_id == site).Select(x => x.site_name).FirstOrDefault();
                            dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                            dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                            tb.Rows.Add(dtr);
                        }

                    }

                    else
                    {
                        dtr = tb.NewRow();
                        dtr["Site_name"] = db.tb_site.Where(x => x.site_id == site).Select(x => x.site_name).FirstOrDefault();
                        dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                        tb.Rows.Add(dtr);
                    }


                }
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockUsageBySiteWithRemainBalance", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();
            }
            catch (Exception ex)
            {

            }
        }
        //public static void GenerateReturnItemtoSupplierReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status)
        //{
        //    try
        //    {
        //        List<Models.ReturnItemtoSupplierViewModel> ReItoSub = new List<Models.ReturnItemtoSupplierViewModel>();
        //        DataTable tb = new DataTable();
        //        DataRow dtr;
        //        using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
        //        {
        //            DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
        //            DataColumn col = new DataColumn();
        //            col.ColumnName = "created_date";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "item_return_number";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "supplier_name";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "supplier_phone";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "discount";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "DateFrom";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "DateTo";
        //            tb.Columns.Add(col);

        //            ReItoSub = (from ReItoSubs in db.tb_item_return
        //                        join inVet in db.tb_inventory on ReItoSubs.item_return_id equals inVet.ref_id
        //                        where ReItoSubs.status == true && ReItoSubs.item_return_status == "Completed" && ReItoSubs.created_date >= dateFrom && ReItoSubs.created_date <= dateTo

        //                        select new Models.ReturnItemtoSupplierViewModel()
        //                        {
        //                            item_return_number = ReItoSubs.item_return_number,
        //                            //created_date=ReItoSubs.created_date,

        //                        }
        //                      ).ToList();
        //            if (ReItoSub.Any())
        //            {
        //                foreach (var items in ReItoSub)
        //                {
        //                    var itemP = (from inVet in db.tb_inventory
        //                                 join inVetD in db.tb_inventory_detail on inVet.inventory_id equals inVetD.inventory_detail_id
        //                                 join sup in db.tb_supplier on inVetD.supplier_id equals sup.supplier_id

        //                                 where inVet.ref_id == items.item_return_number
        //                                 select new Models.ReturnItemtoSupplierViewModel()
        //                                 {
        //                                     supplier_name = sup.supplier_name,
        //                                     supplier_phone = sup.supplier_phone,
        //                                     created_date = items.created_date,

        //                                 }
        //                               ).ToList();
        //                    foreach (var item in itemP)
        //                    {
        //                        dtr = tb.NewRow();
        //                        dtr["created_date"] = item.created_date;
        //                        dtr["item_return_number"] = item.item_return_number;
        //                        dtr["supplier_name"] = item.supplier_name;
        //                        dtr["supplier_phone"] = item.supplier_phone;
        //                        dtr["discount"] = item.discount;

        //                        dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                        dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                        tb.Rows.Add(dtr);
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                dtr = tb.NewRow();
        //                dtr["dateFrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                dtr["dateTo"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                tb.Rows.Add(dtr);
        //            }



        //        }
        //        rv.LocalReport.ReportPath = reportPath;
        //        rv.LocalReport.DataSources.Clear();
        //        ReportDataSource rdc = new ReportDataSource("ReturnItemtoSupplier", tb);
        //        rv.LocalReport.DataSources.Add(rdc);
        //        rv.LocalReport.Refresh();
        //    }
        //    catch (Exception ex) { }
        ////}
        public static void GenerateStockBalanceByWarehouseReport(ReportViewer rv, string reportPath, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceBywarehouseReport> objs = new List<Models.StockBalanceBywarehouseReport>();
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DataTable tb = new DataTable();
                    DataRow dtr;
                    DataColumn col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemTypeName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_quantity";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total";
                    tb.Columns.Add(col);

                    IQueryable<Models.WareHouseViewModel> warehouses;
                    List<Models.InventoryViewModel> orginalItems = new List<Models.InventoryViewModel>();
                    List<Models.InventoryViewModel> stockBalanceItems = new List<Models.InventoryViewModel>();
                    if (string.IsNullOrEmpty(WarehouseId))
                        warehouses = db.tb_inventory.OrderBy(x => x.warehouse_id).Select(x => new Models.WareHouseViewModel() { warehouse_id = x.warehouse_id }).Distinct();
                    else
                        warehouses = db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, WarehouseId) == 0).Select(x => new Models.WareHouseViewModel() { warehouse_id = x.warehouse_id });
                    if (warehouses.Any())
                    {
                        foreach (var warehouse in warehouses)
                        {
                            List<Models.InventoryViewModel> items = new List<Models.InventoryViewModel>();
                            List<Models.InventoryViewModel> items1 = new List<Models.InventoryViewModel>();
                            items1 = db.tb_inventory.OrderBy(x => x.product_id).ThenByDescending(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, warehouse.warehouse_id) == 0).Select(x => new Models.InventoryViewModel()
                            {
                                inventory_date = x.inventory_date,
                                product_id = x.product_id,
                                warehouse_id = x.warehouse_id,
                                total_quantity = x.total_quantity,
                            }).ToList();
                            var dup = items1.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (dup.Any())
                            {
                                foreach (var ditem in dup)
                                {
                                    Models.InventoryViewModel item = new Models.InventoryViewModel();
                                    item = items1.Where(x => string.Compare(x.product_id, ditem) == 0).First();
                                    items.Add(item);
                                }
                                foreach (var item in items1)
                                {
                                    var checkItem = dup.Where(x => string.Compare(x, item.product_id) == 0).ToList();
                                    if (checkItem.Any()) { }
                                    else
                                        items.Add(new Models.InventoryViewModel() { inventory_date = item.inventory_date, product_id = item.product_id, warehouse_id = item.warehouse_id, total_quantity = item.total_quantity });
                                }
                            }
                            else
                                items = items1;

                            if (items.Any())
                            {
                                foreach (var item in items)
                                {
                                    orginalItems.Add(new Models.InventoryViewModel() { inventory_date = item.inventory_date, product_id = item.product_id, total_quantity = item.total_quantity, warehouse_id = item.warehouse_id });
                                }
                            }
                        }
                    }
                    var duplicationItems = orginalItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (duplicationItems.Any())
                    {
                        foreach (var dItem in duplicationItems)
                        {
                            decimal totalQuantity = 0;
                            var items = orginalItems.Where(x => string.Compare(x.product_id, dItem) == 0).Select(x => x.total_quantity).ToList();
                            foreach (var quantiy in items)
                                totalQuantity = totalQuantity + Convert.ToDecimal(quantiy);
                            foreach (var item in orginalItems)
                            {
                                Models.InventoryViewModel iitem = new Models.InventoryViewModel();
                                iitem.inventory_date = item.inventory_date;
                                iitem.product_id = item.product_id;
                                iitem.itemCode = CommonClass.GetProductDetail(iitem.product_id).product_code;
                                iitem.warehouse_id = item.warehouse_id;
                                if (string.Compare(item.product_id, dItem) == 0)
                                {
                                    iitem.total_quantity = totalQuantity;
                                }
                                else
                                {
                                    iitem.total_quantity = item.total_quantity;
                                }
                                stockBalanceItems.Add(iitem);
                            }
                        }
                    }
                    else
                        stockBalanceItems = orginalItems;
                    string warehouseName = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => string.Compare(x.warehouse_id, WarehouseId) == 0).Select(x => x.warehouse_name).FirstOrDefault().ToString();

                    List<Models.InventoryViewModel> inventories = new List<Models.InventoryViewModel>();
                    inventories = stockBalanceItems.OrderBy(x => x.itemCode).ToList();
                    foreach (var item in inventories)
                    {
                        dtr = tb.NewRow();
                        Models.ProductViewModel product = new Models.ProductViewModel();
                        product = CommonClass.GetProductDetail(item.product_id);
                        dtr["itemCode"] = product.product_code;
                        dtr["itemName"] = product.product_name;
                        dtr["itemUnit"] = product.product_unit;
                        dtr["itemTypeName"] = product.p_category_name;
                        dtr["unit_price"] = product.unit_price;
                        dtr["warehouseName"] = warehouseName;
                        dtr["total_quantity"] = item.total_quantity;
                        //dtr["total"] = item.total;
                        tb.Rows.Add(dtr);
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockBalanceBywarehouse", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();

                    /*
                    var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    foreach (var obj in objss)
                    {
                        var jobs = (from inVet in db.tb_inventory
                                    join ware in db.tb_warehouse on inVet.warehouse_id equals ware.warehouse_id
                                    where ware.warehouse_status == true
                                    select new Models.StockMovementWarehouseViewModel()
                                    {
                                        Warehouse_name = ware.warehouse_name,
                                    }).ToList();


                        if (jobs.Any())
                        {
                            //foreach (var job in jobs)
                            //{
                                var items = (from ware_i in db.tb_warehouse
                                             join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                                             join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                                             join item_pro_c in db.tb_product_category on item_pro.p_category_id equals item_pro_c.p_category_id
                                             where item_Invet.warehouse_id == obj.warehouse_id
                                             select new Models.StockBalanceBywarehouseInventoryDetialViewModel()
                                             {
                                                 itemCode = item_pro.product_code,
                                                 itemName = item_pro.product_name,
                                                 itemUnit = item_pro.product_unit,
                                                 warehouseName = ware_i.warehouse_name,
                                                 itemTypeName = item_pro_c.p_category_name,
                                                 total_quantity = item_Invet.total_quantity,
                                                 unit_price = item_pro.unit_price,

                                             }

                                            ).ToList();
                                if (items.Any())
                                {
                                    foreach (var item in items)
                                    {
                                        objs.Add(new Models.StockBalanceBywarehouseReport()
                                        {
                                            warehouseName = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault(),
                                            itemCode = item.itemCode,
                                            itemName = item.itemName,
                                            itemUnit = item.itemUnit,
                                            itemTypeName = item.itemTypeName,
                                            unit_price = Convert.ToDecimal(item.unit_price),
                                            total_quantity = Convert.ToDecimal(item.total_quantity),
                                        }
                                       );
                                    }

                                }
                           // }
                        }

                        foreach (var item in objs)
                        {
                            dtr = tb.NewRow();
                            dtr["itemCode"] = item.itemCode;
                            dtr["itemName"] = item.itemName;
                            dtr["itemUnit"] = item.itemUnit;
                            dtr["itemTypeName"] = item.itemTypeName;
                            dtr["warehouseName"] = item.warehouseName;
                            dtr["unit_price"] = item.unit_price;
                            dtr["total_quantity"] = item.total_quantity;
                            dtr["total"] = item.total;
                            tb.Rows.Add(dtr);
                        }
                        rv.LocalReport.ReportPath = reportPath;
                        rv.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("StockBalanceBywarehouse", tb);
                        rv.LocalReport.DataSources.Add(rdc);
                        rv.LocalReport.Refresh();
                    }
                    */

                }
            }
            catch (Exception ex) { }
        }
        public static void GenerateStockBalanceByDateandWarehouseReport2(ReportViewer rv, string reportPath, DateTime date, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceByDateandWarehouseViewModel> objs = new List<Models.StockBalanceByDateandWarehouseViewModel>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare = new List<Models.StockBalanceBydateAndwarehouseReport>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare1 = new List<Models.StockBalanceBydateAndwarehouseReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    //var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    DateTime dates = Convert.ToDateTime(date).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);


                    var jobs = (from inVet in db.tb_inventory
                                join wares in db.tb_warehouse on inVet.warehouse_id equals wares.warehouse_id
                                where wares.warehouse_status == true
                                select new Models.StockMovementWarehouseViewModel()
                                {
                                    Warehouse_name = wares.warehouse_name,
                                }

                           ).ToList();
                    if (jobs.Any())
                    {
                        var ware = (from ware_i in db.tb_warehouse
                                    join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                                    join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                                    join item_pro_c in db.tb_brand on item_pro.brand_id equals item_pro_c.brand_id
                                    join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id
                                    where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                                    //where item_Invet.warehouse_id == obj.warehouse_id
                                    select new { ware_i, item_Invet, item_pro, item_pro_c, item_Ivent_s }).ToList();
                        foreach (var ware_i in ware)
                        {

                            allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                            {
                                warehouseName = ware_i.ware_i.warehouse_name,
                                date = Convert.ToDateTime(date),
                                dateinventory = Convert.ToDateTime(ware_i.item_Invet.inventory_date),
                                itemCode = ware_i.item_pro.product_code,
                                itemName = ware_i.item_pro.product_name,
                                itemUnit = ware_i.item_pro.product_unit,
                                warehouse_id = ware_i.ware_i.warehouse_id,


                                inventory_status_id = ware_i.item_Invet.inventory_status_id,
                                stock_status_id = ware_i.item_Ivent_s.s_status_id,
                                //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                                //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),

                            });
                        }
                        if (!string.IsNullOrEmpty(WarehouseId))
                            allWare = allWare.Where(x => x.warehouseName == (db.tb_warehouse.Where(m => m.warehouse_id == WarehouseId).Select(m => m.warehouse_name).FirstOrDefault())).ToList();
                        if (allWare.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            var show = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (show.Any())
                            {
                                foreach (var it in show)
                                {
                                    var item = allWare.Where(x => x.warehouseName == it.warehouseName).FirstOrDefault();
                                    allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    {
                                        warehouseName = item.warehouseName,
                                        date = Convert.ToDateTime(date),
                                        dateinventory = Convert.ToDateTime(item.dateinventory),
                                        itemCode = item.itemCode,
                                        itemName = item.itemName,
                                        itemUnit = item.itemUnit,
                                        warehouse_id = item.warehouse_id,

                                        inventory_status_id = item.inventory_status_id,
                                        stock_status_id = item.stock_status_id,

                                    });
                                }
                                foreach (var stoc in allWare)
                                {
                                    foreach (var item in show)
                                        if (item.warehouseName == stoc.warehouseName)
                                            break;
                                        else
                                        {
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                dateinventory = Convert.ToDateTime(stoc.dateinventory),
                                                itemCode = stoc.itemCode,
                                                itemName = stoc.itemName,
                                                itemUnit = stoc.itemUnit,
                                                warehouse_id = stoc.warehouse_id,

                                                inventory_status_id = stoc.inventory_status_id,
                                                stock_status_id = stoc.stock_status_id,
                                            });
                                        }
                                }
                            }
                            else
                                //allWare1 = allWare;
                                // purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                                //allWare1 = allWare1.OrderBy(x => x.dateinventory).ToList();
                                foreach (var item in allWare1)
                                {
                                    var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                    var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    if (show1 == show2)
                                    {

                                        dtr = tb.NewRow();
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                }

                            foreach (var item in allWare)
                            {
                                var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                if (show1 == show2)
                                {

                                    dtr = tb.NewRow();
                                    dtr["warehouse_id"] = item.warehouse_id;
                                    dtr["itemCode"] = item.itemCode;
                                    dtr["itemName"] = item.itemName;
                                    dtr["itemUnit"] = item.itemUnit;
                                    dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                    //dtr["warehouseName"] = item.warehouseName;
                                    dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                    dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    tb.Rows.Add(dtr);

                                }
                            }
                        }
                    }
                }

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();

            }
            catch (Exception ex) { }

        }
        public static void GenerateStockBalanceByDateandWarehouseReport1(ReportViewer rv, string reportPath, DateTime date, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceByDateandWarehouseViewModel> objs = new List<Models.StockBalanceByDateandWarehouseViewModel>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare = new List<Models.StockBalanceBydateAndwarehouseReport>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare1 = new List<Models.StockBalanceBydateAndwarehouseReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    //var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    DateTime dates = Convert.ToDateTime(date).AddDays(1);



                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);


                    var jobs = (from inVet in db.tb_inventory
                                join wares in db.tb_warehouse on inVet.warehouse_id equals wares.warehouse_id
                                where wares.warehouse_status == true
                                select new Models.StockMovementWarehouseViewModel()
                                {
                                    Warehouse_name = wares.warehouse_name,
                                }

                           ).ToList();
                    if (jobs.Any())
                    {
                        var ware = (from ware_i in db.tb_warehouse
                                    join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                                    join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                                    join item_pro_c in db.tb_brand on item_pro.brand_id equals item_pro_c.brand_id
                                    join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id
                                    where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                                    //where item_Invet.warehouse_id == obj.warehouse_id
                                    select new { ware_i, item_Invet, item_pro, item_pro_c, item_Ivent_s }).ToList();
                        foreach (var ware_i in ware)
                        {

                            allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                            {
                                warehouseName = ware_i.ware_i.warehouse_name,
                                date = Convert.ToDateTime(date),
                                dateinventory = Convert.ToDateTime(ware_i.item_Invet.inventory_date),
                                itemCode = ware_i.item_pro.product_code,
                                itemName = ware_i.item_pro.product_name,
                                itemUnit = ware_i.item_pro.product_unit,
                                warehouse_id = ware_i.ware_i.warehouse_id,
                                in_issue_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_damage = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_issue = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                in_receive = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_transfer = Convert.ToDecimal(ware_i.item_Invet.total_quantity),

                                inventory_status_id = ware_i.item_Invet.inventory_status_id,
                                stock_status_id = ware_i.item_Ivent_s.s_status_id,
                                //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                                //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),

                            });
                        }
                        if (!string.IsNullOrEmpty(WarehouseId))
                            allWare = allWare.Where(x => x.warehouseName == (db.tb_warehouse.Where(m => m.warehouse_id == WarehouseId).Select(m => m.warehouse_name).FirstOrDefault())).ToList();
                        if (allWare.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            var show = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (show.Any())
                            {
                                foreach (var it in show)
                                {
                                    var item = allWare.Where(x => x.warehouseName == it.warehouseName).FirstOrDefault();
                                    allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    {
                                        warehouseName = item.warehouseName,
                                        date = Convert.ToDateTime(date),
                                        dateinventory = Convert.ToDateTime(item.dateinventory),
                                        itemCode = item.itemCode,
                                        itemName = item.itemName,
                                        itemUnit = item.itemUnit,
                                        warehouse_id = item.warehouse_id,
                                        in_issue_return = Convert.ToDecimal(item.in_issue_return),
                                        out_damage = Convert.ToDecimal(item.out_damage),
                                        out_issue = Convert.ToDecimal(item.out_issue),
                                        out_return = Convert.ToDecimal(item.out_return),
                                        in_receive = Convert.ToDecimal(item.in_receive),
                                        out_transfer = Convert.ToDecimal(item.out_transfer),

                                        inventory_status_id = item.inventory_status_id,
                                        stock_status_id = item.stock_status_id,

                                    });
                                }
                                foreach (var stoc in allWare)
                                {
                                    foreach (var item in show)
                                        if (item.warehouseName == stoc.warehouseName)
                                            break;
                                        else
                                        {
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                dateinventory = Convert.ToDateTime(stoc.dateinventory),
                                                itemCode = stoc.itemCode,
                                                itemName = stoc.itemName,
                                                itemUnit = stoc.itemUnit,
                                                warehouse_id = stoc.warehouse_id,
                                                in_issue_return = Convert.ToDecimal(stoc.in_issue_return),
                                                out_damage = Convert.ToDecimal(stoc.out_damage),
                                                out_issue = Convert.ToDecimal(stoc.out_issue),
                                                out_return = Convert.ToDecimal(stoc.out_return),
                                                in_receive = Convert.ToDecimal(stoc.in_receive),
                                                out_transfer = Convert.ToDecimal(stoc.out_transfer),
                                                inventory_status_id = stoc.inventory_status_id,
                                                stock_status_id = stoc.stock_status_id,
                                            });
                                        }
                                }
                            }
                            else
                                //allWare1 = allWare;
                                // purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                                //allWare1 = allWare1.OrderBy(x => x.dateinventory).ToList();
                                foreach (var item in allWare1)
                                {
                                    var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                    var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    if (show1 == show2)
                                    {

                                        if (item.inventory_status_id == "2")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "3")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "5")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "6")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "7")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }

                                        else if (item.inventory_status_id == "8")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                    }
                                }

                            foreach (var item in allWare)
                            {
                                var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                if (show1 == show2)
                                {

                                    // dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                    if (item.inventory_status_id == "2")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "3")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "5")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "6")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "7")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                    else if (item.inventory_status_id == "8")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                }
                            }
                        }
                    }
                }

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();

            }
            catch (Exception ex) { }

        }
        public static void GenerateStockBalanceByDateandWarehouseReport3(ReportViewer rv, string reportPath, DateTime date, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceByDateandWarehouseViewModel> objs = new List<Models.StockBalanceByDateandWarehouseViewModel>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare = new List<Models.StockBalanceBydateAndwarehouseReport>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare1 = new List<Models.StockBalanceBydateAndwarehouseReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    //var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    DateTime dates = Convert.ToDateTime(date).AddDays(1);



                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);


                    var jobs = (from inVet in db.tb_inventory
                                join wares in db.tb_warehouse on inVet.warehouse_id equals wares.warehouse_id
                                where wares.warehouse_status == true
                                select new Models.StockMovementWarehouseViewModel()
                                {
                                    Warehouse_name = wares.warehouse_name,
                                }

                           ).ToList();
                    if (jobs.Any())
                    {
                        var ware = (from ware_i in db.tb_warehouse
                                    join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                                    join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                                    join item_pro_c in db.tb_brand on item_pro.brand_id equals item_pro_c.brand_id
                                    join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id
                                    where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                                    //where item_Invet.warehouse_id == obj.warehouse_id
                                    select new { ware_i, item_Invet, item_pro, item_pro_c, item_Ivent_s }).ToList();
                        foreach (var ware_i in ware)
                        {

                            allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                            {
                                warehouseName = ware_i.ware_i.warehouse_name,
                                date = Convert.ToDateTime(date),
                                dateinventory = Convert.ToDateTime(ware_i.item_Invet.inventory_date),
                                itemCode = ware_i.item_pro.product_code,
                                itemName = ware_i.item_pro.product_name,
                                itemUnit = ware_i.item_pro.product_unit,
                                warehouse_id = ware_i.ware_i.warehouse_id,
                                in_issue_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_damage = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_issue = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                in_receive = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_transfer = Convert.ToDecimal(ware_i.item_Invet.total_quantity),

                                inventory_status_id = ware_i.item_Invet.inventory_status_id,
                                stock_status_id = ware_i.item_Ivent_s.s_status_id,

                                //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                                //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),

                            });

                        }
                        if (!string.IsNullOrEmpty(WarehouseId))
                            allWare = allWare.Where(x => x.warehouseName == (db.tb_warehouse.Where(m => m.warehouse_id == WarehouseId).Select(m => m.warehouse_name).FirstOrDefault())).ToList();
                        if (allWare.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            var show = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (show.Any())
                            {
                                foreach (var it in show)
                                {
                                    var item = allWare.Where(x => x.warehouseName == it.warehouseName).FirstOrDefault();
                                    allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    {
                                        warehouseName = item.warehouseName,
                                        date = Convert.ToDateTime(date),
                                        dateinventory = Convert.ToDateTime(item.dateinventory),
                                        itemCode = item.itemCode,
                                        itemName = item.itemName,
                                        itemUnit = item.itemUnit,
                                        warehouse_id = item.warehouse_id,
                                        in_issue_return = Convert.ToDecimal(item.in_issue_return),
                                        out_damage = Convert.ToDecimal(item.out_damage),
                                        out_issue = Convert.ToDecimal(item.out_issue),
                                        out_return = Convert.ToDecimal(item.out_return),
                                        in_receive = Convert.ToDecimal(item.in_receive),
                                        out_transfer = Convert.ToDecimal(item.out_transfer),

                                        inventory_status_id = item.inventory_status_id,
                                        stock_status_id = item.stock_status_id,

                                    });
                                }
                                foreach (var stoc in allWare)
                                {
                                    foreach (var item in show)
                                        if (item.warehouseName == stoc.warehouseName)
                                            break;
                                        else
                                        {
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                dateinventory = Convert.ToDateTime(stoc.dateinventory),
                                                itemCode = stoc.itemCode,
                                                itemName = stoc.itemName,
                                                itemUnit = stoc.itemUnit,
                                                warehouse_id = stoc.warehouse_id,
                                                in_issue_return = Convert.ToDecimal(stoc.in_issue_return),
                                                out_damage = Convert.ToDecimal(stoc.out_damage),
                                                out_issue = Convert.ToDecimal(stoc.out_issue),
                                                out_return = Convert.ToDecimal(stoc.out_return),
                                                in_receive = Convert.ToDecimal(stoc.in_receive),
                                                out_transfer = Convert.ToDecimal(stoc.out_transfer),
                                                inventory_status_id = stoc.inventory_status_id,
                                                stock_status_id = stoc.stock_status_id,
                                            });
                                        }
                                }
                            }
                            else
                                //allWare1 = allWare;
                                // purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                                //allWare1 = allWare1.OrderBy(x => x.dateinventory).ToList();
                                foreach (var item in allWare1)
                                {
                                    var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                    var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    if (show1 == show2)
                                    {
                                        if (item.inventory_status_id == "2")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "3")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "5")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "6")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "7")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }

                                        else if (item.inventory_status_id == "8")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else
                                        {
                                            dtr = tb.NewRow();
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                    }
                                }

                            foreach (var item in allWare)
                            {
                                var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                if (show1 == show2)
                                {

                                    // dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                    if (item.inventory_status_id == "2")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "3")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "5")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "6")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "7")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                    else if (item.inventory_status_id == "8")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else
                                    {
                                        dtr = tb.NewRow();
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                }
                            }
                        }
                    }
                }

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();

            }
            catch (Exception ex) { }

        }
        public static void GenerateStockBalanceByDateandWarehouseReport4(ReportViewer rv, string reportPath, DateTime date, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceByDateandWarehouseViewModel> objs = new List<Models.StockBalanceByDateandWarehouseViewModel>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare = new List<Models.StockBalanceBydateAndwarehouseReport>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare1 = new List<Models.StockBalanceBydateAndwarehouseReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    //var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    DateTime dates = Convert.ToDateTime(date).AddDays(1);



                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    var jobs = (from inVet in db.tb_inventory
                                join wares in db.tb_warehouse on inVet.warehouse_id equals wares.warehouse_id

                                where wares.warehouse_status == true

                                select new Models.StockMovementWarehouseViewModel()
                                {
                                    Warehouse_name = wares.warehouse_name,
                                }

                           ).ToList();
                    if (jobs.Any())
                    {
                        var ware = (from ware_i in db.tb_warehouse
                                    join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                                    join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                                    join item_pro_c in db.tb_brand on item_pro.brand_id equals item_pro_c.brand_id
                                    join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id

                                    where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                                    //group item_Invet.total_quantity by item_pro.product_code as 
                                    //where item_Invet.warehouse_id == obj.warehouse_id



                                    select new
                                    {
                                        ware_i,
                                        item_Invet,
                                        item_pro,
                                        item_pro_c,
                                        item_Ivent_s
                                    }).ToList();



                        //if (ware.Any())
                        //{
                        //    var query = from row in db.tb_product.AsEnumerable()

                        //                group row by row.product_code into qty
                        //                orderby qty.Key
                        //                select new
                        //                {
                        //                    Name = qty.Key,
                        //                    CountOforder = qty.Count()

                        //                };
                        //    foreach(var item in query)
                        //    {
                        //        allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                        //        {
                        //            in_issue_return = item.CountOforder,
                        //        });
                        //    }
                        //}

                        foreach (var ware_i in ware)
                        {
                            //var articles = allWare.GroupBy(l => l.itemCode).Select(a => new { qt = a.SUM(b => b.total_quantity), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();
                            //foreach(var item in articles)
                            //{

                            //}
                            //var articles = allWare.GroupBy(l => l.itemCode).Select(a => new { qt = a.Sum(b => b.total_quantity), itemcode = a.Key }).OrderByDescending(a => a.qt).ToList();
                            allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                            {
                                warehouseName = ware_i.ware_i.warehouse_name,
                                date = Convert.ToDateTime(date),
                                dateinventory = Convert.ToDateTime(ware_i.item_Invet.inventory_date),
                                itemCode = ware_i.item_pro.product_code,
                                itemName = ware_i.item_pro.product_name,
                                itemUnit = ware_i.item_pro.product_unit,
                                warehouse_id = ware_i.ware_i.warehouse_id,

                                //in_issue_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_damage = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_issue = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                in_receive = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                                out_transfer = Convert.ToDecimal(ware_i.item_Invet.total_quantity),


                                inventory_status_id = ware_i.item_Invet.inventory_status_id,
                                stock_status_id = ware_i.item_Ivent_s.s_status_id,

                                //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                                //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),

                            });

                        }
                        //var articles = allWare.GroupBy(l => l.in_issue_return).Select(a => new { qt = a.Min(b => b.dateinventory), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();
                        //var articles = allWare.GroupBy(l => l.itemCode).Select(a => new { qt = a.Sum(b => b.total_quantity), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();
                        //var articles = allWare.GroupBy(l => l.in_issue_return).Select(a => new { qt = a.Sum(b => b.total_quantity), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();
                        //if (articles.Any())
                        //{

                        //}
                        if (!string.IsNullOrEmpty(WarehouseId))

                            allWare = allWare.Where(x => x.warehouseName == (db.tb_warehouse.Where(m => m.warehouse_id == WarehouseId).Select(m => m.warehouse_name).FirstOrDefault())).ToList();
                        if (allWare.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("dd-MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            //var articles = allWare.GroupBy(l => l.in_issue_return).Select(a => new { qt = a.Min(b => b.dateinventory), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();
                            //var articles = allWare.GroupBy(l => l.itemCode).Select(a => new { qt = a.Sum(b => b.total_quantity), ArticleID = a.Key }).OrderByDescending(a => a.qt).ToList();

                            //var show = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();

                            //var show = allWare.GroupBy(a => new { a.itemCode }).Select(a=>new {qt=a.Sum(b=>b.total_quantity), itemcode = a.Key}).ToList();
                            //if (show.Any())
                            //{
                            //    foreach (var item in show)
                            //    {
                            //        allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                            //        {
                            //            in_issue_return = Convert.ToDecimal(item.qt),

                            //        });
                            //    }
                            //}

                            var show1 = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (show1.Any())
                            {
                                foreach (var it in show1)
                                {
                                    var item = allWare.Where(x => x.warehouseName == it.warehouseName).FirstOrDefault();
                                    var show = allWare.GroupBy(a => new { a.itemCode }).Select(a => new { qt = a.Min(b => b.dateinventory), itemcode = a.Key }).ToList();
                                    if (show.Any())
                                    {
                                        foreach (var items in show)
                                        {
                                            var itemstock = allWare.GroupBy(a => new { a.itemCode }).Select(a => new { qt = a.Min(b => b.dateinventory), itemcode = a.Key }).FirstOrDefault();
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                //in_issue_return = Convert.ToDecimal(items.qt),
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                dateinventory = Convert.ToDateTime(itemstock.qt),
                                                itemCode = item.itemCode,
                                                itemName = item.itemName,
                                                itemUnit = item.itemUnit,
                                                warehouse_id = item.warehouse_id,
                                                in_issue_return = Convert.ToDecimal(item.in_issue_return),
                                                out_damage = Convert.ToDecimal(item.out_damage),
                                                out_issue = Convert.ToDecimal(item.out_issue),
                                                out_return = Convert.ToDecimal(item.out_return),
                                                in_receive = Convert.ToDecimal(item.in_receive),
                                                out_transfer = Convert.ToDecimal(item.out_transfer),

                                                inventory_status_id = item.inventory_status_id,
                                                stock_status_id = item.stock_status_id,

                                            });
                                        }

                                        foreach (var stoc in allWare)
                                        {
                                            foreach (var itemss in show)
                                                if (item.warehouseName == stoc.warehouseName)
                                                    break;
                                                else
                                                {
                                                    allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                                    {

                                                        dateinventory = Convert.ToDateTime(itemss.qt),
                                                    });
                                                }

                                        }
                                    }


                                    //allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    //{
                                    //    warehouseName = item.warehouseName,
                                    //    date = Convert.ToDateTime(date),
                                    //    dateinventory = Convert.ToDateTime(item.dateinventory),
                                    //    itemCode = item.itemCode,
                                    //    itemName = item.itemName,
                                    //    itemUnit = item.itemUnit,
                                    //    warehouse_id = item.warehouse_id,
                                    //    in_issue_return = Convert.ToDecimal(item.in_issue_return),
                                    //    out_damage = Convert.ToDecimal(item.out_damage),
                                    //    out_issue = Convert.ToDecimal(item.out_issue),
                                    //    out_return = Convert.ToDecimal(item.out_return),
                                    //    in_receive = Convert.ToDecimal(item.in_receive),
                                    //    out_transfer = Convert.ToDecimal(item.out_transfer),

                                    //    inventory_status_id = item.inventory_status_id,
                                    //    stock_status_id = item.stock_status_id,

                                    //});

                                }
                                foreach (var stoc in allWare)
                                {
                                    foreach (var item in show1)
                                        if (item.warehouseName == stoc.warehouseName)
                                            break;
                                        else
                                        {
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                //dateinventory = Convert.ToDateTime(stoc.q),
                                                itemCode = stoc.itemCode,
                                                itemName = stoc.itemName,
                                                itemUnit = stoc.itemUnit,
                                                warehouse_id = stoc.warehouse_id,
                                                in_issue_return = Convert.ToDecimal(stoc.in_issue_return),
                                                out_damage = Convert.ToDecimal(stoc.out_damage),
                                                out_issue = Convert.ToDecimal(stoc.out_issue),
                                                out_return = Convert.ToDecimal(stoc.out_return),
                                                in_receive = Convert.ToDecimal(stoc.in_receive),
                                                out_transfer = Convert.ToDecimal(stoc.out_transfer),
                                                inventory_status_id = stoc.inventory_status_id,
                                                stock_status_id = stoc.stock_status_id,
                                            });
                                        }

                                }
                            }
                            else
                                //allWare1 = allWare;
                                // purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                                //allWare1 = allWare1.OrderBy(x => x.dateinventory).ToList();
                                foreach (var item in allWare1)
                                {
                                    var show11 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                    var show22 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    if (show11 == show22)
                                    {
                                        if (item.inventory_status_id == "2")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "3")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "5")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "6")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "7")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }

                                        else if (item.inventory_status_id == "8")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else
                                        {
                                            dtr = tb.NewRow();
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                    }
                                }

                            foreach (var item in allWare)
                            {
                                var show11 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                var show22 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                if (show11 == show22)
                                {

                                    // dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                    if (item.inventory_status_id == "2")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "3")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "5")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "6")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "7")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                    else if (item.inventory_status_id == "8")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else
                                    {
                                        dtr = tb.NewRow();
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                }
                            }
                        }
                    }
                }

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();

            }
            catch (Exception ex) { }

        }
        public static void GenerateStockBalanceByDateandWarehouseReport(ReportViewer rv, string reportPath, DateTime date, string WarehouseId)
        {
            try
            {
                List<Models.StockBalanceByDateandWarehouseViewModel> objs = new List<Models.StockBalanceByDateandWarehouseViewModel>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare = new List<Models.StockBalanceBydateAndwarehouseReport>();
                List<Models.StockBalanceBydateAndwarehouseReport> allWare1 = new List<Models.StockBalanceBydateAndwarehouseReport>();
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    //var objss = db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).ToList();
                    DateTime dates = Convert.ToDateTime(date).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    var jobs = (from inVet in db.tb_inventory
                                join wares in db.tb_warehouse on inVet.warehouse_id equals wares.warehouse_id
                                join pro in db.tb_product on inVet.product_id equals pro.product_id
                                where wares.warehouse_status == true
                                //group inVet by new { pro.product_code, wares.warehouse_name, inVet.total_quantity }
                                //into to
                                select new Models.StockMovementWarehouseViewModel()
                                {
                                    Warehouse_name = wares.warehouse_name,
                                    //Warehouse_name = to.Key.warehouse_name,
                                }

                           ).ToList();
                    if (jobs.Any())
                    {
                        //var ware = (from ware_i in db.tb_warehouse
                        //            join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                        //            join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                        //            join item_pro_c in db.tb_product_category on item_pro.p_category_id equals item_pro_c.p_category_id
                        //            join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id
                        //            where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                        //            //group item_pro by new {item_pro.product_code} into test
                        //            //where item_Invet.warehouse_id == obj.warehouse_id
                        //            select new { ware_i, item_Invet, item_pro, item_pro_c, item_Ivent_s }).ToList();
                        //if (ware.Any())
                        //{
                        //   // var test = objs.GroupBy(a => new { a.itemCode }).Select(a => new { qt = a.Sum(b => b.total_quantity), itemcode = a.Key }).ToList();

                        //    foreach (var ware_i in ware)
                        //    {
                        //        var test = ware.GroupBy(l => new { l.item_pro.product_code, l.ware_i.warehouse_name }).Select(a => new { qt = a.Sum(b => b.item_Invet.total_quantity), itemcode = a.Key }).FirstOrDefault();

                        //        allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                        //        {
                        //            warehouseName = ware_i.ware_i.warehouse_name,
                        //            date = Convert.ToDateTime(date),
                        //            dateinventory = Convert.ToDateTime(ware_i.item_Invet.inventory_date),
                        //            itemCode = ware_i.item_pro.product_code,
                        //            itemName = ware_i.item_pro.product_name,
                        //            itemUnit = ware_i.item_pro.product_unit,
                        //            warehouse_id = ware_i.ware_i.warehouse_id,
                        //            in_issue_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //            // out_damage = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //            out_damage = Convert.ToDecimal(test.qt),
                        //            out_issue = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //            out_return = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //            in_receive = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //            out_transfer = Convert.ToDecimal(ware_i.item_Invet.total_quantity),

                        //            inventory_status_id = ware_i.item_Invet.inventory_status_id,
                        //            stock_status_id = ware_i.item_Ivent_s.s_status_id,
                        //            // in_issue_return = Convert.ToDecimal(test.qt),
                        //            //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                        //            //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),


                        //        });
                        //    }
                        //}



                        //var ware = (from ware_i in db.tb_warehouse
                        //            join item_Invet in db.tb_inventory on ware_i.warehouse_id equals item_Invet.warehouse_id
                        //            join item_pro in db.tb_product on item_Invet.product_id equals item_pro.product_id
                        //            join item_pro_c in db.tb_product_category on item_pro.p_category_id equals item_pro_c.p_category_id
                        //            join item_Ivent_s in db.tb_inventory_status on item_Invet.inventory_status_id equals item_Ivent_s.s_status_id
                        //            where item_Invet.inventory_status_id == item_Ivent_s.s_status_id
                        //            //group item_pro by new {item_pro.product_code} into test
                        //            //where item_Invet.warehouse_id == obj.warehouse_id
                        //            select new { ware_i, item_Invet, item_pro, item_pro_c, item_Ivent_s }).ToList();
                        //if (ware.Any())
                        //{

                        //    var stoware = ware.GroupBy(l => new
                        //    {
                        //        l.item_pro.product_code,
                        //        l.ware_i.warehouse_name,
                        //        l.ware_i.warehouse_id,
                        //        l.item_Invet.inventory_date,
                        //        l.item_pro.product_name,
                        //        l.item_pro.product_unit,
                        //        l.item_Invet.total_quantity,
                        //        l.item_Invet.inventory_status_id,
                        //        l.item_Ivent_s.s_status_id

                        //    }).Select(a => new { qt = a.Sum(b => b.item_Invet.total_quantity), itemcode = a.Key }).ToList();

                        //    if (stoware.Any())
                        //    {
                        //        // var test = objs.GroupBy(a => new { a.itemCode }).Select(a => new { qt = a.Sum(b => b.total_quantity), itemcode = a.Key }).ToList();

                        //        foreach (var ware_i in stoware)
                        //        {
                        //            //var test = ware.GroupBy(l => new { l.item_pro.product_code, l.ware_i.warehouse_name }).Select(a => new { qt = a.Sum(b => b.item_Invet.total_quantity), itemcode = a.Key }).FirstOrDefault();

                        //            allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                        //            {
                        //                warehouseName = ware_i.itemcode.warehouse_name,
                        //                date = Convert.ToDateTime(date),
                        //                dateinventory = Convert.ToDateTime(ware_i.itemcode.inventory_date),
                        //                itemCode = ware_i.itemcode.product_code,
                        //                itemName = ware_i.itemcode.product_name,
                        //                itemUnit = ware_i.itemcode.product_unit,
                        //                warehouse_id = ware_i.itemcode.warehouse_id,
                        //                in_issue_return = Convert.ToDecimal(ware_i.itemcode.total_quantity),
                        //                // out_damage = Convert.ToDecimal(ware_i.item_Invet.total_quantity),
                        //                out_damage = Convert.ToDecimal(ware_i.qt),
                        //                out_issue = Convert.ToDecimal(ware_i.itemcode.total_quantity),
                        //                out_return = Convert.ToDecimal(ware_i.itemcode.total_quantity),
                        //                in_receive = Convert.ToDecimal(ware_i.itemcode.total_quantity),
                        //                out_transfer = Convert.ToDecimal(ware_i.itemcode.total_quantity),

                        //                inventory_status_id = ware_i.itemcode.inventory_status_id,
                        //                stock_status_id = ware_i.itemcode.s_status_id,
                        //                // in_issue_return = Convert.ToDecimal(test.qt),
                        //                //in_issue_return=Convert.ToDecimal(ware_i.item_ivent_d.quantity),
                        //                //out_damage= Convert.ToDecimal(ware_i.item_Invet_d.quantity),

                        //            });
                        //        }
                        //    }

                        //}



                        var query = (from w in db.tb_warehouse
                                     join inv in db.tb_inventory on w.warehouse_id equals inv.warehouse_id
                                     join p in db.tb_product on inv.product_id equals p.product_id
                                     join invs in db.tb_inventory_status on inv.inventory_status_id equals invs.s_status_id
                                     where invs.s_status_id == inv.inventory_status_id
                                     group inv by new { inv.product_id, p.product_code, p.product_name, w.warehouse_name, w.warehouse_id, inv.inventory_date, inv.total_quantity }
                                         into grp
                                     //select new Models.StockBalanceByDateandWarehouseInventoryDetialViewModel()
                                     select new
                                     {
                                         grp.Key.product_id,
                                         grp.Key.product_code,
                                         grp.Key.product_name,
                                         grp.Key.inventory_date,
                                         grp.Key.warehouse_name,
                                         quantity = grp.Sum(t => t.total_quantity)
                                     }).ToList();
                        if (query.Any())
                        {
                            foreach (var tt in query)
                            {
                                allWare.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                {
                                    itemCode = tt.product_code,
                                    itemName = tt.product_name,
                                    total_quantity = Convert.ToDecimal(tt.quantity),
                                    dateinventory = Convert.ToDateTime(tt.inventory_date),
                                    warehouseName = tt.warehouse_name,
                                    date = Convert.ToDateTime(date),
                                    out_damage = Convert.ToDecimal(tt.quantity),

                                });
                            }
                        }


                        if (!string.IsNullOrEmpty(WarehouseId))
                            allWare = allWare.Where(x => x.warehouseName == (db.tb_warehouse.Where(m => m.warehouse_id == WarehouseId).Select(m => m.warehouse_name).FirstOrDefault())).ToList();
                        if (allWare.Count == 0)
                        {
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            tb.Rows.Add(dtr);
                        }
                        else
                        {
                            var show = allWare.GroupBy(x => new { x.warehouseName }).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (show.Any())
                            {
                                foreach (var it in show)
                                {
                                    var item = allWare.Where(x => x.warehouseName == it.warehouseName).FirstOrDefault();

                                    allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    {
                                        warehouseName = item.warehouseName,
                                        date = Convert.ToDateTime(date),
                                        //dateinventory = Convert.ToDateTime(item.dateinventory),
                                        itemCode = item.itemCode,
                                        itemName = item.itemName,
                                        itemUnit = item.itemUnit,
                                        warehouse_id = item.warehouse_id,
                                        in_issue_return = Convert.ToDecimal(item.in_issue_return),
                                        out_damage = Convert.ToDecimal(item.out_damage),
                                        out_issue = Convert.ToDecimal(item.out_issue),
                                        out_return = Convert.ToDecimal(item.out_return),
                                        in_receive = Convert.ToDecimal(item.in_receive),
                                        out_transfer = Convert.ToDecimal(item.out_transfer),

                                        inventory_status_id = item.inventory_status_id,
                                        stock_status_id = item.stock_status_id,

                                    });

                                    //var show1 = allWare.GroupBy(a => new { a.itemCode }).Select(a => new { qt = a.Min(b => b.dateinventory), itemcode = a.Key }).ToList();
                                    //var show1 = db.tb_inventory.GroupBy(a => new { a.product_id }).Select(a => new { qt = a.Min(b => b.inventory_date), itemcode = a.Key }).ToList();
                                    //if (show1.Any())
                                    //{
                                    //    foreach (var item1 in show1)
                                    //    {
                                    //        allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    //        {

                                    //            dateinventory = Convert.ToDateTime(item1.qt),

                                    //        });
                                    //    }
                                    //    foreach(var stoc1 in allWare)
                                    //    {
                                    //        allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                    //        {
                                    //            dateinventory = Convert.ToDateTime(stoc1.dateinventory),

                                    //        });
                                    //    }

                                    //}
                                }
                                foreach (var stoc in allWare)
                                {
                                    foreach (var item in show)
                                        if (item.warehouseName == stoc.warehouseName)
                                            break;
                                        else
                                        {
                                            allWare1.Add(new Models.StockBalanceBydateAndwarehouseReport()
                                            {
                                                warehouseName = item.warehouseName,
                                                date = Convert.ToDateTime(date),
                                                //dateinventory = Convert.ToDateTime(stoc.dateinventory),
                                                itemCode = stoc.itemCode,
                                                itemName = stoc.itemName,
                                                itemUnit = stoc.itemUnit,
                                                warehouse_id = stoc.warehouse_id,
                                                in_issue_return = Convert.ToDecimal(stoc.in_issue_return),
                                                out_damage = Convert.ToDecimal(stoc.out_damage),
                                                out_issue = Convert.ToDecimal(stoc.out_issue),
                                                out_return = Convert.ToDecimal(stoc.out_return),
                                                in_receive = Convert.ToDecimal(stoc.in_receive),
                                                out_transfer = Convert.ToDecimal(stoc.out_transfer),
                                                inventory_status_id = stoc.inventory_status_id,
                                                stock_status_id = stoc.stock_status_id,
                                            });
                                        }
                                }
                            }
                            else
                                //allWare1 = allWare;
                                // purchaseOrders1 = purchaseOrders1.OrderBy(x => x.purchase_order_date).ThenBy(x => x.purchase_order_number).ToList();
                                //allWare1 = allWare1.OrderBy(x => x.dateinventory).ToList();
                                foreach (var item in allWare1)
                                {
                                    var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                    var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    if (show1 == show2)
                                    {
                                        if (item.inventory_status_id == "2")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "3")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "5")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "6")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else if (item.inventory_status_id == "7")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }

                                        else if (item.inventory_status_id == "8")
                                        {
                                            dtr = tb.NewRow();
                                            dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                        else
                                        {
                                            dtr = tb.NewRow();
                                            dtr["warehouse_id"] = item.warehouse_id;
                                            dtr["stock_status_id"] = item.stock_status_id;
                                            dtr["inventory_status_id"] = item.inventory_status_id;
                                            dtr["itemCode"] = item.itemCode;
                                            dtr["itemName"] = item.itemName;
                                            dtr["itemUnit"] = item.itemUnit;
                                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                            //dtr["warehouseName"] = item.warehouseName;
                                            dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                            tb.Rows.Add(dtr);
                                        }
                                    }
                                }

                            foreach (var item in allWare)
                            {
                                var show1 = Convert.ToDateTime(item.dateinventory).ToString("MMM-yyyy");
                                var show2 = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                if (show1 == show2)
                                {

                                    // dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                    if (item.inventory_status_id == "2")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_issue"] = Convert.ToDecimal(item.out_issue);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "3")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_damage"] = Convert.ToDecimal(item.out_damage);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "5")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_issue_return"] = Convert.ToDecimal(item.in_issue_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "6")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_transfer"] = Convert.ToDecimal(item.out_transfer);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else if (item.inventory_status_id == "7")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["in_receive"] = Convert.ToDecimal(item.in_receive);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                    else if (item.inventory_status_id == "8")
                                    {
                                        dtr = tb.NewRow();
                                        dtr["out_return"] = Convert.ToDecimal(item.out_return);
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }
                                    else
                                    {
                                        dtr = tb.NewRow();
                                        dtr["warehouse_id"] = item.warehouse_id;
                                        dtr["stock_status_id"] = item.stock_status_id;
                                        dtr["inventory_status_id"] = item.inventory_status_id;
                                        dtr["itemCode"] = item.itemCode;
                                        dtr["itemName"] = item.itemName;
                                        dtr["itemUnit"] = item.itemUnit;
                                        dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                                        //dtr["warehouseName"] = item.warehouseName;
                                        dtr["dateinventory"] = Convert.ToDateTime(item.dateinventory).ToString("dd-MMM-yyyy");
                                        dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                        tb.Rows.Add(dtr);
                                    }

                                }
                            }
                        }
                    }
                }

                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();

            }
            catch (Exception ex) { }

        }
        public static void GenerateQuoteReport(ReportViewer rv, string reportPath, string id)
        {
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                DataColumn col = new DataColumn();
                col.ColumnName = "Quote_No";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Supplier";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Prepare_By";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Modify_By";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Code";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Unit";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Price";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Discount";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Remark";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "created_date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Modify_date";
                tb.Columns.Add(col);


                var obj = (from quote in db.tb_quote
                           join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                           where quote.quote_id == id
                           select new { quote, supplier }).FirstOrDefault();
                if (obj != null)
                {
                    var dQuotes = (from dQuote in db.tb_quote_detail
                                   join item in db.tb_product on dQuote.item_id equals item.product_id
                                   orderby item.product_code
                                   where dQuote.quote_id == obj.quote.quote_id
                                   select new { dQuote, item }).ToList();
                    if (dQuotes.Any())
                    {
                        foreach (var quote in dQuotes)
                        {
                            dtr = tb.NewRow();
                            dtr["Quote_No"] = obj.quote.quote_no;
                            dtr["Supplier"] = obj.supplier.supplier_name;
                            dtr["Prepare_By"] = BT_KimMex.Class.CommonClass.GetUserFullname(obj.quote.created_by);
                            dtr["Modify_By"] = BT_KimMex.Class.CommonClass.GetUserFullname(obj.quote.updated_by);
                            dtr["Item_Code"] = quote.item.product_code;
                            dtr["Item_Name"] = quote.item.product_name;
                            dtr["Item_Unit"] = quote.item.product_unit;
                            dtr["Price"] = Convert.ToDecimal(quote.dQuote.price);
                            dtr["Discount"] = quote.dQuote.discount;
                            dtr["Remark"] = quote.dQuote.remark;
                            //dtr["created_date"] = obj.quote.created_date;
                            //dtr["Modify_date"] = obj.quote.updated_date;

                            dtr["created_date"] = Convert.ToDateTime(obj.quote.created_date).ToString("dd-MMM-yyyy");
                            dtr["Modify_date"] = Convert.ToDateTime(obj.quote.updated_date).ToString("dd-MMM-yyyy");

                            tb.Rows.Add(dtr);
                        }
                    }
                }
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("Quote", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();
            }
        }
        //Bora 30 Jun 2021
     
        public static void GenerateGRNReport(ReportViewer rv, string reportPath, string id)
        {
            ItemReceiveViewModel itemReceive = new ItemReceiveViewModel();
            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                DataColumn col = new DataColumn();
                col.ColumnName = "Date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Re_Type";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "MR_RefNo";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "To_Warehouse";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "From_Warehouse";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "received_number";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "ref_number";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "project_full_name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Code";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Quantity";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Received_QTY";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "received_type";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Remain";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Invoice_No";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Invoice_Date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Status";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Note";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Document";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Unit";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Supplier_name";
                tb.Columns.Add(col);
                string ref_number = string.Empty;
                itemReceive = (from ire in db.tb_receive_item_voucher
                               where string.Compare(ire.receive_item_voucher_id, id) == 0
                               select new ItemReceiveViewModel()
                               {
                                   receive_item_voucher_id = ire.receive_item_voucher_id,
                                   received_number = ire.received_number,
                                   received_status = ire.received_status,
                                   received_type = ire.received_type,
                                   po_report_number = ire.po_report_number,
                                   supplier_id = ire.supplier_id,
                                   ref_id = ire.ref_id,
                                   created_date = ire.created_date
                               }).FirstOrDefault();
                if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                {
                    ref_number = db.tb_stock_transfer_voucher.Where(m => m.status == true && m.stock_transfer_id == itemReceive.ref_id).Select(m => m.stock_transfer_no).FirstOrDefault();
                    //itemReceive.receivedItems = Inventory.GetInventoryItem(itemReceive.ref_id);
                    itemReceive.receivedItems = Inventory.GetStockTransferItems(itemReceive.ref_id);
                    itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                 join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                 where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                 select mr.ir_no).FirstOrDefault().ToString();
                    itemReceive.project_full_name = (from pro in db.tb_project
                                                     join mr in db.tb_item_request on pro.project_id equals mr.ir_project_id
                                                     join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                                                     where string.Compare(st.stock_transfer_id, itemReceive.ref_id) == 0
                                                     select pro.project_full_name).FirstOrDefault().ToString();
                }
                else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                {
                    //ref_number = db.tb_purchase_order.Where(m => m.status == true && string.Compare(m.purchase_order_id, itemReceive.ref_id) == 0).Select(m => m.purchase_oder_number).FirstOrDefault();
                    ref_number = itemReceive.po_report_number;
                    itemReceive.mr_ref_number = (from mr in db.tb_item_request
                                                 join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                 join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                 join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                 where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                 select mr.ir_no).FirstOrDefault();
                    itemReceive.project_full_name = (from mr in db.tb_item_request
                                                     join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                                     join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                                     join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                                     join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                                                     where string.Compare(po.pruchase_request_id, itemReceive.ref_id) == 0
                                                     select pro.project_full_name).FirstOrDefault();
                    itemReceive.receivedItems = Inventory.ConvertFromPOtoInventory(itemReceive.ref_id, id, itemReceive.po_report_number, itemReceive.supplier_id);

                }
                else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                {
                    var transferRef = (from transfer in db.transferformmainstocks
                                       join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                       join project in db.tb_project on mr.ir_project_id equals project.project_id
                                       join site in db.tb_site on project.site_id equals site.site_id
                                       join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                       where string.Compare(transfer.stock_transfer_id, itemReceive.ref_id) == 0
                                       select new
                                       {
                                           transfer,
                                           project,
                                           mr,
                                           wh
                                       }).FirstOrDefault();
                    ref_number = transferRef.transfer.stock_transfer_no;
                    itemReceive.mr_ref_number = transferRef.mr.ir_no;
                    itemReceive.project_full_name = transferRef.project.project_full_name;
                    itemReceive.receivedItems = Inventory.GetTransferfromWorkshopItems(itemReceive.ref_id);

                }
                else if (string.Compare(itemReceive.received_type, "Stock Return") == 0)
                {
                    var returnRef = (from sreturn in db.tb_stock_issue_return
                                     join transfer in db.tb_stock_transfer_voucher on sreturn.stock_issue_ref equals transfer.stock_transfer_id
                                     join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                                     join project in db.tb_project on mr.ir_project_id equals project.project_id
                                     join site in db.tb_site on project.site_id equals site.site_id
                                     join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                                     where string.Compare(sreturn.stock_issue_return_id, itemReceive.ref_id) == 0
                                     select new
                                     {
                                         sreturn,
                                         transfer,
                                         mr,
                                         project,
                                         wh
                                     }).FirstOrDefault();
                    ref_number = returnRef.sreturn.issue_return_number;
                    itemReceive.mr_ref_number = returnRef.mr.ir_no;
                    itemReceive.project_full_name = returnRef.project.project_full_name;
                    itemReceive.receivedItems = Inventory.GetStockReturnItems(itemReceive.ref_id);
                }
                else if (string.Compare(itemReceive.received_type, "Return Workshop") == 0)
                {
                    var returnRef = (
                        from wreturn in db.tb_return_main_stock
                        join transfer in db.transferformmainstocks on wreturn.return_ref_id equals transfer.stock_transfer_id
                        join mr in db.tb_item_request on transfer.item_request_id equals mr.ir_id
                        join project in db.tb_project on mr.ir_project_id equals project.project_id
                        join site in db.tb_site on project.site_id equals site.site_id
                        join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                        where string.Compare(wreturn.return_main_stock_id, itemReceive.ref_id) == 0
                        select new
                        {
                            wreturn,
                            transfer,
                            project,
                            mr,
                            wh
                        }).FirstOrDefault();
                    ref_number = returnRef.wreturn.return_main_stock_no;
                    itemReceive.mr_ref_number = returnRef.mr.ir_no;
                    itemReceive.project_full_name = returnRef.project.project_full_name;
                    itemReceive.receivedItems = Inventory.GetReturnWorkshopItems(itemReceive.ref_id);
                }
                    itemReceive.ref_number = ref_number;
                    itemReceive.receivedHistories = Inventory.GetReceivedHistory(itemReceive.ref_id, itemReceive.receive_item_voucher_id);
                    itemReceive.inventories = Inventory.GetReceivedItemList(itemReceive);
                    itemReceive.attachments = Inventory.GetItemReceiveAttachment(itemReceive.receive_item_voucher_id);
                    itemReceive.rejects = CommonClass.GetRejectByRequest(id);
                 if(itemReceive != null)
                 {
                    foreach (var item in itemReceive.receivedItems)
                    {
                        dtr = tb.NewRow();
                        dtr["Date"] = itemReceive.created_date;
                        dtr["Re_Type"] = itemReceive.received_type;
                        dtr["MR_RefNo"] = itemReceive.mr_ref_number;
                        // dtr["To_Warehouse"] = itemReceive.inventories[0].warehouseName;
                        dtr["received_number"] = itemReceive.received_number;
                        dtr["ref_number"] = itemReceive.ref_number;
                        dtr["project_full_name"] = itemReceive.project_full_name;
                       // foreach (var item in itemReceive.receivedItems)
                        //{
                            foreach (var inv in itemReceive.inventories)
                            {
                                if (string.Compare(item.product_id, inv.product_id) == 0)
                                {
                                    dtr["To_Warehouse"] = itemReceive.inventories[0].warehouseName;
                                    dtr["Item_Name"] = item.itemName;
                                    dtr["Item_Code"] = item.itemCode;
                                    if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                                    {
                                        dtr["received_type"] = string.Format("{0:G29}", Decimal.Parse(item.total_quantity.ToString())) + item.itemUnitName;

                                    }
                                    else
                                    {
                                        dtr["received_type"] = string.Format("{0:G29}", Decimal.Parse(item.out_quantity.ToString())) + item.itemUnitName;
                                    }
                                    foreach (var obj in itemReceive.receivedHistories)
                                    {
                                        var re = obj.inventories.Where(s => string.Compare(s.product_id, item.product_id) == 0).FirstOrDefault();
                                        if (re != null)
                                        {
                                            if (string.Compare(obj.received_status, Status.Completed) == 0)
                                            {
                                                dtr["Quantity"] = re.in_quantity;
                                            }
                                            else
                                            {
                                                dtr["Quantity"] = re.in_quantity;
                                            }
                                        }
                                        else
                                        {


                                        }
                                    }
                                    // count++;
                                    // dtr["Quantity"] = Convert.ToDecimal(quote.dQuote.price);
                                    dtr["Received_QTY"] = string.Format("{0:G29}", Decimal.Parse(inv.in_quantity.ToString()));
                                    dtr["Supplier_name"] = inv.supplier_name;
                                    dtr["Remain"] = inv.unitName;
                                    dtr["Unit"] = inv.unitName;
                                    if (string.Compare(itemReceive.received_type, "Stock Transfer") == 0)
                                    {
                                        dtr["Remain"] = CommonFunctions.GetStokTransferRemainBalancebyItem(itemReceive.ref_id, item.product_id);
                                    }
                                    else if (string.Compare(itemReceive.received_type, "Transfer Workshop") == 0)
                                    {
                                        dtr["Remain"] = item.remain_quantity;
                                    }
                                    else if (string.Compare(itemReceive.received_type, "Purchase Order") == 0)
                                    {
                                        dtr["Remain"] = item.remain_quantity; ;
                                    }
                                    else
                                    {
                                        dtr["Remain"] = item.remain_quantity;
                                    }
                                    dtr["Invoice_No"] = inv.invoice_number;
                                    dtr["Invoice_Date"] = Convert.ToDateTime(inv.invoice_date).ToString("dd-MMM-yyyy");
                                    // dtr["Status"] = obj.quote.updated_date;
                                    if (inv.completed)
                                    {
                                        dtr["Status"] = "Complete";
                                    }
                                    else
                                    {
                                        if (string.Compare(inv.item_status, "approved") == 0)
                                        {
                                            dtr["Status"] = inv.item_status;
                                        }
                                        else
                                        {
                                            dtr["Status"] = inv.item_status;
                                        }
                                    }
                                    dtr["Note"] = inv.remark;
                                    // dtr["Document"] = Convert.ToDateTime(obj.quote.updated_date).ToString("dd-MMM-yyyy");

                                }

                           // }

                            }

                        tb.Rows.Add(dtr);
                     }
                  }


                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("GRNreport", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();
            }
        }
      
        public static void GenerateMaterialRequestReport(ReportViewer rv, string reportPath, string id)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();

            using (BT_KimMex.Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                DataColumn col = new DataColumn();
                col.ColumnName = "Mr_Date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Mr_No";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "projectname";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Purpose";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "No";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_code";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Item_Name";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "re_Qty";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "re_unit";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "remark";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "reason";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "Remark";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "AppQty";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "created_date";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "SONo";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "CreatedBy";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "ApprovedBy";
                tb.Columns.Add(col);
                col = new DataColumn();
                col.ColumnName = "ApprovedDate";
                tb.Columns.Add(col);

                var request = (from i in db.tb_item_request
                               join pj in db.tb_project on i.ir_project_id equals pj.project_id
                               join purpose in db.tb_purpose on i.ir_purpose_id equals purpose.purpose_id into pp
                               from purpose in pp.DefaultIfEmpty()
                               where i.ir_id == id
                               select new { i, pj, purpose }).FirstOrDefault();
                if (request != null)
                {
                    itemRequest = new ItemRequestViewModel();
                    itemRequest.ir_id = request.i.ir_id;
                    itemRequest.ir_no = request.i.ir_no;
                    itemRequest.ir_project_id = request.i.ir_project_id;
                    itemRequest.project_full_name = request.pj.project_full_name;
                    itemRequest.ir_purpose_id = request.i.ir_purpose_id;
                    itemRequest.purpose_description = request.purpose == null ? string.Empty : request.purpose.purpose_name;
                    itemRequest.ir_status = request.i.ir_status;
                    itemRequest.created_date = request.i.created_date;
                    itemRequest.created_by = CommonFunctions.GetUserFullnamebyUserId(request.i.created_by);
                    itemRequest.updated_by = CommonFunctions.GetUserFullnamebyUserId(request.i.updated_by);
                    itemRequest.checked_by = CommonFunctions.GetUserFullnamebyUserId(request.i.checked_by);
                    itemRequest.approved_by = CommonFunctions.GetUserFullnamebyUserId(request.i.approved_by);
                    itemRequest.is_mr = request.i.is_mr;
                }

                List<ItemRequestDetail1ViewModel> ir1 = new List<ItemRequestDetail1ViewModel>();
                var dIrs = (from ir in db.tb_ir_detail1
                            join jc in db.tb_job_category on ir.ir_job_category_id equals jc.j_category_id
                            orderby jc.created_date
                            where ir.ir_id == itemRequest.ir_id
                            select new
                            {
                                ir_detail1_id = ir.ir_detail1_id,
                                ir_id = ir.ir_id,
                                ir_job_category_id = ir.ir_job_category_id,
                                job_category_description = jc.j_category_name
                            }).ToList();
                if (dIrs.Any())
                {
                   // int dCount = 1;
                    foreach (var dIr in dIrs)
                    {

                        List<ItemRequestDetail2ViewModel> ir2 = new List<ItemRequestDetail2ViewModel>();
                        var ddIrs = (from ir in db.tb_ir_detail2
                                     join it in db.tb_product on ir.ir_item_id equals it.product_id
                                     join u in db.tb_unit on it.product_unit equals u.Id
                                     orderby ir.ordering_number, it.product_code
                                     where ir.ir_detail1_id == dIr.ir_detail1_id
                                     select new
                                     {
                                         ir,it,u
                                     }).ToList();
                        if (ddIrs.Any())
                        {
                           
                                foreach (var i in ddIrs)
                                {
                                    dtr = tb.NewRow();
                                    dtr["Mr_No"] = request.i.ir_no;
                                    dtr["projectname"] =string.Format("{0},{1}", request.pj.project_full_name, request.pj.project_no);
                                  //  dtr["Purpose"] = request.purpose.purpose_name;
                                    // dtr["No"] = BT_KimMex.Class.CommonClass.GetUserFullname(obj.quote.updated_by);
                                    dtr["Item_code"] = i.it.product_code;
                                    dtr["Item_Name"] = i.it.product_name;
                                    dtr["re_Qty"] = i.ir.ir_qty;
                                    dtr["re_unit"] = i.u.Name;
                                    dtr["reason"] = i.ir.reason;
                                    dtr["AppQty"] = i.ir.approved_qty;
                                    dtr["remark"] = i.ir.remark;

                                //string remark = "Requested by " + CommonFunctions.GetUserFullnamebyUserId(request.i.created_by);
                                //if (!string.IsNullOrEmpty(CommonFunctions.GetUserFullnamebyUserId(request.i.approved_by)))
                                //{
                                //remark = remark + " and Approved by " + CommonFunctions.GetUserFullnamebyUserId(request.i.approved_by);
                                //}
                                dtr["Remark"] = request.i.expected_delivery_date.HasValue ? Convert.ToDateTime(request.i.expected_delivery_date).ToString("dd-MMM-yy") : "NA";


                                dtr["created_date"] =string.Format("{0},{1}", Convert.ToDateTime(request.i.created_date).ToString("dd-MMM-yy"),itemRequest.created_by);
                                dtr["Purpose"]= string.Format("{0},{1}", request.i.approved_date.HasValue ? Convert.ToDateTime(request.i.approved_date).ToString("dd-MMM-yy") : string.Empty, itemRequest.approved_by);

                                //    dtr["ApprovedDate"] = request.i.approved_date.HasValue ? Convert.ToDateTime(request.i.approved_date).ToString("dd/MM/yyyy") : string.Empty;
                                //dtr["CreatedBy"] = itemRequest.created_by;
                                //dtr["ApprovedBy"] = itemRequest.approved_by;
                                //dtr["SONo"] = request.pj.project_no;
                                //dtr["Modify_date"] = obj.quote.updated_date;

                                tb.Rows.Add(dtr);
                                }
                           
                        }
                    }


                }
                rv.LocalReport.ReportPath = reportPath;
                rv.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("Materailrequest", tb);
                rv.LocalReport.DataSources.Add(rdc);
                rv.LocalReport.Refresh();
            }
          //  return itemRequest;
        }

        public static void ExportExcelMR(ReportViewer rv, RadioButtonList rbFormat)
        {
            try
            {
                Warning[] warnings;
                string[] streamIds;
                string contentType;
                string encoding;
                string extension;
                //Export the RDLC Report to Byte Array.
                byte[] bytes = rv.LocalReport.Render(rbFormat.SelectedItem.Value, null, out contentType, out encoding, out extension, out streamIds, out warnings);

                //Download the RDLC Report in Word, Excel, PDF and Image formats.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.ContentType = contentType;
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=RDLC." + extension);
                HttpContext.Current.Response.BinaryWrite(bytes);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
            catch(Exception ex)
            {
                
            }
        }

        //kosal 12 May 2020

        public static void WorkOrderIsuseVSWorkOrderReturnReport(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status, string project)
        {
            try
            {
                List<Models.WorkOrderIsuseVSWorkOrderReturnReport> objs = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();

                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
                    DataColumn col = new DataColumn();
                    col.ColumnName = "item_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "datefrom";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "todate";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "project_name";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "issue_no";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "issue_item_code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "issue_description";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "issue_unit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "issue_qty";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "return_no";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "return_item_code";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "return_description";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "return_unit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "return_qty";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_type";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    //Added by Terd May 11 2020
                    if (string.IsNullOrEmpty(project))
                    {
                        objs = (from woi in db.tb_stock_issue
                                join pro in db.tb_project on woi.project_id equals pro.project_id
                                where string.Compare(woi.stock_issue_status, Status.Completed) == 0 && woi.status == true && (woi.created_date >= dateFrom && woi.created_date <= toDate)
                                select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
                                {
                                    issue_id = woi.stock_issue_id,
                                    issue_no = woi.stock_issue_number,
                                    project_id = woi.project_id,
                                    project_name = "All",
                                }).ToList();
                    }
                    else
                    {
                        objs = (from woi in db.tb_stock_issue
                                join pro in db.tb_project on woi.project_id equals pro.project_id
                                where woi.status == true && string.Compare(woi.project_id, project) == 0 && string.Compare(woi.stock_issue_status, Status.Completed) == 0 && woi.created_date >= dateFrom && woi.created_date <= toDate
                                select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
                                {
                                    issue_id = woi.stock_issue_id,
                                    issue_no = woi.stock_issue_number,
                                    project_id = woi.project_id,
                                    project_name = pro.project_full_name,
                                }).ToList();
                    }
                    if (objs.Any())
                    {
                        foreach (var obj in objs)
                        {
                            string issueReturnNUmber = string.Empty;
                            List<Models.InventoryDetailViewModel> returnItems = new List<Models.InventoryDetailViewModel>();
                            var issueReturns = db.tb_workorder_returned.Where(s => s.status == true && string.Compare(s.workorder_issued_id, obj.issue_id) == 0).ToList();
                            foreach (var issReturn in issueReturns)
                            {
                                issueReturnNUmber = string.Format("{0}{1},", issueReturnNUmber, issReturn.workorder_returned_number);
                                var oobjs = db.tb_inventory_detail.Where(s => string.Compare(s.inventory_ref_id, issReturn.workorder_returned_id) == 0).Select(s => new Models.InventoryDetailViewModel()
                                {
                                    inventory_item_id = s.inventory_item_id,
                                    quantity = s.quantity,
                                    unit = s.unit,
                                }).ToList();
                                foreach (var oobj in oobjs)
                                    returnItems.Add(oobj);
                            }
                            var issueItems = (from inv in db.tb_inventory
                                              join pro in db.tb_product on inv.product_id equals pro.product_id
                                              join un in db.tb_unit on pro.product_unit equals un.Id
                                              where string.Compare(inv.ref_id, obj.issue_id) == 0
                                              select new { inv, pro, un }).ToList();
                            foreach (var issitem in issueItems)
                            {
                                dtr = tb.NewRow();
                                dtr["item_id"] = issitem.inv.product_id;
                                dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
                                dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
                                dtr["project_name"] = obj.project_name;
                                dtr["issue_no"] = obj.issue_no;
                                dtr["issue_item_code"] = issitem.pro.product_code;
                                dtr["issue_description"] = issitem.pro.product_name;
                                dtr["issue_unit"] = issitem.un.Name;
                                dtr["issue_qty"] = issitem.inv.out_quantity;
                                dtr["return_no"] = issueReturnNUmber;
                                dtr["return_qty"] = returnItems.Where(s => string.Compare(issitem.inv.product_id, s.inventory_item_id) == 0).Sum(s => s.quantity);
                                tb.Rows.Add(dtr);
                            }
                        }
                    }
                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("WorkOrderIsuseVSWorkOrderReturn", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception ex) { }
        }
        //public static void WorkOrderIsuseVSWorkOrderReturnReportCopy(ReportViewer rv, string reportPath, DateTime dateFrom, DateTime dateTo, string status, string project)
        //{
        //    try
        //    {
        //        List<Models.WorkOrderIsuseVSWorkOrderReturnReport> objs = new List<Models.WorkOrderIsuseVSWorkOrderReturnReport>();

        //        DataTable tb = new DataTable();
        //        DataRow dtr;
        //        using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
        //        {
        //            DateTime toDate = Convert.ToDateTime(dateTo).AddDays(1);
        //            DataColumn col = new DataColumn();
        //            col.ColumnName = "item_id";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "datefrom";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "todate";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "project_name";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_no";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_item_code";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_description";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_unit";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "issue_qty";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_no";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_item_code";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_description";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_unit";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "return_qty";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();
        //            col.ColumnName = "inventory_type";
        //            tb.Columns.Add(col);
        //            col = new DataColumn();


        //            objs = (from woi in db.tb_stock_issue
        //                    join pro in db.tb_project on woi.project_id equals pro.project_id
        //                    where woi.project_id == project
        //                    select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                    ).ToList();



        //            if (string.Compare(project, "") == 0)
        //            {
        //                objs = (from woi in db.tb_stock_issue
        //                        join wor in db.tb_workorder_returned on woi.stock_issue_id equals wor.workorder_issued_id
        //                        where woi.stock_issue_status == "completed" && woi.status == true
        //                        select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                        {
        //                            project_name = "All",
        //                            issue_no = woi.stock_issue_number,
        //                            return_no = wor.workorder_returned_number,
        //                            status_i = wor.workorder_returned_status,
        //                            wrs = wor.status.ToString(),
        //                            issue_id = woi.stock_issue_id,
        //                            return_id = wor.workorder_returned_id,


        //                        }
        //                    ).ToList();
        //                foreach (var re in objs)
        //                {
        //                    var itemsR = (from inv in db.tb_inventory
        //                                  join pro in db.tb_product on inv.product_id equals pro.product_id
        //                                  join invD in db.tb_inventory_detail on inv.product_id equals invD.inventory_item_id
        //                                  join unit in db.tb_unit on pro.product_unit equals unit.Id
        //                                  //where inv.ref_id == re.issue_id && invD.inventory_ref_id == re.issue_id
        //                                  where inv.ref_id == re.issue_id && invD.inventory_type == "5" && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)
        //                                  //where inv.ref_id == re.issue_id && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)

        //                                  select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                  {
        //                                      issue_item_code = pro.product_code,
        //                                      issue_description = pro.product_name,
        //                                      issue_unit = unit.Name,
        //                                      issue_qty = invD.quantity.ToString(),
        //                                      return_qty = invD.quantity.ToString(),
        //                                  }).ToList();

        //                    var items = (from inv in db.tb_inventory
        //                                 join pro in db.tb_product on inv.product_id equals pro.product_id
        //                                 join invD in db.tb_inventory_detail on inv.product_id equals invD.inventory_item_id
        //                                 join unit in db.tb_unit on pro.product_unit equals unit.Id
        //                                 where inv.ref_id == re.issue_id && (invD.inventory_ref_id == re.issue_id || invD.inventory_ref_id == re.return_id)

        //                                 select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                                 {
        //                                     issue_item_code = pro.product_code,
        //                                     issue_description = pro.product_name,
        //                                     issue_unit = unit.Name,
        //                                     issue_qty = invD.quantity.ToString(),
        //                                     return_qty = invD.quantity.ToString(),

        //                                 }).ToList();

        //                    var numbers = new[] { itemsR };
        //                    var words = new[] { items };
        //                    foreach (var item in itemsR)
        //                    {
        //                        dtr = tb.NewRow();
        //                        dtr["item_id"] = "";
        //                        dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                        dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                        dtr["project_name"] = re.project_name;
        //                        dtr["issue_no"] = re.issue_no;
        //                        dtr["issue_item_code"] = item.issue_item_code;
        //                        dtr["issue_description"] = item.issue_description;
        //                        dtr["issue_unit"] = item.issue_unit;
        //                        dtr["issue_qty"] = item.issue_qty;
        //                        if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                        {
        //                            dtr["return_no"] = re.return_no;
        //                        }
        //                        else
        //                        {
        //                            dtr["return_no"] = "";
        //                            dtr["return_qty"] = "";
        //                        }
        //                        tb.Rows.Add(dtr);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                objs = (from proj in db.tb_project
        //                        join woi in db.tb_stock_issue on proj.project_id equals woi.project_id
        //                        join wor in db.tb_workorder_returned on woi.stock_issue_id equals wor.workorder_issued_id

        //                        where proj.project_id == project && woi.stock_issue_status == "completed" && woi.status == true
        //                        select new Models.WorkOrderIsuseVSWorkOrderReturnReport()
        //                        {
        //                            project_name = proj.project_full_name,
        //                            issue_no = woi.stock_issue_number,
        //                            return_no = wor.workorder_returned_number,
        //                            status_i = wor.workorder_returned_status,
        //                            wrs = wor.status.ToString(),

        //                        }
        //                    ).ToList();
        //                foreach (var re in objs)
        //                {

        //                    dtr = tb.NewRow();
        //                    dtr["item_id"] = "";
        //                    dtr["datefrom"] = Convert.ToDateTime(dateFrom).ToString("dd-MMM-yyyy");
        //                    dtr["todate"] = Convert.ToDateTime(dateTo).ToString("dd-MMM-yyyy");
        //                    dtr["project_name"] = re.project_name;
        //                    dtr["issue_no"] = re.issue_no;
        //                    dtr["issue_item_code"] = "";
        //                    dtr["issue_description"] = "";
        //                    dtr["issue_unit"] = "";
        //                    dtr["issue_qty"] = "";
        //                    if ((string.Compare(re.status_i, "completed") == 0) && (string.Compare(re.wrs, "True") == 0))
        //                    {
        //                        dtr["return_no"] = re.return_no;
        //                        dtr["return_item_code"] = "";
        //                        dtr["return_description"] = "";
        //                        dtr["return_unit"] = "";
        //                        dtr["return_qty"] = "";

        //                    }
        //                    else
        //                    {
        //                        dtr["return_no"] = "";
        //                        dtr["return_item_code"] = "";
        //                        dtr["return_description"] = "";
        //                        dtr["return_unit"] = "";
        //                        dtr["return_qty"] = "";

        //                    }

        //                    tb.Rows.Add(dtr);


        //                }
        //            }







        //            rv.LocalReport.ReportPath = reportPath;
        //            rv.LocalReport.DataSources.Clear();
        //            ReportDataSource rdc = new ReportDataSource("WorkOrderIsuseVSWorkOrderReturn", tb);
        //            rv.LocalReport.DataSources.Add(rdc);
        //            rv.LocalReport.Refresh();
        //        }
        //    }
        //    catch (Exception ex) { }
        //}


        //kosal 13May2020
        public static void GenerateMonthlyStockBalancecopy2(ReportViewer rv, string reportPath, DateTime date, string WarehouseId, string project)
        {
            try
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    //Rathana Add 24.04.2019

                    col = new DataColumn();
                    col.ColumnName = "IN";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "transfer_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_increase";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_decrease";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "internal_usage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "sela";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "amount";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "upd_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "projectName";
                    tb.Columns.Add(col);

                    //End Rathana Add
                    List<Models.InventoryViewModel> obj = new List<Models.InventoryViewModel>();
                    IQueryable<Models.InventoryViewModel> objs;
                    List<Models.StockBalanceMonthyViewModel> originalItems = new List<Models.StockBalanceMonthyViewModel>();
                    List<Models.StockBalanceMonthyViewModel> stockBalances = new List<Models.StockBalanceMonthyViewModel>();
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


                    var projectname = (from p in db.tb_project
                                       where p.project_id == project
                                       select p
                                       ).FirstOrDefault();
                    if (project == "All" && WarehouseId == "")
                    {
                        obj = (from w in db.tb_site
                               join p in db.tb_project on w.site_id equals p.site_id
                               join ws in db.tb_warehouse on p.project_id equals ws.warehouse_project_id
                               select new Models.InventoryViewModel
                               {
                                   warehouse_id = ws.warehouse_id,
                                   product_id = p.project_id
                               }
                                  ).ToList();
                    }
                    else if (project == "All" && WarehouseId != "")
                    {
                        obj = (from w in db.tb_site
                                  join p in db.tb_project on w.site_id equals p.site_id
                                  join ws in db.tb_warehouse on p.project_id equals ws.warehouse_project_id
                                  where w.site_id == WarehouseId
                                  select new Models.InventoryViewModel
                                  {
                                      warehouse_id = ws.warehouse_id,
                                      product_id = p.project_id
                                  }
                                  ).ToList();
                    }
                    else if (project != "All" && WarehouseId != "")
                    {
                        var projectnames = (from p in db.tb_project
                                            where p.project_id == project
                                            select p
                                       ).FirstOrDefault();
                        obj = (from w in db.tb_warehouse
                               where w.warehouse_project_id == projectname.project_id
                               select new Models.InventoryViewModel
                               {
                                   warehouse_id = w.warehouse_id,
                               }
                                  ).ToList();
                    }
                    else
                    {
                        var projectnames = (from p in db.tb_project
                                            where p.project_id == project
                                            select p
                                       ).FirstOrDefault();
                        obj = (from w in db.tb_warehouse
                                  where w.warehouse_project_id == projectname.project_id
                                  select new Models.InventoryViewModel
                                  {
                                      warehouse_id = w.warehouse_id,
                                  }
                                  ).ToList();
                    }

               
                        foreach(var re in obj)
                        {
                            objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, re.warehouse_id) == 0 && x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth).Select(x => new Models.InventoryViewModel()
                            {
                                inventory_id = x.inventory_id,
                                inventory_date = x.inventory_date,
                                inventory_status_id = x.inventory_status_id,
                                warehouse_id = x.warehouse_id,
                                product_id = x.product_id,
                                total_quantity = x.total_quantity,
                                in_quantity = x.in_quantity,
                                out_quantity = x.out_quantity,
                            });
                            if (objs.Any())
                            {
                                double BigBalance = 0;
                                double EndingBalance = 0;
                                double ReceiveBalance = 0;
                                double IssueReturnBalance = 0;
                                double ReturnBalance = 0;
                                double TransferBalance = 0;
                                double DamangeBalance = 0;
                                double IssueBalance = 0;
                                double TransferIn = 0;
                                double IN = 0;
                                double UPrice = 0;

                            var duplicateItems = objs.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (duplicateItems.Any())
                            {
                                foreach (var dupitem in duplicateItems)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    var items = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                    BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                    EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);
                                    foreach (var item in items)
                                    {
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                        }


                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == re.warehouse_id &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == re.warehouse_id
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    }
                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = dupitem,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice
                                    });
                                }

                                foreach (var item in objs)
                                {
                                    var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                    if (isDuplicate == null)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == re.warehouse_id &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == re.warehouse_id
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in objs)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                    EndingBalance = Convert.ToDouble(item.total_quantity);
                                    //stock in 
                                    if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                    {
                                        ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                    {
                                        IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    //stock out
                                    else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                    {
                                        ReturnBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                    {
                                        TransferBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                    {
                                        DamangeBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                    {
                                        IssueBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == re.warehouse_id &&
                                                                            x.item_status == "approved").FirstOrDefault().quantity ?? 0);
                                    IN = (double)((from PO in db.tb_purchase_order_detail
                                                   join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                   join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                   join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                   join S in db.tb_site on P.site_id equals S.site_id
                                                   join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                   where PO.item_status == "approved" &&
                                                   PO.item_id == item.product_id &&
                                                         (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                         POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                         W.warehouse_id == re.warehouse_id
                                                   select PO.quantity).FirstOrDefault() ?? 0
                                                          );
                                    UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.product_id,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice
                                    });
                                }
                            }



                            List<Models.StockBalanceMonthyViewModel> secondHandItems = new List<Models.StockBalanceMonthyViewModel>();
                                var duplicateItemInventory = originalItems.GroupBy(x => x.ItemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                if (duplicateItemInventory.Any())
                                {
                                    foreach (var item in duplicateItemInventory)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        var dupItems = originalItems.Where(x => string.Compare(x.ItemId, item) == 0).ToList();
                                        foreach (var ii in dupItems)
                                        {
                                            BigBalance = BigBalance + ii.BigBalance;
                                            EndingBalance = EndingBalance + ii.EndingBalance;
                                            ReceiveBalance = ReceiveBalance + ii.ReceivedBalance;
                                            IssueReturnBalance = IssueReturnBalance + ii.IssueReturnBalance;
                                            ReturnBalance = ReturnBalance + ii.ReturnBalance;
                                            TransferBalance = TransferBalance + ii.TransferBalance;
                                            DamangeBalance = DamangeBalance + ii.DamangeBalance;
                                            IssueBalance = IssueBalance + ii.IssueBalance;
                                            TransferIn = TransferIn + ii.TransferIn;
                                            IN = IN + ii.IN;
                                        }
                                        secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }

                                    foreach (var item in originalItems)
                                    {
                                        var isDuplicate = duplicateItemInventory.Where(x => string.Compare(x, item.ItemId) == 0).FirstOrDefault();
                                        if (isDuplicate == null)
                                        {
                                            secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                            {
                                                ItemId = item.ItemId,
                                                BigBalance = item.BigBalance,
                                                EndingBalance = item.EndingBalance,
                                                ReceivedBalance = item.ReceivedBalance,
                                                IssueReturnBalance = item.IssueReturnBalance,
                                                ReturnBalance = item.ReturnBalance,
                                                TransferBalance = item.TransferBalance,
                                                DamangeBalance = item.DamangeBalance,
                                                IssueBalance = item.IssueBalance,
                                                TransferIn = TransferIn,
                                                IN = IN,
                                                UPrice = item.UPrice,
                                            });
                                        }
                                    }
                                }
                                else
                                    secondHandItems = originalItems;
                                foreach (var item in secondHandItems)
                                {
                                    var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                                    stockBalances.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.ItemId,
                                        ItemCode = product.product_code,
                                        ItemName = product.product_name,
                                        ItemUnit = product.product_unit,
                                        BigBalance = item.BigBalance,
                                        EndingBalance = item.EndingBalance,
                                        ReceivedBalance = item.ReceivedBalance,
                                        IssueReturnBalance = item.IssueReturnBalance,
                                        ReturnBalance = item.ReturnBalance,
                                        TransferBalance = item.TransferBalance,
                                        DamangeBalance = item.DamangeBalance,
                                        IssueBalance = item.IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        //UPrice = UPrice
                                        UPrice = item.UPrice,
                                    });
                                }
                                stockBalances = stockBalances.OrderBy(x => x.ItemCode).ToList();

                                foreach (var item in stockBalances)
                                {
                                    var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                                    dtr = tb.NewRow();
                                    dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                                    //dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();

                                    var k = (from i in db.tb_project
                                             join s in db.tb_site on i.site_id equals s.site_id
                                             where i.project_id == project
                                             select s
                                             ).FirstOrDefault();



                                    // dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_site.Where(x => x.site_id == k.site_id).Select(x => x.site_name).FirstOrDefault();
                                    dtr["itemCode"] = product.product_code;
                                    dtr["itemName"] = product.product_name;
                                    dtr["itemUnit"] = product.product_unit;
                                    dtr["bigbalance"] = item.BigBalance;
                                    dtr["ending_balance"] = item.EndingBalance;
                                    dtr["in_receive"] = item.ReceivedBalance;
                                    dtr["in_issue_return"] = item.IssueReturnBalance;
                                    dtr["out_return"] = item.ReturnBalance;
                                    dtr["out_transfer"] = item.TransferBalance;
                                    dtr["out_damage"] = item.DamangeBalance;
                                    dtr["out_issue"] = item.IssueBalance;
                                    dtr["total_in"] = item.ReceivedBalance + item.IssueReturnBalance;
                                    dtr["total_out"] = item.ReturnBalance + item.TransferBalance + item.DamangeBalance + item.IssueBalance;

                                    //Rathana Add 24.04.2019
                                    double adjustentQuantity = item.EndingBalance - item.ReceivedBalance;
                                    if (adjustentQuantity > 0)
                                    {
                                        dtr["adjust_increase"] = adjustentQuantity;
                                        dtr["adjust_decrease"] = 0;
                                    }
                                    else
                                    {
                                        dtr["adjust_decrease"] = -(adjustentQuantity);
                                        dtr["adjust_increase"] = 0;
                                    }
                                    dtr["transfer_in"] = item.TransferIn;
                                    dtr["IN"] = item.IN;
                                    dtr["unit_price"] = item.UPrice;
                                    dtr["amount"] = item.EndingBalance * item.UPrice;

                                    if (project == "All")
                                    {
                                        dtr["projectName"] = "All";
                                    }
                                    else
                                    {

                                        dtr["projectName"] = projectname.project_full_name;
                                    }
                                    //End Rathana Add
                                    tb.Rows.Add(dtr);
                                }
                            }

                        }



                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception e) { }
        }
        public static void GenerateMonthlyStockBalance(ReportViewer rv, string reportPath, DateTime date, string WarehouseId, string project,int report=0)
        {
            try
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    //Rathana Add 24.04.2019

                    col = new DataColumn();
                    col.ColumnName = "IN";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "transfer_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_increase";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_decrease";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "internal_usage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "sela";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "amount";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "upd_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "projectName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "labour_hour";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "start_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "end_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "generate_date";
                    tb.Columns.Add(col);

                    IQueryable<Models.InventoryViewModel> objs;
                    List<Models.StockBalanceMonthyViewModel> originalItems = new List<Models.StockBalanceMonthyViewModel>();
                    List<Models.StockBalanceMonthyViewModel> stockBalances = new List<Models.StockBalanceMonthyViewModel>();
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1);

                    /* New Enhancement 20230505 Milstone1 */
                    //var projectname = (from p in db.tb_project
                    //                   where p.project_id == project
                    //                   select p
                    //                   ).FirstOrDefault();

                    
                    project = string.IsNullOrEmpty(project) ? "All" : project;
                    //if (project == "All" && string.IsNullOrEmpty(WarehouseId)){

                    //}

                    if (string.Compare(project,"All")==0 && !string.IsNullOrEmpty(WarehouseId))
                    {
                        //var projectnames = (from p in db.tb_project
                        //                    where p.project_id == project
                        //                    select p
                        //               ).FirstOrDefault();
                        var wh = (from w in db.tb_site
                                  join p in db.tb_project on w.site_id equals p.site_id
                                  join ws in db.tb_warehouse on p.project_id equals ws.warehouse_project_id

                                  where w.site_id == WarehouseId
                                  select new {w , p , ws}
                                  ).FirstOrDefault();
                        WarehouseId = wh.w.site_id;
                    }

                    else if (string.Compare(project, "All") != 0 && !string.IsNullOrEmpty(WarehouseId))
                    {

                        var wh = (from w in db.tb_warehouse
                                  where w.warehouse_project_id == project
                                  select w
                                  ).FirstOrDefault();
                        WarehouseId = wh.warehouse_id;
                    }


                    /* New Enhancement 20230505 Milstone1 by Irith */
                    //var projectObj = db.tb_project.Find(project);
                    //if (projectObj != null)
                    //{
                    //    WarehouseId = db.tb_warehouse.Where(s => s.warehouse_status == true && string.Compare(s.warehouse_project_id, projectObj.project_id) == 0).Select(s => s.warehouse_id).FirstOrDefault();
                    //}

                    if (string.IsNullOrEmpty(WarehouseId))
                        objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth).Select(x => new Models.InventoryViewModel()
                        {
                            inventory_id = x.inventory_id,
                            inventory_date = x.inventory_date,
                            inventory_status_id = x.inventory_status_id,
                            warehouse_id = x.warehouse_id,
                            product_id = x.product_id,
                            total_quantity = x.total_quantity,
                            in_quantity = x.in_quantity,
                            out_quantity = x.out_quantity,
                        });
                    else if (!string.IsNullOrEmpty(WarehouseId) && project == "All")
                        objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth )
                            .Select(x => new Models.InventoryViewModel()
                        {
                            inventory_id = x.inventory_id,
                            inventory_date = x.inventory_date,
                            inventory_status_id = x.inventory_status_id,
                            warehouse_id = x.warehouse_id,
                            product_id = x.product_id,
                            total_quantity = x.total_quantity,
                            in_quantity = x.in_quantity,
                            out_quantity = x.out_quantity,
                        });
                    else
                        objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, WarehouseId) == 0 && x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth && string.Compare(x.inventory_status_id,"1")!=0).Select(x => new Models.InventoryViewModel()
                        {
                            inventory_id = x.inventory_id,
                            inventory_date = x.inventory_date,
                            inventory_status_id = x.inventory_status_id,
                            warehouse_id = x.warehouse_id,
                            product_id = x.product_id,
                            total_quantity = x.total_quantity,
                            in_quantity = x.in_quantity,
                            out_quantity = x.out_quantity,
                        });

                    if (objs.Any())
                    {
                        double BigBalance = 0;
                        double EndingBalance = 0;
                        double ReceiveBalance = 0;
                        double IssueReturnBalance = 0;
                        double ReturnBalance = 0;
                        double TransferBalance = 0;
                        double DamangeBalance = 0;
                        double IssueBalance = 0;
                        double TransferIn = 0;
                        double IN = 0;
                        double UPrice = 0;
                        double TotalIN = 0;
                        double TotalOut = 0;

                        if (string.IsNullOrEmpty(WarehouseId))
                        {
                            var allwarehouses = db.tb_warehouse.Where(x => x.warehouse_status == true).Select(x => x.warehouse_id).ToList();
                            foreach (var warehouse in allwarehouses)
                            {
                                var inventoryItems = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();

                                var duplicateItems = inventoryItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                if (duplicateItems.Any())
                                {
                                    foreach (var dupitem in duplicateItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        var items = inventoryItems.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                        BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                        EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);

                                        foreach (var item in items)
                                        {
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail
                                                                .Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved")
                                                                .Sum(x => x.quantity) ?? 0);

                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                                 PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);
                                        }

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = dupitem,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }

                                    foreach (var item in inventoryItems)
                                    {
                                        var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                        if (isDuplicate == null)
                                        {
                                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                            BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                            EndingBalance = Convert.ToDouble(item.total_quantity);
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                           PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                            originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                            {
                                                ItemId = item.product_id,
                                                BigBalance = BigBalance,
                                                EndingBalance = EndingBalance,
                                                ReceivedBalance = ReceiveBalance,
                                                IssueReturnBalance = IssueReturnBalance,
                                                ReturnBalance = ReturnBalance,
                                                TransferBalance = TransferBalance,
                                                DamangeBalance = DamangeBalance,
                                                IssueBalance = IssueBalance,
                                                TransferIn = TransferIn,
                                                IN = IN,
                                                UPrice = UPrice
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in inventoryItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(WarehouseId) && project == "All")
                        {
                            var allwarehouses = db.tb_warehouse.Where(x => x.warehouse_status == true).Select(x => x.warehouse_id).ToList();
                            foreach (var warehouse in allwarehouses)
                            {
                                var inventoryItems = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();

                                var duplicateItems = inventoryItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                if (duplicateItems.Any())
                                {
                                    foreach (var dupitem in duplicateItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        var items = inventoryItems.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                        BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                        EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);

                                        foreach (var item in items)
                                        {
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail
                                                                .Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved")
                                                                .Sum(x => x.quantity) ?? 0);

                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                                 PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);
                                        }

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = dupitem,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }

                                    foreach (var item in inventoryItems)
                                    {
                                        var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                        if (isDuplicate == null)
                                        {
                                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                            BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                            EndingBalance = Convert.ToDouble(item.total_quantity);
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                           PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                            originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                            {
                                                ItemId = item.product_id,
                                                BigBalance = BigBalance,
                                                EndingBalance = EndingBalance,
                                                ReceivedBalance = ReceiveBalance,
                                                IssueReturnBalance = IssueReturnBalance,
                                                ReturnBalance = ReturnBalance,
                                                TransferBalance = TransferBalance,
                                                DamangeBalance = DamangeBalance,
                                                IssueBalance = IssueBalance,
                                                TransferIn = TransferIn,
                                                IN = IN,
                                                UPrice = UPrice
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in inventoryItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }
                                }
                            }
                        }

                        else
                        {
                            var duplicateItems = objs.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (duplicateItems.Any())
                            {
                                foreach (var dupitem in duplicateItems)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    TotalIN = 0; TotalOut = 0;
                                    var items = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                    BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                    EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);
                                    foreach (var item in items)
                                    {
                                        if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                                        {
                                            TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                            TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                                        }
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                        }


                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == WarehouseId
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    }
                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = dupitem,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice,
                                        TotalIn = TotalIN,
                                        TotalOut=TotalOut,
                                    }) ;
                                }

                                foreach (var item in objs)
                                {
                                    var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                    if (isDuplicate == null)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        TotalIN = 0;TotalOut = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                                        {
                                            TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                            TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                                        }
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == WarehouseId
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice,
                                            TotalIn = TotalIN,
                                            TotalOut=TotalOut
                                        });
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in objs)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    TotalIN = 0;
                                    TotalOut = 0;

                                    BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                    EndingBalance = Convert.ToDouble(item.total_quantity);
                                    if (string.Compare(item.inventory_status_id, "1") != 0) //except stock adjustment
                                    {
                                        TotalIN = TotalIN + Convert.ToDouble(item.in_quantity);
                                        TotalOut = TotalOut + Convert.ToDouble(item.out_quantity);
                                    }
                                    //stock in 
                                    if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                    {
                                        ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                    {
                                        IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    //stock out
                                    else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                    {
                                        ReturnBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                    {
                                        TransferBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                    {
                                        DamangeBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                    {
                                        IssueBalance = Convert.ToDouble(item.out_quantity);
                                    }

                                    var TransferInObj = db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").FirstOrDefault();
                                    if (TransferInObj != null)
                                        TransferIn =(double)TransferInObj.quantity;

                                    IN = (double)((from PO in db.tb_purchase_order_detail
                                                   join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                   join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                   join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                   join S in db.tb_site on P.site_id equals S.site_id
                                                   join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                   where PO.item_status == "approved" &&
                                                   PO.item_id == item.product_id &&
                                                         (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                         POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                         W.warehouse_id == WarehouseId
                                                   select PO.quantity).FirstOrDefault() ?? 0
                                                          );
                                    UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.product_id,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice,
                                        TotalIn=TotalIN,
                                        TotalOut=TotalOut
                                    });
                                }
                            }
                        }


                        List<Models.StockBalanceMonthyViewModel> secondHandItems = new List<Models.StockBalanceMonthyViewModel>();
                        var duplicateItemInventory = originalItems.GroupBy(x => x.ItemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateItemInventory.Any())
                        {
                            foreach (var item in duplicateItemInventory)
                            {
                                BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                TotalIN = 0;TotalOut = 0;
                                var dupItems = originalItems.Where(x => string.Compare(x.ItemId, item) == 0).ToList();
                                foreach (var ii in dupItems)
                                {
                                    BigBalance = BigBalance + ii.BigBalance;
                                    EndingBalance = EndingBalance + ii.EndingBalance;
                                    ReceiveBalance = ReceiveBalance + ii.ReceivedBalance;
                                    IssueReturnBalance = IssueReturnBalance + ii.IssueReturnBalance;
                                    ReturnBalance = ReturnBalance + ii.ReturnBalance;
                                    TransferBalance = TransferBalance + ii.TransferBalance;
                                    DamangeBalance = DamangeBalance + ii.DamangeBalance;
                                    IssueBalance = IssueBalance + ii.IssueBalance;
                                    TransferIn = TransferIn + ii.TransferIn;
                                    IN = IN + ii.IN;
                                    TotalIN=TotalIN+ii.TotalIn;
                                    TotalOut = TotalOut = ii.TotalOut;
                                }
                                secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                {
                                    ItemId = item,
                                    BigBalance = BigBalance,
                                    EndingBalance = EndingBalance,
                                    ReceivedBalance = ReceiveBalance,
                                    IssueReturnBalance = IssueReturnBalance,
                                    ReturnBalance = ReturnBalance,
                                    TransferBalance = TransferBalance,
                                    DamangeBalance = DamangeBalance,
                                    IssueBalance = IssueBalance,
                                    TransferIn = TransferIn,
                                    IN = IN,
                                    UPrice = UPrice,
                                    TotalIn=TotalIN,
                                    TotalOut=TotalOut
                                });
                            }

                            foreach (var item in originalItems)
                            {
                                var isDuplicate = duplicateItemInventory.Where(x => string.Compare(x, item.ItemId) == 0).FirstOrDefault();
                                if (isDuplicate == null)
                                {
                                    secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.ItemId,
                                        BigBalance = item.BigBalance,
                                        EndingBalance = item.EndingBalance,
                                        ReceivedBalance = item.ReceivedBalance,
                                        IssueReturnBalance = item.IssueReturnBalance,
                                        ReturnBalance = item.ReturnBalance,
                                        TransferBalance = item.TransferBalance,
                                        DamangeBalance = item.DamangeBalance,
                                        IssueBalance = item.IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = item.UPrice,
                                        TotalIn=item.TotalIn,
                                        TotalOut=item.TotalOut
                                    });
                                }
                            }
                        }
                        else
                            secondHandItems = originalItems;
                        foreach (var item in secondHandItems)
                        {
                            var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                            stockBalances.Add(new Models.StockBalanceMonthyViewModel()
                            {
                                ItemId = item.ItemId,
                                ItemCode = product.product_code,
                                ItemName = product.product_name,
                                ItemNumber = product.product_number,
                                ItemUnit = product.product_unit,
                                BigBalance = item.BigBalance,
                                EndingBalance = item.EndingBalance,
                                ReceivedBalance = item.ReceivedBalance,
                                IssueReturnBalance = item.IssueReturnBalance,
                                ReturnBalance = item.ReturnBalance,
                                TransferBalance = item.TransferBalance,
                                DamangeBalance = item.DamangeBalance,
                                IssueBalance = item.IssueBalance,
                                TransferIn = TransferIn,
                                IN = IN,
                                //UPrice = UPrice
                                UPrice = item.UPrice,
                                TotalOut=item.TotalOut,
                                TotalIn=item.TotalIn
                            }); ;
                        }
                        stockBalances = stockBalances.OrderBy(x => x.ItemNumber).ToList();

                        var projectObj = db.tb_project.Find(project);

                        foreach (var item in stockBalances)
                        {
                            var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                            //dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            if (string.IsNullOrEmpty(WarehouseId))
                            {
                                dtr["warehouseName"] = "All";
                            }
                            else if (project != "All")
                            {
                                var k = (from i in db.tb_project
                                         join s in db.tb_site on i.site_id equals s.site_id
                                         where i.project_id == project
                                         select s
                                     ).FirstOrDefault();
                                dtr["warehouseName"] = db.tb_site.Where(x => x.site_id == k.site_id).Select(x => x.site_name).FirstOrDefault();
                            }
                            else
                            {
                                dtr["warehouseName"] = db.tb_site.Where(x => x.site_id == WarehouseId).Select(x => x.site_name).FirstOrDefault();
                            }
                            //  dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_site.Where(x => x.site_id == k.site_id).Select(x => x.site_name).FirstOrDefault();
                            
                            //update formula
                            

                            dtr["itemCode"] = product.product_code;
                            dtr["itemName"] = product.product_name;
                            dtr["itemUnit"] = product.product_unit;
                            dtr["labour_hour"] = product.labour_hour;
                            dtr["bigbalance"] = item.BigBalance;
                            //update formula
                            dtr["ending_balance"] = item.EndingBalance;
                            dtr["ending_balance"] =( item.BigBalance + item.TotalIn)-item.TotalOut;
                            dtr["in_receive"] = item.ReceivedBalance;
                            dtr["in_issue_return"] = item.IssueReturnBalance;
                            dtr["out_return"] = item.ReturnBalance;
                            dtr["out_transfer"] = item.TransferBalance;
                            dtr["out_damage"] = item.DamangeBalance;
                            dtr["out_issue"] = item.IssueBalance;
                            //dtr["total_in"] = item.ReceivedBalance + item.IssueReturnBalance;
                            //dtr["total_out"] = item.ReturnBalance + item.TransferBalance + item.DamangeBalance + item.IssueBalance;
                            dtr["total_in"] = item.TotalIn;
                            dtr["total_out"] = item.TotalOut;

                            //Rathana Add 24.04.2019
                            double adjustentQuantity = item.EndingBalance - item.ReceivedBalance;
                            if (adjustentQuantity > 0)
                            {
                                dtr["adjust_increase"] = adjustentQuantity;
                                dtr["adjust_decrease"] = 0;
                            }
                            else
                            {
                                dtr["adjust_decrease"] = -(adjustentQuantity);
                                dtr["adjust_increase"] = 0;
                            }
                            dtr["transfer_in"] = item.TransferIn;
                            dtr["IN"] = item.IN;
                            dtr["unit_price"] = item.UPrice;
                            dtr["amount"] = item.EndingBalance * item.UPrice;

                            if (project == "All")
                            {
                                dtr["projectName"] = "All";
                            }
                            else
                            {

                                dtr["projectName"] = projectObj.project_full_name;
                            }
                            dtr["generate_date"] = CommonClass.ToLocalTime(DateTime.Now).ToString("dd-MMM-yyyy");
                            dtr["start_date"] = CommonClass.ToLocalTime(firstDayOfMonth).ToString("dd-MMM-yyyy");
                            dtr["end_date"] = CommonClass.ToLocalTime(lastDayOfMonth).ToString("dd-MMM-yyyy");
                            //End Rathana Add
                            tb.Rows.Add(dtr);
                        }
                    }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.SizeToReportContent = true;
                    rv.Width = Unit.Percentage(100);
                    rv.Height = Unit.Percentage(100);

                    rv.LocalReport.ReportPath = reportPath;

                    rv.ShowPrintButton = true;
                    rv.ShowRefreshButton = true;

                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception e) { }
        }

        public static void GenerateStockBalanceMonthlyByWarehouse(ReportViewer rv, string reportPath,string warerhouseId,DateTime dateFrom,DateTime dateTo)
        {
            try
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    //Rathana Add 24.04.2019

                    col = new DataColumn();
                    col.ColumnName = "IN";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "transfer_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_increase";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_decrease";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "internal_usage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "sela";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "amount";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "upd_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "projectName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "labour_hour";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "start_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "end_date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "generate_date";
                    tb.Columns.Add(col);

                    DateTime newDateFrom = Convert.ToDateTime(dateFrom);
                    DateTime newDateTo = Convert.ToDateTime(dateTo);
                    //var results= InventoryViewModel.GetStockBalancebyWarehouse(warehouseId,newDateFrom,newDateTo);
                    var results = InventoryViewModel.GetStockBalanceWarehouseV2(warerhouseId, newDateFrom, newDateTo);
                    foreach(var rs in results)
                    {
                        tb_warehouse warehouse= db.tb_warehouse.Where(x => x.warehouse_id == warerhouseId).FirstOrDefault();
                        dtr = tb.NewRow();
                        dtr["date"] = Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy");
                        dtr["warehouseName"] = string.IsNullOrEmpty(warerhouseId) ? string.Empty : warehouse.warehouse_name;
                        

                        //update formula


                        dtr["itemCode"] = rs.itemCode;
                        dtr["itemName"] = rs.itemName;
                        dtr["itemUnit"] = rs.itemUnit;
                        dtr["labour_hour"] = rs.labourHour;
                        dtr["bigbalance"] = rs.bigBalance;
                        //update formula
                        dtr["ending_balance"] = rs.endingBalance;
                        dtr["ending_balance"] =Convert.ToDecimal(rs.bigBalance + rs.totalIn) - Convert.ToDecimal(rs.totalOut);
                        dtr["in_receive"] = rs.inReceivedBalance;
                        dtr["in_issue_return"] = rs.inIssueReturnBalance;
                        dtr["out_return"] = rs.outReturnBalance;
                        dtr["out_transfer"] = rs.outTransferBalance;
                        dtr["out_damage"] = rs.outDamageBalance;
                        dtr["out_issue"] = rs.outIssueBalance;
                        //dtr["total_in"] =  + item.IssueReturnBalance;
                        //dtr["total_out"] = item.ReturnBalance + item.TransferBalance + item.DamangeBalance + item.IssueBalance;
                        dtr["total_in"] = rs.totalIn;
                        dtr["total_out"] = rs.totalOut;

                        //Rathana Add 24.04.2019
                        double adjustentQuantity =Convert.ToDouble(rs.endingBalance) - Convert.ToDouble(rs.inReceivedBalance);
                        if (adjustentQuantity > 0)
                        {
                            dtr["adjust_increase"] = adjustentQuantity;
                            dtr["adjust_decrease"] = 0;
                        }
                        else
                        {
                            dtr["adjust_decrease"] = -(adjustentQuantity);
                            dtr["adjust_increase"] = 0;
                        }
                        dtr["transfer_in"] = rs.inTransfer;
                        dtr["IN"] = rs.inn;
                        dtr["unit_price"] = rs.unitPrice;
                        dtr["amount"] =Convert.ToDecimal(rs.endingBalance) * Convert.ToDecimal(rs.unitPrice);

                        //if (project == "All")
                        //{
                        //    dtr["projectName"] = "All";
                        //}
                        //else
                        //{

                        //    dtr["projectName"] = projectObj.project_full_name;
                        //}
                        dtr["generate_date"] = CommonClass.ToLocalTime(DateTime.Now).ToString("dd-MMM-yyyy");
                        dtr["start_date"] = CommonClass.ToLocalTime(dateFrom).ToString("dd-MMM-yyyy");
                        dtr["end_date"] = CommonClass.ToLocalTime(dateTo).ToString("dd-MMM-yyyy");

                        tb_project project = db.tb_project.Where(s => string.Compare(s.project_id, warehouse.warehouse_project_id) == 0).FirstOrDefault();
                        if (project != null)
                        {
                            dtr["projectName"] = project.project_full_name;
                        }

                        tb.Rows.Add(dtr);
                    }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.SizeToReportContent = true;
                    rv.Width = Unit.Percentage(100);
                    rv.Height = Unit.Percentage(100);

                    rv.LocalReport.ReportPath = reportPath;

                    rv.ShowPrintButton = true;
                    rv.ShowRefreshButton = true;

                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch(Exception ex)
            {

            }
        }
        
        //kosal
        public static void GenerateMonthlyStockBalanceCopy1(ReportViewer rv, string reportPath, DateTime date, string WarehouseId, string project)
        {
            try
            {
                DataTable tb = new DataTable();
                DataRow dtr;
                using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "dateinventory";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemUnit";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouseName";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "unit_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "date";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "itemCode";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "bigbalance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_receive";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "in_issue_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_return";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_transfer";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_damage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "out_issue";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "total_out";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "ending_balance";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "inventory_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "stock_status_id";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "warehouse_id";
                    tb.Columns.Add(col);

                    //Rathana Add 24.04.2019

                    col = new DataColumn();
                    col.ColumnName = "IN";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "transfer_in";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_increase";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "adjust_decrease";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "internal_usage";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "sela";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "amount";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "upd_price";
                    tb.Columns.Add(col);
                    col = new DataColumn();
                    col.ColumnName = "projectName";
                    tb.Columns.Add(col);

                    //End Rathana Add

                    IQueryable<Models.InventoryViewModel> objs;
                    List<Models.StockBalanceMonthyViewModel> originalItems = new List<Models.StockBalanceMonthyViewModel>();
                    List<Models.StockBalanceMonthyViewModel> stockBalances = new List<Models.StockBalanceMonthyViewModel>();
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);



                    var projectname = (from p in db.tb_project
                                       where p.project_id == project
                                       select p
                                       ).FirstOrDefault();
                    if (project == "All")
                    {

                    }
                    else
                    {
                        var projectnames = (from p in db.tb_project
                                            where p.project_id == project
                                            select p
                                       ).FirstOrDefault();
                        var wh = (from w in db.tb_warehouse
                                  where w.warehouse_project_id == projectname.project_id
                                  select w
                                  ).FirstOrDefault();
                        WarehouseId = wh.warehouse_id;
                    }

                    if (string.IsNullOrEmpty(WarehouseId))
                        objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth).Select(x => new Models.InventoryViewModel()
                        {
                            inventory_id = x.inventory_id,
                            inventory_date = x.inventory_date,
                            inventory_status_id = x.inventory_status_id,
                            warehouse_id = x.warehouse_id,
                            product_id = x.product_id,
                            total_quantity = x.total_quantity,
                            in_quantity = x.in_quantity,
                            out_quantity = x.out_quantity,
                        });
                    else
                        objs = db.tb_inventory.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, WarehouseId) == 0 && x.inventory_date >= firstDayOfMonth && x.inventory_date <= lastDayOfMonth).Select(x => new Models.InventoryViewModel()
                        {
                            inventory_id = x.inventory_id,
                            inventory_date = x.inventory_date,
                            inventory_status_id = x.inventory_status_id,
                            warehouse_id = x.warehouse_id,
                            product_id = x.product_id,
                            total_quantity = x.total_quantity,
                            in_quantity = x.in_quantity,
                            out_quantity = x.out_quantity,
                        });

                    if (objs.Any())
                    {
                        double BigBalance = 0;
                        double EndingBalance = 0;
                        double ReceiveBalance = 0;
                        double IssueReturnBalance = 0;
                        double ReturnBalance = 0;
                        double TransferBalance = 0;
                        double DamangeBalance = 0;
                        double IssueBalance = 0;
                        double TransferIn = 0;
                        double IN = 0;
                        double UPrice = 0;

                        if (string.IsNullOrEmpty(WarehouseId))
                        {
                            var allwarehouses = db.tb_warehouse.Where(x => x.warehouse_status == true).Select(x => x.warehouse_id).ToList();
                            foreach (var warehouse in allwarehouses)
                            {
                                var inventoryItems = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.warehouse_id, warehouse) == 0).ToList();

                                var duplicateItems = inventoryItems.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                                if (duplicateItems.Any())
                                {
                                    foreach (var dupitem in duplicateItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        var items = inventoryItems.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                        BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                        EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);

                                        foreach (var item in items)
                                        {
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail
                                                                .Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved")
                                                                .Sum(x => x.quantity) ?? 0);

                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                                 PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);
                                        }

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = dupitem,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }

                                    foreach (var item in inventoryItems)
                                    {
                                        var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                        if (isDuplicate == null)
                                        {
                                            BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                            BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                            EndingBalance = Convert.ToDouble(item.total_quantity);
                                            //stock in 
                                            if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                            {
                                                ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                            {
                                                IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                            }
                                            //stock out
                                            else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                            {
                                                ReturnBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                            {
                                                TransferBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                            {
                                                DamangeBalance = Convert.ToDouble(item.out_quantity);
                                            }
                                            else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                            {
                                                IssueBalance = Convert.ToDouble(item.out_quantity);
                                            }

                                            TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                            IN = (double)((from PO in db.tb_purchase_order_detail
                                                           join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                           where PO.item_status == "approved" &&
                                                           PO.item_id == item.product_id &&
                                                                 (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                                 POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                           select PO.quantity).Sum() ?? 0
                                                          );
                                            UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                            originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                            {
                                                ItemId = item.product_id,
                                                BigBalance = BigBalance,
                                                EndingBalance = EndingBalance,
                                                ReceivedBalance = ReceiveBalance,
                                                IssueReturnBalance = IssueReturnBalance,
                                                ReturnBalance = ReturnBalance,
                                                TransferBalance = TransferBalance,
                                                DamangeBalance = DamangeBalance,
                                                IssueBalance = IssueBalance,
                                                TransferIn = TransferIn,
                                                IN = IN,
                                                UPrice = UPrice
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var item in inventoryItems)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            var duplicateItems = objs.GroupBy(x => x.product_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                            if (duplicateItems.Any())
                            {
                                foreach (var dupitem in duplicateItems)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    var items = objs.OrderBy(x => x.inventory_date).Where(x => string.Compare(x.product_id, dupitem) == 0).ToList();
                                    BigBalance = Convert.ToDouble(items[0].total_quantity + items[0].out_quantity - items[0].in_quantity);
                                    EndingBalance = Convert.ToDouble(items[items.Count - 1].total_quantity);
                                    foreach (var item in items)
                                    {
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = ReceiveBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = IssueReturnBalance + Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = ReturnBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = TransferBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = DamangeBalance + Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = IssueBalance + Convert.ToDouble(item.out_quantity);
                                        }


                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == WarehouseId
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    }
                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = dupitem,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice
                                    });
                                }

                                foreach (var item in objs)
                                {
                                    var isDuplicate = duplicateItems.Where(x => string.Compare(x, item.product_id) == 0).FirstOrDefault();
                                    if (isDuplicate == null)
                                    {
                                        BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                        BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                        EndingBalance = Convert.ToDouble(item.total_quantity);
                                        //stock in 
                                        if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                        {
                                            ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                        {
                                            IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                        }
                                        //stock out
                                        else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                        {
                                            ReturnBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                        {
                                            TransferBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                        {
                                            DamangeBalance = Convert.ToDouble(item.out_quantity);
                                        }
                                        else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                        {
                                            IssueBalance = Convert.ToDouble(item.out_quantity);
                                        }

                                        TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").Sum(x => x.quantity) ?? 0);
                                        IN = (double)((from PO in db.tb_purchase_order_detail
                                                       join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                       join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                       join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                       join S in db.tb_site on P.site_id equals S.site_id
                                                       join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                       where PO.item_status == "approved" &&
                                                       PO.item_id == item.product_id &&
                                                             (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                             POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                             W.warehouse_id == WarehouseId
                                                       select PO.quantity).Sum() ?? 0
                                                          );
                                        UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                        originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                        {
                                            ItemId = item.product_id,
                                            BigBalance = BigBalance,
                                            EndingBalance = EndingBalance,
                                            ReceivedBalance = ReceiveBalance,
                                            IssueReturnBalance = IssueReturnBalance,
                                            ReturnBalance = ReturnBalance,
                                            TransferBalance = TransferBalance,
                                            DamangeBalance = DamangeBalance,
                                            IssueBalance = IssueBalance,
                                            TransferIn = TransferIn,
                                            IN = IN,
                                            UPrice = UPrice
                                        });
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in objs)
                                {
                                    BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                    BigBalance = Convert.ToDouble(item.total_quantity + item.out_quantity - item.in_quantity);
                                    EndingBalance = Convert.ToDouble(item.total_quantity);
                                    //stock in 
                                    if (string.Compare(item.inventory_status_id, "7") == 0) //Receive
                                    {
                                        ReceiveBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "5") == 0) //Issue Return
                                    {
                                        IssueReturnBalance = Convert.ToDouble(item.in_quantity);
                                    }
                                    //stock out
                                    else if (string.Compare(item.inventory_status_id, "8") == 0) //Item Return
                                    {
                                        ReturnBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "6") == 0) //Transfer
                                    {
                                        TransferBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "3") == 0) //Damage
                                    {
                                        DamangeBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    else if (string.Compare(item.inventory_status_id, "2") == 0) //Issue
                                    {
                                        IssueBalance = Convert.ToDouble(item.out_quantity);
                                    }
                                    TransferIn = (double)(db.tb_stock_transfer_detail.Where(x => x.st_item_id == item.product_id &&
                                                                            x.invoice_date >= firstDayOfMonth &&
                                                                            x.invoice_date <= lastDayOfMonth &&
                                                                            x.st_warehouse_id == WarehouseId &&
                                                                            x.item_status == "approved").FirstOrDefault().quantity ?? 0);
                                    IN = (double)((from PO in db.tb_purchase_order_detail
                                                   join POD in db.tb_purchase_order on PO.purchase_order_id equals POD.purchase_order_id
                                                   join PR in db.tb_item_request on POD.purchase_oder_number equals PR.ir_no
                                                   join P in db.tb_project on PR.ir_project_id equals P.project_id
                                                   join S in db.tb_site on P.site_id equals S.site_id
                                                   join W in db.tb_warehouse on S.site_id equals W.warehouse_site_id
                                                   where PO.item_status == "approved" &&
                                                   PO.item_id == item.product_id &&
                                                         (POD.purchase_order_status == "Approved" || POD.purchase_order_status == "Completed") &&
                                                         POD.created_date >= firstDayOfMonth && POD.created_date <= lastDayOfMonth &&
                                                         W.warehouse_id == WarehouseId
                                                   select PO.quantity).FirstOrDefault() ?? 0
                                                          );
                                    UPrice = (double)(db.tb_product.Where(x => x.product_id == item.product_id).FirstOrDefault().unit_price ?? 0);

                                    originalItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.product_id,
                                        BigBalance = BigBalance,
                                        EndingBalance = EndingBalance,
                                        ReceivedBalance = ReceiveBalance,
                                        IssueReturnBalance = IssueReturnBalance,
                                        ReturnBalance = ReturnBalance,
                                        TransferBalance = TransferBalance,
                                        DamangeBalance = DamangeBalance,
                                        IssueBalance = IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice
                                    });
                                }
                            }
                        }


                        List<Models.StockBalanceMonthyViewModel> secondHandItems = new List<Models.StockBalanceMonthyViewModel>();
                        var duplicateItemInventory = originalItems.GroupBy(x => x.ItemId).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        if (duplicateItemInventory.Any())
                        {
                            foreach (var item in duplicateItemInventory)
                            {
                                BigBalance = 0; EndingBalance = 0; ReceiveBalance = 0; IssueReturnBalance = 0; ReturnBalance = 0; TransferBalance = 0; DamangeBalance = 0; IssueBalance = 0;
                                var dupItems = originalItems.Where(x => string.Compare(x.ItemId, item) == 0).ToList();
                                foreach (var ii in dupItems)
                                {
                                    BigBalance = BigBalance + ii.BigBalance;
                                    EndingBalance = EndingBalance + ii.EndingBalance;
                                    ReceiveBalance = ReceiveBalance + ii.ReceivedBalance;
                                    IssueReturnBalance = IssueReturnBalance + ii.IssueReturnBalance;
                                    ReturnBalance = ReturnBalance + ii.ReturnBalance;
                                    TransferBalance = TransferBalance + ii.TransferBalance;
                                    DamangeBalance = DamangeBalance + ii.DamangeBalance;
                                    IssueBalance = IssueBalance + ii.IssueBalance;
                                    TransferIn = TransferIn + ii.TransferIn;
                                    IN = IN + ii.IN;
                                }
                                secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                {
                                    ItemId = item,
                                    BigBalance = BigBalance,
                                    EndingBalance = EndingBalance,
                                    ReceivedBalance = ReceiveBalance,
                                    IssueReturnBalance = IssueReturnBalance,
                                    ReturnBalance = ReturnBalance,
                                    TransferBalance = TransferBalance,
                                    DamangeBalance = DamangeBalance,
                                    IssueBalance = IssueBalance,
                                    TransferIn = TransferIn,
                                    IN = IN,
                                    UPrice = UPrice
                                });
                            }

                            foreach (var item in originalItems)
                            {
                                var isDuplicate = duplicateItemInventory.Where(x => string.Compare(x, item.ItemId) == 0).FirstOrDefault();
                                if (isDuplicate == null)
                                {
                                    secondHandItems.Add(new Models.StockBalanceMonthyViewModel()
                                    {
                                        ItemId = item.ItemId,
                                        BigBalance = item.BigBalance,
                                        EndingBalance = item.EndingBalance,
                                        ReceivedBalance = item.ReceivedBalance,
                                        IssueReturnBalance = item.IssueReturnBalance,
                                        ReturnBalance = item.ReturnBalance,
                                        TransferBalance = item.TransferBalance,
                                        DamangeBalance = item.DamangeBalance,
                                        IssueBalance = item.IssueBalance,
                                        TransferIn = TransferIn,
                                        IN = IN,
                                        UPrice = UPrice
                                    });
                                }
                            }
                        }
                        else
                            secondHandItems = originalItems;
                        foreach (var item in secondHandItems)
                        {
                            var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                            stockBalances.Add(new Models.StockBalanceMonthyViewModel()
                            {
                                ItemId = item.ItemId,
                                ItemCode = product.product_code,
                                ItemName = product.product_name,
                                ItemUnit = product.product_unit,
                                BigBalance = item.BigBalance,
                                EndingBalance = item.EndingBalance,
                                ReceivedBalance = item.ReceivedBalance,
                                IssueReturnBalance = item.IssueReturnBalance,
                                ReturnBalance = item.ReturnBalance,
                                TransferBalance = item.TransferBalance,
                                DamangeBalance = item.DamangeBalance,
                                IssueBalance = item.IssueBalance,
                                TransferIn = TransferIn,
                                IN = IN,
                                UPrice = UPrice
                            });
                        }
                        stockBalances = stockBalances.OrderBy(x => x.ItemCode).ToList();

                        foreach (var item in stockBalances)
                        {
                            var product = db.tb_product.Where(x => string.Compare(x.product_id, item.ItemId) == 0).FirstOrDefault();
                            dtr = tb.NewRow();
                            dtr["date"] = Convert.ToDateTime(date).ToString("MMM-yyyy");
                            dtr["warehouseName"] = string.IsNullOrEmpty(WarehouseId) ? "All" : db.tb_warehouse.Where(x => x.warehouse_id == WarehouseId).Select(x => x.warehouse_name).FirstOrDefault();
                            dtr["itemCode"] = product.product_code;
                            dtr["itemName"] = product.product_name;
                            dtr["itemUnit"] = db.tb_unit.Find(product.product_unit).Name;
                            dtr["bigbalance"] = item.BigBalance;
                            dtr["ending_balance"] = item.EndingBalance;
                            dtr["in_receive"] = item.ReceivedBalance;
                            dtr["in_issue_return"] = item.IssueReturnBalance;
                            dtr["out_return"] = item.ReturnBalance;
                            dtr["out_transfer"] = item.TransferBalance;
                            dtr["out_damage"] = item.DamangeBalance;
                            dtr["out_issue"] = item.IssueBalance;
                            dtr["total_in"] = item.ReceivedBalance + item.IssueReturnBalance;
                            dtr["total_out"] = item.ReturnBalance + item.TransferBalance + item.DamangeBalance + item.IssueBalance;

                            //Rathana Add 24.04.2019
                            double adjustentQuantity = item.EndingBalance - item.ReceivedBalance;
                            if (adjustentQuantity > 0)
                            {
                                dtr["adjust_increase"] = adjustentQuantity;
                                dtr["adjust_decrease"] = 0;
                            }
                            else
                            {
                                dtr["adjust_decrease"] = -(adjustentQuantity);
                                dtr["adjust_increase"] = 0;
                            }
                            dtr["transfer_in"] = item.TransferIn;
                            dtr["IN"] = item.IN;
                            dtr["unit_price"] = item.UPrice;
                            dtr["amount"] = item.EndingBalance * item.UPrice;

                            if (project == "All")
                            {
                                dtr["projectName"] = "All";
                            }
                            else
                            {

                                dtr["projectName"] = projectname.project_full_name;
                            }
                            //End Rathana Add
                            tb.Rows.Add(dtr);
                        }
                    }

                    rv.LocalReport.ReportPath = reportPath;
                    rv.LocalReport.DataSources.Clear();
                    ReportDataSource rdc = new ReportDataSource("StockBalanceByDateandWarehouse", tb);
                    rv.LocalReport.DataSources.Add(rdc);
                    rv.LocalReport.Refresh();
                }
            }
            catch (Exception e) { }
        }

    }
}


