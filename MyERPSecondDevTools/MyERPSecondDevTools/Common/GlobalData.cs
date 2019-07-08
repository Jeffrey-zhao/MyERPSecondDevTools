using System;
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
    }
}