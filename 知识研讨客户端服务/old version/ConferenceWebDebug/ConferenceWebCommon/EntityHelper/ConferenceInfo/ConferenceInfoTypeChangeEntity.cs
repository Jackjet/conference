using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityHelper.ConferenceInfo
{
    [Serializable]
    public class ConferenceInfoTypeChangeEntity
    {
        bool isSimpleModel;
        /// <summary>
        /// 经典标准模式切换
        /// </summary>
        public bool IsSimpleModel
        {
            get { return isSimpleModel; }
            set { isSimpleModel = value; }
        }

        bool isEducationModel;
        /// <summary>
        /// 教育模式
        /// </summary>
        public bool IsEducationModel
        {
            get { return isEducationModel; }
            set { isEducationModel = value; }
        }

    }
}
