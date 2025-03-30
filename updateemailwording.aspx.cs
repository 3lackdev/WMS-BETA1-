using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class updateemailwording : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {


//###################### check login
        if (isWho() == "" || (isWho().ToUpper() == "IUSR" && Request.QueryString["username"] == null))
        {
            Session["loginname"] = null;

            //############## check mobile internet
           string strUserAgent = Request.UserAgent.ToString().ToLower();
           if (strUserAgent != null)
           {
               if (Request.Browser.IsMobileDevice == true ||
                   strUserAgent.Contains("iphone") ||
                   strUserAgent.Contains("blackberry") ||
                   strUserAgent.Contains("mobile") ||
                   strUserAgent.Contains("windows ce") ||
                   strUserAgent.Contains("opera mini") ||
                   strUserAgent.Contains("palm") ||
                   strUserAgent.Contains("ipad") ||
                   strUserAgent.Contains("android"))
               {
                   Session["lastpage"] = this.Page.Request.Url;
                   Response.Redirect("http://115.31.163.110:8080/wf/login.aspx?logout=y");
               }
           }
            //#######


           if (Request.QueryString["domain"] != null)
           {
               if (Request.QueryString["domain"].ToString().ToUpper().IndexOf("JBE.CO.TH")>=0)
               {
                   Response.Redirect(ServerCheckAuthenUrl+"?page=" + (this.Page.Request.Url.ToString() + "&domain=JBE.CO.TH").ToString().Replace("&", ";and"));
               }
               else
               {
                   Response.Redirect(ServerCheckAuthenUrl + "?page=" + (this.Page.Request.Url.ToString() + "&domain=BST.CO.TH").ToString().Replace("&", ";and"));
               }
           }
           
            MultiView1.ActiveViewIndex = 2;
            return;

        }
        else
        {
            if (Request.QueryString["username"] != null)
            {
                if (isWho().ToUpper() != Request.QueryString["username"].ToUpper())
                {
                    Session["logindomain"] = (Request.QueryString["domain"] != null ? Request.QueryString["domain"].ToString().Replace(",jbe.co.th", "").Replace(",bst.co.th", "").Replace(",JBE.CO.TH", "").Replace(",BST.CO.TH", "") : "JBE.CO.TH");
                    Session["loginname"] = Request.QueryString["username"].ToString().Replace("JBE\\\\", "").Replace("/", "&");
                    Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("JBE\\\\", "").Replace("/", "&");
                    Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "").Replace("/", "&");
                    Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "").Replace("/", "&");

                    try
                    {
                        Response.Cookies["UserName"].Value = Session["loginname"].ToString().Replace("@bst.co.th", "").Replace("@jbe.co.th", "").Replace("@BST.CO.TH", "").Replace("@JBE.CO.TH", "");
                        Response.Cookies["Domain"].Value = Session["logindomain"].ToString();
                    }catch(Exception ex){}
                }



                if (Session["loginname"].ToString().Length > 20 || Session["loginname"].ToString().IndexOf("/") > 0)
                {

                    Response.Cookies["UserName"].Value = "";
                    Response.Cookies["Domain"].Value = "";

                    if (Request.QueryString["domain"] != null)
                    {

                        if (Request.QueryString["domain"].ToString().ToUpper().IndexOf("JBE.CO.TH")>=0)
                        {
                            Response.Redirect(ServerCheckAuthenUrl+"?page=" + (this.Page.Request.Url.ToString().Replace("username=", "usernamexxx=") + "&domain=JBE.CO.TH").ToString().Replace("&", ";and"));
                        }
                        else
                        {
                            Response.Redirect(ServerCheckAuthenUrl + "?page=" + (this.Page.Request.Url.ToString().Replace("username=", "usernamexxx=") + "&domain=BST.CO.TH").ToString().Replace("&", ";and"));
                        }
                    }
                }
            }

            //Response.Write(IsDomain());
        }
        //##############################


        if (Request.QueryString["rowid"] != null)
        {
            refresh();
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
                                                        where WorkFlowStep.WFRowId='" + Request.QueryString["rowid"].ToString() + @"' order by WorkFlowStep.GroupName,WorkFlowStep.seq;
                                                    select * from WorkFlowTemplate where WFRowId='" + Request.QueryString["rowid"].ToString() + @"';
                                                    select * from Prosition where WFRowId='" + Request.QueryString["rowid"].ToString() + @"' order by positionname;
                                                    select * from WorkFlowStep where WFRowId='" + Request.QueryString["rowid"].ToString() + @"' order by groupname,seq;
                                                    select * from WorkFlow where WFRowId='" + Request.QueryString["rowid"].ToString() + "'");


        GridView1.DataSource = ds.Tables[0];
        GridView1.DataBind();
        
        lb_wfname.Text = "WorkFlow Name : " + ds.Tables[4].Rows[0]["WFName"].ToString();

    }
    protected void btn_back_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 1;
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        rowid_txt.Text = GridView1.Rows[Row.RowIndex].Cells[0].Text;
        lb_message.Text = "";
       getCururrentData();
        lb_eventname.Text = "Step : " + ((Label)GridView1.Rows[Row.RowIndex].Cells[0].FindControl("Label1")).Text;

    }

    private void getCururrentData()
    {
        div_eventsave.Visible = false;
        div_eventapprove.Visible = false;
        div_eventreject.Visible = false;
        DataSet ds = this.SQLExecuteReader("system", "select * from WorkFlowStep where StepRowId='" + rowid_txt.Text + "' order by seq;");
        if (ds.Tables[0].Rows.Count > 0)
        {
           
            if(ds.Tables[0].Rows[0]["EnableReject"].ToString()=="Y")
            {
                div_eventreject.Visible = true;
            }
            if (ds.Tables[0].Rows[0]["EnableApprove"].ToString() == "Y")
            {
                div_eventapprove.Visible = true;
            }
            if (ds.Tables[0].Rows[0]["EnableSave"].ToString() == "Y")
            {
                div_eventsave.Visible = true;
            }
            
          

            //####################### save
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveValueToselect"].ToString() != "")
                {
                    SaveConditionVal.Visible = true;
                   
                    
                }
                else
                {
                    SaveConditionVal.Visible = false;
                }
               

            }
            catch { }
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveCC1ValueToselect"].ToString() != "")
                {
                    
                    SaveConditionValCC1.Visible = true;
                    
                   
                }
                else
                {
                    SaveConditionValCC1.Visible = false;
                }
              

            }
            catch { }
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterSaveInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledSaveInformValueToselect"].ToString() != "")
                {
                    
                    SaveConditionValInform.Visible = true;
                    
                }
                else
                {
                    SaveConditionValInform.Visible = false;
                }
              

            }
            catch { }



            //####################### approve
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterApproveMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveValueToselect"].ToString() != "")
                {
                   
                    ApproveConditionVal.Visible = true;
                  
                }
                else
                {
                    ApproveConditionVal.Visible = false;
                }
               

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterApproveCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveCC1ValueToselect"].ToString() != "")
                {
                    
                    ApproveConditionValCC1.Visible = true;
                
                  
                }
                else
                {
                    ApproveConditionValCC1.Visible = false;
                }
               

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterApproveInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledApproveInformValueToselect"].ToString() != "")
                {
                   
                    ApproveConditionValInform.Visible = true;
                    
                   
                }
                else
                {
                    ApproveConditionValInform.Visible = false;
                }
               

            }
            catch { }


            //####################### reject
            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectValueToselect"].ToString() != "")
                {
                   
                    RejectConditionVal.Visible = true;
                    
                   
                }
                else
                {
                    RejectConditionVal.Visible = false;
              
                }
              

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectCC1MailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectCC1ValueToselect"].ToString() != "")
                {
                   
                    RejectConditionValCC1.Visible = true;
                   
                    
                }
                else
                {
                    RejectConditionValCC1.Visible = false;
                }
                

            }
            catch { }

            try
            {
                if (ds.Tables[0].Rows[0]["EventAfterRejectInformMailToPosition"].ToString() == "" && ds.Tables[0].Rows[0]["DinamicFiledRejectInformValueToselect"].ToString() != "")
                {
                    
                    RejectConditionValInform.Visible = true;
                   
                    
                }
                else
                {
                    RejectConditionValInform.Visible = false;
                }
               

            }
            catch { }



            txt_EventAfterSaveEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterSaveEmailContent"].ToString().Replace("\n", "<br />");
            txt_EventAfterApproveEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterApproveEmailContent"].ToString().Replace("\n", "<br />");
            txt_EventAfterRejectEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterRejectEmailContent"].ToString().Replace("\n", "<br />");

            try
            {
                txt_EventAfterSaveInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterSaveInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");

                txt_EventAfterSaveEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterSaveEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterSaveEmailSubject"].ToString());

                txt_EventAfterApproveInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterApproveInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");

                txt_EventAfterApproveEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterApproveEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterApproveEmailSubject"].ToString());

                txt_EventAfterRejectInformEmailContent.Text = ds.Tables[0].Rows[0]["EventAfterRejectInformEmailContent"].ToString().Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");

                txt_EventAfterRejectEmailSubject.Text = (ds.Tables[0].Rows[0]["EventAfterRejectEmailSubject"].ToString() == "" ? "{sys_workflowname} [{sys_nextstepname}]" : ds.Tables[0].Rows[0]["EventAfterRejectEmailSubject"].ToString());

            }
            catch { }


          
       
        }
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (div_eventsave.Visible)
        {
            SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterSaveEmailSubject='" + txt_EventAfterSaveEmailSubject.Text.Replace("'", "''") + "',EventAfterSaveEmailContent='" + txt_EventAfterSaveEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',EventAfterSaveInformEmailContent='" + txt_EventAfterSaveInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "' where StepRowId='" + rowid_txt.Text + "'");
        }
        if (div_eventapprove.Visible)
        {
            SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterApproveEmailSubject='" + txt_EventAfterApproveEmailSubject.Text.Replace("'", "''") + "',EventAfterApproveEmailContent='" + txt_EventAfterApproveEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',EventAfterApproveInformEmailContent='" + txt_EventAfterApproveInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "' where StepRowId='" + rowid_txt.Text + "'");
        }
        if (div_eventreject.Visible)
        {
            SQLeExecuteNonQuery("system", "update WorkFlowStep set EventAfterRejectEmailSubject='" + txt_EventAfterRejectEmailSubject.Text.Replace("'", "''") + "',EventAfterRejectEmailContent='" + txt_EventAfterRejectEmailContent.Text.Replace("'", "''").Replace("\n", "") + "',EventAfterRejectInformEmailContent='" + txt_EventAfterRejectInformEmailContent.Text.Replace("'", "''").Replace("\n", "") + "' where StepRowId='" + rowid_txt.Text + "'");
        }

        lb_message.Text = "Save complated";
        lb_message.Visible = true;
        lb_message.ForeColor = System.Drawing.Color.Green;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("updatetemplate.aspx?rowid=" + Request.QueryString["rowid"].ToString());
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("updatePosition.aspx?rowid=" + Request.QueryString["rowid"].ToString());
    }
}