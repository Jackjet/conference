using ConferenceCommon.LogHelper;
using Microsoft.Lync.Model;
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

namespace Conference.View.Resource
{
    /// <summary>
    /// 会话管理使用的联系人卡片
    /// </summary>
    public partial class ContactCustomCard : UserControl
    {
        #region 静态字段

        /// <summary>
        /// 选取的联系人集合
        /// </summary>
        public static List<string> ContactCustomCardSourceList = new List<string>();

        ///// <summary>
        ///// 选取的联系人名称集合
        ///// </summary>
        //public static List<string> ContactCustomCardList = new List<string>();

        #endregion

        #region 一般属性

        string source;
        /// <summary>
        /// 指定会议人
        /// </summary>
        public string Source
        {
            get { return source; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //CardItem.ForColor_Self = new SolidColorBrush(Colors.Black);
                    //CardItem.Source = value;

                    //this.TextFill(value);

                    source = value;
                }
            }
        }

       
        ///// <summary>
        ///// 会话管理使用的联系人卡片的显示名称
        ///// </summary>
        //public string DisplayName
        //{
        //    get { return this.CardItem.DisplayName; }
        //}

        #endregion

        #region 构造函数

        public ContactCustomCard()
        {
            InitializeComponent();
        }

        #endregion

        #region old solution[显示当前用户友好名称]

        /// <summary>
        /// 显示当前用户友好名称
        /// </summary>
        public void TextFill(string uri)
        {
            string displsyName = string.Empty;
            try
            {
                ////首先判断lync是否可以用（签入）
                //if (Constant.lyncClient != null && Constant.lyncClient.State == Microsoft.Lync.Model.ClientState.SignedIn)
                //{
                //    //获取联系人
                //    Contact contact = Constant.lyncClient.ContactManager.GetContactByUri(uri);
                //    //获取联系人的显示名称
                //    displsyName = Convert.ToString(contact.GetContactInformation(ContactInformationType.DisplayName));


                //    //this.txtDisplayName.Text = t;
                //}
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 联系人选取

        /// <summary>
        /// 联系人获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ContactCustomCard.ContactCustomCardSourceList.Contains(this.source))
                {
                    ContactCustomCard.ContactCustomCardSourceList.Add(this.source);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 联系人取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ContactCustomCard.ContactCustomCardSourceList.Contains(this.source))
                {
                    ContactCustomCard.ContactCustomCardSourceList.Remove(this.source);
                }              
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
