using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace 系统维护
{
    class HttpSend
    {
        static HttpWebRequest requset = null;
        static HttpWebResponse response = null;
        static CookieContainer cc = new CookieContainer();
        static Stream stream;
        static string _cookiesstr;
        public static string Cookiesstr
        {
            get { return _cookiesstr; }
            set { _cookiesstr = string.Empty; }
        }

        public static string PostSend(string postdata, string url)
        {

            requset = (HttpWebRequest)WebRequest.Create(url);//实例化Web访问类
            requset.Method = "POST";//数据请求方式为POST
            //模拟头
            requset.ContentType = "application/x-www-form-urlencoded";
            byte[] postdatabytes = Encoding.UTF8.GetBytes(postdata);
            requset.ContentLength = postdatabytes.Length;
            requset.AllowAutoRedirect = false;
            requset.CookieContainer = cc;
            requset.KeepAlive = true;
            //提交请求
            stream = requset.GetRequestStream();
            stream.Write(postdatabytes, 0, postdatabytes.Length);
            stream.Close();
            //接受响应
            response = (HttpWebResponse)requset.GetResponse();
            //保存返回cookies
            response.Cookies = requset.CookieContainer.GetCookies(requset.RequestUri);
            CookieCollection cook = response.Cookies;
            string strook = requset.CookieContainer.GetCookieHeader(requset.RequestUri);
            _cookiesstr = strook;
            //取第一次GET跳转地址
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = sr.ReadToEnd();
            response.Close();
            sr.Close();
            return content;
            //string[] substr = content.Split(new char[] { '"' });
        }
        public static string GetSend(string url)
        {
            requset = (HttpWebRequest)WebRequest.Create(url);
            requset.Method = "GET";
            requset.KeepAlive = true;

            requset.Headers.Add("Cookie:" + Cookiesstr);
            requset.CookieContainer = cc;
            requset.AllowAutoRedirect = false;
            response = (HttpWebResponse)requset.GetResponse();
            //设置Cookie
            _cookiesstr = requset.CookieContainer.GetCookieHeader(requset.RequestUri);
            //再次取跳转连接
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            string content = sr.ReadToEnd();
            sr.Close();
            response.Close();
            return content;
        }



    }
}
