using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using WindowsFormsApplication1.VXSEr;
using System.Threading;
using System.Xml.Linq;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        WxService VX = new WxService();
      
        public Form1()
        {
    
            
            InitializeComponent();
           

         //   string postString = "{'groupId':54,'phone':'18613072779','password':'za123456'}";





            //http://member.gzzbpizza.com/WxFood/activity/SecondKill.aspx?GroupId=54&DrawNo=20171101

            //CookieContainer cookie = new CookieContainer();
            //if (PostLogin(postString, post_signIn, ref cookie).Equals("ok"))
            //{
            //  
                
            //}
          

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Result result = VX.Login(54, "18613072779", "za123456");

            // string url="http://member.gzzbpizza.com/WxFood/activity/SecondKill.aspx?GroupId=54&DrawNo=20171101";
            ////string content = "{'NET_SessionId':'" + textBox1.Text + "','GroupId':'" + textBox2.Text + "','LGK':'" + textBox3.Text + "','Phone':" + textBox3.Text + "'}";

            //Post(url, content);
          
            string postString = "groupId=" + 54 + "&phone=" + textBox1.Text + "&password="+ textBox2.Text ;
            string post_signIn = "http://member.gzzbpizza.com/WxFood/WxService.asmx/Login";
            CookieContainer cook = GetCookie(postString, post_signIn);//登录
            while (true)
            {
                string post_getContJsonData = "http://member.gzzbpizza.com/WxFood/WxService.asmx/QuickSeckill";
            //  {drawNo:'20171101',periodNo:'01',ticketNo:'20171101'}
            string content = "";
            if (radioButton1.Checked == true)
            {
                content = "drawNo=" + 20171101 + "&periodNo=" + "01" + "&ticketNo=20171101";//上午
            }
            if (radioButton2.Checked == true)
            {
                content = "drawNo=" + 20171101 + "&periodNo=" + "02" + "&ticketNo=20171101";//下午
            }
                try
                {
                    string strCont = PostRequest(content, post_getContJsonData, cook);//操作 
                    if (strCont == "true") { MessageBox.Show("成功"); return; }
                }
                catch { }

                    Thread.Sleep(10);

            }
        }


        //public static string PostLogin(string postData, s     tring requestUrlString, ref CookieContainer cookie)
        //{
        //    ASCIIEncoding encoding = new ASCIIEncoding();
        //    byte[] data = encoding.GetBytes(postData);
        //    //向服务端请求
        //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(requestUrlString);
        //    myRequest.Method = "POST";
        //    myRequest.ContentType = "application/x-www-form-urlencoded";
        //    myRequest.ContentLength = data.Length;
        //    myRequest.CookieContainer = new CookieContainer();
        //    Stream newStream = myRequest.GetRequestStream();
        //    newStream.Write(data, 0, data.Length);
        //    newStream.Close();
        //    //将请求的结果发送给客户端(界面、应用)
        //    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
        //    cookie.Add(myResponse.Cookies);
        //    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //    return reader.ReadToEnd();
        //}


        public static string PostRequest(string postData, string requestUrlString, CookieContainer cookie)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(requestUrlString);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            myRequest.CookieContainer = cookie;
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

            
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

            string result = reader.ReadToEnd();


            SortedDictionary<string, string> requestXML = GetInfoFromXml(result);
            foreach (KeyValuePair<string, string> k in requestXML)
            {
                if (k.Key == "IsOk")
                {
                    result = k.Value;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// 把XML数据转换为SortedDictionary<string, string>集合
        /// </summary>
        /// <param name="strxml"></param>
        /// <returns></returns>
        public static SortedDictionary<string, string> GetInfoFromXml(string xmlstring)
        {
            SortedDictionary<string, string> sParams = new SortedDictionary<string, string>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlstring);
                XmlElement root = doc.DocumentElement;
                int len = root.ChildNodes.Count;
                for (int i = 0; i < len; i++)
                {
                    string name = root.ChildNodes[i].Name;
                    if (!sParams.ContainsKey(name))
                    {
                        sParams.Add(name.Trim(), root.ChildNodes[i].InnerText.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                //LxCommomHelper.Commom.TraceLog.LogWrite(ex.ToString(), LxCommomHelper.Commom.LogEnum.Pay);
            }
            return sParams;
        }

        static CookieContainer GetCookie(string postString, string postUrl)
        {
          //   string postString = "{'groupId':54,'phone':'18613072779','password':'za123456'}";
           // string postUrl = "http://member.gzzbpizza.com/WxFood/activity/SecondKill.aspx?GroupId=54&DrawNo=20171101";


            CookieContainer cookie = new CookieContainer();

            HttpWebRequest httpRequset = (HttpWebRequest)HttpWebRequest.Create(postUrl);//创建http 请求
            httpRequset.CookieContainer = cookie;//设置cookie
            httpRequset.Method = "POST";//POST 提交
            httpRequset.KeepAlive = true;
            httpRequset.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            httpRequset.Accept = "text/html, application/xhtml+xml, */*";
            httpRequset.ContentType = "application/x-www-form-urlencoded";//以上信息在监听请求的时候都有的直接复制过来
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postString);
            httpRequset.ContentLength = bytes.Length;
            Stream stream = httpRequset.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();//以上是POST数据的写入

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequset.GetResponse();//获得 服务端响应
            return cookie;//拿到cookie
        }













        //static string GetContent(CookieContainer cookie, string url, string postString)
        //{
        //    string content;
        //    HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        //    httpRequest.CookieContainer = cookie;
        //    httpRequest.Referer = url;
        //    httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
        //    httpRequest.Accept = "text/html, application/xhtml+xml, */*";
        //    httpRequest.ContentType = "application/x-www-form-urlencoded";
        //    httpRequest.Method = "GET";

        //    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();



        //    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postString);
        //    httpResponse.ContentLength = bytes.Length;
        //    Stream stream = httpResponse.GetRequestStream();
        //    stream.Write(bytes, 0, bytes.Length);
        //    stream.Close();//以上是POST数据的写入

        //    HttpWebResponse httpResponse = (HttpWebResponse)httpResponse.GetResponse();//获得 服务端响应




        //    using (Stream responsestream = httpResponse.GetResponseStream())
        //    {

        //        using (StreamReader sr = new StreamReader(responsestream, System.Text.Encoding.UTF8))
        //        {
        //            content = sr.ReadToEnd();
        //        }
        //    }

        //    return content;
        //}


        
        /// <summary>  
        /// 指定Post地址使用Get 方式获取全部字符串  
        /// </summary>  
        /// <param name="url">请求后台地址</param>  
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>  
        /// <returns></returns>  
        public static string Post(string url, string content)
        {
 
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
