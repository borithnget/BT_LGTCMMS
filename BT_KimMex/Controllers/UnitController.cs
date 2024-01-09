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
    public class UnitController : Controller
    {
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(UnitViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_unit unit = new tb_unit();
                    unit.Id = Guid.NewGuid().ToString();
                    unit.Name = model.Name;
                    unit.unit_description = model.unit_description;
                    unit.status = true;
                    unit.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    unit.created_by = User.Identity.Name;
                    db.tb_unit.Add(unit);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = "Your data is error been saving!";
            }
            return View();
        }
        public ActionResult Details(string Id)
        {
            UnitViewModel unit = new UnitViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var unitDetail = (from tbl in db.tb_unit where tbl.Id == Id select tbl).FirstOrDefault();
                if (unitDetail != null)
                {
                    unit = new UnitViewModel();
                    unit.Id = unitDetail.Id;
                    unit.Name = unitDetail.Name;
                    unit.unit_description = unitDetail.unit_description;
                    unit.created_date = unitDetail.created_date;
                }
            }catch(Exception ex) { }
            return View(unit);
        }
        public ActionResult Edit(string Id)
        {
            UnitViewModel unit = new UnitViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var unitDetail = (from tbl in db.tb_unit where tbl.Id == Id select tbl).FirstOrDefault();
                if (unitDetail != null)
                {
                    unit = new UnitViewModel();
                    unit.Id = unitDetail.Id;
                    unit.Name = unitDetail.Name;
                    unit.unit_description = unitDetail.unit_description;
                    unit.created_date = unitDetail.created_date;
                }
            }
            catch (Exception ex) { }
            return View(unit);
        }
        [HttpPost]
        public ActionResult Edit(UnitViewModel unitVM,string Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    kim_mexEntities db = new kim_mexEntities();
                    tb_unit unit = db.tb_unit.FirstOrDefault(m => m.Id == Id);
                    unit.Name = unitVM.Name;
                    unit.unit_description = unitVM.unit_description;
                    unit.status = true;
                    unit.updated_by = User.Identity.Name;
                    unit.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = "Your data is error been updating!";
            }
            return View(unitVM);
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_unit unit = db.tb_unit.FirstOrDefault(m => m.Id == id);
                unit.status = false;
                unit.updated_by = User.Identity.Name;
                unit.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
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
            return View();
        }
        public ActionResult UnitDataTable()
        {
            List<UnitViewModel> units = new List<UnitViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var unitList = (from tbl in db.tb_unit orderby tbl.Name where tbl.status==true select tbl).ToList();
                if (unitList.Any())
                {
                    foreach(var unit in unitList)
                    {
                        units.Add(new UnitViewModel() { Id = unit.Id, Name = unit.Name,unit_description=unit.unit_description,created_date=unit.created_date });
                    }
                }
            }catch(Exception ex) { }
            return Json(new { data = units }, JsonRequestBehavior.AllowGet);
        }
    }
}