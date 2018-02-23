namespace TFOF.Areas.Core.Services {

    using System.Text.RegularExpressions;

    public static class TextService
    { 
        /// <summary>
        /// Creates a slug out of nicely formatted text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Slugify(this string text) 
        { 
            string str = text.RemoveAccent().ToLower(); 
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); 
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim(); 
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str; 
        } 

        /// <summary>
        /// Removes accents
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAccent(this string text) 
        { 
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text); 
            return System.Text.Encoding.ASCII.GetString(bytes); 
        }

        /// <summary>
        /// Takes Field or Model Name and makes titles or labels out of them.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Title(this string text )
        {
           return Regex.Replace(text, @"([A-Z])", " $1".ToUpper());
           
        }

    }
}