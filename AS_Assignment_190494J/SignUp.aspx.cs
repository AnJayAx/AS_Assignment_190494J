using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Globalization;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace AS_Assignment_190494J
{
    public partial class SignUp : System.Web.UI.Page
    {
        string ASDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ASDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        static String activationCode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string Password = tb_Password.Text;
                tb_Password.Attributes.Add("value", Password);
            }
            tb_BirthDate.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public class ASObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        private bool ValidateInput()
        {
            tb_FirstName.Text = HttpUtility.HtmlEncode(tb_FirstName.Text);
            tb_LastName.Text = HttpUtility.HtmlEncode(tb_LastName.Text);
            tb_CreditCard.Text = HttpUtility.HtmlEncode(tb_CreditCard.Text);
            tb_Email.Text = HttpUtility.HtmlEncode(tb_Email.Text);
            tb_BirthDate.Text = HttpUtility.HtmlEncode(tb_BirthDate.Text);
            tb_Password.Text = HttpUtility.HtmlEncode(tb_Password.Text);
            tb_ConfirmPassword.Text = HttpUtility.HtmlEncode(tb_ConfirmPassword.Text);
            bool result;
            lb_FNameCheck.Text = String.Empty;
            lb_LNameCheck.Text = String.Empty;
            lb_EmailCheck.Text = String.Empty;
            lb_CreditCheck.Text = String.Empty;
            lb_DOBCheck.Text = String.Empty;
            lb_PasswordCheck.Text = String.Empty;
            lb_ConfirmPassCheck.Text = String.Empty;

            var lettersOnly = @"^[a-zA-Z]{1,25}$";

            if (String.IsNullOrEmpty(tb_FirstName.Text))
            {
                lb_FNameCheck.Text = "Please enter your first name";
                lb_FNameCheck.ForeColor = System.Drawing.Color.Red;
            }
            var fnamelettersmatch = Regex.Match(tb_FirstName.Text, lettersOnly);
            if (!fnamelettersmatch.Success)
            {
                lb_FNameCheck.Text = "First Name should only contain letters!";
                lb_FNameCheck.ForeColor = System.Drawing.Color.Red;
            }

            if (String.IsNullOrEmpty(tb_LastName.Text))
            {
                lb_LNameCheck.Text = "Please enter your last name";
                lb_LNameCheck.ForeColor = System.Drawing.Color.Red;
            }
            var lnamelettersmatch = Regex.Match(tb_LastName.Text, lettersOnly);
            if (!lnamelettersmatch.Success)
            {
                lb_LNameCheck.Text = "Last Name should only contain letters!";
                lb_LNameCheck.ForeColor = System.Drawing.Color.Red;
            }

            if (String.IsNullOrEmpty(tb_Email.Text))
            {
                lb_EmailCheck.Text = "Please enter your email";
                lb_EmailCheck.ForeColor = System.Drawing.Color.Red;
            }
            if (String.IsNullOrEmpty(tb_CreditCard.Text) || tb_CreditCard.Text.Length > 16)
            {
                lb_CreditCheck.Text = "Please enter your credit card number and must be 16 digit long";
                lb_CreditCheck.ForeColor = System.Drawing.Color.Red;
            }

            if (String.IsNullOrEmpty(tb_BirthDate.Text))
            {
                lb_DOBCheck.Text = "Please enter your date of birth";
                lb_DOBCheck.ForeColor = System.Drawing.Color.Red;
            }
            result = DateTime.TryParse(tb_BirthDate.Text, out DateTime dob);
            //var parameterDate = DateTime.ParseExact(tb_BirthDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var todaydate = DateTime.Today;
            if (!result)
            {
                lb_DOBCheck.Text = "Birth Date is invalid!";
                lb_DOBCheck.ForeColor = System.Drawing.Color.Red;
            }

            if (Convert.ToDateTime(tb_BirthDate.Text) > todaydate)
            {
                lb_DOBCheck.Text = "Birth Date should not be after today !";
                lb_DOBCheck.ForeColor = System.Drawing.Color.Red;
            }
            if (String.IsNullOrEmpty(tb_Password.Text))
            {
                lb_PasswordCheck.Text = "Please enter a password";
                lb_PasswordCheck.ForeColor = System.Drawing.Color.Red;
            }
            if (String.IsNullOrEmpty(tb_ConfirmPassword.Text))
            {
                lb_ConfirmPassCheck.Text = "Please confirm your password";
                lb_ConfirmPassCheck.ForeColor = System.Drawing.Color.Red;
            }

            //Check if confirm password is the same as password
            if (tb_ConfirmPassword.Text != tb_Password.Text)
            {
                lb_ConfirmPassCheck.Text = "Not same as password!";
                lb_ConfirmPassCheck.ForeColor = System.Drawing.Color.Red;
            }

            // Regex password validation
            var regex = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
            var match = Regex.Match(tb_Password.Text, regex, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                lb_PasswordCheck.Text = "Password needs to have 8 - 20 characters, 1 uppercase and 1 lowercase alphabet, 1 number and 1 special character!";
                lb_PasswordCheck.ForeColor = System.Drawing.Color.Red;
            }

            //check if email exists in database

            SqlConnection con = new SqlConnection();
            con.ConnectionString = ASDBConnectionString;
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM Account where Email = @userEmail";
            cmd.Parameters.AddWithValue("userEmail", tb_Email.Text);
            cmd.Connection = con;
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                lb_EmailCheck.Text = "Email already registered";
                lb_EmailCheck.ForeColor = System.Drawing.Color.Red;
            }

            if (String.IsNullOrEmpty(lb_FNameCheck.Text) && String.IsNullOrEmpty(lb_LNameCheck.Text) && String.IsNullOrEmpty(lb_DOBCheck.Text) && String.IsNullOrEmpty(lb_CreditCheck.Text) && String.IsNullOrEmpty(lb_EmailCheck.Text) && String.IsNullOrEmpty(lb_PasswordCheck.Text) && String.IsNullOrEmpty(lb_ConfirmPassCheck.Text))
            {
                //lb_SubmitMsg.Text = "Success! Account Added.";
                //lb_SubmitMsg.ForeColor = System.Drawing.Color.Green;
                //return true;
                //ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertSucess()", true);
                return true;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertFailed()", true);
                return false;
            }

        }
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string email = tb_Email.Text.ToString();
                bool validinput = ValidateInput();

                if (validinput)
                {
                    string pwd = tb_Password.Text.ToString().Trim();

                    //Generating random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    createAccount();
                    sendCode();
                    Response.Redirect("AccActivation.aspx?emailadd=" + tb_Email.Text);
                    //activationCode = getActivationCode(tb_Email.Text);
                    //ClientScript.RegisterStartupScript(this.GetType(), "randomtext", "alertInput(activationCode)", true);
                    //updateVerification(email);
                }
            }

        }

        private void sendCode()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new System.Net.NetworkCredential("190494jsitconnect@gmail.com", "P@@55w0rd");
            smtp.EnableSsl = true;
            MailMessage msg = new MailMessage();
            msg.Subject = "Activation Code to Verify Email Address";
            activationCode = getActivationCode(tb_Email.Text);
            msg.Body = "Dear " + tb_Email.Text + ", your activation code is " + activationCode + ".";
            string toAddress = tb_Email.Text;
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

        public bool ValidateCaptcha()
        {
            bool result = true;

            //When user submits the recaptcha form, the user gets a response POST parameter.
            //captchaResponse consist of the user click pattern. Behaviour analytics.
            string captchaResponse = Request.Form["g-recaptcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Le0lBUaAAAAAAjlt1maL7EXES9whcaRIHjmxSx9 &response=" + captchaResponse);

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

        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ASDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FirstName, @LastName, @Email, @CreditCardNo, @DOB, @PasswordHash, @PasswordSalt, @PasswordHash2, @PasswordSalt2, @PasswordHash3, @PasswordSalt3, @EmailVerified, @IV, @Key, @Status, @LockoutCounter, @LockoutReset, @MinPassAge, @MaxPassAge, @ActivationCode)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            DateTime currentdatetime = DateTime.Now;
                            DateTime minpassage = currentdatetime.AddMinutes(5);
                            DateTime maxpassage = currentdatetime.AddMinutes(15);
                            Random random = new Random();
                            activationCode = random.Next(1001, 9999).ToString();
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FirstName", tb_FirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_LastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", tb_Email.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCardNo", Convert.ToBase64String(encryptData(tb_CreditCard.Text.Trim())));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@PasswordHash2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordSalt2", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordHash3", DBNull.Value);
                            cmd.Parameters.AddWithValue("@PasswordSalt3", DBNull.Value);
                            cmd.Parameters.AddWithValue("@EmailVerified", "False");
                            cmd.Parameters.AddWithValue("@DOB", tb_BirthDate.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@Status", "Open");
                            cmd.Parameters.AddWithValue("@LockoutCounter", 0);
                            cmd.Parameters.AddWithValue("@LockoutReset", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MinPassAge", minpassage);
                            cmd.Parameters.AddWithValue("@MaxPassAge", maxpassage);
                            cmd.Parameters.AddWithValue("@ActivationCode", activationCode);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }


        //protected void cbShowPass_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (cbShowPass.Checked == false)
        //    {
        //        tb_Password.TextMode = TextBoxMode.Password;
        //    }
        //    if (cbShowPass.Checked == true)
        //    {
        //        tb_Password.TextMode = TextBoxMode.SingleLine;
        //    }
        //}
    }
}