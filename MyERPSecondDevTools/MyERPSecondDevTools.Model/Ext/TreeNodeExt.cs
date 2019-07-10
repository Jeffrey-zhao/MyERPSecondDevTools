﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyERPSecondDevTools.Model.Ext
{
    public class TreeNodeExt : TreeNode
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 对应JS组件名称
        /// </summary>
        public string JsModuleName { get; set; }

        /// <summary>
        /// 对应的JS方法名称
        /// </summary>
        public string JsFunctionName { get; set; }
    }
}
