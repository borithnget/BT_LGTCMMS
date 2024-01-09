using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;
//using System.Activities.Expressions;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace BT_KimMex.Controllers
{
    public class BrandController : Controller
    {
        public object brand { get; private set; }

        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }



        // GET: Brand/Create
        public ActionResult Create()
        {
            List<tb_product_type> productType = new List<tb_product_type>();
            productType = this.GetProductTypeDropdownList();
            ViewBag.productType = productType;
            return View();
        }
        private List<tb_product_type> GetProductTypeDropdownList()
        {
            List<tb_product_type> Category = new List<tb_product_type>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_product_type.ToList();
            }
        }
        // POST: Brand/Create
        [HttpPost]
        public ActionResult Create(BrandViewModel model)
        {
            // TODO: Add insert logic here
            if (!ModelState.IsValid)
                return View(model);
            kim_mexEntities db = new kim_mexEntities();
            tb_brand brand = new tb_brand()
            {
                brand_id = Guid.NewGuid().ToString(),
                product_type_id = model.product_type_id,
                brand_name = model.brand_name,
                active = true,
                created_at = DateTime.Now,
                created_by = User.Identity.GetUserId(),
                updated_at = DateTime.Now,
                updated_by = User.Identity.GetUserId()
            };
            db.tb_brand.Add(brand);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Brand/Details/5
        public ActionResult Details(string id)
        {
            var model = CommonFunctions.GetBrandItem(id);
            return View(model);
        }
        // GET: Brand/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetBrandItem(id);
            var productTypes = CommonFunctions.GetProductTypeListItems().Select(x => new SelectListItem
            {
                Value = x.product_type_id,
                Text = x.product_type_name,
                Selected = x.product_type_id == model.product_type_id,
            }).ToList();
            ViewBag.ProductTypes = productTypes;
            return View(model);
        }
        // POST: Brand/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, BrandViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_brand brand = db.tb_brand.Find(id);
                brand.product_type_id = model.product_type_id;
                brand.brand_name = model.brand_name;
                brand.updated_at = DateTime.Now;
                brand.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                TempData["message"] = "Your data has been updated!";
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            // TODO: Add update logic here

        }

        // GET: Brand/Delete/5
        public ActionResult Delete(string id)
        {
            //kim_mexEntities db = new kim_mexEntities();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_brand brand = db.tb_brand.Find(id);
                brand.active = false;
                brand.updated_at = DateTime.Now;
                brand.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult GetBrandListJson()
        public ActionResult GetBrandListJson()
        {
            return Json(new { data = CommonFunctions.GetBrandListItems() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBrandDataDropdownlistbyCategoryId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                    return Json(db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList(), JsonRequestBehavior.AllowGet);
                else
                    return Json(db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true && string.Compare(w.product_type_id, id) == 0).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetBrandListItemsbyCategoryId(string id,string group_id="")
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(group_id))
                    {
                        var models = (from br in db.tb_brand
                                      join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                      orderby br.brand_name
                                      where br.active == true && !string.IsNullOrEmpty(br.brand_name)
                                      select new BrandViewModel()
                                      {
                                          brand_id = br.brand_id,
                                          brand_name = br.brand_name,
                                          product_type_name = pt.product_type_name
                                      }).ToList();
                        return Json(new { data = models }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var models = (from br in db.tb_brand
                                      join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                      orderby br.brand_name
                                      where br.active == true && string.Compare(pt.product_category_id, group_id) == 0 && !string.IsNullOrEmpty(br.brand_name)
                                      select new BrandViewModel()
                                      {
                                          brand_id = br.brand_id,
                                          brand_name = br.brand_name,
                                          product_type_name = pt.product_type_name
                                      }).ToList();
                        return Json(new { data = models }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var models = (from br in db.tb_brand
                                  join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                  orderby br.brand_name
                                  where br.active == true && string.Compare(pt.product_type_id, id) == 0 && !string.IsNullOrEmpty(br.brand_name)
                                  select new BrandViewModel()
                                  {
                                      brand_id = br.brand_id,
                                      brand_name = br.brand_name,
                                      product_type_name = pt.product_type_name
                                  }).ToList();
                    return Json(new { data = models }, JsonRequestBehavior.AllowGet);

                }
                    
            }
        }

    }

}

//public ActionResult GetBrandDataDropdownlistbyCategoryId(string id)
//{
//    using (kim_mexEntities db = new kim_mexEntities())
//    {
//        if (string.IsNullOrEmpty(id))
//            return Json(db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList(), JsonRequestBehavior.AllowGet);
//        else
//            return Json(db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true && string.Compare(w.product_type_id, id) == 0).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList(), JsonRequestBehavior.AllowGet);
//    }
//}
//public ActionResult GetBrandListItemsbyCategoryId(string id)
//{
//    using (kim_mexEntities db = new kim_mexEntities())
//    {
//        if (string.IsNullOrEmpty(id))
//            return Json(new { data = db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList() }, JsonRequestBehavior.AllowGet);
//        else
//            return Json(new { data = db.tb_brand.OrderBy(o => o.brand_name).Where(w => w.active == true && string.Compare(w.product_type_id, id) == 0).Select(s => new BrandViewModel() { brand_id = s.brand_id, brand_name = s.brand_name }).ToList() }, JsonRequestBehavior.AllowGet);
//    }
//}
//    }

//}

