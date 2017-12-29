using ConferenceWeb.Common;
using ConferenceWeb.MobilePhoneEntity;
using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;
using ConferenceWebCommon.EntityHelper.ConferenceDiscuss;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Xml;

namespace ConferenceWeb
{
    /// <summary>
    /// ConferenceAudioDiscuss 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceAudioWebservice : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 临时存储（用于校验的实例，没有父节点和子节点，一对一） 
        /// </summary>
        //public static Dictionary<string, ConferenceAudioItemTransferEntity> ConferenceDiscussItemTransferEntity_temp_dic = new Dictionary<string, ConferenceAudioItemTransferEntity>();

        /// <summary>
        /// 后续进入的成员进行一次初始化的加载
        /// </summary>
        public static Dictionary<int, ConferenceAudioInitRefleshEntity> ConferenceDiscussInitRefleshEntity_dic = new Dictionary<int, ConferenceAudioInitRefleshEntity>();

        /// <summary>
        /// 线程锁辅助对象(获取源数据)
        /// </summary>
        static private object objGetAll = new object();

        /// <summary>
        /// 线程锁辅助对象（添加节点）
        /// </summary>
        static private object objAdd = new object();

        /// <summary>
        /// 线程锁辅助对象（添加节点）
        /// </summary>
        static private object objAddByMobile = new object();

        /// <summary>
        /// 线程锁辅助对象（更新节点）
        /// </summary>
        static private object objUpdate = new object();

        /// <summary>
        /// 线程锁辅助对象（更新节点）
        /// </summary>
        static private object objDelete = new object();

        /// <summary>
        /// 线程锁辅助对象（语音转文字）
        /// </summary>
        static private object objTransfer = new object();

        /// <summary>
        /// 线程锁辅助对象（上传完成事件）
        /// </summary>
        static private object objUploadCompleate = new object();

        #endregion

