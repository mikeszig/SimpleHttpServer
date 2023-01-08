using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Page
{
    public class PageNotFound : PageHandler
    {
        public PageNotFound(string _path) : base(_path)
        {
        }
    }
}
