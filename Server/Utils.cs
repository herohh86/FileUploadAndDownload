using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Server
{
    public class Utils
    {
        public static byte[] ConvertJSBytes2Str(string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                return null;
            }
            // JS中，String.fromCharCode接受Unicode字符，并转成字符串
            byte[] fileBytes = System.Text.Encoding.Unicode.GetBytes(fileContent);
            byte[] adjustedFileBytes = new byte[fileBytes.Length / 2];
            var index = 0;
            for (var i = 0; i < fileBytes.Length; i = i + 2)
            {
                adjustedFileBytes[index] = fileBytes[i];
                index++;
            }

            return adjustedFileBytes;
        }

        public static byte[] BitMap2Bytes(Bitmap bmp)
        {
            MemoryStream m = new MemoryStream();
            bmp.Save(m, ImageFormat.Png);
            byte[] arr = new byte[m.Length];
            m.Position = 0;
            m.Read(arr, 0, (int)m.Length);
            m.Close();
            return arr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileContent">文件的字节数组</param>
        /// <param name="fileName"></param>
        /// <param name="isSavedPartly">文件是否是分多次读取并存储的(当文件太大的时候，可以分次读取和保存)</param>
        /// <returns></returns>
        public static bool SaveFile2Disk(byte[] fileContent, string fileName, bool isSavedPartly = false)
        {
            if (fileContent == null || fileContent.Length == 0)
            {
                throw new ArgumentNullException("文件内容不能为空!");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("文件名不能为空!");
            }
            var targetFloder = HttpContext.Current.Server.MapPath("~/UploadFileFloder/");
            var fullPath = Path.Combine(targetFloder, fileName);
            DirectoryInfo di = new DirectoryInfo(targetFloder);
            if (di.Exists == false)
            {
                di.Create();
            }
            FileStream fileStream;
            try
            {
                if (isSavedPartly)
                {
                    fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Read, 1024);
                }
                else
                {
                    fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                //write log
                return false;
            }
            try
            {
                fileStream.Write(fileContent, 0, fileContent.Length);
            }
            catch (IOException ex)
            {
                //write log
                return false;
            }
            finally
            {
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
            }
            return true;
        }

        public static byte[] GetByteArrayByFilePath(string fileFullPath)
        {
            if (string.IsNullOrEmpty(fileFullPath))
            {
                return null;
            }

            FileStream fs = null;
            byte[] fileContent = null;
            try
            {
                FileInfo fi = new FileInfo(fileFullPath);
                fileContent = new byte[fi.Length];
                //fs = new FileStream(fileFullPath, FileMode.Open);
                fs = File.OpenRead(fileFullPath);
                fs.Read(fileContent, 0, fileContent.Length);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return fileContent;
        }
    }
}