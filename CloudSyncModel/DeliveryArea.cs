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

    public class PostCodeSector
    {
        public int Id { set; get; }
        public int DeliveryZoneId { set; get; }
        public string PostCodeSectorName { set; get; }
        public bool IsSelected { set; get; }
        public string StoreId { set; get; }        
    }
}
