using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    public class GetJsPluginFunctionResponse
    {
        public List<string> pluginFunctions { get; set; }

        public string moduleName { get; set; }

        public string functionName { get; set; }
    }
}
