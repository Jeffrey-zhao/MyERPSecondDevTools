using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyERPSecondDevTools.Common
{
    public class FiddlerHelper
    {
        /// <summary>
        /// 获取ERP二开单点登录跳转地址
        /// </summary>
        /// <param name="pageUrl"></param>
        public static string GetERPNavigationPageUrl(string pageUrl)
        {
            Uri uri = new Uri(pageUrl);
            var host = uri.Authority;
            if (!pageUrl.ToLower().Contains("http://"))
                pageUrl = "http://" + pageUrl;
            if (!host.ToLower().Contains("http://"))
                host = "http://" + host;
            //OA集成登录地址
            var formatRequestUrl = $"{host}/PubPlatform/Nav/Login/SSO/Login.aspx?UserCode={GlobalData.ERPUserCode}&PageUrl={System.Web.HttpUtility.UrlEncode(pageUrl)}";
            return formatRequestUrl;
        }
    }
}
