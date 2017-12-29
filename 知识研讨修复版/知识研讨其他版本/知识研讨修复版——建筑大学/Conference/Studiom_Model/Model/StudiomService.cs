using ConferenceCommon.LogHelper;
//using Studiom_Common.Log;
using Studiom_Model.Pro_KnowledgeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Studiom_Model.Model
{
    /// <summary>
    /// 物联网服务对接
    /// </summary>
    public class StudiomService
    {
        #region 事件回调

        /// <summary>
        /// 获取所有数据（温度、湿度、CO2、光照、pm2.5）回调
        /// </summary>
        Action<string, string> DataAll_Get_Back = null;

        /// <summary>
        /// 通用方法(录播控制、电源时序器、继电器)回调
        /// </summary>
        Action<string, string> MethodInvok1_Call_Back = null;

        /// <summary>
        /// 通用方法(风扇、全部开关量、窗帘、警报、灯光)回调
        /// </summary>
        Action<string, string> MethodInvok2_Call_Back = null;

        /// <summary>
        /// 矩阵切换
        /// </summary>
        Action<string, string> MatrixChange_Call_Back = null;

        #endregion

        #region 事件注册

        /// <summary>
        /// 事件注册
        /// </summary>
        public void StudiomServiceInit()
        {
            try
            {
                Constant.Client.DataAll_GetCompleted += Client_DataAll_GetCompleted;

                Constant.Client.MethodInvoke1Completed += Client_MethodInvoke1Completed;

                Constant.Client.MethodInvoke2Completed += Client_MethodInvoke2Completed;

                Constant.Client.MatrixChangeCompleted += Client_MatrixChangeCompleted;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

     
        #endregion

        #region 获取所有环境数据

        public void DataAll_Get(Action<string, string> callback)
        {
            try
            {
                this.DataAll_Get_Back = callback;

                Constant.Client.DataAll_GetAsync();

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        void Client_DataAll_GetCompleted(object sender, Pro_KnowledgeService.DataAll_GetCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    if (!string.IsNullOrEmpty(e.Result.InnerError))
                    {
                        LogManage.WriteLog(this.GetType(), e.Result.InnerError);
                    }
                    this.DataAll_Get_Back(e.Result.Return_Param, e.Result.ServerError);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
                this.DataAll_Get_Back(ex.ToString(), e.Result.ServerError);
            }
        }

        #endregion

        #region 通用方法(录播控制、电源时序器、继电器)

        /// <summary>
        /// 通用方法(录播控制、电源时序器、继电器)
        /// </summary>
        public void MethodInvoke1(Pro_KnowledgeService.ControlType1 controltype, Action<string, string> callback)
        {
            try
            {
                //Console.WriteLine(controltype);
                this.MethodInvok1_Call_Back = callback;
                Constant.Client.MethodInvoke1Async(controltype);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 通用方法(录播控制、电源时序器、继电器)回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_MethodInvoke1Completed(object sender, Pro_KnowledgeService.MethodInvoke1CompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    this.MethodInvok1_Call_Back(e.Result.InnerError, e.Result.ServerError);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 通用方法(风扇、全部开关量、窗帘、警报、灯光)

        /// <summary>
        /// 通用方法(风扇、全部开关量、窗帘、警报、灯光)
        /// </summary>
        public void MethodInvoke2(Pro_KnowledgeService.ControlType2 controltype, Action<string, string> callback)
        {
            try
            {
                Console.WriteLine(controltype);
                this.MethodInvok2_Call_Back = callback;
                Constant.Client.MethodInvoke2Async(controltype);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 通用方法(风扇、全部开关量、窗帘、警报、灯光)回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_MethodInvoke2Completed(object sender, Pro_KnowledgeService.MethodInvoke2CompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    this.MethodInvok2_Call_Back(e.Result.InnerError, e.Result.ServerError);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 矩阵切换

        /// <summary>
        /// 矩阵切换
        /// </summary>
        public void MatrixChange(MatrixChangeType  matrixChangeType,Action<string,string> callBack)
        {
            try
            {
                this.MatrixChange_Call_Back = callBack;

                Constant.Client.MatrixChangeAsync(matrixChangeType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 矩阵切换完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_MatrixChangeCompleted(object sender, Pro_KnowledgeService.MatrixChangeCompletedEventArgs e)
        {
             try
            {
                 if(e.Error  == null)
                 {
                     this.MatrixChange_Call_Back(e.Result.InnerError, e.Result.ServerError);
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
