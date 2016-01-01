using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class Application
    {
        public int Id { get; set; }
        public string ExternalApplicationId { get; set; }
        public string Name { get; set; }
        public string Sites { get; set; }
        public string ExternalDisplayName { get; set; }
    }
}
