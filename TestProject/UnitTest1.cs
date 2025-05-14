using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace TestProject 
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SendEmail()
        {

            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ashish.durve@bitbuffs.com", "Admin");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("durve.ashish@gmail.com", "Ashish Durve");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }

        [TestMethod]
        public void SendEmail_SMTP()
        {
            var smtpClient = new SmtpClient("smtp.sendgrid.net")
            {
                Port = 587,
                Credentials = new NetworkCredential("apikey", "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"),
                EnableSsl = true,
            };

            smtpClient.Send("ashish.durve@bitbuffs.com", "durve.ashish@gmail.com", "Test SendGrid", "Test Send Grid HTML");
        }
    }
}