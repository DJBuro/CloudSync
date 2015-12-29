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

        public static bool RestPut(string url, string xml, out string responseXml)
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
