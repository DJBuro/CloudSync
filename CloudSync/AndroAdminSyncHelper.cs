using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroAdminDataAccess.DataAccess;
using AndroAdminDataAccess.EntityFramework.DataAccess;
using CloudSync.Extensions;
using CloudSyncModel;
using CloudSyncModel.StoreDeviceModels;
using Newtonsoft.Json;

namespace CloudSync
{
    public static class AndroAdminSyncHelper 
    {
        public static string TryGetExportSyncXml(int fromVersion, int masterVersion, out string syncXml)
        {
            SyncModel syncModel = new SyncModel();

            // The current data version
            syncModel.FromDataVersion = fromVersion;
            syncModel.ToDataVersion = masterVersion;

            // Get the store DAO
            IStoreDAO storeDao = AndroAdminDataAccessFactory.GetStoreDAO();
            // Get the partner DAO
            IPartnerDAO partnerDao = AndroAdminDataAccessFactory.GetPartnerDAO();
            // Get the store payment provider DAO
            IStorePaymentProviderDAO storePaymentProviderDao = AndroAdminDataAccessFactory.GetStorePaymentProviderDAO();

            //get the list of hosts, host types and connections based on version. 
            //var a = AndroAdminDataAccessFactory.gethost

            if (SyncHelper.ConnectionStringOverride != null)
            {
                storeDao.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
                partnerDao.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
                storePaymentProviderDao.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
            }

            AndroAdminSyncHelper.AddInStoreUpdates(storeDao, syncModel, fromVersion);

            AndroAdminSyncHelper.AddInPartnerUpdates(partnerDao, storeDao, syncModel, fromVersion);

            AndroAdminSyncHelper.AddInStorePaymentProviders(storePaymentProviderDao, syncModel, fromVersion);

            AndroAdminSyncHelper.AddInHubTasks(syncModel, fromVersion);

            //Menu updates as pushed on by MyAndromeda.
            AndroAdminSyncHelper.AddinMenuUpdates(syncModel, fromVersion);

            //Host V2 changes 
            AndroAdminSyncHelper.AddInHostV2List(syncModel, fromVersion);

            AndroAdminSyncHelper.AddInStoreDevices(syncModel, fromVersion);
            // Serialize the sync model to XML

            ////add delivery areas to sync model
            //AndroAdminSyncHelper.AddDeliveryAreas(syncModel, fromVersion);

            //add postcodeSectors to sync model
            AndroAdminSyncHelper.AddPostCodeSectors(syncModel, fromVersion);

            AndroAdminSyncHelper.AddLoyalty(syncModel, fromVersion);

            syncXml = SerializeHelper.Serialize<SyncModel>(syncModel);

            return string.Empty;
        }

        private static void AddLoyalty(SyncModel syncModel, int fromVersion)
        {
            LoyaltyDataService loyaltyDataService = new LoyaltyDataService();

            var storeLoyaltyRecords = loyaltyDataService.List(e => e.DataVersion > fromVersion);

            if (!storeLoyaltyRecords.Any())
            {
                return;
            }

            syncModel.LoyaltyUpdates = new CloudSyncModel.Loyalty.LoyaltyUpdates();

            var fillMe = syncModel.LoyaltyUpdates.AddOrUpdate ?? (syncModel.LoyaltyUpdates.AddOrUpdate = new List<CloudSyncModel.Loyalty.StoreLoyaltySyncModel>());
            var deleteMe = syncModel.LoyaltyUpdates.TryToRemove ?? (syncModel.LoyaltyUpdates.TryToRemove = new List<Guid>());

            foreach (var storeLoyalty in storeLoyaltyRecords)
            {
                dynamic config = JsonConvert.DeserializeObject(storeLoyalty.Configuration ?? "{}");

                if (config.Enabled != null && config.Enabled.Value)
                {
                    fillMe.Add(storeLoyalty.ToSyncModel());
                }
                else
                {
                    deleteMe.Add(storeLoyalty.Id);
                }
            }
        }

