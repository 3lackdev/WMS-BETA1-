using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

public partial class TableDataBind : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["bindname"] != null)
        {
            refresh();
        }
    }


    protected void refresh()
    {

        try
        {
            String fillter = "";
            String fillter_inline = "";



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
                    sql += @" 
                            and " + fillter_inline;
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

                ds2 = SQLExecuteReader(ds.Tables[0].Rows[0]["Server"].ToString(), sql);
            }

            //DataSet ds2 = SQLExecuteReader("system", ds.Tables[0].Rows[0][0].ToString());
            ds2.Tables[0].DefaultView.RowFilter = fillter;
            DataTable dt = ds2.Tables[0].DefaultView.ToTable();
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(dt);


            Response.Write(JSONresult);
           

          
        }
        catch (Exception ex) { }

    }
}