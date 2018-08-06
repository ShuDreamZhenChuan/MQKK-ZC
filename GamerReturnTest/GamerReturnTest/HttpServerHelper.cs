using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using System.Threading;

namespace GamerReturnTest
{
    public class HttpServerHelper
    {
        public static string _IPAddress;

        public static string _Port;

        private static CookieContainer _cookie =new CookieContainer();

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        
        //public static string HttpPostRequest(string Interfacename,IDictionary<string, string> parameters,Encoding charset)
        //{
          public static string HttpPostRequest(string Interfacename,string parameterjson,Encoding charset)
        { 
                //Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(strjson);  
                System.GC.Collect();
                string reponsejason;
                string url = "http://" + _IPAddress + ":" + _Port + "/frontWeb/card/" + Interfacename;
                //string url = "http://120.77.34.115:8080/" + Interfacename;
                //string url = "http://172.16.2.144:16666/VTM_RESTFuls/rest/" + Interfacename;
                //string url = "http://172.16.2.146:18080/VTM_RESTFuls/rest/" + Interfacename;
                //string url = "http://192.168.0.146:8080/VTM_RESTFuls/rest/" + Interfacename;
                HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse _reponse=null;

                if (_cookie.Count > 0)
                {
                    _request.CookieContainer = _cookie;
                }
                else
                    _request.CookieContainer = new CookieContainer();
                _request.Method = "POST";
                _request.Timeout = 20000;
               
                _request.KeepAlive =true;      
                
                //_request.ProtocolVersion = HttpVersion.Version10;
                _request.UserAgent = DefaultUserAgent; 
                _request.ContentType ="application/x-www-form-urlencoded";
               
            try
                {
                    
                    //if (!(parameters == null || parameters.Count == 0))
                    //{
                        StringBuilder buffer = new StringBuilder();
                        //int i = 0;
                        //foreach (string key in parameters.Keys)
                        //{
                        //    if (i > 0)
                        //    {
                        //        //buffer.AppendFormat("，\"{0}\":\"{1}\"", key, parameters[key]);
                        //        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        //    }
                        //    else
                        //    {
                        //        //buffer.AppendFormat("\"{0}\":\"{1}\"", key, parameters[key]);
                        //        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        //        i++;
                        //    }
                        //}
                        
                        //string adadad = buffer.ToString();//@"{" + buffer.ToString() + "}";
                        string adadad=parameterjson;
                        string sendBuf = adadad.Replace("+", "%2B");            
                        //string adadad = buffer.ToString();
                        byte[] data = charset.GetBytes(sendBuf);
                        string ccc = charset.GetString(data);
                        _request.ContentLength = data.Length;
                        using (Stream stream = _request.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                        //streamWriter writer = new Stre amWriter(_request.GetRequestStream());
                        //writer.Write(datajason);            
                   //}
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            Thread.Sleep(500);
            try
            {
                 _reponse = (HttpWebResponse)_request.GetResponse();
                 
                 if (_reponse.Cookies.Count > 0)
                 {
                     _cookie.Add(_reponse.Cookies);
                 }
                using (StreamReader sr = new StreamReader(_reponse.GetResponseStream()))
                {
                    reponsejason = sr.ReadToEnd();
                    //reponsejason = ServerCommunication.Decrypt_DES(reponsejason);
                    return reponsejason;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (_reponse != null)
                {
                    _reponse.Close();
                }
                if (_request != null)
                {
                    _request.Abort();
                }            
            }
                
        }


          public static string HttpPostRequestJson(string Interfacename, string parameterjson, Encoding charset)
          {
              //Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(strjson);  
              System.GC.Collect();
              string reponsejason="";
              string url = "http://localhost:8086/Service1.svc/" + Interfacename;
              //string url = "http://120.77.34.115:8080/" + Interfacename;
              //string url = "http://172.16.2.144:16666/VTM_RESTFuls/rest/" + Interfacename;
              //string url = "http://172.16.2.146:18080/VTM_RESTFuls/rest/" + Interfacename;
              //string url = "http://192.168.0.146:8080/VTM_RESTFuls/rest/" + Interfacename;
              //reponsejason += "url:" + url + " parameter:" + parameterjson;
              HttpWebRequest _request = (HttpWebRequest)WebRequest.Create(url);
              HttpWebResponse _reponse = null;
              _request.Method = "POST";
              _request.Timeout = 20000;
              _request.KeepAlive = true;
              //_request.ProtocolVersion = HttpVersion.Version10;
              _request.UserAgent = DefaultUserAgent;
              _request.ContentType = "application/json";
              try
              {

                  //if (!(parameters == null || parameters.Count == 0))
                  //{
                  StringBuilder buffer = new StringBuilder();
                  //int i = 0;
                  //foreach (string key in parameters.Keys)
                  //{
                  //    if (i > 0)
                  //    {
                  //        //buffer.AppendFormat("，\"{0}\":\"{1}\"", key, parameters[key]);
                  //        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                  //    }
                  //    else
                  //    {
                  //        //buffer.AppendFormat("\"{0}\":\"{1}\"", key, parameters[key]);
                  //        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                  //        i++;
                  //    }
                  //}


                  //string adadad = buffer.ToString();//@"{" + buffer.ToString() + "}";
                  string adadad = parameterjson;

                  string sendBuf = adadad.Replace("+", "%2B");
                  //string adadad = buffer.ToString();
                  byte[] data = charset.GetBytes(sendBuf);
                  string ccc = charset.GetString(data);
                  _request.ContentLength = data.Length;
                  using (Stream stream = _request.GetRequestStream())
                  {
                      stream.Write(data, 0, data.Length);
                  }
                  //streamWriter writer = new Stre amWriter(_request.GetRequestStream());
                  //writer.Write(datajason);            
                  //}
              }
              catch (Exception ex)
              {
                  return reponsejason+=ex.Message;
              }
              Thread.Sleep(500);
              try
              {
                  _reponse = (HttpWebResponse)_request.GetResponse();
                  using (StreamReader sr = new StreamReader(_reponse.GetResponseStream()))
                  {
                      reponsejason = sr.ReadToEnd();
                    //reponsejason = ServerCommunication.Decrypt_DES(reponsejason);
                      reponsejason=reponsejason.Replace("\\",string.Empty);  
                      
                      return reponsejason;
                  }
              }
              catch (Exception ex)
              {
                  return reponsejason += ex.Message;
              }
              finally
              {
                  if (_reponse != null)
                  {
                      _reponse.Close();
                  }
                  if (_request != null)
                  {
                      _request.Abort();
                  }
              }

          }

        public class JsonSerDeserializeClass
        {
            public string msg { get; set; }
            public string code { get; set; }
            public dynamic data { get; set; }
        }

    }
}
