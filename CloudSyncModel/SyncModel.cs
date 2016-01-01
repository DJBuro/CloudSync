using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CloudSyncModel
{
    [XmlRoot(ElementName = "CloudSync")]
    public class SyncModel
    {
        public int FromDataVersion { get; set; }
        public int ToDataVersion { get; set; }
        public List<Store> Stores { get; set; }
        public List<Partner> Partners { get; set; }
        public List<StorePaymentProvider> StorePaymentProviders { get; set; }

        public SyncModel()
        {
            this.FromDataVersion = 0;
            this.ToDataVersion = 0;
            this.Stores = new List<Store>();
            this.Partners = new List<Partner>();
        }
    }
}