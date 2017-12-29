using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_IMM.Common
{
    public enum AudioItemType
    {
        /// <summary>
        /// 带语音（有权限）
        /// </summary>
        LimitHasAudio,
        /// <summary>
        /// 不带语音（有权限）
        /// </summary>
        LimitNoAudio,
        /// <summary>
        /// 带语音（无权限）
        /// </summary>
        NoLimitHasAudio,
        /// <summary>
        /// 不带语音（无权限）
        /// </summary>
        NoLimitNoAudio,
        /// <summary>
        /// 移除上传提示
        /// </summary>
        DeleteTipControl
    }
}
