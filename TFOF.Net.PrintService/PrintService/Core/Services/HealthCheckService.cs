using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using static PrintServiceCore.Core.Services.CoreService;

namespace HealthCheckServiceCore.Core.Services
{
    public class HealthCheckServices : IDisposable
    {
        void IDisposable.Dispose()
        {
        }

        ServiceResult serviceResult = new ServiceResult();
        private string _serviceName = Convert.ToString(ConfigurationManager.AppSettings["ServiceName"]);
        private string _url = Convert.ToString(ConfigurationManager.AppSettings["URLToCheck"]);
        private string _toEmailAddress = Convert.ToString(ConfigurationManager.AppSettings["AdminEmail"]);

        public void Check()
        {
            WebRequest request = WebRequest.Create(_url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = _serviceName;
                    eventLog.WriteEntry($"Health check: '{_url}' could not be reached.", EventLogEntryType.Error, 102);
                    serviceResult.Error($"Health check: '{_url}' could not be reached.");
                    serviceResult.EmailResults(new List<String> { _toEmailAddress }, $"Health Check Error: {_url}");
                }
                
            }
            using(EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = _serviceName;
                eventLog.WriteEntry($"Health check: '{_url}' is up.", EventLogEntryType.Information, 102);

            }

        }

    }
}