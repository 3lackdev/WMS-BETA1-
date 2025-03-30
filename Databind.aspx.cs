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
            
            this.SetCurentNavigatePage("Data Bind");

            refresh();
        }
    }

    private void refresh()
    {

        DataSet ds = this.SQLExecuteReader("system", "SELECT   BindRowID, BindName, Server, Sql, DataFieldText, DataFieldValue,crby FROM DataBindValue where BindName like '%"+txt_tt.Text+"%'");
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                if (ds.Tables[0].Rows[i]["crby"].ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
                {
                    LinkButton lb = (LinkButton)GridView1.Rows[i].Cells[1].FindControl("LinkButton1");
                    if (lb != null)
                    {
                        lb.Visible = false;
                    }

                }
              
            }
    }

   
   
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }
    protected void btn_new_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        btn_save0.Text = "Save";
        txt_BindName.Text = "";
        txt_Sql.Text = "";
        txt_DataFieldValue.Text = "";
        txt_DataFieldText.Text = "";

        dl_Server.Items.Clear();
        for (int i = 0; i < System.Web.Configuration.WebConfigurationManager.ConnectionStrings.Count; i++)
        {
            dl_Server.Items.Add(new ListItem(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString(), System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString()));

        }
    }
    protected void btn_save0_Click(object sender, EventArgs e)
    {
      

        if (btn_save0.Text == "Save")
        {
            SQLeExecuteNonQuery("system", "insert into DataBindValue ( BindName, Server, Sql, DataFieldText, DataFieldValue,CrBy,udby) values ('" + txt_BindName.Text + "','" + dl_Server.SelectedValue + "','" + txt_Sql.Text.Replace("'","''") + "','" + txt_DataFieldText.Text + "','" + txt_DataFieldValue.Text + "','" + isWho() + "','" + isWho() + "');");

        }
        else
        {
            SQLeExecuteNonQuery("system", "update DataBindValue set BindName='" + txt_BindName.Text.Replace("'", "''") + "',Server='" + dl_Server.SelectedValue + "',DataFieldText='" + txt_DataFieldText.Text + "', DataFieldValue='" + txt_DataFieldValue.Text + "',Sql='" + txt_Sql.Text.Replace("'", "''") + "' where BindRowID='" + HiddenField1.Value + "'");

        }


        error.Visible = false;
        error.Text = "";
        string sql = txt_Sql.Text;
        DataSet ds2 = new DataSet();

        try
        {
            if (dl_Server.SelectedValue.IndexOf("ORA_") == 0)
            {

                serviceoracle.Service1 sv = new serviceoracle.Service1();
                sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dl_Server.SelectedValue].ToString(), "select 1 from dual");
                String cc = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dl_Server.SelectedValue].ToString();
                ds2 = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dl_Server.SelectedValue].ToString(), sql);

                if (ds2.Tables[0].DefaultView.ToTable().Rows[0][0].ToString().ToLower().IndexOf("ora-") >= 0)
                {
                    error.Text = ds2.Tables[0].DefaultView.ToTable().Rows[0][0].ToString();
                    error.Visible = true;
                    return;
                }
            }
            else
            {
                ds2 = SQLExecuteReader(dl_Server.SelectedValue, sql);
            }


            DataTable dt = ds2.Tables[0].DefaultView.ToTable();


            MultiView1.ActiveViewIndex = 0;
            refresh();


        }
        catch (Exception ex)
        {
            error.Visible = true;
            error.Text = ex.Message;
            return;
        }



    }
    protected void lnk_edit_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        dl_Server.Items.Clear();
        for (int i = 0; i < System.Web.Configuration.WebConfigurationManager.ConnectionStrings.Count; i++)
        {
            dl_Server.Items.Add(new ListItem(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString(), System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString()));

        }
        DataSet ds = SQLExecuteReader("system", "select * from DataBindValue where BindRowID='" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            txt_BindName.Text = ds.Tables[0].Rows[0]["BindName"].ToString();
            txt_Sql.Text = ds.Tables[0].Rows[0]["Sql"].ToString();
            dl_Server.SelectedValue = ds.Tables[0].Rows[0]["Server"].ToString();
            txt_DataFieldText.Text = ds.Tables[0].Rows[0]["DataFieldText"].ToString();
            txt_DataFieldValue.Text = ds.Tables[0].Rows[0]["DataFieldValue"].ToString();
            HiddenField1.Value = ds.Tables[0].Rows[0]["BindRowID"].ToString();
        }
        btn_save0.Text = "Save Edit";
        MultiView1.ActiveViewIndex = 1;
    }
    protected void btn_del_Click(object sender, EventArgs e)
    {
        SQLeExecuteNonQuery("system", "delete from DataBindValue where BindRowID='" + HiddenField1.Value + "';");
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        refresh();
    }
}