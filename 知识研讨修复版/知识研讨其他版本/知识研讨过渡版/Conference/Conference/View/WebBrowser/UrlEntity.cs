using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Conference.View.WebBrowser
{
    [Serializable]
    public class UrlEntity : INotifyPropertyChanged
    {
        private string urlName;
        private string url;
        public event PropertyChangedEventHandler PropertyChanged;
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
                    this.PropertyChangedNotity("UrlName");
                }
            }
        }
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
                    this.PropertyChangedNotity("Url");
                }
            }
        }
        public UrlEntity(string _urlName, string _url)
        {
            this.UrlName = _urlName;
            this.Url = _url;
        }
        public void PropertyChangedNotity(string propertyInfo)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyInfo));
            }
        }
    }
}
