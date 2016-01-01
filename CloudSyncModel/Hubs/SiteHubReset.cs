namespace CloudSyncModel.Hubs
{
    /// <summary>
    /// A ticket requiring the reset of a store/site hub
    /// </summary>
    public class SiteHubReset 
    {
        /// <summary>
        /// Gets or sets the Andromeda site id.
        /// </summary>
        /// <value>The Andromeda site id.</value>
        public int AndromedaSiteId { get; set; }

        /// <summary>
        /// Gets or sets the external site id.
        /// </summary>
        /// <value>The external site id.</value>
        public string ExternalSiteId { get; set; }
    }
}