using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSyncModel
{
    public class DeliveryArea
    {
        public System.Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string DeliveryArea1 { get; set; }
        public bool Removed { set; get; }
        public virtual Store Store { get; set; }
    }
}
