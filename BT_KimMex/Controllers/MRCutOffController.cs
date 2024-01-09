using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    public class MRCutOffController : Controller
    {
        // GET: MRCutOff
        public ActionResult Index()
        {
            return View(ClsMRCutOff.GetMRCutOffListItems(User.IsInRole(Role.SystemAdmin),User.IsInRole(Role.Purchaser),User.IsInRole(Role.SiteSupervisor),User.Identity.GetUserId().ToString()));
        }

        // GET: MRCutOff/Details/5
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            return View(ClsMRCutOff.GetMRCutOffItem(id));
        }

        // GET: MRCutOff/Create
        public ActionResult Create(string id=null)
        {
            ViewBag.MCONumber = CommonClass.GenerateProcessNumber("MCO");
            MRCutOffViewModel model = new MRCutOffViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                kim_mexEntities db = new kim_mexEntities();
                model.material_request_id = id;
                model.project_id= db.tb_item_request.Find(id).ir_project_id;
                model.materialRequests = ClsMRCutOff.GetAllItemRequestDropdownList().Where(w => string.Compare(w.ir_project_id, model.project_id) == 0).ToList();
                model.materialRequestItems = ItemRequest.GetMaterialRequestListItems(id);
            }
            return View(model);
        }

        // POST: MRCutOff/Create
        [HttpPost]
        public ActionResult Create(MRCutOffViewModel model)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_mr_cut_off mco = new tb_mr_cut_off();
                mco.mr_cut_off_id = Guid.NewGuid().ToString();
                mco.mr_cut_off_number = CommonClass.GenerateProcessNumber("MCO");
                mco.material_request_id = model.material_request_id;
                mco.active = true;
                mco.created_by = User.Identity.GetUserId();
                mco.updated_by = User.Identity.GetUserId();
                mco.created_at = CommonClass.ToLocalTime(DateTime.Now);
                mco.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                mco.mr_cut_off_status = Status.Pending;
                db.tb_mr_cut_off.Add(mco);

                foreach(var item in model.mrCutOffDetail)
                {
                    if (item.cut_off_qty > 0)
                    {
                        tb_mr_cut_off_detail mcod = new tb_mr_cut_off_detail();
                        mcod.cut_off_detail_id = Guid.NewGuid().ToString();
                        mcod.cut_off_id = mco.mr_cut_off_id;
                        mcod.item_id = item.item_id;
                        mcod.item_unit_id = item.item_unit_id;
                        mcod.material_request_qty = item.material_request_qty;
                        mcod.cut_off_qty = item.cut_off_qty;
                        mcod.cut_off_reason = item.cut_off_reason;
                        mcod.item_status = Status.Pending;
                        db.tb_mr_cut_off_detail.Add(mcod);
                        db.SaveChanges();
                    }
                }

                tb_item_request mr = db.tb_item_request.Find(mco.material_request_id);
                mr.is_cut_off = true;
                db.SaveChanges();

                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MRCutOff/Edit/5
        public ActionResult RequestCancel(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_mr_cut_off mrCutOff = db.tb_mr_cut_off.Find(id);
                mrCutOff.mr_cut_off_status = Status.RequestCancelled;
                mrCutOff.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                mrCutOff.updated_by = User.Identity.GetUserId().ToString();
                db.SaveChanges();
                tb_item_request mr = db.tb_item_request.Find(mrCutOff.material_request_id);
                mr.is_cut_off = false;
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: MRCutOff/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Approval(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            return View(ClsMRCutOff.GetMRCutOffItem(id));
        }

        [HttpPost]
        public ActionResult Approval(string id, List<MRCutOffDetailViewModel> models)
        {
            try
            {
                // TODO: Add delete logic here
                kim_mexEntities db = new kim_mexEntities();
                tb_mr_cut_off cutoff = db.tb_mr_cut_off.Find(id);
                cutoff.mr_cut_off_status = Status.Approved;
                cutoff.approved_at = CommonClass.ToLocalTime(DateTime.Now);
                cutoff.approved_by = User.Identity.GetUserId().ToString();
                db.SaveChanges();
                foreach(var item in models)
                {
                    string codID = item.cut_off_detail_id;
                    tb_mr_cut_off_detail cutoff_detail = db.tb_mr_cut_off_detail.Find(codID);
                    cutoff_detail.item_status = item.item_status;
                    cutoff_detail.approval_comment = item.approval_comment;
                    db.SaveChanges();
                    //update material item request qty
                    if (string.Compare(cutoff_detail.item_status, Status.Approved) == 0)
                    {
                        tb_ir_detail2 mrDetail = (from mr1 in db.tb_ir_detail1
                                                  join mr2 in db.tb_ir_detail2 on mr1.ir_detail1_id equals mr2.ir_detail1_id
                                                  where string.Compare(mr1.ir_id, cutoff.material_request_id) == 0 && string.Compare(mr2.ir_item_id, cutoff_detail.item_id) == 0
                                                  select mr2).FirstOrDefault();
                        mrDetail.ir_qty = mrDetail.ir_qty-(mrDetail.remain_qty-cutoff_detail.cut_off_qty);
                        mrDetail.approved_qty=mrDetail.approved_qty- (mrDetail.remain_qty - cutoff_detail.cut_off_qty);
                        mrDetail.remain_qty = cutoff_detail.cut_off_qty;
                        db.SaveChanges();
                    }
                    
                }
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
