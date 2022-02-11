using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// Sends an email
/// </summary>
namespace VisitorSignInSystem.Server.BLL
{
    public class VsisMail
    {
        private const string _SMTP = "mail.manateepao.com";
        private const int _PORT = 25;

        private static MailMessage message;

        public string MailFrom { get; set; }
        public string MailDisplayName { get; set; }
        public string MailTo { get; set; }
        public string MailSubj { get; set; }
        public string MailTitle { get; set; }
        public string MailBody { get; set; }

        public VsisMail(string mail_from, string mail_display_name, string mail_to, string mail_subj, string mail_title, string mail_body)
        {
            MailFrom = mail_from;
            MailDisplayName = mail_display_name;
            MailTo = mail_to;
            MailSubj = mail_subj;
            MailTitle = mail_title;
            MailBody = mail_body;
        }

        public void SendTheMail()
        {
            try
            {
                SmtpClient client = new SmtpClient(_SMTP, _PORT);
                MailAddress from = new MailAddress(MailFrom, MailDisplayName, System.Text.Encoding.UTF8);

                message = new MailMessage();
                
                // split multiple addresses
                foreach (var address in MailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    message.To.Add(address);

                message.From = from;
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Body = FormatMail(MailBody, MailTitle);
                message.Subject = MailSubj;
                message.Priority = MailPriority.High;

                // not using callback or token
                // Set the method that is called back when the send operation ends.
                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                // The userState can be any object that allows
                // callback method to identify this send operation.
                //client.SendAsync(message, userState);
                client.SendAsync(message, null);
            }
            catch (Exception){}
        }

        private string FormatMail(string mail_body, string mail_title)
        {
            return $"<html><head><meta charset=\"utf-8\"></head><body><div style=\"font-family:'Open Sans';font-weight:400;font-size:16px;color:#0A4370;\"><h3 style=\"color:#F51720;\">{mail_title}</h3>\r\n{mail_body}</div></body></html>";
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            //String token = (string)e.UserState;
            //if (e.Cancelled)
            //{
            //}
            if (e.Error != null)
            {
            }
            else
            {
            }
            message.Dispose();
            //mailSent = true;
        }
    }
}
