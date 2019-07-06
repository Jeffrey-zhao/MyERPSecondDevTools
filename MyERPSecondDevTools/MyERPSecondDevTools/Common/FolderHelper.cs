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
    }
}