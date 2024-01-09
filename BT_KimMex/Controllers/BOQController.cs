using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace BT_KimMex.Controllers
{
    
    public class BOQController : Controller
    {
        public kim_mexEntities db = new kim_mexEntities();
        Importing importBoQClass = new Importing();
        // GET: BOQ
        [Authorize(Roles = "Admin,Economic Engineer,Project Manager,Director")]
        public ActionResult Index(string status)
        {
            List<BOQViewModel> boqs = new List<BOQViewModel>();
            boqs = this.BOQLists(status);
            return View(boqs);
        }
        //public ActionResult SendEmail(string receiver, string subject, string message) {
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var senderEmail = new MailAddress("soklakhena999@gmail.com", "Gmail");
        //            var receiverEmail = new MailAddress(receiver, "Receiver");
        //            var password = "Lakhena12345";
        //            var sub = subject;
        //            var body = message;
        //            var smtp = new SmtpClient
        //            {
        //                Host = "smtp.gmail.com",
        //                Port = 587,
        //                EnableSsl = true,
        //                DeliveryMethod = SmtpDeliveryMethod.Network,
        //                UseDefaultCredentials = false,
        //                Credentials = new NetworkCredential(senderEmail.Address, password)
        //            };

        //            using (var mess = new MailMessage(senderEmail, receiverEmail)
        //            {
        //                Subject = subject,
        //                Body = body
        //            })
        //            {
        //                smtp.Send(mess);
        //            }
        //            return View();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ViewBag.Error = "Send Fail!!!";

        //    }

        //    return View ();
        //}

        [Authorize(Roles = "Admin,Economic Engineer")]
        public ActionResult Create(string id)
        {
            if (id != null)
            {
                BOQViewModel boq = new BOQViewModel();
                var project_detail = (from tbl in db.tb_project
                                      join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id
                                      orderby tbl.project_id
                                      where tbl.project_id == id
                                      select new ProjectViewModel()
                                      {
                                          project_id = tbl.project_id,
                                          project_short_name = tbl.project_short_name,
                                          project_full_name = tbl.project_full_name,
                                          customer_name = cus.customer_name,
                                          cutomer_signatory = tbl.cutomer_signatory,
                                          cutomer_project_manager = tbl.cutomer_project_manager,
                                          customer_telephone = tbl.customer_telephone
                                      }).FirstOrDefault();
                if (project_detail != null)
                {
                    boq = new BOQViewModel();
                    boq.project_id = project_detail.project_id;
                    boq.customer_name = project_detail.customer_name;
                    boq.cutomer_signatory = project_detail.cutomer_signatory;
                    boq.cutomer_project_manager = project_detail.cutomer_project_manager;
                    boq.project_telephone = project_detail.project_telephone;
                    TempData["project"] = "True";
                    return View(boq);
                }
            }


            return View();
        }
        //fucntion SendEmail
        public void SendingEmail() {
            try
            {
                var SelectUser = db.tb_user_detail.Where(i => i.user_position_id == "6" && i.status == true).ToList();
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("soklakhena999@gmail.com", "Kim Mex");
                    var password = "Lakhena12345";
                    var sub = "Approve";
                    var body = "hello111";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    foreach (var i in SelectUser)
                    {
                        using (var mess = new MailMessage(senderEmail, new MailAddress(i.user_email, "Customer"))
                        {
                            Subject = sub,
                            Body = body
                        })
                        {
                            smtp.Send(mess);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public ActionResult CreateBOQ(BOQViewModel model, List<BOQDetail1> boq_detail1, List<BOQDetail2> boq_detail2, List<BOQDetail3> boq_detail3,List<String> Attachment)
        {
     
            try
            {
                if (!GlobalMethod.IsBOQProjectExist(model.project_id))
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_build_of_quantity boq = new tb_build_of_quantity();
                    boq.boq_id = Guid.NewGuid().ToString();
                    boq.boq_no = model.boq_no;
                    boq.project_id = model.project_id;
                    //boq.boq_status = "Pending";
                    boq.boq_status = "Draft";
                    boq.status = true;
                    boq.created_by = User.Identity.Name;
                    boq.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_build_of_quantity.Add(boq);
                    db.SaveChanges();

                    tb_boq_detail1 boq_detail_1 = new tb_boq_detail1();
                    for (int i = 0; i < boq_detail1.Count(); i++)
                    {
                        boq_detail_1 = new tb_boq_detail1();
                        boq_detail_1.boq_detail1_id = Guid.NewGuid().ToString();
                        boq_detail_1.boq_id = boq.boq_id;
                        boq_detail_1.job_category_id = boq_detail1[i].job_category_id;
                        boq_detail_1.job_category_code = boq_detail1[i].job_category_code;
                        boq_detail_1.amount = boq_detail1[i].amount;
                        boq_detail_1.remark = boq_detail1[i].remark;
                        db.tb_boq_detail1.Add(boq_detail_1);
                        db.SaveChanges();
                        if (boq_detail2 != null)
                        {
                            for (var j = 0; j < boq_detail2.Count(); j++)
                            {
                                if (boq_detail1[i].job_group == boq_detail2[j].job_group)
                                {
                                    tb_boq_detail2 boq_detail_2 = new tb_boq_detail2();
                                    boq_detail_2.boq_detail2_id = Guid.NewGuid().ToString();
                                    boq_detail_2.boq_detail1_id = boq_detail_1.boq_detail1_id;
                                    boq_detail_2.item_type_id = boq_detail2[j].item_type_id;
                                    boq_detail_2.amount = boq_detail2[j].amount;
                                    boq_detail_2.remark = boq_detail2[j].remark;
                                    db.tb_boq_detail2.Add(boq_detail_2);
                                    db.SaveChanges();
                                    if (boq_detail3 != null)
                                    {
                                        for (int k = 0; k < boq_detail3.Count(); k++)
                                        {
                                            if (boq_detail2[j].type_group == boq_detail3[k].type_group)
                                            {
                                                tb_boq_detail3 boq_detail_3 = new tb_boq_detail3();
                                                boq_detail_3.boq_detail3_id = Guid.NewGuid().ToString();
                                                boq_detail_3.boq_detail2_id = boq_detail_2.boq_detail2_id;
                                                boq_detail_3.item_id = boq_detail3[k].item_id;
                                                boq_detail_3.item_qty = boq_detail3[k].item_qty;
                                                boq_detail_3.item_unit_price = boq_detail3[k].item_unit_price;
                                                db.tb_boq_detail3.Add(boq_detail_3);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Attachment != null)
                    {
                        for(int i = 0; i < Attachment.Count(); i++)
                        {
                            string attID = Attachment[i];
                            tb_attachment att = db.tb_attachment.Where(m => m.attachment_id == attID).FirstOrDefault();
                            att.attachment_ref_id = boq.boq_id;
                            db.SaveChanges();
                        }
                    }

                    #region add send mail july 11 2018
  
                          SendingEmail();

                    //try {
                    //    var SelectUser = db.tb_user_detail.Where(i => i.user_position_id == "6"&&i.status==true).ToList();
                    //    if (ModelState.IsValid) {

                    //        var senderEmail = new MailAddress("soklakhena999@gmail.com", "Kim Mex");                    
                    //        var password = "Lakhena12345";
                    //        var sub = "Approve";
                    //        var body = "hello111";
                    //        var smtp = new SmtpClient
                    //        {
                    //            Host = "smtp.gmail.com",
                    //            Port = 587,
                    //            EnableSsl = true,
                    //            DeliveryMethod = SmtpDeliveryMethod.Network,
                    //            UseDefaultCredentials = false,
                    //            Credentials = new NetworkCredential(senderEmail.Address, password)
                    //        };
                    //        foreach (var i in SelectUser)
                    //        {
                    //            using (var mess = new MailMessage(senderEmail, new MailAddress (i.user_email,"Customer"))
                    //            {
                    //                Subject = sub,
                    //                Body = body
                    //            })
                    //            {
                    //                smtp.Send(mess);
                    //            }

                    //        }
                    //    }    
                    //} catch (Exception ex) { }
     #endregion

                    return Json(new { Message = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "exist" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Details(string id)
        {
            BOQViewModel boq = new BOQViewModel();
            boq = this.GetBOQViewDetail(id);
            return View(boq);
        }
        [Authorize(Roles = "Admin,Economic Engineer")]
        public ActionResult Edit(string id)
        {
            BOQViewModel boq = new BOQViewModel();
            boq = this.GetBOQViewDetail(id);
            return View(boq);
        }
        [Authorize(Roles = "Admin,Economic Engineer")]
        public ActionResult EditBOQ(BOQViewModel model, List<BOQDetail1> boq_detail1, List<BOQDetail2> boq_detail2, List<BOQDetail3> boq_detail3, List<String> Attachment)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_build_of_quantity boq = db.tb_build_of_quantity.FirstOrDefault(m => m.boq_id == model.boq_id);
                boq.boq_no = model.boq_no;
                boq.project_id = model.project_id;
                boq.boq_status = "Draft";
                boq.status = true;
                boq.updated_by = User.Identity.Name;
                boq.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();

                tb_boq_detail1 boq_detail_1 = new tb_boq_detail1();
                for (int i = 0; i < boq_detail1.Count(); i++)
                {
                    boq_detail_1 = new tb_boq_detail1();
                    boq_detail_1.boq_detail1_id = Guid.NewGuid().ToString();
                    boq_detail_1.boq_id = boq.boq_id;
                    boq_detail_1.job_category_id = boq_detail1[i].job_category_id;
                    boq_detail_1.job_category_code = boq_detail1[i].job_category_code;
                    boq_detail_1.amount = boq_detail1[i].amount;
                    boq_detail_1.remark = boq_detail1[i].remark;
                    db.tb_boq_detail1.Add(boq_detail_1);
                    db.SaveChanges();
                    if (boq_detail2 != null)
                    {
                        for (var j = 0; j < boq_detail2.Count(); j++)
                        {
                            if (boq_detail1[i].job_group == boq_detail2[j].job_group)
                            {
                                tb_boq_detail2 boq_detail_2 = new tb_boq_detail2();
                                boq_detail_2.boq_detail2_id = Guid.NewGuid().ToString();
                                boq_detail_2.boq_detail1_id = boq_detail_1.boq_detail1_id;
                                boq_detail_2.item_type_id = boq_detail2[j].item_type_id;
                                boq_detail_2.amount = boq_detail2[j].amount;
                                boq_detail_2.remark = boq_detail2[j].remark;
                                db.tb_boq_detail2.Add(boq_detail_2);
                                db.SaveChanges();
                                if (boq_detail3 != null)
                                {
                                    for (int k = 0; k < boq_detail3.Count(); k++)
                                    {
                                        if (boq_detail2[j].type_group == boq_detail3[k].type_group)
                                        {
                                            tb_boq_detail3 boq_detail_3 = new tb_boq_detail3();
                                            boq_detail_3.boq_detail3_id = Guid.NewGuid().ToString();
                                            boq_detail_3.boq_detail2_id = boq_detail_2.boq_detail2_id;
                                            boq_detail_3.item_id = boq_detail3[k].item_id;
                                            boq_detail_3.item_qty = boq_detail3[k].item_qty;
                                            boq_detail_3.item_unit_price = boq_detail3[k].item_unit_price;
                                            db.tb_boq_detail3.Add(boq_detail_3);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Attachment != null)
                {
                    for (int i = 0; i < Attachment.Count(); i++)
                    {
                        string attID = Attachment[i];
                        tb_attachment att = db.tb_attachment.Where(m => m.attachment_id == attID).FirstOrDefault();
                        att.attachment_ref_id = boq.boq_id;
                        db.SaveChanges();
                    }
                }
                
                return Json(new { message = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteBOQDetail(List<String> boq_details1, List<String> boq_details2, List<String> boq_details3)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                if (boq_details3 != null)
                {
                    for (int i = 0; i < boq_details3.Count(); i++)
                    {
                        var id = boq_details3[i];
                        tb_boq_detail3 boq_detail3 = db.tb_boq_detail3.Where(m => m.boq_detail3_id == id).FirstOrDefault();
                        db.tb_boq_detail3.Remove(boq_detail3);
                        db.SaveChanges();
                    }
                }
                if (boq_details2 != null)
                {
                    for (int i = 0; i < boq_details2.Count(); i++)
                    {
                        string id = boq_details2[i];
                        tb_boq_detail2 boq_detail2 = db.tb_boq_detail2.Where(m => m.boq_detail2_id == id).FirstOrDefault();
                        db.tb_boq_detail2.Remove(boq_detail2);
                        db.SaveChanges();
                    }
                }
                if (boq_details1 != null)
                {
                    for (int i = 0; i < boq_details1.Count(); i++)
                    {
                        string id = boq_details1[i];
                        tb_boq_detail1 boq_detail1 = db.tb_boq_detail1.Where(m => m.boq_detail1_id == id).FirstOrDefault();
                        db.tb_boq_detail1.Remove(boq_detail1);
                        db.SaveChanges();
                    }
                }
                var results = new { message = "success" };
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var results = new { message = "fail", errorText = ex.Message };
                return Json(results, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_build_of_quantity boq = db.tb_build_of_quantity.FirstOrDefault(m => m.boq_id == id);
                boq.status = false;
                boq.updated_by = User.Identity.Name;
                boq.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                TempData["message"] = "Your data has been deleted!";
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "fail",message=ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Submit(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_build_of_quantity boq = db.tb_build_of_quantity.Find(id);
                boq.boq_status = "Pending";
                boq.updated_by = User.Identity.Name;
                boq.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" },JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Message(EnumConstants.MessageParameter? message)
        {
            ViewBag.StatusMessage = message == EnumConstants.MessageParameter.SaveSuccessfull ? "Your data has been saved!" :
                message == EnumConstants.MessageParameter.SaveError ? "Your data is error while saving!" :
                message == EnumConstants.MessageParameter.UpdateSuccessfull ? "Your data has been updated!" :
                message == EnumConstants.MessageParameter.UpdateError ? "Your data is error while updating!" :
                message == EnumConstants.MessageParameter.DeleteSuccessfull ? "Your data has been deleted!" :
                message == EnumConstants.MessageParameter.DeleteError ? "Your data is error while deleting"
                : "";
            return View();
        }
        public ActionResult GetBOQNumber()
        {
            try
            {
                string boq_no = "", digit = "";
                kim_mexEntities db = new kim_mexEntities();
                var last_no = (from tbl in db.tb_build_of_quantity orderby tbl.created_date descending select tbl.boq_no).FirstOrDefault();
                if (last_no == null || last_no == "")
                    digit = "001";
                else
                {
                    last_no = last_no.Substring(last_no.Length - 3, 3);
                    int number = Convert.ToInt32(last_no) + 1;
                    if (number.ToString().Length == 1)
                        digit = "00" + number;
                    else if (number.ToString().Length == 2)
                        digit = "0" + number;
                    else if (number.ToString().Length == 3)
                        digit = number.ToString();
                }
                string dd = Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Day.ToString();
                string mm = Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString().Length == 1 ? "0" + Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString() : Class.CommonClass.ToLocalTime(DateTime.Now).Month.ToString();
                boq_no = "BOQ-" + dd + "-" + mm + "-" + digit;
                return Json(new { data = boq_no }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<BOQViewModel> BOQLists(string boq_status)
        {
            List<BOQViewModel> boqs = new List<BOQViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                IQueryable<BOQViewModel> boq_list;
                if (boq_status == "All")
                {
                    boq_list = (from tbl in db.tb_build_of_quantity
                                join pro in db.tb_project on tbl.project_id equals pro.project_id
                                orderby tbl.boq_no
                                where tbl.status == true
                                select new BOQViewModel
                                {
                                    boq_id = tbl.boq_id,
                                    boq_no = tbl.boq_no,
                                    project_id = tbl.project_id,
                                    project_short_name = pro.project_short_name,
                                    project_full_name = pro.project_full_name,
                                    cutomer_id = pro.cutomer_id,
                                    customer_name = pro.tb_customer.customer_name,
                                    cutomer_signatory = pro.cutomer_signatory,
                                    cutomer_project_manager = pro.cutomer_project_manager,
                                    project_telephone = pro.project_telephone,
                                    created_date = tbl.created_date,
                                    created_by = tbl.created_by,
                                    checked_by = tbl.checked_by,
                                    checked_date = tbl.checked_date,
                                    approved_by = tbl.approved_by,
                                    approved_date = tbl.approved_date,
                                    boq_status = tbl.boq_status
                                });
                }
                else
                {
                    boq_list = (from tbl in db.tb_build_of_quantity
                                join pro in db.tb_project on tbl.project_id equals pro.project_id
                                orderby tbl.boq_no
                                where tbl.status == true && tbl.boq_status == boq_status
                                select new BOQViewModel
                                {
                                    boq_id = tbl.boq_id,
                                    boq_no = tbl.boq_no,
                                    project_id = tbl.project_id,
                                    project_short_name = pro.project_short_name,
                                    project_full_name = pro.project_full_name,
                                    cutomer_id = pro.cutomer_id,
                                    customer_name = pro.tb_customer.customer_name,
                                    cutomer_signatory = pro.cutomer_signatory,
                                    cutomer_project_manager = pro.cutomer_project_manager,
                                    project_telephone = pro.project_telephone,
                                    created_date = tbl.created_date,
                                    created_by = tbl.created_by,
                                    checked_by = tbl.checked_by,
                                    checked_date = tbl.checked_date,
                                    approved_by = tbl.approved_by,
                                    approved_date = tbl.approved_date,
                                    boq_status = tbl.boq_status
                                });
                }
                if (boq_list.Any())
                {
                    foreach (var boq in boq_list)
                    {
                        boqs.Add(new BOQViewModel() { boq_id = boq.boq_id, boq_no = boq.boq_no, project_id = boq.project_id, project_short_name = boq.project_short_name, project_full_name = boq.project_full_name, customer_name = boq.customer_name, cutomer_signatory = boq.cutomer_signatory, cutomer_project_manager = boq.cutomer_project_manager, project_telephone = boq.project_telephone, boq_status = boq.boq_status });
                    }
                }
            }
            catch
            {
                return null;
            }
            return boqs;
        }
        public ActionResult BOQDataTable(string boq_status)
        {
            List<BOQViewModel> boqs = new List<BOQViewModel>();
            boqs = this.BOQLists(boq_status);
            return Json(new { data = boqs }, JsonRequestBehavior.AllowGet);
        }   
        public ActionResult ImportBoQ()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ImportBoQ(string fileName)
        {
            string message = string.Empty;
            if (Request.Files.Count > 0)
            {
                var file0 = Request.Files[0];
                if (file0 != null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    message= importBoQClass.GetDataModelFromExcelContact(path, true);
                    TempData["message"] = message;
                }
            }
            //return View();
            return RedirectToAction("Index");
        }
        public ActionResult BOQViewDetail(string id)
        {
            try
            {
                List<BOQDetail1> boq_details_1 = new List<BOQDetail1>();
                List<BOQDetail2> boq_details_2 = new List<BOQDetail2>();
                List<BOQDetail3> boq_details_3 = new List<BOQDetail3>();
                int count_boq_detail = 1;
                int count_boq_detail2 = 1;
                int count_boq_detail3 = 1;

                var boq = (from tbl in db.tb_build_of_quantity
                           join pro in db.tb_project on tbl.project_id equals pro.project_id
                           where tbl.status == true && tbl.boq_id == id
                           select new BOQViewModel()
                           {
                               created_date = tbl.created_date,
                               boq_id = tbl.boq_id,
                               boq_no = tbl.boq_no,
                               boq_status = tbl.boq_status,
                               project_id = tbl.project_id,
                               project_short_name = pro.project_short_name,
                               project_full_name = pro.project_full_name,
                               cutomer_id = pro.cutomer_id,
                               customer_name = pro.tb_customer.customer_name,
                               cutomer_signatory = pro.cutomer_signatory,
                               cutomer_project_manager = pro.cutomer_project_manager,
                           }).FirstOrDefault();
                /* get boq detail 1*/
                var boq_details1_list = (from tbl in db.tb_boq_detail1

                                         join job_cat in db.tb_job_category on tbl.job_category_id equals job_cat.j_category_id
                                         orderby tbl.job_category_code
                                         where tbl.boq_id == boq.boq_id
                                         select new BOQDetail1()
                                         {
                                             boq_detail1_id = tbl.boq_detail1_id,
                                             job_category_code = tbl.job_category_code,
                                             job_category_id = tbl.job_category_id,
                                             j_category_name = job_cat.j_category_name,
                                             j_description = job_cat.j_description,
                                             amount = tbl.amount,
                                             remark = tbl.remark
                                         }).ToList();
                if (boq_details1_list.Any())
                {
                    count_boq_detail = 1;
                    foreach (var boq_detail_1 in boq_details1_list)
                    {
                        boq_details_1.Add(new BOQDetail1()
                        {
                            boq_detail1_id = boq_detail_1.boq_detail1_id,
                            job_category_code = boq_detail_1.job_category_code,
                            job_category_id = boq_detail_1.job_category_id,
                            j_category_name = boq_detail_1.j_category_name,
                            j_description = boq_detail_1.j_description,
                            amount = boq_detail_1.amount,
                            remark = boq_detail_1.remark,
                            job_group = count_boq_detail.ToString()
                        });
                        /* get boq detail 2 */
                        var boq_details2_list = (from tbl in db.tb_boq_detail2
                                                 join pro_cat in db.tb_product_category on tbl.item_type_id equals pro_cat.p_category_id
                                                 orderby pro_cat.p_category_code
                                                 where tbl.boq_detail1_id == boq_detail_1.boq_detail1_id
                                                 select new BOQDetail2()
                                                 {
                                                     boq_detail2_id = tbl.boq_detail2_id,
                                                     boq_detail1_id = boq_detail_1.boq_detail1_id,
                                                     job_group = count_boq_detail.ToString(),
                                                     item_type_id = tbl.item_type_id,
                                                     p_category_code = pro_cat.p_category_code,
                                                     p_category_name = pro_cat.p_category_name,
                                                     p_category_address = pro_cat.p_category_address,
                                                     chart_account=pro_cat.chart_account,
                                                     amount = tbl.amount,
                                                     remark = tbl.remark,
                                                 }).ToList();
                        if (boq_details2_list.Any())
                        {
                            count_boq_detail2 = 1;
                            foreach (var boq_detail2 in boq_details2_list)
                            {
                                decimal ItemTypeAmount = 0;
                                
                                /* get boq detail 3 */
                                var boq_details3_list = (from tbl in db.tb_boq_detail3
                                                         join item in db.tb_product on tbl.item_id equals item.product_id
                                                         orderby item.product_code
                                                         where tbl.boq_detail2_id == boq_detail2.boq_detail2_id
                                                         select new BOQDetail3()
                                                         {
                                                             boq_detail3_id = tbl.boq_detail3_id,
                                                             boq_detail2_id = boq_detail2.boq_detail2_id,
                                                             item_id = tbl.item_id,
                                                             item_qty = tbl.item_qty,
                                                             item_unit_price = tbl.item_unit_price,
                                                             p_category_id = item.brand_id,
                                                             product_code = item.product_code,
                                                             product_name = item.product_name,
                                                             product_unit = item.product_unit,

                                                         }).ToList();
                                if (boq_details3_list.Any())
                                {
                                    foreach (var boq_detail3 in boq_details3_list)
                                    {
                                        ItemTypeAmount = ItemTypeAmount +Convert.ToDecimal(boq_detail3.item_qty * boq_detail3.item_unit_price);
                                        boq_details_3.Add(new BOQDetail3()
                                        {
                                            boq_detail3_id = boq_detail3.boq_detail3_id,
                                            boq_detail2_id = boq_detail3.boq_detail2_id,
                                            item_id = boq_detail3.item_id,
                                            item_qty = boq_detail3.item_qty,
                                            item_unit_price = boq_detail3.item_unit_price,
                                            p_category_id = boq_detail3.p_category_id,
                                            product_code = boq_detail3.product_code,
                                            product_name = boq_detail3.product_name,
                                            product_unit = boq_detail3.product_unit,
                                            type_group = count_boq_detail.ToString() + (Convert.ToChar(64 + count_boq_detail2)).ToString()
                                            //type_group=boq_detail2.type_group
                                        });
                                    }
                                }
                                /* end get boq detail 3 */
                                boq_details_2.Add(new BOQDetail2()
                                {
                                    boq_detail2_id = boq_detail2.boq_detail2_id,
                                    boq_detail1_id = boq_detail2.boq_detail1_id,
                                    item_type_id = boq_detail2.item_type_id,
                                    p_category_code = boq_detail2.p_category_code,
                                    p_category_name = boq_detail2.p_category_name,
                                    p_category_address = boq_detail2.p_category_address,
                                    job_group = boq_detail2.job_group,
                                    type_group = boq_detail2.job_group + (Convert.ToChar(64 + count_boq_detail2)).ToString(),
                                    //amount = boq_detail2.amount,
                                    amount=ItemTypeAmount,
                                    remark = boq_detail2.remark,
                                    chart_account = boq_detail2.chart_account,
                                });
                                count_boq_detail2++;
                            }
                        }
                        /* end get boq detail 2 */
                        count_boq_detail++;
                    }
                }
                /* end get boq detail 1 */
                var results = new { result = "success", boq = boq, boq_details_1 = boq_details_1, boq_details_2 = boq_details_2, boq_details_3 = boq_details_3 };
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public BOQViewModel GetBOQViewDetail(string id)
        {
            try
            {
                List<BOQDetail1> boq_details_1 = new List<BOQDetail1>();
                List<BOQDetail2> boq_details_2 = new List<BOQDetail2>();
                List<BOQDetail3> boq_details_3 = new List<BOQDetail3>();
                int count_boq_detail = 1;
                int count_boq_detail2 = 1;
                var boq = (from tbl in db.tb_build_of_quantity
                           join pro in db.tb_project on tbl.project_id equals pro.project_id
                           where tbl.status == true && tbl.boq_id == id
                           select new BOQViewModel()
                           {
                               created_date = tbl.created_date,
                               boq_id = tbl.boq_id,
                               boq_no = tbl.boq_no,
                               boq_status = tbl.boq_status,
                               project_id = tbl.project_id,
                               project_short_name = pro.project_short_name,
                               project_full_name = pro.project_full_name,
                               cutomer_id = pro.cutomer_id,
                               customer_name = pro.tb_customer.customer_name,
                               cutomer_signatory = pro.cutomer_signatory,
                               cutomer_project_manager = pro.cutomer_project_manager,
                           }).FirstOrDefault();
                /* get boq detail 1*/
                var boq_details1_list = (from tbl in db.tb_boq_detail1

                                         join job_cat in db.tb_job_category on tbl.job_category_id equals job_cat.j_category_id
                                         orderby tbl.job_category_code
                                         where tbl.boq_id == boq.boq_id
                                         select new BOQDetail1()
                                         {
                                             boq_detail1_id = tbl.boq_detail1_id,
                                             job_category_code = tbl.job_category_code,
                                             job_category_id = tbl.job_category_id,
                                             j_category_name = job_cat.j_category_name,
                                             j_description = job_cat.j_description,
                                             amount = tbl.amount,
                                             remark = tbl.remark
                                         }).ToList();
                if (boq_details1_list.Any())
                {
                    count_boq_detail = 1;
                    foreach (var boq_detail_1 in boq_details1_list)
                    {
                        boq_details_1.Add(new BOQDetail1()
                        {
                            boq_detail1_id = boq_detail_1.boq_detail1_id,
                            job_category_code = boq_detail_1.job_category_code,
                            job_category_id = boq_detail_1.job_category_id,
                            j_category_name = boq_detail_1.j_category_name,
                            j_description = boq_detail_1.j_description,
                            amount = boq_detail_1.amount,
                            remark = boq_detail_1.remark,
                            job_group = count_boq_detail.ToString()
                        });
                        /* get boq detail 2 */
                        var boq_details2_list = (from tbl in db.tb_boq_detail2
                                                 join pro_cat in db.tb_product_category on tbl.item_type_id equals pro_cat.p_category_id
                                                 orderby pro_cat.p_category_code
                                                 where tbl.boq_detail1_id == boq_detail_1.boq_detail1_id
                                                 select new BOQDetail2()
                                                 {
                                                     boq_detail2_id = tbl.boq_detail2_id,
                                                     boq_detail1_id = boq_detail_1.boq_detail1_id,
                                                     job_group = count_boq_detail.ToString(),
                                                     item_type_id = tbl.item_type_id,
                                                     p_category_code = pro_cat.p_category_code,
                                                     p_category_name = pro_cat.p_category_name,
                                                     p_category_address = pro_cat.p_category_address,
                                                     chart_account = pro_cat.chart_account,
                                                     amount = tbl.amount,
                                                     remark = tbl.remark,
                                                 }).ToList();
                        if (boq_details2_list.Any())
                        {
                            count_boq_detail2 = 1;
                            foreach (var boq_detail2 in boq_details2_list)
                            {
                                boq_details_2.Add(new BOQDetail2()
                                {
                                    boq_detail2_id = boq_detail2.boq_detail2_id,
                                    boq_detail1_id = boq_detail2.boq_detail1_id,
                                    item_type_id = boq_detail2.item_type_id,
                                    p_category_code = boq_detail2.p_category_code,
                                    p_category_name = boq_detail2.p_category_name,
                                    p_category_address = boq_detail2.p_category_address,
                                    job_group = boq_detail2.job_group,
                                    type_group = boq_detail2.job_group + (Convert.ToChar(64 + count_boq_detail2)).ToString(),
                                    type_group_letter= (Convert.ToChar(64 + count_boq_detail2)).ToString(),
                                    amount = boq_detail2.amount,
                                    remark = boq_detail2.remark,
                                    chart_account = boq_detail2.chart_account,
                                });
                                /* get boq detail 3 */
                                var boq_details3_list = (from tbl in db.tb_boq_detail3
                                                         join item in db.tb_product on tbl.item_id equals item.product_id
                                                         orderby item.product_code
                                                         where tbl.boq_detail2_id == boq_detail2.boq_detail2_id
                                                         select new BOQDetail3()
                                                         {
                                                             boq_detail3_id = tbl.boq_detail3_id,
                                                             boq_detail2_id = boq_detail2.boq_detail2_id,
                                                             item_id = tbl.item_id,
                                                             item_qty = tbl.item_qty,
                                                             item_unit_price = tbl.item_unit_price,
                                                             p_category_id = item.brand_id,
                                                             product_code = item.product_code,
                                                             product_name = item.product_name,
                                                             product_unit = item.product_unit,

                                                         }).ToList();
                                if (boq_details3_list.Any())
                                {
                                    foreach (var boq_detail3 in boq_details3_list)
                                    {
                                        boq_details_3.Add(new BOQDetail3()
                                        {
                                            boq_detail3_id = boq_detail3.boq_detail3_id,
                                            boq_detail2_id = boq_detail3.boq_detail2_id,
                                            item_id = boq_detail3.item_id,
                                            item_qty = boq_detail3.item_qty,
                                            item_unit_price = boq_detail3.item_unit_price,
                                            p_category_id = boq_detail3.p_category_id,
                                            product_code = boq_detail3.product_code,
                                            product_name = boq_detail3.product_name,
                                            product_unit = boq_detail3.product_unit,
                                            type_group = count_boq_detail.ToString() + (Convert.ToChar(64 + count_boq_detail2)).ToString()
                                            //type_group=boq_detail2.type_group
                                        });
                                    }
                                }
                                /* end get boq detail 3 */
                                count_boq_detail2++;
                            }
                        }
                        /* end get boq detail 2 */
                        count_boq_detail++;
                    }
                }
                BOQ obj = new BOQ();
                boq.attachments = obj.GetBOQAttachments(boq.boq_id);
                boq.rejects = CommonClass.GetRejectByRequest(id);
                /* end get boq detail 1 */
                var results = new { result = "success", boq = boq, boq_details_1 = boq_details_1, boq_details_2 = boq_details_2, boq_details_3 = boq_details_3 };
                return boq;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GetBOQItemByJobCategory(string boq_id, string jcat_id)
        {
            try
            {
                List<BOQDetail3> boq_items = new List<BOQDetail3>();
                var item_lists = (from tbl in db.tb_boq_detail3
                                  join item in db.tb_product on tbl.item_id equals item.product_id
                                  join boq2 in db.tb_boq_detail2 on tbl.boq_detail2_id equals boq2.boq_detail2_id
                                  join boq1 in db.tb_boq_detail1 on boq2.boq_detail1_id equals boq1.boq_detail1_id
                                  where boq1.boq_id == boq_id && boq1.job_category_id == jcat_id
                                  select new BOQDetail3()
                                  {
                                      item_id = tbl.item_id,
                                      product_name = item.product_name,
                                      product_code = item.product_code,
                                      product_unit = item.product_unit,
                                      item_qty = tbl.item_qty
                                  }
                                ).ToList();

                if (item_lists.Any())
                {
                    foreach (var item in item_lists)
                    {
                        boq_items.Add(new BOQDetail3() { item_id = item.item_id, product_name = item.product_name, product_code = item.product_code, product_unit = item.product_unit, item_qty = item.item_qty });
                    }
                }
                return Json(new { result = "success", data = boq_items }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBOQItemByJobCategoryItemID(string boq_id, string jcat_id, string product_id)
        {
            try
            {
                List<BOQDetail3> boq_items = new List<BOQDetail3>();
                BOQDetail3 dItem = new BOQDetail3();
                var items = (from tbl in db.tb_boq_detail3
                             join item in db.tb_product on tbl.item_id equals item.product_id
                             join boq2 in db.tb_boq_detail2 on tbl.boq_detail2_id equals boq2.boq_detail2_id
                             join boq1 in db.tb_boq_detail1 on boq2.boq_detail1_id equals boq1.boq_detail1_id
                             where boq1.boq_id == boq_id && boq1.job_category_id == jcat_id && item.product_id == product_id
                             select new
                             {
                                 item_id = tbl.item_id,
                                 product_name = item.product_name,
                                 product_code = item.product_code,
                                 product_unit = item.product_unit,
                                 item_qty = tbl.item_qty
                             }
                             ).FirstOrDefault();
                return Json(new { result = "success", data = items }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBOQItemByItemID(string item_id,string type_id)
        {
            try
            {
                var items = (from item in db.tb_product
                             join type in db.tb_brand on item.brand_id equals type.brand_id
                             where item.status==true&& item.product_id == item_id && item.brand_id==type_id
                             select new ProductViewModel()
                             {
                                 product_id=item.product_id,
                                 product_name = item.product_name,
                                 product_code = item.product_code,
                                 product_unit = item.product_unit,
                             }
                                ).FirstOrDefault();
                return Json(new { result = "success", data = items }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "fail", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Project Manager,Director")]

        public ActionResult Approve(string id,string role)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                tb_build_of_quantity boq = db.tb_build_of_quantity.Find(id);
                if (string.Compare(role, "Project Manager") == 0)
                {
                    boq.boq_status = "Approved";
                    boq.checked_by = User.Identity.Name;
                    boq.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now).ToString();
                    db.SaveChanges();
                } else if (string.Compare(role, "Director") == 0)
                {
                    boq.boq_status = "Completed";
                    boq.approved_by = User.Identity.Name;
                    boq.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }

   #region add send mail july 11 2018

                var SelectUser = db.AspNetRoles.Where(i => i.Name == "Economic Engineer").ToList();
                try
                {

                    var senderEmail = new MailAddress("soklakhena999@gmail.com", "Kim Mex");
                    var password = "Lakhena12345";
                    var sub = "Approved";
                    var body = "approved";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    foreach (var i in SelectUser)
                    {
                        using (var mess = new MailMessage(senderEmail, new MailAddress(i.Name,"Customer"))
                        {
                            Subject = sub,
                            Body = body
                        })
                        {
                            smtp.Send(mess);
                        }
                    }
                    
               } catch (Exception ex) { }
  #endregion

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "Admin,Project Manager,Director")]
        public ActionResult Reject(string id,string role)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
              
                tb_build_of_quantity boq = db.tb_build_of_quantity.Find(id);
                if(string.Compare(role, "Project Manager") == 0)
                {
                    boq.boq_status = "Rejected";
                    boq.checked_by = User.Identity.Name;
                    boq.checked_date = Class.CommonClass.ToLocalTime(DateTime.Now).ToString();
                    db.SaveChanges();
                }else if(string.Compare(role, "Director") == 0)
                {
                    boq.boq_status = "Rejected";
                    boq.approved_by = User.Identity.Name;
                    boq.approved_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                }
     #region add send mail july 11 2018
                try
                {

                    var SelectUser = db.tb_user_detail.Where(i => i.user_position_id == "3" && i.status == true).ToList();
                    var senderEmail = new MailAddress("soklakhena999@gmail.com", "Kim Mex");
                    var password = "Lakhena12345";
                    var sub = "Reject";
                    var body = "rejected";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    foreach (var i in SelectUser) {
                        using (var mess = new MailMessage(senderEmail, new MailAddress(i.user_email,"customer"))
                        {
                            Subject = sub,
                            Body = body
                        })
                        {
                            smtp.Send(mess);
                        }

                    }
                }
     #endregion
                catch (Exception ex) { }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
        }
        #region Attachment
        public JsonResult UploadAttachment()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_attachment att = new tb_attachment();
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var file_name = Path.GetFileName(file.FileName);
                    var file_extension = Path.GetExtension(file_name);
                    var file_id = Guid.NewGuid().ToString();
                    var file_path = Path.Combine(Server.MapPath("~/Documents/Attachment"), file_id + file_extension);
                    file.SaveAs(file_path);
                    att.attachment_id = file_id;
                    att.attachment_name = file_name;
                    att.attachment_extension = file_extension;
                    att.attachment_path = file_path;
                    att.attachment_ref_type = "BOQ";
                    db.tb_attachment.Add(att);
                    db.SaveChanges();
                }
                return Json(new { result = "success",attachment_id=att.attachment_id }, JsonRequestBehavior.AllowGet);
            }
        }
        public FileResult Download(String p,String d)
        {
            return File(Path.Combine(Server.MapPath("~/Documents/Attachment/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
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
                tb_attachment att = db.tb_attachment.Find(id);
                if (att == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { result = "error" });
                }
                db.tb_attachment.Remove(att);
                db.SaveChanges();
                var path = Path.Combine(Server.MapPath("~/Documents/Attachment/"), att.attachment_id + att.attachment_extension);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                return Json(new { result = "ok" });
            }
        }
        #endregion
    }
}