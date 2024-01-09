using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Class;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        // GET: Supplier
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(SupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    tb_supplier supplier = new tb_supplier();
                    supplier.supplier_id = Guid.NewGuid().ToString();
                    supplier.supplier_name = model.supplier_name;
                    supplier.supplier_email = model.supplier_email;
                    supplier.supplier_address = model.supplier_address;
                    supplier.supplier_contact_person = model.supplier_contact_person;
                    supplier.supplier_phone = model.supplier_phone;
                    supplier.discount = model.discount;
                    supplier.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    supplier.created_by = User.Identity.GetUserId();
                    supplier.status = true;
                    if (string.Compare(model.supplier_type, "local") == 0)
                        supplier.is_local = true;
                    else
                        supplier.is_local = false;
                    supplier.incoterm = model.incoterm;
                    supplier.payment = model.payment;
                    supplier.delivery = model.delivery;
                    supplier.shipment = model.shipment;
                    supplier.warranty = model.warranty;
                    supplier.vendor_ref = model.vendor_ref;
                    db.tb_supplier.Add(supplier);
                    db.SaveChanges();
                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = "Your data is error been saving!";
            return View();
        }
        [HttpPost]
        public ActionResult CreateJson(SupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    tb_supplier supplier = new tb_supplier();
                    supplier.supplier_id = Guid.NewGuid().ToString();
                    supplier.supplier_name = model.supplier_name;
                    supplier.supplier_email = model.supplier_email;
                    supplier.supplier_address = model.supplier_address;
                    supplier.supplier_contact_person = model.supplier_contact_person;
                    supplier.supplier_phone = model.supplier_phone;
                    supplier.discount = model.discount;
                    supplier.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    supplier.created_by = User.Identity.GetUserId();
                    supplier.status = true;
                    if (string.Compare(model.supplier_type, "local") == 0)
                        supplier.is_local = true;
                    else
                        supplier.is_local = false;
                    supplier.incoterm = model.incoterm;
                    supplier.payment = model.payment;
                    supplier.delivery = model.delivery;
                    supplier.shipment = model.shipment;
                    supplier.warranty = model.warranty;
                    supplier.vendor_ref = model.vendor_ref;
                    db.tb_supplier.Add(supplier);
                    db.SaveChanges();
                    return Json(new { result = "success",supplier_id=supplier.supplier_id }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { result = "error", message = "Please fill all required fields." }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(string id)
        {
            SupplierViewModel supplier = new SupplierViewModel();
            supplier = this.GetSupplierItem(id);
            supplier.supplier_type = Convert.ToBoolean(supplier.is_local) ? "local" : "oversea";
            return View(supplier);
        }
        public ActionResult Edit(string id)
        {
            SupplierViewModel supplier = new SupplierViewModel();
            supplier = this.GetSupplierItem(id);
            supplier.supplier_type =Convert.ToBoolean(supplier.is_local) ? "local" : "oversea";
            return View(supplier);
        }
        [HttpPost]
        public ActionResult Edit(SupplierViewModel supplierVM,string id)
        {
            if (ModelState.IsValid)
            {
                using(kim_mexEntities db=new kim_mexEntities())
                {
                    tb_supplier supplier = db.tb_supplier.FirstOrDefault(model => model.supplier_id == id);
                    supplier.supplier_name = supplierVM.supplier_name;
                    supplier.supplier_email = supplierVM.supplier_email;
                    supplier.supplier_address = supplierVM.supplier_address;
                    supplier.supplier_contact_person = supplierVM.supplier_contact_person;
                    supplier.supplier_phone = supplierVM.supplier_phone;
                    supplier.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    supplier.updated_by = User.Identity.GetUserId();
                    supplier.status = true;
                    supplier.discount = supplierVM.discount;
                    if (string.Compare(supplierVM.supplier_type, "local") == 0)
                        supplier.is_local = true;
                    else
                        supplier.is_local = false;
                    supplier.incoterm = supplierVM.incoterm;
                    supplier.payment = supplierVM.payment;
                    supplier.delivery = supplierVM.delivery;
                    supplier.shipment = supplierVM.shipment;
                    supplier.warranty = supplierVM.warranty;
                    supplier.vendor_ref = supplierVM.vendor_ref;
                    db.SaveChanges();
                    TempData["message"] = "Your data has been updated!";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = "Your data is error been updating!";
            return View(supplierVM);
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_supplier supplier = db.tb_supplier.FirstOrDefault(m => m.supplier_id == id);
                supplier.status = false;
                supplier.updated_by = User.Identity.Name;
                supplier.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
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
        public ActionResult SupplierDataTable()
        {
            List<SupplierViewModel> suppliers = new List<SupplierViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var supplierList = (from tbl in db.tb_supplier where tbl.status==true select tbl);
                if (supplierList.Any())
                {
                    foreach (var supplier in supplierList)
                    {
                        string stype =Convert.ToBoolean(supplier.is_local) ? "Local" : "Oversea";
                        suppliers.Add(new SupplierViewModel() {
                            supplier_id = supplier.supplier_id,
                            supplier_name = supplier.supplier_name,
                            supplier_address = supplier.supplier_address,
                            supplier_email = supplier.supplier_email,
                            supplier_contact_person=supplier.supplier_contact_person,
                            supplier_phone=supplier.supplier_phone,
                            created_date=supplier.created_date,
                            discount=supplier.discount,
                            is_local=supplier.is_local,
                            supplier_type=stype,
                        });
                    }
                }
            }
            catch(Exception ex) { }
            return Json(new { data = suppliers }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupplierDropdownList()
        {
            List<SupplierViewModel> suppliers = new List<SupplierViewModel>();
            using(kim_mexEntities db=new kim_mexEntities())
            {
                suppliers = db.tb_supplier.OrderBy(x => x.supplier_name).Where(x => x.status == true).Select(x => new SupplierViewModel() {
                    supplier_id = x.supplier_id, supplier_name = x.supplier_name,
                    incoterm=x.incoterm,
                    payment=x.payment,
                    delivery=x.delivery,
                    shipment=x.shipment,
                    warranty=x.warranty,
                    vendor_ref=x.vendor_ref,
                }).ToList();
            }
            return Json(new { result = "sucess", data = suppliers }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSupplierAutoSuggestName(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json((db.tb_supplier.Where(w => w.status == true && w.supplier_name.ToLower().Contains(term.ToLower())).Select(s => new { label = s.supplier_name, id = s.supplier_name }).Take(100)), JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetSupplierIdbySupplierName(string name)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                QuoteViewModel quote = new QuoteViewModel();
                quote = CommonClass.GetQuoteNumberSupplier(name);
                return Json(new { data = quote },JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult FindSupplierItem(string id)
        {
            var data = GetSupplierItem(id);
            return Json(new { data = data, result = "success" }, JsonRequestBehavior.AllowGet);
        }

        public SupplierViewModel GetSupplierItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_supplier.Where(s => string.Compare(s.supplier_id, id) == 0).Select(s => new SupplierViewModel()
                {
                    supplier_id=s.supplier_id,
                    supplier_name=s.supplier_name,
                    supplier_address=s.supplier_address,
                    supplier_email=s.supplier_email,
                    supplier_contact_person=s.supplier_contact_person,
                    supplier_phone=s.supplier_phone,
                    supplier_fax=s.supplier_fax,
                    created_date=s.created_date,
                    discount=s.discount,
                    is_local=s.is_local,
                    incoterm=s.incoterm,
                    payment=s.payment,
                    delivery=s.delivery,
                    shipment=s.shipment,
                    warranty=s.warranty,
                    vendor_ref=s.vendor_ref,
                }).FirstOrDefault();
            }
        }
    }
}