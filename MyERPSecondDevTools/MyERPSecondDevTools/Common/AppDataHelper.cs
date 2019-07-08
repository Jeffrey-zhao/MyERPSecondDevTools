using MyERPSecondDevTools.Model.Model;
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
                var jsonObj = JsonConvert.DeserializeObject<AppDataModel>(json);
                jsonObj.path = historyPaths;
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

        /// <summary>
        /// 写入应用ID
        /// </summary>
        private static Guid WriteApplicationId()
        {
            var applicationId = ReadApplicationId();
            if (applicationId == Guid.Empty)
            {
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/appData.json";
                if (!File.Exists(filePath))
                {
                    var emptyData = new
                    {
                        path = new List<string>(),
                        applicationId = string.Empty
                    };
                    string emptyString = JsonConvert.SerializeObject(emptyData, Formatting.Indented);
                    //File.Create(filePath);
                    File.WriteAllText(filePath, emptyString);
                }

                string json = File.ReadAllText(filePath);
                var jsonObj = JsonConvert.DeserializeObject<AppDataModel>(json);
                jsonObj.applicationId = Guid.NewGuid();
                string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText(filePath, output);
                return jsonObj.applicationId;
            }
            else
            {
                return applicationId;
            }
        }

        /// <summary>
        /// 读取应用ID
        /// </summary>
        private static Guid ReadApplicationId()
        {
            try
            {
                Guid applicationId = Guid.Empty;
                DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);
                var filePath = directory.Parent.Parent.FullName + "/AppData/appData.json";
                if (File.Exists(filePath))
                {
                    StreamReader file = File.OpenText(filePath);
                    JsonTextReader reader = new JsonTextReader(file);
                    JObject jsonObject = (JObject)JToken.ReadFrom(reader);
                    var tokenApplicationId = jsonObject["applicationId"];
                    if (tokenApplicationId != null)
                        applicationId = Guid.Parse(tokenApplicationId.ToString());
                    
                    file.Close();
                }

                return applicationId;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 获取应用ID
        /// </summary>
        /// <returns></returns>
        public static Guid GetApplicationId()
        {
            var applicationId = ReadApplicationId();
            if(applicationId == Guid.Empty)
            {
                return WriteApplicationId();
            }
            else
            {
                return applicationId;
            }
        }
    }
}