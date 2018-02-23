using PrintServiceCore.Models;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;

namespace PrintServiceCore.Core.Services
{
    public class PrintServices : IDisposable
    {
        void IDisposable.Dispose()
        {
        }

        //PrintDAL PrintDal = new IPrintDAL();
        //Get the failed re-print retry count.
        private string _serviceName = Convert.ToString(ConfigurationManager.AppSettings["ServiceName"]);
        private string _userId = Convert.ToString(ConfigurationManager.AppSettings["UserId"]);
        private int _retryCount = Convert.ToInt32(ConfigurationManager.AppSettings["RetryCount"]);
        private string _localBase = Convert.ToString(ConfigurationManager.AppSettings["PrintServiceDirectory"]);
        private double _days = Convert.ToDouble(ConfigurationManager.AppSettings["DocumentRetentionDays"]);
        private bool _sendToPrinter = Convert.ToBoolean(ConfigurationManager.AppSettings["SendToPrinter"]);

        private BaseModelContext<PrintModel> printDb = new BaseModelContext<PrintModel>();
        private BaseModelContext<PrintLogModel> printLogDb = new BaseModelContext<PrintLogModel>();

        public void ProcessDocuments()
        {
            //Get the documents to be printed.
            List<PrintModel> printModels = printDb.Models
                .Where(a => (a.Status == PrintModel.queuedStatus || a.Status == PrintModel.failedStatus) && a.RetryCount < 5)
                .ToList();

            //Loop the document list and send the document to printer
            foreach (PrintModel printModel in printModels)
            {
                //Null out any previous errors
                printModel.ErrorMessage = null;
                try
                {
                    //Validate the file
                    if (string.IsNullOrEmpty(printModel.FileLocation) || !File.Exists(printModel.FileLocation.Trim()))
                    {
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = _serviceName;
                            eventLog.WriteEntry($"File '{printModel.FileLocation}' not found.", EventLogEntryType.Error, 101, 1);
                            printModel.ErrorMessage = $"File '{printModel.FileLocation}' not found.";
                        }
                    }
                    else
                    {
                        using (PdfDocument pdfDocument = new PdfDocument())
                        {
                            pdfDocument.LoadFromFile(printModel.FileLocation.Trim());
                            PrintDocument printDocument = pdfDocument.PrintDocument;
                            printDocument.PrinterSettings.PrinterName = printModel.Printer.Name.Trim();

                            //Validate the printer name
                            if (printModel.Printer == null || string.IsNullOrEmpty(printModel.Printer.Name) || !printDocument.PrinterSettings.IsValid)
                            {
                                //serviceResult.Error("No Printer information found!");
                                using (EventLog eventLog = new EventLog("Application"))
                                {
                                    eventLog.Source = _serviceName;
                                    eventLog.WriteEntry($"Printer '{printModel.Printer}' found!", EventLogEntryType.Error, 101);
                                    printModel.ErrorMessage = $"Printer '{printModel.Printer}' found!";
                                }
                            }
                            else
                            {
                                //Check out the paper source
                                if (printModel.Printer.PrinterTray != null)
                                {
                                    printDocument.DefaultPageSettings.PaperSource = printDocument.PrinterSettings.PaperSources[printModel.Printer.PrinterTray.Index];
                                }

                                //Check the config setting if we want to actually send to printer.
                                if (_sendToPrinter)
                                {
                                    printDocument.Print();
                                }

                                printModel.Status = PrintModel.printedStatus;

                            }

                            //Update the print status
                            if (!string.IsNullOrWhiteSpace(printModel.ErrorMessage))
                            {
                                printModel.Status = PrintModel.failedStatus;
                            }

                            printModel.RetryCount = (printModel.Status != PrintModel.printedStatus ? printModel.RetryCount + 1 : printModel.RetryCount);
                            printModel.PrintedDateTime = (printModel.Status.Equals(PrintModel.printedStatus) ? DateTime.Now : printModel.PrintedDateTime);
                        }
                    }
                }
                catch (Exception e)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = _serviceName;
                        eventLog.WriteEntry("Print service Exception " + e.Message, EventLogEntryType.Error, 101, 1);
                    }
                    printModel.Status = PrintModel.failedStatus;
                    printModel.RetryCount = (printModel.Status != PrintModel.printedStatus ? printModel.RetryCount + 1 : printModel.RetryCount);
                    printModel.ErrorMessage = "Print service Exception " + e.Message;
                }
                printDb.Entry(printModel).State = EntityState.Modified;
            }
            printDb.SaveChanges(_userId);
        }

        public void DeleteExpiredDocument()
        {
            DateTime expirationDate = DateTime.Today.AddDays(_days);

            List<PrintLogModel> printLogModels = printLogDb.Models
                .Where(p => p.PrintedDateTime != null && p.PrintedDateTime < expirationDate && p.Status != PrintModel.archivedStatus)
                .ToList();
            
            if (printLogModels != null)
            {
                foreach (PrintLogModel printLogModel in printLogModels)
                {
                    if (printLogModel.FileLocation.StartsWith(_localBase))
                    {
                        try
                        {
                            File.Delete(printLogModel.FileLocation);
                            printLogModel.Status = PrintModel.archivedStatus;
                            printLogModel.ErrorMessage = null;
                        }
                        catch (Exception e)
                        {
                            printLogModel.ErrorMessage = "Archive failed. " + e.Message;
                        }
                        printLogDb.Entry(printLogModel).State = EntityState.Modified;

                    }
                }
                printLogDb.SaveChanges(_userId);
            }
        }

        public void ArchiveReports()
        {
            var retryCountParam = new SqlParameter
            {
                ParameterName = "RetryCount",
                Value = _retryCount
            };
            List<PrintModel> printModels = printDb.Models.Where(p => p.Status == PrintModel.printedStatus || p.RetryCount >= _retryCount).ToList();
            foreach (PrintModel printModel in printModels)
            {
                PrintLogModel printLogModel = new PrintLogModel()
                {

                    PrintId = printModel.Id,
                    BatchId = printModel.BatchId,
                    EntityName = printModel.EntityName,
                    PrimaryKey = printModel.PrimaryKey,
                    FileLocation = printModel.FileLocation,
                    PrinterId = printModel.PrinterId,
                    PrintedDateTime = printModel.PrintedDateTime,
                    Status = printModel.Status,
                    RetryCount = printModel.RetryCount,
                    DeleteFile = printModel.DeleteFile,
                    ErrorMessage = printModel.ErrorMessage,
                    ModifiedById = _userId,
                    Modified = DateTime.Now,
                    CreatedById = printModel.CreatedById,
                    Created = printModel.Created
                };
                printLogDb.Models.Add(printLogModel);
                printDb.Models.Remove(printModel);
            }
            printLogDb.SaveChanges();
            printDb.SaveChanges(_userId);
        }
    }
}