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
            this.SetCurentNavigatePage("Template");

            refresh();
        }

        ClientScript.RegisterStartupScript(this.GetType(), "ware", "document.getElementById('navi').style.display = 'none';document.getElementById('logomain').style.display = 'none';", true);
    }

    private void refresh()
    {
        if (Request.QueryString["rowid"] != null)
        {
            DataSet ds = this.SQLExecuteReader("system", "SELECT TemplateRowId, TemplateName FROM   WorkFlowTemplate    where WFRowId='" + Request.QueryString["rowid"].ToString() + "';select * from WorkFlow where WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();

            lb_wfname.Text = "WorkFlow Name : " + ds.Tables[1].Rows[0]["WFName"].ToString();

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
    protected void Button1_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        getdesign(sender);
    }

    private void getdesign(object sender)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        DataSet ds = SQLExecuteReader("system", "select * from WorkFlowTemplate where TemplateRowId='" + GridView1.Rows[Row.RowIndex].Cells[0].Text+ "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            FreeTextBox1.Text=ds.Tables[0].Rows[0]["TemplateContent"].ToString();
            rowid_txt.Text = GridView1.Rows[Row.RowIndex].Cells[0].Text;
            lb_templatename.Text = "Template Name : " + ds.Tables[0].Rows[0]["TemplateName"].ToString();
            btn_preview.OnClientClick = "window.open('PreviewTemplate.aspx?rowid=" + rowid_txt.Text + "');return false;";
        
        }

    }
    protected void btn_save_Click(object sender, EventArgs e)
    {

        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

       // string ddd = Editor1.XHTML;
        //string ddd1 = Editor1.Text;
        doc.LoadHtml(FreeTextBox1.Text);
        if (doc == null) return;

        rowid_txt0.Text = rowid_txt.Text;
        cb_fieldlist.Items.Clear();
        cb_fieldlistselect.Items.Clear();
        int i = 0;
        if (doc.DocumentNode.SelectNodes("//input") != null)
        {
            
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//input"))
            {
                if (node.Attributes["name"] != null && node.Attributes["id"] != null)
                {
                    if (node.Attributes["name"].ToString() == node.Attributes["id"].ToString())
                    {

                        cb_fieldlist.Items.Add(node.Attributes["name"].Value.ToString());
                        cb_fieldlist.Items[i].Selected = true;
                        i++;
                    }
                    
                }
            }
 

        }

        if (doc.DocumentNode.SelectNodes("//textarea") != null)
        {
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//textarea"))
            {
                if (node.Attributes["name"] != null && node.Attributes["id"] != null)
                {

                    if (node.Attributes["name"].ToString() == node.Attributes["id"].ToString())
                    {
                        cb_fieldlist.Items.Add(node.Attributes["name"].Value.ToString());
                        cb_fieldlist.Items[i].Selected = true;
                        i++;
                    }
                }
            }
        }

        i = 0;
        if (doc.DocumentNode.SelectNodes("//select") != null)
        {

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//select"))
            {
                if (node.Attributes["name"] != null && node.Attributes["id"] != null)
                {
                    if (node.Attributes["name"].ToString() == node.Attributes["id"].ToString())
                    {
                        cb_fieldlistselect.Items.Add(node.Attributes["name"].Value.ToString());
                        cb_fieldlistselect.Items[i].Selected = true;
                        i++;
                    }
                }
            }
        }

        if (SQLeExecuteNonQuery("system", "update WorkFlowTemplate set TemplateName=TemplateName,TemplateContent='" + FreeTextBox1.Text.Replace("'", "''") + "' where TemplateRowId='" + rowid_txt.Text + "'"))
        {

            MultiView1.ActiveViewIndex = 2;
        }
      
    }
    protected void btn_savefield_Click(object sender, EventArgs e)
    {
        String sql = "";
        String objname_type = "";

        if (rowid_txt0.Text != "")
        {
            objname_type = "''";
            for (int i = 0; i < cb_fieldlist.Items.Count; i++)
            {
                if (cb_fieldlist.Items[i].Selected == true)
                {

                    DataSet ds = SQLExecuteReader("system", "select * from TemplateObject where TemplateRowId='" + rowid_txt0.Text + "' and ObjectName='" + cb_fieldlist.Items[i].Value + "' and ObjectType='Textbox';");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        sql += "insert into TemplateObject (TemplateRowId, ObjectName, ObjectType,CrBy, UdBy) values ('" + rowid_txt0.Text + "','" + cb_fieldlist.Items[i].Value + "','Textbox','" + isWho() + "','" + isWho() + "');";
                    }

                    objname_type += ",'"+cb_fieldlist.Items[i].Value + ":Textbox'";
                    if (sql != "")
                    {
                        SQLeExecuteNonQuery("system", sql);
                    }
                }
            }

            for (int i = 0; i < cb_fieldlistselect.Items.Count; i++)
            {
                if (cb_fieldlistselect.Items[i].Selected == true)
                {

                    DataSet ds = SQLExecuteReader("system", "select * from TemplateObject where TemplateRowId='" + rowid_txt0.Text + "' and ObjectName='" + cb_fieldlistselect.Items[i].Value + "' and ObjectType='Select';");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        sql += "insert into TemplateObject (TemplateRowId, ObjectName, ObjectType,CrBy, UdBy) values ('" + rowid_txt0.Text + "','" + cb_fieldlistselect.Items[i].Value + "','Select','" + isWho() + "','" + isWho() + "');";
                    }

                    objname_type += ",'" + cb_fieldlistselect.Items[i].Value + ":Select'";
                    if (sql != "")
                    {
                        SQLeExecuteNonQuery("system", sql);
                    }
                }
            }
             SQLeExecuteNonQuery("system", "delete from TemplateObject where TemplateRowId='" + rowid_txt0.Text + "' and ObjectName+':'+ObjectType not in (" + objname_type + ");");
             MultiView1.ActiveViewIndex = 0;
        }
    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("updateemailwording.aspx?rowid=" + Request.QueryString["rowid"].ToString());
    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }
    protected void btn_new_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 3;
        btn_save0.Text = "Save";
        txt_templatename.Text = "";
    }
    protected void btn_save0_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["rowid"] != null)
        {
        if (btn_save0.Text == "Save")
        {
            SQLeExecuteNonQuery("system", "insert into WorkFlowTemplate (WFRowId,TemplateName,CrBy,udby) values ('" + Request.QueryString["rowid"].ToString() + "','" + txt_templatename.Text.Replace("'", "''") + "','" + isWho() + "','" + isWho() + "');");

        }
        else
        {
            SQLeExecuteNonQuery("system", "update WorkFlowTemplate set TemplateName='" + txt_templatename.Text.Replace("'", "''") + "' where TemplateRowId='" + HiddenField1.Value + "'");

        }
        }
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void lnk_edit_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;

        DataSet ds = SQLExecuteReader("system", "select * from WorkFlowTemplate where TemplateRowId='" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            txt_templatename.Text = ds.Tables[0].Rows[0]["TemplateName"].ToString();
            HiddenField1.Value = ds.Tables[0].Rows[0]["TemplateRowId"].ToString();
        }
        btn_save0.Text = "Save Edit";
        MultiView1.ActiveViewIndex = 3;
    }
    protected void btn_del_Click(object sender, EventArgs e)
    {
        SQLeExecuteNonQuery("system", "delete from TemplateObject where TemplateRowId='" + HiddenField1.Value + "';delete from StepEnableObject where TemplateRowId='" + HiddenField1.Value + "';delete from WorkFlowTemplate where TemplateRowId='" + HiddenField1.Value + "';");
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
}