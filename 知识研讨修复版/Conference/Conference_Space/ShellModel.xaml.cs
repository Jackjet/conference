using ConferenceCommon.LogHelper;
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

namespace Conference_Space
{
    /// <summary>
    /// ShellModel.xaml 的交互逻辑
    /// </summary>
    public partial class ShellModel : Grid
    {
        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ShellModel()
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
      
        #region 书架模式

        /// <summary>
        /// 书架两行模式
        /// </summary>
        public void SetTwoRowShellModel()
        {
              try
            {
                //合理布局，以达到书本与书架之间的磨合【添加4行】
                this.RowDefinitions.Add(new RowDefinition());
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
                this.RowDefinitions.Add(new RowDefinition());
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(13) });
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
        /// 书架单行模式
        /// </summary>
        public void SetOneRowShellModel()
        {
            try
            {
                //合理布局，以达到书本与书架之间的磨合【添加2行】
                this.RowDefinitions.Add(new RowDefinition());
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(17) });
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

        #region 书架添加列

        /// <summary>
        /// 书架添加列
        /// </summary>
        public  void AddNewColumn()
        {
             try
            {
                //添加一列
                this.ColumnDefinitions.Add(new ColumnDefinition());
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
