namespace TFOF.Areas.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class AttributesExtensions
    {
        public static RouteValueDictionary AdditionalAttributes(
            this object htmlAttributes,
            Dictionary<string, string> newAttributes,
            bool disabled,
            bool required
            
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (newAttributes != null && newAttributes.Count() > 0)
            {
                foreach (var attr in newAttributes)
                {
                    attributes[attr.Key.ToString()] = attr.Value.ToString();
                }
            }
            if (disabled)
            {
                attributes["disabled"] = "disabled";
            }
            if (required)
            {
                attributes["required"] = "required";
            }
            return attributes;

        }
        
        public static RouteValueDictionary Disabled(
            this object htmlAttributes,
            bool disabled
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (disabled)
            {
                attributes["disabled"] = "disabled";
            }
            return attributes;

        }

        public static RouteValueDictionary Required(
            this object htmlAttributes,
            bool required
        )
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (required)
            {
                attributes["required"] = "required";
            }
            return attributes;

        }

        public static string AttributesToString(
            this object htmlAttributes,
            Dictionary<string, string> newAttributes)
        {
           List<String> attributeList =  new List<string>();
            foreach (var attribute in newAttributes) {
                attributeList.Add(attribute.Key.ToString() + "=" + attribute.Value + "");
           }
            return String.Join(" ", attributeList);
        } 
    }

    public static class PrincipalExtensions
    {
        public static bool IsInAllRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.All(r => principal.IsInRole(r));
        }

        public static bool IsInAnyRoles(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(r => principal.IsInRole(r));
        }
    }

    public static class RazorHelper
    {
        public static string Render(object ViewModel, ControllerContext controller, string path)
        {


            var sb = new StringWriter();
            ViewDataDictionary ViewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();
            ViewData.Model = ViewModel;
            var razor = new RazorView(controller, path, null, false, null);
            var viewContext = new ViewContext(controller, razor, ViewData, tempData, sb);
            razor.Render(viewContext, sb);
            return sb.ToString();


        }
    }

    public static class BrowserHelper
    {
        public static bool IsMobile(HttpRequestBase request)
        {
            if (request.Browser.IsMobileDevice || request.UserAgent.Contains("iPhone") || request.UserAgent.Contains("Windows Phone") || request.UserAgent.Contains("Android"))
            {
                return true;
            }
            return false;
        }
    }

    public static class Mask
    {
        public static string MaskRight(this string value, int length, char mask = '*')
        {
            if (value.Length >= length)
            {
                string left = value.Substring(0, length);
                return  left + (new String(mask, value.Length - length));
            }
            return value;

        }

        public static string MaskLeft(this string value, int length, char mask = '*')
        {
            if (value.Length >= length)
            {
                string right = value.Substring(Math.Max(0, value.Length - length)); 
                return (new String(mask, value.Length - length)) + right;
            }
            return value;
        }
    }

    //http://www.blackbeltcoder.com/Articles/strings/converting-text-to-html
    public static class StringMethodExtensions
    {
        private static string _paraBreak = "\n";
        private static string _link = "<a href=\"{0}\">{1}</a>";
        private static string _linkNoFollow = "<a href=\"{0}\" rel=\"nofollow\">{1}</a>";

        /// <summary>
        /// Returns a copy of this string converted to HTML markup.
        /// </summary>
        public static string ToHtml(this string s)
        {
            return ToHtml(s, false);
        }

        /// <summary>
        /// Returns a copy of this string converted to HTML markup.
        /// </summary>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        public static string ToHtml(this string s, bool nofollow)
        {
            StringBuilder sb = new StringBuilder();

            int pos = 0;
            while (pos < s.Length)
            {
                // Extract next paragraph
                int start = pos;
                pos = s.IndexOf(_paraBreak, start);
                if (pos < 0)
                    pos = s.Length;
                string para = s.Substring(start, pos - start).Trim();

                // Encode non-empty paragraph
                //if (para.Length > 0)
                EncodeParagraph(para, sb, nofollow);

                // Skip over paragraph break
                pos += _paraBreak.Length;
            }
            // Return result
            return sb.ToString();
        }

        /// <summary>
        /// Encodes a single paragraph to HTML.
        /// </summary>
        /// <param name="s">Text to encode</param>
        /// <param name="sb">StringBuilder to write results</param>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        private static void EncodeParagraph(string s, StringBuilder sb, bool nofollow)
        {
            // Start new paragraph
            sb.AppendLine("<p>");

            // HTML encode text
            s = HttpUtility.HtmlEncode(s);

            // Convert single newlines to <br>
            s = s.Replace(Environment.NewLine, "<br />\r\n");

            // Encode any hyperlinks
            EncodeLinks(s, sb, nofollow);

            // Close paragraph
            sb.AppendLine("\r\n</p>");
        }

        /// <summary>
        /// Encodes [[URL]] and [[Text][URL]] links to HTML.
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <param name="sb">StringBuilder to write results</param>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        private static void EncodeLinks(string s, StringBuilder sb, bool nofollow)
        {
            // Parse and encode any hyperlinks
            int pos = 0;
            while (pos < s.Length)
            {
                // Look for next link
                int start = pos;
                pos = s.IndexOf("[[", pos);
                if (pos < 0)
                    pos = s.Length;
                // Copy text before link
                sb.Append(s.Substring(start, pos - start));
                if (pos < s.Length)
                {
                    string label, link;

                    start = pos + 2;
                    pos = s.IndexOf("]]", start);
                    if (pos < 0)
                        pos = s.Length;
                    label = s.Substring(start, pos - start);
                    int i = label.IndexOf("][");
                    if (i >= 0)
                    {
                        link = label.Substring(i + 2);
                        label = label.Substring(0, i);
                    }
                    else
                    {
                        link = label;
                    }
                    // Append link
                    sb.Append(String.Format(nofollow ? _linkNoFollow : _link, link, label));

                    // Skip over closing "]]"
                    pos += 2;
                }
            }
        }
    }

}