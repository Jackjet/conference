using Conference_MyConference.Common;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.JsonHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceModel;
using ConferenceModel.ConferenceInfoWebService;
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

namespace Conference_MyConference
{
    /// <summary>
    /// ConferenceRoom_View.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceRoom_View : UserControl
    {
        #region 自定义事件回调
     
        /// <summary>
        /// 会议点击回调
        /// </summary>
        public Action<ConferenceInformationEntityPC> ItemClickSendItemCallBack = null;

        /// <summary>
        /// 加载卡片事件回调
        /// </summary>
        public Action AddCardEventCallBack = null;

        #endregion

        #region 静态字段

        /// <summary>
        /// 自我绑定
        /// </summary>
       public static ConferenceRoom_View conferenceRoom_View = null;

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

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConferenceRoom_View()
        {
            try
            {
                //UI加载
                InitializeComponent();
                //自我绑定
                conferenceRoom_View = this;
                //加载事件
                this.Loaded += ConferenceRoom_View_Loaded;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 初始化加载

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        #endregion

        #region 获取会议信息（会议室）

        /// <summary>
        /// 获取会议信息（会议室）
        /// </summary>
        public void ConferenceRoomFlesh(Action action)
        {
            try
            {
                ModelManage.ConferenceInfo.GetConferenceInformation(MyConferenceCodeEnterEntity.WebLoginUserName, new Action<bool, List<ConferenceInformationEntityPC>>((successed, json) =>
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
                            if (item.JoinPeople.Contains(MyConferenceCodeEnterEntity.SelfUri))
                            {
                                //创建一个会议视图子项
                                ConferenceRoomItem roomItem = new ConferenceRoomItem(item);                              
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
                if (this.ItemClickSendItemCallBack != null)
                {
                    if (this.AddCardEventCallBack != null)
                    {
                        this.AddCardEventCallBack();
                    }

                    //当前选择的会议聊天室
                    if (this.CurrentConferenceRoomItem != null)
                    {
                        this.CurrentConferenceRoomItem.StyleChangeToNoSelected();
                    }
                    //临时存储的会议信息
                    MyConferenceCodeEnterEntity.TempConferenceInformationEntity = item;
                    this.CurrentConferenceRoomItem = roomItem;
                    this.CurrentConferenceRoomItem.StyleChangeToSelected();

                    MyConferenceView.myConferenceView.TipShow(true);

                    //点击回调
                    this.ItemClickSendItemCallBack(item);
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

        /// <summary>
        /// 筛选会议室
        /// </summary>
        /// <param name="conferenceInformationEntityPC"></param>
        public void RoomItemForceToSelectedStyle(ConferenceInformationEntityPC conferenceInformationEntityPC)
        {
            try
            {
                foreach (var item in this.wrapPanle.Children)
                {
                    if (item is ConferenceRoomItem)
                    {
                        ConferenceRoomItem conferenceRoomItem = item as ConferenceRoomItem;
                        if (conferenceInformationEntityPC.MeetingName.Equals(conferenceRoomItem.conferenceInformationEntityPC.MeetingName))
                        {
                            //当前选择的会议聊天室
                            if (this.CurrentConferenceRoomItem != null)
                            {
                                this.CurrentConferenceRoomItem.StyleChangeToNoSelected();
                            }
                            //临时存储的会议信息
                            MyConferenceCodeEnterEntity.TempConferenceInformationEntity = conferenceInformationEntityPC;
                            this.CurrentConferenceRoomItem = conferenceRoomItem;
                            this.CurrentConferenceRoomItem.StyleChangeToSelected();
                            break;
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

        /// <summary>
        /// 调整大小
        /// </summary>
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
