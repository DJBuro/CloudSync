using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccessEntityFramework.DataAccess;
using CloudSync.Model;

namespace CloudSync
{
    public class SyncHelper
    {
        /// <summary>
        /// Must be called from the Cloud Master.  Syncs Cloud master with all cloud servers
        /// </summary>
        public static void ServerSync()
        {
            // Get all the hosts
            IList<AndroAdminDataAccess.Domain.Host> hosts = AndroAdminDataAccessFactory.GetHostDAO().GetAll();

            foreach (AndroAdminDataAccess.Domain.Host host in hosts)
            {
                // 1) Ask the server for it's current data version.
                int fromVersion = 4; // Test fudge
                int toVersion = 18; // Test fudge

                // 2) Generate a block of XML that contains Add, Delete, Update objects that have changed on Cloud Master after the Cloud Servers version for:
                string syncXml = SyncHelper.ExportSyncXml(fromVersion, toVersion);

                // 3) Send sync XML to Cloud Server.  Cloud server returns a list of logs and audit data which have occurred since the last update.

                // 4) Insert logs/audit data into Cloud Master.
            }
        }

        public static string ExportSyncXml(int fromVersion, int toVersion)
        {
            SyncModel syncModel = new SyncModel();

            // The current data version
            syncModel.DataVersion = toVersion;
            
            // Get all the stores that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.Store> stores = (List<AndroAdminDataAccess.Domain.Store>)AndroAdminDataAccessFactory.GetStoreDAO().GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.Store store in stores)
            {
                // Add the store
                Store syncStore = new Store()
                {
                    AndromedaSiteId = store.AndromedaSiteId,
                    ExternalSiteId = store.ExternalSiteId,
                    ExternalSiteName = store.ExternalSiteName,
                    StoreStatus = store.StoreStatus.Status
                };
                syncModel.Stores.Add(syncStore);
            }

            // Get all the partners that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.Partner> partners = (List<AndroAdminDataAccess.Domain.Partner>)AndroAdminDataAccessFactory.GetPartnerDAO().GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.Partner partner in partners)
            {
                // Add the partner
                Partner syncPartner = new Partner()
                {
                    ExternalId = partner.ExternalId,
                    Name = partner.Name
                };
                syncModel.Partners.Add(syncPartner);

                // Get all the applications that have changed for this partner since the last sync with this specific cloud server
                IList<AndroAdminDataAccess.Domain.ACSApplication> acsApplications = AndroAdminDataAccessFactory.GetACSApplicationDAO().GetByPartnerAfterDataVersion(partner.Id, fromVersion);
                foreach (AndroAdminDataAccess.Domain.ACSApplication acsApplication in acsApplications)
                {
                    // Add the application
                    Application syncApplication = new Application()
                    {
                        ExternalApplicationId = acsApplication.ExternalApplicationId,
                        Name = acsApplication.Name
                    };
                    syncPartner.Applications.Add(syncApplication);

                    // Get all the application stores that have changed for this application since the last sync with this specific cloud server
                    StringBuilder siteIds = new StringBuilder(); 
                    IList<AndroAdminDataAccess.Domain.Store> acsApplicationStores = AndroAdminDataAccessFactory.GetStoreDAO().GetByACSApplicationId(acsApplication.Id);
                    foreach (AndroAdminDataAccess.Domain.Store store in acsApplicationStores)
                    {
                        if (siteIds.Length > 0) siteIds.Append(",");
 
                        siteIds.Append(store.AndromedaSiteId.ToString());
                    }

                    syncApplication.Sites = siteIds.ToString();
                }
            }

            return SerializeHelper.Serialize<SyncModel>(syncModel);
        }

        public static void ImportSyncXml(string syncXml)
        {
            SyncModel syncModel = new SyncModel();

            string errorMessage = SerializeHelper.Deserialize<SyncModel>(syncXml, out syncModel);

            // Process each store that's changed
            foreach (Store store in syncModel.Stores)
            {
                // Does the store already exist?
                ISyncDataAccess syncDataAccess = new SyncDataAccess();
                

      //          store.AndromedaSiteId
            }

            // Process each partner that's changed
            foreach (Partner partner in syncModel.Partners)
            {
            }
        }
    }
}