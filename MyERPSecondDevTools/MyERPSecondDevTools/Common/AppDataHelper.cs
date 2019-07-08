using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyERPSecondDevTools.Common
{
    public class AppDataHelper
    {
        /// <summary>
        /// 写入ERP目录选择历史
        /// </summary>
        /// <param name="path">路径</param>
        public static void WriteERPPathHistory(string path)
        {
            var historyPaths = ReadERPPathHistory();
            if (!historyPaths.Contains(path))
            {
                historyPaths.Add(path);
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/appData.json";
                if (!File.Exists(filePath))
                {
                    var emptyData = new
                    {
                        path = new List<string>()
                    };
                    string emptyString = JsonConvert.SerializeObject(emptyData, Formatting.Indented);
                    //File.Create(filePath);
                    File.WriteAllText(filePath, emptyString);
                }

                string json = File.ReadAllText(filePath);
                var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                jsonObj["path"] = historyPaths;
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
        }

        /// <summary>
        /// 读取ERP目录历史记录
        /// </summary>
        public static List<string> ReadERPPathHistory()
        {
            try
            {
                List<string> historyPaths = new List<string>();
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/appData.json";
                if (File.Exists(filePath))
                {
                    StreamReader file = File.OpenText(filePath);
                    JsonTextReader reader = new JsonTextReader(file);
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    var paths = jsonObject["path"];
                    foreach (var item in paths)
                    {
                        historyPaths.Add(item.ToString());
                    }
                    file.Close();
                }

                return historyPaths;
            }
            catch
            {
                return null;
            }
        }
    }
}