using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceModel.ConferenceMatrixWebservice;
using System.ServiceModel;
using System.Security.Principal;
using ConferenceModel.LogHelper;

namespace ConferenceModel.Model
{
    public class ConferenceMatrix
    {
        #region 静态字段


        static ConferenceMatrixWebserviceSoapClient client;
        /// <summary>
        /// 启用lync扩展服务（语音）
        /// </summary>
        internal static ConferenceMatrixWebserviceSoapClient Client
        {
            get { return ConferenceMatrix.client; }
        }

        #endregion

        #region 事件回调

        /// 矩阵设置回调
        /// </summary>
        Action<bool> MatrixSync_CallBack = null;

        /// 进入座位回调
        /// </summary>
        Action<bool, List<SeatEntity>> IntoOneSeatSync_CallBack = null;

        /// <summary>
        /// 离开座位
        /// </summary>
        Action<bool> LeaveOneSeatSync_CallBack = null;

        #endregion

        #region 客户端对象模型初始化

        /// <summary>
        /// 客户端对象模型初始化
        /// </summary>
        internal void ConferenceInfoInit(BasicHttpBinding binding, EndpointAddress endpoint)
        {
            try
            {
                if (ConferenceMatrix.client == null)
                {
                    //webservice客户端对象模型生成
                    ConferenceMatrix.client = new ConferenceMatrixWebserviceSoapClient(binding, endpoint);
                    //凭据的设置
                    ConferenceMatrix.client.ClientCredentials.Windows.AllowedImpersonationLevel =
                        TokenImpersonationLevel.Impersonation;
                    //投影设置完成事件
                    ConferenceMatrix.Client.SetMatrixEntityCompleted += Client_SetMatrixEntityCompleted;
                    //进入一个座位完成事件
                    ConferenceMatrix.Client.InToOneSeatCompleted += Client_InToOneSeatCompleted;
                    //离开座位完成事件
                    ConferenceMatrix.Client.LeaveOneSeatCompleted += Client_LeaveOneSeatCompleted;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion


        #region 投影占用

        /// <summary>
        /// 投影占用
        /// </summary>
        /// <param name="confrenceName"></param>
        /// <param name="lyncConversationEntity"></param>
        /// <param name="callBack"></param>
        public void MatrixSetting(string confrenceName, string sharer, ConferenceMatrixOutPut conferenceMatrixOutPut, Action<bool> callBack)
        {
            //设置事件回调
            this.MatrixSync_CallBack = callBack;
            //投影占用
            ConferenceMatrix.Client.SetMatrixEntityAsync(confrenceName, sharer, conferenceMatrixOutPut);
        }

        /// <summary>
        /// 投影占用完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_SetMatrixEntityCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    this.MatrixSync_CallBack(true);
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

        #region 进入到一个座位

        /// <summary>
        /// 进入到一个座位
        /// </summary>
        public void IntoOneSeat(string conferenceName,string seatList ,string selfName, string selfIP, Action<bool, List<SeatEntity>> callBack)
        {
            try
            {
                //回调绑定
                this.IntoOneSeatSync_CallBack = callBack;
                //进入一个座位
                ConferenceMatrix.Client.InToOneSeatAsync(conferenceName, seatList, selfName, selfIP);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 进入一个座位完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_InToOneSeatCompleted(object sender, InToOneSeatCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    //回调
                    this.IntoOneSeatSync_CallBack(true, e.Result.ToList<SeatEntity>());
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

        #region 离开座位

        /// <summary>
        /// 离开座位
        /// </summary>
        public void LeaveOneSeat(string conferenceName, string selfName, string selfIP, Action<bool> callBack)
        {
            try
            {
                //回调绑定
                this.LeaveOneSeatSync_CallBack = callBack;
                //离开座位事件
                ConferenceMatrix.Client.LeaveOneSeatAsync(conferenceName, selfName, selfIP);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// //离开座位完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_LeaveOneSeatCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if(e.Error == null)
                {
                    this.LeaveOneSeatSync_CallBack(true);
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
