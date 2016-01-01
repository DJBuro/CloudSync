using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel.Loyalty
{
    public class LoyaltyUpdates 
    {
        /// <summary>
        /// Gets or sets the add or update list.
        /// </summary>
        /// <value>The add or update.</value>
        public List<StoreLoyaltySyncModel> AddOrUpdate { get; set; }

        /// <summary>
        /// Gets or sets the try to remove.
        /// </summary>
        /// <value>The try to remove.</value>
        public List<Guid> TryToRemove { get; set; }
    }
}
