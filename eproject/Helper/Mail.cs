using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace eproject.Helper
{
    public class Mail
    {
        //mail sending
        public void sendVerifyMail(string body,string to)
        {
            SmtpClient client = new SmtpClient();
            MailMessage emailMessage = new MailMessage();

            emailMessage.From = new MailAddress("localhost@gmail.com");
            emailMessage.To.Add(new MailAddress(to));
            emailMessage.Subject = "Verify Email";
            emailMessage.Body = body;
            client.Send(emailMessage);
        }

    }
}