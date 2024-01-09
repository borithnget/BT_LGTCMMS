using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class JobCategoryController : Controller
    {
        // GET: JobCategory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(JobCategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_job_category job_category = new tb_job_category();
                    job_category.j_category_id = Guid.NewGuid().ToString();
                    job_category.j_category_name = model.j_category_name;
                    job_category.j_description = model.j_description;
                    job_category.j_status = true;
                    job_category.created_by = User.Identity.Name;
                    job_category.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_job_category.Add(job_category);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = "Your data is error while saving!";
            }
            return View();
        }
        public ActionResult Details(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                JobCategoryViewModel job_category = new JobCategoryViewModel();
                var job_category_detail = (from tbl in db.tb_job_category where tbl.j_category_id == id select tbl).FirstOrDefault();
                if (job_category_detail != null)
                {
                    job_category.j_category_id = job_category_detail.j_category_id;
                    job_category.j_category_name = job_category_detail.j_category_name;
                    job_category.j_description = job_category_detail.j_description;
                    job_category.created_date = job_category_detail.created_date;
                }
                return View(job_category);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult Edit(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                JobCategoryViewModel job_category = new JobCategoryViewModel();
                var job_category_detail = (from tbl in db.tb_job_category where tbl.j_category_id == id select tbl).FirstOrDefault();
                if (job_category_detail != null)
                {
                    job_category.j_category_id = job_category_detail.j_category_id;
                    job_category.j_category_name = job_category_detail.j_category_name;
                    job_category.j_description = job_category_detail.j_description;
                    job_category.created_date = job_category_detail.created_date;
                }
                return View(job_category);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Edit(JobCategoryViewModel job_category_vm,string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_job_category job_category = db.tb_job_category.FirstOrDefault(m => m.j_category_id == id);
                    job_category.j_category_name = job_category_vm.j_category_name;
                    job_category.j_description = job_category_vm.j_description;
                    job_category.j_status = true;
                    job_category.updated_by = User.Identity.Name;
                    job_category.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = "Your data is error been updating!";
            }
            return View();
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_job_category job_category = db.tb_job_category.FirstOrDefault(m => m.j_category_id == id);
                job_category.j_status = false;
                job_category.updated_by = User.Identity.Name;
                job_category.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
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
            ViewBag.ReturnUrl = Url.Action("Message");
            return View();
        }
        public ActionResult JobCategoryDataTable()
        {
            try
            {
                List<JobCategoryViewModel> job_categories = new List<JobCategoryViewModel>();
                kim_mexEntities db = new kim_mexEntities();
                var job_categories_list = (from tbl in db.tb_job_category orderby tbl.j_category_name where tbl.j_status == true select tbl).ToList();
                if (job_categories_list.Any())
                {
                    foreach(var job_category in job_categories_list)
                    {
                        job_categories.Add(new JobCategoryViewModel() { j_category_id = job_category.j_category_id, j_category_name = job_category.j_category_name, j_description = job_category.j_description, created_date = job_category.created_date });
                    }
                }
                return Json(new { data = job_categories }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return null;
            }
        }
        public ActionResult JobCategoriesDropdownList()
        {
            try
            {
                List<JobCategoryViewModel> job_categories = new List<JobCategoryViewModel>();
                kim_mexEntities db = new kim_mexEntities();
                var job_categories_list = (from tbl in db.tb_job_category orderby tbl.j_category_name where tbl.j_status == true select tbl).ToList();
                if (job_categories_list.Any())
                {
                    foreach(var job_category in job_categories_list)
                    {
                        job_categories.Add(new JobCategoryViewModel() { j_category_id = job_category.j_category_id, j_category_name = job_category.j_category_name });
                    }
                }
                return Json(new { data = job_categories }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return null;
            }
        }
    }
}