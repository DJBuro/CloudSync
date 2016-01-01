using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel.HostV2
{
    public class HostV2Models
    {
        public List<string> HostTypes { get; set; }
        public List<HostV2Model> Hosts { get; set; }

        public List<HostLinkStore> Stores { get; set; }
        public List<HostLinkApplication> Applications { get; set; }
    }

    public class HostV2Model
    {
        public Guid Id { get; set; }
        public string HostTypeName { get; set; }

        public DateTime LastUpdateUtc { get; set; }

        public bool OptInOnly { get; set; }

        public int Order { get; set; }

        public bool Public { get; set; }

        public string Url { get; set; }

        public int Version { get; set; }
    }

    public class HostLinkStore
    {
        public Guid HostId { get; set; }
        public int AndromedaStoreId { get; set; }
    }

    public class HostLinkApplication
    {
        public Guid HostId { get; set; }
        public int ApplicationId { get; set; }
    }
}
