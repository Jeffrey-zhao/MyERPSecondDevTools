using ICSharpCode.TextEditor.Document;
using Mono.Cecil;
using MyERPSecondDevTools.Common;
using MyERPSecondDevTools.Decompiler;
using MyERPSecondDevTools.Model.Enum;
using MyERPSecondDevTools.Model.Ext;
using MyERPSecondDevTools.Model.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyERPSecondDevTools
{
    public partial class MyERPSecondDevTools : Form
    {
        #region 局部变量

        /// <summary>
        /// 访问的页面地址
        /// </summary>
        private string GoPageUrl { get; set; } = string.Empty;

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

        /// <summary>
        /// JS模块所有方法数据
        /// </summary>
        private List<GetJsModuleAllFunctionResponse> JsModuleAllFunctionList { get; set; }

        /// <summary>
        /// 后台程序集信息
        /// </summary>
        private List<MyERPBusinessAssemblyInfo> MyERPBusinessAssemblyInfos { get; set; }

        /// <summary>
        /// 后台所有程序集信息的类型聚合数据
        /// </summary>
        private List<MyERPBusinessAssemblyTypeInfo> MyERPBusinessAssemblyTypeInfos { get; set; }

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
            txt_CodeView.IsReadOnly = true;
            item_after.Click += Item_after_Click;
            item_before.Click += Item_before_Click;
            item_override.Click += Item_override_Click;
            btn_Build.Visible = false;
            btn_Copy.Visible = false;
        }

        /// <summary>
        /// 窗体关闭前确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyERPSecondDevTools_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确认退出吗？", "退出询问"
                , MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result != DialogResult.OK)
            {
                e.Cancel = true;//告诉窗体关闭这个任务取消
            }
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

            button_fiddler.Enabled = false;
            txt_CodeView.Text = "";
            if (!GoPageUrl.Equals(txt_pageUrl.Text))
            {
                //新地址，浏览器跳转
                GoPageUrl = txt_pageUrl.Text;
                var url = FiddlerHelper.GetERPNavigationPageUrl(txt_pageUrl.Text);
                webBrowser.Navigate(url);
                webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
            }
            else
            {
                //再次解析页面
                while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                MyERPResponseHtml = webBrowser.Document.All[1].OuterHtml;
                HtmlAgilityPack(MyERPResponseHtml);
            }
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

            var tdDocument = webBrowser.Document.GetElementById("appForm_mainTabId");
            if (tdDocument != null)
            {
                var tempDocument = tdDocument.GetElementsByTagName("td");

                foreach (HtmlElement td in tempDocument)
                {
                    //查找页面的选项卡元素，绑定点击事件
                    if (td.GetAttribute("className").Contains("mini-tab"))
                    {
                        td.Click += Td_Click;
                    }
                }
            }
        }

        /// <summary>
        /// 页面选项页签的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Td_Click(object sender, HtmlElementEventArgs e)
        {
            timer_InvorkScript.Start();
        }

        /// <summary>
        /// 执行页面脚本Timer，延迟执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_InvorkScript_Tick(object sender, EventArgs e)
        {
            timer_InvorkScript.Stop();
            HtmlElement ele = webBrowser.Document.CreateElement("script");
            ele.SetAttribute("type", "text/javascript");
            ele.SetAttribute("text", "function webBrowerMouse(){ $('#design-mode-entrance').find('.btn-menu').trigger('mouseover'); $('.nav-setting-entrance-menu').hide(); }");
            webBrowser.Document.Body.AppendChild(ele);
            webBrowser.Document.InvokeScript("webBrowerMouse");
        }

        #endregion

        #region 解析获取的ERP网页源码数据
        /// <summary>
        /// 解析ERP二开页面HTML
        /// </summary>
        /// <param name="resHtml"></param>
        private void HtmlAgilityPack(string resHtml)
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show("站点访问异常，请确认能够正常访问！");
                button_fiddler.Enabled = true;
            }
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
                    if (item.JsName.Equals(item2.JsPath))
                    {
                        MyERPBusJsAndTreeModel model = new MyERPBusJsAndTreeModel();
                        model.JsPath = item.JsName;
                        model.JsSourceCode = item.JsContent;
                        model.JsSyntaxTree = item2.JsSyntaxTreeJson;
                        model.JsModuleName = item2.JsModuleName;
                        model.JsLocalPath = GlobalData.ERPPath + model.JsPath.Replace(GlobalData.ERPHost, string.Empty);
                        model.JsLocalPath = model.JsLocalPath.Replace("/", @"\");
                        MyERPBusJsAndTreeModels.Add(model);
                    }
                }
            }
        }
        #endregion

        #region 加载树数据

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
            if (divDesignerList.Count > 0)
            {
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
                getJsPluginFunctionRequest.applicationId = GlobalData.ApplicationId.ToString();
                getJsPluginFunctionRequest.body = new List<GetJsPluginFunctionRequestBody>();
                //请求方法引用的AppService
                GetJsPluginFunctionRequest getJsAppServiceReferenceRequest = new GetJsPluginFunctionRequest();
                getJsAppServiceReferenceRequest.applicationId = GlobalData.ApplicationId.ToString();
                getJsAppServiceReferenceRequest.body = new List<GetJsPluginFunctionRequestBody>();
                //请求模块下所有方法及扩展方法
                GetJsModuleAllFunctionRequest getJsModuleAllFunctionRequest = new GetJsModuleAllFunctionRequest();
                getJsModuleAllFunctionRequest.applicationId = GlobalData.ApplicationId.ToString();
                getJsModuleAllFunctionRequest.body = new List<GetJsModuleAllFunctionRequestItem>();

                MyERPBusJsAndTreeModels.ForEach(m =>
                {
                    getJsModuleAllFunctionRequest.body.Add(new GetJsModuleAllFunctionRequestItem
                    {
                        moduleName = m.JsModuleName
                    });
                });
                //请求解析站点，获取JS方法对应的扩展方法
                var jsonPostData3 = JsonConvert.SerializeObject(getJsModuleAllFunctionRequest);
                var responseData3 = ERPWebRequestHelper.PostWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "GetAllFunctionByModule", jsonPostData3, Encoding.UTF8);
                JsModuleAllFunctionList = JsonConvert.DeserializeObject<List<GetJsModuleAllFunctionResponse>>(responseData3);

                JsModuleAllFunctionList.ForEach(m =>
                {
                    var groupData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == m.moduleName);
                    if (groupData != null)
                    {
                        m.functions.ForEach(mItem =>
                        {
                            var jsModuleName = m.moduleName;
                            var jsFunctionName = mItem.name;

                            jsFunctionName = jsFunctionName.Contains("(") ? jsFunctionName.Substring(0, jsFunctionName.IndexOf("(")) : jsFunctionName;

                            var pluginModuleName = jsModuleName.Contains("Plugin") ? jsModuleName : jsModuleName + ".Plugin";
                            //请求模块扩展方法，传入扩展模块名称
                            getJsPluginFunctionRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = pluginModuleName
                            });

                            //扩展方法的引用
                            getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = pluginModuleName
                            });

                            getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = pluginModuleName,
                                pluginName = "before"
                            });

                            getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = pluginModuleName,
                                pluginName = "after"
                            });

                            getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = pluginModuleName,
                                pluginName = "override"
                            });

                            //常规方法引用
                            getJsAppServiceReferenceRequest.body.Add(new GetJsPluginFunctionRequestBody
                            {
                                functionName = jsFunctionName,
                                moduleName = jsModuleName
                            });
                        });
                    }
                });
                #endregion

                #region 加载平台常用引用
                TreeNodeExt firstTn = new TreeNodeExt();
                firstTn.Text = "平台常用引用";
                firstTn.Expand();
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
                                Type = "point",
                                GlobalType = "front"
                            };

                            foreach (var secItem in subItem.Events)
                            {
                                var trimFunctionName = secItem.FunctionName.TrimEnd(';');
                                var jsModuleName = trimFunctionName.Substring(0, trimFunctionName.LastIndexOf('.'));
                                var jsFunctionName = trimFunctionName.Substring(trimFunctionName.LastIndexOf('.') + 1);
                                var jsFunctionInfo = GetJsFunctionNameInfo(jsFunctionName);
                                TreeNodeExt secTn = new TreeNodeExt
                                {
                                    Text = trimFunctionName,
                                    Tag = trimFunctionName,
                                    Type = "event",
                                    JsModuleName = jsModuleName,
                                    JsFunctionName = jsFunctionInfo.Item1,
                                    JsFunctionNameValue = jsFunctionInfo.Item1,
                                    GlobalType = "front",
                                    ContextMenuStrip = contextMenuStrip,
                                    JsArgsParams = jsFunctionInfo.Item2
                                };
                                jsFunctionName = jsFunctionName.Contains("(") ? jsFunctionName.Substring(0, jsFunctionName.IndexOf("(")) : jsFunctionName;
                                secTn.JsFunctionNameValue = jsFunctionName;
                                subTn.Nodes.Add(secTn);
                            }

                            tn.Nodes.Add(subTn);
                        }
                    }
                    firstTn.Nodes.Add(tn);
                }
                tv_code.Nodes.Add(firstTn);
                #endregion

                #region 所有引用
                TreeNodeExt firstTn2 = new TreeNodeExt();
                firstTn2.Text = "所有引用";
                firstTn2.Expand();
                MyERPBusJsAndTreeModels.ForEach(m =>
                {
                    TreeNodeExt tn = new TreeNodeExt();
                    tn.Text = m.JsModuleName;
                    tn.Type = "point";
                    tn.JsModuleName = m.JsModuleName;
                    tn.GlobalType = "front";
                    //记载模块下所有方法
                    var moduleAllFunctions = JsModuleAllFunctionList.FirstOrDefault(p => p.moduleName == m.JsModuleName);
                    if (moduleAllFunctions != null)
                    {
                        foreach (var item in moduleAllFunctions.functions)
                        {
                            TreeNodeExt subNode = new TreeNodeExt();
                            subNode.Text = item.type == "plugin" ? "Plugin：" + item.name : item.name;
                            subNode.Type = "event";
                            subNode.JsModuleName = moduleAllFunctions.moduleName;
                            subNode.JsFunctionName = item.name;
                            subNode.JsFunctionNameValue = item.name;
                            subNode.JsArgsParams = item.argsParams;
                            subNode.GlobalType = "front";
                            subNode.ContextMenuStrip = contextMenuStrip;
                            tn.Nodes.Add(subNode);
                        }
                    }

                    firstTn2.Nodes.Add(tn);
                });
                tv_code.Nodes.Add(firstTn2);
                #endregion

                #region 请求JS语法解析
                //请求解析站点，获取JS方法对应的扩展方法
                var jsonPostData = JsonConvert.SerializeObject(getJsPluginFunctionRequest);
                var responseData = ERPWebRequestHelper.PostWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "GetPluginFunction", jsonPostData, Encoding.UTF8);
                JsPluginFunctionList = JsonConvert.DeserializeObject<List<GetJsPluginFunctionResponse>>(responseData);

                //请求解析站点，获取JS方法对应的AppService方法引用
                var jsonPostData2 = JsonConvert.SerializeObject(getJsAppServiceReferenceRequest);
                var responseData2 = ERPWebRequestHelper.PostWebRequest(GlobalData.ToolsJsSyntaxAnalysisWebSite + "GetAppServicesFunction", jsonPostData2, Encoding.UTF8);
                JsAppServiceReferenceList = JsonConvert.DeserializeObject<List<GetJsAppServiceReferenceResponse>>(responseData2);

                #endregion

                tabControl.SelectedTab = tabPage2;

                //后台线程程序集还没有加载完毕，需要等待执行成功
                while (!FolderHelper.IsInitAssemblySuccess())
                {
                    Thread.Sleep(500);
                }
                MyERPBusinessAssemblyInfos = GlobalData.MyERPBusinessAssemblyInfos.ToList();
                MyERPBusinessAssemblyTypeInfos = MyERPBusinessAssemblyInfos.Aggregate(Enumerable.Empty<MyERPBusinessAssemblyTypeInfo>(), (total, next) =>
                {
                    return total.Union(next.Types);
                }).ToList();
                //初始化IOC映射信息
                FolderHelper.InitIOCMapping();

                button_fiddler.Enabled = true;
            }
            else
            {
                MessageBox.Show("站点访问异常，请确认能够正常访问！");
                button_fiddler.Enabled = true;
            }
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
            btn_Build.Visible = false;
            btn_Copy.Visible = false;

            var selectNode = (TreeNodeExt)tv_code.SelectedNode;
            if (selectNode.GlobalType == "front")
            {
                txt_CodeView.SetHighlighting("JavaScript");
                if (selectNode.Type == "event" || selectNode.Type == "pluginEvent" || selectNode.Type == "appServiceEvent" || selectNode.Type == "allPoint" || selectNode.Type == "point")
                {
                    var jsSourceData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                    if (jsSourceData != null)
                    {
                        txt_CodeView.Text = "//代码文件路径:" + jsSourceData.JsLocalPath + "\n" + jsSourceData.JsSourceCode;
                    }

                    if (selectNode.Type == "event" || selectNode.Type == "pluginEvent")
                    {
                        foreach (TreeNodeExt item in selectNode.Nodes)
                        {
                            InitAppService(item);
                        }
                    }

                    if (selectNode.Type == "point")
                    {
                        foreach (TreeNodeExt item in selectNode.Nodes)
                        {
                            InitPluginMethod(item);
                            InitAppService(item);
                        }
                    }

                    SelectKeyWord(selectNode.JsFunctionName, "front");
                }
            }
            else
            {
                if (selectNode.Type == "appServiceEvent")
                {
                    txt_CodeView.SetHighlighting("JavaScript");
                    var jsSourceData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                    if (jsSourceData != null)
                    {
                        txt_CodeView.Text = "//代码文件路径:" + jsSourceData.JsLocalPath + "\n" + jsSourceData.JsSourceCode;
                    }

                    SelectKeyWord(selectNode.JsFunctionName, "front");
                }
                else if (selectNode.Type == "backMethod" || selectNode.Type == "backMethodReference" || selectNode.Type == "backMethodReferenceMethod")
                {
                    txt_CodeView.SetHighlighting("C#");
                    try
                    {
                        txt_CodeView.Text = "正在反编译源代码，请稍后。。。";
                        MyERPBusinessAssemblyInfo assemblyInfo = null;
                        MyERPBusinessAssemblyTypeInfo typeInfo = null;
                        foreach (var assembly in MyERPBusinessAssemblyInfos)
                        {
                            var searchType = assembly.Types.FirstOrDefault(p => p.TypeFullName == selectNode.CsTypeName);
                            if (searchType != null)
                            {
                                assemblyInfo = assembly;
                                typeInfo = searchType;
                                break;
                            }
                        }
                        if (assemblyInfo == null)
                            return;
                        var textOutPut = DecompilerHelper.GetDecompilerTypeInfo(GlobalData.ERPPath + @"\bin", assemblyInfo.AssemblyPath, selectNode.CsTypeName);
                        txt_CodeView.Text = textOutPut.b.ToString();
                    }
                    catch (Exception)
                    {
                        txt_CodeView.Text = "反编译出现错误，请重试！";
                    }

                    if (selectNode.Type == "backMethod")
                    {
                        //加载后端方法引用的方法列表
                        foreach (TreeNodeExt item in selectNode.Nodes)
                        {
                            InitBackMehtodReferenceMethod(item);
                        }
                    }

                    SelectKeyWord(selectNode.CsMethodName, "back");
                }
                else if (selectNode.Type == "backType")
                {
                    //加载后端方法引用
                    foreach (TreeNodeExt item in selectNode.Nodes)
                    {
                        InitBackMethodReference(item);
                    }
                }
            }

            foreach (TreeNodeExt item in selectNode.Nodes)
            {
                InitBackMethod(item);
            }
        }

        /// <summary>
        /// 获取方法扩展列表，返回Before、After、Override
        /// </summary>
        private void InitPluginMethod(TreeNodeExt selectNode)
        {
            var isExists = false;
            foreach (TreeNodeExt node in selectNode.Nodes)
            {
                if (node.Type == "pluginEvent")
                {
                    isExists = true;
                    break;
                }
            }
            if (!isExists)
            {
                if (JsPluginFunctionList != null && JsPluginFunctionList.Count > 0)
                {
                    var selectJsModuleName = selectNode.JsModuleName.Contains("Plugin") ? selectNode.JsModuleName : selectNode.JsModuleName + ".Plugin";
                    var plugin = JsPluginFunctionList.FirstOrDefault(p => p.moduleName == selectJsModuleName && p.functionName == selectNode.JsFunctionNameValue);
                    if (plugin != null && plugin.pluginFunctions != null && plugin.pluginFunctions.Count > 0)
                    {
                        var goupData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectJsModuleName);
                        foreach (var item in plugin.pluginFunctions)
                        {
                            var jsFunctionInfo = GetJsFunctionNameInfo(selectNode.JsFunctionName);

                            TreeNodeExt tn = new TreeNodeExt
                            {
                                Text = item,
                                Tag = item,
                                Type = "pluginEvent",
                                JsModuleName = !selectNode.JsModuleName.Contains("Plugin") ? selectNode.JsModuleName + ".Plugin" : selectNode.JsModuleName, //扩展方法，定义到Plugin模块
                                JsFunctionName = jsFunctionInfo.Item1,
                                JsFunctionNameValue = jsFunctionInfo.Item1,
                                JsArgsParams = jsFunctionInfo.Item2,
                                ToolTipText = goupData.JsPath,
                                GlobalType = "front"
                            };
                            selectNode.Nodes.Add(tn);
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
            var isExists = false;
            foreach (TreeNodeExt node in selectNode.Nodes)
            {
                if (node.Type == "appServiceEvent")
                {
                    isExists = true;
                    break;
                }
            }

            if (!isExists)
            {
                if (JsAppServiceReferenceList != null && JsAppServiceReferenceList.Count > 0)
                {
                    Func<GetJsAppServiceReferenceResponse, string, bool> compare = (model, selectText) =>
                    {
                        //如果是产品js的AppService引用，不参与筛选
                        if (model.pluginName == null)
                            return true;
                        if (model == null)
                            return false;

                        if ((new string[] { "before", "after", "override" }).Contains(selectText))
                        {
                            return model.pluginName.Equals(selectText);
                        }
                        else
                        {
                            return true;
                        }
                    };
                    var appServices = JsAppServiceReferenceList.FirstOrDefault(p => p.moduleName == selectNode.JsModuleName && p.functionName == selectNode.JsFunctionNameValue && compare(p, selectNode.Text));
                    if (appServices != null && appServices.appServiceFunctions != null && appServices.appServiceFunctions.Count > 0)
                    {
                        var goupData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                        foreach (var item in appServices.appServiceFunctions)
                        {
                            TreeNodeExt tn = new TreeNodeExt
                            {
                                Text = "AppService：" + item.functionName,
                                Tag = item.appServiceName + item.functionName,
                                Type = "appServiceEvent",
                                JsModuleName = item.appServiceName, //扩展方法，定义到Plugin模块
                                JsFunctionName = item.functionName,
                                JsFunctionNameValue = item.functionName,
                                GlobalType = "back",
                                ContextMenuStrip = contextMenuStrip
                            };
                            selectNode.Nodes.Add(tn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载后台AppService方法
        /// </summary>
        /// <param name="selectNode"></param>
        private void InitBackMethod(TreeNodeExt selectNode)
        {
            var isExists = false;
            foreach (TreeNodeExt node in selectNode.Nodes)
            {
                if (node.Type == "backType" || node.Type == "backMethod")
                {
                    isExists = true;
                    break;
                }
            }

            if (!isExists)
            {
                var type = MyERPBusinessAssemblyTypeInfos.FirstOrDefault(p => p.TypeFullName == selectNode.JsModuleName);
                if (type != null)
                {
                    TreeNodeExt node = new TreeNodeExt { Text = "后端引用", Type = "backType", GlobalType = "back" };
                    type.Methods.ForEach(m =>
                    {
                        if (m.MethodName == selectNode.JsFunctionName)
                        {
                            var showMethodName = string.Empty;
                            if (m.Paramters.Count > 0)
                            {
                                var sb = new StringBuilder();
                                foreach (var item in m.Paramters)
                                {
                                    sb.AppendFormat("{0} {1},", item.ParameterType, item.ParameterName);
                                }
                                var tempStr = sb.ToString().TrimEnd(',');
                                showMethodName = type.TypeName + "." + m.MethodName + "(" + tempStr + ")";
                            }
                            else
                            {
                                showMethodName = type.TypeName + "." + m.MethodName;
                            }
                            TreeNodeExt subnode = new TreeNodeExt
                            {
                                Text = showMethodName,
                                CsMethodName = m.MethodName,
                                CsTypeName = type.TypeFullName,
                                Tag = type.TypeFullName + "." + m.MethodName,
                                Type = "backMethod",
                                GlobalType = "back",
                                ContextMenuStrip = contextMenuStrip
                            };

                            node.Nodes.Add(subnode);
                        }
                    });

                    selectNode.Nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 初始化后端方法引用
        /// </summary>
        private void InitBackMethodReference(TreeNodeExt selectNode)
        {
            var isExists = false;
            foreach (TreeNodeExt node in selectNode.Nodes)
            {
                if (node.Type == "backMethodReference")
                {
                    isExists = true;
                    break;
                }
            }

            if (!isExists)
            {
                var typeInfo = MyERPBusinessAssemblyTypeInfos.FirstOrDefault(p => p.TypeFullName == selectNode.CsTypeName);
                if (typeInfo != null)
                {
                    Dictionary<string, string> referenceTypes = new Dictionary<string, string>();
                    var isExistsPublicInterface = typeInfo.Fields.Any(a => a.FieldTypeName.Contains("PublicService"));
                    typeInfo.Fields.ForEach(f =>
                    {
                        f.FieldTypeName = DecompilerHelper.GetFieldTypeName(f.FieldTypeName);
                        if (f.FieldTypeName.EndsWith("DomainService"))
                        {
                            referenceTypes.Add(f.FieldName, f.FieldTypeName);
                        }
                        if (f.FieldTypeName.EndsWith("PublicService"))
                        {
                            if (isExistsPublicInterface && GlobalData.IOCMapping.ContainsKey(f.FieldTypeName))
                            {
                                var value = GlobalData.IOCMapping[f.FieldTypeName];
                                if (value != null)
                                    referenceTypes.Add(f.FieldName, value);
                            }
                        }
                    });
                    foreach (var dict in referenceTypes)
                    {
                        TreeNodeExt node = new TreeNodeExt
                        {
                            Text = dict.Key + ": " + dict.Value,
                            Type = "backMethodReference",
                            GlobalType = "back",
                            CsTypeName = dict.Value,
                        };

                        selectNode.Nodes.Add(node);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化后端方法引用的相关方法
        /// </summary>
        /// <param name="selectNoe"></param>
        private void InitBackMehtodReferenceMethod(TreeNodeExt selectNode)
        {
            var isExists = false;
            foreach (TreeNodeExt node in selectNode.Nodes)
            {
                if (node.Type == "backMethodReferenceMethod")
                {
                    isExists = true;
                    break;
                }
            }

            if (!isExists)
            {
                var typeInfo = MyERPBusinessAssemblyTypeInfos.FirstOrDefault(p => p.TypeFullName == selectNode.CsTypeName);
                if (typeInfo != null)
                {
                    typeInfo.Methods.ForEach(m =>
                    {
                        var showMethodName = string.Empty;
                        if (m.Paramters.Count > 0)
                        {
                            var sb = new StringBuilder();
                            foreach (var item in m.Paramters)
                            {
                                sb.AppendFormat("{0} {1},", item.ParameterType, item.ParameterName);
                            }
                            var tempStr = sb.ToString().TrimEnd(',');
                            showMethodName = m.MethodName + "(" + tempStr + ")";
                        }
                        else
                        {
                            showMethodName = m.MethodName;
                        }
                        TreeNodeExt subnode = new TreeNodeExt
                        {
                            Text = showMethodName,
                            CsMethodName = m.MethodName,
                            CsTypeName = typeInfo.TypeFullName,
                            Tag = m.MethodName,
                            Type = "backMethodReferenceMethod",
                            GlobalType = "back",
                            ContextMenuStrip = contextMenuStrip
                        };

                        selectNode.Nodes.Add(subnode);
                    });
                }
            }
        }

        /// <summary>
        /// 选中关键字，跳转到指定行
        /// </summary>
        /// <param name="keyWord"></param>
        private void SelectKeyWord(string keyWord, string type)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                if (type == "back")
                    keyWord = " " + keyWord + "(";
                else if (type == "front")
                    keyWord = keyWord + ":";
                var text = txt_CodeView.Text;
                if (text.Contains(keyWord))
                {
                    var positionIndex = text.IndexOf(keyWord);

                    //设置选择的文本。
                    var start = this.txt_CodeView.Document.OffsetToPosition(positionIndex);
                    var end = this.txt_CodeView.Document.OffsetToPosition(positionIndex + keyWord.Length);
                    this.txt_CodeView.ActiveTextAreaControl.SelectionManager.SetSelection(new DefaultSelection(this.txt_CodeView.Document, start, end));

                    //滚动到选择的位置。
                    end.Line += 30;
                    this.txt_CodeView.ActiveTextAreaControl.Caret.Position = end;
                    this.txt_CodeView.ActiveTextAreaControl.TextArea.ScrollToCaret();
                }
            }
        }

        /// <summary>
        /// 根据Js方法全称拆分出来方法名和参数列表
        /// </summary>
        /// <param name="jsFunctionName"></param>
        /// <returns></returns>
        private Tuple<string, List<string>> GetJsFunctionNameInfo(string jsFunctionName)
        {
            if (jsFunctionName.Contains("(") && jsFunctionName.Contains(")"))
            {
                var functionName = jsFunctionName.Substring(0, jsFunctionName.IndexOf("("));
                var paramStr = jsFunctionName.Substring(jsFunctionName.IndexOf("("));
                paramStr = paramStr.TrimStart('(');
                paramStr = paramStr.TrimEnd(')');
                var tempStrList = paramStr.Split(',');
                return new Tuple<string, List<string>>(functionName, tempStrList.ToList());
            }

            return new Tuple<string, List<string>>(jsFunctionName, null);
        }
        #endregion

        #region 扩展操作功能
        /// <summary>
        /// 扩展模块方法或者后台方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PluginModule(string type)
        {
            var selectNode = (TreeNodeExt)tv_code.SelectedNode;
            if (selectNode.GlobalType == "front")
            {
                #region 前端扩展
                var moduleInfo = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                if (moduleInfo.JsModuleName.EndsWith(".Plugin") || moduleInfo.JsLocalPath.Contains("/Customize/"))
                {
                    MessageBox.Show("此模块为二开模块，不能扩展！");
                    return;
                }

                var isExistsPlugin = false;
                foreach (TreeNodeExt node in selectNode.Nodes)
                {
                    var pluginList = new List<string> { "after", "before", "override" };
                    if (type.Equals(node.Text))
                    {
                        isExistsPlugin = true;
                        break;
                    }
                }
                if (isExistsPlugin)
                {
                    MessageBox.Show($"{type}扩展已存在");
                    return;
                }

                //查询扩展模块是否存在
                var pluginModule = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName + ".Plugin");
                if (pluginModule == null)
                {
                    var templateText = File.ReadAllText("../../Template/PluginModuleTemplate.js");
                    templateText = templateText.Replace("{{modulePluginName}}", selectNode.JsModuleName + ".Plugin");
                    templateText = templateText.Replace("{{functionName}}", selectNode.JsFunctionName);
                    templateText = templateText.Replace("{{pluginType}}", type);
                    var paramSb = new StringBuilder();
                    selectNode.JsArgsParams.ForEach(m =>
                    {
                        if (m == "e")
                            paramSb.Append("args, ");
                        else
                            paramSb.Append(m + ", ");
                    });
                    paramSb.Append("$e");
                    templateText = templateText.Replace("{{functionParams}}", paramSb.ToString());
                    txt_CodeView.Text = templateText;
                }
                else
                {
                    //查找__module_plugins__代码块的结束所在位置
                    Func<string, Tuple<int, int>> GetModulePluginEndPosition = text =>
                    {
                        var resultPosition = 0;
                        var resultPluginMethodEndPosition = 0;
                        var pluginModulePosition = text.IndexOf("__module_plugins__");
                        //先进后出集合
                        Stack<int> startBraces = new Stack<int>();
                        //记录扩展模块对应的大括号的集合个数
                        var pluginStartListCount = 0;
                        var charArray = text.ToCharArray();
                        for (int i = 0; i < charArray.Length; i++)
                        {
                            //遇到大括号开头，加入集合
                            if (charArray[i] == '{')
                            {
                                startBraces.Push(i);
                                //已经找到扩展代码块的位置，跳出循环
                                if (i > pluginModulePosition && pluginStartListCount == 0)
                                {
                                    pluginStartListCount = startBraces.Count;
                                }
                            }
                            else if (charArray[i] == '}')
                            {
                                if (startBraces.Count == pluginStartListCount + 1)
                                {
                                    //表明是扩展代码块最后一个方法结尾
                                    if (charArray[i + 1] != ',')
                                    {
                                        resultPluginMethodEndPosition = i;
                                    }
                                }
                                startBraces.Pop(); //移除掉最近一个大括号开始
                                if (startBraces.Count < pluginStartListCount)
                                {
                                    resultPosition = i;
                                    break;
                                }
                            }
                        }

                        return new Tuple<int, int>(resultPosition, resultPluginMethodEndPosition);
                    };

                    //扩展模块脚本
                    var pluginText = File.ReadAllText(pluginModule.JsLocalPath);
                    //查找__module_plugins__关键字所在位置
                    var positionTuple = GetModulePluginEndPosition(pluginText);
                    var ntext = pluginText;
                    if (positionTuple.Item2 != 0)
                    {
                        ntext = pluginText.Insert(positionTuple.Item2 + 1, ",");
                    }
                    var templateText = File.ReadAllText("../../Template/PluginJsFunctionTemplate.js");
                    templateText = templateText.Replace("{{functionName}}", selectNode.JsFunctionName);
                    templateText = templateText.Replace("{{pluginType}}", type);
                    var paramSb = new StringBuilder();
                    selectNode.JsArgsParams.ForEach(m =>
                    {
                        if (m == "e")
                            paramSb.Append("args, ");
                        else
                            paramSb.Append(m + ", ");
                    });
                    paramSb.Append("$e");
                    templateText = templateText.Replace("{{functionParams}}", paramSb.ToString());
                    ntext = ntext.Insert(positionTuple.Item1 - 1, templateText);
                    txt_CodeView.Text = ntext;

                    //设置选择的文本。
                    var start = this.txt_CodeView.Document.OffsetToPosition(positionTuple.Item1 - 1 + 2);
                    var end = this.txt_CodeView.Document.OffsetToPosition(positionTuple.Item1 - 1 + templateText.Length - 6);
                    this.txt_CodeView.ActiveTextAreaControl.SelectionManager.SetSelection(new DefaultSelection(this.txt_CodeView.Document, start, end));

                    //滚动到选择的位置。
                    end.Line += 30;
                    this.txt_CodeView.ActiveTextAreaControl.Caret.Position = end;
                    this.txt_CodeView.ActiveTextAreaControl.TextArea.ScrollToCaret();
                }

                btn_Build.Visible = true;
                btn_Copy.Visible = true;
                #endregion
            }
            else if (selectNode.GlobalType == "back")
            {
                #region 后端扩展
                var typeInfo = MyERPBusinessAssemblyTypeInfos.FirstOrDefault(p => p.TypeFullName == selectNode.CsTypeName);
                if (typeInfo != null)
                {
                    var method = typeInfo.Methods.FirstOrDefault(p => p.MethodName == selectNode.CsMethodName);
                    if (method != null)
                    {
                        if (!method.IsPublic)
                            MessageBox.Show("此方法不是Public访问修饰，不能扩展！");
                    }

                    var pluginType = string.Empty;
                    var returnType = string.Empty;
                    var notesReturnType = string.Empty;
                    switch (type)
                    {
                        case "before":
                            pluginType = "Before";
                            returnType = "void";
                            break;
                        case "after":
                            pluginType = "After";
                            returnType = "void";
                            break;
                        case "override":
                            pluginType = "Override";
                            returnType = DecompilerHelper.GetFieldTypeName(method.ReturnType);
                            notesReturnType = "        /// <returns></returns>\r\n";
                            break;
                    }

                    var templateText = File.ReadAllText("../../Template/CsharpPluginMethodTemplate.txt");
                    templateText = templateText.Replace("{{methodFullName}}", selectNode.CsTypeName + "." + selectNode.CsMethodName);
                    templateText = templateText.Replace("{{pluginType}}", pluginType);
                    templateText = templateText.Replace("{{pluginReturnType}}", returnType);
                    templateText = templateText.Replace("{{pluginMethodName}}", method.MethodName);
                    templateText = templateText.Replace("{{notes}}", method.MethodName + "方法扩展");
                    templateText = templateText.Replace("{{notesReturnType}}", notesReturnType);
                    var paramSb = new StringBuilder();
                    var notesParamSb = new StringBuilder();
                    method.Paramters.ForEach(m =>
                    {
                        paramSb.Append(m.ParameterType + " " + m.ParameterName + ",");
                        notesParamSb.AppendLine(string.Format(@"        /// <param name=""{0}""></param>", m.ParameterName));
                    });
                    templateText = templateText.Replace("{{notesParams}}", notesParamSb.ToString());
                    templateText = templateText.Replace("{{methodParams}}", paramSb.ToString().TrimEnd(','));
                    templateText = templateText.Replace(@"\r\n\r\n", @"\r\n");
                    txt_CodeView.Text = templateText;
                    btn_Copy.Visible = true;
                }
                #endregion
            }
        }

        /// <summary>
        /// Override点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_override_Click(object sender, EventArgs e)
        {
            PluginModule("override");
        }

        /// <summary>
        /// Before点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_before_Click(object sender, EventArgs e)
        {
            PluginModule("before");
        }

        /// <summary>
        /// After点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_after_Click(object sender, EventArgs e)
        {
            PluginModule("after");
        }

        /// <summary>
        /// 复制到粘贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Copy_Click(object sender, EventArgs e)
        {
            if (txt_CodeView.ActiveTextAreaControl.SelectionManager.SelectedText != "")
            {
                Clipboard.SetText(txt_CodeView.ActiveTextAreaControl.SelectionManager.SelectedText);
                MessageBox.Show("已复制所选内容");
            }
            else
            {
                Clipboard.SetText(txt_CodeView.Text);
                MessageBox.Show("已复制全部内容");
            }
        }

        /// <summary>
        /// 生成到项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Build_Click(object sender, EventArgs e)
        {
            var selectNode = (TreeNodeExt)tv_code.SelectedNode;
            if (selectNode.GlobalType == "front")
            {
                DialogResult result = MessageBox.Show("确认生成到项目中吗？", "生成到项目中询问"
                , MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result != DialogResult.OK)
                {
                    return;
                }
                var filePath = string.Empty;
                var pluginModule = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName + ".Plugin");
                if (pluginModule != null)
                {
                    filePath = pluginModule.JsLocalPath;
                }
                else
                {
                    var productModuleInfo = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                    filePath = GlobalData.ERPPath + @"\Customize\" + productModuleInfo.JsLocalPath.Replace(GlobalData.ERPPath, string.Empty);
                }
                
                FileInfo fileInfo = new FileInfo(filePath);
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                File.WriteAllText(filePath, txt_CodeView.Text);
                var erpPath = new DirectoryInfo(GlobalData.ERPPath);
                var parentPath = erpPath.Parent;
                var frontBuildFile = parentPath.GetFiles("前端编译.cmd");
                if (frontBuildFile.Length > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        //执行前端编译
                        Process proc = new Process();
                        string targetDir = string.Format(frontBuildFile[0].DirectoryName);

                        proc.StartInfo.WorkingDirectory = targetDir;
                        proc.StartInfo.FileName = frontBuildFile[0].Name;
                        proc.StartInfo.Arguments = string.Format("10");

                        proc.Start();
                        proc.WaitForExit();
                    });
                }

                //MessageBox.Show($"生成成功，生成的JS文件路径：{filePath}");
            }
        }
        #endregion
    }
}
