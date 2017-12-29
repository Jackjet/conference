using ConferenceModel.LogHelper;
using ConferenceModel.ConferenceSpaceAsyncWebservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace ConferenceModel.Model
{
    public class ConferenceSpaceAsync
    {
        #region 静态字段

        static ConferenceSpaceAsyncWebserviceSoapClient client;
        /// <summary>
        /// Word同步
        /// </summary>
        internal static ConferenceSpaceAsyncWebserviceSoapClient Client
        {
            get { return ConferenceSpaceAsync.client; }
        }

        #endregion

        #region 事件回调

        /// 填充服务同步数据回调
        /// </summary>
        Action<bool> FillConferenceOfficeServiceData_CallBack = null;

        #endregion

        #region 注册事件

        /// <summary>
        /// 客户端对象模型初始化
        /// </summary>
        internal void ConferenceOfficeAsyncInit(BasicHttpBinding binding, EndpointAddress endpoint)
        {
            try
            {
                if (ConferenceSpaceAsync.client == null)
                {
                    //生成客户端对象模型
                    ConferenceSpaceAsync.client = new ConferenceSpaceAsyncWebservice.ConferenceSpaceAsyncWebserviceSoapClient(binding, endpoint);

                    //设置凭据类型
                    ConferenceSpaceAsync.client.ClientCredentials.Windows.AllowedImpersonationLevel =
                      TokenImpersonationLevel.Impersonation;

                    //填充服务器数据（智存空间文件共享）
                    ConferenceSpaceAsync.Client.FillSyncServiceDataCompleted += Client_FillSyncServiceDataCompleted;

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 填充服务同步数据

        /// <summary>
        /// 填充服务同步数据
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <param name="bytes"></param>
        /// <param name="callBack"></param>
        public void FillConferenceOfficeServiceData(string conferenceName, string sharer, string uri, FileType fileType, Action<bool> callBack)
        {
            try
            {
                this.FillConferenceOfficeServiceData_CallBack = callBack;

                ConferenceSpaceAsync.Client.FillSyncServiceDataAsync(conferenceName, sharer, uri, fileType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 填充服务同步数据完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_FillSyncServiceDataCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    if (this.FillConferenceOfficeServiceData_CallBack != null)
                    {
                        this.FillConferenceOfficeServiceData_CallBack(true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 参加一个会议
        /// </summary>
        public void JoinMainConference()
        {
                try
            {               
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
