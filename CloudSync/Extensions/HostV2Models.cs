using CloudSyncModel.HostV2;
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
}
