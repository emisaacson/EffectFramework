using Local.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace Local.Classes.Helpers
{
    public class EmailSender
    {
        public static void SendEmail(SendEmailViewModel model, string body)
        {
            var mailMessage = new MailMessage();
            foreach (var recipient in model.To)
            {
                mailMessage.To.Add(recipient);
            }
            if (model.CC != null)
            {
                foreach (var recipient in model.CC)
                {
                    mailMessage.CC.Add(recipient);
                }
            }
            if (model.BCC != null)
            {
                foreach (var recipient in model.BCC)
                {
                    mailMessage.Bcc.Add(recipient);
                }
            }
            mailMessage.Subject = model.Subject ?? "";
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            var smptClient = new SmtpClient();
            smptClient.Send(mailMessage);
        }

        public static void SendMail(string from, string to, string subject, string body, List<MailerAttachment> attachments = null)
        {
            //mx1.hostinternational.net
            //string server = "192.168.200.202";
           // string server = ConfigurationManager.AppSettings["SmtpServer"];
            MailMessage msg = new MailMessage(from, to, subject, body);

            // set default reply to
            if (ConfigurationManager.AppSettings["HRSupportEmail"] != null)
            {
                msg.ReplyToList.Add(new MailAddress(ConfigurationManager.AppSettings["HRSupportEmail"]));
            }

            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    var s = new MemoryStream(item.Data);
                    var attach = new System.Net.Mail.Attachment(s, item.Name, item.Type);
                    msg.Attachments.Add(attach);
                }
            }

            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Send(msg);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                msg.Dispose();
            }
        }
       
    }
    public class MailerAttachment
    {
        public string Name;
        public byte[] Data;
        public string Type;
    }
}
