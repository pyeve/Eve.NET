using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eve.Tests
{
    [TestClass]
    public class BasicAuthenticator
    {
        [TestMethod] 
        public void BasicAuthenticatorDefaults() { 
            var ba = new Authenticators.BasicAuthenticator("user", "pw");
            Assert.AreEqual(ba.UserName, "user"); 
            Assert.AreEqual(ba.Password, "pw");
        }
    }
}
