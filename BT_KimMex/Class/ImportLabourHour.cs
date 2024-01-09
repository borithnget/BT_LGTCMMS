using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BT_KimMex.Class
{
    public class ImportLabourHourExcelResponse
    {
        public static ImportProductLabourHourResultResponse GetDataModelFromExcelContact(string path, bool hasHeader = true)
        {
            List<ExcelProductLabourHourViewModel> listExcelModel = new List<ExcelProductLabourHourViewModel>();
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
                        ExcelProductLabourHourViewModel excelModel = new ExcelProductLabourHourViewModel();
                        
                        excelModel.product_code = wsRow[rowNum, 1].Text;
                        excelModel.labour_hour = wsRow[rowNum, 2].Text;
                        
                        listExcelModel.Add(excelModel);
                    }
                    listExcelModel = listExcelModel.Where(s => !string.IsNullOrEmpty(s.product_code) && !string.IsNullOrEmpty(s.labour_hour)).ToList();
                    return ImportLabourHourExcelResponse.SaveDataToDatabase(listExcelModel);
                }
                catch (Exception ex)
                {
                    //ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ImportProductfromExcel.cs", "GetDataModelFromExcelContact", ex.StackTrace, ex.Message);
                    message = message + " " + string.Format("Importing Excel file error row {0} column {1}", errorLine, errorColumn);
                }
            }
            return new ImportProductLabourHourResultResponse();
        }
        public static ImportProductLabourHourResultResponse SaveDataToDatabase(List<ExcelProductLabourHourViewModel> listExcelModel)
        {
            ImportProductLabourHourResultResponse response = new ImportProductLabourHourResultResponse();
            List<ExcelProductLabourHourViewModel> successImported = new List<ExcelProductLabourHourViewModel>();
            List<ExcelProductLabourHourViewModel> errorImported = new List<ExcelProductLabourHourViewModel>();
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                foreach(var item in listExcelModel)
                {
                    if(!string.IsNullOrEmpty(item.product_code) && !string.IsNullOrEmpty(item.labour_hour))
                    {
                        var product = db.tb_product.Where(s => s.status == true && string.Compare(s.product_code, item.product_code) == 0).FirstOrDefault();
                        if(product==null)
                        {
                            response.error.Add(item);
                        }
                        else
                        {
                            product.labour_hour = Convert.ToDecimal(item.labour_hour);
                            db.SaveChanges();
                            response.success.Add(item);
                        }
                    }
                }
            }catch(Exception ex)
            {


            }
            return response;
        }
    }
    public class ExcelProductLabourHourViewModel
    {
        public string product_code { get; set; }
        public string labour_hour { get; set; }
    }
    public class ImportProductLabourHourResultResponse
    {
        public List<ExcelProductLabourHourViewModel> success { get; set; }
        public List<ExcelProductLabourHourViewModel> error { get; set; }
        public ImportProductLabourHourResultResponse()
        {
            success = new List<ExcelProductLabourHourViewModel>();
            error = new List<ExcelProductLabourHourViewModel>();
        }
    }
}