using AndroAdminDataAccess.DataAccess;
using AndroAdminDataAccess.EntityFramework.DataAccess;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccessEntityFramework.DataAccess;
using CloudSyncModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CloudSyncModel.HostV2;
using CloudSync.Extensions;
using CloudSyncModel.StoreDeviceModels;
using Newtonsoft.Json;
using System.Configuration;
using AndroAdminDataAccess.Domain;
using AndroCloudHelper;

namespace CloudSync
{
    public class SignpostServerSync
    {
        public static string ConnectionStringOverride { get; set; }

        /// <summary>
        /// Must be called from the cloud signPost master.  Syncs cloud SignPost master with all cloud signpost servers
        /// </summary>
        public static string ServerSync()
        {
            // Get the current data version
            string toVersionString = string.Empty;
            AndroAdminDataAccessFactory.GetSettingsDAO().GetByName("DataVersion", out toVersionString);

            int toVersion = 0;
            if (!int.TryParse(toVersionString, out toVersion))
            {
                return "Internal error";
            }

            // Get the signpost server names
            string signpostServerList = ConfigurationManager.AppSettings["SignPostServers"];
            string[] signpostServers = signpostServerList.Split(',');

            string errorMessage = string.Empty;

            foreach (string signpostServer in signpostServers)
            {
                // 1) Ask the server for it's current data version
                int signpostServerDataVersion = -1;
                errorMessage = SignpostServerSync.GetSignpostServerDataVersion(signpostServer, out signpostServerDataVersion);

                if (errorMessage.Length == 0)
                {
                    // 2) Generate a block of JSON that contains the sync data
                    string exportJson = string.Empty;

                    errorMessage = SignpostServerSync.GenerateExportJson(signpostServerDataVersion, out exportJson);
                    if (errorMessage.Length != 0) return errorMessage;

                    // 3) Send sync XML to signpost server
                    errorMessage = SignpostServerSync.DoSync(signpostServer, exportJson);
                    if (errorMessage != null && errorMessage.Length != 0) return errorMessage;

                    // 4) Insert logs/audit data into Cloud Master.
                    // Run out of time - future task
                }

            }

            return errorMessage;
        }

        private static string GetSignpostServerDataVersion(string signpostServer, out int signpostServerDataVersion)
        {
            signpostServerDataVersion = -1;

            // Build the web service url for the signpost server
            string url = signpostServer + "/DataVersion?secretkey=CDEF9B9357CD47B7A87CDF510674C327";

            string responseJson = "";

            // Call the web service on the signpost server
            if (!HttpHelper.RestGet(url, "application/json", out responseJson))
            {
                return "Error connecting to " + url;
            }

            // Check for errors
            string errorMessage = SignpostServerSync.CheckForError(responseJson);
            if (errorMessage != null) return errorMessage;

            // Extract the data version from the JSON returned by the signpost server
            SignPostServerVersion signPostServerVersion = JsonConvert.DeserializeObject<SignPostServerVersion>(responseJson);

            if (signPostServerVersion == null)
            {
                return "Data version missing from Signpost web service json: " + url + " " + responseJson;
            }

            signpostServerDataVersion = signPostServerVersion.Version;

            return "";
        }

        private static string GenerateExportJson(int fromDataVersion, out string exportJson)
        {
            string error = "";

            exportJson = "";
            int toDataVersion = -1;
            string settingValue = "";

            // Get the current data version
            error = AndroAdminDataAccessFactory.GetSettingsDAO().GetByName("DataVersion", out settingValue);

            if (String.IsNullOrEmpty(error))
            {
                 if (!int.TryParse(settingValue, out toDataVersion))
                 {
                     error = "Version setting is not a number: " + settingValue;
                 }
            }
            SignpostSync signpostSync = null;

            if (String.IsNullOrEmpty(error))
            {
                List<SignpostACSApplication> signpostACSApplications = new List<SignpostACSApplication>();

                // Get the acs applications that have changed since the last sync
                IList<ACSApplication> acsApplications = AndroAdminDataAccessFactory.GetACSApplicationDAO().GetDataBetweenVersions(fromDataVersion, toDataVersion);

                signpostSync = new SignpostSync()
                {
                    fromVersion = fromDataVersion,
                    toVersion = toDataVersion
                };

                foreach (ACSApplication acsApplication in acsApplications)
                {
                    // Get the sites for the acs application
                    IList<int> siteIds = AndroAdminDataAccessFactory.GetACSApplicationDAO().GetSites(acsApplication.Id);

                    // Put together the ACS application data
                    SignpostACSApplication signpostACSApplication = new SignpostACSApplication()
                    {
                        id = acsApplication.Id,
                        acsApplicationSites = siteIds,
                        environmentId = acsApplication.EnvironmentId,
                        externalApplicationId = acsApplication.ExternalApplicationId,
                        name = acsApplication.Name
                    };

                    signpostSync.acsApplicationSyncs.Add(signpostACSApplication);
                }
            }

            if (String.IsNullOrEmpty(error))
            {
                if (signpostSync != null)
                {
                    exportJson = JsonConvert.SerializeObject(signpostSync);
                }
            }

            return error;
        }

        private static string DoSync(string signpostServer, string syncJson)
        {
            // Build the web service url for the signpost server
            string url = signpostServer + "/sync?secretkey=CDEF9B9357CD47B7A87CDF510674C327";

            string responseJson = "";

            // Call the web service on the signpost server
            if (!HttpHelper.RestPost(url, syncJson, "application/json", null, out responseJson))
            {
                return "Error connecting to " + url;
            }

            // Check for errors
            return SignpostServerSync.CheckForError(responseJson);
        }

        private static string CheckForError(string responseJson)
        {
            string errorMessage = null;

            Error error = null;

            try
            {
                error = JsonConvert.DeserializeObject<Error>(responseJson);
            }
            catch
            {
                errorMessage = "Invalid JSON returned: " + responseJson;
            };

            if (errorMessage == null && error != null && (error.Result != ResultEnum.NoError || !String.IsNullOrEmpty(error.ErrorMessage)))
            {
                // The server returned an error
                errorMessage = error.ErrorMessage;
            }

            return errorMessage;
        }
    }

    public class SignPostServerVersion
    {
        public int Version { get; set; }
    }

    public class SignpostSync
    {
        public int fromVersion { get; set; }
        public int toVersion { get; set; }
        public List<SignpostACSApplication> acsApplicationSyncs { get; set; }

        public SignpostSync()
        {
            this.acsApplicationSyncs = new List<SignpostACSApplication>();
        }
    }

    public class SignpostACSApplication
    {
        public int id { get; set; }
        public string name { get; set; }
        public Guid environmentId { get; set; }
        public string externalApplicationId { get; set; }
        public IList<int> acsApplicationSites { get; set; }
    }
}