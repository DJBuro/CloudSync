﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using CloudSyncModel.Hubs;
using CloudSyncModel.Menus;

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
            this.Partners = new List<Partner>();

            this.HubUpdates = new HubUpdates();
            this.MenuUpdates = new StoreMenuUpdates();
        }

        public int FromDataVersion { get; set; }
        public int ToDataVersion { get; set; }

        public List<Store> Stores { get; set; }
        public List<Partner> Partners { get; set; }
        public List<StorePaymentProvider> StorePaymentProviders { get; set; }
        
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
    }
}