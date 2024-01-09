using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class ProductViewModel
    {
        //internal DateTime? updated_date;
        //internal tb_multiple_uom uom;
        //internal string product_unit_name;

        [Key]
        public string product_id { get; set; }
        [Display(Name = "Product Description :")]

        public string product_name { get; set; }

        //[Required(ErrorMessage ="Product ID is required.")]
        [Display(Name = "Product ID :")]
        public string product_code { get; set; }

        // [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit : ")]
        public string product_unit { get; set; }

        [Display(Name = "Product Class : ")]
        public string product_class_name { get; set; }
        public string product_category_id { get; set; }
        [Display(Name = "Product Type")]
        public string product_type_id { get; set; }
        public string product_type_name { get; set; }

        [Display(Name = "Brand :")]
        //[Required(ErrorMessage = "Brand is required. ")]
        public string brand_id { get; set; }
        public string brand_name { get; set; }
        public string class_id { get; set; }
        public string class_name { get; set; }

        //[Required(ErrorMessage = "Product Category is required.")]
        [Display(Name = "Product Category : ")]
        public string p_category_id { get; set; }

        [Display(Name = "Product Category :")]
        //[Required(ErrorMessage = "Product Category is required. ")]
        public string p_category_name { get; set; }

        [Display(Name = "Unit Price:")]
        public Nullable<decimal> unit_price { get; set; }
        [Display(Name = "Date:")]
        //public DateTime? updated_date { get; set; }
        public Nullable<System.DateTime> created_date { get; set; }
        [Display(Name = "Remark:")]
        public string Remark { get; set; }
        public string uom1_id { get; set; }
        public Nullable<decimal> uom1_qty { get; set; }
        public string uom2_id { get; set; }
        public Nullable<decimal> uom2_qty { get; set; }
        public string uom3_id { get; set; }
        public Nullable<decimal> uom3_qty { get; set; }
        public string uom4_id { get; set; }
        public Nullable<decimal> uom4_qty { get; set; }
        public string uom5_id { get; set; }
        public Nullable<decimal> uom5_qty { get; set; }
        public Nullable<decimal> uom1_price { get; set; }
        public Nullable<decimal> uom2_price { get; set; }
        public Nullable<decimal> uom3_price { get; set; }
        public Nullable<decimal> uom4_price { get; set; }
        public Nullable<decimal> uom5_price { get; set; }
        public decimal stock_balance { get; set; }
        public List<QuoteViewModel> quoteSuppliers { get; set; }
        public string updated_by { get; set; }
        [Display(Name = "Product Size : ")]
        public string product_size { get; set; }

        [Display(Name = "Model/ Factory Code : ")]
        public string model_factory_code { get; set; }

        [Display(Name = "Supplier : ")]
        //[Required(ErrorMessage = "Supplier is required.")]
        public string supplier_id { get; set; }
        [Display(Name = "Labor Hour : ")]
        public string labor_hour { get; set; }
        [Display(Name = "Insulation Color : ")]
        public string insulation_color { get; set; }
        [Display(Name = "Sheath Color : ")]
        public string sheath_color { get; set; }

        //[Required(ErrorMessage = "Product Class is required.")]
        [Display(Name = "Product Class : ")]
        public string product_class_id { get; set; }

        [Display(Name = "Created By")]
        public string created_by { get; set; }
        public string unit_name { get; set; }
        public string number_of_core { get; set; }
        public string outer_sheath { get; set; }
        public string color { get; set; }
        public string color_id { get; set; }
        public string color_name { get; set; }
        public string product_size_id { get; set; }
     
        public string product_size_name { get; set; }
        public string product_color_name { get; set; }
        public string sub_group_id { get; set; }
        public string sub_group_name { get; set; }
        public string so_number { get; set; }
        public string cash_flow { get; set; }
        public Nullable<decimal> labour_hour { get; set; }


        public static ProductViewModel GetProductItem(string id)
        {
            ProductViewModel product = new ProductViewModel();
            tb_multiple_uom uom = new tb_multiple_uom();
            using (kim_mexEntities db = new kim_mexEntities())
            {
                product = (from pro in db.tb_product
                           join un in db.tb_unit on pro.product_unit equals un.Id into jun
                           from un in jun.DefaultIfEmpty()
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
                           join ps in db.tb_product_size on pro.product_size equals ps.product_size_id into jps
                           from ps in jps.DefaultIfEmpty()
                           join cl in db.tb_color on pro.color equals cl.color_id into jcl
                           from cl in jcl.DefaultIfEmpty()
                           join sg in db.tb_sub_group on pro.sub_group_id equals sg.sub_group_id into psg
                           from sg in psg.DefaultIfEmpty()

                           where pro.product_id == id

                           select new ProductViewModel
                           {
                               product_id = pro.product_id,
                               class_id=cls.class_id,
                               class_name = cls.class_name,
                               p_category_id=cat.p_category_id,
                               p_category_name = cat.p_category_name,
                               product_type_id=pt.product_type_id,
                               product_type_name = pt.product_type_name,
                               product_class_id = pc.product_class_id,
                               product_class_name = pc.product_class_name,
                               product_size_name = ps.product_size_name,
                               product_size_id = ps.product_size_id,
                               color = cl.color_id,
                               color_id = pro.color,
                               color_name = cl.color_name,
                               model_factory_code = pro.model_factory_code,
                               product_name = pro.product_name,
                               product_code = pro.product_code,
                               product_unit=pro.product_unit,
                               unit_name = un.Name,
                               
                               brand_id = pro.brand_id,
                               brand_name = br.brand_name,
                               unit_price = pro.unit_price,
                               created_date = pro.created_date,
                               Remark = pro.Remark,
                               created_by = pro.created_by,
                               updated_by = pro.updated_by,
                               sub_group_id = pro.sub_group_id,
                               sub_group_name = sg.sub_group_name,
                               labour_hour = pro.labour_hour,
                               so_number = pro.so_number,
                               cash_flow = pro.cash_flow,
                           }).FirstOrDefault();
                uom = db.tb_multiple_uom.Where(x => x.product_id == product.product_id).FirstOrDefault();
                if (uom != null)
                {
                    product.uom1_id = uom.uom1_id;
                    product.uom1_qty = uom.uom1_qty;
                    product.uom2_id = uom.uom2_id;
                    product.uom2_qty = uom.uom2_qty;
                    product.uom3_id = uom.uom3_id;
                    product.uom3_qty = uom.uom3_qty;
                    product.uom4_id = uom.uom4_id;
                    product.uom4_qty = uom.uom4_qty;
                    product.uom5_id = uom.uom5_id;
                    product.uom5_qty = uom.uom5_qty;
                    product.uom1_price = uom.uom1_price;
                    product.uom2_price = uom.uom2_price;
                    product.uom3_price = uom.uom3_price;
                    product.uom4_price = uom.uom4_price;
                    product.uom5_price = uom.uom5_price;
                }
                #region get item quote
                List<QuoteViewModel> itemQuotes = new List<QuoteViewModel>();
                var quotes = (from qd in db.tb_quote_detail
                              join q in db.tb_quote on qd.quote_id equals q.quote_id
                              join sup in db.tb_supplier on q.supplier_id equals sup.supplier_id
                              where string.Compare(qd.item_id, id) == 0 && q.status == true
                              select new QuoteViewModel()
                              {
                                  quote_id = q.quote_id,
                                  quote_no = q.quote_no,
                                  supplier_id = q.supplier_id,
                                  supplier_name = sup.supplier_name,
                                  price = qd.price,
                                  created_date = q.created_date
                              }).ToList();
                var dupSuppliers = quotes.GroupBy(g => g.supplier_id).Where(c => c.Count() > 1).Select(s => s.Key).ToList();
                if (dupSuppliers.Any())
                {
                    foreach (var dupSupplier in dupSuppliers)
                    {
                        var quote = quotes.OrderByDescending(q => q.created_date).Where(s => string.Compare(s.supplier_id, dupSupplier) == 0).FirstOrDefault();
                        itemQuotes.Add(new QuoteViewModel()
                        {
                            quote_id = quote.quote_id,
                            quote_no = quote.quote_no,
                            supplier_id = quote.supplier_id,
                            supplier_name = quote.supplier_name,
                            price = quote.price,
                        });
                    }
                    foreach (var item in quotes)
                    {
                        bool isDuplication = dupSuppliers.Where(s => string.Compare(s, item.supplier_id) == 0).Count() > 0 ? true : false;
                        if (!isDuplication)
                            itemQuotes.Add(new QuoteViewModel()
                            {
                                quote_id = item.quote_id,
                                quote_no = item.quote_no,
                                supplier_id = item.supplier_id,
                                supplier_name = item.supplier_name,
                                price = item.price,
                            });
                    }
                }
                else
                {
                    foreach (var item in quotes)
                    {
                        itemQuotes.Add(new QuoteViewModel()
                        {
                            quote_id = item.quote_id,
                            quote_no = item.quote_no,
                            supplier_id = item.supplier_id,
                            supplier_name = item.supplier_name,
                            price = item.price,
                        });
                    }
                }
                itemQuotes = itemQuotes.OrderBy(q => q.quote_no).ToList();
                product.quoteSuppliers = itemQuotes;
                #endregion
            }
            return product;
        }

    }

    //public class CategoryViewModel
    //{
    //    [Key]
    //    public string p_category_id { get; set; }
    //    [Required(ErrorMessage = "Type code is required.")]
    //    [Display(Name = "Type Code:")]
    //    public string p_category_code { get; set; }
    //    [Required(ErrorMessage = "Description is required.")]
    //    [Display(Name = "Description:")]
    //    public string p_category_name { get; set; }
    //    [Display(Name = "Category Address")]
    //    public string p_category_address { get; set; }
    //    [Display(Name = "Chart Account:")]
    //    public string chart_account { get; set; }
    //    [Display(Name = "Date:")]
    //    public Nullable<System.DateTime> created_date { get; set; }
    //    public Nullable<bool> status { get; set; }
    //    [Required(ErrorMessage = "Class is required.")]
    //    [Display(Name = "Class : ")]
    //    public string class_id { get; set; }
    //    public string class_name { get; set; }
    //}


    //public class CategoryPViewModel
    //{
    //    internal object categories;

    //    [Key]
    //    public string p_category_id { get; set; }
    //    [Required(ErrorMessage = "Type code is required.")]
    //    [Display(Name = "Type Code:")]
    //    public string p_category_code { get; set; }
    //    [Required(ErrorMessage = "Description is required.")]
    //    [Display(Name = "Description:")]
    //    public string p_category_name { get; set; }
    //    [Display(Name = "Category Address")]
    //    public string p_category_address { get; set; }
    //    [Display(Name = "Chart Account:")]
    //    public string chart_account { get; set; }
    //    [Display(Name = "Date:")]
    //    public Nullable<System.DateTime> created_date { get; set; }
    //    public Nullable<bool> status { get; set; }
    //    [Required(ErrorMessage = "Class is required.")]
    //    [Display(Name = "Class : ")]
    //    public string class_id { get; set; }
    //    public string class_name { get; set; }
    //}
    public class ProductFilteringCondition
    {
        public List<ClassViewModel> groups { get; set; }
        public List<CategoryViewModel> categories { get; set; }
        public List<ProductTypeViewModel> types { get; set; }
        public List<ProductClassViewModel> classes { get; set; }
        public List<BrandViewModel> brands { get; set; }
        public List<ProductSizeViewModel> sizes { get; set; }
        public List<ColorViewModel> colors { get; set; }
    }

    public class ProductCodeResponse
    {
        public string code { get; set; }
        public decimal number { get; set; }
        public ProductCodeResponse(string code,decimal number)
        {
            this.code = code;
            this.number = number;
        }
    }
}