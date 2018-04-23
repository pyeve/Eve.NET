using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Eve.Tests
{
    /// <summary>
    ///  Test that GET requests to resource endpoints are properly executed.
    /// </summary>
    [TestClass]
    public class GetMany : MethodsBase
    {
        internal Company Original2;

        [TestInitialize]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            Original2 = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name2" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
        }

        [TestMethod]
        public void UseResourceName()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.GetAsync<Company>().Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [TestMethod]
        public void UseResourceNameConsiderIms()
        {
            System.Threading.Thread.Sleep(1000);

            // POST in order to get a valid ETag
            var original3 = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name3" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);

            EveClient.ResourceName = Endpoint;
            var result = EveClient.GetAsync<Company>(original3.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 0);

            result = EveClient.GetAsync<Company>(Original2.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);
            ValidateAreEquals(original3, result[0]);
        }

        [TestMethod]
        public void AcceptEndpoint()
        {
            var result = EveClient.GetAsync<Company>(Endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [TestMethod]
        public void AcceptEndpointConsiderIms()
        {
            System.Threading.Thread.Sleep(1000);

            // POST in order to get a valid ETag
            var original3 = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name3" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);

            var result = EveClient.GetAsync<Company>(Endpoint, original3.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 0);

            result = EveClient.GetAsync<Company>(Endpoint, Original2.Updated).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);
            ValidateAreEquals(original3, result[0]);
        }

        [TestMethod]
        public void AcceptEndpointConsiderImsAndSoftDeleteAndQuery()
        {
            System.Threading.Thread.Sleep(1000);

            var rawQuery = @"{""name"": ""Name2""}";
            var result = EveClient.GetAsync<Company>(Endpoint, null, rawQuery: rawQuery).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);

            var original3 = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name3" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);

            rawQuery = @"{""name"": ""Name3""}";
            result = EveClient.GetAsync<Company>(Endpoint, null, rawQuery: rawQuery).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);

            result = EveClient.GetAsync<Company>(Endpoint, original3.Updated, rawQuery).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 0);

            result = EveClient.GetAsync<Company>(Endpoint, Original2.Updated, rawQuery).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 1);

            var r = EveClient.DeleteAsync(Endpoint, original3).Result;
            Assert.AreEqual(HttpStatusCode.NoContent, r.StatusCode);

            result = EveClient.GetAsync<Company>(Endpoint, Original2.Updated, true).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].Deleted);
        }


        [TestMethod]
        public async Task BaseAddessNullException()
        {
            EveClient.BaseAddress = null;

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>(Endpoint);
            });
        }

        [TestMethod]
        public async Task ResourceNameNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>(resourceName: null);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await EveClient.GetAsync<Company>(string.Empty);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }
    }
}
