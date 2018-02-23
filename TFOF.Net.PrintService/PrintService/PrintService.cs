using PrintServiceCore.Core.Services;
using HealthCheckServiceCore.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PrintServiceCore.Core.Services.CoreService;

namespace PrintService
{
    public partial class PrintService : ServiceBase
    {

        //ServiceResult serviceResult;

        private Task _proccessQueueTask;
        private Task _proccessHealthCheckTask;
        private CancellationTokenSource _cancellationTokenSource;
        private string _serviceName = Convert.ToString(ConfigurationManager.AppSettings["ServiceName"]);
        private int _healthCheckInterval = Convert.ToInt32(ConfigurationManager.AppSettings["HealthCheckInterval"]);
        private string _toEmailAddress = Convert.ToString(ConfigurationManager.AppSettings["AdminEmail"]);

        PrintServices printService;
        HealthCheckServices healthCheckService;
        ServiceResult serviceResult;

        public PrintService()
        {
            InitializeComponent();
            //serviceResult = new ServiceResult();
            printService = new PrintServices();
            healthCheckService = new HealthCheckServices();
            serviceResult = new ServiceResult();

        }

        protected override void OnStart(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken _cts = _cancellationTokenSource.Token;
            _proccessQueueTask = Task.Run(() => DoWorkAsync(_cts));
            _proccessHealthCheckTask = Task.Run(() => DoHealthChecksAsync(_cts));
        }
        

        public async Task DoWorkAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    printService.ProcessDocuments();
                    printService.DeleteExpiredDocument();
                    printService.ArchiveReports();
                }
                catch (Exception e)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = _serviceName;
                        eventLog.WriteEntry("PrintService Exception " + e.Message, EventLogEntryType.Error, 101);
                        serviceResult.Error(e.ToString());
                        serviceResult.EmailResults(new List<String> { _toEmailAddress }, "PrintService Exception " + e.Message);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), token);
            }
        }

        public async Task DoHealthChecksAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    healthCheckService.Check();
                    
                }
                catch (Exception e)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {   
                        eventLog.Source = _serviceName;
                        eventLog.WriteEntry("Health Check Service Exception " + e.Message, EventLogEntryType.Error, 102);
                        serviceResult.Error(e.ToString());
                        serviceResult.EmailResults(new List<String> { _toEmailAddress }, "Health Check Service Exception " + e.Message);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(_healthCheckInterval), token);
            }
        }
        protected override void OnStop()
        {
            //serviceResult.SetMessage(MessageModel.Types.Debug, "Print service has been stoped.");
            _cancellationTokenSource.Cancel();
            try
            {
                _proccessQueueTask.Wait();
                _proccessHealthCheckTask.Wait();
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = _serviceName;
                    eventLog.WriteEntry("OnStop Exception " + e.Message, EventLogEntryType.Error, 101, 1);
                }
            }

        }

        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
