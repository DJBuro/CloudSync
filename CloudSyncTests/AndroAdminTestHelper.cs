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
            string name,
            string telephone,
            string timezone,
            Address address)
        {
            Country country = null;
            StoreStatus storeStatus = null;

            using (AndroAdminDataAccess.EntityFramework.AndroAdminEntities entitiesContext = new AndroAdminDataAccess.EntityFramework.AndroAdminEntities())
            {
                entitiesContext.Database.Connection.ConnectionString = connectionString;

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
                    Status = storeStatusEntity.Status,
                };
            }

            // We need to set the address country
            address.Country = country;

            // Create a new store
            StoreModel storeModel = new StoreModel()
            {
                Store = new Store()
                {
                    AndromedaSiteId = andromedaSiteId,
                    Address = address,
                    CustomerSiteId = customerSiteId,
                    ExternalSiteId = externalSiteId,
                    ExternalSiteName = externalSiteName,
                    ClientSiteName = clientSiteName,
                    LastFTPUploadDateTime = lastFTPUploadDateTime,
                    Name = name,
                    StoreStatus = storeStatus,
                    Telephone = telephone,
                    TimeZone = timezone
                },
                Selected = true
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

        public static ACSApplicationModel GetTestPartnerApplication(Partner partner, List<StoreModel> stores, int id, string externalApplicationId, string name)
        {
            ACSApplication acsApplication = new ACSApplication()
            {
                Id = id,
                ExternalApplicationId = externalApplicationId,
                Name = name,
                PartnerId = partner.Id
            };

            ACSApplicationModel acsApplicationModel = new ACSApplicationModel()
            {
                Partner = partner,
                ACSApplication = acsApplication,
                Stores = stores
            };

            return acsApplicationModel;
        }

        public static string CheckForModelError(ActionResult actionResult, Controller controller)
        {
            if (actionResult is RedirectToRouteResult)
            {
                RedirectToRouteResult redirectToRouteResult = (RedirectToRouteResult)actionResult;

                foreach (Object value in redirectToRouteResult.RouteValues.Values)
                {
                    // Are we redirecting to the error controller?
                    if (value is string && ((string)value) == "Error")
                    {
                        return "Controller returned an error: " + redirectToRouteResult.RouteName;
                    }
                }
            }

            // Check for model errors
            if (controller.ViewData.ModelState.Count != 0)
            {
                return "Controller returned model errors:" + controller.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            return "";
        }

        public static string CheckStores(StoreController storeController, List<StoreModel> expectedStoreModels)
        {
            // Get stores
            ViewResult viewResult = (ViewResult)storeController.Index();

            if (storeController.ViewData.ModelState.Count != 0)
            {
                return "StoreController returned " + storeController.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            if (viewResult.ViewData.Model.GetType() != typeof(List<Store>))
            {
                return "StoreController returned wrong action result. Expected: " + typeof(List<Store>).ToString() + " Got: " + viewResult.ViewData.Model.GetType().ToString();
            }

            List<Store> actualStores = (List<Store>)viewResult.ViewData.Model;

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

                if (foundStore.Address.Org1 != expectedStoreModel.Store.Address.Org1) return "Org1 mismatch. Expected: " + expectedStoreModel.Store.Address.Org1 + " Got: " + foundStore.Address.Org1;
                if (foundStore.Address.Org2 != expectedStoreModel.Store.Address.Org2) return "Org2 mismatch. Expected: " + expectedStoreModel.Store.Address.Org2 + " Got: " + foundStore.Address.Org2;
                if (foundStore.Address.Org3 != expectedStoreModel.Store.Address.Org3) return "Org3 mismatch. Expected: " + expectedStoreModel.Store.Address.Org3 + " Got: " + foundStore.Address.Org3;
                if (foundStore.Address.Prem1 != expectedStoreModel.Store.Address.Prem1) return "Prem1 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem1 + " Got: " + foundStore.Address.Prem1;
                if (foundStore.Address.Prem2 != expectedStoreModel.Store.Address.Prem2) return "Prem2 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem2 + " Got: " + foundStore.Address.Prem2;
                if (foundStore.Address.Prem3 != expectedStoreModel.Store.Address.Prem3) return "Prem3 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem3 + " Got: " + foundStore.Address.Prem3;
                if (foundStore.Address.Prem4 != expectedStoreModel.Store.Address.Prem4) return "Prem4 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem4 + " Got: " + foundStore.Address.Prem4;
                if (foundStore.Address.Prem5 != expectedStoreModel.Store.Address.Prem5) return "Prem5 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem5 + " Got: " + foundStore.Address.Prem5;
                if (foundStore.Address.Prem6 != expectedStoreModel.Store.Address.Prem6) return "Prem6 mismatch. Expected: " + expectedStoreModel.Store.Address.Prem6 + " Got: " + foundStore.Address.Prem6;
                if (foundStore.Address.RoadNum != expectedStoreModel.Store.Address.RoadNum) return "RoadNum mismatch. Expected: " + expectedStoreModel.Store.Address.RoadNum + " Got: " + foundStore.Address.RoadNum;
                if (foundStore.Address.RoadName != expectedStoreModel.Store.Address.RoadName) return "RoadName mismatch. Expected: " + expectedStoreModel.Store.Address.RoadName + " Got: " + foundStore.Address.RoadName;
                if (foundStore.Address.Locality != expectedStoreModel.Store.Address.Locality) return "Locality mismatch. Expected: " + expectedStoreModel.Store.Address.Locality + " Got: " + foundStore.Address.Locality;
                if (foundStore.Address.Town != expectedStoreModel.Store.Address.Town) return "Town mismatch. Expected: " + expectedStoreModel.Store.Address.Town + " Got: " + foundStore.Address.Town;
                if (foundStore.Address.County != expectedStoreModel.Store.Address.County) return "County mismatch. Expected: " + expectedStoreModel.Store.Address.County + " Got: " + foundStore.Address.County;
                if (foundStore.Address.State != expectedStoreModel.Store.Address.State) return "State mismatch. Expected: " + expectedStoreModel.Store.Address.State + " Got: " + foundStore.Address.State;
                if (foundStore.Address.PostCode != expectedStoreModel.Store.Address.PostCode) return "PostCode mismatch. Expected: " + expectedStoreModel.Store.Address.PostCode + " Got: " + foundStore.Address.PostCode;
                if (foundStore.Address.DPS != expectedStoreModel.Store.Address.DPS) return "DPS mismatch. Expected: " + expectedStoreModel.Store.Address.DPS + " Got: " + foundStore.Address.DPS;
                if (foundStore.Address.Lat != expectedStoreModel.Store.Address.Lat) return "Lat mismatch. Expected: " + expectedStoreModel.Store.Address.Lat + " Got: " + foundStore.Address.Lat;
                if (foundStore.Address.Long != expectedStoreModel.Store.Address.Long) return "Long mismatch. Expected: " + expectedStoreModel.Store.Address.Long + " Got: " + foundStore.Address.Long;
                if (foundStore.Address.Country.Id != expectedStoreModel.Store.Address.Country.Id) return "Country.Id mismatch. Expected: " + expectedStoreModel.Store.Address.Country.Id + " Got: " + foundStore.Address.Country.Id;

                // We need to store the db id in the expected objects cos we use these later for more testing
                expectedStoreModel.Store.Id = foundStore.Id;
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

            // Did the controller return any errors?
            if (partnerController.ViewData.ModelState.Count != 0)
            {
                return "PartnerController returned " + partnerController.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            IList<Partner> actualPartners = (List<Partner>)viewResult.ViewData.Model;
            
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

        public static string CheckPartnerApplications(PartnerController partnerController, Partner partner, List<ACSApplicationModel> expectedACSApplicationModels)
        {
            // Get partners applications
            ViewResult viewResult = (ViewResult)partnerController.Details(partner.Id, false);

            if (viewResult.ViewData.Model.GetType() != typeof(PartnerModel))
            {
                return "PartnerController returned wrong action result. Expected: " + typeof(PartnerModel).ToString() + " Got: " + viewResult.ViewData.Model.GetType().ToString();
            }

            // Did the controller return any errors?
            if (partnerController.ViewData.ModelState.Count != 0)
            {
                return "PartnerController returned " + partnerController.ViewData.ModelState.Count.ToString(); // + " model errors: " + storeController.ViewData.ModelState[0].Errors[0]);
            }

            PartnerModel actualPartnerModel = (PartnerModel)viewResult.ViewData.Model;

            // Check that the partner has the correct applications
            foreach (ACSApplicationModel expectedACSApplicationModel in expectedACSApplicationModels)
            {
                ACSApplication foundACSApplication = null;
                foreach (ACSApplication actualACSApplication in actualPartnerModel.ACSApplications)
                {
                    if (expectedACSApplicationModel.ACSApplication.Id == actualACSApplication.Id)
                    {
                        foundACSApplication = actualACSApplication;
                        break;
                    }
                }

                if (foundACSApplication == null)
                {
                    return "PartnerController didn't return partner application " + expectedACSApplicationModel.ACSApplication.Id.ToString();
                }

                if (foundACSApplication.ExternalApplicationId != expectedACSApplicationModel.ACSApplication.ExternalApplicationId) return "ExternalApplicationId mismatch. Expected: " + expectedACSApplicationModel.ACSApplication.ExternalApplicationId + " Got: " + foundACSApplication.ExternalApplicationId;
                if (foundACSApplication.Name != expectedACSApplicationModel.ACSApplication.Name) return "Name mismatch. Expected: " + expectedACSApplicationModel.ACSApplication.Name + " Got: " + foundACSApplication.Name;
                if (foundACSApplication.PartnerId != expectedACSApplicationModel.ACSApplication.PartnerId) return "PartnerId mismatch. Expected: " + expectedACSApplicationModel.ACSApplication.PartnerId + " Got: " + foundACSApplication.PartnerId;
            }

            // All good
            return "";
        }
    }
}
