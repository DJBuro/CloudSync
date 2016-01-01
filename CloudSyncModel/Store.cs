using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class Store
    {
        public virtual string ExternalSiteName { get; set; }

        public virtual int AndromedaSiteId { get; set; }

        public virtual string ExternalSiteId { get; set; }

        public virtual string StoreStatus { get; set; }
    }
}
