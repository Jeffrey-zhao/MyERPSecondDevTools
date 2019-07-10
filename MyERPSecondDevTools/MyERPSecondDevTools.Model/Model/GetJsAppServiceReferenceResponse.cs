using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    public class GetJsAppServiceReferenceResponse
    {
        public List<GetJsAppServiceReferenceResponseItem> appServiceFunctions { get; set; }

        public string moduleName { get; set; }

        public string functionName { get; set; }

        public string pluginName { get; set; }
    }

    public class GetJsAppServiceReferenceResponseItem
    {
        public string appServiceName { get; set; }

        public string callName { get; set; }

        public string functionName { get; set; }
    }
}
