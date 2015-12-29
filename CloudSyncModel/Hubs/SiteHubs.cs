using System;

namespace CloudSyncModel.Hubs
{
    public class SiteHubs
    {
        /// <summary>
        /// Gets or sets the site id.
        /// </summary>
        /// <value>The site id.</value>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets or sets the hub id.
        /// </summary>
        /// <value>The hub id.</value>
        public Guid HubId { get; set; }
    }
}