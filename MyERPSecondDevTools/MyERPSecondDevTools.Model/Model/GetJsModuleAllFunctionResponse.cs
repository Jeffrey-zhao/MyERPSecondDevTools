using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    public class GetJsModuleAllFunctionResponse
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string moduleName { get; set; }

        /// <summary>
        /// 模块内方法列表
        /// </summary>
        public List<GetJsModuleAllFunctionResponseItem> functions { get; set; }
    }

    public class GetJsModuleAllFunctionResponseItem
    {
        /// <summary>
        /// 方法类型、plugin或normal
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 参数名称列表
        /// </summary>
        public List<string> argsParams { get; set; }
    }
}