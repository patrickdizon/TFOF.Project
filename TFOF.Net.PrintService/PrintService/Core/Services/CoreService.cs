using EmailServiceCore.Core.Services;
using PrintServiceCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintServiceCore.Core.Services
{
    public class CoreService
    {
        public class ServiceResult
        {
            public List<MessageModel> Messages = new List<MessageModel>();

            public ServiceResult Error(string message, string severity = null)
            {
                HasErrors = true;
                return SetMessage(MessageModel.Types.Error, message, severity);
            }

            public ServiceResult Info(string message, string severity = null)
            {
                return SetMessage(MessageModel.Types.Info, message, severity);
            }

            public ServiceResult Success(string message, string severity = null)
            {
                return SetMessage(MessageModel.Types.Success, message, severity);
            }

            public ServiceResult Warning(string message, string severity = null)
            {
                return SetMessage(MessageModel.Types.Warning, message, severity);
            }

            public ServiceResult Debug(string message, string severity = null)
            {
                return SetMessage(MessageModel.Types.Debug, message, severity);
            }

            public ServiceResult SetMessage(string type, string message, string severity = null)
            {
                Messages.Add(new MessageModel(type, message, severity));
                return this;
            }


            public bool HasErrors
            {
                get
                {
                    foreach (MessageModel message in Messages)
                    {
                        if (message.Type == MessageModel.Types.Error) return true;
                    }
                    return false;
                }
                private set { }
            }

            public bool HasWarnings
            {
                get
                {
                    foreach (MessageModel message in Messages)
                    {
                        if (message.Type == MessageModel.Types.Warning) return true;
                    }
                    return false;
                }
                private set { }
            }

            
            public ServiceResult EmailResults(List<string> emails, string subject)
            {
                //Email the requester.
                if (emails != null)
                {
                    EmailService emailService = new EmailService();
                    emailService.Subject = "PrintService: " + subject;

                    if (Messages.Count() > 1)
                    {
                        emailService.Message = "<p>Below are the results of your request: </p>";
                        foreach (var message in Messages.GroupBy(g => g.Type, (key, items) => new
                        {
                            Key = key,
                            Items = items
                        }))
                        {
                            emailService.Message += "<ul>";
                            foreach (MessageModel m in message.Items)
                            {
                                emailService.Message += "<li style=\"color: " + (m.Type == MessageModel.Types.Error ? "red" : "") + "\">" + m.Message + "</li>";
                            }
                            emailService.Message += "</ul>";
                        }
                    }
                    else
                    {
                        emailService.Message = Messages.First().Message;
                    }
                    emailService.Send(string.Join(";", emails), true);
                }

                return this;
            }

        }

        public class ServiceResult<T> : ServiceResult
        {

            public ServiceResult<T> Error(T model, string message, string severity = null)
            {
                return SetMessage(model, MessageModel.Types.Error, message, severity);
            }

            public ServiceResult<T> Info(T model, string message, string severity = null)
            {
                return SetMessage(model, MessageModel.Types.Info, message, severity);
            }

            public ServiceResult<T> Success(T model, string message, string severity = null)
            {
                return SetMessage(model, MessageModel.Types.Success, message, severity);
            }

            public ServiceResult<T> Warning(T model, string message, string severity = null)
            {
                return SetMessage(model, MessageModel.Types.Warning, message, severity);
            }

            public ServiceResult<T> Debug(T model, string message, string severity = null)
            {
                return SetMessage(model, MessageModel.Types.Debug, message, severity);
            }

            public ServiceResult<T> SetMessage(T model, string type, string message, string severity = null)
            {
                Messages.Add(new MessageModel(model, type, message, severity));
                return this;
            }

        }
    }
}
