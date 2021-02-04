using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment_190494J
{
    public partial class SignOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    throw new HttpException(401, "401 Error");
                }
                else
                {
                    lb_Msg.Text = "Congratulations! You are logged in.";
                    lb_Msg.ForeColor = System.Drawing.Color.Green;
                }

            }
            else
            {
                throw new HttpException(401, "401 Error");
            }
        }

        //protected void Logout(object sender, EventArgs e)
        //{
        //    Session.Clear();
        //    Session.Abandon();
        //    Session.RemoveAll();

        //    Response.Redirect("SignIn.aspx", false);
        //    if (Request.Cookies["ASP.NET_SessionId"] != null)
        //    {
        //        Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
        //        Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
        //    }

        //    if (Request.Cookies["AuthToken"] != null)
        //    {
        //        Response.Cookies["Authtoken"].Value = string.Empty;
        //        Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
        //    }
        //}
    }
}