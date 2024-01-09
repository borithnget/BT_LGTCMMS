<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MR Report</title>
    <script runat="server">
        void Page_Load(object sender,EventArgs s)
        {
            if (!IsPostBack)
            {
                string id = string.Empty;
                string reportPath = Server.MapPath("~/Reports/MRrequestreport.rdlc");
                if (Request.QueryString["id"] != null)
                    id = Request.QueryString["id"].ToString();

                if (!string.IsNullOrEmpty(id))
                    BT_KimMex.Class.ReportHandler.GenerateMaterialRequestReport(this.ReportViewer1, reportPath, id);
            }
        }
        //protected void Export(object sender, EventArgs e)
        //{
        //    if(!IsPostBack)
        //        BT_KimMex.Class.ReportHandler.ExportExcelMR(this.ReportViewer1, this.rbFormat);
        //}



    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="False" SizeToReportContent="True">
        </rsweb:ReportViewer>
        <br />
    <%--Format:
    <asp:RadioButtonList ID="rbFormat" runat="server" RepeatDirection="Horizontal">
        <asp:ListItem Text="Word" Value="WORD" Selected="True" />
        <asp:ListItem Text="Excel" Value="EXCEL" />
        <asp:ListItem Text="PDF" Value="PDF" />
        <asp:ListItem Text="Image" Value="IMAGE" />
    </asp:RadioButtonList>
    <br />
    <asp:Button ID="btnExport" Text="Export" runat="server" OnClick="Export" />--%>
    </div>
    </form>
</body>
</html>
