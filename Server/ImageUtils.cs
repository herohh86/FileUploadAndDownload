using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Server
{
    public class ImageUtils
    {
        public static byte[] GetThumbnail(Image rawImage, int width, int height, ThumbnailMode mode)
        {
            int rawHeight = rawImage.Height;
            int rawWidth = rawImage.Width;
            int xIndex = 0;
            int yIndex = 0;
            switch (mode)
            {
                case ThumbnailMode.HW:
                    {
                        if ((double)rawWidth / rawHeight > (double)width / height)
                        {
                            rawHeight = (height / width) * rawWidth;
                            yIndex = (rawImage.Height - rawHeight) / 2;
                        }
                        else
                        {
                            rawWidth = (width / height) * rawHeight;
                            xIndex = (rawImage.Width - rawWidth) / 2;
                        }
                    }
                    break;
                case ThumbnailMode.H:
                    {
                        width = (height / rawHeight) * rawWidth;
                    }
                    break;
                case ThumbnailMode.W:
                    {
                        height = (width / rawWidth) * rawHeight;
                    }
                    break;
                case ThumbnailMode.CUT:
                    {
                        if ((double)rawWidth / rawHeight > (double)width / height)
                        {
                            width = (width / height) * rawHeight;
                            xIndex = (rawImage.Width - width) / 2;
                        }
                        else
                        {
                            height = (height / width) * rawWidth;
                            yIndex = (rawImage.Height - height) / 2;
                        }
                    }
                    break;
                default:
                    break;
            }

            Bitmap b = new Bitmap(width, height);
            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Clear(Color.White);
                g.DrawImage(rawImage, new Rectangle(0, 0, width, height), new Rectangle(xIndex, yIndex, rawWidth, rawHeight), GraphicsUnit.Pixel);
                return Utils.BitMap2Bytes(b);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                rawImage.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageContent">图片的字节数组</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] Cut(byte[] imageContent, int width = 120, int height = 120, ThumbnailMode mode = ThumbnailMode.CUT)
        {
            try
            {
                MemoryStream m = new MemoryStream(imageContent);
                Image img = Image.FromStream(m);
                m.Close();
                return GetThumbnail(img, width, height, mode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public enum ThumbnailMode
    {
        HW, //按指定宽高缩放
        H,  //指定高，宽按比例
        W,  //指定宽，高按比例
        CUT //指定宽高裁剪
    }
}