        #region 获取源数据（）

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回语音所有节点</returns>
        [WebMethod]
        public ConferenceAudioInitRefleshEntity GetAll(int conferenceID)
        {
            //上锁,达到线程互斥作用
            lock (objGetAll)
            {
                ConferenceAudioInitRefleshEntity initRefleshEntity = new ConferenceAudioInitRefleshEntity();
                try
                {
                    //会议信息不为null
                    if (conferenceID != 0)
                    {
                        //后续进入的成员进行一次初始化的加载
                        if (ConferenceDiscussInitRefleshEntity_dic.ContainsKey(conferenceID))
                        {
                            //获取整个语音节点
                            initRefleshEntity = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                        }
                        else
                        {
                            //若不存在,添加一个新实例（语音节点）
                            ConferenceDiscussInitRefleshEntity_dic.Add(conferenceID, initRefleshEntity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return initRefleshEntity;
            }
        }

        #endregion

        #region 更新一个节点

        /// <summary>
        /// 更新一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">语音映射实体</param>
        [WebMethod]
        public void UpdateOne(int conferenceID, ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objUpdate)
            {
                try
                {
                    //会议名称为null则不执行以下操作
                    if (conferenceID == 0) return;

                    //语音集实体
                    ConferenceAudioInitRefleshEntity audioReflesh = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                    //语音节点集合
                    List<ConferenceAudioItemTransferEntity> audioList = audioReflesh.AcademicReviewItemTransferEntity_ObserList;

                    //找到对应的节点进行更改
                    for (int i = 0; i < audioList.Count; i++)
                    {
                        //获取遍历中的一个节点
                        var item = audioList[i];
                        if (item.Equals(academicReviewItemTransferEntity))
                        {
                            //操作类型更改为更新
                            academicReviewItemTransferEntity.Operation = ConferenceAudioOperationType.UpdateType;
                            //设置文本信息（对应的节点）
                            audioList[i] = academicReviewItemTransferEntity;
                            //实时同步
                            InformClient(conferenceID, academicReviewItemTransferEntity);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 提醒上传音频文件完成

        [WebMethod]
        public void NotifyAudioFileUploadCompleate(int conferenceID, ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            lock (objUploadCompleate)
            {
                try
                {
                    //会议名称为null则不执行以下操作
                    if (conferenceID == 0) return;

                    //语音集实体
                    ConferenceAudioInitRefleshEntity audioReflesh = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                    //语音节点集合
                    List<ConferenceAudioItemTransferEntity> audioList = audioReflesh.AcademicReviewItemTransferEntity_ObserList;

                    //找到对应的节点进行更改
                    for (int i = 0; i < audioList.Count; i++)
                    {
                        //获取遍历中的一个节点
                        var item = audioList[i];
                        if (item.Equals(academicReviewItemTransferEntity))
                        {
                            //操作类型更改为提交完成
                            academicReviewItemTransferEntity.Operation = ConferenceAudioOperationType.UploadCompleateType;
                            ////设置文本信息（对应的节点）
                            //audioList[i] = academicReviewItemTransferEntity;
                            //实时同步
                            InformClient(conferenceID, academicReviewItemTransferEntity);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 添加一个节点

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">语音映射实体</param>
        [WebMethod]
        public int AddOne(int conferenceID, ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objAdd)
            {
                int guid = -1;
                try
                {
                    //当前会议为null，则不执行下列操作
                    if (conferenceID == 0) return guid;

                    //语音集实体
                    ConferenceAudioInitRefleshEntity audioReflesh = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                    //是否包含
                    if (ConferenceDiscussInitRefleshEntity_dic.ContainsKey(conferenceID))
                    {
                        //类型设置
                        academicReviewItemTransferEntity.Operation = ConferenceAudioOperationType.AddType;
                        //获取相应的语音树
                        ConferenceAudioInitRefleshEntity conferenceAudioInitRefleshEntity = audioReflesh;
                        //guid绑定为当前rootcount
                        academicReviewItemTransferEntity.Guid = conferenceAudioInitRefleshEntity.RootCount;
                        guid = academicReviewItemTransferEntity.Guid;
                        //参数递增
                        conferenceAudioInitRefleshEntity.RootCount++;
                        //语音节点实体集合添加子节点
                        conferenceAudioInitRefleshEntity.AcademicReviewItemTransferEntity_ObserList.Add(academicReviewItemTransferEntity);

                        //实时同步
                        InformClient(conferenceID, academicReviewItemTransferEntity);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }

                finally
                {
                }
                return guid;
            }
        }

        #endregion

        #region 删除一个节点

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">语音映射实体</param>
        [WebMethod]
        public void DeleteOne(int conferenceID, ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objDelete)
            {
                try
                {
                    //当前会议为null，则不执行下列操作
                    if (conferenceID == 0) return;

                    //语音集实体
                    ConferenceAudioInitRefleshEntity audioReflesh = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                    //语音节点集合
                    List<ConferenceAudioItemTransferEntity> audioList = audioReflesh.AcademicReviewItemTransferEntity_ObserList;


                    //判断是否包含该节点,从而进行删除操作
                    if (audioList.Contains(academicReviewItemTransferEntity))
                    {
                        //操作类型改为删除
                        academicReviewItemTransferEntity.Operation = ConferenceAudioOperationType.DeleteType;
                        //删除该子节点
                        audioList.Remove(academicReviewItemTransferEntity);
                        //实时同步
                        InformClient(conferenceID, academicReviewItemTransferEntity);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 语音转文字

        /// <summary>
        /// 语音转文字
        /// </summary>
        [WebMethod]
        public void SettingAudioTransferTxt(int conferenceID, ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objTransfer)
            {
                try
                {
                    //当前会议为null，则不执行下列操作
                    if (conferenceID == 0) return;

                    if (string.IsNullOrEmpty(conferenceAudioItemTransferEntity.AudioMessage))
                    {
                        //音频文件字节数组
                        byte[] array = null;
                        //获取webservice路径
                        string strLocal = this.Server.MapPath(".");
                        conferenceAudioItemTransferEntity.AudioFileName = Path.GetFileName(conferenceAudioItemTransferEntity.AudioUrl);
                        string fileName = strLocal + "\\" + Constant.AudioLocalRootName + "\\" + conferenceAudioItemTransferEntity.AudioFileName;

                        if (File.Exists(fileName))
                        {
                            //通过文件流将音频文件转为字节数组
                            using (System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.Delete))
                            {
                                array = new byte[fileStream.Length];
                                //将文件流读出给指定字节数组
                                fileStream.Read(array, 0, array.Length);
                            }
                            AudioFileType audioFileType = (AudioFileType)Enum.Parse(typeof(AudioFileType), Path.GetExtension(fileName).Replace(".", string.Empty));
                            //语音转文字（通用方法）
                            string message = AudioTransfer.AudioToText(array, audioFileType);
                            //结束语音识别
                            if (string.IsNullOrEmpty(message))
                            {
                                message = "无法识别";
                            }
                            //操作方式改为更新
                            conferenceAudioItemTransferEntity.Operation = ConferenceAudioOperationType.UpdateType;
                            //语音文本
                            conferenceAudioItemTransferEntity.AudioMessage = message;
                            //更新一个节点
                            this.UpdateOne(conferenceID, conferenceAudioItemTransferEntity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {

                }
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(int conferenceID, ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntity)
        {

            try
            {
                //会议名称不为空
                if (conferenceID != 0)
                {
                    //生成数据包
                    PackageBase pack = new PackageBase() { ConferenceClientAcceptType = ConferenceClientAcceptType.ConferenceAudio, ConferenceAudioItemTransferEntity = conferenceAudioItemTransferEntity };

                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicAudioMeetServerSocket, conferenceID, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #endregion

        #region 手机应用

        ///// <summary>
        ///// 手机应用 信息交流缓存
        ///// </summary>
        //static Dictionary<string, ConferenceAudioItemTransferEntity> dicConferenceAudioItem_TransferEntity = new Dictionary<string, ConferenceAudioItemTransferEntity>();

        [WebMethod]
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回语音所有节点</returns>       
        public string GetAllByMobile(string strConferenceID)
        {
            string json = null;

            ConferenceAudioInitRefleshEntity initRefleshEntity = null;

            MessageInfo mi = new MessageInfo();


            mi.State = "0000";
            mi.Message = "";

            try
            {
                int conferenceID = 0;
                int.TryParse(strConferenceID, out conferenceID);
                if (conferenceID != 0)
                {
                    initRefleshEntity = GetAll(conferenceID);
                    if (initRefleshEntity == null)
                    {
                        mi.State = "0001";
                        mi.Message = "当前会议无信息";
                    }
                    else
                    {
                        json = CommonMethod.Serialize(initRefleshEntity);
                        mi.Result = json;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();

            return json;
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">语音映射实体</param>
        [WebMethod]
        public string AddOneByJson(string strConferenceID, string json)
        {
            lock (objAddByMobile)
            {
                string result = null;

                int conferenceID = 0;
                int.TryParse(strConferenceID, out conferenceID);
                if (conferenceID != 0)
                {
                    //return Serialize(mi);
                    MessageInfo mi = new MessageInfo();

                    //信息添加辅助
                    this.AddOneByJsonHelper(conferenceID, json, null);

                    mi.State = "0000";
                    mi.Message = "";
                    mi.Result = "True";

                    Context.Response.ContentType = "application/json";
                    Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
                    Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Context.Response.Write(CommonMethod.Serialize(mi));
                    Context.Response.End();
                }

                return result;
            }
        }

        /// <summary>
        /// 信息添加辅助
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public string AddOneByJsonHelper(int conferenceID, string json, string audioUri)
        {
            string result = null;
            List<ConferenceAudioItemTransferEntity> conferenceAudioItemTransferEntityList = JsonToEntity<ConferenceAudioItemTransferEntity>(json);

            if (conferenceAudioItemTransferEntityList.Count > 0)
            {
                ConferenceAudioItemTransferEntity ConferenceAudioItem_TransferEntity = conferenceAudioItemTransferEntityList[0];

                try
                {
                    //当前会议为null，则不执行下列操作
                    if (conferenceID == 0) return result;

                    //是否包含
                    if (ConferenceDiscussInitRefleshEntity_dic.ContainsKey(conferenceID))
                    {
                        //类型设置
                        ConferenceAudioItem_TransferEntity.Operation = ConferenceAudioOperationType.AddType;
                        //个人头像
                        ConferenceAudioItem_TransferEntity.PersonalImg = Constant.PersonImgHttp + ConferenceAudioItem_TransferEntity.AddAuthor + ".png";
                        if (audioUri != null)
                        {
                            //信息语音地址
                            ConferenceAudioItem_TransferEntity.AudioUrl = audioUri;
                        }

                        //获取相应的语音树
                        ConferenceAudioInitRefleshEntity conferenceAudioInitRefleshEntity = ConferenceDiscussInitRefleshEntity_dic[conferenceID];
                        //guid绑定为当前rootcount
                        ConferenceAudioItem_TransferEntity.Guid = conferenceAudioInitRefleshEntity.RootCount;

                        //参数递增
                        conferenceAudioInitRefleshEntity.RootCount++;
                        //语音节点实体集合添加子节点
                        conferenceAudioInitRefleshEntity.AcademicReviewItemTransferEntity_ObserList.Add(ConferenceAudioItem_TransferEntity);

                        //实时同步
                        InformClient(conferenceID, ConferenceAudioItem_TransferEntity);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }

                finally
                {
                }
            }
            return result;
        }

        /// <summary>
        /// IMM缓存添加(手机应用)
        /// </summary>
        /// <param name="conferenceAudioItemTransferEntity"></param>
        static void MobileIMMTempAdd(string conferenceName, ConferenceAudioItemTransferEntity conferenceAudioItemTransferEntity)
        {
            //if (dicConferenceAudioItem_TransferEntity.ContainsKey(conferenceName))
            //{
            //    dicConferenceAudioItem_TransferEntity[conferenceName] = conferenceAudioItemTransferEntity;
            //}
            //else
            //{
            //    dicConferenceAudioItem_TransferEntity.Add(conferenceName, conferenceAudioItemTransferEntity);
            //}
        }

        /// <summary>
        /// 将json解析成对应实体实例
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="json">json数据</param>
        /// <param name="split">分割符</param>
        /// <returns></returns>
        public static List<T> JsonToEntity<T>(string json)
        {
            List<T> tlist = new List<T>();

            Type type = typeof(T);

            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                var propertyArray = type.GetProperties();
                if (arrayList.Count > 0)
                {
                    foreach (var item in arrayList)
                    {
                        if (item is Dictionary<string, object>)
                        {
                            T obj = (T)Activator.CreateInstance(type);

                            var dicList = item as Dictionary<string, object>;


                            foreach (var dicChild in dicList)
                            {
                                foreach (var property in propertyArray)
                                {
                                    if (property.Name.Equals(dicChild.Key))
                                    {
                                        if (property.PropertyType == typeof(string))
                                        {
                                            property.SetValue(obj, dicChild.Value, null);
                                        }
                                        else if (property.PropertyType == typeof(int))
                                        {
                                            int newVlaue = 0;
                                            if (dicChild.Value != null)
                                            {
                                                Int32.TryParse(Convert.ToString(dicChild.Value), out newVlaue);
                                            }
                                            property.SetValue(obj, newVlaue, null);
                                        }
                                        else if (property.PropertyType == typeof(DateTime))
                                        {
                                            DateTime newVlaue;
                                            DateTime.TryParse(Convert.ToString(dicChild.Value), out newVlaue);
                                            //var newVlaue = Convert.ToDateTime(dicChild.Value);
                                            property.SetValue(obj, newVlaue, null);
                                        }
                                        else if (property.PropertyType == typeof(List<string>))
                                        {
                                            var array = Convert.ToString(dicChild.Value);
                                            //var newVlaue = array.ToList<string>();
                                            property.SetValue(obj, array, null);
                                        }
                                        else if (property.PropertyType == typeof(Uri))
                                        {
                                            var newVlaue = new Uri(Convert.ToString(dicChild.Value), UriKind.RelativeOrAbsolute);
                                            property.SetValue(obj, newVlaue, null);
                                        }
                                        else if (property.PropertyType == typeof(ConferenceAudioOperationType))
                                        {
                                            ConferenceAudioOperationType conferenceAudioOperationType = ConferenceWebCommon.EntityHelper.ConferenceDiscuss.ConferenceAudioOperationType.AddType;
                                            if (dicChild.Value != null)
                                            {
                                                int value = Convert.ToInt32(dicChild.Value);
                                                if (value < 3)
                                                {
                                                    conferenceAudioOperationType = (ConferenceAudioOperationType)value;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            tlist.Add(obj);
                        }
                    }
                }
            }
            catch
            {

            }
            return tlist;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetIMMJson(string conferenceName, string author)
        {
            string json = null;

            MessageInfo mi = new MessageInfo();

            mi.State = "0000";
            mi.Message = "";

            try
            {
                //if (dicConferenceAudioItem_TransferEntity.ContainsKey(conferenceName))
                //{
                //    #region old solution

                //    //if (dicConferenceAudioItem_TransferEntity[conferenceName].AddAuthor.Equals(author))
                //    //{                       
                //    //    //dicConferenceAudioItem_TransferEntity[conferenceName].IsSelfSend = true;
                //    //    json = Serialize(dicConferenceAudioItem_TransferEntity[conferenceName]);
                //    //}
                //    //else
                //    //{
                //    //    json = Serialize(dicConferenceAudioItem_TransferEntity[conferenceName]);
                //    //}

                //    #endregion

                //    json = CommonMethod.Serialize(dicConferenceAudioItem_TransferEntity[conferenceName]);
                //}
                //else
                //{
                //    mi.State = "0001";
                //    mi.Message = "当前无人发送信息";
                //}
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }

            mi.Result = json;
            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();

            return json;
        }

        /// <summary>
        /// 手机上传文件
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string AddOneByJsonIncludeAudio()
        {

            MessageInfo mi = new MessageInfo();
            mi.State = "0000";
            mi.Message = "";
            mi.Result = "false";
            string name = string.Empty;
            try
            {
                //string conferenceName = Context.Request["conferenceName"];
                int conferenceID = Convert.ToInt32(Context.Request["conferenceID"]);
                string json = Context.Request["json"];
                name = Context.Request.Files[0].FileName;
                Stream stream = Context.Request.Files[0].InputStream;
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                stream.Dispose();

                string fsName = System.AppDomain.CurrentDomain.BaseDirectory + Constant.AudioLocalRootName + "\\" + name;

                string fsNameUrl = Constant.AudioTempHttp + name;

                using (FileStream fs = File.Create(fsName))
                {
                    fs.Write(data, 0, data.Length);
                }
                //信息添加辅助
                this.AddOneByJsonHelper(conferenceID, json, fsNameUrl);
                mi.Result = "true";
            }
            catch (Exception e)
            {

            }
            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();
            return name;
        }

        #endregion

    }
}
