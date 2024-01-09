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
    public class ClassController : Controller
    {

        // GET: Class
        public ActionResult Index()
        {
            return View();
        }

        // GET: Class/Details/5
        public ActionResult Details(string id)
        {
            var model = CommonFunctions.GetClassItem(id);
            return View(model);
        }

        // GET: Class/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Class/Create
        [HttpPost]
        public ActionResult Create(ClassViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_class classs = new tb_class();
                classs.class_id = Guid.NewGuid().ToString();
                classs.class_type_id = model.class_type_id;
                classs.class_code = string.IsNullOrEmpty(model.class_code) ? ClassViewModel.GenerateGroupCode() : model.class_code;
                classs.class_name = model.class_name;
                classs.active = true;
                classs.created_at = DateTime.Now;
                classs.updated_at = DateTime.Now;
                classs.created_by = User.Identity.GetUserId();
                classs.updated_by = User.Identity.GetUserId();
                db.tb_class.Add(classs);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetClassItem(id);
            return View(model);
          
        }

        // POST: Class/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ClassViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_class classs = db.tb_class.Find(id);
                classs.class_type_id = model.class_type_id;
                classs.class_name = model.class_name;
                classs.class_code= model.class_code;
                classs.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                classs.updated_by = User.Identity.GetUserId();
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Class/Delete/5
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_class classs = db.tb_class.Find(id);
                classs.active = false;
                classs.updated_at = DateTime.Now;
                classs.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetClassListJson()
        {
            return Json(new { data = CommonFunctions.GetClassListItems() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassDataDropdownlist()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var model = db.tb_class.OrderBy(o => o.class_name).Where(w => w.active == true).Select(s => new ClassViewModel()
                {
                    class_id = s.class_id,
                    class_name = s.class_name,
                }).ToList();
                return Json(new { result = "success", data = model }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
