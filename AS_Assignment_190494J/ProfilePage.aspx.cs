using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment_190494J
{
    public partial class ProfilePage : System.Web.UI.Page
    {
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] creditcard = null;
        string userEmail = null;
        static string salt;
        static string finalHash;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("401ErrorPage.aspx", false);
                }
                else
                {
                    userEmail = (string)Session["LoggedIn"];
                    getUserProfile(userEmail);
                }

            }
            else
            {
                throw new HttpException(401, "401 Error");
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                //Create a decryptor to perform the stream transform
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                //Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string.
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(500, ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected void getUserProfile(string userEmail)
        {
            SqlConnection conn = new SqlConnection(ASDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@UserEmail";
            SqlCommand command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@UserEmail", userEmail);

            try
            {
                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FirstName"] != DBNull.Value)
                        {
                            tb_FName.Text = reader["FirstName"].ToString();
                        }
                        if (reader["LastName"] != DBNull.Value)
                        {
                            tb_LName.Text = reader["LastName"].ToString();
                        }
                        if (reader["Email"] != null)
                        {
                            tb_Email.Text = reader["Email"].ToString();
                        }
                        if (reader["DOB"] != DBNull.Value)
                        {
                            tb_DOB.Text = reader["DOB"].ToString();
                        }
                        if (reader["CreditCardNo"] != DBNull.Value)
                        {
                            creditcard = Convert.FromBase64String(reader["CreditCardNo"].ToString());
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    tb_CreditCard.Text = decryptData(creditcard);
                    tb_CreditCard.Text = "**** **** **** " + tb_CreditCard.Text.Substring(tb_CreditCard.Text.Length - 4);
                }
            }
            catch (Exception ex)
            {
                throw new HttpException(500, ex.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        protected void btn_ResetPass_Click(object sender, EventArgs e)
        {
            DateTime currenttime = DateTime.Now;
            DateTime minpassage = Convert.ToDateTime(getMinPassAge(tb_Email.Text));
            int compareminage = DateTime.Compare(currenttime, minpassage);
            if (compareminage >= 0)
            {
                lb_OldPass.Visible = true;
                tb_OldPass.Visible = true;
                lb_NewPass.Visible = true;
                tb_NewPass.Visible = true;
                lb_ConfirmNewPass.Visible = true;
                tb_ConfirmNewPass.Visible = true;
                btn_UpdatePass.Visible = true;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertMinPassAge()", true);
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

        protected string getMinPassAge(string userEmail)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(ASDBConnectionString);
            string sql = "SELECT MinPassAge FROM ACCOUNT WHERE Email=@USEREMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USEREMAIL", userEmail);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["MinPassAge"] != null)
                        {
                            if (reader["MinPassAge"] != DBNull.Value)
                            {
                                s = reader["MinPassAge"].ToString();
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

        protected void btn_UpdatePass_Click(object sender, EventArgs e)
        {
            string oldpassword = tb_OldPass.Text.ToString().Trim();
            string newpassword = tb_NewPass.Text.ToString().Trim();
            string confirmpassword = tb_ConfirmNewPass.Text.ToString().Trim();
            string email = tb_Email.Text.ToString();

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            string dbHash2 = getDBHash2(email);
            string dbSalt2 = getDBSalt2(email);
            string dbHash3 = getDBHash3(email);
            string dbSalt3 = getDBSalt3(email);

            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    //checking if passwordhash2 and passwordsalt2 is empty | Add new pass into passwordhash2 and passwordsalt2
                    if (dbSalt2 == null && dbHash2 == null)
                    {
                        //************* Start of making new password into database **************
                        string pwdSalt = oldpassword + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);

                        string pwdSalt3 = oldpassword + dbSalt3;
                        byte[] hashWithSalt3 = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt3));
                        string userHash3 = Convert.ToBase64String(hashWithSalt3);

                        // if old password is correct, continue
                        if (userHash.Equals(dbHash))
                        {
                            //Continue with adding the new password
                            lb_ErrorPass.Text = "";
                            bool validinput = ValidateInput();

                            if (validinput)
                            {
                                string newpass = HttpUtility.HtmlEncode(tb_NewPass.Text.ToString().Trim());
                                if (dbSalt3 == null && dbHash3 == null)
                                {
                                    if (newpass != oldpassword)
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
                                            updatePassAge(tb_Email.Text);

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
                                            lb_ErrorConfirmPass.Visible = true;
                                            lb_ErrorConfirmPass.Text = "Does not match with New Password!";
                                            lb_ErrorConfirmPass.ForeColor = Color.Red;
                                        }
                                    }
                                    else
                                    {
                                        lb_ErrorNewPass.Visible = true;
                                        lb_ErrorNewPass.Text = "Your password cannot be the same as your old one!";
                                        lb_ErrorNewPass.ForeColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    if (newpass != oldpassword && newpass != userHash3)
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
                                            updatePassAge(tb_Email.Text);

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
                                            lb_ErrorConfirmPass.Visible = true;
                                            lb_ErrorConfirmPass.Text = "Does not match with New Password!";
                                            lb_ErrorConfirmPass.ForeColor = Color.Red;
                                        }
                                    }
                                    else
                                    {
                                        lb_ErrorNewPass.Visible = true;
                                        lb_ErrorNewPass.Text = "Your password cannot be the same as your last 2 ones!";
                                        lb_ErrorNewPass.ForeColor = Color.Red;
                                    }
                                }

                            }
                        }
                        else
                        {
                            lb_ErrorPass.Visible = true;
                            lb_ErrorPass.Text = "Old password is incorrect!";
                            lb_ErrorPass.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        if (dbSalt3 == null && dbHash3 == null)
                        {
                            //************* Start of making new password into database **************
                            string pwdSalt = oldpassword + dbSalt2;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            //string pwdSalt1 = oldpassword + dbSalt;
                            //byte[] hashWithSalt1 = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt1));
                            //string userHash1 = Convert.ToBase64String(hashWithSalt1);

                            string newpassSalt = newpassword + dbSalt;
                            byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpassSalt));
                            string newuserHash = Convert.ToBase64String(newhashWithSalt);

                            // if old password is correct, continue
                            if (userHash.Equals(dbHash2))
                            {
                                //Continue with adding the new password
                                lb_ErrorPass.Text = "";
                                bool validinput = ValidateInput();

                                if (validinput)
                                {
                                    string newpass = HttpUtility.HtmlEncode(tb_NewPass.Text.ToString().Trim());
                                    if (newpass != oldpassword && newuserHash != dbHash)
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

                                            updatePassAge(tb_Email.Text);
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
                                            lb_ErrorConfirmPass.Visible = true;
                                            lb_ErrorConfirmPass.Text = "Does not match with New Password!";
                                            lb_ErrorConfirmPass.ForeColor = Color.Red;
                                        }

                                    }
                                    else
                                    {
                                        lb_ErrorNewPass.Visible = true;
                                        lb_ErrorNewPass.Text = "Your password cannot be the same as your old one!";
                                        lb_ErrorNewPass.ForeColor = Color.Red;
                                    }

                                }
                            }
                            else
                            {
                                lb_ErrorPass.Visible = true;
                                lb_ErrorPass.Text = "Old password is incorrect!";
                                lb_ErrorPass.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            //************* Start of making new password into database **************
                            string pwdSalt = oldpassword + dbSalt3;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            string newpassSalt = newpassword + dbSalt3;
                            byte[] newhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newpassSalt));
                            string newuserHash = Convert.ToBase64String(newhashWithSalt);

                            // if old password is correct, continue
                            if (userHash.Equals(dbHash3))
                            {
                                //Continue with adding the new password
                                lb_ErrorPass.Text = "";
                                bool validinput = ValidateInput();

                                if (validinput)
                                {
                                    string newpass = HttpUtility.HtmlEncode(tb_NewPass.Text.ToString().Trim());
                                    if (newpass != oldpassword && newuserHash != dbHash3)
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

                                            updatePassAge(tb_Email.Text);

                                            int updateresult = updatePassword(email);
                                            if (updateresult == 1)
                                            {
                                                Session.Clear();
                                                Session.Abandon();
                                                Session.RemoveAll();
                                                makePassNull(tb_Email.Text);

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
                                            lb_ErrorConfirmPass.Visible = true;
                                            lb_ErrorConfirmPass.Text = "Does not match with New Password!";
                                            lb_ErrorConfirmPass.ForeColor = Color.Red;
                                        }

                                    }
                                    else
                                    {
                                        lb_ErrorNewPass.Visible = true;
                                        lb_ErrorNewPass.Text = "Your password cannot be the same as your old one!";
                                        lb_ErrorNewPass.ForeColor = Color.Red;
                                    }

                                }
                            }
                            else
                            {
                                lb_ErrorPass.Visible = true;
                                lb_ErrorPass.Text = "Old password is incorrect!";
                                lb_ErrorPass.ForeColor = Color.Red;
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

        private bool ValidateInput()
        {

            tb_NewPass.Text = HttpUtility.HtmlEncode(tb_NewPass.Text);
            tb_ConfirmNewPass.Text = HttpUtility.HtmlEncode(tb_ConfirmNewPass.Text);

            lb_ErrorNewPass.Text = String.Empty;
            lb_ErrorConfirmPass.Text = String.Empty;

            if (String.IsNullOrEmpty(tb_NewPass.Text))
            {
                lb_ErrorNewPass.Text = "Please enter a password";   
                lb_ErrorNewPass.ForeColor = System.Drawing.Color.Red;
            }
            if (String.IsNullOrEmpty(tb_ConfirmNewPass.Text))
            {
                lb_ErrorConfirmPass.Text = "Please confirm your password";
                lb_ErrorConfirmPass.ForeColor = System.Drawing.Color.Red;
            }

            //Check if confirm password is the same as password
            if (tb_ConfirmNewPass.Text != tb_NewPass.Text)
            {
                lb_ErrorConfirmPass.Text = "Not same as password!";
                lb_ErrorConfirmPass.ForeColor = System.Drawing.Color.Red;
            }

            // Regex password validation
            var regex = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            var match = Regex.Match(tb_NewPass.Text, regex, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                lb_ErrorNewPass.Text = "Password needs to have 8 - 20 characters, 1 uppercase and 1 lowercase alphabet, 1 number and 1 special character!";
                lb_ErrorNewPass.ForeColor = System.Drawing.Color.Red;
            }


            if (String.IsNullOrEmpty(lb_ErrorNewPass.Text) && String.IsNullOrEmpty(lb_ErrorConfirmPass.Text))
            {
                return true;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertFailed()", true);
                return false;
            }

        }
    }
}