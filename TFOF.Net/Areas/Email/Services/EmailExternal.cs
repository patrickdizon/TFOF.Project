using System;
using TFOF.Areas.Email.Models;
using TFOF.Areas.Core.Services;

namespace TFOF.Areas.Email.Services
{
	
	public class EmailExternal
	{
		
		public EmailExternal()
		{	}
		
		private bool EmailLog(int customerID,string to, bool isHtml = false, string cc = null, string bcc = null)
		{
			
			return true;
		}

		
		public bool SendEmailLease()
		{
			// To Do : Gather required information and populate the tables so that email notification service would pickup the email to send and log
			return true;
		}

	
		public bool SendWebsitePasswordEmail(int customerID, string emailAddress)
		{
			// To Do : Gather required information and call EmailLog the email
			return true;
		}
	}
}