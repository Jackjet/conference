using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.Common
{
    public static class InnerResourceHelper
    {
        /// <summary>
        /// 手机传送连接命令分隔符
        /// </summary>
        public static readonly Char MobileConnectCommondSplitChar = Convert.ToChar(System.Configuration.ConfigurationManager.AppSettings["MobileConnectCommondSplitChar"]);
    }
}
