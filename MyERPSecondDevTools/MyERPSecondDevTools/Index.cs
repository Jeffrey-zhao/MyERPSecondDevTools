﻿using MyERPSecondDevTools.Common;
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
        /// <summary>
        /// 浏览器控件
        /// </summary>
        private WebBrowser webBrowser = new WebBrowser();

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
                    GlobalData.InitERPData(dialog.SelectedPath);
                    toolErpPathLabel.Text = dialog.SelectedPath;
                    //写入选择历史
                    WriteERPPathHistory(dialog.SelectedPath);
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
                var historyPaths = ReadERPPathHistory();
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
            toolErpPathLabel.Text = sender.ToString();
            GlobalData.InitERPData(sender.ToString());
        }

        /// <summary>
        /// 写入ERP目录选择历史
        /// </summary>
        /// <param name="path">路径</param>
        private void WriteERPPathHistory(string path)
        {
            var historyPaths = ReadERPPathHistory();
            if (!historyPaths.Contains(path))
            {
                historyPaths.Add(path);
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/erpPathHistory.json";
                if (!File.Exists(filePath))
                {
                    var emptyData = new
                    {
                        path = new List<string>()
                    };
                    string emptyString = JsonConvert.SerializeObject(emptyData, Formatting.Indented);
                    //File.Create(filePath);
                    File.WriteAllText(filePath, emptyString);
                }

                string json = File.ReadAllText(filePath);
                var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                jsonObj["path"] = historyPaths;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filePath, output);

                //重载目录历史
                InitHistoryERPPath(path);
            }
        }

        /// <summary>
        /// 读取ERP目录历史记录
        /// </summary>
        private List<string> ReadERPPathHistory()
        {
            try
            {
                List<string> historyPaths = new List<string>();
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/erpPathHistory.json";
                if (File.Exists(filePath))
                {
                    StreamReader file = File.OpenText(filePath);
                    JsonTextReader reader = new JsonTextReader(file);
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    var paths = jsonObject["path"];
                    foreach (var item in paths)
                    {
                        historyPaths.Add(item.ToString());
                    }
                    file.Close();
                }

                return historyPaths;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 加载页面地址框PlaceHolder
        /// </summary>
        private void InitTxtPageUrlPlaceHolder()
        {
            txt_pageUrl.Text = "请输入二开的页面地址";

            txt_pageUrl.GotFocus += txtPageGotFocus;
            txt_pageUrl.LostFocus += txtPageLostFocus;
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

        #region 获取异步请求结束后的页面全部响应内容
        /// <summary>
        /// Fiddler点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_fiddler_Click(object sender, EventArgs e)
        {
            var url = FiddlerHelper.GetERPNavigationPageUrl(txt_pageUrl.Text);
            webBrowser.Navigate(url);
            webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
        }

        /// <summary>
        /// 元素加载完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
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
            richTextBox1.Text = webBrowser.Document.All[1].OuterHtml;
            //只执行一次
            timer_GetResponse.Stop();
            HtmlAgilityPack(richTextBox1.Text);
        }

        /// <summary>
        /// 解析ERP二开页面HTML
        /// </summary>
        /// <param name="resHtml"></param>
        private void HtmlAgilityPack(string resHtml)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resHtml);
            var scriptList = doc.DocumentNode.SelectNodes("/html/head/script");
            foreach (var item in scriptList)
            {

            }
        }
        #endregion
    }
}