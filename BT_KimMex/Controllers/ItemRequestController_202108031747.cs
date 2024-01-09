using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using System.Collections;

namespace BT_KimMex.Controllers
{
   // [Authorize(Roles = "Admin,Site Manager,Economic Engineer,Main Stock Controller,Purchaser,Site Admin,Site Supervisor,Technical Department,Project Manager,Procurement")]
    public class ItemRequestController_202108031747 : Controller
    {
        private kim_mexEntities db = new kim_mexEntities();
       
        // GET: ItemRequest
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            BOQ obj = new BOQ();
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            //List<ProductSizeViewModel> productsize = new List<ProductSizeViewModel>();
            var productSize = this.GetProductSizeList();
            projects = obj.GetBOQProjects();
            if (User.IsInRole(Role.SystemAdmin))
                projects = CommonClass.GetAllProject();
            else
                projects = CommonClass.GetAllProject(User.Identity.GetUserId());
            ViewBag.ProjectID = new SelectList(projects, "project_id", "project_full_name");
            //ViewBag.ProductsizeID = new SelectList(productsize, "product_size_id", "product_size_name");
            ViewBag.PurchaseRequisitionNumber = Class.CommonClass.GenerateProcessNumber("MR");
            return View();
        }
        //bora 07.02.21
        [HttpPost]
        public ActionResult Print(string id)
        {
            return Json(new { result = "success", id = id }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MRReport()
        {
            return View();
        }
        public ActionResult Detail(string id)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();
            itemRequest = this.GetItemRequestDetail(id);
            return View(itemRequest);
        }

        public class Result
        {
            public const string Success = "success";
            public const string Error = "error";
        }

        public ActionResult Edit(string id)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();
            List<tb_project> projects = new List<tb_project>();
            List<tb_purpose> purposes = new List<tb_purpose>();
            List<tb_product_category> types = new List<tb_product_category>();
            List<BOQDetail1> jobCategories = new List<BOQDetail1>();
            List<BOQDetail1> jobCategories1 = new List<BOQDetail1>();
            itemRequest = this.GetItemRequestDetail(id);
            projects = GlobalMethod.GetProjectDropdownlist();
            purposes = this.GetPurposeDropdownList();
            types = GlobalMethod.GetItemTypeDropdownlist();
            jobCategories = this.GetJobCategoryDropdownlist(itemRequest.ir_project_id);
            jobCategories1 = this.GetJobCategoryDropdownlist(itemRequest.ir_project_id);

            var jobs = (from irJob in db.tb_ir_detail1
                        join job in db.tb_job_category on irJob.ir_job_category_id equals job.j_category_id
                        where irJob.ir_id == id
                        select new JobCategoryViewModel() { j_category_id=irJob.ir_job_category_id,j_category_name=job.j_category_name }).ToList();
            if (jobs.Any())
            {
                foreach(JobCategoryViewModel job in jobs)
                {
                    var isFound = jobCategories.Where(x => x.job_category_id == job.j_category_id).FirstOrDefault();
                    if (isFound == null)
                        jobCategories.Add(new BOQDetail1() { job_category_id = job.j_category_id, j_category_name = job.j_category_name });
                } 
            }

            ViewBag.ProjectID = new SelectList(projects, "project_id", "project_full_name");
            ViewBag.PurposeID = new SelectList(purposes, "purpose_id", "purpose_name");
            ViewBag.TypeID = new SelectList(types, "p_category_id", "p_category_name");
            //block on jul 20 2018//ViewBag.JobCategoryID = jobCategories; //new SelectList(jobCategories, "job_category_id", "j_category_name");
            ViewBag.JobCategoryID = db.tb_job_category.OrderBy(x=>x.j_category_name).Where(x=>x.j_status==true).Select(x=>new JobCategoryViewModel() {j_category_id=x.j_category_id,j_category_name=x.j_category_name }).ToList();
            return View(itemRequest);
        }
        //[Authorize(Roles = "Admin,Site Manager")]
        public ActionResult Approved(string id)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();
            itemRequest = this.GetItemRequestDetail(id);
            return View(itemRequest);
        }
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_item_request ir = db.tb_item_request.Find(id);
                ir.status = false;
                ir.updated_by = User.Identity.GetUserId();
                ir.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestCancel(string id)
        {
            using (kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_request ir = db.tb_item_request.Find(id);
                ir.ir_status = Status.RequestCancelled;
                ir.updated_by = User.Identity.GetUserId();
                ir.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult CreateItemRequest(ItemRequestViewModel model,List<ItemRequestDetail1ViewModel> ir1,List<ItemRequestDetail2ViewModel> ir2,List<IRTypeViewModel> irType)
        {
            try
            {
                if (ItemRequest.ValidationOnSave(ir2))
                {
                    return Json(new { result = "fail", message = "Duplication item in the same job category." }, JsonRequestBehavior.AllowGet);
                }
                //Save to IR
                tb_item_request ir = new tb_item_request();
                ir.ir_id = Guid.NewGuid().ToString();
                //ir.ir_no = model.ir_no;
                ir.ir_no = Class.CommonClass.GenerateProcessNumber("MR");
                ir.ir_project_id = model.ir_project_id;
                ir.ir_purpose_id = model.ir_purpose_id;
                ir.ir_status = "Pending";
                ir.status = true;
                ir.is_mr = false;
                ir.created_by = User.Identity.GetUserId();
                ir.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.tb_item_request.Add(ir);
                db.SaveChanges();

                tb_ir_detail1 ir_detail1 = new tb_ir_detail1();
                ir_detail1.ir_detail1_id = Guid.NewGuid().ToString();
                ir_detail1.ir_id = ir.ir_id;
                ir_detail1.ir_job_category_id = "1";
                db.tb_ir_detail1.Add(ir_detail1);
                db.SaveChanges();

                if (ir2 != null)
                {
                    tb_ir_detail2 ir_detail2 = new tb_ir_detail2();
                    for (int j = 0; j < ir2.Count(); j++)
                    {
                        ir_detail2 = new tb_ir_detail2();
                        ir_detail2.ir_detail2_id = Guid.NewGuid().ToString();
                        ir_detail2.ir_detail1_id = ir_detail1.ir_detail1_id;
                        ir_detail2.ir_item_id = ir2[j].ir_item_id;
                        ir_detail2.ir_item_unit = ir2[j].product_unit;
                        ir_detail2.ir_qty = ir2[j].ir_qty;
                        ir_detail2.remark = ir2[j].remark;
                        ir_detail2.ordering_number = ir2[j].ordering_number;
                        db.tb_ir_detail2.Add(ir_detail2);
                        db.SaveChanges();
                    }
                }

                //if (ir1 != null)
                //{
                //    tb_ir_detail1 ir_detail1 = new tb_ir_detail1();
                //    for (int i = 0; i < ir1.Count(); i++)
                //    {
                //        ir_detail1 = new tb_ir_detail1();
                //        ir_detail1.ir_detail1_id = Guid.NewGuid().ToString();
                //        ir_detail1.ir_id = ir.ir_id;
                //        ir_detail1.ir_job_category_id =ItemRequest.checkJobCategory(ir1[i].ir_job_category_id);
                //        db.tb_ir_detail1.Add(ir_detail1);
                //        db.SaveChanges();
                //        if (ir2 != null)
                //        {
                //            tb_ir_detail2 ir_detail2 = new tb_ir_detail2();
                //            for(int j = 0; j < ir2.Count(); j++)
                //            {
                //                if (ir1[i].job_group == ir2[j].job_group)
                //                {
                //                    ir_detail2 = new tb_ir_detail2();
                //                    ir_detail2.ir_detail2_id = Guid.NewGuid().ToString();
                //                    ir_detail2.ir_detail1_id = ir_detail1.ir_detail1_id;
                //                    ir_detail2.ir_item_id = ir2[j].ir_item_id;
                //                    ir_detail2.ir_item_unit = ir2[j].product_unit;
                //                    ir_detail2.ir_qty = ir2[j].ir_qty;
                //                    ir_detail2.remark = ir2[j].remark;
                //                    /* closed on jul 20 2018
                //                    if (!this.isItemInBOQ(ir.ir_project_id,ir_detail1.ir_job_category_id,ir_detail2.ir_item_id))
                //                    {
                //                        ir_detail2.remark = "Not in BOQ";
                //                    }
                //                    */
                //                    db.tb_ir_detail2.Add(ir_detail2);
                //                    db.SaveChanges();
                //                }
                //            }
                //        }
                //    }
                //}

                if (model.att_id != null)
                {
                    tb_ir_attachment att = db.tb_ir_attachment.Where(m => m.ir_attachment_id == model.att_id).FirstOrDefault();
                    att.ir_id = ir.ir_id;
                    db.SaveChanges();
                }
                //closed on jul 20 2018
                //ItemRequest ir_obj = new ItemRequest();
                //ir_obj.addItemBOQ(ir.ir_project_id, ir1, ir2, irType);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditItemRequest(ItemRequestViewModel model, List<ItemRequestDetail1ViewModel> ir1, List<ItemRequestDetail2ViewModel> ir2, List<IRTypeViewModel> irType)
        {
            try
            {
                if (ItemRequest.ValidationOnSave(ir1, ir2))
                {
                    return Json(new { result = "fail", message = "Duplication item in the same job category." }, JsonRequestBehavior.AllowGet);
                }
                this.DeleteItemRequestDetail(model.ir_id);
                tb_item_request itemRequest = db.tb_item_request.Where(m => m.ir_id == model.ir_id).FirstOrDefault();
                itemRequest.ir_no = model.ir_no;
                itemRequest.ir_project_id = model.ir_project_id;
                itemRequest.ir_purpose_id = model.ir_purpose_id;
                itemRequest.ir_status = "Pending";
                itemRequest.status = true;
                itemRequest.updated_by = User.Identity.GetUserId();
                itemRequest.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                if (ir1 != null)
                {
                    tb_ir_detail1 ir_detail1 = new tb_ir_detail1();
                    for (int i = 0; i < ir1.Count(); i++)
                    {
                        ir_detail1 = new tb_ir_detail1();
                        ir_detail1.ir_detail1_id = Guid.NewGuid().ToString();
                        ir_detail1.ir_id = itemRequest.ir_id;
                        ir_detail1.ir_job_category_id = ItemRequest.checkJobCategory(ir1[i].ir_job_category_id);
                        db.tb_ir_detail1.Add(ir_detail1);
                        db.SaveChanges();
                        if (ir2 != null)
                        {
                            tb_ir_detail2 ir_detail2 = new tb_ir_detail2();
                            for (int j = 0; j < ir2.Count(); j++)
                            {
                                if (ir1[i].job_group == ir2[j].job_group)
                                {
                                    ir_detail2 = new tb_ir_detail2();
                                    ir_detail2.ir_detail2_id = Guid.NewGuid().ToString();
                                    ir_detail2.ir_detail1_id = ir_detail1.ir_detail1_id;
                                    ir_detail2.ir_item_id = ir2[j].ir_item_id;
                                    ir_detail2.ir_item_unit = ir2[j].product_unit;
                                    ir_detail2.ir_qty = ir2[j].ir_qty;
                                    ir_detail2.remark = ir2[j].remark;
                                    //closed 
                                    //if (!this.isItemInBOQ(itemRequest.ir_project_id, ir_detail1.ir_job_category_id, ir_detail2.ir_item_id))
                                    //{
                                    //    ir_detail2.remark = "Not in BOQ";
                                    //}
                                    //end 
                                    db.tb_ir_detail2.Add(ir_detail2);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }

                if (model.att_id != null)
                {
                    tb_ir_attachment att = db.tb_ir_attachment.Where(m => m.ir_attachment_id == model.att_id).FirstOrDefault();
                    att.ir_id = itemRequest.ir_id;
                    db.SaveChanges();
                }

                //ItemRequest ir_obj = new ItemRequest();
                //ir_obj.addItemBOQ(itemRequest.ir_project_id, ir1, ir2, irType);

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private void DeleteItemRequestDetail(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<string> dIr1 = new List<string>();
                List<string> ddIr2 = new List<string>();
                var dIrs = db.tb_ir_detail1.Where(m => m.ir_id == id).ToList();
                if (dIrs != null)
                {
                    foreach(var dir in dIrs)
                    {
                        dIr1.Add(dir.ir_detail1_id);
                    }
                }
                for(int i = 0; i < dIr1.Count(); i++)
                {
                    var ir_detail1_id = Convert.ToString(dIr1[i]);
                    var ddIrs = db.tb_ir_detail2.Where(m => m.ir_detail1_id == ir_detail1_id).ToList();
                    if (ddIrs != null)
                    {
                        foreach(var ddIr  in ddIrs)
                        {
                            ddIr2.Add(ddIr.ir_detail2_id);
                        }
                    }
                }
                if (ddIr2.Count > 0)
                {
                    for(int i = 0; i < ddIr2.Count(); i++)
                    {
                        string ird = ddIr2[i];
                        tb_ir_detail2 ir = db.tb_ir_detail2.Find(ird);
                        db.tb_ir_detail2.Remove(ir);
                        db.SaveChanges();
                    }
                }
                if (dIr1.Count() > 0)
                {
                    for(int i = 0; i < dIr1.Count(); i++)
                    {
                        string ird = dIr1[i];
                        tb_ir_detail1 ir = db.tb_ir_detail1.Find(ird);
                        db.tb_ir_detail1.Remove(ir);
                        db.SaveChanges(); 
                    }
                }
            }
        }
        //[Authorize(Roles = "Admin,Site Manager")]
        public ActionResult ApprovedItemRequest(string id,List<ItemRequestDetail2ViewModel> itemDetails)
        {
            try
            {
                tb_item_request itemRequest = db.tb_item_request.Find(id);
                itemRequest.ir_status = "Approved";
                itemRequest.approved_by = User.Identity.GetUserId();
                itemRequest.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                foreach(ItemRequestDetail2ViewModel itemDetail in itemDetails)
                {
                    string itemDetailId = itemDetail.ir_detail2_id;
                    tb_ir_detail2 irdd = db.tb_ir_detail2.Find(itemDetailId);
                    irdd.is_approved = itemDetail.is_approved;
                    irdd.approved_qty = itemDetail.approved_qty;
                    irdd.remain_qty = itemDetail.approved_qty;
                    irdd.reason = itemDetail.reason;
                    db.SaveChanges();
                }
                //ItemRequest obj = new ItemRequest();
                //obj.addItemBOQ(itemRequest.ir_id);
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UploadAttachment()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_ir_attachment att = new tb_ir_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/IR Attachment/"), file_id + file_extension);
                    file.SaveAs(file_path);
                    att.ir_attachment_id = file_id;
                    att.ir_attachment_name = file_name;
                    att.ir_attachment_extension = file_extension;
                    att.file_path = file_path;
                    db.tb_ir_attachment.Add(att);
                    db.SaveChanges();    
                }
                return Json(new { result = "success", attachment_id = att.ir_attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult Download(String p,String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/IR Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }
        [HttpPost]
        public JsonResult DeleteAttachment(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { result = "error" });
            }
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_ir_attachment att = db.tb_ir_attachment.Find(id);
                if (att == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_ir_attachment.Remove(att);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/IR Attachment/"), att.ir_attachment_id + att.ir_attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        public ActionResult Approve(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_item_request po = db.tb_item_request.Find(id);
                po.ir_status = "Approved";
                po.approved_by = User.Identity.GetUserId();
                po.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Reject(string id,string comment)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_item_request ir = db.tb_item_request.Find(id);
                ir.ir_status = "Rejected";
                ir.approved_by = User.Identity.GetUserId();
                ir.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                tb_reject reject = new tb_reject();
                reject.reject_id = Guid.NewGuid().ToString();
                reject.ref_id = ir.ir_id;
                reject.ref_type = "Item Request";
                reject.comment = comment;
                reject.rejected_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                reject.rejected_by = User.Identity.GetUserId();
                db.tb_reject.Add(reject);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ItemRequestDataTable(string ir_status,string role)
        {
            //DateTime mDate = date.AddDays(1);
            List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();
            try
            {
                IQueryable<ItemRequestViewModel> irs_list;
                if (role != null)
                {
                    if (ir_status == "All")
                    {
                        irs_list = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp from purpose in pp.DefaultIfEmpty()
                                    orderby tbl.ir_no
                                    where tbl.status == true && tbl.created_by==role
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date
                                    }
                                      );
                    }
                    else
                    {
                        irs_list = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    orderby tbl.ir_no
                                    where tbl.status == true && tbl.ir_status == ir_status && tbl.created_by == role
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date
                                    }
                                      );
                    }
                }
                else
                {
                    if (ir_status == "All")
                    {
                        irs_list = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    orderby tbl.ir_no
                                    where tbl.status == true
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date
                                    }
                                      );
                    }
                    else
                    {
                        irs_list = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    orderby tbl.ir_no
                                    where tbl.status == true && tbl.ir_status == ir_status
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date
                                    }
                                      );
                    }
                }
                
                if (irs_list.Any())
                {
                    foreach(var ir in irs_list)
                    {
                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
                        item_requests.Add(new ItemRequestViewModel()
                        {
                            ir_id = ir.ir_id,
                            ir_no = ir.ir_no,
                            ir_project_id = ir.ir_project_id,
                            ir_purpose_id = ir.ir_purpose_id,
                            ir_status = ir.ir_status,
                            created_date = ir.created_date,
                            project_full_name = ir.project_full_name,
                            project_short_name = ir.project_short_name,
                            purpose_description = ir.purpose_description,
                            isCompleted=isCompleted,
                        });
                    }
                }
                return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex) {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ItemRequestController.cs", "ItemRequestDataTable", ex.StackTrace, ex.Message);
                return null;
            }
        }
        public ActionResult GetItemRequestNo()
        {
            try
            {
                string last_no = "", ir_no;
                var number = (from tbl in db.tb_item_request orderby tbl.created_date descending select tbl.ir_no).FirstOrDefault();
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
                //string dd = Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString();
                string yy = Class.CommonClass.ToLocalTime(DateTime.Now).Year.ToString().Substring(2, 2);
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                ir_no = "PR-" + yy + "-" + mm + "-" + last_no;
                return Json(new { data = ir_no }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return null; }
        }
        public ActionResult GetIRPurposeDropdownList()
        {
            try
            {
                List<ItemRequestViewModel> purposes = new List<ItemRequestViewModel>();
                var purpose_list = (from tbl in db.tb_purpose orderby tbl.purpose_name select tbl).ToList();
                if (purpose_list.Any())
                {
                    foreach(var purpose in purpose_list)
                    {
                        purposes.Add(new ItemRequestViewModel() { ir_purpose_id = purpose.purpose_id, purpose_description = purpose.purpose_name });
                    }
                }
                return Json(new { result = "success", data = purposes }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex) {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBOQJobCategoryDropdownlist(string id)
        {
            try
            {
                /*
                List<BOQDetail1> job_categories = new List<BOQDetail1>();
                var job_category_list = (from tbl in db.tb_build_of_quantity
                                         join boq_detail1 in db.tb_boq_detail1 on tbl.boq_id equals boq_detail1.boq_id
                                         join j_cat in db.tb_job_category on boq_detail1.job_category_id equals j_cat.j_category_id
                                         orderby tbl.project_id
                                         where tbl.project_id == id && tbl.boq_status == "Completed" && tbl.status==true
                                         select new BOQDetail1()
                                         {
                                             job_category_id = boq_detail1.job_category_id,
                                             j_category_name = j_cat.j_category_name,
                                             boq_id=tbl.boq_id,
                                         }
                                       ).ToList();
                
                if (job_category_list.Any())
                {
                    foreach(var job_category in job_category_list)
                    {
                        job_categories.Add(new BOQDetail1() {boq_id=job_category.boq_id, job_category_id = job_category.job_category_id, j_category_name = job_category.j_category_name });
                    }
                    
                }
                */
                List<JobCategoryViewModel> job_categories = new List<JobCategoryViewModel>();
                job_categories = db.tb_job_category.OrderBy(x => x.j_category_name).Where(x => x.j_status == true).Select(x => new JobCategoryViewModel() { j_category_id = x.j_category_id, j_category_name = x.j_category_name }).ToList();
                return Json(new { result = "success", data = job_categories }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetItemRequestedJobCategoryDropdownlist(string id,string ir_id)
        {
            try
            {
                List<BOQDetail1> jobCategories = new List<BOQDetail1>();
                List<BOQDetail1> jobCategories1 = new List<BOQDetail1>();

                jobCategories = this.GetJobCategoryDropdownlist(id);
                jobCategories1 = this.GetJobCategoryDropdownlist(id);

                var jobs = (from irJob in db.tb_ir_detail1
                            join job in db.tb_job_category on irJob.ir_job_category_id equals job.j_category_id
                            where irJob.ir_id == ir_id
                            select new JobCategoryViewModel() { j_category_id = irJob.ir_job_category_id, j_category_name = job.j_category_name }).ToList();
                if (jobs.Any())
                {
                    foreach (JobCategoryViewModel job in jobs)
                    {
                        var isFound = jobCategories.Where(x => x.job_category_id == job.j_category_id).FirstOrDefault();
                        if (isFound == null)
                            jobCategories.Add(new BOQDetail1() { job_category_id = job.j_category_id, j_category_name = job.j_category_name });
                    }
                }

                return Json(new { result = "success", data = jobCategories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ItemRequestViewModel GetItemRequestDetail(string id)
        {
            ItemRequestViewModel itemRequest = new ItemRequestViewModel();

            //itemRequest = (from i in db.tb_item_request
            //               join pj in db.tb_project on i.ir_project_id equals pj.project_id
            //               join purpose in db.tb_purpose on i.ir_purpose_id equals purpose.purpose_id into pp from purpose in pp.DefaultIfEmpty()
            //               where i.status == true && i.ir_id == id
            //               select new ItemRequestViewModel()
            //               {
            //                   ir_id = i.ir_id,
            //                   ir_no = i.ir_no,
            //                   ir_project_id = i.ir_project_id,
            //                   project_short_name = pj.project_short_name,
            //                   project_full_name = pj.project_full_name,
            //                   ir_purpose_id = i.ir_purpose_id,
            //                   purpose_description = purpose.purpose_name,
            //                   ir_status = i.ir_status,
            //                   created_date = i.created_date,
            //                   created_by=CommonClass.GetUserFullname(i.created_by),
            //                   updated_by=i.updated_by,
            //                   checked_by=i.checked_by,
            //                   approved_by=i.approved_by,
            //               }).FirstOrDefault();
            var request= (from i in db.tb_item_request
                           join pj in db.tb_project on i.ir_project_id equals pj.project_id
                           join purpose in db.tb_purpose on i.ir_purpose_id equals purpose.purpose_id into pp
                           from purpose in pp.DefaultIfEmpty()
                           where i.status == true && i.ir_id == id
                           select new {i,pj,purpose}).FirstOrDefault();
            if (request!=null){
                itemRequest = new ItemRequestViewModel();
                itemRequest.ir_id = request.i.ir_id;
                itemRequest.ir_no = request.i.ir_no;
                itemRequest.ir_project_id = request.i.ir_project_id;
                itemRequest.project_full_name = request.pj.project_full_name;
                itemRequest.ir_purpose_id = request.i.ir_purpose_id;
                itemRequest.purpose_description = request.purpose==null?string.Empty:request.purpose.purpose_name;
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
                int dCount = 1;
                foreach (var dIr in dIrs)
                {

                    List<ItemRequestDetail2ViewModel> ir2 = new List<ItemRequestDetail2ViewModel>();
                    var ddIrs = (from ir in db.tb_ir_detail2
                                 join it in db.tb_product on ir.ir_item_id equals it.product_id
                                 join u in db.tb_unit on it.product_unit equals u.Id
                                 orderby ir.ordering_number,it.product_code
                                 where ir.ir_detail1_id == dIr.ir_detail1_id
                                 select new
                                 {
                                     ir_detail2_id = ir.ir_detail2_id,
                                     ir_detail1_id = ir.ir_detail1_id,
                                     ir_item_id = ir.ir_item_id,
                                     product_code = it.product_code,
                                     product_name = it.product_name,
                                     product_unit = it.product_unit,
                                     ir_qty = ir.ir_qty,
                                     requested_unit = ir.ir_item_unit,
                                     is_approved =ir.is_approved,
                                     approved_qty=ir.approved_qty,
                                     remark=ir.remark,
                                     reason=ir.reason,
                                     unit_name=u.Name,
                                     ir_item_unit=ir.ir_item_unit
                                 }).ToList();
                    if (ddIrs.Any())
                    {
                        foreach (var ddIr in ddIrs)
                        {
                            ItemRequestDetail2ViewModel irItem = new ItemRequestDetail2ViewModel();
                            irItem.ir_detail2_id = ddIr.ir_detail2_id;
                            irItem.ir_detail1_id = ddIr.ir_detail1_id;
                            irItem.ir_item_id = ddIr.ir_item_id;
                            irItem.ir_item_unit = ddIr.ir_item_unit;
                            irItem.product_code = ddIr.product_code;
                            irItem.product_name = ddIr.product_name;
                            irItem.unit_id = ddIr.product_unit;
                            irItem.product_unit = Regex.Replace(ddIr.unit_name.Trim(),@"\t|\n|\r","");
                            //irItem.product_unit =​Regex.Replace(ddIr.product_unit.Trim(), @"\t|\n|\r", "");
                            irItem.ir_qty = ddIr.ir_qty;
                            //irItem.requested_unit =​Regex.Replace(ddIr.requested_unit.Trim(), @"\t|\n|\r", ""); 
                            //irItem.requested_unit = Regex.Replace(ddIr.requested_unit.Trim(), @"\t|\n|\r", "");
                            irItem.requested_unit_id = ddIr.requested_unit;
                            irItem.requested_unit = db.tb_unit.Where(w => string.Compare(w.Id, ddIr.requested_unit) == 0).Select(s => s.Name).FirstOrDefault();
                            irItem.remark = ddIr.remark;
                            irItem.reason = ddIr.reason;
                            irItem.is_approved = ddIr.is_approved;
                            irItem.approved_qty = ddIr.approved_qty;
                            irItem.boq_qty = ItemRequest.GetBoqItemQty(itemRequest.ir_project_id, dIr.ir_job_category_id, ddIr.ir_item_id);
                            irItem.job_group = dCount.ToString();
                            irItem.uom = db.tb_multiple_uom.Where(x => x.product_id == irItem.ir_item_id).Select(x=>new ProductViewModel() {uom1_id=x.uom1_id,uom1_qty=x.uom1_qty, uom2_id = x.uom2_id, uom2_qty = x.uom2_qty, uom3_id = x.uom3_id, uom3_qty = x.uom3_qty, uom4_id = x.uom4_id, uom4_qty = x.uom4_qty, uom5_id = x.uom5_id, uom5_qty = x.uom5_qty }).FirstOrDefault();
                            ir2.Add(irItem);
                        }
                    }
                    ir1.Add(new ItemRequestDetail1ViewModel() { ir_detail1_id = dIr.ir_detail1_id, ir_id = dIr.ir_id, ir_job_category_id = dIr.ir_job_category_id, job_category_description = dIr.job_category_description, job_group = dCount.ToString(), ir2 = ir2 });
                    dCount++;
                }
            }
            itemRequest.ir1 = ir1;
            var att = db.tb_ir_attachment.Where(m => m.ir_id == id).FirstOrDefault();
            if (att != null)
            {
                itemRequest.att_id = att.ir_attachment_id;
                itemRequest.ir_attachment_name = att.ir_attachment_name;
                itemRequest.ir_attachment_extension = att.ir_attachment_extension;
                itemRequest.file_path = att.file_path;
            }
            itemRequest.rejects = CommonClass.GetRejectByRequest(id);
            itemRequest.boq_id = ItemRequest.GetBOQId(itemRequest.ir_project_id);
            return itemRequest;
        }
        public List<tb_purpose> GetPurposeDropdownList()
        {
            List<tb_purpose> purposes = new List<tb_purpose>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                purposes = db.tb_purpose.OrderBy(m => m.purpose_name).ToList();
            }
            return purposes;
        }
        public List<BOQDetail1> GetJobCategoryDropdownlist(string id)
        {
            List<BOQDetail1> job_categories = new List<BOQDetail1>();
            var job_category_list = (from tbl in db.tb_build_of_quantity
                                     join boq_detail1 in db.tb_boq_detail1 on tbl.boq_id equals boq_detail1.boq_id
                                     join j_cat in db.tb_job_category on boq_detail1.job_category_id equals j_cat.j_category_id
                                     orderby tbl.project_id
                                     where tbl.project_id == id && tbl.boq_status == "Completed" && tbl.status == true
                                     select new BOQDetail1()
                                     {
                                         job_category_id = boq_detail1.job_category_id,
                                         j_category_name = j_cat.j_category_name,
                                         boq_id = tbl.boq_id,
                                     }
                                   ).ToList();
            if (job_category_list.Any())
            {
                foreach (var job_category in job_category_list)
                {
                    job_categories.Add(new BOQDetail1() { boq_id = job_category.boq_id, job_category_id = job_category.job_category_id, j_category_name = job_category.j_category_name });
                }

            }
            return job_categories;
        }
        
        public bool isItemInBOQ(string projectId,string jobCategoryId,string itemId)
        {
            bool isFound;
            string boqId = ItemRequest.GetBOQId(projectId);
            var items = (from tbl in db.tb_boq_detail3
                         join item in db.tb_product on tbl.item_id equals item.product_id
                         join boq2 in db.tb_boq_detail2 on tbl.boq_detail2_id equals boq2.boq_detail2_id
                         join boq1 in db.tb_boq_detail1 on boq2.boq_detail1_id equals boq1.boq_detail1_id
                         where boq1.boq_id == boqId && boq1.job_category_id == jobCategoryId && item.product_id == itemId
                         select new
                         {
                             item_id = tbl.item_id,
                             product_name = item.product_name,
                             product_code = item.product_code,
                             product_unit = item.product_unit,
                             item_qty = tbl.item_qty
                         }).FirstOrDefault();
            if (items == null)
                isFound = false;
            else
                isFound = true;
            return isFound;
        }
        #region added by Borith Jan 02 2020
        public ActionResult NewRequest()
        {
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            //projects = CommonClass.GetAllProject();
            //projects = obj.GetBOQProjects();
            if (User.IsInRole(Role.SystemAdmin))
                projects = CommonClass.GetAllProject();
            else
                projects = CommonClass.GetAllProject(User.Identity.GetUserId());
            ViewBag.ProjectID = new SelectList(projects, "project_id", "project_full_name");
            ViewBag.PurchaseRequisitionNumber = Class.CommonClass.GenerateProcessNumber("MR");
            return View();
        }
        public ActionResult MyRequest()
        {
            return View();
        }
        public ActionResult GetMyRequestJson()
        {
            List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();
            using (kim_mexEntities db=new kim_mexEntities())
            {
                string userid = User.Identity.GetUserId();
                var models= (from tbl in db.tb_item_request
                            join project in db.tb_project on tbl.ir_project_id equals project.project_id
                            join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                            from purpose in pp.DefaultIfEmpty()
                            orderby tbl.ir_no
                            where tbl.status == true && string.Compare(tbl.created_by,userid)==0
                            select new ItemRequestViewModel
                            {
                                ir_id = tbl.ir_id,
                                ir_no = tbl.ir_no,
                                ir_project_id = tbl.ir_project_id,
                                project_full_name = project.project_full_name,
                                project_short_name = project.project_short_name,
                                ir_purpose_id = tbl.ir_purpose_id,
                                purpose_description = purpose.purpose_name,
                                ir_status = tbl.ir_status,
                                created_date = tbl.created_date
                            });
                if (models.Any())
                {
                    foreach (var ir in models)
                    {
                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
                        item_requests.Add(new ItemRequestViewModel()
                        {
                            ir_id = ir.ir_id,
                            ir_no = ir.ir_no,
                            ir_project_id = ir.ir_project_id,
                            ir_purpose_id = ir.ir_purpose_id,
                            ir_status = ir.ir_status,
                            created_date = ir.created_date,
                            project_full_name = ir.project_full_name,
                            project_short_name = ir.project_short_name,
                            purpose_description = ir.purpose_description,
                            isCompleted = isCompleted,
                        });
                    }
                }
                return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
            }
            
        }
        public JsonResult PermissionUserApproves(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var uid = User.Identity.GetUserId();
                //var user = (from us in db.AspNetUsers
                //            where us.Id == uid
                //            select us
                //            ).FirstOrDefault();
                var ppro = (from smpr in db.tb_site_manager_project
                            join proj in db.tb_project on smpr.project_id equals proj.project_id
                            join itemre in db.tb_item_request on proj.project_id equals itemre.ir_project_id
                            where smpr.site_manager == uid && smpr.project_id == proj.project_id && itemre.ir_id == id
                            select smpr).ToList();

                if (ppro.Count() == 0)
                {
                    return Json(new { result = Result.Error, data = "nopermission" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var k in ppro)
                    {
                        return Json(new { result = Result.Success, data = k.site_manager_project_id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result = Result.Success }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetMyApprovalRequestJson()
        //{
        //    List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();

        //    ApplicationDbContext context = new ApplicationDbContext();
        //    var users = db.AspNetUsers.OrderBy(x => x.UserName).ToList();

        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        string userid = User.Identity.GetUserId();
        //        var models1 = (from tbl in db.tb_item_request
        //                       join project in db.tb_project on tbl.ir_project_id equals project.project_id
        //                       join smproject in db.tb_site_manager_project on tbl.ir_project_id equals smproject.project_id
        //                       join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
        //                       from purpose in pp.DefaultIfEmpty()
        //                       orderby tbl.ir_no
        //                       where tbl.status == true && userid == smproject.site_manager && tbl.ir_project_id == project.project_id && project.project_id == smproject.project_id
        //                       select new ItemRequestViewModel
        //                       {
        //                           ir_id = tbl.ir_id,
        //                           ir_no = tbl.ir_no,
        //                           ir_project_id = tbl.ir_project_id,
        //                           project_full_name = project.project_full_name,
        //                           project_short_name = project.project_short_name,
        //                           ir_purpose_id = tbl.ir_purpose_id,
        //                           purpose_description = purpose.purpose_name,
        //                           ir_status = tbl.ir_status,
        //                           created_date = tbl.created_date,
        //                           //rolename = roless.Name,
        //                           status = tbl.status.ToString(),
        //                       }).ToList();

        //        if (models1.Any())
        //        {
        //            foreach (var ir in models1)
        //            {
        //                bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
        //                item_requests.Add(new ItemRequestViewModel()
        //                {
        //                    ir_id = ir.ir_id,
        //                    ir_no = ir.ir_no,
        //                    ir_project_id = ir.ir_project_id,
        //                    ir_purpose_id = ir.ir_purpose_id,
        //                    ir_status = ir.ir_status,
        //                    created_date = ir.created_date,
        //                    project_full_name = ir.project_full_name,
        //                    project_short_name = ir.project_short_name,
        //                    purpose_description = ir.purpose_description,
        //                    isCompleted = isCompleted,
        //                    status = "havepermission",
        //                });
        //            }
        //        }
        //        if (models1.Count == 0)
        //        {
        //            var models = (from tbl in db.tb_item_request
        //                          join project in db.tb_project on tbl.ir_project_id equals project.project_id
        //                          join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
        //                          from purpose in pp.DefaultIfEmpty()
        //                          orderby tbl.ir_no
        //                          where tbl.status == true
        //                          select new ItemRequestViewModel
        //                          {
        //                              ir_id = tbl.ir_id,
        //                              ir_no = tbl.ir_no,
        //                              ir_project_id = tbl.ir_project_id,
        //                              project_full_name = project.project_full_name,
        //                              project_short_name = project.project_short_name,
        //                              ir_purpose_id = tbl.ir_purpose_id,
        //                              purpose_description = purpose.purpose_name,
        //                              ir_status = tbl.ir_status,
        //                              created_date = tbl.created_date
        //                          }).ToList();

        //            if (models.Any())
        //            {
        //                foreach (var ir in models)
        //                {
        //                    bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
        //                    item_requests.Add(new ItemRequestViewModel()
        //                    {
        //                        ir_id = ir.ir_id,
        //                        ir_no = ir.ir_no,
        //                        ir_project_id = ir.ir_project_id,
        //                        ir_purpose_id = ir.ir_purpose_id,
        //                        ir_status = ir.ir_status,
        //                        created_date = ir.created_date,
        //                        project_full_name = ir.project_full_name,
        //                        project_short_name = ir.project_short_name,
        //                        purpose_description = ir.purpose_description,
        //                        isCompleted = isCompleted,
        //                    });
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var m1 in models1.Take(1))
        //            {
        //                var models = (from tbl in db.tb_item_request
        //                              join project in db.tb_project on tbl.ir_project_id equals project.project_id
        //                              join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
        //                              from purpose in pp.DefaultIfEmpty()
        //                              orderby tbl.ir_no
        //                              where tbl.status == true && tbl.ir_project_id != m1.ir_project_id
        //                              select new ItemRequestViewModel
        //                              {
        //                                  ir_id = tbl.ir_id,
        //                                  ir_no = tbl.ir_no,
        //                                  ir_project_id = tbl.ir_project_id,
        //                                  project_full_name = project.project_full_name,
        //                                  project_short_name = project.project_short_name,
        //                                  ir_purpose_id = tbl.ir_purpose_id,
        //                                  purpose_description = purpose.purpose_name,
        //                                  ir_status = tbl.ir_status,
        //                                  created_date = tbl.created_date
        //                              }).ToList();

        //                if (models.Any())
        //                {
        //                    foreach (var ir in models)
        //                    {
        //                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
        //                        item_requests.Add(new ItemRequestViewModel()
        //                        {
        //                            ir_id = ir.ir_id,
        //                            ir_no = ir.ir_no,
        //                            ir_project_id = ir.ir_project_id,
        //                            ir_purpose_id = ir.ir_purpose_id,
        //                            ir_status = ir.ir_status,
        //                            created_date = ir.created_date,
        //                            project_full_name = ir.project_full_name,
        //                            project_short_name = ir.project_short_name,
        //                            purpose_description = ir.purpose_description,
        //                            isCompleted = isCompleted,
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult GetMyApprovalListItemsJSON()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();
                if (User.IsInRole(Role.SystemAdmin))
                {
                    item_requests= (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    orderby tbl.ir_no
                                    where tbl.status == true
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date
                                    }).ToList();
                }
                else
                {
                    string userid = User.Identity.GetUserId().ToString();
                    item_requests = (from tbl in db.tb_item_request
                                   join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                   join smproject in db.tb_site_manager_project on project.project_id equals smproject.project_id
                                   join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                   from purpose in pp.DefaultIfEmpty()
                                   orderby tbl.ir_no
                                   where tbl.status == true && ( string.Compare(smproject.site_manager,userid)==0 || string.Compare(tbl.approved_by,userid)==0)//&& tbl.ir_project_id == project.project_id && project.project_id == smproject.project_id
                                   select new ItemRequestViewModel
                                   {
                                       ir_id = tbl.ir_id,
                                       ir_no = tbl.ir_no,
                                       ir_project_id = tbl.ir_project_id,
                                       project_full_name = project.project_full_name,
                                       project_short_name = project.project_short_name,
                                       ir_purpose_id = tbl.ir_purpose_id,
                                       purpose_description = purpose.purpose_name,
                                       ir_status = tbl.ir_status,
                                       created_date = tbl.created_date,
                                       //rolename = roless.Name,
                                       status = tbl.status.ToString(),
                                   }).ToList();
                }



                return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMyApprovalRequestJosn()
        {
            List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();

            ApplicationDbContext context = new ApplicationDbContext();
            var users = db.AspNetUsers.OrderBy(x => x.UserName).ToList();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                string userid = User.Identity.GetUserId();

                var models = (from tbl in db.tb_item_request
                              join project in db.tb_project on tbl.ir_project_id equals project.project_id
                              join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                              from purpose in pp.DefaultIfEmpty()
                              orderby tbl.ir_no
                              where tbl.status == true
                              select new ItemRequestViewModel
                              {
                                  ir_id = tbl.ir_id,
                                  ir_no = tbl.ir_no,
                                  ir_project_id = tbl.ir_project_id,
                                  project_full_name = project.project_full_name,
                                  project_short_name = project.project_short_name,
                                  ir_purpose_id = tbl.ir_purpose_id,
                                  purpose_description = purpose.purpose_name,
                                  ir_status = tbl.ir_status,
                                  created_date = tbl.created_date
                              }).ToList();

                if (models.Any())
                {
                    foreach (var ir in models)
                    {
                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
                        item_requests.Add(new ItemRequestViewModel()
                        {
                            ir_id = ir.ir_id,
                            ir_no = ir.ir_no,
                            ir_project_id = ir.ir_project_id,
                            ir_purpose_id = ir.ir_purpose_id,
                            ir_status = ir.ir_status,
                            created_date = ir.created_date,
                            project_full_name = ir.project_full_name,
                            project_short_name = ir.project_short_name,
                            purpose_description = ir.purpose_description,
                            isCompleted = isCompleted,
                        });
                    }
                }

                return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMyApprovalRequestJosnAdmin()
        {
            List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string userid = User.Identity.GetUserId();
                var models = (from tbl in db.tb_item_request
                              join project in db.tb_project on tbl.ir_project_id equals project.project_id
                              join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                              from purpose in pp.DefaultIfEmpty()
                              orderby tbl.ir_no
                              where tbl.status == true
                              select new ItemRequestViewModel
                              {
                                  ir_id = tbl.ir_id,
                                  ir_no = tbl.ir_no,
                                  ir_project_id = tbl.ir_project_id,
                                  project_full_name = project.project_full_name,
                                  project_short_name = project.project_short_name,
                                  ir_purpose_id = tbl.ir_purpose_id,
                                  purpose_description = purpose.purpose_name,
                                  ir_status = tbl.ir_status,
                                  created_date = tbl.created_date
                              });
                if (models.Any())
                {
                    foreach (var ir in models)
                    {
                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
                        item_requests.Add(new ItemRequestViewModel()
                        {
                            ir_id = ir.ir_id,
                            ir_no = ir.ir_no,
                            ir_project_id = ir.ir_project_id,
                            ir_purpose_id = ir.ir_purpose_id,
                            ir_status = ir.ir_status,
                            created_date = ir.created_date,
                            project_full_name = ir.project_full_name,
                            project_short_name = ir.project_short_name,
                            purpose_description = ir.purpose_description,
                            isCompleted = isCompleted,
                        });
                    }
                }
                return Json(new { data = item_requests }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMaterialRequestNumberbyPOJson(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var item = (from mr in db.tb_item_request
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            //join site in db.tb_site on pro.site_id equals site.site_id
                            //join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into pwh from wh in pwh.DefaultIfEmpty()
                            join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                          join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                          join po in db.tb_purchase_request on quote.purchase_order_id equals po.purchase_order_id
                          where string.Compare(po.pruchase_request_id, id) == 0
                          select new { mr_number = mr.ir_no, project_fullanme = pro.project_full_name, warehouse_id = wh.warehouse_id, warehouse_name = wh.warehouse_name }).FirstOrDefault();
                return Json(new { result = "success", data = item }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMaterialRequestNumberbyStockTransferJson(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var item = (from mr in db.tb_item_request
                            join st in db.tb_stock_transfer_voucher on mr.ir_id equals st.item_request_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            //join site in db.tb_site on pro.site_id equals site.site_id
                            //join wh in db.tb_warehouse on site.site_id equals wh.warehouse_site_id
                            join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id into whp from wh in whp.DefaultIfEmpty()
                            where string.Compare(st.stock_transfer_id, id) == 0
                            select new { mr_number=mr.ir_no,project_fullanme=pro.project_full_name,warehouse_id=wh.warehouse_id,warehouse_name=wh.warehouse_name }).FirstOrDefault();
                return Json(new { result = "success", data = item }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RequestedList()
        {
            List<ItemRequestViewModel> item_requests = new List<ItemRequestViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ItemRequestViewModel> models = new List<ItemRequestViewModel>();
                string userid = User.Identity.GetUserId();
                if(User.IsInRole(Role.SystemAdmin)||User.IsInRole(Role.Purchaser)||User.IsInRole(Role.AstOperationDirector)||User.IsInRole(Role.OperationDirector)||User.IsInRole(Role.TechnicalDirector)||User.IsInRole(Role.ManagingDirector))
                    models = (from tbl in db.tb_item_request
                              join project in db.tb_project on tbl.ir_project_id equals project.project_id
                              join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                              from purpose in pp.DefaultIfEmpty()
                              orderby tbl.ir_no descending
                              where tbl.status == true && (string.Compare(tbl.ir_status,"Pending")==0 || string.Compare(tbl.ir_status, "Approved") == 0 || string.Compare(tbl.ir_status, "Completed") == 0)
                              select new ItemRequestViewModel
                              {
                                  ir_id = tbl.ir_id,
                                  ir_no = tbl.ir_no,
                                  ir_project_id = tbl.ir_project_id,
                                  project_full_name = project.project_full_name,
                                  project_short_name = project.project_short_name,
                                  ir_purpose_id = tbl.ir_purpose_id,
                                  purpose_description = purpose.purpose_name,
                                  ir_status = tbl.ir_status,
                                  created_date = tbl.created_date,
                                  is_mr=tbl.is_mr,
                                  po_status=tbl.po_status,
                                  st_status=tbl.st_status,
                                  tw_status=tbl.tw_status,
                              }).ToList(); 
                else 
                {

                    if (User.IsInRole(Role.ProjectManager))
                    {
                        var objs = (from tbl in db.tb_item_request
                                  join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                  join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                  from purpose in pp.DefaultIfEmpty()
                                  join pm in db.tb_project_pm on project.project_id equals pm.project_id
                                  orderby tbl.ir_no descending
                                  where tbl.status == true && (string.Compare(tbl.ir_status, "Pending") == 0 || string.Compare(tbl.ir_status, "Approved") == 0 || string.Compare(tbl.ir_status, "Completed") == 0) && string.Compare(pm.project_manager_id,userid)==0
                                  select new ItemRequestViewModel
                                  {
                                      ir_id = tbl.ir_id,
                                      ir_no = tbl.ir_no,
                                      ir_project_id = tbl.ir_project_id,
                                      project_full_name = project.project_full_name,
                                      project_short_name = project.project_short_name,
                                      ir_purpose_id = tbl.ir_purpose_id,
                                      purpose_description = purpose.purpose_name,
                                      ir_status = tbl.ir_status,
                                      created_date = tbl.created_date,
                                      is_mr=tbl.is_mr,
                                      po_status = tbl.po_status,
                                      st_status = tbl.st_status,
                                      tw_status = tbl.tw_status,
                                  }).ToList();
                        foreach(var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.ir_id, obj.ir_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteManager))
                    {
                        var objs = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    join sm in db.tb_site_manager_project on project.project_id equals sm.project_id
                                    orderby tbl.ir_no descending
                                    where tbl.status == true && (string.Compare(tbl.ir_status, "Pending") == 0 || string.Compare(tbl.ir_status, "Approved") == 0 || string.Compare(tbl.ir_status, "Completed") == 0) && string.Compare(sm.site_manager, userid) == 0
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date,
                                        is_mr = tbl.is_mr,
                                        po_status = tbl.po_status,
                                        st_status = tbl.st_status,
                                        tw_status = tbl.tw_status,
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.ir_id, obj.ir_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteSupervisor))
                    {
                        var objs = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    join sup in db.tbSiteSiteSupervisors on project.project_id equals sup.site_id
                                    orderby tbl.ir_no descending
                                    where tbl.status == true && (string.Compare(tbl.ir_status, "Pending") == 0 || string.Compare(tbl.ir_status, "Approved") == 0 || string.Compare(tbl.ir_status, "Completed") == 0) && string.Compare(sup.site_supervisor_id, userid) == 0
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date,
                                        is_mr = tbl.is_mr,
                                        po_status = tbl.po_status,
                                        st_status = tbl.st_status,
                                        tw_status = tbl.tw_status,
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.ir_id, obj.ir_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                    if (User.IsInRole(Role.SiteAdmin))
                    {
                        var objs = (from tbl in db.tb_item_request
                                    join project in db.tb_project on tbl.ir_project_id equals project.project_id
                                    join purpose in db.tb_purpose on tbl.ir_purpose_id equals purpose.purpose_id into pp
                                    from purpose in pp.DefaultIfEmpty()
                                    join sa in db.tb_site_site_admin on project.project_id equals sa.site_id
                                    orderby tbl.ir_no descending
                                    where tbl.status == true && (string.Compare(tbl.ir_status, "Pending") == 0 || string.Compare(tbl.ir_status, "Approved") == 0 || string.Compare(tbl.ir_status, "Completed") == 0) && string.Compare(sa.site_admin_id, userid) == 0
                                    select new ItemRequestViewModel
                                    {
                                        ir_id = tbl.ir_id,
                                        ir_no = tbl.ir_no,
                                        ir_project_id = tbl.ir_project_id,
                                        project_full_name = project.project_full_name,
                                        project_short_name = project.project_short_name,
                                        ir_purpose_id = tbl.ir_purpose_id,
                                        purpose_description = purpose.purpose_name,
                                        ir_status = tbl.ir_status,
                                        created_date = tbl.created_date,
                                        is_mr = tbl.is_mr,
                                        po_status = tbl.po_status,
                                        st_status = tbl.st_status,
                                        tw_status = tbl.tw_status,
                                    }).ToList();
                        foreach (var obj in objs)
                        {
                            var isExist = models.Where(s => string.Compare(s.ir_id, obj.ir_id) == 0).FirstOrDefault() == null ? false : true;
                            if (!isExist)
                                models.Add(obj);
                        }
                    }
                }
                if (models.Any())
                {
                    foreach (var ir in models)
                    {
                        bool isCompleted = CommonClass.GetAvailableRequestItemDetails(ir.ir_id).Count() == 0 ? true : false;
                        ItemRequestViewModel materialRequest = new ItemRequestViewModel();
                        materialRequest.ir_id = ir.ir_id;
                        materialRequest.ir_no = ir.ir_no;
                        materialRequest.ir_project_id = ir.ir_project_id;
                        materialRequest.ir_purpose_id = ir.ir_purpose_id;
                        materialRequest.ir_status = ir.ir_status;
                        materialRequest.created_date = ir.created_date;
                        materialRequest.project_full_name = ir.project_full_name;
                        materialRequest.project_short_name = ir.project_short_name;
                        materialRequest.purpose_description = ir.purpose_description;
                        materialRequest.isCompleted = isCompleted;
                        materialRequest.is_mr = ir.is_mr;
                        materialRequest.po_status = ir.po_status;
                        materialRequest.st_status = ir.st_status;
                        materialRequest.tw_status = ir.tw_status;
                        #region get Transfer Workshop status
                        if(string.Compare(materialRequest.ir_status,"Approved")==0 || string.Compare(materialRequest.ir_status, "Completed") == 0)
                        {
                            var latestTransferWorkshop = db.transferformmainstocks.OrderByDescending(s=>s.created_date).Where(s => s.status == true && string.Compare(s.item_request_id, materialRequest.ir_id) == 0).FirstOrDefault();
                            if (latestTransferWorkshop != null)
                            {
                                if (String.Compare(latestTransferWorkshop.stock_transfer_status, Status.Completed) == 0)
                                {
                                    var obj= db.tb_received_status.OrderByDescending(s => s.created_date).Where(s => string.Compare(s.received_ref_id, latestTransferWorkshop.stock_transfer_id) == 0).FirstOrDefault();
                                    if (obj != null)
                                        materialRequest.tw_status = obj.status;
                                }

                            }
                        }
                        #endregion

                        item_requests.Add(materialRequest);
                        //item_requests.Add(new ItemRequestViewModel()
                        //{
                        //    ir_id = ir.ir_id,
                        //    ir_no = ir.ir_no,
                        //    ir_project_id = ir.ir_project_id,
                        //    ir_purpose_id = ir.ir_purpose_id,
                        //    ir_status = ir.ir_status,
                        //    created_date = ir.created_date,
                        //    project_full_name = ir.project_full_name,
                        //    project_short_name = ir.project_short_name,
                        //    purpose_description = ir.purpose_description,
                        //    isCompleted = isCompleted,
                        //    is_mr = ir.is_mr,
                        //    po_status = ir.po_status,
                        //    st_status = ir.st_status,
                        //    tw_status = ir.tw_status,
                        //});
                    }
                }
                return View(item_requests);
            }
        }

        #endregion
        public List<tb_product_size> GetProductSizeList()
        {
            List<tb_product_size> itemreques = new List<tb_product_size>();
            using (kim_mexEntities db = new kim_mexEntities())
            {

                itemreques = db.tb_product_size.OrderBy(x => x.product_size_id).Where(x => x.active == true).ToList();
            }
            return itemreques;
        }
        public ActionResult GetMaterialRequestListItemsJson(string id)
        {
            kim_mexEntities db = new kim_mexEntities();
            List<ItemRequestDetail2ViewModel> items = new List<ItemRequestDetail2ViewModel>();
            items = ItemRequest.GetMaterialRequestListItems(id);
            //return Json(new { data = GetItemRequestDetail(id) }, JsonRequestBehavior.AllowGet);
            return Json(new { data = items }, JsonRequestBehavior.AllowGet);
        }

    }
}