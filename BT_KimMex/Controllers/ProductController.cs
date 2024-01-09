using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using Microsoft.AspNet.Identity;
using System.Windows.Forms;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        //private object uom;
        //private IEnumerable productType;
        //private string product_type_id;
        //private string product_class_id;
        //private string product_size_id;

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        //add new 9/7/18 : 4:52 pm not success
        [HttpGet]
        public PartialViewResult IndexPartial(ProductViewModel m)
        {
            // Do stuff with my model
            kim_mexEntities db = new kim_mexEntities();
            tb_product product = new tb_product();
            product.product_code = m.product_code;
            product.product_id = Guid.NewGuid().ToString();
            product.product_name = m.product_name;
            product.unit_price = m.unit_price;
            product.status = true;
            product.product_unit = m.product_unit;
            product.brand_id = m.p_category_id;
            product.product_class_id = m.product_class_id;
            product.brand_id = m.brand_id;
            product.color = m.color_id;
            product.product_size = m.product_size_id;
            product.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
            product.Remark = m.Remark;
            db.tb_product.Add(product);
            db.SaveChanges();
            return PartialView(m.product_id);
        }
        //End add new 9/7/18 : 4:52 pm not success

        public JsonResult CheckItemCodeAvailable(string itemdata)
        {
            System.Threading.Thread.Sleep(1000);
            kim_mexEntities db = new kim_mexEntities();
            var SearchData = db.tb_product.Where(x => string.Compare(x.product_code.ToLower(), itemdata.ToLower()) == 0 && x.status == true).FirstOrDefault();


            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }

        public JsonResult CheckItemCodeAvailableforEdit(string itemdata, string id)
        {
            System.Threading.Thread.Sleep(1000);
            kim_mexEntities db = new kim_mexEntities();
            var SearchData = db.tb_product.Where(x => x.product_code == itemdata && x.status == true).FirstOrDefault();
            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var productClass = this.GetProductClassList();
            var brands = this.GetBranchList();
            var units = this.GetUnitList();
            var categories = this.GetProductCategoryList();
            var productType = CommonFunctions.GetProductTypeListItems();
            var color = this.GetColorList();
            var productSize = this.GetProductSize();

            ViewBag.productSize = new SelectList(productSize, "product_size_id", "product_size_name");
            ViewBag.ProductClasses = new SelectList(productClass, "product_class_id", "product_class_name");
            ViewBag.Brands = new SelectList(brands, "brand_id", "brand_name");
            ViewBag.Unit = new SelectList(units, "Id", "Name");
            ViewBag.Categories = new SelectList(categories, "p_category_id", "p_category_name");
            ViewBag.ProductType = new SelectList(productType, "product_type_id", "product_type_name");
            ViewBag.Color = new SelectList( color, "color_id", "color_name");
            return View();
        }

       

        [HttpPost]
        public ActionResult Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    ProductCodeResponse codeResponse= CommonFunctions.GenerateProductCode(model.class_id, model.sub_group_id);
                    tb_product product = new tb_product();
                    product.product_id = Guid.NewGuid().ToString();
                    product.product_name = CommonFunctions.GenerateProductDescription(model.p_category_id,model.product_type_id,model.product_class_id, model.product_size_id,model.color_id,model.brand_id,model.model_factory_code);
                    product.product_code = codeResponse.code;
                    product.product_number = codeResponse.number;
                    product.brand_id = model.brand_id;
                    product.product_class_id = model.product_class_id;
                    product.product_unit = model.product_unit;
                    product.unit_price = model.unit_price == null ? 0 : model.unit_price ;
                    product.Remark = model.Remark;
                    product.status = true;
                    product.created_by = User.Identity.Name;
                    product.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    product.product_size = model.product_size_id;
                    product.product_size_id = model.product_size_id;
                    product.model_factory_code = model.model_factory_code;
                    product.supplier_id = model.supplier_id;
                    product.labor_hour = model.labor_hour;
                    product.insulation_color = model.insulation_color;
                    product.sheath_color = model.sheath_color;
                    product.product_class_id = model.product_class_id;
                    product.number_of_core = model.number_of_core;
                    product.outer_sheath = model.outer_sheath;
                    product.color = model.color_id;
                    product.product_category_id = model.p_category_id;
                    product.product_type_id = model.product_type_id;
                    product.group_id = model.class_id;
                    product.sub_group_id = model.sub_group_id;
                    product.so_number = model.so_number;
                    product.cash_flow = model.cash_flow;
                    product.labour_hour=model.labour_hour;
                    db.tb_product.Add(product);
                    db.SaveChanges();

                    tb_multiple_uom uom = new tb_multiple_uom();
                    uom.uom_id = Guid.NewGuid().ToString();
                    uom.product_id = product.product_id;
                    if (model.uom1_id != null && model.uom1_qty > 0)
                    {
                        uom.uom1_id = model.uom1_id;
                        uom.uom1_qty = model.uom1_qty;
                    }
                    if (model.uom2_id != null && model.uom2_qty > 0)
                    {
                        uom.uom2_id = model.uom2_id;
                        uom.uom2_qty = model.uom2_qty;
                    }
                    if (model.uom3_id != null && model.uom3_qty > 0)
                    {
                        uom.uom3_id = model.uom3_id;
                        uom.uom3_qty = model.uom3_qty;
                    }
                    if (model.uom4_id != null && model.uom4_qty > 0)
                    {
                        uom.uom4_id = model.uom4_id;
                        uom.uom4_qty = model.uom4_qty;
                    }
                    if (model.uom5_id != null && model.uom5_qty > 0)
                    {
                        uom.uom5_id = model.uom5_id;
                        uom.uom5_qty = model.uom5_qty;
                    }

                    if (model.uom1_price > 0)
                    {
                        uom.uom1_price = model.uom1_price;

                    }
                    if (model.uom2_price > 0)
                    {
                        uom.uom2_price = model.uom2_price;

                    }
                    if (model.uom3_price > 0)
                    {
                        uom.uom3_price = model.uom3_price;

                    }
                    if (model.uom4_price > 0)
                    {
                        uom.uom4_price = model.uom4_price;

                    }
                    if (model.uom5_price > 0)
                    {
                        uom.uom5_price = model.uom5_price;

                    }

                    db.tb_multiple_uom.Add(uom);
                    db.SaveChanges();

                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            List<tb_unit> units = new List<tb_unit>();
            List<tb_product_category> categories = new List<tb_product_category>();
            units = this.GetUnitList();
            categories = this.GetProductCategoryList();
            ViewBag.ProductClasses = new SelectList(this.GetProductClassList(), "product_class_id", "product_class_name");
            ViewBag.Brands = new SelectList(this.GetBranchList(), "brand_id", "brand_name");
            ViewBag.Color = new SelectList(this.GetColorList(), "color_id", "color_name");
            ViewBag.ProductSize = new SelectList(this.GetProductSize(), "product_size_id", "product_size_name");
            ViewBag.Unit = new SelectList(units, "Name", "Name");
            ViewBag.Categories = new SelectList(categories, "p_category_id", "p_category_name");
            TempData["message"] = "Your data is error while saving!";
            return View();
        }

        //private IEnumerable GetProductSize()
        //{
        //    throw new NotImplementedException();
        //}


        //add new for quots

        public ActionResult CreateJson(string uom1_id, string uom2_id, string uom3_id, string uom4_id, string uom5_id,
            string product_code, string product_name, string product_unit, string p_category_id,
            string Remark, Nullable<decimal> unit_price = 0, Nullable<decimal> uom1_price = 0, Nullable<decimal> uom2_price = 0,
            Nullable<decimal> uom3_price = 0, Nullable<decimal> uom4_price = 0, Nullable<decimal> uom5_price = 0,
            Nullable<decimal> uom1_qty = 0, Nullable<decimal> uom2_qty = 0, Nullable<decimal> uom3_qty = 0,
            Nullable<decimal> uom4_qty = 0, Nullable<decimal> uom5_qty = 0)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_product product = new tb_product();

                product.product_code = product_code;
                product.product_id = Guid.NewGuid().ToString();
                product.product_name = product_name;
                product.unit_price = unit_price;
                product.status = true;
                product.product_unit = product_unit;
                product.brand_id = p_category_id;
                product.created_by = User.Identity.Name;
                product.created_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                product.Remark = Remark;
                db.tb_product.Add(product);
                db.SaveChanges();

                tb_multiple_uom uom = new tb_multiple_uom();
                uom.uom_id = Guid.NewGuid().ToString();
                uom.product_id = product.product_id;
                uom.uom1_id = uom1_id;
                uom.uom1_qty = uom1_qty;
                uom.uom1_price = uom1_price;
                uom.uom2_id = uom2_id;
                uom.uom2_qty = uom2_qty;
                uom.uom2_price = uom2_price;
                uom.uom3_id = uom3_id;
                uom.uom3_qty = uom3_qty;
                uom.uom3_price = uom3_price;
                uom.uom4_id = uom4_id;
                uom.uom4_qty = uom4_qty;
                uom.uom4_price = uom4_price;
                uom.uom5_id = uom5_id;
                uom.uom5_qty = uom5_qty;
                uom.uom5_price = uom5_price;

                db.tb_multiple_uom.Add(uom);
                db.SaveChanges();

                return Json(new { Message = "Success", pro_id = product.product_id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadDataItemForQuote(string id, string uom1_id, string uom2_id, string uom3_id, string uom4_id, string uom5_id, string product_code, string product_name, string product_unit, string p_category_id, string p_category_name, string Remark, Nullable<decimal> unit_price = 0, Nullable<decimal> uom1_price = 0, Nullable<decimal> uom2_price = 0, Nullable<decimal> uom3_price = 0, Nullable<decimal> uom4_price = 0, Nullable<decimal> uom5_price = 0, Nullable<decimal> uom1_qty = 0, Nullable<decimal> uom2_qty = 0, Nullable<decimal> uom3_qty = 0, Nullable<decimal> uom4_qty = 0, Nullable<decimal> uom5_qty = 0)
        {


            IEnumerable<ProductViewModel> modelList = new List<ProductViewModel>();
            using (kim_mexEntities context = new kim_mexEntities())
            {
                var books = context.tb_product.Where(x => x.product_id == id).ToList();

                modelList = books.Select(x =>
                            new ProductViewModel()
                            {

                                product_id = x.product_id,
                                product_code = x.product_code,
                                product_name = x.product_name,
                                unit_price = x.unit_price,
                                product_unit = x.product_unit,
                                Remark = x.Remark,
                                p_category_id = x.brand_id,
                                created_date = x.created_date,

                            });

            }
            return Json(modelList, JsonRequestBehavior.AllowGet);

        }
       public ActionResult EditItem(string id, string uom1_id, string uom2_id, string uom3_id, string uom4_id, string uom5_id, string product_code, string product_name, string product_unit, string p_category_id, string p_category_name, string Remark, Nullable<decimal> unit_price = 0, Nullable<decimal> uom1_price = 0, Nullable<decimal> uom2_price = 0, Nullable<decimal> uom3_price = 0, Nullable<decimal> uom4_price = 0, Nullable<decimal> uom5_price = 0, Nullable<decimal> uom1_qty = 0, Nullable<decimal> uom2_qty = 0, Nullable<decimal> uom3_qty = 0, Nullable<decimal> uom4_qty = 0, Nullable<decimal> uom5_qty = 0)
        {

            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_product product = db.tb_product.Find(id);
                product.status = false;
                product.updated_by = User.Identity.Name;
                product.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
            }

            IEnumerable<ProductViewModel> modelList = new List<ProductViewModel>();
            using (kim_mexEntities context = new kim_mexEntities())
            {
                var books = context.tb_product.Where(x => x.product_id == id).ToList();

                modelList = books.Select(x =>
                            new ProductViewModel()
                            {

                                product_id = x.product_id,
                                product_code = x.product_code,
                                product_name = x.product_name,
                                unit_price = x.unit_price,
                                product_unit = x.product_unit,
                                Remark = x.Remark,
                                p_category_id = x.brand_id,
                                created_date = x.created_date,

                            });
            }
            return Json(modelList, JsonRequestBehavior.AllowGet);

          
        }
        public ActionResult CreateJsonforEdit(string id, string product_code, string brand,string product_name, string product_unit, string p_category_id, string product_size_id,string Remark, Nullable<decimal> unit_price = 0)
        {
            ProductViewModel product = new ProductViewModel();
            tb_multiple_uom uom = new tb_multiple_uom();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                product = (from pro in db.tb_product
                           join cat in db.tb_brand on pro.brand_id equals cat.brand_id
                           where pro.product_id == id
                           select new ProductViewModel
                           {
                               product_id = pro.product_id,
                               product_name = pro.product_name,
                               product_code = pro.product_code,
                               //product_size_id = pro.product_size_id,
                               product_unit = pro.product_unit.Trim(),
                               p_category_id = pro.brand_id,
                               p_category_name = cat.brand_name,
                               unit_price = pro.unit_price,
                               created_date = pro.created_date,
                               Remark = pro.Remark,
                           }).FirstOrDefault();
            }
            return View(product);
        }

        public ActionResult Details(string id)
        {            
            return View(ProductViewModel.GetProductItem(id));
        }


        public ActionResult Edit(string id)
        {
            ViewData["Category"] = ProductCategoryDropdownList();
            //ViewData["Unit"] = UnitDropdownList();
            //List<tb_unit> units = new List<tb_unit>();
            //ViewBag.Unit = this.GetUnitList();
            ViewBag.Unit = CommonClass.GetAllUnits();

            List<UnitViewModel> units = CommonClass.GetAllUnits();
            ViewBag.Units = new SelectList(units, "Id", "Name");


            return View(ProductViewModel.GetProductItem(id));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ProductViewModel productVM, string id)
        {
            if (ModelState.IsValid)
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    tb_product product = new tb_product();
                    product = db.tb_product.FirstOrDefault(x => x.product_id == id);
                    product.product_name = CommonFunctions.GenerateProductDescription(productVM.p_category_id, productVM.product_type_id, productVM.product_class_id,productVM.product_size_id, productVM.color_id, productVM.brand_id, productVM.model_factory_code);
                    //product.product_code = CommonFunctions.GenerateProductCode();
                    product.brand_id = productVM.brand_id;
                    //product.class_id = productVM.class_id;
                    product.product_class_id = productVM.product_class_id;
                    product.product_unit = productVM.product_unit;
                    product.unit_price = productVM.unit_price==null?product.unit_price:productVM.unit_price;
                    product.Remark = productVM.Remark;
                    product.updated_by = User.Identity.GetUserId();
                    product.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                    product.product_size = productVM.product_size_id;
                    product.product_size_id = productVM.product_size_id;
                    product.model_factory_code = productVM.model_factory_code;
                    product.supplier_id = productVM.supplier_id;
                    product.labor_hour = productVM.labor_hour;
                    product.insulation_color = productVM.insulation_color;
                    product.sheath_color = productVM.sheath_color;
                    product.number_of_core = productVM.number_of_core;
                    product.outer_sheath = productVM.outer_sheath;
                    product.color = productVM.color_id;
                    product.product_class_id = productVM.product_class_id;
                    product.product_type_id = productVM.product_type_id;
                    product.product_category_id = productVM.p_category_id;
                    product.group_id = productVM.class_id;
                    product.sub_group_id = productVM.sub_group_id;
                    product.so_number = productVM.so_number;
                    product.cash_flow = productVM.cash_flow;
                    product.labour_hour = productVM.labour_hour;
                    db.SaveChanges();

                    //// Update Group
                    //var brand = db.tb_brand.FirstOrDefault(x => x.brand_id == product.brand_id);
                    //if (brand != null)
                    //{
                    //    brand.product_type_id = productVM.product_type_id;

                    //    var productType = db.tb_product_type.FirstOrDefault(x => x.product_type_id == brand.product_type_id);
                    //    if (productType != null)
                    //    {
                    //        productType.product_category_id = productVM.p_category_id;
                    //        var productCategory = db.tb_product_category.FirstOrDefault(x => x.p_category_id == productType.product_category_id);
                    //        if (productCategory != null)
                    //        {
                    //            productCategory.class_id = productVM.class_id;
                    //        }

                    //    }


                    //}

                    db.SaveChanges();

                    tb_multiple_uom uom = new tb_multiple_uom();
                    //uom.uom_id = Guid.NewGuid().ToString();
                    uom = db.tb_multiple_uom.FirstOrDefault(x => x.product_id == product.product_id);

                    if (uom == null)
                    {
                        uom = new tb_multiple_uom();
                    }
                    uom.product_id = product.product_id;
                    if (productVM.uom1_id != null && productVM.uom1_qty > 0)
                    {
                        uom.uom1_id = productVM.uom1_id;
                        uom.uom1_qty = productVM.uom1_qty;
                    }
                    if (productVM.uom2_id != null && productVM.uom2_qty > 0)
                    {
                        uom.uom2_id = productVM.uom2_id;
                        uom.uom2_qty = productVM.uom2_qty;
                    }
                    if (productVM.uom3_id != null && productVM.uom3_qty > 0)
                    {
                        uom.uom3_id = productVM.uom3_id;
                        uom.uom3_qty = productVM.uom3_qty;
                    }
                    if (productVM.uom4_id != null && productVM.uom4_qty > 0)
                    {
                        uom.uom4_id = productVM.uom4_id;
                        uom.uom4_qty = productVM.uom4_qty;
                    }
                    if (productVM.uom5_id != null && productVM.uom5_qty > 0)
                    {
                        uom.uom5_id = productVM.uom5_id;
                        uom.uom5_qty = productVM.uom5_qty;
                    }

                    if (productVM.uom1_price > 0)
                    {
                        uom.uom1_price = productVM.uom1_price;

                    }
                    if (productVM.uom2_price > 0)
                    {
                        uom.uom2_price = productVM.uom2_price;

                    }
                    if (productVM.uom3_price > 0)
                    {
                        uom.uom3_price = productVM.uom3_price;

                    }
                    if (productVM.uom4_price > 0)
                    {
                        uom.uom4_price = productVM.uom4_price;
                    }
                    if (productVM.uom5_price > 0)
                    {
                        uom.uom5_price = productVM.uom5_price;
                   }
                   // db.tb_multiple_uom.Add(uom);
                    db.SaveChanges();

                    TempData["message"] = "Your data has been saved!";
                    return RedirectToAction("Index");
                }
            }
            List<tb_unit> units = new List<tb_unit>();
            List<tb_product_category> categories = new List<tb_product_category>();
            units = this.GetUnitList();
            categories = this.GetProductCategoryList();
            ViewBag.ProductClasses = new SelectList(this.GetProductClassList(), "product_class_id", "product_class_name");
            ViewBag.Brands = new SelectList(this.GetBranchList(), "brand_id", "brand_name");
            ViewBag.Unit = new SelectList(units, "Name", "Name");
            ViewBag.Categories = new SelectList(categories, "p_category_id", "p_category_name");
            TempData["message"] = "Your data is error while saving!";
            return View();
            //TempData["message"] = "Your data is error been updating!";
            //return Edit(id);
        }
        public ActionResult Delete(string id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                tb_product product = db.tb_product.FirstOrDefault(m => m.product_id == id);
                product.status = false;
                product.updated_date = Class.CommonClass.ToLocalTime(DateTime.Now);
                db.SaveChanges();
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
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
        public ActionResult ProductDataTable()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var productList = from pro in db.tb_product
                                  join un in db.tb_unit on pro.product_unit equals un.Id into unn from un in unn.DefaultIfEmpty()
                                  join pc in db.tb_product_class on pro.product_class_id equals pc.product_class_id into pcl
                                  from pc in pcl.DefaultIfEmpty()
                                  join br in db.tb_brand on pro.brand_id equals br.brand_id into bra
                                  from br in bra.DefaultIfEmpty()
                                  join pt in db.tb_product_type on pro.product_type_id equals pt.product_type_id into ptt
                                  from pt in ptt.DefaultIfEmpty()
                                  join cat in db.tb_product_category on pro.product_category_id equals cat.p_category_id into catt
                                  from cat in catt.DefaultIfEmpty()
                                  join cls in db.tb_class on pro.group_id equals cls.class_id into clss
                                  from cls in clss.DefaultIfEmpty()
                                  join pss in db.tb_product_size on pro.product_size equals pss.product_size_id into psss
                                  from pss in psss.DefaultIfEmpty()
                                  join col in db.tb_color on pro.color equals col.color_id into jcol
                                  from col in jcol.DefaultIfEmpty()
                                  join sg in db.tb_sub_group on pro.sub_group_id equals sg.sub_group_id into psg from sg in psg.DefaultIfEmpty()
                                  where pro.status == true
                                  select new ProductViewModel
                                  {
                                      product_id = pro.product_id,
                                      product_code = pro.product_code,
                                      product_name = pro.product_name,
                                      product_unit = un.Name,
                                      unit_price = pro.unit_price,
                                      product_class_name = pc.product_class_name,
                                      brand_name = br.brand_name,
                                      created_date = pro.created_date,
                                      p_category_name = cat.p_category_name,
                                      class_name = cls.class_name,
                                      product_type_name = pt.product_type_name,
                                      product_size_name = pss.product_size_name,
                                      color_id = pro.color,
                                      color = pro.color,
                                      color_name = col == null ? "" : col.color_name,
                                      model_factory_code = pro.model_factory_code,
                                      sub_group_id=sg.sub_group_id,
                                      sub_group_name = sg.sub_group_name,
                                      labour_hour=pro.labour_hour,
                                  };
                foreach(var item in productList)
                {
                    ProductViewModel product = new ProductViewModel();
                    product = item;
                    product.product_color_name = string.IsNullOrEmpty(product.color) ? string.Empty : db.tb_color.Find(product.color).color_name;
                    products.Add(product);
                }
                //products = productList.ToList();
                //products = productList.ToList();
            }
            //return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            var jsonResult = Json(new { data = products }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult UnitList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var list = (from tbl in db.tb_unit orderby tbl.Name where tbl.status == true select tbl).ToList();
                if (list.Any())
                {
                    return Json(new { data = list }, JsonRequestBehavior.AllowGet);
                }
            }
            return View();
        }

        //add new
        //public ActionResult UnitList()
        //{
        //    List<ProductViewModel> unitlists = new List<ProductViewModel>();
        //    using (kim_mexEntities db = new kim_mexEntities())

        //    {

        //        //end
        //        var list = (from tbl in db.tb_unit orderby tbl.Name where tbl.status == true select tbl).ToList();
        //        if (list.Any())
        //        {
        //            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        //        }

        //        var listUnit = (from tbl in db.tb_multiple_uom
        //                        join tblpro in db.tb_product on tbl.product_id equals tblpro.product_id
        //                        orderby tbl.product_id
        //                        where tbl.product_id == tblpro.product_id
        //                        select tbl).ToList();


        //        if (listUnit.Any())
        //        {
        //            foreach (var unitUom in listUnit)
        //            {
        //                unitlists = (from tbl in db.tb_multiple_uom
        //                             join tblpro in db.tb_product on tbl.product_id equals tblpro.product_id
        //                             orderby tbl.product_id
        //                             where tbl.product_id == tblpro.product_id
        //                             select new Models.ProductViewModel()).ToList();

        //                unitlists.Add(new ProductViewModel() { });
        //                if (unitlists.Any())
        //                {

        //                }

        //            }
        //        }
        //        return View();
        //    }
        //}
        //end

        public ActionResult ProductCategory()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var categoryList = (from tbl in db.tb_product_category where tbl.status == true select tbl).ToList();
                return Json(new { data = categoryList }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<SelectListItem> ProductCategoryDropdownList()
        {
            List<SelectListItem> categoryList = new List<SelectListItem>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var categories = (from tbl in db.tb_product_category where tbl.status == true select tbl).ToList();
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        categoryList.Add(new SelectListItem { Text = category.p_category_name, Value = category.p_category_id.ToString() });
                    }
                }
            }
            return categoryList;
        }
        public List<SelectListItem> UnitDropdownList()
        {
            List<SelectListItem> unitList = new List<SelectListItem>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var units = (from tbl in db.tb_unit orderby tbl.Name where tbl.status == true select tbl).ToList();
                if (units != null)
                {
                    foreach (var unit in units)
                    {
                        unitList.Add(new SelectListItem { Text = unit.Name, Value = unit.Name.ToString() });
                    }
                }
            }
            return unitList;
        }
        public ActionResult GetProductDataRow(string category_id)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<ProductViewModel> products = new List<ProductViewModel>();
                var product_list = (from tbl in db.tb_product orderby tbl.brand_id where tbl.brand_id == category_id && tbl.status == true select tbl).ToList();
                if (product_list.Any())
                {
                    foreach (var product in product_list)
                    {
                        products.Add(new ProductViewModel()
                        {
                            product_id = product.product_id,
                            product_code = product.product_code,
                            product_name = product.product_name,
                            product_unit = product.product_unit,
                            product_class_id = product.product_class_id,
                            brand_id = product.brand_id,
                            unit_price = product.unit_price,
                            Remark = product.Remark
                            ,
                            uom1_id = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom1_id).FirstOrDefault(),
                            uom1_qty = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom1_qty).FirstOrDefault(),
                            uom2_id = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom2_id).FirstOrDefault(),
                            uom2_qty = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom2_qty).FirstOrDefault(),
                            uom3_id = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom3_id).FirstOrDefault(),
                            uom3_qty = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom3_qty).FirstOrDefault(),
                            uom4_id = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom4_id).FirstOrDefault(),
                            uom4_qty = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom4_qty).FirstOrDefault(),
                            uom5_id = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom5_id).FirstOrDefault(),
                            uom5_qty = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).Select(x => x.uom5_qty).FirstOrDefault(),

                        });
                    }
                }
                return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GetProductById(string p_code)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var product = (from tbl in db.tb_product
                               orderby tbl.product_code
                               where tbl.product_code == p_code && tbl.status == true
                               select new ProductViewModel()
                               {
                                   product_id = tbl.product_id,
                                   product_code = tbl.product_code,
                                   product_name = tbl.product_name,
                                   product_unit = tbl.product_unit,
                                   product_class_id = tbl.product_class_id,
                                   brand_id = tbl.brand_id,
                                   unit_price = tbl.unit_price,
                                   Remark = tbl.Remark,
                               }
                               ).FirstOrDefault();
                return Json(new { data = product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GetProductAutoSuggest(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json((db.tb_product.Where(c => c.product_code.Contains(term) && c.status == true).Select(a => new { label = a.product_code + " " + a.product_name, id = a.product_id }).Take(100)), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductAutoSuggest1(string term)
        {
            kim_mexEntities db = new kim_mexEntities();

            return Json((db.tb_product.Where(c => c.product_code.Contains(term) && c.status == true).Select(a => new { label = a.product_code, id = a.product_id }).Take(100)), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductAutoSuggestByName(string term)
        {
            kim_mexEntities db = new kim_mexEntities();
            return Json(db.tb_product.Where(c => c.product_name.Contains(term) && c.status == true).Select(a => new { label = a.product_code + " " + a.product_name, id = a.product_id }), JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetProductAutoSuggestName(string term)
        //{
        //    kim_mexEntities db = new kim_mexEntities();
        //    return Json((db.tb_product.Where(c => c.product_name.ToLower().Contains(term.ToLower()) || c.product_code.ToLower().StartsWith(term.ToLower()) && c.status == true).Select(a => new { label =a.product_code+" "+ a.product_name, id = a.product_id }).Take(100)), JsonRequestBehavior.AllowGet);
        //}

        //new process Dec 16 2020
        public ActionResult GetProductAutoSuggestName(string term, string classId,string subGroupId, string categoryId, string productTypeId, string brandId, string productSizeId)
        {
            kim_mexEntities db = new kim_mexEntities();
            if (string.IsNullOrEmpty(classId) && string.IsNullOrEmpty(subGroupId) && string.IsNullOrEmpty(categoryId) && string.IsNullOrEmpty(productTypeId) && string.IsNullOrEmpty(brandId) && string.IsNullOrEmpty(productSizeId))
                return Json((db.tb_product.OrderBy(s=>s.product_number).OrderBy(s=>s.product_name).Where(c => (c.product_name.ToLower().Contains(term.ToLower()) || c.product_code.ToLower().StartsWith(term.ToLower())) && c.status == true).Select(a => new { label = a.product_code + " - " + a.product_name, id = a.product_id }).Take(100)), JsonRequestBehavior.AllowGet);
            else
            {
                if (!string.IsNullOrEmpty(brandId))
                {
                    if (!string.IsNullOrEmpty(productSizeId))
                    {
                        var products = (from pro in db.tb_product
                                        orderby pro.product_number,pro.product_name
                                        where pro.status == true && pro.product_name.ToLower().Contains(term.ToLower()) && string.Compare(pro.brand_id, brandId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var products = (from pro in db.tb_product
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.brand_id, brandId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }

                }
                else if (!string.IsNullOrEmpty(productTypeId))
                {
                    if (!string.IsNullOrEmpty(productSizeId))
                    {
                        var products = (from pro in db.tb_product
                                            //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                            //join prot in db.tb_product_type on pro.product_type_id equals prot.product_type_id
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_type_id, productTypeId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var products = (from pro in db.tb_product
                                            //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                            //join prot in db.tb_product_type on pro.product_type_id equals prot.product_type_id
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_type_id, productTypeId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }

                }
                else if (!string.IsNullOrEmpty(categoryId) || !string.IsNullOrEmpty(productSizeId))
                {
                    if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(productSizeId))
                    {
                        var products = (from pro in db.tb_product
                                            //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                            //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                            //join cat in db.tb_product_category on pro.product_category_id equals cat.p_category_id
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_category_id, categoryId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }
                    else if (!string.IsNullOrEmpty(productSizeId))
                    {
                        var products = (from pro in db.tb_product
                                            //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                            //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                            //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_size, productSizeId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var products = (from pro in db.tb_product
                                            //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                            //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                            //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                        orderby pro.product_number, pro.product_name
                                        where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_category_id, categoryId) == 0
                                        select new
                                        {
                                            label = pro.product_code + " - " + pro.product_name,
                                            id = pro.product_id
                                        }).Take(100);
                        return Json(products, JsonRequestBehavior.AllowGet);
                    }

                }
                else if (!string.IsNullOrEmpty(subGroupId))
                {
                    var products = (from pro in db.tb_product
                                    orderby pro.product_number, pro.product_name
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.sub_group_id, subGroupId) == 0
                                    select new
                                    {
                                        label = pro.product_code + " - " + pro.product_name,
                                        id = pro.product_id
                                    }).Take(100);
                    return Json(products, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var products = (from pro in db.tb_product
                                        //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                        //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                        //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                        //join cls in db.tb_class on cat.class_id equals cls.class_id
                                    orderby pro.product_number, pro.product_name
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.group_id, classId) == 0
                                    select new
                                    {
                                        label = pro.product_code + " - " + pro.product_name,
                                        id = pro.product_id
                                    }).Take(100);
                    return Json(products, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GetProductFilterResult(string term, string classId, string categoryId, string productTypeId, string brandId, string productSizeId)
        {
            kim_mexEntities db = new kim_mexEntities();
            IQueryable<tb_product> products;
            if (string.IsNullOrEmpty(classId) && string.IsNullOrEmpty(categoryId) && string.IsNullOrEmpty(productTypeId) && string.IsNullOrEmpty(brandId) && string.IsNullOrEmpty(productSizeId))
            //products = db.tb_product.Where(c => (c.product_name.ToLower().Contains(term.ToLower()) || c.product_code.ToLower().StartsWith(term.ToLower())) && c.status == true).Select(a => new { a }).Take(100);
            {
                products = (from pro in db.tb_product
                            where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower()))
                            select pro).Take(100);
            }
            else
            {
                if (!string.IsNullOrEmpty(brandId))
                {
                    if (!string.IsNullOrEmpty(productSizeId))
                    {
                        products = (from pro in db.tb_product
                                    where pro.status == true && pro.product_name.ToLower().Contains(term.ToLower()) && string.Compare(pro.brand_id, brandId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                    select pro).Take(100);
                    }
                    else
                    {
                        products = (from pro in db.tb_product
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.brand_id, brandId) == 0
                                    select pro).Take(100);
                    }

                }
                else if (!string.IsNullOrEmpty(productTypeId))
                {
                    if (!string.IsNullOrEmpty(productSizeId))
                    {
                        products = (from pro in db.tb_product
                                    //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                    //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_type_id, productTypeId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                    select pro).Take(100);
                    }
                    else
                    {
                        products = (from pro in db.tb_product
                                    //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                    //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_type_id, productTypeId) == 0
                                    select pro).Take(100);
                    }

                }
                else if (!string.IsNullOrEmpty(categoryId) || !string.IsNullOrEmpty(productSizeId))
                {
                    if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(productSizeId))
                    {
                        products = (from pro in db.tb_product
                                    //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                    //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                    //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_category_id, categoryId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
                                    select pro).Take(100);
                    }
                    else if (!string.IsNullOrEmpty(productSizeId))
                    {
                        products = (from pro in db.tb_product
                                    //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                    //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                    //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_size, productSizeId) == 0
                                    select pro).Take(100);
                    }
                    else
                    {
                        products = (from pro in db.tb_product
                                    //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                    //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                    //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_category_id, categoryId) == 0
                                    select pro).Take(100);
                    }

                }
                else
                {
                    products = (from pro in db.tb_product
                                //join br in db.tb_brand on pro.brand_id equals br.brand_id
                                //join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
                                //join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
                                //join cls in db.tb_class on cat.class_id equals cls.class_id
                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.group_id, classId) == 0
                                select pro).Take(100);

                }


            }
            List<ProductViewModel> models = new List<ProductViewModel>();

            foreach (var item in products)
            {
                models.Add(GetItemDetailbyId(item.product_id));
            }
            return Json(new { results = models }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetProductAutoSuggestName(string term, string classId, string categoryId, string productTypeId, string brandId, string productSizeId)
        //{
        //    kim_mexEntities db = new kim_mexEntities();
        //    if (string.IsNullOrEmpty(classId) && string.IsNullOrEmpty(categoryId) && string.IsNullOrEmpty(productTypeId) && string.IsNullOrEmpty(brandId) && string.IsNullOrEmpty(productSizeId))
        //        return Json((db.tb_product.Where(c =>(c.product_name.ToLower().Contains(term.ToLower()) || c.product_code.ToLower().StartsWith(term.ToLower())) && c.status == true).Select(a => new { label = a.product_code + " - " + a.product_name, id = a.product_id }).Take(100)), JsonRequestBehavior.AllowGet);
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(brandId))
        //        {
        //            if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                var products = (from pro in db.tb_product
        //                                where pro.status == true && pro.product_name.ToLower().Contains(term.ToLower()) && string.Compare(pro.brand_id, brandId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+" - "+ pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var products = (from pro in db.tb_product
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.brand_id, brandId) == 0
        //                                select new
        //                                {
        //                                    label = pro.product_code + " - " + pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else if (!string.IsNullOrEmpty(productTypeId))
        //        {
        //            if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                var products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(prot.product_type_id, productTypeId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+ " - " + pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(prot.product_type_id, productTypeId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+" - "+ pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else if (!string.IsNullOrEmpty(categoryId) || !string.IsNullOrEmpty(productSizeId))
        //        {
        //            if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(productSizeId))
        //            {
        //                var products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cat.p_category_id, categoryId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+" - "+ pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }
        //            else if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                var products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_size, productSizeId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+" - "+ pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                var products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cat.p_category_id, categoryId) == 0
        //                                select new
        //                                {
        //                                    label =pro.product_code+" - "+ pro.product_name,
        //                                    id = pro.product_id
        //                                }).Take(100);
        //                return Json(products, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //        else
        //        {
        //            var products = (from pro in db.tb_product
        //                            join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                            join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                            join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                            join cls in db.tb_class on cat.class_id equals cls.class_id
        //                            where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cls.class_id, classId) == 0
        //                            select new
        //                            {
        //                                label =pro.product_code+" - "+ pro.product_name,
        //                                id = pro.product_id
        //                            }).Take(100);
        //            return Json(products, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}

        //public ActionResult GetProductFilterResult(string term, string classId, string categoryId, string productTypeId, string brandId, string productSizeId)
        //{
        //    kim_mexEntities db = new kim_mexEntities();
        //    IQueryable<tb_product> products;
        //    if (string.IsNullOrEmpty(classId) && string.IsNullOrEmpty(categoryId) && string.IsNullOrEmpty(productTypeId) && string.IsNullOrEmpty(brandId) && string.IsNullOrEmpty(productSizeId))
        //    //products = db.tb_product.Where(c => (c.product_name.ToLower().Contains(term.ToLower()) || c.product_code.ToLower().StartsWith(term.ToLower())) && c.status == true).Select(a => new { a }).Take(100);
        //    {
        //        products = (from pro in db.tb_product
        //                    where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower()))
        //                    select pro).Take(100);
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(brandId))
        //        {
        //            if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                products = (from pro in db.tb_product
        //                            where pro.status == true && pro.product_name.ToLower().Contains(term.ToLower()) && string.Compare(pro.brand_id, brandId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                            select pro).Take(100);
        //            }
        //            else
        //            {
        //                products = (from pro in db.tb_product
        //                            where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.brand_id, brandId) == 0
        //                            select pro).Take(100);
        //            }

        //        }
        //        else if (!string.IsNullOrEmpty(productTypeId))
        //        {
        //            if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(prot.product_type_id, productTypeId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                                select pro).Take(100);
        //            }
        //            else
        //            {
        //                products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(prot.product_type_id, productTypeId) == 0
        //                                select pro).Take(100);
        //            }

        //        }
        //        else if (!string.IsNullOrEmpty(categoryId) || !string.IsNullOrEmpty(productSizeId))
        //        {
        //            if (!string.IsNullOrEmpty(categoryId) && !string.IsNullOrEmpty(productSizeId))
        //            {
        //                products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cat.p_category_id, categoryId) == 0 && string.Compare(pro.product_size, productSizeId) == 0
        //                                select pro).Take(100);
        //            }
        //            else if (!string.IsNullOrEmpty(productSizeId))
        //            {
        //                products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(pro.product_size, productSizeId) == 0
        //                                select pro).Take(100);
        //            }
        //            else
        //            {
        //                products = (from pro in db.tb_product
        //                                join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                                join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                                join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                                where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cat.p_category_id, categoryId) == 0
        //                                select pro).Take(100);
        //            }

        //        }
        //        else
        //        {
        //            products = (from pro in db.tb_product
        //                            join br in db.tb_brand on pro.brand_id equals br.brand_id
        //                            join prot in db.tb_product_type on br.product_type_id equals prot.product_type_id
        //                            join cat in db.tb_product_category on prot.product_category_id equals cat.p_category_id
        //                            join cls in db.tb_class on cat.class_id equals cls.class_id
        //                            where pro.status == true && (pro.product_name.ToLower().Contains(term.ToLower()) || pro.product_code.ToLower().StartsWith(term.ToLower())) && string.Compare(cls.class_id, classId) == 0
        //                            select pro).Take(100);

        //        }


        //    }
        //    List<ProductViewModel> models = new List<ProductViewModel>();

        //    foreach (var item in products)
        //    {
        //        models.Add(GetItemDetailbyId(item.product_id));
        //    }
        //    return Json(new { results = models }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetProductDataByID(string id, string cID)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var product = (from tbl in db.tb_product
                               orderby tbl.product_code
                               where tbl.product_id == id && tbl.brand_id == cID && tbl.status == true
                               select new ProductViewModel()
                               {
                                   product_id = tbl.product_id,
                                   product_code = tbl.product_code,
                                   product_name = tbl.product_name,
                                   product_unit = tbl.product_unit,
                                   unit_price = tbl.unit_price
                               }
                               ).FirstOrDefault();
                return Json(new { data = product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult GetProductDataByCode(string id)
        {
            try
            {
                ProductViewModel product = new ProductViewModel();
                kim_mexEntities db = new kim_mexEntities();
                product = (from tbl in db.tb_product
                           join pros in db.tb_product_class on tbl.product_class_id equals pros.product_class_id into pcl from pros in pcl.DefaultIfEmpty()
                           join unit in db.tb_unit on tbl.product_unit equals unit.Id into funit from unit in funit.DefaultIfEmpty()
                           join br in db.tb_brand on tbl.brand_id equals br.brand_id into bra from br in bra.DefaultIfEmpty()
                           join pt in db.tb_product_type on tbl.product_type_id equals pt.product_type_id into ptt
                           from pt in ptt.DefaultIfEmpty()
                           join cat in db.tb_product_category on tbl.product_category_id equals cat.p_category_id into catt
                           from cat in catt.DefaultIfEmpty()
                           join cls in db.tb_class on tbl.group_id equals cls.class_id into clss
                           from cls in clss.DefaultIfEmpty()
                           join pss in db.tb_product_size on tbl.product_size equals pss.product_size_id into psss
                           from pss in psss.DefaultIfEmpty()
                           orderby tbl.product_code
                           where tbl.product_id == id && tbl.status == true
                           select new ProductViewModel()
                           {
                               product_id = tbl.product_id,
                               product_code = tbl.product_code,
                               product_name = tbl.product_name,
                               product_unit = tbl.product_unit,
                               unit_price = tbl.unit_price,
                               unit_name = unit.Name,
                               product_class_name = pros.product_class_name,
                               model_factory_code = tbl.model_factory_code,
                               insulation_color = tbl.insulation_color,
                               sheath_color = tbl.sheath_color,
                               number_of_core = tbl.number_of_core,
                               outer_sheath = tbl.outer_sheath,
                               color = tbl.color,
                               sub_group_id=tbl.sub_group_id,
                               labour_hour= tbl.labour_hour,
                               cash_flow    = tbl.cash_flow,
                               so_number= tbl.so_number,
                           }).FirstOrDefault();
                var uom = db.tb_multiple_uom.Where(x => x.product_id == id).FirstOrDefault();
                if (uom != null)
                {

                    product.uom1_id = string.IsNullOrEmpty(uom.uom1_id) ? " " : Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                    product.uom1_qty = uom.uom1_qty;
                    product.uom2_id = string.IsNullOrEmpty(uom.uom2_id) ? " " : Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                    product.uom2_qty = uom.uom2_qty;
                    product.uom3_id = string.IsNullOrEmpty(uom.uom3_id) ? " " : Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                    product.uom3_qty = uom.uom3_qty;
                    product.uom4_id = string.IsNullOrEmpty(uom.uom4_id) ? " " : Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                    product.uom4_qty = uom.uom4_qty;
                    product.uom5_id = string.IsNullOrEmpty(uom.uom5_id) ? " " : Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                    product.uom5_qty = uom.uom5_qty;
                }
                return Json(new { data = product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetProductDatawithStockBalance(string id, string warehouseID)
        {
            try
            {
                ProductViewModel product = new ProductViewModel();
                kim_mexEntities db = new kim_mexEntities();
                product = (from tbl in db.tb_product
                           join unit in db.tb_unit on tbl.product_unit equals unit.Id
                           orderby tbl.product_code
                           where tbl.product_id == id && tbl.status == true
                           select new ProductViewModel()
                           {
                               product_id = tbl.product_id,
                               product_code = tbl.product_code,
                               product_name = tbl.product_name,
                               product_unit = tbl.product_unit,
                               unit_price = tbl.unit_price,
                               unit_name=unit.Name,
                           }).FirstOrDefault();
                var uom = db.tb_multiple_uom.Where(x => x.product_id == id).FirstOrDefault();
                if (uom != null)
                {
                    product.uom1_id = string.IsNullOrEmpty(uom.uom1_id) ? " " : Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                    product.uom1_qty = uom.uom1_qty;
                    product.uom2_id = string.IsNullOrEmpty(uom.uom2_id) ? " " : Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                    product.uom2_qty = uom.uom2_qty;
                    product.uom3_id = string.IsNullOrEmpty(uom.uom3_id) ? " " : Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                    product.uom3_qty = uom.uom3_qty;
                    product.uom4_id = string.IsNullOrEmpty(uom.uom4_id) ? " " : Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                    product.uom4_qty = uom.uom4_qty;
                    product.uom5_id = string.IsNullOrEmpty(uom.uom5_id) ? " " : Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                    product.uom5_qty = uom.uom5_qty;
                }
                var inventory = db.tb_inventory.OrderByDescending(x => x.inventory_date).Where(x => string.Compare(x.product_id, id) == 0 && string.Compare(x.warehouse_id, warehouseID) == 0).Select(x => x.total_quantity).FirstOrDefault();
                product.stock_balance = inventory == null ? 0 : Convert.ToDecimal(inventory);
                return Json(new { data = product }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GenerateItemCodebyItemType(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<Int32> codeItems = new List<int>();
                string itemCode = string.Empty;
                string typeCode = string.Empty;
                int lastCode = 0;
                //string lastItemCode = db.tb_product.OrderByDescending(p => p.product_code).Where(p => string.Compare(p.p_category_id, id) == 0).Select(p => p.product_code).FirstOrDefault().ToString();
                var items = db.tb_product.Where(p => string.Compare(p.brand_id, id) == 0).ToList();
                if (items.Any())
                {
                    foreach (var item in items)
                        codeItems.Add(Convert.ToInt32(Regex.Match(item.product_code, @"\d+").Value));
                    string lastItemCode = codeItems.OrderByDescending(x => x).FirstOrDefault().ToString();
                    string codeLetter = new String(lastItemCode.Where(Char.IsLetter).ToArray());
                    string codeNumber = Regex.Match(lastItemCode, @"\d+").Value;
                    if (codeNumber.Length > 2)
                    {
                        typeCode = codeNumber.Substring(0, 2);
                        lastCode = Convert.ToInt32(codeNumber.Substring(2, codeNumber.Length - 2)) + 1;

                    }
                    else
                        lastCode = Convert.ToInt32(codeNumber) + 1;
                    string itemTypeCode = db.tb_product_category.Where(pc => string.Compare(pc.p_category_id, id) == 0).Select(pc => pc.p_category_code).FirstOrDefault().ToString();
                    //itemCode = string.Format("{0}{1}", itemTypeCode, lastCode.ToString().Length == 1 ? "0" + lastCode.ToString() : lastCode.ToString());
                    string itemTypeLetter = new string(itemTypeCode.Where(Char.IsLetter).ToArray());
                    string itemTypeCode1 = string.IsNullOrEmpty(itemTypeLetter) ? itemTypeCode : string.Format("{0}{1}", itemTypeLetter, typeCode);
                    itemCode = string.Format("{0}{1}", itemTypeCode1, lastCode.ToString().Length == 1 ? "0" + lastCode.ToString() : lastCode.ToString());
                }
                else
                {
                    string itemTypeCode = db.tb_product_category.Where(pc => string.Compare(pc.p_category_id, id) == 0).Select(pc => pc.p_category_code).FirstOrDefault().ToString();
                    itemCode = string.Format("{0}01", itemTypeCode);
                }
                if (this.IsItemCodeExist(itemCode))
                    itemCode = this.GenerateNewItemCode(itemCode);
                return Json(new { code = itemCode }, JsonRequestBehavior.AllowGet);
            }
        }
        public List<tb_unit> GetUnitList()
        {
            List<tb_unit> units = new List<tb_unit>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                units = db.tb_unit.OrderBy(x => x.Name ).Where(x => x.status == true && !string.IsNullOrEmpty(x.Name)).ToList();
            }
            return units;
        }
        public List<tb_product_size> GetProductSize()
        {
            List<tb_product_size> units = new List<tb_product_size>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                units = db.tb_product_size.OrderBy(x => x.product_size_id).Where(x => x.active == true && !string.IsNullOrEmpty(x.product_size_name)).ToList();
            }
            return units;
        }
        public List<tb_product_category> GetProductCategoryList()
        {
            List<tb_product_category> categories = new List<tb_product_category>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                categories = db.tb_product_category.OrderBy(x => x.p_category_code).Where(x => x.status == true && !string.IsNullOrEmpty(x.p_category_name)).ToList();
            }
            return categories;
        }

        public List<tb_brand> GetBranchList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_brand.Where(x => x.active == true && !string.IsNullOrEmpty(x.brand_name)).OrderBy(x => x.brand_name).ToList();
            }
        }

        public List<tb_color> GetColorList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_color.Where(x => x.active == true && !string.IsNullOrEmpty(x.color_name)).OrderBy(x => x.color_name).ToList();
            }
        }
        public List<tb_product_class> GetProductClassList()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_product_class.Where(x => x.active == true && !string.IsNullOrEmpty(x.product_class_name)).OrderBy(x => x.product_class_name).ToList();
            }
        }

        public ActionResult GetProductUOM(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<UnitViewModel> units = new List<UnitViewModel>();
                string proUnit = db.tb_product.Where(m => m.product_id == id).Select(x => x.product_unit).FirstOrDefault();
                units.Add(new UnitViewModel() { Name = proUnit });
                var uom = db.tb_multiple_uom.Where(x => x.product_id == id).FirstOrDefault();
                if (uom != null)
                {

                }
                return Json(new { result = "success", data = units }, JsonRequestBehavior.AllowGet);
            }
        }
        //on page quote
        public ActionResult UnitDropDown()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var units = (from tblu in db.tb_unit orderby tblu.Name where tblu.status == true select tblu).ToList();
                return Json(new { data = units }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return null; }
        }
        public ActionResult TypeNameDropdownList()
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                List<CategoryViewModel> listtypename = new List<CategoryViewModel>();
                var listTypeName = (from tbl in db.tb_product_category orderby tbl.p_category_name where tbl.status == true select tbl).ToList();
                if (listTypeName.Any())
                {
                    foreach (var typeName in listTypeName)
                    {
                        listtypename.Add(new CategoryViewModel() { p_category_id = typeName.p_category_id, p_category_name = typeName.p_category_name, p_category_code = typeName.p_category_code });
                    }
                }
                return Json(new { data = listtypename }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //end quots
        //--------------- check duplicate item code --------------------//

        public bool IsItemCodeExist(string itemCode)
        {
            kim_mexEntities db = new kim_mexEntities();
            var objs = db.tb_product.Where(w => w.status == true && string.Compare(w.product_code, itemCode) == 0).ToList();
            if (objs.Any())
                return true;
            else
                return false;
        }
        public string GenerateNewItemCode(string itemCode)
        {
            string newItemCode = string.Empty;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                int itemNumber = 0;
                string itemTypeLetter = new string(itemCode.Where(Char.IsLetter).ToArray());
                int itemFullNumber = Convert.ToInt32(Regex.Match(itemCode, @"\d+").Value);
                int itemTypeNumber = Convert.ToInt32(itemFullNumber.ToString().Substring(0, 2));
                int _2digitItemCode = Convert.ToInt32(itemFullNumber.ToString().Substring(2, itemFullNumber.ToString().Length - 2));
                if (_2digitItemCode == 99)
                {
                    itemTypeNumber = itemTypeNumber + 1;
                    itemNumber = 1;
                }
                else
                    itemNumber = _2digitItemCode + 1;
                newItemCode = string.Format("{0}{1}{2}", itemTypeLetter, itemTypeNumber, itemNumber.ToString().Length == 1 ? "0" + itemNumber.ToString() : itemNumber.ToString());
                if (this.IsItemCodeExist(newItemCode))
                    newItemCode = GenerateNewItemCode(newItemCode);
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ProductController.cs", "CheckDuplicationItembyItemCode", ex.StackTrace, ex.Message);
            }
            return newItemCode;
        }

        #region added by Borith Jan 02 2019
        public ActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ImportExcel(string filename)
        {
            if (Request.Files.Count > 0)
            {
                List<ExcelProductViewModel> models = new List<ExcelProductViewModel>();
                ImportProductExcelReasponse response = new ImportProductExcelReasponse();
                var file0 = Request.Files[0];
                if (file0 != null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    response = ImportProductfromExcel.GetDataModelFromExcelContact(path, true);
                    return View(response);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult GenerateProductDescriptionJson(string categroy = null, string type = null, string pclass = null, string size = null, string color = null, string brand = null, string model = null)
        {
            return Json(new { data = CommonFunctions.GenerateProductDescription(categroy, type, pclass, size, color, brand, model) }, JsonRequestBehavior.AllowGet);
        }
        public ProductViewModel GetItemDetailbyId(string id)
        {
            try
            {
                ProductViewModel product = new ProductViewModel();
                kim_mexEntities db = new kim_mexEntities();
                //product = (from tbl in db.tb_product
                //           join pros in db.tb_product_class on tbl.product_class_id equals pros.product_class_id into pcl
                //           from pros in pcl.DefaultIfEmpty()
                //           join unit in db.tb_unit on tbl.product_unit equals unit.Id into funit
                //           from unit in funit.DefaultIfEmpty()
                //           join br in db.tb_brand on tbl.brand_id equals br.brand_id into bra
                //           from br in bra.DefaultIfEmpty()
                //           join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id into ptt
                //           from pt in ptt.DefaultIfEmpty()
                //           join cat in db.tb_product_category on pt.product_category_id equals cat.p_category_id into catt
                //           from cat in catt.DefaultIfEmpty()
                //           join cls in db.tb_class on cat.class_id equals cls.class_id into clss
                //           from cls in clss.DefaultIfEmpty()
                //           join pss in db.tb_product_size on tbl.product_size equals pss.product_size_id into psss
                //           from pss in psss.DefaultIfEmpty()
                //           orderby tbl.product_code
                //           where tbl.product_id == id && tbl.status == true
                //           select new ProductViewModel()
                //           {
                //               product_id = tbl.product_id,
                //               product_code = tbl.product_code,
                //               product_name = tbl.product_name,
                //               product_unit = tbl.product_unit,
                //               unit_price = tbl.unit_price,
                //               unit_name = unit.Name,
                //               product_class_name = pros.product_class_name,
                //               model_factory_code = tbl.model_factory_code,
                //               insulation_color = tbl.insulation_color,
                //               sheath_color = tbl.sheath_color,
                //               number_of_core = tbl.number_of_core,
                //               outer_sheath = tbl.outer_sheath,
                //               color = tbl.color,
                //           }).FirstOrDefault();

                product = (from pro in db.tb_product
                           join un in db.tb_unit on pro.product_unit equals un.Id into unn
                           from un in unn.DefaultIfEmpty()
                           join pc in db.tb_product_class on pro.product_class_id equals pc.product_class_id into pcl
                           from pc in pcl.DefaultIfEmpty()
                           join br in db.tb_brand on pro.brand_id equals br.brand_id into bra
                           from br in bra.DefaultIfEmpty()
                           join pt in db.tb_product_type on pro.product_type_id equals pt.product_type_id into ptt
                           from pt in ptt.DefaultIfEmpty()
                           join cat in db.tb_product_category on pro.product_category_id equals cat.p_category_id into catt
                           from cat in catt.DefaultIfEmpty()
                           join cls in db.tb_class on pro.group_id equals cls.class_id into clss
                           from cls in clss.DefaultIfEmpty()
                           join pss in db.tb_product_size on pro.product_size equals pss.product_size_id into psss
                           from pss in psss.DefaultIfEmpty()
                           join col in db.tb_color on pro.color equals col.color_id into jcol
                           from col in jcol.DefaultIfEmpty()
                           where pro.status == true && string.Compare(pro.product_id,id)==0
                           select new ProductViewModel 
                           {
                               product_id = pro.product_id,
                               product_code = pro.product_code,
                               product_name = pro.product_name,
                               product_unit = un.Name,
                               unit_price = pro.unit_price,
                               product_class_name = pc.product_class_name,
                               brand_name = br.brand_name,
                               created_date = pro.created_date,
                               p_category_name = cat.p_category_name,
                               class_name = cls.class_name,
                               product_type_name = pt.product_type_name,
                               product_size_name = pss.product_size_name,
                               color_id = pro.color,
                               color = pro.color,
                               color_name = col == null ? "" : col.color_name,
                               model_factory_code = pro.model_factory_code
                           }).FirstOrDefault();
                product.product_color_name = string.IsNullOrEmpty(product.color) ? string.Empty : db.tb_color.Find(product.color).color_name;

                var uom = db.tb_multiple_uom.Where(x => x.product_id == id).FirstOrDefault();
                if (uom != null)
                {

                    product.uom1_id = string.IsNullOrEmpty(uom.uom1_id) ? " " : Regex.Replace(uom.uom1_id, @"\t|\n|\r", "");
                    product.uom1_qty = uom.uom1_qty;
                    product.uom2_id = string.IsNullOrEmpty(uom.uom2_id) ? " " : Regex.Replace(uom.uom2_id, @"\t|\n|\r", "");
                    product.uom2_qty = uom.uom2_qty;
                    product.uom3_id = string.IsNullOrEmpty(uom.uom3_id) ? " " : Regex.Replace(uom.uom3_id, @"\t|\n|\r", "");
                    product.uom3_qty = uom.uom3_qty;
                    product.uom4_id = string.IsNullOrEmpty(uom.uom4_id) ? " " : Regex.Replace(uom.uom4_id, @"\t|\n|\r", "");
                    product.uom4_qty = uom.uom4_qty;
                    product.uom5_id = string.IsNullOrEmpty(uom.uom5_id) ? " " : Regex.Replace(uom.uom5_id, @"\t|\n|\r", "");
                    product.uom5_qty = uom.uom5_qty;
                }
                return product;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public ActionResult UploadExcelLabourHour()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UploadExcelLabourHour(string filename)
        {
            if (Request.Files.Count > 0)
            {
                List<ExcelProductViewModel> models = new List<ExcelProductViewModel>();
                ImportProductLabourHourResultResponse response = new ImportProductLabourHourResultResponse();
                var file0 = Request.Files[0];
                if (file0 != null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    response = ImportLabourHourExcelResponse.GetDataModelFromExcelContact(path, true);
                    return View(response);
                }
            }
            return View();
        }

        public ActionResult UpdateProductViaExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdateProductViaExcel(string filename)
        {
            if (Request.Files.Count > 0)
            {
                List<ExcelProductViewModel> models = new List<ExcelProductViewModel>();
                UpdateProductViaExcelResultResponse response = new UpdateProductViaExcelResultResponse();
                var file0 = Request.Files[0];
                if (file0 != null && file0.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Documents/"), Path.GetFileName(file0.FileName));
                    file0.SaveAs(path);
                    response = UpdateProductViaExcelModel.GetDataModelFromExcelContent(path, true);
                    return View(response);
                }
            }
            return View();
        }

    }

   
}


