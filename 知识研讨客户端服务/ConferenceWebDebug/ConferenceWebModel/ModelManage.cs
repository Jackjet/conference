
using ConferenceWebModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace ConferenceWebModel
{
    public class ModelManage
    {
        #region 静态字段

        /// <summary>
        /// http终结点
        /// </summary>
        static BasicHttpBinding binding = GetBasicHttpBinding();

        #endregion

        #region 公共属性（Model外部调用）

        static ConferenceWebInfo conferenceWebInfo;
        /// <summary>
        /// 研讨树
        /// </summary>
        public static ConferenceWebInfo ConferenceWebInfo
        {
            get
            {
                if (conferenceWebInfo == null)
                {
                    conferenceWebInfo = new ConferenceWebInfo();
                }
                return ModelManage.conferenceWebInfo;
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 客户端对象模型初始化
        /// </summary>
        /// <param name="webUrl"></param>
        /// <param name="clientModelType"></param>
        /// <param name="domain"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        public static void ClientInit(string webUrl, string domain, string userName, string pwd)
        {
            try
            {
                EndpointAddress endpoint =
                              new EndpointAddress(webUrl);


                //客户端对象模型初始化
                ConferenceWebInfo.ConferenceInfoInit(binding, endpoint,domain,userName,pwd);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ModelManage), ex);
            }
        }


        /// <summary>
        /// 生成终结点
        /// </summary>
        /// <returns></returns>
        static BasicHttpBinding GetBasicHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            try
            {
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 20971520;
                binding.ReaderQuotas.MaxArrayLength = 20971520;
                binding.ReaderQuotas.MaxBytesPerRead = 40960;
                binding.ReaderQuotas.MaxNameTableCharCount = 163840;
                binding.MaxReceivedMessageSize = 209715200;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ModelManage), ex);
            }
            return binding;
        }

        #endregion
    }
}