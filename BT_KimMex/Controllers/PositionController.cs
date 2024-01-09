using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    public class PositionController : Controller
    {
        // GET: Position
        public ActionResult Index()
        {
            kim_mexEntities db = new kim_mexEntities();
            return View(db.tb_position.OrderBy(s=>s.position_name).Where(s=>s.status==true).Select(s=>new PositionViewModel() {position_id=s.position_id,position_name=s.position_name }).ToList());
        }

        // GET: Position/Details/5
        public ActionResult Details(string id)
        {
            return View(this.GetPositionDetail(id));
        }

        // GET: Position/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Position/Create
        [HttpPost]
        public ActionResult Create(PositionViewModel collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    kim_mexEntities db = new kim_mexEntities();
                    tb_position position = new tb_position();
                    position.position_id = Guid.NewGuid().ToString();
                    position.position_name = collection.position_name;
                    position.status = true;
                    position.created_by = User.Identity.GetUserId();
                    position.updated_by = User.Identity.GetUserId();
                    position.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    position.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_position.Add(position);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(collection);
            }
            catch
            {
                return View();
            }
        }

        // GET: Position/Edit/5
        public ActionResult Edit(string id)
        {
            return View(this.GetPositionDetail(id));
        }

        // POST: Position/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, PositionViewModel collection)
        {
            try
            {
                if (!ModelState.IsValid) return View(collection);
                kim_mexEntities db = new kim_mexEntities();
                tb_position position = db.tb_position.Find(id);
                position.position_name = collection.position_name;
                position.updated_by = User.Identity.GetUserId();
                position.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Position/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        public PositionViewModel GetPositionDetail(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_position.Where(s => string.Compare(s.position_id, id) == 0).Select(s => new PositionViewModel()
                {
                    position_id = s.position_id,
                    position_name = s.position_name
                }).FirstOrDefault();
            }
        }
    }
}
