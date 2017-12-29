using ConferenceModel.LogHelper;
using ConferenceModel.ConferenceAudioWebservice;
using ConferenceModel.ConferenceTreeWebService;
using ConferenceModel.ConferenceVersionWebservice;
using ConferenceModel.Enum;
using ConferenceModel.FileSyncAppPoolWebService;

using ConferenceModel.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace ConferenceModel
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

        static ConferenceTree conferenceTree;
        /// <summary>
        /// 研讨树
        /// </summary>
        public static ConferenceTree ConferenceTree
        {
            get
            {
                if (conferenceTree == null)
                {
                    conferenceTree = new ConferenceTree();
                }
                return ModelManage.conferenceTree;
            }
        }

        static ConferenceAudio conferenceAudio;
        /// <summary>
        /// 研讨语音
        /// </summary>
        public static ConferenceAudio ConferenceAudio
        {
            get
            {
                if (conferenceAudio == null)
                {
                    conferenceAudio = new ConferenceAudio();
                }
                return ModelManage.conferenceAudio;
            }
        }

        static ConferenceVersion conferenceVersion;
        /// <summary>
        /// 版本更新服务
        /// </summary>
        public static ConferenceVersion ConferenceVersion
        {
            get
            {
                if (conferenceVersion == null)
                {
                    conferenceVersion = new ConferenceVersion();
                }
                return ModelManage.conferenceVersion;
            }
        }

        static FileSyncAppPool fileSyncAppPool;
        /// <summary>
        /// 文件同步（甩屏）
        /// </summary>
        public static FileSyncAppPool FileSyncAppPool
        {
            get
            {
                if (fileSyncAppPool == null)
                {
                    fileSyncAppPool = new FileSyncAppPool();
                }
                return ModelManage.fileSyncAppPool;
            }
        }


        static ConferenceSpaceAsync conferenceWordAsync;
        /// <summary>
        /// wrod同步【协同编辑】
        /// </summary>
        public static ConferenceSpaceAsync ConferenceWordAsync
        {
            get
            {
                if (conferenceWordAsync == null)
                {
                    conferenceWordAsync = new ConferenceSpaceAsync();
                }
                return ModelManage.conferenceWordAsync;
            }
        }

        static ConferenceInfo conferenceInfo;
        /// <summary>
        /// 信息同步
        /// </summary>
        public static ConferenceInfo ConferenceInfo
        {
            get
            {
                if (conferenceInfo == null)
                {
                    conferenceInfo = new ConferenceInfo();
                }
                return ModelManage.conferenceInfo;
            }
        }

        static ConferenceLyncConversation conferenceLyncConversation;
        /// <summary>
        /// lync会话同步
        /// </summary>
        public static ConferenceLyncConversation ConferenceLyncConversation
        {
            get
            {
                if (conferenceLyncConversation == null)
                {
                    conferenceLyncConversation = new ConferenceLyncConversation();
                }
                return ModelManage.conferenceLyncConversation;
            }
        }

        static ConferenceMatrix conferenceMatrix;
        /// <summary>
        /// 矩阵切换
        /// </summary>
        public static ConferenceMatrix ConferenceMatrix
        {
            get
            {
                if (conferenceMatrix == null)
                {
                    conferenceMatrix = new ConferenceMatrix();
                }
                return ModelManage.conferenceMatrix;
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
        public static void ClientInit(string webUrl, ClientModelType clientModelType, string domain, string userName, string pwd)
        {
            try
            {
                EndpointAddress endpoint =
                              new EndpointAddress(webUrl);

                switch (clientModelType)
                {

                    case ClientModelType.ConferenceVersion:
                        //客户端对象模型初始化
                        ConferenceVersion.ConferenceVersionInit(binding, endpoint);

                        break;
                    case ClientModelType.ConferenceTree:
                        //客户端对象模型初始化
                        ConferenceTree.ConferenceTreeInit(binding, endpoint);

                        break;
                    case ClientModelType.ConferenceAudio:
                        //客户端对象模型初始化
                        ConferenceAudio.ConferenceAudioInit(binding, endpoint);

                        break;
                    case ClientModelType.FileSync:
                        //客户端对象模型初始化
                        FileSyncAppPool.FileSyncAppPoolInit(binding, endpoint);

                        break;
                    case ClientModelType.Spacesync:
                        //客户端对象模型初始化
                        ConferenceWordAsync.ConferenceOfficeAsyncInit(binding, endpoint);

                        break;
                    case ClientModelType.ConferenceInfo:
                        //客户端对象模型初始化
                        ConferenceInfo.ConferenceInfoInit(binding, endpoint);
                        break;

                    case ClientModelType.LyncConversationSync:
                        //客户端对象模型初始化
                        ConferenceLyncConversation.ConferenceInfoInit(binding, endpoint);
                        break;
                    case ClientModelType.MaxtriSync:
                        //客户端对象模型初始化
                        ConferenceMatrix.ConferenceInfoInit(binding, endpoint);
                        break;                      
                    default:
                        break;
                }
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


#region old solution

//case ClientModelType.MeetingInfo:
//    if (revertClient == null)
//    {
//        revertClient = new RevertClient(binding, endpoint);

//        #region 用户验证

//        //使用工厂模式
//        revertClient.ChannelFactory.Credentials.Windows.ClientCredential.Domain = domain;
//        revertClient.ChannelFactory.Credentials.Windows.ClientCredential.UserName = userName;
//        revertClient.ChannelFactory.Credentials.Windows.ClientCredential.Password = pwd;

//        #endregion

//        revertClient.ClientCredentials.Windows.AllowedImpersonationLevel =
//           TokenImpersonationLevel.Impersonation;

//        IgthDataService.IgthDataServiceEventRegedit();
//    }
//    break;

#endregion