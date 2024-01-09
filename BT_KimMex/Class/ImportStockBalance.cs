using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using OfficeOpenXml;

namespace BT_KimMex.Class
{
    public class ImportStockBalance
    {
        //public StockBalancedImportReturn GetDataFromExcelContent(string path,bool hasHeader = true)
        //{
        //    var db = new BT_KimMex.Entities.kim_mexEntities();
        //    string message = string.Empty;
        //    List<ImportStockBalanceModel> ErrorItem = new List<ImportStockBalanceModel>();
        //    List<ImportStockBalanceModel> SuccessItems = new List<ImportStockBalanceModel>();
        //    int SuccessItemCount = 0;
        //    int rowNumber = 0;

        //    using(var pck=new OfficeOpenXml.ExcelPackage())
        //    {
        //        try
        //        {
        //            using (var stream = File.OpenRead(path))
        //            {
        //                pck.Load(stream);
        //            }
        //            var ws = pck.Workbook.Worksheets[1];
        //            int startRow = hasHeader ? 13 : 1;
        //            string warehouseId = string.Empty;
        //            string WarehouseName = ws.Cells[10, 1].Text;

        //            //check Warehouse name is existing or not
        //            var wh = db.tb_warehouse.Where(x => x.warehouse_name == WarehouseName).FirstOrDefault();
        //            if(wh == null)
        //            {
        //                message = "No Warehouse exist in system, Please create a new warehouse!!!";
        //            }
        //            else
        //            {
        //                List<ImportStockBalanceModel> lstExcelModel = new List<ImportStockBalanceModel>();
        //                for(int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        //                {
        //                    rowNumber = rowNum;
        //                    //Retrive Data from each cells
        //                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
        //                    ImportStockBalanceModel excelModel = new ImportStockBalanceModel();
        //                    excelModel.itemCode = wsRow[rowNum, 2].Text;
        //                    excelModel.itemName = wsRow[rowNum, 3].Text;
        //                    excelModel.itemUnit = wsRow[rowNum, 4].Text;
        //                    excelModel.itemQuantity =string.IsNullOrEmpty(wsRow[rowNum, 5].Text) ? 0 : Convert.ToDecimal(wsRow[rowNum, 5].Text);
        //                    if(!String.IsNullOrEmpty(wsRow[rowNum, 6].Text))
        //                    {
        //                        decimal value;
        //                        var splitPrice = wsRow[rowNum, 6].Text.Replace("$", "");
        //                        if(Decimal.TryParse(splitPrice,out value))
        //                            excelModel.itemPrice = Convert.ToDouble(wsRow[rowNum, 6].Text.Replace("$", ""));
        //                    }

        //                    //Check item Code & Name
        //                    if (excelModel.itemCode != "" && excelModel.itemName != "" )
        //                    {
        //                        Entities.tb_inventory NewItem = new Entities.tb_inventory();
        //                        NewItem.inventory_id = Guid.NewGuid().ToString();
        //                        NewItem.inventory_date = DateTime.Now;
        //                        NewItem.inventory_status_id = "9";
        //                        NewItem.warehouse_id = wh.warehouse_id;

        //                        //Check Product
        //                        var Pro = db.tb_product.FirstOrDefault(x => x.product_code == excelModel.itemCode || x.product_name == excelModel.itemName);
        //                        if(Pro == null)
        //                        {
        //                            //Check Product_Category Id 
        //                            string categoryCode = excelModel.itemCode.Substring(0,3);
        //                            var CategoryID = db.tb_product_category.FirstOrDefault(x => x.p_category_code == categoryCode);
        //                            if(CategoryID == null)
        //                            {
        //                                ErrorItem.Add(excelModel);
        //                            }
        //                            else
        //                            {
        //                                Entities.tb_product newProduct = new Entities.tb_product();
        //                                newProduct.product_id = Guid.NewGuid().ToString();
        //                                newProduct.brand_id = CategoryID.p_category_id;
        //                                newProduct.product_code = excelModel.itemCode;
        //                                newProduct.product_name = excelModel.itemName;
        //                                newProduct.product_unit = excelModel.itemUnit;
        //                                newProduct.unit_price = Convert.ToDecimal(excelModel.itemPrice);
        //                                newProduct.status = true;
        //                                newProduct.created_date = DateTime.Now;
        //                                newProduct.created_by = "System";
        //                                db.tb_product.Add(newProduct);
        //                                db.SaveChanges();

