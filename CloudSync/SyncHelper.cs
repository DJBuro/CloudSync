using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AndroAdminDataAccess.DataAccess;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccessEntityFramework.DataAccess;
using CloudSyncModel;

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
            // Get all the hosts
            IList<AndroAdminDataAccess.Domain.Host> hosts = AndroAdminDataAccessFactory.GetHostDAO().GetAll();

            string errorMessage = "";

            foreach (AndroAdminDataAccess.Domain.Host host in hosts)
            {
                // 1) Ask the server for it's current data version.
                int fromVersion = 4; // Test fudge
                int toVersion = 18; // Test fudge

                // 2) Generate a block of XML that contains Add, Delete, Update objects that have changed on Cloud Master after the Cloud Servers version for:
                string syncXml = "";

                errorMessage = SyncHelper.ExportSyncXml(fromVersion, toVersion, out syncXml);

                // 3) Send sync XML to Cloud Server.  Cloud server returns a list of logs and audit data which have occurred since the last update.

                // 4) Insert logs/audit data into Cloud Master.
            }

            return errorMessage;
        }

        public static string ExportSyncXml(int fromVersion, int masterVersion, out string syncXml)
        {
            SyncModel syncModel = new SyncModel();

            // The current data version
            syncModel.FromDataVersion = fromVersion;
            syncModel.ToDataVersion = masterVersion;

            // Get the store DAO
            IStoreDAO storeDAO = AndroAdminDataAccessFactory.GetStoreDAO();
            if (SyncHelper.ConnectionStringOverride != null) storeDAO.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

            // Get all the stores that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.Store> stores = (List<AndroAdminDataAccess.Domain.Store>)storeDAO.GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.Store store in stores)
            {
                // Add the store
                Store syncStore = new Store()
                {
                    AndromedaSiteId = store.AndromedaSiteId,
                    ExternalSiteId = store.ExternalSiteId,
                    ExternalSiteName = store.ExternalSiteName,
                    StoreStatus = store.StoreStatus.Status,
                    Phone = store.Telephone,
                    TimeZone = store.TimeZone,
                    Address = new Address()
                    {
                        Id = store.Address.Id,
                        Org1 = store.Address.Org1,
                        Org2 = store.Address.Org2,
                        Org3 = store.Address.Org3,
                        Prem1 = store.Address.Prem1,
                        Prem2 = store.Address.Prem2,
                        Prem3 = store.Address.Prem3,
                        Prem4 = store.Address.Prem4,
                        Prem5 = store.Address.Prem5,
                        Prem6 = store.Address.Prem6,
                        RoadNum = store.Address.RoadNum,
                        RoadName = store.Address.RoadName,
                        Locality = store.Address.Locality,
                        Town = store.Address.Town,
                        County = store.Address.County,
                        State = store.Address.State,
                        PostCode = store.Address.PostCode,
                        DPS = store.Address.DPS,
                        Lat = store.Address.Lat,
                        Long = store.Address.Long,
                        CountryId = store.Address.Country.Id,
                    }
                };
                syncModel.Stores.Add(syncStore);
            }

            // Get the partner DAO
            IPartnerDAO partnerDAO = AndroAdminDataAccessFactory.GetPartnerDAO();
            if (SyncHelper.ConnectionStringOverride != null) partnerDAO.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

            // Get all the partners that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.Partner> partners = (List<AndroAdminDataAccess.Domain.Partner>)partnerDAO.GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.Partner partner in partners)
            {
                // Add the partner
                Partner syncPartner = new Partner()
                {
                    Id = partner.Id,
                    ExternalId = partner.ExternalId,
                    Name = partner.Name
                };
                syncModel.Partners.Add(syncPartner);

                // Get the partner DAO
                IACSApplicationDAO acsApplicationDAO = AndroAdminDataAccessFactory.GetACSApplicationDAO();
                if (SyncHelper.ConnectionStringOverride != null) acsApplicationDAO.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

                // Get all the applications that have changed for this partner since the last sync with this specific cloud server
                IList<AndroAdminDataAccess.Domain.ACSApplication> acsApplications = acsApplicationDAO.GetByPartnerAfterDataVersion(partner.Id, fromVersion);
                foreach (AndroAdminDataAccess.Domain.ACSApplication acsApplication in acsApplications)
                {
                    // Add the application
                    Application syncApplication = new Application()
                    {
                        Id = acsApplication.Id,
                        ExternalApplicationId = acsApplication.ExternalApplicationId,
                        Name = acsApplication.Name
                    };
                    syncPartner.Applications.Add(syncApplication);

                    // Get all the application stores that have changed for this application since the last sync with this specific cloud server
                    StringBuilder siteIds = new StringBuilder();
                    IList<AndroAdminDataAccess.Domain.Store> acsApplicationStores = storeDAO.GetByACSApplicationId(acsApplication.Id);
                    foreach (AndroAdminDataAccess.Domain.Store store in acsApplicationStores)
                    {
                        if (siteIds.Length > 0) siteIds.Append(",");
 
                        siteIds.Append(store.AndromedaSiteId.ToString());
                    }

                    syncApplication.Sites = siteIds.ToString();
                }
            }

            // Serialize the sync model to XML
            syncXml = SerializeHelper.Serialize<SyncModel>(syncModel);

            return "";
        }

        public static string ImportSyncXml(string syncXml)
        {
            SyncModel syncModel = new SyncModel();

            string errorMessage = SerializeHelper.Deserialize<SyncModel>(syncXml, out syncModel);

            if (errorMessage.Length == 0)
            {
                // Import the sync XML
                ISyncDataAccess syncDataAccess = new SyncDataAccess();
                if (SyncHelper.ConnectionStringOverride != null) syncDataAccess.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

                return syncDataAccess.Sync(syncModel);
            }

            return errorMessage;
        }
    }
}