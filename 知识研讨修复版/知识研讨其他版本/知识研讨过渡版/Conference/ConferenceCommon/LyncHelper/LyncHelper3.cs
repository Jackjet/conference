using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.TimerHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceCommon.LyncHelper
{
    public partial class LyncHelper
    {
        #region 添加参会人为常用联系人

        /// <summary>
        /// 填充参会人信息
        /// </summary>
        public static void AddContacts()
        {
            #region 填充参会人信息

            try
            {
                if (Constant.lyncClient.State == ClientState.SignedIn)
                {
                    //第一次加载参会人
                    bool IsFirstInitNeedWait = false;                    

                    lyncContactManage = new LyncContactManage(Constant.lyncClient.ContactManager);

                    int timerSpan = 0;
                    foreach (var Participant in Constant.ParticipantList)
                    {
                        //获取联系人
                        Contact contact = Constant.lyncClient.ContactManager.GetContactByUri(Participant);

                        if (!lyncContactManage.OtherContactsGroup.Contains(contact) && !Constant.lyncClient.Self.Contact.Equals(contact))
                        {
                            bool canAddContact = lyncContactManage.OtherContactsGroup.CanInvoke(Microsoft.Lync.Model.Group.GroupAction.AddContact, contact);
                            if (canAddContact)
                            {
                                //添加其他联系人
                                lyncContactManage.OtherContactsGroup.BeginAddContact(contact, lyncContactManage.AddContact_Callback, null);
                                IsFirstInitNeedWait = true;
                            }
                        }
                    }
                    //获取联系人
                    Contact contactBigScreen = Constant.lyncClient.ContactManager.GetContactByUri(Constant.BigScreenName);
                    if (!lyncContactManage.OtherContactsGroup.Contains(contactBigScreen))
                    {
                        //添加其他联系人
                        lyncContactManage.OtherContactsGroup.BeginAddContact(contactBigScreen, lyncContactManage.AddContact_Callback, null);
                    }
                    if (IsFirstInitNeedWait)
                    {

                        timerSpan = 3000;
                    }
                    else
                    {
                        timerSpan = 100;
                    }
                    TimerJob.StartRun(new Action(() =>
                    {
                        foreach (var Participant in Constant.ParticipantList)
                        {
                            //获取联系人
                            Contact contact = Constant.lyncClient.ContactManager.GetContactByUri(Participant);
                            //获取联系人的显示名称
                            string displayName = Convert.ToString(contact.GetContactInformation(ContactInformationType.DisplayName));
                            //参会人信息字典加载参会人
                            if (!Constant.DicParticipant.ContainsKey(Participant))
                            {
                                Constant.DicParticipant.Add(Participant, displayName);
                            }
                        }
                    }), timerSpan);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }

            #endregion
        }

        #endregion

        #region 显示/隐藏内容

        /// <summary>
        /// 显示内容
        /// </summary>
        public static void ShowWindowContent()
        {
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                {
                    LyncHelper.MainConversation.ShowContent();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
        }

        public static void HidenWindowContent()
        {
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                {
                    LyncHelper.MainConversation.HideContent();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
        }

        #endregion
    }
}
