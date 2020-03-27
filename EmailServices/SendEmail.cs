using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace EmailServices
{
    public class SendEmail : ISendEmail
    {
        public readonly IConfiguration configuration;

        public SendEmail(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Send(string Body, MemoryStream memory = null)
        {
            var emailConfig = this.configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

            bool Resultado = false;
            int _ConfigPortSmtp = emailConfig.Port;
            string _ConfigSmtp = emailConfig.SmtpServer;
            string _FromEmail = emailConfig.From;
            string _FromEmailPswd = emailConfig.Password;
            string _CCEmail = emailConfig.UserName;
            string _ToEmail = emailConfig.To;
            string _SubjectEmail = emailConfig.Subject;
            string NameFile = string.Format("Backup_Data {0}-{1}-{2}.sql", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_ConfigSmtp);
                mail.From = new MailAddress(_FromEmail);
                mail.To.Add(_ToEmail);
                mail.CC.Add(_CCEmail);
                mail.Subject = _SubjectEmail;
                SmtpServer.Port = _ConfigPortSmtp;

                if (memory != null)
                {
                    mail.Attachments.Add(new Attachment(memory, NameFile, "application/sql"));
                }

                SmtpServer.Credentials = new System.Net.NetworkCredential(_FromEmail, _FromEmailPswd);
                SmtpServer.EnableSsl = true;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                Resultado = true;
            }
            catch (Exception ex)
            {
                Send(string.Format("Error to create {0}, Excepcion description: {1}", NameFile, ex));
            }
            return Resultado;
        }
    }
}
