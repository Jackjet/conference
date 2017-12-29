using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_WebBrowser.Common
{
    public static class WebBrowserCodeEnterEntity
    {
        /// <summary>
        /// 网络浏览地址
        /// </summary>
        public static string WebUri { get; set; }

          /// <summary>
        /// url文件名称
        /// </summary>
        public static string urlStoreFileName { get; set; }

        /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName { get; set; }

        /// <summary>
        /// 智存空间登陆密码
        /// </summary>
        public static string WebLoginPassword { get; set; }
    }
}
