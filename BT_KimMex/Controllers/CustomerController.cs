using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    tb_customer customer = new tb_customer();
                    customer.customer_id = Guid.NewGuid().ToString();
                    customer.customer_name = model.customer_name;
                    customer.customer_address = model.customer_address;
                    customer.customer_email = model.customer_email;
                    customer.customer_phone = model.customer_phone;
                    customer.customer_created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    customer.customer_created_by = User.Identity.Name;
                    customer.status = true;
                    db.tb_customer.Add(customer);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = "Your data is error while saving!";
            return View();
        }
        public ActionResult CreateJson(string customer_name, string customer_telephone, string customer_email, string customer_address)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_customer customer = new tb_customer();
                customer.customer_id = Guid.NewGuid().ToString();
                customer.customer_name = customer_name;
                customer.customer_address = customer_address;
                customer.customer_email = customer_email;
                customer.customer_phone = customer_telephone;
                customer.customer_created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                customer.customer_created_by = User.Identity.Name;
                customer.status = true;
                db.tb_customer.Add(customer);
                db.SaveChanges();
                return Json(new { Message = "Success",cus_id=customer.customer_id }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult Details(string id)
        {
            CustomerViewModel customer = new CustomerViewModel();
            using (kim_mexEntities db=new kim_mexEntities())
            {
                var customerDetail = (from tbl in db.tb_customer where tbl.customer_id == id select tbl).FirstOrDefault();
                if (customerDetail != null)
                {
                    customer.customer_id = customerDetail.customer_id;
                    customer.customer_name = customerDetail.customer_name;
                    customer.customer_address = customerDetail.customer_address;
                    customer.customer_email = customerDetail.customer_email;
                    customer.customer_phone = customerDetail.customer_phone;
                    customer.customer_created_date = customerDetail.customer_created_date;
                }
            }
            return View(customer);
        }
        public ActionResult Edit(string id)
        {
            CustomerViewModel customer = new CustomerViewModel();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var customerDetail = (from tbl in db.tb_customer where tbl.customer_id == id select tbl).FirstOrDefault();
                if (customerDetail != null)
                {
                    customer.customer_id = customerDetail.customer_id;
                    customer.customer_name = customerDetail.customer_name;
                    customer.customer_address = customerDetail.customer_address;
                    customer.customer_email = customerDetail.customer_email;
                    customer.customer_phone = customerDetail.customer_phone;
                    customer.customer_created_date = customerDetail.customer_created_date;
                }
            }
            return View(customer);
        }
        [HttpPost]
        public ActionResult Edit(CustomerViewModel customerVM,string id)
        {
            if (ModelState.IsValid)
            {
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_customer customer = db.tb_customer.FirstOrDefault(model => model.customer_id == id);
                    customer.customer_name = customerVM.customer_name;
                    customer.customer_address = customerVM.customer_address;
                    customer.customer_email = customerVM.customer_email;
                    customer.customer_phone = customerVM.customer_phone;
                    customer.customer_updated_by = User.Identity.Name;
                    customer.customer_updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    customer.status = true;
                    db.SaveChanges();
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = "Your data has been updating!";
            return View();
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_customer customer = db.tb_customer.FirstOrDefault(m => m.customer_id == id);
                customer.status = false;
                customer.customer_updated_by = User.Identity.Name;
                customer.customer_updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
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
            ViewBag.ReturnUrl = Url.Action("Message");
            return View();
        }
        public ActionResult CustomerDataTable()
        {
            List<CustomerViewModel> customers = new List<CustomerViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var customerList = (from tbl in db.tb_customer orderby tbl.customer_name where tbl.status==true select tbl);
                if (customerList.Any())
                {
                    foreach (var customer in customerList)
                    {
                        customers.Add(new CustomerViewModel() { customer_id = customer.customer_id, customer_name = customer.customer_name, customer_address = customer.customer_address, customer_email = customer.customer_email, customer_phone = customer.customer_phone,customer_created_date=customer.customer_created_date });
                    }
                }
            }catch(Exception ex) { }
            return Json(new { data = customers }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var customers = (from tbl in db.tb_customer orderby tbl.customer_name where tbl.status==true select tbl).ToList();
                return Json(new { data = customers }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex) { return null; }
        }
    }
}