using ConferenceCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Conference_WebBrowser
{
    [Serializable]
    public class UrlEntity
    {
        #region 属性

        private string urlName;
        private string url;
        /// <summary>
        /// 网络地址描述
        /// </summary>
        public string UrlName
        {
            get
            {
                return this.urlName;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.urlName = value;
                    //this.PropertyChangedNotity("UrlName");
                }
            }
        }
        /// <summary>
        /// uri地址
        /// </summary>
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.url = value;
                    //this.PropertyChangedNotity("Url");
                }
            }
        }

        #endregion

        #region 构造函数

        public UrlEntity(string _urlName, string _url)
        {
            try
            {
                this.UrlName = _urlName;
                this.Url = _url;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion
    }
}
