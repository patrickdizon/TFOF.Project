using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TFOF.Areas.Account.Helpers
{
    public class TimeZoneHelpers
    {
        internal static SelectList GetTimeZones()
        {
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            return new SelectList( timeZones.Select(tz => new SelectListItem
            {
                Text = tz.DisplayName,
                Value = tz.Id
            }), "Value", "Text");
        }
    }
}