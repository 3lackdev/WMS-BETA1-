using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class workflowlist : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.SetCurentNavigatePage("WorkFlow");
            if (Session["viewme"] != null)
            {
                meonly.Checked = (Boolean)Session["viewme"];

            }

            refresh();
        }
       
    }
    private void refresh()
    {
        DataSet ds = this.SQLExecuteReader("system", "SELECT  WFRowId, WFName,useredit FROM  WorkFlow where (case when UserView='' then '" + isWho() + "' else UserView end) like '%" + isWho() + "%' and useredit like '%" + (meonly.Checked ? isWho() : "") + "%' order by WFName;");
        if (ds.Tables.Count>0)
        {
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();
        }

        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            if (ds.Tables[0].Rows[i]["useredit"].ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
            {
               LinkButton lb = (LinkButton)GridView1.Rows[i].Cells[1].FindControl("LinkButton1");
               if (lb != null)
               {
                   lb.Visible = false;
                 
               }
            }
        }




        DataSet dss = this.SQLExecuteReader("LegalMail", "select system,system+' (Mail Profile : '+mailprofile+')' as name from " + ServerNotificationDB + ".dbo.SystemAndMailProfile order by system");
        sqlmail.DataSource = dss.Tables[0];
        sqlmail.DataTextField = "name";
        sqlmail.DataValueField = "system";
        sqlmail.DataBind();
        sqlmail.Items.Insert(0, new ListItem("", ""));

    }

    protected void btn_new_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        btn_save.Text = "Save";
        sqlmail.Text = "WEBWF";
        txt_workflowname.Text = "";
        txt_applicationfolder.Text = "";
        txt_useredit.Text = isWho();
        txt_special.Text = isWho();
        txt_sender.Text = "";
        txt_defaultdomain.Text = ServerDomain;
    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (txt_workflowname.Text != "")
        {
            if (btn_save.Text == "Save")
            {
                SQLeExecuteNonQuery("system", "insert into WorkFlow (WFName,CrBy,udby,UserEdit,UserView,SQLMailProfile,ApplicationFolder,UserSpecialAction,SenderDisplay,DefaultDomain) values ('" + txt_workflowname.Text.Replace("'", "''") + "','" + isWho() + "','" + isWho() + "','" + txt_useredit.Text.Replace("'", "''") + "','" + txt_userview.Text.Replace("'", "''") + "','" + sqlmail.SelectedValue + "','" + txt_applicationfolder.Text.Replace("'", "''") + "','" + txt_special.Text.Replace("'", "''") + @"','" + txt_sender.Text.Replace("'", "''") + "','" + txt_defaultdomain.Text + "');");

            }
            else
            {
                SQLeExecuteNonQuery("system", "update WorkFlow set WFName='" + txt_workflowname.Text.Replace("'", "''") + "',UserEdit='" + txt_useredit.Text.Replace("'", "''") + "',UserSpecialAction='" + txt_special.Text.Replace("'", "''") + @"',UserView='" + txt_userview.Text.Replace("'", "''") + "',SQLMailProfile='" + sqlmail.SelectedValue + "',ApplicationFolder='" + txt_applicationfolder.Text.Replace("'", "''") + "',SenderDisplay='" + txt_sender.Text + "',DefaultDomain='" + txt_defaultdomain.Text + "' where WFRowId='" + HiddenField1.Value + "'");

            }
        }
        MultiView1.ActiveViewIndex = 0;
        refresh();

    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;

        DataSet ds = SQLExecuteReader("system", "select * from WorkFlow where WFRowId='" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            txt_workflowname.Text = ds.Tables[0].Rows[0]["WFName"].ToString();
            HiddenField1.Value = ds.Tables[0].Rows[0]["WFRowId"].ToString();
            txt_useredit.Text = ds.Tables[0].Rows[0]["UserEdit"].ToString();
            txt_userview.Text = ds.Tables[0].Rows[0]["UserView"].ToString();
            try
            {
                sqlmail.SelectedValue = ds.Tables[0].Rows[0]["SQLMailProfile"].ToString();
            }
            catch { }
            txt_applicationfolder.Text = ds.Tables[0].Rows[0]["ApplicationFolder"].ToString();
            txt_special.Text = ds.Tables[0].Rows[0]["UserSpecialAction"].ToString();
            txt_sender.Text = ds.Tables[0].Rows[0]["SenderDisplay"].ToString();
            txt_defaultdomain.Text = ds.Tables[0].Rows[0]["defaultdomain"].ToString();
        }
        btn_save.Text = "Save Edit";
        MultiView1.ActiveViewIndex = 1;
    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }
    protected void btn_del_Click(object sender, EventArgs e)
    {
        SQLeExecuteNonQuery("system", "delete from WorkFlow where WFRowId='" + HiddenField1.Value + "';delete from TemplateObject where TemplateRowId in (select TemplateRowId from WorkFlowTemplate where WFRowId='" + HiddenField1.Value + "');delete from StepEnableObject where TemplateRowId in (select TemplateRowId from WorkFlowTemplate where WFRowId='" + HiddenField1.Value + "');delete from WorkFlowTemplate where WFRowId='" + HiddenField1.Value + "';delete from WorkFlowStep where WFRowId='" + HiddenField1.Value + "';delete from prosition where WFRowId='" + HiddenField1.Value + "';");
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 2;
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        emb2.Text = "<iframe frameborder=\"no\" height=\"800\" id=\"workflowiframe\" name=\"workflowiframe\" src=\""+ServerLocalUrl+"/updateemailwording.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "\" width=\"100%\"></iframe>";
        emb.Text = "<iframe frameborder=\"no\" height=\"1400\" id=\"workflowiframe\" name=\"workflowiframe\" src=\""+ServerLocalUrl+"/WorkflowRuning.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "\" width=\"100%\"></iframe>";
        emb0.Text = "<iframe frameborder=\"no\" height=\"800\" id=\"workflowiframe\" name=\"workflowiframe\" src=\"" + ServerLocalUrl + "/ViewRuningHistory.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "\" width=\"100%\"></iframe>";
        HyperLink2.NavigateUrl = ServerLocalUrl+"/WorkflowRuning.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString();
        HyperLink3.NavigateUrl = ServerLocalUrl + "/ViewRuningHistory.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString();
        HyperLink6.NavigateUrl = ServerLocalUrl + "/updateemailwording.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString();
        emb1.Text = ServerLocalUrl + "/WorkflowRuning.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "&btnaction={?}&{objtemplatename}={?}";

        Literal1.Text = "<b>For POST Method</b><br />Action : " + ServerLocalUrl + "/WorkflowRuning.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "&btnaction={?}<br /><br />";
        Literal1.Text += "<b>btnaction</b><br />Save<br />Approve<br />Reject<br /><br /><b>objtemplatename</b><br />";
        DataSet ds = SQLExecuteReader("system", @"SELECT     TemplateObject.ObjectName, StepEnableObject.RequireField
                                                FROM         WorkFlowStep INNER JOIN
                                                                      StepEnableObject ON WorkFlowStep.StepRowId = StepEnableObject.StepRowId AND WorkFlowStep.TemplateRowId = StepEnableObject.TemplateRowId INNER JOIN
                                                                      TemplateObject ON WorkFlowStep.TemplateRowId = TemplateObject.TemplateRowId AND StepEnableObject.ObjectRowId = TemplateObject.ObjectRowId
                                                WHERE     (WorkFlowStep.WFRowId = '" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + @"') AND (WorkFlowStep.DefaultStart = 'Y')");
        for(int i=0;i<ds.Tables[0].Rows.Count;i++)
        {
            Literal1.Text += ds.Tables[0].Rows[i]["ObjectName"].ToString() + (ds.Tables[0].Rows[i]["RequireField"].ToString()=="Y"?" <font color=orange>Require Filed</font>":"") + "<br />";
        }

        Literal2.Text = @"<b>For API JSON</b><br />https://guru.bst.co.th:440/api/{workflowid}/{column}/{where}<br />https://guru.bst.co.th:440/api/{workflowid}/{column}/{where}/{orderby}<br /><br />{workflowid} Require<br />
{column} option<br />
{where} option<br />
{orderby} option<br /><br /><br />


RESTFUL API JSON Format<br /><br />

ตัวอย่าง<br /><br />

https://guru.bst.co.th:440/api/" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + @"<br />
";

    }
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        SQLeExecuteNonQuery("system", "copy_workflow '" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString() + "','"+isWho()+"';");
        refresh();
    }
    protected void meonly_CheckedChanged(object sender, EventArgs e)
    {
        Session["viewme"] = meonly.Checked;
        refresh();
    }


    protected void LinkButton4_Click(object sender, EventArgs e)
    {
        Label8.Text = Label9.Text = "";


        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        MultiView1.ActiveViewIndex = 3;
        HiddenField2.Value = GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString();
        String editcan = GridView1.DataKeys[Row.RowIndex].Value.ToString();

        btn_save1.Visible = btn_save0.Visible= (editcan.ToLower().IndexOf(isWho().ToLower()) >= 0);
       

        HyperLink4.NavigateUrl = "ViewRuningHistory.aspx?rowid=" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString();

        ck_View.Checked=true;
        ck_history.Checked=true;
        ck_spacialaction.Items[0].Selected=true;
        ck_spacialaction.Items[1].Selected = false;
         ck_wfname.Checked=true;
        ck_udby.Checked=true;
         ck_uddate.Checked=true;
         ck_flowstatus.Checked = true;
         ck_currentstepname.Checked = true;
         txt_View_width.Text = "";
         txt_history_width.Text = "";
         txt_spacialaction_width.Text = "";
         txt_wfname_width.Text = "";
         txt_udby_width.Text = "";
         txt_uddate_width.Text = "";
         txt_flowstatus_width.Text = "";
         txt_currentstepname_width.Text = "";

        DataSet ds = SQLExecuteReader("system", @"SELECT top 1 * from WorkFlow where wfrowid='" + GridView1.Rows[Row.RowIndex].Cells[0].Text.ToString()+"'");

        if (ds.Tables[0].Rows.Count > 0)
        {
            try
            {
              

                String config = ds.Tables[0].Rows[0]["HisDisplayStdConfig"].ToString();
                FilterHistory.Text = ds.Tables[0].Rows[0]["FilterHistory"].ToString();

                if (config != "")
                {

                    String[] part = splitpart(config, ";");

                    for (int i = 0; i < part.Length; i++)
                    {
                        if (part[i].ToString() != "" && part[i].IndexOf("spacialaction") < 0)
                        {
                            String[] l = splitpart(part[i].Replace("(hide)", ""), ":");
                            CheckBox cb = (CheckBox)View4.FindControl("ck_" + l[0]);
                            if (part[i].IndexOf("(hide)") > 0)
                            {
                                cb.Checked = false;
                            }
                            TextBox txt = (TextBox)View4.FindControl("txt_" + l[0] + "_width");
                            if (txt != null)
                            {
                                txt.Text = l[1];
                            }

                        }
                        else if (part[i].IndexOf("spacialaction(hide)")>= 0)
                        {
                            ck_spacialaction.Items[0].Selected = false;
                            ck_spacialaction.Items[1].Selected = false;
                        }
                        else if (part[i].IndexOf("spacialaction(full)") >= 0)
                        {
                            ck_spacialaction.Items[0].Selected = false;
                            ck_spacialaction.Items[1].Selected = true;
                        }
                        else if (part[i].IndexOf("spacialaction") >= 0)
                        {
                            ck_spacialaction.Items[0].Selected = true;
                            ck_spacialaction.Items[1].Selected = false;
                        }

                    }

                }
            }
            catch { }


            try
            {

                for (int i = 1; i < 11; i++)
                {
                    String c = ds.Tables[0].Rows[0]["HisDisplayConfigETC"+i.ToString()].ToString();
                    if (c.IndexOf(";") >= 0)
                    {
                        String[] sp = splitpart(c, ";");
                        TextBox txt = (TextBox)View4.FindControl("txt_etc"+i.ToString());
                        CheckBox cb = (CheckBox)View4.FindControl("ck_etc" + i.ToString());
                        TextBox txtw = (TextBox)View4.FindControl("txt_width" + i.ToString());
                        txt.Text = sp[0];
                        cb.Checked = (sp[1] == "N" ? false : true);
                        txtw.Text = sp[2];
                    }


                }

            }
            catch { }
        }

    }


    private string[] splitpart(String config,String code)
    {
        return config.Split(code.ToCharArray());
    }
    protected void btn_save0_Click(object sender, EventArgs e)
    {

        String sql = "update WorkFlow set FilterHistory='" + FilterHistory.Text.Replace("'","''") + "',udby='" + isWho() + "',uddate=getdate()";


        sql += @",HisDisplayStdConfig='" + (ck_View.Checked ? "View" : "View(hide)") + ":" + txt_View_width.Text +
                                    @";" + (ck_history.Checked ? "history" : "history(hide)") + ":" + txt_history_width.Text +
                                    @";" + (ck_spacialaction.SelectedValue == "Full" ? "spacialaction(full)" : (ck_spacialaction.SelectedValue == "Standard" ? "spacialaction" : "spacialaction(hide)")) + ":" + txt_spacialaction_width.Text +
                                    @";:;:;" + (ck_wfname.Checked ? "wfname" : "wfname(hide)") + ":" + txt_wfname_width.Text +
                                    @";" + (ck_currentstepname.Checked ? "currentstepname" : "currentstepname(hide)") + ":" + txt_currentstepname_width.Text +
                                    @";" + (ck_udby.Checked ? "udby" : "udby(hide)") + ":" + txt_udby_width.Text +
                                    @";" + (ck_uddate.Checked ? "uddate" : "uddate(hide)") + ":" + txt_uddate_width.Text +
                                    @";" + (ck_flowstatus.Checked ? "flowstatus" : "flowstatus(hide)") + ":" + txt_flowstatus_width.Text + "'";
           


        

        for (int i = 1; i < 11; i++)
        {

            TextBox txt = (TextBox)View4.FindControl("txt_etc" + i.ToString());
            CheckBox cb = (CheckBox)View4.FindControl("ck_etc" + i.ToString());
            TextBox txtw = (TextBox)View4.FindControl("txt_width" + i.ToString());
            if (txt != null & cb != null & txtw != null)
            {
                sql += ",HisDisplayConfigETC" + i.ToString() + "='" + (txt.Text + ";" + (cb.Checked ? "Y" : "N") + ";" + txtw.Text) + "'";
            }

        }
       // Label8.Text = sql + " where wfrowid='" + HiddenField2.Value + "'";
        if (SQLeExecuteNonQuery("system", sql + " where wfrowid='" + HiddenField2.Value + "'"))
        {

            Label8.Text = Label9.Text = "Save completed";
           // LinkButton4_Click(this, new EventArgs());
        }
        else
        {
            Label8.Text = Label9.Text = "Error!!!!!!";
        }
    }
}