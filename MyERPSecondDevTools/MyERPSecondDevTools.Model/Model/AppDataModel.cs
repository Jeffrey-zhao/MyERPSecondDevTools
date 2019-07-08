using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyERPSecondDevTools.Model.Model
{
    public class AppDataModel
    {
        /// <summary>
        /// 历史路径
        /// </summary>
        public List<string> path { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid applicationId { get; set; }
    }
}