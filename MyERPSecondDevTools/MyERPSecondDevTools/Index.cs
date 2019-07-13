using MyERPSecondDevTools.Common;
using MyERPSecondDevTools.Decompiler;
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

        /// <summary>
        /// JS模块所有方法数据
        /// </summary>
        private List<GetJsModuleAllFunctionResponse> JsModuleAllFunctionList { get; set; }

        /// <summary>
        /// 后台程序集信息
        /// </summary>
        private List<MyERPBusinessAssemblyInfo> MyERPBusinessAssemblyInfos { get; set; }
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
            if (!FolderHelper.IsInitAssemblySuccess())
            {
                MessageBox.Show("程序集信息还未加载完成，请稍后再试");
                return;
            }
            else
            {
                MyERPBusinessAssemblyInfos = GlobalData.MyERPBusinessAssemblyInfos.ToList();
            }

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
                            TreeNodeExt secTn = new TreeNodeExt
                            {
                                Text = trimFunctionName,
                                Tag = trimFunctionName,
                                Type = "event",
                                JsModuleName = jsModuleName,
                                JsFunctionName = jsFunctionName,
                                GlobalType = "front",
                                ContextMenuStrip = contextMenuStrip
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
            if (selectNode.GlobalType == "front")
            {
                if (selectNode.Type == "event" || selectNode.Type == "pluginEvent" || selectNode.Type == "appServiceEvent" || selectNode.Type == "allPoint")
                {
                    var jsSourceData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                    if (jsSourceData != null)
                    {
                        richTextBox1.Text = "//代码文件路径:" + jsSourceData.JsLocalPath + "\n" + jsSourceData.JsSourceCode;
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
            else
            {
                if (selectNode.Type == "appServiceEvent")
                {
                    var jsSourceData = MyERPBusJsAndTreeModels.FirstOrDefault(p => p.JsModuleName == selectNode.JsModuleName);
                    if (jsSourceData != null)
                    {
                        richTextBox1.Text = "//代码文件路径:" + jsSourceData.JsLocalPath + "\n" + jsSourceData.JsSourceCode;
                    }
                    else
                        richTextBox1.Text = "未找到对应源码，请手动查找！";
                }
                else if (selectNode.Type == "backMethod")
                {
                    try
                    {
                        richTextBox1.Text = "正在反编译源代码，请稍后。。。";
                        MyERPBusinessAssemblyInfo assemblyInfo = null;
                        foreach (var assembly in MyERPBusinessAssemblyInfos)
                        {
                            var searchType = assembly.Types.FirstOrDefault(p => p.TypeFullName == selectNode.CsTypeName);
                            if (searchType != null)
                            {
                                assemblyInfo = assembly;
                                break;
                            }
                        }
                        if (assemblyInfo == null)
                            return;
                        var textOutPut = DecompilerHelper.GetDecompilerTypeInfo(GlobalData.ERPPath + @"\bin", assemblyInfo.AssemblyPath, selectNode.CsTypeName);
                        richTextBox1.Text = textOutPut.b.ToString();
                    }
                    catch (Exception)
                    {
                        richTextBox1.Text = "反编译出现错误，请重试！";
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
                            TreeNodeExt tn = new TreeNodeExt
                            {
                                Text = item,
                                Tag = item,
                                Type = "pluginEvent",
                                JsModuleName = !selectNode.JsModuleName.Contains("Plugin") ? selectNode.JsModuleName + ".Plugin" : selectNode.JsModuleName, //扩展方法，定义到Plugin模块
                                JsFunctionName = selectNode.JsFunctionName,
                                JsFunctionNameValue = selectNode.JsFunctionNameValue,
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
                var assemblyList = MyERPBusinessAssemblyInfos.Aggregate(Enumerable.Empty<MyERPBusinessAssemblyTypeInfo>(), (total, next) =>
                {
                    return total.Union(next.Types);
                });
                var type = assemblyList.FirstOrDefault(p => p.TypeFullName == selectNode.JsModuleName);
                if (type != null)
                {
                    TreeNodeExt node = new TreeNodeExt { Text = "后台方法", Type = "backType" };
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
        #endregion
    }
}
