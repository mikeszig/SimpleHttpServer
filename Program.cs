using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Drawing;
using Microsoft.VisualBasic;
using Xabe.FFmpeg;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using HttpServer.Page;

// Change path as you need
namespace WebServer
{
    class HttpServer
    {
        private static readonly HttpListener listener;

        private static readonly string _path404 = @"C:\..\HttpServer\resources\404.html";
        private static readonly string _pathIndex = @"C:\..\HttpServer\resources\index.html";

        private static readonly string urlIndex = "http://localhost:8000/";

        private static IndexPage indexPage;
        private static PageNotFound pageNotFound;

        private static async Task HandleIncomingConnections(HttpListenerContext context)
        {
            bool runServer = true;

            string[] domain =
                new string[] { "/", "/background.jpg", "/audio.mp3", "/jquery-3.6.3.js", "/favicon.ico" };

            while (runServer)
            {
                HttpListenerRequest req = context.Request;
                HttpListenerResponse resp = context.Response;

                pageNotFound = new PageNotFound(_path404);
                indexPage = new IndexPage(_pathIndex);

                if (!domain.Contains(req.Url.AbsolutePath))
                {
                    byte[] data404 = pageNotFound.GetHtml();
                    await SentResponse(resp, data404, "text/html");

                    Console.WriteLine($"Page not Found ({req.Url.AbsolutePath}) requested by ({req.UserHostName})");
                    Console.WriteLine(data404.Length);
                    Console.WriteLine();
                    resp.Close();
                    continue;
                }

                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if ((req.HttpMethod == "GET") && req.Url.AbsolutePath.StartsWith("/background.jpg"))
                {
                    var dataImg = Utils.Utils.ImageMapFromLibrary();
                    await SentResponse(resp, dataImg, "image/jpeg");
                }

                if ((req.HttpMethod == "GET") && req.Url.AbsolutePath.StartsWith("/audio.mp3"))
                {
                    var dataAudio = await Utils.Utils.ReadAudioFromLibrary();
                    await SentResponse(resp, dataAudio, "audio/mpeg");
                }

                byte[] dataIndex = indexPage.GetHtml();
                await SentResponse(resp, dataIndex, "text/html");
                resp.Close();
            }
        }

        private static async Task SentResponse(HttpListenerResponse _resp, byte[] _data, string _contentType)
        {
            _resp.ContentType = _contentType;
            _resp.ContentEncoding = Encoding.UTF8;
            _resp.ContentLength64 = _data.Length;

            await _resp.OutputStream.WriteAsync(_data, 0, _data.Length);
        }

        public static async Task Listen(string prefix, int maxConcurrentRequests)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            Console.WriteLine("Listening for connections on {0}", urlIndex);

            var requests = new HashSet<Task>();
            for (int i = 0; i < maxConcurrentRequests; i++)
                requests.Add(listener.GetContextAsync());

            while (true)
            {
                Task t = await Task.WhenAny(requests);
                requests.Remove(t);

                if (t is Task<HttpListenerContext>)
                {
                    var context = (t as Task<HttpListenerContext>).Result;
                    requests.Add(HandleIncomingConnections(context));
                    requests.Add(listener.GetContextAsync());
                }
            }
        }

        public static void Main()
        {
            Task listenTask = Listen(urlIndex, 99);
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}