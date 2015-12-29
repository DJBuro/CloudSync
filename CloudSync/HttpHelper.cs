using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CloudSync
{
    public class HttpHelper
    {
        public static bool RestGet(string url, out string xml)
        {
            bool success = false;

//            Andromeda.Logging.Log.LogEvent("AndroAdminClient", "GET url " + url.Replace("429C19EE237245358F8E1189ABDB1388", "XXX"), Andromeda.Logging.EventTypeEnum.Information);

            xml = "";

            // Accept invalid SSL certs
            ServicePointManager.ServerCertificateValidationCallback = delegate
            {
                return true;
            };

            // The REST service to call
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = "application/xml";
            webRequest.Timeout = 20000;
            webRequest.ReadWriteTimeout = 20000;
            webRequest.AllowAutoRedirect = false;

            StreamReader readStream = null;
            WebResponse webResponse = null;

            try
            {
                // Submit the request to the server
                webResponse = webRequest.GetResponse();

                // Get the http response
                Stream receiveStream = webResponse.GetResponseStream();

                Encoding encode = Encoding.GetEncoding("utf-8");

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                readStream = new StreamReader(receiveStream, encode);

                xml = readStream.ReadToEnd();

                success = true;

  //              Andromeda.Logging.Log.LogEvent("AndroAdminClient", "GET succeeded: " + xml, Andromeda.Logging.EventTypeEnum.Information);
            }
            catch (Exception exception)
            {
//                Andromeda.Logging.Log.LogEvent("AndroAdminClient", "GET failed", Andromeda.Logging.EventTypeEnum.Error, exception);

                success = false;
            }
            finally
            {
                // Is the http response stream open?
                if (readStream != null)
                {
                    // Yes.  Try and close it, ignoring any errors
                    try
                    {
                        readStream.Close();
                    }
                    catch { }
                }

                // Is the web response open?
                if (webResponse != null)
                {
                    // Yes.  Try and close it, ignoring any errors
                    try
                    {
                        webResponse.Close();
                    }
                    catch { }
                }
            }

            return success;
        }

        public static bool RestPut(string url, string xml, out string responseXml)
        {
            bool success = false;
            responseXml = "";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(xml);

            // Accept invalid SSL certs
            ServicePointManager.ServerCertificateValidationCallback = delegate
            {
                return true;
            };

            // The REST service to call
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Method = "PUT";
            webRequest.PreAuthenticate = true;
            webRequest.ContentType = "application/xml";
            webRequest.Accept = "application/xml";
            webRequest.ContentLength = data.Length;
            webRequest.Timeout = 20000;
            webRequest.ReadWriteTimeout = 20000;
            webRequest.AllowAutoRedirect = false;

            WebResponse webResponse = null;
            StreamReader readStream = null;

            try
            {
                // Open a connection to the server
                using (Stream contentStream = webRequest.GetRequestStream())
                {
                    contentStream.Write(data, 0, data.Length);
                    contentStream.Close();
                }

                // Submit the request to the server
                webResponse = webRequest.GetResponse();

                // Get the http response
                Stream receiveStream = webResponse.GetResponseStream();

                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                readStream = new StreamReader(receiveStream, encode);

                responseXml = readStream.ReadToEnd();

                success = true;
            }
            catch (Exception exception)
            {
 //               Log.LogEvent(Global.LogName, "LivePepper HTTP REST PUT failed", EventTypeEnum.Error, exception);
                success = false;
            }
            finally
            {
                // Is the http response stream open?
                if (readStream != null)
                {
                    // Yes.  Try and close it, ignoring any errors
                    try
                    {
                        readStream.Close();
                    }
                    catch { }
                }

                // Is the web response open?
                if (webResponse != null)
                {
                    // Yes.  Try and close it, ignoring any errors
                    try
                    {
                        webResponse.Close();
                    }
                    catch { }
                }
            }

            return success;
        }
    }
}
