using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BT_KimMex.Class
{
    public class UpdateProductViaExcelModel
    {
        public static UpdateProductViaExcelResultResponse GetDataModelFromExcelContent(string path,bool hasHeader = true)
        {
            List<ExcelProductUpdatedModel> listExcelModel = new List<ExcelProductUpdatedModel>();
            string message=string.Empty;
            int errorLine = 0;
            int errorColumn = 0;
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[1];
                    var startRow = hasHeader ? 3 : 1;

                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row - 1; rowNum++)
                    {
                        errorLine = rowNum;
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        ExcelProductUpdatedModel excelModel = new ExcelProductUpdatedModel();
                        excelModel.product_id = wsRow[rowNum, 1].Text;
                        excelModel.product_code = wsRow[rowNum, 2].Text;
                        excelModel.new_product_code = wsRow[rowNum, 3].Text;
                        excelModel.product_name = wsRow[rowNum, 4].Text;
                        excelModel.product_unit = wsRow[rowNum, 5].Text;
                        excelModel.group_id = wsRow[rowNum, 13].Text;
                        excelModel.sub_group_id = wsRow[rowNum, 14].Text;
                        excelModel.labour_hour = wsRow[rowNum, 15].Text;
                        excelModel.so_number = wsRow[rowNum, 16].Text;
                        excelModel.cash_flow = wsRow[rowNum, 17].Text;
                        excelModel.product_type_id = wsRow[rowNum, 18].Text;
                        excelModel.brand_id = wsRow[rowNum, 19].Text;
                        excelModel.product_class_id = wsRow[rowNum, 20].Text;
                        excelModel.product_size_id = wsRow[rowNum, 21].Text;
                        excelModel.model_factory = wsRow[rowNum, 22].Text;
                        excelModel.color = wsRow[rowNum, 29].Text; ;
                        excelModel.product_size = wsRow[rowNum, 30].Text;
                        excelModel.product_category_id = wsRow[rowNum, 31].Text;
    
                        listExcelModel.Add(excelModel);
                    }
                    //listExcelModel = listExcelModel.Where(s => !string.IsNullOrEmpty(s.product_code) && !string.IsNullOrEmpty(s.labour_hour)).ToList();
                    return UpdateProductViaExcelModel.SaveDataToDatabase(listExcelModel);
                }
                catch(Exception ex)
                {
                    message = message + " " + string.Format("Importing Excel file error row {0} column {1}", errorLine, errorColumn);
                }
            }
            return new UpdateProductViaExcelResultResponse();
        }
        public static UpdateProductViaExcelResultResponse SaveDataToDatabase(List<ExcelProductUpdatedModel> listExcelModel)
        {
            UpdateProductViaExcelResultResponse response = new UpdateProductViaExcelResultResponse();
            List<ExcelProductUpdatedModel> successImported = new List<ExcelProductUpdatedModel>();
            List<ExcelProductUpdatedModel> errorImported = new List<ExcelProductUpdatedModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                foreach(var item in listExcelModel)
                {
                    if (!string.IsNullOrEmpty(item.product_id))
                    {
                        tb_product product = db.tb_product.Find(item.product_id);
                        if (product != null)
                        {
                            var isNumeric = int.TryParse(product.product_code, out _);
                            product.product_number = isNumeric ? Convert.ToDecimal(product.product_code) : product.product_number;
                            product.product_code = item.new_product_code;
                            product.group_id = item.group_id;
                            product.sub_group_id = item.sub_group_id;
                            product.labour_hour = string.IsNullOrEmpty(item.labour_hour) ? 0 : Convert.ToDecimal(item.labour_hour);
                            product.so_number = item.so_number;
                            product.cash_flow = item.cash_flow;
                            product.product_category_id = item.product_category_id;
                            product.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                            db.SaveChanges();
                            response.success.Add(item);

                        }
                        else
                        {
                            response.failed.Add(item);

                        }
                    }
                    else
                    {
                        tb_product product = db.tb_product.Where(s => string.Compare(s.product_code, item.product_code) == 0 && s.status==true).FirstOrDefault();
                        if (product != null)
                        {
                            var splitNewCode = item.new_product_code.Split('-');
                            //var isNumeric = int.TryParse(product.product_code, out _);
                            //product.product_number = isNumeric ? Convert.ToDecimal(product.product_code) : product.product_number;
                            product.product_number = Convert.ToInt32(splitNewCode[2]);
                            product.product_code = item.new_product_code;
                            product.group_id = item.group_id;
                            product.sub_group_id = item.sub_group_id;
                            product.labour_hour = string.IsNullOrEmpty(item.labour_hour) ? 0 : Convert.ToDecimal(item.labour_hour);
                            product.so_number = item.so_number;
                            product.cash_flow = item.cash_flow;
                            product.product_category_id = item.product_category_id;
                            product.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                            db.SaveChanges();
                            response.success.Add(item);
                        }
                        else
                        {
                            response.failed.Add(item);
                        }
                    }
                    
                }
            }catch(Exception ex)
            {

            }
            return response;
        }
    }
    public class ExcelProductUpdatedModel
    {
        public string product_id { get; set; }//0
        public string product_code { get; set; }//1
        public string new_product_code { get; set; }
        public string product_name { get; set; }
        public string product_unit { get; set; }
        public string unit_price {
            get; set;
        }
        public string group_id { get; set; }
        public string sub_group_id { get; set; }
        public string labour_hour { get; set; }
        public string so_number { get; set; }
        public string cash_flow { get; set; }
        public string product_type_id { get; set; }
        public string brand_id { get; set; }
        public string product_class_id { get; set; }
        public string product_size_id { get; set; }
        public string model_factory { get; set; }
        public string color { get; set; }
        public string product_size { get; set; }
        public string product_category_id { get; set; }
    }
    public class UpdateProductViaExcelResultResponse
    {
        public List<ExcelProductUpdatedModel> success { get; set; }
        public List<ExcelProductUpdatedModel> failed { get;set; }
        public UpdateProductViaExcelResultResponse()
        {
            success= new List<ExcelProductUpdatedModel>();
            failed= new List<ExcelProductUpdatedModel>();
        }
    }
}