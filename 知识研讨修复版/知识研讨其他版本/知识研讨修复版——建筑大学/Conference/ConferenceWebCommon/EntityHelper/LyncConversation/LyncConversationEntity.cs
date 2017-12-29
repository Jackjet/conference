using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityHelper.LyncConversation
{
    [Serializable]
    public class LyncConversationEntity
    {
        string jonConferencePerson;
        /// <summary>
        /// 参会人
        /// </summary>
        public string JonConferencePerson
        {
            get { return jonConferencePerson; }
            set { jonConferencePerson = value; }
        }
    }
}
