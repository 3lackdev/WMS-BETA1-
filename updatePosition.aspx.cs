using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using HtmlAgilityPack;



public partial class template : BST.libDB
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           // this.Master.Visible = false;
            this.SetCurentNavigatePage("Position");

            refresh();
        }
        ClientScript.RegisterStartupScript(this.GetType(), "ware", "document.getElementById('navi').style.display = 'none';document.getElementById('logomain').style.display = 'none';", true);

    }

    private void refresh()
    {
        if (Request.QueryString["rowid"] != null)
        {
            DataSet ds = this.SQLExecuteReader("system", "SELECT  PositionRowId, PositionName, WFRowId, replace(PositionEmail,';',';<br />') as PositionEmail, SelectedKey FROM   Prosition    where WFRowId='" + Request.QueryString["rowid"].ToString() + "';select * from WorkFlow where WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();

            lb_poname.Text = "WorkFlow Name : " + ds.Tables[1].Rows[0]["WFName"].ToString();


            if (ds.Tables[1].Rows[0]["useredit"].ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    LinkButton lb = (LinkButton)GridView1.Rows[i].Cells[1].FindControl("lnk_edit");
                    if (lb != null)
                    {
                        lb.Visible = false;
                    }

                    Button bt = (Button)GridView1.Rows[i].Cells[1].FindControl("Button1");
                    if (bt != null)
                    {
                        bt.Visible = false;
                    }

                }

                btn_new.Visible = false;
            }
        }
    }

   
   
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("updateemailwording.aspx?rowid="+Request.QueryString["rowid"].ToString());
    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }
    protected void btn_new_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        btn_save0.Text = "Save";
        txt_templatename.Text = "";
        txt_email.Text = "";
        txt_selectedkey.Text = "";
    }
    protected void btn_save0_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["rowid"] != null)
        {
        if (btn_save0.Text == "Save")
        {
            SQLeExecuteNonQuery("system", "insert into Prosition (WFRowId,PositionName,PositionEmail,SelectedKey,CrBy,udby) values ('" + Request.QueryString["rowid"].ToString() + "','" + txt_templatename.Text.Replace("'", "''") + "','" + txt_email.Text + "','"+txt_selectedkey.Text.Replace("'","''")+"','" + isWho() + "','" + isWho() + "');");

        }
        else
        {
            SQLeExecuteNonQuery("system", "update Prosition set PositionName='" + txt_templatename.Text.Replace("'", "''") + "',PositionEmail='" + txt_email.Text + "',SelectedKey='"+txt_selectedkey.Text+"' where PositionRowId='" + HiddenField1.Value + "'");

        }
        }
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void lnk_edit_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;

        DataSet ds = SQLExecuteReader("system", "select * from Prosition where PositionRowId='" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            txt_templatename.Text = ds.Tables[0].Rows[0]["PositionName"].ToString();
            txt_email.Text = ds.Tables[0].Rows[0]["PositionEmail"].ToString();
            txt_selectedkey.Text = ds.Tables[0].Rows[0]["SelectedKey"].ToString();
            HiddenField1.Value = ds.Tables[0].Rows[0]["PositionRowId"].ToString();
        }
        btn_save0.Text = "Save Edit";
        MultiView1.ActiveViewIndex = 1;
    }
    protected void btn_del_Click(object sender, EventArgs e)
    {
        SQLeExecuteNonQuery("system", "delete from Prosition where PositionRowId='" + HiddenField1.Value + "';");
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
}