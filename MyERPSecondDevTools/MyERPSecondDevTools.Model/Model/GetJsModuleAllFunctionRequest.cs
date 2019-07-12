using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class GetJsModuleAllFunctionRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string applicationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<GetJsModuleAllFunctionRequestItem> body { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GetJsModuleAllFunctionRequestItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string moduleName { get; set; }
    }
}
