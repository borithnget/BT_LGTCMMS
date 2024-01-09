using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using System.IO;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ProductCategoryController : Controller
    {
        private object categoriesList;

        // GET: ProductCategory
        public ActionResult Index()
        {
            return View();

            //List<CategoryViewModel> itemTypes = new List<CategoryViewModel>();
            //using (kim_mexEntities db = new kim_mexEntities())
            //{
            //    itemTypes = (from it in db.tb_product_category
            //                 join cl in db.tb_class on it.class_id equals cl.class_id
            //                 where it.status == true
            //                 select new CategoryViewModel()
            //                 {
            //                     p_category_id = it.p_category_id,
            //                     p_category_code = it.p_category_code,
            //                     p_category_name = it.p_category_name,
            //                     created_date = it.created_date,
            //                     class_id = it.class_id,
            //                     class_name = cl.class_name
            //                 }).ToList();
            //}
            //return View(itemTypes);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CategoryViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View();
                kim_mexEntities db = new kim_mexEntities();
                tb_product_category category = new tb_product_category();
                category.p_category_id = Guid.NewGuid().ToString();
                category.p_category_code = model.p_category_code;
                category.p_category_name = model.p_category_name;
                category.chart_account = model.chart_account;
                category.class_id = model.class_id;
                category.sub_group_id= model.sub_group_id;
                category.status = true;
                category.created_by = User.Identity.GetUserId();
                category.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                category.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                category.updated_by = User.Identity.GetUserId();
                db.tb_product_category.Add(category);
                db.SaveChanges();
                TempData["message"] = "Your data has been saved!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["message"] = "Your data is error while saving!";
            }
            return View();
        }
        public ActionResult Details(string id)
        {
            //CategoryViewModel category = new CategoryViewModel();
            //using (kim_mexEntities db = new kim_mexEntities())
            //{
            //    var categoryDetail = (
            //        from tbl in db.tb_product_category where tbl.p_category_id == id select tbl).FirstOrDefault();
            //    if (categoryDetail != null)
            //    {
            //        category.p_category_id = categoryDetail.p_category_id;
            //        category.p_category_code = categoryDetail.p_category_code;
            //        category.p_category_name = categoryDetail.p_category_name;
            //        category.p_category_address = categoryDetail.p_category_address;
            //        category.p_class_name = categoryDetail.class_id;
            //        category.created_date = categoryDetail.created_date;
            //        category.chart_account = categoryDetail.chart_account;
            //        category.class_id = category.class_id;

            //    }
            //}
            return View(CommonFunctions.GetCategoryItem(id));
        }
        public ActionResult Edit(string id)
        {

            //CategoryViewModel category = new CategoryViewModel();
            //using (kim_mexEntities db = new kim_mexEntities())
            //{
            //    var categoryDetail = (from tbl in db.tb_product_category where tbl.p_category_id == id select tbl).FirstOrDefault();
            //    if (categoryDetail != null)
            //    {
            //        category.class_id = categoryDetail.class_id;
            //        category.p_category_id = categoryDetail.p_category_id;
            //        category.p_category_code = categoryDetail.p_category_code;
            //        category.p_category_name = categoryDetail.p_category_name;
            //        category.p_category_address = categoryDetail.p_category_address;
            //        category.created_date = categoryDetail.created_date;
            //        category.chart_account = categoryDetail.chart_account;
            //    }
            //}
            //return View(category);
            return View(CommonFunctions.GetCategoryItem(id));
        }
        [HttpPost]
        public ActionResult Edit(CategoryViewModel categoryVM, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_product_category category = db.tb_product_category.FirstOrDefault(model => model.p_category_id == id);
                    category.p_category_code = categoryVM.p_category_code;
                    category.p_category_name = categoryVM.p_category_name;
                    category.p_category_address = categoryVM.p_category_address;
                    category.class_id = categoryVM.class_id;
                    category.chart_account = categoryVM.chart_account;
                    category.status = true;
                    category.updated_by = User.Identity.Name;
                    category.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Your data is error been updating!";
            }
            return View();
        }
        public ActionResult Delete(string id)
        {
            //kim_mexEntities db = new kim_mexEntities

            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_product_category productCategory = db.tb_product_category.Find(id);
                productCategory.status = false;
                productCategory.updated_date = DateTime.Now;
                productCategory.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult Delete(string id)
        //{
        //    try
        //    {
        //        kim_mexEntities db = new kim_mexEntities();
        //        tb_product_category p_category = db.tb_product_category.FirstOrDefault(m => m.p_category_id == id);
        //        p_category.status = false;
        //        p_category.updated_by = User.Identity.Name;
        //        p_category.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
        //        db.SaveChanges();
        //        return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult Message(EnumConstants.MessageParameter? message)
        //{
        //    ViewBag.StatusMessage = message == EnumConstants.MessageParameter.SaveSuccessfull ? "Your data has been saved!" :
        //        message == EnumConstants.MessageParameter.SaveError ? "Your data is error while saving!" :
        //        message == EnumConstants.MessageParameter.UpdateSuccessfull ? "Your data has been updated!" :
        //        message == EnumConstants.MessageParameter.UpdateError ? "Your data is error while updating!" :
        //        message == EnumConstants.MessageParameter.DeleteSuccessfull ? "Your data has been deleted!" :
        //        message == EnumConstants.MessageParameter.DeleteError ? "Your data is error while deleting"
        //        : "";
        //    ViewBag.ReturnUrl = Url.Action("Message");
        //    return View();
        //}
        public ActionResult ProductCategoryDataTable()
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                categories = (from tbl in db.tb_product_category
                              join cl in db.tb_class on tbl.class_id equals cl.class_id
                              where !string.IsNullOrEmpty(tbl.class_id) && tbl.status == true
                              select new CategoryViewModel
                              {
                                  p_category_id = tbl.p_category_id,
                                  p_category_code = tbl.p_category_code,
                                  p_category_name = tbl.p_category_name,
                                  class_name = cl.class_name,
                                  p_category_address = tbl.p_category_address,
                                  created_date = tbl.created_date,
                                  updated_date = tbl.updated_date,
                                  chart_account = tbl.chart_account,
                              }).ToList();
                //categories = categoriesList.ToList();
                //if (categoriesList.Any())
                //{
                //    foreach (var category in categoriesList)
                //    {
                //        categories.Add(new CategoryViewModel() {
                //            p_category_id = category.p_category_id,
                //            p_category_code =category.p_category_code,
                //            p_category_name = category.p_category_name,
                //            p_category_address = category.p_category_address,
                //            created_date =category.created_date,
                //            chart_account =category.chart_account
                //        });
                //    }
                //}
            }
            return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductCategoryCodes(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json(db.tb_product_category.Where(c => c.p_category_code.StartsWith(term) && c.status == true).Select(a => new { label = a.p_category_code + " " + a.p_category_name, id = a.p_category_id }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetItemTypeDataTable()
        {
            List<CategoryViewModel> itemTypes = new List<CategoryViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                itemTypes = (from it in db.tb_product_category
                             join cl in db.tb_class on it.class_id equals cl.class_id into pcl from cl in pcl.DefaultIfEmpty()
                             join sg in db.tb_sub_group on it.sub_group_id equals sg.sub_group_id into psg from sg in psg.DefaultIfEmpty()
                             where it.status == true && !string.IsNullOrEmpty(it.p_category_name)
                             select new CategoryViewModel()
                             {
                                 p_category_id = it.p_category_id,
                                 p_category_code = it.p_category_code,
                                 p_category_name = it.p_category_name,
                                 created_date = it.created_date,
                                 updated_date = it.updated_date,
                                 class_id = it.class_id,
                                 class_name = cl.class_name,
                                 sub_group_id=sg.sub_group_id,
                                 sub_group_name=sg.sub_group_name
                             }).ToList();
            }
            return Json(new { data = itemTypes }, JsonRequestBehavior.AllowGet);
        }
        [WebMethod]
        //public List<CategoryViewModel> GetProductCategoryCodes(string term)
        //{
        //    kim_mexEntities db = new kim_mexEntities();
        //    List<CategoryViewModel> categories = new List<CategoryViewModel>();
        //    var fetch_category = db.tb_product_category.Where(m => m.p_category_code.ToLower().StartsWith(term.ToLower()) && m.status == true).ToList();
        //    if (fetch_category.Any())
        //    {
        //        foreach (var category in fetch_category)
        //        {
        //            categories.Add(new CategoryViewModel() { p_category_code = fetch_category.ToString() });
        //        }
        //    }
        //    return categories;
        //}
        public ActionResult GetProjectCategoryNames(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json(db.tb_product_category.Where(c => c.p_category_name.StartsWith(term) && c.status == true).Select(a => new { label = a.p_category_name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductCategoryInfo(string category_id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                CategoryViewModel category = new CategoryViewModel();
                var category_info = (from tbl in db.tb_product_category orderby tbl.p_category_code where tbl.p_category_id == category_id && tbl.status == true select tbl).FirstOrDefault();
                if (category_info != null)
                {
                    category = new CategoryViewModel();
                    category.p_category_id = category_info.p_category_id;
                    category.p_category_code = category_info.p_category_code;
                    category.p_category_name = category_info.p_category_name;
                    category.chart_account = category_info.chart_account;
                }
                return Json(new { data = category }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GetItemTypeDataDropdownlist()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var categories = (from it in db.tb_product_category orderby it.p_category_code where it.status == true select new { item_type_id = it.p_category_id, item_type_name = it.p_category_name }).ToList();
                return Json(new { result = "success", data = categories }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetCategoryDataDropdownlistbyClassId(string id,string categoryId)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(categoryId))
                {
                    var categories = db.tb_product_category.OrderBy(o => o.p_category_name).Where(w => w.status == true && !string.IsNullOrEmpty(w.p_category_name) ).Select(s => new CategoryViewModel()
                    {
                        p_category_id = s.p_category_id,
                        p_category_name = s.p_category_name
                    }).ToList();
                    return Json(new { result = "success", data = categories }, JsonRequestBehavior.AllowGet);
                }
                else if (!string.IsNullOrEmpty(categoryId))
                {
                    var categories = db.tb_product_category.OrderBy(o => o.p_category_name).Where(w => w.status == true && string.Compare(w.sub_group_id, categoryId) == 0 && !string.IsNullOrEmpty(w.p_category_name)).Select(s => new CategoryViewModel()
                    {
                        p_category_id = s.p_category_id,
                        p_category_name = s.p_category_name
                    }).ToList();
                    return Json(new { result = "success", data = categories }, JsonRequestBehavior.AllowGet);
                }
                else if(!string.IsNullOrEmpty(id))
                {
                    var categories = db.tb_product_category.OrderBy(o => o.p_category_name).Where(w => w.status == true && string.Compare(w.class_id, id) == 0 && !string.IsNullOrEmpty(w.p_category_name)).Select(s => new CategoryViewModel()
                    {
                        p_category_id = s.p_category_id,
                        p_category_name = s.p_category_name
                    }).ToList();
                    return Json(new { result = "success", data = categories }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = "success", data = new List<CategoryViewModel>() }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult UpdateProductCategoryViaExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateProductCategoryViaExcel(string fileName)
        {
            if (Request.Files.Count > 0)
            {
                List<ExcelProductCategoryModel> models= new List<ExcelProductCategoryModel>();
                UpdateProductCategoryViaExcelResultResponse response = new UpdateProductCategoryViaExcelResultResponse();
                var file0 = Request.Files[0];
                if (file0 != null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    response = UpdateProductCategoryViaExcelModel.GetDataFromExcelContent(path, true);
                    return View(response);
                }

            }
            return View();
        }
    }
}