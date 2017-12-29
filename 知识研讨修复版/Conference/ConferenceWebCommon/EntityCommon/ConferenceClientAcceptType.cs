using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityCommon
{
    [Serializable]
    public enum ConferenceClientAcceptType
    {
        /// <summary>
        /// 研讨树
        /// </summary>
        ConferenceTree =0,
        /// <summary>
        /// 语音树
        /// </summary>
        ConferenceAudio =1,
        /// <summary>
        /// 文件同步
        /// </summary>
        ConferenceFileSync =2,
       
        /// <summary>
        /// 智存空间
        /// </summary>
        ConferenceSpaceSync =3,

        /// <summary>
        /// 信息同步
        /// </summary>
        ConferenceInfoSync =4,

        /// <summary>
        /// lync会话同步
        /// </summary>
        LyncConversationSync =5,

        /// <summary>
        /// 矩阵应用
        /// </summary>
        ConferenceMatrixSync =6,
    }
}
