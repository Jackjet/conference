using ConferenceCommon.LogHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace ConferenceCommon.FileHelper
{
    public class FileManage
    {
        #region 序列化将某个对象存储到文件

        /// <summary>
        /// 序列化将某个对象存储到文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void Save_Entity(Object obj, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, obj);
                    fileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }
        }

        #endregion

        #region 将某个文件反序列化还原成实体对象

        /// <summary>
        /// 将某个文件反序列化还原成实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Load_Entity<T>(string filePath)
        {
            T entity = (T)Activator.CreateInstance(typeof(T));
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                else
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (fileStream.Length > 0L)
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            object obj = binaryFormatter.Deserialize(fileStream);
                            if (obj is T)
                            {
                                entity = (T)obj;
                            }
                        }
                        else
                        {
                            entity = Activator.CreateInstance<T>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.Delete(filePath);
                LogManage.WriteLog(typeof(FileManage), ex);
            }
            return entity;
        }

        #endregion


        #region 序列化将某个对象存储到文件(xml方式)

        /// <summary>
        /// 序列化将某个对象存储到文件(xml方式)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        public static void Save_EntityInXml(Object obj, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer binaryFormatter = new XmlSerializer(obj.GetType());
                    binaryFormatter.Serialize(fileStream, obj);
                    fileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }
        }

        #endregion

        #region 将某个文件反序列化还原成实体对象（xml方式）

        public static T Load_EntityInXml<T>(string filePath)
        {
            T entity = (T)Activator.CreateInstance(typeof(T));
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                else
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (fileStream.Length > 0L)
                        {
                            XmlSerializer binaryFormatter = new XmlSerializer(typeof(T));
                            object obj = binaryFormatter.Deserialize(fileStream);
                            if (obj is T)
                            {
                                entity = (T)obj;
                            }
                        }
                        else
                        {
                            entity = Activator.CreateInstance<T>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.Delete(filePath);
                LogManage.WriteLog(typeof(FileManage), ex);
            }
            return entity;
        }

        #endregion

        #region 拷贝文件

        /// <summary>
        /// 确保文件存在系统目录
        /// </summary>
        public static void CheckDebugHasTheFile(string FileName, string sourceFileRoot)
        {
            try
            {
                //paintFile所要生成的文件（本系统输出目录）
                var file = Environment.CurrentDirectory + "\\" + FileName;

                //确保该文件在应用程序启动之后存在（参数设置需要使用该dll文件）
                if (!System.IO.File.Exists(file))
                {
                    //paintFile所备份的文件
                    var file2 = sourceFileRoot + "\\" + FileName;
                    //判断是否需要拷贝文件
                    if (System.IO.File.Exists(file2))
                    {
                        //文件拷贝
                        System.IO.File.Copy(file2, file);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 生成文件

        /// <summary>
        /// 生成word，pdf文件（html）
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateWPFile(string fileName, System.Windows.Forms.WebBrowser webBrowser, string elementName)
        {
            try
            {
                //存储文件对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;

                //设置默认文件名（可以不设置）
                saveFileDialog.FileName = fileName;

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileManage.CreateWPFile2(saveFileDialog.FileName, webBrowser, elementName);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 生成word，pdf文件（html）
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateWPFile2(string fileName, System.Windows.Forms.WebBrowser webBrowser, string elementName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        System.Windows.Forms.HtmlElement element = webBrowser.Document.GetElementById(elementName);
                        sw.Write(element.OuterHtml);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }

        }

        #endregion

        #region 打开文件存储对话框

        /// <summary>
        /// 打开文件存储对话框
        /// </summary>
        public static void OpenDialogThenDoing(string defaultFileName,Action<string> callBack)
        {
            try
            {
                //存储文件对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;

                //设置默认文件名（可以不设置）
                saveFileDialog.FileName = System.IO.Path.GetFileName(defaultFileName);

                if (saveFileDialog.ShowDialog() == true)
                {
                    callBack(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(FileManage), ex);
            }

        }

        #endregion
    }
}
