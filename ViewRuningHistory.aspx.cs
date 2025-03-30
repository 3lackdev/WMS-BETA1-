using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ViewRuningHistory : BST.libDB
{
    private string tmpCategoryName = "";
    private string tmpFillterHistory = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DropDownList2.SelectedValue = DateTime.Now.Year.ToString();
            //########### check login
            if (Request.QueryString["username"] != null && Session["loginname"] == null)
            {
                Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "");
                Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "");
            }
            else if (Session["loginname"] == null && this.Page.Request.Url.ToString().IndexOf("rowid") > 0)
            {
                Response.Redirect("http://172.20.128.10:8080/checkauthen/default.aspx?page=" + this.Page.Request.Url.ToString().Replace("&", ";and"));
            }
            // Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "");

            //###################

            if (Request.QueryString["DropDownList1"] != null)
            {
                try
                {
                    DropDownList1.SelectedValue = Request.QueryString["DropDownList1"].ToString();
                }
                catch { }
            }
            if (Request.QueryString["TextBox1"] != null)
            {
                TextBox1.Text = Request.QueryString["TextBox1"].ToString();
            }

            try
            {
                refresh();
            }
            catch (Exception ex)
            {
                SendEmail("chakrapan_a@bst.co.th;chadsada_t@bst.co.th;", null, "!!!WEBDB ERROR", "<br /><br />" + ex.Message.ToString() + "<br /><br />Check link <a href='https://guru.bst.co.th:440/workflowlist.aspx'>Check WF</a>", "WF-Guru440 Workflow <chakrapan_a@bst.co.th>");
                Response.Clear();
                Response.Write(@"
 Page not responding... please try again later or inform IT in case urgent.
ขณะนี้ระบบมีการประมวณผลในปริมาณที่มาก กรุณาทำรายการอีกครั้งภายหลัง.. หรือแจ้ง IT ให้ตรวจสอบกรณีเร่งด่วน");
                Response.End();
            }

        }
    }

    private void refresh()
    {
        if (Request.QueryString["rowid"] != null)
        {
            String fillter = "";


            randerConfigDisplay();

            if (tmpFillterHistory != "" && !showall.Checked)
            {
                fillter = " and " + tmpFillterHistory.Replace("{sys_username}", isWho());
            }

            /*
            if (Request.QueryString["ETC2"] != null)
            {
                try
                {
                    GridView1.Columns[5].Visible = true;
                    GridView1.Columns[5].HeaderText = Request.QueryString["ETC2"].ToString();
                    DropDownList1.Items.Add(new ListItem(Request.QueryString["ETC2"].ToString(), "ETC2"));
                }
                catch (Exception ex) { }
            }
            if (Request.QueryString["ETC3"] != null)
            {
                try
                {
                    GridView1.Columns[6].Visible = true;
                    GridView1.Columns[6].HeaderText = Request.QueryString["ETC3"].ToString();
                    DropDownList1.Items.Add(new ListItem(Request.QueryString["ETC3"].ToString(), "ETC3"));
                }
                catch (Exception ex) { }
            }
            if (Request.QueryString["ETC4"] != null)
            {
                try
                {
                    GridView1.Columns[7].Visible = true;
                    GridView1.Columns[7].HeaderText = Request.QueryString["ETC4"].ToString();
                    DropDownList1.Items.Add(new ListItem(Request.QueryString["ETC4"].ToString(), "ETC4"));
                }
                catch (Exception ex) { }
            }
            if (Request.QueryString["ETC5"] != null)
            {
                try
                {
                    GridView1.Columns[8].Visible = true;
                    GridView1.Columns[8].HeaderText = Request.QueryString["ETC5"].ToString();
                    DropDownList1.Items.Add(new ListItem(Request.QueryString["ETC5"].ToString(), "ETC5"));
                }
                catch (Exception ex) { }
            }


            if (Request.QueryString["WFName"] != null)
            {
                try
                {
                    if (Request.QueryString["WFName"].ToString() == "hide")
                    {
                        GridView1.Columns[9].Visible = false;
                    }

                }
                catch (Exception ex) { }
            }
            if (Request.QueryString["Current StepName"] != null)
            {
                try
                {
                    if (Request.QueryString["Current StepName"].ToString() == "hide")
                    {
                        GridView1.Columns[10].Visible = false;
                    }

                }
                catch (Exception ex) { }
            }

            if (Request.QueryString["UdBy"] != null)
            {
                try
                {
                    if (Request.QueryString["UdBy"].ToString() == "hide")
                    {
                        GridView1.Columns[11].Visible = false;
                    }

                }
                catch (Exception ex) { }
            }

           

            if (Request.QueryString["UdDate"] != null)
            {
                try
                {
                    if (Request.QueryString["UdDate"].ToString() == "hide")
                    {
                        GridView1.Columns[12].Visible = false;
                    }

                }
                catch (Exception ex) { }
            }
            if (Request.QueryString["Status"] != null)
            {
                try
                {
                    if (Request.QueryString["Status"].ToString() == "hide")
                    {
                        GridView1.Columns[13].Visible = false;
                    }

                }
                catch (Exception ex) { }
            }
            */
            if (Request.QueryString["where"] != null)
            {
                try
                {
                    fillter += " and " + Request.QueryString["where"].ToString();

                }
                catch (Exception ex) { }
            }

            else
            {
                fillter += "";
            }




            if (TextBox1.Text != "")
            {

                fillter += " and " + DropDownList1.SelectedValue + " like '%" + TextBox1.Text + "%'";
            }



            try
            {

                /***********************************
                 * Edit by Phon_y
                 * 2022-05-10
                 * add colum Workflow.UserEdit
                 * ***********************************
                 */
                DataSet ds = new DataSet();
                if (Request.QueryString["rowid"].ToString().Replace(",", "','") == "b4a1ab65-919c-4324-a938-23e6aac734fb")
                {

                    ds = SQLExecuteReader("system", @"SELECT DISTINCT WorkFlowRunningStatus.WFRowId,WorkFlowRunningStatus.StartRowID
                                            ,WorkFlowRunningStatus.ETC1,WorkFlowRunningStatus.ETC2,WorkFlowRunningStatus.ETC3
                                            ,WorkFlowRunningStatus.ETC4, WorkFlowRunningStatus.ETC5, WorkFlowRunningStatus.ETC6
                                            , WorkFlowRunningStatus.ETC7, WorkFlowRunningStatus.ETC8, WorkFlowRunningStatus.ETC9
                                            , WorkFlowRunningStatus.ETC10,WorkFlow.WFName,
                                            case when status='finished' then 'Finished'
                                            else (case when status='Hold' then 'Hold' else WorkFlowStep.StepName end) end as [Current StepName],
                                            case when charindex('udby(hide)',WorkFlow.HisDisplayStdConfig)>0 then ''
                                            else (select top 1 replace(Email,';','; ') from WorkFlowEventsHistory
                                            where StartRowID=WorkFlowRunningStatus.StartRowID order by crdate desc) end as udby,
                                            WorkFlowRunningStatus.UdDate,
                                            WorkFlowRunningStatus.status as [Status]
                                            ,(case when isnull(ApplicationFolder,'')='' then 'capa'
                                            else ApplicationFolder end) as ApplicationFolder
                                            ,WorkFlow.UserEdit,WorkFlow.UserSpecialAction,WorkFlow.HisDisplayStdConfig
                                            ,WorkFlowRunningStatus.crdate
                                            FROM WorkFlowRunningStatus
                                            LEFT OUTER JOIN WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId
                                            LEFT OUTER JOIN WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId
                                            LEFT JOIN TemplateInputCurrentValue ON TemplateInputCurrentValue.WFRowID = 'b4a1ab65-919c-4324-a938-23e6aac734fb'
                                            AND TemplateInputCurrentValue.StartRowID = WorkFlowRunningStatus.StartRowID
                                            where WorkFlowRunningStatus.ParentStartRowID is null and year(WorkFlowRunningStatus.crdate)='" + DropDownList2.SelectedValue + @"'
                                            and WorkFlowRunningStatus.WFRowId in ('b4a1ab65-919c-4324-a938-23e6aac734fb')
                                            AND ((TemplateInputCurrentValue.ObjectName='Auditee' AND TemplateInputCurrentValue.Value like '" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='ResponsibleDivApprove' AND TemplateInputCurrentValue.Value like '" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='apprDepartment' AND TemplateInputCurrentValue.Value like '" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='LeadAuditor' AND TemplateInputCurrentValue.Value like '" + Session["loginname"].ToString() + @"%')
                                            OR (WorkFlow.UserSpecialAction like '%" + Session["loginname"].ToString() + @"%'))
                                            AND WorkFlowRunningStatus.ParentStartRowID is null and year(WorkFlowRunningStatus.crdate)='" + DropDownList2.SelectedValue + "' and WorkFlowRunningStatus.WFRowId in ('b4a1ab65-919c-4324-a938-23e6aac734fb') " + fillter + @"
                                            order by WorkFlowRunningStatus.crdate desc");


                }
                else if (Request.QueryString["rowid"].ToString().Replace(",", "','") == "1c3d61f2-7895-4386-bfea-4e47044164b5")
                {

                    ds = SQLExecuteReader("system", @"SELECT DISTINCT WorkFlowRunningStatus.WFRowId,WorkFlowRunningStatus.StartRowID
                                            ,WorkFlowRunningStatus.ETC1,WorkFlowRunningStatus.ETC2,WorkFlowRunningStatus.ETC3
                                            ,WorkFlowRunningStatus.ETC4, WorkFlowRunningStatus.ETC5, WorkFlowRunningStatus.ETC6
                                            , WorkFlowRunningStatus.ETC7, WorkFlowRunningStatus.ETC8, WorkFlowRunningStatus.ETC9
                                            , WorkFlowRunningStatus.ETC10,WorkFlow.WFName,
                                            case when status='finished' then 'Finished'
                                            else (case when status='Hold' then 'Hold' else WorkFlowStep.StepName end) end as [Current StepName],
                                            case when charindex('udby(hide)',WorkFlow.HisDisplayStdConfig)>0 then ''
                                            else (select top 1 replace(Email,';','; ') from WorkFlowEventsHistory
                                            where StartRowID=WorkFlowRunningStatus.StartRowID order by crdate desc) end as udby,
                                            WorkFlowRunningStatus.UdDate,
                                            WorkFlowRunningStatus.status as [Status]
                                            ,(case when isnull(ApplicationFolder,'')='' then 'capa'
                                            else ApplicationFolder end) as ApplicationFolder
                                            ,WorkFlow.UserEdit,WorkFlow.UserSpecialAction,WorkFlow.HisDisplayStdConfig
                                            ,WorkFlowRunningStatus.crdate
                                            FROM WorkFlowRunningStatus
                                            LEFT OUTER JOIN WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId
                                            LEFT OUTER JOIN WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId
                                            LEFT JOIN TemplateInputCurrentValue ON TemplateInputCurrentValue.WFRowID = '1c3d61f2-7895-4386-bfea-4e47044164b5'
                                            AND TemplateInputCurrentValue.StartRowID = WorkFlowRunningStatus.StartRowID
                                            where WorkFlowRunningStatus.ParentStartRowID is null and year(WorkFlowRunningStatus.crdate)='" + DropDownList2.SelectedValue + @"'
                                            and WorkFlowRunningStatus.WFRowId in ('1c3d61f2-7895-4386-bfea-4e47044164b5')
                                            AND ((TemplateInputCurrentValue.ObjectName='Auditee' AND TemplateInputCurrentValue.Value like '%" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='DivisionMgrBy' AND TemplateInputCurrentValue.Value like '%" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='DeptMgrBy' AND TemplateInputCurrentValue.Value like '%" + Session["loginname"].ToString() + @"%')
                                            OR (TemplateInputCurrentValue.ObjectName='LeadAuditor' AND TemplateInputCurrentValue.Value like '%" + Session["loginname"].ToString() + @"%')
                                            OR (WorkFlow.UserSpecialAction like '%" + Session["loginname"].ToString() + @"%'))
                                            AND WorkFlowRunningStatus.ParentStartRowID is null and year(WorkFlowRunningStatus.crdate)='" + DropDownList2.SelectedValue + "' and WorkFlowRunningStatus.WFRowId in ('" + Request.QueryString["rowid"].ToString().Replace(",", "','") + @"') " + fillter + @"
                                            order by WorkFlowRunningStatus.crdate desc");

                }
                else
                {
                    ds = SQLExecuteReader("system", @"SELECT     WorkFlowRunningStatus.WFRowId,WorkFlowRunningStatus.StartRowID,WorkFlowRunningStatus.ETC1,WorkFlowRunningStatus.ETC2,WorkFlowRunningStatus.ETC3,WorkFlowRunningStatus.ETC4, WorkFlowRunningStatus.ETC5, WorkFlowRunningStatus.ETC6, WorkFlowRunningStatus.ETC7, WorkFlowRunningStatus.ETC8, WorkFlowRunningStatus.ETC9, WorkFlowRunningStatus.ETC10,WorkFlow.WFName, 
                                                                    --case when status='finished' then (case when (select count(*) from WorkFlowRunningStatus a where ParentStartRowID=WorkFlowRunningStatus.StartRowID and status<>'finished' and ParentStartRowID is not null)>0 then 'Finished *' else 'Finished' end ) else (case when status='Hold' then 'Hold' else WorkFlowStep.StepName end) end as [Current StepName], 
																	case when status='finished' then  'Finished' else (case when status='Hold' then 'Hold' else WorkFlowStep.StepName end) end as [Current StepName],
																	case when charindex('udby(hide)',WorkFlow.HisDisplayStdConfig)>0 then '' else (select top 1 replace(Email,';','; ') from WorkFlowEventsHistory where StartRowID=WorkFlowRunningStatus.StartRowID order by crdate desc) end as udby, 
                                                                     WorkFlowRunningStatus.UdDate, 
--convert(nvarchar,WorkFlowRunningStatus.Status)+case when (select count(*) from WorkFlowRunningStatus a where a.ParentStartRowID=WorkFlowRunningStatus.StartRowID and a.status<>'finished' and WorkFlowRunningStatus.status='finished' and ParentStartRowID is not null)>0 then ' *' else '' end as [status]
WorkFlowRunningStatus.status as [Status]
,(case when isnull(ApplicationFolder,'')='' then 'capa' else ApplicationFolder end) as ApplicationFolder,WorkFlow.UserEdit,WorkFlow.UserSpecialAction,WorkFlow.HisDisplayStdConfig
                                                    FROM         WorkFlowRunningStatus LEFT OUTER JOIN
                                                                          WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                          WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId where WorkFlowRunningStatus.ParentStartRowID is null and year(WorkFlowRunningStatus.crdate)='" + DropDownList2.SelectedValue + "' and WorkFlowRunningStatus.WFRowId in ('" + Request.QueryString["rowid"].ToString().Replace(",", "','") + @"') " + fillter + @" order by WorkFlowRunningStatus.crdate desc
                                                    ");
                }


                /*try
                {
                    if (Request.QueryString["distinct"] != null)
                    {
                        DataView view = new DataView(ds.Tables[0]);
                        GridView1.DataSource = view.ToTable(true, Request.QueryString["distinct"].ToString());
                                GridView1.DataBind();
                    }
                    else
                    {
                                GridView1.DataSource = ds.Tables[0];
                                GridView1.DataBind();
                    }
                }
                catch (Exception ex)
                        {*/
                GridView1.DataSource = ds.Tables[0];
                GridView1.DataBind();
                //}




                GridView1.Columns[3].ControlStyle.CssClass = "nna";
                GridView1.Columns[3].HeaderStyle.CssClass = "nna";
                GridView1.Columns[3].ItemStyle.CssClass = "nna";

                if (("chakrapan_a;kate_p;bundit_l;orrathai_c").ToLower().IndexOf(isWho().ToLower()) < 0)
                {
                    //  GridView1.Columns[1].Visible = false;
                }


                Label5.Text = "พบรายการทั้งหมด :  " + ds.Tables[0].Rows.Count.ToString() + " รายการ";
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        Label4.Visible = false;
        DataSet ds = SQLExecuteReader("system", @"SELECT        WorkFlowEventsHistory.RowID, WorkFlowStep_1.StepName, WorkFlowEventsHistory.Event, Prosition.PositionName AS SentEmailTo, REPLACE(REPLACE(REPLACE(REPLACE(WorkFlowEventsHistory.Email, ';;', 
                                                                         ';'), ';', ';<br />'), ';<br />;', ';'), ';<br />;', ';') AS Email, REPLACE(REPLACE(REPLACE(REPLACE(WorkFlowEventsHistory.CCEmail, ';;', ';'), ';', ';<br />'), ';<br />;', ';'), ';<br />;', ';') AS CCEmail, 
                                                                         (CASE WHEN WorkFlowEventsHistory.NextStep = '9999C689-4C15-445F-9CBB-B0788A0C920A' THEN 'HOLD' ELSE isnull(WorkFlowStep.StepName, 'Finished') END) AS NextStep, WorkFlowEventsHistory.CrDate, 
                                                                         WorkFlowEventsHistory.CrBy, WorkFlowStep_1.GroupName+ case when WorkFlowRunningStatus.ParentStartRowID is not null then '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;('+ convert(nvarchar(50),WorkFlowRunningStatus.StartRowID) +')' else '' end as GroupName,WorkFlowStep_1.DefaultStart,WorkFlowRunningStatus.StartRowID
                                                FROM            WorkFlowEventsHistory LEFT OUTER JOIN
                                                                         WorkFlowRunningStatus ON WorkFlowEventsHistory.StartRowID = WorkFlowRunningStatus.StartRowID LEFT OUTER JOIN
                                                                         WorkFlowStep ON CASE WHEN WorkFlowEventsHistory.NextStep = '' THEN newid() ELSE WorkFlowEventsHistory.NextStep END = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                         Prosition ON CASE WHEN WorkFlowEventsHistory.SentEmailTo = '' THEN newid() ELSE WorkFlowEventsHistory.SentEmailTo END = Prosition.PositionRowId LEFT OUTER JOIN
                                                                         WorkFlowStep AS WorkFlowStep_1 ON WorkFlowEventsHistory.StepRowID = WorkFlowStep_1.StepRowId
                                                WHERE        (WorkFlowEventsHistory.StartRowID = '" + GridView1.SelectedRow.Cells[3].Text.ToString() + @"') OR
                                                                         (WorkFlowRunningStatus.ParentStartRowID = '" + GridView1.SelectedRow.Cells[3].Text.ToString() + @"')
                                                ORDER BY isnull(WorkFlowStep_1.GroupName,''), WorkFlowEventsHistory.CrDate");

        Label3.Text = "Runing RowID : " + GridView1.SelectedRow.Cells[3].Text.ToString();
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

        geouphead();
    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }
    protected void btn_back1_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        GridView1.SelectedIndex = -1;

    }
    protected void btn_view_Click(object sender, EventArgs e)
    {
        Response.Redirect("WorkflowRuning.aspx?rowid=preview&runingkey=" + GridView1.SelectedRow.Cells[2].Text.ToString());
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        refresh();
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
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        LinkButton lb = (LinkButton)sender;
        if (lb != null)
        {
            String[] sc = lb.ToolTip.Split((",").ToCharArray());
            if (sc.Length == 2)
            {
                SQLeExecuteNonQuery("system", @"update  WorkFlowRunningStatus set wfrowid=null,StartRowID=newid()
 ,etc5='Delete(StartRowID=" + sc[0] + ",wfrowid=" + sc[1] + ")',status='Deleted',UdBy='" + Session["loginname"].ToString() + "', UdDate=GETDATE() " +
 " where StartRowID='" + sc[1] + "'");
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
            DataSet ds = SQLExecuteReader("system", @"SELECT     WorkFlowEventsHistory.RowID, WorkFlowEventsHistory.StartRowID, WorkFlowEventsHistory.StepRowID, WorkFlowEventsHistory.Event, 
                      WorkFlowEventsHistory.SentEmailTo, replace(replace(replace(replace(WorkFlowEventsHistory.Email,';;',';'),';',';'),';;',';'),';;',';') as Email,  replace(replace(replace(replace(WorkFlowEventsHistory.CCEmail,';;',';'),';',';'),';;',';'),';;',';') as CCEmail, WorkFlowEventsHistory.EmailCommandForResent, 
                      WorkFlowEventsHistory.ExecuteCommand, WorkFlowEventsHistory.NextStep, WorkFlowEventsHistory.CrDate, WorkFlowEventsHistory.CrBy, WorkFlowEventsHistory.UdDate, 
                      WorkFlowEventsHistory.UdBy, WorkFlowStep.StepName
FROM         WorkFlowEventsHistory left outer JOIN
                      WorkFlowStep ON convert(nvarchar(100),WorkFlowEventsHistory.NextStep) = convert(nvarchar(100),WorkFlowStep.StepRowId) where WorkFlowEventsHistory.RowID='" + hd.Value + "';");
            if (ds.Tables[0].Rows.Count > 0)
            {
                Label6.Text = "Runing RowID : " + hd.Value.ToString();
                Label15.Text = hd.Value.ToString();

                Label8.Text = (ds.Tables[0].Rows[0]["StepName"].ToString() == "" ? "Fishished แล้ว ไม่สามารถ Rewind ได้" : ds.Tables[0].Rows[0]["StepName"].ToString());
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
        GridView1_SelectedIndexChanged(this, new EventArgs());

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
            Label12.Text = Label12.Text.Replace(Label6.Text.Replace("Runing RowID : ", ""), ds.Tables[0].Rows[0]["rowid"].ToString());

            if (SQLeExecuteNonQuery("system", @"set xact_abort on;
                                BEGIN TRAN T1; update WorkFlowRunningStatus set Status='" + (ds.Tables[0].Rows[0]["NextStep"].ToString() == "9999C689-4C15-445F-9CBB-B0788A0C920A" ? "Hold" : "Active") + "',CurrentStepRowId='" + (ds.Tables[0].Rows[0]["NextStep"].ToString() == "9999C689-4C15-445F-9CBB-B0788A0C920A" ? ds.Tables[0].Rows[0]["StepRowId"].ToString() : ds.Tables[0].Rows[0]["NextStep"].ToString()) + "' where StartRowID='" + Label15.Text + @"';update WorkFlowEventsHistory set EmailCommandForResent='" + Label12.Text + @"' where rowid='" + ds.Tables[0].Rows[0]["rowid"].ToString() + @"';COMMIT TRAN T1;"))
            {
                SQLeExecuteNonQuery("LegalMail", Label12.Text.ToString().Replace("||?", "'").Replace("\" & vbCrLf & \"", ""));

                MultiView1.ActiveViewIndex = 1;
                Label4.Visible = false;


                /*DataSet ds2 = SQLExecuteReader("system", @"SELECT      WorkFlowEventsHistory.rowid,WorkFlowStep_1.StepName, WorkFlowEventsHistory.Event, Prosition.PositionName AS SentEmailTo,  replace(replace(WorkFlowEventsHistory.Email,';;',';'),';',';<br />') as Email,replace(replace(WorkFlowEventsHistory.CCEmail,';;',';'),';',';<br />') as CCEmail, isnull(WorkFlowStep.StepName,'Finished') AS NextStep,WorkFlowEventsHistory.crdate,WorkFlowEventsHistory.crby
                                                    FROM         WorkFlowEventsHistory LEFT OUTER JOIN
                                                                          WorkFlowStep ON CASE WHEN WorkFlowEventsHistory.NextStep = '' THEN newid() ELSE WorkFlowEventsHistory.NextStep END = WorkFlowStep.StepRowId LEFT OUTER JOIN
                                                                          Prosition ON CASE WHEN WorkFlowEventsHistory.SentEmailTo = '' THEN newid() ELSE WorkFlowEventsHistory.SentEmailTo END = Prosition.PositionRowId LEFT OUTER JOIN
                                                                          WorkFlowStep AS WorkFlowStep_1 ON WorkFlowEventsHistory.StepRowID = WorkFlowStep_1.StepRowId
                                                    where WorkFlowEventsHistory.StartRowID='" + GridView1.SelectedRow.Cells[2].Text.ToString() + @"'
                                                    ORDER BY WorkFlowEventsHistory.CrDate  ");
                Label3.Text = "Runing RowID : " + GridView1.SelectedRow.Cells[3].Text.ToString();
                GridView2.DataSource = ds2.Tables[0];
                GridView2.DataBind();*/

                GridView1_SelectedIndexChanged(this, new EventArgs());

                refresh();


            }


        }

    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView GridView1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataRow row = ((DataRowView)e.Row.DataItem).Row;
            String name = row.Field<String>("HisDisplayStdConfig");
            name = (name == null ? "" : name);
            Panel p = (Panel)e.Row.FindControl("Panel1");
            if (p != null)
            {
                if (Request.QueryString["Spacial Action"] != null)
                {
                    try
                    {
                        if (Request.QueryString["Spacial Action"].ToString() == "standard")
                        {
                            Label l = (Label)e.Row.FindControl("HyperLink2_");
                            Label l1 = (Label)e.Row.FindControl("HyperLink3_");
                            HyperLink li1 = (HyperLink)e.Row.FindControl("HyperLink2");
                            HyperLink li2 = (HyperLink)e.Row.FindControl("HyperLink3");
                            l.Visible = l1.Visible = li1.Visible = li2.Visible = false;
                            GridView1.Columns[2].HeaderStyle.Width = 150;
                        }

                    }
                    catch (Exception ex) { }
                }

                if (name.IndexOf("spacialaction:") > 0)
                {
                    try
                    {

                        Label l = (Label)e.Row.FindControl("HyperLink2_");
                        Label l1 = (Label)e.Row.FindControl("HyperLink3_");
                        HyperLink li1 = (HyperLink)e.Row.FindControl("HyperLink2");
                        HyperLink li2 = (HyperLink)e.Row.FindControl("HyperLink3");
                        l.Visible = l1.Visible = li1.Visible = li2.Visible = false;
                        GridView1.Columns[2].HeaderStyle.Width = 150;




                    }
                    catch (Exception ex) { }
                }


                if (GridView1.DataKeys[e.Row.RowIndex].Value.ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
                {
                    GridView1.Columns[2].Visible = false;
                    showall.Visible = false;
                    ////p.Visible = false;
                    //GridView1.Columns[1].Visible = false;
                    //GridView1.Columns[2].HeaderStyle.Width = 200;
                    //p.Controls.Clear();
                    //Literal cc = new Literal();
                    //cc.Text = "------ไม่ได้รับสิทธิ Special Action------";
                    //p.Controls.Add(cc);
                }


                /***********************************
                 * Edit by Phon_y
                 * 2022-05-10
                 * Check condition hide button Del for MOC
                 * ***********************************
                 */
                string UserName = Session["loginname"].ToString();
                //if (row["UserEdit"].ToString().ToLower().IndexOf(UserName.ToLower()) <= 0
                //    && Request.QueryString["rowid"].ToString().Replace(",", "','") == "404c66fd-afe0-4aeb-9e36-d20f8102b3eb")
                //{
                //    LinkButton lbDel = (LinkButton)e.Row.FindControl("LinkButton2"); // Del button
                //    lbDel.Visible = false;

                //    LinkButton lbDel2 = (LinkButton)e.Row.FindControl("btnDel"); // Del button
                //    lbDel2.Visible = false;


                //    Label lbLineShow = (Label)e.Row.FindControl("lbLineShow"); // label ' | '
                //    lbLineShow.Visible = false;
                //}
                //else
                //{
                Label lbLineShow = (Label)e.Row.FindControl("lbLineShow"); // label ' | '
                lbLineShow.Visible = true;

                if (Request.QueryString["rowid"].ToString().Replace(",", "','") == "404c66fd-afe0-4aeb-9e36-d20f8102b3eb"
                    || Request.QueryString["rowid"].ToString().Replace(",", "','") == "45752dc1-bcf4-489c-ad8b-9a453642d787")
                {
                    LinkButton lbDel = (LinkButton)e.Row.FindControl("LinkButton2"); // Del button
                    lbDel.Visible = false;


                    // check permission User can delete
                    // Kotchapon_A, tanida_k
                    DataSet dSet = new DataSet();
                    dSet = SQLExecuteReader("system", @"SELECT Username FROM DeletePermission WHERE WFRowID='" + Request.QueryString["rowid"].ToString().Replace(",", "','") + "'" +
                        " and Username='" + Session["loginname"].ToString() + "'");

                    LinkButton lbDel2 = (LinkButton)e.Row.FindControl("btnDel"); // Del button
                    if (dSet.Tables[0].Rows.Count > 0)
                    {
                        lbDel2.Visible = true;
                    }
                    else
                    {
                        lbDel2.Visible = false;
                        lbLineShow.Visible = false;
                    }


                }
                else
                {
                    LinkButton lbDel = (LinkButton)e.Row.FindControl("LinkButton2"); // Del button
                    lbDel.Visible = true;

                    LinkButton lbDel2 = (LinkButton)e.Row.FindControl("btnDel"); // Del button
                    lbDel2.Visible = false;
                }



                //}
                //****************************************************


            }
        }

    }
    protected void Button6_Click(object sender, EventArgs e)
    {
        GridView1_SelectedIndexChanged(GridView1, new EventArgs());
    }


    private void randerConfigDisplay()
    {
        tmpFillterHistory = "";
        for (int j = 0; j < GridView1.Columns.Count; j++)
        {
            GridView1.Columns[j].Visible = true;
        }

        if (Request.QueryString["rowid"] != null)
        {
            String[] field = { "View", "history", "spacialaction", "startrowid", "etc1", "etc2", "etc3", "etc4", "etc5", "etc6", "etc7", "etc8", "etc9", "etc10", "wfname", "currentstepname", "udby", "uddate", "flowstatus" };

            DataSet ds = SQLExecuteReader("system", @"SELECT top 1 * from WorkFlow where wfrowid='" + Request.QueryString["rowid"].ToString() + "'");

            if (ds.Tables[0].Rows.Count > 0)
            {

                try
                {
                    tmpFillterHistory = ds.Tables[0].Rows[0]["FilterHistory"].ToString();

                    String config = ds.Tables[0].Rows[0]["HisDisplayStdConfig"].ToString();

                    String[] part = splitpart(config, ";");

                    for (int i = 0; i < part.Length; i++)
                    {
                        if (part[i].ToString() != "")
                        {
                            String[] l = splitpart(part[i].Replace("(hide)", ""), ":");

                            for (int k = 0; k < field.Length; k++)
                            {
                                if (l[0] == field[k] & part[i].IndexOf("(hide)") > 0)
                                    GridView1.Columns[k].Visible = false;

                                try
                                {
                                    if (l[0] == field[k] && l[1] != "")
                                        GridView1.Columns[k].HeaderStyle.Width = int.Parse(l[1]);
                                }
                                catch { }
                            }

                        }

                    }


                    try
                    {

                        for (int i = 1; i < 11; i++)
                        {
                            String c = ds.Tables[0].Rows[0]["HisDisplayConfigETC" + i.ToString()].ToString();
                            if (c.IndexOf(";") >= 0)
                            {
                                String[] sp = splitpart(c, ";");
                                for (int k = 0; k < field.Length; k++)
                                {
                                    if (field[k].ToUpper() == "ETC" + i.ToString())
                                    {
                                        if (sp[0] != "")
                                            GridView1.Columns[k].HeaderText = sp[0];


                                        if (sp[1] == "N")
                                        {
                                            GridView1.Columns[k].Visible = false;
                                        }
                                        else
                                        {
                                            if (DropDownList1.Items.FindByValue("ETC" + i.ToString()) == null)
                                            {
                                                DropDownList1.Items.Insert(0, new ListItem(sp[0], "ETC" + i.ToString()));

                                                if (i == 1)
                                                    DropDownList1.Items.FindByValue("ETC1").Selected = true;
                                            }
                                        }

                                        if (sp[2] != "")
                                            GridView1.Columns[k].HeaderStyle.Width = int.Parse(sp[2]);

                                    }
                                }
                            }


                        }

                    }
                    catch (Exception ee) { Response.Write(ee.Message); }

                }
                catch { }

            }
        }
    }

    private string[] splitpart(String config, String code)
    {
        return config.Split(code.ToCharArray());
    }

    private void deleteChildFlow(String runrowid, String wfrowid)
    {
        SQLeExecuteNonQuery("system", "update  WorkFlowRunningStatus set wfrowid=null,StartRowID=newid(),etc5='Delete(StartRowID=" + wfrowid + ",wfrowid=" + runrowid + ")',status='Deleted' where StartRowID='" + runrowid + "'");
        GridView1_SelectedIndexChanged(GridView1, new EventArgs());
    }

    private void geouphead()
    {
        DataTable dt = (DataTable)GridView2.DataSource;
        int j = 0;
        String main = "";
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["DefaultStart"].ToString() == "Y")
            {
                main = dt.Rows[i]["GroupName"].ToString();
            }

            if (tmpCategoryName != dt.Rows[i]["GroupName"].ToString())
            {
                tmpCategoryName = dt.Rows[i]["GroupName"].ToString();

                Table tbl = GridView2.Rows[i].Parent as Table;
                if (tbl != null)
                {
                    if (j > 0)
                    {
                        j++;


                        GridViewRow row1 = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                        TableCell cell1 = new TableCell();

                        // Span the row across all of the columns in the Gridview
                        cell1.ColumnSpan = this.GridView2.Columns.Count;

                        cell1.Width = Unit.Percentage(100);
                        cell1.Style.Add("font-weight", "bold");
                        cell1.Style.Add("background-color", "#ffffff");
                        cell1.Style.Add("color", "#ffffff");

                        System.Web.UI.HtmlControls.HtmlGenericControl span1 = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
                        span1.InnerHtml = "Group : " + tmpCategoryName;

                        cell1.Controls.Add(span1);
                        row1.Cells.Add(cell1);

                        tbl.Rows.AddAt(i + j, row1);

                    }

                    j++;

                    GridViewRow row = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                    TableCell cell = new TableCell();

                    // Span the row across all of the columns in the Gridview
                    cell.ColumnSpan = this.GridView1.Columns.Count;

                    cell.Width = Unit.Percentage(100);
                    cell.Style.Add("font-weight", "bold");
                    cell.Style.Add("background-color", "saddleBrown");
                    cell.Style.Add("color", "floralwhite");

                    System.Web.UI.HtmlControls.HtmlGenericControl span = new System.Web.UI.HtmlControls.HtmlGenericControl("span");

                    if (main == dt.Rows[i]["GroupName"].ToString())
                    {
                        span.InnerHtml = "Group : " + tmpCategoryName;
                    }
                    else
                    {
                        span.InnerHtml = "Group : " + tmpCategoryName + " &nbsp;&nbsp;|&nbsp;<a target='_blank' href='WorkflowRuning.aspx?rowid=force&runingkey=" + dt.Rows[i]["StartRowID"].ToString() + "&referencekey=view'><font color=blue>View</font></a>&nbsp;|&nbsp;<a  target='_blank' href='WorkflowRuning.aspx?rowid=force&runingkey=" + dt.Rows[i]["StartRowID"].ToString() + "&referencekey=force&action=update'><font color=blue>Up.Val</font></a>&nbsp;|&nbsp;<a href=\"javascript:__doPostBack('delsub','" + Request.QueryString["rowid"].ToString() + "," + dt.Rows[i]["StartRowID"].ToString() + "')\" onclick=\"return confirm('ยืนยันการลบ')\"><font color=red>Del</font></a>&nbsp;|";
                    }
                    cell.Controls.Add(span);
                    row.Cells.Add(cell);

                    tbl.Rows.AddAt(i + j, row);



                }
            }

        }

    }

    protected void delsub_Click(object sender, EventArgs e)
    {
        GridView1_SelectedIndexChanged(this, new EventArgs());
        string parameter = Request["__EVENTARGUMENT"];

        String[] sc = parameter.Split((",").ToCharArray());
        if (sc.Length == 2)
        {

            SQLeExecuteNonQuery("system", "update  WorkFlowRunningStatus set wfrowid=null,ParentStartRowID=null,StartRowID=newid(),etc5='Delete(StartRowID=" + sc[0] + ",wfrowid=" + sc[1] + ",ParentStartRowID=" + Label3.Text.Replace("Runing RowID : ", "") + ")',status='Deleted' where StartRowID='" + sc[1] + "'");
        }
        GridView1_SelectedIndexChanged(this, new EventArgs());

    }
    protected void lnk_exp_csv_Click(object sender, EventArgs e)
    {
        refresh();
        DataTable dt = (DataTable)GridView1.DataSource;
        dt.Columns.Remove("ApplicationFolder");
        dt.Columns.Remove("UserSpecialAction");
        dt.Columns.Remove("HisDisplayStdConfig");
        ExportToExcel(dt, this.Page);
    }


    #region ExportToExcel
    public void ExportToExcel(DataTable dt, System.Web.UI.Page p)
    {
        if (dt.Rows.Count > 0)
        {
            string filename = "DataExcel.xls";
            System.IO.StringWriter tw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            DataGrid dgGrid = new DataGrid();
            dgGrid.DataSource = replaceColumnName(dt);
            dgGrid.DataBind();

            dgGrid.RenderControl(hw);
            p.Response.ContentType = "application/vnd.ms-excel";
            p.Response.ContentEncoding = System.Text.Encoding.Unicode;
            p.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            p.Response.Charset = "utf-8";
            p.Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            p.EnableViewState = false;
            p.Response.Write(tw.ToString());
            p.Response.End();

        }
    }
    #endregion


    private DataTable replaceColumnName(DataTable dt)
    {
        DataSet ds = SQLExecuteReader("system", "select * FROM   WorkFlow WHERE  (WFRowId = '" + Request.QueryString["rowid"].ToString() + "')");

        if (ds.Tables[0].Rows.Count > 0)
        {
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC1"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc1"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC1"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC2"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc2"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC2"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC3"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc3"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC3"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC4"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc4"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC4"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC5"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc5"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC5"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC6"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc6"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC6"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC7"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc7"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC7"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC8"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc8"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC8"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC9"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc9"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC9"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
            if (ds.Tables[0].Rows[0]["HisDisplayConfigETC10"].ToString() != ";N;")
            {
                try
                {
                    dt.Columns["etc10"].ColumnName = ds.Tables[0].Rows[0]["HisDisplayConfigETC10"].ToString().Replace(";Y;", "");
                }
                catch { }
            }
        }

        return dt;
    }

    protected void LinkButton4_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 3;
        loaddelete();
    }

    private void loaddelete()
    {
        DataSet ds = new DataSet();

        if (Request.QueryString["rowid"].ToString().Replace(",", "','") == "404c66fd-afe0-4aeb-9e36-d20f8102b3eb"
           || Request.QueryString["rowid"].ToString().Replace(",", "','") == "45752dc1-bcf4-489c-ad8b-9a453642d787")
        {
            ds = SQLExecuteReader("system", @"SELECT ETC1 as MOC_NO, ETC2 as Location, ETC3 as Moc_Title, ETC4 as Step 
                                ,dh.DeleteReason as [Delete Reason]
                                ,CrDate, CrBy, UdDate as [Deleted Date], UdBy as [Deleted By]                                
                                FROM WorkFlowRunningStatus wfrs
                                LEFT JOIN DeleteHistory dh on wfrs.ETC1=dh.DocNo or wfrs.etc5 like CONCAT('%', dh.StartRowID, '%')
                                where Status='deleted' and etc5 like '%" + Request.QueryString["rowid"].ToString().Replace(",", "','") + @"%' 
                                order by uddate desc");

        }
        else
        {
            ds = SQLExecuteReader("system", @"SELECT  ETC1 as MOC_NO, ETC2 as Location, ETC3 as Moc_Title, ETC4 as Step ,CrDate, CrBy, UdDate as [Deleted Date], UdBy as [Deleted By]
            FROM  WorkFlowRunningStatus
            where Status='deleted' and etc5 like '%" + Request.QueryString["rowid"].ToString().Replace(",", "','") + @"%'
            order by uddate desc");
        }

        GridView3.DataSource = ds.Tables[0];
        GridView3.DataBind();

    }
    protected void showall_CheckedChanged(object sender, EventArgs e)
    {
        refresh();
    }

    protected void btnDel_Click(object sender, EventArgs e)
    {

    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "OnDel")
        {
            lbDocNo.Text = e.CommandArgument.ToString().Split(',')[2];
            txtDeleteReason.Text = "";

            hdWFID.Value = e.CommandArgument.ToString().Split(',')[0];
            hdRUNID.Value = e.CommandArgument.ToString().Split(',')[1];

            lbDeleteBy.Text = Session["loginname"].ToString();
            lbDeleteDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            lbModalError.Text = "";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "Pop", "$(document).ready(function () {$('#myModal').modal();});", true);
        }
    }

    protected void btSaveDel_Click(object sender, EventArgs e)
    {
        if (txtDeleteReason.Text.Trim() != "")
        {
            SQLeExecuteNonQuery("system", @"update  WorkFlowRunningStatus set wfrowid=null,StartRowID=newid()
 ,etc5='Delete(StartRowID=" + hdRUNID.Value + ",wfrowid=" + hdWFID.Value + ")',status='Deleted',UdBy='" + Session["loginname"].ToString() + "', UdDate=GETDATE() " +
" where StartRowID='" + hdRUNID.Value + "'");

            SQLeExecuteNonQuery("system", @"INSERT INTO DeleteHistory([StartRowID],[WFRowID],[DocNo],[DeleteReason],[DeleteBy],[DeleteDate]) 
                            VALUES('" + hdRUNID.Value + "','" + hdWFID.Value + "','" + lbDocNo.Text.Trim() + "','" + txtDeleteReason.Text.Trim().Replace("'","''") + "'" +
                                ",'" + Session["loginname"].ToString() + "',GETDATE())");

            refresh();
        }
        else
        {
            lbModalError.Text = "Please input Delete Reason!";
            txtDeleteReason.Focus();
        }

    }
}