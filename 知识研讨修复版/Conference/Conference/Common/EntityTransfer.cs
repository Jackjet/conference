using ConferenceCommon.LogHelper;
using ConferenceModel.ConferenceAudioWebservice;
using ConferenceModel.ConferenceTreeWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference.Common
{
    public class EntityTransfer
    {
        #region 协议实体层转换

        /// <summary>
        /// 语音映射实体转换
        /// </summary>
        public static ConferenceAudioItemTransferEntity AudioTransferEntityChanged(ConferenceWebCommon.EntityHelper.ConferenceDiscuss.ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntity)
        {
            //返回的语音映射实体（weservicve自引用）
            ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntityReturn = new ConferenceAudioItemTransferEntity();
            try
            {
                //GUID绑定
                conferenceAudioItemTransferEntityReturn.Guid = conferenceAudioItemTransferEntity.Guid;
                //消息绑定
                conferenceAudioItemTransferEntityReturn.AudioMessage = conferenceAudioItemTransferEntity.AudioMessage;
                //子节点添加人
                conferenceAudioItemTransferEntityReturn.AddAuthor = conferenceAudioItemTransferEntity.AddAuthor;
                //发送时间
                conferenceAudioItemTransferEntityReturn.MessageSendTime = conferenceAudioItemTransferEntity.MessageSendTime;
                //发送人
                conferenceAudioItemTransferEntityReturn.MessageSendName = conferenceAudioItemTransferEntity.MessageSendName;
                //是否为自己所发送
                //conferenceAudioItemTransferEntityReturn.IsSelfSend = conferenceAudioItemTransferEntityReturn.IsSelfSend;
                //音频地址绑定
                conferenceAudioItemTransferEntityReturn.IMMType = conferenceAudioItemTransferEntity.IMMType;
                //音频地址绑定
                conferenceAudioItemTransferEntityReturn.AudioUrl = conferenceAudioItemTransferEntity.AudioUrl;
                //音频文件名称
                conferenceAudioItemTransferEntityReturn.AudioFileName = conferenceAudioItemTransferEntity.AudioFileName;
                //个人头像
                conferenceAudioItemTransferEntityReturn.PersonalImg = conferenceAudioItemTransferEntity.PersonalImg;
                //类型绑定
                conferenceAudioItemTransferEntityReturn.Operation = (ConferenceAudioOperationType)((int)conferenceAudioItemTransferEntity.Operation);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(EntityTransfer), ex);
            }
            finally
            {

            }
            return conferenceAudioItemTransferEntityReturn;
        }

        /// <summary>
        /// 知识树映射实体转换
        /// </summary>
        public static ConferenceTreeItemTransferEntity TreeTransferEntityChanged(ConferenceWebCommon.EntityHelper.ConferenceTree.ConferenceTreeItemTransferEntity conferenceTreeItemTransferEntity)
        {
            //返回的语音映射实体（weservicve自引用）
            ConferenceTreeItemTransferEntity conferenceTreeItemTransferEntityReturn = new ConferenceTreeItemTransferEntity();
            try
            {
                //GUID绑定
                conferenceTreeItemTransferEntityReturn.Guid = conferenceTreeItemTransferEntity.Guid;
                //论点
                conferenceTreeItemTransferEntityReturn.Comment = conferenceTreeItemTransferEntity.Comment;             
                //操作人
                conferenceTreeItemTransferEntityReturn.Operationer = conferenceTreeItemTransferEntity.Operationer;
                //父节点GUID
                conferenceTreeItemTransferEntityReturn.ParentGuid = conferenceTreeItemTransferEntity.ParentGuid;              
                //标题
                conferenceTreeItemTransferEntityReturn.Title = conferenceTreeItemTransferEntity.Title;
                //链接列表
                conferenceTreeItemTransferEntityReturn.LinkList = conferenceTreeItemTransferEntity.LinkList;
                //焦点占用者
                conferenceTreeItemTransferEntityReturn.FocusAuthor = conferenceTreeItemTransferEntity.FocusAuthor;
                //类型绑定
                conferenceTreeItemTransferEntityReturn.Operation = (ConferenceTreeOperationType)((int)conferenceTreeItemTransferEntity.Operation);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(EntityTransfer), ex);
            }
            finally
            {

            }
            return conferenceTreeItemTransferEntityReturn;
        }

        #endregion
    }
}
