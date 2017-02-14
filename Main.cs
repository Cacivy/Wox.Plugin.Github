using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System;

namespace Wox.Plugin.Github
{
    public class ApiResult
    {
        public int total_count { get; set; }
        public bool incomplete_results { get; set; }
        public List<ApiResultItem> items { get; set; }
    }

    public class ApiResultOwner
    {
        public string login { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
    }

    public class ApiResultItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public string description { get; set; }
        public bool @private { get; set; }
        public string html_url { get; set; }
        public int stargazers_count { get; set; }
        public ApiResultOwner owner { get; set; }
    }

    public class Main : IPlugin
    {
        private PluginInitContext context;
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private const string ApiUrl = "https://api.github.com/search/repositories?sort=stars&order=desc&q=";
        const string ico = "Images\\github.ico";

        public void Init(PluginInitContext context)
        {
            this.context = context;
        }

        public List<Result> Query(Query query)
        {
            List<Result> list = new List<Result>();
            if (query.Search.Length == 0)
            {
                list.Add(new Result
                {
                    Title = "开始Github查询",
                    SubTitle = "基于Github API",
                    IcoPath = ico
                });
                return list;
            }
            HttpWebRequest request = null;
            string url = ApiUrl + (query.Search ?? "vue");
            request = WebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                StreamReader objReader = new StreamReader(stream);
                var json = objReader.ReadToEnd();
                ApiResult o = JsonConvert.DeserializeObject<ApiResult>(json);

                if (o.total_count > 0)
                {
                    foreach (var item in o.items)
                    {
                        list.Add(new Result
                        {
                            Title = item.full_name,
                            SubTitle = item.description,
                            IcoPath = ico, // item.owner.avatar_url,
                            Action = (e) =>
                            {
                                context.API.HideApp();
                                System.Diagnostics.Process.Start(item.html_url);
                                return true;
                            }
                        });
                    }
                } else
                {
                    list.Add(new Result
                    {
                        Title = "没有查询到相关的库",
                        IcoPath = ico
                    });
                }
                
            }

            return list;
        }

    }
}
