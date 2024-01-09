using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.Web.Services;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    public class ProductSizeController : Controller
    {
        //private object id;

        // GET: ProductSize
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductSize/Details/5
        public ActionResult Details(string id)
        {
            var model = CommonFunctions.GetProductSizeItems(id);
            return View(model);
        }

        // GET: ProductSize/Create
        public ActionResult Create()
        {
            List<tb_class> classs= new List<tb_class>();
            classs = this.GetClassDropdownList();
            ViewBag.Class = classs;
            return View();
        }

        private List<tb_class> GetClassDropdownList()
        {
            List<tb_class> classs = new List<tb_class>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_class.ToList();
            }
        }

        

        // POST: ProductSize/Create
        [HttpPost]
        public ActionResult Create(ProductSizeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_product_size productSize = new tb_product_size();
                productSize.product_size_id = Guid.NewGuid().ToString();
                productSize.brand_id = model.class_id;
                productSize.product_size_name = model.product_size_name;
                productSize.active = true;                
                productSize.updated_at = DateTime.Now;
                //productSize.created_at = DateTime.Now;
                //productSize.created_by = User.Identity.GetUserId();
                productSize.updated_by = User.Identity.GetUserId();

                db.tb_product_size.Add(productSize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductSize/Edit/5
        public ActionResult Edit(string id)
        {
            var model = CommonFunctions.GetProductSizeItems(id);
            return View(model);
        }

        // POST: ProductSize/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, ProductSizeViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (!ModelState.IsValid)
                    return View(model);
                kim_mexEntities db = new kim_mexEntities();
                tb_product_size productSize = db.tb_product_size.FirstOrDefault(x => x.product_size_id == id);
                if (productSize != null)
                {
                    productSize.brand_id = model.class_id;
                    productSize.product_size_name = model.product_size_name;
                    productSize.updated_at = DateTime.Now;
                    productSize.updated_by = User.Identity.GetUserId();
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
        // POST: ProductSize/Delete/5 
        public ActionResult Delete(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_product_size productSize = db.tb_product_size.Find(id);
                productSize.active = false;
                productSize.updated_at = DateTime.Now;
                productSize.updated_by = User.Identity.GetUserId();
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    Message = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetProductSizeListJson()
        {
            return Json(new { data = CommonFunctions.GetProductSizeItems() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductSizeListItemsJson()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {

                return Json(new
                {
                    data = (from ps in db.tb_product_size
                            //join br in db.tb_brand on ps.brand_id equals br.brand_id into pbr from br in pbr.DefaultIfEmpty()
                            //join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id into ppt from pt in ppt.DefaultIfEmpty()
                                                                                                                       //join pc in db.tb_product_category on pt.product_category_id equals pc.p_category_id
                                                                                                                       //join cls in db.tb_brand on ps.brand_id equals cls.brand_id
                            join cls in db.tb_class on ps.brand_id equals cls.class_id into pcls from cls in pcls.DefaultIfEmpty()
                            orderby ps.product_size_name

                            where ps.active == true
                            select new ProductSizeViewModel()
                            {
                                product_size_id = ps.product_size_id,
                                class_id = cls.class_id,
                                class_name = cls.class_name,
                                product_size_name = ps.product_size_name,
                                created_at = ps.created_at,
                                updated_at = ps.updated_at,
                                // class_id = cls.brand_id, 
                                //class_name=cls.brand_name
                            }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetProductDataDropdownlistbyClassId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    var productSizes = db.tb_product_size.OrderBy(o => o.product_size_name).Where(w => w.active == true).Select(s => new ProductSizeViewModel()
                    {
                        product_size_id = s.product_size_id,
                        product_size_name = s.product_size_name,
                        updated_at = s.updated_at

                    }).ToList();
                    return Json(new { result = "success", data = productSizes }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var productSizes = db.tb_product_size.OrderBy(o => o.product_size_name).Where(w => w.active == true && string.Compare(w.brand_id, id) == 0).Select(s => new ProductSizeViewModel()
                    var size = from ps in db.tb_product_size
                                   //join br in db.tb_brand on ps.brand_id equals br.brand_id
                                   //join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                   //join pc in db.tb_product_category on pt.product_category_id equals pc.p_category_id
                                   //where ps.active ?? true && pc.class_id == id
                               //join gr in db.tb_class on ps.brand_id equals gr.class_id
                               orderby ps.product_size_name
                               where ps.active==true && string.Compare(ps.brand_id,id)==0
                               select new ProductSizeViewModel
                               {
                                   product_size_id = ps.product_size_id,
                                   product_size_name = ps.product_size_name,
                                   updated_at = ps.updated_at
                               };

                    var productSizes = size.ToList();
                    return Json(new { result = "success", data = productSizes }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult GetProductSizeDataDropdownlistbyClassId(string id)
        {

            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    var productSizes = db.tb_product_size.OrderBy(o => o.product_size_name).Where(w => w.active == true).Select(s => new ProductSizeViewModel()
                    {
                        product_size_id = s.product_size_id,
                        product_size_name = s.product_size_name,
                        updated_at = s.updated_at

                    }).ToList();
                    return Json(new { result = "success", data = productSizes }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //var productSizes = db.tb_product_size.OrderBy(o => o.product_size_name).Where(w => w.active == true && string.Compare(w.brand_id, id) == 0).Select(s => new ProductSizeViewModel()
                    var size = from ps in db.tb_product_size
                               join br in db.tb_brand on ps.brand_id equals br.brand_id
                               join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                               join pc in db.tb_product_category on pt.product_category_id equals pc.p_category_id
                               where ps.active ?? true && pc.class_id == id
                               select new ProductSizeViewModel
                               {
                                   product_size_id = ps.product_size_id,
                                   product_size_name = ps.product_size_name,
                                   updated_at = ps.updated_at
                               };

                    var productSizes = size.ToList();
                    return Json(new { result = "success", data = productSizes }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
