using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TextView;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Decompiler
{
    /// <summary>
    /// 反编译帮助类
    /// </summary>
    public class DecompilerHelper
    {
        /// <summary>
        /// C#语言类型
        /// </summary>
        private static CSharpLanguage CSharpLanguage
        {
            get
            {
                return new CSharpLanguage();
            }
        }

        /// <summary>
        /// 反编译选项类
        /// </summary>
        private static DecompilationOptions DecompilationOptions
        {
            get
            {
                var str = File.ReadAllText(@"../../AppData/options.json");
                return Newtonsoft.Json.JsonConvert.DeserializeObject<DecompilationOptions>(str);
            }
        }
        /// <summary>
        /// 获取反编译的类信息
        /// </summary>
        public static AvalonEditTextOutput GetDecompilerTypeInfo(string dllPath, string modulePath, string typeFullName)
        {
            BaseAssemblyResolver.MonoCecilResolvePath = dllPath;
            AvalonEditTextOutput textOutput = new AvalonEditTextOutput();
            ReaderParameters parameters = new ReaderParameters
            {
                ReadingMode = ReadingMode.Deferred,
                ReadSymbols = false,
            };
            var module = ModuleDefinition.ReadModule(modulePath, parameters);
            var types = module.Types;
            foreach (var t in types)
            {
                if (t.FullName == typeFullName)
                {
                    CSharpLanguage.DecompileType(t, textOutput, DecompilationOptions);
                    break;
                }
            }
            module = null;
            return textOutput;
        }

        /// <summary>
        /// 通过正则表达式获取字段类型名称
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string GetFieldTypeName(string typeName)
        {
            var str = typeName;
            if (str.Contains("<") && str.Contains(">"))
            {
                Regex rex = new Regex("(?<MYSTR><.*>)");
                str = rex.Match(typeName).Groups["MYSTR"].ToString();
                str = str.TrimStart('<');
                var lastChar = str.Substring(str.Length - 1);
                if (lastChar == ">")
                    str = str.Substring(0, str.Length - 1);
                while (str.Contains("<") && str.Contains(">"))
                {
                    str = rex.Match(str).Groups["MYSTR"].ToString();
                    str = str.TrimStart('<');
                    lastChar = str.Substring(str.Length - 1);
                    if (lastChar == ">")
                        str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }
    }
}