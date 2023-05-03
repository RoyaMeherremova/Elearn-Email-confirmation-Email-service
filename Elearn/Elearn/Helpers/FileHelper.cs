using System.IO;
using Elearn.Models;

namespace Elearn.Helpers
{
    public static class FileHelper
    {
        public static bool CheckFileType(this IFormFile file,string pattern)  //static extention yaziriq(komekci herdefe eyno kodu yazmamaq ucun)
        {
            //this IFormFile file-faylin tipine yaziriq extentionu
            //string patern -ne tipde olsun file(check)
            return file.ContentType.Contains(pattern);

        }

        public static bool CheckFileSize(this IFormFile file, long size)
        {
            return file.Length / 1024 < size;
        }

        public static void DeleteFile(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
         
        }

        public static string GetFilePath(string root,string folder,string file)
        {
            return Path.Combine(root, folder,file);
        }
        public static async Task SaveFileAsync(string path, IFormFile file)   //method for save file in project,IFormFile=fayla
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}
