using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Coldairarrow.Business;
using Coldairarrow.Util;
using Coldairarrow.Util.Configuration;
using Coldairarrow.Util.Helper;

namespace Coldairarrow.Business.Utilities
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

		public static string GetCurrentDownLoadPath()
		{
			return "Download\\".MapPath();
		}

		public static bool DirectoryExists(string path)
		{
			return Directory.Exists(path.ReplacePath());
		}

		public static string Read_File(string fullpath, string filename, string suffix)
		{
			return ReadFile((fullpath + "\\" + filename + suffix).MapPath());
		}

		public static string ReadFile(string fullName)
		{
			string temp = fullName.MapPath().ReplacePath();
			string str = "";
			if (!File.Exists(temp))
			{
				return str;
			}
			StreamReader sr = null;
			try
			{
				sr = new StreamReader(temp);
				str = sr.ReadToEnd();
			}
			catch
			{
			}
			sr?.Close();
			sr?.Dispose();
			return str;
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

		/// <summary>
		/// 多文件上传
		/// </summary>
		/// <param name="filelist"></param>
		/// <param name="uploadFilePath"></param>
		/// <param name="isTimeFolder"></param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> UploadFile(IFormFileCollection filelist, string uploadFilePath = "", bool isTimeFolder = true)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			string uploadName = (string.IsNullOrEmpty(uploadFilePath) ? AppSetting.GetSettingString("UploadFileName") : uploadFilePath);
			if (filelist != null && filelist.Count > 0)
			{
				for (int i = 0; i < filelist.Count; i++)
				{
					IFormFile file = filelist[i];
					if (file.Length <= 0)
					{
						continue;
					}
					Random rd = new Random();
					int num = rd.Next(0, 100);
					string Tpath = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
					string _filename = file.FileName;
					string type2 = Path.GetExtension(_filename);
					string FileCode = DateTime.Now.ToString("yyyyMMddHHmmssfff") + num.ToString().PadLeft(3, '0');
					string FileName = FileCode + type2;
					string FilePath;
					string relativePath;
					if (isTimeFolder)
					{
						FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + Tpath;
						relativePath = "/" + uploadName + Tpath + FileName;
					}
					else
					{
						FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + "/";
						relativePath = "/" + uploadName + "/" + FileName;
					}
					DirectoryInfo di = new DirectoryInfo(FilePath);
					if (!di.Exists)
					{
						di.Create();
					}
					try
					{
						string FileAbsolutelyPath = FilePath + FileName;
						list.Add(new FilePathInfo
						{
							FileMax = file.Length,
							OldFileName = _filename,
							FileCode = FileCode,
							FileName = FileName,
							FileType = type2,
							FileRelativePath = relativePath,
							FileAbsolutelyPath = FileAbsolutelyPath
						});
						type2 = type2.TrimStart('.');
						if (type2.ToUpper() == "TXT")
						{
							Stream streamInfo = file.OpenReadStream();
							using (FileStream fsIn = File.Open(FileAbsolutelyPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
							{
								byte[] bytes = ConvertToByteArray(streamInfo);
								string str = Encoding.UTF8.GetString(bytes);
								byte[] outbytes = str.ToBytes();
								await fsIn.WriteAsync(outbytes.AsMemory(0, outbytes.Length));
								fsIn.Close();
							}
							using FileStream fs = new FileStream(FileAbsolutelyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
							StreamReader streamReader = new StreamReader(fs);
							File.WriteAllText(FileAbsolutelyPath, streamReader.ReadToEnd(), Encoding.UTF8);
							fs.Close();
						}
						else
						{
							using FileStream stream = File.Create(FileAbsolutelyPath);
							await file.CopyToAsync(stream);
						}
					}
					catch (Exception ex)
					{
						_ = "上传文件写入失败：" + ex.Message;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 多文件上传(大平台任务交付物)
		/// </summary>
		/// <param name="filelist"></param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> BigTaskUploadFile(IFormFileCollection filelist)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			string uploadName = AppSetting.GetSettingString("UploadFileName");
			if (filelist != null && filelist.Count > 0)
			{
				for (int i = 0; i < filelist.Count; i++)
				{
					IFormFile file = filelist[i];
					if (file.Length <= 0)
					{
						continue;
					}
					Random rd = new Random();
					int num = rd.Next(0, 100);
					string Tpath = DateTime.Now.ToString("yyyy-MM-dd");
					string _filename = file.FileName;
					string type2 = Path.GetExtension(_filename);
					string FileCode = DateTime.Now.ToString("yyyyMMddHHmmssfff") + num.ToString().PadLeft(3, '0');
					string FileName = FileCode + type2;
					string FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + "/BigTaskOutFile/" + Tpath;
					string relativePath = "/" + uploadName + "/BigTaskOutFile/" + Tpath + "/" + FileName;
					DirectoryInfo di = new DirectoryInfo(FilePath);
					if (!di.Exists)
					{
						di.Create();
					}
					try
					{
						string FileAbsolutelyPath = FilePath + "/" + FileName;
						list.Add(new FilePathInfo
						{
							FileMax = file.Length,
							OldFileName = _filename,
							FileCode = FileCode,
							FileName = FileName,
							FileType = type2,
							FileRelativePath = relativePath,
							FileAbsolutelyPath = FileAbsolutelyPath
						});
						type2 = type2.TrimStart('.');
						if (type2.ToUpper() == "TXT")
						{
							Stream streamInfo = file.OpenReadStream();
							using (FileStream fsIn = File.Open(FileAbsolutelyPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
							{
								byte[] bytes = ConvertToByteArray(streamInfo);
								string str = Encoding.UTF8.GetString(bytes);
								byte[] outbytes = str.ToBytes();
								await fsIn.WriteAsync(outbytes.AsMemory(0, outbytes.Length));
								fsIn.Close();
							}
							using FileStream fs = new FileStream(FileAbsolutelyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
							StreamReader streamReader = new StreamReader(fs);
							File.WriteAllText(FileAbsolutelyPath, streamReader.ReadToEnd(), Encoding.UTF8);
							fs.Close();
						}
						else
						{
							using FileStream stream = File.Create(FileAbsolutelyPath);
							await file.CopyToAsync(stream);
						}
					}
					catch (Exception ex)
					{
						_ = "上传文件写入失败：" + ex.Message;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 单文件上传
		/// </summary>
		/// <param name="file"></param>
		/// <param name="uploadFilePath"></param>
		/// <param name="isTimeFolder"></param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> UploadFileInfo(IFormFile file, string uploadFilePath = "", bool isTimeFolder = true)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			string uploadName = (string.IsNullOrEmpty(uploadFilePath) ? AppSetting.GetSettingString("UploadFileName") : uploadFilePath);
			if (file != null && file.Length > 0)
			{
				Random rd = new Random();
				int num = rd.Next(0, 100);
				string Tpath = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
				string _filename = file.FileName;
				string type = Path.GetExtension(_filename);
				string FileCode = DateTime.Now.ToString("yyyyMMddHHmmssfff") + num.ToString().PadLeft(3, '0');
				string FileName = FileCode + type;
				string jmFileName = (string.IsNullOrEmpty(_filename) ? ("jm_" + DateTime.Now.ToString("yyyyMMddHHmmssfff")) : ("jm_" + _filename));
				string FilePath;
				string relativePath;
				if (isTimeFolder)
				{
					FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + Tpath;
					relativePath = "/" + uploadName + Tpath + FileName;
				}
				else
				{
					FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + "/";
					relativePath = "/" + uploadName + "/" + FileName;
				}
				DirectoryInfo di = new DirectoryInfo(FilePath);
				if (!di.Exists)
				{
					di.Create();
				}
				try
				{
					string FileAbsolutelyPath = FilePath + FileName;
					_ = FilePath + jmFileName;
					list.Add(new FilePathInfo
					{
						FileMax = file.Length,
						OldFileName = _filename,
						FileCode = FileCode,
						FileName = FileName,
						FileType = type,
						FileRelativePath = relativePath,
						FileAbsolutelyPath = FileAbsolutelyPath
					});
					FileAbsolutelyPath.Replace("/", "\\");
		           if (type.ToUpper() == "TXT")
					{
						Stream streamInfo = file.OpenReadStream();
						using FileStream fsIn = File.Open(FileAbsolutelyPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
						byte[] bytes = ConvertToByteArray(streamInfo);
						string str = Encoding.UTF8.GetString(bytes);
						byte[] outbytes = str.ToBytes();
						await fsIn.WriteAsync(outbytes.AsMemory(0, outbytes.Length));
						fsIn.Close();
					}
					else
					{
						using FileStream stream = File.Create(FileAbsolutelyPath);
						await file.CopyToAsync(stream);
					}
				}
				catch (Exception ex)
				{
					_ = "上传文件写入失败：" + ex.Message;
				}
			}
			return list;
		}

		/// <summary>
		/// 单文件上传(指定目录)
		/// </summary>
		/// <param name="file"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> UploadFileInfoByFilePath(IFormFile file, string filePath)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			if (file != null && file.Length > 0)
			{
				string _filename = file.FileName;
				Path.GetExtension(_filename);
				DirectoryInfo di = new DirectoryInfo(filePath);
				if (!di.Exists)
				{
					di.Create();
				}
				try
				{
					filePath = filePath + "/" + _filename;
					using FileStream stream = File.Create(filePath);
					await file.CopyToAsync(stream);
				}
				catch (Exception ex)
				{
					_ = "上传文件写入失败：" + ex.Message;
				}
			}
			return list;
		}

		/// <summary>
		/// 多文件上传(指定目录)
		/// </summary>
		/// <param name="filelist"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> UploadFileListByFilePath(IFormFileCollection filelist, string filePath)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			if (filelist != null && filelist.Count > 0)
			{
				DirectoryInfo di = new DirectoryInfo(filePath);
				if (!di.Exists)
				{
					di.Create();
				}
				for (int i = 0; i < filelist.Count; i++)
				{
					IFormFile file = filelist[i];
					if (file.Length <= 0)
					{
						continue;
					}
					string _filename = file.FileName;
					Path.GetExtension(_filename);
					try
					{
						string uploadFilePath = filePath + "/" + _filename;
						using FileStream stream = File.Create(uploadFilePath);
						await file.CopyToAsync(stream);
					}
					catch (Exception ex2)
					{
						Exception ex = ex2;
						_ = "上传文件写入失败：" + ex.Message;
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 单文件上传
		/// </summary>
		/// <param name="filePath">文件地址</param>
		/// <returns></returns>
		public static async Task<List<FilePathInfo>> CopyFileInfo(string filePath)
		{
			List<FilePathInfo> list = new List<FilePathInfo>();
			string uploadName = AppSetting.GetSettingString("UploadFileName");
			FileInfo file = new FileInfo(filePath);
			if (file != null && file.Length > 0)
			{
				Random rd = new Random();
				int num = rd.Next(0, 100);
				string Tpath = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
				string _filename = file.FullName;
				string type = Path.GetExtension(_filename);
				string fileRealName = file.Name;
				string FileCode = DateTime.Now.ToString("yyyyMMddHHmmssfff") + num.ToString().PadLeft(3, '0');
				string FileName = FileCode + type;
				string jmFileName = (string.IsNullOrEmpty(_filename) ? ("jm_" + DateTime.Now.ToString("yyyyMMddHHmmssfff")) : ("jm_" + _filename));
				string FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + Tpath;
				string relativePath = "/" + uploadName + Tpath + FileName;
				DirectoryInfo di = new DirectoryInfo(FilePath);
				if (!di.Exists)
				{
					di.Create();
				}
				try
				{
					string FileAbsolutelyPath = FilePath + FileName;
					_ = FilePath + jmFileName;
					list.Add(new FilePathInfo
					{
						FileMax = file.Length,
						OldFileName = fileRealName,
						FileCode = FileCode,
						FileName = FileName,
						FileType = type,
						FileRelativePath = relativePath,
						FileAbsolutelyPath = FileAbsolutelyPath
					});
					FileAbsolutelyPath.Replace("/", "\\");
		            if (type.ToUpper() == "TXT")
					{
						FileStream streamInfo = file.OpenRead();
						using FileStream fsIn = File.Open(FileAbsolutelyPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
						byte[] bytes = ConvertToByteArray(streamInfo);
						string str = Encoding.UTF8.GetString(bytes);
						byte[] outbytes = str.ToBytes();
						await fsIn.WriteAsync(outbytes.AsMemory(0, outbytes.Length));
						fsIn.Close();
					}
					else
					{
						file.CopyTo(FileAbsolutelyPath, overwrite: true);
					}
				}
				catch (Exception ex)
				{
					_ = "上传文件写入失败：" + ex.Message;
				}
			}
			return list;
		}

		/// <summary>
		/// 单文件上传
		/// </summary>
		/// <param name="filePath">文件地址</param>
		/// <param name="fileCode"></param>
		/// <returns></returns>
		public static async Task<string> CopyFileInfoByPath(string filePath, string fileCode)
		{
			string resultFilePath = "";
			new List<FilePathInfo>();
			string uploadName = AppSetting.GetSettingString("UploadFileName");
			FileInfo file = new FileInfo(filePath);
			if (file != null && file.Length > 0)
			{
				Random rd = new Random();
				int num = rd.Next(0, 100);
				string Tpath = "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
				string _filename = file.FullName;
				string type = Path.GetExtension(_filename);
				_ = file.Name;
				string FileCode = DateTime.Now.ToString("yyyyMMddHHmmssfff") + num.ToString().PadLeft(3, '0');
				string FileName = FileCode + "[[" + fileCode + "]]" + type;
				string jmFileName = (string.IsNullOrEmpty(_filename) ? ("jm_" + DateTime.Now.ToString("yyyyMMddHHmmssfff")) : ("jm_" + _filename));
				string FilePath = Directory.GetCurrentDirectory() + "/" + uploadName + Tpath;
				_ = "/" + uploadName + Tpath + FileName;
				DirectoryInfo di = new DirectoryInfo(FilePath);
				if (!di.Exists)
				{
					di.Create();
				}
				try
				{
					string FileAbsolutelyPath = FilePath + FileName;
					_ = FilePath + jmFileName;
					FileAbsolutelyPath.Replace("/", "\\");
					resultFilePath = FileAbsolutelyPath;
					file.CopyTo(FileAbsolutelyPath, overwrite: true);
				}
				catch (Exception ex)
				{
					_ = "上传文件写入失败：" + ex.Message;
				}
			}
			return await Task.FromResult(resultFilePath);
		}

		/// <summary>
		/// 文件复制到目标位置
		/// </summary>
		/// <param name="filePath">文件地址</param>
		/// <param name="fileCode"></param>
		/// <param name="targetFilePath">目标路径</param>
		/// <returns></returns>
		public static async Task<List<string>> CopyFileToTargetPath(string filePath, string fileCode, string targetFilePath)
		{
			List<string> fileList = new List<string>();
			new List<FilePathInfo>();
			FileInfo file = new FileInfo(filePath);
			if (file != null && file.Length > 0)
			{
				string _filename = file.FullName;
				string type = Path.GetExtension(_filename);
				_ = file.Name;
				string FileName = fileCode + type;
				string FilePath = Directory.GetCurrentDirectory() + "/" + targetFilePath;
				string relativePath = "/" + targetFilePath + FileName;
				DirectoryInfo di = new DirectoryInfo(FilePath);
				if (!di.Exists)
				{
					di.Create();
				}
				try
				{
					string fileAbsolutelyPath = FilePath + FileName;
					fileAbsolutelyPath.Replace("/", "\\");
					file.CopyTo(fileAbsolutelyPath, overwrite: true);
					fileList.Add(relativePath);
					fileList.Add(fileAbsolutelyPath);
				}
				catch (Exception ex)
				{
					_ = "上传文件写入失败：" + ex.Message;
				}
			}
			return await Task.FromResult(fileList);
		}



		/// <summary>
		///  加密
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		public static void EncryptFile(string inputFile, string outputFile)
		{
			try
			{
				UnicodeEncoding UE = new UnicodeEncoding();
				byte[] key = UE.GetBytes(password);
				FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
				RijndaelManaged RMCrypto = new RijndaelManaged();
				CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);
				FileStream fsIn = File.Open(inputFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				int data;
				while ((data = fsIn.ReadByte()) != -1)
				{
					cs.WriteByte((byte)data);
				}
				fsIn.Close();
				cs.Close();
				fsCrypt.Close();
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		///  加密
		/// </summary>
		/// <param name="fsIn"></param>
		/// <param name="outputFile"></param>
		public static void FileEncrypt(FileStream fsIn, string outputFile)
		{
			try
			{
				UnicodeEncoding UE = new UnicodeEncoding();
				byte[] key = UE.GetBytes(password);
				FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
				RijndaelManaged RMCrypto = new RijndaelManaged();
				CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);
				int data;
				while ((data = fsIn.ReadByte()) != -1)
				{
					cs.WriteByte((byte)data);
				}
				cs.Close();
				fsCrypt.Close();
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		///  解密
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		public static void DecryptFile(string inputFile, string outputFile)
		{
			UnicodeEncoding UE = new UnicodeEncoding();
			byte[] key = UE.GetBytes(password);
			FileStream fsCrypt = new FileStream(inputFile, FileMode.OpenOrCreate);
			RijndaelManaged RMCrypto = new RijndaelManaged();
			CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);
			FileStream fsOut = new FileStream(outputFile, FileMode.OpenOrCreate);
			int data;
			while ((data = cs.ReadByte()) != -1)
			{
				fsOut.WriteByte((byte)data);
			}
			fsOut.Close();
			cs.Close();
			fsCrypt.Close();
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
