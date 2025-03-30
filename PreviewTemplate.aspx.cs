using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class PreviewTemplate : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["rowid"] != null && Request.QueryString["type"] == null)
        {
            DataSet ds = SQLExecuteReader("system", "select * from WorkFlowTemplate where TemplateRowId='" + Request.QueryString["rowid"].ToString() + "'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                content.Text = ds.Tables[0].Rows[0]["TemplateContent"].ToString();

            }
        }
        else if (Request.QueryString["rowid"] != null && Request.QueryString["type"] != null)
        {
            DataSet ds = SQLExecuteReader("system", "select * from TemplateObject where TemplateRowId='" + Request.QueryString["rowid"].ToString() + "';SELECT WorkFlowStep.StepName,ObjectRowId FROM StepEnableObject INNER JOIN WorkFlowStep ON StepEnableObject.StepRowId = WorkFlowStep.StepRowId AND StepEnableObject.TemplateRowId = WorkFlowStep.TemplateRowId where StepEnableObject.TemplateRowId='" + Request.QueryString["rowid"].ToString() + "' order by seq");
            if (ds.Tables[0].Rows.Count > 0)
            {
                content.Text = "<table border=1 cellpadding=5><tr bgcolor=gray style='color:#ffffff'><td width=300px>Object Name</td><td>Type</td><td width=800px>Steps Where Use</td></tr>";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ds.Tables[1].DefaultView.RowFilter = "ObjectRowId='" + ds.Tables[0].Rows[i]["ObjectRowId"].ToString() + "'";
                    string stp = "";
                    for (int j = 0; j < ds.Tables[1].DefaultView.Count; j++)
                    {
                        stp += ds.Tables[1].DefaultView[j]["StepName"].ToString() + "<b>,</b> ";
                    }

                    content.Text += "<tr><td>{" + ds.Tables[0].Rows[i]["ObjectName"].ToString() + "} </td><td>" + ds.Tables[0].Rows[i]["ObjectType"].ToString() + "</td><td>" + stp + "</td>";
                }
                content.Text += "</table>";

            }
        }
    }
}