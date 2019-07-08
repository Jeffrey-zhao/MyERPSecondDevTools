using MyERPSecondDevTools.Model.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyERPSecondDevTools.Common
{
    public class FiddlerHelper
    {
        /// <summary>
        /// 获取ERP二开单点登录跳转地址
        /// </summary>
        /// <param name="pageUrl"></param>
        public static string GetERPNavigationPageUrl(string pageUrl)
        {
            Uri uri = new Uri(pageUrl);
            var host = uri.Authority;
            if (!pageUrl.ToLower().Contains("http://"))
                pageUrl = "http://" + pageUrl;
            if (!host.ToLower().Contains("http://"))
                host = "http://" + host;
            //OA集成登录地址
            var formatRequestUrl = $"{host}/PubPlatform/Nav/Login/SSO/Login.aspx?UserCode={GlobalData.ERPUserCode}&PageUrl={System.Web.HttpUtility.UrlEncode(pageUrl)}";
            return formatRequestUrl;
        }

        /// <summary>
        /// 获取明源ERP业务JS
        /// </summary>
        /// <param name="jsPathList"></param>
        /// <returns></returns>
        public static List<MyERPBusinessJsModel> GetERPBusinessJsModels(List<string> jsPathList)
        {
            var totalCount = jsPathList.Count;
            //获取JS源码任务四等分，除不尽五等分，并行执行
            var factorNum = totalCount / 4;
            var currentNum = 0;

            Func<IEnumerable<string>, List<MyERPBusinessJsModel>> downLoadBody = param =>
            {
                WebClient webClient = new WebClient();
                List<MyERPBusinessJsModel> models = new List<MyERPBusinessJsModel>();
                foreach (var m in param)
                {
                    Uri uri = new Uri(m);
                    var downLoadString = Encoding.UTF8.GetString(webClient.DownloadData(m));
                    models.Add(new MyERPBusinessJsModel
                    {
                        JsName = m,
                        JsContent = downLoadString
                    });
                }

                return models;
            };

            List<Task<List<MyERPBusinessJsModel>>> tasks = new List<Task<List<MyERPBusinessJsModel>>>();

            tasks.Add(Task.Factory.StartNew(() =>
            {
                var currentList = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
                var models = downLoadBody(currentList);
                return models;
            }));
            currentNum++;

            tasks.Add(Task.Factory.StartNew(() =>
            {
                var currentList = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
                var models = downLoadBody(currentList);
                return models;
            }));
            currentNum++;

            tasks.Add(Task.Factory.StartNew(() =>
            {
                var currentList = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
                var models = downLoadBody(currentList);
                return models;
            }));
            currentNum++;

            tasks.Add(Task.Factory.StartNew(() =>
            {
                var currentList = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
                var models = downLoadBody(currentList);

                return models;
            }));
            currentNum++;

            var remainder = totalCount % 4;
            if (remainder > 0)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var currentList = jsPathList.Skip(currentNum * factorNum).Take(remainder);
                    var models = downLoadBody(currentList);
                    return models;
                }));
            }
            
            var taskResult = Task.WhenAll(tasks);
            var result =taskResult.Result.Aggregate(Enumerable.Empty<MyERPBusinessJsModel>(), (total, next) =>
            {
                return total.Union(next);
            }).ToList();

            return result;
        }
    }
}
