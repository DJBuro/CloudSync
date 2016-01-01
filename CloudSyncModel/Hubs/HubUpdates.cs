using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSyncModel.Hubs
{
    /// <summary>
    /// Lists of different hub updates to make
    /// </summary>
    public class HubUpdates
    {
        /// <summary>
        /// Gets or sets the active hub list - i.e. hubs to add/amend.
        /// </summary>
        /// <value>The active hub list.</value>
        public List<HubHostModel> ActiveHubList { get; set; }

        /// <summary>
        /// Gets or sets the in active hub list - i.e. hubs to remove.
        /// </summary>
        /// <value>The in active hub list.</value>
        public List<HubHostModel> InActiveHubList { get; set; }

        /// <summary>
        /// Gets or sets the site hub resets - i.e. sets the Site.HardwareKey to null.
        /// </summary>
        /// <value>The site hub resets.</value>
        public List<SiteHubReset> SiteHubHardwareKeyResets { get; set; }
    }
}
