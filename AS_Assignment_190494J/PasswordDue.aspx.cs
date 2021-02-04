using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment_190494J
{
    public partial class PasswordDue : System.Web.UI.Page
    {
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        static string salt;
        static string finalHash;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    throw new HttpException(401, "401 Error");
                }
            }
            else
            {
                throw new HttpException(401, "401 Error");
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            string confirmpassword = tb_ConfirmPass.Text.ToString().Trim();
            string email = (string)Session["LoggedIn"];
            string newpass = HttpUtility.HtmlEncode(tb_NewPassword.Text.ToString().Trim());

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            string dbHash2 = getDBHash2(email);
            string dbSalt2 = getDBSalt2(email);
            string dbHash3 = getDBHash3(email);
            string dbSalt3 = getDBSalt3(email);

            string pwdSalt = newpass + dbSalt;
            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt));
            string userHash = Convert.ToBase64String(hashWithSalt);

            string pwdSalt2 = newpass + dbSalt2;
            byte[] hashWithSalt2 = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt2));
            string userHash2 = Convert.ToBase64String(hashWithSalt2);

            string pwdSalt3 = newpass + dbSalt3;
            byte[] hashWithSalt3 = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt3));
            string userHash3 = Convert.ToBase64String(hashWithSalt3);

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    //checking if passwordhash2 and passwordsalt2 is empty | Add new pass into passwordhash2 and passwordsalt2
                    if (dbSalt2 == null && dbHash2 == null)
                    {
                        //************* Start of making new password into database **************

                        //Continue with adding the new password
                        bool validinput = ValidateInput();

                        if (validinput)
                        {

                            if (dbSalt3 == null && dbHash3 == null)
                            {
                                if (confirmpassword == newpass)
                                {
                                    //Generating random "salt"
                                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                    byte[] saltByte = new byte[8];

                                    //Fills array of bytes with a cryptographically strong sequence of random values.
                                    rng.GetBytes(saltByte);
                                    salt = Convert.ToBase64String(saltByte);

                                    SHA512Managed newhashing = new SHA512Managed();

                                    string pwdWithSalt = newpass + salt;
                                    byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpass));
                                    byte[] hashAndSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                                    finalHash = Convert.ToBase64String(hashAndSalt);

                                    RijndaelManaged cipher = new RijndaelManaged();
                                    cipher.GenerateKey();
                                    Key = cipher.Key;
                                    IV = cipher.IV;
                                    updatePassAge(email);

                                    int updateresult = updatePassword2(email);
                                    if (updateresult == 1)
                                    {
                                        Session.Clear();
                                        Session.Abandon();
                                        Session.RemoveAll();

                                        if (Request.Cookies["ASP.NET_SessionId"] != null)
                                        {
                                            Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                                        }

                                        if (Request.Cookies["AuthToken"] != null)
                                        {
                                            Response.Cookies["Authtoken"].Value = string.Empty;
                                            Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                                        }
                                        ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSuccess()", true);
                                    }
                                }
                                else
                                {
                                    lb_ConfirmPassError.Visible = true;
                                    lb_ConfirmPassError.Text = "Does not match with New Password!";
                                    lb_ConfirmPassError.ForeColor = Color.Red;
                                }

                            }
                            else
                            {
                                if (dbHash != userHash && dbHash3 != userHash3)
                                {
                                    if (confirmpassword == newpass)
                                    {
                                        //Generating random "salt"
                                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                        byte[] saltByte = new byte[8];

                                        //Fills array of bytes with a cryptographically strong sequence of random values.
                                        rng.GetBytes(saltByte);
                                        salt = Convert.ToBase64String(saltByte);

                                        SHA512Managed newhashing = new SHA512Managed();

                                        string pwdWithSalt = newpass + salt;
                                        byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpass));
                                        byte[] hashAndSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                                        finalHash = Convert.ToBase64String(hashAndSalt);

                                        RijndaelManaged cipher = new RijndaelManaged();
                                        cipher.GenerateKey();
                                        Key = cipher.Key;
                                        IV = cipher.IV;
                                        updatePassAge(email);

                                        int updateresult = updatePassword2(email);
                                        if (updateresult == 1)
                                        {
                                            Session.Clear();
                                            Session.Abandon();
                                            Session.RemoveAll();

                                            if (Request.Cookies["ASP.NET_SessionId"] != null)
                                            {
                                                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                                            }

                                            if (Request.Cookies["AuthToken"] != null)
                                            {
                                                Response.Cookies["Authtoken"].Value = string.Empty;
                                                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                                            }
                                            ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSuccess()", true);
                                        }
                                    }
                                    else
                                    {
                                        lb_ConfirmPassError.Visible = true;
                                        lb_ConfirmPassError.Text = "Does not match with New Password!";
                                        lb_ConfirmPassError.ForeColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    lb_NewPassError.Visible = true;
                                    lb_NewPassError.Text = "Your password cannot be the same as your last 2 ones!";
                                    lb_NewPassError.ForeColor = Color.Red;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (dbSalt3 == null && dbHash3 == null)
                        {
                            //************* Start of making new password into database **************

                            //Continue with adding the new password
                            bool validinput = ValidateInput();

                            if (validinput)
                            {
                                if (userHash2 != dbHash2 && userHash != dbHash)
                                {
                                    if (confirmpassword == newpass)
                                    {
                                        //Generating random "salt"
                                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                        byte[] saltByte = new byte[8];

                                        //Fills array of bytes with a cryptographically strong sequence of random values.
                                        rng.GetBytes(saltByte);
                                        salt = Convert.ToBase64String(saltByte);

                                        SHA512Managed newhashing = new SHA512Managed();

                                        string pwdWithSalt = newpass + salt;
                                        byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpass));
                                        byte[] hashAndSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                                        finalHash = Convert.ToBase64String(hashAndSalt);

                                        RijndaelManaged cipher = new RijndaelManaged();
                                        cipher.GenerateKey();
                                        Key = cipher.Key;
                                        IV = cipher.IV;

                                        updatePassAge(email);
                                        int updateresult = updatePassword3(email);
                                        if (updateresult == 1)
                                        {
                                            Session.Clear();
                                            Session.Abandon();
                                            Session.RemoveAll();


                                            if (Request.Cookies["ASP.NET_SessionId"] != null)
                                            {
                                                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                                            }

                                            if (Request.Cookies["AuthToken"] != null)
                                            {
                                                Response.Cookies["Authtoken"].Value = string.Empty;
                                                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                                            }
                                            ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSuccess()", true);
                                        }
                                    }
                                    else
                                    {
                                        lb_ConfirmPassError.Visible = true;
                                        lb_ConfirmPassError.Text = "Does not match with New Password!";
                                        lb_ConfirmPassError.ForeColor = Color.Red;
                                    }

                                }
                                else
                                {
                                    lb_NewPassError.Visible = true;
                                    lb_NewPassError.Text = "Your password cannot be the same as your old one!";
                                    lb_NewPassError.ForeColor = Color.Red;
                                }

                            }

                        }
                        else
                        {
                            //************* Start of making new password into database **************


                            //Continue with adding the new password
                            bool validinput = ValidateInput();

                            if (validinput)
                            {
                                if (userHash2 != dbHash2 && userHash3 != dbHash3)
                                {
                                    if (confirmpassword == newpass)
                                    {
                                        //Generating random "salt"
                                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                        byte[] saltByte = new byte[8];

                                        //Fills array of bytes with a cryptographically strong sequence of random values.
                                        rng.GetBytes(saltByte);
                                        salt = Convert.ToBase64String(saltByte);

                                        SHA512Managed newhashing = new SHA512Managed();

                                        string pwdWithSalt = newpass + salt;
                                        byte[] plainHash = newhashing.ComputeHash(Encoding.UTF8.GetBytes(newpass));
                                        byte[] hashAndSalt = newhashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                                        finalHash = Convert.ToBase64String(hashAndSalt);

                                        RijndaelManaged cipher = new RijndaelManaged();
                                        cipher.GenerateKey();
                                        Key = cipher.Key;
                                        IV = cipher.IV;

                                        updatePassAge(email);

                                        int updateresult = updatePassword(email);
                                        if (updateresult == 1)
                                        {
                                            Session.Clear();
                                            Session.Abandon();
                                            Session.RemoveAll();
                                            makePassNull(email);

                                            if (Request.Cookies["ASP.NET_SessionId"] != null)
                                            {
                                                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                                                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                                            }

                                            if (Request.Cookies["AuthToken"] != null)
                                            {
                                                Response.Cookies["Authtoken"].Value = string.Empty;
                                                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                                            }
                                            ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSuccess()", true);
                                        }
                                    }
                                    else
                                    {
                                        lb_ConfirmPassError.Visible = true;
                                        lb_ConfirmPassError.Text = "Does not match with New Password!";
                                        lb_ConfirmPassError.ForeColor = Color.Red;
                                    }

                                }
                                else
                                {
                                    lb_NewPassError.Visible = true;
                                    lb_NewPassError.Text = "Your password cannot be the same as your old one!";
                                    lb_NewPassError.ForeColor = Color.Red;
                                }

                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new HttpException(400, ex.ToString());
            }
            finally { }
        }

        private bool ValidateInput()
        {

            tb_NewPassword.Text = HttpUtility.HtmlEncode(tb_NewPassword.Text);
            tb_ConfirmPass.Text = HttpUtility.HtmlEncode(tb_ConfirmPass.Text);

            lb_NewPassError.Text = String.Empty;
            lb_ConfirmPassError.Text = String.Empty;

            if (String.IsNullOrEmpty(tb_NewPassword.Text))
            {
                lb_NewPassError.Text = "Please enter a password";
                lb_NewPassError.ForeColor = System.Drawing.Color.Red;
            }
            if (String.IsNullOrEmpty(tb_ConfirmPass.Text))
            {
                lb_ConfirmPassError.Text = "Please confirm your password";
                lb_ConfirmPassError.ForeColor = System.Drawing.Color.Red;
            }

            //Check if confirm password is the same as password
            if (tb_ConfirmPass.Text != tb_NewPassword.Text)
            {
                lb_ConfirmPassError.Text = "Not same as password!";
                lb_ConfirmPassError.ForeColor = System.Drawing.Color.Red;
            }

            // Regex password validation
            var regex = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            var match = Regex.Match(tb_NewPassword.Text, regex, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                lb_NewPassError.Text = "Password needs to have 8 - 20 characters, 1 uppercase and 1 lowercase alphabet, 1 number and 1 special character!";
                lb_NewPassError.ForeColor = System.Drawing.Color.Red;
            }


            if (String.IsNullOrEmpty(lb_NewPassError.Text) && String.IsNullOrEmpty(lb_ConfirmPassError.Text))
            {
                return true;
            }
            else
            {
                return false;
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
                throw new HttpException(500, ex.ToString());
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
                throw new HttpException(500, ex.ToString());
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
                throw new HttpException(500, ex.ToString());
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
                throw new HttpException(500, ex.ToString());
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
                throw new HttpException(500, ex.ToString());
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
                throw new HttpException(500, ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        public int updatePassAge(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET MinPassAge = @paraMinPassAge, MaxPassAge = @paraMaxPassAge where Email = @paraEmail"))
                    {
                        DateTime currentdatetime = DateTime.Now;
                        DateTime minpassage = currentdatetime.AddMinutes(5);
                        DateTime maxpassage = currentdatetime.AddMinutes(15);
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraMinPassAge", minpassage);
                        cmd.Parameters.AddWithValue("@paraMaxPassAge", maxpassage);
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
                throw new HttpException(500, ex.ToString());
            }
        }

        // To update new password if passHash2, passSalt2, passHash3, passHash3 is filled
        public int updatePassword(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash = @paraPasswordHash, PasswordSalt = @paraPasswordSalt where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraPasswordHash", finalHash);
                        cmd.Parameters.AddWithValue("@paraPasswordSalt", salt);
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
                throw new HttpException(500, ex.ToString());
            }
        }

        // To update new password to passHash2 and passSalt2
        public int updatePassword2(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash2 = @paraPasswordHash2, PasswordSalt2 = @paraPasswordSalt2 where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraPasswordHash2", finalHash);
                        cmd.Parameters.AddWithValue("@paraPasswordSalt2", salt);
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
                throw new HttpException(500, ex.ToString());
            }
        }

        // To update new password to passHash3 and passSalt3 if passHash2 and passSalt2 is filled.
        public int updatePassword3(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash3 = @paraPasswordHash3, PasswordSalt3 = @paraPasswordSalt3 where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraPasswordHash3", finalHash);
                        cmd.Parameters.AddWithValue("@paraPasswordSalt3", salt);
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
                throw new HttpException(500, ex.ToString());
            }
        }

        // To update passHash2, passSalt2, passHash3, passSalt3 = null
        public int makePassNull(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash2 = @paraPasswordHash2, PasswordSalt2 = @paraPasswordSalt2 where Email = @paraEmail"))
                    {
                        cmd.Parameters.AddWithValue("@paraEmail", email);
                        cmd.Parameters.AddWithValue("@paraPasswordHash2", DBNull.Value);
                        cmd.Parameters.AddWithValue("@paraPasswordSalt2", DBNull.Value);
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
                throw new HttpException(500, ex.ToString());
            }
        }
    }
}