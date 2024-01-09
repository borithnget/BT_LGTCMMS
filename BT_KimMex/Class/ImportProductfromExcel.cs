using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;

namespace BT_KimMex.Class
{
    public class ImportProductfromExcel
    {
        public static ImportProductExcelReasponse GetDataModelFromExcelContact(string path,bool hasHeader = true)
        {
            ImportProductExcelReasponse response = new ImportProductExcelReasponse();
            List<ExcelProductViewModel> listExcelModel = new List<ExcelProductViewModel>();
            string message = string.Empty;
            int errorLine = 0;
            int errorColumn = 0;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var pck=new OfficeOpenXml.ExcelPackage())
            {
                try
                {
                    using(var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[1];
                    var startRow = hasHeader ? 3 : 1;
                    
                    for(int rowNum = startRow; rowNum <= ws.Dimension.End.Row - 1; rowNum++)
                    {
                        errorLine = rowNum;
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        ExcelProductViewModel excelModel = new ExcelProductViewModel();
                        excelModel.classType = pck.Workbook.Worksheets[1].ToString();
                        excelModel.productCode = wsRow[rowNum, 1].Text;
                        excelModel.classs = wsRow[rowNum, 2].Text;
                        excelModel.subGroup = wsRow[rowNum, 3].Text;
                        excelModel.category = wsRow[rowNum, 4].Text;
                        excelModel.productType = wsRow[rowNum, 5].Text;
                        excelModel.productClass = wsRow[rowNum, 6].Text;
                        excelModel.productSize = wsRow[rowNum, 7].Text;
                        excelModel.color = wsRow[rowNum, 8].Text;
                        excelModel.brand = wsRow[rowNum, 9].Text;
                        excelModel.model = wsRow[rowNum, 10].Text;
                        excelModel.unit = wsRow[rowNum, 11].Text;
                        excelModel.productDescription = wsRow[rowNum, 12].Text;
                        excelModel.laborHour = wsRow[rowNum, 13].Text;
                        listExcelModel.Add(excelModel);
                    }
                    listExcelModel = listExcelModel.Where(s => !string.IsNullOrEmpty(s.classs) && !string.IsNullOrEmpty(s.productDescription)).ToList();
                    return ImportProductfromExcel.SaveDataToDatabase(listExcelModel);
                }
                catch(Exception ex)
                {
                    //ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ImportProductfromExcel.cs", "GetDataModelFromExcelContact", ex.StackTrace, ex.Message);
                    message = message +" " + string.Format("Importing Excel file error row {0} column {1} : {2}", errorLine, errorColumn,ex.Message);
                    response.message=message;
                }
            }
            //return new ImportProductExcelReasponse();
            return response;
        }
        public static ImportProductExcelReasponse SaveDataToDatabase(List<ExcelProductViewModel> listExcelModel)
        {
            List<ExcelProductViewModel> models = new List<ExcelProductViewModel>();
            ImportProductExcelReasponse response = new ImportProductExcelReasponse();
            try
            {
                
                kim_mexEntities db = new kim_mexEntities();
                foreach(var item in listExcelModel)
                {
                    if(!string.IsNullOrEmpty(item.classs)&& !string.IsNullOrEmpty(item.productDescription))
                    {
                        var isproduct_exist = (from pro in db.tb_product
                                               join br in db.tb_brand on pro.brand_id equals br.brand_id into pbr from br in pbr.DefaultIfEmpty()
                                               join pt in db.tb_product_type on pro.product_type_id equals pt.product_type_id
                                               join pcat in db.tb_product_category on pro.product_category_id equals pcat.p_category_id
                                               where pro.status == true && string.Compare(pro.product_code, item.productCode) == 0 && string.Compare(pro.model_factory_code, item.model) == 0 && (string.IsNullOrEmpty(item.brand)|| string.Compare(br.brand_name, item.brand) == 0) && string.Compare(pt.product_type_name, item.productType) == 0 && string.Compare(pcat.p_category_name, item.category) == 0
                                               select pro).FirstOrDefault();

                        var isProductNameExist= db.tb_product.Where(s=>s.status==true && string.Compare(s.product_name.ToLower().Replace(" ","").Trim(),item.productDescription.ToLower().Replace(" ","").Trim())==0).FirstOrDefault();


                        if (isproduct_exist == null && isProductNameExist==null)
                        {
                            tb_product product = new tb_product();
                            product.product_id = Guid.NewGuid().ToString();

                            //Group
                            if (!string.IsNullOrEmpty(item.classs))
                                product.group_id = GetClassIdByName(item);

                            if (!string.IsNullOrEmpty(item.subGroup))
                                product.sub_group_id = GetSubGroupIdbyName(item);

                            if (!string.IsNullOrEmpty(item.category))
                                product.product_category_id = GetCategoryIdByName(item);

                            if (!string.IsNullOrEmpty(item.productType))
                                product.product_type_id = GetProductTypeIdByName(item);

                            ProductCodeResponse codeResponse = CommonFunctions.GenerateProductCode(product.group_id, product.sub_group_id);
                            //product.product_code = CommonFunctions.GenerateProductCode();
                            product.product_code = codeResponse.code;
                            product.product_number = codeResponse.number;
                            product.product_name = string.IsNullOrEmpty(item.productDescription) ? CommonFunctions.GenerateProductDescription1(item.category, item.productType, item.productClass, item.productSize, item.color, item.brand, item.model) : item.productDescription;
                            if (!string.IsNullOrEmpty(item.unit))
                                product.product_unit = GetUnitIdbyName(item.unit);
                            //product.unit_price = Convert.ToDecimal(item.unitprice);
                            product.status = true;

                            if (!string.IsNullOrEmpty(item.productSize))
                                product.product_size = GetProductSizeIdByName(item);
                            product.model_factory_code = item.model;

                            if (!string.IsNullOrEmpty(item.productClass))
                                product.product_class_id = GetProductClassIdbyName(item.productClass, item.classType);                           
                            
                            if (!string.IsNullOrEmpty(item.brand))
                                product.brand_id = GetBrandIdbyName(item);

                            product.color = GetColorIdbyNmae(item.color);
                            product.created_by = System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                            product.created_date = DateTime.Now;
                            product.unit_price = 0;
                            product.labour_hour = Convert.ToDecimal(item.laborHour);

                            db.tb_product.Add(product);
                            db.SaveChanges();

                            item.productCode = product.product_code;
                            models.Add(item);
                            response.successProducts.Add(product);

                        }
                        else
                        {
                            if (isProductNameExist != null)
                            {
                                response.failProducts.Add(isProductNameExist);
                            }else if(isproduct_exist!= null)
                            {
                                response.failProducts.Add(isproduct_exist);
                            }
                        }
                    }
                    
                }
                
            }
            catch(Exception ex)
            {
                //ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ImportProductfromExcel.cs", "SaveDateToDatabase", ex.StackTrace, ex.Message);
            }
            return response;
        }
        public static string GetProductClassIdbyName(string productClassName,string classTypeName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_product_class.Where(w => w.active == true && string.Compare(w.product_class_name, productClassName) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_product_class productClass = new tb_product_class();
                    productClass.product_class_id = Guid.NewGuid().ToString();
                    productClass.product_class_name = productClassName;
                    productClass.class_type_id = GetClassTypeIdByName(classTypeName);
                    productClass.active = true;
                    productClass.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    productClass.created_at = DateTime.Now;
                    db.tb_product_class.Add(productClass);
                    db.SaveChanges();
                    return productClass.product_class_id;
                }
                else
                    return model.product_class_id;
            }
        }
        public static string GetClassTypeIdByName(string classTypeName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_class_type.Where(w => w.active == true && string.Compare(w.class_type_name.ToLower().Trim(), classTypeName.ToLower().Trim()) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_class_type classType = new tb_class_type();
                    classType.class_type_id = Guid.NewGuid().ToString();
                    classType.class_type_name = classTypeName;
                    classType.active = true;
                    classType.created_at = DateTime.Now;
                    classType.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_class_type.Add(classType);
                    db.SaveChanges();
                    return classType.class_type_id;
                }
                else
                    return model.class_type_id;

            }
        }
        public static string GetSupplierIdbyName(string supplierName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_supplier.Where(w => w.status == true && string.Compare(w.supplier_name, supplierName) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_supplier supplier = new tb_supplier();
                    supplier.supplier_id = Guid.NewGuid().ToString();
                    supplier.supplier_name = supplierName;
                    supplier.status = true;
                    supplier.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    supplier.created_date = DateTime.Now;
                    db.tb_supplier.Add(supplier);
                    db.SaveChanges();
                    return supplier.supplier_id;

                }
                else
                    return model.supplier_id;
            }
        }
        public static string GetUnitIdbyName(string unitName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_unit.Where(w => w.status == true && string.Compare(w.Name, unitName) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_unit unit = new tb_unit();
                    unit.Id = Guid.NewGuid().ToString();
                    unit.Name = unitName;
                    unit.status = true;
                    unit.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    unit.created_date = DateTime.Now;
                    db.tb_unit.Add(unit);
                    db.SaveChanges();
                    return unit.Id;
                }
                else
                    return model.Id;
            }
        }
        public static string GetBrandIdbyName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //var model = db.tb_brand.Where(w => w.active == true && string.Compare(w.brand_name, item.brand) == 0).FirstOrDefault();
                var model = (from br in db.tb_brand
                             join pt in db.tb_product_type on br.product_type_id equals pt.product_type_id
                             join gr in db.tb_class on pt.product_category_id equals gr.class_id
                             where br.active == true && string.Compare(br.brand_name.ToLower().Trim(), item.brand.ToLower().Trim()) == 0 
                             && string.Compare(pt.product_type_name.ToLower().Trim(), item.productType.ToLower().Trim()) == 0
                             && string.Compare(gr.class_name.ToLower().Trim(),item.classs.ToLower().Trim())==0
                             select br).FirstOrDefault();

                if (model == null)
                {
                    tb_brand brand = new tb_brand();
                    brand.brand_id = Guid.NewGuid().ToString();
                    brand.brand_name = item.brand;
                    brand.product_type_id = GetProductTypeIdByName(item);
                    brand.active = true;
                    brand.created_at = CommonClass.ToLocalTime(DateTime.Now);
                    brand.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_brand.Add(brand);
                    db.SaveChanges();
                    return brand.brand_id;
                }
                else
                    return model.brand_id;
            }
        }
        public static string GetProductTypeIdByName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //var model = db.tb_product_type.Where(w => w.active == true && string.Compare(w.product_type_name, item.productType) == 0).FirstOrDefault();
                var model = (from pt in db.tb_product_type
                             join ct in db.tb_class on pt.product_category_id equals ct.class_id
                             where pt.active == true && string.Compare(pt.product_type_name.ToLower().Trim(), item.productType.ToLower().Trim()) == 0 
                             && string.Compare(ct.class_name.ToLower().Trim(), item.classs.ToLower().Trim()) == 0
                             select pt).FirstOrDefault();
                if (model == null)
                {

                    tb_product_type productType = new tb_product_type();
                    productType.product_type_id = Guid.NewGuid().ToString();
                    //productType.product_category_id = GetCategoryIdByName(item);
                    productType.product_category_id= GetClassIdByName(item);
                    productType.product_type_name = item.productType;
                    productType.active = true;
                    productType.created_at = DateTime.Now;
                    productType.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_product_type.Add(productType);
                    db.SaveChanges();
                    return productType.product_type_id;
                }
                else
                    return model.product_type_id;
            }
        }
        public static string GetProductSizeIdByName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                //var model = db.tb_product_size.Where(w => w.active == true && string.Compare(w.product_size_name, item.productSize) == 0).FirstOrDefault();
                var model = (from ps in db.tb_product_size
                             join gr in db.tb_class on ps.brand_id equals gr.class_id
                             where ps.active == true && string.Compare(ps.product_size_name, item.productSize) == 0 && string.Compare(gr.class_name, item.classs) == 0
                             select ps).FirstOrDefault();
                if (model == null)
                {
                    tb_product_size productSize = new tb_product_size();
                    productSize.product_size_id = Guid.NewGuid().ToString();
                    if(!string.IsNullOrEmpty(item.classs))
                    //productSize.brand_id = GetBrandIdbyName(item);
                        productSize.brand_id = GetClassIdByName(item);
                    productSize.product_size_name = item.productSize;
                    productSize.active = true;
                    productSize.created_at = DateTime.Now;
                    productSize.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_product_size.Add(productSize);
                    db.SaveChanges();
                    return productSize.product_size_id;
                }
                else
                    return model.product_size_id;
            }
        }
        public static string GetCategoryIdByName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_product_category.Where(w => w.status == true && string.Compare(w.p_category_name, item.category) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_product_category category = new tb_product_category();
                    category.p_category_id = Guid.NewGuid().ToString();
                    category.p_category_name = item.category;
                    category.class_id = GetClassIdByName(item);
                    category.status = true;
                    category.created_date = DateTime.Now;
                    category.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_product_category.Add(category);
                    db.SaveChanges();
                    return category.p_category_id;
                }
                else
                    return model.p_category_id;
            }
        }
        public static string GetClassIdByName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = db.tb_class.Where(w => w.active == true && string.Compare(w.class_name.Trim().ToString(), item.classs.ToLower().Trim()) == 0).FirstOrDefault();
                if (model == null)
                {
                    tb_class classs = new tb_class();
                    classs.class_id = Guid.NewGuid().ToString();
                    classs.class_type_id = GetClassTypeIdByName(item.classType);
                    classs.class_code = ClassViewModel.GenerateGroupCode();
                    classs.class_name = item.classs;
                    classs.active = true;
                    classs.created_at =CommonClass.ToLocalTime(DateTime.Now);
                    classs.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    db.tb_class.Add(classs);
                    db.SaveChanges();
                    return classs.class_id;
                }
                else
                    return model.class_id;
            }
        }

        public static string GetSubGroupIdbyName(ExcelProductViewModel item)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var model = (from sg in db.tb_sub_group
                             join gr in db.tb_class on sg.class_id equals gr.class_id
                             where sg.is_active == true && string.Compare(sg.sub_group_name.ToLower().Trim().Replace(" ", ""), item.subGroup.ToLower().Replace(" ", "").Trim()) == 0
                             && string.Compare(gr.class_name.ToLower().Replace(" ", "").Trim(), item.classs.ToLower().Replace(" ", "").Trim()) == 0
                             select sg).FirstOrDefault();
                if (model == null)
                {
                    tb_sub_group sub_Group= new tb_sub_group();
                    sub_Group.sub_group_id = Guid.NewGuid().ToString();
                    sub_Group.class_id = GetClassIdByName(item);
                    sub_Group.sub_group_code = SubGroupModel.generateSubGroupCode();
                    sub_Group.sub_group_name = item.subGroup;
                    sub_Group.is_active= true;
                    sub_Group.created_at = CommonClass.ToLocalTime(DateTime.Now);
                    db.tb_sub_group.Add(sub_Group);
                    db.SaveChanges();
                    return sub_Group.sub_group_id;
                }
                else
                    return model.sub_group_id;
            }
        }
        public static string GetColorIdbyNmae(string colorName)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                var color = db.tb_color.Where(w => w.active == true && string.Compare(w.color_name, colorName) == 0).FirstOrDefault();
                if (color==null)
                {
                    tb_color item = new tb_color();
                    item.color_id = Guid.NewGuid().ToString() ;
                    item.color_name = colorName;
                    item.active = true;
                    item.created_at = DateTime.Now;
                    item.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().GetUserId();
                    item.updated_at = DateTime.Now;
                    db.tb_color.Add(item);
                    db.SaveChanges();
                    return item.color_id;

                }
                else
                    return color.color_id;
            }
        }
    }

    public class ExcelProductViewModel
    {
        public string productCode { get; set; }
        public string classs { get; set; }//group 
        public string subGroup { get; set; }
        public string category { get; set; }
        public string productDescription { get; set; }
        public string productType { get; set; }
        public string brand { get; set; }
        public string productSize { get; set; }
        public string productClass { get; set; }
        public string model { get; set; }
        public string supplier { get; set; }
        public string quantity { get; set; }
        public string unit { get; set; }
        public string unitprice { get; set; }
        public string laborHour { get; set; }
        public string remark { get; set; }
        public string insulationColor { get; set; }
        public string sheathColor { get; set; }
        public string classType { get; set; }
        public string numberOfCore { get; set; }
        public string outerSheath { get; set; }
        public string color { get; set; }
    }
    public class ImportProductExcelReasponse
    {
        public List<tb_product> successProducts { get; set; }
        public List<tb_product> failProducts { get; set; }
        public string message { get; set; }
        public ImportProductExcelReasponse()
        {
            successProducts = new List<tb_product>();
            failProducts = new List<tb_product>();
        }
    }
}