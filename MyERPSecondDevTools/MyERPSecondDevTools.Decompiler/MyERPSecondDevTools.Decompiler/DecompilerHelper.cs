using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TextView;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyERPSecondDevTools.Decompiler
{
    public class DecompilerHelper
    {
        /// <summary>
        /// 获取反编译的类信息
        /// </summary>
        public static AvalonEditTextOutput GetDecompilerTypeInfo(string dllPath, string modulePath, string typeFullName)
        {
            BaseAssemblyResolver.MonoCecilResolvePath = dllPath;
            var language = new CSharpLanguage();
            AvalonEditTextOutput textOutput = new AvalonEditTextOutput();
            var str = File.ReadAllText(@"../../AppData/options.json");
            DecompilationOptions decompilationOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<DecompilationOptions>(str);
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
                    language.DecompileType(t, textOutput, decompilationOptions);
                    break;
                }
            }

            return textOutput;
        }
    }
}