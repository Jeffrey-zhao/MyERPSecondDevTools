using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    public class GetJsPluginFunctionRequest
    {
        public string applicationId { get; set; }

        public List<GetJsPluginFunctionRequestBody> body { get; set; }
    }

    public class GetJsPluginFunctionRequestBody
    {
        public string moduleName { get; set; }

        public string functionName { get; set; }

        public string pluginName { get; set; }
    }
}
