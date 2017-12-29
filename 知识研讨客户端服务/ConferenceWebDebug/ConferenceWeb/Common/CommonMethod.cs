using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ConferenceWeb.Common
{
    public static class CommonMethod
    {
        /// <summary>
        /// 实体转json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(obj);
            return json;
        }
    }
}
