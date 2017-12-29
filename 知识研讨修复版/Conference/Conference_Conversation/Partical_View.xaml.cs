using Conference_Conversation.Common;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.LogHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
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

namespace Conference_Conversation
{
    /// <summary>
    /// Partical_View.xaml 的交互逻辑
    /// </summary>
    public partial class Partical_View : UserControl
    {
        #region 字段
        
        /// <summary>
        /// 联系人列表
        /// </summary>
        Dictionary<Participant, Partical_View_Item> dicParticipant = new Dictionary<Participant, Partical_View_Item>();

        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public Partical_View()
        {
            try
            {
                InitializeComponent();
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

        #region 添加子项
        
        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="participant"></param>
        public void AddItem(Participant participant)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    if (!this.dicParticipant.ContainsKey(participant))
                    {
                        Partical_View_Item partical_View_Item = new Partical_View_Item(participant);
                        this.dicParticipant.Add(participant, partical_View_Item);
                        this.stackPanel.Children.Add(partical_View_Item);
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

        #region 移除子项
        
        /// <summary>
        /// 移除子项
        /// </summary>
        /// <param name="participant"></param>
        public void RemoveItem(Participant participant)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    if (this.dicParticipant.ContainsKey(participant))
                    {
                        Partical_View_Item Partical_View_Item = dicParticipant[participant];
                        if (stackPanel.Children.Contains(Partical_View_Item))
                        {
                            this.stackPanel.Children.Remove(Partical_View_Item);
                        }
                        this.dicParticipant.Remove(participant);
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

        #region 联系人列表初始化
        
        /// <summary>
        /// 联系人列表初始化
        /// </summary>
        public void ParticalListInit()
        {
              try
            {
               if(LyncHelper.MainConversation!= null)
               {
                   this.stackPanel.Children.Clear();
                   Conversation conversation = LyncHelper.MainConversation.Conversation;

                   foreach (var item in conversation.Participants)
                   {
                       this.AddItem(item);
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
