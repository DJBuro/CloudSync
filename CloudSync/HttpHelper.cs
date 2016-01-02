using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AndroAdmin.Helpers;

namespace CloudSync
{
    public class HttpHelper
    {
        public static bool RestGet(string url, out string xml)
        {
            return HttpHelper.RestGet(url, null, "application/xml", out xml);
        }
        public static bool RestGet(string url, string accept, out string xml)
        {
            return HttpHelper.RestGet(url, null, accept, out xml);
        }
        public static bool RestGet(string url, Dictionary<string, string> headers, out string xml)
        {
            return HttpHelper.RestGet(url, null, "application/xml", out xml);
        }
        public static bool RestGet(string url, Dictionary<string, string> headers, string accept, out string xml)
        {
            bool success = false;

            ErrorHelper.LogError("DEBUG", "CloudSync.HttpHelper.RestGet: url=" + url, null);

            xml = string.Empty;

            // Accept invalid SSL certs
            ServicePointManager.ServerCertificateValidationCallback = delegate
            {
                return true;
            };

            // The REST service to call
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Accept = accept;
            webRequest.Timeout = 20000;
            webRequest.ReadWriteTimeout = 20000;
            webRequest.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }

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
            }
            catch (WebException webException)
            {
                ErrorHelper.LogError("ERROR", "CloudSync.HttpHelper.RestGet", webException);

                if (webException.Response != null)
                {
                    // Get the http response
                    Stream receiveStream = webException.Response.GetResponseStream();

                    Encoding encode = Encoding.GetEncoding("utf-8");

                    // Pipe the stream to a higher level stream reader with the required encoding format. 
                    readStream = new StreamReader(receiveStream, encode);

                    xml = readStream.ReadToEnd();
                }

                success = false;
            }
            catch (Exception exception)
            {
                ErrorHelper.LogError("ERROR", "CloudSync.HttpHelper.RestGet", exception);

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

        public static bool RestPut(string url, string xml, Dictionary<string, string> headers, out string responseXml)
        {
            return HttpHelper.RestPut(url, xml, headers, "application/xml", out responseXml);
        }

        public static bool RestPut(string url, string xml, Dictionary<string, string> headers, string contentType, out string responseXml)
        {
            bool success = false;
            responseXml = string.Empty;

            ErrorHelper.LogError("DEBUG", "CloudSync.HttpHelper.RestPut: url=" + url, null);

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
            webRequest.ContentType = contentType;
            webRequest.Accept = contentType;
            webRequest.ContentLength = data.Length;
            webRequest.Timeout = 20000;
            webRequest.ReadWriteTimeout = 20000;
            webRequest.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }

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
            catch (WebException webException)
            {
                ErrorHelper.LogError("ERROR", "RestPut", webException);

                // Is there a response with the exception?
                if (webException.Response != null)
                {
                    // Did the server return any data with the exception
                    if (webException.Response.ContentLength != 0)
                    {
                        // Get the response
                        using (Stream stream = webException.Response.GetResponseStream())
                        {
                            // Get the response data
                            using (StreamReader reader = new StreamReader(stream, encoding))
                            {
                                responseXml = reader.ReadToEnd();
                            }
                        }
                    }
                }

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

        public static bool RestPost(string url, string xml, out string responseXml)
        {
            return HttpHelper.RestPost(url, xml, null, out responseXml);
        }

        public static bool RestPost(string url, string xml, Dictionary<string, string> headers, out string responseXml)
        {
            return HttpHelper.RestPost(url, xml, "application/xml", null, out responseXml);
        }

        public static bool RestPost(string url, string xml, string dataType, Dictionary<string, string> headers, out string responseXml)
        {
            bool success = false;
            responseXml = string.Empty;

            ErrorHelper.LogError("DEBUG", "CloudSync.HttpHelper.RestPut: url=" + url, null);

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(xml);

            // Accept invalid SSL certs
            ServicePointManager.ServerCertificateValidationCallback = delegate
            {
                return true;
            };

            // The REST service to call
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Method = "POST";
            webRequest.PreAuthenticate = true;
            webRequest.ContentType = dataType;
            webRequest.Accept = dataType;
            webRequest.ContentLength = data.Length;
            webRequest.Timeout = 20000;
            webRequest.ReadWriteTimeout = 20000;
            webRequest.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    webRequest.Headers.Add(header.Key, header.Value);
                }
            }

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
            catch (WebException webException)
            {
                ErrorHelper.LogError("ERROR", "RestPost", webException);

                // Is there a response with the exception?
                if (webException.Response != null)
                {
                    // Did the server return any data with the exception
                    if (webException.Response.ContentLength != 0)
                    {
                        // Get the response
                        using (Stream stream = webException.Response.GetResponseStream())
                        {
                            // Get the response data
                            using (StreamReader reader = new StreamReader(stream, encoding))
                            {
                                responseXml = reader.ReadToEnd();
                            }
                        }
                    }
                }

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
