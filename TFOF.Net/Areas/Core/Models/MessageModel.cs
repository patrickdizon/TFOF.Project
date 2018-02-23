using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFOF.Areas.Core.Models
{
    public class MessageModel
    {
        public string Type { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public object Model { get; set; }

        public MessageModel(string type, string message, string severity = null)
        {
            Type = type;
            Message = message;
            Severity = severity;
        }
        public MessageModel(object model, string type, string message, string severity = null)
        {
            Model = model;
            Type = type;
            Message = message;
            Severity = severity;
        }

        public class Types
        {
            public static string Error = "Error";
            public static string Info = "Info";
            public static string Success = "Success";
            public static string Warning = "Warning";
            public static string Debug = "Debug";
        }

    }
    
}