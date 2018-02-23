using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFOF.Areas.Core.Attributes
{
    public class SSNFormatAttribute : Attribute
    {
        public SSNFormatAttribute(string ssn)
        {
            ssn = ssn.Trim().Substring(Math.Max(0, ssn.Length - 4));
        }
    }
}