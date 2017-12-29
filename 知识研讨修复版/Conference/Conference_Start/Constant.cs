using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using config = System.Configuration.ConfigurationManager;

namespace Conference_Start
{
    public static class Constant
    {
        /// <summary>
        /// 应用程序名称(全名称)
        /// </summary>
        public static string ApplicationFullName = Environment.CurrentDirectory + "\\" + config.AppSettings["ApplicationFullName"];


        /// <summary>
        /// 应用程序名称(全名称)
        /// </summary>
        public static string ApplicationName =  config.AppSettings["ApplicationName"];


        /// <summary>
        /// 应用程序名称(全名称)
        /// </summary>
        public static string LyncName = config.AppSettings["LyncName"];

    }
}
