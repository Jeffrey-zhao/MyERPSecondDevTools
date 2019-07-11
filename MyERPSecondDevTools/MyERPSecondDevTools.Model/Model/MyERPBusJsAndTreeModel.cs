using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// 业务JS源码与语法树聚合信息
    /// </summary>
    public class MyERPBusJsAndTreeModel
    {
        /// <summary>
        /// JS路径
        /// </summary>
        public string JsPath { get; set; }

        /// <summary>
        /// JS本地路径
        /// </summary>
        public string JsLocalPath { get; set; }

        /// <summary>
        /// JS源码
        /// </summary>
        public string JsSourceCode { get; set; }

        /// <summary>
        /// JS模块名称
        /// </summary>
        public string JsModuleName { get; set; }

        /// <summary>
        /// JS语法树JSON数据
        /// </summary>
        public string JsSyntaxTree { get; set; }
    }
}
