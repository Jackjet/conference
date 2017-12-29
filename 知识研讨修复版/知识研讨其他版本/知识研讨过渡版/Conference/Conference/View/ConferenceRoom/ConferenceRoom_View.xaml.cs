using Conference.Common;
using Conference.Page;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.JsonHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceModel;
using ConferenceModel.ConferenceInfoWebService;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference.View.ConferenceRoom
{
    /// <summary>
    /// ConferenceRoom_View.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceRoom_View : UserControl
    {
        #region 自定义事件

        public delegate void ItemClickEventHandle(ConferenceInformationEntityPC conferenceInformationEntity, Action callBack);
        /// <summary>
        /// 会议点击事件
        /// </summary>
        public event ItemClickEventHandle ItemClickCallBackToMainPage = null;

        #endregion

        #region 一般属性

        List<ConferenceInformationEntityPC> conferenceInformationEntityList = new List<ConferenceInformationEntityPC>();
        /// <summary>
        /// 会议信息集合
        /// </summary>
        public List<ConferenceInformationEntityPC> ConferenceInformationEntityList
        {
            get { return conferenceInformationEntityList; }
            set { conferenceInformationEntityList = value; }
        }


        ConferenceRoomItem currentConferenceRoomItem = null;
        /// <summary>
        /// 当前选择的会议聊天室
        /// </summary>
        public ConferenceRoomItem CurrentConferenceRoomItem
        {
            get { return currentConferenceRoomItem; }
            set { currentConferenceRoomItem = value; }
        }

        #endregion

        #region 构造函数

        public ConferenceRoom_View()
        {
            try
            {
                //UI加载
                InitializeComponent();

                this.Loaded += ConferenceRoom_View_Loaded;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        void ConferenceRoom_View_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MesureSize();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }


        #region 获取会议信息（会议室）

        /// <summary>
        /// 获取会议信息（会议室）
        /// </summary>
        public void ConferenceRoomFlesh(Action action)
        {
            try
            {
                ModelManage.ConferenceInfo.GetConferenceInformation(Constant.WebLoginUserName, new Action<bool, List<ConferenceInformationEntityPC>>((successed, json) =>
                {
                    //获取会议信息返回标示
                    if (successed)
                    {
                        //清除会议信息集合
                        this.ConferenceInformationEntityList.Clear();
                        //json转换为对应的实体
                        this.ConferenceInformationEntityList = json;
                        //遍历会议信息
                        foreach (var item in ConferenceInformationEntityList)
                        {
                            //若为该会议的成员则加载该条会议
                            if (item.JoinPeople.Contains(Constant.SelfUri))
                            {
                                //创建一个会议视图子项
                                ConferenceRoomItem roomItem = new ConferenceRoomItem(item);
                                //视图子项点击事件
                                roomItem.ItemClick += () =>
                                {
                                    this.RoomCardInit(roomItem,item);
                                };
                                //添加子项
                                this.wrapPanle.Children.Add(roomItem);
                            }
                        }
                        //回调
                        action();
                    }
                    else
                    {
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 会议卡片加载
        /// </summary>
        public void RoomCardInit(ConferenceRoomItem roomItem, ConferenceInformationEntityPC item)
        {
             try
            {
                if (this.ItemClickCallBackToMainPage != null)
                {
                    #region old solution

                    //if(Constant.lyncClient.ConversationManager.Conversations.Count>0)
                    //{
                    //    Constant.lyncClient.ConversationManager.Conversations.Clear();
                    //}

                    #endregion

                    //离开会话
                    if (LyncHelper.MainConversation != null)
                    {
                        //离开当前会话
                        //LyncHelper.MainConversation.Close();
                        LyncHelper.MainConversation.Close();
                        foreach (var conversation in Constant.lyncClient.ConversationManager.Conversations)
                        {
                            ConversationWindow window = Constant.lyncAutomation.GetConversationWindow(conversation);
                           window.Close();
                        }

                        ModelManage.ConferenceLyncConversation.LeaveConversation(Constant.ConferenceName, Constant.SelfUri, new Action<bool>((isSuccessed) =>
                        {

                        }));
                        LyncHelper.MainConversation = null;
                    }

                    //当前选择的会议聊天室
                    if (this.CurrentConferenceRoomItem != null)
                    {
                        this.CurrentConferenceRoomItem.StyleChangeToNoSelected();
                    }
                    //临时存储的会议信息
                    MainWindow.MainPageInstance.TempConferenceInformationEntity = item;
                    this.CurrentConferenceRoomItem = roomItem;
                    this.CurrentConferenceRoomItem.StyleChangeToSelected();

                    MainWindow.MainPageInstance.MyConferenceView.TipShow(true);

                    //点击回调
                    this.ItemClickCallBackToMainPage(item, new Action(() =>
                    {
                        //刷新数据
                        MainWindow.MainPageInstance.MyConferenceView.FillDataSource();
                        MainWindow.MainPageInstance.MyConferenceView.TipShow(false);
                    }));
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

        #endregion

        #region 移除会议室

        /// <summary>
        /// 移除会议室(清空)
        /// </summary>
        public void ClearRoom()
        {
            try
            {
                //移除会议室(清空)
                this.wrapPanle.Children.Clear();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 筛选会议室

        public void RoomItemForceToSelectedStyle(ConferenceInformationEntityPC conferenceInformationEntityPC)
        {
            try
            {
                foreach (var item in this.wrapPanle.Children)
                {
                    if (item is ConferenceRoomItem)
                    {
                        ConferenceRoomItem conferenceRoomItem = item as ConferenceRoomItem;
                        if (conferenceInformationEntityPC.MeetingName.Equals(conferenceRoomItem.ConferenceInformationEntity.MeetingName))
                        {
                            //当前选择的会议聊天室
                            if (this.CurrentConferenceRoomItem != null)
                            {
                                this.CurrentConferenceRoomItem.StyleChangeToNoSelected();
                            }
                            //临时存储的会议信息
                            MainWindow.MainPageInstance.TempConferenceInformationEntity = conferenceInformationEntityPC;
                            this.CurrentConferenceRoomItem = conferenceRoomItem;
                            this.CurrentConferenceRoomItem.StyleChangeToSelected();
                        }
                    }
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

        #region 调整大小

        public void MesureSize()
        {
            try
            {
                if (this.wrapPanle.Children.Count > 0)
                {
                    this.borD.Width = this.ActualWidth - (this.wrapPanle.Children[0] as FrameworkElement).ActualWidth * this.wrapPanle.Children.Count -30;
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
