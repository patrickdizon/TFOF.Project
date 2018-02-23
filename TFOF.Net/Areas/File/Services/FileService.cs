

namespace TFOF.Areas.File.Services
{
    using TFOF.Areas.File.Models;
    using Core.Models;
    using System.Linq;
    using System;
    using System.Diagnostics;
    using System.Web;
    using Core.Services;

    public class FileService : CoreService
    {
        public ServiceResult ProcessFileAction(string userId, string url, Int64? Id)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (BaseModelContext<TFOF.Areas.File.Models.FileModel> fileModeldb = new BaseModelContext<TFOF.Areas.File.Models.FileModel>())
            {
                var fileinfo = fileModeldb.Models.Find(Id);
                if (fileinfo != null)
                {
                    if (fileinfo.Action.ToUpper() == "EXPORT")
                    {
                        if (System.IO.Directory.Exists(fileinfo.FileLocation))
                        {
                            string argument = "/select, \"" + fileinfo.FileLocation + @"\" + fileinfo.FileName + "\"";
                            Process.Start("explorer.exe", argument);
                        }
                        else
                        {
                            serviceResult.Error("File not found!");
                        }
                    }
                    else if (fileinfo.Action.ToUpper() == "IMPORT")
                    {
                        if (System.IO.Directory.Exists(fileinfo.FileLocation))
                        {
                            //Download the file
                            System.IO.FileInfo file = new System.IO.FileInfo(fileinfo.FileLocation + @"\" + fileinfo.FileName);
                            // prepare the response to the client. resp is the client Response
                            var Response = HttpContext.Current.Response;
                            Response.Clear();
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                            Response.AddHeader("Content-Length", file.Length.ToString());
                            Response.WriteFile(file.FullName);
                            Response.End();

                            serviceResult.Success("File downloaded successfully!");
                        }
                        else
                        {
                            serviceResult.Error("File path not found!");

                        }
                    }
                    else if (fileinfo.Action.ToUpper() == "PRINT")
                    {
                        //using (BaseModelContext<PrintModel> printModeldb = new BaseModelContext<PrintModel>())
                        //{

                        //    PrintModel printModel = new PrintModel();
                        //    printModel.EntityName = "RevenueShareMYOBExport";
                        //    printModel.PrimaryKey = "MYOBExport";// need to check with pd
                        //    printModel.FileLocation = fileinfo.FileLocation + @"\" + fileinfo.FileName;
                        //    printModel.Printer = "local";// need to check with pd
                        //    printModel.Status = "Queued";
                        //    printModel.DeleteFile = true;
                        //    printModel.CreatedById = userId;
                        //    printModel.Created = DateTime.Now.Date;
                        //    printModeldb.Models.Add(printModel);
                        //    printModeldb.SaveChanges();
                        //    serviceResult.Success("Document has been added to print queue.");
                        //}
                        //return serviceResult;
                    }

                }
                else
                {
                    serviceResult.Error("File not found!");
                }
            }
            return serviceResult;

        }

    }
    public static class FileDirectoryService
    {

        public static string GetPathByName(string name)
        {

            FileDirectoryModel fileDirectory = new BaseModelContext<FileDirectoryModel>().Models.Where(n => n.Name == name).FirstOrDefault();
            if (fileDirectory != null)
            {
                return fileDirectory.Path;
            }

            throw new ArgumentNullException($"Could not find {name} in File Directories");
        }
    }
}