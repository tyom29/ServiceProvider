using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ServiceProvider.Services.EmailService
{
    public static class MailingService
    {
        public static HttpStatusCode SendAsync(IdentityMessage message)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);
                
                mail.From = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"]);
                mail.To.Add(message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml=true;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailUsername"], ConfigurationManager.AppSettings["EmailPassword"]);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}