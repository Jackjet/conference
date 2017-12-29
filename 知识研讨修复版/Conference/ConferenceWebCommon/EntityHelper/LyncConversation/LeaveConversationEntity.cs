using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityHelper.LyncConversation
{
     [Serializable]
    public class LeaveConversationEntity
    {
         /// <summary>
         /// 离开的参会人
         /// </summary>
         public string ContactUri { get; set; }
     
    }
}
