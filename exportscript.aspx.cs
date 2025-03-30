using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class exportscript : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Request.QueryString["wfrowid"] != null)
            {
                string rowid = Request.QueryString["wfrowid"].ToString();
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=" + Request.QueryString["wfrowid"].ToString() + ".sql");
                Response.AddHeader("content-type", "text/plain");
                using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                {


                    writer.WriteLine("BEGIN TRAN T1;");
                    writer.WriteLine(Environment.NewLine + @"--################################################  WorkFlow " + Environment.NewLine);
                    writer.WriteLine(getColumn("WorkFlow"));
                    writer.WriteLine(getRow("WorkFlow", " where WFRowId='" + rowid + "'"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end WorkFlow " + Environment.NewLine);


                    writer.WriteLine(Environment.NewLine + @"--################################################  WorkFlowStep " + Environment.NewLine);
                    writer.WriteLine(getColumn("WorkFlowStep"));
                    writer.WriteLine(getRow("WorkFlowStep", " where WFRowId='" + rowid + "'"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end WorkFlowStep " + Environment.NewLine);


                    writer.WriteLine(Environment.NewLine + @"--################################################  WorkFlowTemplate " + Environment.NewLine);
                    writer.WriteLine(getColumn("WorkFlowTemplate"));
                    writer.WriteLine(getRow("WorkFlowTemplate", " where WFRowId='" + rowid + "'"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end WorkFlowTemplate " + Environment.NewLine);


                    writer.WriteLine(Environment.NewLine + @"--################################################  TemplateObject " + Environment.NewLine);
                    writer.WriteLine(getColumn("TemplateObject"));
                    writer.WriteLine(getRow("TemplateObject", " where TemplateRowId in (select TemplateRowId from WorkFlowTemplate where WFRowId= '" + rowid + "')"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end TemplateObject " + Environment.NewLine);

                    writer.WriteLine(Environment.NewLine + @"--################################################  StepEnableObject " + Environment.NewLine);
                    writer.WriteLine(getColumn("StepEnableObject"));
                    writer.WriteLine(getRow("StepEnableObject", " where StepRowId in (select StepRowId from WorkFlowStep where WFRowId= '" + rowid + "')"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end StepEnableObject " + Environment.NewLine);


                    writer.WriteLine(Environment.NewLine + @"--################################################  Prosition " + Environment.NewLine);
                    writer.WriteLine(getColumn("Prosition"));
                    writer.WriteLine(getRow("Prosition", " where WFRowId= '" + rowid + "'"));
                    writer.WriteLine(Environment.NewLine + @"--################################################ end Prosition " + Environment.NewLine);

                    writer.WriteLine("COMMIT TRAN T1;");


                }
                Response.End();

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
                                                set @tbname='"+tbname+ @"'



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
                outp=outp+@"
                    (";
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (ds.Tables[1].Rows[j]["DATA_TYPE"].ToString() == "datetime")
                    {
                        outp = outp + "'" + DateTime.Parse(ds.Tables[0].Rows[i][ds.Tables[0].Columns[j].ToString()].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "', ";
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
}