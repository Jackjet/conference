using Conference_MyConference.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
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
using studiom = Studiom_Model.Pro_KnowledgeService;
using maxtrix = ConferenceModel.ConferenceMatrixWebservice;
using webCommonMaxtrix = ConferenceWebCommon.EntityHelper.ConferenceMatrix;

namespace Conference_MyConference
{
    public class SeatPanelBase:UserControl
    {
        #region 内部字段

        /// <summary>
        /// 座位字典集1
        /// </summary>
       protected Dictionary<int, NavicateButton> dicSeatButton1 = new Dictionary<int, NavicateButton>();

        /// <summary>
        /// 座位字典集2
        /// </summary>
       protected Dictionary<int, NavicateButton> dicSeatButton2 = new Dictionary<int, NavicateButton>();

        /// <summary>
        /// 自己的座位
        /// </summary>
       protected maxtrix.SeatEntity selfSeatEntity = null;

        #endregion

       #region 控件映射

        /// <summary>
        /// 桌位面板
        /// </summary>
       protected Grid GridSeatPanel = null;

        /// <summary>
        /// 大屏
        /// </summary>
       protected Border BorScreen1 = null;

       /// <summary>
       /// 小屏
       /// </summary>
       protected Border BorScreen2 = null;

       #endregion

       #region 座位初始化

       /// <summary>
       /// 座位初始化
       /// </summary>
       public void SeatDataFill()
       {
           try
           {
               //座位数量统计
               int count = this.GridSeatPanel.Children.Count;

               //遍历同一面板的所有座位并进行加载
               for (int i = 0; i < count; i++)
               {
                   //获取元素
                   var element = this.GridSeatPanel.Children[i];
                   if (element.GetType() == typeof(NavicateButton))
                   {
                       //类型还原
                       NavicateButton btn = element as NavicateButton;

                       if (i < count / 2)
                       {
                           //记录
                           this.dicSeatButton1.Add(i + 1, btn);
                       }
                       else
                       {
                           this.dicSeatButton2.Add(i + 1, btn);
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

       #region 刷新座位分布情况

       /// <summary>
       /// 刷新座位分布情况
       /// </summary>
       public void Reflesh(List<maxtrix.SeatEntity> seatEntity)
       {
           try
           {
               //座位数量统计
               int count = this.GridSeatPanel.Children.Count;
               //当前座位列表
               List<maxtrix.SeatEntity> selfSeatEntityList = seatEntity.Where(Item => Item.UserName != null &&
                     Item.UserName.Equals(MyConferenceCodeEnterEntity.SelfName)).ToList();
               if (selfSeatEntityList.Count > 0)
               {
                   this.selfSeatEntity = selfSeatEntityList[0];
               }
               int halfCount = count / 2;
               if (this.selfSeatEntity != null && this.selfSeatEntity.SettingNummber <= halfCount)
               {
                   //正级桌位初始化
                   this.Seat_Init_Plus(seatEntity, halfCount);
               }
               else
               {
                   //负级桌位初始化
                   this.Seat_Init_Neg(seatEntity, halfCount);
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

       #region 正级桌位初始化

       /// <summary>
       /// 正级桌位初始化
       /// </summary>
       /// <param name="seatEntity">座位列表</param>
       /// <param name="count">座位数量</param>
       private void Seat_Init_Plus(List<maxtrix.SeatEntity> seatEntity, int halfCount)
       {
           try
           {
               //遍历设置座位
               foreach (var item in seatEntity)
               {
                   int number = item.SettingNummber;
                   string content = "座位" + number;
                   if (!string.IsNullOrEmpty(item.UserName))
                   {
                       content = item.UserName;
                   }
                   if (number <= halfCount)
                   {
                       if (this.dicSeatButton1.ContainsKey(number))
                       {
                           NavicateButton btnd = this.dicSeatButton1[number];
                           btnd.Content = content;
                           btnd.IntType = item.SettingNummber;
                       }
                   }
                   else
                   {
                       if (this.dicSeatButton2.ContainsKey(number))
                       {
                           NavicateButton btnd = this.dicSeatButton2[number];
                           btnd.Content = content;
                           btnd.IntType = item.SettingNummber;
                       }
                   }
               }
               this.BorScreen1.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
               if(this.BorScreen2!= null)
               {
                   this.BorScreen2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
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

       #region 负级桌位初始化

       /// <summary>
       /// 负级桌位初始化
       /// </summary>
       /// <param name="seatEntity">座位列表</param>
       /// <param name="halfCount">座位数量的一半</param>
       private void Seat_Init_Neg(List<maxtrix.SeatEntity> seatEntity, int halfCount)
       {
           try
           {
               //遍历设置座位
               foreach (var item in seatEntity)
               {
                   int number = item.SettingNummber;
                   string content = "座位" + number;
                   if (!string.IsNullOrEmpty(item.UserName))
                   {
                       content = item.UserName;
                   }
                   if (number <= halfCount)
                   {
                       if (this.dicSeatButton2.ContainsKey(number + halfCount))
                       {
                           NavicateButton btnd = this.dicSeatButton2[number + halfCount];
                           btnd.Content = content;
                           btnd.IntType = item.SettingNummber;
                       }
                   }
                   else
                   {
                       if (this.dicSeatButton1.ContainsKey(number - halfCount))
                       {
                           NavicateButton btnd = this.dicSeatButton1[number - halfCount];
                           btnd.Content = content;
                           btnd.IntType = item.SettingNummber;
                       }
                   }
               }
               this.BorScreen1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
               if (this.BorScreen2 != null)
               {
                   this.BorScreen2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
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

       #region 更改一个座位信息（自己或其他参会人）

       /// <summary>
       /// 更改一个座位信息（自己或其他参会人）
       /// </summary>
       public void UpdateOneSeat(webCommonMaxtrix.SeatEntity seatEntity)
       {
           try
           {
               //座位数量统计
               int count = this.GridSeatPanel.Children.Count;

               if (this.selfSeatEntity != null)
               {
                   int number = seatEntity.SettingNummber;
                   if (this.selfSeatEntity.SettingNummber <= count / 2)
                   {

                       if (number <= count / 2)
                       {
                           this.dicSeatButton1[number].Content = seatEntity.UserName;
                       }
                       else
                       {
                           this.dicSeatButton2[number].Content = seatEntity.UserName;
                       }
                   }
                   else
                   {
                       if (number <= count / 2)
                       {
                           //座位对应名称设置
                           this.dicSeatButton2[number + count / 2].Content = seatEntity.UserName;
                       }
                       else
                       {
                           //座位对应名称设置
                           this.dicSeatButton1[number - count / 2].Content = seatEntity.UserName;
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

    }
}
