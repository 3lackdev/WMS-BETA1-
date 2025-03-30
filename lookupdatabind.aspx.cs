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
        if (!IsPostBack)
        {
            if (Request.QueryString["parent"] != null && Request.QueryString["bindname"] != null)
            {
                refresh();
            }
        }
    }
    protected void refresh()
    {

        try
        {
            String fillter = "";
            String fillter_inline = "";

            if (txt.Text != "" && DropDownList1.SelectedValue!="")
            {
                fillter = DropDownList1.SelectedValue + " like '%" + txt.Text + "%'";
            }


            if (Request.QueryString["filter"] != null)
            {
                String f = Request.QueryString["filter"].ToString();
                String[] num = f.Split(("}").ToCharArray());
                for (int i = 0; i < num.Length; i++)
                {
                    if (num[i] != "")
                    {
                        String a = num[i].Substring(1, num[i].Length - 1);
                        String[] b = a.Split((",").ToCharArray());

                        if (fillter_inline != "")
                        {
                            fillter_inline += " and " + b[1] + " = '" + b[0] + "'";
                        }
                        else
                        {
                            string[] n = b[1].ToString().Split((".").ToCharArray());
                            if (n.Length == 1)
                            {
                                fillter_inline = n[0] + " = '" + b[0] + "'";
                            }
                            else
                            {
                                fillter_inline = n[1] + " = '" + b[0] + "'";
                            }
                        }
                    }
                }
            }


            DataSet ds = SQLExecuteReader("system", "select sql,Server from DataBindValue where bindname='" + Request.QueryString["bindname"].ToString() + "'");

            DataSet ds2 = new DataSet();
            String sql = "";

            if (ds.Tables[0].Rows[0]["Server"].ToString().IndexOf("ORA_") == 0)
            {
                sql = ds.Tables[0].Rows[0][0].ToString();
                if (sql.ToString().ToLower().IndexOf("where ") > 0)
                {
			if (fillter_inline != "")
                    	{
                    		sql += @" 
                            		and " + fillter_inline;
			}
                }
                else
                {
                    if (fillter_inline != "")
                    {
                        sql += @" 
                                where " + fillter_inline;
                    }
                }

                serviceoracle.Service1 sv = new serviceoracle.Service1();
                String cc = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[ds.Tables[0].Rows[0]["Server"].ToString()].ToString();
                ds2 = sv.ExecuteReader(System.Web.Configuration.WebConfigurationManager.ConnectionStrings[ds.Tables[0].Rows[0]["Server"].ToString()].ToString(), sql);
                //Response.Write(sv.GetCerrentError().ToString());
            }
            else
            {
                sql = ds.Tables[0].Rows[0][0].ToString();
                if (sql.ToString().ToLower().IndexOf(" where ") > 0)
                {
                    sql += " and " + fillter_inline;
                }
                else
                {
                    if (fillter_inline != "")
                    {
                        sql += " where " + fillter_inline;
                    }
                }

 		try
                {
                	if((sql.LastIndexOf(" and")+4)<sql.Length)
                	{
                    		sql = sql.Substring(0,sql.LastIndexOf(" and"));
                	}
     		}
                catch { }

                ds2 = SQLExecuteReader(ds.Tables[0].Rows[0]["Server"].ToString(), sql);
            }

            //DataSet ds2 = SQLExecuteReader("system", ds.Tables[0].Rows[0][0].ToString());
            ds2.Tables[0].DefaultView.RowFilter = fillter;
            GridView2.DataSource = ds2.Tables[0].DefaultView.ToTable();
            GridView2.DataBind();


           

            if (!IsPostBack)
            {
                DropDownList1.Items.Clear();
                for (int i = 0; i < ds2.Tables[0].DefaultView.ToTable().Columns.Count; i++)
                {
                    DropDownList1.Items.Add(new ListItem(ds2.Tables[0].DefaultView.ToTable().Columns[i].ToString()));
                }
            }
        }
        catch(Exception ex) {  }

    }

    protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
    {
        refresh();

        if (Request.QueryString["parent"] != null && Request.QueryString["return"]!=null)
        {
            
            string[] st = Request.QueryString["return"].Split(("}").ToCharArray());
            if (st.Length > 0)
            {
                string outp="";
                foreach (string s in st)
                {
                    string[] sp = s.Split((",").ToCharArray());
                    if (sp.Length > 1)
                    {
                        sp[0] = sp[0].Replace("{", "");
                        sp[1] = sp[1].Replace("}", "");

                        outp += "addin('" + sp[0] + "','" + GridView2.Rows[GridView2.SelectedIndex].Cells[int.Parse(sp[1])].Text.Replace("&amp;", "&").Replace(System.Environment.NewLine, "\\n") + "');";
                            
                        
                    }


                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "ww", outp, true);
            }

            this.ClientScript.RegisterStartupScript(this.GetType(), "www", "window.close();",true);
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "ww", "window.close();",true);
        }
    }
    protected void Button5_Click(object sender, EventArgs e)
    {
        refresh();
    }
    protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {

            if (Request.QueryString["hidecolumn"] != null)
            {
                string[] st = Request.QueryString["hidecolumn"].Split((",").ToCharArray());
                for (int i = 0; i < st.Length; i++)
                {
                    try
                    {
                        e.Row.Cells[int.Parse(st[i])].Visible = false;
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (st.Length == 0)
                {
                    e.Row.Cells[int.Parse(Request.QueryString["hidecolumn"])].Visible = false;
                }
            }
        }
    }
}