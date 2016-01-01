using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CloudSync.Model
{
    [XmlRoot(ElementName = "CloudSync")]
    public class SyncModel
    {
        public int DataVersion { get; set; }
        public List<Store> Stores { get; set; }
        public List<Partner> Partners { get; set; }

        public SyncModel()
        {
            this.DataVersion = 0;
            this.Stores = new List<Store>();
            this.Partners = new List<Partner>();
        }
    }
}