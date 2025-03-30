using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

public partial class download : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            DataSet ds2 = SQLExecuteReader("system", @"SELECT    *
                                                        FROM      Attechment where Attechment.id=" + Request.QueryString["id"].ToString());
            if (ds2.Tables[0].Rows.Count > 0)
            {
                DownloadFile(Request.QueryString["id"].ToString());
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ware", "window.close();", true);
            }
        }
    }

    #region Download

    public void DownloadFile(String ID)
    {


        DataSet ds = this.SQLExecuteReader("system", "select Name, Attechment, Content_type, Size from Attechment where id=" + ID.ToString());

        if (ds.Tables[0].Rows.Count > 0)
        {
            Response.AddHeader("Content-type", ds.Tables[0].Rows[0]["Content_type"].ToString());
            string enCodeFileName = "";
            if (Request.Browser.Type.Contains("Firefox"))
            {
                enCodeFileName = ds.Tables[0].Rows[0]["Name"].ToString();
            }
            else
            {
                enCodeFileName = Server.UrlEncode(ds.Tables[0].Rows[0]["Name"].ToString());
            }
            Response.AddHeader("Content-Disposition", "attachment;filename=\"" + enCodeFileName + "\"");
            //Response.AddHeader("Content-Disposition", "attachment; filename=" + ds.Tables[0].Rows[0]["Name"].ToString());
            Response.BinaryWrite((Byte[])ds.Tables[0].Rows[0]["Attechment"]);
            Response.Flush();
            Response.End();
        }
    }

    #endregion
}