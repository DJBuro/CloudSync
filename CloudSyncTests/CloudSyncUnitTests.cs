using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AndroAdmin.Controllers;
using AndroAdmin.Model;
using AndroAdminDataAccess.Domain;
using AndroAdminDataAccess.EntityFramework.DataAccess;
using CloudSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudSyncTests
{
    [TestClass]
    public class CloudSyncUnitTests
    {
        [TestMethod]
        public void SyncTest()
        {
            // Create a test database
            string connectionString = DatabaseHelper.CreateTestAndroAdminDatabase();

            StoreModel storeModel = AndroAdminTestHelper.GetTestStore(
                connectionString, 
                "United Kingdom", 
                "Live", 
                123, 
                "TestCustomerSiteId", 
                "TestExternalSiteId", 
                "TestExternalSiteName", 
                "TestClientSiteName", 
                new DateTime(2001, 2, 3), 
                "TestName",
                "1234567890",
                "GMT",
                new Address()
                {
                    Org1 = "Test1_Org1",
                    Org2  = "Test1_Org2",
                    Org3 = "Test1_Org3",
                    Prem1 = "Test1_Prem1",
                    Prem2 = "Test1_Prem2",
                    Prem3 = "Test1_Prem3",
                    Prem4 = "Test1_Prem4",
                    Prem5 = "Test1_Prem5",
                    Prem6 = "Test1_Prem6",
                    RoadNum = "Test1_RoadNum",
                    RoadName = "Test1_RoadName",
                    Locality = "Test1_Locality",
                    Town = "Test1_Town",
                    County = "Test1_County",
                    State = "Test1_State",
                    PostCode = "Test1_PostCode",
                    DPS = "DPS1",
                    Lat = "1.2",
                    Long = "3.4"
                }
            );

            StoreModel storeModel2 = AndroAdminTestHelper.GetTestStore(
                connectionString, 
                "United Kingdom", 
                "Live", 
                321, 
                "TestCustomerSiteId2", 
                "TestExternalSiteId2", 
                "TestExternalSiteName2", 
                "TestClientSiteName2", 
                new DateTime(2003, 2, 1), 
                "TestName2",
                "0987654321",
                "GMT",
                new Address()
                {
                    Org1 = "Test2_Org1",
                    Org2 = "Test2_Org2",
                    Org3 = "Test2_Org3",
                    Prem1 = "Test2_Prem1",
                    Prem2 = "Test2_Prem2",
                    Prem3 = "Test2_Prem3",
                    Prem4 = "Test2_Prem4",
                    Prem5 = "Test2_Prem5",
                    Prem6 = "Test2_Prem6",
                    RoadNum = "Test2_RoadNum",
                    RoadName = "Test2_RoadName",
                    Locality = "Test2_Locality",
                    Town = "Test2_Town",
                    County = "Test2_County",
                    State = "Test2_State",
                    PostCode = "Test2_PostCode",
                    DPS = "DPS2",
                    Lat = "1.2",
                    Long = "3.4"
                }
            );

            // Add a single store to the db then do a sync
            this.FirstStore(connectionString, storeModel);

            // Add a second store to the db then do a sync
            this.SecondStore(connectionString, storeModel, storeModel2);

            // Add a partner
            Partner partner = AndroAdminTestHelper.GetTestPartner(1, "testpartner1", "test partner 1");
            this.FirstPartner(connectionString, partner);

            // Add an application to the partner
            ACSApplicationModel acsApplicationModel = AndroAdminTestHelper.GetTestPartnerApplication(partner, null, 1, "TestExternalApplicationid", "TestName");
            this.FirstPartnerApplication(connectionString, acsApplicationModel);

            // Add a store to the application
            acsApplicationModel.Stores = new List<StoreModel> { storeModel };
            this.FirstPartnerApplicationStore(connectionString, acsApplicationModel);

            // Add a second store to the application
            acsApplicationModel.Stores = new List<StoreModel> { storeModel, storeModel2 };
            this.SecondPartnerApplicationStore(connectionString, acsApplicationModel);

            // Remove the first store from the application
            acsApplicationModel.Stores = new List<StoreModel> { storeModel2 };
            this.RemoveFirstPartnerApplicationStore(connectionString, acsApplicationModel);
        }

        private void FirstStore(string connectionString, StoreModel storeModel)
        {
            StoreController storeController = new StoreController();
            storeController.ConnectionStringOverride = connectionString;
            
            // Add a first store
            ActionResult actionResult = storeController.Add(storeModel);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, storeController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the store exists
            error = AndroAdminTestHelper.CheckStores(storeController, new List<StoreModel> { storeModel });
            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(0, 1, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml =
                "<CloudSync>" +
                    "<FromDataVersion>0</FromDataVersion>" +
                    "<ToDataVersion>1</ToDataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteName</ExternalSiteName>" +
                            "<AndromedaSiteId>123</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteId</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                            "<Phone>1234567890</Phone>" +
                            "<TimeZone>GMT</TimeZone>" +
                            "<Address>" +
                                "<Id>1</Id>" +
                                "<Org1>Test1_Org1</Org1>" +
                                "<Org2>Test1_Org2</Org2>" +
                                "<Org3>Test1_Org3</Org3>" +
                                "<Prem1>Test1_Prem1</Prem1>" +
                                "<Prem2>Test1_Prem2</Prem2>" +
                                "<Prem3>Test1_Prem3</Prem3>" +
                                "<Prem4>Test1_Prem4</Prem4>" +
                                "<Prem5>Test1_Prem5</Prem5>" +
                                "<Prem6>Test1_Prem6</Prem6>" +
                                "<RoadNum>Test1_RoadNum</RoadNum>" +
                                "<RoadName>Test1_RoadName</RoadName>" +
                                "<Locality>Test1_Locality</Locality>" +
                                "<Town>Test1_Town</Town>"+
                                "<County>Test1_County</County>" +
                                "<State>Test1_State</State>" +
                                "<PostCode>Test1_PostCode</PostCode>" +
                                "<DPS>DPS1</DPS>" +
                                "<Lat>1.2</Lat>" +
                                "<Long>3.4</Long>" +
                                "<CountryId>234</CountryId>" +
                            "</Address>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml, xml, "Incorrect sync xml generated: " + xml);
        }

        private void SecondStore(string connectionString, StoreModel storeModel, StoreModel storeModel2)
        {
            StoreController storeController = new StoreController();
            storeController.ConnectionStringOverride = connectionString;

            // Add a second store
            ActionResult actionResult = storeController.Add(storeModel2);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, storeController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the store exists
            error = AndroAdminTestHelper.CheckStores(storeController, new List<StoreModel> { storeModel, storeModel2 });

            if (error.Length > 0) { Assert.Fail(error); }

            // Export to XML
            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(1, 2, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>1</FromDataVersion>" +
                    "<ToDataVersion>2</ToDataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteName2</ExternalSiteName>" +
                            "<AndromedaSiteId>321</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteId2</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                            "<Phone>0987654321</Phone>" +
                            "<TimeZone>GMT</TimeZone>" +
                            "<Address>" +
                                "<Id>2</Id>" +
                                "<Org1>Test2_Org1</Org1>" +
                                "<Org2>Test2_Org2</Org2>" +
                                "<Org3>Test2_Org3</Org3>" +
                                "<Prem1>Test2_Prem1</Prem1>" +
                                "<Prem2>Test2_Prem2</Prem2>" +
                                "<Prem3>Test2_Prem3</Prem3>" +
                                "<Prem4>Test2_Prem4</Prem4>" +
                                "<Prem5>Test2_Prem5</Prem5>" +
                                "<Prem6>Test2_Prem6</Prem6>" +
                                "<RoadNum>Test2_RoadNum</RoadNum>" +
                                "<RoadName>Test2_RoadName</RoadName>" +
                                "<Locality>Test2_Locality</Locality>" +
                                "<Town>Test2_Town</Town>" +
                                "<County>Test2_County</County>" +
                                "<State>Test2_State</State>" +
                                "<PostCode>Test2_PostCode</PostCode>" +
                                "<DPS>DPS2</DPS>" +
                                "<Lat>1.2</Lat>" +
                                "<Long>3.4</Long>" +
                                "<CountryId>234</CountryId>" +
                            "</Address>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void FirstPartner(string connectionString, Partner partner)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.ConnectionStringOverride = connectionString;

            // Add a partner
            ActionResult actionResult = partnerController.Add(partner);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, partnerController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the partner exists
            error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { partner });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(2, 3, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>2</FromDataVersion>" +
                    "<ToDataVersion>3</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1</Name>" +
                            "<ExternalId>testpartner1</ExternalId>" +
                            "<Applications />" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void FirstPartnerApplication(string connectionString, ACSApplicationModel application)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.ConnectionStringOverride = connectionString;

            // Add an application
            ActionResult actionResult = partnerController.AddApplication(application);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, partnerController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the partner exists
            error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { application.Partner });

            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the application exists
            error = AndroAdminTestHelper.CheckPartnerApplications(partnerController, application.Partner, new List<ACSApplicationModel> { application });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(3, 4, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>3</FromDataVersion>" +
                    "<ToDataVersion>4</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1</Name>" +
                            "<ExternalId>testpartner1</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites />" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void FirstPartnerApplicationStore(string connectionString, ACSApplicationModel application)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.ConnectionStringOverride = connectionString;

            // Add a store to the application
            ActionResult actionResult = partnerController.ApplicationStores(application);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, partnerController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the partner exists
            error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { application.Partner });

            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the application exists
            error = AndroAdminTestHelper.CheckPartnerApplications(partnerController, application.Partner, new List<ACSApplicationModel> { application });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(4, 5, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>4</FromDataVersion>" +
                    "<ToDataVersion>5</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1</Name>" +
                            "<ExternalId>testpartner1</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites>" +
                                    "123" +
                                    "</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void SecondPartnerApplicationStore(string connectionString, ACSApplicationModel application)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.ConnectionStringOverride = connectionString;

            // Add a store to the application
            ActionResult actionResult = partnerController.ApplicationStores(application);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, partnerController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the partner exists
            error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { application.Partner });

            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the application exists
            error = AndroAdminTestHelper.CheckPartnerApplications(partnerController, application.Partner, new List<ACSApplicationModel> { application });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(5, 6, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>5</FromDataVersion>" +
                    "<ToDataVersion>6</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1</Name>" +
                            "<ExternalId>testpartner1</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites>" +
                                    "123,321" +
                                    "</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void RemoveFirstPartnerApplicationStore(string connectionString, ACSApplicationModel application)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.ConnectionStringOverride = connectionString;

            // Remove the first store
            application.Stores[0].Selected = false;

            // Remove the first store from the application
            ActionResult actionResult = partnerController.ApplicationStores(application);

            // Check for model errors
            string error = AndroAdminTestHelper.CheckForModelError(actionResult, partnerController);
            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the partner exists
            error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { application.Partner });

            if (error.Length > 0) { Assert.Fail(error); }

            // Check to see if the application exists
            error = AndroAdminTestHelper.CheckPartnerApplications(partnerController, application.Partner, new List<ACSApplicationModel> { application });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = "";
            string errorMessage = SyncHelper.ExportSyncXml(6, 7, out xml);
            Assert.AreEqual<string>("", errorMessage);

            string expectedXml2 =
                "<CloudSync>" +
                    "<FromDataVersion>6</FromDataVersion>" +
                    "<ToDataVersion>7</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1</Name>" +
                            "<ExternalId>testpartner1</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites>" +
                                    "123" +
                                    "</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }
    }
}
