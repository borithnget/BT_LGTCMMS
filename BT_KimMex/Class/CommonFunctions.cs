using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Models;
using BT_KimMex.Entities;
using Microsoft.AspNet.Identity;
using System.Net;
using Microsoft.Ajax.Utilities;
using System.IO.IsolatedStorage;

namespace BT_KimMex.Class
{
    public class CommonFunctions
    {
        //private static string id;
        //private static object un;
        #region Class Type
        public static List<ClassTypeViewModel> GetClassTypeListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_class_type.OrderBy(o => o.class_type_name).Where(w => w.active == true).Select(s => new ClassTypeViewModel()
                {
                    class_type_id = s.class_type_id,
                    class_type_name = s.class_type_name,
                    created_at = s.created_at,
                    updated_at = s.updated_at,
                }).ToList();
            }
        }
public static ClassTypeViewModel GetClassTypeItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_class_type.Where(w => string.Compare(w.class_type_id, id) == 0).Select(s => new ClassTypeViewModel()
                {
                    class_type_id = s.class_type_id,
                    class_type_name = s.class_type_name,
                    created_at = s.created_at,
                    updated_at = s.updated_at
                }).FirstOrDefault();
            }
        }

        #endregion
        #region Product Class
        public static List<ProductClassViewModel> GetProductClassListItems(string group_id="")
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ProductClassViewModel> models = new List<ProductClassViewModel>();
                if(string.IsNullOrEmpty(group_id))
                    return (from pc in db.tb_product_class
                        join ct in db.tb_class_type on pc.class_type_id equals ct.class_type_id
                        orderby pc.product_class_name
                        where pc.active == true
                        select new ProductClassViewModel()
                        {
                            product_class_id = pc.product_class_id,
                            product_class_name = pc.product_class_name,
                            created_at = pc.created_at,
                            updated_at = pc.updated_at,
                            class_type_id = pc.class_type_id,
                            class_type_name = ct.class_type_name
                        }).ToList();
                else
                {
                    return (from pc in db.tb_product_class
                            join ct in db.tb_class_type on pc.class_type_id equals ct.class_type_id
                            join gr in db.tb_class on ct.class_type_id equals gr.class_type_id
                            orderby pc.product_class_name
                            where pc.active == true && string.Compare(gr.class_id,group_id)==0
                            select new ProductClassViewModel()
                            {
                                product_class_id = pc.product_class_id,
                                product_class_name = pc.product_class_name,
                                created_at = pc.created_at,
                                updated_at = pc.updated_at,
                                class_type_id = pc.class_type_id,
                                class_type_name = ct.class_type_name
                            }).ToList();
                }
            }
        }
        public static ProductClassViewModel GetProductClassItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from pc in db.tb_product_class
                        join ct in db.tb_class_type on pc.class_type_id equals ct.class_type_id
                        orderby pc.product_class_name
                        where pc.active == true && string.Compare(pc.product_class_id, id) == 0
                        select new ProductClassViewModel()
                        {
                            product_class_id = pc.product_class_id,
                            product_class_name = pc.product_class_name,
                            created_at = pc.created_at,
                            class_type_id = pc.class_type_id,
                            class_type_name = ct.class_type_name,
                            updated_at = pc.updated_at,
                        }).FirstOrDefault();
            }
        }
        #endregion
        #region Class
        public static List<ClassViewModel> GetClassListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from cs in db.tb_class
                        join ct in db.tb_class_type on cs.class_type_id equals ct.class_type_id
                        orderby cs.class_name
                        where cs.active == true
                        select new ClassViewModel()
                        {
                            class_id = cs.class_id,
                            class_type_id = cs.class_type_id,
                            class_name = cs.class_name,
                            created_at = cs.created_at,
                            class_type_name = ct.class_type_name,
                            updated_at = cs.updated_at,
                            class_code=cs.class_code
                        }).ToList();
            }
        }

        internal static object GetProductSizeItems()
        {
            throw new NotImplementedException();
        }

        public static ClassViewModel GetClassItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from cs in db.tb_class
                        join ct in db.tb_class_type on cs.class_type_id equals ct.class_type_id
                        where string.Compare(cs.class_id, id) == 0
                        select new ClassViewModel()
                        {
                            class_id = cs.class_id,
                            class_type_id = cs.class_type_id,
                            class_name = cs.class_name,
                            created_at = cs.created_at,
                            class_type_name = ct.class_type_name,
                            updated_at = cs.updated_at,
                            class_code=cs.class_code,
                        }).FirstOrDefault();
            }
        }
        #endregion
        #region Product type
        public static List<ProductTypeViewModel> GetProductTypeListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from pt in db.tb_product_type
                        //join cat in db.tb_product_category on pt.product_category_id equals cat.p_category_id
                        join gr in db.tb_class on pt.product_category_id equals gr.class_id
                        orderby pt.product_type_name
                        where pt.active == true && !string.IsNullOrEmpty(pt.product_type_name)
                        select new ProductTypeViewModel()
                        {
                            product_type_id = pt.product_type_id,
                            product_category_id = pt.product_category_id,
                            product_type_name = pt.product_type_name,
                            created_at = pt.created_at,
                            updated_at = pt.updated_at,
                            product_category_name = gr.class_name,
                        }).ToList();
            }
        }

        //public static List<ProductTypeViewModel> GetProductTypeListItems()
        //{
        //    using (kim_mexEntities db = new kim_mexEntities())
        //    {
        //        return (from pt in db.tb_product_type
        //                join cat in db.tb_product_category on pt.product_category_id equals cat.p_category_id
        //                where string.Compare(pt.product_type_id, id) == 0
        //                select new ProductTypeViewModel()
        //                {

        //                    product_type_id = pt.product_type_id,
        //                    product_category_id = pt.product_category_id,
        //                    product_type_name = pt.product_type_name,
        //                    created_at = pt.created_at,
        //                    updated_at = pt.updated_at,
        //                    product_category_name = cat.p_category_name,
        //                }).ToList();

        //    }
        //}
        public static ProductTypeViewModel GetProductTypeItems(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from pt in db.tb_product_type
                        //join cat in db.tb_product_category on pt.product_category_id equals cat.p_category_id
                        join gr in db.tb_class on pt.product_category_id equals gr.class_id
                        where string.Compare(pt.product_type_id, id) == 0
                        select new ProductTypeViewModel()
                        {

                            product_type_id = pt.product_type_id,
                            product_category_id = pt.product_category_id,
                            product_type_name = pt.product_type_name,
                            created_at = pt.created_at,
                            updated_at = pt.updated_at,
                            product_category_name = gr.class_name,
                        }).FirstOrDefault();
            }
        }
        public static List<ProductTypeViewModel> GetProductTypeListItemsbyCategoryId(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    return db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true).Select(s => new ProductTypeViewModel()
                    {
                        product_type_id = s.product_type_id,
                        product_type_name = s.product_type_name
                    }).ToList();
                }
                else
                {
                    return db.tb_product_type.OrderBy(o => o.product_type_name).Where(w => w.active == true && string.Compare(w.product_category_id, id) == 0).Select(s => new ProductTypeViewModel()
                    {
                        product_type_id = s.product_type_id,
                        product_type_name = s.product_type_name
                    }).ToList();
                }
            }
        }

        #endregion
        #region Brand
        public static List<BrandViewModel> GetBrandListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from br in db.tb_brand
                        join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                        orderby br.brand_name
                        where br.active == true
                        select new BrandViewModel()
                        {
                            brand_id = br.brand_id,
                            product_type_id = br.product_type_id,
                            brand_name = br.brand_name,
                            created_at = br.created_at,
                            updated_at = br.updated_at,
                            product_type_name = pt.product_type_name
                        }).ToList();
            }
        }
        public static BrandViewModel GetBrandItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from br in db.tb_brand
                        join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                        where string.Compare(br.brand_id, id) == 0
                        select new BrandViewModel()
                        {
                            brand_id = br.brand_id,
                            product_type_id = br.product_type_id,
                            brand_name = br.brand_name,
                            created_at = br.created_at,
                            product_type_name = pt.product_type_name
                        }).FirstOrDefault();
            }
        }
        public static List<BrandViewModel> GetBrandListItemsbyCategoryId(string id, string group_id = "")
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(group_id))
                    {
                        var models = (from br in db.tb_brand
                                      join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                      where br.active == true
                                      select new BrandViewModel()
                                      {
                                          brand_id = br.brand_id,
                                          brand_name = br.brand_name,
                                          product_type_name = pt.product_type_name
                                      }).ToList();
                        return models;
                    }
                    else
                    {
                        var models = (from br in db.tb_brand
                                      join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                      where br.active == true && string.Compare(pt.product_category_id, group_id) == 0
                                      select new BrandViewModel()
                                      {
                                          brand_id = br.brand_id,
                                          brand_name = br.brand_name,
                                          product_type_name = pt.product_type_name
                                      }).ToList();
                        return models;
                    }
                }
                else
                {
                    var models = (from br in db.tb_brand
                                  join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                                  where br.active == true && string.Compare(pt.product_type_id, id) == 0
                                  select new BrandViewModel()
                                  {
                                      brand_id = br.brand_id,
                                      brand_name = br.brand_name,
                                      product_type_name = pt.product_type_name
                                  }).ToList();
                    return models;

                }

            }
        }

        #endregion
        #region Category
        public static List<CategoryViewModel> GetCategoryListItem()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from tbl in db.tb_product_category
                        join cl in db.tb_class on tbl.class_id equals cl.class_id
                        where !string.IsNullOrEmpty(tbl.class_id) && tbl.status == true
                        select new CategoryViewModel
                        {
                            p_category_id = tbl.p_category_id,
                            p_category_code = tbl.p_category_code,
                            p_category_name = tbl.p_category_name,
                            class_name = cl.class_name,
                            p_category_address = tbl.p_category_address,
                            created_date = tbl.created_date,
                            updated_date = tbl.updated_date,
                            chart_account = tbl.chart_account,
                        }).ToList();
            }
        }

        public static CategoryViewModel GetCategoryItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from tbl in db.tb_product_category
                        join cl in db.tb_class on tbl.class_id equals cl.class_id
                        where string.Compare(tbl.p_category_id,id)==0
                        select new CategoryViewModel
                        {
                            p_category_id = tbl.p_category_id,
                            p_category_code = tbl.p_category_code,
                            p_category_name = tbl.p_category_name,
                            class_id=tbl.class_id,
                            class_name = cl.class_name,
                            p_category_address = tbl.p_category_address,
                            created_date = tbl.created_date,
                            updated_date = tbl.updated_date,
                            chart_account = tbl.chart_account,
                        }).FirstOrDefault();
            }
        }
        public static List<CategoryViewModel> GetCategoryDataDropdownlistbyClassId(string id,string p_category_id,string product_id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(id))
                {
                    return db.tb_product_category.OrderBy(o => o.p_category_name).Where(w => w.status == true && !string.IsNullOrEmpty(w.p_category_name)).Select(s => new CategoryViewModel()
                    {
                        p_category_id = s.p_category_id,
                        p_category_name = s.p_category_name
                    }).ToList();
                }
                else
                {
                    List<CategoryViewModel> results = new List<CategoryViewModel>();

                    if (!string.IsNullOrEmpty(p_category_id))
                    {
                        tb_product_category productCategory = db.tb_product_category.Where(s => s.status == true && string.Compare(s.class_id, id) == 0 && string.Compare(s.p_category_id, p_category_id) == 0).FirstOrDefault();
                        if (productCategory == null)
                        {
                            string p_category_name = db.tb_product_category.Find(p_category_id).p_category_name;
                            productCategory = new tb_product_category();
                            productCategory.p_category_id = Guid.NewGuid().ToString();
                            productCategory.class_id = id;
                            productCategory.p_category_name = p_category_name;
                            productCategory.status = true;
                            productCategory.created_date = CommonClass.ToLocalTime(DateTime.Now);
                            productCategory.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                            db.tb_product_category.Add(productCategory);
                            db.SaveChanges();

                            tb_product product = db.tb_product.Find(product_id);
                            if (product != null)
                            {
                                product.product_category_id = productCategory.p_category_id;
                                db.SaveChanges();
                            }
                        }
                    }

                    results= db.tb_product_category.OrderBy(o => o.p_category_name).Where(w => w.status == true && string.Compare(w.class_id, id) == 0).Select(s => new CategoryViewModel()
                    {
                        p_category_id = s.p_category_id,
                        p_category_name = s.p_category_name
                    }).ToList();



                    return results;

                }
            }
        }
        #endregion
        #region Color
        public static List<ColorViewModel> GetColorListItem()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from cl in db.tb_color
                        where !string.IsNullOrEmpty(cl.color_id) && cl.active == true
                        select new ColorViewModel
                        {
                            color_id = cl.color_id,
                            color_name = cl.color_name,
                            created_by =cl.created_by,
                            updated_by = cl.updated_by,
                        }).ToList();

            }
        }
        public static ColorViewModel GetColorItem(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from cl in db.tb_color
                        where !string.IsNullOrEmpty(cl.color_id) && cl.active == true
                        select new ColorViewModel
                        {
                            color_id = cl.color_id,
                            color_name = cl.color_name,
                            created_by = cl.created_by,
                            updated_by = cl.updated_by,
                        }).FirstOrDefault();

            }
        }

        #endregion
        #region Supplier
        public static List<SupplierViewModel> GetSupplierListItems()
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return db.tb_supplier.OrderBy(o => o.supplier_name).Where(w => w.status == true).Select(s => new SupplierViewModel()
                {
                    supplier_id = s.supplier_id,
                    supplier_name = s.supplier_name
                }).ToList();
            }
        }
        #endregion
        #region Product
        public static string GenerateProductCode()
        {
            string productCode = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var product = db.tb_product.OrderByDescending(o => o.created_date).FirstOrDefault();
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.product_code))
                    {
                        productCode = Convert.ToString(Convert.ToInt32(product.product_code) + 1);
                    }
                    else
                        productCode = "1";
                }
                else
                    productCode = "1";
            }
            return productCode;
        }
        public static ProductCodeResponse GenerateProductCode(string clasa_id,string sub_group_id)
        {
            string productCode = string.Empty;
            decimal productNumber = 1;
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //gnerate via auto increament
                var product = db.tb_product.OrderByDescending(o => o.product_number).FirstOrDefault();
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.product_code))
                    {
                        productNumber = Convert.ToInt32(product.product_number) + 1;
                    }
                    else
                        productNumber = 1;
                }

                //Generate by subgroup
                //var count = db.tb_product.Where(s => s.status == true && string.Compare(s.group_id, clasa_id) == 0 && string.Compare(s.sub_group_id, sub_group_id) == 0).Count();
                //productNumber = count + 1;

                string productCodeString = productNumber.ToString().Length == 1 ? string.Format("000{0}", productNumber) : 
                    productNumber.ToString().Length == 2 ? string.Format("00{0}", productNumber) : 
                    productNumber.ToString().Length == 3 ? string.Format("0{0}", productNumber) : 
                    string.Format("{0}", productNumber);

                var group = db.tb_class.Find(clasa_id);
                var sub_group = db.tb_sub_group.Find(sub_group_id);
                productCode = string.Format("{0}-{1}-{2}", group.class_code, sub_group.sub_group_code, productCodeString);
            }
            return new ProductCodeResponse(productCode,productNumber);
        }
        public static String ReGenerateProductCode(string class_id, string sub_group_id,tb_product product)
        {
            string productCode = string.Empty ;
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var productNumber = product.product_number;
                string productCodeString = productNumber.ToString().Length == 1 ? string.Format("000{0}", productNumber) :
                    productNumber.ToString().Length == 2 ? string.Format("00{0}", productNumber) :
                    productNumber.ToString().Length == 3 ? string.Format("0{0}", productNumber) :
                    string.Format("{0}", productNumber);

                var group = db.tb_class.Find(class_id);
                var sub_group = db.tb_sub_group.Find(sub_group_id);
                productCode = string.Format("{0}-{1}-{2}", group.class_code, sub_group.sub_group_code, productCodeString);
            }
            catch(Exception)
            {

            }
            return productCode;
        }
        public static string GenerateProductDescription(string category = null, string type = null, string pclass = null, string psize = null, string pcolor = null, string brand = null, string model = null,string classs=null)
        {
            string productDescription = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                string strCategory = string.IsNullOrEmpty(category) ? string.Empty : db.tb_product_category.Where(w => string.Compare(w.p_category_id, category) == 0).Select(s => s.p_category_name).FirstOrDefault().ToString();
                string strType = string.IsNullOrEmpty(type) ? string.Empty : db.tb_product_type.Where(w => string.Compare(w.product_type_id, type) == 0).Select(s => s.product_type_name).FirstOrDefault().ToString();
                string strClass = string.IsNullOrEmpty(pclass) ? string.Empty :db.tb_product_class.Where(w => string.Compare(w.product_class_id, pclass) == 0).Select(s => s.product_class_name).FirstOrDefault().ToString();
                string strSize = string.IsNullOrEmpty(psize) ? string.Empty : db.tb_product_size.Find(psize).product_size_name.ToString();
                string strBrand = string.IsNullOrEmpty(brand) ? string.Empty : db.tb_brand.Find(brand).brand_name.ToString();
                string strClasss = string.IsNullOrEmpty(classs) ? string.Empty : db.tb_class.Where(w => string.Compare(w.class_id, classs) == 0).Select(s => s.class_name).FirstOrDefault().ToString();
                string strColor = string.IsNullOrEmpty(pcolor) ? string.Empty : db.tb_color.Where(w => string.Compare(w.color_id, pcolor) == 0).Select(s => s.color_name).FirstOrDefault().ToString();
                 
                productDescription = string.Format("{0} {1} {2} {3} {4} {5} {6}", strCategory, strType, strClass, strSize, strColor, strBrand, model,classs);
            }
            return productDescription;
        }
        public static string GenerateProductDescription1(string category = null, string type = null, string pclass = null, string psize = null, string pcolor = null, string brand = null, string model = null, string classs = null)
        {
            string productDescription = string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                productDescription = string.Format("{0} {1} {2} {3} {4} {5} {6}", category, type, pclass, psize, pcolor, brand, model,classs);
            }
            return productDescription;
        }
        #endregion
        #region Product Size
        public static ProductSizeViewModel GetProductSizeItems(string id)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                return (from ps in db.tb_product_size
                        //join br in db.tb_brand on ps.brand_id equals br.brand_id
                        //join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                        //join pc in db.tb_product_category on pt.product_category_id equals pc.p_category_id
                        join cls in db.tb_class on ps.brand_id equals cls.class_id into pcls from cls in pcls.DefaultIfEmpty()
                        orderby ps.product_size_name
                        where ps.active == true && ps.product_size_id == id
                        select new ProductSizeViewModel()
                        {
                            product_size_id = ps.product_size_id,
                            class_id = ps.brand_id,
                            class_name = cls.class_name,
                            product_size_name = ps.product_size_name,
                            created_at = ps.created_at,
                            updated_at = ps.updated_at,
                            //class_name = cls.brand_name
                        }).FirstOrDefault();
            }
        }
        public static List<ProductSizeViewModel> GetProductSizeListItems(string group_id="")
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                if (string.IsNullOrEmpty(group_id))
                {
                    var productSizes = db.tb_product_size.OrderBy(o => o.product_size_name).Where(w => w.active == true).Select(s => new ProductSizeViewModel()
                    {
                        product_size_id = s.product_size_id,
                        product_size_name = s.product_size_name,
                        updated_at = s.updated_at

                    }).ToList();
                    return productSizes;
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
                               where ps.active == true && string.Compare(ps.brand_id, group_id) == 0
                               select new ProductSizeViewModel
                               {
                                   product_size_id = ps.product_size_id,
                                   product_size_name = ps.product_size_name,
                                   updated_at = ps.updated_at
                               };

                    var productSizes = size.ToList();
                    return productSizes;
                }
            }
        }
        #endregion
        #region User
        public static string GetUserFullnamebyUserId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var user = db.tb_user_detail.Where(w => string.Compare(w.user_id, id) == 0).FirstOrDefault();
                if (user == null)
                    return string.Empty;
                return string.Format("{0} {1}", user.user_first_name, user.user_last_name);
            }

        }

        internal static string GenerateProductDescription(object p_category_id, object product_type_id, object product_class_id, string product_size, object color_id, object brand_id, object model_factory_code)
        {
            throw new NotImplementedException();
        }

        internal static string GenerateProductDescription(string p_category_id, object product_type_id, object product_class_id, string product_size, object color_id, object brand_id, object model_factory_code)
        {
            throw new NotImplementedException();
        }

        internal static string GenerateProductDescription(string p_category_id, string product_type_id, object product_class_id, string product_size, object color_id, object brand_id, object model_factory_code)
        {
            throw new NotImplementedException();
        }

        internal static string GenerateProductDescription(string p_category_id, string product_type_id, string product_class_id, string product_size, object color_id, object brand_id, object model_factory_code)
        {
            throw new NotImplementedException();
        }

        internal static string GenerateProductDescription(string p_category_id, string product_type_id, string product_class_id, string product_size, object color_id, object brand_id, string model_factory_code)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Quote
        public static List<PurchaseOrderViewModel> GetMyPendingApprovalRequest(string userId, bool isAdmin, bool isCFO, bool isDirector, bool isOD)
        {
            List<PurchaseOrderViewModel> purchaseOrders = new List<PurchaseOrderViewModel>();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                IQueryable<PurchaseOrderViewModel> pos;
                if (isAdmin || (isCFO && isDirector && isOD))
                {
                    pos = (from po in db.tb_purchase_order
                           join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                           join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                           join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                           where po.status == true 
                           && string.Compare(po.purchase_order_status, "Completed") != 0 
                           && string.Compare(po.purchase_order_status.ToLower(), Status.Rejected) != 0 
                           && string.Compare(po.purchase_order_status.ToLower(), Status.cancelled) != 0 
                           && string.Compare(po.purchase_order_status.ToLower(), Status.RequestCancelled) != 0
                           && string.Compare(po.purchase_order_status.ToLower(),Status.MREditted)!=0

                           select new PurchaseOrderViewModel {
                               purchase_order_id = po.purchase_order_id,
                               purchase_oder_number = po.purchase_oder_number,
                               item_request_id = po.item_request_id,
                               ir_no = pr.purchase_requisition_number,
                               project_full_name = pro.project_full_name,
                               purchase_order_status = po.purchase_order_status,
                               created_by = po.created_by,
                               checked_by = po.checked_by,
                               approved_by = po.approved_by,
                               created_date = po.created_date,
                               mr_id=ir.ir_id,
                               mr_no=ir.ir_no
                           });
                    //if (pos.Any())
                    //{
                    //    foreach (var po in pos)
                    //    {
                    //        //List<string> ReportNumberVAT = new List<string>();
                    //        //List<string> ReportNumberNonVAT = new List<string>();
                    //        //var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                    //        //var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                    //        //foreach (var vat in vatNumbers)
                    //        //    ReportNumberVAT.Add(vat.po_report_number);
                    //        //foreach (var vat in vatNonNumbers)
                    //        //    ReportNumberNonVAT.Add(vat.po_report_number);

                    //        purchaseOrders.Add(new PurchaseOrderViewModel() {
                    //            purchase_order_id = po.purchase_order_id,
                    //            purchase_oder_number = po.purchase_oder_number,
                    //            item_request_id = po.purchase_order_id,
                    //            ir_no = po.ir_no,
                    //            project_full_name = po.project_full_name,
                    //            purchase_order_status = po.purchase_order_status,
                    //            created_by = po.created_by,
                    //            checked_by = po.checked_by,
                    //            approved_by = po.approved_by,
                    //            created_date = po.created_date,
                    //            created_by_fullname = CommonClass.GetUserFullnameByUserId(po.created_by),
                    //            strReqeustedDate = CommonClass.ToLocalTime(Convert.ToDateTime(po.created_date)).ToString("dd/MM/yyyy"),
                    //            purchase_order_show_status = ShowStatus.GetQuoteShowStatus(po.purchase_order_status),
                    //            mr_id = po.mr_id,
                    //            //ReportNumberVAT = ReportNumberVAT,
                    //            //ReportNumberNonVAT = ReportNumberNonVAT
                    //        });
                    //    }
                    //}
                }
                else
                {
                    pos = null;
                    //Project Manager
                    if (isCFO)
                    {
                        pos = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               join pm in db.tb_project_pm on pro.project_id equals pm.project_id
                               where po.status == true && string.Compare(po.purchase_order_status, "Pending") == 0 && string.Compare(pm.project_manager_id,userId)==0
                               select new PurchaseOrderViewModel {
                                   purchase_order_id = po.purchase_order_id,
                                   purchase_oder_number = po.purchase_oder_number,
                                   item_request_id = po.item_request_id,
                                   ir_no = pr.purchase_requisition_number,
                                   project_full_name = pro.project_full_name,
                                   purchase_order_status = po.purchase_order_status,
                                   created_by = po.created_by,
                                   checked_by = po.checked_by,
                                   approved_by = po.approved_by,
                                   created_date = po.created_date,
                                   mr_id=ir.ir_id,
                                   mr_no=ir.ir_no
                               });
                    }
                    //--- Technical Director 
                    if (isDirector)
                    {
                        pos = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               where po.status == true && string.Compare(po.purchase_order_status, "Approved") == 0
                               select new PurchaseOrderViewModel {
                                   purchase_order_id = po.purchase_order_id,
                                   purchase_oder_number = po.purchase_oder_number,
                                   item_request_id = po.item_request_id,
                                   ir_no = pr.purchase_requisition_number,
                                   project_full_name = pro.project_full_name,
                                   purchase_order_status = po.purchase_order_status,
                                   mr_id = ir.ir_id,
                                   mr_no = ir.ir_no,
                                   created_by = po.created_by, checked_by = po.checked_by, approved_by = po.approved_by, created_date = po.created_date });
                    }
                    //--- Operation Director
                    if (isOD)
                    {
                        pos = (from po in db.tb_purchase_order
                               join pr in db.tb_purchase_requisition on po.item_request_id equals pr.purchase_requisition_id
                               join ir in db.tb_item_request on pr.material_request_id equals ir.ir_id
                               join pro in db.tb_project on ir.ir_project_id equals pro.project_id
                               where po.status == true && string.Compare(po.purchase_order_status, Status.Checked) == 0
                               select new PurchaseOrderViewModel
                               {
                                   purchase_order_id = po.purchase_order_id,
                                   purchase_oder_number = po.purchase_oder_number,
                                   item_request_id = po.item_request_id,
                                   ir_no = pr.purchase_requisition_number,
                                   project_full_name = pro.project_full_name,
                                   purchase_order_status = po.purchase_order_status,
                                   mr_id = ir.ir_id,
                                   mr_no = ir.ir_no,
                                   created_by = po.created_by,
                                   checked_by = po.checked_by,
                                   approved_by = po.approved_by,
                                   created_date = po.created_date
                               });
                    }

                    
                }
                if (pos.Any())
                {
                    foreach (var po in pos)
                    {
                        //List<string> ReportNumberVAT = new List<string>();
                        //List<string> ReportNumberNonVAT = new List<string>();
                        //var vatNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == true && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        //var vatNonNumbers = db.tb_po_report.OrderBy(x => x.po_report_number).Where(x => x.vat_status == false && string.Compare(x.po_ref_id, po.purchase_order_id) == 0).Select(x => new { x.po_report_number }).ToList();
                        //foreach (var vat in vatNumbers)
                        //    ReportNumberVAT.Add(vat.po_report_number);
                        //foreach (var vat in vatNonNumbers)
                        //    ReportNumberNonVAT.Add(vat.po_report_number);

                        purchaseOrders.Add(new PurchaseOrderViewModel()
                        {
                            purchase_order_id = po.purchase_order_id,
                            purchase_oder_number = po.purchase_oder_number,
                            item_request_id = po.purchase_order_id,
                            ir_no = po.ir_no,
                            project_full_name = po.project_full_name,
                            purchase_order_status = po.purchase_order_status,
                            created_by = po.created_by,
                            checked_by = po.checked_by,
                            approved_by = po.approved_by,
                            created_date = po.created_date,
                            //ReportNumberVAT = ReportNumberVAT,
                            //ReportNumberNonVAT = ReportNumberNonVAT
                            created_by_fullname = CommonClass.GetUserFullnameByUserId(po.created_by),
                            strReqeustedDate = CommonClass.ToLocalTime(Convert.ToDateTime(po.created_date)).ToString("dd/MM/yyyy"),
                            purchase_order_show_status = ShowStatus.GetQuoteShowStatus(po.purchase_order_status),
                            mr_no = po.mr_no,
                            mr_id = po.mr_id
                        });
                    }
                }
            }
            return purchaseOrders;
        }
        #endregion
        #region General Functions
        public static void SaveProcessWorkflow(string refId,string status,string userId,string reason = "")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //tb_workflow workflow = new tb_workflow();
                //workflow.ref_id = refId;
                //workflow.workflow_status = status;
                //workflow.created_at = CommonClass.ToLocalTime(DateTime.Now);
                //workflow.created_by = userId;
                //workflow.active = true;
                //workflow.reason = reason;
                //db.tb_workflow.Add(workflow);
                //db.SaveChanges();
            }
        }
        public static bool isSMinSitebyPurchaseRequisition(string id,string userid)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var issm = (
                    from po in db.tb_purchase_request
                    join pr in db.tb_purchase_order on po.purchase_order_id equals pr.purchase_order_id
                    join prr in db.tb_purchase_requisition on pr.item_request_id equals prr.purchase_requisition_id
                            join mr in db.tb_item_request on prr.material_request_id equals mr.ir_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            join smpro in db.tb_site_manager_project on pro.project_id equals smpro.project_id
                            where string.Compare(po.pruchase_request_id, id) == 0 && string.Compare(smpro.site_manager, userid) == 0
                            select pr).ToList();
                if (issm.Any()) return true;
                return false;
            }
        }
        public static bool isSMinSitebyStockTransfer(string id,string userid)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var issm = (from st in db.tb_stock_transfer_voucher
                            join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            join smpro in db.tb_site_manager_project on pro.project_id equals smpro.project_id
                            where string.Compare(st.stock_transfer_id, id) == 0 && string.Compare(smpro.site_manager, userid) == 0
                            select st).ToList();
                if (issm.Any()) return true;
                return false;
            }
        }
        public static bool isSMinSitebyStockReturn(string id, string userid)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var issm = (from rt in db.tb_stock_issue_return
                            join st in db.tb_stock_transfer_voucher on rt.stock_issue_ref equals st.stock_transfer_id
                            join wh in db.tb_warehouse on st.from_warehouse_id equals wh.warehouse_id
                            join pro in db.tb_project on wh.warehouse_site_id equals pro.site_id
                            join smpro in db.tb_site_manager_project on pro.project_id equals smpro.project_id
                            where string.Compare(rt.stock_issue_return_id, id) == 0 && string.Compare(smpro.site_manager, userid) == 0
                            select rt).ToList();

                if (issm.Any()) return true;
                return false;
            }
        }
        public static bool isSMinSitebyWorkShopTransfer(string id, string userid)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                var issm = (from st in db.transferformmainstocks
                            join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                            join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                            join smpro in db.tb_site_manager_project on pro.project_id equals smpro.project_id
                            where string.Compare(st.stock_transfer_id, id) == 0 && string.Compare(smpro.site_manager, userid) == 0
                            select st).ToList();
                if (issm.Any()) return true;
                return false;
            }
        }
        #region check project permission on qaqc
        public static bool isQCQAbyWorkshopTransfer(string transferId,string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var isQAQC = (from wt in db.transferformmainstocks
                              join mr in db.tb_item_request on wt.item_request_id equals mr.ir_id
                              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                              join whqaqc in db.tb_warehouse_qaqc on wh.warehouse_id equals whqaqc.warehouse_id
                              where string.Compare(wt.stock_transfer_id, transferId) == 0 && string.Compare(whqaqc.qaqc_id, userId) == 0
                              select wh).ToList();
                if (isQAQC.Any())
                    return true;
                return false;
            }
        }
        public static bool isQAQCbyStockTransfer(string transferId,string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var isQAQC = (from st in db.tb_stock_transfer_voucher
                              join mr in db.tb_item_request on st.item_request_id equals mr.ir_id
                              join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                              join wh in db.tb_warehouse on proj.project_id equals wh.warehouse_project_id
                              join qaqcwh in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqcwh.warehouse_id
                              where string.Compare(st.stock_transfer_id, transferId) == 0 && string.Compare(qaqcwh.qaqc_id, userId) == 0
                              select st).ToList();
                if (isQAQC.Any()) return true;
                return false;
            }
        }
        public static bool isQAQCbyPurchaseOrder(string poId, string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var isQAQC = (from po in db.tb_purchase_request
                              join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id
                              join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                              join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                              join pro in db.tb_project on mr.ir_project_id equals pro.project_id
                              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                              join qaqcwh in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqcwh.warehouse_id
                              where string.Compare(po.pruchase_request_id, poId) == 0 && string.Compare(qaqcwh.qaqc_id, userId) == 0
                              select po).ToList();
                if (isQAQC.Any())
                    return true;
                return false;
            }
        }
        public static bool isQAQCbyStockReturn(string returnId,string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var isQAQC = (from rt in db.tb_stock_issue_return
                              join st in db.tb_stock_transfer_voucher on rt.stock_issue_ref equals st.stock_transfer_id
                              join wh in db.tb_warehouse on st.from_warehouse_id equals wh.warehouse_id
                              join qaqcwh in db.tb_warehouse_qaqc on wh.warehouse_id equals qaqcwh.warehouse_id
                              where string.Compare(rt.stock_issue_return_id, returnId) == 0 && string.Compare(qaqcwh.qaqc_id, userId) == 0
                              select rt).ToList();
                if (isQAQC.Any())
                    return true;
                return false;
            }
        }
        #endregion
        public static bool isProjectManagerinQuote(string id,string userid)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var pm = (from quote in db.tb_purchase_order
                          join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id
                          join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id
                          join propm in db.tb_project_pm on mr.ir_project_id equals propm.project_id
                          where string.Compare(quote.purchase_order_id, id) == 0 && string.Compare(propm.project_manager_id, userid) == 0
                          select quote).ToList();
                if (pm.Any()) return true;
                return false;
            }
        }
        public static List<UserRolesViewModel> GetUserListItemsByRole(string strrole)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<UserRolesViewModel> users = new List<UserRolesViewModel>();
                ApplicationDbContext context = new ApplicationDbContext();
                var userRoles = (from user in context.Users

                                 select new
                                 {
                                     UserId = user.Id,
                                     Username = user.UserName,

                                     Email = user.Email,
                                     RoleName = (from userRole in user.Roles
                                                 join role in context.Roles on userRole.RoleId equals role.Id
                                                 where String.Compare(role.Name,strrole)==0

                                                 select role.Name).ToList()
                                 }).ToList().Select(p => new UserRolesViewModel()
                                 {
                                     UserId = p.UserId,

                                     Username = p.Username,
                                     Email = p.Email,
                                     Role = string.Join(",", p.RoleName)
                                 }).Where(p =>string.Compare(p.Role,strrole)==0).OrderBy(p => p.Username);
                foreach (var item in userRoles)
                    users.Add(item);
                return users;
            }
        }
        public static ProductFilteringCondition GetProductFilteringCondition(string group,string categoryId,string typeId,string classId,string brandId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                ProductFilteringCondition model = new ProductFilteringCondition();
                if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(categoryId) && string.IsNullOrEmpty(typeId) && string.IsNullOrEmpty(classId) && string.IsNullOrEmpty(brandId))
                {
                    model.groups = db.tb_class.OrderBy(o => o.class_name).Where(s => s.active == true).Select(s => new ClassViewModel() { class_id = s.class_id, class_name = s.class_name }).ToList();
                    model.categories = db.tb_product_category.OrderBy(o => o.p_category_name).Where(s => s.status == true).Select(s => new CategoryViewModel(){ class_id = s.class_id, p_category_name = s.p_category_name, p_category_id = s.p_category_id }).ToList();
                    model.types = db.tb_product_type.OrderBy(o => o.product_type_name).Where(s => s.active == true).Select(s => new ProductTypeViewModel() { product_category_id = s.product_category_id, class_type_id = s.product_type_id, product_type_name = s.product_type_name }).ToList();
                    model.classes = db.tb_product_class.OrderBy(o => o.product_class_name).Where(s => s.active == true).Select(s => new ProductClassViewModel() { product_class_id = s.product_class_id, product_class_name = s.product_class_name, class_type_id = s.class_type_id }).ToList();
                }
                return model;
            }
        }
        public static ProjectViewModel GetProjectItembyQuoteId(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from pro in db.tb_project
                        join mr in db.tb_item_request on pro.project_id equals mr.ir_project_id
                        join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                        join quote in db.tb_purchase_order on pr.purchase_requisition_id equals quote.item_request_id
                        where string.Compare(quote.purchase_order_id, id) == 0
                        select new ProjectViewModel() { project_id = pro.project_id, project_full_name = pro.project_full_name }).FirstOrDefault();
            }
        }
        public static string GetMaterialRequestIdbyPR(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from mr in db.tb_item_request
                        join pr in db.tb_purchase_requisition on mr.ir_id equals pr.material_request_id
                        where string.Compare(pr.purchase_requisition_id, id) == 0
                        select mr.ir_id).FirstOrDefault().ToString();
            }
        }
        public static List<WareHouseViewModel> GetWarehouseListItemsbySiteStockKeeper(bool isAdmin,bool isSSK,string userID=null)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (isAdmin)
                {
                    return (from wh in db.tb_warehouse
                     join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                     where wh.warehouse_status == true && pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0
                     select new WareHouseViewModel()
                     {
                         warehouse_id = wh.warehouse_id,
                         warehouse_name = wh.warehouse_name,
                     }).ToList();
                }else if(isSSK)
                {
                    return (from wh in db.tb_warehouse
                     join sitestock in db.tb_stock_keeper_warehouse on wh.warehouse_id equals sitestock.warehouse_id
                     join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                     where wh.warehouse_status == true && pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Completed) != 0 && string.Compare(sitestock.stock_keeper, userID) == 0
                     select new WareHouseViewModel()
                     {
                         warehouse_id = wh.warehouse_id,
                         warehouse_name = wh.warehouse_name,
                     }).ToList();
                }
                return new List<WareHouseViewModel>();
            }
        }
        #endregion
        #region Color
        public static List<ColorViewModel> GetColorListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_color.OrderBy(o => o.color_name).Where(s => s.active == true).Select(s => new ColorViewModel()
                {
                    color_id = s.color_id,
                    color_name = s.color_name
                }).ToList();
            }
        }
        public static ColorViewModel GetColorItemById(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_color.Where(s => string.Compare(s.color_id, id) == 0).Select(s => new ColorViewModel()
                {
                    color_id=s.color_id,
                    color_name=s.color_name
                }).FirstOrDefault();
            }
        }
        #endregion
        #region Project 
        public static tb_warehouse GetWarehousebyProject(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //return (from wh in db.tb_warehouse
                //        join site in db.tb_site on wh.warehouse_site_id equals site.site_id
                //        join pro in db.tb_project on site.site_id equals pro.site_id
                //        where string.Compare(pro.project_id, id) == 0
                //        select wh).FirstOrDefault();
                return (from wh in db.tb_warehouse where string.Compare(wh.warehouse_project_id, id) == 0 select wh).FirstOrDefault();
            }
        }
        public static List<ProjectViewModel> GetReturnToWorkshopProjectListitems(bool isAdmin,string userid=null)
        {
            using (kim_mexEntities db = new kim_mexEntities())
            {
                List<ProjectViewModel> models = new List<ProjectViewModel>();
                if (isAdmin)
                {
                    models = db.tb_project.OrderBy(s => s.project_full_name).Where(s => s.project_status == true && string.Compare(s.p_status, ProjectStatus.Active) == 0).Select(s => new ProjectViewModel()
                    {
                        project_id=s.project_id,
                        project_full_name=s.project_full_name,
                    }).ToList();
                }else
                {
                    models = (from pro in db.tb_project
                              join wh in db.tb_warehouse on pro.project_id equals wh.warehouse_project_id
                              join skeep in db.tb_stock_keeper_warehouse on wh.warehouse_id equals skeep.warehouse_id
                              where pro.project_status == true && string.Compare(pro.p_status, ProjectStatus.Active) == 0 && string.Compare(skeep.stock_keeper, userid) == 0
                              orderby pro.project_full_name
                              select new ProjectViewModel()
                              {
                                  project_id = pro.project_id,
                                  project_full_name = pro.project_full_name,
                              }).ToList();
                }
                return models;
            }
        }
        public static ProjectViewModel GetProjectDetailbyId(string id)
        {
            ProjectViewModel project = new ProjectViewModel();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                project = (from tbl in db.tb_project
                                     join st in db.tb_site on tbl.site_id equals st.site_id into s
                                     from site in s.DefaultIfEmpty()
                                     join cus in db.tb_customer on tbl.cutomer_id equals cus.customer_id into c
                                     from customer in c.DefaultIfEmpty()
                           join wh in db.tb_warehouse on tbl.project_id equals wh.warehouse_project_id into w
                           from wh in w.DefaultIfEmpty()
                           where tbl.project_id == id
                                     select new ProjectViewModel
                                     {
                                         project_id = tbl.project_id,
                                         project_no = tbl.project_no,
                                         created_date = tbl.created_date,
                                         project_short_name = tbl.project_short_name,
                                         project_full_name = tbl.project_full_name,
                                         project_start_date = tbl.project_start_date,
                                         project_actual_start_date = tbl.project_actual_start_date,
                                         project_end_date = tbl.project_end_date,
                                         project_actual_end_date = tbl.project_actual_end_date,
                                         project_address = tbl.project_address,
                                         cutomer_id = tbl.cutomer_id,
                                         customer_name = customer.customer_name,
                                         cutomer_signatory = tbl.cutomer_signatory,
                                         cutomer_project_manager = tbl.cutomer_project_manager,
                                         site_id = tbl.site_id,
                                         site_name = site.site_name,
                                         site_code = site.site_code,
                                         project_manager = tbl.project_manager,
                                         project_telephone = tbl.project_telephone,
                                         customer_telephone = tbl.customer_telephone,
                                         p_status = tbl.p_status,
                                         warehouse_project_id = wh.warehouse_id,
                                         warehouse_project_name = wh.warehouse_name,
                                     }).FirstOrDefault();

                List<SiteManagerViewModel> site_managers = new List<SiteManagerViewModel>();
                var project_site_managers = (from tbl in db.tb_site_manager_project where tbl.project_id == id select tbl);
                if (project_site_managers.Any())
                {
                    foreach (var site_manager in project_site_managers)
                    {
                        SiteManagerViewModel siteManager = new SiteManagerViewModel();
                        var sm = db.tb_user_detail.Where(x => x.user_id == site_manager.site_manager).FirstOrDefault();
                        //siteManager.site_manager = site_manager.site_manager;
                        if (sm != null)
                        {
                            siteManager.site_manager = site_manager.site_manager;
                            siteManager.site_manager_name = sm.user_first_name + " " + sm.user_last_name;
                            site_managers.Add(siteManager);
                        }
                    }
                }
                project.project_site_managers = site_managers;
                var spacemen_attachment = (from tbl in db.tb_spacemen_attachment where tbl.project_id == id select tbl).FirstOrDefault();
                if (spacemen_attachment != null)
                {
                    project.spacemen_att_id = spacemen_attachment.spacemen_att_id;
                    project.spacemen_filename = spacemen_attachment.spacemen_filename;
                    project.spacemen_extension = spacemen_attachment.spacemen_extension;
                    project.file_path = spacemen_attachment.file_path;
                }
                project.boq_id = db.tb_build_of_quantity.Where(m => m.status == true && m.boq_status == "Completed" && m.project_id == id).Select(x => x.boq_id).FirstOrDefault();

                project.projectProjectManagers = db.tb_project_pm.Where(w => string.Compare(w.project_id, project.project_id) == 0).Select(s => new ProjectPMViewModel()
                {
                    project_pm_id = s.project_pm_id,
                    project_id = s.project_id,
                    project_manager_id = s.project_manager_id,

                }).ToList();
                //Site supervisor
                var sitesups = db.tbSiteSiteSupervisors.Where(s => string.Compare(s.site_id, project.project_id) == 0).ToList();
                foreach (var sitesup in sitesups)
                {
                    SiteSiteSupervisorViewModel ss = new SiteSiteSupervisorViewModel();
                    ss.site_supervisor_id = sitesup.site_supervisor_id;
                    var ud = db.tb_user_detail.Where(s => string.Compare(s.user_id, sitesup.site_supervisor_id) == 0).FirstOrDefault();
                    if (ud == null)
                    {
                        ss.site_supervisor_name = string.Empty;
                    }
                    else
                    {
                        ss.site_supervisor_name = ud.user_first_name + " " + ud.user_last_name;
                        project.siteSupervisors.Add(ss);
                    }


                }
                //site Admin
                var siteAdmins = db.tb_site_site_admin.Where(s => string.Compare(s.site_id, project.project_id) == 0).ToList();
                foreach (var siteadmin in siteAdmins)
                {
                    var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, siteadmin.site_admin_id) == 0).FirstOrDefault();
                    if (userdetail != null)
                        project.siteAdmins.Add(new SiteSiteAdminViewModel()
                        {
                            site_admin_id=siteadmin.site_admin_id,
                            site_id=siteadmin.site_id,
                            site_site_admin_id=siteadmin.site_site_admin_id,
                            site_admin_fullname = userdetail.user_first_name + " " + userdetail.user_last_name }
                        );
                }
                if (project.siteAdmins.Count() > 0)
                {
                    project.siteAdmin = project.siteAdmins.FirstOrDefault();
                }
                //Site Stock Keeper
                var siteStockKeepers = (from skeeper in db.tb_stock_keeper_warehouse
                                        join wh in db.tb_warehouse on skeeper.warehouse_id equals wh.warehouse_id
                                        //where string.Compare(wh.warehouse_site_id, project.site_id) == 0
                                        where string.Compare(wh.warehouse_project_id, project.project_id) == 0
                                        select new { skeeper }).ToList();
                foreach (var stockstockkeeper in siteStockKeepers)
                {
                    var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, stockstockkeeper.skeeper.stock_keeper) == 0).FirstOrDefault();
                    if (userdetail != null)
                        project.warehouseStockKeepers.Add(new tb_stock_keeper_warehouse() { stock_keeper = userdetail.user_first_name + " " + userdetail.user_last_name });
                }
                //qaqc 
                var qaqcs = (from qaqc in db.tb_warehouse_qaqc
                             join wh in db.tb_warehouse on qaqc.warehouse_id equals wh.warehouse_id
                             //where string.Compare(wh.warehouse_site_id, project.site_id) == 0
                             where string.Compare(wh.warehouse_project_id, project.project_id) == 0
                             select new { qaqc }).ToList();
                foreach (var qaqc in qaqcs)
                {
                    var userdetail = db.tb_user_detail.Where(s => string.Compare(s.user_id, qaqc.qaqc.qaqc_id) == 0).FirstOrDefault();
                    if (userdetail != null)
                        project.warehouseQAQCs.Add(new WarehouseQAQCViewModel() { qaqc_id = userdetail.user_first_name + " " + userdetail.user_last_name });
                }

            }
            catch(Exception ex)
            {
                
            }
            return project;
        }
        #endregion
        #region Stock Transfer
        public static decimal GetStokTransferRemainBalancebyItem(string stockTransferId,string itemId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                decimal totalRemainBalance = 0;
                var items = db.tb_stock_transfer_detail.Where(s => string.Compare(s.st_ref_id, stockTransferId) == 0 && string.Compare(s.st_item_id, itemId) == 0).ToList();
                if (items.Any())
                {
                    foreach (var item in items)
                        totalRemainBalance = totalRemainBalance +Convert.ToDecimal(item.remain_quantity);
                }
                return totalRemainBalance;
            }
        }
        #endregion
        #region Site
        public static List<SiteViewModel> GetSiteListItems()
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //return db.tb_site.OrderBy(s => s.site_name).Where(s => s.status == true).Select(s => new SiteViewModel() { site_id = s.site_id, site_name = s.site_name }).ToList();
                List<SiteViewModel> models = new List<SiteViewModel>();
                var sites = db.tb_site.Where(s => s.status == true).ToList();
                foreach(var site in sites)
                {
                    var projects = db.tb_project.Where(s => s.project_status == true && string.Compare(s.site_id, site.site_id) == 0 && string.Compare(s.p_status, ProjectStatus.Completed) != 0).ToList();
                    if (projects.Any())
                        models.Add(new SiteViewModel() { site_id = site.site_id, site_name = site.site_name });
                }
                models = models.OrderBy(s => s.site_name).ToList();
                return models;
            }
        }
        #endregion
        #region Warehouse
        public static List<WareHouseViewModel> GetWarehouseListItems(bool isBySite,string userid)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //return db.tb_warehouse.OrderBy(s => s.warehouse_name).Where(s => s.warehouse_status == true).Select(s => new WareHouseViewModel() { warehouse_id = s.warehouse_id, warehouse_name = s.warehouse_name }).ToList();
                if (isBySite)
                {

                }
                else
                {
                    return (from wh in db.tb_warehouse
                            join pro in db.tb_project on wh.warehouse_project_id equals pro.project_id
                            where wh.warehouse_status == true && string.Compare(pro.p_status, "Completed") != 0
                            select new WareHouseViewModel()
                            {
                                warehouse_id = wh.warehouse_id,
                                warehouse_name = wh.warehouse_name,
                            }).ToList();
                }
                return new List<WareHouseViewModel>();
            }
        }
        #endregion
        #region Good Received Note
        public static List<Models.GoodReceivedNoteStatusViewModel> GetGoodReceivedStatusListItemsbyRefId(string id)
        {
            using (Entities.kim_mexEntities db = new Entities.kim_mexEntities())
            {
                return db.tb_received_status.OrderByDescending(s => s.created_date).Where(s => string.Compare(s.received_ref_id, id) == 0)
                    .Select(s => new GoodReceivedNoteStatusViewModel()
                    {
                        received_status_id=s.received_status_id,
                        received_id=s.received_id,
                        received_ref_id=s.received_ref_id,
                        status=s.status,
                        created_date=s.created_date
                    }).ToList();
            }
        }
        public static string GetLatestGRNStatusbyRefId(string id)
        {
            using(Entities.kim_mexEntities db=new kim_mexEntities())
            {
                return db.tb_received_status.OrderByDescending(s => s.created_date).Where(s => string.Compare(s.received_ref_id, id) == 0).Select(s => s.status).FirstOrDefault() == null ? string.Empty :
                    db.tb_received_status.OrderByDescending(s => s.created_date).Where(s => string.Compare(s.received_ref_id, id) == 0).Select(s => s.status).FirstOrDefault().ToString();
            }
        }
        #endregion
        #region Purchase Order
        public static List<PurchaseRequestViewModel> GetPurchaseOrderMyRequestList(bool isAdmin,string userId,string dateRange,string status)
        {
            List<PurchaseRequestViewModel> models = new List<PurchaseRequestViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);

                List<PurchaseOrderFilterResult> results = new List<PurchaseOrderFilterResult>();

                if (string.Compare(status, "0") == 0)
                {
                    results = (from po in db.tb_purchase_request
                               join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                               from quote in pquote.DefaultIfEmpty()
                               join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                               from pr in ppr.DefaultIfEmpty()
                               join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                               from mr in pmr.DefaultIfEmpty()
                               join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                               orderby po.created_date descending
                               where po.status == true && (string.Compare(po.created_by, userId) == 0 || string.Compare(po.updated_by, userId) == 0) && po.created_date>=startDate && po.updated_date<=endDate
                               select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                }else
                {
                    results = (from po in db.tb_purchase_request
                               join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                               from quote in pquote.DefaultIfEmpty()
                               join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                               from pr in ppr.DefaultIfEmpty()
                               join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                               from mr in pmr.DefaultIfEmpty()
                               join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                               orderby po.created_date descending
                               where po.status == true && (string.Compare(po.created_by, userId) == 0 || string.Compare(po.updated_by, userId) == 0) && po.created_date >= startDate && po.updated_date <= endDate && string.Compare(po.purchase_request_status,status)==0
                               select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                }
                

                foreach(var obj in results)
                {
                    PurchaseRequestViewModel model = new PurchaseRequestViewModel();
                    model.pruchase_request_id = obj.po.pruchase_request_id;
                    model.purchase_request_number = obj.po.purchase_request_number;
                    model.str_created_date =Convert.ToDateTime(obj.po.created_date).ToString("dd/MM/yyyy");
                    model.created_by = GetUserFullnamebyUserId(obj.po.created_by);
                    model.quote_number = obj.quote.purchase_oder_number;
                    model.mr_id = obj.mr.ir_id;
                    model.mr_number = obj.mr.ir_no;
                    model.project_short_name = obj.proj.project_full_name;
                    model.purchase_request_status = obj.po.purchase_request_status;
                    model.purchase_order_id = obj.po.purchase_order_id;
                    if (!string.IsNullOrEmpty(model.purchase_order_id))
                    {
                        model.poDetails = PurchaseOrderReportViewModel.GetPOReportbyPurchaseOrderId(model.pruchase_request_id);

                    }
                    models.Add(model);
                }

            }
            catch(Exception ex)
            {

            }
            return models;
        }
        public static List<PurchaseRequestViewModel> GetPurchaseOrderMyApprovalList(bool isAdmin,bool isFinance,bool isAccount,bool isOperationDirector, string userId,string dateRange,string status)
        {
            List<PurchaseRequestViewModel> models = new List<PurchaseRequestViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                string[] splitDateRanges = dateRange.Split('-');
                DateTime startDate = Convert.ToDateTime(splitDateRanges[0]);
                DateTime endDate = Convert.ToDateTime(splitDateRanges[1]).AddDays(1).AddMilliseconds(-1);
                List<PurchaseOrderFilterResult> results = new List<PurchaseOrderFilterResult>();

                if (string.Compare(status, "0") == 0)
                {
                    if (isAdmin || ((isFinance || isAccount) && isOperationDirector))
                    {
                        var objs = (from po in db.tb_purchase_request
                                    join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                    from quote in pquote.DefaultIfEmpty()
                                    join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                    from pr in ppr.DefaultIfEmpty()
                                    join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                    from mr in pmr.DefaultIfEmpty()
                                    join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                    orderby po.created_date descending
                                    where po.status == true && ((string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.purchase_request_status, Status.Approved) == 0) || (string.Compare(po.approved_by, userId) == 0 || string.Compare(po.checked_by, userId) == 0)) && po.created_date >= startDate && po.created_date <= endDate
                                    select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();

                        results.AddRange(objs);
                    }
                    else
                    {
                        if (isFinance || isAccount)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.approved_by, userId) == 0) && po.created_date >= startDate && po.created_date <= endDate
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                        if (isOperationDirector)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.checked_by, userId) == 0) && po.created_date>=startDate && po.created_date<=endDate
                                        select new PurchaseOrderFilterResult() { po=po,quote= quote,mr= mr,proj= proj }).ToList();
                            results.AddRange(objs);
                        }

                    }
                    
                }
                else
                {
                    if (isAdmin || ((isFinance || isAccount) && isOperationDirector))
                    {
                        var objs = (from po in db.tb_purchase_request
                                    join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                    from quote in pquote.DefaultIfEmpty()
                                    join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                    from pr in ppr.DefaultIfEmpty()
                                    join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                    from mr in pmr.DefaultIfEmpty()
                                    join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                    orderby po.created_date descending
                                    where po.status == true && ((string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.purchase_request_status, Status.Approved) == 0) || (string.Compare(po.approved_by, userId) == 0 || string.Compare(po.checked_by, userId) == 0)) && po.created_date >= startDate && po.created_date <= endDate 
                                    && string.Compare(po.purchase_request_status,status)==0
                                    select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();

                        results.AddRange(objs);
                    }
                    else
                    {
                        if (isFinance || isAccount)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.approved_by, userId) == 0) && po.created_date >= startDate && po.created_date <= endDate
                                        && string.Compare(po.purchase_request_status, status) == 0
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                        if (isOperationDirector)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.checked_by, userId) == 0) && po.created_date >= startDate && po.created_date <= endDate
                                        && string.Compare(po.purchase_request_status, status) == 0
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                    }
                }

                foreach (var obj in results.DistinctBy(s=>s).ToList())
                {
                    PurchaseRequestViewModel model = new PurchaseRequestViewModel();
                    model.pruchase_request_id = obj.po.pruchase_request_id;
                    model.purchase_request_number = obj.po.purchase_request_number;
                    model.str_created_date = Convert.ToDateTime(obj.po.created_date).ToString("dd/MM/yyyy");
                    model.created_by = GetUserFullnamebyUserId(obj.po.created_by);
                    model.quote_number = obj.quote.purchase_oder_number;
                    model.mr_id = obj.mr.ir_id;
                    model.mr_number = obj.mr.ir_no;
                    model.project_short_name = obj.proj.project_full_name;
                    model.purchase_request_status = obj.po.purchase_request_status;
                    model.purchase_order_id = obj.po.purchase_order_id;

                    if (!string.IsNullOrEmpty(model.purchase_order_id))
                    {
                        model.poDetails = PurchaseOrderReportViewModel.GetPOReportbyPurchaseOrderId(model.pruchase_request_id);

                    }

                    models.Add(model);
                }

                models = models.OrderByDescending(s => s.purchase_request_number).ToList();
            }
            catch (Exception ex)
            {

            }
            return models;
        }
        public static List<PurchaseRequestViewModel> GetPurchaseOrderMyApprovalList(bool isAdmin, bool isFinance, bool isAccount, bool isOperationDirector, string userId, string status)
        {
            List<PurchaseRequestViewModel> models = new List<PurchaseRequestViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                
                List<PurchaseOrderFilterResult> results = new List<PurchaseOrderFilterResult>();

                if (string.Compare(status, "0") == 0)
                {
                    if (isAdmin || ((isFinance || isAccount) && isOperationDirector))
                    {
                        var objs = (from po in db.tb_purchase_request
                                    join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                    from quote in pquote.DefaultIfEmpty()
                                    join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                    from pr in ppr.DefaultIfEmpty()
                                    join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                    from mr in pmr.DefaultIfEmpty()
                                    join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                    orderby po.created_date descending
                                    where po.status == true && ((string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.purchase_request_status, Status.Approved) == 0) || (string.Compare(po.approved_by, userId) == 0 || string.Compare(po.checked_by, userId) == 0)) 
                                    select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();

                        results.AddRange(objs);
                    }
                    else
                    {
                        if (isFinance || isAccount)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.approved_by, userId) == 0) 
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                        if (isOperationDirector)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.checked_by, userId) == 0) 
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                    }

                }
                else
                {
                    if (isAdmin || ((isFinance || isAccount) && isOperationDirector))
                    {
                        var objs = (from po in db.tb_purchase_request
                                    join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                    from quote in pquote.DefaultIfEmpty()
                                    join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                    from pr in ppr.DefaultIfEmpty()
                                    join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                    from mr in pmr.DefaultIfEmpty()
                                    join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                    orderby po.created_date descending
                                    where po.status == true && ((string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.purchase_request_status, Status.Approved) == 0) || (string.Compare(po.approved_by, userId) == 0 || string.Compare(po.checked_by, userId) == 0)) 
                                    && string.Compare(po.purchase_request_status, status) == 0
                                    select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();

                        results.AddRange(objs);
                    }
                    else
                    {
                        if (isFinance || isAccount)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Pending) == 0 || string.Compare(po.approved_by, userId) == 0) 
                                        && string.Compare(po.purchase_request_status, status) == 0
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                        if (isOperationDirector)
                        {
                            var objs = (from po in db.tb_purchase_request
                                        join quote in db.tb_purchase_order on po.purchase_order_id equals quote.purchase_order_id into pquote
                                        from quote in pquote.DefaultIfEmpty()
                                        join pr in db.tb_purchase_requisition on quote.item_request_id equals pr.purchase_requisition_id into ppr
                                        from pr in ppr.DefaultIfEmpty()
                                        join mr in db.tb_item_request on pr.material_request_id equals mr.ir_id into pmr
                                        from mr in pmr.DefaultIfEmpty()
                                        join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                        orderby po.created_date descending
                                        where po.status == true && (string.Compare(po.purchase_request_status, Status.Approved) == 0 || string.Compare(po.checked_by, userId) == 0) 
                                        && string.Compare(po.purchase_request_status, status) == 0
                                        select new PurchaseOrderFilterResult() { po = po, quote = quote, mr = mr, proj = proj }).ToList();
                            results.AddRange(objs);
                        }

                    }
                }

                foreach (var obj in results.DistinctBy(s => s).ToList())
                {
                    PurchaseRequestViewModel model = new PurchaseRequestViewModel();
                    model.pruchase_request_id = obj.po.pruchase_request_id;
                    model.purchase_request_number = obj.po.purchase_request_number;
                    model.str_created_date = Convert.ToDateTime(obj.po.created_date).ToString("dd/MM/yyyy");
                    model.created_by = GetUserFullnamebyUserId(obj.po.created_by);
                    model.quote_number = obj.quote.purchase_oder_number;
                    model.mr_id = obj.mr.ir_id;
                    model.mr_number = obj.mr.ir_no;
                    model.project_short_name = obj.proj.project_full_name;
                    model.purchase_request_status = obj.po.purchase_request_status;
                    model.purchase_order_id = obj.po.purchase_order_id;

                    if (!string.IsNullOrEmpty(model.purchase_order_id))
                    {
                        model.poDetails = PurchaseOrderReportViewModel.GetPOReportbyPurchaseOrderId(model.pruchase_request_id);
                    }

                    models.Add(model);
                }

                models = models.OrderByDescending(s => s.purchase_request_number).ToList();
            }
            catch (Exception ex)
            {

            }
            return models;
        }
        #endregion
    }
}

