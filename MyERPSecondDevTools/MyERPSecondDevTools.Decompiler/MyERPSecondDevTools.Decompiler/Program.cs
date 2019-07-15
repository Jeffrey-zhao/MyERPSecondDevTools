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
    class Program
    {
        static void Main(string[] args)
        {
            BaseAssemblyResolver.MonoCecilResolvePath = @"G:\Work\金茂重构\接口并行期分支\src\00-ERP站点\bin";
            var language = new CSharpLanguage();
            AvalonEditTextOutput textOutput = new AvalonEditTextOutput();
            var str = File.ReadAllText(@"../../options.json");
            DecompilationOptions decompilationOptions = Newtonsoft.Json.JsonConvert.DeserializeObject<DecompilationOptions>(str);
            ReaderParameters parameters = new ReaderParameters
            {
                ReadingMode = ReadingMode.Deferred,
                ReadSymbols = false,
            };
            var module = ModuleDefinition.ReadModule(@"G:\Work\金茂重构\接口并行期分支\src\00-ERP站点\bin\Mysoft.Cbxt.PayMng.dll", parameters);
            var types = module.Types;
            foreach (var t in types)
            {
                if (t.FullName == "Mysoft.Cbxt.PayMng.AppServices.HtfkApplyAppService")
                {
                    language.DecompileType(t, textOutput, decompilationOptions);
                    Console.WriteLine(textOutput.b.ToString());
                }
            }
            Console.Read();
        }
    }
}
