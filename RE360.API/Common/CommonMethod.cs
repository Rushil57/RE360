using RE360.API.Auth;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;

namespace RE360.API.Common
{
    public class CommonMethod
    {
        static IConfiguration _configuration = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        private readonly RE360AppDbContext _db;
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;
        public CommonMethod(RE360AppDbContext dbContext)
        {
            _db = dbContext;
        }

        public string GenerateRandomPassword()
        {
            PasswordOptions opts = null;
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }
            //string newPassword = string.Join("", chars.ToArray());
            return string.Join("", chars.ToArray());
        }

        public async Task<string> SendMail(string receiverEmailId, string newpassword, string firstName)
        {
            var filePath = _configuration["BlobStorageSettings:DocumentPath"] + "logo.png" + _configuration["BlobStorageSettings:DocumentPathToken"];
            var senderPassword = _configuration["Smtp:Password"].ToString();
            var sendEmailId = _configuration["Smtp:FromAddress"].ToString();
            var host = _configuration["Smtp:Server"].ToString();
            var port = Convert.ToInt32(_configuration["Smtp:Port"]);

            var mailSubject = "Password Reset";
            var mailBody = "<p style='padding-left:2%;'>Hello " + firstName + "," +
                            "</p><p style='padding-left: 5%;'>Your password has been updated successfully." +
                            "<br>Your updated password is: <b>" + newpassword + "</b>" +
                            "<br><b>Note:- </b> We recommend you to change the password when you login first time with this new password.</p>";
            var sumUp = "<p style='padding-top: 3%;padding-left: 3%;border-left: 1px solid #d5d5ec;'>RE360</p>";


            AlternateView alternateView = AlternateView.CreateAlternateViewFromString
            (
             mailBody + "<br> <div style=\"display: flex;\"><p style=\"padding-left: 2%;\">" +
             "<img src=" + filePath + " style=\"height: 100px;width: 120px;\"></p>" +
             sumUp + "</div>", null, "text/html"
            );

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmailId);
                mail.From = new MailAddress(sendEmailId);
                mail.Subject = mailSubject;
                mail.AlternateViews.Add(alternateView);
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(sendEmailId, senderPassword);
                try
                {
                    smtp.Send(mail);
                    return "Mail Sent Successfully";
                }
                catch (Exception ex)
                {
                    string SendMailError = ex.Message + ex.StackTrace;
                    CommonDBHelper.ErrorLog("AuthenticateController - SendMail", ex.Message, ex.StackTrace);
                    return SendMailError;
                }

            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                CommonDBHelper.ErrorLog("AuthenticateController - SendMail", ex.Message, ex.StackTrace);
                return Error;
            }

        }

    }
}
