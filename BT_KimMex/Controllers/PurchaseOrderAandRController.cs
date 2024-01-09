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
using BT_KimMex.Views;
using Microsoft.Ajax.Utilities;

namespace BT_KimMex.Controllers
{
    public class PurchaseOrderAandRController : Controller
    {
        private DateTime minDate;
        private string rej;
        private string purchase_number;
        private string po;

        // GET: PurchaseOrderAandR
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        public ActionResult Index()
        {

            //return View(this.GetPurchaseOrderListItems());
            return View();
        }
        public ActionResult MyRequest()
        {
            return View();
        }
        public ActionResult MyApproval()
        {
            return View();
        }
        // GET: PurchaseOrderAandR/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            return View(PurchaseRequestViewModel.GetPurchaseOrderItem(id));
        }

        public Purchase_OrderAandR GetPurchseOrderAandRViewModel(int id)
        {
            using (var db = new kim_mexEntities())
            {
                var pos = db.Purchase_OrderAandR.FirstOrDefault(x => x.Purchase_order_id == id);
                return pos;
            }
        }

        // GET: PurchaseOrderAandR/Create

        public ActionResult Create ()
        {
            var supplierName = string.Empty;
            var ponumber = this.GetponumberDropdownList();
            ViewBag.ApprovealNumber = this.GenerateApprovalNumber();
            ViewBag.PONumberID = new SelectList(ponumber, "purchase_order_id", "purchase_oder_number");
            return View();
        }
        [HttpPost]
        public ActionResult Create(string po_number,List<string> detail_id,List<string> amount,List<string> amount_before_vat,List<string> vat_amount)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                int count = 0;
                int countOverAmount = 0;
                tb_purchase_request po = new tb_purchase_request();
                po.pruchase_request_id = Guid.NewGuid().ToString();
                po.purchase_request_number = this.GenerateApprovalNumber();
                po.purchase_order_id = po_number;
                po.purchase_request_status = Status.Pending;
                po.created_by = User.Identity.GetUserId().ToString();
                po.created_date =CommonClass.ToLocalTime(DateTime.Now);
                po.updated_by = User.Identity.GetUserId().ToString();
                po.updated_date =CommonClass.ToLocalTime(DateTime.Now);
                po.status = true;
                db.tb_purchase_request.Add(po);
                db.SaveChanges();
                foreach (var item in detail_id)
                {
                    tb_purchase_request_detail pod = new tb_purchase_request_detail();
                    pod.pr_detail_id = Guid.NewGuid().ToString();
                    pod.purchase_request_id = po.pruchase_request_id;
                    pod.po_report_id = item;
                    pod.amount = Convert.ToDecimal(amount[count]);
                    pod.amount_before_vat = Convert.ToDecimal(amount_before_vat[count]);
                    pod.vat_amount = Convert.ToDecimal(vat_amount[count]);
                    pod.status = Status.Pending;
                    pod.outstanding_amount= Convert.ToDecimal(amount[count]);

                    db.tb_purchase_request_detail.Add(pod);
                    db.SaveChanges();
                    if (pod.amount > 3000)
                        countOverAmount++;
                    count++;

                    //Update PO Report status
                    //tb_po_report po_Report = db.tb_po_report.Find(pod.po_report_id);
                    //if (po_Report != null)
                    //{
                    //    po_Report.po_report_status = Status.Pending;
                    //    db.SaveChanges();
                    //}

                }
                if (countOverAmount > 0)
                {
                    tb_purchase_request pr = db.tb_purchase_request.Find(po.pruchase_request_id);
                    pr.is_check = true;
                    db.SaveChanges();
                }
                tb_purchase_order poo = db.tb_purchase_order.Find(po_number);
                poo.is_po_checked = true;
                db.SaveChanges();

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.POCreated,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.POCreated);

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.created_by, po.created_date);
            }
            return RedirectToAction("MyRequest");
        }

        //[HttpPost]
        //public ActionResult CreateNew(string purchase_order_number, string purchase_number,List<string> approves, List<string> rejects)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(purchase_order_number))
        //        {
        //            using (var db = new kim_mexEntities())
        //            {   
        //                var minDate = new DateTime(1900, 1, 1);
        //                var po = db.tb_purchase_order.FirstOrDefault(x => x.purchase_order_id == purchase_order_number);

        //                if (approves != null)
        //                {
        //                    foreach (var appr in approves)
        //                    {
        //                        var poApprove = new Purchase_OrderAandR()
        //                        {
        //                            Purchar_Number = purchase_number,
        //                            //purchase_oder_number = po?.purchase_oder_number ?? string.Empty,
        //                            purchase_oder_number= purchase_order_number,
        //                            Create_date = DateTime.Now,
        //                            status ="Pending",
        //                            Action ="true",
        //                            //status = "Approved",
        //                            //Action = "Approved",
        //                            supplier_id = appr,
        //                            checked_by = User.Identity.GetUserId(),
        //                            checked_date = DateTime.Now,
        //                            Update_by = string.Empty,
        //                            Update_Date = minDate,
        //                            Upprove_by = User.Identity.GetUserId(),
        //                            Uprove_date = DateTime.Now,
        //                            rejected_date = minDate,
        //                            Item_request_id = "",
        //                            active = true,
        //                        };
        //                        db.Purchase_OrderAandR.Add(poApprove);
        //                    }
        //                }
        //                if (rejects != null)
        //                {
        //                    foreach (var rej in rejects)
        //                    {
        //                        var poReject = new Purchase_OrderAandR()
        //                        {
        //                            Purchar_Number = purchase_number,
        //                            purchase_oder_number = po?.purchase_oder_number ?? string.Empty,
        //                            Create_date = DateTime.Now,
        //                            status = "Rejected",
        //                            Action = "True",
        //                            supplier_id = rej,
        //                            checked_by = User.Identity.GetUserId(),
        //                            checked_date = DateTime.Now,
        //                            Update_by = string.Empty,
        //                            Update_Date = minDate,
        //                            Upprove_by = "",
        //                            Uprove_date = minDate,
        //                            //rejected_by = User.Identity.GetUserId(),
        //                            rejected_date = DateTime.Now,
        //                            Item_request_id = "",
        //                            active = true,
        //                        };
        //                        db.Purchase_OrderAandR.Add(poReject);
        //                    }
        //                }
        //                db.SaveChanges();
        //            }
        //        }

        //        return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        private object GetSuppliersList()
        {
            List<tb_Purchase_OrderAandR> purchaseoder = new List<tb_Purchase_OrderAandR>();
            throw new NotImplementedException();
        }

        public List<tb_purchase_order> GetponumberDropdownList()
        {
            List<tb_purchase_order> purchaseorder = new List<tb_purchase_order>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                purchaseorder = db.tb_purchase_order.OrderByDescending(m => m.purchase_oder_number).Where(m => string.Compare(m.purchase_order_status,Status.Completed)==0 && m.status == true && m.is_po_checked==false).ToList();
            }
            return purchaseorder;
        }

        // GET: PurchaseOrderAandR/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PurchaseOrderAandR/Edit/5
        [HttpPost]
        public ActionResult Edit(string purchase_order_number, string purchase_number,
            List<string> approves, List<string> rejects)
        {
            try
            {



                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult Approval(string id)
        {
            return View(PurchaseRequestViewModel.GetPurchaseOrderItem(id));
        }
        [HttpPost]
        public ActionResult Approval(string id,List<PurchaseRequestDetailViewModel> models)
        {
            try
            {
                int countItemApproved = 0;
                int countItemVoid = 0;
                kim_mexEntities db = new kim_mexEntities();
                List<string> productIds = new List<string>();

                foreach(var item in models)
                {
                    string iid = item.pr_detail_id;
                    tb_purchase_request_detail dpo = db.tb_purchase_request_detail.Find(iid);
                    dpo.status = item.status;
                    db.SaveChanges();
                    if (string.Compare(item.status, Status.Approved) == 0 )
                        countItemApproved++;
                    if (string.Compare(item.status, Status.Void) == 0)
                        countItemVoid++;

                    //Get PO product id
                    var poProducts = (from por in db.tb_po_report
                                      join pod in db.tb_purchase_order_detail on por.po_ref_id equals pod.purchase_order_id
                                      join pos in db.tb_po_supplier on pod.po_detail_id equals pos.po_detail_id
                                      where string.Compare(por.po_report_id, dpo.po_report_id) == 0 && string.Compare(pos.supplier_id, por.po_supplier_id) == 0 && pos.is_selected == true
                                      select pod).ToList();
                    foreach(var pro in poProducts)
                    {
                        productIds.Add(pro.item_id);
                    }
                }

                tb_purchase_request po = db.tb_purchase_request.Find(id);
                ////po.purchase_request_status = countItemApproved == models.Count() ?po.is_check==true? Status.Approved:Status.Completed : Status.Rejected;
                po.purchase_request_status = countItemVoid == models.Count()? Status.Rejected : po.is_check == true ? Status.Approved : Status.Completed;
                po.approved_by = User.Identity.GetUserId().ToString();
                po.approved_date =CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                var poo = db.tb_purchase_order.Where(s => string.Compare(s.purchase_order_id, po.purchase_order_id) == 0).FirstOrDefault();

                //Update Materail Request Item relate with PO
                string mrId = (from pr in db.tb_purchase_requisition
                               where string.Compare(pr.purchase_requisition_id, poo.item_request_id) == 0
                               select pr.material_request_id).FirstOrDefault();
                var ir_detail1 = db.tb_ir_detail1.Where(s => string.Compare(s.ir_id, mrId) == 0).FirstOrDefault();
                foreach(var proId in productIds)
                {
                    var mrDetail = db.tb_ir_detail2.Where(s => string.Compare(s.ir_detail1_id, ir_detail1.ir_detail1_id) == 0 && string.Compare(s.ir_item_id, proId) == 0).FirstOrDefault();
                    if (mrDetail != null)
                    {
                        mrDetail.is_po = true;
                        db.SaveChanges();
                    }
                }
                var MRItemNotYetPO = db.tb_ir_detail2.Where(s => string.Compare(s.ir_detail1_id, ir_detail1.ir_detail1_id) == 0 && s.is_po == false).Count();
                tb_item_request mr = db.tb_item_request.Find(mrId);
                mr.is_po_completed = MRItemNotYetPO == 0 ? true : false;
                mr.is_po_partial = MRItemNotYetPO == 0 ? false : true;
                db.SaveChanges();

                //Generate PO Status
                string poStatus = string.Empty;         
                if (string.Compare(po.purchase_request_status, Status.Rejected) == 0)
                {
                    poo.is_po_checked = false;
                    db.SaveChanges();
                    poStatus = ShowStatus.PORejected;
                }
                else
                {
                    if (Convert.ToBoolean(po.is_check))
                    {
                        poStatus =Convert.ToBoolean(mr.is_po_partial)?ShowStatus.POPartialApproved: ShowStatus.POApproved;
                        ////CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.POApproved);
                        ////StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.POApproved);
                    }
                    else
                    {
                        poStatus = ShowStatus.GRNPending;
                        ////CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.GRNPending);
                        ////StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.GRNPending);
                    }
                }

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), poStatus,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), poStatus);
                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.approved_by, po.approved_date);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckApproval(string id,string status,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_purchase_request po = db.tb_purchase_request.Find(id);
                if (string.Compare(status, Status.Approved) == 0)
                {
                    po.purchase_request_status = Status.Completed;
                    po.checked_by = User.Identity.GetUserId();
                    po.checked_date =CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();

                    var poo = db.tb_purchase_order.Where(s => string.Compare(s.purchase_order_id, po.purchase_order_id) == 0).FirstOrDefault();
                    CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.GRNPending,User.Identity.GetUserId());
                    StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.GRNPending);
                }
                else if (string.Compare(status, Status.Rejected) == 0)
                {
                    po.purchase_request_status = Status.Rejected;
                    po.checked_by = User.Identity.GetUserId();
                    po.checked_date =CommonClass.ToLocalTime(DateTime.Now);
                    po.checked_comment = comment;
                    db.SaveChanges();

                    tb_purchase_order poo = db.tb_purchase_order.Find(po.purchase_order_id);
                    poo.is_po_checked = false;
                    db.SaveChanges();

                    CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORejected,User.Identity.GetUserId());
                    StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORejected);

                }

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.checked_by, po.checked_date, comment);

            }
            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RejectApproval(string id,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_request po = db.tb_purchase_request.Find(id);
                po.purchase_request_status = Status.Rejected;
                po.approved_by = User.Identity.GetUserId();
                po.approved_date = CommonClass.ToLocalTime(DateTime.Now);
                po.approved_comment = comment;
                db.SaveChanges();

                tb_purchase_order poo = db.tb_purchase_order.Find(po.purchase_order_id);
                poo.is_po_checked = false;
                db.SaveChanges();

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORejected,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORejected);

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.approved_by, po.approved_date, comment);

            }
            return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelMR(string id,string comment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_purchase_request po = db.tb_purchase_request.Find(id);
                
                if (string.Compare(po.purchase_request_status, Status.Pending) == 0)
                {
                    po.purchase_request_status = Status.CancelledMR;
                    po.approved_by = User.Identity.GetUserId();
                    po.approved_date = CommonClass.ToLocalTime(DateTime.Now);
                    po.approved_comment = comment;
                    db.SaveChanges();
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.approved_by, po.approved_date, comment);
                }
                else
                {
                    po.purchase_request_status = Status.CancelledMR;
                    po.checked_by = User.Identity.GetUserId();
                    po.checked_date = CommonClass.ToLocalTime(DateTime.Now);
                    po.checked_comment = comment;
                    db.SaveChanges();
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.checked_by, po.checked_date, comment);
                }

                var poo = db.tb_purchase_order.Where(s => string.Compare(s.purchase_order_id, po.purchase_order_id) == 0).FirstOrDefault();
                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.MRCancelled,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.MRCancelled);

                #region Update MR
                var prr = (from poo0 in db.tb_purchase_request
                          join quote in db.tb_purchase_order on poo0.purchase_order_id equals quote.purchase_order_id
                           join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                           where string.Compare(poo0.pruchase_request_id,po.pruchase_request_id)==0
                           select pr).FirstOrDefault();
                if (prr != null)
                {
                    tb_item_request mr = db.tb_item_request.Find(prr.material_request_id);
                    if (mr != null)
                    {
                       tb_reject reject = new tb_reject();
                        reject.reject_id = Guid.NewGuid().ToString();
                        reject.ref_id = mr.ir_id;
                        reject.ref_type = "Item Request";
                        reject.comment = comment;
                        reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                        reject.rejected_by = User.Identity.GetUserId();
                        db.tb_reject.Add(reject);
                        db.SaveChanges();
                        CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), mr.ir_id, Status.CancelledMR, User.Identity.GetUserId(), CommonClass.ToLocalTime(DateTime.Now), comment);
                    }
                }
                #endregion

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetMyRequestAJax(string dateRange,string status)
        {
            try
            {
                List<PurchaseRequestViewModel> data = new List<PurchaseRequestViewModel>();
                data = CommonFunctions.GetPurchaseOrderMyRequestList(User.IsInRole(Role.SystemAdmin), User.Identity.GetUserId().ToString(),dateRange,status);

                return Json(new { data = data }, JsonRequestBehavior.AllowGet);

            }catch(Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPurchaseOrderApprovalAJax(string dateRange,string status)
        {
            try
            {
                List<PurchaseRequestViewModel> data = new List<PurchaseRequestViewModel>();
                data = CommonFunctions.GetPurchaseOrderMyApprovalList(User.IsInRole(Role.SystemAdmin), User.IsInRole(Role.FinanceManager),User.IsInRole(Role.AccountingManager),User.IsInRole(Role.OperationDirector) , User.Identity.GetUserId().ToString(),dateRange,status);

                return Json(new { data = data }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //​​​      public ActionResult Reject(string id)
        //        {
        //            using (kim_mexEntities db = new kim_mexEntities())
        //            {

        //                tb_Purchase_OrderAandR purchaseorderItemId = new tb_Purchase_OrderAandR();
        //                purchaseorderItemId.supplier_element = "Rejected";
        //                purchaseorderItemId.Upprove_by = User.Identity.GetUserId();
        //                purchaseorderItemId.Uprove_date = Class.CommonClass.ToLocalTime(DateTime.Now);
        //                db.SaveChanges();
        //                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        // GET: PurchaseOrderAandR/Delete/5
        //public ActionResult Delete(PurchaseOrderAandRViewModel model)
        //{
        //    var minDate = new DateTime(1900, 1, 1);
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            using (kim_mexEntities db = new kim_mexEntities())
        //            {
        //                tb_Purchase_OrderAandR purchaseorder = new tb_Purchase_OrderAandR();
        //                purchaseorder.active = false;
        //                purchaseorder.Update_by = string.Empty;
        //                purchaseorder.Update_Date = minDate;
        //                purchaseorder.purchase_order_id = Guid.NewGuid().ToString();
        //                purchaseorder.purchase_order_number = model.purchase_oder_number;

        //            }

        //        }

        //        return View();
        //    }
        //}

        // POST: PurchaseOrderAandR/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                Purchase_OrderAandR purchaseorder = db.Purchase_OrderAandR.Find(id);
                purchaseorder.active = false;
                purchaseorder.Update_Date =CommonClass.ToLocalTime(DateTime.Now);
                purchaseorder.Upprove_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using (kim_mexEntities db = new kim_mexEntities())
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
        public JsonResult DeleteItemPurchaseOrderAandRAttachment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "errror" });
            }
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_supplier supplier = db.tb_supplier.Find(id);
                if (supplier == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_supplier.Remove(supplier);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Supplier/"), supplier.supplier_id + supplier.supplier_id);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public ActionResult GetPurchaseOrderApprovalDataTable()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<PurchaseOrderAandRViewModel> purchaseorder = new List<PurchaseOrderAandRViewModel>();
                purchaseorder = this.GetPurchaseOrderApprovalList();
                return Json(new { result = "success", data = purchaseorder }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<PurchaseOrderAandRViewModel> GetPurchaseOrderApprovalList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var q = from po in db.Purchase_OrderAandR
                        join por in db.tb_purchase_order on po.purchase_oder_number equals por.purchase_oder_number
                        join su in db.tb_supplier  on po.supplier_id equals  su. supplier_id
                        where po.active == true
                        select new PurchaseOrderAandRViewModel
                        {
                            Purchase_order_id = po.Purchase_order_id,
                           // purchase_oder_number = po.purchase_oder_number,
                            purchase_oder_number = por.purchase_oder_number,
                            Purchar_Number = po.Purchar_Number,
                            //ref_id = po.ref_id,
                            checked_by = po.checked_by,
                            //checked_date = po.checked_date,
                            Create_date = po.Create_date,
                            Action = po.Action,
                            //rejected_by = po.rejected_by,
                            //rejected_date = po.rejected_date,
                            status = po.status,
                            Update_by = po.Update_by,
                            //Update_Date = po.Update_Date,
                            Upprove_by = po.Upprove_by,
//Uprove_date = po.Uprove_date,

                        };
                return q.OrderByDescending(x => x.Create_date).ToList();
            }
        }

        public List<PurchaseOrderViewModel> GetPurchaseOrderViewModel()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var q = from de in db.tb_purchase_order_detail
                        join po in db.tb_purchase_order on de.purchase_order_id equals po.purchase_order_id
                        join su_po in db.tb_po_supplier on de.po_detail_id equals su_po.po_detail_id
                        join su in db.tb_supplier on su_po.supplier_id equals su.supplier_id
                        select new PurchaseOrderDetailViewModel
                        {
                            supplier_name = su.supplier_name,

                        };
                //return q.OrderByDescending(x=> x.created_date).ToList();  
                return new List<PurchaseOrderViewModel>();
            }
        }
       
        public string GenerateApprovalNumber()
        {
            string quoteNo = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string last_no = "", quoteNum,number=string.Empty;
                var model = db.tb_purchase_request.OrderByDescending(x => x.created_date).FirstOrDefault();
                if(model!=null)
                    number = db.tb_purchase_request.OrderByDescending(x => x.created_date).FirstOrDefault().purchase_request_number;
                if (string.IsNullOrEmpty(number))
                    last_no = "001";
                else
                {
                    string[] splitNumber = number.Split('-');
                    //number = number.Substring(number.Length - 3, 3);
                    number = splitNumber[splitNumber.Count() - 1];
                    int num = Convert.ToInt32(number) + 1;
                    if (num.ToString().Length == 1) last_no = "00" + num;
                    else if (num.ToString().Length == 2) last_no = "0" + num;
                    else last_no = num.ToString();
                }
                string yy = (Class.CommonClass.ToLocalTime(DateTime.Now).Year).ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                quoteNo = "PON-" + yy + "-" + mm + "-" + last_no;
            }
            return quoteNo;
        }

        public ActionResult GetPOSupplierDataTabel(string id)
        {
            //var po_supplier = GetPOSupplierItem(id);

            using(kim_mexEntities db=new kim_mexEntities())
            {
                PurchaseOrderViewModel model = new PurchaseOrderViewModel();
                List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
                List<PurchaseOrderDetailViewModel> poDetails = new List<PurchaseOrderDetailViewModel>();
                List<PurchaseOrderDetailViewModel> poDetails1 = new List<PurchaseOrderDetailViewModel>();

                model.project_full_name = (from tbl in db.tb_project
                                           join mr in db.tb_item_request on tbl.project_id equals mr.ir_project_id
                                           join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                                           join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                                           where string.Compare(quote.purchase_order_id, id) == 0
                                           select tbl.project_full_name).FirstOrDefault();

                var poReports = (from report in db.tb_po_report
                                 orderby report.created_date
                                 where string.Compare(report.po_ref_id, id) == 0
                                 select report).ToList();

                foreach(var por in poReports)
                {
                    PurchaseOrderReportViewModel pReport = new PurchaseOrderReportViewModel();
                    pReport.po_report_id = por.po_report_id;
                    pReport.po_report_number = por.po_report_number;
                    pReport.vat_status = por.vat_status;
                    pReport.created_date = por.created_date;
                    pReport.po_supplier_id = por.po_supplier_id;
                    pReport.supplier = db.tb_supplier.Where(s => string.Compare(s.supplier_id, pReport.po_supplier_id) == 0).Select(s => new SupplierViewModel()
                    { supplier_id = s.supplier_id, supplier_name = s.supplier_name, is_local = s.is_local,discount=s.discount }).FirstOrDefault();
                    pReport.purchaseOrderDetails = (from pd in db.tb_purchase_order_detail
                                                    join item in db.tb_product on pd.item_id equals item.product_id
                                                    join unit in db.tb_unit on pd.po_unit equals unit.Id
                                                    join psup in db.tb_po_supplier on pd.po_detail_id equals psup.po_detail_id
                                                    where string.Compare(pd.purchase_order_id, id) == 0 && pd.item_vat == pReport.vat_status && string.Compare(psup.supplier_id,pReport.po_supplier_id)==0 && psup.is_selected==true
                                                    select new PurchaseOrderDetailViewModel()
                                                    {
                                                        item_id=pd.item_id,
                                                        product_name=item.product_name,
                                                        product_unit=unit.Name,
                                                        unit_price=pd.unit_price,
                                                        po_quantity=pd.po_quantity,
                                                        lump_sum_discount_amount=pd.lump_sum_discount_amount,
                                                    }).ToList();
                    pReport.lump_sum_discount_amount = pReport.purchaseOrderDetails.GroupBy(s => s.lump_sum_discount_amount).Select(s => s.Key).FirstOrDefault();
                    model.poReports.Add(pReport);

                }

                //poDetails = (from dPO in db.tb_purchase_order_detail
                //             join item in db.tb_product on dPO.item_id equals item.product_id
                //             where dPO.purchase_order_id == id && dPO.item_status == Status.Approved
                //             select new PurchaseOrderDetailViewModel() {
                //                 po_detail_id = dPO.po_detail_id,
                //                 purchase_order_id = dPO.purchase_order_id,
                //                 item_id = dPO.item_id,
                //                 quantity = dPO.quantity,
                //                 product_code = item.product_code,
                //                 product_name = item.product_name,
                //                 product_unit = item.product_unit,
                //                 unit_price = dPO.unit_price,
                //                 item_status = dPO.item_status,
                //                 po_quantity = dPO.po_quantity
                //             }).ToList();
                //foreach (var pod in poDetails)
                //{
                //    //List<PurchaseOrderItemSupplier> pois = new List<PurchaseOrderItemSupplier>();
                //    PurchaseOrderDetailViewModel detail = pod;

                //    PurchaseOrderItemSupplier pois = new PurchaseOrderItemSupplier();
                //    pois = (from pos in db.tb_po_supplier
                //            join sup in db.tb_supplier on pos.supplier_id equals sup.supplier_id
                //            orderby pos.sup_number
                //            where pos.po_detail_id == pod.po_detail_id && pos.is_selected == true
                //            select new PurchaseOrderItemSupplier()
                //            {
                //                po_supplier_id = pos.po_supplier_id,
                //                po_detail_id = pos.po_detail_id,
                //                unit_price = pos.unit_price,
                //                supplier_id = sup.supplier_id,
                //                supplier_name = sup.supplier_name,
                //                sup_number = pos.sup_number,
                //                is_selected = pos.is_selected,
                //                Reason = pos.Reason,
                //                po_quantity = pos.po_qty,
                //                supplier_address = sup.supplier_address,
                //                supplier_phone = sup.supplier_phone,
                //                supplier_email = sup.supplier_email
                //            }).FirstOrDefault();
                //    //pod.poSuppliers = pois;
                //    poSuppliers.Add(pois);
                //    pod.poSuppliers.Add(pois);

                //    detail.supplier_id = pois.supplier_id;
                //    detail.unit_price = pois.unit_price;
                //    poDetails1.Add(detail);
                //}
                //model.poDetails = poDetails1;

                //var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                //foreach (var sup in suppliers)
                //{
                //    var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();

                //    if (supplier != null)
                //    {
                //        PurchaseOrderItemSupplier posup = new PurchaseOrderItemSupplier();
                        
                //        posup.supplier_id = sup;
                //        posup.supplier_name = supplier.supplier_name;
                //        posup.supplier_email = supplier.supplier_email;
                //        posup.supplier_address = supplier.supplier_address;
                //        posup.supplier_phone = supplier.supplier_phone;
                //        posup.reports = db.tb_po_report.Where(w => string.Compare(w.po_ref_id, id) == 0 && string.Compare(w.po_supplier_id, sup) == 0).Select(w => new PurchaseOrderReportViewModel()
                //        {
                //            po_report_id=w.po_report_id,
                //            po_report_number=w.po_report_number,

                //        }).ToList();

                //        model.poSuppliers.Add(posup);
                //    }

                //}

                return Json(new { result = "success", data = model }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public PurchaseOrderViewModel GetPOSupplierItem(string id)
        {
            PurchaseOrderViewModel model = new PurchaseOrderViewModel();
            List<PurchaseOrderItemSupplier> poSuppliers = new List<PurchaseOrderItemSupplier>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                model = (from po in db.tb_purchase_order
                         join ir in db.tb_item_request on po.item_request_id equals ir.ir_id
                         join proj in db.tb_project on ir.ir_project_id equals proj.project_id
                         where po.purchase_order_id == id
                         select new PurchaseOrderViewModel()
                         {
                             purchase_order_id = po.purchase_order_id,
                             item_request_id = po.item_request_id,
                             ir_no = ir.ir_no,
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
                                     select new PurchaseOrderDetailViewModel() {
                                         po_detail_id = dPO.po_detail_id,
                                         purchase_order_id = dPO.purchase_order_id,
                                         item_id = dPO.item_id,
                                         quantity = dPO.quantity,
                                         product_code = item.product_code,
                                         product_name = item.product_name,
                                         product_unit = item.product_unit,
                                         unit_price = dPO.unit_price,
                                         item_status = dPO.item_status,
                                         po_quantity = dPO.po_quantity }).ToList();
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
                                        vat = pos.vat,
                                        Reason = pos.Reason,                                     
                                        po_quantity = pos.po_qty,
                                       supplier_address = sup.supplier_address,
                                        supplier_phone = sup.supplier_phone,
                                        supplier_email = sup.supplier_email }).FirstOrDefault();
                            pois.po_quantity = pod.quantity;
                            //pod.poSuppliers = pois;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                        model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();
                            var po_supplier = poSuppliers.FirstOrDefault(x => x.supplier_id == sup);
                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
                                    supplier_id = sup,
                                    supplier_name = supplier.supplier_name,
                                    supplier_email = supplier.supplier_email,
                                    supplier_address = supplier.supplier_address,
                                    supplier_phone = supplier.supplier_phone,
                                    po_quantity = po_supplier?.po_quantity,
                                    unit_price = po_supplier?.unit_price,
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
                                     select new PurchaseOrderDetailViewModel() {
                                         po_detail_id = dPO.po_detail_id,
                                         purchase_order_id = dPO.purchase_order_id,
                                         item_id = dPO.item_id,
                                         
                                         quantity = dPO.quantity,
                                         product_code = item.product_code,
                                         product_name = item.product_name,
                                         product_unit = item.product_unit,
                                         unit_price = dPO.unit_price,
                                         item_status = dPO.item_status,
                                         po_quantity = dPO.po_quantity }).ToList();
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
                                        supplier_email = sup.supplier_email }).FirstOrDefault();
                            //pod.poSuppliers = pois;
                            pois.po_quantity = pod.quantity;
                            poSuppliers.Add(pois);
                            pod.poSuppliers.Add(pois);
                        }
                       // model.poDetails = poDetails;

                        var suppliers = (from ss in poSuppliers select ss.supplier_id).Distinct();
                        foreach (var sup in suppliers)
                        {
                            var supplier = db.tb_supplier.Where(x => x.supplier_id == sup).FirstOrDefault();
                            var po_supplier = poSuppliers.FirstOrDefault(x => x.supplier_id == sup);
                            if (supplier != null)
                            {
                                model.poSuppliers.Add(new PurchaseOrderItemSupplier() {
                                    supplier_id = sup,
                                    supplier_name = supplier.supplier_name,
                                    supplier_email = supplier.supplier_email,
                                    supplier_address = supplier.supplier_address,
                                    supplier_phone = supplier.supplier_phone ,
                                    po_quantity = po_supplier?.po_quantity,
                                    unit_price = po_supplier?.unit_price,
                                });

                            }

                        }
                    }

                }
            }
            return model;
        }

        //public ActionResult Cancel(string id)
        //{
        //    PurchaseOrderViewModel model = new PurchaseOrderViewModel();          
        //    model = this.GetPOSupplierItem(id);
        //    return View(model);
        //}

        [HttpPost]
        public ActionResult Cancel(string id,string comment)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_purchase_request po = db.tb_purchase_request.Find(id);
                po.purchase_request_status = Status.RequestCancelled;
                po.updated_by = User.Identity.GetUserId().ToString();
                po.updated_date =CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                tb_purchase_order poo = db.tb_purchase_order.Find(po.purchase_order_id);
                poo.is_po_checked = false;
                db.SaveChanges();

                CommonClass.UpdateMaterialRequestStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORequestCancelled,User.Identity.GetUserId());
                StockTransfer.UpdateStockTransferProcessStatus(CommonFunctions.GetMaterialRequestIdbyPR(poo.item_request_id), ShowStatus.PORequestCancelled);

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), po.pruchase_request_id, po.purchase_request_status, po.updated_by, po.updated_date,comment);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ApproveByCO(int id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                Purchase_OrderAandR purchaseorder = db.Purchase_OrderAandR.Find(id);
                if (purchaseorder.status == "Pending")
                {
                    purchaseorder.status = "Approved";
                    db.SaveChanges();
                    return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
                }
                else if (purchaseorder.status == "Approved")
                {
                    return Json(new { result = "fail", message = "Purchase order already approved!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult RejectByCO(int id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                Purchase_OrderAandR purchaseorder = db.Purchase_OrderAandR.Find(id);
                if (purchaseorder.status == "Pending")
                {
                    purchaseorder.status = "Reject";
                    db.SaveChanges();
                    return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
                }
                else if (purchaseorder.status == "Reject")
                {
                    return Json(new { result = "fail", message = "Purchase order already approved!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public List<tb_purchase_request> GetPurchaseOrderListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if(User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.ManagingDirector) || User.IsInRole(Role.TechnicalDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.AstOperationDirector) || User.IsInRole(Role.WorkshopController))
                    return db.tb_purchase_request.OrderByDescending(s => s.created_date).Where(s => s.status == true).ToList();
                else
                {
                    List<tb_purchase_request> models = new List<tb_purchase_request>();
                    string userid = User.Identity.GetUserId().ToString();
                    if (User.IsInRole(Role.Purchaser))
                    {
                        var items = db.tb_purchase_request.Where(s => s.status == true &&( string.Compare(s.created_by, userid) == 0 || string.Compare(s.updated_by, userid) == 0)).ToList();
                        foreach (var item in items)
                            models.Add(item);
                    }
                    if(User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager))
                    {
                        var items = db.tb_purchase_request.Where(s => s.status == true && (string.Compare(s.purchase_request_status, Status.Pending) == 0 || string.Compare(s.approved_by, userid) == 0)).ToList();
                        foreach (var item in items)
                            models.Add(item);
                    }
                    if (User.IsInRole(Role.OperationDirector))
                    {
                        var items = db.tb_purchase_request.Where(s => s.status == true && (string.Compare(s.purchase_request_status, Status.Approved) == 0 || string.Compare(s.checked_by, userid) == 0)).ToList();
                        foreach (var item in items)
                            models.Add(item);
                    }

                    if (User.IsInRole(Role.ProjectManager))
                    {
                        var items = (from po in db.tb_purchase_request
                                    join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                    from quote in pquote.DefaultIfEmpty()
                                    join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                    from pr in ppr.DefaultIfEmpty()
                                    join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                    from mr in pmr.DefaultIfEmpty()
                                    join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                    join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                                    orderby po.created_date descending
                                    where po.status == true  && string.Compare(pm.project_manager_id,userid)==0
                                    select po).ToList();
                        foreach (var item in items)
                            models.Add(item);
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        var items = (from po in db.tb_purchase_request
                                     join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                     from quote in pquote.DefaultIfEmpty()
                                     join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                     from pr in ppr.DefaultIfEmpty()
                                     join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                     from mr in pmr.DefaultIfEmpty()
                                     join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                     join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                                     orderby po.created_date descending
                                     where po.status == true && string.Compare(sm.site_manager, userid) == 0
                                     select po).ToList();
                        foreach (var item in items)
                            models.Add(item);
                    }

                    models = models.OrderByDescending(o => o.created_date).DistinctBy(s=>s.pruchase_request_id).ToList();
                    return models;
                }
            }
        }

        public List<PurchaseRequestViewModel> GetPurchaseOrderListItemsByDateRangeandStatus(string dateRange,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<PurchaseRequestViewModel> models = new List<PurchaseRequestViewModel>();
                List<tb_purchase_request> results = new List<tb_purchase_request>();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                if (string.Compare(status, "0") == 0)
                {
                    if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.ManagingDirector) || User.IsInRole(Role.TechnicalDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.AstOperationDirector) || User.IsInRole(Role.WorkshopController) || User.IsInRole(Role.Purchaser))
                    {
                        results = db.tb_purchase_request.OrderByDescending(s => s.created_date).Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate).ToList();
                    }
                    else
                    {
                        string userid = User.Identity.GetUserId().ToString();
                        //if (User.IsInRole(Role.Purchaser))
                        //{
                        //    var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && (string.Compare(s.created_by, userid) == 0 || string.Compare(s.updated_by, userid) == 0)).ToList();
                        //    foreach (var item in items)
                        //        results.Add(item);
                        //}
                        if (User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager))
                        {
                            var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && (string.Compare(s.purchase_request_status, Status.Pending) == 0 || string.Compare(s.approved_by, userid) == 0)).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }
                        if (User.IsInRole(Role.OperationDirector))
                        {
                            var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && (string.Compare(s.purchase_request_status, Status.Approved) == 0 || string.Compare(s.checked_by, userid) == 0)).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }

                        if (User.IsInRole(Role.ProjectManager))
                        {
                            var items = (from po in db.tb_purchase_request
                                         join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                         from quote in pquote.DefaultIfEmpty()
                                         join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                         from pr in ppr.DefaultIfEmpty()
                                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                         from mr in pmr.DefaultIfEmpty()
                                         join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                         join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                                         orderby po.created_date descending
                                         where po.status == true && string.Compare(pm.project_manager_id, userid) == 0 && po.created_date >= startDate && po.created_date <= endDate
                                         select po).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }
                        if (User.IsInRole(Role.SiteManager))
                        {
                            var items = (from po in db.tb_purchase_request
                                         join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                         from quote in pquote.DefaultIfEmpty()
                                         join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                         from pr in ppr.DefaultIfEmpty()
                                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                         from mr in pmr.DefaultIfEmpty()
                                         join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                         join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                                         orderby po.created_date descending
                                         where po.status == true && string.Compare(sm.site_manager, userid) == 0 && po.created_date >= startDate && po.created_date <= endDate
                                         select po).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }

                        results = results.OrderByDescending(o => o.created_date).DistinctBy(s => s.pruchase_request_id).ToList();
                    }
                }
                else
                {
                    if (User.IsInRole(Role.SystemAdmin) || User.IsInRole(Role.ManagingDirector) || User.IsInRole(Role.TechnicalDirector) || User.IsInRole(Role.ProcurementManager) || User.IsInRole(Role.AstOperationDirector) || User.IsInRole(Role.WorkshopController) || User.IsInRole(Role.Purchaser))
                    {
                        results = db.tb_purchase_request.OrderByDescending(s => s.created_date).Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && string.Compare(s.purchase_request_status,status)==0 ).ToList();
                    }
                    else
                    {
                        string userid = User.Identity.GetUserId().ToString();
                        //if (User.IsInRole(Role.Purchaser))
                        //{
                        //    var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && string.Compare(s.purchase_request_status, status) == 0 && (string.Compare(s.created_by, userid) == 0 || string.Compare(s.updated_by, userid) == 0)).ToList();
                        //    foreach (var item in items)
                        //        results.Add(item);
                        //}
                        if (User.IsInRole(Role.FinanceManager) || User.IsInRole(Role.AccountingManager))
                        {
                            var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && string.Compare(s.purchase_request_status, status) == 0 && (string.Compare(s.purchase_request_status, Status.Pending) == 0 || string.Compare(s.approved_by, userid) == 0)).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }
                        if (User.IsInRole(Role.OperationDirector))
                        {
                            var items = db.tb_purchase_request.Where(s => s.status == true && s.created_date >= startDate && s.created_date <= endDate && string.Compare(s.purchase_request_status, status) == 0 && (string.Compare(s.purchase_request_status, Status.Approved) == 0 || string.Compare(s.checked_by, userid) == 0)).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }

                        if (User.IsInRole(Role.ProjectManager))
                        {
                            var items = (from po in db.tb_purchase_request
                                         join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                         from quote in pquote.DefaultIfEmpty()
                                         join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                         from pr in ppr.DefaultIfEmpty()
                                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                         from mr in pmr.DefaultIfEmpty()
                                         join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                         join pm in db.tb_project_pm on proj.project_id equals pm.project_id
                                         orderby po.created_date descending
                                         where po.status == true && string.Compare(pm.project_manager_id, userid) == 0 && po.created_date >= startDate && po.created_date <= endDate && string.Compare(po.purchase_request_status, status) == 0
                                         select po).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }
                        if (User.IsInRole(Role.SiteManager))
                        {
                            var items = (from po in db.tb_purchase_request
                                         join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                         from quote in pquote.DefaultIfEmpty()
                                         join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                         from pr in ppr.DefaultIfEmpty()
                                         join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                         from mr in pmr.DefaultIfEmpty()
                                         join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                         join sm in db.tb_site_manager_project on proj.project_id equals sm.project_id
                                         orderby po.created_date descending
                                         where po.status == true && string.Compare(sm.site_manager, userid) == 0 && po.created_date >= startDate && po.created_date <= endDate && string.Compare(po.purchase_request_status, status) == 0
                                         select po).ToList();
                            foreach (var item in items)
                                results.Add(item);
                        }

                        results = results.OrderByDescending(o => o.created_date).DistinctBy(s => s.pruchase_request_id).ToList();
                    }
                }

                foreach(var rs in results)
                {
                    PurchaseRequestViewModel model = new PurchaseRequestViewModel();
                    model.pruchase_request_id = rs.pruchase_request_id;
                    model.purchase_order_id = rs.purchase_order_id;
                    model.created_by = rs.created_by;
                    model.created_date = rs.created_date;
                    model.str_created_date = Convert.ToDateTime(rs.created_date).ToString("yyyy/MM/dd");
                    model.purchase_request_number = rs.purchase_request_number;
                    model.purchase_request_status = rs.purchase_request_status;
                    model.quote_number= string.IsNullOrEmpty(rs.purchase_order_id) ? string.Empty : db.tb_purchase_order.Find(rs.purchase_order_id).purchase_oder_number;
                    model.project_short_name= string.IsNullOrEmpty(rs.purchase_order_id) ? string.Empty : CommonFunctions.GetProjectItembyQuoteId(rs.purchase_order_id).project_full_name;
                    if (!string.IsNullOrEmpty(rs.purchase_order_id))
                    {
                        var mr_obj = (from q in db.tb_purchase_order
                                      join pr in db.tb_purchase_requisition on q.item_request_id equals pr.purchase_requisition_id
                                      join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                      where string.Compare(q.purchase_order_id, rs.purchase_order_id) == 0
                                      select mr).FirstOrDefault();
                        if (mr_obj != null)
                        {
                            model.mr_number = mr_obj.ir_no;
                            model.mr_id = mr_obj.ir_id;

                        }
                    }
                    model.created_by_text = CommonFunctions.GetUserFullnamebyUserId(rs.created_by);
                    model.purchase_request_status_full = ShowStatus.GetPurchaseOrderShowStatus(model.purchase_request_status);
                    if (!string.IsNullOrEmpty(rs.purchase_order_id))
                    {
                        model.poDetails = PurchaseOrderReportViewModel.GetPOReportbyPurchaseOrderId(model.pruchase_request_id);

                    }
                    
                    models.Add(model);
                }

                return models;
            }
        }
        public ActionResult GetPurchaseOrderDataTableByDateRangeandStatus(string dateRange,string status)
        {
            return Json(new { data = GetPurchaseOrderListItemsByDateRangeandStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseOrderList()
        {
            return View();
        }
        public ActionResult GetPurchaseOrderReportbyDaterange(string dateRange)
        {
            return Json(new { data = PurchaseOrderReportViewModel.GetPORequestByDateRange(dateRange) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletePurchaseOrderReportbyId(string id)
        {
            bool is_success = true;
            string message = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();

                tb_po_report po_report = db.tb_po_report.Find(id);
                if (po_report != null)
                {
                    tb_po_report_deleted po_report_deleted = new tb_po_report_deleted();
                    po_report_deleted.po_report_id = po_report.po_report_id;
                    po_report_deleted.po_ref_id = po_report.po_ref_id;
                    po_report_deleted.po_report_number = po_report.po_report_number;
                    po_report_deleted.po_supplier_id = po_report.po_supplier_id;
                    po_report_deleted.created_date = po_report.created_date;
                    po_report_deleted.vat_status = po_report.vat_status;
                    po_report_deleted.is_completed = po_report.is_completed;
                    po_report_deleted.is_lpo = po_report.is_lpo;
                    po_report_deleted.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                    po_report_deleted.updated_by = User.Identity.GetUserId();
                    db.tb_po_report_deleted.Add(po_report_deleted);
                    db.SaveChanges();

                    db.tb_po_report.Remove(po_report);
                    db.SaveChanges();
                }
                
            }catch(Exception ex)
            {

            }
            return Json(new { is_success = is_success, message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewPOReportDetail(string id)
        {
            return View(PurchaseOrderReportViewModel.GetPOReportItemDetail(id));
        }
        public ActionResult UpdatePOReportStatusForGRN(string po_report_number)
        {
            bool isSuccess = true;
            string message = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_po_report poReport = db.tb_po_report.Find(po_report_number);
                if (poReport != null)
                {
                    poReport.is_completed = false;
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {
                isSuccess = false;
                message = ex.InnerException.InnerException.ToString();
            }
            return Json(new { isSuccess = isSuccess, message = message }, JsonRequestBehavior.AllowGet);
        }

    }
}
