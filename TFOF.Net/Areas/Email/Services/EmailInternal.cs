using System;
using TFOF.Areas.Email.Models;
using TFOF.Areas.Core.Services;
using System.Collections.Generic;

namespace TFOF.Areas.Email.Services
{
	
	public class EmailInternal
	{
		public string message;
		public string subject;

		public EmailInternal(string subject = null, string message = null)
		{
			if (subject != null)
			{
				this.subject = subject;
			}
			if (message != null)
			{
				this.message = message;
			}
		}
		
		public bool Send(string to, bool isHtml = false, string cc = null, string bcc = null,List<string> Attachments=null,string userId=null)
		{
			Core.Services.EmailService emailService = new Core.Services.EmailService();
			emailService.Subject = this.subject;
			emailService.Message = this.message;
			emailService.Send(to, isHtml, cc, bcc,Attachments);
			// log to emailLogInternal table
			EmailModelContext db = new EmailModelContext();
			EmailInternalLogModel emailLog = new EmailInternalLogModel
			{
				SentTo = to,
				SentCC = cc,
				SentBCC = bcc,
				SentDate = DateTime.Now,
				Subject = subject,
				Body = message
			};
			db.EmailInternalLogs.Add(emailLog);
			if (string.IsNullOrEmpty(userId))
				db.SaveChanges();
			else
				db.SaveChanges(userId);
			return true;
		}
	}
}