        private static void AddPostCodeSectors(SyncModel syncModel, int fromVersion)
        {
            DeliveryAreaDataService deliveryAreaDataService = new DeliveryAreaDataService();
            var postCodeSectors = deliveryAreaDataService.GetListPostCodes(e => e.DataVersion > fromVersion).ToList();

            var model = syncModel.PostCodeSectors ?? (syncModel.PostCodeSectors = new List<PostCodeSector>());

            foreach (var postcode in postCodeSectors)
            {
                CloudSyncModel.PostCodeSector postCodeSector = new CloudSyncModel.PostCodeSector
                {
                    DeliveryZoneId = postcode.DeliveryZoneId,
                    Id = postcode.Id,
                    IsSelected = postcode.IsSelected,
                    PostCodeSectorName = postcode.PostCodeSector1,
                    StoreId = postcode.DeliveryZoneName.Store.ExternalId
                };
                syncModel.PostCodeSectors.Add(postCodeSector);
            }
        }

        private static void AddInStoreDevices(SyncModel syncModel, int fromVersion)
        {
            IStoreDevicesDataService storeDevicesDataService = new StoreDevicesDataService();
            IDevicesDataService devicesDataService = new DevicesDataService();
            IExternalApiDataService externalApiDataService = new ExternalApiDataService();

            var updatedExternalApis = externalApiDataService.List(e => e.DataVersion > fromVersion);
            var updatedDevices = devicesDataService.List(e => e.DataVersion > fromVersion);
            var allUpdatedStoreDevices = storeDevicesDataService.Query(e => e.DataVersion > fromVersion);

            var model = syncModel.StoreDeviceModels ?? (syncModel.StoreDeviceModels = new StoreDevicesModels());

            //add or update
            model.ExternalApis = updatedExternalApis.Select(e => e.ToSyncModel()).ToList();
            model.Devices = updatedDevices.Select(e => e.ToSyncModel()).ToList();
            model.SiteDevices = new List<SiteDeviceScaffold>();
            //model.SiteDevices = storeDevices.Select(e => e.ToSyncModel()).ToList();

            //models to remove
            model.RemovedExternalApis = externalApiDataService
                                                              .ListRemoved(e => e.DataVersion > fromVersion)
                                                              .Select(e => e.ToSyncModel())
                                                              .ToList();
            model.RemovedDevices = devicesDataService
                                                     .ListRemoved(e => e.DataVersion > fromVersion)
                                                     .Select(e => e.ToSyncModel())
                                                     .ToList();

            model.RemovedSiteDevices = new List<SiteDeviceScaffold>();
            //model.RemovedSiteDevices = storeDevicesDataService
            //    .ListRemoved(e => e.DataVersion > fromVersion)
            //    .Select(e => e.ToSyncModel())
            //    .ToList();


            //a oddity arises if there is ever more than one device, and that store device is changed. 
            foreach (var deviceGroup in allUpdatedStoreDevices.GroupBy(e => e.StoreId))
            {
                //ignore the above, just interested in the stores as we are going to send all of them up. 
                var storeDevices = storeDevicesDataService.Query(e => e.StoreId == deviceGroup.Key).ToList();

                //remove all of them on ACS as its impossible to tell which item has switched on device changes. 
                var remove = storeDevices.Select(e => e.ToSyncModel()).ToArray();
                
                //and ... then add all those which are still not removed
                var addOrUpdate = storeDevices.Where(e => !e.Removed).Select(e => e.ToSyncModel()).ToArray();

                //add to the export feed. 
                model.RemovedSiteDevices.AddRange(remove);
                model.SiteDevices.AddRange(addOrUpdate);
            }
            //all done. 
        }

