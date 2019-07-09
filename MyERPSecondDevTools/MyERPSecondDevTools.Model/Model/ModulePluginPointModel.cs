using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// 模块扩展点实体
    /// </summary>
    public class ModulePluginPointModel
    {
        /// <summary>
        /// 模块标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 扩展点
        /// </summary>
        public List<PluginPointModel> PluginPointModels { get; set; }
    }
}
