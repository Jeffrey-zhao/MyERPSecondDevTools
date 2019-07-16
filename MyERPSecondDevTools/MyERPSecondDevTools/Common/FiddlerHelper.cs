using MyERPSecondDevTools.Model.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            GlobalData.ERPHost = host;

            //OA集成登录地址
            var formatRequestUrl = $"{host}/PubPlatform/Nav/Login/SSO/Login.aspx?{GlobalData.ERPUserCodeParamName}={GlobalData.ERPUserCode}&{GlobalData.ERPJumpPageParamName}={System.Web.HttpUtility.UrlEncode(pageUrl)}";
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

            //下载源码任务方法
            Func<IEnumerable<string>, List<MyERPBusinessJsModel>> downLoadBody = param =>
            {
                List<MyERPBusinessJsModel> models = new List<MyERPBusinessJsModel>();
                try
                {
                    WebClient webClient = new WebClient();
                    
                    foreach (var m in param)
                    {
                        Uri uri = new Uri(m);
                        var downLoadString = Encoding.UTF8.GetString(webClient.DownloadData(m));
                        models.Add(new MyERPBusinessJsModel
                        {
                            ApplicationId = applicationId,
                            JsName = m.Substring(0, m.IndexOf("?")),
                            JsContent = downLoadString
                        });
                    }
                }
                catch (Exception)
                {
                    
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

        /// <summary>
        /// 获取可扩展控件
        /// </summary>
        /// <param name="metaDataId"></param>
        public static List<ModulePluginPointModel> GetControlMetaData(List<DesignModel> designModels)
        {
            var modulePluginPointModels = new List<ModulePluginPointModel>();

            #region 转换扩展点名称方法
            Func<string, string, string> GetTitleByName = (functionName, name) =>
            {
                if (functionName.Contains("_appForm_load"))
                    return "表单加载完成事件";
                else if (functionName.Contains("_Grid_load"))
                    return "网格加载完成事件";
                else if (functionName.Contains("_Grid_query"))
                    return "网格查询事件";
                else if (functionName.Contains("_appForm_beforeSubmit"))
                    return "表单提交前事件";
                else if (functionName.Contains("_appForm_afterSubmit"))
                    return "表单提交后事件";
                else if (functionName.Contains("_appTreeGrid") && name == "onquery")
                    return "树网格查询事件";
                else if (functionName.Contains("_appTreeGrid") && name == "onload")
                    return "树网格加载事件";
                else if (functionName.Contains("_appGrid") && name == "onload")
                    return "网格加载事件";
                else if (functionName.Contains("_appGrid") && functionName.Contains("_query"))
                    return "网格加载事件";
                else
                    return functionName;
            };

            Func<string, string, string> GetToolBarTitleByName = (type, align) =>
            {
                if (type == "global")
                    return "页面按钮";
                else if (type == "row" && align == "left")
                    return "行左侧按钮";
                else if (type == "row" && align == "center")
                    return "行中间按钮";
                else if (type == "row" && align == "right")
                    return "行右侧按钮";
                else
                    return "按钮";
            };
            #endregion

            designModels.ForEach(m =>
            {
                if (m.DataType != "page")
                {
                    ModulePluginPointModel modulePluginPointModel = new ModulePluginPointModel();
                    modulePluginPointModel.Title = m.DataLabel;
                    var pluginPointModels = new List<PluginPointModel>();
                    var postData = new
                    {
                        design = true,
                        controlName = m.DataType,
                        metadataId = m.DataId
                    };
                    var jsonPostData = JsonConvert.SerializeObject(postData);
                    var responseData = ERPWebRequestHelper.PostWebRequest(GlobalData.ERPHost + "/ajax/Mysoft.Map6.Modeling.Handlers.Metadatas.MetadataAjaxHandler/GetControlMetadata.aspx", jsonPostData, Encoding.UTF8);
                    JObject jsonObject = (JObject)JToken.Parse(responseData);
                    var events = jsonObject["item"]["layout"]["events"];
                    foreach (var item in events)
                    {
                        pluginPointModels.Add(new PluginPointModel
                        {
                            Type = m.DataType,
                            ControlId = item["functionName"].ToString(),
                            Title = GetTitleByName(item["functionName"].ToString(), item["name"].ToString()),
                            Events = new List<PluginPointModelEvent>
                        {
                            new PluginPointModelEvent
                            {
                                 EventName= item["name"].ToString(),
                                 FunctionName= item["functionName"].ToString(),
                            }
                        }
                        });
                    }

                    var toolBars = jsonObject["item"]["layout"]["toolbars"];
                    foreach (var item in toolBars)
                    {
                        foreach (var subItem in item["groups"])
                        {
                            foreach (var secItem in subItem["items"])
                            {
                                var model = new PluginPointModel
                                {
                                    Type = "按钮",
                                    ControlId = secItem["id"].ToString(),
                                    MetaDataStatus = secItem["metadataStatus"].ToString(),
                                    Title = GetToolBarTitleByName(item["type"].ToString(), subItem["align"].ToString()) + "-" + secItem["title"].ToString(),
                                    Events = new List<PluginPointModelEvent>()
                                };
                                var flag = true;
                                foreach (var fourItem in secItem["events"])
                                {
                                    if (fourItem["functionName"].ToString().StartsWith("Mysoft.Map"))
                                        flag = false;
                                    else
                                        model.Events.Add(new PluginPointModelEvent
                                        {
                                            EventName = fourItem["name"].ToString(),
                                            FunctionName = fourItem["functionName"].ToString(),
                                        });
                                }
                                if (flag)
                                    pluginPointModels.Add(model);
                            }
                        }
                    }
                    modulePluginPointModel.PluginPointModels = pluginPointModels;
                    modulePluginPointModels.Add(modulePluginPointModel);
                }
            });

            //列表类元素，找不到对应的模块名称,取页面类的模块名称
            var pageMetaData = designModels.FirstOrDefault(p => p.DataType == "page");
            var pageModuleName = string.Empty;
            if (pageMetaData != null)
            {
                var responseData = ERPWebRequestHelper.GetWebRequest(GlobalData.ERPHost + $"/ajax/Mysoft.Map6.Modeling.Handlers.Metadatas.MetadataAjaxHandler/GetFunctionPageMetadata.aspx?metadataId={pageMetaData.DataId}");
                JObject jsonObject = (JObject)JToken.Parse(responseData);
                pageModuleName = jsonObject["item"]["jsResource"].ToString();

                modulePluginPointModels.ForEach(m =>
                {
                    m.PluginPointModels.ForEach(item =>
                    {
                        item.Events.ForEach(sitem =>
                        {
                            if (sitem.FunctionName.IndexOf(".") <= 0)
                            {
                                //页面是产品页面，按钮是二开，则更新为Plugin
                                if (jsonObject["item"]["metadataStatus"].ToString() == "product" && item.MetaDataStatus == "customize")
                                {
                                    sitem.FunctionName = pageModuleName + ".Plugin." + sitem.FunctionName;
                                    sitem.IsProduct = false;
                                }
                                else
                                {
                                    sitem.FunctionName = pageModuleName + "." + sitem.FunctionName;
                                    sitem.IsProduct = true;
                                }
                            }
                        });
                    });
                });
            }

            return modulePluginPointModels;
        }
    }
}
