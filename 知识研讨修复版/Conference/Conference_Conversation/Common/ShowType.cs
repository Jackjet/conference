using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_Conversation.Common
{
    public enum ShowType
    {
        /// <summary>
        /// 正常共享视图
        /// </summary>
        ConversationView,
        /// <summary>
        /// 当前参会人共享桌面视图
        /// </summary>
        SelfDeskTopShowView,
        /// <summary>
        /// 隐藏视图
        /// </summary>
        HidenView,
        /// <summary>
        /// 未知
        /// </summary>
        None,
    }
}