        private static void AddInHostV2List(
            SyncModel syncModel,
            int fromVersion)
        {
            IHostTypesDataService hostTypesDataService = AndroAdminDataAccessFactory.GetHostTypesDataService();
            IHostV2DataService hostV2DataService = AndroAdminDataAccessFactory.GetHostV2DataService();
            IHostV2ForStoreDataService hostV2ForStore = AndroAdminDataAccessFactory.GetHostV2ForStoreDataService();
            IHostV2ForApplicationDataService hostV2ForApplication = AndroAdminDataAccessFactory.GetHostV2ForApplicationDataService();

            var hosts = hostV2DataService.List(e => e.DataVersion > fromVersion);

            var activeHosts = hosts.Where(e => e.Enabled);
            var disabledHosts = hosts.Where(e => !e.Enabled);

            var deletedHosts = hostV2DataService.ListDeleted(e => e.DataVersion > fromVersion);

            var hosttypes = hostTypesDataService.List(e => true);
            var hostApplications = hostV2ForApplication.ListHostConnections(e => e.HostV2.Any(hostV2 => hostV2.DataVersion > fromVersion));
            var hostStores = hostV2ForStore.ListHostConnections(e => e.HostV2.Any(hostV2 => hostV2.DataVersion > fromVersion));

            syncModel.HostV2Models = new CloudSyncModel.HostV2.HostV2Models()
            {
                //full list of host types that are available
                HostTypes = hosttypes.Select(e => e.Name).ToList(),
                //actual list of active, available hosts
                Hosts = activeHosts.Select(hostV2 => hostV2.ToSyncModel()).ToList(),
                //any hosts that must be removed (disabled for maintenance or removed completely, even the ones that may even be back later)
                DeletedHosts = disabledHosts.Union(deletedHosts).Select(hostV2 => hostV2.ToSyncModel()).ToList(),
                //store specific links to the hosts
                StoreLinks = hostStores.Select(e => e.ToSyncModel()).ToList(),
                //application specific links to the hosts
                ApplicationLinks = hostApplications.Select(e => e.ToSyncModel()).ToList()
            };
        }

        private static void AddInHubTasks(SyncModel syncModel, int fromVersion)
        {
            //get the hub and site-hub data services 
            IHubDataService hubDataDao = AndroAdminDataAccessFactory.GetHubDAO();
            IStoreHubDataService storeHubDataDao = AndroAdminDataAccessFactory.GetSiteHubDAO();
            IHubResetDataService storeHubResetDao = AndroAdminDataAccessFactory.GetHubResetDAO();

            var signalRHubs = hubDataDao.GetAfterDataVersion(fromVersion);

            var activeHubs = signalRHubs.Where(e => !e.Removed && e.Active).ToList();
            var inactiveHubs = signalRHubs.Where(e => e.Removed || !e.Active).ToList();
            var resets = storeHubResetDao.GetStoresToResetAfterDataVersion(fromVersion);

            syncModel.HubUpdates = new CloudSyncModel.Hubs.HubUpdates()
            {
                ActiveHubList = activeHubs.Select(e => new CloudSyncModel.Hubs.HubHostModel()
                {
                    Id = e.Id,
                    Url = e.Address,
                    SiteHubs = storeHubDataDao.GetSitesUsingHub(e.Id).Select(s => new CloudSyncModel.Hubs.SiteHubs() { HubId = e.Id, ExternalId = s.StoreExternalId }).ToList()
                }).ToList(),
                InActiveHubList = inactiveHubs.Select(e => new CloudSyncModel.Hubs.HubHostModel()
                {
                    Id = e.Id,
                    Url = e.Address,
                    SiteHubs = new List<CloudSyncModel.Hubs.SiteHubs>()//empty list as the tables will enforce the dropping related rows
                }).ToList(),
                SiteHubHardwareKeyResets = resets.Select(e => new CloudSyncModel.Hubs.SiteHubReset()
                {
                    AndromedaSiteId = e.AndromedaSiteId,
                    ExternalSiteId = e.ExternalSiteId
                }).ToList()
            };
        }

