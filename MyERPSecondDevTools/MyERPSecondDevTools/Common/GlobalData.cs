using MyERPSecondDevTools.Model.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MyERPSecondDevTools.Common
{
    /// <summary>
    /// 公共数据
    /// </summary>
    public static class GlobalData
    {
        /// <summary>
        /// 当前选择的ERP站点目录
        /// </summary>
        public static string ERPPath { get; set; }

        /// <summary>
        /// ERP系统数据库连接字符串
        /// </summary>
        public static string ERPSQLConnectionString { get; set; }

        /// <summary>
        /// 用户编码参数名称
        /// </summary>
        public static string ERPUserCodeParamName { get; set; }

        /// <summary>
        /// 跳转页面参数名称
        /// </summary>
        public static string ERPJumpPageParamName { get; set; }

        /// <summary>
        /// ERP系统登录用户编码
        /// </summary>
        public static string ERPUserCode { get { return ConfigurationManager.AppSettings["erpUserCode"]; } }

        /// <summary>
        /// 工具库连接字符串
        /// </summary>
        public static string ToolsDataBaseConnectionString { get { return ConfigurationManager.AppSettings["ToolsDataBase"]; } }

        /// <summary>
        /// 全局应用ID
        /// </summary>
        public static Guid ApplicationId { get { return AppDataHelper.GetApplicationId(); } }

        /// <summary>
        /// JS解析站点的地址
        /// </summary>
        public static string ToolsJsSyntaxAnalysisWebSite { get { return ConfigurationManager.AppSettings["ToolsJsSyntaxAnalysisWebSite"]; } }

        /// <summary>
        /// ERP AppId
        /// </summary>
        public static string ERPPubAppId { get; set; }

        /// <summary>
        /// ERP AppKey
        /// </summary>
        public static string ERPPubAppKey { get; set; }

        /// <summary>
        /// ERP站点主机名
        /// </summary>
        public static string ERPHost { get; set; }

        /// <summary>
        /// 明源ERP程序集信息
        /// </summary>
        public static ConcurrentBag<MyERPBusinessAssemblyInfo> MyERPBusinessAssemblyInfos { get; set; }
    }
}