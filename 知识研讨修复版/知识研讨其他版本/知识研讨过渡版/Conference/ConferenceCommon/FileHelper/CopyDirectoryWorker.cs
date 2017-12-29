using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceCommon.FileHelper
{
   public  class CopyDirectoryWorker
    {
        public delegate void CopyFileEventHandler(long lngHad, long lngCount, string strShow);//定义一个委托
        public event CopyFileEventHandler OnCopyFile;//定义一个事件,在Copy文件时触发

        public delegate void WorkOverEventHandler();//定义一个委托
        public event WorkOverEventHandler WorkOvered;//定义一个事件,在Copy文件完成时触发

        public CopyDirectoryWorker()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        private string _sourceDirectory;
        public string SourceDirectory
        {
            get
            {
                return _sourceDirectory;
            }
            set
            {
                _sourceDirectory = value;
            }
        }

        private string _aimDirectory;
        public string AimDirectory
        {
            get
            {
                return _aimDirectory;
            }
            set
            {
                _aimDirectory = value;
            }
        }

        /// <summary>
        /// 递归拷贝文件,把源目录下所有文件和文件夹拷贝到目标目录
        /// </summary>
        /// <param name="sourceDirectory">源路径</param>
        /// <param name="aimDirectory">目标路径</param>
        public void CopyFiles()
        {
            if (!System.IO.Directory.Exists(SourceDirectory) & !System.IO.Directory.Exists(AimDirectory))
                throw new Exception("文件夹不存在");

            string strTemp = SourceDirectory.Substring(SourceDirectory.LastIndexOf(@"\"));
            string strRealAimDirecotry = AimDirectory + strTemp;// System.IO.Path.Combine(aimDirectory,strTemp);

            if (!System.IO.Directory.Exists(strRealAimDirecotry))
                System.IO.Directory.CreateDirectory(strRealAimDirecotry);

            RecursionCopyFiles(SourceDirectory, strRealAimDirecotry);//调用真正文件夹复制程序

            WorkOvered();//触发事件，调用完成复制后的处理程序
        }

        /// <summary>
        /// 二进制读取文件,任何文件
        /// </summary>
        private void CopyFile(string SourceFile, string AimFile)
        {
            byte[] bytTemp = new byte[4096];//字节数组

            long lngHad = 0;
            long lngCount;
            int z = 5000;

            //源文件流
            System.IO.FileStream fsSource = new System.IO.FileStream(SourceFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read, 4);
            //二进制读取器
            System.IO.BinaryReader bRead = new System.IO.BinaryReader(fsSource);
            //定位源文件流的头部
            bRead.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            if (fsSource.Position > 0)
                fsSource.Position = 0;

            lngCount = fsSource.Length;

            if (System.IO.File.Exists(AimFile))
                System.IO.File.Delete(AimFile);

            //目标文件流
            System.IO.FileStream fsAim = new System.IO.FileStream(AimFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Write, 4);
            //二进制写入器
            System.IO.BinaryWriter bWrite = new System.IO.BinaryWriter(fsAim);

            while (z >= 4096)
            {
                z = (int)bRead.Read(bytTemp, 0, bytTemp.Length);//读入字节数组,返回读取的字节数量，如果小于4096，则到了文件尾
                bWrite.Write(bytTemp, 0, bytTemp.Length);//从字节数组写入目标文件流

                lngHad += z;
                string show = "从" + SourceFile + "到" + AimFile;
                OnCopyFile(lngHad, lngCount, show);//触发事件 来控制主线程 这里是进度条和已完成复制文件字节显示   
            }

            bWrite.Flush();//清理缓存区

            bWrite.Close();
            bRead.Close();

            fsAim.Close();
            fsSource.Close();
        }

        /// <summary>
        /// 递归拷贝文件,把源目录下所有文件和文件夹拷贝到目标目录
        /// </summary>
        /// <param name="sourceDirectory">源路径</param>
        /// <param name="aimDirectory">目标路径</param>
        private bool RecursionCopyFiles(string sourceDirectory, string aimDirectory)
        {
            if (!System.IO.Directory.Exists(sourceDirectory) & !System.IO.Directory.Exists(aimDirectory))//
                return false;
            try
            {
                string[] directories = System.IO.Directory.GetDirectories(sourceDirectory);
                if (directories.Length > 0)
                {
                    foreach (string dir in directories)//递归调用
                    {
                        RecursionCopyFiles(dir, aimDirectory + dir.Substring(dir.LastIndexOf(@"\")));//attention: "/" cann't instead of "\"
                    }
                }

                if (!System.IO.Directory.Exists(aimDirectory))
                {
                    System.IO.Directory.CreateDirectory(aimDirectory);//if not exist the aimDirectory,create it
                }

                string[] files = System.IO.Directory.GetFiles(sourceDirectory);

                if (files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        //不需要进度条效果，可以直接使用下一句来拷贝文件
                        // System.IO.File.Copy(file,aimDirectory+file.Substring(file.LastIndexOf(@"\")));//Copy The File To The Aim
                        string sourceFile = file;
                        string aimFile = aimDirectory + file.Substring(file.LastIndexOf(@"\"));
                        CopyFile(sourceFile, aimFile);//调用文件拷贝函数
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }    
}
