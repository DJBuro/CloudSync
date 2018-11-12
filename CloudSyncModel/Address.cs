using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSyncModel
{
    public class Address
    {
        public int Id { get; set; }
        public string Org1 { get; set; }
        public string Org2 { get; set; }
        public string Org3 { get; set; }
        public string Prem1 { get; set; }
        public string Prem2 { get; set; }
        public string Prem3 { get; set; }
        public string Prem4 { get; set; }
        public string Prem5 { get; set; }
        public string Prem6 { get; set; }
        public string RoadNum { get; set; }
        public string RoadName { get; set; }
        public string Locality { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string DPS { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public int CountryId { get; set; }
   }
}