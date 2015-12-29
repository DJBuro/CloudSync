using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AndroAdmin.Controllers;
using AndroAdmin.Model;
using AndroAdminDataAccess.Domain;
using AndroAdminDataAccess.EntityFramework.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudSyncTests
{
    public class AndroAdminTestHelper
    {
        public static StoreModel GetTestStore(
            string connectionString,
            string countryName,
            string storeStatusName,
            int andromedaSiteId,
            string customerSiteId,
            string externalSiteId,
            string externalSiteName,
            string clientSiteName,
            DateTime lastFTPUploadDateTime,
            string name)
        {
            Country country = null;
            StoreStatus storeStatus = null;

            using (AndroAdminDataAccess.EntityFramework.AndroAdminEntities entitiesContext = new AndroAdminDataAccess.EntityFramework.AndroAdminEntities(connectionString))
            {
                // Get a country for our test store
                var countryQuery = from s in entitiesContext.Countries
                            where s.CountryName == countryName
                            select s;

                AndroAdminDataAccess.EntityFramework.Country countryEntity = countryQuery.FirstOrDefault();

                // Does the country exist?
                Assert.IsNotNull(countryEntity, "Unknown country name");

                country = new Country()
                {
                    Id = countryEntity.Id,
                    CountryName = countryEntity.CountryName,
                    ISO3166_1_alpha_2 = countryEntity.ISO3166_1_alpha_2,
                    ISO3166_1_numeric = countryEntity.ISO3166_1_numeric
                };

                // Get a store status for our test store
                var storeStatusQuery = from s in entitiesContext.StoreStatus
                            where s.Status == storeStatusName
                            select s;

                AndroAdminDataAccess.EntityFramework.StoreStatu storeStatusEntity = storeStatusQuery.FirstOrDefault();

                // Does the store status exist?
                Assert.IsNotNull(storeStatusEntity, "Unknown store status");

                storeStatus = new StoreStatus()
                {
                    Id = storeStatusEntity.Id,
                    Description = storeStatusEntity.Description,
                    Status = storeStatusEntity.Status
                };
            }

            // Create a new store
            StoreModel storeModel = new StoreModel()
            {
                Store = new Store()
                {
                    AndromedaSiteId = andromedaSiteId,
                    Country = country,
                    CustomerSiteId = customerSiteId,
                    ExternalSiteId = externalSiteId,
                    ExternalSiteName = externalSiteName,
                    ClientSiteName = clientSiteName,
                    LastFTPUploadDateTime = lastFTPUploadDateTime,
                    Name = name,
                    StoreStatus = storeStatus
                }
            };

            return storeModel;
        }

        public static Partner GetTestPartner(int id, string externalId, string name)
        {
            Partner partner = new Partner()
            {
                Id = id,
                ExternalId = externalId,
                Name = name
            };

            return partner;
        }

        public static string CheckStores(StoreController storeController, List<StoreModel> expectedStoreModels)
        {
            // Get stores
            ViewResult viewResult = (ViewResult)storeController.Index();

            if (viewResult.ViewData.Model.GetType() != typeof(List<Store>))
            {
                return "StoreController returned wrong action result. Expected: " + typeof(List<Store>).ToString() + " Got: " + viewResult.ViewData.Model.GetType().ToString();
            }

            List<Store> actualStores = (List<Store>)viewResult.ViewData.Model;

            if (storeController.ViewData.ModelState.Count != 0)
            {
                return "StoreController returned " + storeController.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            if (actualStores.Count != expectedStoreModels.Count)
            {
                return "StoreController returned the wrong number of stores.  Expected: " + expectedStoreModels.Count + " Got: " + actualStores.Count.ToString();
            }

            // Check that each store was found
            foreach (StoreModel expectedStoreModel in expectedStoreModels)
            {
                Store foundStore = null;
                foreach (Store actualStore in actualStores)
                {
                    if (expectedStoreModel.Store.AndromedaSiteId == actualStore.AndromedaSiteId)
                    {
                        foundStore = actualStore;
                        break;
                    }
                }

                if (foundStore == null)
                {
                    return "StoreController didn't return store " + expectedStoreModel.Store.AndromedaSiteId.ToString();
                }

                if (foundStore.ClientSiteName != expectedStoreModel.Store.ClientSiteName) return "ClientSiteName mismatch. Expected: " + expectedStoreModel.Store.ClientSiteName + " Got: " + foundStore.ClientSiteName;
                if (foundStore.ExternalSiteId != expectedStoreModel.Store.ExternalSiteId) return "ExternalSiteId mismatch. Expected: " + expectedStoreModel.Store.ExternalSiteId + " Got: " + foundStore.ExternalSiteId;
                if (foundStore.ExternalSiteName != expectedStoreModel.Store.ExternalSiteName) return "ExternalSiteName mismatch. Expected: " + expectedStoreModel.Store.ExternalSiteName + " Got: " + foundStore.ExternalSiteName;
                if (foundStore.Name != expectedStoreModel.Store.Name) return "Name mismatch. Expected: " + expectedStoreModel.Store.Name + " Got: " + foundStore.Name;
                if (foundStore.StoreStatus.Id != expectedStoreModel.Store.StoreStatus.Id) return "StoreStatus.Id mismatch. Expected: " + expectedStoreModel.Store.StoreStatus.Id + " Got: " + foundStore.StoreStatus.Id;
 // NEED TO SORT OUT ADDRESSES AS COUNTRY IS PART OF ADDRESS
                //               if (foundStore.Country.Id != expectedStoreModel.Store.Country.Id) return "Country.Id mismatch. Expected: " + expectedStoreModel.Store.Country.Id + " Got: " + foundStore.Country.Id;
            }

            // All good
            return "";
        }

        public static string CheckPartners(PartnerController partnerController, List<Partner> expectedPartners)
        {
            // Get partners
            ViewResult viewResult = (ViewResult)partnerController.Index();

            if (viewResult.ViewData.Model.GetType() != typeof(List<Partner>))
            {
                return "PartnerController returned wrong action result. Expected: " + typeof(List<Partner>).ToString() + " Got: " + viewResult.ViewData.Model.GetType().ToString();
            }

            IList<Partner> actualPartners = (List<Partner>)viewResult.ViewData.Model;

            if (partnerController.ViewData.ModelState.Count != 0)
            {
                return "PartnerController returned " + partnerController.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            if (actualPartners.Count != expectedPartners.Count)
            {
                return "PartnerController returned the wrong number of stores.  Expected: " + expectedPartners.Count + " Got: " + actualPartners.Count.ToString();
            }

            // Check that each partner was found
            foreach (Partner expectedPartner in expectedPartners)
            {
                Partner foundPartner = null;
                foreach (Partner actualPartner in actualPartners)
                {
                    if (expectedPartner.Id == actualPartner.Id)
                    {
                        foundPartner = actualPartner;
                        break;
                    }
                }

                if (foundPartner == null)
                {
                    return "PartnerController didn't return partner " + expectedPartner.Id.ToString();
                }

                if (foundPartner.ExternalId != expectedPartner.ExternalId) return "ExternalId mismatch. Expected: " + expectedPartner.ExternalId + " Got: " + foundPartner.ExternalId;
                if (foundPartner.Name != expectedPartner.Name) return "Name mismatch. Expected: " + expectedPartner.Name + " Got: " + foundPartner.Name;
            }

            // All good
            return "";
        }
    }
}
