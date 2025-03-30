using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;

  
public partial class MasterPage : System.Web.UI.MasterPage
    {
    protected void Page_Load(object sender, EventArgs e)
    {


        if (Session["loginname"] != null)
        {
            if (Session["loginname"].ToString() == "guest")
            {
               mainnavigate.Text="<a href=\"#\">หน้าแรกโปรแกรม</a> ";

            }
        }

        if (!IsPostBack)
        {

            if (Request.QueryString["logout"] != null)
            {
                Session["loginname"] = null;
                Session["loginfullname"] = null;

                Label1.Visible = false;
            }
            else if (Session["loginname"] == null)
            {
                if (Request.QueryString["logout"] != null)
                {
                    Response.Redirect("login.aspx?logout=y");
                }
                else
                {
                    if (Request.QueryString["username"] != null)
                    {
                        Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP//", "");
                        Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("BST_CORP//", "");
                    }

                    Session["lastpage"] = this.Page.Request.Url;
                    Response.Redirect("login.aspx");
                }
            }
            else if (Session["loginname"] != null)
            {
                 if (Request.QueryString["username"] != null)
                    {
                        Session["loginname"] = Request.QueryString["username"].ToString().Replace("BST_CORP//", "");
                        Session["loginfullname"] = Request.QueryString["username"].ToString().Replace("BST_CORP//", "");
                    }

                 if (Session["loginname"].ToString().ToUpper() == "IUSR" || Request.Cookies["Password"] == null)
                {
                    Session["lastpage"] = this.Page.Request.Url;
                   Response.Redirect("login.aspx?logout=y");
                }
                Label1.Text = "<l>เข้าระบบโดย : " + Session["loginfullname"].ToString() + "</l> &nbsp&nbsp(<a href='login.aspx?logout=y&clear=true'>Logout</a>)";
            }
        }

    }
        

       


        #region navigate
        public void generatenav(String navigate, String navigatelink, int level)
        {

            ArrayList path = new ArrayList();
            Boolean b = false;
            if (Session["path"] != null)
            {
                path = (ArrayList)Session["path"];
            }

            for (int i = 0; i < path.Count; i++)
            {
                String[] s = (String[])path[i];
                if (s[0].ToString() == navigate)
                {
                    path.RemoveRange(i + 1, path.Count - (i + 1));
                    b = true;
                }
                if (int.Parse(s[2]) == level && b == false)
                {
                    path.RemoveRange(i, path.Count - i);
                }
            }

            if (!b)
            {
                path.Add(new String[] { navigate, navigatelink, level.ToString() });
                Session["path"] = path;
            }

            Literal nav = this.mainnavigate;
            nav.Text = "";
            for (int i = 0; i < path.Count; i++)
            {
                String[] ss = (String[])path[i];
                if (i + 1 == path.Count)
                {
                    nav.Text += @"<div class=""breadcrumb_divider""></div> 
                              <a class=""current"" >" + ss[0] + @"</a>";
                }
                else
                {
                    nav.Text += @"<div class=""breadcrumb_divider""></div> 
                              <a href=""" + ss[1] + @""">" + ss[0] + @"</a>";

                }


            }

        }


        private String getnavname()
        {
            string inarr = "";
            ArrayList path = new ArrayList();
            if (Session["path"] != null)
            {
                path = (ArrayList)Session["path"];
            }

            for (int i = 0; i < path.Count; i++)
            {
                inarr = (inarr != "" ? inarr + "," : inarr);
                inarr += "'" + ((String[])path[i])[0].ToString() + "'";
            }


            return inarr;
        }
        #endregion

      


    }

