using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.IO;
using System.Net;
using Microsoft.AspNet.Identity;
using BT_KimMex.Class;
using System.Drawing;
using Aspose.Words;
using Spire.Pdf;

namespace BT_KimMex.Controllers
{
    //[Authorize(Roles = "Admin,Price Manager")]
    [Authorize]
    public class QuoteController : Controller
    {
      
        // GET: Quote
        public ActionResult Index()
        {
            return View(this.GetQuoteItemsList());
        }
        public ActionResult PendingRequest()
        {
            return View();
        }
        public JsonResult CheckItemCodeAvailable(string itemdata)
        {
            kim_mexEntities db = new kim_mexEntities();
            System.Threading.Thread.Sleep(1000);
            var SearchData = db.tb_product.Where(x => x.product_code == itemdata && x.status == true).FirstOrDefault();
            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult GetItemForEdit()
        {
            kim_mexEntities db = new kim_mexEntities();
            var data = from o in db.tb_quote
                       select o;
            return Json(data.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }
   
        // GET: Quote/Details/5
        public ActionResult Details(string id)
        {
            QuoteViewModel model = new QuoteViewModel();
            model = QuoteViewModel.GetQuoteDetail(id);
            return View(model);
        }
        // GET: Quote/Create
        public ActionResult Create()
        {
            List<tb_supplier> suppliers = new List<tb_supplier>();
            suppliers = this.GetSupplierDropdownList();
            ViewBag.QuoteNumber = this.GenerateQuoteNumber();
            ViewBag.SupplierID = new SelectList(suppliers, "supplier_id", "supplier_name");
            return View();
        }
        // POST: Quote/Create
        [HttpPost]
        public ActionResult Create(string quoteNo,string supplierId,string mrId,List<QuoteItemsViewModel> items,List<string> Attachment,
            string incoterm,string payment,string delivery, string shipment, string warranty, string vendor_ref,decimal grand_total_amount,decimal lump_sum_discount_amount)
        {
            QuoteItemsViewModel obj = new QuoteItemsViewModel();
            try
            {
                using (kim_mexEntities db=new kim_mexEntities())
                {
                    var dupItem = items.GroupBy(x => x.item_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                    if (dupItem.Count() > 0)
                    {
                        string errorMessage = string.Empty;
                        foreach (var dItem in dupItem)
                        {
                            var item = db.tb_product.Find(dItem);
                            errorMessage = string.Format("{0}\n{1} - {2}", errorMessage, item.product_code, item.product_name);
                        }
                        return Json(new { result = "fail", message = "Duplicate item:\n" + errorMessage }, JsonRequestBehavior.AllowGet);
                    }
                    if (items != null)
                    {
                        tb_quote quote = new tb_quote();
                        quote = new tb_quote();
                        quote.quote_id = Guid.NewGuid().ToString();
                        quote.quote_no = quoteNo;
                        quote.supplier_id = supplierId;
                        quote.mr_id = mrId;
                        quote.status = true;
                        quote.created_by = User.Identity.GetUserId();
                        quote.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        quote.updated_by = User.Identity.GetUserId();
                        quote.updated_date= Class.CommonClass.ToLocalTime(DateTime.Now);
                        quote.quote_status =Class.Status.Approved;
                        quote.incoterm = incoterm;
                        quote.payment = payment;
                        quote.delivery = delivery;
                        quote.shipment = shipment;
                        quote.warranty = warranty;
                        quote.vendor_ref = vendor_ref;
                        quote.grand_total_amount = grand_total_amount;
                        quote.lump_sum_discount_amount = lump_sum_discount_amount;
                       
                        db.tb_quote.Add(quote);
                        db.SaveChanges();
                        foreach (QuoteItemsViewModel item in items)
                        {
                            obj = item;
                            tb_quote_detail dQuote = new tb_quote_detail();
                            dQuote.quote_detail_id = Guid.NewGuid().ToString();
                            dQuote.quote_id = quote.quote_id;
                            dQuote.item_id = item.item_id;
                            dQuote.price = item.price;
                            dQuote.discount = item.discount;
                            dQuote.qty = item.qty;
                            //dQuote.discount_amount =decimal.Parse(decimal.Parse(item.discount_amount.ToString()).ToString("0.##################"));
                            dQuote.discount_amount =Convert.ToDecimal(decimal.Parse(item.discount_amount.ToString()).ToString("G29"));
                            dQuote.ordering_number = item.ordering_number;
                            dQuote.supplier_item_name = item.supplier_item_name;
                            db.tb_quote_detail.Add(dQuote);
                            db.SaveChanges();
                        }
                        if (Attachment != null)
                        {
                            if (Attachment.Count() > 0)
                            {
                                foreach (string att in Attachment)
                                {
                                    tb_attachment attachment = db.tb_attachment.Where(m => m.attachment_id == att).FirstOrDefault();
                                    attachment.attachment_ref_id = quote.quote_id;
                                    db.SaveChanges();


                                    if (CommonClass.isFileIsImageByExtenstion(attachment.attachment_extension))
                                    {
                                        var file_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment.attachment_id + attachment.attachment_extension);
                                        Image image = Image.FromFile(file_path);
                                        Graphics imageGraphics = Graphics.FromImage(image);

                                        /* CREATED Section */
                                        string checkedBySignature = CommonClass.GetUserSignature(quote.created_by);
                                        if (!string.IsNullOrEmpty(checkedBySignature))
                                        {
                                            var checked_signature_path = Server.MapPath("~" + checkedBySignature);
                                            string created_by_name = CommonClass.GetUserFullnameByUserId(quote.created_by);
                                            Image watermarkImage = Image.FromFile(checked_signature_path);
                                            TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                            //int y = (image.Height - (watermarkImage.Height + 40));
                                            int y = (image.Height - (watermarkImage.Height + 50));
                                            //int x = watermarkImage.Width/3;
                                            int x = 20;
                                            //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                            watermarkBrush.TranslateTransform(x, y);
                                            imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                                            Brush brush = new SolidBrush(Color.Black);
                                            System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                                            SizeF textSize = new SizeF();
                                            textSize = imageGraphics.MeasureString(created_by_name, font);
                                            int textY = image.Height - 50;
                                            int textX = watermarkImage.Width / 2;
                                            Point position = new Point(textX, textY);
                                            imageGraphics.DrawString(created_by_name, font, brush, position);

                                            textSize = new SizeF();
                                            textSize = imageGraphics.MeasureString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font);
                                            textY = image.Height - 30;
                                            position = new Point(textX, textY);
                                            imageGraphics.DrawString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font, brush, position);

                                        }
                                        var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment.attachment_id + "_watermark" + attachment.attachment_extension);
                                        image.Save(file_path_after_watermark);
                                    }else if (string.Compare(attachment.attachment_extension, ".pdf") == 0)
                                    {
                                        var file_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment.attachment_id + attachment.attachment_extension);
                                        PdfDocument pdfdocument = new PdfDocument(file_path);
                                        for (int i = 0; i < pdfdocument.Pages.Count; i++)
                                        {
                                            tb_attachment newAttachment = new tb_attachment();
                                            System.Drawing.Image pdf_image = pdfdocument.SaveAsImage(i, 96, 96);
                                            var attachment_id = Guid.NewGuid().ToString();
                                            var new_image_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment_id + ".png");
                                            pdf_image.Save(new_image_path, System.Drawing.Imaging.ImageFormat.Png);

                                            newAttachment.attachment_id = attachment_id;
                                            newAttachment.attachment_name = attachment.attachment_name + "_" + (i + 1) + ".png";
                                            newAttachment.attachment_extension = ".png";
                                            newAttachment.attachment_path = new_image_path;
                                            newAttachment.attachment_ref_type = "Quote";
                                            newAttachment.attachment_ref_id = attachment.attachment_id;
                                            db.tb_attachment.Add(newAttachment);
                                            db.SaveChanges();

                                            /* Add Watermark to new photo */
                                            Image image = Image.FromFile(new_image_path);
                                            Graphics imageGraphics = Graphics.FromImage(image);

                                            /* CREATED Section */
                                            string checkedBySignature = CommonClass.GetUserSignature(quote.created_by);
                                            if (!string.IsNullOrEmpty(checkedBySignature))
                                            {
                                                var checked_signature_path = Server.MapPath("~" + checkedBySignature);
                                                string created_by_name = CommonClass.GetUserFullnameByUserId(quote.created_by);
                                                Image watermarkImage = Image.FromFile(checked_signature_path);
                                                TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                                //int y = (image.Height - (watermarkImage.Height + 40));
                                                int y = (image.Height - (watermarkImage.Height + 50));
                                                //int x = watermarkImage.Width/3;
                                                int x = 20;
                                                //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                                watermarkBrush.TranslateTransform(x, y);
                                                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                                                Brush brush = new SolidBrush(Color.Black);
                                                System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                                                SizeF textSize = new SizeF();
                                                textSize = imageGraphics.MeasureString(created_by_name, font);
                                                int textY = image.Height - 50;
                                                int textX = watermarkImage.Width / 2;
                                                Point position = new Point(textX, textY);
                                                imageGraphics.DrawString(created_by_name, font, brush, position);

                                                textSize = new SizeF();
                                                textSize = imageGraphics.MeasureString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font);
                                                textY = image.Height - 30;
                                                position = new Point(textX, textY);
                                                imageGraphics.DrawString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font, brush, position);

                                            }
                                            var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/Attachment/"), newAttachment.attachment_id + "_watermark" + newAttachment.attachment_extension);
                                            image.Save(file_path_after_watermark);
                                        }

                                        //var doc = new Document(file_path);

                                        //for (int page = 0; page < doc.PageCount; page++)
                                        //{
                                        //    /* Extract PDF page as photo */
                                        //    var extractedPage = doc.ExtractPages(page, 1);
                                        //    tb_attachment newAttachment = new tb_attachment();
                                        //    var attachment_id = Guid.NewGuid().ToString();
                                        //    var new_image_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment_id+ ".jpg");
                                        //    extractedPage.Save(new_image_path);
                                        //    newAttachment.attachment_id = attachment_id;
                                        //    newAttachment.attachment_name = attachment.attachment_name+"_"+(page+1)+".jpg";
                                        //    newAttachment.attachment_extension =".jpg" ;
                                        //    newAttachment.attachment_path = new_image_path;
                                        //    newAttachment.attachment_ref_type = "Quote";
                                        //    newAttachment.attachment_ref_id = attachment.attachment_id;
                                        //    db.tb_attachment.Add(newAttachment);
                                        //    db.SaveChanges();

                                        //    /* Add Watermark to new photo */
                                        //    Image image = Image.FromFile(new_image_path);
                                        //    Graphics imageGraphics = Graphics.FromImage(image);

                                        //    /* CREATED Section */
                                        //    string checkedBySignature = CommonClass.GetUserSignature(quote.created_by);
                                        //    if (!string.IsNullOrEmpty(checkedBySignature))
                                        //    {
                                        //        var checked_signature_path = Server.MapPath("~" + checkedBySignature);
                                        //        string created_by_name = CommonClass.GetUserFullnameByUserId(quote.created_by);
                                        //        Image watermarkImage = Image.FromFile(checked_signature_path);
                                        //        TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                        //        //int y = (image.Height - (watermarkImage.Height + 40));
                                        //        int y = (image.Height - (watermarkImage.Height + 50));
                                        //        //int x = watermarkImage.Width/3;
                                        //        int x = 20;
                                        //        //TextureBrush watermarkBrush = new TextureBrush(watermarkImage);
                                        //        watermarkBrush.TranslateTransform(x, y);
                                        //        imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));

                                        //        Brush brush = new SolidBrush(Color.Black);
                                        //        System.Drawing.Font font = new System.Drawing.Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                                        //        SizeF textSize = new SizeF();
                                        //        textSize = imageGraphics.MeasureString(created_by_name, font);
                                        //        int textY = image.Height - 50;
                                        //        int textX = watermarkImage.Width / 2;
                                        //        Point position = new Point(textX, textY);
                                        //        imageGraphics.DrawString(created_by_name, font, brush, position);

                                        //        textSize = new SizeF();
                                        //        textSize = imageGraphics.MeasureString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font);
                                        //        textY = image.Height - 30;
                                        //        position = new Point(textX, textY);
                                        //        imageGraphics.DrawString(CommonClass.ToLocalTime(Convert.ToDateTime(quote.created_date)).ToString("dd-MMM-yyyy"), font, brush, position);

                                        //    }
                                        //    var file_path_after_watermark = Path.Combine(Server.MapPath("~/Documents/Attachment/"), newAttachment.attachment_id + "_watermark" + newAttachment.attachment_extension);
                                        //    image.Save(file_path_after_watermark);

                                        //}
                                    
                                    }
                                }
                            }
                        }
                        //update status in MR
                        string materialRequestId = (from mr in db.tb_item_request
                                                    join pr in db.tb_purchase_requisition
                                                    on mr.ir_id equals pr.material_request_id
                                                    where string.Compare(pr.purchase_requisition_id, quote.mr_id) == 0
                                                    select mr.ir_id).FirstOrDefault().ToString();
                        //tb_item_request materialRequest = db.tb_item_request.Find(materialRequestId);
                        //materialRequest.po_status = ShowStatus.SupplierQuoteCreated;
                        //materialRequest.st_status = ShowStatus.SupplierQuoteCreated;
                        //db.SaveChanges();
                        CommonClass.UpdateMaterialRequestStatus(materialRequestId, ShowStatus.SupplierQuoteCreated, User.Identity.GetUserId());
                        StockTransfer.UpdateStockTransferProcessStatus(materialRequestId, ShowStatus.SupplierQuoteCreated);
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message,errorObj=obj },JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Quote/Edit/5
        public ActionResult Edit(string id)
        {
            QuoteViewModel model = new QuoteViewModel();
            model = QuoteViewModel.GetQuoteDetail(id);
            List<tb_supplier> suppliers = new List<tb_supplier>();
            suppliers = this.GetSupplierDropdownList();
            ViewBag.SupplierID = suppliers;
            return View(model);
        }

        // POST: Quote/Edit/5
        [HttpPost]
        public ActionResult Edit(string quoteId, string supplierId, List<QuoteItemsViewModel> items, List<string> Attachment,
             string incoterm, string payment, string delivery, string shipment, string warranty, string vendor_ref,decimal grand_total_amount, decimal lump_sum_discount_amount)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var dupItem = items.GroupBy(x => x.item_id).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
                if (dupItem.Count() > 0)
                {
                    string errorMessage = string.Empty;
                    foreach(var dItem in dupItem)
                    {
                        var item = db.tb_product.Find(dItem);
                        errorMessage = string.Format("{0}\n{1} - {2}", errorMessage, item.product_code, item.product_name);
                    }
                    return Json(new { result = "fail", message = "Duplicate item:\n"+errorMessage }, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(quoteId)){
                    this.DeleteQuoteItemDetail(quoteId);
                }
                tb_quote quote = db.tb_quote.Find(quoteId);
                //quote.supplier_id = supplierId;
                //quote.status = true;
                quote.updated_by = User.Identity.GetUserId();
                quote.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                quote.quote_status = Status.Edit;
                quote.incoterm = incoterm;
                quote.payment = payment;
                quote.delivery = delivery;
                quote.shipment = shipment;
                quote.warranty = warranty;
                quote.vendor_ref = vendor_ref;
                quote.grand_total_amount = grand_total_amount;
                quote.lump_sum_discount_amount = lump_sum_discount_amount;

                db.SaveChanges();
                foreach (QuoteItemsViewModel item in items)
                {
                    tb_quote_detail dQuote = new tb_quote_detail();
                    dQuote.quote_detail_id = Guid.NewGuid().ToString();
                    dQuote.quote_id = quote.quote_id;
                    dQuote.item_id = item.item_id;
                    dQuote.price = item.price;
                    dQuote.discount = item.discount;
                    dQuote.qty = item.qty;
                    dQuote.discount_amount = Convert.ToDecimal(decimal.Parse(item.discount_amount.ToString()).ToString("G29"));
                    dQuote.supplier_item_name = item.supplier_item_name;
                    db.tb_quote_detail.Add(dQuote);
                    db.SaveChanges();
                }
                if (Attachment!=null)
                {
                    foreach (string att in Attachment)
                    {
                        tb_attachment attachment = db.tb_attachment.Where(m => m.attachment_id == att).FirstOrDefault();
                        attachment.attachment_ref_id = quote.quote_id;
                        db.SaveChanges();
                    }
                }
                //update status in MR
                string materialRequestId = (from mr in db.tb_item_request
                                            join pr in db.tb_purchase_requisition
                                            on mr.ir_id equals pr.material_request_id
                                            where string.Compare(pr.purchase_requisition_id, quote.mr_id) == 0
                                            select mr.ir_id).FirstOrDefault().ToString();
                //tb_item_request materialRequest = db.tb_item_request.Find(materialRequestId);
                //materialRequest.po_status = ShowStatus.SupplierQuoteEdited;
                //materialRequest.st_status = ShowStatus.SupplierQuoteEdited;
                //db.SaveChanges();
                CommonClass.UpdateMaterialRequestStatus(materialRequestId, ShowStatus.SupplierQuoteCreated, User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(materialRequestId, ShowStatus.SupplierQuoteEdited);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Quote/Delete/5
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_quote quote = db.tb_quote.Find(id);
                quote.status = false;
                quote.updated_by = User.Identity.Name;
                quote.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Cancel(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_quote quote = db.tb_quote.Find(id);
                quote.quote_status = Status.cancelled;
                quote.updated_by = User.Identity.GetUserId();
                quote.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                //update status in MR
                string materialRequestId = (from mr in db.tb_item_request
                                            join pr in db.tb_purchase_requisition
                                            on mr.ir_id equals pr.material_request_id
                                            where string.Compare(pr.purchase_requisition_id, quote.mr_id) == 0
                                            select mr.ir_id).FirstOrDefault().ToString();
                tb_item_request materialRequest = db.tb_item_request.Find(materialRequestId);
                materialRequest.po_status = ShowStatus.SupplierQuoteCancelled;
                materialRequest.st_status = ShowStatus.SupplierQuoteCancelled;
                db.SaveChanges();
                StockTransfer.UpdateStockTransferProcessStatus(materialRequestId, ShowStatus.SupplierQuoteCancelled);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error",message=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
       

        [HttpPost]
        public ActionResult Print(string id)
        {
            return Json(new { result = "success", id = id}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult QuoteReport()
        {
            return View();
        }
        public ActionResult GetQuoteItemsDataTable()
        {
            List<QuoteViewModel> quotes = new List<QuoteViewModel>();
            quotes = this.GetQuoteItemsList();
            return Json(new { data = quotes }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetItemsQuoteDataTable(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<QuoteItemsViewModel> quoteItems = new List<QuoteItemsViewModel>();
                string lastestQuoteId = db.tb_quote.OrderByDescending(x => x.created_date).Where(x => string.Compare(x.supplier_id, id) == 0).Select(x => x.quote_id).FirstOrDefault();
                if (!string.IsNullOrEmpty(lastestQuoteId))
                {
                    quoteItems = db.tb_quote_detail
                    .Join(db.tb_product, qi => qi.item_id, it => it.product_id, (qi, it) => new { qi, it })
                    .OrderBy(x => x.it.product_code)
                    .Where(x => x.qi.quote_id == lastestQuoteId)
                    .Select(x => new QuoteItemsViewModel() { quote_detail_id = x.qi.quote_detail_id, item_id = x.qi.item_id, product_code = x.it.product_code, product_name = x.it.product_name, product_unit = x.it.product_unit, price = x.qi.price, discount = x.qi.discount, remark = x.qi.remark }).ToList();
                }
                return Json(new { data = quoteItems }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSupplierQuoteDataTableByDateRangeAndStatusAJAX(string dateRange,string status)
        {
            return Json(new { data = GetQuoteItemsListbyDateRangeandStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }

        public List<QuoteViewModel> GetQuoteItemsList()
        {
            List<QuoteViewModel> items = new List<QuoteViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                //items = db.tb_quote
                //    .Join(db.tb_supplier,q=>q.supplier_id,s=>s.supplier_id,(q,s)=>new { q,s})
                //    .Where(x => x.q.status == true).Select(x => new QuoteViewModel() { quote_id=x.q.quote_id,quote_no=x.q.quote_no,supplier_id=x.q.supplier_id,created_date=x.q.created_date,supplier_name=x.s.supplier_name,quote_status=x.q.quote_status}).ToList();

                items = (from quote in db.tb_quote
                         join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                         join pr in db.tb_purchase_requisition on quote.mr_id equals pr.purchase_requisition_id
                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                         join project in db.tb_project on mr.ir_project_id equals project.project_id
                         orderby quote.created_date descending
                         where quote.status == true
                         select new QuoteViewModel()
                         {
                             quote_id = quote.quote_id,
                             quote_no =quote.quote_no,
                             supplier_id = quote.supplier_id,
                             created_date = quote.created_date,
                             supplier_name = supplier.supplier_name,
                             quote_status = quote.quote_status,
                             mr_id=quote.mr_id,
                             mr_number=pr.purchase_requisition_number,
                             project_id=project.project_id,
                             project_fullname=project.project_full_name,
                             is_quote_complete=pr.is_quote_complete,
                             grand_total_amount=quote.grand_total_amount,
                         }).ToList();
            }
            return items;
        }
        public List<QuoteViewModel> GetQuoteItemsListbyDateRangeandStatus(string dateRange,string status)
        {
            List<QuoteViewModel> items = new List<QuoteViewModel>();
            List<QuoteFilterResultModel> results = new List<QuoteFilterResultModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
                if (string.Compare(status, "0") == 0)
                {
                    results= (from quote in db.tb_quote
                              join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                              join pr in db.tb_purchase_requisition on quote.mr_id equals pr.purchase_requisition_id
                              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                              join project in db.tb_project on mr.ir_project_id equals project.project_id
                              orderby quote.created_date descending
                              where quote.status == true && quote.created_date>=startDate && quote.created_date<=endDate
                              select new QuoteFilterResultModel()
                              {
                                  quote=quote,
                                  supplier=supplier,
                                  pr=pr,
                                  mr=mr,
                                  project=project
                              }).ToList();
                }
                else
                {
                    results = (from quote in db.tb_quote
                               join supplier in db.tb_supplier on quote.supplier_id equals supplier.supplier_id
                               join pr in db.tb_purchase_requisition on quote.mr_id equals pr.purchase_requisition_id
                               join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                               join project in db.tb_project on mr.ir_project_id equals project.project_id
                               orderby quote.created_date descending
                               where quote.status == true && quote.created_date >= startDate && quote.created_date <= endDate && string.Compare(quote.quote_status,status)==0
                               select new QuoteFilterResultModel()
                               {
                                   quote = quote,
                                   supplier = supplier,
                                   pr = pr,
                                   mr = mr,
                                   project = project
                               }).ToList();
                }
                foreach(var obj in results.Select((value, i) => new { i, value }))
                {
                    var rs = obj.value;
                    string created_at_text =CommonClass.ToLocalTime(Convert.ToDateTime(rs.quote.created_date)).ToString("dd/MM/yyyy HH:mm");
                    string grand_total_amount_text = string.Format("{0:N4}", decimal.Parse(rs.quote.grand_total_amount.ToString()));
                    string show_status = ShowStatus.GetSupplierQuoteShowStatus(rs.quote.quote_status);
                    items.Add(new QuoteViewModel()
                    {
                        number = obj.i + 1,
                        quote_id = rs.quote.quote_id,
                        quote_no = rs.quote.quote_no,
                        supplier_id = rs.quote.supplier_id,
                        created_date = rs.quote.created_date,
                        supplier_name = rs.supplier.supplier_name,
                        quote_status = rs.quote.quote_status,
                        mr_id = rs.mr.ir_id,
                        mr_number = rs.mr.ir_no,
                        project_id = rs.project.project_id,
                        project_fullname = rs.project.project_full_name,
                        is_quote_complete = rs.pr.is_quote_complete,
                        grand_total_amount = rs.quote.grand_total_amount,
                        created_at_text=created_at_text,
                        grand_total_amount_text=grand_total_amount_text,
                        show_quote_status=show_status,
                        pr_id=rs.quote.mr_id,
                        pr_no=rs.pr.purchase_requisition_number,
                    });
                }
                
            }
            return items;
        }
        public JsonResult UploadAttachment()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_attachment attachment = new tb_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    attachment.attachment_id = file_id;
                    attachment.attachment_name = file_name;
                    attachment.attachment_extension = file_extension;
                    attachment.attachment_path = file_path;
                    attachment.attachment_ref_type = "Quote";
                    db.tb_attachment.Add(attachment);
                    db.SaveChanges();

                    

                }
                return Json(new { result = "success", attachment_id = attachment.attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UploadAttachmentbyItem()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_item_quote_attachment attachment = new tb_item_quote_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/Quote/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    attachment.quote_attachment_id = file_id;
                    attachment.quote_attachment_name = file_name;
                    attachment.quote_attachment_extension = file_extension;
                    attachment.quote_attachment_part = file_path;
                    db.tb_item_quote_attachment.Add(attachment);
                    db.SaveChanges();
                }
                return Json(new { result = "success", attachment_id = attachment.quote_attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult Download(String p,String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        public FileResult DownloadQuoteAttachment(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Quote/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        [HttpPost]
        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using (kim_mexEntities db=new kim_mexEntities())
            {
                tb_attachment attachment = db.tb_attachment.Find(id);
                if (attachment == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_attachment.Remove(attachment);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), attachment.attachment_id + attachment.attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        [HttpPost]
        public JsonResult DeleteItemQuoteAttachment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "errror" });
            }
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_quote_attachment attachment = db.tb_item_quote_attachment.Find(id);
                if (attachment == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_item_quote_attachment.Remove(attachment);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Quote/"), attachment.quote_attachment_id + attachment.quote_attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public ActionResult ItemMaster()
        {
            return View();
        }
        public ActionResult ItemPirceDetail(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("ItemMaster");
            kim_mexEntities db = new kim_mexEntities();
            ItemQuoteSupplierViewModel model = new ItemQuoteSupplierViewModel();
            ViewBag.SupplierQuotes = Class.CommonClass.GetItemPricebySupplier(id);
            model = db.tb_product.Where(w => string.Compare(w.product_id, id) == 0).Select(s => new ItemQuoteSupplierViewModel()
            {
                ItemId=s.product_id,
                ItemCode=s.product_code,
                ItemName=s.product_name,
                ItemUnit=s.product_unit,
                ItemPrice=s.unit_price,
                ItemTypeId=s.brand_id,
               // ItemTypeName=s.tb_brand.brand_name,
            }).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult ItemPirceDetail(ItemQuoteSupplierViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var dupSuppliers = model.Quotes.GroupBy(g => g.SupplierID).Where(w => w.Count() > 1).Select(s => s.Key).ToList();
                if (dupSuppliers.Any())
                {
                    string errorMessage = string.Empty;
                    foreach (var dSup in dupSuppliers)
                    {
                        var supplier = db.tb_supplier.Find(dSup);
                        errorMessage = string.Format("{0}\n{1} - {2}", errorMessage, supplier.supplier_name);
                    }
                    return Json(new { result = "fail", message = "Duplicate item:\n" + errorMessage }, JsonRequestBehavior.AllowGet);
                }
                foreach (var quote in model.Quotes)
                {
                    string quoteDetailId = string.Empty;
                    if (string.IsNullOrEmpty(quote.QuoteID))
                    {
                        tb_quote q = new tb_quote();
                        q.quote_id = Guid.NewGuid().ToString();
                        q.quote_no = Class.CommonClass.GenerateQuoteNumber();
                        q.supplier_id = quote.SupplierID;
                        q.is_selected = true;
                        q.status = true;
                        q.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        q.created_by = User.Identity.Name;
                        q.updated_by = User.Identity.Name;
                        q.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        db.tb_quote.Add(q);
                        db.SaveChanges();

                        tb_quote_detail dQuote = new tb_quote_detail();
                        quoteDetailId = Guid.NewGuid().ToString();
                        dQuote.quote_detail_id = quoteDetailId;
                        dQuote.quote_id = q.quote_id;
                        dQuote.item_id = model.ItemId;
                        dQuote.price = quote.Price;
                        dQuote.discount = 0;
                        dQuote.remark = string.Empty;
                        db.tb_quote_detail.Add(dQuote);
                        db.SaveChanges();
                    }
                    else
                    {
                        tb_quote_detail quoteDetail = db.tb_quote_detail.Where(w => string.Compare(w.quote_id, quote.QuoteID) == 0 && string.Compare(w.item_id, model.ItemId) == 0).FirstOrDefault();
                        if (quoteDetail == null)
                        {
                            tb_quote_detail dQuote = new tb_quote_detail();
                            dQuote.quote_detail_id = Guid.NewGuid().ToString();
                            dQuote.quote_id = quote.QuoteID;
                            dQuote.item_id = model.ItemId;
                            dQuote.price = quote.Price;
                            dQuote.discount = 0;
                            dQuote.remark = string.Empty;
                            db.tb_quote_detail.Add(dQuote);
                            db.SaveChanges();
                            quoteDetailId = dQuote.quote_detail_id;
                        }
                        else
                        {
                            quoteDetail.price = quote.Price;
                            db.SaveChanges();
                            quoteDetailId = quoteDetail.quote_detail_id;
                        }
                    }
                    if (quote.Attachments!=null)
                    {
                        foreach(string aid in quote.Attachments)
                        {
                            tb_item_quote_attachment att = db.tb_item_quote_attachment.Find(aid);
                            att.quote_attachment_ref_id = quoteDetailId;
                            db.SaveChanges();
                        }
                    }
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GenerateQuoteNumber()
        {
            string quoteNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", quoteNum;
                string number = (from tbl in db.tb_quote orderby tbl.created_date descending select tbl.quote_no).FirstOrDefault();
                if (number == null)
                    last_no = "001";
                else
                {
                    //number = number.Substring(number.Length - 3, 3);
                    string[] splitNumber = number.Split('-');
                    number = splitNumber[splitNumber.Length - 1];
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1) last_no = "00" + num;
                    else if (num.ToString().Length == 2) last_no = "0" + num;
                    //else if (num.ToString().Length == 3) last_no = num.ToString();
                    else
                        last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                quoteNo = "QU-" + yy + "-" + mm + "-" + last_no;
            }
            return quoteNo;
        }
        public List<tb_supplier> GetSupplierDropdownList()
        {
            List<tb_supplier> suppliers = new List<tb_supplier>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                suppliers = db.tb_supplier.OrderBy(m => m.supplier_name).Where(m => m.status == true).ToList();
            }
            return suppliers;
        }
        
        public void DeleteQuoteItemDetail(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var items = db.tb_quote_detail.Where(x => x.quote_id == id).ToList();
                foreach(var item in items)
                {
                    var itemId = item.quote_detail_id;
                    tb_quote_detail qd = db.tb_quote_detail.Find(itemId);
                    db.tb_quote_detail.Remove(qd);
                    db.SaveChanges();
                }
            }
        }
        public ActionResult GetAllQuoteItem(string id)
        {
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    List<QuoteItemsViewModel> quoteItems = new List<QuoteItemsViewModel>();
                    string lastestQuoteId = db.tb_quote.OrderByDescending(x => x.created_date).Where(x => string.Compare(x.supplier_id, id) == 0).Select(x => x.quote_id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lastestQuoteId))
                    {
                        //quoteItems = db.tb_quote_detail
                        //.Join(db.tb_product, qi => qi.item_id, it => it.product_id, (qi, it) => new { qi, it })
                        //.OrderBy(x => x.it.product_code)
                        //.Where(x => x.qi.quote_id == lastestQuoteId)
                        //.Select(x => new QuoteItemsViewModel() { quote_detail_id = x.qi.quote_detail_id, item_id = x.qi.item_id, product_code = x.it.product_code, product_name = x.it.product_name, product_unit = x.it.product_unit, price = x.qi.price, discount = x.qi.discount, remark = x.qi.remark }).ToList();
                        quoteItems = (from qi in db.tb_quote_detail
                                      join it in db.tb_product on qi.item_id equals it.product_id
                                      join u in db.tb_unit on it.product_unit equals u.Id
                                      orderby it.product_name
                                      where qi.quote_id == lastestQuoteId
                                      select new QuoteItemsViewModel()
                                      {
                                          quote_detail_id = qi.quote_detail_id,
                                          item_id = qi.item_id,
                                          product_code = it.product_code,
                                          product_name = it.product_name,
                                          product_unit = it.product_unit,
                                          price = qi.price,
                                          discount = qi.discount,
                                          remark = qi.remark,
                                          unit_name = u.Name,
                                      }).ToList();
                    }
                    return Json(new { result = "success", data = quoteItems }, JsonRequestBehavior.AllowGet);
                }
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetItemsPriceList()
        {
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    List<ProductViewModel> items = new List<ProductViewModel>();
                    List<QuoteViewModel> quotes = new List<QuoteViewModel>();
                    bool check = false;
                    int maxSupplier = 0;
                    var iitems = db.tb_product.OrderBy(p => p.product_code).Where(p => p.status == true).Select(p => new ProductViewModel()
                    {
                        product_id = p.product_id
                    }).ToList();
                    if (iitems.Any())
                    {
                        foreach (ProductViewModel item in iitems)
                        {
                            quotes = new List<QuoteViewModel>();
                            var itemQuotes = (from qd in db.tb_quote_detail
                                              join q in db.tb_quote on qd.quote_id equals q.quote_id
                                              join sup in db.tb_supplier on q.supplier_id equals sup.supplier_id
                                              orderby q.created_date descending
                                              where string.Compare(qd.item_id, item.product_id) == 0 && q.status == true
                                              select new QuoteViewModel()
                                              {
                                                  supplier_id = q.supplier_id,
                                                  supplier_name = sup.supplier_name,
                                                  item_id = qd.item_id,
                                                  price = qd.price,
                                                  created_date = q.created_date
                                              }).ToList();
                            var duplicateQuoteSuppliers = itemQuotes.GroupBy(g => g.supplier_id).Where(w => w.Count() > 1).Select(s => s.Key).ToList();
                            foreach (var dup in duplicateQuoteSuppliers)
                            {
                                var quote = itemQuotes.OrderByDescending(x => x.created_date).Where(w => string.Compare(w.supplier_id, dup) == 0).FirstOrDefault();
                                quotes.Add(new QuoteViewModel()
                                {
                                    supplier_id = quote.supplier_id,
                                    supplier_name = quote.supplier_name,
                                    item_id = quote.item_id,
                                    price = quote.price
                                });
                            }
                            foreach (var itemQuote in itemQuotes)
                            {
                                bool isDuplicationSupplier = duplicateQuoteSuppliers.Where(w => string.Compare(w, itemQuote.supplier_id) == 0).Count() > 0 ? true : false;
                                if (!isDuplicationSupplier)
                                {
                                    quotes.Add(new QuoteViewModel()
                                    {
                                        supplier_id = itemQuote.supplier_id,
                                        supplier_name = itemQuote.supplier_name,
                                        item_id = itemQuote.item_id,
                                        price = itemQuote.price
                                    });
                                }
                            }
                            if (maxSupplier < quotes.Count())
                                maxSupplier = quotes.Count();
                            var itemDetail = db.tb_product.Where(w => string.Compare(w.product_id, item.product_id) == 0).FirstOrDefault();
                            items.Add(new ProductViewModel()
                            {
                                product_id = itemDetail.product_id,
                                product_code = itemDetail.product_code,
                                product_name = itemDetail.product_name,
                                product_unit = itemDetail.product_unit,
                                quoteSuppliers = quotes
                            });
                        }
                    }
                    //return Json(new { data = items, maxSupplier = maxSupplier }, JsonRequestBehavior.AllowGet);
                    var jsonResult= Json(new { data = items, maxSupplier = maxSupplier }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMyRequestListItemsJson()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<QuoteViewModel> quotes = new List<QuoteViewModel>();
                string userId = User.Identity.GetUserId().ToString();
                var models = db.tb_quote.OrderByDescending(s => s.created_date).Where(s => s.status == true && string.Compare(s.created_by, userId) == 0).ToList();
                foreach(var model in models)
                {
                    QuoteViewModel quote = new QuoteViewModel();
                    quote = (from q in db.tb_quote
                             join s in db.tb_supplier on q.supplier_id equals s.supplier_id
                             where string.Compare(q.quote_id, model.quote_id) == 0
                             select new QuoteViewModel()
                             {
                                 quote_id = q.quote_id,
                                 quote_no = q.quote_no,
                                 supplier_id = q.supplier_id,
                                 created_date = q.updated_date,
                                 supplier_name = s.supplier_name,
                                 quote_status=q.quote_status,
                             }).FirstOrDefault();
                    quote.amount =Convert.ToDecimal(db.tb_quote_detail.Where(s => string.Compare(s.quote_id, model.quote_id) == 0).Sum(s => s.price));
                    quotes.Add(quote);
                }
                return Json(new { data = quotes }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approval(string id,string status,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_quote quote = db.tb_quote.Find(id);
                if (string.Compare(status, Class.Status.Approved) == 0)
                {
                    quote.quote_status = Class.Status.Completed;
                    quote.approved_by = User.Identity.GetUserId();
                    quote.approved_date = DateTime.Now;
                    db.SaveChanges();
                }
                else if (string.Compare(status, Class.Status.Rejected) == 0)
                {
                    quote.quote_status = Class.Status.Rejected;
                    quote.approved_comment = comment;
                    quote.approved_by = User.Identity.GetUserId();
                    quote.approved_date = DateTime.Now;
                    db.SaveChanges();
                }
                else if (string.Compare(status, Class.Status.CheckRequest) == 0)
                {
                    quote.quote_status = Class.Status.CheckRequest;
                    quote.approved_by = User.Identity.GetUserId();
                    quote.approved_date = DateTime.Now;
                    db.SaveChanges();
                } else if (string.Compare(status, Class.Status.Checked) == 0)
                {
                    quote.quote_status = Class.Status.Completed;
                    quote.checked_by = User.Identity.GetUserId();
                    quote.checked_date = DateTime.Now;
                    db.SaveChanges();
                }else if (string.Compare(status, Class.Status.CheckRejected) == 0)
                {
                    quote.quote_status = Class.Status.CheckRejected;
                    quote.checked_by = User.Identity.GetUserId();
                    quote.checked_date = DateTime.Now;
                    quote.checked_comment = comment;
                    db.SaveChanges();
                }
                
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}
