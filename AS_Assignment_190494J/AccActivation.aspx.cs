using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment_190494J
{
    public partial class AccActivation : System.Web.UI.Page
    {
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            string email = Request.QueryString["emailadd"].ToString();
            if (email != null)
            {
                lb_para.Text = "Your email is " + HttpUtility.HtmlEncode(email) + " , Please check your email for activation code";
            }
            
            else
            {
                Response.Redirect("401ErrorPage.aspx", false);
            }
        }


        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string email = Request.QueryString["emailadd"].ToString();
            string verifycode = tb_verify.Text;
            string actualcode = getActivationCode(email);
            if (verifycode.Length != 4 || verifycode != null)
            {
                if (verifycode == actualcode)
                {
                    updateVerification(email);
                    ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSuccess()", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertFailed()", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertFailed()", true);
            }
        }

        public int updateVerification(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET EmailVerified = @paraEmailVerified where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraEmailVerified", "True");
                        cmd.Connection = conn;
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getActivationCode(string userEmail)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select ActivationCode FROM Account WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["ActivationCode"] != null)
                        {
                            if (reader["ActivationCode"] != DBNull.Value)
                            {
                                h = reader["ActivationCode"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }
    }
}