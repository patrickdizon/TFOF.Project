namespace TFOF.Areas.Core.Services
{
    using System;
    using System.Net.Mail;
    using System.Net;
    using System.Configuration;
    using Microsoft.AspNet.Identity;
    using System.Threading.Tasks;
	using System.Collections.Generic;

    public class EmailService 
	{
		public string Message { get; set; }
		public string Subject { get; set; }
        public string ReplyTo { get; set; }

		public EmailService(string subject = null, string message = null)
		{
			if (subject != null)
			{
				this.Subject = subject;
			}
			if (message != null)
			{
				this.Message = message;
			}
		}

        public bool Send(string to, bool isHtml = false, string cc = null, string bcc = null,List<string> Attachments=null)
		{
			using (var smtp = new SmtpClient())
			{
                ///Some smtp do not require credentials
                if (ConfigurationManager.AppSettings["EmailUserName"] != null)
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["EmailUserName"].ToString(),
                        Password = ConfigurationManager.AppSettings["EmailPassword"].ToString()
                    };
                    smtp.Credentials = credential;
                }
                
				smtp.Host = ConfigurationManager.AppSettings["EmailHost"].ToString();
				smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
				smtp.EnableSsl = true;

				var message = new MailMessage();
				if (Globals.ISPRODUCTION)
				{
					foreach (var toAddress in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
					{
						message.To.Add(toAddress);
                    }

                    if (!string.IsNullOrWhiteSpace(cc))
                    {
                        foreach (var ccAddress in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.CC.Add(ccAddress);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(bcc))
                    {
                        foreach (var bccAddress in bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.Bcc.Add(bccAddress);
                        }
                    }
				}
				else
				{
                    ; 
                    //We use permitted suffixes to make sure that we do not accidentally email customers
                    //Loop through premitted suffixes then loop through emails.
                    string permittedEmailSuffix = ConfigurationManager.AppSettings["PermittedEmailSuffixes"];
                    foreach (var emailSuffix in permittedEmailSuffix.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        foreach (var toAddress in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (toAddress.EndsWith(emailSuffix,StringComparison.OrdinalIgnoreCase))
                            {
                                message.To.Add(toAddress);
                            }
                        }
                    }

                    //If the To is still empty, send the email to the admin.
                    if(message.To.Count == 0)
                    {
                        to = ConfigurationManager.AppSettings["AdminEmail"].ToString();
                        message.To.Add(to);
                    }
                    else
                    {
                        message.Bcc.Add(ConfigurationManager.AppSettings["AdminEmail"].ToString());
                    }
                    // need to decide how to send multiple emails in dev environment
                    Subject = "[" + ConfigurationManager.AppSettings["buildEnvironment"].ToString()  + "] " + Subject;
				}

				if (Attachments!=null)
				{
					foreach(string attachment in Attachments)
					{
						message.Attachments.Add(new Attachment(attachment));
					}
				}
					

				message.From = new MailAddress(ConfigurationManager.AppSettings["From"].ToString());
				message.Subject = this.Subject;
				message.Body = this.Message;
				message.IsBodyHtml = isHtml;
                if(!string.IsNullOrWhiteSpace(ReplyTo))
                {
                    message.ReplyToList.Add(new MailAddress(ReplyTo));
                }
				smtp.Send(message);

                //Send copy to ReplyTo
                if (!string.IsNullOrWhiteSpace(ReplyTo))
                {
                    message.To.Clear();
                    message.To.Add(new MailAddress(ReplyTo));
                    message.Subject = message.Subject + " - Copy";
                    smtp.Send(message);
                }

            }
            return true;
		}

		public Task SendAsync(IdentityMessage message)
		{
			var smtp = new SmtpClient();
			smtp.SendCompleted += (s, e) => {
				smtp.Dispose();
			};
            ///Some smtp do not require credentials
            if (ConfigurationManager.AppSettings["EmailUserName"] != null)
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["EmailUserName"].ToString(),
                    Password = ConfigurationManager.AppSettings["EmailPassword"].ToString()
                };
                smtp.Credentials = credential;
            }
			smtp.Host = ConfigurationManager.AppSettings["EmailHost"].ToString();
			smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
			smtp.EnableSsl = true;

			var asyncMessage = new MailMessage();
			if (Globals.ISPRODUCTION)
			{
				asyncMessage.To.Add(message.Destination);
                Subject = message.Subject;
			}
			else
			{
                //We use permitted suffixes to make sure that we do not accidentally email customers
                //Loop through premitted suffixes then loop through emails.
                string permittedEmailSuffix = ConfigurationManager.AppSettings["PermittedEmailSuffixes"];
                foreach (var emailSuffix in permittedEmailSuffix.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (message.Destination.EndsWith(emailSuffix))
                    {
                        asyncMessage.To.Add(message.Destination);
                    }
                }

                //If the To is still empty, send the email to the admin.
                if (asyncMessage.To.Count == 0)
                {
                    asyncMessage.To.Add(ConfigurationManager.AppSettings["AdminEmail"].ToString());
                }
                else
                {
                    asyncMessage.Bcc.Add(ConfigurationManager.AppSettings["AdminEmail"].ToString());
                }
                Subject = "[" + ConfigurationManager.AppSettings["buildEnvironment"].ToString() + "] " + message.Subject;
			}
			asyncMessage.From = new MailAddress(ConfigurationManager.AppSettings["From"].ToString());
			asyncMessage.Subject = Subject;
			asyncMessage.Body = message.Body;
			asyncMessage.IsBodyHtml = true;
            if (!string.IsNullOrWhiteSpace(ReplyTo))
            {
                asyncMessage.ReplyToList.Add(new MailAddress(ReplyTo));
            }
            return smtp.SendMailAsync(asyncMessage);
		}
	}
}