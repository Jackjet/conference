using ConferenceCommon.LogHelper;
using ConferenceCommon.WebHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Conference.View.Space
{
    /// <summary>
    /// FileBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class FileBrowser : Window
    {
        #region 字段

        /// <summary>
        /// web凭据管理模型
        /// </summary>
        WebCredentialManage manage = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileUri">网页地址</param>
        public FileBrowser(string fileUri)
        {
            try
            {
                //UI加载
                InitializeComponent();
                //初始化
                FileBrowserInit(fileUri);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //文件浏览器关闭事件
                this.Closed += FileBrowser_Closed;
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

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileUri"></param>
        private void FileBrowserInit(string fileUri)
        {
            try
            {
                //设置文件浏览器的尺寸
                this.Width = (SystemParameters.WorkArea.Width - 100);
                this.Height = (SystemParameters.WorkArea.Height - 100);

                //设置文件浏览器的坐标
                this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
                this.Top = 20;

                //生成凭据通过验证
                manage = new WebCredentialManage(this.webBrowser, Constant.WebLoginUserName, Constant.WebLoginPassword);
                //导航
                manage.Navicate(fileUri);
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
      
        #region 文件浏览器关闭事件
        
        /// <summary>
        /// 文件浏览器关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FileBrowser_Closed(object sender, EventArgs e)
        {
            try
            {
                if (this.manage != null)
                {
                    //清除缓存（用户）
                    this.manage.SessionClear();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
     
    }
}
