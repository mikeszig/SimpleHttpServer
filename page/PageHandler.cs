using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Page
{
    public abstract class PageHandler
    {
        private readonly string _path = "";
        private byte[] _data;

        public PageHandler(string _path)
        {
            this._path = _path;
            _ = InitializeComponents();
        }

        private async Task InitializeComponents()
        {
            _data = await LoadPage(_path);
        }

        private async Task<byte[]> LoadPage(string filePath)
        {
            var data = await File.ReadAllBytesAsync(filePath);

            return data;
        }

        public byte[] GetHtml()
        {
            return _data;
        }
    }
}
