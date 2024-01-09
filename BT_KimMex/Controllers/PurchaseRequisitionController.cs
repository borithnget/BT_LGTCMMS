using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class PurchaseRequisitionController : Controller
    {
        
        // GET: PurchaseRequisition
        public ActionResult Index()
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            //return View(this.GetPurchaseRequisitionListItems());
            return View();
        }
        public ActionResult RequestList()
        {
            List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
            if (User.IsInRole(Role.SystemAdmin))
            {
                models = this.GetPurchaseRequisitionListItems();
            }
            else
            {
                string userid = User.Identity.GetUserId().ToString();
                kim_mexEntities db = new kim_mexEntities();
                models = (from pr in db.tb_purchase_requisition
                          join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                          join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                          orderby pr.updated_at descending
                          where pr.status == true && string.Compare(pr.created_by,userid)==0
                          select new PurchaseRequisitionViewModel()
                          {
                              purchase_requisition_id = pr.purchase_requisition_id,
                              purchase_requisition_number = pr.purchase_requisition_number,
                              material_request_id = pr.material_request_id,
                              materail_request_number = mr.ir_no,
                              created_at = pr.updated_at,
                              project_id = mr.ir_project_id,
                              project_fullname = pro.project_full_name,
                              created_by = pr.created_by,
                              purchase_requisition_status = pr.purchase_requisition_status
                          }).ToList();
            }
            return View(models);
        }
        public ActionResult ApprovalList()
        {
            //List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
            //if (User.IsInRole(Role.SystemAdmin))
            //{
            //    models = this.GetPurchaseRequisitionListItems();
            //}
            //else
            //{
            //    string userid = User.Identity.GetUserId().ToString();
            //    kim_mexEntities db = new kim_mexEntities();
            //    models = (from pr in db.tb_purchase_requisition
            //              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
            //              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
            //              orderby pr.updated_at descending
            //              where pr.status == true && ( string.Compare(pr.purchase_requisition_status,Status.Pending)==0 || string.Compare(pr.approved_by,userid)==0)
            //              select new PurchaseRequisitionViewModel()
            //              {
            //                  purchase_requisition_id = pr.purchase_requisition_id,
            //                  purchase_requisition_number = pr.purchase_requisition_number,
            //                  material_request_id = pr.material_request_id,
            //                  materail_request_number = mr.ir_no,
            //                  created_at = pr.updated_at,
            //                  project_id = mr.ir_project_id,
            //                  project_fullname = pro.project_full_name,
            //                  created_by = pr.created_by,
            //                  purchase_requisition_status = pr.purchase_requisition_status
            //              }).ToList();
            //}
            //return View(models);
            return View();
        }
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();
            return View(this.GetPurchaseRequisitionItem(id));
        }
        public ActionResult Create(string id=null)
        {
            ViewBag.PRNumber = CommonClass.GenerateProcessNumber("PR");
            PurchaseRequisitionViewModel model = new PurchaseRequisitionViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                kim_mexEntities db = new kim_mexEntities();
                model.material_request_id = id;
                model.project_id = db.tb_item_request.Find(id).ir_project_id;
                model.materialRequests= this.GetAllItemRequestDropdownList().Where(w => string.Compare(w.ir_project_id, model.project_id) == 0).ToList();
                model.materialRequestItems = ItemRequest.GetMaterialRequestListItems(id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(PurchaseRequisitionViewModel model,List<string> item_id,List<string> item_qty,List<string> item_unit)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            if (!ModelState.IsValid)
            {
                ViewBag.PRNumber = CommonClass.GenerateProcessNumber("PR");
                return View();
            }
            kim_mexEntities db = new kim_mexEntities();
            tb_purchase_requisition purchaseRequisition = new tb_purchase_requisition();
            purchaseRequisition.purchase_requisition_id = Guid.NewGuid().ToString();
            purchaseRequisition.purchase_requisition_number = CommonClass.GenerateProcessNumber("PR");
            purchaseRequisition.material_request_id = model.material_request_id;
            purchaseRequisition.purchase_requisition_status = Status.Pending;
            purchaseRequisition.is_quote_complete = false;
            purchaseRequisition.status = true;
            purchaseRequisition.created_at =CommonClass.ToLocalTime(DateTime.Now);
            purchaseRequisition.created_by = User.Identity.GetUserId();
            purchaseRequisition.updated_at =CommonClass.ToLocalTime(DateTime.Now);
            purchaseRequisition.updated_by = User.Identity.GetUserId();
            db.tb_purchase_requisition.Add(purchaseRequisition);
            db.SaveChanges();

            CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(controllerName), purchaseRequisition.purchase_requisition_id, purchaseRequisition.purchase_requisition_status, purchaseRequisition.created_by, purchaseRequisition.created_at);

            if (item_id.Count() > 0)
            {
                for (int i = 0; i < item_id.Count(); i++)
                {
                    tb_purchase_requisition_detail detail = new tb_purchase_requisition_detail();
                    detail.purchase_requisition_detail_id = Guid.NewGuid().ToString();
                    detail.purchase_requisition_id = purchaseRequisition.purchase_requisition_id;
                    detail.item_id = item_id[i];
                    detail.item_unit = item_unit[i];
                    detail.approved_qty = Convert.ToDecimal(item_qty[i]);
                    detail.remain_qty = Convert.ToDecimal(item_qty[i]);
                    detail.item_status = Status.WaitingApproval;
                    db.tb_purchase_requisition_detail.Add(detail);
                    db.SaveChanges();
                }
            }
            tb_item_request materialRequest = db.tb_item_request.Find(purchaseRequisition.material_request_id);
            materialRequest.is_mr = true;
            //materialRequest.po_status = ShowStatus.PRCreated;
            //materialRequest.st_status = ShowStatus.PRCreated;
            db.SaveChanges();
            CommonClass.UpdateMaterialRequestStatus(materialRequest.ir_id, ShowStatus.PRCreated, User.Identity.GetUserId());
            //update status in stock transfer return
            StockTransfer.UpdateStockTransferProcessStatus(purchaseRequisition.material_request_id, ShowStatus.PRCreated);


            return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_purchase_requisition purchaseRequisition = db.tb_purchase_requisition.Find(id);
                purchaseRequisition.status = false;
                purchaseRequisition.updated_at =CommonClass.ToLocalTime(DateTime.Now);
                purchaseRequisition.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                tb_item_request materialRequest = db.tb_item_request.Find(purchaseRequisition.material_request_id);
                materialRequest.is_mr = false;
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestCancel(string id,string remark)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_purchase_requisition purchaseRequisition = db.tb_purchase_requisition.Find(id);
                purchaseRequisition.purchase_requisition_status = Status.RequestCancelled;
                purchaseRequisition.updated_at =CommonClass.ToLocalTime(DateTime.Now);
                purchaseRequisition.updated_by = User.Identity.GetUserId();
                db.SaveChanges();

                tb_item_request materialRequest = db.tb_item_request.Find(purchaseRequisition.material_request_id);
                materialRequest.is_mr = false;
                //materialRequest.po_status = ShowStatus.PRRequestCancelled;
                //materialRequest.st_status = ShowStatus.PRRequestCancelled;
                db.SaveChanges();

                CommonClass.UpdateMaterialRequestStatus(materialRequest.ir_id, ShowStatus.PRRequestCancelled, User.Identity.GetUserId());

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), purchaseRequisition.purchase_requisition_id, purchaseRequisition.purchase_requisition_status, User.Identity.GetUserId(), CommonClass.ToLocalTime(DateTime.Now) ,remark);

                //update status in stock transfer return
                StockTransfer.UpdateStockTransferProcessStatus(purchaseRequisition.material_request_id, ShowStatus.PRRequestCancelled);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Approval(string id,string status,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_purchase_requisition purchaseRequisition = db.tb_purchase_requisition.Find(id);
                purchaseRequisition.purchase_requisition_status = status;
                purchaseRequisition.approved_at =CommonClass.ToLocalTime(DateTime.Now);
                purchaseRequisition.approved_by = User.Identity.GetUserId();
                purchaseRequisition.approved_comment = comment;
                db.SaveChanges();

                CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), purchaseRequisition.purchase_requisition_id, purchaseRequisition.purchase_requisition_status, purchaseRequisition.approved_by, purchaseRequisition.approved_at, comment);

                string shwoStatus = string.Compare(status, Status.Approved) == 0 ? ShowStatus.SupplierQuotePending : string.Compare(status, Status.Rejected) == 0 ? ShowStatus.PRRejected : ShowStatus.MRCancelled;
                //update MR Status
                string mrId = purchaseRequisition.material_request_id;

                tb_item_request mrObj = db.tb_item_request.Find(mrId);

                mrObj.is_mr = string.Compare(purchaseRequisition.purchase_requisition_status, Status.Rejected) == 0 ? false : mrObj.is_mr;

                //mrObj.po_status = shwoStatus;
                //mrObj.st_status = shwoStatus;

                db.SaveChanges();
                CommonClass.UpdateMaterialRequestStatus(mrObj.ir_id, shwoStatus, User.Identity.GetUserId());

                StockTransfer.UpdateStockTransferProcessStatus(mrId, shwoStatus);

                #region Update MR
                if (string.Compare(purchaseRequisition.purchase_requisition_status, Status.cancelled) == 0)
                {
                    tb_reject reject = new tb_reject();
                    reject.reject_id = Guid.NewGuid().ToString();
                    reject.ref_id = mrObj.ir_id;
                    reject.ref_type = "Item Request";
                    reject.comment = comment;
                    reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    reject.rejected_by = User.Identity.GetUserId();
                    db.tb_reject.Add(reject);
                    db.SaveChanges();
                    CommonClass.SubmitProcessWorkflow(CommonClass.GetSytemMenuIdbyControllerName(this.GetControllerName()), mrObj.ir_id, Status.CancelledMR, User.Identity.GetUserId(), CommonClass.ToLocalTime(DateTime.Now), comment);
                }

                #endregion

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPurchaseRequisitionByProjectJson(string id)
        {
            return Json(PurchaseRequisitionViewModel.GetPurchaseRequisitionCompletedListItems().Where(s => string.Compare(s.project_id, id) == 0).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPurchaseRequisitionItemListbyJson(string id)
        {

            //return Json(new { data = this.GetPurchaseRequisitionItem(id) }, JsonRequestBehavior.AllowGet);
            //Enhancement Milstone1 20230502
            return Json(new { data = ItemRequest.GetMRRemainQuantityItemByPRId (id) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetItemRequestsbyProject(string id)
        {
            List<ItemRequestViewModel> itemRequests = new List<ItemRequestViewModel>();
            itemRequests = this.GetAllItemRequestDropdownList().Where(w => string.Compare(w.ir_project_id, id) == 0).ToList();
            return Json(itemRequests, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPurchaseRequisitionItemsByDateRangeandStatusAJax(string dateRange,string status)
        {
            return Json(new { data = GetPurchaseRequisitionItemsByDateRangeAndStatus(dateRange, status) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPurchaseRequisitionApprovalListItemsByDateRangeandStatusAJAX(string dateRange,string status,bool isIncludeDate=true)
        {
            return Json(new { data = GetPurchaseRequisitionApprovalListItemsByDatetimeRangeAndStatus(dateRange, status, isIncludeDate) },JsonRequestBehavior.AllowGet);
        }
        public List<Models.ItemRequestViewModel> GetAllItemRequestDropdownList(string id = null)
        {
            List<Models.ItemRequestViewModel> models = new List<ItemRequestViewModel>();
            using (kim_mexEntities context = new kim_mexEntities())
            {
                //models = context.tb_item_request.OrderByDescending(x => x.created_date).Where(x => x.status == true && x.is_mr == false && ((string.Compare(x.ir_status, "Approved") == 0 && x.is_completed==false) || (string.Compare(x.ir_status,"Completed")==0 && x.is_completed==false ))).Select(x => new Models.ItemRequestViewModel() { ir_id = x.ir_id, ir_no = x.ir_no, created_date = x.created_date, ir_project_id = x.ir_project_id }).ToList();
                models = context.tb_item_request.OrderByDescending(x => x.created_date).Where(x => x.status == true && x.is_mr == false && x.is_completed==false && (string.Compare(x.ir_status, "Approved") == 0  || string.Compare(x.ir_status, "Completed") == 0)).Select(x => new Models.ItemRequestViewModel() { ir_id = x.ir_id, ir_no = x.ir_no, created_date = x.created_date, ir_project_id = x.ir_project_id }).ToList();
                if (!string.IsNullOrEmpty(id))
                {
                    var isExits = models.Where(m => string.Compare(m.ir_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExits)
                    {
                        var item = context.tb_item_request.Find(id);
                        models.Add(new ItemRequestViewModel() { ir_id = item.ir_id, ir_no = item.ir_no, created_date = item.created_date, ir_project_id = item.ir_project_id });
                        models = models.OrderByDescending(x => x.created_date).ToList();
                    }
                }
            }
            return models;
        }
        public List<PurchaseRequisitionViewModel> GetPurchaseRequisitionListItems()
        {
            List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                models = (from pr in db.tb_purchase_requisition
                          join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                          join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                          orderby pr.updated_at descending
                          where pr.status == true
                          select new PurchaseRequisitionViewModel()
                          {
                              purchase_requisition_id=pr.purchase_requisition_id,
                              purchase_requisition_number=pr.purchase_requisition_number,
                              material_request_id=pr.material_request_id,
                              materail_request_number=mr.ir_no,
                              created_at=pr.updated_at,
                              project_id=mr.ir_project_id,
                              project_fullname=pro.project_full_name,
                              created_by=pr.created_by,
                              purchase_requisition_status=pr.purchase_requisition_status
                          }).ToList();
            }
            return models;
        }
        public List<PurchaseRequisitionViewModel> GetPurchaseRequisitionItemsByDateRangeAndStatus(string dateRange,string status)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
                List<PRFilterRequestModel> results = new List<PRFilterRequestModel>();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                if (string.Compare(status, "0") == 0)
                {
                    results = (from pr in db.tb_purchase_requisition
                               join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                               join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                               orderby pr.updated_at descending
                               where pr.status == true && pr.created_at >= startDate && pr.created_at <= endDate
                               select new PRFilterRequestModel()
                               {
                                   pr = pr,
                                   mr = mr,
                                   pro = pro
                               }).ToList();                 
                }else
                {
                    results = (from pr in db.tb_purchase_requisition
                              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                              orderby pr.updated_at descending
                              where pr.status == true && string.Compare(pr.purchase_requisition_status,status)==0 && pr.created_at >= startDate && pr.created_at <= endDate
                              select new PRFilterRequestModel()
                              {
                                  pr = pr,
                                  mr = mr,
                                  pro = pro
                              }).ToList();
                }

                foreach (var rs in results)
                {
                    string created_at_text = Convert.ToDateTime(rs.pr.created_at).ToString("dd/MM/yyyy");
                    string created_by_text = CommonFunctions.GetUserFullnamebyUserId(rs.pr.created_by);
                    string show_status_text = ShowStatus.GetPRShowStatus(rs.pr.purchase_requisition_status);
                    models.Add(new PurchaseRequisitionViewModel()
                    {
                        purchase_requisition_id = rs.pr.purchase_requisition_id,
                        purchase_requisition_number = rs.pr.purchase_requisition_number,
                        material_request_id = rs.pr.material_request_id,
                        materail_request_number = rs.mr.ir_no,
                        created_at = rs.pr.updated_at,
                        project_id = rs.mr.ir_project_id,
                        project_fullname = rs.pro.project_full_name,
                        created_by = rs.pr.created_by,
                        purchase_requisition_status = rs.pr.purchase_requisition_status,
                        created_at_text = created_at_text,
                        created_by_text = created_by_text,
                        show_status_text=show_status_text,
                    });
                }

                return models;
            }
        }
        public List<PurchaseRequisitionViewModel> GetPurchaseRequisitionApprovalListItemsByDatetimeRangeAndStatus(string dateRange,string status,bool isIncludeDate=true)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<PurchaseRequisitionViewModel> models = new List<PurchaseRequisitionViewModel>();
                List<PRFilterRequestModel> results = new List<PRFilterRequestModel>();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                if (User.IsInRole(Role.SystemAdmin))
                {
                    if(isIncludeDate)
                        results= (from pr in db.tb_purchase_requisition
                                  join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                  join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                  orderby pr.updated_at descending
                                  where pr.status == true && pr.created_at >= startDate && pr.created_at <= endDate
                                  select new PRFilterRequestModel()
                                  {
                                      pr=pr,
                                      mr=mr,
                                      pro=pro
                                  }).ToList();
                    else
                    {
                        results = (from pr in db.tb_purchase_requisition
                                   join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                   join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                   orderby pr.updated_at descending
                                   where pr.status == true && string.Compare(pr.purchase_requisition_status,status)==0
                                   select new PRFilterRequestModel()
                                   {
                                       pr = pr,
                                       mr = mr,
                                       pro = pro
                                   }).ToList();
                    }
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    if(isIncludeDate)
                    results = (from pr in db.tb_purchase_requisition
                              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                              orderby pr.updated_at descending
                              where pr.status == true && (string.Compare(pr.purchase_requisition_status, Status.Pending) == 0 || string.Compare(pr.purchase_requisition_status,Status.Feedbacked)==0 || string.Compare(pr.approved_by, userid) == 0)
                              && pr.created_at>=startDate && pr.created_at<=endDate
                              select new PRFilterRequestModel()
                              {
                                  pr = pr,
                                  mr = mr,
                                  pro = pro
                              }).ToList();
                    else
                    {
                        results = (from pr in db.tb_purchase_requisition
                                   join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                                   join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                                   orderby pr.updated_at descending
                                   where pr.status == true && (string.Compare(pr.purchase_requisition_status, Status.Pending) == 0  || string.Compare(pr.purchase_requisition_status, Status.Feedbacked) == 0)
                                   
                                   select new PRFilterRequestModel()
                                   {
                                       pr = pr,
                                       mr = mr,
                                       pro = pro
                                   }).ToList();
                    }
                }

                if (string.Compare(status, "0") != 0)
                {
                    results = results.Where(s => string.Compare(s.pr.purchase_requisition_status, status) == 0).ToList();
                }

                foreach (var rs in results)
                {
                    string created_at_text = Convert.ToDateTime(rs.pr.created_at).ToString("dd/MM/yyyy");
                    string created_by_text = CommonFunctions.GetUserFullnamebyUserId(rs.pr.created_by);
                    string show_status_text = ShowStatus.GetPRShowStatus(rs.pr.purchase_requisition_status);
                    models.Add(new PurchaseRequisitionViewModel()
                    {
                        purchase_requisition_id = rs.pr.purchase_requisition_id,
                        purchase_requisition_number = rs.pr.purchase_requisition_number,
                        material_request_id = rs.pr.material_request_id,
                        materail_request_number = rs.mr.ir_no,
                        created_at = rs.pr.updated_at,
                        project_id = rs.mr.ir_project_id,
                        project_fullname = rs.pro.project_full_name,
                        created_by = rs.pr.created_by,
                        purchase_requisition_status = rs.pr.purchase_requisition_status,
                        created_at_text = created_at_text,
                        created_by_text = created_by_text,
                        show_status_text = show_status_text,
                    });
                }

                return models;
            }
        }
        public PurchaseRequisitionViewModel GetPurchaseRequisitionItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                PurchaseRequisitionViewModel model = new PurchaseRequisitionViewModel();
                model= (from pr in db.tb_purchase_requisition
                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                        join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                        where string.Compare(pr.purchase_requisition_id,id)==0
                        select new PurchaseRequisitionViewModel()
                        {
                            purchase_requisition_id = pr.purchase_requisition_id,
                            purchase_requisition_number = pr.purchase_requisition_number,
                            material_request_id = pr.material_request_id,
                            materail_request_number = mr.ir_no,
                            created_at = pr.updated_at,
                            project_id = mr.ir_project_id,
                            project_fullname = pro.project_full_name,
                            created_by = pr.created_by,
                            purchase_requisition_status = pr.purchase_requisition_status,
                            is_quote_complete=pr.is_quote_complete,
                        }).FirstOrDefault();
                model.purchaseRequisitionDetails = (from prd in db.tb_purchase_requisition_detail
                                                    join item in db.tb_product on prd.item_id equals item.product_id
                                                    join unit in db.tb_unit on prd.item_unit equals unit.Id
                                                    orderby item.product_code
                                                    where string.Compare(prd.purchase_requisition_id, model.purchase_requisition_id) == 0
                                                    select new PurchaseRequisitionDetailViewModel()
                                                    {
                                                        purchase_requisition_detail_id=prd.purchase_requisition_detail_id,
                                                        purchase_requisition_id=prd.purchase_requisition_id,
                                                        item_id=prd.item_id,
                                                        item_code=item.product_code,
                                                        item_name=item.product_name,
                                                        item_unit=prd.item_unit,
                                                        item_unit_name=unit.Name,
                                                        approved_qty=prd.approved_qty,
                                                        remain_qty=prd.remain_qty,
                                                        reason=prd.reason,
                                                        remark=prd.remark,
                                                        item_status=prd.item_status,
                                                    }).ToList();
                model.processWorkflow = ProcessWorkflowModel.GetProcessWorkflowByRefId(model.purchase_requisition_id);

                return model;
            }
        }
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
         
    }
}