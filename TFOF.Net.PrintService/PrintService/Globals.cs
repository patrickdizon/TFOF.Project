using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintService
{
    /// <summary>
	/// As name indicates this class holds all the static properties required within the project.
	/// </summary>
	/// <remarks>
	/// some of these variables are assigned during application start reading from the config files
	/// 06/16/2015 - Created by Laxmana Davuluri
	/// </remarks>
	public static class Globals
    {
        public static bool ISPRODUCTION;

        public static string ENVIRONMENT;
    }
}
