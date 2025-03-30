using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class workflowhistory : BST.libDB
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.SetCurentNavigatePage("WorkFlow Ran History");
            bindwf();

            refresh();
        }
    }

    private void bindwf()
    {
        wflist.DataSource = SQLExecuteReader("system", "SELECT WFName, WFRowId FROM  WorkFlow where (case when UserView='' then '" + isWho() + "' else UserView end) like '%" + isWho() + "%' and useredit like '%" + (meonly.Checked ? isWho() : "") + "%'");
        wflist.DataTextField = "WFName";
        wflist.DataValueField = "WFRowId";
        wflist.DataBind();
        wflist.Items.Insert(0, new ListItem("ทั้งหมด", "%"));
        refresh();

    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        Label4.Visible = false;
        DataSet ds = SQLExecuteReader("system", @"SELECT      WorkFlowEventsHistory.rowid,WorkFlowStep_1.StepName, WorkFlowEventsHistory.Event, Prosition.PositionName AS SentEmailTo,  replace(replace(replace(replace(WorkFlowEventsHistory.Email,';;',';'),';',';<br />'),';<br />;',';'),';<br />;',';') as Email,replace(replace(replace(replace(WorkFlowEventsHistory.CCEmail,';;',';'),';',';<br />'),';<br />;',';'),';<br />;',';') as CCEmail, isnull(WorkFlowStep.StepName,'Finished') AS NextStep,WorkFlowEventsHistory.crdate,WorkFlowEventsHistory.crby
                                                    FROM         WorkFlowEventsHistory LEFT OUTER JOIN
                                                                          WorkFlowStep ON CASE WHEN WorkFlowEventsHistory.NextStep = '' THEN newid() ELSE WorkFlowEventsHistory.NextStep END = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                          Prosition ON CASE WHEN WorkFlowEventsHistory.SentEmailTo = '' THEN newid() ELSE WorkFlowEventsHistory.SentEmailTo END = Prosition.PositionRowId LEFT OUTER JOIN
                                                                          WorkFlowStep AS WorkFlowStep_1 ON WorkFlowEventsHistory.StepRowID = WorkFlowStep_1.StepRowId
                                                    where WorkFlowEventsHistory.StartRowID='" + GridView1.SelectedRow.Cells[2].Text.ToString() + @"'
                                                    ORDER BY WorkFlowEventsHistory.CrDate  ");
        Label3.Text = "Runing RowID : " + GridView1.SelectedRow.Cells[2].Text.ToString();
        GridView2.DataSource = ds.Tables[0];
        GridView2.DataBind();

        if (GridView1.DataKeys[GridView1.SelectedIndex].Value.ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
        {
            GridView2.Columns[0].Visible = false;
        }
        else
        {
            GridView2.Columns[0].Visible = true;
        }

    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }
    protected void btn_back1_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        refresh();
        GridView1.SelectedIndex = -1;
    }
    protected void btn_view_Click(object sender, EventArgs e)
    {
        Response.Redirect("WorkflowRuning.aspx?rowid=preview&runingkey=" + GridView1.SelectedRow.Cells[2].Text.ToString());
    }


    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        HiddenField hd = (HiddenField)GridView2.Rows[Row.RowIndex].Cells[0].FindControl("HiddenField1");
        if (hd != null)
        {
            DataSet ds = SQLExecuteReader("system", "select EmailCommandForResent from WorkFlowEventsHistory where RowID='" + hd.Value + "';");
            if (ds.Tables[0].Rows.Count > 0)
            {
                SQLeExecuteNonQuery("LegalMail", ds.Tables[0].Rows[0][0].ToString().Replace("||?", "'").Replace("\" & vbCrLf & \"", ""));
                
                    Label4.Visible = true;
                
               
            }
        }
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        refresh();
        GridView1.PageIndex = e.NewPageIndex;
        GridView1.DataBind();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        refresh();
    }

    private void refresh()
    {

        String fillter = "";

        if (TextBox1.Text != "")
        {

            fillter = " and " + DropDownList1.SelectedValue + " like '%" + TextBox1.Text + "%'";
        }
        else
        {
            fillter = "";
        }

        //   DataSet ds = SQLExecuteReader("system", @"SELECT     WorkFlowRunningStatus.StartRowID,WorkFlowRunningStatus.ETC1, WorkFlow.WFName, case when status='finished' then 'Finished' else WorkFlowStep.StepName end as [Current StepName], WorkFlowRunningStatus.UdBy, WorkFlowRunningStatus.UdDate, WorkFlowRunningStatus.Status
        //                                          FROM         WorkFlowRunningStatus LEFT OUTER JOIN
        //                                                                 WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId LEFT OUTER JOIN
        //                                                                WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId where WorkFlowRunningStatus.WFRowId='" + Request.QueryString["rowid"].ToString() + @"' " + fillter + @" order by WorkFlowRunningStatus.crdate desc
        //                                          ");
        DataSet ds = SQLExecuteReader("system", @"SELECT    WorkFlowRunningStatus.WFRowId, WorkFlowRunningStatus.StartRowID, WorkFlowRunningStatus.ETC1,WorkFlow.WFName, case when status='finished' then 'Finished' else WorkFlowStep.StepName end as [Current StepName], WorkFlowRunningStatus.UdBy, WorkFlowRunningStatus.UdDate, WorkFlowRunningStatus.Status,(case when isnull(ApplicationFolder,'')='' then 'capa' else ApplicationFolder end) as ApplicationFolder,WorkFlow.UserSpecialAction
                                                    FROM         WorkFlowRunningStatus LEFT OUTER JOIN
                                                                          WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                          WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId where 1=1 and (case when UserView='' then '" + isWho() + "' else UserView end) like '%" + isWho() + "%' and  useredit like '%" + (meonly.Checked ? isWho() : "") + @"%' and WorkFlow.WFRowId like '%" + wflist.SelectedValue + @"%' " + fillter + @" order by WorkFlowRunningStatus.crdate desc
                                                    ");

        GridView1.DataSource = ds.Tables[0];
        GridView1.DataBind();

        GridView1.Columns[2].ControlStyle.CssClass = "nna";
        GridView1.Columns[2].HeaderStyle.CssClass = "nna";
        GridView1.Columns[2].ItemStyle.CssClass = "nna";

        if (("chakrapan_a").ToLower().IndexOf(isWho().ToLower()) < 0)
        {
            
           // GridView1.Columns[1].Visible = false;
        }


        Label5.Text = "พบรายการทั้งหมด :  " + ds.Tables[0].Rows.Count.ToString() + " รายการ";

    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        LinkButton lb = (LinkButton)sender;
        if (lb != null)
        {
            String[] sc = lb.ToolTip.Split((",").ToCharArray());
            if (sc.Length == 2)
            {
                SQLeExecuteNonQuery("system", "update  WorkFlowRunningStatus set wfrowid=null,StartRowID=newid(),etc5='Delete(StartRowID=" + sc[0] + ",wfrowid=" + sc[1] + ")',status='Deleted' where StartRowID='" + sc[1] + "'");
                refresh();
            }
        }
    }
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 2;

       GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        HiddenField hd = (HiddenField)GridView2.Rows[Row.RowIndex].Cells[0].FindControl("HiddenField1");
        if (hd != null)
        {
            DataSet ds = SQLExecuteReader("system", @"SELECT     WorkFlowEventsHistory.RowID, WorkFlowEventsHistory.StartRowID, WorkFlowEventsHistory.StepRowID, WorkFlowEventsHistory.Event, WorkFlowEventsHistory.ContentBody, 
                      WorkFlowEventsHistory.SentEmailTo, replace(replace(replace(replace(WorkFlowEventsHistory.Email,';;',';'),';',';'),';;',';'),';;',';') as Email,  replace(replace(replace(replace(WorkFlowEventsHistory.CCEmail,';;',';'),';',';'),';;',';'),';;',';') as CCEmail, WorkFlowEventsHistory.EmailCommandForResent, 
                      WorkFlowEventsHistory.ExecuteCommand, WorkFlowEventsHistory.NextStep, WorkFlowEventsHistory.CrDate, WorkFlowEventsHistory.CrBy, WorkFlowEventsHistory.UdDate, 
                      WorkFlowEventsHistory.UdBy, WorkFlowStep.StepName
FROM         WorkFlowEventsHistory left outer JOIN
                      WorkFlowStep ON convert(nvarchar(100),WorkFlowEventsHistory.NextStep) = convert(nvarchar(100),WorkFlowStep.StepRowId) where WorkFlowEventsHistory.RowID='" + hd.Value + "';");
            if (ds.Tables[0].Rows.Count > 0)
            {
                Label6.Text = "Runing RowID : " + hd.Value.ToString();
                Label15.Text = hd.Value.ToString();

                Label8.Text = (ds.Tables[0].Rows[0]["StepName"].ToString()==""?"Fishished แล้ว ไม่สามารถ Rewind ได้":ds.Tables[0].Rows[0]["StepName"].ToString());
                if (ds.Tables[0].Rows[0]["NextStep"].ToString() == "")
                {
                    Button4.Enabled = false;
                }
                else
                {
                    Button4.Enabled = true;
                }
                TextBox2.Text = ds.Tables[0].Rows[0]["Email"].ToString();

                TextBox3.Text = ds.Tables[0].Rows[0]["CCEmail"].ToString();

                try
                {
                    int start = ds.Tables[0].Rows[0]["EmailCommandForResent"].ToString().IndexOf("@body = N||?");
                    int end = ds.Tables[0].Rows[0]["EmailCommandForResent"].ToString().IndexOf("@subject");

                    Label11.Text = ds.Tables[0].Rows[0]["EmailCommandForResent"].ToString().Substring((start + 12), (end) - (start + 12)).Replace("||?,", "");

                    string email = ds.Tables[0].Rows[0]["Email"].ToString();
                    string ccemail = ds.Tables[0].Rows[0]["CCEmail"].ToString();

                    Label12.Text = ds.Tables[0].Rows[0]["EmailCommandForResent"].ToString();
                    Label13.Text = ds.Tables[0].Rows[0]["Email"].ToString();
                    Label14.Text = ds.Tables[0].Rows[0]["CCEmail"].ToString();
                    Label15.Text = ds.Tables[0].Rows[0]["StartRowID"].ToString();
                }
                catch { Label11.Text = ""; }
            }
            else
            {
                Label11.Text = "";
                Label6.Text = "Runing RowID : ";
            }
        }
        else
        {
            Label6.Text = "Runing RowID : ";
        }
       
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        if (Label13.Text != "")
            Label12.Text = Label12.Text.ToLower().Replace("@recipients = n||?" + Label13.Text.ToLower() + "||?", "@recipients = n||?" + TextBox2.Text + "||?");
        if (Label14.Text != "")
            Label12.Text = Label12.Text.ToLower().Replace("@copy_recipients = n||?" + Label14.Text.ToLower() + "||?", "@copy_recipients = n||?" + TextBox3.Text + "||?");

        Label12.Text = Label12.Text.Replace("n||", "N||");



        DataSet ds = SQLExecuteReader("system", @"set xact_abort on;
                                BEGIN TRAN T1;insert into WorkFlowEventsHistory (StartRowID, StepRowID, Event, ContentBody, SentEmailTo, Email, CCEmail, EmailCommandForResent, ExecuteCommand, NextStep, CrBy, UdBy) 
           SELECT     StartRowID, StepRowID, 'ReWind', ContentBody, SentEmailTo, '" + TextBox2.Text + @"', '" + TextBox3.Text + @"', '" + Label12.Text + "', ExecuteCommand, NextStep, '" + isWho() + @"', '" + isWho() + @"'
            FROM         WorkFlowEventsHistory where RowID='" + Label6.Text.Replace("Runing RowID : ", "") + "';select top 1 * from WorkFlowEventsHistory order by crdate desc;COMMIT TRAN T1;");


        if (ds.Tables[0].Rows.Count > 0)
        {
           Label12.Text= Label12.Text.Replace(Label6.Text.Replace("Runing RowID : ", ""), ds.Tables[0].Rows[0]["rowid"].ToString());

            if(SQLeExecuteNonQuery("system", @"set xact_abort on;
                                BEGIN TRAN T1; update WorkFlowRunningStatus set Status='Active',CurrentStepRowId='" + ds.Tables[0].Rows[0]["NextStep"].ToString() + "' where StartRowID='" + Label15.Text + @"';update WorkFlowEventsHistory set EmailCommandForResent='" + Label12.Text + @"' where rowid='" + ds.Tables[0].Rows[0]["rowid"].ToString() + @"';COMMIT TRAN T1;"))
            {
                SQLeExecuteNonQuery("LegalMail", Label12.Text.ToString().Replace("||?", "'").Replace("\" & vbCrLf & \"", ""));

                MultiView1.ActiveViewIndex = 1;
                Label4.Visible = false;
                DataSet ds2 = SQLExecuteReader("system", @"SELECT      WorkFlowEventsHistory.rowid,WorkFlowStep_1.StepName, WorkFlowEventsHistory.Event, Prosition.PositionName AS SentEmailTo,  replace(replace(WorkFlowEventsHistory.Email,';;',';'),';',';<br />') as Email,replace(replace(WorkFlowEventsHistory.CCEmail,';;',';'),';',';<br />') as CCEmail, isnull(WorkFlowStep.StepName,'Finished') AS NextStep,WorkFlowEventsHistory.crdate,WorkFlowEventsHistory.crby
                                                    FROM         WorkFlowEventsHistory LEFT OUTER JOIN
                                                                          WorkFlowStep ON CASE WHEN WorkFlowEventsHistory.NextStep = '' THEN newid() ELSE WorkFlowEventsHistory.NextStep END = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                          Prosition ON CASE WHEN WorkFlowEventsHistory.SentEmailTo = '' THEN newid() ELSE WorkFlowEventsHistory.SentEmailTo END = Prosition.PositionRowId LEFT OUTER JOIN
                                                                          WorkFlowStep AS WorkFlowStep_1 ON WorkFlowEventsHistory.StepRowID = WorkFlowStep_1.StepRowId
                                                    where WorkFlowEventsHistory.StartRowID='" + GridView1.SelectedRow.Cells[2].Text.ToString() + @"'
                                                    ORDER BY WorkFlowEventsHistory.CrDate  ");
                Label3.Text = "Runing RowID : " + GridView1.SelectedRow.Cells[2].Text.ToString();
                GridView2.DataSource = ds2.Tables[0];
                GridView2.DataBind();
                
                refresh();

            }
 

        }

    }
    protected void meonly_CheckedChanged(object sender, EventArgs e)
    {
        bindwf();
    }
    protected void wflist_SelectedIndexChanged(object sender, EventArgs e)
    {
        refresh();
    }
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView GridView1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Panel p = (Panel)e.Row.FindControl("Panel1");
            if (p != null)
            {
                if (GridView1.DataKeys[e.Row.RowIndex].Value.ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
                {
                    //p.Visible = false;
                    p.Controls.Clear();
                    Literal cc = new Literal();
                    cc.Text = "------ไม่ได้รับสิทธิ Special Action------";
                    p.Controls.Add(cc);
                }
                
            }
        }
        
    }
}