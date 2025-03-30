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

            this.SetCurentNavigatePage("Template");

            refresh();
        }
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
                      //  bt.Visible = false;
                        btn_save.Visible = false;
                    }

                }

                btn_new.Visible = false;
            }

            if (Request.QueryString["action"] != null)
            {
                getdesign(Request.QueryString["action"].ToString());
                return;
            }
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        getdesign(GridView1.Rows[Row.RowIndex].Cells[0].Text);
    }

    private void getdesign(String templateid)
    {
        MultiView1.ActiveViewIndex = 1;
        DataSet ds = SQLExecuteReader("system", "select * from WorkFlowTemplate where TemplateRowId='" + templateid + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {

            FreeTextBox1.Text=ds.Tables[0].Rows[0]["TemplateContent"].ToString();
            rowid_txt.Text = templateid;
            lb_templatename.Text = "Template Name : " + ds.Tables[0].Rows[0]["TemplateName"].ToString();
            btn_preview.OnClientClick = "window.open('PreviewTemplate.aspx?rowid=" + rowid_txt.Text + "');return false;";

            try
            {
                DropDownList1.SelectedIndex = 0;
                DropDownList2.SelectedIndex = 0;
                string additional = "";
                if (ds.Tables[0].Rows[0]["CssLib"].ToString() != "")
                {

                    DropDownList1.SelectedValue = ds.Tables[0].Rows[0]["CssLib"].ToString();
                    switch (ds.Tables[0].Rows[0]["CssLib"].ToString())
                    {

                        case "Boostrap 3.4.1":
                            additional = additional + @"<link href=""bootstrap3/css/bootstrap.min.css"" rel=""stylesheet"" type=""text/css"" />" + System.Environment.NewLine + @"
                                                    <script src=""bootstrap3/js/bootstrap.min.js""></script>";
                            break;
                        case "Boostrap 4.0.0":
                            additional = additional + @"<link href=""bootstrap4/css/bootstrap.min.css"" rel=""stylesheet"" type=""text/css"" />" + System.Environment.NewLine + @"
                                                        <script src=""bootstrap4/js/bootstrap.min.js""></script>";
                            break;
                    }
                }
                
                if (ds.Tables[0].Rows[0]["JsLib"].ToString() != "")
                {

                    DropDownList2.SelectedValue = ds.Tables[0].Rows[0]["JsLib"].ToString();
                     switch (ds.Tables[0].Rows[0]["JsLib"].ToString())
                    {
                        case "HtmlEditor":

                            additional = additional + @"<link href=""bootstrap3/css/bootstrap.min.css"" rel=""stylesheet"" type=""text/css"" />"+System.Environment.NewLine+@"
                                                        <script src=""bootstrap3/js/bootstrap.min.js""></script>" + System.Environment.NewLine + @"
                                                        <link href=""summernote/summernote.min.css"" rel=""stylesheet"">" + System.Environment.NewLine + @"
                                                        <script src=""summernote/summernote.min.js""></script>";
                        break;
                    }
                }


                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(FreeTextBox1.Text);
                if (doc.DocumentNode.SelectNodes("//additional") != null)
                {
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//additional"))
                    {
                        node.Remove();
                    }
                }
                
                 FreeTextBox1.Text=doc.DocumentNode.OuterHtml;
                 FreeTextBox1.Text = FreeTextBox1.Text + System.Environment.NewLine +"<additional><!-- do not delete -->" + additional + "</additional>";

            }
            catch (Exception ex)
            {

            }
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



        if (SQLeExecuteNonQuery("system", "update WorkFlowTemplate set TemplateName=TemplateName,TemplateContent='" + FreeTextBox1.Text.Replace("'", "''") + "',CssLib='" + DropDownList1.SelectedValue + "',JsLib='" + DropDownList2.SelectedValue + "' where TemplateRowId='" + rowid_txt.Text + "'"))
        {
            if (((Button)sender).ID == "btn_save")
            {
                MultiView1.ActiveViewIndex = 2;
            }
            else
            {
                String st1 = @"<script language='javascript' type='text/javascript'>

                                            try
{
                                            Sys.Application.add_load(func);
                                            function func() {
                                           Sys.Application.remove_load(func);

                                        alert('Save Only Template Completed');
                                 } 
}
catch(e)
{}
                                            </script>";

                ScriptManager.RegisterClientScriptBlock(this, GetType(), "error", st1.ToString(), false);
            }
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
                  //  if (sql != "")
                   // {
                   //     SQLeExecuteNonQuery("system", sql);
                   // }
                }
            }

            if (sql != "")
            {
                SQLeExecuteNonQuery("system", sql);
            }

             SQLeExecuteNonQuery("system", "delete from TemplateObject where TemplateRowId='" + rowid_txt0.Text + "' and ObjectName+':'+ObjectType not in (" + objname_type + ");");
             MultiView1.ActiveViewIndex = 0;
        }
    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("workflowlist.aspx");
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
    protected void DropDownList_includeSave(object sender, EventArgs e)
    {

        if (SQLeExecuteNonQuery("system", "update WorkFlowTemplate set CssLib='" + DropDownList1.SelectedValue + "',JsLib='" + DropDownList2.SelectedValue + "' where TemplateRowId='" + rowid_txt.Text + "'"))
        {
            Response.Redirect("template.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&action=" + rowid_txt.Text);

        }
    }
}