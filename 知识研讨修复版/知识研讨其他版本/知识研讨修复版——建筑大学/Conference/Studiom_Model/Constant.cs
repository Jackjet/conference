using Studiom_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Studiom_Model
{
    /// <summary>
    /// 持久类
    /// </summary>
    public static class Constant
    {
        static Pro_KnowledgeService.Pro_KnowledgeServiceSoapClient client;

        internal static Pro_KnowledgeService.Pro_KnowledgeServiceSoapClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new Pro_KnowledgeService.Pro_KnowledgeServiceSoapClient();

                }
                return client;
            }
        }


        //static MessageService.IgthClient gthClient;

        //public static MessageService.IgthClient GthClient
        //{           
        //     get
        //    {

        //        return gthClient;
        //    }
        //}

        //public static void GthClientInit(string webUri)
        //{
        //    if (gthClient == null)
        //    {
        //        BasicHttpBinding binding = new BasicHttpBinding();

        //        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
        //        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

        //        //binding.TransferMode = TransferMode.Streamed;
        //        binding.ReaderQuotas.MaxDepth = 32;
        //        binding.ReaderQuotas.MaxStringContentLength = 20971520;
        //        binding.ReaderQuotas.MaxArrayLength = 20971520;
        //        binding.ReaderQuotas.MaxBytesPerRead = 40960;
        //        binding.ReaderQuotas.MaxNameTableCharCount = 163840;
        //        binding.MaxReceivedMessageSize = 209715200;

        //        EndpointAddress endpoint =
        //            new EndpointAddress(webUri);

        //        //gthClient = new MessageService.IgthClient(binding, endpoint);


        //        //使用工厂模式
        //        gthClient.ChannelFactory.Credentials.Windows.ClientCredential.Domain = "SP";
        //        gthClient.ChannelFactory.Credentials.Windows.ClientCredential.UserName = "Administrator";
        //        gthClient.ChannelFactory.Credentials.Windows.ClientCredential.Password = "ZSSC@123";


        //        gthClient.ClientCredentials.Windows.AllowedImpersonationLevel =
        //            System.Security.Principal.TokenImpersonationLevel.Impersonation;
        //    }
        //}


        static StudiomService studiomDataInstance;
        /// <summary>
        /// 电源时序器
        /// </summary>
        public static StudiomService StudiomDataInstance
        {
            get
            {
                if (studiomDataInstance == null)
                {
                    studiomDataInstance = new StudiomService();
                    studiomDataInstance.StudiomServiceInit();
                }
                return Constant.studiomDataInstance;
            }

        }

        /// <summary>
        /// 初始化矩阵服务模型
        /// </summary>
        public static void StudiomDataInstanceInit()
        {
            if (studiomDataInstance == null)
            {
                studiomDataInstance = new StudiomService();
                studiomDataInstance.StudiomServiceInit();
            }
        }
    }
}
