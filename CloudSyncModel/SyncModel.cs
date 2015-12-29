using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using CloudSyncModel.Hubs;
using CloudSyncModel.Menus;
using CloudSyncModel.HostV2;
using CloudSyncModel.StoreDeviceModels;

namespace CloudSyncModel
{
    [XmlRoot(ElementName = "CloudSync")]
    public class SyncModel
    {
        public SyncModel()
        {
            this.FromDataVersion = 0;
            this.ToDataVersion = 0;
            this.Stores = new List<Store>();
            this.StoreEdt = new List<StoreEdt>();
            this.Partners = new List<Partner>();

            this.HubUpdates = new HubUpdates();
            this.MenuUpdates = new StoreMenuUpdates();

            this.HostV2Models = new HostV2Models();
            this.StoreDeviceModels = new StoreDevicesModels();

            this.LoyaltyUpdates = new Loyalty.LoyaltyUpdates();
        }

        public int FromDataVersion { get; set; }
        public int ToDataVersion { get; set; }

        public List<Store> Stores { get; set; }
        public List<StoreEdt> StoreEdt { get; set; }

        public List<Partner> Partners { get; set; }
        public List<StorePaymentProvider> StorePaymentProviders { get; set; }

        /// <summary>
        /// Gets or sets the loyalty updates.
        /// </summary>
        /// <value>The loyalty updates.</value>
        public Loyalty.LoyaltyUpdates LoyaltyUpdates { get; set; }

        /// <summary>
        /// Gets or sets the hub updates.
        /// </summary>
        /// <value>The hub updates.</value>
        public HubUpdates HubUpdates { get; set; }

        /// <summary>
        /// Gets or sets the menu updates.
        /// </summary>
        /// <value>The menu updates.</value>
        public StoreMenuUpdates MenuUpdates { get; set; }

        /// <summary>
        /// Gets or sets the host v2 models.
        /// </summary>
        /// <value>The host v2 models.</value>
        public HostV2Models HostV2Models { get; set; }

        /// <summary>
        /// Gets or sets the store device models.
        /// </summary>
        /// <value>The store device models.</value>
        public StoreDevicesModels StoreDeviceModels { get; set; }

        /// <summary>
        /// Gets or sets the delivery areas.
        /// </summary>
        /// <value>The delivery areas.</value>
        public List<DeliveryArea> DeliveryAreas { get; set; }

        /// <summary>
        /// Gets or sets the post code sectors.
        /// </summary>
        /// <value>The post code sectors.</value>
        public List<PostCodeSector> PostCodeSectors { set; get; }

    }
}