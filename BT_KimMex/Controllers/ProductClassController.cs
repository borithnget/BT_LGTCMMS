using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ProductClassController : Controller
    {

        // GET: ProductClass
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductClass/Details/5

        public ActionResult Details(string id)
        {
            var model = CommonFunctions.GetProductClassItem(id);
            return View(model);
        }


        // GET: ProductClass/Create


        public ActionResult Create()
        {
            List<tb_class_type> classTypes = new List<tb_class_type>();
            classTypes = this.GetClassTypeDropdownList();
            ViewBag.ClassTypes = classTypes;
            return View();
        }

        private List<tb_class_type> GetClassTypeDropdownList()
        {
            List<tb_class_type> classType = new List<tb_class_type>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_class_type.ToList();
            }
            //return classType;
        }

        [HttpPost]


        public ActionResult Create(ProductClassViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            kim_mexEntities db = new kim_mexEntities();
            tb_product_class productclass = new tb_product_class();
            productclass.product_class_id = Guid.NewGuid().ToString();
            productclass.product_class_name = model.product_class_name;
            productclass.class_type_id = model.class_type_id;
            productclass.active = true;
            productclass.created_at = DateTime.Now;
            productclass.updated_at = DateTime.Now;
            productclass.created_by = User.Identity.GetUserId();
            productclass.updated_by = User.Identity.GetUserId();
            db.tb_product_class.Add(productclass);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: ProductClass/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetProductClassItem(id);
            return View(model);
        }

        // POST: ProductClass/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ProductClassViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_product_class productClass = db.tb_product_class.Find(id);
                productClass.class_type_id = model.class_type_id;
                productClass.product_class_name = model.product_class_name;
                productClass.updated_at = DateTime.Now;
                productClass.updated_by = User.Identity.GetUserId();
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductClass/Delete/5
        public ActionResult Delete(string id)
        {
            //kim_mexEntities db = new kim_mexEntities();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_product_class productClass = db.tb_product_class.Find(id);
                productClass.active = false;
                productClass.updated_at = DateTime.Now;
                productClass.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
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
            return View(Index);
        }

        private ActionResult View(Func<ActionResult> index)
        {
            throw new NotImplementedException();
        }


        //public ActionResult Delete(string id)
        //{
        //    //kim_mexEntities db = new kim_mexEntities();
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        tb_product_class productClass = db.tb_product_class.Find(id);
        //        productClass.active = false;
        //        productClass.updated_at = DateTime.Now;
        //        productClass.updated_by = User.Identity.GetUserId();
        //        db.SaveChanges();
        //        return Json(new
        //        {
        //            success = true,
        //            Message = "Success",
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public ActionResult GetProductClassListJson()
        {
            return Json(new { data = CommonFunctions.GetProductClassListItems() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductclassDropdownList(string group_id="")
        {
            List<ProductClassViewModel> productclass = new List<ProductClassViewModel>();
            productclass = CommonFunctions.GetProductClassListItems(group_id);

            return Json(new { result = "success", data = productclass }, JsonRequestBehavior.AllowGet);
        }

        //private List<tb_producttype> GetProductclassList(int iid)
        //{
        //    throw new NotImplementedException();
        //}

        public ActionResult GetAllProductclassDropdown()
        {
            List<ProductClassViewModel> warehouses = new List<ProductClassViewModel>();
            warehouses = this.GetAllProductclass();
            return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        }

        private List<ProductClassViewModel> GetAllProductclass()
        {
            throw new NotImplementedException();
        }
    }

    internal class tb_producttype
    {
    }
}