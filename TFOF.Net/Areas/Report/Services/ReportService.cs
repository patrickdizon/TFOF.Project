using System.Configuration;
using System.Web.UI.WebControls;
using System.Data;

using TFOF.Areas.Core.Services;
using TFOF.Areas.Report.Models;
using System;
using TFOF.Areas.Core.Helpers;
using TFOF.Areas.Core.Models;
using System.Net;
using TFOF.Areas.Core.Forms;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using TFOF.Areas.Core.Attributes;
using System.Web.Security;
using TFOF.Areas.User.Models;
using System.Web;

namespace TFOF.Areas.Report.Services
{
    public class ReportService
    {
        //ReportViewer
        static string reportServerPathViewer = ConfigurationManager.AppSettings["MvcReportViewer.ReportServerUrl"];
        //ReportRunner
        static string reportServerPathRunner = ConfigurationManager.AppSettings["ReportServerPath"];

        //ReportServer Credentials
        static string domain = ConfigurationManager.AppSettings["ReportServerDomain"];
        static string userName = ConfigurationManager.AppSettings["MvcReportViewer.Username"];
        static string password = ConfigurationManager.AppSettings["MvcReportViewer.Password"];

        public static ReportModel GetReport(string SSRSReportName, IPrincipal User)
        {
            return GetReports(new BaseModelContext<ReportModel>().Models.Where(r => r.SSRSReportName == SSRSReportName).FirstOrDefault(), null, User).FirstOrDefault();
        }

        public static ReportModel GetReport(ReportModel reportModel, IPrincipal User)
        {
            return GetReports(reportModel, null, User).FirstOrDefault();

        }

        public static ReportModel GetReport(int id, IPrincipal User)
        {
            return GetReports(new BaseModelContext<ReportModel>().Models.Find(id), null, User).FirstOrDefault();

        }

        public static List<ReportModel> GetReports(string category, IPrincipal User)
        {
            return GetReports(null, category, User);
        }

        public static List<ReportModel> GetReports(ReportModel reportModel, string category, IPrincipal User)
        {
            BaseModelContext<ReportModel> reportDb = new BaseModelContext<ReportModel>();
            List<ReportModel> reports;
            //Get list of active reports
            if (!User.IsInRole(SiteRole.Administrators))
            {
                reports = reportDb.Models.Where(r => r.IsActive == true).ToList();
            }
            else
            {
                reports = reportDb.Models.ToList();
            }
            //Reduce to category if category is specified
            if (!string.IsNullOrWhiteSpace(category)) {
                reports = reports.Where(r => r.Category == category).ToList(); 
            }

            //Reduce to a specific report if specified
            if(reportModel != null)
            {
                reports = reports.Where(r => r.Id == reportModel.Id).ToList();
            }

            //If Admin return all report/s
            if (User.IsInRole(SiteRole.Administrators))
            {
                return reports;
            }
            List<ReportModel> userReports = new List<ReportModel>();

            //Add report/s for everyone
            userReports = userReports.Concat(
                reports.Where(r => r.Permit == ReportModel.permitEveryone)
            ).ToList();
            
            //Gather roleIds of user
            string userId = User.Identity.GetUserId();
            BaseModelContext<UserModel> userDb = new BaseModelContext<UserModel>();
            UserModel userModel = userDb.Models.Find(userId);

            //UserRoles are not preloaded. So load them.
            userDb.Entry(userModel).Collection("UserRoles").Load();

            if (userModel != null) {
                List<string> roleIds = userModel.UserRoles.Select(r => r.RoleId).ToList();

                //Add reports matching user roles
                userReports = userReports.Concat(
                    reports.Where(r => r.Permit == ReportModel.permitUserOrRole).SelectMany(r => r.ReportRoles.Where(e => roleIds.Contains(e.RoleId))).Select( r => r.Report)
                ).ToList();


                //Add reprots matching user
                userReports = userReports.Concat(
                    reports.Where(r => r.Permit == ReportModel.permitUserOrRole && r.ReportUsers.Select(u => u.UserId).Contains(userId))
                ).ToList();

            }

            //Return Unique Reports.
            return new HashSet<ReportModel>(userReports).ToList();
        } 

        

