using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudSyncClientTests
{
    public class XmlHelper
    {
        public static string CheckSite(XElement xElement, string siteId, string name)
        {
            var firstSite = (from sites in xElement.Elements("Site")
                             where (string)sites.Element("SiteId").Value == siteId
                             select sites).ToList();

            Assert.AreEqual<int>(1, firstSite.Count);
            XElement firstSiteElement = firstSite[0];

            Assert.AreEqual<string>(siteId, firstSiteElement.Element("SiteId").Value);
            Assert.AreEqual<string>(name, firstSiteElement.Element("Name").Value);
            
            return "";
        }

        public static string CheckSiteDetails(
            XElement xElement, 
            string siteId,
            string name,
            string timeZone,
            string phone,
            string longitude,
            string latitude,
            string prem1,
            string prem2,
            string prem3,
            string prem4,
            string prem5,
            string prem6,
            string org1,
            string org2,
            string org3,
            string roadNum,
            string roadName,
            string town,
            string postcode,
            string dps,
            string county,
            string locality,
            string country)
        {
            Assert.AreEqual<string>(siteId, xElement.Element("SiteId").Value);
            Assert.AreEqual<string>(name, xElement.Element("Name").Value);
            Assert.AreEqual<string>(timeZone, xElement.Element("TimeZone").Value);
            Assert.AreEqual<string>(phone, xElement.Element("Phone").Value);

            var addressXElement = (from address in xElement.Elements("Address")
                                   select address).ToList();

            Assert.AreEqual<int>(1, addressXElement.Count);
            XElement addressElement = addressXElement[0];

            Assert.AreEqual<string>(longitude, addressElement.Element("Long").Value);
            Assert.AreEqual<string>(latitude, addressElement.Element("Lat").Value);
            Assert.AreEqual<string>(prem1, addressElement.Element("Prem1").Value);
            Assert.AreEqual<string>(prem2, addressElement.Element("Prem2").Value);
            Assert.AreEqual<string>(prem3, addressElement.Element("Prem3").Value);
            Assert.AreEqual<string>(prem4, addressElement.Element("Prem4").Value);
            Assert.AreEqual<string>(prem5, addressElement.Element("Prem5").Value);
            Assert.AreEqual<string>(prem6, addressElement.Element("Prem6").Value);
            Assert.AreEqual<string>(org1, addressElement.Element("Org1").Value);
            Assert.AreEqual<string>(org2, addressElement.Element("Org2").Value);
            Assert.AreEqual<string>(org3, addressElement.Element("Org3").Value);
            Assert.AreEqual<string>(roadNum, addressElement.Element("RoadNum").Value);
            Assert.AreEqual<string>(roadName, addressElement.Element("RoadName").Value);
            Assert.AreEqual<string>(town, addressElement.Element("Town").Value);
            Assert.AreEqual<string>(postcode, addressElement.Element("Postcode").Value);
            Assert.AreEqual<string>(dps, addressElement.Element("Dps").Value);
            Assert.AreEqual<string>(county, addressElement.Element("County").Value);
            Assert.AreEqual<string>(locality, addressElement.Element("Locality").Value);
            Assert.AreEqual<string>(country, addressElement.Element("Country").Value);

            return "";
        }
    }
}
