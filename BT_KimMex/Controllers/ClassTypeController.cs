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
    public class ClassTypeController : Controller
    {
        // GET: ClassType
        public ActionResult Index()
        {
            return View();
        }

        // GET: ClassType/Details/5
        public ActionResult Details(string id)
        {
            return View(CommonFunctions.GetClassTypeItem(id));
        }

        // GET: ClassType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClassType/Create
        [HttpPost]
        public ActionResult Create(ClassTypeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            kim_mexEntities db = new kim_mexEntities();
            tb_class_type classType = new tb_class_type();
            classType.class_type_id = Guid.NewGuid().ToString();
            classType.class_type_name = model.class_type_name;
            classType.active = true;
            classType.created_at = DateTime.Now;
            classType.updated_at = DateTime.Now;
            classType.created_by = User.Identity.GetUserId();
            classType.updated_by = User.Identity.GetUserId();
            db.tb_class_type.Add(classType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ClassType/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetClassTypeItem(id);
            return View(model);
        }

        // POST: ClassType/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ClassTypeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            kim_mexEntities db = new kim_mexEntities();
            tb_class_type classType = db.tb_class_type.Find(id);
            classType.class_type_name = model.class_type_name;
            classType.updated_at = DateTime.Now;
            classType.updated_by = User.Identity.GetUserId();
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ClassType/Delete/5
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_class_type classType = db.tb_class_type.Find(id);
                classType.active = false;
                classType.updated_at = DateTime.Now;
                classType.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //try
        //{
        //    kim_mexEntities db = new kim_mexEntities();
        //    tb_class_type classType = db.tb_class_type.Find(id);
        //    classType.active = false;
        //    classType.updated_at = DateTime.Now;
        //    classType.updated_by = User.Identity.GetUserId();
        //    db.SaveChanges();
        //    return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
        //}catch(Exception ex)
        //{
        //    return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult ClassTypeListJSON()
        {
            return Json(new { data = Class.CommonFunctions.GetClassTypeListItems() }, JsonRequestBehavior.AllowGet);
        }
    }
}
