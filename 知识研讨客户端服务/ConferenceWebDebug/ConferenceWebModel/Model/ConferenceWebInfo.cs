using ConferenceWebModel.ConferenceInfoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace ConferenceWebModel.Model
{
    public class ConferenceWebInfo
    {
        #region 静态字段

        static RevertClient client;
        /// <summary>
        /// 启用lync扩展服务（语音）
        /// </summary>
        internal static RevertClient Client
        {
            get { return ConferenceWebInfo.client; }
        }

        #endregion

        #region 事件回调

        /// 获取会议预定会议信息
        /// </summary>
        Action<string, bool> FillConferenceInfoServiceData_CallBack = null;

        #endregion

        #region 客户端对象模型初始化

        /// <summary>
        /// 客户端对象模型初始化
        /// </summary>
        internal void ConferenceInfoInit(BasicHttpBinding binding, EndpointAddress endpoint, string domain, string userName, string password)
        {
            try
            {
                if (ConferenceWebInfo.client == null)
                {

                    //webservice客户端对象模型生成
                    ConferenceWebInfo.client = new RevertClient(binding, endpoint);

                    #region 用户验证

                    //使用工厂模式
                    client.ChannelFactory.Credentials.Windows.ClientCredential.Domain = domain;
                    client.ChannelFactory.Credentials.Windows.ClientCredential.UserName = userName;
                    client.ChannelFactory.Credentials.Windows.ClientCredential.Password = password;

                    #endregion

                    //凭据的设置
                    ConferenceWebInfo.client.ClientCredentials.Windows.AllowedImpersonationLevel =
                        TokenImpersonationLevel.Impersonation;
                    client.GetMeetInfoNowCompleted += client_GetMeetInfoCompleted;

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }



        #endregion

        #region 获取会议预定信息

        public string GetReservationConferenceInfo()
        {
            string info = string.Empty;
            try
            {               
               info=  ConferenceWebInfo.client.GetMeetInfoNow();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return info;
        }

        public void GetReservationConferenceInfo(Action<string, bool> callBack)
        {
            try
            {
                this.FillConferenceInfoServiceData_CallBack = callBack;
                ConferenceWebInfo.client.GetMeetInfoNowAsync();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        void client_GetMeetInfoCompleted(object sender, GetMeetInfoNowCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && this.FillConferenceInfoServiceData_CallBack != null)
                {
                    this.FillConferenceInfoServiceData_CallBack(e.Result, true);
                }
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
