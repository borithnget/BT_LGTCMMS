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
    public class ProductTypeController : Controller
    {
        private List<tb_product_category> Category;

        // GET: ProductType
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductType/Details/5
        public ActionResult Details(string id)
        {
            var model = CommonFunctions.GetProductTypeItems(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(CommonFunctions.GetProductTypeItems(id));
        }

        // GET: ProductType/Create
        public ActionResult Create()
        {
            List<tb_product_category> category = new List<tb_product_category>();
            category = this.GetCategoryDropdownList();
            ViewBag.category = category;
            return View();
        }



        private List<tb_product_category> GetCategoryDropdownList()
        {
            List<tb_product_category> Category = new List<tb_product_category>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_product_category.ToList();
            }
            //return classType;
        }
        // POST: ProductType/Create
        [HttpPost]
        public ActionResult Create(ProductTypeViewModel model)
        {

            if (!ModelState.IsValid) return View(model);
            kim_mexEntities db = new kim_mexEntities();
            tb_product_type productType = new tb_product_type();
            //  tb_product_category productcategory = new tb_product_category();
            productType.product_type_id = Guid.NewGuid().ToString();
            productType.product_category_id = model.product_category_id;
            productType.product_type_name = model.product_type_name;
            productType.active = true;
            productType.created_at = DateTime.Now;
            productType.created_by = User.Identity.GetUserId();
            productType.updated_at = DateTime.Now;
            productType.updated_by = User.Identity.GetUserId();
            db.tb_product_type.Add(productType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ProductType/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetProductTypeItems(id);
            return View(model);
        }

        // POST: ProductType/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ProductTypeViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_product_type productType = db.tb_product_type.Find(id);
                productType.product_category_id = model.product_category_id;
                productType.product_type_name = model.product_type_name;
                productType.updated_at = DateTime.Now;
                productType.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductType/Delete/5
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_product_type productType = db.tb_product_type.Find(id);
                productType.active = false;
                productType.updated_at = DateTime.Now;
                productType.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetProducttypeJson()
        {
            return Json(new { data = CommonFunctions.GetProductTypeListItems() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProducttypeDropdownList(int iid = 0)
        {
            List<tb_producttype> producttype = new List<tb_producttype>();
            producttype = GetProducttypeList(iid);

            return Json(new { result = "success", data = producttype }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetAllProducttypeDropdown()
        //{
        //    List<ProductClassViewModel> warehouses = new List<ProductClassViewModel>();
        //    warehouses = this.GetAllProducttype();
        //    return Json(new { result = "success", data = warehouses }, JsonRequestBehavior.AllowGet);
        //}

        private List<ProductClassViewModel> GetAllProducttype()
        {
            throw new NotImplementedException();
        }

        private List<tb_producttype> GetProducttypeList(int iid)
        {
            throw new NotImplementedException();
        }
        public ActionResult GetProductTypeDataDropdownlistbyCategoryId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true).Select(s => new ProductTypeViewModel()
                    {
                        product_type_id = s.product_type_id,
                        product_type_name = s.product_type_name
                    }).ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true && string.Compare(w.product_category_id, id) == 0).Select(s => new ProductTypeViewModel()
                    {
                        product_type_id = s.product_type_id,
                        product_type_name = s.product_type_name
                    }).ToList(), JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GetProductTypeListItemsbyCategoryId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new
                    {
                        data = db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true && !string.IsNullOrEmpty(w.product_type_name) ).Select(s => new ProductTypeViewModel()
                        {
                            product_type_id = s.product_type_id,
                            product_type_name = s.product_type_name
                        }).ToList()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        data = db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true && string.Compare(w.product_category_id, id) == 0 && !string.IsNullOrEmpty(w.product_type_name)).Select(s => new ProductTypeViewModel()
                        {
                            product_type_id = s.product_type_id,
                            product_type_name = s.product_type_name
                        }).ToList()
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

    }
}
