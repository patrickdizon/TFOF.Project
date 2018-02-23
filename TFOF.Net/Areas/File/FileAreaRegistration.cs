using TFOF.Areas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFOF.Areas.File
{
    public class FileAreaRegistration : CoreAreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "File";
            }
        }
    }
}