using BT_KimMex.Entities;
using BT_KimMex.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BT_KimMex.Class
{
    public class Importing
    {
        public BT_KimMex.Entities.kim_mexEntities kimEntity = new Entities.kim_mexEntities();
        public string GetDataModelFromExcelContact(string path, bool hasHeader = true)
        {
            int errorLine = 0;
            int errorColumn = 0;
            var message = string.Empty;
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[2];
                    var startRow = hasHeader ? 5 : 1;
                    var projectName = string.Empty;
                    List<ExcelBoQModel> listExcelModel = new List<ExcelBoQModel>();
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row-3; rowNum++)
                    {
                        errorLine = rowNum;
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        projectName = wsRow[startRow, 2].Text;
                        ExcelBoQModel excelModel = new ExcelBoQModel();
                        excelModel.itemCode = wsRow[rowNum, 3].Text;
                        excelModel.categoryName = wsRow[rowNum, 4].Text;
                        excelModel.subCategoryName = wsRow[rowNum, 5].Text;
                        excelModel.subCategoryItem = wsRow[rowNum, 5].Text;
                        excelModel.chartAccount = wsRow[rowNum, 7].Text;
                        excelModel.unit = wsRow[rowNum, 8].Text;
                        if (!String.IsNullOrEmpty(wsRow[rowNum, 9].Text))
                        {
                            excelModel.qty = Convert.ToDouble(removeCurrencyFormat(wsRow[rowNum, 9].Text));
                            errorColumn = 9;
                        }
                        if (!String.IsNullOrEmpty(wsRow[rowNum, 11].Text))
                        {
                            excelModel.amount = Convert.ToDouble(removeCurrencyFormat(wsRow[rowNum, 11].Text));
                            excelModel.jobCategoryAmount = Convert.ToDouble(removeCurrencyFormat(wsRow[rowNum, 11].Text));
                            errorColumn = 11;
                        }
                        if (!String.IsNullOrEmpty(wsRow[rowNum, 10].Text))
                        {
                            excelModel.unitPrice = Convert.ToDouble(removeCurrencyFormat(wsRow[rowNum, 10].Text));
                            errorColumn = 10;
                        }
                        excelModel.remark = wsRow[rowNum, 12].Text;
                        excelModel.jobCategoryRemark = wsRow[rowNum, 12].Text;
                        listExcelModel.Add(excelModel);

                    }
                    message=SaveDataToBoQ(listExcelModel, projectName);
                }
                catch (Exception ex)
                {
                    //throw ex;
                    ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "Importing.cs", "GetDataModelFromExcelContact", ex.StackTrace, ex.Message);
                    message = string.Format("Importing Excel file error row {0} column {1}",errorLine,errorColumn);
                }
            }
            return message;
        }
        private string SaveDataToBoQ(List<ExcelBoQModel> listExcelModel, string projectName)
        {
            bool isExist = false;
            string message = String.Empty;
            try
            {
                var project_id = getProjectIDByProjectName(projectName);
                isExist = GlobalMethod.IsProjectExist(project_id);
                if (isExist)
                {
                    isExist = GlobalMethod.IsBOQProjectExist(project_id);
                    if (!isExist)
                    {
                        var boqid = System.Guid.NewGuid().ToString();
                        using (kim_mexEntities db = new kim_mexEntities())
                        {
                            tb_build_of_quantity boq = new tb_build_of_quantity();
                            boq.project_id = getProjectIDByProjectName(projectName);
                            boq.boq_id = boqid;
                            boq.boq_no = GenerateBOQCode();
                            boq.created_date = DateTime.Now;
                            boq.updated_date = DateTime.Now;
                            boq.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            boq.boq_status = "Imported";
                            boq.status = true;
                            db.tb_build_of_quantity.Add(boq);
                            db.SaveChanges();
                            //}
                            //using (kim_mexEntities db = new kim_mexEntities())
                            //{
                            string boqDetail1ID = string.Empty;
                            string boqDetail2ID = string.Empty;
                            string itemTypeID = string.Empty;
                            foreach (ExcelBoQModel excelItem in listExcelModel)
                            {
                                if (!String.IsNullOrEmpty(excelItem.categoryName))
                                {
                                    tb_boq_detail1 boqDetail1 = new tb_boq_detail1();
                                    boqDetail1.boq_id = boqid;
                                    boqDetail1.boq_detail1_id = System.Guid.NewGuid().ToString();
                                    boqDetail1.job_category_code = excelItem.itemCode;
                                    boqDetail1.job_category_id = getJobCategoryID(excelItem.categoryName);
                                    boqDetail1.amount = Convert.ToDecimal(excelItem.amount);
                                    boqDetail1.remark = excelItem.remark;
                                    db.tb_boq_detail1.Add(boqDetail1);
                                    db.SaveChanges();
                                    boqDetail1ID = boqDetail1.boq_detail1_id;
                                }
                                else if (String.IsNullOrEmpty(excelItem.unit))
                                {
                                    tb_boq_detail2 boqDetail2 = new tb_boq_detail2();
                                    boqDetail2.boq_detail1_id = boqDetail1ID;
                                    boqDetail2.boq_detail2_id = System.Guid.NewGuid().ToString();

                                    boqDetail2.remark = excelItem.remark;
                                    boqDetail2.amount = Convert.ToDecimal(excelItem.amount);
                                    boqDetail2.item_type_id = getProductCategoryIDByCategoryCode(excelItem.itemCode, excelItem.subCategoryName, excelItem.chartAccount);
                                    db.tb_boq_detail2.Add(boqDetail2);
                                    db.SaveChanges();
                                    boqDetail2ID = boqDetail2.boq_detail2_id;
                                    itemTypeID = boqDetail2.item_type_id;
                                }
                                else if (!String.IsNullOrEmpty(excelItem.unit))
                                {
                                    tb_boq_detail3 boqDetail3 = new tb_boq_detail3();
                                    boqDetail3.boq_detail2_id = boqDetail2ID;
                                    boqDetail3.boq_detail3_id = System.Guid.NewGuid().ToString();

                                    tb_product pro = getProductIdByProductCode(itemTypeID, excelItem.subCategoryName, excelItem.itemCode, excelItem.unit, excelItem.unitPrice);

                                    boqDetail3.item_id = pro.product_id;
                                    boqDetail3.item_unit_price = pro.unit_price;
                                    boqDetail3.item_qty = Convert.ToDecimal(excelItem.qty);
                                    db.tb_boq_detail3.Add(boqDetail3);
                                    db.SaveChanges();

                                }

                            }
                        }
                    }
                    else
                        message = "Project BOQ is already exist!";
                }else
                {
                    message = "Project not found! Please create Project.";
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "Importing.cs", "SaveDataToBoQ", ex.StackTrace, ex.Message);
            }
            return message;
        }
        private string getProjectIDByProjectName(string projectName)
        {
            string projectId = string.Empty;
            try
            {
                projectId = kimEntity.tb_project.Where(project => project.project_short_name == projectName && project.p_status!="Inactive"&&project.project_status==true).FirstOrDefault().project_id;
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "Importing.cs", "getProjectIDByProjectName", ex.StackTrace, ex.Message);
            }
            return projectId;
        }

        private string getProductCategoryIDByCategoryCode(string categoryCode,string categoryName,string chartAccount)
        {
            string productCategoryID = string.Empty;
            try
            {
                var productCategory = kimEntity.tb_product_category.Where(category => category.p_category_code == categoryCode && category.status==true).FirstOrDefault();
                if(productCategory !=null)
                {
                    productCategoryID = productCategory.p_category_id;
                }
                else
                {
                    productCategoryID = createProductCategory(categoryCode, categoryName, chartAccount);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "Importing.cs", "getProductCategoryIDByCategoryName", ex.StackTrace, ex.Message);
            }

            return productCategoryID;
        }
        private tb_product getProductIdByProductCode(string categoryId, string productName, string productCode, string unit, double unitPrice)
        {
            tb_product productID = new tb_product();
            try
            {
                productID = kimEntity.tb_product.Where(product => product.product_code == productCode && product.status==true).FirstOrDefault();
                if(productID ==null)
                {
                    productID = createProduct(categoryId, productName, productCode, unit, unitPrice);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "Importing.cs", "getProductCategoryIDByCategoryName", ex.StackTrace, ex.Message);
            }
            return productID;
        }

        public string GenerateBOQCode()
        {
            string boq_no = "", digit = "";
            kim_mexEntities db = new kim_mexEntities();
            var last_no = (from tbl in db.tb_build_of_quantity orderby tbl.created_date descending select tbl.boq_no).FirstOrDefault();
            if (last_no == null || last_no == "")
                digit = "001";
            else
            {
                last_no = last_no.Substring(last_no.Length - 3, 3);
                int number = Convert.ToInt32(last_no) + 1;
                if (number.ToString().Length == 1)
                    digit = "00" + number;
                else if (number.ToString().Length == 2)
                    digit = "0" + number;
                else if (number.ToString().Length == 3)
                    digit = number.ToString();
            }
            string dd = DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string mm = DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            boq_no = "BOQ-" + dd + "-" + mm + "-" + digit;
            return boq_no;
        }
        public string getJobCategoryID(String categoryName)
        {
            string jobCategoryID = string.Empty;
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {

                    var tbj = from tb in db.tb_job_category select tb;
                    foreach (var item in tbj)
                    {
                        if (item.j_category_name == categoryName)
                        {
                            jobCategoryID = item.j_category_id;
                            break;
                        }
                    }

                    if (String.IsNullOrEmpty(jobCategoryID))
                    {
                        jobCategoryID = createJobCategory(categoryName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jobCategoryID;
        }
        private string createJobCategory(string categoryName)
        {
            string jobCategoryID = System.Guid.NewGuid().ToString();

            using (kim_mexEntities db = new kim_mexEntities())
            {
                tb_job_category jobcat = new tb_job_category();
                jobcat.j_category_id = jobCategoryID;
                jobcat.j_category_name = categoryName;
                jobcat.j_status = true;
                jobcat.created_date = DateTime.Now;
                jobcat.updated_date = DateTime.Now;
                jobcat.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().Name; 
                db.tb_job_category.Add(jobcat);
                db.SaveChanges();
            }

            return jobCategoryID;
        }
        private string createProductCategory(string categoryCode,string categoryName,string chartAccount)
        {
            string categoryID = string.Empty;
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    categoryID = System.Guid.NewGuid().ToString();
                    tb_product_category productCate = new tb_product_category();
                    productCate.p_category_id =  categoryID;
                    productCate.p_category_name = categoryName;
                    productCate.p_category_code = categoryCode;
                    productCate.chart_account = chartAccount;
                    productCate.status = true;
                    productCate.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    productCate.created_date = DateTime.Now;
                    productCate.updated_date = DateTime.Now;
                    db.tb_product_category.Add(productCate);
                    db.SaveChanges();

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return categoryID;
        }
        private tb_product createProduct(string categoryId, string productName,string productCode,string unit,double unitPrice)
        {
            string productId = System.Guid.NewGuid().ToString();
            tb_product product = new tb_product();
            try
            {
                using (kim_mexEntities db = new kim_mexEntities())
                {
                    
                    product.brand_id = categoryId;
                    product.product_id = productId;
                    product.product_name = productName;
                    product.product_code = productCode;
                    product.product_unit = unit;
                    product.unit_price = Convert.ToDecimal(unitPrice);
                    product.status = true;
                    product.created_date = DateTime.Now;
                    product.updated_date = DateTime.Now;
                    product.created_by= System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    db.tb_product.Add(product);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return product;
        }
        private string removeCurrencyFormat(string currency)
        {
            string resultString = currency;
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (regexItem.IsMatch(resultString))
            {
                resultString = resultString.Replace("$", "");
                resultString = resultString.Replace(",", "");
            }
            return resultString;
        }
        private string removeDotInString (string str)
        {
            return str.Replace(".", "");
        }
    }

}