        private static void AddinMenuUpdates(SyncModel syncModel, int fromVersion)
        {
            IStoreMenuThumbnailsDataService dataService = AndroAdminDataAccessFactory.GetStoreMenuThumbnailDAO();

            IEnumerable<AndroAdminDataAccess.Domain.StoreMenu> menuChanges =
                dataService.GetStoreMenuChangesAfterDataVersion(fromVersion);
            IEnumerable<AndroAdminDataAccess.Domain.StoreMenuThumbnails> thumbnailChanges =
                dataService.GetStoreMenuThumbnailChangesAfterDataVersion(fromVersion);

            foreach (var change in menuChanges)
            {
                syncModel.MenuUpdates.MenuChanges.Add(new CloudSyncModel.Menus.StoreMenuUpdate()
                {
                    AndromediaSiteId = change.AndromedaSiteId,
                    Data = change.MenuData,
                    Id = change.Id,
                    LastUpdated = change.LastUpdated,
                    MenuType = change.MenuType,
                    Version = change.Version,
                });
            }

            foreach (var change in thumbnailChanges)
            {
                syncModel.MenuUpdates.MenuThumbnailChanges.Add(new CloudSyncModel.Menus.StoreMenuUpdate()
                {
                    Id = change.Id,
                    LastUpdated = change.LastUpdate,
                    AndromediaSiteId = change.AndromediaSiteId,
                    Data = change.Data,
                    MenuType = change.MenuType
                });
            }
        }

        private static void AddInStorePaymentProviders(IStorePaymentProviderDAO storePaymentProviderDao, SyncModel syncModel, int fromVersion)
        {
            syncModel.StorePaymentProviders = new List<StorePaymentProvider>();

            var storePaymentProviders = storePaymentProviderDao.GetAfterDataVersion(fromVersion);

            foreach (AndroAdminDataAccess.Domain.StorePaymentProvider storePaymentProvider in storePaymentProviders)
            {
                StorePaymentProvider diffStorePaymentProvider = new StorePaymentProvider()
                {
                    Id = storePaymentProvider.Id,
                    ClientId = storePaymentProvider.ClientId,
                    ClientPassword = storePaymentProvider.ClientPassword,
                    DisplayText = storePaymentProvider.DisplayText,
                    ProviderName = storePaymentProvider.ProviderName
                };

                syncModel.StorePaymentProviders.Add(diffStorePaymentProvider);
            }
        }

        private static void AddInPartnerUpdates(IPartnerDAO partnerDao, IStoreDAO storeDao, SyncModel syncModel, int fromVersion)
        {
            // Get all the partners that have changed since the last sync with this specific cloud server
            List<AndroAdminDataAccess.Domain.Partner> partners = (List<AndroAdminDataAccess.Domain.Partner>)partnerDao.GetAfterDataVersion(fromVersion);
            foreach (AndroAdminDataAccess.Domain.Partner partner in partners)
            {
                // Add the partner
                Partner syncPartner = new Partner()
                {
                    Id = partner.Id,
                    ExternalId = partner.ExternalId,
                    Name = partner.Name
                };

                syncModel.Partners.Add(syncPartner);

                // Get the partner DAO
                IACSApplicationDAO acsApplicationDao = AndroAdminDataAccessFactory.GetACSApplicationDAO();
                if (SyncHelper.ConnectionStringOverride != null)
                    acsApplicationDao.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;

                // Get all the applications that have changed for this partner since the last sync with this specific cloud server
                IList<AndroAdminDataAccess.Domain.ACSApplication> acsApplications = acsApplicationDao.GetByPartnerAfterDataVersion(partner.Id, fromVersion);
                foreach (AndroAdminDataAccess.Domain.ACSApplication acsApplication in acsApplications)
                {
                    // Add the application
                    Application syncApplication = new Application()
                    {
                        Id = acsApplication.Id,
                        ExternalApplicationId = acsApplication.ExternalApplicationId,
                        Name = acsApplication.Name,
                        ExternalDisplayName = acsApplication.ExternalApplicationName
                    };
                    syncPartner.Applications.Add(syncApplication);

                    // Get all the application stores that have changed for this application since the last sync with this specific cloud server
                    StringBuilder siteIds = new StringBuilder();
                    IList<AndroAdminDataAccess.Domain.Store> acsApplicationStores = storeDao.GetByACSApplicationId(acsApplication.Id);
                    foreach (AndroAdminDataAccess.Domain.Store store in acsApplicationStores)
                    {
                        if (siteIds.Length > 0)
                            siteIds.Append(",");

                        siteIds.Append(store.AndromedaSiteId.ToString());
                    }

                    syncApplication.Sites = siteIds.ToString();
                }
            }
        }

