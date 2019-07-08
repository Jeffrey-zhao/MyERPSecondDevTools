﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;

namespace MyERPSecondDevTools.Common
{
    /// <summary>
    /// 文件夹帮助类
    /// </summary>
    public class FolderHelper
    {
        /// <summary>
        /// 查找文件夹下是否存在指定的文件夹列表
        /// </summary>
        /// <param name="formPath">源文件夹</param>
        /// <param name="searchDirectorys">要查找的文件夹集合</param>
        /// <returns></returns>
        public static bool GetFoldersIsExists(string formPath, params string[] searchirectory)
        {
            foreach (var path in searchirectory)
            {
                DirectoryInfo dir = new DirectoryInfo(formPath + @"\" + path);
                if (!dir.Exists)//文件夹不存在
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 选择目录初始化基本配置
        /// </summary>
        /// <param name="erpPath"></param>
        public static void InitERPData(string erpPath)
        {
            GlobalData.ERPPath = erpPath;

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
                    GlobalData.ERPSQLConnectionString = xmlNode.Attributes["value"].Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(GlobalData.ERPSQLConnectionString))
            {
                MessageBox.Show("ERP站点未配置完成，请确保站点能够正常访问");
            }

            InitOALogin();
            InitPubInterface();
        }

        /// <summary>
        /// 初始化OA登录集成
        /// </summary>
        private static void InitOALogin()
        {
            SqlHelper sqlHelper = new SqlHelper(GlobalData.ERPSQLConnectionString);

            var isEnable = Convert.ToBoolean(sqlHelper.ExecuteScalar("SELECT IsEnable FROM p_OAIntegrateConfig WHERE SchemeCode = 'Code'"));
            if (!isEnable)
            {
                sqlHelper.ExecuteNonQuery("UPDATE p_OAIntegrateConfig SET IsEnable = 1 WHERE SchemeCode = 'Code'");
            }
        }

        /// <summary>
        /// 初始化ERP接口AppKey
        /// </summary>
        private static void InitPubInterface()
        {
            SqlHelper sqlHelper = new SqlHelper(GlobalData.ERPSQLConnectionString);
            var pubData = sqlHelper.ExecuteReader("SELECT TOP 1 AppId, AppKey FROM myOpenApiIdentity WHERE IsSystem = 1");
            if(pubData != null && pubData.Count > 0)
            {
                GlobalData.ERPPubAppId = pubData[0]["AppId"];
                GlobalData.ERPPubAppKey = pubData[0]["AppKey"];
            }
        }
    }
}