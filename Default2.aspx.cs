using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Default2 : BST.libDB
{
    protected void Page_Load(object sender, EventArgs e)
    {
 Response.Write(getemailbyuser("preeyanuch_s"));
 Response.Write(IsDomain());
        Label3.Text = ("chakrapan_a@bst.co.th;Kattika_k@bst.co.th;").ToLower().IndexOf(getemailbyuser("Kattika_M").ToLower()).ToString();
    }
}