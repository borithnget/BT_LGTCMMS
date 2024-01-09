using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Microsoft.Reporting.WebForms;
using BT_KimMex.Entities;
using System.Data;

namespace BT_KimMex.Reports.ReportController
{
    public partial class Project : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ReportViewer1.ProcessingMode = ProcessingMode.Local;
            //ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Project.rdlc");

            //using (kim_mexEntities db = new kim_mexEntities())
            //{
            //    //var obj = db.tb_project.Where(x => x.project_status == true).ToList();
            //    var obj = db.tb_product.OrderBy(o => o.product_code).Where(w => w.status == true).ToList();
            //    DataTable tb = new DataTable();
            //    if (obj.Any())
            //    {
            //        DataRow dtr;
            //        DataColumn col = new DataColumn();
            //        /*
            //        col.ColumnName = "project_id";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "project_no";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "project_short_name";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "project_full_name";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "project_start_date";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "project_actual_start_date";
            //        tb.Columns.Add(col);

            //        foreach (var pro in obj)
            //        {
            //            dtr = tb.NewRow();
            //            dtr["project_id"] = pro.project_id;
            //            dtr["project_no"] = pro.project_no;
            //            dtr["project_short_name"] = pro.project_short_name;
            //            dtr["project_full_name"] = pro.project_full_name;
            //            dtr["project_start_date"] = pro.project_start_date;
            //            dtr["project_actual_start_date"] = pro.project_actual_start_date;
            //            tb.Rows.Add(dtr);
            //        }
            //        */

            //        col.ColumnName = "item_code";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "item_name";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "item_unit";
            //        tb.Columns.Add(col);
            //        col = new DataColumn();
            //        col.ColumnName = "item_price";
            //        tb.Columns.Add(col);

            //        foreach(var o in obj)
            //        {
            //            dtr = tb.NewRow();
            //            dtr["item_code"] = o.product_code;
            //            dtr["item_name"] = o.product_name;
            //            dtr["item_unit"] = o.product_unit;
            //            dtr["item_price"] = o.unit_price;
            //            tb.Rows.Add(dtr);
                    
            //        }

            //        //ReportViewer1.Reset();
            //        ReportViewer1.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Project.rdlc";

            //        ReportViewer1.LocalReport.DataSources.Clear();
            //        //ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Project", tb));
            //        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("ItemList", tb));

            //        //ReportDataSource rds1 = new ReportDataSource("Project", tb);
            //        //ReportDataSource rds2 = new ReportDataSource("Project", tb);
            //        //ReportViewer1.LocalReport.DataSources.Clear();
            //        //ReportViewer1.LocalReport.DataSources.Add(rds1);
            //        //ReportViewer1.LocalReport.DataSources.Add(rds2);
            //        ////ReportViewer1.LocalReport.ReportEmbeddedResource = "BT_KimMex.Reports.Project.rdlc";
            //        ////ReportViewer1.LocalReport.ReportEmbeddedResource = "BT_KimMex.Reports.Project.rdlc";
            //        //ReportViewer1.LocalReport.Refresh();
            //        //ReportViewer1.DataBind();

            //        ReportViewer1.LocalReport.Refresh();
            //    }
            //}

        }
    }
}