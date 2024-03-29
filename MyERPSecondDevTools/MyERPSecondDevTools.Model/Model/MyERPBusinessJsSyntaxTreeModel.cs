﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// 明源ERP业务JS语法Model
    /// </summary>
    public class MyERPBusinessJsSyntaxTreeModel
    {
        public string JsPath { get; set; }

        public string JsModuleName { get; set; }

        public string JsSyntaxTreeJson { get; set; }
    }
}