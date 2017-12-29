using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityHelper;
using ConferenceWebCommon.EntityHelper.ConferenceTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Services;

namespace ConferenceWeb
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceTreeWebService : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 临时存储（用于校验的实例，没有父节点和子节点，一对一） 
        /// </summary>
        //public static Dictionary<string, ConferenceTreeItemTransferEntity> ConferenceTreeItemTransferEntity_temp_dic = new Dictionary<string, ConferenceTreeItemTransferEntity>();

        /// <summary>
        /// 后续进入的成员进行一次初始化的加载
        /// </summary>
        public static Dictionary<string, ConferenceTreeInitRefleshEntity> ConferenceTreeInitRefleshEntity_dic = new Dictionary<string, ConferenceTreeInitRefleshEntity>();

        /// <summary>
        /// 线程锁辅助对象（获取元数据）
        /// </summary>
        static private object objGetAll = new object();

        /// <summary>
        /// 线程锁辅助对象（更新一个节点）
        /// </summary>
        static private object objUpdateOne = new object();

        /// <summary>
        /// 线程锁辅助对象（更新一个标题）
        /// </summary>
        static private object objUpdateTittle = new object();

        /// <summary>
        /// 线程锁辅助对象（树节点替换）
        /// </summary>
        static private object objInstead = new object();

        /// <summary>
        /// 线程锁辅助对象（更新一个论点）
        /// </summary>
        static private object objUpdateComment = new object();

        /// <summary>
        /// 线程锁辅助对象（投票）
        /// </summary>
        static private object objVoteChanged = new object();

        /// <summary>
        /// 线程锁辅助对象（清除某个节点的所有投票）
        /// </summary>
        static private object objClearItemAllVote = new object();

        /// <summary>
        /// 线程锁辅助对象（添加一个节点）
        /// </summary>
        static private object objAddOne = new object();

        /// <summary>
        /// 线程锁辅助对象（删除一个节点）
        /// </summary>
        static private object objDeleteOne = new object();

        /// <summary>
        /// 线程锁辅助对象（更新整个树）
        /// </summary>
        static private object objSetAll = new object();

        /// <summary>
        /// 线程锁辅助对象（查看当前节点自己是否投过票）
        /// </summary>
        static private object objCheckVoteListContainsSelf = new object();

        /// <summary>
        /// 线程锁辅助对象（强制占用焦点）
        /// </summary>
        static private object objFocus = new object();

        /// <summary>
        /// 添加超链接
        /// </summary>
        static private object objLink = new object();

        #endregion

        #region 获取元数据（）

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>获取知识树所有节点</returns>
        [WebMethod]
        public ConferenceTreeInitRefleshEntity GetAll(string conferenceName)
        {
            //上锁,达到线程互斥作用
            //lock (objGetAll)
            //{
                ConferenceTreeInitRefleshEntity initRefleshEntity = null;
                try
                {
                    //会议正在进行中
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        //包含该会议
                        if (ConferenceTreeInitRefleshEntity_dic.ContainsKey(conferenceName))
                        {
                            //获取会议的树信息
                            initRefleshEntity = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                        }
                        else
                        {
                            //生成会议树的源数据
                            initRefleshEntity = new ConferenceTreeInitRefleshEntity();
                            //字典添加树视图的源数据
                            ConferenceTreeInitRefleshEntity_dic.Add(conferenceName, initRefleshEntity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return initRefleshEntity;
            //}
        }

        #endregion

        #region 更新一个节点

        /// <summary>
        /// 更新一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        [WebMethod]
        public void UpdateOne(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objUpdateOne)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;

                     //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                      //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                    //根节点
                    ConferenceTreeItemTransferEntity rootTree = treeReflesh.RootParent_AcademicReviewItemTransferEntity;

                    //操作类型设置为更新
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.UpdateType;

                    //GUID为0,证明为根节点
                    if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //设置根节点的论点
                        rootTree.Comment = academicReviewItemTransferEntity.Comment;

                        //设置根节点的标题
                        rootTree.Title = academicReviewItemTransferEntity.Title;
                        //更改链接
                        rootTree.LinkList = academicReviewItemTransferEntity.LinkList;

                        //设置根节点的操作人
                        rootTree.Operationer = academicReviewItemTransferEntity.Operationer;
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    else
                    {
                        //遍历所有节点
                        for (int i = 0; i < treeList.Count; i++)
                        {
                            //获取遍历过程中的子节点
                            var item = treeList[i];
                            //找到需要更新的子节点
                            if (item.Equals(academicReviewItemTransferEntity))
                            {
                                //设置子节点的论点
                                treeList[i].Comment = academicReviewItemTransferEntity.Comment;

                                //设置子节点的标题
                                treeList[i].Title = academicReviewItemTransferEntity.Title;

                                //更改链接
                                treeList[i].LinkList = academicReviewItemTransferEntity.LinkList;

                                //设置子节点的操作人
                                treeList[i].Operationer = academicReviewItemTransferEntity.Operationer;

                                //实时同步
                                InformClient(conferenceName, academicReviewItemTransferEntity);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }


        /// <summary>
        /// 更改标题
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <param name="academicReviewItemTransferEntity"></param>
        [WebMethod]
        public void UpdateTittle(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objUpdateTittle)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;

                     //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                      //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                     //根节点
                    ConferenceTreeItemTransferEntity rootTree = treeReflesh.RootParent_AcademicReviewItemTransferEntity;

                    //操作类型设置为更新
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.UpdateTittle;

                    //GUID为0,证明为根节点
                    if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //设置根节点的标题
                        rootTree.Title = academicReviewItemTransferEntity.Title;
                        //设置根节点的操作人
                        rootTree.Operationer = academicReviewItemTransferEntity.Operationer;
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    else
                    {
                        //遍历所有节点
                        for (int i = 0; i < treeList.Count; i++)
                        {
                            //获取遍历过程中的子节点
                            var item = treeList[i];
                            //找到需要更新的子节点
                            if (item.Equals(academicReviewItemTransferEntity))
                            {
                                //设置子节点的标题
                                treeList[i].Title = academicReviewItemTransferEntity.Title;
                                //设置子节点的操作人
                                treeList[i].Operationer = academicReviewItemTransferEntity.Operationer;

                                //实时同步
                                InformClient(conferenceName, academicReviewItemTransferEntity);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        /// <summary>
        /// 更改论点
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <param name="academicReviewItemTransferEntity"></param>
        [WebMethod]
        public void UpdateComment(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objUpdateOne)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;

                     //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                      //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                     //根节点
                    ConferenceTreeItemTransferEntity rootTree = treeReflesh.RootParent_AcademicReviewItemTransferEntity;

                    //操作类型设置为更新
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.UpdateComment;

                    //GUID为0,证明为根节点
                    if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //设置根节点的论点
                        rootTree.Comment = academicReviewItemTransferEntity.Comment;

                        //设置根节点的操作人
                        rootTree.Operationer = academicReviewItemTransferEntity.Operationer;
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    else
                    {
                        //遍历所有节点
                        for (int i = 0; i < treeList.Count; i++)
                        {
                            //获取遍历过程中的子节点
                            var item = treeList[i];
                            //找到需要更新的子节点
                            if (item.Equals(academicReviewItemTransferEntity))
                            {
                                //设置子节点的论点
                                treeList[i].Comment = academicReviewItemTransferEntity.Comment;

                                //设置子节点的操作人
                                treeList[i].Operationer = academicReviewItemTransferEntity.Operationer;

                                //实时同步
                                InformClient(conferenceName, academicReviewItemTransferEntity);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        [WebMethod]
        public void LinkAdd(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objLink)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;

                     //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                      //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                     //根节点
                    ConferenceTreeItemTransferEntity rootTree = treeReflesh.RootParent_AcademicReviewItemTransferEntity;

                    //操作类型设置为更新
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.LinkAdd;

                    //GUID为0,证明为根节点
                    if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //超链接
                        rootTree.LinkList = academicReviewItemTransferEntity.LinkList;
                        //设置根节点的操作人
                        rootTree.Operationer = academicReviewItemTransferEntity.Operationer;
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    else
                    {
                        //遍历所有节点
                        for (int i = 0; i < treeList.Count; i++)
                        {
                            //获取遍历过程中的子节点
                            var item = treeList[i];
                            //找到需要更新的子节点
                            if (item.Equals(academicReviewItemTransferEntity))
                            {
                                //超链接
                                treeList[i].LinkList = academicReviewItemTransferEntity.LinkList;
                                //设置子节点的操作人
                                treeList[i].Operationer = academicReviewItemTransferEntity.Operationer;

                                //实时同步
                                InformClient(conferenceName, academicReviewItemTransferEntity);
                                break;
                            }
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

        #region 元素替换法

        [WebMethod]
        public void Instead(string conferenceName, int beforeItemGuid, int nowItemGuid)
        {
            lock (objInstead)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;

                     //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                      //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                    //遍历所有节点
                    for (int i = 0; i < treeList.Count; i++)
                    {
                        //获取遍历过程中的子节点
                        var item = treeList[i];
                        //找到需要更新的子节点
                        if (item.Guid.Equals(beforeItemGuid))
                        {
                            item.ParentGuid = nowItemGuid;
                            this.InformClient(conferenceName, beforeItemGuid, nowItemGuid);
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

        #region 强制占用焦点

        /// <summary>
        /// 强制占用焦点
        /// </summary>
        [WebMethod]
        public void ForceOccuptFocus(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //上锁,达到线程互斥作用
                lock (objFocus)
                {
                    try
                    {
                        //会议名称为null则不执行以下操作
                        if (string.IsNullOrEmpty(conferenceName)) return;

                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
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

        #endregion

        #region 投票

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        /// <param name="Participant">当前参会人</param>
        [WebMethod]
        public void VoteChanged(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity, string Participant)
        {
            //上锁,达到线程互斥作用
            lock (objVoteChanged)
            {
                try
                {
                    ////会议信息为null则不执行
                    //if (string.IsNullOrEmpty(conferenceName)) return;
                    ////设置类型改为更新
                    //academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.UpdateType;

                    ////父节点
                    //if (academicReviewItemTransferEntity.Guid == 0)
                    //{
                    //    //只要有一个投票不同，则进行更改（赞成，反对）
                    //    if (ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.NoVoteCount != academicReviewItemTransferEntity.NoVoteCount
                    //        || ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.YesVoteCount != academicReviewItemTransferEntity.YesVoteCount
                    //        )
                    //    {
                    //        //反对（给知识树父节点，以会议名称为索引）
                    //        ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.NoVoteCount = academicReviewItemTransferEntity.NoVoteCount;
                    //        //赞成（给知识树父节点，以会议名称为索引）
                    //        ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.YesVoteCount = academicReviewItemTransferEntity.YesVoteCount;
                    //        //给知识树父节点添加投票人
                    //        if (!ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.VotedParticipantList.Contains(Participant))
                    //        {
                    //            ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.VotedParticipantList.Add(Participant);
                    //        }

                    //        //通知客户端
                    //        InformClient(conferenceName, academicReviewItemTransferEntity);
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList.Count; i++)
                    //    {
                    //        var item = ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i];
                    //        if (item.Equals(academicReviewItemTransferEntity))
                    //        {
                    //            //只要有一个投票不同，则进行更改（赞成，反对）
                    //            if (ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].NoVoteCount != academicReviewItemTransferEntity.NoVoteCount
                    //                || ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].YesVoteCount != academicReviewItemTransferEntity.YesVoteCount
                    //                )
                    //            {
                    //                //反对（给知识树子节点，以会议名称为索引）
                    //                ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].NoVoteCount = academicReviewItemTransferEntity.NoVoteCount;
                    //                //赞成（给知识树子节点，以会议名称为索引）
                    //                ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].YesVoteCount = academicReviewItemTransferEntity.YesVoteCount;

                    //                //给知识树子节点添加投票人
                    //                if (!ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].VotedParticipantList.Contains(Participant))
                    //                {
                    //                    ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].VotedParticipantList.Add(Participant);
                    //                }
                    //                //通知客户端
                    //                InformClient(conferenceName, academicReviewItemTransferEntity);
                    //            }

                    //            break;
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 清除某个节点的所有投票

        /// <summary>
        /// 清除某个节点的所有投票
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        [WebMethod]
        public void ClearItemAllVote(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objClearItemAllVote)
            {
                try
                {
                    if (string.IsNullOrEmpty(conferenceName)) return;
                    //操作模式改为更新
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.UpdateType;
                    //父节点
                    if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //反对（给知识树父节点，以会议名称为索引）
                        //ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.NoVoteCount = 0;
                        ////赞成（给知识树父节点，以会议名称为索引）
                        //ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.YesVoteCount = 0;
                        ////对应知识树父节点投票人列表更改
                        //ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.VotedParticipantList.Clear();
                        //通知客户端
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    for (int i = 0; i < ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList.Count; i++)
                    {
                        var item = ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i];
                        if (item.Equals(academicReviewItemTransferEntity))
                        {
                            ////反对（给知识树子节点，以会议名称为索引）
                            //ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].NoVoteCount = 0;
                            ////赞成（给知识树子节点，以会议名称为索引）
                            //ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].YesVoteCount = 0;
                            ////对应知识树子节点投票人列表更改
                            //ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i].VotedParticipantList.Clear();
                            //通知客户端
                            InformClient(conferenceName, academicReviewItemTransferEntity);

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
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        [WebMethod]
        public void AddOne(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objAddOne)
            {
                try
                {
                    if (string.IsNullOrEmpty(conferenceName)) return;

                    //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                    //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;

                    //知识树添加节点（做判断）
                    if (!treeList.Contains(academicReviewItemTransferEntity))
                    {
                        //操作类型改为添加
                        academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.AddType;
                        //知识树添加节点
                        treeList.Add(academicReviewItemTransferEntity);
                        //guid绑定为当前rootcount
                        academicReviewItemTransferEntity.Guid = treeReflesh.RootCount;
                        //进阶
                        treeReflesh.RootCount++;
                        //通知客户端
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 删除一个节点

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        [WebMethod]
        public void DeleteOne(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            //上锁,达到线程互斥作用
            lock (objDeleteOne)
            {
                try
                {
                    //会议名称不能为空
                    if (string.IsNullOrEmpty(conferenceName)) return;

                    //相关会议的知识树
                    ConferenceTreeInitRefleshEntity treeReflesh = ConferenceTreeInitRefleshEntity_dic[conferenceName];
                    //知识树节点集合
                    List<ConferenceTreeItemTransferEntity> treeList = treeReflesh.AcademicReviewItemTransferEntity_ObserList;
                    //操作类型改为删除
                    academicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.DeleteType;
                    //子节点
                    if (treeList.Contains(academicReviewItemTransferEntity))
                    {
                        //删除节点的子节点
                        for (int i = treeList.Count - 1; i > -1; i--)
                        {
                            if (treeList[i].ParentGuid == academicReviewItemTransferEntity.Guid)
                            {
                                treeList.Remove(treeList[i]);
                            }
                        }
                        treeList.Remove(academicReviewItemTransferEntity);
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }
                    //根节点
                    else if (academicReviewItemTransferEntity.Guid == 0)
                    {
                        //删除所有子节点
                        treeList.Clear();
                        //实时同步
                        InformClient(conferenceName, academicReviewItemTransferEntity);
                    }

                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 删除所有节点（除根节点）

        /// <summary>
        /// 删除所有节点（除根节点）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void DeleteAll(string conferenceName)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 更新整个树

        /// <summary>
        /// 更新整个树
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="conferenceTreeInitRefleshEntity">所有节点集</param>
        [WebMethod]
        public void SetAll(string conferenceName, ConferenceTreeInitRefleshEntity conferenceTreeInitRefleshEntity)
        {
            //上锁,达到线程互斥作用
            lock (objSetAll)
            {
                try
                {
                    //知识树清除
                    if (ConferenceTreeInitRefleshEntity_dic.ContainsKey(conferenceName))
                    {
                        //移除当前会议的所有节点
                        ConferenceTreeInitRefleshEntity_dic.Remove(conferenceName);
                        //刷新
                        ConferenceTreeInitRefleshEntity_dic.Add(conferenceName, conferenceTreeInitRefleshEntity);
                        //操作类型为刷新
                        conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.RefleshAllType;

                        //实时同步(刷新所有)
                        InformClient(conferenceName, new ConferenceTreeItemTransferEntity() { Operation = ConferenceTreeOperationType.RefleshAllType });
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 查看当前节点自己是否投过票

        /// <summary>
        /// 查看当前节点自己是否投过票
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="academicReviewItemTransferEntity">知识树映射实体</param>
        /// <param name="Participant">当前参会人</param>
        /// <returns>是否已透过票</returns>
        [WebMethod]
        public bool CheckVoteListContainsSelf(string conferenceName, ConferenceTreeItemTransferEntity academicReviewItemTransferEntity, string Participant)
        {
            //上锁,达到线程互斥作用
            lock (objCheckVoteListContainsSelf)
            {
                //是否投过票（当前用户）
                bool result = true;
                //try
                //{
                //    //会议名称为null则不执行
                //    if (string.IsNullOrEmpty(conferenceName)) return false;

                //    //获取知识树根节点
                //    if (academicReviewItemTransferEntity.Guid == 0)
                //    {
                //        //查看根节点有没有自己投的票
                //        if (ConferenceTreeInitRefleshEntity_dic[conferenceName].RootParent_AcademicReviewItemTransferEntity.VotedParticipantList.Contains(Participant))
                //        {
                //            //已投
                //            result = true;
                //        }
                //        else
                //        {
                //            //未投
                //            result = false;
                //        }
                //    }
                //    //遍历所有节点
                //    for (int i = 0; i < ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList.Count; i++)
                //    {
                //        //获取遍历的子节点
                //        var item = ConferenceTreeInitRefleshEntity_dic[conferenceName].AcademicReviewItemTransferEntity_ObserList[i];
                //        //找到对应的子节点
                //        if (item.Equals(academicReviewItemTransferEntity))
                //        {
                //            //查看该节点自己是否已投票
                //            if (item.VotedParticipantList.Contains(Participant))
                //            {
                //                //已投
                //                result = true;
                //            }
                //            else
                //            {
                //                //未投
                //                result = false;
                //            }
                //            break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    LogManage.WriteLog(this.GetType(), ex);
                //}
                return result;
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(string conferenceName, ConferenceTreeItemTransferEntity conferenceTreeItemTransferEntity)
        {
            try
            {
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成数据包
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceTree,
                       
                    };
                    pack.ConferenceTreeFlg.ConferenceTreeItemTransferEntity = conferenceTreeItemTransferEntity;
                    pack.ConferenceTreeFlg.ConferenceTreeFlgType = ConferenceTreeFlgType.normal;
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicTreeMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(string conferenceName, int beforeGuid, int nowGuid)
        {
            try
            {
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成数据包
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceTree,
                        
                    };

                    pack.ConferenceTreeFlg.ConferenceTreeInsteadEntity = new ConferenceTreeInsteadEntity() { BeforeItemGuid = beforeGuid ,NowItemGuid =nowGuid};
                    pack.ConferenceTreeFlg.ConferenceTreeFlgType = ConferenceTreeFlgType.instead;

                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicTreeMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #endregion
    }
}