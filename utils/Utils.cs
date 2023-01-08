using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Change path as you need
namespace WebServer.Utils
{
    public static class Utils
    {
        public static async Task<byte[]> ReadAudioFromLibrary()
        {
            Random rdm = new Random();
            int i = rdm.Next(5) + 1;
            string path = @$"C:\..\HttpServer\resources\playlist\{i}.mp3";

            var data = await File.ReadAllBytesAsync(path);

            return data;
        }

        public static byte[] ImageMapFromLibrary()
        {
            Random rdm = new Random();
            int i = rdm.Next(3) + 1;
            string path = @$"C:\..\HttpServer\resources\img\{i}.jpg";
            Bitmap bmp = new Bitmap(path);

            MemoryStream ms = new MemoryStream();

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();

            return bmpBytes;
        }
    }
}
