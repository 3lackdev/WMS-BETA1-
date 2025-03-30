<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        RouteTable.Routes.MapPageRoute("api", "api/{workflowid}/{column}", "~/api/getdata.aspx");
        RouteTable.Routes.MapPageRoute("apiarg", "api/{workflowid}", "~/api/getdata.aspx");
        RouteTable.Routes.MapPageRoute("apiargWhere", "api/{workflowid}/{column}/{where}", "~/api/getdata.aspx");
        RouteTable.Routes.MapPageRoute("apiargWhereOrder", "api/{workflowid}/{column}/{where}/{orderby}", "~/api/getdata.aspx");

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
