using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFOF.Areas.Core.Attributes
{
    /// <summary>
    /// Set IsModified to false when SaveChanges is called in BaseModelContext
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute
    {
    }
}