        //                                NewItem.product_id = newProduct.product_id;
        //                                NewItem.total_quantity = excelModel.itemQuantity;
        //                                NewItem.in_quantity = excelModel.itemQuantity;
        //                                SuccessItemCount++;
        //                                db.tb_inventory.Add(NewItem);

        //                                SuccessItems.Add(excelModel);
        //                            }    
        //                        }
        //                        else
        //                        {
        //                            NewItem.product_id = Pro.product_id;
        //                            NewItem.total_quantity = excelModel.itemQuantity;
        //                            NewItem.in_quantity = excelModel.itemQuantity;
        //                            SuccessItemCount++;
        //                            db.tb_inventory.Add(NewItem);

        //                            SuccessItems.Add(excelModel);
        //                        }                          
        //                    }
               
        //                }
        //                db.SaveChanges();
        //                message = "File import to system is successfully.";
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            message = ex.Message+" "+rowNumber;
        //            ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ImportStockBalance.cs", "GetDataFromExcelContent", ex.StackTrace, ex.Message);
        //        }
        //    }

        //    return new StockBalancedImportReturn { Message = message, ErrorItem = ErrorItem, SuccessItemCount = SuccessItemCount,successItems=SuccessItems };
        //}
        /*
        public string SaveStockBalanceDataToDB(List<ImportStockBalanceModel> excelModel)
        {

        }
        */

