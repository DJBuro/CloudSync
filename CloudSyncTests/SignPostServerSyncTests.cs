using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSync;

namespace CloudSyncTests
{
    [TestClass]
    public class SignPostServerSyncTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            string message = SignpostServerSync.ServerSync();

            Assert.IsNull(message, "ServerSync failed with " + message);
        }
    }
}
