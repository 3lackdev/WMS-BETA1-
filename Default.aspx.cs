using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            try
            {

                Literal l = (Literal)this.Master.FindControl("mainnavigate");
                if (l != null)
                {
                    if(Session["username"]!=null)
                    {
                        if (Session["username"].ToString() != "guest")
                        {
                            l.Text = "<a href=\"#\">หน้าแรกโปรแกรม</a> ";
                            return;
                        }
                    }

                    l.Text = "<a href=\"default.aspx\">หน้าแรกโปรแกรม</a> ";
                    MasterPage pp = (MasterPage)this.Master;
                   
                }
            }
            catch { }
         
        }
    }
}