using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eve.Tests
{
    /// <summary>
    /// Test that PUT requests to resource endpoints are properly executed.
    /// </summary>
    [TestClass]
    public class Put : MethodsBase
    {
         [TestInitialize]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name", StateOrProvince="state" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);

            Original.Name = "Another Name";
        }

        [TestMethod]
        public void AcceptEndpointAndObject()
        {
            var result = EveClient.PutAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [TestMethod]
        public void AcceptObject()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.PutAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [TestMethod]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = EveClient.PutAsync(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [TestMethod]
        public void AcceptObjectReturnHttpResponse()
        {
            EveClient.ResourceName = Endpoint;
            var message = EveClient.PutAsync(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [TestMethod]
        public async Task BaseAddressPropertyNullException()
        {
            EveClient.BaseAddress = null;

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PutAsync("resource", Original);
            });
            Assert.IsTrue(ex.Message.Contains("BaseAddress"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PutAsync(null, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await EveClient.PutAsync(string.Empty, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ObjArgumentNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PutAsync("resource", null);
            });
            Assert.IsTrue(ex.Message.Contains("obj"));
        }
    }
}
