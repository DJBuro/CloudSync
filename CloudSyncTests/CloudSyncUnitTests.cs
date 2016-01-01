using System;
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
            string result = SyncHelper.ExportSyncXml(38, 42);
            Console.WriteLine(result);
        }
    }
}
