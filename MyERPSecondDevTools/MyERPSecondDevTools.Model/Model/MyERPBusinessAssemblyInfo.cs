using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Model.Model
{
    /// <summary>
    /// ERP程序集信息
    /// </summary>
    public class MyERPBusinessAssemblyInfo
    {
        public string AssemblyPath { get; set; }

        public string AssemblyName { get; set; }

        public List<MyERPBusinessAssemblyTypeInfo> Types { get; set; } = new List<MyERPBusinessAssemblyTypeInfo>();
    }

    public class MyERPBusinessAssemblyTypeInfo
    {
        public string TypeName { get; set; }

        public string TypeFullName { get; set; }

        public List<MyERPBusinessAssemblyFieldInfo> Fields { get; set; } = new List<MyERPBusinessAssemblyFieldInfo>();

        public List<MyERPBusinessAssemblyMethodInfo> Methods { get; set; } = new List<MyERPBusinessAssemblyMethodInfo>();
    }

    public class MyERPBusinessAssemblyFieldInfo
    {
        public string FieldName { get; set; }

        public string FieldTypeName { get; set; }
    }

    public class MyERPBusinessAssemblyMethodInfo
    {
        public string MethodName { get; set; }

        public List<MyERPBusinessAssemblyMethodParamInfo> Paramters { get; set; } = new List<MyERPBusinessAssemblyMethodParamInfo>();
    }

    public class MyERPBusinessAssemblyMethodParamInfo
    {
        public string ParameterName { get; set; }

        public string ParameterType { get; set; }
    }
}
