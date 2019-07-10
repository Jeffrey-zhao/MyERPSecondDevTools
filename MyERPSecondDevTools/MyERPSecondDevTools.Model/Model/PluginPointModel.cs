using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// 扩展点Model
    /// </summary>
    public class PluginPointModel
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 控件ID
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// 控件标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 元数据状态
        /// </summary>
        public string MetaDataStatus { get; set; }

        /// <summary>
        /// 事件列表
        /// </summary>
        public List<PluginPointModelEvent> Events { get; set; }
    }

    public class PluginPointModelEvent
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 事件名
        /// </summary>
        public string EventName { get; set; }
    }
}
