using System;
using AndroCloudDataAccess;
using AndroCloudDataAccess.DataAccess;
using AndroCloudDataAccess.Domain;
using AndroCloudDataAccessEntityFramework;
using AndroCloudDataAccessEntityFramework.DataAccess;
using AndroCloudHelper;
using AndroCloudServices.Services;
using CloudSync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            this.FirstPartnerApplicationStore();

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
            Response siteListResponse = SiteService.Get("TestExternalApplicationid", "", "", "", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteListXml,
                siteListResponse.ResponseText,
                "Wrong site list xml.  Expected: " + expectedSiteListXml + " Got: " + siteListResponse.ResponseText);

            // Expected site details xml
            string expectedSiteDetailsXml =
                "<SiteDetails>" +
                    "<SiteId>TestExternalSiteId</SiteId>" +
                    "<Name>TestExternalSiteName</Name>" +
                    "<MenuVersion>1</MenuVersion>" +
                    "<IsOpen>false</IsOpen>" +
                    "<EstDelivTime>0</EstDelivTime>" +
                    "<Phone>1234567890</Phone>" +
                    "<TimeZone>GMT</TimeZone>" +
                    "<Address>" +
                        "<Long>3.4000000953674316</Long>" +
                        "<Lat>1.2000000476837158</Lat>" +
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
                    "<OpeningHours/>" +
                    "<PaymentProvider />" +
                    "<PaymentClientId />" +
                    "<PaymentClientPassword />" +
                "</SiteDetails>";

            // Get the site details
            Response siteDetailsResponse = SiteDetailsService.Get("TestExternalApplicationid", "TestExternalSiteId", AndroCloudHelper.DataTypeEnum.XML, dataAccess, out sourceId);

            Assert.AreEqual<string>(
                expectedSiteDetailsXml,
                siteDetailsResponse.ResponseText,
                "Wrong site details xml.  Expected: " + expectedSiteDetailsXml + " Got: " + siteDetailsResponse.ResponseText);
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
                                "<Town>Test1_Town</Town>" +
                                "<County>Test1_County</County>" +
                                "<State>Test1_State</State>" +
                                "<PostCode>Test1_PostCode</PostCode>" +
                                "<DPS>DPS1</DPS>" +
                                "<Lat>1.2000000476837158</Lat>" +
                                "<Long>3.4000000953674316</Long>" +
                                "<CountryId>234</CountryId>" +
                            "</Address>" +
                        "</Store>" +
                    "</Stores>" +
                    "<Partners />" +
                "</CloudSync>";

            SyncHelper.ImportSyncXml(syncXml);

            // Get the store (we need the id)
            ISiteDataAccess siteDataAccess = new SitesDataAccess();
            if (SyncHelper.ConnectionStringOverride != null) siteDataAccess.ConnectionStringOverride = SyncHelper.ConnectionStringOverride;
            Site site = null;

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

            SyncHelper.ImportSyncXml(syncXml);
        }

        public void FirstPartnerApplication()
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
                                    "<ExternalApplicationId>TestExternalApplicationid</ExternalApplicationId>" +
                                    "<Name>TestName</Name>" +
                                    "<Sites />" +
                                "</Application>" +
                            "</Applications>" +
                        "</Partner>" +
                    "</Partners>" +
                "</CloudSync>";

            SyncHelper.ImportSyncXml(syncXml);
        }

        public void FirstPartnerApplicationStore()
        {
            string syncXml =
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

            SyncHelper.ImportSyncXml(syncXml);
        }
    }
}
