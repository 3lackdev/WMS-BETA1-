using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using HtmlAgilityPack;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Configuration;

public partial class WorkflowRuning : BST.libDB
{
    private DataSet dataInfor = new DataSet();
    private ArrayList fieldinput = new ArrayList();
    private Boolean calltigerBtn = false;
    private String globalStartFlowID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["btnaction"] != null)
            {
                Response.Clear();
                if (Session["loginname"] == null)
                {
                    Session["loginname"] = "SYSTEM";
                    Session["loginfullname"] = "SYSTEM";
                }
		
                Response.Write(autostart());
                HttpContext.Current.Response.Flush(); 
                HttpContext.Current.Response.SuppressContent = true;  
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            checklogin();
        }
        catch (Exception ex) { Response.Write(ex.Message); }
        try
        {
            Label5.Text = "You are : <font color=blue>" + isWho() + "</font>";
        }
        catch { Label5.Text = ""; }

        if (!IsPostBack)
        {

            fieldel();
           
            //############### redirect force to current step for admin pass though
try
            {

                system_loginname.Value = Session["loginname"].ToString();
                system_loginfullname.Value = getdomainfullname(this.isWho(), this.IsDomain());
            }
            catch { }
            if (Request.QueryString["rowid"] != null && Request.QueryString["referencekey"] != null && Request.QueryString["runingkey"] != null)
            {
                DataSet dsrowid = SQLExecuteReader("system", "select top 1 WFRowId from WorkFlowRunningStatus where startrowid='" + Request.QueryString["runingkey"].ToString() + "';select top 1 rowid  from WorkFlowEventsHistory where startrowid='" + Request.QueryString["runingkey"].ToString() + "' order by crdate desc");

                if (Request.QueryString["rowid"].ToString().ToLower() == "force" && (Request.QueryString["referencekey"].ToString().ToLower() == "force" || Request.QueryString["referencekey"].ToString().ToLower() == "view") && Request.QueryString["runingkey"] != null)
                {

                    if (dsrowid.Tables[0].Rows.Count > 0 && dsrowid.Tables[1].Rows.Count > 0)
                    {
                        if (Request.QueryString["action"] != null)
                        {
                            Response.Redirect("WorkflowRuning.aspx?rowid=" + dsrowid.Tables[0].Rows[0]["WFRowId"].ToString() + "&runingkey=" + Request.QueryString["runingkey"].ToString() + "&referencekey=" + dsrowid.Tables[1].Rows[0]["rowid"].ToString() + "&bypass=" + Request.QueryString["action"].ToString().ToLower());
                        }
                        else
                        {
                            if (Request.QueryString["referencekey"].ToString().ToLower() == "force")
                            {
                                Response.Redirect("WorkflowRuning.aspx?rowid=" + dsrowid.Tables[0].Rows[0]["WFRowId"].ToString() + "&runingkey=" + Request.QueryString["runingkey"].ToString() + "&referencekey=" + dsrowid.Tables[1].Rows[0]["rowid"].ToString() + "&bypass=force");
                            }
                            else
                            {
                                Response.Redirect("WorkflowRuning.aspx?rowid=" + dsrowid.Tables[0].Rows[0]["WFRowId"].ToString() + "&runingkey=" + Request.QueryString["runingkey"].ToString() + "&referencekey=" + dsrowid.Tables[1].Rows[0]["rowid"].ToString());
                            }
                        }
                    }
                }
                else if (Request.QueryString["referencekey"].ToLower() != dsrowid.Tables[1].Rows[0]["rowid"].ToString().ToLower())
                {
                    Response.Redirect("WorkflowRuning.aspx?rowid=" + dsrowid.Tables[0].Rows[0]["WFRowId"].ToString() + "&runingkey=" + Request.QueryString["runingkey"].ToString() + "&referencekey=" + dsrowid.Tables[1].Rows[0]["rowid"].ToString());
                }
            }
            //############### end redirect force to current step for admin pass though

            try
            {
                if (Request.QueryString["rowid"] != null && Request.QueryString["runingkey"] == null)
                {
                    getstart();
                }
                else if (Request.QueryString["rowid"] != null && Request.QueryString["runingkey"] != null)
                {
                    getstep(Request.QueryString["runingkey"].ToString());
                }
            }
            catch (Exception ex)
            {
                SendEmail("Pongsan_K@bst.co.th;Teerapong_T@bst.co.th;Sarawoot_k@bst.co.th", null, "!!!WEBDB ERROR", "<br /><br />" + ex.Message.ToString()+"<br /><br />Check link <a href='https://guru.bst.co.th:440/workflowlist.aspx'>Check WF</a>", "WF-Guru440 Workflow <no-reply_webwf@bst.co.th>");
                Response.Clear();
                Response.Write(@"
 Page not responding... please try again later or inform IT in case urgent.
ขณะนี้ระบบมีการประมวณผลในปริมาณที่มาก กรุณาทำรายการอีกครั้งภายหลัง.. หรือแจ้ง IT ให้ตรวจสอบกรณีเร่งด่วน");

          	try
                {
                    lineNotify("ช่วยด้วย!!\n\n!!WEBDB ERROR \n" + ex.Message.ToString() + " ใครว่างช่วย Stop/Start IIS ใน server GURU หน่อยครับ วิธีตามนี้ \n http://cmp.bst.co.th:8080/line/ckp/howto.png  \n กราบบบบ.. \n\nลองคลิ๊กดูว่า Error ไหมที่  https://guru.bst.co.th:440/workflowlist.aspx");
                }
                catch { }

                Response.End();
            }

            if (Request.QueryString["rowid"] != null)
            {
                LinkButton3.OnClientClick = "window.open('ViewRuningHistory.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "');return false;";
            }
        }
    }

    private void fieldel()
    {
        try
        {
            if (Request.QueryString["filedel"] != null)
            {
                this.SQLeExecuteNonQuery("system", "delete from Attechment where id='" + Request.QueryString["filedel"] + "'");
                Response.Redirect(Request.Url.ToString().Replace("&filedel=" + Request.QueryString["filedel"].ToString(), ""));
            }
        }
        catch (Exception e) { }
    }

    private void checklogin()
    {
        if (Request.QueryString["username"] != null && isWho().ToString().ToUpper()=="IUSR")
        {

            Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "");
            Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "");
            Session["logindomain"] = ServerDomain;
            Response.Redirect(this.Page.Request.Url.ToString().Replace("&username=" + Request.QueryString["username"].ToString().Replace("BST_CORP\\\\", "").ToString(), ""));
            return;
        }

        else if ((Session["loginname"] == null && this.Page.Request.Url.ToString().IndexOf("rowid") > 0) || isWho().ToString().ToLower() == "guest" || isWho().ToString().ToUpper() == "IUSR")
        {
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
                    Response.Redirect(ServerInternetUrl + "/login.aspx?logout=y");
                }
            }
            //#######


            String[] localip = ServerLocalIP.ToString().Split((";").ToCharArray());
            Boolean islocal=false;
            for (int i = 0; i < localip.Length; i++)
            {
                //Response.Write(localip[i]);
                if (Request.UserHostAddress.ToString().IndexOf(localip[i]) >= 0 || Request.UserHostAddress.ToString().IndexOf("::")>0)
                    islocal = true;
            }

            if (islocal)
            {

                if (Request.Cookies["UserName"] != null)
                {

                    try
                    {
                        Session["loginfullname"] = Request.Cookies["UserName"].Value.ToString();
                        Session["loginname"] = Request.Cookies["UserName"].Value.ToString();
                        Session["logindomain"] = Request.Cookies["Domain"].Value.ToString();
                    }
                    catch {

                        Session["logindomain"] = ServerDomain;
                    }


                }
                else
                {

                    try
                    {
                     //   Response.Write("1");
                        DataSet ds = SQLExecuteReader("system", @"SELECT DefaultDomain from workflow where WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
                        // Response.Write(ds.Tables[0].Rows[0][0].ToString().Trim());
                        if (ds.Tables[0].Rows[0][0].ToString().Trim() == "")
                        {
                         //   Response.Write("2");
                            Session["lastpage"] = this.Page.Request.Url.ToString();
                            Response.Redirect("login.aspx?logout=y");
                        }
                        else
                        {
                       //     Response.Write("3");
                         //   Response.Write(ServerCheckAuthenUrl + "/default.aspx?page=" + this.Page.Request.Url.ToString().Replace("&", ";and"));
                        //    Session["cout"] = int.Parse((Session["cout"]==null?"0":Session["cout"].ToString())) + 1;
                            Response.Redirect(ServerCheckAuthenUrl + "/default.aspx?page=" + this.Page.Request.Url.ToString().Replace("&", ";and"));
                            //Response.Redirect(this.Page.Request.Url.ToString()+"&username=jbeadmin");
                            return;
                        }
                      //  Response.Write("4");
                    }
                    catch (Exception ex)
                    {
                       // Response.Write("5");
                        Session["lastpage"] = this.Page.Request.Url.ToString();
                      //  Response.Redirect("login.aspx?logout=y&e=" + Session["cout"].ToString() + isWho().ToString() + this.Page.Request.Url.ToString());
                    }

                }
                 
                
            }
            else
            {
               
               // Response.Write(Request.UserHostAddress.ToString());
                Session["lastpage"] = this.Page.Request.Url.ToString();
                Response.Redirect(ServerInternetUrl+"/login.aspx?logout=y");
               
            }
        }


        if (Session["loginname"] != null && Session["logindomain"]!=null)
        {
            Response.Cookies["UserName"].Value = Session["loginname"].ToString();
            Response.Cookies["Domain"].Value = Session["logindomain"].ToString();
            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
            Response.Cookies["Domain"].Expires = DateTime.Now.AddDays(30);
        }
        
    }

    private void getstart()
    {
        fieldinput.Clear();
        DataSet ds = SQLExecuteReader("system", @"SELECT     WorkFlow.WFName, WorkFlowTemplate.TemplateContent, WorkFlowTemplate.TemplateRowId, WorkFlowStep.StepRowId,WorkFlowStep.StepName, WorkFlowStep.EventBeforeSaveCallJavascriptFunction, 
                                                                      WorkFlowStep.EventAfterSaveMailToPosition, WorkFlowStep.EventAfterSaveEmailContent, WorkFlowStep.EventAfterSaveGotoStep, WorkFlowStep.EventAfterSaveExecAtServer, 
                                                                      WorkFlowStep.EventAfterSaveExecCommand, WorkFlowStep.EventBeforeApproveCallJavascriptFunction, WorkFlowStep.EventAfterApproveMailToPosition, 
                                                                      WorkFlowStep.EventAfterApproveEmailContent, WorkFlowStep.EventAfterApproveGotoStep, WorkFlowStep.EventAfterApproveExecAtServer, WorkFlowStep.EventAfterApproveExecCommand, 
                                                                      WorkFlowStep.EventBeforeRejectCallJavascriptFunction, WorkFlowStep.EventAfterRejectMailToPosition, WorkFlowStep.EventAfterRejectEmailContent, WorkFlowStep.EventAfterRejectGotoStep, 
                                                                      WorkFlowStep.EventAfterRejectExecAtServer, WorkFlowStep.EventAfterRejectExecCommand, WorkFlowStep.EnableSave, WorkFlowStep.EnableApprove, WorkFlowStep.EnableReject, 
                                                                      WorkFlowTemplate.WFRowId,WorkFlowStep.DinamicFiledSaveValueToselect,WorkFlowStep.DinamicFiledApproveValueToselect,WorkFlowStep.DinamicFiledRejectValueToselect,  EventAfterSaveCC1MailToPosition, DinamicFiledSaveCC1ValueToselect, EventAfterSaveInformMailToPosition, DinamicFiledSaveInformValueToselect, EventAfterApproveCC1MailToPosition, 
                                                                      DinamicFiledApproveCC1ValueToselect, EventAfterApproveInformMailToPosition, DinamicFiledApproveInformValueToselect, EventAfterRejectCC1MailToPosition, DinamicFiledRejectCC1ValueToselect, 
                                                                      EventAfterRejectInformMailToPosition, DinamicFiledRejectInformValueToselect,WorkFlowStep.AliasNameSave,WorkFlowStep.AliasNameApprove,WorkFlowStep.AliasNameReject,WorkFlowStep.EventAfterSaveInformEmailContent,
                                                                      WorkFlowStep.EventAfterApproveInformEmailContent,WorkFlowStep.EventAfterRejectInformEmailContent,WorkFlowStep.guideline,JavaScriptOnload,DinamicFiledSaveValueToStep,DinamicFiledApproveValueToStep,DinamicFiledRejectValueToStep,EventAfterSaveEmailSubject,EventAfterApproveEmailSubject,EventAfterRejectEmailSubject,
                                                                     WorkFlowStep.TriggerSave, WorkFlowStep.HideButtonSave, 
                                                                     WorkFlowStep.CheckTiggerSave, WorkFlowStep.TriggerApprove, WorkFlowStep.HideButtonApprove, WorkFlowStep.CheckTiggerApprove, WorkFlowStep.TriggerReject, WorkFlowStep.HideButtonReject, 
                                                                     WorkFlowStep.CheckTiggerReject, WorkFlowStep.StepAutoStart, WorkFlowStep.TiggerStartStepCondition, WorkFlowStep.TiggerStartStepConditionButton, WorkFlowStep.EventAfterSaveUpdateValueETC1, 
                                                                     WorkFlowStep.EventAfterSaveUpdateValueETC2, WorkFlowStep.EventAfterSaveUpdateValueETC3, WorkFlowStep.EventAfterSaveUpdateValueETC4, WorkFlowStep.EventAfterSaveUpdateValueETC5, 
                                                                     WorkFlowStep.EventAfterApproveUpdateValueETC1, WorkFlowStep.EventAfterApproveUpdateValueETC2, WorkFlowStep.EventAfterApproveUpdateValueETC3, WorkFlowStep.EventAfterApproveUpdateValueETC4, 
                                                                     WorkFlowStep.EventAfterApproveUpdateValueETC5, WorkFlowStep.EventAfterRejectUpdateValueETC1, WorkFlowStep.EventAfterRejectUpdateValueETC2, WorkFlowStep.EventAfterRejectUpdateValueETC3, 
                                                                     WorkFlowStep.EventAfterRejectUpdateValueETC4, WorkFlowStep.EventAfterRejectUpdateValueETC5, WorkFlowStep.PassValueOldTemplate,WorkFlowStep.CheckConditionAutoStartSave,WorkFlowStep.CheckConditionAutoStartApprove,WorkFlowStep.CheckConditionAutoStartReject,'' as ParentStartRowID,
                                                                    WorkFlowStep.EventSaveDonotSendMail, WorkFlowStep.EventApproveDonotSendMail, WorkFlowStep.EventRejectDonotSendMail

                                                FROM         WorkFlow INNER JOIN
                                                                      WorkFlowStep ON WorkFlow.WFRowId = WorkFlowStep.WFRowId INNER JOIN
                                                                      WorkFlowTemplate AS WorkFlowTemplate ON WorkFlowStep.TemplateRowId = WorkFlowTemplate.TemplateRowID
                                                WHERE     (" + (Request.QueryString["startwithstep"] != null ? "WorkFlowStep.StepRowId='" + Request.QueryString["startwithstep"].ToString()+"'" : "WorkFlowStep.DefaultStart = 'Y'") + @") 
                                                AND       WorkFlow.WFRowId='" + Request.QueryString["rowid"].ToString() + "';");
        
        if (ds.Tables[0].Rows.Count > 0)
        {
            loadGuideline(ds.Tables[0].Rows[0]["guideline"].ToString());
            dataInfor = ds;
            currentstep_rowid.Value = ds.Tables[0].Rows[0]["StepRowId"].ToString();
            string content_txt ="";

            wftitle.Text = ds.Tables[0].Rows[0]["WFName"].ToString();

            lb_currentstep.Text = "Step :" + ds.Tables[0].Rows[0]["StepName"].ToString();

            content_txt = ds.Tables[0].Rows[0]["TemplateContent"].ToString();

            //############# child start by step
            if(Request.QueryString["parentstartrowid"]!=null)
            {
                DataSet dsdd = SQLExecuteReader("system", @"SELECT        WorkFlowRunningStatus.ContentBody, WorkFlowStep.TemplateRowId
                                                        FROM            WorkFlowRunningStatus INNER JOIN
                                                                                 WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId
                                                        WHERE        (WorkFlowRunningStatus.StartRowID = '" + Request.QueryString["parentstartrowid"].ToString() + "')");
                if(dsdd.Tables[0].Rows.Count>0)
                {
                    if (dsdd.Tables[0].Rows[0]["TemplateRowId"].ToString() == ds.Tables[0].Rows[0]["TemplateRowId"].ToString())
                    {
                        content_txt = dsdd.Tables[0].Rows[0]["ContentBody"].ToString();
                    }
                }
                


            }

            //####end  child start by step


            uploadfile.Visible = false;

            //string content_txt = (ds.Tables[0].Rows[0]["CurrentStepRowId"].ToString() == "" ? ds.Tables[0].Rows[0]["TemplateContent"].ToString() : ds.Tables[0].Rows[0]["ContentBody"].ToString());

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content_txt);

            doc = appendTableDataBindHtml(doc);

            DataSet dd = SQLExecuteReader("system", @"SELECT     StepEnableObject.RequireField, TemplateObject.ObjectName, StepEnableObject.StepEnableObjectId, StepEnableObject.BindRowID, DataBindValue.Server, DataBindValue.Sql, 
                                                                          DataBindValue.DataFieldText, DataBindValue.DataFieldValue,StepEnableObject.defaultval,Afteraction
                                                    FROM         DataBindValue RIGHT OUTER JOIN
                                                                          StepEnableObject ON DataBindValue.BindRowID = StepEnableObject.BindRowID RIGHT OUTER JOIN
                                                                          WorkFlowStep INNER JOIN
                                                                          TemplateObject ON WorkFlowStep.TemplateRowId = TemplateObject.TemplateRowId ON StepEnableObject.StepRowId = WorkFlowStep.StepRowId AND 
                                                                          StepEnableObject.TemplateRowId = TemplateObject.TemplateRowId AND StepEnableObject.ObjectRowId = TemplateObject.ObjectRowId
                                                        WHERE     (WorkFlowStep.StepRowId ='" + currentstep_rowid.Value + "')  order by Afteraction;");
                for (int i = 0; i < dd.Tables[0].Rows.Count; i++)
                {

                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) == null)
                    {
                        continue;
                    }

                    //############################# bind dropdrowlist

                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null)
                    {
                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "select":
                                if (dd.Tables[0].Rows[i]["bindrowid"].ToString() != "")
                                {
                                    try
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAllChildren();

                                        DataSet ds__ = new DataSet();
                                        if (dd.Tables[0].Rows[i]["Server"].ToString().ToUpper().IndexOf("ORA_") == 0)
                                        {
                                            serviceoracle.Service1 sv = new serviceoracle.Service1();
                                            ds__ = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dd.Tables[0].Rows[i]["Server"].ToString()].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }
                                        else
                                        {
                                            ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }

                                        //DataSet ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        for (int j = 0; j < ds__.Tables[0].Rows.Count; j++)
                                        {
                                            HtmlAgilityPack.HtmlNode n = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                            n.SetAttributeValue("value", ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldValue"].ToString()].ToString());
                                            n.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode(ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldText"].ToString()].ToString()));
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Add(n);
                                        }
                                        HtmlAgilityPack.HtmlNode nn = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                        nn.SetAttributeValue("value", "");
                                        nn.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode("  Please select  "));

                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Insert(0, nn);

                                    }
                                    catch { }

                                }
                                break;
                        }
                    }

                    //############################# end bind dropdrowlist

                    if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()] != null)
                    {
                        //  Net check Setdefault
                        if (dd.Tables[0].Rows[i]["Afteraction"].ToString() == "Y")
                        {
                            doc = setdefault(doc, dd.Tables[0].Rows[i]);
                        }
                        //end

                        switch(doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "input":
                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"] != null)
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("checked", "checked");
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "OK" });
                                    }
                                    else
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                    }
                                }
                                else
                                {
                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                }
                                break;
                            case "textarea":
                                //Response.Write(Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() != "")
                                {
                                    String ggg = Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString();
                                    String objss = Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString();
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("class", "").ToString().ToLower().IndexOf("htmledit") >= 0)
                                    {
                                        objss = "<div>" + Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() + "</div>";
                                    }
                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(objss));
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                }
                                else
                                {
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).InnerText });
                                }
                                break;
                            case "select":
                                if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() != "")
                                {
                                    for (int x = 0; x < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; x++)
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].GetAttributeValue("value","") == Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString())
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].SetAttributeValue("selected", "selected");
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                        }
                                    }
                                }          
                                break;
                        }
                       

                    }
                    else  //####################   get old value to fieldinput
                    {

                        //############################# bind dropdrowlist

                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "select":
                                if (dd.Tables[0].Rows[i]["bindrowid"].ToString() != "")
                                {
                                    try
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAllChildren();

                                        DataSet ds__ = new DataSet();
                                        if (dd.Tables[0].Rows[i]["Server"].ToString().ToUpper().IndexOf("ORA_") == 0)
                                        {
                                            serviceoracle.Service1 sv = new serviceoracle.Service1();
                                            ds__ = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dd.Tables[0].Rows[i]["Server"].ToString()].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }
                                        else
                                        {
                                            ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }


                                       // DataSet ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        for (int j = 0; j < ds__.Tables[0].Rows.Count; j++)
                                        {
                                            HtmlAgilityPack.HtmlNode n = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                            n.SetAttributeValue("value", ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldValue"].ToString()].ToString());
                                            n.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode(ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldText"].ToString()].ToString()));
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Add(n);
                                        }
                                        HtmlAgilityPack.HtmlNode nn = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                        nn.SetAttributeValue("value", "");
                                        nn.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode("  Please select  "));

                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Insert(0, nn);
                                    }
                                    catch { }

                                }
                                break;
                        }

                        //############################# END dropdrowlist

                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "input":

                                if(doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"]!=null)
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("checked", "") == "checked")
                                        {
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "OK" });
                                        }
                                        else
                                        {
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "" });
                                        }
                                    }
                                    else
                                    {
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("value", "") });
                                    }

                                }
                                else
                                {
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("value", "") });
                                }
                                break;
                            case "textarea":
                                fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).InnerText });
                                break;
                            case "select":
                                try
                                {
                                    for (int b = 0; b < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; b++)
                                    {
                                        String v = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("selected", "");
                                        if (v == "selected")
                                        {
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("value", "") });
                                        }
                                    }
                                }
                                catch { }

                                break;
                        }

                    }
                   


                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null && dd.Tables[0].Rows[i]["StepEnableObjectId"].ToString() == "")
                    {
                          String vvv = "";
                        try
                        {
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Add("disabled", "disabled");
                            vvv=doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("background-color:", "background:") + ";background-color:#FAF8F8");

                          
                        }
                        catch { try { doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Add("style", "background-color:#FAF8F8"); } catch { } 
                        }

                    }
                    else/// ojbect ที่ enable
                    {
                        #region new setdefault
                        if (dd.Tables[0].Rows[i]["Afteraction"].ToString() != "Y")
                        {
                            doc = setdefault(doc, dd.Tables[0].Rows[i]);
                        }
                        else
                        {
                            if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["class"] != null)
                            {
                                var ool = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("class", "");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("class",ool + " defaultpostback");
                            }
                            else
                            {
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Append("class", "defaultpostback");
                            }
                        }

                        #region olddefault
                        
                        /*
                        #region setdefault 
                        //################## set default value
                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("value","")=="" && dd.Tables[0].Rows[i]["defaultval"].ToString()!="")
                        {
                            String finalSQL = dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower();
                             string[] pram = dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower().Replace("sql{","").Split(("{").ToCharArray());
                             for (int v = 0; v < pram.Length; v++)
                             {
                                 pram[v] = "{"+pram[v];
                                 if (pram[v].IndexOf("}") > 0)
                                 {
                                     pram[v] = pram[v].Substring(0, pram[v].IndexOf("}")+1);
                                     if (pram[v].Substring(0, 1) == "{" && pram[v].Substring(pram[v].Length - 1, 1) == "}")                                           
                                     {
                                         if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")) != null)
                                         {
                                             if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).Name.ToLower() == "select")
                                             {

                                                 for (int b = 0; b < doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes.Count; b++)
                                                 {
                                                     String x = doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("selected", "");
                                                     if (x == "selected")
                                                     {
                                                         finalSQL = finalSQL.ToLower().Replace(pram[v].ToLower(), doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("value", ""));
                                                     }
                                                 }
                                                 finalSQL = finalSQL.Replace(pram[v], "?");
                                             }
                                             else
                                             {
                                                 finalSQL = finalSQL.Replace(pram[v], doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", ""));
                                             }
                                         }
                                     }
                                 }
                             }

                            switch(dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower().Trim())
                            {
                                case "{sys_date}":
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("yyyy-MM-dd")));
                                    }
                                    else
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    break;
                                case "{sys_time}":
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("HH:mm")));
                                    }
                                    else
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("HH:mm"));
                                    }
                                    break;
                                case "{sys_username}":
                                    try
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(isWho()));
                                        }
                                        else
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", isWho());
                                        }
                                    }
                                    catch { }
                                    break;
                                case "{sys_workflowid}":
                                    try
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["rowid"].ToString()));
                                        }
                                        else
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["rowid"].ToString());
                                        }
                                    }
                                    catch { }
                                    break;
                                case "{sys_startworkflowid}":
                                    try
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["runingkey"].ToString()));
                                        }
                                        else
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["runingkey"].ToString());
                                        }
                                    }
                                    catch { }
                                    break;
                                case "{sys_referenceeventhistory}":
                                    try
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["referencekey"].ToString()));
                                        }
                                        else
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["referencekey"].ToString());
                                        }
                                    }
                                    catch { }
                                    break;
				                case "{sys_logonemail}":
                                    try
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(getemailbyuser(isWho())));
                                        }
                                        else
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", getemailbyuser(isWho()));
                                        }
                                    }
                                    catch { }
                                    break;
                                default:

                                    if (finalSQL.IndexOf("sql{") >= 0)
                                    {
                                        try
                                        {
                                            DataSet dds = SQLExecuteReader("system", finalSQL.ToLower().Replace("sql{", "").Trim().Substring(0, finalSQL.ToLower().Replace("sql{", "").Trim().Length - 1));
                                            if (dds.Tables.Count > 0)
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", dds.Tables[0].Rows[0][0].ToString().ToUpper());
                                            }
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        try { doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", finalSQL); }
                                        catch { }
                                    }
                                    break;
                            }
                        }
                        //###################
                        #endregion
                        */
                        #endregion olddefault

                        #endregion

                        if (dd.Tables[0].Rows[i]["RequireField"].ToString() == "Y")
                        {
                            String vvv = "";
                            try
                            {

                    

                               vvv= doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("background-color:", "background:") + ";background-color:yellow");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("accesskey", "require");

                            }
                            catch { try { doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Add("style", "background-color:yellow"); } catch { } 
                            }
                        }
                        else
                        {
                            String vvv = "";
                            try
                            {
                              

                                vvv=doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("background-color:", "background:") + ";background-color:#ffffff;border-style: solid;border-width: 1px;");

                            }
                            catch { try { doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Add("style", "background-color:#ffffff;border-style: solid;border-width: 1px;"); } catch { } 
                            }
                        }
                    }


                    //###################################### upload file
                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"] != null)
                    {
                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "file" )
                        {
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", (dd.Tables[0].Rows[i]["RequireField"].ToString() == "Y" ? doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "") + ";border: 3px solid yellow" : doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "")));
                        }
                    }
                


                }


           //Update add function tigger
            btn_save.Visible = (ds.Tables[0].Rows[0]["EnableSave"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonSave"].ToString() == "N" ? true : false);
            btn_approve.Visible = (ds.Tables[0].Rows[0]["EnableApprove"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonApprove"].ToString() == "N" ? true : false);
            btn_reject.Visible = (ds.Tables[0].Rows[0]["EnableReject"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonReject"].ToString() == "N" ? true : false);
            //endUpdate add function tigger

            btn_save.Text = (ds.Tables[0].Rows[0]["AliasNameSave"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameSave"].ToString() : btn_save.Text);
            btn_approve.Text = (ds.Tables[0].Rows[0]["AliasNameApprove"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameApprove"].ToString() : btn_approve.Text);
            btn_reject.Text = (ds.Tables[0].Rows[0]["AliasNameReject"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameReject"].ToString() : btn_reject.Text);



            otherscript.Text = @"<script>
                                    function onSaveClick()
                                    {
                                        " + (ds.Tables[0].Rows[0]["EventBeforeSaveCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeSaveCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>
                                <script>
                                    function onApproveClick()
                                    {
  					" + (ds.Tables[0].Rows[0]["EventBeforeApproveCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeApproveCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>
                                <script>
                                    function onRejectClick()
                                    {
					" + (ds.Tables[0].Rows[0]["EventBeforeRejectCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeRejectCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>";

            btn_save.OnClientClick = "return OnSaveCheck();";
            btn_approve.OnClientClick = "return OnApproveCheck();";
            btn_reject.OnClientClick = "return OnRejectCheck();";

            if (!IsPostBack)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "onstart", ds.Tables[0].Rows[0]["JavaScriptOnload"].ToString(), true);
            }


//##############querystring set
            if(!IsPostBack)
            {
                foreach (String key in Request.QueryString.AllKeys)
                {

                    if (doc.GetElementbyId(key.ToString()) != null)
                    {
                        switch (doc.GetElementbyId(key).Name.ToLower())
                        {
                            case "input":

                                if (doc.GetElementbyId(key).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                {
                                    if (Request.QueryString[key].ToString().ToLower() == "checked")
                                    {
                                        doc.GetElementbyId(key).SetAttributeValue("checked", "checked");
                                        fieldinput.Add(new String[] { key, "OK" });
                                    }
                                }
                                else
                                {
                                    doc.GetElementbyId(key).SetAttributeValue("value", Request.QueryString[key].ToString());

                                    for (int j = 0; j < fieldinput.Count; j++)
                                    {
                                        if ((((String[])fieldinput[j])[0]).ToString() == key)
                                        {
                                            fieldinput[j] = new String[] { key, Request.QueryString[key].ToString() };
                                        }

                                    }
                                }
                                break;
                            case "textarea":
                                if (Request.QueryString[key].ToString() != "")
                                {

                                    doc.GetElementbyId(key).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine)));

                                    for (int j = 0; j < fieldinput.Count; j++)
                                    {
                                        if ((((String[])fieldinput[j])[0]).ToString() == key)
                                        {
                                            fieldinput[j] = new String[] { key, Request.QueryString[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine) };
                                        }

                                    }

                                }

                                break;
                            case "select":
                                if (Request.QueryString[key].ToString() != "")
                                {
                                    for (int x = 0; x < doc.GetElementbyId(key).ChildNodes.Count; x++)
                                    {
                                        if (doc.GetElementbyId(key).ChildNodes[x].GetAttributeValue("value", "") == Request.QueryString[key].ToString())
                                        {
                                            doc.GetElementbyId(key).ChildNodes[x].SetAttributeValue("selected", "selected");
                                            fieldinput.Add(new String[] { key, Request.QueryString[key].ToString() });
                                        }
                                    }
                                }
                                break;
                        }

                    }
                }
           }

//######end query set

            content.Text = doc.DocumentNode.OuterHtml.ToString();
       
        }
    }

    private void getstep(String runingid)
    {
        
        fieldinput.Clear();
        DataSet ds = SQLExecuteReader("system", @"SELECT     TOP (1) WorkFlowRunningStatus.CurrentStepRowId, WorkFlowRunningStatus.ContentBody AS currentbody, WorkFlowStep_1.TemplateRowId AS oldtemplaterowid, 
                                                                      WorkFlowStep.TemplateRowId AS currenttemplaterowid, WorkFlowStep.EnableSave, WorkFlowStep.EnableApprove, WorkFlowStep.EnableReject, 
                                                                      WorkFlowStep.EventBeforeSaveCallJavascriptFunction, WorkFlowStep.EventAfterSaveMailToPosition, WorkFlowStep.EventAfterSaveEmailContent, WorkFlowStep.EventAfterSaveGotoStep, 
                                                                      WorkFlowStep.EventAfterSaveExecAtServer, WorkFlowStep.EventAfterSaveExecCommand, WorkFlowStep.EventBeforeApproveCallJavascriptFunction, 
                                                                      WorkFlowStep.EventAfterApproveMailToPosition, WorkFlowStep.EventAfterApproveEmailContent, WorkFlowStep.EventAfterApproveGotoStep, WorkFlowStep.EventAfterApproveExecAtServer, 
                                                                      WorkFlowStep.EventAfterApproveExecCommand, WorkFlowStep.EventBeforeRejectCallJavascriptFunction, WorkFlowStep.EventAfterRejectMailToPosition, 
                                                                      WorkFlowStep.EventAfterRejectEmailContent, WorkFlowStep.EventAfterRejectGotoStep, WorkFlowStep.EventAfterRejectExecAtServer, WorkFlowStep.EventAfterRejectExecCommand, 
                                                                      WorkFlowTemplate.TemplateContent, WorkFlowStep.StepName ,WorkFlowRunningStatus.Status,WorkFlowStep.DinamicFiledSaveValueToselect,WorkFlowStep.DinamicFiledApproveValueToselect,WorkFlowStep.DinamicFiledRejectValueToselect,WorkFlowEventsHistory.rowid,  WorkFlowStep.EventAfterSaveCC1MailToPosition, WorkFlowStep.DinamicFiledSaveCC1ValueToselect, WorkFlowStep.EventAfterSaveInformMailToPosition, WorkFlowStep.DinamicFiledSaveInformValueToselect, WorkFlowStep.EventAfterApproveCC1MailToPosition, 
                                                                      WorkFlowStep.DinamicFiledApproveCC1ValueToselect, WorkFlowStep.EventAfterApproveInformMailToPosition, WorkFlowStep.DinamicFiledApproveInformValueToselect, WorkFlowStep.EventAfterRejectCC1MailToPosition, WorkFlowStep.DinamicFiledRejectCC1ValueToselect, 
                                                                      WorkFlowStep.EventAfterRejectInformMailToPosition, WorkFlowStep.DinamicFiledRejectInformValueToselect,WorkFlowEventsHistory.email,WorkFlowEventsHistory.ccemail,WorkFlowStep.AliasNameSave,WorkFlowStep.AliasNameApprove,WorkFlowStep.AliasNameReject,WorkFlowStep.AllowUseUploadMgr, WorkFlowStep.AllowMoreUploadFile, WorkFlowStep.AllowDeleteFile,WorkFlowStep.AutoExpand,WorkFlowStep.EventAfterSaveInformEmailContent,
                                                                      WorkFlowStep.EventAfterApproveInformEmailContent,WorkFlowStep.EventAfterRejectInformEmailContent,(select top 1 WFName from WorkFlow where WFRowId=WorkFlowRunningStatus.WFRowId ) as WFName,WorkFlowStep.guideline,WorkFlowStep.JavaScriptOnload,WorkFlowStep.JavaScriptOnload,WorkFlowStep.DinamicFiledSaveValueToStep,WorkFlowStep.DinamicFiledApproveValueToStep,WorkFlowStep.DinamicFiledRejectValueToStep,WorkFlowStep.EventAfterSaveEmailSubject,WorkFlowStep.EventAfterApproveEmailSubject,WorkFlowStep.EventAfterRejectEmailSubject,
                                                                     WorkFlowStep.TriggerSave, WorkFlowStep.HideButtonSave, 
                                                                     WorkFlowStep.CheckTiggerSave, WorkFlowStep.TriggerApprove, WorkFlowStep.HideButtonApprove, WorkFlowStep.CheckTiggerApprove, WorkFlowStep.TriggerReject, WorkFlowStep.HideButtonReject, 
                                                                     WorkFlowStep.CheckTiggerReject, WorkFlowStep.StepAutoStart, WorkFlowStep.TiggerStartStepCondition, WorkFlowStep.TiggerStartStepConditionButton, WorkFlowStep.EventAfterSaveUpdateValueETC1, 
                                                                     WorkFlowStep.EventAfterSaveUpdateValueETC2, WorkFlowStep.EventAfterSaveUpdateValueETC3, WorkFlowStep.EventAfterSaveUpdateValueETC4, WorkFlowStep.EventAfterSaveUpdateValueETC5, 
                                                                     WorkFlowStep.EventAfterApproveUpdateValueETC1, WorkFlowStep.EventAfterApproveUpdateValueETC2, WorkFlowStep.EventAfterApproveUpdateValueETC3, WorkFlowStep.EventAfterApproveUpdateValueETC4, 
                                                                     WorkFlowStep.EventAfterApproveUpdateValueETC5, WorkFlowStep.EventAfterRejectUpdateValueETC1, WorkFlowStep.EventAfterRejectUpdateValueETC2, WorkFlowStep.EventAfterRejectUpdateValueETC3, 
                                                                     WorkFlowStep.EventAfterRejectUpdateValueETC4, WorkFlowStep.EventAfterRejectUpdateValueETC5, WorkFlowStep.PassValueOldTemplate,WorkFlowStep.CheckConditionAutoStartSave,WorkFlowStep.CheckConditionAutoStartApprove,WorkFlowStep.CheckConditionAutoStartReject,WorkFlowRunningStatus.ParentStartRowID,
                                                                    WorkFlowStep.EventSaveDonotSendMail, WorkFlowStep.EventApproveDonotSendMail, WorkFlowStep.EventRejectDonotSendMail
                                                FROM         WorkFlowEventsHistory INNER JOIN
                                                                      WorkFlowRunningStatus ON WorkFlowEventsHistory.StartRowID = WorkFlowRunningStatus.StartRowID INNER JOIN
                                                                      WorkFlowStep ON WorkFlowRunningStatus.CurrentStepRowId = WorkFlowStep.StepRowId INNER JOIN
                                                                      WorkFlowTemplate ON WorkFlowStep.TemplateRowId = WorkFlowTemplate.TemplateRowId LEFT OUTER JOIN
                                                                      WorkFlowStep AS WorkFlowStep_1 ON WorkFlowEventsHistory.StepRowID = WorkFlowStep_1.StepRowId
                                                WHERE     (WorkFlowEventsHistory.StartRowID = '" + runingid+@"')
                                                ORDER BY WorkFlowEventsHistory.CrDate DESC;");


        if (ds.Tables[0].Rows.Count > 0)
        {
            loadGuideline(ds.Tables[0].Rows[0]["guideline"].ToString());
            dataInfor = ds;
            lb_currentstep.Text = "Step :" + ds.Tables[0].Rows[0]["StepName"].ToString() + "  (<font color='green'>" + ds.Tables[0].Rows[0]["status"].ToString() + "</font>)";
            currentstep_rowid.Value = ds.Tables[0].Rows[0]["CurrentStepRowId"].ToString();
            string content_txt = "";

            wftitle.Text = ds.Tables[0].Rows[0]["WFName"].ToString();
            if (ds.Tables[0].Rows[0]["oldtemplaterowid"].ToString() != ds.Tables[0].Rows[0]["currenttemplaterowid"].ToString())
            {
                content_txt = ds.Tables[0].Rows[0]["TemplateContent"].ToString();
            }
            else
            {
                content_txt = ds.Tables[0].Rows[0]["currentbody"].ToString();
            }


            //string content_txt = (ds.Tables[0].Rows[0]["CurrentStepRowId"].ToString() == "" ? ds.Tables[0].Rows[0]["TemplateContent"].ToString() : ds.Tables[0].Rows[0]["ContentBody"].ToString());

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content_txt);


            doc = appendTableDataBindHtml(doc);
            

            DataSet dd = SQLExecuteReader("system", @"SELECT     StepEnableObject.RequireField, TemplateObject.ObjectName, StepEnableObject.StepEnableObjectId, StepEnableObject.BindRowID, DataBindValue.Server, DataBindValue.Sql, 
                                                                          DataBindValue.DataFieldText, DataBindValue.DataFieldValue,StepEnableObject.defaultval,PassValueOldTemplate,Afteraction
                                                    FROM         DataBindValue RIGHT OUTER JOIN
                                                                          StepEnableObject ON DataBindValue.BindRowID = StepEnableObject.BindRowID RIGHT OUTER JOIN
                                                                          WorkFlowStep INNER JOIN
                                                                          TemplateObject ON WorkFlowStep.TemplateRowId = TemplateObject.TemplateRowId ON StepEnableObject.StepRowId = WorkFlowStep.StepRowId AND 
                                                                          StepEnableObject.TemplateRowId = TemplateObject.TemplateRowId AND StepEnableObject.ObjectRowId = TemplateObject.ObjectRowId
                                                        WHERE     (WorkFlowStep.StepRowId ='" + currentstep_rowid.Value + "')  order by RequireField;");


            #region template bind inherit

            if (dd.Tables[0].Rows[0]["PassValueOldTemplate"].ToString() == "Y")
            {
                DataSet ds_ = SQLExecuteReader("system", "SELECT TOP (1) * FROM  WorkFlowRunningStatus where StartRowID='" + runingid + "' and  CurrentStepRowId='" + currentstep_rowid.Value + "' order by crdate desc");
                if (ds_.Tables[0].Rows.Count > 0)
                {
                    HtmlAgilityPack.HtmlDocument doc_old = new HtmlAgilityPack.HtmlDocument();
                    doc_old.LoadHtml(ds_.Tables[0].Rows[0]["contentbody"].ToString());


                    for (int i = 0; i < dd.Tables[0].Rows.Count; i++)
                    {

                        if (doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null)
                        {
                            if (doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("type", "").ToString() != "file")
                            {
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ParentNode.ReplaceChild(doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()));

                            }
                        }
                    }
                }


                //############## resume pass old template value 

                if (Request.QueryString["resumestartrowid"] != null)
                {
                    DataSet ds_1 = SQLExecuteReader("system", "SELECT contentbody FROM  WorkFlowRunningStatus where ParentStartRowID='" + Request.QueryString["resumestartrowid"] + "' and status='Finished'");
                     if (ds_1.Tables[0].Rows.Count > 0)
                     {
                         HtmlAgilityPack.HtmlDocument doc_old = new HtmlAgilityPack.HtmlDocument();
                         for (int j = 0; j < ds_1.Tables[0].Rows.Count; j++)
                         {
                             doc_old.LoadHtml("");
                             doc_old.LoadHtml(ds_1.Tables[0].Rows[j]["contentbody"].ToString());

                             for (int i = 0; i < dd.Tables[0].Rows.Count; i++)
                             {

                                 if (doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null && doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString())!=null)
                                 {
                                     if (doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("type", "").ToString() != "file")
                                     {
                                         HtmlAgilityPack.HtmlNode a = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString());
                                         HtmlAgilityPack.HtmlNode b = doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString());
                                         switch (a.Name.ToLower())
                                         {
                                             case "input":

                                                 if (a.Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                                     doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("checked", b.GetAttributeValue("checked", ""));
                                                 else
                                                     doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", b.GetAttributeValue("value", ""));
                                                 break;
                                             case "textarea":
                                                 if (a.LastChild != null)
                                                 {
                                                     doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ReplaceChild(b.LastChild, doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).LastChild);
                                                 }
                                                 else
                                                 {
                                                     if (b.LastChild != null)
                                                     {
                                                         doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(b.LastChild);
                                                     }
                                                 }
                                                 break;
                                             case "select":
                                                // doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ParentNode.ReplaceChild(doc_old.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()));

                                                 for (int x = 0; x < b.ChildNodes.Count; x++)
                                                 {

                                                     if (a.ChildNodes[x].GetAttributeValue("value", "") == a.ChildNodes[x].GetAttributeValue("value", "") && b.ChildNodes[x].GetAttributeValue("selected", "") == "selected")
                                                     {
                                                         doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].SetAttributeValue("selected", "selected");
                                                     }
                                                     else
                                                     {
                                                         if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"] != null)
                                                         {
                                                             doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"].Remove();
                                                         }
                                                     }
                                                 }

                                                 break;
                                         }
                                     }
                                 }
                             }
                         }
                     }
                }

                //end ############## resume pass old template value 

            }

            #endregion

            
            for (int i = 0; i < dd.Tables[0].Rows.Count; i++)
            {

                if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()] != null)
                {

                    //############################# bind dropdrowlist

                    #region bind dropdrowlist
                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null)
                    {
                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "select":
                                if (dd.Tables[0].Rows[i]["bindrowid"].ToString() != "")
                                {
                                    try
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAllChildren();

                                        DataSet ds__ = new DataSet();
                                        if (dd.Tables[0].Rows[i]["Server"].ToString().ToUpper().IndexOf("ORA_") == 0)
                                        {
                                            serviceoracle.Service1 sv = new serviceoracle.Service1();
                                            ds__ = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dd.Tables[0].Rows[i]["Server"].ToString()].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }
                                        else
                                        {
                                            ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }
                                        //DataSet ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        for (int j = 0; j < ds__.Tables[0].Rows.Count; j++)
                                        {
                                            HtmlAgilityPack.HtmlNode n = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                            n.SetAttributeValue("value", ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldValue"].ToString()].ToString());
                                            n.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode(ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldText"].ToString()].ToString()));
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Add(n);
                                        }

                                        HtmlAgilityPack.HtmlNode nn = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                        nn.SetAttributeValue("value", "");
                                        nn.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode("  Please select  "));

                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Insert(0, nn);
                                    }
                                    catch { }

                                }
                                break;
                        }
                    }
                    #endregion
                    //############################# END dropdrowlist


                    //## new setdefault
                    #region new setdefault
                    String checkbypassPostback = "";
                    try
                    {
                        checkbypassPostback = (Request.QueryString["bypass"] == null ? "" : Request.QueryString["bypass"].ToString());
                    }
                    catch { }

                    if (checkbypassPostback != "update")
                    {
                        if (dd.Tables[0].Rows[i]["Afteraction"].ToString() == "Y")
                        {
                            if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() == "")
                            {
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", "");
                            }
                            doc = setdefault(doc, dd.Tables[0].Rows[i]);
                        }
                        else
                        {
                            String ddclass = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("class", "");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("class", ddclass.Replace("defaultpostback", ""));
                        }
                    }
                    #endregion new setdefault
                    //end


                    #region set oldvalue

                  //  if (dd.Tables[0].Rows[i]["ObjectName"].ToString() == "P10_r10_mainreason")
                  //  {
                  //      String ddddd = Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString();
                  //  }

                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null)
                    {

                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "input":
                                try
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("checked", "checked");
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "OK" });
                                    }
                                    else
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                    }
                                }
                                catch
                                {
                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                }
                                break;
                            case "textarea":
                               // Response.Write(Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString());
                                if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() != "")
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count > 0)
                                    {
                                        String ffff = Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString();
                                        HtmlNode nn_new = HtmlAgilityPack.HtmlNode.CreateNode("<div>"+Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString()+"</div>");
                                        HtmlNode nn = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString());
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ReplaceChild(nn_new.LastChild, nn.LastChild);

                                    }
                                    else
                                    {
                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString()));
                                    }
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                }
                                else
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count > 0)
                                    {
                                        try
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Remove(doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).LastChild);
                                        }
                                        catch { }
                                    }
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).InnerText });
                                }
                                break;
                            case "select":
                                if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() != "")
                                {
                                    for (int x = 0; x < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; x++)
                                    {

                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].GetAttributeValue("value", "") == Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString())
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].SetAttributeValue("selected", "selected");
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                        }
                                        else
                                        {
                                            if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"] != null)
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"].Remove();
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    for (int x = 0; x < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; x++)
                                    {
                                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"] != null)
                                        {
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"].Remove();
                                        }
                                    }
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()].ToString() });
                                }
                                break;
                        }
                    }
                    #endregion


                }
                else  //####################   get old value to fieldinput
                {
                    //############################# bind dropdrowlist

                    #region set old value from database disable control
                    try
                    {

                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "select":
                                if (dd.Tables[0].Rows[i]["bindrowid"].ToString() != "")
                                {
                                    try
                                    {
                                        string ccc = "";
                                        for (int b = 0; b < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; b++)
                                        {
                                            String v = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("selected", "");
                                            if (v == "selected")
                                            {
                                                ccc = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("value", "");
                                            }
                                        }

                                      

                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAllChildren();

                                        DataSet ds__ = new DataSet();
                                        if (dd.Tables[0].Rows[i]["Server"].ToString().ToUpper().IndexOf("ORA_") == 0)
                                        {
                                            serviceoracle.Service1 sv = new serviceoracle.Service1();
                                            ds__ = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dd.Tables[0].Rows[i]["Server"].ToString()].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }
                                        else
                                        {
                                            ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        }

                                       // DataSet ds__ = SQLExecuteReader(dd.Tables[0].Rows[i]["Server"].ToString(), dd.Tables[0].Rows[i]["sql"].ToString());
                                        for (int j = 0; j < ds__.Tables[0].Rows.Count; j++)
                                        {
                                            HtmlAgilityPack.HtmlNode n = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                            n.SetAttributeValue("value", ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldValue"].ToString()].ToString());
                                            n.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode(ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldText"].ToString()].ToString()));
                                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Add(n);

                                            if (!IsPostBack)
                                            {
                                                if (ccc == ds__.Tables[0].Rows[j][dd.Tables[0].Rows[i]["DataFieldValue"].ToString()].ToString())
                                                {
                                                    n.SetAttributeValue("selected", "selected");
                                                }
                                            }
                                        }
                                        HtmlAgilityPack.HtmlNode nn = HtmlAgilityPack.HtmlNode.CreateNode("<option>");
                                        nn.SetAttributeValue("value", "");
                                        nn.ChildNodes.Add(HtmlAgilityPack.HtmlNode.CreateNode("  Please select  "));

                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Insert(0, nn);


                                       
                                    }
                                    catch { }

                                }
                                break;

                            case "input":

                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                {

                                    try
                                    {
                                        if (IsPostBack)
                                        {
                                            if (Request.Form[dd.Tables[0].Rows[i]["ObjectName"].ToString()] == null && (dd.Tables[0].Rows[i]["StepEnableObjectId"].ToString() != "" || Request.QueryString["bypass"] != null))
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["checked"] != null)
                                                {
                                                    if (Request.QueryString["bypass"] != null && Request.QueryString["bypass"].ToString().ToLower() != "update")
                                                    {
                                                    }
                                                    else
                                                    {
                                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["checked"].Remove();
                                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "" });

                                                    }
                                                }
                                            }


                                        }
                                    }
                                    catch { }


                                }
                                break;
                        }

                        //############################# END dropdrowlist

                        switch (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower())
                        {
                            case "input":

                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                {
                                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("checked", "") == "checked")
                                    {
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "OK" });
                                    }
                                    else
                                    {
                                        fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), "" });
                                    }

                                }
                                else
                                {
                                    fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("value", "") });
                                }
                                break;
                            case "textarea":
                                fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).InnerText });
                                break;
                            case "select":
                                try
                                {


                                    for (int b = 0; b < doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes.Count; b++)
                                    {
                                        String v = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("selected", "");
                                        if (v == "selected")
                                        {
                                            fieldinput.Add(new String[] { dd.Tables[0].Rows[i]["ObjectName"].ToString(), doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).ChildNodes[b].GetAttributeValue("value", "") });
                                        }
                                    }
                                }
                                catch { }

                                break;
                        }
                    }
                    catch (Exception ex) { }

                    #endregion


                }


                try
                {

                    //########  dd.Tables[0].Rows[i]["ObjectName"].ToString() = null is disable object    //// finished all control disiable also
                    if ((doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null && dd.Tables[0].Rows[i]["StepEnableObjectId"].ToString() == "") || ds.Tables[0].Rows[0]["Status"].ToString() == "Hold"  || ds.Tables[0].Rows[0]["Status"].ToString() == "Finished" || (ds.Tables[0].Rows[0]["rowid"].ToString() != (Request.QueryString["referencekey"] != null ? Request.QueryString["referencekey"].ToString() : "")))
                    {

                        #region set disable object from step
                        String vvv = "";
                        try
                        {
                            if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("disabled", "") != "disabled")
                            {
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Add("disabled", "disabled");
                            }
                            vvv = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("background-color:", "background:") + ";background-color:#FAF8F8");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("accesskey");
                            String ddclass = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("class", "");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("class", ddclass.Replace("defaultpostback", ""));
                        }
                        catch { }
                        #endregion
                    }
                    else
                    {


                        #region setdefault
                        //################## set default value

                        String checkbypass = "";
                        try
                        {
                            checkbypass = (Request.QueryString["bypass"] == null ? "" : Request.QueryString["bypass"].ToString());
                        }
                        catch { }

                        if (checkbypass != "update" && !IsPostBack)
                        {


                            #region setdefault
                            if (dd.Tables[0].Rows[i]["Afteraction"].ToString() != "Y")
                            {
                                doc = setdefault(doc, dd.Tables[0].Rows[i]);
                                String ddclass = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("class", "");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("class", ddclass.Replace("defaultpostback", ""));
                            }
                            else
                            {
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Append("class", "defaultpostback");
                            }

                            #endregion

                            #region oldval
                            /*
                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("value", "") == "" && dd.Tables[0].Rows[i]["defaultval"].ToString() != "")
                                {
                                    String finalSQL = dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower();
                                    string[] pram = dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower().Replace("sql{", "").Split(("{").ToCharArray());
                                    for (int v = 0; v < pram.Length; v++)
                                    {
                                        pram[v] = "{" + pram[v];
                                        if (pram[v].IndexOf("}") > 0)
                                        {
                                            pram[v] = pram[v].Substring(0, pram[v].IndexOf("}") + 1);
                                            if (pram[v].Substring(0, 1) == "{" && pram[v].Substring(pram[v].Length - 1, 1) == "}")
                                            {
                                                if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")) != null)
                                                {
                                                   // if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", "") != "")
                                                   // {
                                                       // string sss = doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", "");
                                                       // String ccc = pram[v].Replace("{", "").Replace("}", "");
                                                        if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).Name.ToLower() == "select")
                                                        {

                                                            for (int b = 0; b < doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes.Count; b++)
                                                            {
                                                                String x = doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("selected", "");
                                                                if (x == "selected")
                                                                {
                                                                    finalSQL = finalSQL.ToLower().Replace(pram[v].ToLower(), doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("value", ""));
                                                                }
                                                            }

                                                            finalSQL = finalSQL.ToLower().Replace(pram[v].ToLower(), "?");
                                                           
                                                        }
                                                        else
                                                        {
                                                            finalSQL = finalSQL.ToLower().Replace(pram[v].ToLower(), doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", ""));
                                                        }
                                                   // }
                                                }
                                            }
                                        }
                                    }

                                    switch (dd.Tables[0].Rows[i]["defaultval"].ToString().ToLower().Trim())
                                    {
                                        case "{sys_date}":
                                            if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("yyyy-MM-dd")));
                                            }
                                            else
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("yyyy-MM-dd"));
                                            }
                                            break;
                                        case "{sys_time}":
                                            if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("HH:mm")));
                                            }
                                            else
                                            {
                                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("HH:mm"));
                                            }
                                            break;
                                        case "{sys_username}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(isWho()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", isWho());
                                                }
                                            }
                                            catch { }
                                            break;
                                        case "{sys_workflowname}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(ds.Tables[0].Rows[0]["wfName"].ToString()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", ds.Tables[0].Rows[0]["wfName"].ToString());
                                                }
                                            }
                                            catch { }
                                            break;
                                        case "{sys_currentstepname}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(ds.Tables[0].Rows[0]["StepName"].ToString()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", ds.Tables[0].Rows[0]["StepName"].ToString());
                                                }
                                            }
                                            catch { }
                                            break;

                                        case "{sys_workflowid}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["rowid"].ToString()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["rowid"].ToString());
                                                }
                                            }
                                            catch { }
                                            break;
                                        case "{sys_startworkflowid}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["runingkey"].ToString()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["runingkey"].ToString());
                                                }
                                            }
                                            catch { }
                                            break;
                                        case "{sys_referenceeventhistory}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["referencekey"].ToString()));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["referencekey"].ToString());
                                                }
                                            }
                                            catch { }
                                            break;
                                        case "{sys_logonemail}":
                                            try
                                            {
                                                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Name.ToLower() == "textarea")
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(getemailbyuser(isWho())));
                                                }
                                                else
                                                {
                                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", getemailbyuser(isWho()));
                                                }
                                            }
                                            catch { }
                                            break;
                                        default:
                                            if (finalSQL.ToLower().IndexOf("sql{") >= 0)
                                            {
                                                try
                                                {
                                                    DataSet dds = SQLExecuteReader("system", finalSQL.ToLower().Replace("sql{", "").Trim().Substring(0, finalSQL.ToLower().Replace("sql{", "").Trim().Length - 1));
                                                    if (dds.Tables.Count > 0)
                                                    {
                                                        string sss = dds.Tables[0].Rows[0][0].ToString();
                                                        doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", dds.Tables[0].Rows[0][0].ToString().ToUpper());
                                                    }
                                                }
                                                catch { }
                                            }
                                            else
                                            {
                                                try { doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("value", finalSQL); }
                                                catch { }
                                            }
                                            break;
                                    }
                                }*/
                            #endregion
                        }
                        //###################
                        #endregion

                        #region check require filed && non require
                        if (dd.Tables[0].Rows[i]["RequireField"].ToString() == "Y")
                        {
                            String vvv = "";
                            try
                            {
                                vvv = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("display:none;", "").Replace("display: none;", "").Replace("display: none", "").Replace("display:none", "").Replace("background-color:", "background:") + ";background-color:yellow");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("disabled");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("accesskey");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("accesskey", "require");

                               
                            }
                            catch { }
                        }
                        else
                        {
                            String vvv = "";
                            try
                            {
                                vvv = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("display:none;", "").Replace("display: none;", "").Replace("display: none", "").Replace("display:none", "").Replace("background-color:", "background:") + ";background-color:#ffffff;border-style: solid;border-width: 1px;");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("disabled");
                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("accesskey");

                                
                            }
                            catch
                            {
                            }
                        }
                        #endregion
                    }
                }
                catch { }


                #region upload file
                //###################################### upload file
                if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()) != null)
                {//<div style="color: red; margin-top: 5px;">  X</div>
                    Boolean candeletefile = true;
                    if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"] != null)
                    {
                        if (doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower() == "file")
                        {
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", (dd.Tables[0].Rows[i]["RequireField"].ToString() == "Y" ? doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "") + ";border: 3px solid yellow" : doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "")).Replace("display:none;", "").Replace("display: none;", "").Replace("display: none", "").Replace("display:none", ""));

                            DataSet dfile = this.SQLExecuteReader("system", "select * from Attechment where Ref_ID='" + runingid + "' and Ref_obj='" + dd.Tables[0].Rows[i]["ObjectName"].ToString() + "'");



                            Boolean hideUploadFile = true;

                            if (Request.QueryString["bypass"] != null)
                            {
                                if (Request.QueryString["bypass"].ToString() == "update")
                                {
                                    hideUploadFile = false;
                                }
                            }


                            if (dd.Tables[0].Rows[i]["RequireField"].ToString() == "" && dfile.Tables[0].Rows.Count > 0)
                            {
                                //check admin can upload or delete atteched
                                if (hideUploadFile)
                                {
                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", "display:none");
                                    candeletefile = false;

                                }
                            }

                            // check history view or not ;;; if history view hind
                            if (ds.Tables[0].Rows[0]["rowid"].ToString() != (Request.QueryString["referencekey"] != null ? Request.QueryString["referencekey"].ToString() : "") || ds.Tables[0].Rows[0]["Status"].ToString() == "Finished")
                            {
                                if (hideUploadFile)
                                {
                                    doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", "display:none");
                                    candeletefile = false;
                                }
                            }

                            if (dfile.Tables[0].Rows.Count > 0)
                            {
                                try
                                {
                                    doc.GetElementbyId("upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAll();
                                }
                                catch { }

                                doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlNode.CreateNode("<div id='upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString() + "' style='padding-left:10px;float:;padding-top:5px'></div>"));
                            }
                            else
                            {
                                try
                                {
                                    doc.GetElementbyId("upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString()).RemoveAll();
                                }
                                catch { }
                            }


                            for (int j = 0; j < dfile.Tables[0].Rows.Count; j++)
                            {
								
                                if (dfile.Tables[0].Rows[j]["name"].ToString().ToLower().IndexOf(".jpg") > 0 || dfile.Tables[0].Rows[j]["name"].ToString().ToLower().IndexOf(".jpeg") > 0 || dfile.Tables[0].Rows[j]["name"].ToString().ToLower().IndexOf(".png") > 0 || dfile.Tables[0].Rows[j]["name"].ToString().ToLower().IndexOf(".bmp") > 0)
                                {
                                    doc.GetElementbyId("upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlNode.CreateNode("<a href='download.aspx?id=" + dfile.Tables[0].Rows[j]["id"].ToString() + "' target='_blank' title='" + dfile.Tables[0].Rows[j]["name"].ToString() + "' ><img src='download.aspx?id=" + dfile.Tables[0].Rows[j]["id"].ToString() + "' style='border:0px;max-width:400px' /></a>"));
                                }
                                else
                                {
                                    if (candeletefile)
                                    {
                                        doc.GetElementbyId("upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlNode.CreateNode("<div style='float:left'><a style='float:left' href='download.aspx?id=" + dfile.Tables[0].Rows[j]["id"].ToString() + "' target='_blank' title='" + dfile.Tables[0].Rows[j]["name"].ToString() + "' ><img src='images/download_icon.jpg' style='border:0px' /></a><div style='color: red; padding-top: 5px;height:40px;float:left' class='filedel'><a href='" + Request.Url.ToString() + "&filedel=" + dfile.Tables[0].Rows[j]["id"].ToString() + "' onclick='return confirm(\"Delete?\")'; style='color:red'>X</a></div></div>"));
                                    }
                                    else
                                    {
                                        doc.GetElementbyId("upload_" + dd.Tables[0].Rows[i]["ObjectName"].ToString()).AppendChild(HtmlNode.CreateNode("<a style='float:left' href='download.aspx?id=" + dfile.Tables[0].Rows[j]["id"].ToString() + "' target='_blank' title='" + dfile.Tables[0].Rows[j]["name"].ToString() + "' ><img src='images/download_icon.jpg' style='border:0px' /></a>"));
                                    }
                                }
                            }


                        }


                    }
                }
                #endregion




              

                #region force update value
                //################# force update

                try
                {
                    if (Request.QueryString["bypass"] != null)
                    {
                        if (Request.QueryString["bypass"].ToString().ToLower() == "update")
                        {
                            String vvv = "";
                            vvv = doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).GetAttributeValue("style", "background-color:");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).SetAttributeValue("style", vvv.Replace("background-color:", "background:") + ";background-color:#fffff0;border-style: solid;border-width: 1px;");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("disabled");
                            doc.GetElementbyId(dd.Tables[0].Rows[i]["ObjectName"].ToString()).Attributes.Remove("accesskey");
                        }
                    }

                }
                catch
                {
                }

                //###########end force update
                #endregion
            }


                #region ebale button upload manager
                //#####ebale button upload manager
            if (!IsPostBack)
            {
                string updaat2 = "";
                if (Request.QueryString["bypass"] != null)
                    if (Request.QueryString["bypass"].ToString().ToLower() == "update")
                        updaat2 = "update";

                if (updaat2 == "update")
                {
                    uploadfile.Visible = utility_update.Visible = true;
                    refreshEtc();
                    LinkButton1_Click(this, new EventArgs());

                }
                else if (ds.Tables[0].Rows[0]["Status"].ToString() == "Active" && ds.Tables[0].Rows[0]["rowid"].ToString() != (Request.QueryString["referencekey"] != null ? Request.QueryString["referencekey"].ToString() : ""))
                {
                    uploadfile.Visible = false;
                    GridView1.Columns[5].Visible = GridView1.Columns[1].Visible = Button4.Visible = false;
                    uploadmore.Visible = false;
                }
                else if (ds.Tables[0].Rows[0]["Status"].ToString() == "Finished")
                {
                    DataSet dfile = this.SQLExecuteReader("system", "select * from Attechment where Ref_ID='" + runingid + "'");
                    if (dfile.Tables[0].Rows.Count > 0)
                    {
                        uploadfile.Visible = true;
                        GridView1.Columns[5].Visible = GridView1.Columns[1].Visible = Button4.Visible = false;
                        uploadmore.Visible = false;
                    }
                    else
                    {
                        uploadfile.Visible = false;
                    }

                }
                else
                {
                    uploadfile.Visible = (ds.Tables[0].Rows[0]["AllowUseUploadMgr"].ToString() == "Y");
                    if (ds.Tables[0].Rows[0]["AutoExpand"].ToString() == "Y")
                    {
                        LinkButton1_Click(this, new EventArgs());
                    }

                    GridView1.Columns[5].Visible = GridView1.Columns[1].Visible = Button4.Visible = (ds.Tables[0].Rows[0]["AllowDeleteFile"].ToString() == "Y");

                    uploadmore.Visible = (ds.Tables[0].Rows[0]["AllowMoreUploadFile"].ToString() == "Y");
                }
            }

            //######################################end upload file
                #endregion




            #region enable button
            //######## enable button
            string updaat ="";
            if (Request.QueryString["bypass"] != null)
                if (Request.QueryString["bypass"].ToString().ToLower() == "update")
                    updaat = "update";


            if (updaat == "update")
            {
                btn_save.Visible = true;
                btn_approve.Visible = false;
                btn_reject.Visible = false;
            }
            else if (ds.Tables[0].Rows[0]["Status"].ToString() == "Active" && ds.Tables[0].Rows[0]["rowid"].ToString() == (Request.QueryString["referencekey"] != null ? Request.QueryString["referencekey"].ToString() : ""))
            {
                //Update add function tigger
                btn_save.Visible = (ds.Tables[0].Rows[0]["EnableSave"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonSave"].ToString() == "N" ? true : false);
                btn_approve.Visible = (ds.Tables[0].Rows[0]["EnableApprove"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonApprove"].ToString() == "N" ? true : false);
                btn_reject.Visible = (ds.Tables[0].Rows[0]["EnableReject"].ToString() == "Y" && ds.Tables[0].Rows[0]["HideButtonReject"].ToString() == "N" ? true : false);
                //endUpdate add function tigger

                btn_save.Text = (ds.Tables[0].Rows[0]["AliasNameSave"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameSave"].ToString() : btn_save.Text);
                btn_approve.Text = (ds.Tables[0].Rows[0]["AliasNameApprove"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameApprove"].ToString() : btn_approve.Text);
                btn_reject.Text = (ds.Tables[0].Rows[0]["AliasNameReject"].ToString() != "" ? ds.Tables[0].Rows[0]["AliasNameReject"].ToString() : btn_reject.Text);

                if (ds.Tables[0].Rows[0]["Email"].ToString().ToLower().IndexOf(isWho().ToLower()) < 0 && ds.Tables[0].Rows[0]["Email"].ToString().ToLower().IndexOf(getemailbyuser(isWho().ToLower()).ToLower()) < 0 && "force" != (Request.QueryString["bypass"] != null ? Request.QueryString["bypass"].ToString() : ""))
                {
                    btn_save.Enabled = false;
                    btn_approve.Enabled = false;
                    btn_reject.Enabled = false;

                    btn_save.BackColor = System.Drawing.Color.Gray;
                    btn_approve.BackColor = System.Drawing.Color.Gray;
                    btn_reject.BackColor = System.Drawing.Color.Gray;
                    Literal1.Text = "<br /><font color=red>*** หมายเหตุ : Step Action นี้เป็นของ ( <b>" + ds.Tables[0].Rows[0]["Email"].ToString().ToLower().Replace("@bst.co.th", "") + "</b> )</font>";

                    ClientScript.RegisterStartupScript(this.GetType(), "www", "$('.filedel').hide();", true);
                }
                else
                {
                    btn_save.Enabled = true;
                    btn_approve.Enabled = true;
                    btn_reject.Enabled = true;


                    if ("force" == (Request.QueryString["bypass"] != null ? Request.QueryString["bypass"].ToString() : ""))
                    {
                        Literal1.Text = "<br /><font color=red>*** หมายเหตุ : Step Action นี้เป็นของ ( <b>" + ds.Tables[0].Rows[0]["Email"].ToString().ToLower().Replace("@bst.co.th","") + "</b> ) คุณกำลังทำหน้าที่นี้แทนคุณเหล่านี้ โปรดตรวจสอบให้แน่ใจก่อนการกระทำ Action ปุ่มต่างๆ</font>";
                    }

                }

                otherscript.Text = @"<script>
                                    function onSaveClick()
                                    {
					" + (ds.Tables[0].Rows[0]["EventBeforeSaveCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeSaveCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>
                                <script>
                                    function onApproveClick()
                                    {

					" + (ds.Tables[0].Rows[0]["EventBeforeApproveCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeApproveCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>
                                <script>
                                    function onRejectClick()
                                    {
                                     
					" + (ds.Tables[0].Rows[0]["EventBeforeRejectCallJavascriptFunction"].ToString()==""?"return true":ds.Tables[0].Rows[0]["EventBeforeRejectCallJavascriptFunction"].ToString()) + @";
                                    }
                                </script>";

                btn_save.OnClientClick = "return OnSaveCheck();";
                btn_approve.OnClientClick = "return OnApproveCheck();";
                btn_reject.OnClientClick = "return OnRejectCheck();";
            }
            else
            {
                btn_save.Visible = false;
                btn_approve.Visible = false;
                btn_reject.Visible = false;


            }
           
            //########### end enable button
             #endregion
            
            content.Text = doc.DocumentNode.OuterHtml.ToString();

            if (!IsPostBack && Request.QueryString["bypass"]==null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "onstart", ds.Tables[0].Rows[0]["JavaScriptOnload"].ToString(), true);
            }

            getChildFlow();
        }
    }


    private HtmlAgilityPack.HtmlDocument appendTableDataBindHtml(HtmlAgilityPack.HtmlDocument doc)
    {
   
        foreach (string key in Request.Form.AllKeys)
        {
            if (key.ToString().IndexOf("htbb")>0)
            {
                try
                {
                    var dd = doc.GetElementbyId(key.Replace("-htbb", "")).InnerHtml;
                    doc.GetElementbyId(key.Replace("-htbb", "")).ChildNodes.Clear();
                    HtmlNode html = HtmlNode.CreateNode(Request.Form[key].ToString());
                    html.InnerHtml = Request.Form[key].ToString();
                    doc.GetElementbyId(key.Replace("-htbb", "")).AppendChild(html);
                   
                }
                catch { }
            }
        }

        return doc;
    }


    private void loadGuideline(String content)
    {
        guideline_content.Text = "";
        if (content != "" && content.Replace(" ", "").Replace("\n", "") != @"<p><br/></p>")
        {
            if (content.Length > 500)
            {
                guideline_content.Text = content;
                linkmore.Visible = true;
            }
            else
            {
                guideline_content.Text = content;
                linkmore.Visible = false;
            }
            guideline.Visible = true;
        }
        else
        {
            guideline.Visible = false;
        }
    }


    #region Procedure setdefault  ## HtmlDocument DOM Object
    private HtmlAgilityPack.HtmlDocument setdefault(HtmlAgilityPack.HtmlDocument doc, DataRow dd)
    {
        string value_ = "";
        //################## set default value


        if (doc.GetElementbyId(dd["ObjectName"].ToString()).GetAttributeValue("value", "") == "" && dd["defaultval"].ToString() != "")
        {
            String finalSQL = dd["defaultval"].ToString().ToLower();
            String finalSQL_org = dd["defaultval"].ToString();
            string[] pram = dd["defaultval"].ToString().ToLower().Replace("sql{", "").Split(("{").ToCharArray());
            for (int v = 0; v < pram.Length; v++)
            {
                pram[v] = "{" + pram[v];
                if (pram[v].IndexOf("}") > 0)
                {
                    pram[v] = pram[v].Substring(0, pram[v].IndexOf("}") + 1);
                    if (pram[v].Substring(0, 1) == "{" && pram[v].Substring(pram[v].Length - 1, 1) == "}")
                    {
                        if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")) != null)
                        {
                            if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).Name.ToLower() == "select")
                            {

                                for (int b = 0; b < doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes.Count; b++)
                                {
                                    String x = doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("selected", "");
                                    if (x == "selected")
                                    {
                                        finalSQL = finalSQL.ToLower().Replace(pram[v].ToLower(), doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).ChildNodes[b].GetAttributeValue("value", ""));
                                    }
                                }
                                finalSQL = finalSQL.Replace(pram[v], "?");
                            }
                            else
                            {
                                try
                                {
                                    if (doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                                    {
                                        finalSQL = finalSQL.Replace(pram[v], doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("checked", "").Replace("checked", "OK"));
                                    }
                                    else
                                    {
                                        finalSQL = finalSQL.Replace(pram[v], doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", ""));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    finalSQL = finalSQL.Replace(pram[v], doc.GetElementbyId(pram[v].Replace("{", "").Replace("}", "")).GetAttributeValue("value", ""));
                                }
                            }
                        }
                    }
                }
            }

            switch (dd["defaultval"].ToString().ToLower().Trim())
            {
                case "{sys_date}":
                    if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                    {
                        doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("yyyy-MM-dd")));
                    }
                    else
                    {
                        doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    value_ = DateTime.Now.ToString("yyyy-MM-dd");
                    break;
                case "{sys_time}":
                    if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                    {
                        doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(DateTime.Now.ToString("HH:mm")));
                    }
                    else
                    {
                        doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", DateTime.Now.ToString("HH:mm"));
                    }
                    value_ = DateTime.Now.ToString("HH:mm");
                    break;
                case "{sys_username}":
                    try
                    {
                        if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(isWho()));
                        }
                        else
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", isWho());
                        }
                        value_ = isWho();
                    }
                    catch { }
                    break;
                case "{sys_workflowid}":
                    try
                    {
                        if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["rowid"].ToString()));
                        }
                        else
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["rowid"].ToString());
                        }
                        value_ = Request.QueryString["rowid"].ToString();
                    }
                    catch { }
                    break;
                case "{sys_startworkflowid}":
                    try
                    {
                        if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["runingkey"].ToString()));
                        }
                        else
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["runingkey"].ToString());
                        }
                        value_ = Request.QueryString["runingkey"].ToString();
                        value_ = (value_ == "" ? globalStartFlowID : value_);
                    }
                    catch { }
                    break;
                case "{sys_referenceeventhistory}":
                    try
                    {
                        if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString["referencekey"].ToString()));
                        }
                        else
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", Request.QueryString["referencekey"].ToString());
                        }
                        value_ = Request.QueryString["referencekey"].ToString();
                    }
                    catch { }
                    break;
                case "{sys_logonemail}":
                    try
                    {
                        if (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower() == "textarea")
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(getemailbyuser(isWho())));
                        }
                        else
                        {
                            doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", getemailbyuser(isWho()));
                        }
                        value_ = getemailbyuser(isWho());
                    }
                    catch { }
                    break;
                default:
                    value_ = finalSQL;
                    if (finalSQL.IndexOf("sql{") >= 0)
                    {
                        try
                        {
                            DataSet dds = SQLExecuteReader("system", finalSQL.ToLower().Replace("sql{", "").Trim().Substring(0, finalSQL.ToLower().Replace("sql{", "").Trim().Length - 1));
                            if (dds.Tables.Count > 0)
                            {
                                string ddd = dds.Tables[0].Rows[0][0].ToString().ToUpper();
                                //doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", dds.Tables[0].Rows[0][0].ToString().ToUpper());
                                value_ = dds.Tables[0].Rows[0][0].ToString().ToUpper();

                                #region new default sql update 21-06-2019
                                try
                                {
                                    switch (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower())
                                    {
                                        case "input":
                                            try
                                            {
                                                String type = doc.GetElementbyId(dd["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower();
                                                if (type == "checkbox")
                                                {
                                                    if (value_ == "OK")
                                                    {
                                                        ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').prop('checked',true);}catch(err){}", true);
                                                    }
                                                }
                                                if (type == "text" || type == "hidden")
                                                {
                                                    ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').val('" + value_.Replace("'", "''") + "');}catch(err){}", true);
                                                }


                                            }
                                            catch (Exception ec) { }
                                            break;
                                        case "textarea":
                                            try
                                            {
                                                ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').val('" + value_.Replace("'", "''") + "');}catch(err){}", true);
                                            }
                                            catch (Exception ec) { }
                                            break;
                                        case "select":
                                            try
                                            {
                                                ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('select[name=\"" + dd["ObjectName"].ToString() + "\"]').val('" + value_.Replace("'", "''") + "').change();}catch(err){}", true);
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                            break;
                                    }

                                }
                                catch { }

                                #endregion new default sql update 21-06-2019

                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {

                            //## change 16/2/2019
                            switch (doc.GetElementbyId(dd["ObjectName"].ToString()).Name.ToLower())
                            {
                                case "input":
                                    try
                                    {
                                        String type = doc.GetElementbyId(dd["ObjectName"].ToString()).Attributes["type"].Value.ToString().ToLower();
                                        if (type == "checkbox")
                                        {
                                            if (finalSQL == "ok")
                                            {
                                                // ### Server code not work !!!! 
                                                try { doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("checked", "checked"); }catch(Exception e){}
                                                ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').prop('checked',true);}catch(err){}", true);
                                            }
                                        }
                                        if (type == "text" || type == "hidden")
                                        {
                                            // ### Server code not work !!!! 
                                            try { doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", value_); }catch (Exception e) { }
                                            ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').val('" + value_.Replace("'", "''") + "');}catch(err){}", true);
                                        }
                                      

                                    }
                                    catch (Exception ec) {
                                        try { doc.GetElementbyId(dd["ObjectName"].ToString()).SetAttributeValue("value", value_); }
                                        catch (Exception e) { }
                                        ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').val('" + value_.Replace("'", "''") + "');}catch(err){}", true);
                                    }
                                    break;
                                case "textarea":
                                    try
                                    {
                                        // ### Server code not work !!!! 
                                        // doc.GetElementbyId(dd["ObjectName"].ToString()).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(finalSQL));
                                        ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('#" + dd["ObjectName"].ToString() + "').val('" + finalSQL_org.Replace("'", "''") + "');}catch(err){}", true);
                                    }
                                    catch (Exception ec) { }
                                    break;
                                case "select":
                                    try
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), dd["ObjectName"].ToString(), "try{$('select[name=\"" + dd["ObjectName"].ToString() + "\"]').val('" + finalSQL_org.Replace("'", "''") + "').change();}catch(err){}", true);

                                        /*  ### Server code not work !!!! 
                                        for (int x = 0; x < doc.GetElementbyId(dd["ObjectName"].ToString()).ChildNodes.Count; x++)
                                        {

                                            if (doc.GetElementbyId(dd["ObjectName"].ToString()).ChildNodes[x].GetAttributeValue("value", "").ToLower() == finalSQL.ToLower())
                                            {
                                                doc.GetElementbyId(dd["ObjectName"].ToString()).ChildNodes[x].SetAttributeValue("selected", "selected");
                                                   
                                            }
                                            else
                                            {
                                                if (doc.GetElementbyId(dd["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"] != null)
                                                {
                                                    doc.GetElementbyId(dd["ObjectName"].ToString()).ChildNodes[x].Attributes["selected"].Remove();
                                                }
                                            }
                                        }*/
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    break;
                            }
                            //####



                        }
                        catch { }
                    }
                    break;
            }

            if (dd["Afteraction"].ToString() == "Y")
            {
                if (Request.Form[dd["ObjectName"].ToString()] == null)
                {
                    var collection = System.Web.HttpContext.Current.Request.Form;
                    var propInfo = collection.GetType().GetProperty("IsReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    propInfo.SetValue(collection, false, new object[] { });
                    Request.Form.Add(dd["ObjectName"].ToString(), value_);
                }
                else
                {
                    if (Request.Form[dd["ObjectName"].ToString()].ToString() == "")
                    {

                        var collection = System.Web.HttpContext.Current.Request.Form;
                        var propInfo = collection.GetType().GetProperty("IsReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        propInfo.SetValue(collection, false, new object[] { });
                        if (dd["defaultval"].ToString().ToLower().Trim() == "{sys_startworkflowid}")
                        {
                            value_ = (value_ == "" ? globalStartFlowID : value_);
                        }
                        Request.Form[dd["ObjectName"].ToString()] = value_;

                        
                    }
                }
            }

        }
        //###################


        return doc;
    }
    #endregion

    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["bypass"] != null)
        {
            if (Request.QueryString["bypass"].ToString().ToLower() == "update")
            {
                try
                {
                    getstep(Request.QueryString["runingkey"].ToString());
                    SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + Request.QueryString["runingkey"].ToString() + "'");
                    String SQLSaveValue = "delete from TemplateInputCurrentValue where StartRowID='" + Request.QueryString["runingkey"].ToString() + "';";
                    foreach (String[] key in fieldinput)
                    {
 		    if (key[1] != "")
                    {
                        SQLSaveValue += "insert into TemplateInputCurrentValue (WFRowID,StartRowID,ObjectName,Value,CrBy,udby) values ('" + Request.QueryString["rowid"].ToString() + "','" + Request.QueryString["runingkey"].ToString() + "','" + key[0].Replace("'","''") + "','" + key[1].Replace("'","''") + "','" + isWho() + "','" + isWho() + "');";
                    }
		    }

                    SQLeExecuteNonQuery("system", SQLSaveValue);

                    UploadFileNoDelete(Request.Files, Request.QueryString["runingkey"].ToString());
                    UpdateEtcField();
                }
                catch { }
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                actionbtn("Save");
            }
        }
        else
        {
            actionbtn("Save");
        }
    }


    protected void btn_approve_Click(object sender, EventArgs e)
    {
        actionbtn("Approve");
    }

    protected void btn_reject_Click(object sender, EventArgs e)
    {
        actionbtn("Reject");
    }

    private void actionbtn(String btnname)
    {
        try
        {
            String mailprofile = "BSTWF";
            String sendermail = "";
            if (Request.QueryString["rowid"] != null)
            {
                if (Request.QueryString["rowid"].ToString() == "preview")
                {
                    return;
                }
            }
            else
            {
                return;
            }


            Boolean bb = true;
            string startflow = "dbo.getStartRowID()";
            string EventsHistoryID = "";

            DataSet dtx = SQLExecuteReader("system", "select NEWID() as startflow,NEWID() as History");
            startflow = "'" + dtx.Tables[0].Rows[0][0].ToString() + "'";
            EventsHistoryID = "'" + dtx.Tables[0].Rows[0][1].ToString() + "'";

            if (Request.QueryString["runingkey"] == null)
            {
                if (Request.QueryString["btnaction"] == null)
                {
                    globalStartFlowID = dtx.Tables[0].Rows[0][0].ToString();
                    getstart();
                }


                bb = SQLeExecuteNonQuery("system", "insert into WorkFlowRunningStatus (StartRowID,ParentStartRowID,WFRowId, CurrentStepRowId, ContentBody, CrBy, UdBy) values (" + startflow + "," + (Request.QueryString["parentstartrowid"] == null ? "null" : "'" + Request.QueryString["parentstartrowid"].ToString() + "'") + ",'" + Request.QueryString["rowid"].ToString() + "','" + currentstep_rowid.Value + "','" + content.Text.Replace("'", "''") + "','" + isWho() + "','" + isWho() + "');");
            }
            else
            {

                //############## check current referencekey 
                try
                {
                    if (Request.QueryString["referencekey"] != null && calltigerBtn == false)
                    {
                        DataSet vvs = SQLExecuteReader("system", "select top 1 rowid  from WorkFlowEventsHistory where startrowid='" + Request.QueryString["runingkey"].ToString() + "' order by crdate desc");
                        if (vvs.Tables[0].Rows.Count > 0)
                        {

                            if (vvs.Tables[0].Rows[0]["rowid"].ToString() != Request.QueryString["referencekey"].ToString())
                            {
                                Response.Write("<script>alert('Please process again!! ,The information may not be current.กรุณาทำรายการอีกครั้ง!! เนื่องจากข้อมูลหน้านี้ไม่เป็นปัจจุบันเนื่องจากมีคนอื่นปรับปรุงข้อมูลไปก่อนหน้านี้');location.href=\"WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + Request.QueryString["runingkey"].ToString() + "&referencekey=" + Request.QueryString["referencekey"].ToString() + "\";</script>");
                                // Response.Write("<script>alert('Please process again!! ,The information may not be current.กรุณาทำรายการอีกครั้ง!! เนื่องจากข้อมูลหน้านี้ไม่เป็นปัจจุบันเนื่องจากมีคนอื่นปรับปรุงข้อมูลไปก่อนหน้านี้');location.href=" + this.Request.Url.ToString() + ";</script>");
                                return;
                            }
                        }
                    }
                }
                catch { }
                //##############  end check current referencekey 


                getstep(Request.QueryString["runingkey"].ToString());
                startflow = "'" + Request.QueryString["runingkey"].ToString() + "'";
            }


            if (bb)
            {

                String SentEmailTo = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "MailToPosition"].ToString();
                String SentEmailToCC1 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "CC1MailToPosition"].ToString();
                String SentEmailToInform = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "InformMailToPosition"].ToString();
                String ExecuteCommand = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecCommand"].ToString().Replace("'", "''");
                String NextStep = (dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "GotoStep"].ToString() == "" ? "Finished" : dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "GotoStep"].ToString());

                String SentEmailToEmail = "";
                String SentEmailToCCEmail = "";
                String SentEmailToInformEmail = "";

                //##############  get dinamic send to email

                #region get dinamic send to email

                if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "ValueToselect"].ToString() != "" && SentEmailTo == "")
                {
                    String emailkey = dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "ValueToselect"].ToString();
                    foreach (String[] f in fieldinput)
                    {
                        if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "ValueToselect"].ToString().IndexOf("{" + f[0] + "}") >= 0)
                        {
                            emailkey = emailkey.Replace("{" + f[0] + "}", f[1]);
                        }
                    }

                    DataSet getmail = SQLExecuteReader("system", "select PositionRowId from Prosition where (SelectedKey in ('" + emailkey.Replace(",", "','").Replace(";", "','") + "') or SelectedKey like '" + emailkey.Replace("'", "") + "' ) and WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
                    if (getmail.Tables[0].Rows.Count > 0)
                    {
                        for (int x = 0; x < getmail.Tables[0].Rows.Count; x++)
                        {
                            SentEmailTo = (SentEmailTo == "" ? "" : SentEmailTo + ",") + getmail.Tables[0].Rows[x]["PositionRowId"].ToString();
                        }
                    }
                }

                if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "CC1ValueToselect"].ToString() != "" && SentEmailToCC1 == "")
                {
                    String emailkey = dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "CC1ValueToselect"].ToString();
                    foreach (String[] f in fieldinput)
                    {
                        if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "CC1ValueToselect"].ToString().IndexOf("{" + f[0] + "}") >= 0)
                        {
                            emailkey = emailkey.Replace("{" + f[0] + "}", f[1]);
                        }
                    }

                    DataSet getmail = SQLExecuteReader("system", "select  PositionRowId from Prosition where (SelectedKey in ('" + emailkey.Replace(",", "','").Replace(";", "','") + "') or  SelectedKey like '" + emailkey.Replace("'", "") + "' )  and WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
                    if (getmail.Tables[0].Rows.Count > 0)
                    {
                        for (int x = 0; x < getmail.Tables[0].Rows.Count; x++)
                        {
                            SentEmailToCC1 = (SentEmailToCC1 == "" ? "" : SentEmailToCC1 + ",") + getmail.Tables[0].Rows[x]["PositionRowId"].ToString();
                        }
                    }
                }

                if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "InformValueToselect"].ToString() != "" && SentEmailToInform == "")
                {
                    String emailkey = dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "InformValueToselect"].ToString();
                    foreach (String[] f in fieldinput)
                    {
                        if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "InformValueToselect"].ToString().IndexOf("{" + f[0] + "}") >= 0)
                        {
                            emailkey = emailkey.Replace("{" + f[0] + "}", f[1]);
                        }
                    }

                    DataSet getmail = SQLExecuteReader("system", "select  PositionRowId from Prosition where (SelectedKey in ('" + emailkey.Replace(",", "','").Replace(";", "','") + "') or  SelectedKey like '" + emailkey.Replace("'", "") + "' )  and WFRowId='" + Request.QueryString["rowid"].ToString() + "'");
                    if (getmail.Tables[0].Rows.Count > 0)
                    {
                        for (int x = 0; x < getmail.Tables[0].Rows.Count; x++)
                        {
                            SentEmailToInform = (SentEmailToInform == "" ? "" : SentEmailToInform + ",") + getmail.Tables[0].Rows[x]["PositionRowId"].ToString();
                        }
                    }
                }

                #endregion

                //###################


                #region  procA
                if (SQLeExecuteNonQuery("system", "insert into WorkFlowEventsHistory (RowID,startrowid,StepRowID, Event, ContentBody, SentEmailTo, ExecuteCommand, NextStep, CrBy, UdBy) values (" + EventsHistoryID + "," + startflow + ",'" + currentstep_rowid.Value + "','" + btnname + "','" + content.Text.Replace("'", "''") + "','" + SentEmailTo + "', '" + ExecuteCommand + "', '" + (NextStep == "Finished" ? "" : NextStep) + "','" + isWho() + "','" + isWho() + "');"))
                {
                    DataSet dds = SQLExecuteReader("system", "select PositionEmail from Prosition where PositionRowId in (" + (SentEmailTo == "" ? "null" : "'" + SentEmailTo.Replace(",", "','") + "'") + ") ;select top 1 " + startflow + " as  startrowid;select top 1 " + EventsHistoryID + " as  RowID;select WFName,SQLMailProfile,SenderDisplay from WorkFlow where WFRowId='" + Request.QueryString["rowid"].ToString() + "';select StepName from WorkFlowStep where StepRowId=" + (NextStep == "Finished" ? "null" : "'" + NextStep + "'") + ";select PositionEmail from Prosition where PositionRowId in (" + (SentEmailToCC1 == "" ? "null" : "'" + SentEmailToCC1.Replace(",", "','") + "'") + ") ;select PositionEmail from Prosition where PositionRowId in (" + (SentEmailToInform == "" ? "null" : "'" + SentEmailToInform.Replace(",", "','") + "'") + ");SELECT  TiggerStartStepCondition, TiggerStartStepConditionButton,StepRowId FROM  WorkFlowStep WHERE (WFRowId = '" + Request.QueryString["rowid"].ToString() + "') AND (StepAutoStart = 'condition') AND (ISNULL(TiggerStartStepCondition, N'') <> '')");
                    if (SentEmailTo != "" || SentEmailToInform != "")
                    {
                        String mailsubject = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "EmailSubject"].ToString();
                        String body = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "EmailContent"].ToString();
                        String bodyInform = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "InformEmailContent"].ToString();



                        #region Set value system variable
                        //########### set sys value


                        foreach (String[] key in fieldinput)
                        {
                            mailsubject = mailsubject.Replace("{" + key[0] + "}", key[1]);
                            body = body.Replace("{" + key[0] + "}", key[1]);
                            bodyInform = bodyInform.Replace("{" + key[0] + "}", key[1]);

                            mailsubject = mailsubject.Replace("{" + key[0] + "}", "");
                            body = body.Replace("{" + key[0] + "}", "");
                            bodyInform = bodyInform.Replace("{" + key[0] + "}", "");
                        }


                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            body = body.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            bodyInform = bodyInform.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_time}", DateTime.Now.ToString("HH:mm"));
                            body = body.Replace("{sys_time}", DateTime.Now.ToString("HH:mm"));
                            bodyInform = bodyInform.Replace("{sys_time}", DateTime.Now.ToString("HH:mm"));
                        }
                        catch { }


                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_username}", isWho());
                            body = body.Replace("{sys_username}", isWho());
                            bodyInform = bodyInform.Replace("{sys_username}", isWho());
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                            body = body.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                            bodyInform = bodyInform.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                            body = body.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                            bodyInform = bodyInform.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                            body = body.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                            bodyInform = bodyInform.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                            body = body.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                            bodyInform = bodyInform.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                            body = body.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                            bodyInform = bodyInform.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                        }
                        catch { }

                        try
                        {
                            mailsubject = mailsubject.Replace("{sys_nextstepname}", (dds.Tables[4].Rows.Count > 0 ? "Inform Step : " + dds.Tables[4].Rows[0]["StepName"].ToString() : "Inform Finished"));
                            body = body.Replace("{sys_nextstepname}", (dds.Tables[4].Rows.Count > 0 ? "Inform Step : " + dds.Tables[4].Rows[0]["StepName"].ToString() : "Inform Finished"));
                            bodyInform = bodyInform.Replace("{sys_nextstepname}", (dds.Tables[4].Rows.Count > 0 ? "Inform Step : " + dds.Tables[4].Rows[0]["StepName"].ToString() : "Inform Finished"));
                        }
                        catch { }



                        #endregion






                        if (dds.Tables[0].Rows.Count > 0 || dds.Tables[6].Rows.Count > 0)
                        {

                            for (int z = 0; z < dds.Tables[0].Rows.Count; z++)
                            {
                                SentEmailToEmail = (SentEmailToEmail == "" ? "" : SentEmailToEmail + ";") + dds.Tables[0].Rows[z]["PositionEmail"].ToString();
                            }

                            for (int z = 0; z < dds.Tables[5].Rows.Count; z++)
                            {
                                SentEmailToCCEmail = (SentEmailToCCEmail == "" ? "" : SentEmailToCCEmail + ";") + dds.Tables[5].Rows[z]["PositionEmail"].ToString();
                            }

                            for (int z = 0; z < dds.Tables[6].Rows.Count; z++)
                            {
                                SentEmailToInformEmail = (SentEmailToInformEmail == "" ? "" : SentEmailToInformEmail + ";") + dds.Tables[6].Rows[z]["PositionEmail"].ToString();
                            }




                            String url = this.Request.Url.ToString().ToLower().Substring(0, this.Request.Url.ToString().ToLower().IndexOf("/workflowruning.aspx"));
                            url = url.Replace("http://172.20.128.10:8080", "https://guru.bst.co.th:440");
                            if (Request.QueryString["runingkey"] == null)
                            {
                                body += "<br /><br />Go to system : <a href=" + url + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + dds.Tables[1].Rows[0][0].ToString() + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >Click here to open system</a><br /><br />";
                                if (ServerInternetUrl.ToLower() != ServerLocalUrl.ToLower())
                                {
                                    body += "For Internet/Mobile : <a href=" + ServerInternetUrl + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + dds.Tables[1].Rows[0][0].ToString() + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >(Open Internet)</a><br /><br />";
                                }
                                bodyInform += "<br /><br />Go to system : <a href=" + url + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + dds.Tables[1].Rows[0][0].ToString() + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >Click here to open system</a><br /><br />";
                            }
                            else
                            {
                                body += "<br /><br />Go to system : <a href=" + url + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + startflow.Replace("'", "") + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >Click here to open system</a><br /><br />";
                                if (ServerInternetUrl.ToLower() != ServerLocalUrl.ToLower())
                                {
                                    body += "For Internet/Mobile : <a href=" + ServerInternetUrl + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + startflow.Replace("'", "") + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >(Open Internet)</a><br /><br />";
                                }

                                bodyInform += "<br /><br />Go to system : <a href=" + url + "/WorkflowRuning.aspx?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + startflow.Replace("'", "") + "&referencekey=" + dds.Tables[2].Rows[0][0].ToString() + " >Click here to open system</a><br /><br />";
                            }

                            String nextsteppp = (dds.Tables[4].Rows.Count > 0 ? "Inform Step : " + dds.Tables[4].Rows[0]["StepName"].ToString() : "Inform Finished");


                            //############################## get mail profile
                            try
                            {
                                if (dds.Tables[3].Rows[0]["SQLMailProfile"].ToString() != "")
                                {
                                    DataSet dprofile = SQLExecuteReader("LegalMail", "select mailprofile from " + ServerNotificationDB + ".dbo.SystemAndMailProfile where system='" + dds.Tables[3].Rows[0]["SQLMailProfile"].ToString() + "'");
                                    if (dprofile.Tables[0].Rows.Count > 0)
                                    {
                                        mailprofile = dprofile.Tables[0].Rows[0][0].ToString();

                                    }
                                    mailprofile = (mailprofile == "" ? "BSTWF" : mailprofile);
                                }

                                if (dds.Tables[3].Rows[0]["SenderDisplay"].ToString().Replace(" ", "") != "")
                                {
                                    sendermail = dds.Tables[3].Rows[0]["SenderDisplay"].ToString();
                                }
                            }
                            catch { }

                            //######## end getmail profile


                            //########### detect value email with input object template
                            SentEmailToEmail = SentEmailToEmail.Replace("{sys_logonemail}", getemailbyuser(isWho()));
                            SentEmailToCCEmail = SentEmailToCCEmail.Replace("{sys_logonemail}", getemailbyuser(isWho()));
                            SentEmailToInformEmail = SentEmailToInformEmail.Replace("{sys_logonemail}", getemailbyuser(isWho()));

                            foreach (String[] key in fieldinput)
                            {
                                SentEmailToEmail = SentEmailToEmail.Replace("{" + key[0] + "}", key[1]);
                                SentEmailToCCEmail = SentEmailToCCEmail.Replace("{" + key[0] + "}", key[1]);
                                SentEmailToInformEmail = SentEmailToInformEmail.Replace("{" + key[0] + "}", key[1]);
                            }

                            SentEmailToEmail = SentEmailToEmail.Replace(" ", "").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";");
                            SentEmailToCCEmail = SentEmailToCCEmail.Replace(" ", "").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";");
                            SentEmailToInformEmail = SentEmailToInformEmail.Replace(" ", "").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";").Replace(";;", ";");

                            //### end


                            #region Set value system variable >> Recheck replace email again
                            try
                            {
                                mailsubject = mailsubject.Replace("{MailTo}", (SentEmailToEmail == "" ? "" : SentEmailToEmail));
                                body = body.Replace("{MailTo}", (SentEmailToEmail == "" ? "" : SentEmailToEmail));
                                bodyInform = bodyInform.Replace("{MailTo}", (SentEmailToEmail == "" ? "" : SentEmailToEmail));
                            }
                            catch { }

                            try
                            {
                                mailsubject = mailsubject.Replace("{CCMailTo}", (SentEmailToCCEmail == "" ? "" : SentEmailToCCEmail));
                                body = body.Replace("{CCMailTo}", (SentEmailToCCEmail == "" ? "" : SentEmailToCCEmail));
                                bodyInform = bodyInform.Replace("{CCMailTo}", (SentEmailToCCEmail == "" ? "" : SentEmailToCCEmail));
                            }
                            catch { }
                            #endregion


                            if (dataInfor.Tables[0].Rows[0]["Event" + btnname + "DonotSendMail"].ToString() != "Y" && mailsubject != "NULL" && mailsubject != "")
                            {
                                SendEmailWithProfile(mailprofile, SentEmailToEmail.Replace(" ", "").Replace(",", ";").Replace(System.Environment.NewLine, ""), SentEmailToCCEmail.Replace(" ", "").Replace(",", ";").Replace(System.Environment.NewLine, ""), mailsubject.Replace("'", "''"), body.Replace("\n", "<br />"), sendermail);
                            }


                            if (SentEmailToInformEmail != "")
                            {
                                SendEmailWithProfile(mailprofile, SentEmailToInformEmail.Replace(" ", "").Replace(",", ";").Replace(System.Environment.NewLine, "").Replace(System.Environment.NewLine, ""), "", mailsubject.Replace("'", "''"), bodyInform.Replace("\n", "<br />"), sendermail);

                            }



                            String historyEmail = "";

                            try
                            {
                                historyEmail = @"EXEC msdb.dbo.sp_send_dbmail  
                                                              @profile_name = N'" + mailprofile + @"',
                                                              @recipients = N'" + SentEmailToEmail.Replace(" ", "").Replace(",", ";").Replace(System.Environment.NewLine, "") + @"',
                                                              @copy_recipients = N'" + SentEmailToCCEmail.Replace(" ", "").Replace(",", ";").Replace(System.Environment.NewLine, "") + @"',
                                                              @body = N'" + body.Replace("'", "''").Replace("\n", "<br />").Replace("'", "''") + @"',
                                                              @subject = N'" + mailsubject.Replace("'", "''") + @"',
                                                              @importance = 'NORMAL',
                                                              @from_address=N'" + sendermail + @"',
                                                              @body_format='HTML'";
                            }
                            catch { }


                            SQLeExecuteNonQuery("system", "update WorkFlowEventsHistory set EmailCommandForResent='" + historyEmail.Replace("'", "||?") + "',email='" + SentEmailToEmail + "',CCEmail='" + SentEmailToCCEmail + "' where RowID='" + dds.Tables[2].Rows[0][0].ToString() + "';");
                        }
                    }


                    //old position  execSQLcommand


                    if (dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC1"].ToString() != "" || dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC2"].ToString() != "" || dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC3"].ToString() != "" || dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC4"].ToString() != "" || dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC5"].ToString() != "")
                    {
                        String etc1 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC1"].ToString();
                        String etc2 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC2"].ToString();
                        String etc3 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC3"].ToString();
                        String etc4 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC4"].ToString();
                        String etc5 = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "UpdateValueETC5"].ToString();


                        foreach (String[] key in fieldinput)
                        {

                            etc1 = etc1.ToString().Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                            etc2 = etc2.ToString().Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                            etc3 = etc3.ToString().Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                            etc4 = etc4.ToString().Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                            etc5 = etc5.ToString().Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                        }

                        SQLeExecuteNonQuery("system", @"update WorkFlowRunningStatus set ETC1=case when '" + etc1 + "'='' then ETC1 else '" + etc1 + @"' end,
                                                                                     ETC2=case when '" + etc2 + "'='' then ETC2 else '" + etc2 + @"' end,
                                                                                     ETC3=case when '" + etc3 + "'='' then ETC3 else '" + etc3 + @"' end,
                                                                                     ETC4=case when '" + etc4 + "'='' then ETC4 else '" + etc4 + @"' end,
                                                                                     ETC5=case when '" + etc5 + "'='' then ETC5 else '" + etc5 + @"' end where StartRowID=" + startflow);
                    }


                    #region dynamic to step
                    //##################### dinamic to step
                    string dd = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "GotoStep"].ToString();
                    if (dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "GotoStep"].ToString() != "")
                    {
                        SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set  Status='Active',CurrentStepRowId='" + dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "GotoStep"].ToString() + "',ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + (Request.QueryString["runingkey"] != null ? Request.QueryString["runingkey"].ToString() : dds.Tables[1].Rows[0][0].ToString()) + "'");
                    }
                    else
                    {
                        if (dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "ValueToStep"].ToString() != "")
                        {
                            String di_keystep = dataInfor.Tables[0].Rows[0]["DinamicFiled" + btnname + "ValueToStep"].ToString();
                            foreach (String[] key in fieldinput)
                            {
                                di_keystep = di_keystep.Replace("{" + key[0] + "}", key[1]);
                            }

                            DataSet ds = SQLExecuteReader("system", "select * from WorkFlowStep where DinamicKeyValue like '" + di_keystep + "' and WFRowId='" + Request.QueryString["rowid"].ToString() + "';");
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set Status='Active',CurrentStepRowId='" + ds.Tables[0].Rows[0]["StepRowId"].ToString() + "',ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + (Request.QueryString["runingkey"] != null ? Request.QueryString["runingkey"].ToString() : dds.Tables[1].Rows[0][0].ToString()) + "';update WorkFlowEventsHistory set NextStep='" + ds.Tables[0].Rows[0]["StepRowId"].ToString() + "' where RowID='" + dds.Tables[2].Rows[0][0].ToString() + "';");

                            }
                            else if (di_keystep == "**HOLD**")
                            {
                                SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set Status='Hold',ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + (Request.QueryString["runingkey"] != null ? Request.QueryString["runingkey"].ToString() : dds.Tables[1].Rows[0][0].ToString()) + "';update WorkFlowEventsHistory set NextStep='9999C689-4C15-445F-9CBB-B0788A0C920A' where RowID='" + dds.Tables[2].Rows[0][0].ToString() + "';");
                            }
                            else
                            {
                                SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set Status='Finished',ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + (Request.QueryString["runingkey"] != null ? Request.QueryString["runingkey"].ToString() : dds.Tables[1].Rows[0][0].ToString()) + "'");
                            }
                        }
                        else
                        {
                            SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set Status='Finished',ContentBody='" + content.Text.Replace("'", "''") + "' where startrowid='" + (Request.QueryString["runingkey"] != null ? Request.QueryString["runingkey"].ToString() : dds.Tables[1].Rows[0][0].ToString()) + "'");
                        }
                    }

                    //#########
                    #endregion



                    #region saveobject value

                    String SQLSaveValue = "delete from TemplateInputCurrentValue where StartRowID='" + (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()) + "';";
                    foreach (String[] key in fieldinput)
                    {
                        if (key[1] != "")
                        {
                            SQLSaveValue += "insert into TemplateInputCurrentValue (WFRowID,StartRowID,ObjectName,Value,CrBy,udby) values ('" + Request.QueryString["rowid"].ToString() + "','" + (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()) + "','" + key[0].Replace("'", "''") + "','" + key[1].Replace("'", "''") + "','" + isWho() + "','" + isWho() + "');";
                        }
                    }

                    SQLeExecuteNonQuery("system", SQLSaveValue);
                    #endregion


                    #region execSQLcommand
                    if (dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecAtServer"].ToString() != "" && dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecCommand"].ToString() != "")
                    {
                        String sql = dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecCommand"].ToString();
                        foreach (String[] key in fieldinput)
                        {
                            sql = sql.Replace("{" + key[0] + "}", key[1].Replace("'", "''"));
                        }

                        try { sql = sql.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm")); }
                        catch { }
                        try { sql = sql.Replace("{sys_username}", isWho()); }
                        catch { }
                        try { sql = sql.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString()); }
                        catch { }
                        try
                        {
                            sql = sql.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                        }
                        catch { }
                        try { sql = sql.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString())); }
                        catch { }

                        try { sql = sql.Replace("{MailTo}", (SentEmailToEmail == "" ? "chakrapan_a@bst.co.th" : SentEmailToEmail)); }
                        catch { }
                        try { sql = sql.Replace("{CCMailTo}", SentEmailToCCEmail); }
                        catch { }

                        if (dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecAtServer"].ToString().IndexOf("ORA_") == 0)
                        {
                            serviceoracle.Service1 sv = new serviceoracle.Service1();
                            String cc = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecAtServer"].ToString()].ToString();
                            int s = sv.ExecuteNonQuery(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecAtServer"].ToString()].ToString(), sql);
                            Response.Write(sv.GetCerrentError().ToString());
                        }
                        else
                        {
                            SQLeExecuteNonQuery(dataInfor.Tables[0].Rows[0]["EventAfter" + btnname + "ExecAtServer"].ToString(), sql);
                        }
                    }
                    #endregion

                    if (!calltigerBtn)
                    {
                        UploadFileNoDelete(Request.Files, (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                    }

                    #region function button trigger
                    //Update add function tigger
                    if (dataInfor.Tables[0].Rows[0]["CheckTigger" + btnname].ToString() == "Y")
                    {
                        String tiggerSave = dataInfor.Tables[0].Rows[0]["TriggerSave"].ToString().Trim();
                        String tiggerApprove = dataInfor.Tables[0].Rows[0]["TriggerApprove"].ToString().Trim();
                        String tiggerReject = dataInfor.Tables[0].Rows[0]["TriggerReject"].ToString().Trim();

                        #region Set value system variable
                        //########### set sys value

                        foreach (String[] key in fieldinput)
                        {
                            tiggerSave = tiggerSave.Replace("{" + key[0] + "}", key[1]);
                            tiggerApprove = tiggerApprove.Replace("{" + key[0] + "}", key[1]);
                            tiggerReject = tiggerReject.Replace("{" + key[0] + "}", key[1]);
                        }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            tiggerApprove = tiggerApprove.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            tiggerReject = tiggerReject.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_username}", isWho());
                            tiggerApprove = tiggerApprove.Replace("{sys_username}", isWho());
                            tiggerReject = tiggerReject.Replace("{sys_username}", isWho());
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                            tiggerApprove = tiggerApprove.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                            tiggerReject = tiggerReject.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                            tiggerApprove = tiggerApprove.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                            tiggerReject = tiggerReject.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                            tiggerApprove = tiggerApprove.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                            tiggerReject = tiggerReject.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{MailTo}", (SentEmailToEmail == "" ? "chakrapan_a@bst.co.th" : SentEmailToEmail));
                            tiggerApprove = tiggerApprove.Replace("{MailTo}", (SentEmailToEmail == "" ? "chakrapan_a@bst.co.th" : SentEmailToEmail));
                            tiggerReject = tiggerReject.Replace("{MailTo}", (SentEmailToEmail == "" ? "chakrapan_a@bst.co.th" : SentEmailToEmail));
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{CCMailTo}", SentEmailToCCEmail);
                            tiggerApprove = tiggerApprove.Replace("{CCMailTo}", SentEmailToCCEmail);
                            tiggerReject = tiggerReject.Replace("{CCMailTo}", SentEmailToCCEmail);
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                            tiggerApprove = tiggerApprove.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                            tiggerReject = tiggerReject.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());
                        }
                        catch { }

                        try
                        {
                            tiggerSave = tiggerSave.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                            tiggerApprove = tiggerApprove.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                            tiggerReject = tiggerReject.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                        }
                        catch { }

                        #endregion


                        if (dataInfor.Tables[0].Rows[0]["EnableSave"].ToString() == "Y" && tiggerSave != "")
                        {
                            try
                            {
                                DataSet check = SQLExecuteReader("system", " select case when   (" + tiggerSave + ") then 'Y' else 'N' end as rel");
                                if (check.Tables[0].Rows[0][0].ToString() == "Y")
                                {
                                    calltigerBtn = true;
                                    actionbtn("Save");
                                }
                            }
                            catch { lb_tiger.Text = "Tigger Error automatic press button Save!!!!!!"; }
                        }
                        if (dataInfor.Tables[0].Rows[0]["EnableApprove"].ToString() == "Y" && tiggerApprove != "")
                        {
                            try
                            {
                                DataSet check = SQLExecuteReader("system", " select case when   (" + tiggerApprove + ") then 'Y' else 'N' end as rel");
                                if (check.Tables[0].Rows[0][0].ToString() == "Y")
                                {
                                    calltigerBtn = true;
                                    actionbtn("Approve");
                                }
                            }
                            catch { lb_tiger.Text = "Tigger Error automatic press button Approve!!!!!!"; }

                        }
                        if (dataInfor.Tables[0].Rows[0]["EnableReject"].ToString() == "Y" && tiggerReject != "")
                        {
                            try
                            {
                                DataSet check = SQLExecuteReader("system", " select case when   (" + tiggerReject + ") then 'Y' else 'N' end as rel");
                                if (check.Tables[0].Rows[0][0].ToString() == "Y")
                                {
                                    calltigerBtn = true;
                                    actionbtn("Reject");
                                }
                            }
                            catch { lb_tiger.Text = "Tigger Error automatic press button Reject!!!!!!"; }

                        }


                    }



                    //endUpdate add function tigger
                    #endregion function button trigger

                    #region check condition open child flow
                    if (dataInfor.Tables[0].Rows[0]["CheckConditionAutoStart" + btnname].ToString() == "Y")
                    {
                        if (dds.Tables[7].Rows.Count > 0)
                        {
                            for (int i = 0; i < dds.Tables[7].Rows.Count; i++)
                            {
                                try
                                {
                                    #region Set value system variable
                                    //########### set sys value
                                    String callflowcondition = dds.Tables[7].Rows[i]["TiggerStartStepCondition"].ToString();
                                    foreach (String[] key in fieldinput)
                                    {
                                        callflowcondition = callflowcondition.Replace("{" + key[0] + "}", key[1]);
                                    }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_date}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_username}", isWho());

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_workflowid}", Request.QueryString["rowid"].ToString());

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_startworkflowid}", (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()));

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_referenceeventhistory}", (Request.QueryString["runingkey"] == null ? dds.Tables[2].Rows[0][0].ToString() : Request.QueryString["referencekey"].ToString()));

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{MailTo}", (SentEmailToEmail == "" ? "chakrapan_a@bst.co.th" : SentEmailToEmail));

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{CCMailTo}", SentEmailToCCEmail);

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_workflowname}", dataInfor.Tables[0].Rows[0]["WFName"].ToString());

                                    }
                                    catch { }

                                    try
                                    {
                                        callflowcondition = callflowcondition.Replace("{sys_currentstepname}", dataInfor.Tables[0].Rows[0]["StepName"].ToString());
                                    }
                                    catch { }

                                    #endregion

                                    DataSet check = SQLExecuteReader("system", " select case when   (" + callflowcondition + ") then 'Y' else 'N' end as rel");
                                    if (check.Tables[0].Rows[0][0].ToString() == "Y")
                                    {
                                        autostartbycondition(startflow.Replace("'", ""), dds.Tables[7].Rows[i]["TiggerStartStepConditionButton"].ToString(), dds.Tables[7].Rows[i]["StepRowId"].ToString());

                                    }
                                }
                                catch (Exception ex) { lb_tiger.Text = "Auto run child WF Error!!!!!!" + ex.Message; }
                            }
                        }
                    }
                    else
                    {



                        #region check auto Continue MainWFbycondition (resume)
                        checkAutoContinueMainWFbycondition((dataInfor.Tables[0].Rows[0]["ParentStartRowID"].ToString() == "" ? (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()) : dataInfor.Tables[0].Rows[0]["ParentStartRowID"].ToString()));
                        #endregion
                    }
                    #endregion


                    MultiView1.ActiveViewIndex = 1;
                    hp1.Focus();

                    if (Request.QueryString["runingkey"] == null)
                    {
                        Button1.PostBackUrl = this.Page.Request.Url.ToString() + "&runingkey=" + (Request.QueryString["runingkey"] == null ? dds.Tables[1].Rows[0][0].ToString() : Request.QueryString["runingkey"].ToString()) + "&referencekey=new";
                    }

                }
                #endregion  procA

            }

            hp1.Focus();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect(this.Page.Request.Url.ToString());
    }


 

    #region UploadFileNoDelete
    public String UploadFileNoDelete(HttpFileCollection files, String ID)
    {
        String b = "";
        try
        {

            byte[] fileData;
            foreach (string fileTagName in files)
            {
                HttpPostedFile file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    int size = file.ContentLength;
                    string name = file.FileName;
                    int position = name.LastIndexOf("\\");
                    name = name.Substring(position + 1);
                    string contentType = file.ContentType;
                    fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);

                    using (SqlConnection connection = new SqlConnection())
                    {
                        connection.ConnectionString = WebConfigurationManager.ConnectionStrings["system"].ToString();
                        connection.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = connection;
                        cmd.CommandTimeout = 0;

                        string commandText = "INSERT INTO Attechment (Ref_Obj, Type, Ref_ID, Name, Attechment, Content_type, Size,CreateBy,UpdateBy) VALUES('" + fileTagName + "',@Type,'" + ID.ToString() + "', @Name, @Attechment, @Content_type, @Size,'" + isWho() + "','" + isWho() + "');";
                        cmd.CommandText = commandText;
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 300);
                        cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 100);
                        cmd.Parameters.Add("@Content_type", SqlDbType.VarChar, 100);
                        cmd.Parameters.Add("@Size", SqlDbType.Int);
                        cmd.Parameters.Add("@Attechment", SqlDbType.VarBinary);

                        cmd.Parameters["@Name"].Value = name;
                        cmd.Parameters["@Type"].Value = contentType;
                        cmd.Parameters["@Content_type"].Value = contentType;
                        cmd.Parameters["@size"].Value = size;
                        cmd.Parameters["@Attechment"].Value = fileData;
                        cmd.ExecuteNonQuery();

                        connection.Close();
                    }


                }
            }
            b = "";
        }
        catch (Exception e)
        {
            b = e.Message;
        }
        return b;
    }
    #endregion

    #region upload file
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
  
        filemgr.Visible = !filemgr.Visible;

        refreshfile();
        GridView1.Columns[0].HeaderStyle.CssClass = "non";
        GridView1.Columns[0].ItemStyle.CssClass = "non";
    }

    private void refreshfile()
    {
        DataSet dfile = this.SQLExecuteReader("system", "select * from Attechment where Ref_ID='" + Request.QueryString["runingkey"].ToString() + "'");
        GridView1.DataSource = dfile.Tables[0];
        GridView1.DataBind();

    }

    private void refreshEtc()
    {

        DataSet detc = this.SQLExecuteReader("system", @"SELECT     WorkFlowRunningStatus.ETC1, WorkFlowRunningStatus.ETC2, WorkFlowRunningStatus.ETC3, WorkFlowRunningStatus.ETC4, WorkFlowRunningStatus.ETC5, WorkFlowRunningStatus.ETC6, 
                                                                    WorkFlowRunningStatus.ETC7, WorkFlowRunningStatus.ETC8, WorkFlowRunningStatus.ETC9, WorkFlowRunningStatus.ETC10, WorkFlow.HisDisplayConfigETC1, WorkFlow.HisDisplayConfigETC2, 
                                                                    WorkFlow.HisDisplayConfigETC3, WorkFlow.HisDisplayConfigETC4, WorkFlow.HisDisplayConfigETC5, WorkFlow.HisDisplayConfigETC6, WorkFlow.HisDisplayConfigETC7, WorkFlow.HisDisplayConfigETC8, 
                                                                    WorkFlow.HisDisplayConfigETC9, WorkFlow.HisDisplayConfigETC10
                                                        FROM        WorkFlowRunningStatus INNER JOIN
                                                                    WorkFlow ON WorkFlowRunningStatus.WFRowId = WorkFlow.WFRowId WHERE (WorkFlowRunningStatus.StartRowID ='" + Request.QueryString["runingkey"].ToString() + "')");
        if (detc.Tables[0].Rows.Count > 0)
        {
            lb_etc1.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC1"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC1"].ToString().IndexOf(";"));
            lb_etc2.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC2"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC2"].ToString().IndexOf(";"));
            lb_etc3.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC3"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC3"].ToString().IndexOf(";"));
            lb_etc4.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC4"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC4"].ToString().IndexOf(";"));
            lb_etc5.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC5"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC5"].ToString().IndexOf(";"));
            lb_etc6.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC6"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC6"].ToString().IndexOf(";"));
            lb_etc7.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC7"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC7"].ToString().IndexOf(";"));
            lb_etc8.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC8"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC8"].ToString().IndexOf(";"));
            lb_etc9.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC9"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC9"].ToString().IndexOf(";"));
            lb_etc10.Text = detc.Tables[0].Rows[0]["HisDisplayConfigETC10"].ToString().Substring(0, detc.Tables[0].Rows[0]["HisDisplayConfigETC10"].ToString().IndexOf(";"));

            lb_etc1.Text = (lb_etc1.Text == "" ? "ETC1" : lb_etc1.Text);
            lb_etc2.Text =  (lb_etc2.Text == "" ? "ETC2" : lb_etc2.Text);
            lb_etc3.Text = (lb_etc3.Text == "" ? "ETC3" : lb_etc3.Text);
            lb_etc4.Text = (lb_etc4.Text == "" ? "ETC4" : lb_etc4.Text);
            lb_etc5.Text = (lb_etc5.Text == "" ? "ETC5" : lb_etc5.Text);
            lb_etc6.Text =  (lb_etc6.Text == "" ? "ETC6" : lb_etc6.Text);
            lb_etc7.Text = (lb_etc7.Text == "" ? "ETC7" : lb_etc7.Text);
            lb_etc8.Text = (lb_etc8.Text == "" ? "ETC8" : lb_etc8.Text);
            lb_etc9.Text =  (lb_etc9.Text == "" ? "ETC9" : lb_etc9.Text);
            lb_etc10.Text = (lb_etc10.Text == "" ? "ETC10" : lb_etc10.Text);

            txt_etc1.Text = detc.Tables[0].Rows[0]["ETC1"].ToString();
            txt_etc2.Text = detc.Tables[0].Rows[0]["ETC2"].ToString();
            txt_etc3.Text = detc.Tables[0].Rows[0]["ETC3"].ToString();
            txt_etc4.Text = detc.Tables[0].Rows[0]["ETC4"].ToString();
            txt_etc5.Text = detc.Tables[0].Rows[0]["ETC5"].ToString();
            txt_etc6.Text = detc.Tables[0].Rows[0]["ETC6"].ToString();
            txt_etc7.Text = detc.Tables[0].Rows[0]["ETC7"].ToString();
            txt_etc8.Text = detc.Tables[0].Rows[0]["ETC8"].ToString();
            txt_etc9.Text = detc.Tables[0].Rows[0]["ETC9"].ToString();
            txt_etc10.Text = detc.Tables[0].Rows[0]["ETC10"].ToString();
        }
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["runingkey"].ToString() != "")
        {
            UploadFileNoDelete(Request.Files, Request.QueryString["runingkey"].ToString());
            refreshfile();
        }
        Button3.Focus();
        getstep(Request.QueryString["runingkey"].ToString());
    }

    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        GridViewRow Row = (GridViewRow)((Control)sender).Parent.Parent;
        if (GridView1.Rows[Row.RowIndex].Cells[0].Text != "")
        {
         
                this.SQLeExecuteNonQuery("system", "delete from Attechment where id='" + GridView1.Rows[Row.RowIndex].Cells[0].Text + "'");
                refreshfile();
                String st1 = @"<script language='javascript' type='text/javascript'>
                                                                                    try
                                    {   
                                                                                Sys.Application.add_load(func);
                                                                                function func() {

                                                                                Sys.Application.remove_load(func);

                                                                                if(confirm('For completely updating system need to refresh. Refresh now? '))
                                                                                {
                                                                                     location.reload();   
                                                                                }
                                                                                }
                                    }
                                    catch(e)
                                    {}
                                            </script>";
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "error", st1.ToString(), false);

        }
    }

   protected void ln_switchuser_Click(object sender, EventArgs e)
    {
        Session["lastpage"]=this.Request.Url;

        Response.Redirect("login.aspx?logout=y&clear=switch");
    }

    #endregion



   private void UpdateEtcField()
   {
       SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set ETC1='" + txt_etc1.Text.Replace("'", "''") + "',ETC2='" + txt_etc2.Text.Replace("'", "''") + "',ETC3='" + txt_etc3.Text.Replace("'", "''") + "',ETC4='" + txt_etc4.Text.Replace("'", "''") + "',ETC5='" + txt_etc5.Text.Replace("'", "''") + "',ETC6='" + txt_etc6.Text.Replace("'", "''") + "',ETC7='" + txt_etc7.Text.Replace("'", "''") + "',ETC8='" + txt_etc8.Text.Replace("'", "''") + "',ETC9='" + txt_etc9.Text.Replace("'", "''") + "',ETC10='" + txt_etc10.Text.Replace("'", "''") + "' where StartRowID='" + Request.QueryString["runingkey"].ToString() + "'");
   }


   private string autostart()
   {
       try
       {

           if (Request.QueryString["resumestartrowid"] != null && Request.QueryString["startwithstep"] != null)
           {
               SQLeExecuteNonQuery("system", "update WorkFlowRunningStatus set CurrentStepRowId='" + Request.QueryString["startwithstep"].ToString() + "' where startrowid='" + Request.QueryString["resumestartrowid"].ToString() + "'");
               getstep(Request.QueryString["resumestartrowid"].ToString());
           }
           else
           {
               getstart();
           }

           HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
           doc.LoadHtml(content.Text);
           
// get method
           foreach (String key in Request.QueryString.AllKeys)
           {

               if (doc.GetElementbyId(key.ToString()) != null)
               {
                   switch (doc.GetElementbyId(key).Name.ToLower())
                   {
                       case "input":

                           if (doc.GetElementbyId(key).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                           {
                               if (Request.QueryString[key].ToString().ToLower() == "checked")
                               {
                                   doc.GetElementbyId(key).SetAttributeValue("checked", "checked");
                                   fieldinput.Add(new String[] { key, "OK" });
                               }
                           }
                           else
                           {
                               doc.GetElementbyId(key).SetAttributeValue("value", Request.QueryString[key].ToString());

                               for (int j = 0; j < fieldinput.Count;j++ )
                               {
                                   if ((((String[])fieldinput[j])[0]).ToString() == key)
                                   {
                                       fieldinput[j] = new String[] { key, Request.QueryString[key].ToString() };
                                   }
                                   
                               }
                           }
                           break;
                       case "textarea":
                           if (Request.QueryString[key].ToString() != "")
                           {
                               doc.GetElementbyId(key).RemoveAllChildren();
                               doc.GetElementbyId(key).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.QueryString[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine)));
                               
                               for (int j = 0; j < fieldinput.Count; j++)
                               {
                                   if ((((String[])fieldinput[j])[0]).ToString() == key)
                                   {
                                       fieldinput[j] = new String[] { key, Request.QueryString[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine) };
                                   }

                               }

                           }

                           break;
                       case "select":
                           if (Request.QueryString[key].ToString() != "")
                           {
                               for (int x = 0; x < doc.GetElementbyId(key).ChildNodes.Count; x++)
                               {
                                   if (doc.GetElementbyId(key).ChildNodes[x].GetAttributeValue("value", "") == Request.QueryString[key].ToString())
                                   {
                                       doc.GetElementbyId(key).ChildNodes[x].SetAttributeValue("selected", "selected");
                                       fieldinput.Add(new String[] { key, Request.QueryString[key].ToString() });
                                   }
                               }
                           }
                           break;
                   }

               }
           }
// end get method

    // post method
           foreach (String key in Request.Form.AllKeys)
           {

               if (doc.GetElementbyId(key.ToString()) != null)
               {
                   switch (doc.GetElementbyId(key).Name.ToLower())
                   {
                       case "input":

                           if (doc.GetElementbyId(key).Attributes["type"].Value.ToString().ToLower() == "checkbox")
                           {
                               if (Request.Form[key].ToString().ToLower() == "checked")
                               {
                                   doc.GetElementbyId(key).SetAttributeValue("checked", "checked");
                                   fieldinput.Add(new String[] { key, "OK" });
                               }
                           }
                           else
                           {
                               doc.GetElementbyId(key).SetAttributeValue("value", Request.Form[key].ToString());

                               for (int j = 0; j < fieldinput.Count; j++)
                               {
                                   if ((((String[])fieldinput[j])[0]).ToString() == key)
                                   {
                                       fieldinput[j] = new String[] { key, Request.Form[key].ToString() };
                                   }

                               }
                           }
                           break;
                       case "textarea":
                           if (Request.Form[key].ToString() != "")
                           {
                               doc.GetElementbyId(key).RemoveAllChildren();
                               doc.GetElementbyId(key).AppendChild(HtmlAgilityPack.HtmlNode.CreateNode(Request.Form[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine)));

                               for (int j = 0; j < fieldinput.Count; j++)
                               {
                                   if ((((String[])fieldinput[j])[0]).ToString() == key)
                                   {
                                       fieldinput[j] = new String[] { key, Request.Form[key].ToString().Replace("\\r\\n", Environment.NewLine).Replace("\\n", Environment.NewLine) };
                                   }

                               }

                           }

                           break;
                       case "select":
                           if (Request.Form[key].ToString() != "")
                           {
                               for (int x = 0; x < doc.GetElementbyId(key).ChildNodes.Count; x++)
                               {
                                   if (doc.GetElementbyId(key).ChildNodes[x].GetAttributeValue("value", "") == Request.Form[key].ToString())
                                   {
                                       doc.GetElementbyId(key).ChildNodes[x].SetAttributeValue("selected", "selected");
                                       fieldinput.Add(new String[] { key, Request.Form[key].ToString() });
                                   }
                               }
                           }
                           break;
                   }

               }
           }

           //end post method

           content.Text = doc.DocumentNode.OuterHtml.ToString();

           if (Request.QueryString["resumestartrowid"] != null)
           {
               btn_save.Visible = true;
               btn_approve.Visible = true;
               btn_reject.Visible = true;
           }


           if (Request.QueryString["btnaction"].ToString().ToLower() == "save" && btn_save.Visible)
           {

               actionbtn("Save");
               Session["loginname"] = null;
               Session["loginfullname"] = null;
               return "Started";
           }
           if (Request.QueryString["btnaction"].ToString().ToLower() == "approve" && btn_approve.Visible)
           {

               actionbtn("Approve");
               Session["loginname"] = null;
               Session["loginfullname"] = null;
               return "Started";
           }
           if (Request.QueryString["btnaction"].ToString().ToLower() == "reject"&& btn_reject.Visible)
           {

               actionbtn("Reject");
               Session["loginname"] = null;
               Session["loginfullname"] = null;
               return "Started";
           }
       }
       catch(Exception ee)
       {
           Session["loginname"] = null;
           Session["loginfullname"] = null;
       return ee.Message;
       }

       Session["loginname"] = null;
       Session["loginfullname"] = null;
       return "fail";
   }


   private void autostartbycondition(string parentStartRowId,String TiggerStartStepConditionButton,String strartStepRowId)
   {

       if (Request.QueryString["rowid"].ToString() != "")
       {
           string ddd = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("?")) + "?rowid=" + Request.QueryString["rowid"].ToString() + "&btnaction=" + TiggerStartStepConditionButton + "&parentstartrowid=" + parentStartRowId + "&startwithstep=" + strartStepRowId;
          // lb_tiger.Text = "Auto run child WF Error : " + ddd;
          // return;
           ddd = ddd.Replace("https://guru.bst.co.th:440/", "http://172.20.128.10:8080/");
           HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ddd);
           HttpWebResponse response = (HttpWebResponse)request.GetResponse();
           System.IO.Stream st = response.GetResponseStream();
           System.IO.StreamReader str = new System.IO.StreamReader(st);
           String outp =  str.ReadToEnd();
           if (outp != "Started")
           {
               lb_tiger.Text = "Auto run child WF Error : " + outp + " >>>> " + ddd;
           }
       }

   }
   private void resumemainWFByStep(string resumeStartRowId, String TiggerStartStepConditionButton, String strartStepRowId)
   {

       if (Request.QueryString["rowid"].ToString() != "")
       {
           string ddd = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("?")) + "?rowid=" + Request.QueryString["rowid"].ToString() + "&runingkey=" + resumeStartRowId + "&btnaction=" + TiggerStartStepConditionButton + "&resumestartrowid=" + resumeStartRowId + "&startwithstep=" + strartStepRowId;
            //ddd = ddd.Replace("https://guru.bst.co.th:440/", "http://172.20.128.10:8080/");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ddd);
           HttpWebResponse response = (HttpWebResponse)request.GetResponse();
           System.IO.Stream st = response.GetResponseStream();
           System.IO.StreamReader str = new System.IO.StreamReader(st);
           String outp = str.ReadToEnd();
           if (outp != "Started")
           {
               lb_tiger.Text = "AutoStart Error : " + outp + " >>>> " + ddd;
           }
       }

   }

   private void checkAutoContinueMainWFbycondition(string mainStartRowId)
   {
        DataSet dddds = SQLExecuteReader("system", @"SELECT        StartRowID, CASE WHEN ParentStartRowID IS NULL THEN 'Main' ELSE 'Child' END AS type, Status,
                                                                                 (SELECT        TOP (1) CONVERT(nvarchar(50), StepRowId) + ';' + CONVERT(nvarchar, TiggerStartStepConditionButton)
                                                                                   FROM            WorkFlowStep
                                                                                   WHERE        (WFRowId = WorkFlowRunningStatus.WFRowId) AND (StepAutoStart = 'continue')) AS ResumeContinue
                                                                    FROM            WorkFlowRunningStatus
                                                                     WHERE        (StartRowID = '" + mainStartRowId + @"') OR
                                                                                                         (ParentStartRowID = '" + mainStartRowId + "')");
        if (dddds.Tables[0].Rows.Count > 0)
        {
            // finish main
           dddds.Tables[0].DefaultView.RowFilter = "type='main' and status='hold'";
           if (dddds.Tables[0].DefaultView.ToTable().Rows.Count > 0)
           {
               dddds.Tables[0].DefaultView.RowFilter = "ResumeContinue is not null";
               if (dddds.Tables[0].DefaultView.ToTable().Rows.Count > 0)
               {
                   dddds.Tables[0].DefaultView.RowFilter = "type='child' and status='active'";
                   if (dddds.Tables[0].DefaultView.ToTable().Rows.Count == 0)
                   {
                       dddds.Tables[0].DefaultView.RowFilter="";
                       string[] st = dddds.Tables[0].Rows[0]["ResumeContinue"].ToString().Split((";").ToCharArray());
                       resumemainWFByStep(mainStartRowId,st[1],st[0]);
                       //continue main
                   }
                  
               }
               else
               {
                   if (SQLeExecuteNonQuery("system", "insert into WorkFlowEventsHistory (startrowid,StepRowID, Event, ContentBody, SentEmailTo, ExecuteCommand, NextStep, CrBy, UdBy) values ('" + mainStartRowId + "','" + currentstep_rowid.Value + "','Force','" + content.Text.Replace("'", "''") + "',null,null, '','system','system');update WorkFlowRunningStatus set status='Finished' where StartRowID='" + mainStartRowId + "'"))
                   {
                       lb_tiger.Text = "System force finished because no have resuming step!!!";
                   }
            
                   //finished main because no have step resume
               }
           }
           
        }
   }

   protected void Button4_Click(object sender, EventArgs e)
   {
       for (int i = 0; i < GridView1.Rows.Count; i++)
       {
           CheckBox cb = (CheckBox)GridView1.Rows[i].FindControl("CheckBox1");
           if (cb.Checked)
           {
               this.SQLeExecuteNonQuery("system", "delete from Attechment where id='" + GridView1.Rows[i].Cells[0].Text + "'");
           }
       }

       refreshfile();
   
       String st1 = @"<script language='javascript' type='text/javascript'>
                                                                                    try
                                    {   
                                                                                Sys.Application.add_load(func);
                                                                                function func() {

                                                                                Sys.Application.remove_load(func);

                                                                                if(confirm('For completely updating system need to refresh. Refresh now? '))
                                                                                {
                                                                                     location.reload();   
                                                                                }
                                                                                }
                                    }
                                    catch(e)
                                    {}
                                            </script>";
       ScriptManager.RegisterClientScriptBlock(this, GetType(), "error", st1.ToString(), false);

   }


   private void getChildFlow()
   {
       try
       {

           if (Session["loginname"] == "SYSTEM")
           {
               Session["loginname"] = null;
               String who = isWho();
           }
       }
       catch { }

       DataSet ds = SQLExecuteReader("system", @"SELECT DISTINCT WorkFlowRunningStatus.StartRowID, WorkFlowStep.GroupName+(case when WorkFlowRunningStatus.Status='finished' then ' <b>(Finished)</b>' else '<b>(Active)</b> ' end) as GroupName, WorkFlowRunningStatus.Status
                                                FROM            WorkFlowRunningStatus INNER JOIN
                                                                         WorkFlowEventsHistory ON WorkFlowRunningStatus.StartRowID = WorkFlowEventsHistory.StartRowID INNER JOIN
                                                                         WorkFlowStep ON WorkFlowEventsHistory.StepRowID = WorkFlowStep.StepRowId
                                                WHERE        (WorkFlowRunningStatus.ParentStartRowID = '" + Request.QueryString["runingkey"].ToString() + "') or (WorkFlowRunningStatus.StartRowID = (select ParentStartRowID from WorkFlowRunningStatus where StartRowID='" + Request.QueryString["runingkey"].ToString() + "'));");
       for (int i =0 ; i<ds.Tables[0].Rows.Count;i++)
       {
           HyperLink hp = new HyperLink();
           hp.Text = ds.Tables[0].Rows[i]["GroupName"].ToString();
           hp.Font.Bold = true;
           hp.Font.Size = 10;
          // hp.Style.Value = "width: 100%; height: 30px; bottom: 5px; padding-top: 10px; position: fixed; background-color: lightsteelblue;";
           //hp.Target = "_blank";
           hp.NavigateUrl = "WorkflowRuning.aspx?rowid=force&runingkey=" + ds.Tables[0].Rows[i]["StartRowID"].ToString() + "&referencekey=view";
           hp.CssClass = "flashit";
           childflow.Controls.Add(hp);
           Label lb = new Label();
           lb.Text = "&nbsp;&nbsp;|&nbsp;&nbsp;";
           if (ds.Tables[0].Rows[i]["Status"].ToString() == "Finished")
           {
               hp.ForeColor = System.Drawing.Color.Green;
               hp.Font.Bold = true;
           }
           else
           {
               hp.ForeColor = System.Drawing.Color.Blue;
           }
           childflow.Controls.Add(lb);
           childflow.Visible = true;

       }


       ClientScript.RegisterStartupScript(this.GetType(), "www", @"      $(function () {
          var p = $(""#childflow"");
          for (var i = 0; i < 10; i++) {
              p.animate({ backgroundColor: ""#ffffff"" }, 500, 'linear')
     .animate({ backgroundColor: ""#ccccc"" }, 500, 'linear');
          }
      });", true);

   }

  private void lineNotify(string msg)
   {
       string token = "wXs2wGhRHykleo64aoqC4lQSrecLhzl8Zcejb7TR3mA";
       try
       {
           var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify");
           WebProxy myProxy = new WebProxy();
           Uri newUri = new Uri("http://eagle.bst.co.th");
           myProxy.Address = newUri;
           myProxy.Credentials = new NetworkCredential("is_trainee", "Welc0me");
           request.Proxy = myProxy;
           var postData = string.Format("message={0}", msg);
           var data = System.Text.Encoding.UTF8.GetBytes(postData);
           request.Method = "POST";
           request.ContentType = "application/x-www-form-urlencoded";
           request.ContentLength = data.Length;
           request.Headers.Add("Authorization", "Bearer " + token);

           using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
           var response = (HttpWebResponse)request.GetResponse();
           var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
       }
       catch (Exception ex)
       {
           Console.WriteLine(ex.ToString());
       }
   }

}