        private static void AddInStoreUpdates(IStoreDAO storeDao, SyncModel syncModel, int fromVersion)
        {
            // Get all the stores that have changed since the last sync with this specific cloud server
            var stores = storeDao.GetAfterDataVersion(fromVersion);// as List<AndroAdminDataAccess.Domain.Store>;
            var etdStores = storeDao.GetEdtAfterDataVersion(fromVersion);

            foreach (AndroAdminDataAccess.Domain.Store store in stores)
            {
                // Sync store opening times
                List<TimeSpanBlock> openingHours = new List<TimeSpanBlock>();
                if (store.OpeningHours != null)
                {
                    foreach (AndroAdminDataAccess.Domain.TimeSpanBlock timeSpanBlock in store.OpeningHours)
                    {
                        openingHours.Add(
                            new TimeSpanBlock()
                            {
                                Day = timeSpanBlock.Day,
                                EndTime = timeSpanBlock.EndTime,
                                OpenAllDay = timeSpanBlock.OpenAllDay,
                                StartTime = timeSpanBlock.StartTime
                            });
                    }
                }

                // Add the store
                Store syncStore = new Store()
                {
                    AndromedaSiteId = store.AndromedaSiteId,
                    ExternalSiteId = store.ExternalSiteId,
                    ExternalSiteName = store.ExternalSiteName,
                    StoreStatus = store.StoreStatus.Status,
                    Phone = store.Telephone,
                    TimeZone = store.TimeZone,
                    TimeZoneInfoId = store.TimeZoneInfoId,
                    UiCulture = store.UiCulture,
                    StorePaymentProviderId = store.PaymentProvider == null ? string.Empty : store.PaymentProvider.Id.ToString(),
                    Address = new Address()
                    {
                        Id = store.Address.Id,
                        Org1 = store.Address.Org1,
                        Org2 = store.Address.Org2,
                        Org3 = store.Address.Org3,
                        Prem1 = store.Address.Prem1,
                        Prem2 = store.Address.Prem2,
                        Prem3 = store.Address.Prem3,
                        Prem4 = store.Address.Prem4,
                        Prem5 = store.Address.Prem5,
                        Prem6 = store.Address.Prem6,
                        RoadNum = store.Address.RoadNum,
                        RoadName = store.Address.RoadName,
                        Locality = store.Address.Locality,
                        Town = store.Address.Town,
                        County = store.Address.County,
                        State = store.Address.State,
                        PostCode = store.Address.PostCode,
                        DPS = store.Address.DPS,
                        Lat = store.Address.Lat,
                        Long = store.Address.Long,
                        CountryId = store.Address.Country.Id
                    },
                    OpeningHours = openingHours
                };

                syncModel.Stores.Add(syncStore);
            }

            foreach (var store in etdStores)
            {
                if (syncModel.StoreEdt == null)
                {
                    syncModel.StoreEdt = new List<StoreEdt>();
                }

                syncModel.StoreEdt.Add(new StoreEdt()
                {
                    AndromedaSiteId = store.AndromedaSiteId,
                    EstimatedTimeForDelivery = store.EstimatedDeliveryTime.GetValueOrDefault(45)
                });
            }
        }
    }
}