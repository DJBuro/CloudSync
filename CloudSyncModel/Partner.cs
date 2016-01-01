using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class Partner
    {
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public List<Application> Applications { get; set; }

        public Partner()
        {
            this.Applications = new List<Application>();
        }
    }
}
