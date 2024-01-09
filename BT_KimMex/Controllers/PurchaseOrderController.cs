using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using System.Web.Security;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using System.Text;
using MoreLinq;
using System.Data.Entity.Validation;
using SelectPdf;
using GroupDocs.Watermark.Options.Pdf;
using GroupDocs.Watermark;
using GroupDocs.Watermark.Watermarks;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class PurchaseOrderController : Controller
    {
        // GET: PurchaseOrder
        public ActionResult Index()
        {
            return View();
        }
        //[Authorize(Roles = "Admin,Purchaser,Main Stock Controller")]
        public ActionResult Create()
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            //itemRequestss = CommonClass.GetAvailableItemRequestList();
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList();
            DateTime now = Class.CommonClass.ToLocalTime(DateTime.Now);
            string sNow = Class.CommonClass.ToLocalTime(DateTime.Now).ToString("dd/MM/yyyy");
            itemRequests = this.GetIRDropdownList();
            ViewBag.Date = sNow;
            //ViewBag.PONumber = this.GetPOId();
            ViewBag.PONumber = Class.CommonClass.GenerateProcessNumber("QN");
            ViewBag.IRID = new SelectList(itemRequestss, "ir_id", "ir_no");
            ViewBag.Projects = new SelectList(CommonClass.GetAllProject(), "project_id", "project_full_name");
            return View();
        }
        public ActionResult CreatePO(PurchaseOrderViewModel model, List<PurchaseOrderDetailViewModel> pDetails, List<PurchaseOrderItemSupplier> supplier1, List<PurchaseOrderItemSupplier> supplier2, List<PurchaseOrderItemSupplier> supplier3, List<String> Attachment)
        {
            try
            {
                
                int countInvalid = 0;
                for (int i = 0; i < pDetails.Count(); i++)
                {
                    int countSelectedSupplier = 0;
                    var poSupplier = pDetails[i].poSuppliers;
                    //added oct 02 2018
                    for(int j = 0; j < poSupplier.Count(); j++)
                    {
                        if (Convert.ToBoolean(poSupplier[j].is_check))
                            countSelectedSupplier++;
                    }
                    if (countSelectedSupplier == 0 || countSelectedSupplier > 3)
                    {
                        countInvalid++;
                    }
                    /*
                    if (poSupplier.Count() == 0 || poSupplier.Count() > 3)
                    {
                        countInvalid++;
                    }
                    */
                }
                if (countInvalid > 0)
                {
                    return Json(new { result = "error", message = "Selected Supplier for each Item is minimum 1 and maximum 3" }, JsonRequestBehavior.AllowGet);
                }
                kim_mexEntities db = new kim_mexEntities();
                tb_purchase_order po = new tb_purchase_order();
                po.purchase_order_id = Guid.NewGuid().ToString();
                po.purchase_oder_number =Class.CommonClass.GenerateProcessNumber("QN");
                po.item_request_id = model.item_request_id;
                po.purchase_order_status = "Pending";
                po.status = true;
                po.created_by = User.Identity.GetUserId();
                po.is_po_checked = false;
                po.created_date =Class.CommonClass.ToLocalTime(DateTime.Now);
                //Rathana Add 10.04.2019
                po.pol_project_short_name_number = Class.CommonClass.GeneratePOLNumberWithProjectShortNameByID(model.ir_project_id);
                po.shipping_to = model.ShippingTo;
                //End Rathana Add
                db.tb_purchase_order.Add(po);
                db.SaveChanges();

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.QuoteCreated,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.QuoteCreated);

                for (int i = 0; i < pDetails.Count(); i++)
                {
                    tb_purchase_order_detail dPO = new tb_purchase_order_detail();
                    dPO.po_detail_id = Guid.NewGuid().ToString();
                    dPO.purchase_order_id = po.purchase_order_id;
                    dPO.item_id = pDetails[i].item_id;
                    dPO.quantity = pDetails[i].quantity;
                    dPO.item_unit = pDetails[i].item_unit;
                    dPO.unit_price = 0;
                    dPO.item_status = "Pending";
                    dPO.item_vat = pDetails[i].item_vat;
                    dPO.status = true;
                    db.tb_purchase_order_detail.Add(dPO);
                    db.SaveChanges();
                    Class.ItemRequest.UpdateItemRemainQuantity(pDetails[i].ir_detail2_id, Convert.ToDecimal(dPO.quantity));
                    var poSupplier = pDetails[i].poSuppliers;
                    //if (poSupplier.Count() > 0 && poSupplier.Count() <= 3){
                        for (int j = 0; j < poSupplier.Count(); j++)
                        {
                            if (pDetails[i].po_detail_id == poSupplier[j].po_detail_id && !string.IsNullOrEmpty(poSupplier[j].supplier_id))
                            {
                                tb_po_supplier sPo = new tb_po_supplier();
                                sPo.po_supplier_id = Guid.NewGuid().ToString();
                                sPo.po_detail_id = dPO.po_detail_id;
                                sPo.supplier_id = poSupplier[j].supplier_id;
                                sPo.unit_price = poSupplier[j].unit_price;
                                sPo.vat = poSupplier[j].vat;
                                sPo.sup_number = poSupplier[j].sup_number;
                                sPo.is_check = poSupplier[j].is_check;
                                sPo.original_price = poSupplier[j].original_price;
                                sPo.discount_percentage = poSupplier[j].discount_percentage;
                            sPo.discount_amount = poSupplier[j].discount_amount;
                            sPo.lumpsum_discount_amount = poSupplier[j].lump_sum_discount_amount;
                                db.tb_po_supplier.Add(sPo);
                                db.SaveChanges();
                            }
                        }
                    //}
                }
                //block old process by tterd apr 03 2020
                //Class.ItemRequest.UpdateCompletedItemRequest(po.item_request_id);
                Class.ItemRequest.UpdateCompletedItemRequest(po.item_request_id,User.Identity.GetUserId());

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.purchase_order_id, po.purchase_order_status, po.created_by, po.created_date);

                if (Attachment != null)
                {
                    for (int i = 0; i < Attachment.Count(); i++)
                    {
                        string attID = Attachment[i];
                        tb_po_attachment att = db.tb_po_attachment.Where(m => m.po_attachment_id == attID).FirstOrDefault();
                        att.po_id = po.purchase_order_id;
                        db.SaveChanges();
                    }
                }

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                string message = string.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {
                    message =message+"\n"+string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message = message+string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return Json(new { result = "fail", message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public JsonResult UploadAttachment()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_po_attachment po_att = new tb_po_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/PO Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);

                    po_att.po_attachment_id = file_id;
                    po_att.po_attachment_name = file_name;
                    po_att.po_attachment_extension = file_extension;
                    po_att.file_path = file_path;
                    db.tb_po_attachment.Add(po_att);
                    db.SaveChanges();

                    //AddWaterWaterToAttachment(file_path,file_id,file_extension);
                }
                return Json(new { result = "success", attachment_id = po_att.po_attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/PO Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        public FileResult DownloadSupplierQuote(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        public void AddWaterWaterToAttachment(string filePath,string file_id,string file_extenstion)
        {
            try
            {
                
                string imgFile = Server.MapPath("~/Assets/images/lotusgreeenteam.png");
                PdfLoadOptions loadOptions = new PdfLoadOptions();
                using(Watermarker watermarker =new Watermarker(filePath, loadOptions))
                {
                    ImageWatermark imageWatermark = new ImageWatermark(imgFile)
                    {
                        Opacity = 0.7,
                        X = 70,
                        Y = 350
                    };
                    // Adding image watermark to the second page  
                    PdfArtifactWatermarkOptions imageWatermarkOptions = new PdfArtifactWatermarkOptions();
                    imageWatermarkOptions.PageIndex = 1;
                    watermarker.Add(imageWatermark, imageWatermarkOptions);
                    var new_file_path = Path.Combine(Server.MapPath("~/Documents/PO Attachment/"), file_id+"_watermark"+file_extenstion);
                    watermarker.Save(new_file_path);
                }



            }
            catch(Exception ex)
            {

            }
        }
        [HttpPost]
        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_po_attachment att = db.tb_po_attachment.Find(id);
                if (att == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_po_attachment.Remove(att);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/PO Attachment/"), att.po_attachment_id + att.po_attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public ActionResult Detail(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            return View(model);
        }
        //[Authorize(Roles = "Admin,Purchaser,Main Stock Controller")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            List<tb_supplier> suppliers = new List<tb_supplier>();
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            List<ItemRequestViewModel> itemRequestss = new List<ItemRequestViewModel>();
            //itemRequestss = CommonClass.GetAvailableItemRequestList();
            
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            itemRequests = this.GetIRDropdownList();
            suppliers = this.GetSuppliersDropdown();
            itemRequestss = Class.ItemRequest.GetAllItemRequestDropdownList(model.item_request_id).Where(x => string.Compare(x.ir_project_id, model.ir_project_id) == 0).ToList();
            //ViewBag.IRID = new SelectList(itemRequestss, "ir_id", "ir_no");
            ViewBag.IRID = itemRequestss;
            ViewBag.SupplierID = suppliers;
            ViewBag.Projects = CommonClass.GetAllProject();
            return View(model);
        }
        //[Authorize(Roles = "Admin,Purchaser,Main Stock Controller")]
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_order po = db.tb_purchase_order.Find(id);
                po.status = false;
                po.updated_by = User.Identity.GetUserId();
                po.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Approve(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            List<tb_supplier> suppliers = new List<tb_supplier>();
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            itemRequests = this.GetIRDropdownList();
            suppliers = this.GetSuppliersDropdown();
            ViewBag.IRID = new SelectList(itemRequests, "ir_id", "ir_no");
            ViewBag.SupplierID = suppliers;
            return View(model);
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer")]
        public ActionResult Approved(string id, List<PurchaseOrderItemSupplier> poSuppliers, bool isRequestTD,decimal totalAmountPerRequest=0)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_order po = db.tb_purchase_order.Find(id);
                tb_purchase_order_detail poD = db.tb_purchase_order_detail.Find(id);
                if (isRequestTD)
                //if(totalAmountPerRequest>3000)
                    po.purchase_order_status = "Approved";
                else
                {
                    if (totalAmountPerRequest > 3000)
                        po.purchase_order_status = Status.Checked;
                    else 
                        po.purchase_order_status = "Completed";
                }    
                    
                po.checked_by = User.Identity.GetUserId();
                po.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.purchase_order_id, po.purchase_order_status, User.Identity.GetUserId(), CommonClass.ToLocalTime(DateTime.Now));

                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.item_request_id, id, true,false);
                
                for (int i = 0; i < poSuppliers.Count(); i++)
                {
                    string posId = poSuppliers[i].po_supplier_id;
                    tb_po_supplier pos = db.tb_po_supplier.Where(m => m.po_supplier_id == posId).FirstOrDefault();
                    pos.is_selected = true;
                    pos.Reason = poSuppliers[i].Reason;
                    db.SaveChanges();

                    string poDetailId = pos.po_detail_id;
                    tb_purchase_order_detail poDetail = db.tb_purchase_order_detail.Find(poDetailId);
                    poDetail.po_quantity = poSuppliers[i].po_quantity;
                    poDetail.po_unit = poSuppliers[i].po_unit;
                    poDetail.remain_quantity= poSuppliers[i].po_quantity;
                    if (pos.vat == true)
                    {
                        poDetail.item_vat = true;
                    }
                    else
                    {
                        poDetail.item_vat = false;
                    }
                    poDetail.lump_sum_discount_amount = pos.lumpsum_discount_amount;

                    if (!isRequestTD)
                    {
                        poDetail.item_status = "approved";
                        //poDetail.unit_price = poSuppliers[i].unit_price;
                        poDetail.unit_price = pos.unit_price;
                        poDetail.original_price = pos.original_price;
                        poDetail.discount_percentage = pos.discount_percentage;
                        poDetail.discount_amount = pos.discount_amount;
                        

                        //update product price
                        var products = db.tb_purchase_order_detail.Where(x => x.item_id == poDetail.item_id && poDetail.item_status == "approved").ToList();
                        if (products.Any())
                        {
                            decimal totalUnitPrice = 0, averagePrice = 0;
                            int totalProduct = products.Count();
                            foreach (var p in products)
                            {
                                totalUnitPrice += Convert.ToDecimal(p.unit_price);
                            }
                            averagePrice = totalUnitPrice / totalProduct;
                            tb_product pro = db.tb_product.Where(x => x.product_id == poDetail.item_id).FirstOrDefault();
                            pro.unit_price = averagePrice;
                            // pro.unit_price = detail.unit_price;

                            db.SaveChanges();
                        }

                        //end update product

                    }

                    db.SaveChanges();

                }
                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.item_request_id, id, false, true);

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.QuoteApproved,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.QuoteApproved);

                /* blocked by rith 
                //insert to po number
                //PurchaseOrderDetailViewModel podd = new PurchaseOrderDetailViewModel();
                PurchaseOrderViewModel purchaseOrder = new PurchaseOrderViewModel();
                purchaseOrder = GetPOSupplierItem(po.purchase_order_id);
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();

                string poCompare = "PO " + yy + "-" + mm + "-";
                string poLastNumber = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare) select tbl.po_report_number).FirstOrDefault();
                int count = 0;
                if (poLastNumber == null)
                    count = 1;
                else
                {
                    count = Convert.ToInt32(poLastNumber.Substring(poLastNumber.Count() - 3, 3)) + 1;
                }
                int countSupplier = purchaseOrder.poSuppliers.Count();
                if (countSupplier > 0)
                {
                    
                    foreach (var sup in purchaseOrder.poSuppliers)
                    {
                        string poReportNumber = string.Empty;
                        string lastSplit = count.ToString().Length == 1 ? "00" + count : count.ToString().Length == 2 ? "0" + count : count.ToString();
                        poReportNumber = "PO " + yy + "-" + mm + "-" + lastSplit;
                        tb_po_report poReport = new tb_po_report();
                        poReport.po_report_id = Guid.NewGuid().ToString();
                        poReport.po_report_number = poReportNumber;
                        poReport.po_ref_id = po.purchase_order_id;
                        poReport.po_supplier_id = sup.supplier_id;
                        poReport.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        poReport.vat_status = true;
                        db.tb_po_report.Add(poReport);
                        db.SaveChanges();
                        count++;
                    }
                }
                PurchaseOrderViewModel purchaseOrder2 = new PurchaseOrderViewModel();
                purchaseOrder2 = GetPOSupplierItem(po.purchase_order_id);
                string yy2 = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);

                string mm2 = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();

                string poCompare2 = "PO / NT " + yy2 + "-" + mm2 + "-";
                string poLastNumber2 = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare2) select tbl.po_report_number).FirstOrDefault();
                int count2 = 0;
                if (poLastNumber2 == null)
                    count2 = 1;
                else
                {
                    count2 = Convert.ToInt32(poLastNumber2.Substring(poLastNumber2.Count() - 3, 3)) + 1;
                }
                int countSupplier2 = purchaseOrder2.poSuppliers.Count();
                if (countSupplier2 > 0)
                {

                    foreach (var sup in purchaseOrder2.poSuppliers)
                    {
                        string poReportNumber = string.Empty;
                        string lastSplit = count2.ToString().Length == 1 ? "00" + count2 : count2.ToString().Length == 2 ? "0" + count2 : count2.ToString();
                        poReportNumber = "PO / NT " + yy2 + "-" + mm2 + "-" + lastSplit;
                        tb_po_report poReport = new tb_po_report();
                        poReport.po_report_id = Guid.NewGuid().ToString();
                        poReport.po_report_number = poReportNumber;
                        poReport.po_ref_id = po.purchase_order_id;
                        poReport.po_supplier_id = sup.supplier_id;
                        poReport.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        poReport.vat_status = false;
                        db.tb_po_report.Add(poReport);
                        db.SaveChanges();
                        count2++;
                    }
                }
                */
                //insert to tb_po_report
                //PurchaseOrderViewModel purchaseOrder = new PurchaseOrderViewModel();
                //purchaseOrder = GetPOSupplierItem(po.purchase_order_id);
                //string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                //string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                //string poCompare = "PO " + yy + "-" + mm + "-";
                //string poLastNumber = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare) select tbl.po_report_number).FirstOrDefault();
                //int count = 0;
                //if (poLastNumber == null)
                //    count = 1;
                //else
                //{
                //    count = Convert.ToInt32(poLastNumber.Substring(poLastNumber.Count() - 3, 3)) + 1;
                //}
                //int countSupplier = purchaseOrder.poSuppliers.Count();
                //if (countSupplier > 0)
                //{
                //    foreach (var sup in purchaseOrder.poSuppliers)
                //    {
                //        string poReportNumber = string.Empty;
                //        string lastSplit = count.ToString().Length == 1 ? "00" + count : count.ToString().Length == 2 ? "0" + count : count.ToString();
                //        poReportNumber = "PO " + yy + "-" + mm + "-" + lastSplit;
                //        tb_po_report poReport = new tb_po_report();
                //        poReport.po_report_id = Guid.NewGuid().ToString();
                //        poReport.po_report_number = poReportNumber;
                //        poReport.po_ref_id = po.purchase_order_id;
                //        poReport.po_supplier_id = sup.supplier_id;
                //        poReport.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //        db.tb_po_report.Add(poReport);
                //        db.SaveChanges();
                //        count++;
                //    }
                //}
                ////insert to po number
                //PurchaseOrderViewModel purchaseOrder2 = new PurchaseOrderViewModel();
                //purchaseOrder2 = GetPOSupplierItem(po.purchase_order_id);
                //string yy2 = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                //string mm2 = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();

                //string poCompare2 = "PO/NT " + yy2 + "-" + mm2 + "-";
                //string poLastNumber2 = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare2) select tbl.po_report_number).FirstOrDefault();
                //int count2 = 0;
                //if (poLastNumber2 == null)
                //    count2 = 1;
                //else
                //{
                //    count2 = Convert.ToInt32(poLastNumber2.Substring(poLastNumber2.Count() - 3, 3)) + 1;
                //}
                //int countSupplier2 = purchaseOrder2.poSuppliers.Count();
                //if (countSupplier2 > 0)
                //{
                //    foreach (var sup in purchaseOrder2.poSuppliers)
                //    {
                //        string poReportNumber = string.Empty;
                //        string lastSplit = count2.ToString().Length == 1 ? "00" + count2 : count2.ToString().Length == 2 ? "0" + count2 : count.ToString();
                //        poReportNumber = "PO/NT " + yy2 + "-" + mm2 + "-" + lastSplit;
                //        tb_po_report poReport = new tb_po_report();
                //        poReport.po_report_id = Guid.NewGuid().ToString();
                //        poReport.po_report_number = poReportNumber;
                //        poReport.po_ref_id = po.purchase_order_id;
                //        poReport.po_supplier_id = sup.supplier_id;
                //        poReport.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //        db.tb_po_report.Add(poReport);
                //        db.SaveChanges();
                //        count2++;
                //    }
                //}
                PurchaseOrder obj = new PurchaseOrder();
                obj.InitialPurchaseOrderPONumber(id);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[Authorize(Roles = "Admin,Director")]
        public ActionResult ApprovedByCFO(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_order po = db.tb_purchase_order.Find(id);
                po.purchase_order_status = "Completed";
                po.approved_by = User.Identity.GetUserId();
                po.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        //[Authorize(Roles = "Admin,Chief of Finance Officer,Director")]
        public ActionResult RejectedByCFO(string id, string role, string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_order po = db.tb_purchase_order.Find(id);
                //po.purchase_order_status = "Rejected";

                //if (string.Compare(role, "Chief of Finance Officer") == 0)
                //{
                //    po.checked_by = User.Identity.GetUserId();
                //    po.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //}
                //else if (string.Compare(role, "Director") == 0)
                //{
                //    po.approved_by = User.Identity.GetUserId();
                //    po.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //}

                if (string.Compare(po.purchase_order_status, "Pending") == 0)
                {
                    po.checked_by = User.Identity.GetUserId();
                    po.checked_date = CommonClass.ToLocalTime(DateTime.Now);
                }
                else if (string.Compare(po.purchase_order_status, "Approved") == 0)
                {
                    po.approved_by = User.Identity.GetUserId();
                    po.approved_date = CommonClass.ToLocalTime(DateTime.Now);
                }


                po.purchase_order_status = Status.cancelled;
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.purchase_order_id, po.purchase_order_status, User.Identity.GetUserId(), Class.CommonClass.ToLocalTime(DateTime.Now), comment);
                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.MRCancelled,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.MRCancelled);
                //if (string.Compare(role, "Director") == 0)
                //{
                var dPos = db.tb_purchase_order_detail.Where(x => x.purchase_order_id == id).ToList();
                    if (dPos.Any())
                    {
                        foreach (var dPo in dPos)
                        {
                            string dId = dPo.po_detail_id;
                            tb_purchase_order_detail poDetail = db.tb_purchase_order_detail.Find(dId);
                            poDetail.item_status = Status.cancelled;
                            db.SaveChanges();
                        }
                    }
                //}
                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = po.purchase_order_id;
                reject.ref_type = "Purchase Order";
                reject.comment = comment;
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                reject.rejected_by = User.Identity.GetUserId();
                db.tb_reject.Add(reject);
                db.SaveChanges();

                #region Update MR
                var prr = (from quote in db.tb_purchase_order
                          join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                          where string.Compare(quote.purchase_order_id,po.purchase_order_id)==0
                          select pr).FirstOrDefault();
                if (prr != null)
                {
                    tb_item_request mr = db.tb_item_request.Find(prr.material_request_id);
                    if (mr != null)
                    {
                        reject = new tb_reject();
                        reject.reject_id = Guid.NewGuid().ToString();
                        reject.ref_id = mr.ir_id;
                        reject.ref_type = "Item Request";
                        reject.comment = comment;
                        reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        reject.rejected_by = User.Identity.GetUserId();
                        db.tb_reject.Add(reject);
                        db.SaveChanges();
                        CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), mr.ir_id,Status.CancelledMR,User.Identity.GetUserId(),CommonClass.ToLocalTime(DateTime.Now), comment);
                    }
                }
                #endregion



                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditPurchaseOrder(PurchaseOrderViewModel model, List<PurchaseOrderDetailViewModel> pDetails, List<PurchaseOrderItemSupplier> supplier1, List<PurchaseOrderItemSupplier> supplier2, List<PurchaseOrderItemSupplier> supplier3, List<String> Attachment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                int countInvalid = 0;
                for (int i = 0; i < pDetails.Count(); i++)
                {
                    int countSelectedSupplier = 0;
                    var poSupplier = pDetails[i].poSuppliers;
                    //added oct 02 2018
                    for (int j = 0; j < poSupplier.Count(); j++)
                    {
                        if (Convert.ToBoolean(poSupplier[j].is_check))
                            countSelectedSupplier++;
                    }
                    if (countSelectedSupplier == 0 || countSelectedSupplier > 3)
                    {
                        countInvalid++;
                    }
                    /*
                    if (poSupplier.Count() == 0 || poSupplier.Count() > 3)
                    {
                        countInvalid++;
                    }
                    */
                }
                if (countInvalid > 0)
                {
                    return Json(new { result = "error", message = "Selected Supplier for each Item is minimum 1 and maximum 3" }, JsonRequestBehavior.AllowGet);
                }

                tb_purchase_order po = db.tb_purchase_order.Where(m => m.purchase_order_id == model.purchase_order_id).FirstOrDefault();
                po.purchase_oder_number = model.purchase_oder_number;
                po.item_request_id = model.item_request_id;
                po.purchase_order_status = "Pending";
                po.status = true;
                po.updated_by = User.Identity.GetUserId();
                po.pol_project_short_name_number = model.POLNumber;
                po.shipping_to = model.ShippingTo;
                po.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                bool isPurchaser;
                if (supplier1 == null && supplier2 == null && supplier3 == null)
                {
                    isPurchaser = true;
                } else
                {
                    isPurchaser = false;
                }
                Class.ItemRequest.RollbackItemRequestRemainQuantity(model.item_request_id, model.purchase_order_id, true,false);
                this.DeletePurchaseOrderDetail(model.purchase_order_id, isPurchaser);
                
                for (int i = 0; i < pDetails.Count(); i++)
                {
                    tb_purchase_order_detail dPO = new tb_purchase_order_detail();
                    dPO.po_detail_id = Guid.NewGuid().ToString();
                    dPO.purchase_order_id = po.purchase_order_id;
                    dPO.item_id = pDetails[i].item_id;
                    dPO.quantity = pDetails[i].quantity;
                    dPO.item_unit = pDetails[i].item_unit;
                    dPO.item_vat = pDetails[i].item_vat;
                    dPO.unit_price = 0;
                    dPO.item_status = "Pending";
                    dPO.status = true;
                    db.tb_purchase_order_detail.Add(dPO);
                    db.SaveChanges();

                    var poSupplier = pDetails[i].poSuppliers;
                    //if (poSupplier.Count() > 0 && poSupplier.Count() <= 3){
                        for (int j = 0; j < poSupplier.Count(); j++)
                        {
                            if (pDetails[i].po_detail_id == poSupplier[j].po_detail_id && !string.IsNullOrEmpty(poSupplier[j].supplier_id))
                            {
                                tb_po_supplier sPo = new tb_po_supplier();
                                sPo.po_supplier_id = Guid.NewGuid().ToString();
                                sPo.po_detail_id = dPO.po_detail_id;
                                sPo.supplier_id = poSupplier[j].supplier_id;
                                sPo.unit_price = poSupplier[j].unit_price;
                                sPo.vat = poSupplier[j].vat;
                                sPo.sup_number = poSupplier[j].sup_number;
                                //sPo.vat = poSupplier[j].vat;
                                sPo.is_check = poSupplier[j].is_check;
                                db.tb_po_supplier.Add(sPo);
                                db.SaveChanges();
                            }
                        }
                    //}
                }
                Class.ItemRequest.RollbackItemRequestRemainQuantity(model.item_request_id, model.purchase_order_id, false,false);
                if (Attachment != null)
                {
                    for (int i = 0; i < Attachment.Count(); i++)
                    {
                        string attID = Attachment[i];
                        tb_po_attachment att = db.tb_po_attachment.Where(m => m.po_attachment_id == attID).FirstOrDefault();
                        att.po_id = po.purchase_order_id;
                        db.SaveChanges();
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DirectorApprove(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult DirectorApprove(string id, List<PurchaseOrderDetailViewModel> poDetails)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                decimal totalAmountPerRequest = 0;

                tb_purchase_order po = db.tb_purchase_order.Find(id);

                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.item_request_id, po.purchase_order_id, true, true);
                foreach (PurchaseOrderDetailViewModel detail in poDetails)
                {
                    var pId = detail.po_detail_id;
                    tb_purchase_order_detail poDetail = db.tb_purchase_order_detail.Find(pId);
                    poDetail.unit_price = detail.unit_price;
                    poDetail.item_status = detail.item_status;
                    poDetail.original_price = detail.original_price;
                    poDetail.discount_percentage = detail.discount_percentage;
                    poDetail.discount_amount = detail.discount_amount;
                    db.SaveChanges();

                    if(string.Compare(poDetail.item_status,Status.Approved)==0)
                    {
                        totalAmountPerRequest = totalAmountPerRequest +Convert.ToDecimal((poDetail.original_price - poDetail.discount_amount) * poDetail.po_quantity);
                    }

                    //update product price
                    var products = db.tb_purchase_order_detail.Where(x => x.item_id == poDetail.item_id && poDetail.item_status == "approved").ToList();
                    if (products.Any())
                    {
                        decimal totalUnitPrice = 0, averagePrice = 0;
                        int totalProduct = products.Count();
                        foreach (var p in products)
                        {
                            totalUnitPrice += Convert.ToDecimal(p.unit_price);
                        }
                        averagePrice = totalUnitPrice / totalProduct;
                        tb_product pro = db.tb_product.Where(x => x.product_id == poDetail.item_id).FirstOrDefault();
                        pro.unit_price = averagePrice;
                        // pro.unit_price = detail.unit_price;

                        db.SaveChanges();
                    }

                    //end update product
                }

                
                //New enhancement 
                if (totalAmountPerRequest > 3000)
                {
                    po.purchase_order_status = Status.Checked;
                }
                else
                {
                    po.purchase_order_status = "Completed";
                }

                
                po.approved_by = User.Identity.GetUserId();
                po.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.purchase_order_id, po.purchase_order_status, User.Identity.GetUserId(), Class.CommonClass.ToLocalTime(DateTime.Now));

                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.item_request_id, po.purchase_order_id, false, true, "approved");
                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.POPending,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(po.item_request_id), ShowStatus.POPending);
                ////insert to po number
                //PurchaseOrderViewModel purchaseOrder = new PurchaseOrderViewModel();
                //purchaseOrder = GetPOSupplierItem(po.purchase_order_id);
                //string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                //string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();

                //string poCompare = "PO " + yy + "-" + mm + "-";
                //string poLastNumber = (from tbl in db.tb_po_report orderby tbl.created_date descending where tbl.po_report_number.Contains(poCompare) select tbl.po_report_number).FirstOrDefault();
                //int count = 0;
                //if (poLastNumber == null)
                //    count = 1;
                //else
                //{
                //    count = Convert.ToInt32(poLastNumber.Substring(poLastNumber.Count() - 3, 3)) + 1;
                //}
                //int countSupplier = purchaseOrder.poSuppliers.Count();
                //if (countSupplier > 0)
                //{
                //    foreach (var sup in purchaseOrder.poSuppliers)
                //    {
                //        string poReportNumber = string.Empty;
                //        string lastSplit = count.ToString().Length == 1 ? "00" + count : count.ToString().Length == 2 ? "0" + count : count.ToString();
                //        poReportNumber = "PO " + yy + "-" + mm + "-" + lastSplit;
                //        tb_po_report poReport = new tb_po_report();
                //        poReport.po_report_id = Guid.NewGuid().ToString();
                //        poReport.po_report_number = poReportNumber;
                //        poReport.po_ref_id = po.purchase_order_id;
                //        poReport.po_supplier_id = sup.supplier_id;
                //        poReport.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                //        db.tb_po_report.Add(poReport);
                //        db.SaveChanges();
                //        count++;
                //    }
                //}

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }

        }
        //New Enhancement
        public ActionResult ODApprovedAJAX(string quoteId)
        {
            AJAXResultModel result = new AJAXResultModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_purchase_order quote = db.tb_purchase_order.Find(quoteId);
                if (quote != null)
                {
                    quote.purchase_order_status = "Completed";
                    quote.od_checked_by = User.Identity.GetUserId();
                    quote.od_checked_date = CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), quote.purchase_order_id, quote.purchase_order_status, User.Identity.GetUserId(), Class.CommonClass.ToLocalTime(DateTime.Now));

                    //Class.ItemRequest.RollbackItemRequestRemainQuantity(quote.item_request_id, quote.purchase_order_id, false, true, "approved");
                    CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.POPending, User.Identity.GetUserId());
                    StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.POPending);

                }

            }catch(Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.InnerException.Message;
            }
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cancel(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = this.GetPOSupplierItem(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Cancel(string id, string[] suppliers)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (suppliers == null || suppliers.Count() == 0)
                {
                    return Json(new { result = "error", message = "Please select at least one supplier to cancel." }, JsonRequestBehavior.AllowGet);
                }
                for (int i = 0; i < suppliers.Count(); i++)
                {
                    var poDetails = db.tb_purchase_order_detail.Where(x => x.purchase_order_id == id && string.Compare(x.item_status, "approved") == 0).ToList();
                    foreach (var poDetail in poDetails)
                    {
                        var poSuppliers = db.tb_po_supplier.Where(x => x.po_detail_id == poDetail.po_detail_id && x.is_selected == true).ToList();
                        foreach (var poSupplier in poSuppliers)
                        {
                            if (string.Compare(poSupplier.supplier_id, suppliers[i].ToString()) == 0)
                            {
                                tb_purchase_order_detail pDetail = db.tb_purchase_order_detail.Where(x => x.po_detail_id == poDetail.po_detail_id).FirstOrDefault();
                                pDetail.item_status = "cancelled";
                                db.SaveChanges();
                            }
                        }
                    }
                }
                tb_purchase_order po = db.tb_purchase_order.Find(id);
                Class.ItemRequest.RollbackItemRequestRemainQuantity(po.item_request_id, id, true, true, "cancelled");
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        #region PO Report
        public ActionResult GenerateReport(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = this.GetPOSupplierItem(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult GenerateReport(string id, string supplierID)
        {
            //return RedirectToAction("Report", new { type = "Purchase Order", id = id, supplierID = supplierID });
            #region added on oct 11 2018
            using (kim_mexEntities db=new kim_mexEntities())
            {
                int countItemVAT = 0;
                int countItemNotVAT = 0;
                bool isHasVAT = false;
                bool isHasNotVAT = false;
                string poStatus = db.tb_purchase_order.Where(m => string.Compare(id, m.purchase_order_id) == 0).Select(m => m.purchase_order_status).FirstOrDefault();
                List<PurchaseOrderDetailViewModel> poDetials = new List<PurchaseOrderDetailViewModel>();
                if (string.Compare(poStatus,"Approved")==0)
                {
                    poDetials = db.tb_purchase_order_detail.Where(m => string.Compare(id, m.purchase_order_id) == 0 && m.status == true && string.Compare(m.item_status, "Pending") == 0).Select(m => new PurchaseOrderDetailViewModel()
                    {
                        po_detail_id = m.po_detail_id,
                    }).ToList();
                }
                else if (string.Compare(poStatus, "Completed") == 0)
                {
                    poDetials = db.tb_purchase_order_detail.Where(m => string.Compare(id, m.purchase_order_id) == 0 && m.status == true && string.Compare(m.item_status,"approved")==0).Select(m=>new PurchaseOrderDetailViewModel()
                    {
                        po_detail_id=m.po_detail_id,
                    }).ToList();
                }
                else if (string.Compare(poStatus, Status.MREditted) == 0)
                {
                    poDetials = db.tb_purchase_order_detail.Where(m => string.Compare(id, m.purchase_order_id) == 0 && m.status == true && (string.Compare(m.item_status, "approved") == 0 || string.Compare(m.item_status, "Pending") == 0)).Select(m => new PurchaseOrderDetailViewModel()
                    {
                        po_detail_id = m.po_detail_id,
                    }).ToList();
                }
                if (poDetials != null)
                {
                    foreach(PurchaseOrderDetailViewModel item in poDetials)
                    {
                        var sup = db.tb_po_supplier.Where(m => string.Compare(m.po_detail_id, item.po_detail_id) == 0 && string.Compare(m.supplier_id, supplierID) == 0 && m.is_selected == true).FirstOrDefault();
                        //bool isVAT=Convert.ToBoolean(db.tb_po_supplier.Where(m => string.Compare(m.po_detail_id, item.po_detail_id) == 0 && string.Compare(m.supplier_id, supplierID) == 0 && m.is_selected == true).Select(m=>m.vat).FirstOrDefault());
                        if (sup != null)
                        {
                            if(sup.vat==true) countItemVAT++;
                            else countItemNotVAT++;
                        }
                    }
                    if (countItemVAT > 0) isHasVAT = true;
                    if (countItemNotVAT > 0) isHasNotVAT = true;
                }
                return Json(new { result = "success", poId = id, supplierId = supplierID,isHasVAT=isHasVAT,isHasNotVAT=isHasNotVAT }, JsonRequestBehavior.AllowGet);
            }
            #endregion
        }
        public ActionResult Report()
        {
            return View();
        }
        public ActionResult ReportNotVAT()
        {
            return View();
        }
        public ActionResult Print(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            return View(model);
        }
        #endregion
        public void DeletePurchaseOrderDetail(string id, bool isPurchaser)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<string> poDetails = new List<string>();
                var dPos = db.tb_purchase_order_detail.Where(m => m.purchase_order_id == id).ToList();
                if (dPos.Any())
                {
                    foreach (var dPo in dPos)
                    {
                        poDetails.Add(dPo.po_detail_id);
                    }
                }
                if (poDetails.Count() > 0)
                {
                    for (int i = 0; i < poDetails.Count(); i++)
                    {
                        string dPoID = poDetails[i];
                        //===> delete po supplier here 
                        //if (!isPurchaser) { }
                        this.DeletePOSupplier(dPoID);
                        tb_purchase_order_detail pod = db.tb_purchase_order_detail.Find(dPoID);
                        db.tb_purchase_order_detail.Remove(pod);
                        db.SaveChanges();
                    }
                }
            }
        }
        public void DeletePOSupplier(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<string> poSupID = new List<string>();
                var poSups = db.tb_po_supplier.Where(m => m.po_detail_id == id).ToList();
                if (poSups.Any())
                {
                    foreach (var poSup in poSups)
                    {
                        poSupID.Add(poSup.po_supplier_id);
                    }
                }
                if (poSupID.Count() > 0)
                {
                    for (int i = 0; i < poSupID.Count(); i++)
                    {
                        string posuID = poSupID[i];
                        tb_po_supplier posu = db.tb_po_supplier.Find(posuID);
                        db.tb_po_supplier.Remove(posu);
                        db.SaveChanges();
                    }
                }
            }
        }
        
        public PurchaseOrderViewModel GetPOSupplierItem(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from po in db.tb_purchase_order
                         join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                         join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                         join proj in db.tb_project on ir.ir_project_id equals proj.project_id
                         where po.purchase_order_id == id
                         select new PurchaseOrderViewModel() {
                             purchase_order_id = po.purchase_order_id,
                             item_request_id = po.item_request_id,
                             ir_no = pr.purchase_requisition_number,
                             project_full_name = proj.project_full_name,
                             purchase_oder_number = po.purchase_oder_number,
                             purchase_order_status = po.purchase_order_status,
                             created_date = po.created_date,
                             POLNumber = po.pol_project_short_name_number,
                             ShippingTo = po.shipping_to
                         }).FirstOrDefault();
                if (model != null)
                {
                    if (string.Compare(model.purchase_order_status, "Completed") == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "approved"
                                     select new PurchaseOrderDetailViewModel() { po_detail_id = dPO.po_detail_id, purchase_order_id = dPO.purchase_order_id, item_id = dPO.item_id, quantity = dPO.quantity, product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, unit_price = dPO.unit_price, item_status = dPO.item_status, po_quantity = dPO.po_quantity }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() {
                                        po_supplier_id = pos.po_supplier_id,
                                        po_detail_id = pos.po_detail_id,
                                        unit_price = pos.unit_price,
                                        supplier_id = sup.supplier_id,
                                        supplier_name = sup.supplier_name,
                                        sup_number = pos.sup_number,
                                        is_selected = pos.is_selected,
                                        vat =pos.vat,
                                        Reason = pos.Reason,
                                        po_quantity = pos.po_qty,
                                        supplier_address = sup.supplier_address,
                                        supplier_phone = sup.supplier_phone,
                                        supplier_email = sup.supplier_email,
                                        discount = sup.discount,
                                    }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
                                    supplier_id = sup,
                                    supplier_name = supplier.supplier_name,
                                    supplier_email = supplier.supplier_email,
                                    supplier_address = supplier.supplier_address,
                                    supplier_phone = supplier.supplier_phone
                                });
                            }

                        }
                    }
                    else if (string.Compare(model.purchase_order_status, "Approved") == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id && dPO.item_status == "Pending"
                                     select new PurchaseOrderDetailViewModel() { po_detail_id = dPO.po_detail_id, purchase_order_id = dPO.purchase_order_id, item_id = dPO.item_id, quantity = dPO.quantity, product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, unit_price = dPO.unit_price, item_status = dPO.item_status, po_quantity = dPO.po_quantity }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier() {
                                        po_supplier_id = pos.po_supplier_id,
                                        po_detail_id = pos.po_detail_id,
                                        unit_price = pos.unit_price,
                                        supplier_id = sup.supplier_id,
                                        supplier_name = sup.supplier_name,
                                        sup_number = pos.sup_number,
                                        is_selected = pos.is_selected,
                                        Reason = pos.Reason,
                                        po_quantity = pos.po_qty,
                                        supplier_address = sup.supplier_address,
                                        supplier_phone = sup.supplier_phone,
                                        supplier_email = sup.supplier_email,
                                        discount=sup.discount,
                                    }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
                                    supplier_id = sup,
                                    supplier_name = supplier.supplier_name,
                                    supplier_email = supplier.supplier_email,
                                    supplier_address = supplier.supplier_address,
                                    supplier_phone = supplier.supplier_phone
                                });
                            }

                        }
                    }else if(string.Compare(model.purchase_order_status, Status.MREditted) == 0)
                    {
                        List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                        poDetails = (from dPO in db.tb_purchase_order_detail
                                     join item in db.tb_product on dPO.item_id equals item.product_id
                                     where dPO.purchase_order_id == model.purchase_order_id &&(dPO.item_status == "approved" || dPO.item_status == "Pending")
                                     select new PurchaseOrderDetailViewModel() { po_detail_id = dPO.po_detail_id, purchase_order_id = dPO.purchase_order_id, item_id = dPO.item_id, quantity = dPO.quantity, product_code = item.product_code, product_name = item.product_name, product_unit = item.product_unit, unit_price = dPO.unit_price, item_status = dPO.item_status, po_quantity = dPO.po_quantity }).ToList();
                        foreach (var pod in poDetails)
                        {
                            //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                            PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                            pois = (from pos in db.tb_po_supplier
                                    join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                                    orderby pos.sup_number
                                    where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                                    select new PurchaseOrderItemSupplier()
                                    {
                                        po_supplier_id = pos.po_supplier_id,
                                        po_detail_id = pos.po_detail_id,
                                        unit_price = pos.unit_price,
                                        supplier_id = sup.supplier_id,
                                        supplier_name = sup.supplier_name,
                                        sup_number = pos.sup_number,
                                        is_selected = pos.is_selected,
                                        vat = pos.vat,
                                        Reason = pos.Reason,
                                        po_quantity = pos.po_qty,
                                        supplier_address = sup.supplier_address,
                                        supplier_phone = sup.supplier_phone,
                                        supplier_email = sup.supplier_email,
                                        discount = sup.discount,
                                    }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier()
                                {
                                    supplier_id = sup,
                                    supplier_name = supplier.supplier_name,
                                    supplier_email = supplier.supplier_email,
                                    supplier_address = supplier.supplier_address,
                                    supplier_phone = supplier.supplier_phone
                                });
                            }

                        }
                    }

                }
            }
            return model;
        }
        private string GetPOId()
        {
            string poNumber = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", poNum;
                string number = (from tbl in db.tb_purchase_order orderby tbl.created_date descending select tbl.purchase_oder_number).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    number = number.Substring(number.Length - 3, 3);
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1)
                        last_no = "00" + num;
                    else if (num.ToString().Length == 2)
                        last_no = "0" + num;
                    else if (num.ToString().Length == 3)
                        last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                poNumber = "POL-" + yy + "-" + mm + "-" + last_no;
            }
            return poNumber;
        }
        private List<tb_item_request> GetIRDropdownList()
        {
            List<tb_item_request> itemRequests = new List<tb_item_request>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                itemRequests = db.tb_item_request.OrderBy(m => m.ir_no).Where(m => m.status == true && m.ir_status == "Approved").ToList();
            }
            return itemRequests;
        }
        public List<Models.ItemRequestViewModel> GetItemRequestDropdownlist()
        {
            List<ItemRequestViewModel> itemRequests = new List<ItemRequestViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var objs = db.tb_item_request.OrderBy(m => m.ir_no).Where(m => m.status == true && m.ir_status == "Approved").ToList();
                if (objs.Any())
                {
                    foreach(var obj in objs)
                    {
                        var irrefs = db.tb_purchase_order.Where(m => string.Compare(m.item_request_id, obj.ir_id) == 0 && m.status==true && string.Compare(m.purchase_order_status,"Rejected")!=0).ToList();
                        if (irrefs.Any())
                        {

                        }
                        else
                        {
                            ItemRequestViewModel itemRequest = new ItemRequestViewModel();
                            itemRequest.ir_id = obj.ir_id;
                            itemRequest.ir_no = obj.ir_no;
                            itemRequests.Add(itemRequest);
                        }
                    }
                }
            }
            return itemRequests;
        }
        public ActionResult GetProjectNameByIRID(string id)
        {
            string projectName = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                projectName = (from ir in db.tb_item_request
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               where ir.ir_id == id && ir.status == true
                               select pro.project_full_name).FirstOrDefault();
                return Json(new { result = "success", data = projectName }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult GetItemRequest(string id)
        //{
        //    List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
        //    using(kim_mexEntities db=new kim_mexEntities())
        //    {
        //        var itemRequests = (from ddIr in db.tb_ir_detail2
        //                            join dIr in db.tb_ir_detail1 on ddIr.ir_detail1_id equals dIr.ir_detail1_id
        //                            join IR in db.tb_item_request on dIr.ir_id equals IR.ir_id
        //                            join item in db.tb_product on ddIr.ir_item_id equals item.product_id
        //                            where IR.ir_id == id && IR.status == true
        //                            select new { itemId=ddIr.ir_item_id,itemCode=item.product_code,itemName=item.product_name,itemIRQty=ddIr.ir_qty,itemUnit=item.product_unit }).ToList();
        //        if (itemRequests.Any())
        //        {
        //            foreach(var item in itemRequests)
        //            {
        //                items.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.itemId, product_code = item.itemCode, product_name = item.itemName, ir_qty = item.itemIRQty, product_unit = item.itemUnit });
        //            }
        //        }
        //    }
        //    return Json(new { result = "success", data = items }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetItemRequest(string id,string poId)
        {
            List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            List<QuoteViewModel> quotes = new List<QuoteViewModel>();
            List<InventoryViewModel> stockTransfers = new List<InventoryViewModel>();
            List<InventoryViewModel> purchaseOrders = new List<InventoryViewModel>();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                
                var stIDs = (from st in db.tb_stock_transfer_voucher
                             where st.item_request_id == id && st.status == true && string.Compare(st.stock_transfer_status,"Rejected")!=0
                             select st.stock_transfer_id).ToList();
                if (stIDs.Any())
                {
                    foreach (var stId in stIDs)
                    {
                        //stockTransfers = (from inv in db.tb_inventory
                        //                  where inv.ref_id == stId
                        //                  select new InventoryViewModel() { product_id = inv.product_id, out_quantity = inv.out_quantity }).ToList();
                        stockTransfers = db.tb_stock_transfer_detail.Where(x => string.Compare(x.st_ref_id, stId) == 0).Select(x => new InventoryViewModel()
                        {
                            product_id=x.st_item_id,
                            unit=x.unit,
                            total_quantity=x.quantity,
                        }).ToList() ;
                    }
                }

                if (string.IsNullOrEmpty(poId))
                {
                    var purchaseOrderRefs = db.tb_purchase_order.OrderBy(x => x.created_date).Where(x => x.status == true && string.Compare(x.item_request_id, id) == 0 && string.Compare(x.purchase_order_status, "Rejected") != 0).ToList();
                    if (purchaseOrderRefs.Any())
                    {
                        foreach (var po in purchaseOrderRefs)
                        {
                            var poDetails = db.tb_purchase_order_detail.Where(x => x.status == true && string.Compare(x.purchase_order_id, po.purchase_order_id) == 0 && string.Compare(x.item_status, "rejected") != 0).ToList();
                            if (poDetails.Any())
                            {
                                foreach (var poDetail in poDetails)
                                {
                                    InventoryViewModel item = new InventoryViewModel();
                                    item.product_id = poDetail.item_id;
                                    item.unit = string.IsNullOrEmpty(poDetail.po_unit) ? Regex.Replace(poDetail.item_unit.Trim(), @"\t|\n|\r", "") : Regex.Replace(poDetail.po_unit.Trim(), @"\t|\n|\r", "");
                                    item.total_quantity = (Convert.ToDecimal(poDetail.po_quantity) == 0 || poDetail.po_quantity == null) ? poDetail.quantity : poDetail.po_quantity;
                                    purchaseOrders.Add(item);
                                }
                            }

                        }
                    }
                }
                else
                {
                    var purchaseOrderRefs = db.tb_purchase_order.OrderBy(x => x.created_date).Where(x => x.status == true && string.Compare(x.item_request_id, id) == 0 && string.Compare(x.purchase_order_id,poId)!=0 && string.Compare(x.purchase_order_status, "Rejected") != 0).ToList();
                    if (purchaseOrderRefs.Any())
                    {
                        foreach (var po in purchaseOrderRefs)
                        {
                            var poDetails = db.tb_purchase_order_detail.Where(x => x.status == true && string.Compare(x.purchase_order_id, po.purchase_order_id) == 0 && string.Compare(x.item_status, "rejected") != 0).ToList();
                            if (poDetails.Any())
                            {
                                foreach (var poDetail in poDetails)
                                {
                                    InventoryViewModel item = new InventoryViewModel();
                                    item.product_id = poDetail.item_id;
                                    item.unit = string.IsNullOrEmpty(poDetail.po_unit) ? Regex.Replace(poDetail.item_unit.Trim(), @"\t|\n|\r", "") : Regex.Replace(poDetail.po_unit.Trim(), @"\t|\n|\r", "");
                                    item.total_quantity = (Convert.ToDecimal(poDetail.po_quantity) == 0 || poDetail.po_quantity == null) ? poDetail.quantity : poDetail.po_quantity;
                                    purchaseOrders.Add(item);
                                }
                            }

                        }
                    }
                }
                
                var itemRequests = (from ddIr in db.tb_ir_detail2
                                    join dIr in db.tb_ir_detail1 on ddIr.ir_detail1_id equals dIr.ir_detail1_id
                                    join IR in db.tb_item_request on dIr.ir_id equals IR.ir_id
                                    join item in db.tb_product on ddIr.ir_item_id equals item.product_id
                                    orderby item.product_code
                                    where IR.ir_id == id && IR.status == true && ddIr.is_approved == true
                                    select new { itemId = ddIr.ir_item_id, itemCode = item.product_code, itemName = item.product_name, itemIRQty = ddIr.ir_qty, itemUnit = item.product_unit, approvedQty = ddIr.approved_qty, itemRequestedUnit = ddIr.ir_item_unit }).ToList();

                if (itemRequests.Any())
                {
                    foreach (var item in itemRequests)
                    {
                        decimal remainApprovedQty = Convert.ToDecimal(item.approvedQty); ;
                        string itemId = item.itemId;
                        var itemsQuote = (from it in db.tb_quote_detail
                                          join quote in db.tb_quote on it.quote_id equals quote.quote_id
                                          join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                                          orderby quote.created_date descending
                                          where string.Compare(it.item_id, itemId) == 0 && quote.status == true
                                          select new QuoteViewModel { supplier_id = quote.supplier_id, supplier_name = supplier.supplier_name, item_id = it.item_id, price = it.price, created_date = quote.created_date }).ToList();

                        var dupSupplier = itemsQuote.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                        foreach (var dupS in dupSupplier)
                        {
                            var qu = itemsQuote.Where(x => x.supplier_id == dupS).FirstOrDefault();
                            quotes.Add(new QuoteViewModel { supplier_id = qu.supplier_id, supplier_name = qu.supplier_name, item_id = qu.item_id, price = qu.price });
                        }
                        bool check = false;
                        if (itemsQuote.Any())
                        {
                            foreach (QuoteViewModel q in itemsQuote)
                            {
                                check = false;
                                if (dupSupplier.Any())
                                {
                                    foreach (var dupS in dupSupplier)
                                    {
                                        if (dupS == q.supplier_id)
                                        {
                                            check = true;
                                            break;

                                        }
                                        //else
                                        //quotes.Add(new QuoteViewModel { supplier_id = q.supplier_id, supplier_name = q.supplier_name, item_id = q.item_id, price = q.price });
                                    }
                                    if (!check)
                                        quotes.Add(new QuoteViewModel { supplier_id = q.supplier_id, supplier_name = q.supplier_name, item_id = q.item_id, price = q.price });
                                }
                                else
                                {
                                    quotes.Add(new QuoteViewModel { supplier_id = q.supplier_id, supplier_name = q.supplier_name, item_id = q.item_id, price = q.price });
                                }
                            }
                        }
                        remainApprovedQty = Convert.ToDecimal(item.approvedQty);
                        if (stockTransfers.Count > 0)
                        {
                            foreach (var st in stockTransfers)
                                if (st.product_id == item.itemId)
                                    remainApprovedQty = remainApprovedQty - CommonClass.ConvertMultipleUnit(st.product_id, st.unit, Convert.ToDecimal(st.total_quantity));
                            //remainApprovedQty = remainApprovedQty + Convert.ToDecimal(item.approvedQty - st.out_quantity);
                            //else
                            //remainApprovedQty = Convert.ToDecimal(item.approvedQty);
                        }
                        /*
                        else
                        {
                            remainApprovedQty = Convert.ToDecimal(item.approvedQty);
                        }
                        */
                        if (purchaseOrders.Count() > 0)
                        {
                            var results = purchaseOrders.Where(x => string.Compare(x.product_id, item.itemId) == 0).ToList();
                            if (results.Any())
                            {
                                
                                foreach(var rs in results)
                                {
                                   remainApprovedQty = remainApprovedQty - CommonClass.ConvertMultipleUnit(rs.product_id, rs.unit, Convert.ToDecimal(rs.total_quantity));
                                   

                                }
                            }
                        }
                        if(stockTransfers.Count()==0 && purchaseOrders.Count()==0)
                        {
                            remainApprovedQty = Convert.ToDecimal(item.approvedQty);
                        }

                        if (remainApprovedQty > 0)
                            items.Add(new ItemRequestDetail2ViewModel() { ir_item_id = item.itemId, product_code = item.itemCode, product_name = item.itemName, ir_qty = item.itemIRQty, requested_unit = item.itemRequestedUnit, product_unit = item.itemUnit, approved_qty = item.approvedQty, remain_approved_qty = remainApprovedQty });
                    }
                }


            }
            return Json(new { result = "success", data = items, quotes = quotes }, JsonRequestBehavior.AllowGet);
        }
        //#region block by terd apr 02 2020 old process
        //public ActionResult GetItemRequestJson(string id,string poId=null)
        //{
        //    List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
        //    List<QuoteViewModel> quotes = new List<QuoteViewModel>();
        //    using(kim_mexEntities db=new kim_mexEntities())
        //    {
        //        items = Class.ItemRequest.GetAllAvailableItem(id,poId);
        //        if (items.Any())
        //        {
        //            foreach(var item in items)
        //            {
        //                string itemId = item.ir_item_id;
        //                var itemsQuote = (from it in db.tb_quote_detail
        //                                  join quote in db.tb_quote on it.quote_id equals quote.quote_id
        //                                  join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
        //                                  orderby quote.created_date descending
        //                                  where string.Compare(it.item_id, itemId) == 0 && quote.status == true && string.Compare(quote.quote_status,Status.Completed)==0
        //                                  select new QuoteViewModel { supplier_id = quote.supplier_id, supplier_name = supplier.supplier_name, item_id = it.item_id, price = it.price, created_date = quote.created_date }).ToList();

        //                var dupSupplier = itemsQuote.GroupBy(x => x.supplier_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        //                foreach (var dupS in dupSupplier)
        //                {
        //                    var qu = itemsQuote.Where(x => x.supplier_id == dupS).FirstOrDefault();
        //                    quotes.Add(new QuoteViewModel { supplier_id = qu.supplier_id, supplier_name = qu.supplier_name, item_id = qu.item_id, price = qu.price });
        //                }
        //                bool check = false;
        //                if (itemsQuote.Any())
        //                {
        //                    foreach (QuoteViewModel q in itemsQuote)
        //                    {
        //                        check = false;
        //                        if (dupSupplier.Any())
        //                        {
        //                            foreach (var dupS in dupSupplier)
        //                            {
        //                                if (dupS == q.supplier_id)
        //                                {
        //                                    check = true;
        //                                    break;

        //                                }
        //                            }
        //                            if (!check)
        //                                quotes.Add(new QuoteViewModel { supplier_id = q.supplier_id, supplier_name = q.supplier_name, item_id = q.item_id, price = q.price });
        //                        }
        //                        else
        //                        {
        //                            quotes.Add(new QuoteViewModel { supplier_id = q.supplier_id, supplier_name = q.supplier_name, item_id = q.item_id, price = q.price });
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return Json(new { result = "success", data = items, quotes = quotes }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetItemRequestJson(string id,string poId = null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
                List<QuoteViewModel> quotes = new List<QuoteViewModel>();
                tb_purchase_requisition pr = db.tb_purchase_requisition.Find(id);
                List<SupplierViewModel> supplierQuotes = new List<SupplierViewModel>();
                if(pr!=null)
                    items = Class.ItemRequest.GetAllAvailableItem(pr.material_request_id, poId);
                var quotesups = (from quote in db.tb_quote
                          join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                          orderby supplier.supplier_name, quote.updated_date descending
                          where quote.status == true && string.Compare(quote.mr_id, id) == 0 
                          && (string.Compare(quote.quote_status,Status.Approved)==0 || string.Compare(quote.quote_status, Status.Edit) == 0)
                          select new
                          {
                              quote,supplier
                          }).ToList().DistinctBy(s=>s.supplier);

                if (quotesups.Any())
                {
                    foreach(var i in quotesups)
                    {
                        SupplierViewModel supplier = SupplierViewModel.ConvertEntityToModel(i.supplier);
                        supplier.lump_sum_discount_amount = i.quote.lump_sum_discount_amount;

                        List<QuoteViewModel> quoteSupplierItems = db.tb_quote_detail.Where(s => string.Compare(s.quote_id, i.quote.quote_id) == 0).Select(s => new QuoteViewModel()
                        {
                            supplier_id=i.quote.supplier_id,
                            supplier_name=i.supplier.supplier_name,
                            item_id=s.item_id,
                            price=s.price,
                            discount=s.discount,
                            discountAmount=s.discount_amount
                        }).ToList();
                        foreach (var ii in quoteSupplierItems)
                        {
                            var isItemQuoteExist = items.Where(s => string.Compare(ii.item_id, s.ir_item_id) == 0).FirstOrDefault();
                            if (isItemQuoteExist != null)
                            {
                                QuoteViewModel iii = new QuoteViewModel();
                                iii.supplier_id = ii.supplier_id;
                                iii.supplier_name = ii.supplier_name;
                                iii.item_id = ii.item_id;
                                iii.price = ii.price;
                                iii.incoterm = i.quote.incoterm;
                                iii.payment = i.quote.payment;
                                iii.delivery = i.quote.delivery;
                                iii.shipment = i.quote.shipment;
                                iii.warranty = i.quote.warranty;
                                iii.vendor_ref = i.quote.vendor_ref;
                                iii.discount = ii.discount;
                                if (ii.discountAmount.HasValue)
                                {
                                    iii.priceDiscount = iii.price - ii.discountAmount;
                                }
                                else
                                {
                                    iii.priceDiscount = iii.discount > 0 ? iii.price - (iii.price * (iii.discount / 100)) : iii.price;
                                }

                                iii.discountAmount = ii.discountAmount;
                                iii.lump_sum_discount_amount = supplier.lump_sum_discount_amount;
                                quotes.Add(iii);
                            }                           
                        }

                        var isSupplierHaveItem = quotes.Where(s => string.Compare(s.supplier_id, supplier.supplier_id) == 0).ToList();
                        if(isSupplierHaveItem.Count()>0)
                            supplierQuotes.Add(supplier);

                    }
                    
                }

                supplierQuotes = supplierQuotes.DistinctBy(s => s.supplier_id).ToList();

                //var groupSupplier = quotes.GroupBy(s => s.supplier_id).Select(s => new { s }).ToList();

                return Json(new { result = "success", data = items, quotes = quotes, supplierQuotes= supplierQuotes }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPurchasseOrderDataTable(string po_status)
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                IQueryable<PurchaseOrderViewModel> pos;
                if (po_status == "All")
                {
                    pos = (from po in db.tb_purchase_order
                           join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = ir.ir_no, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                }
                else
                {
                    pos = (from po in db.tb_purchase_order
                           join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && po.purchase_order_status == po_status
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = ir.ir_no, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                }
                if (pos.Any())
                {
                    foreach (var po in pos)
                    {
                        List<string> ReportNumberVAT = new List<string>();
                        List<string> ReportNumberNonVAT = new List<string>();
                        var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        foreach(var vat in vatNumbers)
                            ReportNumberVAT.Add(vat.po_report_number);
                        foreach (var vat in vatNonNumbers)
                            ReportNumberNonVAT.Add(vat.po_report_number);
                        purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.purchase_order_id, ir_no = po.ir_no, project_full_name = po.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date ,ReportNumberVAT=ReportNumberVAT,ReportNumberNonVAT=ReportNumberNonVAT});
                    }
                }
            }
            //return Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetItemAutoSuggestionByCodes(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json(db.tb_product.Where(c => c.product_code.StartsWith(term) && c.status == true).Select(a => new { label = a.product_code + " " + a.product_name, id = a.product_id }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetItemDataByIRID(string ir_id, string item_id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                ItemRequestDetail2ViewModel item = new ItemRequestDetail2ViewModel();
                item = (from ddIr in db.tb_ir_detail2
                        join dIr in db.tb_ir_detail1 on ddIr.ir_detail1_id equals dIr.ir_detail1_id
                        join ir in db.tb_item_request on dIr.ir_id equals ir.ir_id
                        join it in db.tb_product on ddIr.ir_item_id equals it.product_id
                        where ddIr.ir_item_id == item_id && ir.ir_id == ir_id
                        select new ItemRequestDetail2ViewModel() { ir_item_id = ddIr.ir_item_id, product_code = it.product_code, product_name = it.product_name, product_unit = it.product_unit }).FirstOrDefault();
                return Json(new { result = "success", data = item }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSupplierDropdownList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<tb_supplier> suppliers = new List<tb_supplier>();
                suppliers = db.tb_supplier.OrderBy(m => m.supplier_name).Where(m => m.status == true).ToList();
                return Json(new { data = suppliers }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPurchaseOrderDetailJson(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            model = PurchaseOrderViewModel.GetPurchaseOrderDetail(id);
            return Json(new { result = "success", data = model }, JsonRequestBehavior.AllowGet);
        }
        public List<tb_supplier> GetSuppliersDropdown()
        {
            List<tb_supplier> suppliers = new List<tb_supplier>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                suppliers = db.tb_supplier.OrderBy(m => m.supplier_name).Where(m => m.status == true).ToList();
            }
            return suppliers;
        }
        public ActionResult GetItemRequestsbyProject(string id)
        {
            List<ItemRequestViewModel> itemRequests = new List<ItemRequestViewModel>();
            itemRequests = ItemRequest.GetAllItemRequestDropdownList().Where(w => string.Compare(w.ir_project_id, id) == 0).ToList();
            return Json(itemRequests,JsonRequestBehavior.AllowGet);
        }
        //Rathana Add 10.04.2019
        [HttpPost]
        public JsonResult GetProjectShortNameByProjectID(string id)
        {
            using(var db = new Entities.kim_mexEntities())
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string PShortName = db.tb_project.FirstOrDefault(x => x.project_id == id).project_short_name;
                    return Json(new {success = true, ProShortName = PShortName }, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
        }
        [HttpPost]
        public JsonResult GetUserDetailAutocomplete(string term)
        {
            try
            {
                using (var db = new Entities.kim_mexEntities())
                {
                    var datas = db.tb_user_detail.Where(x => (x.user_first_name.Contains(term) || x.user_last_name.Contains(term)) && x.status == true)
                                                .Select(x => new
                                                {
                                                    label = x.user_first_name + " " + x.user_last_name,
                                                    value = x.user_detail_id
                                                }).ToArray();
                    return Json(datas, JsonRequestBehavior.AllowGet);
                }
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "PurchaseOrderController.cs", "GetUserDetailAutocomplete", ex.StackTrace, ex.Message);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        //End Rathana Add
        public ActionResult MyRequest()
        {
            return View();
        }
        public ActionResult MyApproval()
        {
            return View();
        }
        public ActionResult GetMyPendingRequestbyUser()
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                IQueryable<PurchaseOrderViewModel> pos;
                if (User.IsInRole("Admin"))
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && string.Compare(po.purchase_order_status, "Pending") == 0
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date,mr_id=ir.ir_id,mr_no=ir.ir_no });
                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && string.Compare(po.purchase_order_status, "Pending") == 0 && string.Compare(po.created_by,userId)==0
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date, mr_id = ir.ir_id, mr_no = ir.ir_no });
                }
                if (pos.Any())
                {
                    foreach (var po in pos)
                    {
                        List<string> ReportNumberVAT = new List<string>();
                        List<string> ReportNumberNonVAT = new List<string>();
                        var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        foreach (var vat in vatNumbers)
                            ReportNumberVAT.Add(vat.po_report_number);
                        foreach (var vat in vatNonNumbers)
                            ReportNumberNonVAT.Add(vat.po_report_number);
                        string created_at_text = CommonClass.ToLocalTime(Convert.ToDateTime(po.created_date)).ToString("dd/MM/yyyy");
                        purchaseOrders.Add(new PurchaseOrderViewModel() {
                            purchase_order_id = po.purchase_order_id,
                            purchase_oder_number = po.purchase_oder_number,
                            item_request_id = po.purchase_order_id,
                            ir_no = po.ir_no,
                            project_full_name = po.project_full_name,
                            purchase_order_status = po.purchase_order_status,
                            created_by = po.created_by, 
                            checked_by = po.checked_by, 
                            approved_by = po.approved_by, 
                            created_date = po.created_date, 
                            ReportNumberVAT = ReportNumberVAT, 
                            ReportNumberNonVAT = ReportNumberNonVAT ,
                            created_at_text=created_at_text
                        });
                    }
                }
            }
            var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetMyRequestedHistorybyUser(string dateRange,string status)
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                IQueryable<PurchaseOrderViewModel> pos;

                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                //if (User.IsInRole("Admin"))
                //{
                //    pos = (from po in db.tb_purchase_order
                //           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                //           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                //           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                //           where po.status == true && string.Compare(po.purchase_order_status, "Pending") !=0 
                //           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                //}
                //else
                //{
                string userId = User.Identity.GetUserId();
                if (string.Compare(status, "0") == 0)
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && string.Compare(po.purchase_order_status, "Pending") != 0 && ( string.Compare(po.created_by, userId) == 0 || string.Compare(po.updated_by, userId) == 0) && po.created_date>=startDate && po.created_date<=endDate
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                }else
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && string.Compare(po.purchase_order_status, "Pending") != 0 && (string.Compare(po.created_by, userId) == 0 || string.Compare(po.updated_by, userId) == 0) && string.Compare(po.created_by, userId) == 0 && po.created_date >= startDate && po.created_date <= endDate && string.Compare(po.purchase_order_status,status)==0
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                }
                    
                //}
                if (pos.DistinctBy(s=>s.purchase_order_id).Any())
                {
                    foreach (var po in pos)
                    {
                        List<string> ReportNumberVAT = new List<string>();
                        List<string> ReportNumberNonVAT = new List<string>();
                        var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        foreach (var vat in vatNumbers)
                            ReportNumberVAT.Add(vat.po_report_number);
                        foreach (var vat in vatNonNumbers)
                            ReportNumberNonVAT.Add(vat.po_report_number);
                        purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.purchase_order_id, ir_no = po.ir_no, project_full_name = po.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date, ReportNumberVAT = ReportNumberVAT, ReportNumberNonVAT = ReportNumberNonVAT });
                    }
                }
            }
            var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetMyPendingApprovalRequest()
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                IQueryable<PurchaseOrderViewModel> pos;
                if (User.IsInRole("Admin") || (User.IsInRole("Chief of Finance Officer") &&User.IsInRole("Director")))
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && string.Compare(po.purchase_order_status, "Completed") != 0 && string.Compare(po.purchase_order_status, "Rejected") != 0
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                    if (pos.Any())
                    {
                        foreach (var po in pos)
                        {
                            List<string> ReportNumberVAT = new List<string>();
                            List<string> ReportNumberNonVAT = new List<string>();
                            var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                            var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                            foreach (var vat in vatNumbers)
                                ReportNumberVAT.Add(vat.po_report_number);
                            foreach (var vat in vatNonNumbers)
                                ReportNumberNonVAT.Add(vat.po_report_number);
                            purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.purchase_order_id, ir_no = po.ir_no, project_full_name = po.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date, ReportNumberVAT = ReportNumberVAT, ReportNumberNonVAT = ReportNumberNonVAT });
                        }
                    }
                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    pos = null;
                    if (User.IsInRole("Chief of Finance Officer"))
                    {
                        pos = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               where po.status == true && string.Compare(po.purchase_order_status, "Pending") == 0
                               select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                    }
                    else if (User.IsInRole("Director"))
                    {
                        pos = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               where po.status == true && string.Compare(po.purchase_order_status, "Approved") == 0
                               select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                    }

                    if (pos.Any())
                    {
                        foreach (var po in pos)
                        {
                            List<string> ReportNumberVAT = new List<string>();
                            List<string> ReportNumberNonVAT = new List<string>();
                            var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                            var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                            foreach (var vat in vatNumbers)
                                ReportNumberVAT.Add(vat.po_report_number);
                            foreach (var vat in vatNonNumbers)
                                ReportNumberNonVAT.Add(vat.po_report_number);
                            purchaseOrders.Add(new PurchaseOrderViewModel() { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.purchase_order_id, ir_no = po.ir_no, project_full_name = po.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date, ReportNumberVAT = ReportNumberVAT, ReportNumberNonVAT = ReportNumberNonVAT });
                        }
                    }
                }
                
            }
            var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetMyApprovalHistoryRequested(string dateRange, string status)
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
                string userid = User.Identity.GetUserId();
                IQueryable<PurchaseOrderViewModel> pos;

                if (string.Compare(status, "0") == 0)
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && (string.Compare(po.checked_by, userid) == 0 || string.Compare(po.approved_by, userid) == 0) && po.created_date>=startDate && po.created_date<=endDate
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date ,
                            mr_id=ir.ir_id,
                           mr_no=ir.ir_no});
                }
                else
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true && (string.Compare(po.checked_by, userid) == 0 || string.Compare(po.approved_by, userid) == 0) && po.created_date >= startDate && po.created_date <= endDate && string.Compare(po.purchase_order_status,status)==0
                           select new PurchaseOrderViewModel { purchase_order_id = po.purchase_order_id, purchase_oder_number = po.purchase_oder_number, item_request_id = po.item_request_id, ir_no = pr.purchase_requisition_number, project_full_name = pro.project_full_name, purchase_order_status = po.purchase_order_status, created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date,
                               mr_no = ir.ir_no
                           });
                }

                
                if (pos.Any())
                {
                    foreach (var po in pos)
                    {
                        List<string> ReportNumberVAT = new List<string>();
                        List<string> ReportNumberNonVAT = new List<string>();
                        var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        foreach (var vat in vatNumbers)
                            ReportNumberVAT.Add(vat.po_report_number);
                        foreach (var vat in vatNonNumbers)
                            ReportNumberNonVAT.Add(vat.po_report_number);
                        string created_at_text = CommonClass.ToLocalTime(Convert.ToDateTime(po.created_date)).ToString("dd/MM/yyyy");
                        purchaseOrders.Add(new PurchaseOrderViewModel() {
                            purchase_order_id = po.purchase_order_id,
                            purchase_oder_number = po.purchase_oder_number,
                            item_request_id = po.purchase_order_id,
                            ir_no = po.ir_no,
                            project_full_name = po.project_full_name,
                            purchase_order_status = po.purchase_order_status,
                            created_by = po.created_by,
                            checked_by = po.checked_by,
                            approved_by = po.approved_by,
                            created_date = po.created_date,
                            ReportNumberVAT = ReportNumberVAT,
                            ReportNumberNonVAT = ReportNumberNonVAT,
                            created_by_fullname=CommonClass.GetUserFullnameByUserId(po.created_by),
                            mr_no=po.mr_no,
                            mr_id=po.mr_id,
                            created_at_text=created_at_text,
                        });
                    }
                }


            }
            var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult GetQuoteApprovalListAJax()
        {
            try
            {
                List<PurchaseOrderViewModel> purchaseOrders = CommonFunctions.GetMyPendingApprovalRequest(User.Identity.GetUserId(), User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.ProjectManager), User.IsInRole(Role.TechnicalDirector),User.IsInRole(Role.OperationDirector));
                var jsonResult = Json(new { data = purchaseOrders }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch(Exception ex)
            {
                return Json(new { message=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult List()
        {
            //return View(this.GetQuoteListItems());
            return View();
        }
        public ActionResult GetQuoteItemDatatablebyDateRangeandStatusAJAX(string dateRange,string status)
        {
            return Json(new { data = this.GetQuoteListItemsByDateRangeandStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }
        public List<PurchaseOrderViewModel> GetQuoteListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from quote in db.tb_purchase_order
                              join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                              orderby quote.created_date descending
                              where quote.status == true
                              select new PurchaseOrderViewModel()
                              {
                                  purchase_order_id=quote.purchase_order_id,
                                  purchase_oder_number=quote.purchase_oder_number,
                                  created_date=quote.created_date,
                                  created_by=quote.created_by,
                                  item_request_id=pr.purchase_requisition_id,
                                  ir_no=pr.purchase_requisition_number,
                                  purchase_order_status=quote.purchase_order_status,
                                  project_full_name=pro.project_full_name
                              }).ToList();
            }
        }
        public List<PurchaseOrderViewModel> GetQuoteListItemsByDateRangeandStatus(string dateRange, string status)
        {
            List<PurchaseOrderViewModel> models = new List<PurchaseOrderViewModel>();


            using(kim_mexEntities db=new kim_mexEntities())
            {
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
                List<QuoteListResultModel> results = new List<QuoteListResultModel>();

                if(User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.Purchaser) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.ManagingDirector) || User.IsInRole(Role.TechnicalDirector)
                        || User.IsInRole(Role.OperationDirector) || User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager) || User.IsInRole(Role.AstOperationDirector))
                {
                    results = (from quote in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                               join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                               join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                               orderby quote.created_date descending
                               where quote.status == true && quote.created_date >= startDate && quote.created_date <= endDate
                               select new QuoteListResultModel() { quote = quote, pr = pr, mr = mr, pro = pro }).ToList();
                }
                else
                {
                    string userId = User.Identity.GetUserId();
                    if (User.IsInRole(Role.ProjectManager))
                    {
                        var filterResults= (from quote in db.tb_purchase_order
                                            join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                            join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                            join pm in db.tb_project_pm on pro.project_id equals pm.project_id
                                            orderby quote.created_date descending
                                            where quote.status == true && quote.created_date >= startDate && quote.created_date <= endDate && string.Compare(pm.project_manager_id,userId)==0
                                            select new QuoteListResultModel() { quote = quote, pr = pr, mr = mr, pro = pro }).ToList();
                        results.AddRange(filterResults);
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        var filterResults = (from quote in db.tb_purchase_order
                                             join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                                             join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                             join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                             join sm in db.tb_site_manager_project on pro.project_id equals sm.project_id
                                             orderby quote.created_date descending
                                             where quote.status == true && quote.created_date >= startDate && quote.created_date <= endDate && string.Compare(sm.site_manager, userId) == 0
                                             select new QuoteListResultModel() { quote = quote, pr = pr, mr = mr, pro = pro }).ToList();
                        results.AddRange(filterResults);
                    }
                }

                    

                foreach(var rs in results)
                {
                    PurchaseOrderViewModel model = new PurchaseOrderViewModel();
                    model.purchase_order_id = rs.quote.purchase_order_id;
                    model.purchase_oder_number = rs.quote.purchase_oder_number;
                    model.created_date = rs.quote.created_date;
                    model.created_by = rs.quote.created_by;
                    model.item_request_id = rs.pr.purchase_requisition_id;
                    model.ir_no = rs.pr.purchase_requisition_number;
                    model.purchase_order_status = rs.quote.purchase_order_status;
                    model.project_full_name = rs.pro.project_full_name;
                    model.mr_id = rs.mr.ir_id;
                    model.mr_no = rs.mr.ir_no;
                    model.created_at_text = Convert.ToDateTime(rs.quote.created_date).ToString("yyyy-MM-dd");
                    model.purchase_order_status = rs.quote.purchase_order_status;
                    models.Add(model);
                }

                if (string.Compare(status, "0") != 0)
                {
                    models = models.Where(s => string.Compare(s.purchase_order_status, status) == 0).ToList();
                }

                //if (string.Compare(status, "0") == 0)
                //{
                //    models= (from quote in db.tb_purchase_order
                //             join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                //             join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                //             join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                //             orderby quote.created_date descending
                //             where quote.status == true && quote.created_date>=startDate && quote.created_date<=endDate
                //             select new PurchaseOrderViewModel()
                //             {
                //                 purchase_order_id = quote.purchase_order_id,
                //                 purchase_oder_number = quote.purchase_oder_number,
                //                 created_date = quote.created_date,
                //                 created_by = quote.created_by,
                //                 item_request_id = pr.purchase_requisition_id,
                //                 ir_no = pr.purchase_requisition_number,
                //                 purchase_order_status = quote.purchase_order_status,
                //                 project_full_name = pro.project_full_name,
                //                 mr_id=mr.ir_id,
                //                 mr_no=mr.ir_no,
                //                 created_at_text=Convert.ToDateTime(quote.created_date).ToString("yyyy-MM-dd"),
                //             }).ToList();
                //}
                //else
                //{
                //    models = (from quote in db.tb_purchase_order
                //              join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                //              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                //              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                //              orderby quote.created_date descending
                //              where quote.status == true && quote.created_date >= startDate && quote.created_date <= endDate
                //              && string.Compare(quote.purchase_order_status,status)==0
                //              select new PurchaseOrderViewModel()
                //              {
                //                  purchase_order_id = quote.purchase_order_id,
                //                  purchase_oder_number = quote.purchase_oder_number,
                //                  created_date = quote.created_date,
                //                  created_by = quote.created_by,
                //                  item_request_id = pr.purchase_requisition_id,
                //                  ir_no = pr.purchase_requisition_number,
                //                  purchase_order_status = quote.purchase_order_status,
                //                  project_full_name = pro.project_full_name,
                //                  mr_id = mr.ir_id,
                //                  mr_no = mr.ir_no,
                //                  created_at_text = Convert.ToDateTime(quote.created_date).ToString("yyyy-MM-dd"),
                //              }).ToList();
                //}
            }

            return models;
        }
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        public ActionResult RequestCancel(string id,string comment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                tb_purchase_order quote = db.tb_purchase_order.Find(id);
                quote.purchase_order_status = Status.RequestCancelled;
                quote.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                quote.updated_by = User.Identity.GetUserId();
                db.SaveChanges();

                var quoteDetails = db.tb_purchase_order_detail.Where(s => string.Compare(s.purchase_order_id, quote.purchase_order_id) == 0).ToList();
                foreach(var qd in quoteDetails)
                {
                    tb_purchase_order_detail quoteDetail = db.tb_purchase_order_detail.Find(qd.po_detail_id);
                    quoteDetail.item_status = Status.RequestCancelled;
                    db.SaveChanges();
                }

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), quote.purchase_order_id, quote.purchase_order_status, quote.updated_by,quote.updated_date,comment);
                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.QuoteRequestCancelled,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.QuoteRequestCancelled);
                ItemRequest.RollbackItemRequestRemainQuantity(quote.item_request_id, id, true, false);

                

                return Json(new { isSuccess = true}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new {isSuccess=false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RejectResubmit(string id,string comment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_purchase_order quote = db.tb_purchase_order.Find(id);
                
                if (string.Compare(quote.purchase_order_status, "Pending") == 0)
                {
                    quote.checked_by = User.Identity.GetUserId();
                    quote.checked_date = CommonClass.ToLocalTime(DateTime.Now);
                }else if (string.Compare(quote.purchase_order_status, "Approved") == 0)
                {
                    quote.approved_by = User.Identity.GetUserId();
                    quote.approved_date = CommonClass.ToLocalTime(DateTime.Now);
                }else if (string.Compare(quote.purchase_order_status, Status.Checked) == 0)
                {
                    quote.od_checked_by= User.Identity.GetUserId();
                    quote.od_checked_date = CommonClass.ToLocalTime(DateTime.Now);
                }
                quote.purchase_order_status = Status.Rejected;
                db.SaveChanges();

                var quoteDetails = db.tb_purchase_order_detail.Where(s => string.Compare(s.purchase_order_id, quote.purchase_order_id) == 0).ToList();
                foreach (var qd in quoteDetails)
                {
                    tb_purchase_order_detail quoteDetail = db.tb_purchase_order_detail.Find(qd.po_detail_id);
                    quoteDetail.item_status = "rejected";
                    db.SaveChanges();
                }

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), quote.purchase_order_id, quote.purchase_order_status, User.Identity.GetUserId(), CommonClass.ToLocalTime(DateTime.Now), comment);
                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.QuoteRejected,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(quote.item_request_id), ShowStatus.QuoteRejected);
                ItemRequest.RollbackItemRequestRemainQuantity(quote.item_request_id, id, true, false);

                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = quote.purchase_order_id;
                reject.ref_type = "Purchase Order";
                reject.comment = comment;
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                reject.rejected_by = User.Identity.GetUserId();
                db.tb_reject.Add(reject);
                db.SaveChanges();

                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            return File(Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", "Grid.xls");
        }
        public ActionResult ExportExcel(string id,string supplierId,bool isVAT,bool isPO=false,bool isQuote=false,bool isMR=false,bool isSignature=false)
        {
            return View(PurchaseOrderReportGenerateModel.GetPurchaseOrderGenerate(id,supplierId,isVAT,isPO,isQuote,isMR,isSignature));
        }

    }
}