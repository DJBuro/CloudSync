using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel.Menus
{
    public class StoreMenuUpdate
    {
        public int Id { get; set; }
        public int AndromediaSiteId { get; set; }
        public string MenuType { get; set; }
        public string Data { get; set; }

        public DateTime LastUpdated { get; set; }
        public int? Version { get; set; }
    }
}
