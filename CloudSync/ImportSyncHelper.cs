using System;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccessEntityFramework.DataAccess;
using CloudSyncModel;
using System.Xml.Linq;

namespace CloudSync
{
    public static class AcsSyncHelper 
    {
        public static string ImportSyncXml(string syncXml, Action<string> successMessages = null, Action<string> failureMessages = null)
        {
            SyncModel syncModel = new SyncModel();

            string errorMessage = SerializeHelper.Deserialize<SyncModel>(syncXml, out syncModel);

            if (errorMessage.Length == 0)
            {
                // Import the sync XML
                ISyncDataAccess syncDataAccess = new SyncDataAccess();
                if (SyncHelper.ConnectionStringOverride != null)
                    syncDataAccess.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

                return syncDataAccess.Sync(syncModel, successMessages, failureMessages);
            }

            return errorMessage;
        }

        public static string GetAcsServerDataVersion(AndroAdminDataAccess.Domain.Host host, out int acsServerDataVersion)
        {
            acsServerDataVersion = 0;

            // Build the web service url for the ACS server
            string url = host.PrivateHostName + "/sync?key=791BB89009C544129F84B409738ACA4E";

            string responseXml = "";

            // Call the web service on the ACS server
            if (!HttpHelper.RestGet(url, out responseXml))
            {
                return "Error connecting to " + url;
            }

            // Extract the data version from the xml returned by the ACS server
            XElement xElement = XElement.Parse(responseXml);

            // Is there a data version on the xml?
            string dataVersionString = xElement.Element("Version").Value;
            if (dataVersionString == null || dataVersionString.Length == 0)
            {
                return "Data version missing from ACS Server web service xml: " + url + " " + responseXml;
            }

            // Is the data version a number?
            if (!int.TryParse(dataVersionString, out acsServerDataVersion))
            {
                return "Invalid version data returned from ACS Server web service xml: " + url + " " + responseXml;
            }

            return "";
        }

        public static string SyncAcsServer(AndroAdminDataAccess.Domain.Host host, string syncXml)
        {
            // Build the web service url for the ACS server
            string url = host.PrivateHostName + "/sync?key=791BB89009C544129F84B409738ACA4E";

            //url = "http://localhost/AndroCloudPrivateWCFServices/privateapi/sync?key=791BB89009C544129F84B409738ACA4E";

            string responseXml = "";

            // Call the web service on the ACS server
            if (!HttpHelper.RestPut(url, syncXml, null, out responseXml))
            {
                return "Error connecting to " + url;
            }

            return "";
        }
    }
}