using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class step_events : BST.libDB
{
    String selectb = "";
    private string tmpCategoryName = "";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["loginname"] != null)
        {
            if (Session["loginname"].ToString() == "guest")
            {
                btn_back0.Visible = false;
            }
        }

        if (!IsPostBack)
        {

            this.SetCurentNavigatePage("Step & Events");

            if (Request.QueryString["rowid"] != null)
            {
                refresh();
                
            }
        }

    }


    private void refresh()
    {
        DataSet ds = this.SQLExecuteReader("system", @"SELECT     WorkFlowStep.StepRowId, WorkFlowStep.Seq,WorkFlowStep.GroupName, WorkFlowStep.StepName, (CASE WHEN WorkFlowStep.DefaultStart = 'N' THEN (case when WorkFlowStep.TiggerStartStepCondition<>'' then '<a href=# title=""'+WorkFlowStep.TiggerStartStepCondition+'"">Auto</a>' else (case when WorkFlowStep.StepAutoStart='continue' then '<a href=# title=''this step will start to resume main workflow when all child flows finished''>* RESUME *</a>' else '' end) end ) ELSE '<font color=orange><b>Yes</b></font>' END) AS DefaultStart, 
                                                                              replace(case when WorkFlowStep.enablesave='Y' and WorkFlowStep_1.StepName is null and isnull(WorkFlowStep.DinamicFiledSaveValueToStep,'')='' then '<font color=green><b>Finished</b></font>' when WorkFlowStep.enablesave='Y' and WorkFlowStep_1.StepName is null and isnull(WorkFlowStep.DinamicFiledSaveValueToStep,'')!='' then 'Dynamic: '+WorkFlowStep.DinamicFiledSaveValueToStep else WorkFlowStep_1.StepName end,'Dynamic: **HOLD**','* HOLD *') AS aftersave, WorkFlowStep_1.StepRowId as StepRowIdAfterSave,
                                                                              (case when WorkFlowStep.enableapprove='Y' and WorkFlowStep_2.StepName is null and isnull(WorkFlowStep.DinamicFiledApproveValueToStep,'')='' then '<font color=green><b>Finished</b></font>' when WorkFlowStep.enableapprove='Y' and WorkFlowStep_2.StepName is null and isnull(WorkFlowStep.DinamicFiledApproveValueToStep,'')!='' then 'Dynamic: '+WorkFlowStep.DinamicFiledApproveValueToStep else WorkFlowStep_2.StepName end) AS afterapprove, WorkFlowStep_2.StepRowId as StepRowIdAfterApprove,
                                                                              (case when WorkFlowStep.enablereject='Y' and WorkFlowStep_3.StepName is null and isnull(WorkFlowStep.DinamicFiledRejectValueToStep,'')='' then '<font color=green><b>Finished</b></font>' when WorkFlowStep.enablereject='Y' and WorkFlowStep_3.StepName is null and isnull(WorkFlowStep.DinamicFiledRejectValueToStep,'')!='' then 'Dynamic: '+WorkFlowStep.DinamicFiledRejectValueToStep else WorkFlowStep_3.StepName end) AS afterreject,WorkFlowStep.DinamicKeyValue,WorkFlowStep_3.StepRowId as StepRowIdAfterReject,
                                                                   (CASE WHEN WorkFlowStep.CheckConditionAutoStartSave = 'Y' THEN '<a href=''#''title=''Check Auto Start Child Workflow''>--> Auto</a>' ELSE '' END) AS callstartsave,(CASE WHEN WorkFlowStep.CheckConditionAutoStartApprove = 'Y' THEN '<a href=''#''title=''Check Auto Start Child Workflow''>--> Auto</a>' ELSE '' END) AS callstartapprove,(CASE WHEN WorkFlowStep.CheckConditionAutoStartReject = 'Y' THEN '<a href=''#''title=''Check Auto Start Child Workflow''>--> Auto</a>' ELSE '' END) AS callstartreject
                                                        FROM         WorkFlowStep LEFT OUTER JOIN
                                                                              WorkFlowStep AS WorkFlowStep_3 ON WorkFlowStep.EventAfterRejectGotoStep = WorkFlowStep_3.StepRowId LEFT OUTER JOIN
                                                                              WorkFlowStep AS WorkFlowStep_2 ON WorkFlowStep.EventAfterApproveGotoStep = WorkFlowStep_2.StepRowId LEFT OUTER JOIN
                                                                              WorkFlowStep AS WorkFlowStep_1 ON WorkFlowStep.EventAfterSaveGotoStep = WorkFlowStep_1.StepRowId
                                                        where WorkFlowStep.WFRowId='" + Request.QueryString["rowid"].ToString() + @"' and (WorkFlowStep.StepName like '%" + txt_tt.Text + @"%' or WorkFlowStep.Seq like '%" + txt_tt.Text + @"%') order by WorkFlowStep.GroupName,WorkFlowStep.seq;
                                                    select * from WorkFlowTemplate where WFRowId='" + Request.QueryString["rowid"].ToString() + @"';
                                                    select * from Prosition where WFRowId='" + Request.QueryString["rowid"].ToString() + @"' order by positionname;
                                                    select * from WorkFlowStep where WFRowId='" + Request.QueryString["rowid"].ToString() + @"' order by groupname,seq;
                                                    select * from WorkFlow where WFRowId='" + Request.QueryString["rowid"].ToString() + "'");


        GridView1.DataSource = ds.Tables[0];
        GridView1.DataBind();
        geouphead();
        rowid_txt.Text = Request.QueryString["rowid"].ToString();
        lb_wfname.Text = "WorkFlow Name : "+ds.Tables[4].Rows[0]["WFName"].ToString();


        if (ds.Tables[4].Rows[0]["useredit"].ToString().ToLower().IndexOf(isWho().ToLower()) < 0)
        {

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                LinkButton lb = (LinkButton)GridView1.Rows[i].Cells[2].FindControl("LinkButton2");
                if (lb != null)
                {
                    lb.Visible = false;
                }


            }

            btn_new.Visible = false;
            btn_save.Visible = btn_save1.Visible = btn_save2.Visible = false;
        }



        dl_templateid.DataSource = ds.Tables[1];
        dl_templateid.DataTextField = "TemplateName";
        dl_templateid.DataValueField = "TemplateRowId";
        dl_templateid.DataBind();
        dl_templateid.Items.Insert(0, new ListItem("Please select", "Please select"));


        //#################### save
        dl_EventAfterSaveMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterSaveMailToPosition.DataTextField = "PositionName";
        dl_EventAfterSaveMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterSaveMailToPosition.DataBind();
        dl_EventAfterSaveMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterSaveMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterSaveCC1MailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterSaveCC1MailToPosition.DataTextField = "PositionName";
        dl_EventAfterSaveCC1MailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterSaveCC1MailToPosition.DataBind();
        dl_EventAfterSaveCC1MailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterSaveCC1MailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));


        dl_EventAfterSaveInformMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterSaveInformMailToPosition.DataTextField = "PositionName";
        dl_EventAfterSaveInformMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterSaveInformMailToPosition.DataBind();
        dl_EventAfterSaveInformMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterSaveInformMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));


        //#################### approve

        dl_EventAfterApproveMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterApproveMailToPosition.DataTextField = "PositionName";
        dl_EventAfterApproveMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterApproveMailToPosition.DataBind();
        dl_EventAfterApproveMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterApproveMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterApproveCC1MailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterApproveCC1MailToPosition.DataTextField = "PositionName";
        dl_EventAfterApproveCC1MailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterApproveCC1MailToPosition.DataBind();
        dl_EventAfterApproveCC1MailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterApproveCC1MailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterApproveInformMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterApproveInformMailToPosition.DataTextField = "PositionName";
        dl_EventAfterApproveInformMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterApproveInformMailToPosition.DataBind();
        dl_EventAfterApproveInformMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterApproveInformMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));


        //#################### reject
        dl_EventAfterRejectMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterRejectMailToPosition.DataTextField = "PositionName";
        dl_EventAfterRejectMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterRejectMailToPosition.DataBind();
        dl_EventAfterRejectMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterRejectMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterRejectCC1MailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterRejectCC1MailToPosition.DataTextField = "PositionName";
        dl_EventAfterRejectCC1MailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterRejectCC1MailToPosition.DataBind();
        dl_EventAfterRejectCC1MailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterRejectCC1MailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterRejectInformMailToPosition.DataSource = ds.Tables[2];
        dl_EventAfterRejectInformMailToPosition.DataTextField = "PositionName";
        dl_EventAfterRejectInformMailToPosition.DataValueField = "PositionRowId";
        dl_EventAfterRejectInformMailToPosition.DataBind();
        dl_EventAfterRejectInformMailToPosition.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterRejectInformMailToPosition.Items.Insert(0, new ListItem("Please select", "Please select"));





        dl_EventAfterSaveGotoStep.DataSource = ds.Tables[3];
        dl_EventAfterSaveGotoStep.DataTextField = "StepName";
        dl_EventAfterSaveGotoStep.DataValueField = "StepRowId";
        dl_EventAfterSaveGotoStep.DataBind();
        dl_EventAfterSaveGotoStep.Items.Insert(0, new ListItem("** Hold ** รอ Step AutoStart", "*HOLD*"));
        dl_EventAfterSaveGotoStep.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterSaveGotoStep.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterApproveGotoStep.DataSource = ds.Tables[3];
        dl_EventAfterApproveGotoStep.DataTextField = "StepName";
        dl_EventAfterApproveGotoStep.DataValueField = "StepRowId";
        dl_EventAfterApproveGotoStep.DataBind();
        dl_EventAfterApproveGotoStep.Items.Insert(0, new ListItem("** Hold ** รอ Step AutoStart", "*HOLD*"));
        dl_EventAfterApproveGotoStep.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterApproveGotoStep.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterRejectGotoStep.DataSource = ds.Tables[3];
        dl_EventAfterRejectGotoStep.DataTextField = "StepName";
        dl_EventAfterRejectGotoStep.DataValueField = "StepRowId";
        dl_EventAfterRejectGotoStep.DataBind();
        dl_EventAfterRejectGotoStep.Items.Insert(0, new ListItem("** Hold ** รอ Step AutoStart", "*HOLD*"));
        dl_EventAfterRejectGotoStep.Items.Insert(0, new ListItem("Use Dynamic Selected Key", "UseKey"));
        dl_EventAfterRejectGotoStep.Items.Insert(0, new ListItem("Please select", "Please select"));

        dl_EventAfterSaveExecAtServer.Items.Clear();
        dl_EventAfterApproveExecAtServer.Items.Clear();
        dl_EventAfterRejectExecAtServer.Items.Clear();
        for (int i = 0; i < System.Web.Configuration.WebConfigurationManager.ConnectionStrings.Count; i++)
        {
            dl_EventAfterSaveExecAtServer.Items.Add(new ListItem(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString(), System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString()));
            dl_EventAfterApproveExecAtServer.Items.Add(new ListItem(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString(), System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString()));
            dl_EventAfterRejectExecAtServer.Items.Add(new ListItem(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString(), System.Web.Configuration.WebConfigurationManager.ConnectionStrings[i].Name.ToString()));

        }

        dl_EventAfterSaveExecAtServer.Items.Insert(0, new ListItem("Please select", "Please select"));
        dl_EventAfterApproveExecAtServer.Items.Insert(0, new ListItem("Please select", "Please select"));
        dl_EventAfterRejectExecAtServer.Items.Insert(0, new ListItem("Please select", "Please select"));

    }

    private void getCururrentData()
    {

        DataSet ds = this.SQLExecuteReader("system", "select * from WorkFlowStep where StepRowId='" + rowid_txt.Text + "' order by seq;");
        if (ds.Tables[0].Rows.Count > 0)
        {
            cb_enablesave.Checked = (ds.Tables[0].Rows[0]["EnableSave"].ToString() == "Y" ? true : false);
            cb_enableapprove.Checked = (ds.Tables[0].Rows[0]["EnableApprove"].ToString() == "Y" ? true : false);
            cb_enablereject.Checked = (ds.Tables[0].Rows[0]["EnableReject"].ToString() == "Y" ? true : false);

            AliasNameSave.Text = ds.Tables[0].Rows[0]["AliasNameSave"].ToString();
            AliasNameApprove.Text = ds.Tables[0].Rows[0]["AliasNameApprove"].ToString();
            AliasNameReject.Text = ds.Tables[0].Rows[0]["AliasNameReject"].ToString();

            FreeTextBox1.Text = ds.Tables[0].Rows[0]["guideline"].ToString();

            txt_javaonload.Text = ds.Tables[0].Rows[0]["JavaScriptOnload"].ToString();

            //Update add function tigger
            TiggerSave.Text = ds.Tables[0].Rows[0]["TriggerSave"].ToString();
            TiggerApprove.Text = ds.Tables[0].Rows[0]["TriggerApprove"].ToString();
            TiggerReject.Text = ds.Tables[0].Rows[0]["TriggerReject"].ToString();

            HideButtonSave.Checked = (ds.Tables[0].Rows[0]["HideButtonSave"].ToString() == "Y" ? true : false);
            HideButtonReject.Checked = (ds.Tables[0].Rows[0]["HideButtonReject"].ToString() == "Y" ? true : false);
            HideButtonApprove.Checked = (ds.Tables[0].Rows[0]["HideButtonApprove"].ToString() == "Y" ? true : false);

            CheckTiggerSave.Checked = (ds.Tables[0].Rows[0]["CheckTiggerSave"].ToString() == "Y" ? true : false);
            CheckTiggerApprove.Checked = (ds.Tables[0].Rows[0]["CheckTiggerApprove"].ToString() == "Y" ? true : false);
            CheckTiggerReject.Checked = (ds.Tables[0].Rows[0]["CheckTiggerReject"].ToString() == "Y" ? true : false);


            CheckConditionAutoStartSave.Checked = (ds.Tables[0].Rows[0]["CheckConditionAutoStartSave"].ToString() == "Y" ? true : false);
            CheckConditionAutoStartApprove.Checked = (ds.Tables[0].Rows[0]["CheckConditionAutoStartApprove"].ToString() == "Y" ? true : false);
            CheckConditionAutoStartReject.Checked = (ds.Tables[0].Rows[0]["CheckConditionAutoStartReject"].ToString() == "Y" ? true : false);
            //endUpdate add function tigger


            rb_stepautostart.SelectedIndex = (ds.Tables[0].Rows[0]["StepAutoStart"].ToString() == "condition" ? 2 : ((ds.Tables[0].Rows[0]["StepAutoStart"].ToString() == "continue" ? 1 : 0)));
            RadioButtonList1_SelectedIndexChanged(this,new EventArgs());

        
            DinamicKey.Text = ds.Tables[0].Rows[0]["DinamicKeyValue"].ToString();
            passvalueoldtemplate.Checked = (ds.Tables[0].Rows[0]["PassValueOldTemplate"].ToString() == "Y" ? true : false);


            cb_enablesave_CheckedChanged(this, new EventArgs());
            cb_enableapprove_CheckedChanged(this, new EventArgs());
            cb_enablereject_CheckedChanged(this, new EventArgs());


            cb_uploadmgr.Checked = (ds.Tables[0].Rows[0]["AllowUseUploadMgr"].ToString() == "Y" ? true : false);
            if (cb_uploadmgr.Checked)
            {
                cb_uploadot.Enabled = true;
                cb_uploadot.Items[0].Selected = (ds.Tables[0].Rows[0]["AllowMoreUploadFile"].ToString() == "Y" ? true : false);
                cb_uploadot.Items[1].Selected = (ds.Tables[0].Rows[0]["AllowDeleteFile"].ToString() == "Y" ? true : false);
                cb_uploadot.Items[2].Selected = (ds.Tables[0].Rows[0]["AutoExpand"].ToString() == "Y" ? true : false);
            }
            else
            {
                cb_uploadot.Items[0].Selected = false;
                cb_uploadot.Items[1].Selected = false;
                cb_uploadot.Items[2].Selected = false;
                cb_uploadot.Enabled = false;
            }


            cb_defaultstart.Checked = (ds.Tables[0].Rows[0]["DefaultStart"].ToString() == "Y" ? true : false);
            try { dl_templateid.SelectedValue = ds.Tables[0].Rows[0]["TemplateRowId"].ToString(); }
            catch { }


            //####################### save
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveValueToselect"].ToString() != "")
                {
                    dl_EventAfterSaveMailToPosition.SelectedValue = "UseKey";
                    SaveConditionVal.Visible = true;
                    txt_DinamicFiledSaveValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledSaveValueToselect"].ToString();
                }
                else
                {
                    SaveConditionVal.Visible = false;
                    txt_DinamicFiledSaveValueToselect.Text = "";
                }
                dl_EventAfterSaveMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveMailToPosition"].ToString();

            }
            catch { }
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveCC1ValueToselect"].ToString() != "")
                {
                    dl_EventAfterSaveCC1MailToPosition.SelectedValue = "UseKey";
                    SaveConditionValCC1.Visible = true;
                    txt_DinamicFiledSaveCC1ValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledSaveCC1ValueToselect"].ToString();
                }
                else
                {
                    SaveConditionValCC1.Visible = false;
                    txt_DinamicFiledSaveCC1ValueToselect.Text = "";
                }
                dl_EventAfterSaveCC1MailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveCC1MailToPosition"].ToString();

            }
            catch { }
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveInformValueToselect"].ToString() != "")
                {
                    dl_EventAfterSaveInformMailToPosition.SelectedValue = "UseKey";
                    SaveConditionValInform.Visible = true;
                    txt_DinamicFiledSaveInformValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledSaveInformValueToselect"].ToString();
                }
                else
                {
                    SaveConditionValInform.Visible = false;
                    txt_DinamicFiledSaveInformValueToselect.Text = "";
                }
                dl_EventAfterSaveInformMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveInformMailToPosition"].ToString();

            }
            catch { }
            
            try
            {

                if (ds.Tables[0].Rows[0]["EventAfterSaveGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveValueToStep"].ToString() == "**HOLD**")
                {
                    dl_EventAfterSaveGotoStep.SelectedValue = "*HOLD*";
                    stepcondition_save.Visible = false;
                    txt_DinamicFiledSaveValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledSaveValueToStep"].ToString();
                }
                else if (ds.Tables[0].Rows[0]["EventAfterSaveGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveValueToStep"].ToString() != "")
                {
                    dl_EventAfterSaveGotoStep.SelectedValue = "UseKey";
                    stepcondition_save.Visible = true;
                    txt_DinamicFiledSaveValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledSaveValueToStep"].ToString();
                }
                else
                {
                    stepcondition_save.Visible = false;
                    txt_DinamicFiledSaveValueToStep.Text = "";
                }
                dl_EventAfterSaveGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveGotoStep"].ToString();

            }
            catch { }



            //####################### approve
            try
            {
                 if (ds.Tables[0].Rows[0]["EventAfterApproveMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveValueToselect"].ToString() != "")
                {
                    dl_EventAfterApproveMailToPosition.SelectedValue = "UseKey";
                    ApproveConditionVal.Visible = true;
                    txt_DinamicFiledApproveValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveValueToselect"].ToString();
                }
                 else if (ds.Tables[0].Rows[0]["EventAfterApproveMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveValueToselect"].ToString() == "")
                 {
                     dl_EventAfterApproveMailToPosition.SelectedValue = "UseKey";
                     ApproveConditionVal.Visible = true;
                     txt_DinamicFiledApproveValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveValueToselect"].ToString();
                 }
                 else
                 {
                     ApproveConditionVal.Visible = false;
                     txt_DinamicFiledApproveValueToselect.Text = "";
                 }
                 dl_EventAfterApproveMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveMailToPosition"].ToString();

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterApproveCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveCC1ValueToselect"].ToString() != "")
                {
                    dl_EventAfterApproveCC1MailToPosition.SelectedValue = "UseKey";
                    ApproveConditionValCC1.Visible = true;
                    txt_DinamicFiledApproveCC1ValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveCC1ValueToselect"].ToString();
                }
                else
                {
                    ApproveConditionValCC1.Visible = false;
                    txt_DinamicFiledApproveCC1ValueToselect.Text = "";
                }
                dl_EventAfterApproveCC1MailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveCC1MailToPosition"].ToString();

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterApproveInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveInformValueToselect"].ToString() != "")
                {
                    dl_EventAfterApproveInformMailToPosition.SelectedValue = "UseKey";
                    ApproveConditionValInform.Visible = true;
                    txt_DinamicFiledApproveInformValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveInformValueToselect"].ToString();
                }
                else
                {
                    ApproveConditionValInform.Visible = false;
                    txt_DinamicFiledApproveInformValueToselect.Text = "";
                }
                dl_EventAfterApproveInformMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveInformMailToPosition"].ToString();

            }
            catch { }
            try
            {
               
                if (ds.Tables[0].Rows[0]["EventAfterApproveGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveValueToStep"].ToString() == "**HOLD**")
                {
                    dl_EventAfterApproveGotoStep.SelectedValue = "*HOLD*";
                    stepcondition_approve.Visible = false;
                    txt_DinamicFiledApproveValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveValueToStep"].ToString();
                }
                else if (ds.Tables[0].Rows[0]["EventAfterApproveGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveValueToStep"].ToString() != "")
                {
                    dl_EventAfterApproveGotoStep.SelectedValue = "UseKey";
                    stepcondition_approve.Visible = true;
                    txt_DinamicFiledApproveValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledApproveValueToStep"].ToString();
                }
                else
                {
                    stepcondition_approve.Visible = false;
                    txt_DinamicFiledApproveValueToStep.Text = "";
                }
                dl_EventAfterApproveGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveGotoStep"].ToString();

            }
            catch { }


            //####################### reject
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectValueToselect"].ToString() != "")
                {
                    dl_EventAfterRejectMailToPosition.SelectedValue = "UseKey";
                    RejectConditionVal.Visible = true;
                    txt_DinamicFiledRejectValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledRejectValueToselect"].ToString();
                }
                else
                {
                    RejectConditionVal.Visible = false;
                    txt_DinamicFiledRejectValueToselect.Text = "";
                }
                dl_EventAfterRejectMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectMailToPosition"].ToString();

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectCC1ValueToselect"].ToString() != "")
                {
                    dl_EventAfterRejectCC1MailToPosition.SelectedValue = "UseKey";
                    RejectConditionValCC1.Visible = true;
                    txt_DinamicFiledRejectCC1ValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledRejectCC1ValueToselect"].ToString();
                }
                else
                {
                    RejectConditionValCC1.Visible = false;
                    txt_DinamicFiledRejectCC1ValueToselect.Text = "";
                }
                dl_EventAfterRejectCC1MailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectCC1MailToPosition"].ToString();

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectInformValueToselect"].ToString() != "")
                {
                    dl_EventAfterRejectInformMailToPosition.SelectedValue = "UseKey";
                    RejectConditionValInform.Visible = true;
                    txt_DinamicFiledRejectInformValueToselect.Text = ds.Tables[0].Rows[0]["DinamicFiledRejectInformValueToselect"].ToString();
                }
                else
                {
                    RejectConditionValInform.Visible = false;
                    txt_DinamicFiledRejectInformValueToselect.Text = "";
                }
                dl_EventAfterRejectInformMailToPosition.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectInformMailToPosition"].ToString();

            }
            catch { }
            try
            {
              
                if (ds.Tables[0].Rows[0]["EventAfterRejectGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectValueToStep"].ToString() == "**HOLD**")
                {
                    dl_EventAfterRejectGotoStep.SelectedValue = "*HOLD*";
                    stepcondition_reject.Visible = false;
                    txt_DinamicFiledRejectValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledRejectValueToStep"].ToString();
                }
                else if (ds.Tables[0].Rows[0]["EventAfterRejectGotoStep"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectValueToStep"].ToString() != "")
                {
                    dl_EventAfterRejectGotoStep.SelectedValue = "UseKey";
                    stepcondition_reject.Visible = true;
                    txt_DinamicFiledRejectValueToStep.Text = ds.Tables[0].Rows[0]["DinamicFiledRejectValueToStep"].ToString();
                }
                else
                {
                    stepcondition_reject.Visible = false;
                    txt_DinamicFiledRejectValueToStep.Text = "";
                }
                dl_EventAfterRejectGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectGotoStep"].ToString();

            }
            catch { }



            //String vv = ds.Tables[0].Rows[0]["EventAfterSaveEmailContent"].ToString();
            txt_EventAfterSaveEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterSaveEmailContent"].ToString().Replace("\n","<br />");
            txt_EventAfterApproveEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterApproveEmailContent"].ToString().Replace("\n", "<br />");
            txt_EventAfterRejectEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterRejectEmailContent"].ToString().Replace("\n", "<br />");

            ck_Save_dont_send.Checked = (ds.Tables[0].Rows[0]["EventSaveDonotSendMail"].ToString() == "Y" ? true : false);
            ck_Approve_dont_send.Checked = (ds.Tables[0].Rows[0]["EventApproveDonotSendMail"].ToString() == "Y" ? true : false);
            ck_Reject_dont_send.Checked = (ds.Tables[0].Rows[0]["EventRejectDonotSendMail"].ToString() == "Y" ? true : false);


            txt_TiggerStartStepCondition.Text = ds.Tables[0].Rows[0]["TiggerStartStepCondition"].ToString();
            String sl = ds.Tables[0].Rows[0]["TiggerStartStepConditionButton"].ToString().Replace("------", "");

            dl_TiggerStartStepConditionButton0.SelectedIndex = (rb_stepautostart.SelectedIndex==1? (sl == "Save" ? 1 : (sl == "Approve" ? 2 : (sl == "Reject" ? 3 : 0))):0);
             dl_TiggerStartStepConditionButton.SelectedIndex = (rb_stepautostart.SelectedIndex==2? (sl == "Save" ? 1 : (sl == "Approve" ? 2 : (sl == "Reject" ? 3 : 0))):0);

            txt_EventAfterSaveUpdateValueETC1.Text = ds.Tables[0].Rows[0]["EventAfterSaveUpdateValueETC1"].ToString();
            txt_EventAfterSaveUpdateValueETC2.Text = ds.Tables[0].Rows[0]["EventAfterSaveUpdateValueETC2"].ToString();
            txt_EventAfterSaveUpdateValueETC3.Text = ds.Tables[0].Rows[0]["EventAfterSaveUpdateValueETC3"].ToString();
            txt_EventAfterSaveUpdateValueETC4.Text = ds.Tables[0].Rows[0]["EventAfterSaveUpdateValueETC4"].ToString();
            txt_EventAfterSaveUpdateValueETC5.Text = ds.Tables[0].Rows[0]["EventAfterSaveUpdateValueETC5"].ToString();

            txt_EventAfterApproveUpdateValueETC1.Text = ds.Tables[0].Rows[0]["EventAfterApproveUpdateValueETC1"].ToString();
            txt_EventAfterApproveUpdateValueETC2.Text = ds.Tables[0].Rows[0]["EventAfterApproveUpdateValueETC2"].ToString();
            txt_EventAfterApproveUpdateValueETC3.Text = ds.Tables[0].Rows[0]["EventAfterApproveUpdateValueETC3"].ToString();
            txt_EventAfterApproveUpdateValueETC4.Text = ds.Tables[0].Rows[0]["EventAfterApproveUpdateValueETC4"].ToString();
            txt_EventAfterApproveUpdateValueETC5.Text = ds.Tables[0].Rows[0]["EventAfterApproveUpdateValueETC5"].ToString();

            txt_EventAfterRejectUpdateValueETC1.Text = ds.Tables[0].Rows[0]["EventAfterRejectUpdateValueETC1"].ToString();
            txt_EventAfterRejectUpdateValueETC2.Text = ds.Tables[0].Rows[0]["EventAfterRejectUpdateValueETC2"].ToString();
            txt_EventAfterRejectUpdateValueETC3.Text = ds.Tables[0].Rows[0]["EventAfterRejectUpdateValueETC3"].ToString();
            txt_EventAfterRejectUpdateValueETC4.Text = ds.Tables[0].Rows[0]["EventAfterRejectUpdateValueETC4"].ToString();
            txt_EventAfterRejectUpdateValueETC5.Text = ds.Tables[0].Rows[0]["EventAfterRejectUpdateValueETC5"].ToString();


            // Set visible email cc content
            try
            {
                txt_EventAfterSaveInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterSaveInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");
                divSave_informmailcontent.Visible = (dl_EventAfterSaveInformMailToPosition.Text != "Please select");
                txt_EventAfterSaveEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterSaveEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterSaveEmailSubject"].ToString());

                txt_EventAfterApproveInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterApproveInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");
                divApprove_informmailcontent.Visible = (dl_EventAfterApproveInformMailToPosition.Text != "Please select");
                txt_EventAfterApproveEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterApproveEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterApproveEmailSubject"].ToString());

                txt_EventAfterRejectInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterRejectInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");
                divReject_informmailcontent.Visible = (dl_EventAfterRejectInformMailToPosition.Text != "Please select");
                txt_EventAfterRejectEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterRejectEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterRejectEmailSubject"].ToString());
                
            }
            catch { }
            //end cc


           // try { dl_EventAfterSaveGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveGotoStep"].ToString(); } catch { }
           // try { dl_EventAfterApproveGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveGotoStep"].ToString(); }catch { }
           // try { dl_EventAfterRejectGotoStep.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectGotoStep"].ToString(); } catch { }


            try { dl_EventAfterSaveExecAtServer.SelectedValue = ds.Tables[0].Rows[0]["EventAfterSaveExecAtServer"].ToString(); }catch { }
            try { dl_EventAfterApproveExecAtServer.SelectedValue = ds.Tables[0].Rows[0]["EventAfterApproveExecAtServer"].ToString(); }catch { }
            try { dl_EventAfterRejectExecAtServer.SelectedValue = ds.Tables[0].Rows[0]["EventAfterRejectExecAtServer"].ToString(); }catch { }


            txt_EventAfterSaveExecCommand.Text = ds.Tables[0].Rows[0]["EventAfterSaveExecCommand"].ToString();
            txt_EventAfterApproveExecCommand.Text = ds.Tables[0].Rows[0]["EventAfterApproveExecCommand"].ToString();
            txt_EventAfterRejectExecCommand.Text = ds.Tables[0].Rows[0]["EventAfterRejectExecCommand"].ToString();
            dl_templateid_SelectedIndexChanged(this,new EventArgs());


            txt_EventBeforeSaveCallJavascriptFunction.Text = ds.Tables[0].Rows[0]["EventBeforeSaveCallJavascriptFunction"].ToString();
            txt_EventBeforeApproveCallJavascriptFunction.Text = ds.Tables[0].Rows[0]["EventBeforeApproveCallJavascriptFunction"].ToString();
            txt_EventBeforeRejectCallJavascriptFunction.Text = ds.Tables[0].Rows[0]["EventBeforeRejectCallJavascriptFunction"].ToString();



          
            if (dl_templateid.SelectedValue != "Please select")
            {
                DataSet ds2 = this.SQLExecuteReader("system", "select * from StepEnableObject where StepRowId='" + rowid_txt.Text + "' and TemplateRowId='" + dl_templateid.SelectedValue + "'");

                for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                {

                    for (int i = 0; i < cb_fieldlist.Items.Count; i++)
                    {
                        if (cb_fieldlist.Items[i].Value == ds2.Tables[0].Rows[j]["ObjectRowId"].ToString())
                        {
                            cb_fieldlist.Items[i].Selected = true;
                            cb_fieldrequire.Items[i].Selected = (ds2.Tables[0].Rows[j]["RequireField"].ToString() == "Y" ? true : false);



                            TextBox tb = (TextBox)Table2.FindControl("default-" + i.ToString());
                            if (tb != null)
                            {
                                tb.Text = ds2.Tables[0].Rows[j]["defaultval"].ToString();
                            }

                            cb_fieldafter.Items[i].Selected = true;
                            cb_fieldafter.Items[i].Selected = (ds2.Tables[0].Rows[j]["Afteraction"].ToString() == "Y" ? true : false);

                        }
                    }
                }
            }
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        rowid_txt.Text = GridView1.DataKeys[Row.RowIndex][0].ToString();
        lb_message.Text = "";
        lb_message0.Text = "";
        getCururrentData();
        lb_eventname.Text = "Step : " + GridView1.DataKeys[Row.RowIndex][1].ToString(); //((Label)GridView1.Rows[Row.RowIndex].Cells[0].FindControl("Label1")).Text;



        setCodeMirror();

    }


    private void setCodeMirror()
    {


        String save = "CodeMirror.fromTextArea(document.getElementById('" + txt_EventBeforeSaveCallJavascriptFunction.ClientID + @"'), {
            lineNumbers: true,
            scrollbarStyle: 'simple',
            extraKeys: { 'Ctrl-Space': 'autocomplete' },
            mode: { name: 'javascript', globalVars: true },
            extraKeys: {
            'F11': function(cm) { cm.setOption('fullScreen', !cm.getOption('fullScreen'));},
            'Esc': function(cm) { if (cm.getOption('fullScreen')) cm.setOption('fullScreen', false);}
            }

        }).on('blur', function(cm){document.getElementById('" + txt_EventBeforeSaveCallJavascriptFunction.ClientID + @"').innerHTML=cm.getValue();});";

        String reject = "CodeMirror.fromTextArea(document.getElementById('" + txt_EventBeforeRejectCallJavascriptFunction.ClientID + @"'), {
            lineNumbers: true,
            scrollbarStyle: 'simple',
            extraKeys: { 'Ctrl-Space': 'autocomplete' },
            mode: { name: 'javascript', globalVars: true },
            extraKeys: {
            'F11': function(cm) { cm.setOption('fullScreen', !cm.getOption('fullScreen'));},
            'Esc': function(cm) { if (cm.getOption('fullScreen')) cm.setOption('fullScreen', false);}
            }

        }).on('blur', function(cm){document.getElementById('" + txt_EventBeforeRejectCallJavascriptFunction.ClientID + @"').innerHTML=cm.getValue();});";


        String approve = "CodeMirror.fromTextArea(document.getElementById('" + txt_EventBeforeApproveCallJavascriptFunction.ClientID + @"'), {
            lineNumbers: true,
            scrollbarStyle: 'simple',
            extraKeys: { 'Ctrl-Space': 'autocomplete' },
            mode: { name: 'javascript', globalVars: true },
            extraKeys: {
            'F11': function(cm) { cm.setOption('fullScreen', !cm.getOption('fullScreen'));},
            'Esc': function(cm) { if (cm.getOption('fullScreen')) cm.setOption('fullScreen', false);}
            }

        }).on('blur', function(cm){document.getElementById('" + txt_EventBeforeApproveCallJavascriptFunction.ClientID + @"').innerHTML=cm.getValue();});";



        String st1 = @"<script language='javascript' type='text/javascript'>

                                            try
{
                                            Sys.Application.add_load(func);
                                            function func() {
                                           Sys.Application.remove_load(func);


CodeMirror.fromTextArea(document.getElementById('" + txt_javaonload.ClientID + @"'), {
            lineNumbers: true,
            scrollbarStyle: 'simple',
            extraKeys: { 'Ctrl-Space': 'autocomplete' },
            mode: { name: 'javascript', globalVars: true },
            extraKeys: {
            'F11': function(cm) { cm.setOption('fullScreen', !cm.getOption('fullScreen'));},
            'Esc': function(cm) { if (cm.getOption('fullScreen')) cm.setOption('fullScreen', false);}
            }

        }).on('blur', function(cm){document.getElementById('" + txt_javaonload.ClientID + @"').innerHTML=cm.getValue();});


" + (cb_enablesave.Checked ? save : "") + @"
" + (cb_enablereject.Checked ? reject : "") + @"
" + (cb_enableapprove.Checked ? approve : "") + @"

                                 } 
}
catch(e)
{}
                                            </script>";

        ScriptManager.RegisterClientScriptBlock(this, GetType(), "error", st1.ToString(), false);

    }

    protected void cb_enablesave_CheckedChanged(object sender, EventArgs e)
    {
        div_eventsave.Visible = cb_enablesave.Checked;

        if (!cb_enablesave.Checked)
        {
            AliasNameSave.Text = "";
        }

        refreshdatab();
        
    }
    protected void cb_enableapprove_CheckedChanged(object sender, EventArgs e)
    {
        div_eventapprove.Visible = cb_enableapprove.Checked;
        if (!cb_enableapprove.Checked)
        {
            AliasNameApprove.Text = "";
        }
        refreshdatab();
      
    }
    protected void cb_enablereject_CheckedChanged(object sender, EventArgs e)
    {
        div_eventreject.Visible = cb_enablereject.Checked;
        if (!cb_enablereject.Checked)
        {
            AliasNameReject.Text = "";
        }
        refreshdatab();
       
    }

    private void refreshdatab()
    {
        DataSet ds = SQLExecuteReader("system","SELECT   BindRowID, BindName FROM   DataBindValue");

        Table1.Rows.Clear();
        Table2.Rows.Clear();
        for (int i = 0; i < cb_fieldlist.Items.Count; i++)
        {
            DropDownList dl = new DropDownList();
            dl.DataSource = ds.Tables[0];
            dl.DataTextField = "BindName";
            dl.DataValueField = "BindRowID";
            dl.DataBind();
            dl.Items.Insert(0,new ListItem("",""));
 
            dl.ID = "binds-" + i.ToString();
            
            dl.Width = 80;
            if (selectb.IndexOf(i.ToString() + ",") < 0)
            {
                dl.Visible = false;
            }

            if (Request.Form["ctl00$ContentPlaceHolder1$binds-" + i.ToString()] != null)
            {
                dl.SelectedValue = Request.Form["ctl00$ContentPlaceHolder1$binds-" + i.ToString()];
                dl.Visible = true;
            }



            TextBox tbb = new TextBox();
            tbb.ID = "default-" + i.ToString();
            tbb.Width = 50;
            tbb.CssClass = "defaultv";
            tbb.Attributes.Add("ondblclick", "on(this)");
            tbb.ToolTip = "Dbl Click จะดูแบบ ขยายช่องยาวชึ้น";

            if (Request.Form["ctl00$ContentPlaceHolder1$default-" + i.ToString()] != null)
            {
                tbb.Text = Request.Form["ctl00$ContentPlaceHolder1$default-" + i.ToString()];
            }
         
            TableRow tr = new TableRow();
            tr.Cells.Add(new TableCell());
           
            Table1.Rows.Add(tr);
            Table1.Rows[i].Cells[0].Controls.Add(dl);

            if (dl.Visible == false)
            {
                Literal lt = new Literal();
                lt.Text = "-";
                Table1.Rows[i].Cells[0].Controls.Add(lt);
            }

            TableRow tr2 = new TableRow();
            tr2.Cells.Add(new TableCell());
            Table2.Rows.Add(tr2);
            Table2.Rows[i].Cells[0].Controls.Add(tbb);


          

        }
        setCodeMirror();
    }
    protected void dl_templateid_SelectedIndexChanged(object sender, EventArgs e)
    {
        cb_fieldrequire.Items.Clear();
        cb_fieldafter.Items.Clear();
   
        selectb = "";
        DataSet ds = this.SQLExecuteReader("system", "select *,'{'+ObjectName+'}' as ObjectName1 from TemplateObject where TemplateRowId='" + dl_templateid.SelectedValue + "';select ObjectRowId,BindRowID,defaultval,Afteraction from StepEnableObject where StepRowId='" + rowid_txt.Text + "' and TemplateRowId='" + dl_templateid.SelectedValue + "';");

            if (ds.Tables[0].Rows.Count > 0)
            {
                cb_fieldlist.Items.Clear();
                selectb = "";
                refreshdatab();
                cb_fieldlist.DataSource = ds.Tables[0];
                cb_fieldlist.DataTextField = "ObjectName1";
                cb_fieldlist.DataValueField = "ObjectRowId";
                cb_fieldlist.DataBind();
                refreshdatab();
                for (int i = 0; i < cb_fieldlist.Items.Count; i++)
                {

                    cb_fieldrequire.Items.Add(new ListItem("Yes", cb_fieldlist.Items[i].Value));
                    cb_fieldafter.Items.Add(new ListItem("", cb_fieldlist.Items[i].Value));
                    try
                    {
                        cb_fieldafter.Items[cb_fieldafter.Items.Count - 1].Attributes.Add("title", "Checkbox หากต้องการใส่ใส่ค่า default ตอนกดปุ่ม Action Save/Approve/Reject");
                    }
                    catch { }
                    if (ds.Tables[0].Rows[i]["ObjectType"].ToString().ToLower() == "select")
                    {
                        selectb = selectb + "," + i.ToString();
                        DropDownList dl = (DropDownList)Table1.FindControl("binds-" + i.ToString());
                        

                        if (dl != null)
                        {
                            ds.Tables[1].DefaultView.RowFilter = "ObjectRowId='" + cb_fieldlist.Items[i].Value + "'";

                           // DataSet ds2 = this.SQLExecuteReader("system", "select BindRowID from StepEnableObject where StepRowId='" + rowid_txt.Text + "' and TemplateRowId='" + dl_templateid.SelectedValue + "' and ObjectRowId='" + cb_fieldlist.Items[i].Value + "'");
                            if (ds.Tables[1].DefaultView.Count > 0)
                            {
                                dl.SelectedValue = ds.Tables[1].DefaultView[0]["BindRowID"].ToString();
                            }
                            ds.Tables[1].DefaultView.RowFilter = "";
                            dl.Visible = true;
                            if (Table1.Rows[i].Cells[0].Controls.Count > 1)
                            {
                                Table1.Rows[i].Cells[0].Controls.RemoveAt(1);
                            }
                        }
                    }
                    else
                    {
                        if (Table1.Rows[i] != null)
                        {
                            Table1.Rows[i].Cells[0].Controls.Clear();
                            Literal lt = new Literal();
                            lt.Text = "-";
                            Table1.Rows[i].Cells[0].Controls.Add(lt);
                        }
                    }

                }

                if (selectb == "")
                {
                    Table1.Rows.Clear();
                }
                
            }
            else
            {
                cb_fieldlist.Items.Clear();
            }

            btn_preview.OnClientClick = "window.open('PreviewTemplate.aspx?rowid=" + dl_templateid.SelectedValue + "');return false;";

           // refreshdatab();
   
    }
    protected void btn_save_Click(object sender, EventArgs e)
    {
        lb_message.Visible = false;
        lb_message0.Visible = false;
        if (rowid_txt.Text != "" &&  Request.QueryString["rowid"] != null)
        {
            if (dl_templateid.SelectedValue == "Please select")
            {
                lb_message.Text = "**  Please Selected template";
                lb_message.Visible = true;
                lb_message.ForeColor = System.Drawing.Color.Red;

                lb_message0.Text = "**  Please Selected template";
                lb_message0.Visible = true;
                lb_message0.ForeColor = System.Drawing.Color.Red;
                refreshdatab();
   
                return;
            }


            if (rb_stepautostart.SelectedIndex == 2 && (txt_TiggerStartStepCondition.Text == "" || dl_TiggerStartStepConditionButton.SelectedIndex==0))
            {
                lb_message.Text = lb_message0.Text = "**  Please fill TriggerStartStepByCondition && Auto Press Button!!";
                lb_message.Visible = lb_message0.Visible=true;
                lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                refreshdatab();

                return;
            }

            if (rb_stepautostart.SelectedIndex == 1 &&  dl_TiggerStartStepConditionButton0.SelectedIndex == 0)
            {
                lb_message.Text = lb_message0.Text = "**  Please fill Auto Press Button!!";
                lb_message.Visible = lb_message0.Visible = true;
                lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                refreshdatab();

                return;
            }

            //Update add function tigger

            if (TiggerSave.Text != "")
            {
                DataSet check = SQLExecuteReader("system", " select case when   (" + TiggerSave.Text + ") then 'Y' else 'N' end as rel");
                try { String s = check.Tables[0].Rows[0]["rel"].ToString(); }
                catch
                {
                    lb_message.Text = lb_message0.Text = "**  Tigger on Save error";
                    lb_message.Visible = lb_message0.Visible = true;
                    lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                    refreshdatab();
               
                    return;
                }

            }
            if (TiggerApprove.Text != "")
            {
                DataSet check = SQLExecuteReader("system", " select case when   (" + TiggerApprove.Text + ") then 'Y' else 'N' end as rel");
                try { String s = check.Tables[0].Rows[0]["rel"].ToString(); }
                catch
                {

                    lb_message.Text = lb_message0.Text = "**  Tigger on Approve error";
                    lb_message.Visible = lb_message0.Visible = true;
                    lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                    refreshdatab();
                    setCodeMirror();
                    return;
                }

            }
            if (TiggerReject.Text != "")
            {
                DataSet check = SQLExecuteReader("system", " select case when   (" + TiggerReject.Text + ") then 'Y' else 'N' end as rel");
                try { String s = check.Tables[0].Rows[0]["rel"].ToString(); }
                catch
                {
                    lb_message.Text = lb_message0.Text = "**  Tigger on Reject error";
                    lb_message.Visible = lb_message0.Visible = true;
                    lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                    refreshdatab();
                    setCodeMirror();
                    return;
                }

            }

            if (txt_TiggerStartStepCondition.Text != "")
            {
                DataSet check = SQLExecuteReader("system", " select case when   (" + txt_TiggerStartStepCondition.Text + ") then 'Y' else 'N' end as rel");
                try { String s = check.Tables[0].Rows[0]["rel"].ToString(); }
                catch
                {
                    lb_message.Text = lb_message0.Text = "**  Paralle Start Flow Condition Error ";
                    lb_message.Visible = lb_message0.Visible = true;
                    lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                    refreshdatab();
                    setCodeMirror();
                    return;
                }

            }

            if ((TiggerSave.Text != "" || TiggerApprove.Text != "" || TiggerReject.Text != "") && (!CheckTiggerSave.Checked && !CheckTiggerApprove.Checked && !CheckTiggerReject.Checked))
            {
                lb_message.Text = lb_message0.Text = "**  Please checkbox check trigger at least 1 button! ";
                lb_message.Visible = lb_message0.Visible = true;
                lb_message.ForeColor = lb_message0.ForeColor = System.Drawing.Color.Red;
                refreshdatab();
                setCodeMirror();
                return;
            }

            //endUpdate add function tigger

            if (cb_defaultstart.Checked)
            {
                SQLeExecuteNonQuery("system", @"update WorkFlowStep set DefaultStart='N' where WFRowId='" + Request.QueryString["rowid"].ToString() + @"' ;");
                                                
            }
            
                SQLeExecuteNonQuery("system", @"update WorkFlowStep set  DefaultStart='" + (cb_defaultstart.Checked ? "Y" : "N") + "', TemplateRowId='" + dl_templateid.SelectedValue + "',AllowUseUploadMgr='" + (cb_uploadmgr.Checked ? "Y" : "N") + "', AllowMoreUploadFile='" + (cb_uploadot.Items[0].Selected ? "Y" : "N") + "', AllowDeleteFile='" + (cb_uploadot.Items[1].Selected ? "Y" : "N") + "',AutoExpand='" + (cb_uploadot.Items[2].Selected ? "Y" : "N") + @"',
                                                    guideline='" + FreeTextBox1.Text.ToString().Replace("'", "''") + "',JavaScriptOnload='" + txt_javaonload.Text.ToString().Replace("'", "''") + "',DinamicKeyValue='" + DinamicKey.Text + "',passvalueoldtemplate='" + (passvalueoldtemplate.Checked ? "Y" : "N") + "',TriggerSave='" + TiggerSave.Text.Replace("'", "''") + "',TriggerApprove='" + TiggerApprove.Text.Replace("'", "''") + "',TriggerReject='" + TiggerReject.Text.Replace("'", "''") + @"',
                                                    HideButtonSave='" + (HideButtonSave.Checked ? "Y" : "N") + "', CheckTiggerSave='" + (CheckTiggerSave.Checked ? "Y" : "N") + "', HideButtonApprove='" + (HideButtonApprove.Checked ? "Y" : "N") + "', CheckTiggerApprove='" + (CheckTiggerApprove.Checked ? "Y" : "N") + "', HideButtonReject='" + (HideButtonReject.Checked ? "Y" : "N") + "', CheckTiggerReject='" + (CheckTiggerReject.Checked ? "Y" : "N") + @"',
                                                    TiggerStartStepCondition='" + txt_TiggerStartStepCondition.Text.Replace("'", "''") + @"',TiggerStartStepConditionButton='" + (dl_TiggerStartStepConditionButton.Text + dl_TiggerStartStepConditionButton0.Text).Replace("------", "").Replace("SaveSave", "Save").Replace("ApproveApprove", "Approve").Replace("RejectReject", "Reject") + @"',StepAutoStart='" + rb_stepautostart.SelectedValue.ToString() + @"' 
                                                    where StepRowId='" + rowid_txt.Text + "';");


                if (((Button)sender).ID.ToString() == "btn_save1" || ((Button)sender).ID.ToString() == "btn_save")
                {
                    string enalble = "";
                    for (int i = 0; i < cb_fieldlist.Items.Count; i++)
                    {

                        if (cb_fieldlist.Items[i].Selected)
                        {
                            string bindd = "null";
                            string defaultv = "null";

                            if (Request.Form["ctl00$ContentPlaceHolder1$binds-" + i.ToString()] != null)
                            {
                                if (Request.Form["ctl00$ContentPlaceHolder1$binds-" + i.ToString()] != "")
                                {
                                    bindd = "'" + Request.Form["ctl00$ContentPlaceHolder1$binds-" + i.ToString()] + "'";
                                }
                            }

                            if (Request.Form["ctl00$ContentPlaceHolder1$default-" + i.ToString()] != null)
                            {
                                if (Request.Form["ctl00$ContentPlaceHolder1$default-" + i.ToString()] != "")
                                {
                                    defaultv = "'" + Request.Form["ctl00$ContentPlaceHolder1$default-" + i.ToString()].Replace("'", "''") + "'";
                                }
                            }



                            enalble += "insert into StepEnableObject (StepRowId, TemplateRowId, ObjectRowId, BGColor,Requirefield,BindRowID,defaultval,Afteraction) values ('" + rowid_txt.Text + "', '" + dl_templateid.SelectedValue + "', '" + cb_fieldlist.Items[i].Value + "','Yellow','" + (cb_fieldrequire.Items[i].Selected ? "Y" : "N") + "'," + bindd + "," + defaultv + ",'" + (cb_fieldafter.Items[i].Selected ? "Y" : "N") + "');";
                        }
                    }
                    SQLeExecuteNonQuery("system", "delete from StepEnableObject where StepRowId='" + rowid_txt.Text + "';" + enalble);
                }

            if (cb_enablesave.Checked)
            {
                SQLeExecuteNonQuery("system", ("update WorkFlowStep set EventAfterSaveEmailSubject='" + txt_EventAfterSaveEmailSubject.Text.Replace("'", "''") + "',EnableSave='Y',AliasNameSave='" + AliasNameSave.Text.Replace("'", "''") + "', EventBeforeSaveCallJavascriptFunction='" + txt_EventBeforeSaveCallJavascriptFunction.Text.Replace("'", "''") + "',EventAfterSaveMailToPosition='" + dl_EventAfterSaveMailToPosition.SelectedValue + "',DinamicFiledSaveValueToselect='" + (dl_EventAfterSaveMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledSaveValueToselect.Text.Replace("'", "''") : "") + "', EventAfterSaveEmailContent='" + txt_EventAfterSaveEmailContent.Text.Replace("''", "''").Replace("\n", "") + "', EventAfterSaveGotoStep='" + dl_EventAfterSaveGotoStep.SelectedValue + "', EventAfterSaveExecAtServer='" + dl_EventAfterSaveExecAtServer.SelectedValue + "', EventAfterSaveExecCommand='" + txt_EventAfterSaveExecCommand.Text.Replace("'", "''") + "',EventAfterSaveCC1MailToPosition='" + dl_EventAfterSaveCC1MailToPosition.SelectedValue + "',DinamicFiledSaveCC1ValueToselect='" + (dl_EventAfterSaveCC1MailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledSaveCC1ValueToselect.Text : "") + "', EventAfterSaveInformMailToPosition='" + dl_EventAfterSaveInformMailToPosition.SelectedValue + "',DinamicFiledSaveInformValueToselect='" + (dl_EventAfterSaveInformMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledSaveInformValueToselect.Text : "") + "',EventAfterSaveInformEmailContent='" + txt_EventAfterSaveInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',DinamicFiledSaveValueToStep='" + (dl_EventAfterSaveGotoStep.SelectedValue.Replace("*HOLD*", "UseKey") == "UseKey" ? txt_DinamicFiledSaveValueToStep.Text : "") + "',EventAfterSaveUpdateValueETC1='" + txt_EventAfterSaveUpdateValueETC1.Text + "',EventAfterSaveUpdateValueETC2='" + txt_EventAfterSaveUpdateValueETC2.Text + "',EventAfterSaveUpdateValueETC3='" + txt_EventAfterSaveUpdateValueETC3.Text + "',EventAfterSaveUpdateValueETC4='" + txt_EventAfterSaveUpdateValueETC4.Text + "',EventAfterSaveUpdateValueETC5='" + txt_EventAfterSaveUpdateValueETC5.Text + @"',CheckConditionAutoStartSave='" + (CheckConditionAutoStartSave.Checked ? "Y" : "N") + "',EventSaveDonotSendMail='" + (ck_Save_dont_send.Checked ? "Y" : "N") + "'  where StepRowId='" + rowid_txt.Text + "';").Replace("'Please select'", "null").Replace("'UseKey'", "null").Replace("'*HOLD*'", "null"));
            }
            else
            {
                SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterSaveEmailSubject=null,EnableSave='N',AliasNameSave='', EventBeforeSaveCallJavascriptFunction=null,EventAfterSaveMailToPosition=null, DinamicFiledSaveValueToselect=null,EventAfterSaveEmailContent=null, EventAfterSaveGotoStep=null, EventAfterSaveExecAtServer=null, EventAfterSaveExecCommand=null,EventAfterSaveCC1MailToPosition=null, DinamicFiledSaveCC1ValueToselect=null, EventAfterSaveInformMailToPosition=null, DinamicFiledSaveInformValueToselect=null,EventAfterSaveInformEmailContent=null,DinamicFiledSaveValueToStep=null,EventAfterSaveUpdateValueETC1=null,EventAfterSaveUpdateValueETC2=null,EventAfterSaveUpdateValueETC3=null,EventAfterSaveUpdateValueETC4=null,EventAfterSaveUpdateValueETC5=null,CheckConditionAutoStartSave='N',EventSaveDonotSendMail='N' where StepRowId='" + rowid_txt.Text + "';");

            }

            if (cb_enableapprove.Checked)
            {
                SQLeExecuteNonQuery("system", ("update WorkFlowStep set EventAfterApproveEmailSubject='" + txt_EventAfterApproveEmailSubject.Text.Replace("'", "''") + "',EnableApprove='Y',AliasNameApprove='" + AliasNameApprove.Text.Replace("'", "''") + "', EventBeforeApproveCallJavascriptFunction='" + txt_EventBeforeApproveCallJavascriptFunction.Text.Replace("'", "''") + "',EventAfterApproveMailToPosition='" + dl_EventAfterApproveMailToPosition.SelectedValue + "',DinamicFiledApproveValueToselect='" + (dl_EventAfterApproveMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledApproveValueToselect.Text.Replace("'", "''") : "") + "', EventAfterApproveEmailContent='" + txt_EventAfterApproveEmailContent.Text.Replace("''", "''").Replace("\n", "") + "', EventAfterApproveGotoStep='" + dl_EventAfterApproveGotoStep.SelectedValue + "', EventAfterApproveExecAtServer='" + dl_EventAfterApproveExecAtServer.SelectedValue + "', EventAfterApproveExecCommand='" + txt_EventAfterApproveExecCommand.Text.Replace("'", "''") + "',EventAfterApproveCC1MailToPosition='" + dl_EventAfterApproveCC1MailToPosition.SelectedValue + "',DinamicFiledApproveCC1ValueToselect='" + (dl_EventAfterApproveCC1MailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledApproveCC1ValueToselect.Text : "") + "', EventAfterApproveInformMailToPosition='" + dl_EventAfterApproveInformMailToPosition.SelectedValue + "',DinamicFiledApproveInformValueToselect='" + (dl_EventAfterApproveInformMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledApproveInformValueToselect.Text : "") + "',EventAfterApproveInformEmailContent='" + txt_EventAfterApproveInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',DinamicFiledApproveValueToStep='" + (dl_EventAfterApproveGotoStep.SelectedValue.Replace("*HOLD*", "UseKey") == "UseKey" ? txt_DinamicFiledApproveValueToStep.Text : "") + "',EventAfterApproveUpdateValueETC1='" + txt_EventAfterApproveUpdateValueETC1.Text + "',EventAfterApproveUpdateValueETC2='" + txt_EventAfterApproveUpdateValueETC2.Text + "',EventAfterApproveUpdateValueETC3='" + txt_EventAfterApproveUpdateValueETC3.Text + "',EventAfterApproveUpdateValueETC4='" + txt_EventAfterApproveUpdateValueETC4.Text + "',EventAfterApproveUpdateValueETC5='" + txt_EventAfterApproveUpdateValueETC5.Text + @"',CheckConditionAutoStartApprove='" + (CheckConditionAutoStartApprove.Checked ? "Y" : "N") + "',EventApproveDonotSendMail='" + (ck_Approve_dont_send.Checked ? "Y" : "N") + "'  where StepRowId='" + rowid_txt.Text + "';").Replace("'Please select'", "null").Replace("'UseKey'", "null").Replace("'*HOLD*'", "null"));
            }
            else
            {
                SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterApproveEmailSubject=null,EnableApprove='N',AliasNameApprove='',EventBeforeApproveCallJavascriptFunction=null, EventAfterApproveMailToPosition=null,DinamicFiledApproveValueToselect=null, EventAfterApproveEmailContent=null, EventAfterApproveGotoStep=null, EventAfterApproveExecAtServer=null, EventAfterApproveExecCommand=null,EventAfterApproveCC1MailToPosition=null, DinamicFiledApproveCC1ValueToselect=null, EventAfterApproveInformMailToPosition=null, DinamicFiledApproveInformValueToselect=null,EventAfterApproveInformEmailContent=null,DinamicFiledApproveValueToStep=null,EventAfterApproveUpdateValueETC1=null,EventAfterApproveUpdateValueETC2=null,EventAfterApproveUpdateValueETC3=null,EventAfterApproveUpdateValueETC4=null,EventAfterApproveUpdateValueETC5=null,CheckConditionAutoStartApprove='N',EventApproveDonotSendMail='N' where StepRowId='" + rowid_txt.Text + "';");

            }

            if (cb_enablereject.Checked)
            {
                SQLeExecuteNonQuery("system", ("update WorkFlowStep set EventAfterRejectEmailSubject='" + txt_EventAfterRejectEmailSubject.Text.Replace("'", "''") + "',EnableReject='Y',AliasNameReject='" + AliasNameReject.Text.Replace("'", "''") + "',EventBeforeRejectCallJavascriptFunction='" + txt_EventBeforeRejectCallJavascriptFunction.Text.Replace("'", "''") + "', EventAfterRejectMailToPosition='" + dl_EventAfterRejectMailToPosition.SelectedValue + "',DinamicFiledRejectValueToselect='" + (dl_EventAfterRejectMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledRejectValueToselect.Text.Replace("'", "''") : "") + "', EventAfterRejectEmailContent='" + txt_EventAfterRejectEmailContent.Text.Replace("''", "''").Replace("\n", "") + "', EventAfterRejectGotoStep='" + dl_EventAfterRejectGotoStep.SelectedValue + "', EventAfterRejectExecAtServer='" + dl_EventAfterRejectExecAtServer.SelectedValue + "', EventAfterRejectExecCommand='" + txt_EventAfterRejectExecCommand.Text.Replace("'", "''") + "',EventAfterRejectCC1MailToPosition='" + dl_EventAfterRejectCC1MailToPosition.SelectedValue + "',DinamicFiledRejectCC1ValueToselect='" + (dl_EventAfterRejectCC1MailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledRejectCC1ValueToselect.Text : "") + "', EventAfterRejectInformMailToPosition='" + dl_EventAfterRejectInformMailToPosition.SelectedValue + "',DinamicFiledRejectInformValueToselect='" + (dl_EventAfterRejectInformMailToPosition.SelectedValue == "UseKey" ? txt_DinamicFiledRejectInformValueToselect.Text : "") + "',EventAfterRejectInformEmailContent='" + txt_EventAfterRejectInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',DinamicFiledRejectValueToStep='" + (dl_EventAfterRejectGotoStep.SelectedValue.Replace("*HOLD*", "UseKey") == "UseKey" ? txt_DinamicFiledRejectValueToStep.Text : "") + "',EventAfterRejectUpdateValueETC1='" + txt_EventAfterRejectUpdateValueETC1.Text + "',EventAfterRejectUpdateValueETC2='" + txt_EventAfterRejectUpdateValueETC2.Text + "',EventAfterRejectUpdateValueETC3='" + txt_EventAfterRejectUpdateValueETC3.Text + "',EventAfterRejectUpdateValueETC4='" + txt_EventAfterRejectUpdateValueETC4.Text + "',EventAfterRejectUpdateValueETC5='" + txt_EventAfterRejectUpdateValueETC5.Text + @"',CheckConditionAutoStartReject='" + (CheckConditionAutoStartReject.Checked ? "Y" : "N") + "',EventRejectDonotSendMail='" + (ck_Reject_dont_send.Checked ? "Y" : "N") + "'  where StepRowId='" + rowid_txt.Text + "';").Replace("'Please select'", "null").Replace("'UseKey'", "null").Replace("'*HOLD*'", "null"));
            }
            else
            {
                SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterRejectEmailSubject=null,EnableReject='N',AliasNameReject='',EventBeforeRejectCallJavascriptFunction=null, EventAfterRejectMailToPosition=null,DinamicFiledRejectValueToselect=null, EventAfterRejectEmailContent=null, EventAfterRejectGotoStep=null, EventAfterRejectExecAtServer=null, EventAfterRejectExecCommand=null,EventAfterRejectCC1MailToPosition=null, DinamicFiledRejectCC1ValueToselect=null, EventAfterRejectInformMailToPosition=null, DinamicFiledRejectInformValueToselect=null,EventAfterRejectInformEmailContent=null,DinamicFiledRejectValueToStep=null,EventAfterRejectUpdateValueETC1=null,EventAfterRejectUpdateValueETC2=null,EventAfterRejectUpdateValueETC3=null,EventAfterRejectUpdateValueETC4=null,EventAfterRejectUpdateValueETC5=null,CheckConditionAutoStartReject='N',EventRejectDonotSendMail='N' where StepRowId='" + rowid_txt.Text + "';");

            }




            //dl_templateid_SelectedIndexChanged(this, new EventArgs());
            lb_message.Text = "Save completed";
            lb_message.Visible = true;
            lb_message.ForeColor = System.Drawing.Color.Green;

            lb_message0.Text = "Save completed";
            lb_message0.Visible = true;
            lb_message0.ForeColor = System.Drawing.Color.Green;
           
        }

        refreshdatab();

    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void btn_back0_Click(object sender, EventArgs e)
    {
        Response.Redirect("workflowlist.aspx");
        
    }
    protected void btn_new_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 2;
        btn_save0.Text = "Save";
        txt_eventname.Text = "";
        txt_seq.Text = "1";
    }
    protected void btn_save0_Click(object sender, EventArgs e)
    {
        if (btn_save0.Text == "Save")
        {
            SQLeExecuteNonQuery("system", "insert into WorkFlowStep (StepName,WFRowId,seq,CrBy,udby,GroupName) values ('" + txt_eventname.Text.Replace("'", "''") + "','" + Request.QueryString["rowid"].ToString() + "','" + txt_seq.Text + "','" + isWho() + "','" + isWho() + "','"+txt_group.Text.Replace("'","''")+"');");

        }
        else
        {
            SQLeExecuteNonQuery("system", "update WorkFlowStep set StepName='" + txt_eventname.Text.Replace("'", "''") + "',seq='" + txt_seq.Text + "',GroupName='"+txt_group.Text.Replace("'","''")+"' where StepRowId='" + HiddenField1.Value + "'");

        }
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        
        DataSet ds = SQLExecuteReader("system", "select * from WorkFlowStep where StepRowId='" + GridView1.DataKeys[Row.RowIndex][0].ToString() + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            txt_eventname.Text = ds.Tables[0].Rows[0]["StepName"].ToString();
            txt_seq.Text = ds.Tables[0].Rows[0]["seq"].ToString();
            txt_group.Text = ds.Tables[0].Rows[0]["GroupName"].ToString();
            HiddenField1.Value = ds.Tables[0].Rows[0]["StepRowId"].ToString();
        }
        btn_save0.Text =  "Save Edit";
        MultiView1.ActiveViewIndex = 2;
    }
    protected void btn_del_Click(object sender, EventArgs e)
    {
        SQLeExecuteNonQuery("system", "delete from StepEnableObject where StepRowId='" + HiddenField1.Value + "' ;delete from WorkFlowStep where StepRowId='" + HiddenField1.Value + "';");
        MultiView1.ActiveViewIndex = 0;
        refresh();
    }

    protected void dl_EventAfterSaveMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        SaveConditionVal.Visible = (dl_EventAfterSaveMailToPosition.SelectedValue == "UseKey"?true:false);
        refreshdatab();
    }
    protected void dl_EventAfterApproveMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        ApproveConditionVal.Visible = (dl_EventAfterApproveMailToPosition.SelectedValue == "UseKey" ? true : false);
        refreshdatab();
    }
    protected void dl_EventAfterRejectMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        RejectConditionVal.Visible = (dl_EventAfterRejectMailToPosition.SelectedValue == "UseKey" ? true : false);
        refreshdatab();
    }
    protected void dl_EventAfterSaveCC1MailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        SaveConditionValCC1.Visible = (dl_EventAfterSaveCC1MailToPosition.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledSaveCC1ValueToselect.Text = "";
        refreshdatab();
    }
    protected void dl_EventAfterSaveInformMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        SaveConditionValInform.Visible = (dl_EventAfterSaveInformMailToPosition.SelectedValue == "UseKey" ? true : false);
        divSave_informmailcontent.Visible = (dl_EventAfterSaveInformMailToPosition.SelectedValue == "Please select" ? false : true);
        txt_EventAfterSaveInformEmailContent.Text = (divSave_informmailcontent.Visible ? "Dear All," : "");
        refreshdatab();
    }
    protected void dl_EventAfterApproveCC1MailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        ApproveConditionValCC1.Visible = (dl_EventAfterApproveCC1MailToPosition.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledApproveCC1ValueToselect.Text = "";
        refreshdatab();
    }
    protected void dl_EventAfterApproveInformMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        ApproveConditionValInform.Visible = (dl_EventAfterApproveInformMailToPosition.SelectedValue == "UseKey" ? true : false);
        divApprove_informmailcontent.Visible = (dl_EventAfterApproveInformMailToPosition.SelectedValue == "Please select" ? false : true);
        txt_EventAfterApproveInformEmailContent.Text = (divApprove_informmailcontent.Visible ? "Dear All," : "");
        refreshdatab();
    }
    protected void dl_EventAfterRejectCC1MailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        RejectConditionValCC1.Visible = (dl_EventAfterRejectCC1MailToPosition.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledRejectCC1ValueToselect.Text = "";
        refreshdatab();
    }
    protected void dl_EventAfterRejectInformMailToPosition_SelectedIndexChanged(object sender, EventArgs e)
    {
        RejectConditionValInform.Visible = (dl_EventAfterRejectInformMailToPosition.SelectedValue == "UseKey" ? true : false);
        divReject_informmailcontent.Visible = (dl_EventAfterRejectInformMailToPosition.SelectedValue == "Please select" ? false : true);
        txt_EventAfterRejectInformEmailContent.Text = (divReject_informmailcontent.Visible ? "Dear All," : "");
        refreshdatab();
    }
    protected void cb_uploadmgr_CheckedChanged(object sender, EventArgs e)
    {
        cb_uploadot.Enabled = !cb_uploadot.Enabled;
        if (!cb_uploadot.Enabled)
        {
            cb_uploadot.ClearSelection();
        }
        refreshdatab();
    }
    protected void dl_EventAfterSaveGotoStep_SelectedIndexChanged(object sender, EventArgs e)
    {
        stepcondition_save.Visible = (dl_EventAfterSaveGotoStep.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledSaveValueToStep.Text = (dl_EventAfterSaveGotoStep.SelectedValue == "*HOLD*" ? "**HOLD**" : "");
        refreshdatab();
    }
    protected void dl_EventAfterApproveGotoStep_SelectedIndexChanged(object sender, EventArgs e)
    {
        stepcondition_approve.Visible = (dl_EventAfterApproveGotoStep.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledApproveValueToStep.Text = (dl_EventAfterApproveGotoStep.SelectedValue == "*HOLD*" ? "**HOLD**" : "");
        refreshdatab();
    }
    protected void dl_EventAfterRejectGotoStep_SelectedIndexChanged(object sender, EventArgs e)
    {
        stepcondition_reject.Visible = (dl_EventAfterRejectGotoStep.SelectedValue == "UseKey" ? true : false);
        txt_DinamicFiledRejectValueToStep.Text = (dl_EventAfterRejectGotoStep.SelectedValue == "*HOLD*" ? "**HOLD**" : "");
        refreshdatab();
    }
    protected void Unnamed1_Click(object sender, EventArgs e)
    {
        refresh();
    }
    protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        

            switch(rb_stepautostart.SelectedValue.ToString())
            {

                case "continue":
                    div_TriggerStartStepByCondition.Visible = false;
                    txt_TiggerStartStepCondition.Text = "";
                    dl_TiggerStartStepConditionButton.SelectedIndex=0;

                    div_TriggerStartStepByCondition2.Visible = true;
                    dl_TiggerStartStepConditionButton0.SelectedIndex = 0;
                    break;
                case "condition":
                    div_TriggerStartStepByCondition.Visible = true;
                    txt_TiggerStartStepCondition.Text = "";
                    dl_TiggerStartStepConditionButton.SelectedIndex = 0;
                    div_TriggerStartStepByCondition2.Visible = false;
                    dl_TiggerStartStepConditionButton0.SelectedIndex = 0;
                   
                    break;
                default:
                    div_TriggerStartStepByCondition.Visible = false;
                    txt_TiggerStartStepCondition.Text = "";
                    dl_TiggerStartStepConditionButton.SelectedIndex = 0;
                    div_TriggerStartStepByCondition2.Visible = false;
                    dl_TiggerStartStepConditionButton0.SelectedIndex = 0;
                    break;

            }

        refreshdatab();
    }
    private void geouphead()
    {
        DataTable dt = (DataTable)GridView1.DataSource;
        int j = 0;
        for (int i = 0; i < dt.Rows.Count; i++)
      {
        
          if (tmpCategoryName != dt.Rows[i]["GroupName"].ToString())
            {
                tmpCategoryName = dt.Rows[i]["GroupName"].ToString();

                Table tbl = GridView1.Rows[i].Parent as Table;
                if (tbl != null)
                {
                    if (j > 0)
                    {
                        j++;


                        GridViewRow row1 = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                        TableCell cell1 = new TableCell();

                        // Span the row across all of the columns in the Gridview
                        cell1.ColumnSpan = this.GridView1.Columns.Count;

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
                    cell.Style.Add("background-color", "lightSlateGray");
                    cell.Style.Add("color", "floralwhite");

                    System.Web.UI.HtmlControls.HtmlGenericControl span = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
                    span.InnerHtml = "Group : "+tmpCategoryName;

                    cell.Controls.Add(span);
                    row.Cells.Add(cell);

                    tbl.Rows.AddAt(i+j , row);

                  
                    
                }
            }
                
            }
        
    }

    #region COPY Step
    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["rowid"] != null)
        {
            GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
            String sql = " BEGIN TRAN T1;";
            sql += getColumn("WorkFlowStep");
            sql += getRow("WorkFlowStep", " where StepRowId='" + GridView1.DataKeys[Row.RowIndex][0].ToString() + "'");
            sql += " COMMIT TRAN T1;";
            SQLeExecuteNonQuery("system", sql);
            DataSet ds = SQLExecuteReader("system", "select top 1 StepRowId from  WorkFlowStep where WFRowId='" + Request.QueryString["rowid"].ToString() + "' order by crdate desc");
            if (ds.Tables[0].Rows.Count > 0)
            {
                sql = " BEGIN TRAN T1;";
                sql += getColumn("StepEnableObject");
                sql += getRow("StepEnableObject", " where StepRowId='" + GridView1.DataKeys[Row.RowIndex][0].ToString() + "'").Replace(GridView1.DataKeys[Row.RowIndex][0].ToString(), ds.Tables[0].Rows[0]["StepRowId"].ToString());
                sql += " COMMIT TRAN T1;";
                SQLeExecuteNonQuery("system", sql);
                refresh();
            }
        }

    }

    private string getColumn(String tbname)
    {
        DataSet ds = SQLExecuteReader("system", @"SET NOCOUNT ON;  
                                                DECLARE @part1 nvarchar(max) = ''
                                                DECLARE @part2 nvarchar(max) = ''
                                                DECLARE @column nvarchar(404)
                                                DECLARE @type nvarchar(50)
                                                declare @tbname nvarchar(50)
                                                set @tbname='" + tbname + @"'



                                                DECLARE cur CURSOR FOR 
                                                    SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = @tbname

                                                OPEN cur
                                                FETCH NEXT FROM cur
                                                    INTO @column,@type

                                                WHILE @@FETCH_STATUS = 0
                                                BEGIN
                                                 if @part1='' 
	                                                set @part1 = 'insert into '+@tbname+' ('
                                                 else
	                                                set @part1 =  @part1+','
	                                                set @part1 = @part1+'['+@column+'] '

                                                    FETCH NEXT FROM cur into @column,@type
                                                END

                                                CLOSE cur
                                                DEALLOCATE cur

                                                select @part1+') values ' as val");

        if (ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0]["val"].ToString();
        }
        else
        {
            return "";
        }
    }

    private string getRow(String tbname, String where)
    {
        String outp = "";
        DataSet ds = SQLExecuteReader("system", "select * from " + tbname + " " + where + ";SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = '" + tbname + "'");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                outp = outp + @"
                    (";
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                   
                    if (ds.Tables[1].Rows[j]["COLUMN_NAME"].ToString().ToLower() == "crdate" || ds.Tables[1].Rows[j]["COLUMN_NAME"].ToString().ToLower() == "uddate")
                    {
                        outp = outp + "getdate(), ";
                    }
                    else if (ds.Tables[1].Rows[j]["DATA_TYPE"].ToString() == "datetime")
                    {
                        outp = outp + "'" + DateTime.Parse(ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                    }
                    else if ((ds.Tables[1].Rows[j]["COLUMN_NAME"].ToString().ToLower() == "steprowid" && tbname == "WorkFlowStep") || ds.Tables[1].Rows[j]["COLUMN_NAME"].ToString().ToLower() == "stepenableobjectid")
                    {
                        outp = outp + "newid(), ";
                    }
                    else if (ds.Tables[1].Rows[j]["COLUMN_NAME"].ToString().ToLower() == "stepname")
                    {
                        outp = outp + "'(Copy) " + ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString().Replace("'", "''") + "', ";
                    }
                    else if (ds.Tables[1].Rows[j]["DATA_TYPE"].ToString() == "uniqueidentifier")
                    {
                        outp = outp + ("'" + ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString().Replace("'", "''") + "'" == "''" ? "null" : "'" + ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString().Replace("'", "''") + "'") + ", ";
                    }
                    else
                    {
                        outp = outp + "'" + ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString().Replace("'", "''") + "', ";
                    }
                }


                if (outp.Substring(outp.Length - 2, 1) == ",")
                {
                    outp = outp.Substring(0, outp.Length - 2) + "),";
                }

            }
        }

        if (outp != "")
        {
            if (outp.Substring(outp.Length - 1, 1) == ",")
            {
                outp = outp.Substring(0, outp.Length - 1) + ";";
            }
        }

        return outp;
    }
    #endregion


}