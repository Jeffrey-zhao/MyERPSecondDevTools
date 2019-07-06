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
        private static string ERPPath { get; set; }

        /// <summary>
        /// ERP系统数据库连接字符串
        /// </summary>
        private static string ERPSQLConnectionString { get; set; }

        /// <summary>
        /// ERP系统登录用户编码
        /// </summary>
        public static string ERPUserCode { get { return ConfigurationManager.AppSettings["erpUserCode"]; } }

        /// <summary>
        /// 获取ERP目录
        /// </summary>
        /// <returns></returns>
        public static string GetERPPath()
        {
            return ERPPath;
        }

        #region 选择目录初始化基本配置
        /// <summary>
        /// 选择目录初始化基本配置
        /// </summary>
        /// <param name="erpPath"></param>
        public static void InitERPData(string erpPath)
        {
            ERPPath = erpPath;

            var erpConfigXml = erpPath + @"\App_Data\configcache_product_erp60.xml";
            if (!File.Exists(erpConfigXml))
            {
                MessageBox.Show("ERP站点未配置完成，请确保站点能够正常访问");
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(erpConfigXml);
            var xmlNodes = xmlDocument.GetElementsByTagName("ConfigItem");
            for (int i = 0; i < xmlNodes.Count; i++)
            {
                var xmlNode = xmlNodes[i];
                if (xmlNode.Attributes["name"].Value == "masterDb")
                {
                    ERPSQLConnectionString = xmlNode.Attributes["value"].Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(ERPSQLConnectionString))
            {
                MessageBox.Show("ERP站点未配置完成，请确保站点能够正常访问");
            }

            SqlHelper.ConnStr = ERPSQLConnectionString;

            InitOALogin();
        }

        /// <summary>
        /// 初始化OA登录集成
        /// </summary>
        private static void InitOALogin()
        {
            var isEnable = Convert.ToBoolean(SqlHelper.ExecuteScalar("SELECT IsEnable FROM p_OAIntegrateConfig WHERE SchemeCode = 'Code'"));
            if (!isEnable)
            {
                SqlHelper.ExecuteNonQuery("UPDATE p_OAIntegrateConfig SET IsEnable = 1 WHERE SchemeCode = 'Code'");
            }
        }
        #endregion
    }
}