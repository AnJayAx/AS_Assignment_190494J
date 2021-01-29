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
    public partial class VerifyAccount : System.Web.UI.Page
    {
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        string activationCode;
        protected void Page_Load(object sender, EventArgs e)
        {
            string email = Request.QueryString["emailagain"].ToString();
            if (email != null)
            {
                if (!IsPostBack)
                {
                    SqlConnection con = new SqlConnection();
                    con.ConnectionString = ASDBConnectionString;
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM Account where Email = @userEmail";
                    cmd.Parameters.AddWithValue("userEmail", email);
                    cmd.Connection = con;
                    SqlDataReader rd = cmd.ExecuteReader();
                    if (rd.HasRows)
                    {
                        lb_para.Text = "Your email is " + email + " , Please check your email for activation code";
                        updateCode(email);
                        sendCode(email);
                    }
                    else
                    {
                        Response.Redirect("401ErrorPage.aspx", false);
                    }
                }
            }

            else
            {
                Response.Redirect("401ErrorPage.aspx", false);
            }
        }

        public int updateCode(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET ActivationCode = @paraActivationCode where Email = @paraEmail"))
                    {
                        Random random = new Random();
                        activationCode = random.Next(1001, 9999).ToString();
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraActivationCode", activationCode);
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

        private void sendCode(string email)
        {
            activationCode = getActivationCode(email);
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential("190494jsitconnect@gmail.com", "P@@55w0rd");
            smtp.EnableSsl = true;
            MailMessage msg = new MailMessage();
            msg.Subject = "Activation Code to Verify Email Address";
            msg.Body = "Dear " + email + ", your activation code is " + activationCode + ".";
            string toAddress = email;
            msg.To.Add(toAddress);
            string fromAddress = "SITCONNECT <190494jsitconnect@gmail.com>";
            msg.From = new MailAddress(fromAddress);
            try
            {
                smtp.Send(msg);
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

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            string email = Request.QueryString["emailagain"].ToString();
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
    }
}