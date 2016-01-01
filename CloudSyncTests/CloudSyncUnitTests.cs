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

            StoreModel storeModel = AndroAdminTestHelper.GetTestStore(connectionString, "United Kingdom", "Live", 123, "TestCustomerSiteId", "TestExternalSiteId", "TestExternalSiteName", "TestClientSiteName", new DateTime(2001, 2, 3), "TestName");
            StoreModel storeModel2 = AndroAdminTestHelper.GetTestStore(connectionString, "United Kingdom", "Live", 321, "TestCustomerSiteId2", "TestExternalSiteId2", "TestExternalSiteName2", "TestClientSiteName2", new DateTime(2003, 2, 1), "TestName2");

            Partner partner = AndroAdminTestHelper.GetTestPartner(1, "testpartner1", "test partner 1");

            // Add a single store to the db then do a sync
            this.FirstStore(connectionString, storeModel);

            // Add a second store to the db then do a sync
            this.SecondStore(connectionString, storeModel, storeModel2);

            // Add a partner
            this.FirstPartner(connectionString, partner);
        }

        private void FirstStore(string connectionString, StoreModel storeModel)
        {
            StoreController storeController = new StoreController();
            storeController.StoreDAO = new StoreDAO();
            storeController.StoreDAO.ConnectionStringOverride = connectionString;
            
            // Add a first store
            storeController.Add(storeModel);

            // Check to see if the store exists
            string error = AndroAdminTestHelper.CheckStores(storeController, new List<StoreModel> { storeModel });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = SyncHelper.ExportSyncXml(0, 1);

            string expectedXml =
                "<CloudSync>" +
                    "<DataVersion>1</DataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteName</ExternalSiteName>" +
                            "<AndromedaSiteId>123</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteId</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml, xml, "Incorrect sync xml generated: " + xml);
        }

        private void SecondStore(string connectionString, StoreModel storeModel, StoreModel storeModel2)
        {
            StoreController storeController = new StoreController();
            storeController.StoreDAO = new StoreDAO();
            storeController.StoreDAO.ConnectionStringOverride = connectionString;

            // Add a second store
            storeController.Add(storeModel2);

            // Check to see if the store exists
            string error = AndroAdminTestHelper.CheckStores(storeController, new List<StoreModel> { storeModel, storeModel2 });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = SyncHelper.ExportSyncXml(1, 2);

            string expectedXml2 =
                "<CloudSync>" +
                    "<DataVersion>2</DataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteName2</ExternalSiteName>" +
                            "<AndromedaSiteId>321</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteId2</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            Assert.AreEqual<string>(expectedXml2, xml, "Incorrect sync xml generated: " + xml);
        }

        private void FirstPartner(string connectionString, Partner partner)
        {
            PartnerController partnerController = new PartnerController();
            partnerController.PartnerDAO = new PartnerDAO();
            partnerController.PartnerDAO.ConnectionStringOverride = connectionString;

            // Add a partner
            partnerController.Add(partner);

            // Check to see if the partner exists
            string error = AndroAdminTestHelper.CheckPartners(partnerController, new List<Partner> { partner });

            if (error.Length > 0) { Assert.Fail(error); }

            SyncHelper.ConnectionStringOverride = connectionString;
            string xml = SyncHelper.ExportSyncXml(2, 3);

            string expectedXml2 =
                "<CloudSync>" +
                    "<DataVersion>3</DataVersion>" +
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
    }
}
