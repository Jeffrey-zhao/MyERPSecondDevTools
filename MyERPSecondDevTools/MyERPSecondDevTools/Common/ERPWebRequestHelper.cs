using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MyERPSecondDevTools.Common
{
    public class ERPWebRequestHelper
    {
        #region 公共方法
        /// <summary>
        /// ERPGet数据接口
        /// </summary>
        /// <param name="getUrl">接口地址</param>
        /// /// <param name="contentType">ContentType</param>
        /// <returns></returns>
        public static string GetWebRequest(string getUrl, string contentType = null)
        {
            string responseContent = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            if(contentType != null)
                request.ContentType = contentType;
            request.Method = "GET";
            request.Headers.Add("AppId", GlobalData.ERPPubAppId);
            request.Headers.Add("AppKey", GlobalData.ERPPubAppKey);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //在这里对接收到的页面内容进行处理
            using (Stream resStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                {
                    responseContent = reader.ReadToEnd().ToString();
                }
            }
            return responseContent;
        }
        /// <summary>
        /// ERPPost数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式(Encoding.UTF8)</param>
        /// <param name="contentType">ContentType</param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode, string contentType = "application/json")
        {
            string responseContent = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                if (contentType != null)
                    webReq.ContentType = contentType;
                webReq.Headers.Add("AppId", GlobalData.ERPPubAppId);
                webReq.Headers.Add("AppKey", GlobalData.ERPPubAppKey);

                webReq.ContentLength = byteArray.Length;
                using (Stream reqStream = webReq.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);//写入参数
                                                                    //reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), dataEncode))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return responseContent;
        }

        #endregion
    }
}