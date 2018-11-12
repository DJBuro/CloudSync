using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class StorePaymentProvider
    {
        public int Id { get; set; }
        public string DisplayText { get; set; }
        public string ProviderName { get; set; }
        public string ClientId { get; set; }
        public string ClientPassword { get; set; }
    }
}
