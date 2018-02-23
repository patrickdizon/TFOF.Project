using TFOF.Areas.Core.Helpers;
using TFOF.Areas.Core.Models;
using TFOF.Areas.Core.Services;
using TFOF.Areas.File.Services;
using TFOF.Areas.Print.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Web.Mvc;

namespace TFOF.Areas.Print.Services
{
    public class PrintService : CoreService
    {

        public static bool AddJob(FileHelper fileModel, string entityName, string primaryKey, int printerId, string userId)
        {
            string printDirectory = FileDirectoryService.GetPathByName("PrintServiceDirectory");
            BaseModelContext<PrintModel> printDb = new BaseModelContext<PrintModel>();
            PrintModel printModel = new PrintModel()
            {
                EntityName = entityName,
                PrimaryKey = primaryKey,
                PrinterId = printerId,
                FileLocation = "C:\\Temp", 
                Status = PrintModel.newStatus
            };
            printDb.Models.Add(printModel);
            printDb.SaveChanges(userId);
            if (printModel.Id > 0) {
                string printServicePath = printDirectory + printModel.Id.ToString() + "-" + fileModel.FileName;
                if (fileModel.CopyTo(printServicePath))
                {
                    printModel.Status = PrintModel.queuedStatus;
                    printModel.FileLocation = printServicePath;
                    printDb.Entry(printModel);
                    printDb.SaveChanges(userId);
                    return true;
                } else
                {
                    printDb.Models.Remove(printModel);
                    printDb.SaveChanges(userId);
                }
            }
            return false;
        }

        public static void GetPrinterTrays(PrinterModel printerModel)
        {
            if (string.IsNullOrWhiteSpace(printerModel.Name)) return;

            BaseModelContext<PrinterTrayModel> printerTrayDb = new BaseModelContext<PrinterTrayModel>();
            PrintDocument printDocument = new PrintDocument();
            List<SelectListItem> trayOptions = new List<SelectListItem>();
            printDocument.PrinterSettings.PrinterName = printerModel.Name.Trim();
            Dictionary<int, string> paperSources = new Dictionary<int, string>();

            if (printDocument.PrinterSettings.IsValid)
            {
                //Get Trays for this printer
                List<PrinterTrayModel> printerTrays = printerTrayDb.Models.Where(t => t.PrinterId == printerModel.Id).ToList();
                //Add printer trays
                int i = 0;
                foreach (PaperSource paperSource in printDocument.PrinterSettings.PaperSources)
                {
                    paperSources[i] = paperSource.SourceName;
                    PrinterTrayModel exists = printerTrays.Where(t => t.Index == i && t.Name == paperSource.SourceName).FirstOrDefault();
                    if(exists == null)
                    {  
                        printerTrayDb.Models.Add(new PrinterTrayModel() {
                            PrinterId = printerModel.Id,
                            Index = i,
                            Name = paperSource.SourceName
                        });
                    }
                    i++;
                }
                //Remove Printer Trays
                foreach (PrinterTrayModel printerTrayModel in printerTrays)
                {
                    if(!(paperSources.ContainsKey(printerTrayModel.Index) && paperSources[printerTrayModel.Index].Equals(printerTrayModel.Name)))
                    {
                        printerTrayDb.Entry(printerTrayModel);
                        printerTrayDb.Models.Remove(printerTrayModel);
                    }
                }
            }
            printerTrayDb.SaveChanges();
        }

        public ServiceResult PrintDocument(string userId, int Id)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (BaseModelContext<PrintModel> printModeldb = new BaseModelContext<PrintModel>())
            {
                //find the record
                var unPrintedDocument = printModeldb.Models.Find(Id);
                PrintModel printModel = new PrintModel();
                printModel.EntityName = unPrintedDocument.EntityName;
                printModel.PrimaryKey = unPrintedDocument.PrimaryKey;
                printModel.FileLocation = unPrintedDocument.FileLocation;
                printModel.PrinterId = unPrintedDocument.PrinterId;
                //printModel.PrintedDateTime = "";
                printModel.Status = "Queued";
                printModel.DeleteFile = unPrintedDocument.DeleteFile;
                printModel.CreatedById = userId;
                printModel.Created = DateTime.Now.Date;
                printModeldb.Models.Add(printModel);
                printModeldb.SaveChanges();
                serviceResult.Success(unPrintedDocument.EntityName + " document has been added to print queue.");
            }
            return serviceResult;
        }

        
        //public ServiceResult processDocument(string userId, int Id)
        //{
        //    string printStatus = string.Empty;
        //    ServiceResult serviceResult = new ServiceResult();
        //    using (PrintDocument printDoc = new PrintDocument())
        //    {
        //        BaseModelContext<PrintModel> printModeldb = new BaseModelContext<PrintModel>();

        //        //find the record
        //        var unPrintedDocument = printModeldb.Models.Find(Id);

        //        //Validate the printer name
        //        if (unPrintedDocument.Printer == null || string.IsNullOrEmpty(unPrintedDocument.Printer.Name))
        //        {
        //            serviceResult.Error("No Printer information found!");
        //        }
        //        else
        //        {
        //            //Set the printer name and ensure it is valid. If not, provide a message to the user.
        //            printDoc.PrinterSettings.PrinterName = "\\" + unPrintedDocument.Printer.Name.Trim();

        //            //Validate the printer
        //            if (!printDoc.PrinterSettings.IsValid)
        //            {
        //                serviceResult.Error("No Printer found!");
        //            }
        //            else
        //            {
        //                //Validate the file path
        //                if (!string.IsNullOrEmpty(unPrintedDocument.FileLocation)) { serviceResult.Error("No file information found!"); }
        //                else
        //                {
        //                    //Validate the file
        //                    if (!System.IO.File.Exists(unPrintedDocument.FileLocation.Trim())) { serviceResult.Error("No file found!"); }
        //                    else
        //                    {
        //                        using (PdfDocument printdoc = new PdfDocument())
        //                        {
        //                            printdoc.LoadFromFile(unPrintedDocument.FileLocation.Trim());
        //                            printdoc.PrintDocument.Print();
        //                            serviceResult.Success("Printing process completed!");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (serviceResult.HasErrors) { printStatus = "Failed"; } else { printStatus = "Printed"; }
        //        unPrintedDocument.Status = printStatus;
        //        unPrintedDocument.Modified = DateTime.Now.Date;
        //        unPrintedDocument.ModifiedById = userId;
        //        printModeldb.Entry(unPrintedDocument).State = EntityState.Modified;
        //        printModeldb.SaveChanges();
        //        return serviceResult;
        //    }
        //}
    }
}