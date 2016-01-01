using CloudSyncModel.Loyalty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSync.Extensions
{
    public static class StoreLoyaltyExtensions
    {
        public static StoreLoyaltySyncModel ToSyncModel(this AndroAdminDataAccess.EntityFramework.StoreLoyalty loyalty)
        {
            return new StoreLoyaltySyncModel()
            {
                Id = loyalty.Id,
                ProviderName = loyalty.ProviderName,
                AndromedaSiteId = loyalty.Store.AndromedaSiteId,
                Configuration = loyalty.Configuration
            };
        }
    }
}