        /// <summary>
        /// Run reports is used for scheduled reports.
        /// </summary>
        /// <param name="reportModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static FileHelper Run(ReportModel reportModel, string userId)
        {
            
            string URL = reportServerPathRunner + reportModel.SSRSReportName.Replace(".rdl", "");
            string Command = "Render";
            string Query = "";

            FileHelper file = new FileHelper((!string.IsNullOrWhiteSpace(reportModel.FileName) ? reportModel.FileName : reportModel.SSRSReportName.Replace(".rdl", "")), userId);

            //Replace filename parameters.
            foreach (ReportParameterModel reportParameter in reportModel.ReportParameters)
            {
                //Setup the query
                if (!string.IsNullOrWhiteSpace(reportParameter.DefaultValue))
                {
                    Query += $"&{reportParameter.Name}={reportParameter.DefaultValue}";
                }
                //Replace any parameter in the name.
                if (file.FileName.Contains("{" + reportParameter.Name + "}"))
                {
                    string defaultValue = reportParameter.DefaultValue;
                    //If the parameter is a date make sure we format it so its fiel friendly.
                    if (!string.IsNullOrWhiteSpace(reportParameter.DataType) && reportParameter.DataType.Equals(ReportParameterModel.dateTimeType))
                    {
                        DateTime dateTimeValue;
                        DateTime.TryParse(reportParameter.DefaultValue, out dateTimeValue);
                        defaultValue = (dateTimeValue != null ? dateTimeValue.ToString("yyyyMMdd") : "");
                    }
                    file.FileName = file.FileName.Replace("{" + reportParameter.Name + "}", defaultValue);
                }
            }
            file.FileName = Regex.Replace(file.FileName, "-{2,}", "-");
            file.FileName = file.FileName + "." + ReportModel.Extensions[reportModel.Format];
            //Delete existing file
            if (file.Delete())
            {

                URL = URL + "&rs:Command=" + Command + "&rs:Format=" + reportModel.Format + Query;

                System.Net.HttpWebRequest Req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                NetworkCredential credential = new NetworkCredential(userName, password, domain);

                Req.Credentials = credential;
                Req.Method = "GET";
                //Specify the path for saving.

                //Do File Service here and return FileService fileService = new FileService();

                //file.FilePathAndName = string.Format("{0}{1}{2}", file.FilePath, file.FileName, @".pdf");
                System.Net.WebResponse objResponse = Req.GetResponse();
                System.IO.FileStream fs = new System.IO.FileStream(file.FilePathAndName, System.IO.FileMode.Create);
                System.IO.Stream stream = objResponse.GetResponseStream();

                byte[] buf = new byte[1024];
                int len = stream.Read(buf, 0, 1024);
                while (len > 0)
                {
                    fs.Write(buf, 0, len);
                    len = stream.Read(buf, 0, 1024);
                }
                stream.Close();
                fs.Close();

                BaseModelContext<ReportUserActivityLogModel> reportUserActivityLogDb = new BaseModelContext<ReportUserActivityLogModel>();
                ReportUserActivityLogModel reportUserActivityModel = new ReportUserActivityLogModel()
                {
                    ReportId = reportModel.Id,
                    UserId = userId,
                    Activity = ReportUserActivityLogModel.RunActivity,
                    Parameters = Query
                };
                reportUserActivityLogDb.Models.Add(reportUserActivityModel);
                reportUserActivityLogDb.SaveChanges(userId);


                BaseModelContext<ReportModel> reportDb = new BaseModelContext<ReportModel>();
                reportModel.LastRun = DateTime.Now;
                reportDb.Entry(reportModel);
                reportDb.SaveChanges(userId);
            }
            return file;

        }

