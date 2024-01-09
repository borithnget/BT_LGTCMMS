using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BT_KimMex.Class
{
    public class UpdateProductCategoryViaExcelModel
    {
        public static UpdateProductCategoryViaExcelResultResponse GetDataFromExcelContent(string path,bool hasHeader = true)
        {
            List<ExcelProductCategoryModel> listExcelModel = new List<ExcelProductCategoryModel>();
            string message = string.Empty;
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
                        ExcelProductCategoryModel excelModel = new ExcelProductCategoryModel();
                        excelModel.product_category_id = wsRow[rowNum, 1].Text;
                        excelModel.sub_group_id = wsRow[rowNum, 12].Text;
                        

                        listExcelModel.Add(excelModel);
                    }
                    return UpdateProductCategoryViaExcelModel.SaveDataToDatabase(listExcelModel); 
                }
                catch(Exception ex)
                {
                    message = message + " " + string.Format("Importing Excel file error row {0} column {1}", errorLine, errorColumn);
                }
            }
            return new UpdateProductCategoryViaExcelResultResponse();
        }

        public static UpdateProductCategoryViaExcelResultResponse SaveDataToDatabase(List<ExcelProductCategoryModel> listExcelModel)
        {
            UpdateProductCategoryViaExcelResultResponse response = new UpdateProductCategoryViaExcelResultResponse();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                foreach(var item in listExcelModel)
                {
                    tb_product_category productCategory = db.tb_product_category.Find(item.product_category_id);
                    if (productCategory == null)
                    {
                        response.failed.Add(item);

                    }
                    else
                    {
                        productCategory.sub_group_id = item.sub_group_id;
                        productCategory.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                        db.SaveChanges();
                        response.success.Add(item);
                    }
                }
            }catch(Exception ex)
            {

            }
            return response;
        }

    }

    public class ExcelProductCategoryModel
    {
        public string product_category_id { get; set; }
        public string sub_group_id { get; set; }
    }
    public class UpdateProductCategoryViaExcelResultResponse
    {
        public List<ExcelProductCategoryModel> success { get; set; }
        public List<ExcelProductCategoryModel> failed { get; set; }
        public UpdateProductCategoryViaExcelResultResponse()
        {
            success = new List<ExcelProductCategoryModel>();
            failed = new List<ExcelProductCategoryModel>();
        }

    }
}