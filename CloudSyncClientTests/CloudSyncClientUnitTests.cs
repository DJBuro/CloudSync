using System;
using System.Xml.Linq;
using AndroCloudDataAccess;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccess.Domain;
using AndroCloudDataAccessEntityFramework;
using AndroCloudDataAccessEntityFramework.DataAccess;
using AndroCloudDataAccessEntityFramework.Model;
using AndroCloudHelper;
using AndroCloudServices.Services;
using CloudSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CloudSyncClientTests
{
    [TestClass]
    public class CloudSyncClientUnitTests
    {
        [TestMethod]
        public void SyncTest()
        {
            // Create a test database
            string connectionString = DatabaseHelper.CreateTestACSDatabase();

            SyncHelper.ConnectionStringOverride = connectionString;

            this.FirstStore();
            this.FirstPartner();
            this.FirstPartnerApplication();
            this.FirstPartnerApplicationStore(connectionString);
            this.ModifyFirstStore(connectionString);
            this.ModifyFirstPartner(connectionString);
            this.ModifyFirstPartnerApplication(connectionString);
            this.SecondStore();
            this.ModifyFirstPartnerApplicationStore(connectionString);
            this.DeleteFirstPartnerApplicationStore(connectionString);
        }

        public void FirstStore()
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>0</FromDataVersion>" +
                    "<ToDataVersion>1</ToDataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteName</ExternalSiteName>" +
                            "<AndromedaSiteId>123</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteId</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                            "<TimeZone>GMT</TimeZone>" +
                            "<Phone>1234567890</Phone>" +
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
                                "<Town>Test1_Town</Town>" +
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

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            // Get the store (we need the id)
            ISiteDataAccess siteDataAccess = new SitesDataAccess();
            if (SyncHelper.ConnectionStringOverride != null) siteDataAccess.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
            AndroCloudDataAccess.Domain.Site site = null;

            siteDataAccess.GetByExternalSiteId("TestExternalSiteId", out site);

            Assert.IsNotNull(site, "Site not found");

            // Create a fake menu for the store
            IDataAccessFactory dataAccessFactory = new EntityFrameworkDataAccessFactory(){ ConnectionStringOverride=SyncHelper.ConnectionStringOverride };
            string sourceId = "";
            
            MenuService.Post(site.AndroId.ToString(), "A24C92FE-92D1-4705-8E33-202F51BCE38D", "testhardwarekey", "1", "testmenu", DataTypeEnum.XML, dataAccessFactory, out sourceId);
        }

        public void FirstPartner()
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>1</FromDataVersion>" +
                    "<ToDataVersion>2</ToDataVersion>" +
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

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);
        }

        public void FirstPartnerApplication()
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>2</FromDataVersion>" +
                    "<ToDataVersion>3</ToDataVersion>" +
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

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);
        }

        public void FirstPartnerApplicationStore(string connectionString)
        {
            string syncXml =
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
                                    "<Sites>123</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            string expectedSiteListXml =
                "<Sites>" +
                    "<Site>" +
                        "<SiteId>TestExternalSiteId</SiteId>" +
                        "<Name>TestExternalSiteName</Name>" +
                        "<MenuVersion>1</MenuVersion>" +
                        "<IsOpen>false</IsOpen>" +
                        "<EstDelivTime>0</EstDelivTime>" +
                    "</Site>" +
                "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationid", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteListXml,
                siteListResponse.ResponseText,
                "Wrong site list xml");

            // Expected site details xml
            string expectedSiteDetailsXml =
                "<SiteDetails>" +
                    "<SiteId>TestExternalSiteId</SiteId>" +
                    "<Name>TestExternalSiteName</Name>" +
                    "<MenuVersion>1</MenuVersion>" +
                    "<IsOpen>false</IsOpen>" +
                    "<EstDelivTime>0</EstDelivTime>" +
                    "<TimeZone>GMT</TimeZone>" +
                    "<Phone>1234567890</Phone>" +
                    "<Address>" +
                        "<Long>3.4</Long>" +
                        "<Lat>1.2</Lat>" +
                        "<Prem1>Test1_Prem1</Prem1>" +
                        "<Prem2>Test1_Prem2</Prem2>" +
                        "<Prem3>Test1_Prem3</Prem3>" +
                        "<Prem4>Test1_Prem4</Prem4>" +
                        "<Prem5>Test1_Prem5</Prem5>" +
                        "<Prem6>Test1_Prem6</Prem6>" +
                        "<Org1>Test1_Org1</Org1>" +
                        "<Org2>Test1_Org2</Org2>" +
                        "<Org3>Test1_Org3</Org3>" +
                        "<RoadNum>Test1_RoadNum</RoadNum>" +
                        "<RoadName>Test1_RoadName</RoadName>" +
                        "<Town>Test1_Town</Town>" +
                        "<Postcode>Test1_PostCode</Postcode>" +
                        "<Dps>DPS1</Dps>" +
                        "<County>Test1_County</County>" +
                        "<Locality>Test1_Locality</Locality>" +
                        "<Country>United Kingdom</Country>" +
                    "</Address>" +
                    "<OpeningHours />" +
                    "<PaymentProvider />" +
                    "<PaymentClientId />" +
                    "<PaymentClientPassword />" +
                "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationid", "TestExternalSiteId", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteDetailsXml,
                siteDetailsResponse.ResponseText,
                "Wrong site details xml");
        }

        public void ModifyFirstStore(string connectionString)
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>4</FromDataVersion>" +
                    "<ToDataVersion>5</ToDataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>TestExternalSiteNameZZZ</ExternalSiteName>" +
                            "<AndromedaSiteId>123</AndromedaSiteId>" +
                            "<ExternalSiteId>TestExternalSiteIdZZZ</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                            "<TimeZone>GMT+1</TimeZone>" +
                            "<Phone>1234567890ZZZ</Phone>" +
                            "<Address>" +
                                "<Id>1</Id>" +
                                "<Org1>Test1_Org1ZZZ</Org1>" +
                                "<Org2>Test1_Org2ZZZ</Org2>" +
                                "<Org3>Test1_Org3ZZZ</Org3>" +
                                "<Prem1>Test1_Prem1ZZZ</Prem1>" +
                                "<Prem2>Test1_Prem2ZZZ</Prem2>" +
                                "<Prem3>Test1_Prem3ZZZ</Prem3>" +
                                "<Prem4>Test1_Prem4ZZZ</Prem4>" +
                                "<Prem5>Test1_Prem5ZZZ</Prem5>" +
                                "<Prem6>Test1_Prem6ZZZ</Prem6>" +
                                "<RoadNum>Test1_RoadNumZZZ</RoadNum>" +
                                "<RoadName>Test1_RoadNameZZZ</RoadName>" +
                                "<Locality>Test1_LocalityZZZ</Locality>" +
                                "<Town>Test1_TownZZZ</Town>" +
                                "<County>Test1_CountyZZZ</County>" +
                                "<State>Test1_StateZZZ</State>" +
                                "<PostCode>Test1_PostCodeZZ</PostCode>" +
                                "<DPS>ZZZ1</DPS>" +
                                "<Lat>9.8</Lat>" +
                                "<Long>7.6</Long>" +
                                "<CountryId>235</CountryId>" +
                            "</Address>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            string expectedSiteListXml =
                "<Sites>" +
                    "<Site>" +
                        "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                        "<Name>TestExternalSiteNameZZZ</Name>" +
                        "<MenuVersion>1</MenuVersion>" +
                        "<IsOpen>false</IsOpen>" +
                        "<EstDelivTime>0</EstDelivTime>" +
                    "</Site>" +
                "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationid", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteListXml,
                siteListResponse.ResponseText,
                "Wrong site list xml");

            // Expected site details xml
            string expectedSiteDetailsXml =
                "<SiteDetails>" +
                    "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                    "<Name>TestExternalSiteNameZZZ</Name>" +
                    "<MenuVersion>1</MenuVersion>" +
                    "<IsOpen>false</IsOpen>" +
                    "<EstDelivTime>0</EstDelivTime>" +
                    "<TimeZone>GMT+1</TimeZone>" +
                    "<Phone>1234567890ZZZ</Phone>" +
                    "<Address>" +
                        "<Long>7.6</Long>" +
                        "<Lat>9.8</Lat>" +
                        "<Prem1>Test1_Prem1ZZZ</Prem1>" +
                        "<Prem2>Test1_Prem2ZZZ</Prem2>" +
                        "<Prem3>Test1_Prem3ZZZ</Prem3>" +
                        "<Prem4>Test1_Prem4ZZZ</Prem4>" +
                        "<Prem5>Test1_Prem5ZZZ</Prem5>" +
                        "<Prem6>Test1_Prem6ZZZ</Prem6>" +
                        "<Org1>Test1_Org1ZZZ</Org1>" +
                        "<Org2>Test1_Org2ZZZ</Org2>" +
                        "<Org3>Test1_Org3ZZZ</Org3>" +
                        "<RoadNum>Test1_RoadNumZZZ</RoadNum>" +
                        "<RoadName>Test1_RoadNameZZZ</RoadName>" +
                        "<Town>Test1_TownZZZ</Town>" +
                        "<Postcode>Test1_PostCodeZZ</Postcode>" +
                        "<Dps>ZZZ1</Dps>" +
                        "<County>Test1_CountyZZZ</County>" +
                        "<Locality>Test1_LocalityZZZ</Locality>" +
                        "<Country>United States</Country>" +
                    "</Address>" +
                    "<OpeningHours />" +
                    "<PaymentProvider />" +
                    "<PaymentClientId />" +
                    "<PaymentClientPassword />" +
                "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationid", "TestExternalSiteIdZZZ", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteDetailsXml,
                siteDetailsResponse.ResponseText,
                "Wrong site details xml");
        }

        public void ModifyFirstPartner(string connectionString)
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>5</FromDataVersion>" +
                    "<ToDataVersion>6</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1XXX</Name>" +
                            "<ExternalId>testpartner1XXX</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites>123</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);
            
            // At the moment there is no way to test that this has worked - we can just test that the correct sites are returned for the application id
            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            string expectedSiteListXml =
                "<Sites>" +
                    "<Site>" +
                        "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                        "<Name>TestExternalSiteNameZZZ</Name>" +
                        "<MenuVersion>1</MenuVersion>" +
                        "<IsOpen>false</IsOpen>" +
                        "<EstDelivTime>0</EstDelivTime>" +
                    "</Site>" +
                "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationid", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteListXml,
                siteListResponse.ResponseText,
                "Wrong site list xml");

            // Expected site details xml
            string expectedSiteDetailsXml =
                "<SiteDetails>" +
                    "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                    "<Name>TestExternalSiteNameZZZ</Name>" +
                    "<MenuVersion>1</MenuVersion>" +
                    "<IsOpen>false</IsOpen>" +
                    "<EstDelivTime>0</EstDelivTime>" +
                    "<TimeZone>GMT+1</TimeZone>" +
                    "<Phone>1234567890ZZZ</Phone>" +
                    "<Address>" +
                        "<Long>7.6</Long>" +
                        "<Lat>9.8</Lat>" +
                        "<Prem1>Test1_Prem1ZZZ</Prem1>" +
                        "<Prem2>Test1_Prem2ZZZ</Prem2>" +
                        "<Prem3>Test1_Prem3ZZZ</Prem3>" +
                        "<Prem4>Test1_Prem4ZZZ</Prem4>" +
                        "<Prem5>Test1_Prem5ZZZ</Prem5>" +
                        "<Prem6>Test1_Prem6ZZZ</Prem6>" +
                        "<Org1>Test1_Org1ZZZ</Org1>" +
                        "<Org2>Test1_Org2ZZZ</Org2>" +
                        "<Org3>Test1_Org3ZZZ</Org3>" +
                        "<RoadNum>Test1_RoadNumZZZ</RoadNum>" +
                        "<RoadName>Test1_RoadNameZZZ</RoadName>" +
                        "<Town>Test1_TownZZZ</Town>" +
                        "<Postcode>Test1_PostCodeZZ</Postcode>" +
                        "<Dps>ZZZ1</Dps>" +
                        "<County>Test1_CountyZZZ</County>" +
                        "<Locality>Test1_LocalityZZZ</Locality>" +
                        "<Country>United States</Country>" +
                    "</Address>" +
                    "<OpeningHours />" +
                    "<PaymentProvider />" +
                    "<PaymentClientId />" +
                    "<PaymentClientPassword />" +
                "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationid", "TestExternalSiteIdZZZ", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteDetailsXml,
                siteDetailsResponse.ResponseText,
                "Wrong site details xml");
        }

        public void ModifyFirstPartnerApplication(string connectionString)
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>6</FromDataVersion>" +
                    "<ToDataVersion>7</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1AAA</Name>" +
                            "<ExternalId>testpartner1AAA</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationidAAA</ExternalApplicationId>" +
                                    "<Name>TestNameAAA</Name>" +
                                    "<Sites>123</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            string expectedSiteListXml =
                "<Sites>" +
                    "<Site>" +
                        "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                        "<Name>TestExternalSiteNameZZZ</Name>" +
                        "<MenuVersion>1</MenuVersion>" +
                        "<IsOpen>false</IsOpen>" +
                        "<EstDelivTime>0</EstDelivTime>" +
                    "</Site>" +
                "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationidAAA", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteListXml,
                siteListResponse.ResponseText,
                "Wrong site list xml");

            // Expected site details xml
            string expectedSiteDetailsXml =
                "<SiteDetails>" +
                    "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
                    "<Name>TestExternalSiteNameZZZ</Name>" +
                    "<MenuVersion>1</MenuVersion>" +
                    "<IsOpen>false</IsOpen>" +
                    "<EstDelivTime>0</EstDelivTime>" +
                    "<TimeZone>GMT+1</TimeZone>" +
                    "<Phone>1234567890ZZZ</Phone>" +
                    "<Address>" +
                        "<Long>7.6</Long>" +
                        "<Lat>9.8</Lat>" +
                        "<Prem1>Test1_Prem1ZZZ</Prem1>" +
                        "<Prem2>Test1_Prem2ZZZ</Prem2>" +
                        "<Prem3>Test1_Prem3ZZZ</Prem3>" +
                        "<Prem4>Test1_Prem4ZZZ</Prem4>" +
                        "<Prem5>Test1_Prem5ZZZ</Prem5>" +
                        "<Prem6>Test1_Prem6ZZZ</Prem6>" +
                        "<Org1>Test1_Org1ZZZ</Org1>" +
                        "<Org2>Test1_Org2ZZZ</Org2>" +
                        "<Org3>Test1_Org3ZZZ</Org3>" +
                        "<RoadNum>Test1_RoadNumZZZ</RoadNum>" +
                        "<RoadName>Test1_RoadNameZZZ</RoadName>" +
                        "<Town>Test1_TownZZZ</Town>" +
                        "<Postcode>Test1_PostCodeZZ</Postcode>" +
                        "<Dps>ZZZ1</Dps>" +
                        "<County>Test1_CountyZZZ</County>" +
                        "<Locality>Test1_LocalityZZZ</Locality>" +
                        "<Country>United States</Country>" +
                    "</Address>" +
                    "<OpeningHours />" +
                    "<PaymentProvider />" +
                    "<PaymentClientId />" +
                    "<PaymentClientPassword />" +
                "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationidAAA", "TestExternalSiteIdZZZ", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteDetailsXml,
                siteDetailsResponse.ResponseText,
                "Wrong site details xml");
        }

        public void SecondStore()
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>7</FromDataVersion>" +
                    "<ToDataVersion>8</ToDataVersion>" +
                    "<Stores>" +
                        "<Store>" +
                            "<ExternalSiteName>Test2ExternalSiteName</ExternalSiteName>" +
                            "<AndromedaSiteId>124</AndromedaSiteId>" +
                            "<ExternalSiteId>Test2ExternalSiteId</ExternalSiteId>" +
                            "<StoreStatus>Live</StoreStatus>" +
                            "<TimeZone>GMT+4</TimeZone>" +
                            "<Phone>2222222</Phone>" +
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
                                "<Lat>2.111</Lat>" +
                                "<Long>4.333</Long>" +
                                "<CountryId>234</CountryId>" +
                            "</Address>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            // Get the store (we need the id)
            ISiteDataAccess siteDataAccess = new SitesDataAccess();
            if (SyncHelper.ConnectionStringOverride != null) siteDataAccess.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
            AndroCloudDataAccess.Domain.Site site = null;

            siteDataAccess.GetByExternalSiteId("Test2ExternalSiteId", out site);

            Assert.IsNotNull(site, "Site not found");

            // Create a fake menu for the store
            IDataAccessFactory dataAccessFactory = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = SyncHelper.ConnectionStringOverride };
            string sourceId = "";

            MenuService.Post(site.AndroId.ToString(), "A24C92FE-92D1-4705-8E33-202F51BCE38D", "testhardwarekey", "1", "testmenu", DataTypeEnum.XML, dataAccessFactory, out sourceId);
        }

        public void ModifyFirstPartnerApplicationStore(string connectionString)
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>8</FromDataVersion>" +
                    "<ToDataVersion>9</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1AAA</Name>" +
                            "<ExternalId>testpartner1AAA</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationidAAA</ExternalApplicationId>" +
                                    "<Name>TestNameAAA</Name>" +
                                    "<Sites>123,124</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            //string expectedSiteListXml =
            //    "<Sites>" +
            //        "<Site>" +
            //            "<SiteId>Test2ExternalSiteId</SiteId>" +
            //            "<Name>Test2ExternalSiteName</Name>" +
            //            "<MenuVersion>1</MenuVersion>" +
            //            "<IsOpen>false</IsOpen>" +
            //            "<EstDelivTime>0</EstDelivTime>" +
            //        "</Site>" +
            //        "<Site>" +
            //            "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
            //            "<Name>TestExternalSiteNameZZZ</Name>" +
            //            "<MenuVersion>1</MenuVersion>" +
            //            "<IsOpen>false</IsOpen>" +
            //            "<EstDelivTime>0</EstDelivTime>" +
            //        "</Site>" +
            //    "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationidAAA", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the sites xml
            XElement xElement = XElement.Parse(siteListResponse.ResponseText);

            // Check that the correct number of sites were returned
            var actualSites = (from sites in xElement.Elements("Site")
                             select sites).ToList();
            Assert.AreEqual<int>(2, actualSites.Count);

            // Check to see if the first site was returned
            errorMessage = XmlHelper.CheckSite(xElement, "Test2ExternalSiteId", "Test2ExternalSiteName");

            // Check to see if the second site was returned
            errorMessage = XmlHelper.CheckSite(xElement, "TestExternalSiteIdZZZ", "TestExternalSiteNameZZZ");

            // Expected site details xml
            //string expectedSiteDetailsXml =
            //    "<SiteDetails>" +
            //        "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
            //        "<Name>TestExternalSiteNameZZZ</Name>" +
            //        "<MenuVersion>1</MenuVersion>" +
            //        "<IsOpen>false</IsOpen>" +
            //        "<EstDelivTime>0</EstDelivTime>" +
            //        "<TimeZone>GMT+1</TimeZone>" +
            //        "<Phone>1234567890ZZZ</Phone>" +
            //        "<Address>" +
            //            "<Long>7.6</Long>" +
            //            "<Lat>9.8</Lat>" +
            //            "<Prem1>Test1_Prem1ZZZ</Prem1>" +
            //            "<Prem2>Test1_Prem2ZZZ</Prem2>" +
            //            "<Prem3>Test1_Prem3ZZZ</Prem3>" +
            //            "<Prem4>Test1_Prem4ZZZ</Prem4>" +
            //            "<Prem5>Test1_Prem5ZZZ</Prem5>" +
            //            "<Prem6>Test1_Prem6ZZZ</Prem6>" +
            //            "<Org1>Test1_Org1ZZZ</Org1>" +
            //            "<Org2>Test1_Org2ZZZ</Org2>" +
            //            "<Org3>Test1_Org3ZZZ</Org3>" +
            //            "<RoadNum>Test1_RoadNumZZZ</RoadNum>" +
            //            "<RoadName>Test1_RoadNameZZZ</RoadName>" +
            //            "<Town>Test1_TownZZZ</Town>" +
            //            "<Postcode>Test1_PostCodeZZ</Postcode>" +
            //            "<Dps>ZZZ1</Dps>" +
            //            "<County>Test1_CountyZZZ</County>" +
            //            "<Locality>Test1_LocalityZZZ</Locality>" +
            //            "<Country>United States</Country>" +
            //        "</Address>" +
            //        "<OpeningHours />" +
            //        "<PaymentProvider />" +
            //        "<PaymentClientId />" +
            //        "<PaymentClientPassword />" +
            //    "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationidAAA", "TestExternalSiteIdZZZ", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the xml
            xElement = XElement.Parse(siteDetailsResponse.ResponseText);

            // Check to see if the first site was returned
            errorMessage = XmlHelper.CheckSiteDetails(
                xElement,
                "TestExternalSiteIdZZZ",
                "TestExternalSiteNameZZZ",
                "GMT+1",
                "1234567890ZZZ",
                "7.6",
                "9.8",
                "Test1_Prem1ZZZ",
                "Test1_Prem2ZZZ",
                "Test1_Prem3ZZZ",
                "Test1_Prem4ZZZ",
                "Test1_Prem5ZZZ",
                "Test1_Prem6ZZZ",
                "Test1_Org1ZZZ",
                "Test1_Org2ZZZ",
                "Test1_Org3ZZZ",
                "Test1_RoadNumZZZ",
                "Test1_RoadNameZZZ",
                "Test1_TownZZZ",
                "Test1_PostCodeZZ",
                "ZZZ1",
                "Test1_CountyZZZ",
                "Test1_LocalityZZZ",
                "United States");

            //// Expected site details xml
            //string expectedSite2DetailsXml =
            //    "<SiteDetails>" +
            //        "<SiteId>Test2ExternalSiteId</SiteId>" +
            //        "<Name>Test2ExternalSiteName</Name>" +
            //        "<MenuVersion>1</MenuVersion>" +
            //        "<IsOpen>false</IsOpen>" +
            //        "<EstDelivTime>0</EstDelivTime>" +
            //        "<TimeZone>GMT+4</TimeZone>" +
            //        "<Phone>2222222</Phone>" +
            //        "<Address>" +
            //            "<Long>4.333</Long>" +
            //            "<Lat>2.111</Lat>" +
            //            "<Prem1>Test2_Prem1</Prem1>" +
            //            "<Prem2>Test2_Prem2/Prem2>" +
            //            "<Prem3>Test2_Prem3</Prem3>" +
            //            "<Prem4>Test2_Prem4</Prem4>" +
            //            "<Prem5>Test2_Prem5</Prem5>" +
            //            "<Prem6>Test2_Prem6</Prem6>" +
            //            "<Org1>Test2_Org1</Org1>" +
            //            "<Org2>Test2_Org2</Org2>" +
            //            "<Org3>Test2_Org3</Org3>" +
            //            "<RoadNum>Test2_RoadNum</RoadNum>" +
            //            "<RoadName>Test2_RoadName</RoadName>" +
            //            "<Town>Test2_Town</Town>" +
            //            "<Postcode>Test2_PostCode</Postcode>" +
            //            "<Dps>DPS2</Dps>" +
            //            "<County>Test2_County</County>" +
            //            "<Locality>Test2_Locality</Locality>" +
            //            "<Country>United Kingdom</Country>" +
            //        "</Address>" +
            //        "<OpeningHours />" +
            //        "<PaymentProvider />" +
            //        "<PaymentClientId />" +
            //        "<PaymentClientPassword />" +
            //    "</SiteDetails>";

            // Get the site2 details
            siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationidAAA", "Test2ExternalSiteId", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the xml
            xElement = XElement.Parse(siteDetailsResponse.ResponseText);

            // Check to see if the first site was returned
            errorMessage = XmlHelper.CheckSiteDetails(
                xElement,
                "Test2ExternalSiteId",
                "Test2ExternalSiteName",
                "GMT+4",
                "2222222",
                "4.333",
                "2.111",
                "Test2_Prem1",
                "Test2_Prem2",
                "Test2_Prem3",
                "Test2_Prem4",
                "Test2_Prem5",
                "Test2_Prem6",
                "Test2_Org1",
                "Test2_Org2",
                "Test2_Org3",
                "Test2_RoadNum",
                "Test2_RoadName",
                "Test2_Town",
                "Test2_PostCode",
                "DPS2",
                "Test2_County",
                "Test2_Locality",
                "United Kingdom");
        }

        public void DeleteFirstPartnerApplicationStore(string connectionString)
        {
            string syncXml =
                "<CloudSync>" +
                    "<FromDataVersion>9</FromDataVersion>" +
                    "<ToDataVersion>10</ToDataVersion>" +
                    "<Stores />" +
                    "<Partners>" +
                        "<Partner>" +
                            "<Id>1</Id>" +
                            "<Name>test partner 1AAA</Name>" +
                            "<ExternalId>testpartner1AAA</ExternalId>" +
                            "<Applications>" +
                                "<Application>" +
                                    "<Id>1</Id>" +
                                    "<ExternalApplicationId>TestExternalApplicationidAAA</ExternalApplicationId>" +
                                    "<Name>TestNameAAA</Name>" +
                                    "<Sites>124</Sites>" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            string errorMessage = AcsSyncHelper.ImportSyncXml(syncXml);
            Assert.AreEqual<string>("", errorMessage);

            IDataAccessFactory dataAccess = new EntityFrameworkDataAccessFactory() { ConnectionStringOverride = connectionString };
            string sourceId = "";

            // Expected site list xml
            //string expectedSiteListXml =
            //    "<Sites>" +
            //        "<Site>" +
            //            "<SiteId>TestExternalSiteIdZZZ</SiteId>" +
            //            "<Name>TestExternalSiteNameZZZ</Name>" +
            //            "<MenuVersion>1</MenuVersion>" +
            //            "<IsOpen>false</IsOpen>" +
            //            "<EstDelivTime>0</EstDelivTime>" +
            //        "</Site>" +
            //    "</Sites>";

            // Get the site list
            Response siteListResponse = SiteService.Get("TestExternalApplicationidAAA", "", "", "", null, AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the sites xml
            XElement xElement = XElement.Parse(siteListResponse.ResponseText);

            // Check that the correct number of sites were returned
            var actualSites = (from sites in xElement.Elements("Site")
                               select sites).ToList();
            Assert.AreEqual<int>(1, actualSites.Count);

            // Try and get the fiirst site details.  This should fail
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationidAAA", "TestExternalSiteIdZZZ", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the xml
            Assert.AreEqual<string>(
                "<Error><ErrorCode>1030</ErrorCode><Message>ApplicationId is not authorized to access this siteId</Message></Error>",
                siteDetailsResponse.ResponseText);

            //// Expected site details xml
            //string expectedSite2DetailsXml =
            //    "<SiteDetails>" +
            //        "<SiteId>Test2ExternalSiteId</SiteId>" +
            //        "<Name>Test2ExternalSiteName</Name>" +
            //        "<MenuVersion>1</MenuVersion>" +
            //        "<IsOpen>false</IsOpen>" +
            //        "<EstDelivTime>0</EstDelivTime>" +
            //        "<TimeZone>GMT+4</TimeZone>" +
            //        "<Phone>2222222</Phone>" +
            //        "<Address>" +
            //            "<Long>4.333</Long>" +
            //            "<Lat>2.111</Lat>" +
            //            "<Prem1>Test2_Prem1</Prem1>" +
            //            "<Prem2>Test2_Prem2/Prem2>" +
            //            "<Prem3>Test2_Prem3</Prem3>" +
            //            "<Prem4>Test2_Prem4</Prem4>" +
            //            "<Prem5>Test2_Prem5</Prem5>" +
            //            "<Prem6>Test2_Prem6</Prem6>" +
            //            "<Org1>Test2_Org1</Org1>" +
            //            "<Org2>Test2_Org2</Org2>" +
            //            "<Org3>Test2_Org3</Org3>" +
            //            "<RoadNum>Test2_RoadNum</RoadNum>" +
            //            "<RoadName>Test2_RoadName</RoadName>" +
            //            "<Town>Test2_Town</Town>" +
            //            "<Postcode>Test2_PostCode</Postcode>" +
            //            "<Dps>DPS2</Dps>" +
            //            "<County>Test2_County</County>" +
            //            "<Locality>Test2_Locality</Locality>" +
            //            "<Country>United Kingdom</Country>" +
            //        "</Address>" +
            //        "<OpeningHours />" +
            //        "<PaymentProvider />" +
            //        "<PaymentClientId />" +
            //        "<PaymentClientPassword />" +
            //    "</SiteDetails>";

            // Get the site2 details
            siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationidAAA", "Test2ExternalSiteId", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            // Parse the xml
            xElement = XElement.Parse(siteDetailsResponse.ResponseText);

            // Check to see if the first site was returned
            errorMessage = XmlHelper.CheckSiteDetails(
                xElement,
                "Test2ExternalSiteId",
                "Test2ExternalSiteName",
                "GMT+4",
                "2222222",
                "4.333",
                "2.111",
                "Test2_Prem1",
                "Test2_Prem2",
                "Test2_Prem3",
                "Test2_Prem4",
                "Test2_Prem5",
                "Test2_Prem6",
                "Test2_Org1",
                "Test2_Org2",
                "Test2_Org3",
                "Test2_RoadNum",
                "Test2_RoadName",
                "Test2_Town",
                "Test2_PostCode",
                "DPS2",
                "Test2_County",
                "Test2_Locality",
                "United Kingdom");
        }
    }
}