        public static Form UserParameterForm(ReportModel reportModel, UrlHelper url)
        {
            Form userParameterForm = new Form();
            userParameterForm.FormAttributes.Add("Name", "RunForm");
            userParameterForm.FormAttributes.Add("Id", "RunForm");
            userParameterForm.FormTitle = reportModel.Name;
            userParameterForm.PostAPIUrl = url.Action("Run", "Report", new { area = "Report" });
            userParameterForm.Fields.Add(new IdField() { Name = "Id", Value = reportModel.Id.ToString() });
            foreach (ReportParameterModel reportParameterModel in reportModel.ReportParameters)
            {
                if (string.IsNullOrWhiteSpace(reportParameterModel.DataType)) reportParameterModel.DataType = ReportParameterModel.stringType;
                if (reportParameterModel.DataType.Equals(ReportParameterModel.stringType))
                {
                    userParameterForm.Fields.Add(new CharField()
                    {
                        Name = reportParameterModel.Name,
                        Label = (string.IsNullOrWhiteSpace(reportParameterModel.Label) ? TextService.Title(reportParameterModel.Name) : reportParameterModel.Label),
                        Value = reportParameterModel.DefaultValue,
                        MaxLength = 1000,
                        HelpText = reportParameterModel.Description
                    });
                }

                if (reportParameterModel.DataType.Equals(ReportParameterModel.dateTimeType))
                {
                    userParameterForm.Fields.Add(new DateTimeField()
                    {
                        Name = reportParameterModel.Name,
                        Label = (string.IsNullOrWhiteSpace(reportParameterModel.Label) ? TextService.Title(reportParameterModel.Name) : reportParameterModel.Label),
                        Value = reportParameterModel.DefaultValue,
                        HelpText = reportParameterModel.Description
                    });
                }

                if (reportParameterModel.DataType.Equals(ReportParameterModel.boolType))
                {
                    bool output = false;
                    bool.TryParse(reportParameterModel.DefaultValue, out output);
                    userParameterForm.Fields.Add(new BooleanField()
                    {
                        Name = reportParameterModel.Name,
                        Label = (string.IsNullOrWhiteSpace(reportParameterModel.Label) ? TextService.Title(reportParameterModel.Name) : reportParameterModel.Label),
                        Value = output,
                        HelpText = reportParameterModel.Description
                    });
                }

                if (reportParameterModel.DataType.Equals(ReportParameterModel.intType))
                {
                    userParameterForm.Fields.Add(new IntegerField()
                    {
                        Name = reportParameterModel.Name,
                        Label = (string.IsNullOrWhiteSpace(reportParameterModel.Label) ? TextService.Title(reportParameterModel.Name) : reportParameterModel.Label),
                        Value = reportParameterModel.DefaultValue,
                        HelpText = reportParameterModel.Description
                    });
                }


            }
            userParameterForm.Fields.Add(new CharField()
            {
                Name = "ReportFormat",
                Label = "Save As...",
                Value = reportModel.Format,
                Options = new SelectList(ReportModel.Formats, "Value", "Text", "Group.Name", reportModel.Format, null)
            });

            userParameterForm.SaveButtonText = null;

            return userParameterForm;
        }
        [Obsolete]
        public static ReportViewerModel ViewReport(ReportModel reportModel, string userId)
        {
            return View(reportModel, userId);
        }

        /// <summary>
        /// View Report is a viewer for the user.
        /// </summary>
        /// <param name="reportModel"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ReportViewerModel View(ReportModel reportModel, string userId)
        {
            if (reportModel!=null)
            {
                var reportviewerModel = new ReportViewerModel
                {
                    Report = reportModel,
                    ReportPath = "/ReportService/" + reportModel.SSRSReportName.Replace(".rdl", ""),
                    Parameters = null,
                    ServerUrl = reportServerPathViewer,
                    UserName = userName,
                    Password = password
                };

                BaseModelContext<ReportUserActivityLogModel> reportUserActivityLogDb = new BaseModelContext<ReportUserActivityLogModel>();
                ReportUserActivityLogModel reportUserActivityModel = new ReportUserActivityLogModel()
                {
                    ReportId = reportModel.Id,
                    UserId = userId,
                    Activity = ReportUserActivityLogModel.ViewActivity
                };
                reportUserActivityLogDb.Models.Add(reportUserActivityModel);
                reportUserActivityLogDb.SaveChanges(userId);

                return reportviewerModel;
            }

            return null;
        }
    }
}