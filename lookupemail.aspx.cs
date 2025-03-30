using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.Data;
using System.Data.SqlClient;

public partial class lookupemail : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        DirectoryEntry entry;
          if(ServerDomain.ToUpper() == "JBE.CO.TH")
          {
                entry = new DirectoryEntry("LDAP://jbe.co.th", "jbeadmin", "lc@mpw11");
          }
          else
          {
                entry = new DirectoryEntry("LDAP://bst.co.th", "service.workflow", "Welc0me");
          }

        DirectorySearcher deSearch = new DirectorySearcher(entry);
        deSearch.PropertiesToLoad.Add("sn");
        deSearch.PropertiesToLoad.Add("cn");
        deSearch.PropertiesToLoad.Add("givenName");
        deSearch.PropertiesToLoad.Add("sAMAccountName");
        deSearch.PropertiesToLoad.Add("mail");
        deSearch.Filter = string.Format("(&(objectCategory=user)(givenName=*{0}*))", (txt.Text == "" ? "xxxx" : txt.Text));


        SearchResultCollection sResultColl = deSearch.FindAll();


        DataTable table = new DataTable();
        table.Clear();
        table.Columns.Add("Full Name", typeof(string));

        table.Columns.Add("OU", typeof(string));
        table.Columns.Add("User Name", typeof(string));
        table.Columns.Add("Email", typeof(string));

        foreach (SearchResult result in sResultColl)
        {
            String[] sp = result.Path.ToString().Split(',');
            String[] sp2 = sp[0].Split('=');
            String[] sp3 = sp[1].Split('=');
            string email = "";
            try
            {
                // 2022-06-08 by Phon_y
                // พี่มาร์ชให้แก้ email ของคุณ surachai ให้แสดงให้ถูกต้อง
                email = result.Properties["mail"][0].ToString();
                if(email.ToLower() == "surachai_p@bst.co.th")
                {
                    email = "Surachai_Pak@bst.co.th";
                }
            }
            catch (Exception)
            {
                Label4.Text = "Error!";
            }

            try
            {
                table.Rows.Add(sp2[1], sp3[1], (result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0] : result.Properties["cn"][0]), email);
                Label4.Text = "";
            }
            catch { Label4.Text = "Error!"; }
        }

        GridView2.DataSource = table;
        GridView2.DataBind();
        msg.Visible = false;
        error.Visible = false;
    }

    protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
    {


        if (Request.QueryString["parent"] != null)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "ww", "addinFullname('" + Request.QueryString["parent"].ToString() + "','" + GridView2.Rows[GridView2.SelectedIndex].Cells[0].Text + "');addin('" + Request.QueryString["parent"].ToString() + "','" + GridView2.Rows[GridView2.SelectedIndex].Cells[1].Text + "');", true);
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "ww", "window.close();");
        }
    }
}