using System.Collections.Generic;

namespace CloudSyncModel.Menus
{
    public class StoreMenuUpdates 
    {
        public StoreMenuUpdates() 
        {
            this.MenuChanges = new List<StoreMenuUpdate>();
            this.MenuThumbnailChanges = new List<StoreMenuUpdate>();
        }

        /// <summary>
        /// Gets or sets the menu changes.
        /// </summary>
        /// <value>The menu changes.</value>
        public List<StoreMenuUpdate> MenuChanges { get; set; }

        /// <summary>
        /// Gets or sets the menu thumbnail changes.
        /// </summary>
        /// <value>The menu thumbnail changes.</value>
        public List<StoreMenuUpdate> MenuThumbnailChanges { get; set; }
    }
}