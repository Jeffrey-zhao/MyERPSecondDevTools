﻿using MyERPSecondDevTools.Common;
using MyERPSecondDevTools.Model.Ext;
using MyERPSecondDevTools.Model.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyERPSecondDevTools
{
    public partial class MyERPSecondDevTools : Form
    {
        #region 局部变量
        /// <summary>
        /// 明源ERP响应HTML
        /// </summary>
        private string MyERPResponseHtml { get; set; }

        /// <summary>
        /// JS语法树响应HTML
        /// </summary>
        private string JsTreeSyntaxResponseHtml { get; set; }

        /// <summary>
        /// JS源码数据
        /// </summary>
        private List<MyERPBusinessJsModel> JsSourceCodeData { get; set; }

        /// <summary>
        /// JS语法树数据
        /// </summary>
        private List<MyERPBusinessJsSyntaxTreeModel> MyERPBusinessJsSyntaxTreeModels { get; set; }

        /// <summary>
        /// JS源码语法树聚合数据
        /// </summary>
        private List<MyERPBusJsAndTreeModel> MyERPBusJsAndTreeModels { get; set; }

        /// <summary>
        /// JS扩展方法集合
        /// </summary>
        private List<GetJsPluginFunctionResponse> JsPluginFunctionList { get; set; }

        /// <summary>
        /// JS方法引用AppService方法集合
        /// </summary>
        private List<GetJsAppServiceReferenceResponse> JsAppServiceReferenceList { get; set; }
        #endregion


        #region 窗体初始化
        public MyERPSecondDevTools()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyERPSecondDevTools_Load(object sender, EventArgs e)
        {
            InitHistoryERPPath();
            InitTxtPageUrlPlaceHolder();
        }
        #endregion

        #region 设置ERP目录、读取历史记录、设置页面地址框PlaceHolder
        /// <summary>
        /// 设置ERP目录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolSetERPPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择一个文件夹";
            dialog.ShowNewFolderButton = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (FolderHelper.GetFoldersIsExists(dialog.SelectedPath, "bin", "Customize", "App_Data"))
                {
                    FolderHelper.InitERPData(dialog.SelectedPath);
                    toolErpPathLabel.Text = dialog.SelectedPath;
                    //写入选择历史
                    AppDataHelper.WriteERPPathHistory(dialog.SelectedPath);
                    //重载目录历史
                    InitHistoryERPPath(dialog.SelectedPath);
                }
                else
                    MessageBox.Show("您选择目录不是ERP站点目录，请重新选择");
            }
        }

        /// <summary>
        /// 初始化ERP目录选择历史
        /// </summary>
        /// <param name="path">ERP目录路径</param>
        private void InitHistoryERPPath(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                //初次加载，展示所有记录
                var historyPaths = AppDataHelper.ReadERPPathHistory();
                if (historyPaths != null)
                {
                    foreach (var erpPath in historyPaths)
                    {
                        toolStripHistory.DropDownItems.Add(erpPath, null, toolStripHistory_Click);
                    }
                }
            }
            else
            {
                //后续单个载入
                toolStripHistory.DropDownItems.Add(path, null, toolStripHistory_Click);
            }
        }

        /// <summary>
        /// 选中ERP目录历史事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripHistory_Click(object sender, EventArgs e)
        {
            if (FolderHelper.GetFoldersIsExists(sender.ToString(), "bin", "Customize", "App_Data"))
            {
                toolErpPathLabel.Text = sender.ToString();
                FolderHelper.InitERPData(sender.ToString());
            }
            else
                MessageBox.Show("您选择目录不是ERP站点目录，请重新选择");
        }


        /// <summary>
        /// 加载页面地址框PlaceHolder
        /// </summary>
        private void InitTxtPageUrlPlaceHolder()
        {
            if (txt_pageUrl.Text == "")
            {
                txt_pageUrl.Text = "请输入二开的页面地址";

                txt_pageUrl.GotFocus += txtPageGotFocus;
                txt_pageUrl.LostFocus += txtPageLostFocus;
            }
        }

        /// <summary>
        /// 页面地址文本框获取焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPageGotFocus(object sender, EventArgs e)
        {
            if (txt_pageUrl.Text == "请输入二开的页面地址")
            {
                txt_pageUrl.Text = "";
            }
        }

        /// <summary>
        /// 页面地址文本框失去焦点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPageLostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_pageUrl.Text))
                txt_pageUrl.Text = "请输入二开的页面地址";
        }

        #endregion

        #region 获取异步请求结束后的页面全部响应内容并解析
        /// <summary>
        /// Fiddler点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_fiddler_Click(object sender, EventArgs e)
        {
            if (toolErpPathLabel.Text == "")
            {
                MessageBox.Show("请设置ERP站点路径");
                return;
            }

            if (txt_pageUrl.Text == "" || txt_pageUrl.Text == "请输入二开的页面地址")
            {
                MessageBox.Show("请输入二开的页面地址");
                return;
            }

            var url = FiddlerHelper.GetERPNavigationPageUrl(txt_pageUrl.Text);
            webBrowser.Navigate(url);
            webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        /// <summary>
        /// ERP站点加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //ERP站点
            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            //webBrowser事件bug，到此事件页面所有数据并未完全加载完成，借用timer不阻断webBrowser加载，执行一次，取得加载内容
            timer_GetResponse.Start();
        }

        /// <summary>
        /// Timer Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_GetResponse_Tick(object sender, EventArgs e)
        {
            MyERPResponseHtml = webBrowser.Document.All[1].OuterHtml;
            //只执行一次
            timer_GetResponse.Stop();
            HtmlAgilityPack(MyERPResponseHtml);
        }

        #endregion

        #region 解析获取的ERP网页源码数据
        /// <summary>
        /// 解析ERP二开页面HTML
        /// </summary>
        /// <param name="resHtml"></param>
        private void HtmlAgilityPack(string resHtml)
        {
            //忽略的非业务JS或者关键路径
            var ignoreScripts = new List<string>
            {
                "template.js",
                "Helper.js",
                "mapnumber.js",
                "CodeFormat.js",
                "BindData.js",
                "underscoreDeepExtend.js",
                "LangRes.js",
                "/_common/",
                "/_frontend/"
            };

            Func<string, bool> ignoreScriptFuc = path =>
            {
                return !ignoreScripts.Any(p => path.Contains(p));
            };

            var businessScripts = new List<string>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resHtml);
            var scriptList = doc.DocumentNode.SelectNodes("/html/head/script");
            foreach (var item in scriptList)
            {
                if (item.Attributes.Count > 0 && item.Attributes.Contains("src") && ignoreScriptFuc(item.Attributes["src"].Value))
                {
                    businessScripts.Add(item.Attributes["src"].Value);
                }
            }
            //业务JS源码数据
            var jsContentModels = FiddlerHelper.GetERPBusinessJsModels(businessScripts);
            JsSourceCodeData = jsContentModels;
            //请求JS解析站点
            var responseStr = ERPWebRequestHelper.GetWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "?applicationId=" + GlobalData.ApplicationId);
            //JS语法解析站点
            JsTreeSyntaxResponseHtml = responseStr;
            //获取JS语法树数据
            GetJsSyntaxTree(JsTreeSyntaxResponseHtml);
            //加载树
            InitTreeData();
        }

        /// <summary>
        /// 获取JS语法树数据
        /// </summary>
        /// <param name="responseStr">解析站点响应的数据</param>
        private void GetJsSyntaxTree(string responseStr)
        {
            MyERPBusinessJsSyntaxTreeModels = JsonConvert.DeserializeObject<List<MyERPBusinessJsSyntaxTreeModel>>(responseStr);

            //组合源码语法树聚合数据
            MyERPBusJsAndTreeModels = new List<MyERPBusJsAndTreeModel>();
            foreach (var item in JsSourceCodeData)
            {
                foreach (var item2 in MyERPBusinessJsSyntaxTreeModels)
                {
                    if (item.JsName.Equals(item2.JsName))
                    {
                        var moduleName = GetModuleNameByJsSynataxData(item2.JsSyntaxTreeJson);
                        if (moduleName != null)
                        {
                            MyERPBusJsAndTreeModel model = new MyERPBusJsAndTreeModel();
                            model.JsPath = item.JsName;
                            model.JsSourceCode = item.JsContent;
                            model.JsSyntaxTree = item2.JsSyntaxTreeJson;
                            model.JsModuleName = moduleName;
                            MyERPBusJsAndTreeModels.Add(model);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据JS语法树获取模块名
        /// </summary>
        /// <param name="jsonData"></param>
        private string GetModuleNameByJsSynataxData(string jsonData)
        {
            JObject jsonObject = (JObject)JToken.Parse(jsonData);
            var bodyType = jsonObject["body"][0]["type"];
            if (bodyType.ToString() == "ExpressionStatement")
            {
                var expression = jsonObject["body"][0]["expression"];
                if (expression["callee"]["name"].ToString() == "define")
                {
                    var moduleName = expression["arguments"][0]["value"].ToString();
                    return moduleName;
                }
            }
            return null;
        }

        /// <summary>
        /// 初始化加载树
        /// </summary>
        private void InitTreeData()
        {
            tv_code.Nodes.Clear();
            Func<string, string> convertDataLabel = label =>
            {
                return label.
                Replace("控件", string.Empty).
                Replace("页面", string.Empty).
                Replace("网格", "列表").
                Replace("表单", "信息");
            };

            var appFormDesignList = new List<DesignModel>();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(MyERPResponseHtml);
            var divDesignerList = doc.DocumentNode.SelectNodes("//div[contains(@class,'nav-designer-sub-menu-item')]");
            foreach (var item in divDesignerList)
            {
                appFormDesignList.Add(new DesignModel
                {
                    DataId = item.Attributes["data-id"].Value,
                    DataName = item.Attributes["data-name"].Value,
                    DataLabel = convertDataLabel(item.Attributes["data-label"].Value),
                    DataType = item.Attributes["data-type"].Value,
                    DataItemType = item.Attributes["data-item-type"].Value,
                });
            }
            //获取树数据
            var treeData = FiddlerHelper.GetControlMetaData(appFormDesignList);

            #region 解析JS语法请求Model
            //获取扩展方法请求Model
            GetJsPluginFunctionRequest getJsPluginFunctionRequest = new GetJsPluginFunctionRequest();
            GetJsPluginFunctionRequest getJsAppServiceReferenceRequest = new GetJsPluginFunctionRequest();
            getJsPluginFunctionRequest.applicationId = GlobalData.ApplicationId.ToString();
            getJsPluginFunctionRequest.body = new List<GetJsPluginFunctionRequestBody>();
            getJsAppServiceReferenceRequest.applicationId = GlobalData.ApplicationId.ToString();
            getJsAppServiceReferenceRequest.body = new List<GetJsPluginFunctionRequestBody>();
            #endregion

            foreach (var item in treeData)
            {
                TreeNodeExt tn = new TreeNodeExt();
                tn.Text = item.Title;
                tn.Type = "page";
                tn.Expand();
                foreach (var subItem in item.PluginPointModels)
                {
                    if (subItem.Events != null && subItem.Events.Count > 0)
                    {
                        TreeNodeExt subTn = new TreeNodeExt
                        {
                            Text = subItem.Title,
                            Tag = subItem.ControlId,
                            Type = "point"
                        };

                        foreach (var secItem in subItem.Events)
                        {
                            var trimFunctionName = secItem.FunctionName.TrimEnd(';');
                            var jsModuleName = trimFunctionName.Substring(0, trimFunctionName.LastIndexOf('.'));
                            var jsFunctionName = trimFunctionName.Substring(trimFunctionName.LastIndexOf('.') + 1);
                            TreeNodeExt secTn = new TreeNodeExt
                            {
                                Text = trimFunctionName,
                                Tag = trimFunctionName,
                                Type = "event",
                                JsModuleName = jsModuleName,
                                JsFunctionName = jsFunctionName
                            };
                            var groupData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == jsModuleName);
                            if (groupData != null)
                            {

                                var jsPath = string.Empty;
                                if (groupData.JsPath.Contains("/Customize/"))
                                    jsPath = groupData.JsPath;
                                else
                                {
                                    var tempStr = groupData.JsPath.Replace(GlobalData.ERPHost, string.Empty);
                                    jsPath = GlobalData.ERPHost + "/Customize" + tempStr;
                                }
                                jsFunctionName = jsFunctionName.Contains("(") ? jsFunctionName.Substring(0, jsFunctionName.IndexOf("(")) : jsFunctionName;
                                secTn.PluginJsPath = jsPath;
                                secTn.JsFunctionNameValue = jsFunctionName;

                                getJsPluginFunctionRequest.body.Add(new GetJsPluginFunctionRequestBody
                                {
                                    functionName = jsFunctionName,
                                    moduleName = jsPath
                                });

                                if (!secItem.IsProduct)
                                {
                                    getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                                    {
                                        functionName = jsFunctionName,
                                        moduleName = jsPath
                                    });

                                    getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                                    {
                                        functionName = jsFunctionName,
                                        moduleName = jsPath,
                                        pluginName = "before"
                                    });

                                    getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                                    {
                                        functionName = jsFunctionName,
                                        moduleName = jsPath,
                                        pluginName = "after"
                                    });

                                    getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                                    {
                                        functionName = jsFunctionName,
                                        moduleName = jsPath,
                                        pluginName = "override"
                                    });
                                }
                                getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                                {
                                    functionName = jsFunctionName,
                                    moduleName = groupData.JsPath
                                });
                            }
                            subTn.Nodes.Add(secTn);
                        }

                        tn.Nodes.Add(subTn);
                    }
                }
                tv_code.Nodes.Add(tn);
            }
            //请求解析站点，获取JS方法对应的扩展方法
            var jsonPostData = JsonConvert.SerializeObject(getJsPluginFunctionRequest);
            var responseData = ERPWebRequestHelper.PostWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "GetPluginFunction", jsonPostData, Encoding.UTF8);
            JsPluginFunctionList = JsonConvert.DeserializeObject<List<GetJsPluginFunctionResponse>>(responseData);

            //请求解析站点，获取JS方法对应的扩展方法
            var jsonPostData2 = JsonConvert.SerializeObject(getJsAppServiceReferenceRequest);
            var responseData2 = ERPWebRequestHelper.PostWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "GetAppServicesFunction", jsonPostData2, Encoding.UTF8);
            JsAppServiceReferenceList = JsonConvert.DeserializeObject<List<GetJsAppServiceReferenceResponse>>(responseData2);


            tabControl.SelectedTab = tabPage2;
        }

        /// <summary>
        /// MouseDown是鼠标按下事件发生在你鼠标单击事件之前,你单击鼠标发生了两个动作,
        /// 一是鼠标按下二是鼠标抬起.执行之后,就会把SelectedNode转变成你鼠标点的那个节点了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_code_MouseDown(object sender, MouseEventArgs e)
        {
            tv_code.SelectedNode = tv_code.GetNodeAt(e.X, e.Y);
        }

        /// <summary>
        /// TreeView节点单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_code_Click(object sender, EventArgs e)
        {
            var selectNode = (TreeNodeExt)tv_code.SelectedNode;
            if (selectNode.Type == "event" || selectNode.Type == "pluginEvent" || selectNode.Type == "appServiceEvent")
            {
                var jsSourceData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                if (jsSourceData != null)
                {
                    richTextBox1.Text = jsSourceData.JsSourceCode;

                }
                else
                    richTextBox1.Text = "未找到对应源码，请手动查找！";

                if (selectNode.Type == "event" || selectNode.Type == "pluginEvent")
                {
                    foreach (TreeNodeExt item in selectNode.Nodes)
                    {
                        InitAppService(item);
                    }
                }
            }
            else if (selectNode.Type == "point")
            {
                foreach (TreeNodeExt item in selectNode.Nodes)
                {
                    InitPluginMethod(item);
                    InitAppService(item);
                }
            }
        }

        /// <summary>
        /// 获取方法扩展列表，返回Before、After、Override
        /// </summary>
        private void InitPluginMethod(TreeNodeExt selectNode)
        {
            if (selectNode.Nodes == null || selectNode.Nodes.Count == 0)
            {
                if (JsPluginFunctionList != null && JsPluginFunctionList.Count > 0)
                {
                    var plugin = JsPluginFunctionList.FirstOrDefault(p => p.moduleName == selectNode.PluginJsPath && p.functionName == selectNode.JsFunctionNameValue);
                    if (plugin != null && plugin.pluginFunctions != null && plugin.pluginFunctions.Count > 0)
                    {
                        var pluginData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsPath == selectNode.PluginJsPath);
                        if (pluginData != null)
                        {
                            foreach (var item in plugin.pluginFunctions)
                            {
                                TreeNodeExt tn = new TreeNodeExt
                                {
                                    Text = item,
                                    Tag = item,
                                    Type = "pluginEvent",
                                    JsModuleName = !selectNode.JsModuleName.Contains("Plugin") ? selectNode.JsModuleName + ".Plugin" : selectNode.JsModuleName, //扩展方法，定义到Plugin模块
                                    JsFunctionName = selectNode.JsFunctionName,
                                    JsFunctionNameValue = selectNode.JsFunctionNameValue
                                };
                                selectNode.Nodes.Add(tn);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取方法扩展列表，返回Before、After、Override
        /// </summary>
        private void InitAppService(TreeNodeExt selectNode)
        {
            if (selectNode.Nodes == null || selectNode.Nodes.Count == 0)
            {
                if (JsAppServiceReferenceList != null && JsAppServiceReferenceList.Count > 0)
                {
                    var appServices = JsAppServiceReferenceList.FirstOrDefault(p => p.moduleName == selectNode.JsModuleName && p.functionName == selectNode.JsFunctionNameValue && p.pluginName == selectNode.Text);
                    if (appServices != null && appServices.appServiceFunctions != null && appServices.appServiceFunctions.Count > 0)
                    {
                        foreach (var item in appServices.appServiceFunctions)
                        {
                            TreeNodeExt tn = new TreeNodeExt
                            {
                                Text = item.functionName,
                                Tag = item.appServiceName + item.functionName,
                                Type = "appServiceEvent",
                                JsModuleName = item.appServiceName, //扩展方法，定义到Plugin模块
                                JsFunctionName = item.functionName,
                                JsFunctionNameValue = item.functionName
                            };
                            selectNode.Nodes.Add(tn);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
