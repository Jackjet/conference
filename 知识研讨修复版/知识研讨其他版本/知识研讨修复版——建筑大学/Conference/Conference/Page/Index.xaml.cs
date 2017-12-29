using Conference.Common;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
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

namespace Conference.Page
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : UserControl
    {
        #region 自定义委托事件

        public delegate void IndexItemSelectedEventHandle(ViewSelectedItemEnum viewSelectedItemEnum);
        /// <summary>
        /// 首页选择事件
        /// </summary>
        public event IndexItemSelectedEventHandle IndexItemSelected = null;

        /// <summary>
        /// 事件激发器
        /// </summary>
        public void OnIndexItemSelected(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //首页子项选择事件
                if (this.IndexItemSelected != null)
                {
                    this.IndexItemSelected(viewSelectedItemEnum);
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

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public Index()
        {
            try
            {
                //UI初始化
                InitializeComponent();

                //一般事件注册
                NormalEventRegedit();

                //精简模式事件注册
                SimpleModelEventRegedit();

                //教学模式事件注册
                EducationModelEventRegedit();
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

        #region 事件注册区域

        /// <summary>
        /// 一般事件注册
        /// </summary>
        private void NormalEventRegedit()
        {
            try
            {
                //我的会议
                this.btnMeet.Click += Navacite;
                //u盘传输
                this.btn_U_disk.Click += Navacite;
                //主持功能
                this.btnChair.Click += Navacite;

                //信息交流
                this.btnIMM.Click += Navacite;
                //共享资源
                this.btnResource.Click += Navacite;
                //智存空间
                this.btnSpace.Click += Navacite;
                //中控功能
                this.btnStudiom.Click += Navacite;
                //系统设置
                this.btnSystemSetting.Click += Navacite;
                //知识树
                this.btnTree.Click += Navacite;
                //会议投票
                this.btnVote.Click += Navacite;
                //个人笔记
                this.btnNote.Click += Navacite;
                //会议切换
                this.btnMeet_Change.Click += Navacite;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 精简模式事件注册
        /// </summary>
        private void SimpleModelEventRegedit()
        {
            try
            {
                //我的会议
                this.btnMeet2.Click += Navacite;
                //信息交流
                this.btnIMM2.Click += Navacite;
                //共享资源
                this.btnResource2.Click += Navacite;
                //智存空间
                this.btnSpace2.Click += Navacite;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 教学模式事件注册
        /// </summary>
        private void EducationModelEventRegedit()
        {
            try
            {
                //我的课堂
                this.btnMeetEducation.Click += Navacite;
                //互动课堂
                this.btnResourceEducation.Click += Navacite;
                //教学资料
                this.btnSpaceEducation.Click += Navacite;
                //课程大纲
                this.btnTreeEducation.Click += Navacite;
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

        #region 导航控制中心

        public void Navacite(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.OnIndexItemSelected(navicateButton.ViewSelectedItemEnum);
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

        #region 会议切换

        /// <summary>
        /// 会议切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMeet_Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.OnIndexItemSelected(navicateButton.ViewSelectedItemEnum);
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

        #region 模式切换

        /// <summary>
        /// 精简与标准模式
        /// </summary>
        public void ViewModelChangedSimple(bool isSimpleModel)
        {
            try
            {
                if (isSimpleModel)
                {
                    this.gridStandardModelView.Visibility = System.Windows.Visibility.Collapsed;
                    this.gridSampleModelView.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.gridStandardModelView.Visibility = System.Windows.Visibility.Visible;
                    this.gridSampleModelView.Visibility = System.Windows.Visibility.Collapsed;
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

        /// <summary>
        /// 教育模式
        /// </summary>
        public void ViewModelChangedEducation(bool isEducationModel)
        {
            try
            {
                if (isEducationModel)
                {
                    this.gridEducationModelView.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.gridEducationModelView.Visibility = System.Windows.Visibility.Collapsed;
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
