using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BT_KimMex.Controllers
{
    public class ErrorLogController : Controller
    {
        // GET: ErrorLog
        public ActionResult Index()
        {
            return View(ErrorLog.ErrorLogger.GetErrorLogs());
        }

        // GET: ErrorLog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ErrorLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ErrorLog/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ErrorLog/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ErrorLog/Edit/5
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

        // GET: ErrorLog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ErrorLog/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
