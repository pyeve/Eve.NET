using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eve.Tests
{
    /// <summary>
    /// Test that POST requests to resource endpoints are properly executed.
    /// </summary>
    [TestClass]
    public class Post : MethodsBase
    {
         [TestInitialize]
        public void DerivedInit()
        {
            Init();
            Original = new Company {Name = "Name", Password="pw", StateOrProvince="state"};
        }

        [TestMethod]
        public void AcceptEndpointAndObject()
        {
            var result = EveClient.PostAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [TestMethod]
        public void AcceptObject()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.PostAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [TestMethod]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = EveClient.PostAsync(Endpoint, Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [TestMethod]
        public void AcceptObjectReturnHttpResponse()
        {
            EveClient.ResourceName = Endpoint;
            var message = EveClient.PostAsync(Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [TestMethod]
        public async Task BaseAddressPropertyNullException()
        {
            EveClient.BaseAddress = null;

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PostAsync("resource", Original);
            });
            Assert.IsTrue(ex.Message.Contains("BaseAddress"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PostAsync(null, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await EveClient.PostAsync(string.Empty, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ObjArgumentNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PostAsync("resource", null);
            });
            Assert.IsTrue(ex.Message.Contains("obj"));
        }
        [TestMethod]
        public async Task BulkPost()
        {
			var c1 = new Company {Name = "c1", Password ="pw1", StateOrProvince="state1"};
			var c2 = new Company {Name = "c2", Password="pw2", StateOrProvince="state2"};
            var objs = new List<Company> { c1, c2 };

            var retObjs = await EveClient.PostAsync(Endpoint, objs);

            Assert.AreEqual(2, retObjs.Count);
            ValidateReturnedObject(retObjs[0], c1);
            ValidateReturnedObject(retObjs[1], c2);
        }
        [TestMethod]
        public async Task BulkPostValidationException()
        {
			var c1 = new Company();
			var c2 = new Company {Name = "c2"};
            var objs = new List<Company> { c1, c2 };

            var retObjs = await EveClient.PostAsync(Endpoint, objs);

            Assert.IsNull(retObjs);
        }
    }
}
