using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Server
{
    /// <summary>
    /// File 的摘要说明
    /// </summary>
    public class FileProcess : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var uploadType = context.Request["uploadType"];
            switch (uploadType)
            {
                case "fileUpload":
                    UploadFile(context);
                    break;
                case "imgUpload":
                    UploadImage(context);
                    break;
                case "download":
                    Download(context);
                    break;
            }
        }

        private void UploadFile(HttpContext context)
        {
            var fileName = context.Request["fileName"];
            var compressedFileContent = context.Request["fileContent"];
            bool isUploadPartly = context.Request["isUploadPartly"] == "true";
            //如果前端没有对文件进行压缩，那么直接获取文件的字节数组即可
            //byte[] fileBytes = Convert.FromBase64String(compressedFileContent);

            //解压
            var fileContent = GZip.GZipDecompress(compressedFileContent);
            byte[] fileBytes = Utils.ConvertJSBytes2Str(fileContent);
            bool isSavedSuccess = Utils.SaveFile2Disk(fileBytes, fileName, isUploadPartly);

            context.Response.Write(isSavedSuccess);
        }

        private void UploadImage(HttpContext context)
        {
            var fileName = context.Request["fileName"];
            var compressedFileContent = context.Request["fileContent"];
            //如果前端没有对文件进行压缩，那么直接获取文件的字节数组即可
            //byte[] fileBytes = Convert.FromBase64String(compressedFileContent);

            //解压
            var fileContent = GZip.GZipDecompress(compressedFileContent);
            byte[] fileBytes = Utils.ConvertJSBytes2Str(fileContent);
            byte[] thumbnailBytes = ImageUtils.Cut(fileBytes, 120, 120, ThumbnailMode.HW);
            bool isSavedSuccess = Utils.SaveFile2Disk(fileBytes, fileName, false);
            isSavedSuccess = Utils.SaveFile2Disk(thumbnailBytes, "缩略图-" + fileName, false);

            var imgBase64Str = "data:image/png;base64," + Convert.ToBase64String(thumbnailBytes);
            context.Response.Write(imgBase64Str);
            //isUploadComplete
        }

        //public void UploadFile2()
        //{
        //    HttpRequest request = System.Web.HttpContext.Current.Request;
        //    if (request.Files == null || request.Files.Count == 0)
        //    {
        //        return;
        //    }

        //    HttpPostedFile file = request.Files[0];

        //    byte[] fileBytes = new byte[file.ContentLength];
        //    try
        //    {
        //        using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
        //        {
        //            fileBytes = binaryReader.ReadBytes(fileBytes.Length);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void Download(HttpContext context)
        {
            var filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/UploadFileFloder/"), "file1.txt");
            byte[] fileContentBytes = Utils.GetByteArrayByFilePath(filePath);
            context.Response.Write(Convert.ToBase64String(fileContentBytes));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}