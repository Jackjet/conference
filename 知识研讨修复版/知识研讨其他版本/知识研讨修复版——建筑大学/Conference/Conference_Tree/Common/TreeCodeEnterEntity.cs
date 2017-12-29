using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace Conference_Tree
{
    public class TreeCodeEnterEntity
    {
        static TreeCodeEnterEntity treeCodeEnterEntityInstance = new TreeCodeEnterEntity();
        /// <summary>
        /// 自身节点
        /// </summary>
        public static TreeCodeEnterEntity TreeCodeEnterEntityInstance
        {
            get { return TreeCodeEnterEntity.treeCodeEnterEntityInstance; }
            set { TreeCodeEnterEntity.treeCodeEnterEntityInstance = value; }
        }

        /// <summary>
        /// 会议信息
        /// </summary>
        public string ConferenceName { get; set; }

        /// <summary>
        /// 是否为会议主持人
        /// </summary>
        public bool IsMeetPresenter { get; set; }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        public Window MainWindow { get; set; }

        /// <summary>
        /// 节点默认名称
        /// </summary>
        public string TreeItemEmptyName { get; set; }

        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginUserName { get; set; }

        /// <summary>
        /// 当前用户的uri地址
        /// </summary>
        public string SelfUri { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string SelfName { get; set; }
    }
}
