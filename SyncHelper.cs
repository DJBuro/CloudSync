using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AndroAdmin.Model;

namespace CloudSync
{
    public class SyncHelper
    {
        public static void Sync()
        {
            // Get all the hosts
            IList<AndroAdminDataAccess.Domain.Host> hosts = AndroAdminDataAccessFactory.GetHostDAO().GetAll();

            foreach (AndroAdminDataAccess.Domain.Host host in hosts)
            {
                // 1) Ask the server for it's current data version.
                int fromVersion = 4; // Test fudge
                int toVersion = 18; // Test fudge

                // 2) Generate a block of XML that contains Add, Delete, Update objects that have changed on Cloud Master after the Cloud Servers version for:
                string syncXml = SyncHelper.GenerateSyncXml(fromVersion, toVersion);

                // 3) Send sync XML to Cloud Server.  Cloud server returns a list of logs and audit data which have occurred since the last update.

                // 4) Insert logs/audit data into Cloud Master.
            }
        }

        public static string GenerateSyncXml(int fromVersion, int toVersion)
        {
            SyncModel syncModel = new SyncModel();

            // The current data version
            syncModel.DataVersion = toVersion;

            // Stores
            List<AndroAdminDataAccess.Domain.Store> stores = (List<AndroAdminDataAccess.Domain.Store>)AndroAdminDataAccessFactory.GetStoreDAO().GetAfterDataVersion(fromVersion);
            syncModel.Stores = stores;

            // Partners
//            IList<AndroAdminDataAccess.Domain.Partner> partners = AndroAdminDataAccessFactory.GetPartnerDAO().GetAfterDataVersion(serverVersion);

            // Applications
 //           IList<AndroAdminDataAccess.Domain.ACSApplication> applications = AndroAdminDataAccessFactory.GetACSApplicationDAO().GetAfterDataVersion(serverVersion);

            return SerializeHelper.Serialize<SyncModel>(syncModel);
        }
    }
}