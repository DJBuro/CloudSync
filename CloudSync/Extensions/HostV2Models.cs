using CloudSyncModel.HostV2;
using CloudSyncModel.StoreDeviceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSync.Extensions
{
    public static class HostV2ModelsExtensions
    {
        public static HostV2Model ToSyncModel(this AndroAdminDataAccess.EntityFramework.HostV2 model)
        {
            return new HostV2Model()
            {
                Id = model.Id,
                HostTypeName = model.HostType.Name,
                LastUpdateUtc = model.LastUpdateUtc,
                OptInOnly = model.OptInOnly,
                Order = model.Order,
                Public = model.Public,
                Url = model.Url,
                Version = model.Version
            };
        }

        public static HostLinkStore ToSyncModel(this AndroAdminDataAccess.DataAccess.HostStoreConnection model)
        {
            return new HostLinkStore()
            {
                AndromedaStoreId = model.AndromedaSiteId,
                HostId = model.HostId
            };
        }

        public static HostLinkApplication ToSyncModel(this AndroAdminDataAccess.DataAccess.HostApplicationConnection model)
        {
            return new HostLinkApplication()
            {
                ApplicationId = model.ApplicationId,
                HostId = model.HostId
            };
        }
    }

    public static class StoreDevicesExtensions 
    {
        public static ExternalApiScaffold ToSyncModel(this AndroAdminDataAccess.EntityFramework.ExternalApi model) 
        {
            return new ExternalApiScaffold() 
            {
                Id = model.Id,
                Name = model.Name,
                Parameters = model.Parameters
            };
        }

        public static DeviceScaffold ToSyncModel(this AndroAdminDataAccess.EntityFramework.Device model) 
        {
            return new DeviceScaffold()
            {
                Id = model.Id,
                Name = model.Name,
                ExternalApiId = model.ExternalApiId
            };
        }

        public static SiteDeviceScaffold ToSyncModel(this AndroAdminDataAccess.EntityFramework.StoreDevice model) 
        {
            return new SiteDeviceScaffold()
            {
                AndromedaSiteId = model.Store.AndromedaSiteId,
                DeviceId = model.DeviceId,
                Parameters = model.Parameters
            };
        }
    }
}