        public StockBalancedImportReturn GetDataFromExcelContent(string path, bool hasHeader = true)
        {
            var db = new BT_KimMex.Entities.kim_mexEntities();
            string message = string.Empty;
            List<ImportStockBalanceModel> ErrorItem = new List<ImportStockBalanceModel>();
            List<ImportStockBalanceModel> SuccessItems = new List<ImportStockBalanceModel>();
            int SuccessItemCount = 0;
            int rowNumber = 0;
            List<ImportStockBalanceModel> lstExcelModel = new List<ImportStockBalanceModel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets[1];
                    int startRow = hasHeader ? 13 : 1;
                    string warehouseId = string.Empty;
                    string WarehouseName = ws.Cells[10, 1].Text;

                    //check Warehouse name is existing or not
                    var wh = db.tb_warehouse.Where(x => x.warehouse_name == WarehouseName).FirstOrDefault();
                    if (wh == null)
                    {
                        message = "No Warehouse exist in system, Please create a new warehouse!!!";
                    }
                    else
                    {
                        
                        for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                        {
                            rowNumber = rowNum;
                            //Retrive Data from each cells
                            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                            ImportStockBalanceModel excelModel = new ImportStockBalanceModel();
                            excelModel.itemCode = wsRow[rowNum, 2].Text;
                            excelModel.itemName = wsRow[rowNum, 3].Text;
                            excelModel.itemUnit = wsRow[rowNum, 4].Text;
                            excelModel.itemQuantity = string.IsNullOrEmpty(wsRow[rowNum, 5].Text) ? 0 : Convert.ToDecimal(wsRow[rowNum, 5].Text);
                            if (!String.IsNullOrEmpty(wsRow[rowNum, 6].Text))
                            {
                                decimal value;
                                var splitPrice = wsRow[rowNum, 6].Text.Replace("$", "");
                                if (Decimal.TryParse(splitPrice, out value))
                                    excelModel.itemPrice = Convert.ToDouble(wsRow[rowNum, 6].Text.Replace("$", ""));
                            }

                            lstExcelModel.Add(excelModel);

                            
                        }
                        lstExcelModel = lstExcelModel.Where(s => !string.IsNullOrEmpty(s.itemCode) && !string.IsNullOrEmpty(s.itemName)).ToList();

                        foreach(var excelModel in lstExcelModel)
                        {
                            //Check item Code & Name
                            if (excelModel.itemCode != "" && excelModel.itemName != "")
                            {
                                Entities.tb_inventory NewItem = new Entities.tb_inventory();
                                NewItem.inventory_id = Guid.NewGuid().ToString();
                                NewItem.inventory_date = DateTime.Now;
                                NewItem.inventory_status_id = "9";
                                NewItem.warehouse_id = wh.warehouse_id;

                                //Check Product
                                var Pro = db.tb_product.FirstOrDefault(x => x.product_code == excelModel.itemCode || x.product_name == excelModel.itemName);
                                if (Pro == null)
                                {
                                    //Check Product_Category Id 
                                    string categoryCode = excelModel.itemCode.Substring(0, 3);
                                    var CategoryID = db.tb_product_category.FirstOrDefault(x => x.p_category_code == categoryCode);
                                    if (CategoryID == null)
                                    {
                                        ErrorItem.Add(excelModel);
                                    }
                                    else
                                    {
                                        Entities.tb_product newProduct = new Entities.tb_product();
                                        newProduct.product_id = Guid.NewGuid().ToString();
                                        newProduct.brand_id = CategoryID.p_category_id;
                                        newProduct.product_code = excelModel.itemCode;
                                        newProduct.product_name = excelModel.itemName;
                                        newProduct.product_unit = excelModel.itemUnit;
                                        newProduct.unit_price = Convert.ToDecimal(excelModel.itemPrice);
                                        newProduct.status = true;
                                        newProduct.created_date = DateTime.Now;
                                        newProduct.created_by = "System";
                                        db.tb_product.Add(newProduct);
                                        db.SaveChanges();

                                        NewItem.product_id = newProduct.product_id;
                                        NewItem.total_quantity = excelModel.itemQuantity;
                                        NewItem.in_quantity = excelModel.itemQuantity;
                                        SuccessItemCount++;
                                        db.tb_inventory.Add(NewItem);

                                        SuccessItems.Add(excelModel);
                                    }
                                }
                                else
                                {
                                    NewItem.product_id = Pro.product_id;
                                    NewItem.total_quantity = excelModel.itemQuantity;
                                    NewItem.in_quantity = excelModel.itemQuantity;
                                    SuccessItemCount++;
                                    db.tb_inventory.Add(NewItem);

                                    SuccessItems.Add(excelModel);
                                }
                            }
                        }

                        db.SaveChanges();
                        message = "File import to system is successfully.";
                    }
                }
                catch (Exception ex)
                {
                    message = ex.InnerException.InnerException.ToString() + " " + rowNumber;
                    ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "ImportStockBalance.cs", "GetDataFromExcelContent", ex.StackTrace, ex.Message);
                }
            }
            if(lstExcelModel.Count() != SuccessItems.Count())
            {
                foreach(var i in lstExcelModel)
                {
                    if (i.itemCode != "" && i.itemName != "")
                    {
                        var isExist = SuccessItems.Where(s => string.Compare(s.itemCode, i.itemCode) == 0).FirstOrDefault();
                        if (isExist == null)
                            ErrorItem.Add(i);
                    }
                }
            }

            return new StockBalancedImportReturn { Message = message, ErrorItem = ErrorItem, SuccessItemCount = SuccessItemCount, successItems = SuccessItems,AllItemCount= lstExcelModel.Count() };
        }

        private string removeCurrencyFormat(string currency)
        {
            string resultString = currency;
            resultString = resultString.Replace("$", "");
            resultString = resultString.Replace(",", "");
            return resultString;
        }
    }

    public class ImportStockBalanceModel
    {
        public string itemId { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public decimal itemQuantity { get; set; }
        public string itemUnit { get; set; }
        public double itemPrice { get; set; }


        public string toString()
        {
            return $"Item Code: {this.itemCode} - Item Name: {this.itemName}";
        }
    }

    public class StockBalancedImportReturn
    {
        public string Message { get; set; }
        public List<ImportStockBalanceModel> ErrorItem = new List<ImportStockBalanceModel>();
        public List<ImportStockBalanceModel> successItems = new List<ImportStockBalanceModel>();
        public int AllItemCount {
            //get {
            //    return SuccessItemCount + ErrorItemCount;    
            //}
            get;set;
        }
        public int SuccessItemCount { get; set; }
        public int ErrorItemCount { get
            {
                return this.ErrorItem.Count;
            }
        }
    }
}