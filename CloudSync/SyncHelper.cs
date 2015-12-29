using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
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
            // Get the current data version
            string toVersionString = "";
            AndroAdminDataAccessFactory.GetSettingsDAO().GetByName("DataVersion", out toVersionString);
            int toVersion = 0;
            if (!int.TryParse(toVersionString, out toVersion))
            {
                return "Internal error";
            }

            // Get all the hosts
            IList<AndroAdminDataAccess.Domain.Host> hosts = AndroAdminDataAccessFactory.GetHostDAO().GetAll();

            string errorMessage = "";

            foreach (AndroAdminDataAccess.Domain.Host host in hosts)
            {
                // 1) Ask the server for it's current data version
                int fromVersion = 0;
                errorMessage = SyncHelper.GetACSServerDataVersion(host, out fromVersion);

                if (errorMessage.Length == 0)
                {
                    // 2) Generate a block of XML that contains Add, Delete, Update objects that have changed on Cloud Master after the Cloud Servers version for:
                    string syncXml = "";
                    errorMessage = SyncHelper.ExportSyncXml(fromVersion, toVersion, out syncXml);
                    if (errorMessage.Length != 0) return errorMessage;

                    // 3) Send sync XML to Cloud Server.  Cloud server returns a list of logs and audit data which have occurred since the last update.
                    errorMessage = SyncHelper.SyncACSServer(host, syncXml);
                    if (errorMessage.Length != 0) return errorMessage;

                    // 4) Insert logs/audit data into Cloud Master.
                    // Run out of time - future task
                }
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
                    StorePaymentProviderId = store.PaymentProvider == null ? "" : store.PaymentProvider.Id.ToString(),
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
                        CountryId = store.Address.Country.Id
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
                        Name = acsApplication.Name,
                        ExternalDisplayName = acsApplication.ExternalApplicationName
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

            // Get the store payment provider DAO
            IStorePaymentProviderDAO storePaymentProviderDAO = AndroAdminDataAccessFactory.GetStorePaymentProviderDAO();
            if (SyncHelper.ConnectionStringOverride != null) storePaymentProviderDAO.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

            syncModel.StorePaymentProviders = new List<StorePaymentProvider>();

            // Get all the payment providers that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.StorePaymentProvider> storePaymentProviders = (List<AndroAdminDataAccess.Domain.StorePaymentProvider>)storePaymentProviderDAO.GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.StorePaymentProvider storePaymentProvider in storePaymentProviders)
            {
                StorePaymentProvider diffStorePaymentProvider = new StorePaymentProvider()
                {
                    Id = storePaymentProvider.Id,
                    ClientId = storePaymentProvider.ClientId,
                    ClientPassword = storePaymentProvider.ClientPassword,
                    DisplayText = storePaymentProvider.DisplayText,
                    ProviderName = storePaymentProvider.ProviderName
                };

                syncModel.StorePaymentProviders.Add(diffStorePaymentProvider);
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

        public static string GetACSServerDataVersion(AndroAdminDataAccess.Domain.Host host, out int acsServerDataVersion)
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

            // Is there a data vesion on the xml?
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

        public static string SyncACSServer(AndroAdminDataAccess.Domain.Host host, string syncXml)
        {
            // Build the web service url for the ACS server
            string url = host.PrivateHostName + "/sync?key=791BB89009C544129F84B409738ACA4E";

            string responseXml = "";

            // Call the web service on the ACS server
            if (!HttpHelper.RestPut(url, syncXml, out responseXml))
            {
                return "Error connecting to " + url;
            }

            return "";
        }
    }
}