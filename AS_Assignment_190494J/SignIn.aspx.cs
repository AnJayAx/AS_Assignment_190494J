using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace AS_Assignment_190494J
{
    public partial class SignIn : System.Web.UI.Page
    {
        public static int MaxInvalidPasswordAttempts { get; }
        //static String lockstatus;
        //static int attemptcount = 0;
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        //string errorMsg;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["AuthToken"] != null)
                {
                    Response.Cookies["AuthToken"].Value = string.Empty;
                    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                }
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }


                //string Password = tb_Password.Text;
                //tb_Password.Attributes.Add("value", Password);
            }

        }

        public class ASObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter.
            //captchaResponse consist of the user click pattern. Behaviour analytics.
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Le1zTgaAAAAALxbir8hTgaBWp2SIiu0jHxmzClN &response=" + captchaResponse);

            try
            {
                //Codes to receive the Response in JSON format from google server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        //The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        //To show the JSON response string for learning purpose
                        lb_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        //Create jsonobject to handle the response e.g. success or Error
                        //Deserialize Json
                        ASObject jsonObject = js.Deserialize<ASObject>(jsonResponse);

                        //Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (!ValidateCaptcha())
            {

                string userEmail = HttpUtility.HtmlEncode(tb_Email.Text.ToString().Trim());
                string pwd = HttpUtility.HtmlEncode(tb_Password.Text.ToString().Trim());

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userEmail);
                string dbSalt = getDBSalt(userEmail);
                string dbHash2 = getDBHash2(userEmail);
                string dbSalt2 = getDBSalt2(userEmail);
                string dbHash3 = getDBHash3(userEmail);
                string dbSalt3 = getDBSalt3(userEmail);

                string IsVerified = getVerified(userEmail);
                //lb_Error.Text = getDBHash(userEmail);
                try
                {
                    if (IsVerified == "True")
                    {


                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {

                            if (dbHash2 == null && dbSalt2 == null)
                            {
                                // ************* Start of loggin in ****************
                                string pwdWithSalt = pwd + dbSalt;
                                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                string userHash = Convert.ToBase64String(hashWithSalt);
                                DateTime datetimenow = DateTime.Now;
                                //DateTime datetimereset = getResetTime(tb_Email.Text).to;

                                //lb_Error.Text = status + "Test" + status;
                                if (getResetTime(tb_Email.Text) != null)
                                {
                                    DateTime datetimereset = Convert.ToDateTime(getResetTime(tb_Email.Text));
                                    int comparetime = DateTime.Compare(datetimenow, datetimereset);
                                    if (comparetime >= 0)
                                    {
                                        accOpen(tb_Email.Text);
                                    }
                                }
                                string status = getStatus(tb_Email.Text);
                                if (status == "Open")
                                {
                                    if (userHash.Equals(dbHash))
                                    {
                                        resetCount(tb_Email.Text);
                                        resetLockoutTimer(tb_Email.Text);
                                        Session["LoggedIn"] = tb_Email.Text.Trim();
                                        //create a new GUID and save into the session
                                        string guid = Guid.NewGuid().ToString();
                                        Session["AuthToken"] = guid;

                                        //Create a new cookie with this guid value
                                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                        Session["UserEmail"] = userEmail;

                                        //insert reset password here if compare(currenttime, maxtime) >= 0
                                        DateTime datetimemax = Convert.ToDateTime(getMaxPassAge(tb_Email.Text));
                                        DateTime timenow = DateTime.Now;
                                        int comparingmaxtime = DateTime.Compare(timenow, datetimemax);
                                        if (comparingmaxtime >= 0)
                                        {
                                            Response.Redirect("PasswordDue.aspx", false);
                                        }
                                        else
                                        {
                                            Response.Redirect("LoggedIn.aspx", false);
                                        }

                                    }
                                    else
                                    {
                                        addCounter(tb_Email.Text);
                                        int counttries = getCounter(tb_Email.Text);
                                        lb_Error.Text = "Email or password is not valid. Please try again. You have " + (3 - counttries) + " tries left.";
                                        //tb_Email.Text = "";
                                        tb_Password.Text = "";
                                        //counttries = counttries + 1;
                                        //Response.Redirect("Login.aspx", false);
                                        if (counttries == 3)
                                        {
                                            accLockout(tb_Email.Text);
                                            lockoutReset(tb_Email.Text);
                                            resetCount(tb_Email.Text);
                                            ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout30()", true);
                                        }
                                    }
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout()", true);
                                }
                            }
                            else
                            {
                                if (dbHash3 == null && dbSalt3 == null)
                                {
                                    // ************* Start of loggin in ****************
                                    string pwdWithSalt = pwd + dbSalt2;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                    string userHash = Convert.ToBase64String(hashWithSalt);
                                    DateTime datetimenow = DateTime.Now;
                                    //DateTime datetimereset = getResetTime(tb_Email.Text).to;

                                    //lb_Error.Text = status + "Test" + status;
                                    if (getResetTime(tb_Email.Text) != null)
                                    {
                                        DateTime datetimereset = Convert.ToDateTime(getResetTime(tb_Email.Text));
                                        int comparetime = DateTime.Compare(datetimenow, datetimereset);
                                        if (comparetime >= 0)
                                        {
                                            accOpen(tb_Email.Text);
                                        }
                                    }
                                    string status = getStatus(tb_Email.Text);
                                    if (status == "Open")
                                    {
                                        if (userHash.Equals(dbHash2))
                                        {
                                            resetCount(tb_Email.Text);
                                            resetLockoutTimer(tb_Email.Text);
                                            Session["LoggedIn"] = tb_Email.Text.Trim();
                                            //create a new GUID and save into the session
                                            string guid = Guid.NewGuid().ToString();
                                            Session["AuthToken"] = guid;

                                            //Create a new cookie with this guid value
                                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                            Session["UserEmail"] = userEmail;
                                            Response.Redirect("LoggedIn.aspx", false);

                                            //insert reset password here if compare(currenttime, maxtime) >= 0
                                            DateTime datetimemax = Convert.ToDateTime(getMaxPassAge(tb_Email.Text));
                                            DateTime timenow = DateTime.Now;
                                            int comparingmaxtime = DateTime.Compare(timenow, datetimemax);
                                            if (comparingmaxtime >= 0)
                                            {
                                                Response.Redirect("PasswordDue.aspx", false);
                                            }
                                            else
                                            {
                                                Response.Redirect("LoggedIn.aspx", false);
                                            }

                                        }
                                        else
                                        {
                                            addCounter(tb_Email.Text);
                                            int counttries = getCounter(tb_Email.Text);
                                            lb_Error.Text = "Email or password is not valid. Please try again. You have " + (3 - counttries) + " tries left.";
                                            //tb_Email.Text = "";
                                            tb_Password.Text = "";
                                            //counttries = counttries + 1;
                                            //Response.Redirect("Login.aspx", false);
                                            if (counttries == 3)
                                            {
                                                accLockout(tb_Email.Text);
                                                lockoutReset(tb_Email.Text);
                                                resetCount(tb_Email.Text);
                                                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout30()", true);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout()", true);
                                    }
                                }
                                else
                                {
                                    // ************* Start of loggin in ****************
                                    string pwdWithSalt = pwd + dbSalt3;
                                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                    string userHash = Convert.ToBase64String(hashWithSalt);
                                    DateTime datetimenow = DateTime.Now;
                                    //DateTime datetimereset = getResetTime(tb_Email.Text).to;

                                    //lb_Error.Text = status + "Test" + status;
                                    if (getResetTime(tb_Email.Text) != null)
                                    {
                                        DateTime datetimereset = Convert.ToDateTime(getResetTime(tb_Email.Text));
                                        int comparetime = DateTime.Compare(datetimenow, datetimereset);
                                        if (comparetime >= 0)
                                        {
                                            accOpen(tb_Email.Text);
                                        }
                                    }
                                    string status = getStatus(tb_Email.Text);
                                    if (status == "Open")
                                    {
                                        if (userHash.Equals(dbHash3))
                                        {
                                            resetCount(tb_Email.Text);
                                            resetLockoutTimer(tb_Email.Text);
                                            Session["LoggedIn"] = tb_Email.Text.Trim();
                                            //create a new GUID and save into the session
                                            string guid = Guid.NewGuid().ToString();
                                            Session["AuthToken"] = guid;

                                            //Create a new cookie with this guid value
                                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                            Session["UserEmail"] = userEmail;
                                            Response.Redirect("LoggedIn.aspx", false);

                                            //insert reset password here if compare(currenttime, maxtime) >= 0
                                            DateTime datetimemax = Convert.ToDateTime(getMaxPassAge(tb_Email.Text));
                                            DateTime timenow = DateTime.Now;
                                            int comparingmaxtime = DateTime.Compare(timenow, datetimemax);
                                            if (comparingmaxtime >= 0)
                                            {
                                                Response.Redirect("PasswordDue.aspx", false);
                                            }
                                            else
                                            {
                                                Response.Redirect("LoggedIn.aspx", false);
                                            }
                                        }
                                        else
                                        {
                                            addCounter(tb_Email.Text);
                                            int counttries = getCounter(tb_Email.Text);
                                            lb_Error.Text = "Email or password is not valid. Please try again. You have " + (3 - counttries) + " tries left.";
                                            //tb_Email.Text = "";
                                            tb_Password.Text = "";
                                            //counttries = counttries + 1;
                                            //Response.Redirect("Login.aspx", false);
                                            if (counttries == 3)
                                            {
                                                accLockout(tb_Email.Text);
                                                lockoutReset(tb_Email.Text);
                                                resetCount(tb_Email.Text);
                                                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout30()", true);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertLockout()", true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertVerify()", true);
                    }
                }
                catch (Exception ex)
                {
                    throw new HttpException(400, ex.ToString());
                }
                finally { }

            }

        }

        protected string getDBHash(string userEmail)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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

        protected string getDBSalt(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordSalt FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
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
            return s;
        }

        protected string getDBHash2(string userEmail)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordHash2 FROM Account WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash2"] != null)
                        {
                            if (reader["PasswordHash2"] != DBNull.Value)
                            {
                                h = reader["PasswordHash2"].ToString();
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

        protected string getDBSalt2(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordSalt2 FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt2"] != null)
                        {
                            if (reader["PasswordSalt2"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt2"].ToString();
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
            return s;
        }

        protected string getDBHash3(string userEmail)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordHash3 FROM Account WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash3"] != null)
                        {
                            if (reader["PasswordHash3"] != DBNull.Value)
                            {
                                h = reader["PasswordHash3"].ToString();
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

        protected string getDBSalt3(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select PasswordSalt3 FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt3"] != null)
                        {
                            if (reader["PasswordSalt3"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt3"].ToString();
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
            return s;
        }

        protected string getVerified(string userEmail)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select EmailVerified FROM Account WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["EmailVerified"] != null)
                        {
                            if (reader["EmailVerified"] != DBNull.Value)
                            {
                                h = reader["EmailVerified"].ToString();
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

        public int accLockout(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET Status = @paraStatus where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraStatus", "Locked");
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

        public int accOpen(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET Status = @paraStatus where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraStatus", "Open");
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

        public int lockoutReset(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutReset = @paraLockReset where Email = @paraEmail"))
                    {
                        DateTime currentTime = DateTime.Now;
                        DateTime resetTime = currentTime.AddSeconds(30);
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraLockReset", resetTime);
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

        public int resetLockoutTimer(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutReset = @paraLockReset where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraLockReset", DBNull.Value);
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

        public int resetCount(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutCounter = @paraLockCounter where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraLockCounter", 0);
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

        public int addCounter(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET LockoutCounter = LockoutCounter + @paraCount where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraCount", 1);
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

        protected int getCounter(string userEmail)
        {
            int s = 0;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select LockoutCounter FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockoutCounter"] != null)
                        {
                            if (reader["LockoutCounter"] != DBNull.Value)
                            {
                                s = Convert.ToInt32(reader["LockoutCounter"]);
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
            return s;
        }

        protected string getStatus(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select Status FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Status"] != null)
                        {
                            if (reader["Status"] != DBNull.Value)
                            {
                                s = reader["Status"].ToString();
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
            return s;
        }

        protected string getResetTime(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select LockoutReset FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["LockoutReset"] != null)
                        {
                            if (reader["LockoutReset"] != DBNull.Value)
                            {
                                s = reader["LockoutReset"].ToString();
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
            return s;
        }

        protected string getMaxPassAge(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "select MaxPassAge FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["MaxPassAge"] != null)
                        {
                            if (reader["MaxPassAge"] != DBNull.Value)
                            {
                                s = reader["MaxPassAge"].ToString();
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
            return s;
        }

        protected void btn_Verify_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertVerifyAccount()", true);

        }
    }
}