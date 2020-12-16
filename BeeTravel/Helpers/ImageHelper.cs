using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap FromBase64StringToImage(this string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    using (Image imgReturn = Image.FromStream(memoryStream))
                    {
                        memoryStream.Close();
                        byteBuffer = null;
                        return new Bitmap(imgReturn);
                    }
                }
            }
            catch { return null; }

        }
        //Отримати повний шлях фото
        public static string GetImagePath(IHostingEnvironment host, string imgName)
        {
            return Path.Combine(host.WebRootPath, "img", imgName);
        }

        //Збереження фото
        public static string SaveImage(IHostingEnvironment host, IFormFile img)
        {
            string uniqImgName = null;

            if (img != null)
            {
                //генеруємо нове імя
                uniqImgName = Guid.NewGuid().ToString() + "_" + img.FileName;
                //дістаємо повний шлях фото
                string filePath = GetImagePath(host, uniqImgName);
                //зберігаємо фото
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    img.CopyTo(fileStream);
                }
            }
            return uniqImgName;
        }

        //Видалення фото
        public static void DeleteImage(IHostingEnvironment host, string imgName)
        {
            string filePath = GetImagePath(host, imgName);
            File.Delete(filePath);
        }
    }
}
