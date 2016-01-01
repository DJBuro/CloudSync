using System.Collections.Generic;

namespace CloudSyncModel.Menus
{
    public class StoreMenuUpdates 
    {
        public StoreMenuUpdates() 
        {
            this.MenuThumbnailChanges = new List<StoreMenuUpdate>();
        } 

        public List<StoreMenuUpdate> MenuThumbnailChanges { get; set; }
    }
}