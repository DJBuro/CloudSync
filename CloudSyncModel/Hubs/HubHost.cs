using System;
using System.Collections.Generic;

namespace CloudSyncModel.Hubs
{
    /// <summary>
    /// Raw Endpoint for a 
    /// </summary>
    public class HubHostModel
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the site hubs.
        /// </summary>
        /// <value>The site hubs.</value>
        public List<SiteHubs> SiteHubs { get; set; }
    }
}