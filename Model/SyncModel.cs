using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using AndroAdminDataAccess.Domain;

namespace AndroAdmin.Model
{
    [XmlRoot(ElementName = "CloudSync")]
    public class SyncModel
    {
        public int DataVersion { get; set; }
        public List<Store> Stores { get; set; }
//        public IList<Partner> Partners { get; set; }
    }
}