

namespace TFOF.Areas.Core.Helpers
{
    using System.Web;
    using System.IO;
    using System;
    using System.Web.Mvc;
    using System.Security.Principal;

    public static class FileMethodExtension
    {
        public static bool HasFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
    }

    public class FileHelper
    {
        string _fileName;
        public string FileName {
            get
            {
                return _fileName;
            }
            set {
                _fileName = value;
                FilePathAndName = FilePath + value;
            }
        }
        public string Extention
        {
            get
            {
                return Path.GetExtension(FilePathAndName).Replace(".",string.Empty);
            }
        }
        public string Content { get; set; }
        public string FilePathAndName { get; set; }
        public string FilePath { get; set; }

        public FileHelper(string fileName, string userId)
        {
            
            if (fileName.Contains("{date}"))
            {
                fileName = fileName.Replace("{date}", DateTime.Now.ToString("yyyyMMdd"));
            }
            if (fileName.Contains("{datetime}"))
            {
                fileName = fileName.Replace("{datetime}", DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
            FilePath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\Temp\\" + userId + "\\";
            FileName = fileName;
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

        }

        public byte[] GetContent()
        {
            if (!Exists())
            {
                return null;
            }
            System.IO.FileStream fs = System.IO.File.OpenRead(FilePathAndName);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(FilePathAndName);

            fs.Close();
            return data;
        }

        public bool CopyTo(string newPath, bool impersonate = false)
        {
            try
            {
                File.Copy(FilePathAndName, newPath, true);
                if (Exists(newPath))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
           
        } 
        
        public FileHelper Write(string content)
        {
            // These examples assume a "C:\Users\Public\TestFolder" folder on your machine.
            // You can modify the path if necessary.

            Content = content;
            File.WriteAllText(FilePathAndName, Content);
            return this;
        }

        public FileHelper Write(byte[] content)
        {
            Content = Convert.ToBase64String(content);
            File.WriteAllBytes(FilePathAndName, content);
            return this;
        }

        public bool Exists(string filePath = null)
        {
            if(!string.IsNullOrWhiteSpace(filePath))
            {
                return File.Exists(filePath);
            }
            return File.Exists(FilePathAndName);
        }

        public bool Delete()
        {
            try
            {
                if (Exists())
                {
                    File.Delete(FilePathAndName);
                    return true;
                }
                return true;
            } catch  {
                throw new Exception($"An error occcured while trying to delete {FilePathAndName}.");
            }
        }

        public FileResult Download(HttpResponseBase response, bool isDeleteFile = true, bool isInline = false)
        {
            var content = GetContent();
            var action = "attachment";
            if(isDeleteFile)
            {
                Delete();
            }
            if(isInline)
            {
                action = "inline";
            }
            response.AddHeader("Content-Disposition", $"{action}; filename={FileName}");
            return new FileContentResult(content, "application/" + Extention);
        }
        
    }


}