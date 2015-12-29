using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class Store
    {
        public string ExternalSiteName { get; set; }
        public int AndromedaSiteId { get; set; }
        public string ExternalSiteId { get; set; }
        public string StoreStatus { get; set; }
        public string Phone { get; set; }
        public string TimeZone { get; set; }
        public Address Address { get; set; }
    }
}
