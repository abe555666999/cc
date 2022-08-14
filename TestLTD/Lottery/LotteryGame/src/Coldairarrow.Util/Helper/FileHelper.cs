using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Http;

namespace Coldairarrow.Util.Helper
{
    public class FileHelper
    {
        public class FilePathInfo
        {
            public string OldFileName { get; set; }

            public string FileCode { get; set; }

            /// <summary>
            /// 文件名称
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// 文件类型
            /// </summary>
            public string FileType { get; set; }

            /// <summary>
            /// 文件大小
            /// </summary>
            public long FileMax { get; set; }

            /// <summary>
            /// 文件相对路径
            /// </summary>
            public string FileRelativePath { get; set; }

            /// <summary>
            /// 文件绝对路径
            /// </summary>
            public string FileAbsolutelyPath { get; set; }
        }

        public class MergeExeclInfo
        {
            public string FilePath { get; set; }

            public string FileName { get; set; }
        }

        private static readonly string password = "9615F193E0D047338B8CB079A41F65BD";

        private static object _filePathObj = new object();

        /// <summary>
        /// 通过迭代器读取平面文件行内容(必须是带有\r\n换行的文件,百万行以上的内容读取效率存在问题,适用于100M左右文件，行100W内，超出的会有卡顿)
        /// </summary>
        /// <param name="fullPath">文件全路径</param>
        /// <param name="page">分页页数</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="seekEnd"> 是否最后一行向前读取,默认从前向后读取</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadPageLine(string fullPath, int page, int pageSize, bool seekEnd = false)
        {
            if (page <= 0)
            {
                page = 1;
            }
            fullPath = fullPath.ReplacePath();
            IEnumerable<string> lines2 = File.ReadLines(fullPath, Encoding.UTF8);
            if (seekEnd)
            {
                int lineCount = lines2.Count();
                int linPageCount = (int)Math.Ceiling((double)lineCount / ((double)pageSize * 1.0));
                if (page > linPageCount)
                {
                    page = 0;
                    pageSize = 0;
                }
                else if (page == linPageCount)
                {
                    pageSize = lineCount - (page - 1) * pageSize;
                    page = ((page != 1) ? (lines2.Count() - page * pageSize) : 0);
                }
                else
                {
                    page = lines2.Count() - page * pageSize;
                }
            }
            else
            {
                page = (page - 1) * pageSize;
            }
            lines2 = lines2.Skip(page).Take(pageSize);
            IEnumerator<string> enumerator = lines2.GetEnumerator();
            for (int count = 1; enumerator.MoveNext() || count <= pageSize; count++)
            {
                yield return enumerator.Current;
            }
            enumerator.Dispose();
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path.ReplacePath());
        }


        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path.ReplacePath());
        }

        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            return filename[start..];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path">路径 </param>
        /// <param name="fileName">文件名</param>
        /// <param name="content">写入的内容</param>
        /// <param name="appendToLast">是否将内容添加到未尾,默认不添加</param>
        public static void WriteFile(string path, string fileName, string content, bool appendToLast = false)
        {
            path = path.ReplacePath();
            fileName = fileName.ReplacePath();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using FileStream stream = File.Open(path + fileName, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] by = Encoding.Default.GetBytes(content);
            if (appendToLast)
            {
                stream.Position = stream.Length;
            }
            else
            {
                stream.SetLength(0L);
            }
            stream.Write(by, 0, by.Length);
        }

        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static void FileAdd(string Path, string strings)
        {
            StreamWriter sw = File.AppendText(Path.ReplacePath());
            sw.Write(strings);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="OrignFile">原始文件</param>
        /// <param name="NewFile">新文件路径</param>
        public static void FileCoppy(string OrignFile, string NewFile)
        {
            File.Copy(OrignFile.ReplacePath(), NewFile.ReplacePath(), overwrite: true);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            File.Delete(Path.ReplacePath());
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="OrignFile">原始路径</param>
        /// <param name="NewFile">新路径</param>
        public static void FileMove(string OrignFile, string NewFile)
        {
            File.Move(OrignFile.ReplacePath(), NewFile.ReplacePath());
        }

        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void FolderCreate(string OrignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(OrignFolder.ReplacePath());
            Directory.CreateDirectory(NewFloder.ReplacePath());
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Path"></param>
        public static void FolderCreate(string Path)
        {
            if (!Directory.Exists(Path.ReplacePath()))
            {
                Directory.CreateDirectory(Path.ReplacePath());
            }
        }

        public static void FileCreate(string Path)
        {
            FileInfo CreateFile = new FileInfo(Path.ReplacePath());
            if (!CreateFile.Exists)
            {
                FileStream FS = CreateFile.Create();
                FS.Close();
            }
        }

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param>  
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            dir = dir.ReplacePath();
            if (!Directory.Exists(dir))
            {
                return;
            }
            string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
            foreach (string d in fileSystemEntries)
            {
                if (File.Exists(d))
                {
                    File.Delete(d);
                }
                else
                {
                    DeleteFolder(d);
                }
            }
            Directory.Delete(dir, recursive: true);
        }

        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                aimPath = aimPath.ReplacePath();
                if (aimPath[^1] != Path.DirectorySeparatorChar)
                {
                    aimPath += Path.DirectorySeparatorChar;
                }
                if (!Directory.Exists(aimPath))
                {
                    Directory.CreateDirectory(aimPath);
                }
                string[] fileList = Directory.GetFileSystemEntries(srcPath.ReplacePath());
                string[] array = fileList;
                foreach (string file in array)
                {
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    }
                    else
                    {
                        File.Copy(file, aimPath + Path.GetFileName(file), overwrite: true);
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            dirPath = dirPath.ReplacePath();
            if (!Directory.Exists(dirPath))
            {
                return 0L;
            }
            long len = 0L;
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo fi in files)
            {
                len += fi.Length;
            }
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length != 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <returns></returns>
        public static string GetFileAttibe(string filePath)
        {
            string str = "";
            filePath = filePath.ReplacePath();
            FileInfo objFI = new FileInfo(filePath);
            return str + "详细路径:" + objFI.FullName + "<br>文件名称:" + objFI.Name + "<br>文件长度:" + objFI.Length + "字节<br>创建时间" + objFI.CreationTime.ToString() + "<br>最后访问时间:" + objFI.LastAccessTime.ToString() + "<br>修改时间:" + objFI.LastWriteTime.ToString() + "<br>所在目录:" + objFI.DirectoryName + "<br>扩展名:" + objFI.Extension;
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile_Old(string fileName)
        {
            try
            {
                using FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                file.Dispose();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                md5.Dispose();
                md5.Clear();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string filePath)
        {
            bool isUpload = true;
            int num = 0;
            string md5Str = "";
            try
            {
                if (File.Exists(filePath))
                {
                    while (isUpload)
                    {
                        using FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using MD5 md5 = new MD5CryptoServiceProvider();
                        StringBuilder sb = new StringBuilder();
                        byte[] retVal = md5.ComputeHash(file);
                        file.Close();
                        file.Dispose();
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            sb.Append(retVal[i].ToString("x2"));
                        }
                        md5.Dispose();
                        md5.Clear();
                        if (file != null)
                        {
                            file.Close();
                            file.Dispose();
                        }
                        isUpload = false;
                        num++;
                        Thread.Sleep(500);
                        md5Str = sb.ToString();
                    }
                }
            }
            catch (Exception)
            {
                num++;
                Thread.Sleep(500);
            }
            finally
            {
                if (num > 2)
                {
                }
                GC.Collect();
            }
            return md5Str;
        }

        public static byte[] ToDecrypt(FileStream fsCrypt)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);
                byte[] bytes = ConvertToByteArray(cs);
                cs.Close();
                fsCrypt.Close();
                return bytes;
            }
            catch (CryptographicException ce)
            {
                throw new Exception(ce.Message);
            }
        }

        public static byte[] ConvertToByteArray(Stream stream)
        {
            byte[] buffer = new byte[16384];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }

    }
}
