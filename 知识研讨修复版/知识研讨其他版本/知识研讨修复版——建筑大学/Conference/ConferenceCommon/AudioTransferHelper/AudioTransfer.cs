using ConferenceCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ConferenceCommon.AudioTransferHelper
{
    public class AudioTransfer
    {
        /// <summary>
        /// 语音服务请求
        /// </summary>
        static HttpWebRequest req = null;
        /// <summary>
        /// 用户ID
        /// </summary>
        static string userId = "YZS1427959866053";

        /// <summary>
        /// 设备ID
        /// </summary>
        static string deviceId = "IMEI1234567890";

        public static string AudioToText(byte[] bytes)
        {
            string result = null;
            try
            {
                string message = string.Empty;
                if (bytes != null)
                {
                    //if (req == null)
                    //{
                        req = (HttpWebRequest)HttpWebRequest.Create("http://api.hivoice.cn/USCService/WebApi?" + "appkey=" + "p2xqpaimmmn7bff5rvatfeopt2gyaibnfwtuyzqt" + "&userid=" + userId + "&id=" + deviceId);
                        req.Method = "POST";
                        req.ContentType = "audio/x-wav;codec=pcm;bit=16;rate=16000";

                        req.Accept = "text/plain";
                        req.Headers.Add("Accept-Language", "zh_CN");
                        req.Headers.Add("Accept-Charset", "utf-8");
                        req.Headers.Add("Accept-Topic", "general");
                    //}

                    req.ContentLength = bytes.Length;

                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                    }
                    using (WebResponse wr = req.GetResponse())
                    {
                        HttpWebResponse response = wr as HttpWebResponse;
                        Stream responseStream = response.GetResponseStream();
                        Encoding encoding = Encoding.GetEncoding("utf-8");
                        StreamReader streamReader = new StreamReader(responseStream, encoding);
                        while ((message = streamReader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(message) && !message.Contains("无法识别"))
                            {
                                result = message;
                            }
                        }
                        streamReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(AudioTransfer), ex);
            }
            return result;
        }

        public static void RequestInit()
        {


        }
    }
}
