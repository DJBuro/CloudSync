using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AndroAdminDataAccess.DataAccess;

namespace CloudSync
{
    public class AndroAdminDataAccessFactory
    {
        

        public static IAMSServerDAO GetAMSServerDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.AMSServerDAO();
        }

        public static IFTPSiteDAO GetFTPSiteDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.FtpSiteDAO();
        }

        public static IFTPSiteTypeDAO GetFTPSiteTypeDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.FtpSiteTypeDAO();
        }

        public static IStoreDAO GetStoreDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.StoreDAO();
        }

        public static IStoreStatusDAO GetStoreStatusDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.StoreStatusDAO();
        }

        public static IStoreAMSServerDAO GetStoreAMSServerDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.StoreAMSServerDAO();
        }

        public static IStoreAMSServerFTPSiteDAO GetStoreAMSServerFTPSiteDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.StoreAMSServerFtpSiteDAO();
        }
        
        public static ILogDAO GetLogDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.LogDAO();
        }

        public static IPartnerDAO GetPartnerDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.PartnerDAO();
        }

        public static IACSApplicationDAO GetACSApplicationDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.ACSApplicationDAO();
        }

        public static IHostDAO GetHostDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.HostDAO();
        }

        public static ISettingsDAO GetSettingsDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.SettingsDAO();
        }

        public static IStorePaymentProviderDAO GetStorePaymentProviderDAO()
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.StorePaymentProviderDAO();
        }

        public static IHubDataService GetHubDAO() 
        {
            return new AndroAdminDataAccess.EntityFramework.DataAccess.HubDataService();
        }


        public static IStoreHubDataService GetSiteHubDAO()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}