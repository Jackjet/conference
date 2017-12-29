using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.TimerHelper;
using Microsoft.Lync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Conference_Conversation.Common
{
    public partial class LyncHelper
    {
        #region 判断是否可以进行嵌入

        /// <summary>
        /// 判断是否可以进行嵌入
        /// </summary>
        public static void IsSignedOutDoSomeThing(Action callBack)
        {
            try
            {
                if (ConversationCodeEnterEntity.lyncClient != null && ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedOut)
                {
                    callBack();
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

        #region 取消登录

        /// <summary>
        /// 取消登录
        /// </summary>
        /// <param name="callBack"></param>
        public static void CancelLyncSigned(Action callBack)
        {
            try
            {
                //停止嵌入并退出登陆框
                if (ConversationCodeEnterEntity.lyncClient.State == ClientState.SigningIn)
                {
                    ConversationCodeEnterEntity.lyncClient.BeginSignOut(null, null);
                }
                callBack();
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

        #region lync签入

        /// <summary>
        /// lync签入
        /// </summary>
        /// <param name="UserUri"></param>
        /// <param name="Password"></param>
        /// <param name="callBack"></param>
        public static void LyncSignning(string UserUri, string Password, Action callBack)
        {
            try
            {
                if (ConversationCodeEnterEntity.lyncClient.State != ClientState.SignedIn && ConversationCodeEnterEntity.lyncClient.State != ClientState.SigningIn)
                {
                    //开始签入
                    IAsyncResult ar = ConversationCodeEnterEntity.lyncClient.BeginSignIn(
                        UserUri,
                        UserUri,
                        Password,
                        null,
                        null);
                }
                if (callBack != null)
                {
                    callBack();
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

        #region lync状态设置

        /// <summary>
        /// lync状态设置
        /// </summary>
        /// <param name="stateIndex"></param>
        public static void LyncStateSetting(int stateIndex)
        {
            try
            {
                if (ConversationCodeEnterEntity.lyncClient.SignInConfiguration != null)
                {

                    //通过选择的用户状态设置lync config相关状态信息
                    var stateEnum = (UserLoginState)stateIndex;
                    //状态（个人模式时使用）
                    switch (stateEnum)
                    {
                        //在线
                        case UserLoginState.Available:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Free;
                            break;
                        //忙碌
                        case UserLoginState.Busy:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Busy;
                            break;
                        //请勿打扰
                        case UserLoginState.DoNotDisturb:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.DoNotDisturb;
                            break;
                        //离开
                        case UserLoginState.Away:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.TemporarilyAway;
                            break;
                        //下班
                        case UserLoginState.OffWork:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Offline;
                            break;
                        //离开
                        case UserLoginState.Leave:
                            ConversationCodeEnterEntity.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Away;
                            break;
                        default:
                            break;
                    }
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

        #region lync签出

        /// <summary>
        /// lync签出
        /// </summary>
        public static void LyncSignOut()
        {
            try
            {
                //如果为签入状态，则设置为签出
                if (ConversationCodeEnterEntity.lyncClient.State == Microsoft.Lync.Model.ClientState.SignedIn || ConversationCodeEnterEntity.lyncClient.State == Microsoft.Lync.Model.ClientState.SigningIn)
                {
                    //lync签出
                    ConversationCodeEnterEntity.lyncClient.BeginSignOut(null, null);
                    //lync异常（与本系统无关）
                    //Constant.lyncClient.SignInConfiguration.ForgetMe(Constant.SelfUri);
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

        /// <summary>
        /// lync签出
        /// </summary>
        public static void LyncSignOut(Action callBack)
        {
            try
            {
                if (ConversationCodeEnterEntity.lyncClient != null)
                {
                    callBack();
                    //签出
                    LyncHelper.LyncSignOut();
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

        #region lync状态更改

        /// <summary>
        /// lync状态更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void lyncClient_StateChanged(object sender, ClientStateChangedEventArgs e)
        {
            //异步委托
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //已签入或者签入中
                    if (e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedIn)
                    {
                        if (StateINCallBack != null)
                        {
                            StateINCallBack();
                        }

                    }
                    //当前lync状态为正要签出的状态
                    else if (e.OldState == ClientState.SignedIn && e.NewState == ClientState.SigningOut)
                    {
                        #region 恢复未登陆前的状态

                        if (StateOutCallBack != null)
                        {
                            StateOutCallBack();
                        }

                        #endregion
                    }
                    else if (e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedOut)
                    {
                        //登陆面板设置为可用
                        //this.LoginPanelIsEnable = true;
                        if (StateIN_OutCallBack != null)
                        {
                            StateIN_OutCallBack();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(typeof(LyncHelper), ex);
                }
            }));
        }

        #endregion

        #region 填充参会人列表

        /// <summary>
        /// 填充参会人列表
        /// </summary>
        public static void FillLyncOnlineInfo(DataGrid datagrid)
        {
            try
            {
                if (ConversationCodeEnterEntity.lyncClient != null)
                {
                    //数据清除
                    currentParticipantsEntityList.Clear();
                    //递增序号
                    int number = 0;
                    //填充颜色
                    StyleCollection();
                    //遍历添加参会人
                    foreach (var item in ConversationCodeEnterEntity.ParticipantList)
                    {
                        //创建参会人子项实体
                        ParticipantsEntity participantsEntity = new ParticipantsEntity();

                        //将参会人的名称取出
                        if (ConversationCodeEnterEntity.DicParticipant.ContainsKey(item))
                        {
                            //获取姓名
                            participantsEntity.ParticipantsName = ConversationCodeEnterEntity.DicParticipant[item];
                            number++;
                            //序号
                            participantsEntity.Number = number;

                            if (ConversationCodeEnterEntity.lyncClient != null)
                            {
                                Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(item);
                                //联系人信息更改事件
                                contact.ContactInformationChanged -= contact_ContactInformationChanged;
                                //联系人信息更改事件
                                contact.ContactInformationChanged += contact_ContactInformationChanged;

                                //登录uri地址
                                participantsEntity.LoginUri = item;
                                //登录名称
                                participantsEntity.LoginName = item.Replace("sip:", string.Empty).Replace(ConversationCodeEnterEntity.UserDomain, string.Empty).Replace("@", string.Empty);
                                //填充信息辅助
                                FillLyncOnlineInfoHelper(contact, participantsEntity);
                                //添加参会人
                                currentParticipantsEntityList.Add(participantsEntity);
                            }
                        }
                    }
                    //将datagrid返回
                    if (BeginRefleshDataGridCallBack != null)
                    {
                        BeginRefleshDataGridCallBack(currentParticipantsEntityList);
                    }
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

        /// <summary>
        /// 样式收集
        /// </summary>
        static void StyleCollection()
        {
            try
            {
                //资源字典
                ResourceDictionary resourceDictionary = Application.Current.Resources;
                //判断当前应用程序是否包含字典
                if (resourceDictionary != null)
                {
                    //普通画刷（字体）
                    if (NormalColorBrush == null)
                    {
                        if (resourceDictionary.Contains("NormalColorBrush"))
                        {
                            NormalColorBrush = resourceDictionary["NormalColorBrush"] as SolidColorBrush;
                        }
                    }
                    //绿色画刷（字体）
                    if (GreenColorBrush == null)
                    {
                        if (resourceDictionary.Contains("GreenColorBrush"))
                        {
                            GreenColorBrush = resourceDictionary["GreenColorBrush"] as SolidColorBrush;
                        }
                    }
                    //绿色条
                    if (GreenColor == null)
                    {
                        if (resourceDictionary.Contains("GreenColor"))
                        {
                            GreenColor = resourceDictionary["GreenColor"] as SolidColorBrush;
                        }
                    }
                    //灰色条
                    if (GrayColor == null)
                    {
                        if (resourceDictionary.Contains("GrayColor"))
                        {
                            GrayColor = resourceDictionary["GrayColor"] as SolidColorBrush;
                        }
                    }
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

        /// <summary>
        /// 参会人状态更改捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            try
            {
                //使用异步委托
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //判断是否有lync实例并判断其实例的状态
                    if (ConversationCodeEnterEntity.lyncClient != null && ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedIn)
                    {
                        //获取联系人
                        Contact contact = sender as Contact;
                        if (contact != null)
                        {
                            //获取相关人的邮箱地址列表
                            System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)contact.GetContactInformation(ContactInformationType.EmailAddresses);
                            //邮箱地址列表数量不为零
                            if (list.Count > 0 && currentParticipantsEntityList != null)
                            {
                                List<ParticipantsEntity> participantsEntityTempList = currentParticipantsEntityList.Where(item => item.LoginUri.ToLower().Equals(Convert.ToString(list[0]).ToLower())).ToList<ParticipantsEntity>();

                                if (participantsEntityTempList.Count > 0)
                                {
                                    //获取对应参会人
                                    ParticipantsEntity Participant = participantsEntityTempList[0];
                                    //填充信息辅助
                                    FillLyncOnlineInfoHelper(contact, Participant);

                                }
                                //返回参会人信息
                                if (BeginRefleshDataGridCallBack != null)
                                {
                                    BeginRefleshDataGridCallBack(currentParticipantsEntityList);
                                }
                            }
                        }
                    }
                }));

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 填充信息辅助
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="Participant"></param>
        static void FillLyncOnlineInfoHelper(Contact contact, ParticipantsEntity Participant)
        {
            try
            {
                //获取相关人的头像地址
                string uriImg = ConversationCodeEnterEntity.TreeServiceAddressFront + ConversationCodeEnterEntity.FtpServercePersonImgName + Participant.LoginName + ".png";
                //生成专用图片
                BitmapImage btimap = new BitmapImage(new Uri(uriImg));

                //获取当前人的公司名称
                Participant.Company = Convert.ToString(contact.GetContactInformation(ContactInformationType.Company));
                ////获取当前人的职位
                Participant.Position = Convert.ToString(contact.GetContactInformation(ContactInformationType.Title));
                //获取相关联系人的活动状态
                string strState = Convert.ToString(contact.GetContactInformation(ContactInformationType.Activity));
                //头像设置
                Participant.HeadPortrait = btimap;
                //是否在线
                if (strState.Equals(notOnlineAboutLync) || strState.Equals(notOnlineAboutSky))
                {
                    //不在线，使用默认的字体和画刷
                    Participant.LoginState = notOnlineShow;
                    Participant.StateForeBrush = NormalColorBrush;
                    Participant.HeadColor = GrayColor;
                }
                else
                {
                    //在线，使用醒目的字体和画刷
                    Participant.LoginState = onlineShow;
                    Participant.StateForeBrush = GreenColorBrush;
                    Participant.HeadColor = GreenColor;
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

        #region 获取当前用户名

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        /// <param name="callBack">函数回调</param>
        public static void SetCurrentUser(Action<string, string> callBack)
        {
            try
            {
                #region 设置当前用户名和邮箱（全局使用）


                //设置当前用户名和当前邮箱地址
                if (ConversationCodeEnterEntity.lyncClient.Self.Contact != null)
                {
                    string selfName = "@";
                    while (selfName.Contains("@"))
                    {
                        //设置当前参会人名称
                        selfName = Convert.ToString(ConversationCodeEnterEntity.lyncClient.Self.Contact.GetContactInformation(ContactInformationType.DisplayName));
                    }
                    //当前参会人
                    ConversationCodeEnterEntity.SelfName = selfName;
                    //设置当前参会人邮箱地址
                    ConversationCodeEnterEntity.SelfUri = ConversationCodeEnterEntity.lyncClient.Self.Contact.Uri.Replace("sip:", string.Empty);
                    callBack(selfName, ConversationCodeEnterEntity.SelfUri);
                }

                #endregion
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

        #region 单独获取用户名称

        /// <summary>
        /// 单独获取用户名称
        /// </summary>
        /// <param name="uri">联系人邮箱地址</param>
        /// <returns>用户名称</returns>
        public static string GetUserName(string uri)
        {
            string userName = null;
            try
            {
                if (!string.IsNullOrEmpty(uri) && uri.Contains("com"))
                {

                    //获取联系人
                    Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(uri);
                    //获取联系人的显示名称
                    userName = Convert.ToString(contact.GetContactInformation(ContactInformationType.DisplayName));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
            return userName;
        }

        #endregion

        #region 添加参会人为常用联系人

        /// <summary>
        /// 填充参会人信息
        /// </summary>
        public static void AddContacts()
        {
            #region 填充参会人信息

            try
            {
                if (ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedIn)
                {
                    //第一次加载参会人
                    bool IsFirstInitNeedWait = false;

                    lyncContactManage = new LyncContactManage(ConversationCodeEnterEntity.contactManager);

                    int timerSpan = 0;
                    foreach (var Participant in ConversationCodeEnterEntity.ParticipantList)
                    {
                        //获取联系人
                        Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(Participant);

                        if (!lyncContactManage.OtherContactsGroup.Contains(contact) && !ConversationCodeEnterEntity.lyncClient.Self.Contact.Equals(contact))
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
                    Contact contactBigScreen = ConversationCodeEnterEntity.contactManager.GetContactByUri(ConversationCodeEnterEntity.BigScreenName);
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

                        foreach (var Participant in ConversationCodeEnterEntity.ParticipantList)
                        {
                            //获取联系人
                            Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(Participant);
                            //获取联系人的显示名称
                            string displayName = Convert.ToString(contact.GetContactInformation(ContactInformationType.DisplayName));
                            //参会人信息字典加载参会人
                            if (!ConversationCodeEnterEntity.DicParticipant.ContainsKey(Participant))
                            {
                                ConversationCodeEnterEntity.DicParticipant.Add(Participant, displayName);
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


        #region 获取某联系人的指定消息

        /// <summary>
        /// 获取某联系人的指定消息
        /// </summary>
        /// <returns></returns>
        public static string GetInformationAcording(Contact contact, ContactInformationType contactInformationType)
        {
            string info = string.Empty;
            try
            {
                if (ConversationCodeEnterEntity.lyncClient != null)
                {
                    info = Convert.ToString(contact.GetContactInformation(contactInformationType));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
            return info;
        }

        /// <summary>
        /// 获取某联系人的指定消息
        /// </summary>
        /// <returns></returns>
        public static string GetInformationAcording(string address, ContactInformationType contactInformationType)
        {
            string info = string.Empty;
            try
            {
                if (ConversationCodeEnterEntity.lyncClient != null)
                {
                   Contact contact = ConversationCodeEnterEntity.lyncClient.ContactManager.GetContactByUri(address);
                    info = Convert.ToString(contact.GetContactInformation(contactInformationType));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
            return info;
        }

        #endregion
    }
}
