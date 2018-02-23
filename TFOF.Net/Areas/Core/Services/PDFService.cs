namespace TFOF.Areas.Core.Services
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    public class PDFService
    {
        /// <summary>
        /// Convert Html page at a given URL to a PDF file using open-source tool wkhtml2pdf
        ///   wkhtml2pdf can be found at: http://code.google.com/p/wkhtmltopdf/
        ///   Useful code used in the creation of this I love the good folk of StackOverflow!: http://stackoverflow.com/questions/1331926/calling-wkhtmltopdf-to-generate-pdf-from-html/1698839
        ///   An online manual can be found here: http://madalgo.au.dk/~jakobt/wkhtmltoxdoc/wkhtmltopdf-0.9.9-doc.html
        ///   
        /// Ensure that the output folder specified is writeable by the ASP.NET process of IIS running on your server
        /// 
        /// This code requires that the Windows installer is installed on the relevant server / client.  This can either be found at:
        ///   http://code.google.com/p/wkhtmltopdf/downloads/list - download wkhtmltopdf-0.9.9-installer.exe
        /// </summary>
        /// <param name="pdfOutputLocation"></param>
        /// <param name="outputFilenamePrefix"></param>
        /// <param name="urls"></param>
        /// <param name="options"></param>
        /// <param name="pdfHtmlToPdfExePath"></param>
        /// <returns>the URL of the generated PDF</returns>
        public PDFFileModel HtmlToPdf(string outputFilenamePrefix, string content, string userId, string[] options = null)
        {
            string outputFilenamePDF = outputFilenamePrefix + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".PDF"; // assemble destination PDF file name
            string outputFilenameHTML = outputFilenamePrefix + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff") + ".HTML"; // assemble destination PDF file name

            FileHelper fileModel = new FileHelper(outputFilenameHTML, userId);
            fileModel.Write(content);
            
            List<string> finalOptions = new List<string>();
            if (options != null)
            {
                foreach (string opt in options)
                {
                    finalOptions.Add(opt);
                }
            }
            //Encapsulate spaces in directory structures
            finalOptions.Add("\"" + fileModel.FilePathAndName + "\"");
            try
            {
                
                var p = new System.Diagnostics.Process()
                {
                    StartInfo = {
                        FileName = ConfigurationManager.AppSettings["WkHtmlToPDFPath"].ToString(),
                        Arguments = ((finalOptions == null) ? "" : String.Join(" ", finalOptions)) + " "  + outputFilenamePDF,
                        UseShellExecute = false, // needs to be false in order to redirect output
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true, // redirect all 3, as it should be all 3 or none
                        WorkingDirectory = fileModel.FilePath
                    }
                };

                p.Start();

                // read the output here...
                var output = p.StandardOutput.ReadToEnd();
                var errorOutput = p.StandardError.ReadToEnd();

                // ...then wait n milliseconds for exit (as after exit, it can't read the output)
                p.WaitForExit(60000);

                // read the exit code, close process
                int returnCode = p.ExitCode;
                p.Close();

                // if 0 or 2, it worked so return path of pdf
                if ((returnCode == 0) || (returnCode == 2))
                    return new PDFFileModel(outputFilenamePDF, userId);
                else
                    throw new Exception(errorOutput);
            }
            catch (Exception exc)
            {
                throw new Exception("Problem generating PDF from HTML, outputFilename: " + outputFilenamePDF, exc);
            }
        }
    }

    public class PDFFileModel : FileHelper
    {
        public PDFFileModel(string fileName, string userId) : base(fileName, userId)
        {

        }
    }
}
