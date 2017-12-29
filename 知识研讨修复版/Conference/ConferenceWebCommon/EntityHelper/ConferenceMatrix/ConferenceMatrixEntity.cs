using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityHelper.ConferenceMatrix
{
    [Serializable]
    public class ConferenceMatrixEntity : ConferenceMatrixBase
    {
        /// <summary>
        /// 分享者
        /// </summary>
        public string Sharer { get; set; }

        /// <summary>
        /// 输出口
        /// </summary>
        public ConferenceMatrixOutPut ConferenceMatrixOutPut { get; set; }
    }
}
