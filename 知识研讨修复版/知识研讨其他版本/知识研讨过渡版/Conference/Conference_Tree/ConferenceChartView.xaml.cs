
using ConferenceCommon.EntityHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Visifire.Charts;

namespace Conference_Tree
{
    /// <summary>
    /// ConferenceChart.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceChartView : UserControlBase
    {
        #region 字段

        /// <summary>
        /// 图表类型
        /// </summary>
        RenderAs render = RenderAs.Column;

        /// <summary>
        /// 标题集合
        /// </summary>
        List<string> titleList = null;

        #endregion

        #region 绑定属性

        List<RenderAsEntity> renderAsEntityList = null;
        /// <summary>
        /// 图表类型实体集合
        /// </summary>
        public List<RenderAsEntity> RenderAsEntityList
        {
            get { return renderAsEntityList; }
            set
            {
                if (this.renderAsEntityList != value)
                {
                    renderAsEntityList = value;
                    this.OnPropertyChanged("RenderAsEntityList");
                }
            }
        }


        RenderAsEntity selectedRenderAsEntity;
        /// <summary>
        /// 当前所选择的图表类型
        /// </summary>
        public RenderAsEntity SelectedRenderAsEntity
        {
            get { return selectedRenderAsEntity; }
            set
            {
                if (this.selectedRenderAsEntity != value)
                {
                    selectedRenderAsEntity = value;
                    this.OnPropertyChanged("SelectedRenderAsEntity");
                }
            }
        }

        #endregion        

        #region 构造函数

        public ConferenceChartView()
        {
            try
            {
                InitializeComponent();
                //设置当前上下文
                this.DataContext = this;
                //图表类型加载
                this.ParametersInit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 图表类型加载

        /// <summary>
        /// 图表类型加载
        /// </summary>
        public void ParametersInit()
        {
            this.RenderAsEntityList = new List<RenderAsEntity>()
            {
             new RenderAsEntity() { ChartTypeName="柱状图",RenderType= RenderAs.Column},
             //new RenderAsEntity() { ChartTypeName="饼图",RenderType=RenderAs.Pie},
             //new RenderAsEntity() { ChartTypeName="折线图",RenderType=RenderAs.Line},
             new RenderAsEntity() { ChartTypeName="条形图",RenderType=RenderAs.Bar},
             //new RenderAsEntity() { ChartTypeName="漏斗形状",RenderType=RenderAs.SectionFunnel},
                };
        }

        #endregion

        #region 图表添加标题(投票)

        public void ChartTittlesAdd(List<string> tittles)
        {
            try
            {
                //标题设置
                this.titleList = tittles;
                //图表重启
                this.chart.Series.Clear();

                //遍历标题集合
                foreach (var item in this.titleList)
                {
                    //生成一个系列
                    DataSeries series = new DataSeries();
                    //类型设置
                    series.RenderAs = this.render;
                    // 显示Lable   
                    series.LabelStyle = LabelStyles.OutSide;
                    //可进行点击
                    series.LabelEnabled = true;
                    //设置图表系列的名称（标题）
                    series.LegendText = item;
                    //图表添加系列
                    this.chart.Series.Add(series);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 图表添加子项（投票）

        /// <summary>
        /// 图表添加子项（投票）
        /// </summary>
        /// <param name="chartItemTittle"></param>
        /// <param name="Agree"></param>
        /// <param name="NoAgree"></param>
        /// <param name="Guid"></param>
        public void ChartItemsAdd(string chartItemTittle, int Agree, int NoAgree, int Guid)
        {
            try
            {
                //图表有两个系列（赞成，反对）
                if (this.chart.Series.Count == 2)
                {
                    //生成一个节点
                    DataPoint dp = new DataPoint();
                    //设置datapoint名称
                    dp.AxisXLabel = chartItemTittle;
                    //设置datapoint的值
                    dp.YValue = Agree;
                    //tag值绑定GUID
                    dp.Tag = Guid;
                    //系列添加节点
                    this.chart.Series[0].DataPoints.Add(dp);

                    //生成一个节点
                    DataPoint dp2 = new DataPoint();
                    //设置datapoint名称
                    dp2.AxisXLabel = chartItemTittle;
                    //设置datapoint的值
                    dp2.YValue = NoAgree;
                    //tag值绑定GUID
                    dp2.Tag = Guid;
                    //系列添加节点
                    this.chart.Series[1].DataPoints.Add(dp2);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 删除所有节点（不包括系列）

        /// <summary>
        /// 删除所有节点（不包括系列）
        /// </summary>
        public void ClearAllPoints()
        {
            try
            {
                for (int i = this.chart.Series.Count - 1; i > -1; i--)
                {
                    this.chart.Series[i].DataPoints.Clear();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 投票树节点删除

        /// <summary>
        /// 投票树节点删除
        /// </summary>
        /// <param name="Guid"></param>
        public void VoteTreeItemRemove(int Guid)
        {
            try
            {
                //遍历删除指定的图表节点（赞成）
                for (int i = this.chart.Series[0].DataPoints.Count - 1; i > -1; i--)
                {
                    //通过GUID来判断删除某个节点
                    var point = this.chart.Series[0].DataPoints[i];
                    if (point.Tag.Equals(Guid))
                    {
                        this.chart.Series[0].DataPoints.RemoveAt(i);
                        break;
                    }
                }

                //遍历删除指定的图表节点（反对）
                for (int i = this.chart.Series[1].DataPoints.Count - 1; i > -1; i--)
                {
                    //通过GUID来判断删除某个节点
                    var point = this.chart.Series[1].DataPoints[i];
                    if (point.Tag.Equals(Guid))
                    {
                        this.chart.Series[1].DataPoints.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 投票树节点更改

        /// <summary>
        /// 投票树节点更改
        /// </summary>
        /// <param name="chartItemTittle"></param>
        /// <param name="Agree"></param>
        /// <param name="NoAgree"></param>
        /// <param name="Guid"></param>
        public void ChartItemsUpdate(string chartItemTittle, int Agree, int NoAgree, int Guid)
        {
            try
            {
                //添加图表节点（赞成）
                foreach (var point in this.chart.Series[0].DataPoints)
                {
                    //通过GUID来判断添加某个节点
                    if (point.Tag.Equals(Guid))
                    {
                        point.YValue = Agree;
                        point.AxisXLabel = chartItemTittle;
                        break;
                    }
                }

                //添加图表节点（反对）
                foreach (var point in this.chart.Series[1].DataPoints)
                {
                    //通过GUID来判断添加某个节点
                    if (point.Tag.Equals(Guid))
                    {
                        point.YValue = NoAgree;
                        point.AxisXLabel = chartItemTittle;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 图表刷新

        /// <summary>
        /// 图表刷新
        /// </summary>
        public void Reflesh()
        {
            try
            {
                //通过遍历系列来更改图表显示类型
                foreach (var series in this.chart.Series)
                {
                    series.RenderAs = this.render;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 图表类型更改（下拉列表）

        /// <summary>
        /// 图表类型更改（下拉列表）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //设置当前所选择的图表类型
                this.render = this.SelectedRenderAsEntity.RenderType;
                //刷新图表
                this.Reflesh();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
