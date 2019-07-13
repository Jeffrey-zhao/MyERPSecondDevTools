using System;
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
using Mono.Cecil;
using System.Diagnostics;
using System.Threading.Tasks;
using MyERPSecondDevTools.Model.Model;

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
            InitAssemblyInfo();
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
                sqlHelper.ExecuteNonQuery("UPDATE p_OAIntegrateConfig SET IsEnable = 1 WHERE SchemeCode = 'Code'; UPDATE p_UserCodeVerify SET UserCodeContrast = 0 ,EncryptionType = 0");
            }

            var pubData = sqlHelper.ExecuteReader("SELECT TOP 1 UserCodeParamName, JumpPageParamName FROM p_UserCodeVerify");
            if (pubData != null && pubData.Count > 0)
            {
                GlobalData.ERPUserCodeParamName = pubData[0]["UserCodeParamName"];
                GlobalData.ERPJumpPageParamName = pubData[0]["JumpPageParamName"];
            }
        }

        /// <summary>
        /// 初始化ERP接口AppKey
        /// </summary>
        private static void InitPubInterface()
        {
            SqlHelper sqlHelper = new SqlHelper(GlobalData.ERPSQLConnectionString);
            var pubData = sqlHelper.ExecuteReader("SELECT TOP 1 AppId, AppKey FROM myOpenApiIdentity WHERE IsSystem = 1");
            if (pubData != null && pubData.Count > 0)
            {
                GlobalData.ERPPubAppId = pubData[0]["AppId"];
                GlobalData.ERPPubAppKey = pubData[0]["AppKey"];
            }
        }

        /// <summary>
        /// 初始化程序集信息
        /// </summary>
        private static void InitAssemblyInfo()
        {
            GlobalData.MyERPBusinessAssemblyInfos = new System.Collections.Concurrent.ConcurrentBag<MyERPBusinessAssemblyInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(GlobalData.ERPPath + "/bin");
            var files = directoryInfo.GetFiles("*.dll").ToList().FindAll(p => p.Name.StartsWith("Mysoft") && !p.Name.StartsWith("Mysoft.Map"));
            var totalCount = files.Count;
            //获取JS源码任务四等分，除不尽五等分，并行执行
            var factorNum = totalCount / 4;
            var currentNum = 0;

            //任务主体方法
            Action<IEnumerable<FileInfo>> ActionBody = param =>
            {
                foreach (var file in param)
                {
                    using (var module = ModuleDefinition.ReadModule(file.FullName))
                    {
                        MyERPBusinessAssemblyInfo assemblyInfo = new MyERPBusinessAssemblyInfo();
                        assemblyInfo.AssemblyName = module.Name;
                        assemblyInfo.AssemblyPath = module.FileName;
                        var types = module.Types;
                        foreach (var t in types)
                        {
                            if (t.FullName.StartsWith("Mysoft"))
                            {
                                MyERPBusinessAssemblyTypeInfo typeInfo = new MyERPBusinessAssemblyTypeInfo();
                                typeInfo.TypeName = t.Name;
                                typeInfo.TypeFullName = t.FullName;

                                var ms = t.Methods;
                                var prs = t.Fields;
                                foreach (var p in prs)
                                {
                                    MyERPBusinessAssemblyFieldInfo fieldInfo = new MyERPBusinessAssemblyFieldInfo();
                                    fieldInfo.FieldName = p.Name;
                                    fieldInfo.FieldTypeName = p.FieldType.FullName;
                                    typeInfo.Fields.Add(fieldInfo);
                                }

                                foreach (var method in ms)
                                {
                                    if (method.Name != ".ctor")
                                    {
                                        MyERPBusinessAssemblyMethodInfo methodInfo = new MyERPBusinessAssemblyMethodInfo();
                                        methodInfo.MethodName = method.Name;

                                        var ps = method.Parameters;
                                        foreach (var p in ps)
                                        {
                                            MyERPBusinessAssemblyMethodParamInfo paramInfo = new MyERPBusinessAssemblyMethodParamInfo();
                                            paramInfo.ParameterName = p.Name;
                                            paramInfo.ParameterType = p.ParameterType.FullName;
                                            methodInfo.Paramters.Add(paramInfo);
                                        }
                                        typeInfo.Methods.Add(methodInfo);
                                    }
                                }
                                assemblyInfo.Types.Add(typeInfo);
                            }
                        }
                        GlobalData.MyERPBusinessAssemblyInfos.Add(assemblyInfo);
                    }
                }
            };
            
            for (int i = 0; i < 4; i++)
            {
                var currentList = files.Skip(currentNum * factorNum).Take(factorNum);
                Task.Factory.StartNew(() =>
                {
                    ActionBody(currentList);
                });
                currentNum++;
            }
            
            var remainder = totalCount % 4;
            if (remainder > 0)
            {
                var currentList = files.Skip(currentNum * factorNum).Take(remainder);
                Task.Factory.StartNew(() =>
                {
                    ActionBody(currentList);
                });
            }
        }
    }
}