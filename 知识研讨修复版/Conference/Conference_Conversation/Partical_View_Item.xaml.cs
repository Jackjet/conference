using Conference_Conversation.Common;
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
    /// Partical_View_Item.xaml 的交互逻辑
    /// </summary>
    public partial class Partical_View_Item : UserControl
    {
        #region 字段
        
        /// <summary>
        /// 联系人
        /// </summary>
        internal Participant Self_Participant = null;

        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="participant"></param>
        public Partical_View_Item(Participant participant)
        {
            try
            {
                this.Self_Participant = participant;
                InitializeComponent();
                //初始联系人信息
                ParticalItemInit(participant);
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

        #region 初始化联系人信息

        /// <summary>
        /// 初始化联系人信息
        /// </summary>
        /// <param name="participant"></param>
        private void ParticalItemInit(Participant participant)
        {
              try
            {
                Contact contact = participant.Contact;
                //登录名称
                string name = contact.Uri.Replace("sip:", string.Empty).Replace(ConversationCodeEnterEntity.UserDomain, string.Empty).Replace("@", string.Empty);
                //获取相关人的头像地址
                string uriImg = ConversationCodeEnterEntity.TreeServiceAddressFront + ConversationCodeEnterEntity.FtpServercePersonImgName + name + ".png";
                //生成专用图片
                BitmapImage btimap = new BitmapImage(new Uri(uriImg));
                //加载图片
                this.borImg.Background = new ImageBrush(btimap);

                //获取当前人的公司名称
                this.txtName.Text = Convert.ToString(contact.GetContactInformation(ContactInformationType.DisplayName));
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
