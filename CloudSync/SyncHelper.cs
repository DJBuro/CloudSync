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

namespace CloudSync
{
    public class SyncHelper
    {
        public static string ConnectionStringOverride { get; set; }

        /// <summary>
        /// Must be called from the Cloud Master.  Syncs Cloud master with all cloud servers
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

            // Get all the hosts
            IList<AndroAdminDataAccess.Domain.Host> hosts = AndroAdminDataAccessFactory
                .GetHostDAO()
                .GetAll();

            string errorMessage = string.Empty;

            foreach (AndroAdminDataAccess.Domain.Host host in hosts)
            {
                // 1) Ask the server for it's current data version
                int fromVersion = 0;
                errorMessage = AcsSyncHelper.GetAcsServerDataVersion(host, out fromVersion);

                //todo make the response enumerable
                //yield errorMessage

                if (errorMessage.Length == 0)
                {
                    // 2) Generate a block of XML that contains Add, Delete, Update objects that have changed on Cloud Master after the Cloud Servers version for:
                    string syncXml = string.Empty;

                    errorMessage = AndroAdminSyncHelper.TryGetExportSyncXml(fromVersion, toVersion, out syncXml);
                    if (errorMessage.Length != 0) return errorMessage;

                    // 3) Send sync XML to Cloud Server.  Cloud server returns a list of logs and audit data which have occurred since the last update.
                    errorMessage = AcsSyncHelper.SyncAcsServer(host, syncXml);
                    if (errorMessage.Length != 0) return errorMessage;

                    // 4) Insert logs/audit data into Cloud Master.
                    // Run out of time - future task
                }

            }

            return errorMessage;
        }

        
        
    }
}