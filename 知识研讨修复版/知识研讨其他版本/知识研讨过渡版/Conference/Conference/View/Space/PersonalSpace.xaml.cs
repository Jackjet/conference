﻿using ConferenceCommon.TimerHelper;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WebHelper;
using mshtml;
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

namespace Conference.View.Space
{
    /// <summary>
    /// PersonalSpace.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalSpace : SpaceBase
    {
        #region 构造函数

        public PersonalSpace()
        {
            try
            {
                //UI加载
                InitializeComponent();


                #region 书架映射区域

                //书架
                base.GridBook = this.gridBook;
                //面包屑
                base.BreadLineRoot = this.breadLineRoot;
                //书架主题
                base.GridBookParent = this.gridParent;
                //左箭头
                base.BtnArrowLeft = this.btnArrowLeft;
                //右箭头
                base.BtnArrowRight = this.btnArrowRight;
                //资源推送
                base.BtnResourceSend = this.btnResourceSend;
                //资源演示
                base.BtnResourceShare = this.btnResourceShare;
                //资源下载
                base.BtnResourceDownLoad = this.btnResourceDownLoad;
                //资源删除
                base.BtnResourceDelete = this.btnResourceDelete;
                //资源移动
                base.BtnResourceMove = this.btnResourceMove;
                //资源重命名
                base.BtnResourceReName = this.btnResourceReName;
                //文件上传
                base.BtnResourceUpload = this.btnResourceUpload;

                #endregion

                //加载提示
                base.Loading = this.loading;

                //面板（存储本地应用）
                base.Panel = this.panel;
                //winform控件的宿主
                base.Host = this.host;
                //装饰
                base.BorDecorate = this.borDecorate;
                //视频
                base.BorContent = this.borContent;


                //列表名称
                base.listName = Constant.PesonalFolderName;
                //文件夹名称
                base.folderName = Constant.LoginUserName;
                //根目录名称
                base.root1 = Constant.SelfName;
                //智存空间入口点
                base.SpaceBaseMainStart();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion
    }
}
