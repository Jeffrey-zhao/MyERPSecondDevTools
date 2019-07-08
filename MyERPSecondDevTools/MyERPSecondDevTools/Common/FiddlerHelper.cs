using MyERPSecondDevTools.Model.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            //应用ID
            var applicationId = GlobalData.ApplicationId;

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
                        ApplicationId = applicationId,
                        JsName = m,
                        JsContent = downLoadString
                    });
                }

                return models;
            };

            List<Task<List<MyERPBusinessJsModel>>> tasks = new List<Task<List<MyERPBusinessJsModel>>>();

            var currentList1 = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var models = downLoadBody(currentList1);
                return models;
            }));
            currentNum++;

            var currentList2 = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var models = downLoadBody(currentList2);
                return models;
            }));
            currentNum++;

            var currentList3 = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var models = downLoadBody(currentList3);
                return models;
            }));
            currentNum++;

            var currentList4 = jsPathList.Skip(currentNum * factorNum).Take(factorNum);
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var models = downLoadBody(currentList4);
                return models;
            }));
            currentNum++;

            var remainder = totalCount % 4;
            if (remainder > 0)
            {
                var currentList5 = jsPathList.Skip(currentNum * factorNum).Take(remainder);
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var models = downLoadBody(currentList5);
                    return models;
                }));
            }

            var taskResult = Task.WhenAll(tasks);
            var result = taskResult.Result.Aggregate(Enumerable.Empty<MyERPBusinessJsModel>(), (total, next) =>
             {
                 return total.Union(next);
             }).ToList();

            //业务js写入到数据库中
            WriteToToolDB(result, applicationId);

            return result;
        }

        /// <summary>
        /// JS源码写入到工具库中
        /// </summary>
        private static void WriteToToolDB(List<MyERPBusinessJsModel> jsModels, Guid applicationId)
        {
            SqlHelper sqlHelper = new SqlHelper(GlobalData.ToolsDataBaseConnectionString);
            //清空原有数据
            sqlHelper.ExecuteNonQuery($"DELETE FROM [MyERPDevToolsScripts] WHERE ApplicationId = '{applicationId}'");
            var insertSb = new StringBuilder();
            jsModels.ForEach(m =>
            {
                var strSql = @"  INSERT INTO dbo.MyERPDevToolsScripts
                                                  ( ApplicationId, JsName, JsData )
                                          VALUES  ( @applicationId,
                                                    @jsName,
                                                    @jsContent
                                                    );";
                sqlHelper.ExecuteNonQuery(strSql, new SqlParameter[]
                {
                    new SqlParameter("applicationId", applicationId),
                    new SqlParameter("jsName", m.JsName),
                    new SqlParameter("jsContent", m.JsContent),
                });
            });
        }
    }
}
