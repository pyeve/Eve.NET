using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Eve.Tests
{
    /// <summary>
    ///  Test that GET requests to resource endpoints are properly executed.
    /// </summary>
    [TestFixture]
    class GetMany : MethodsBase
    {
        internal Company Original2;

        [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            Original2 = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name2" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
        }

        [Test]
        public void UseResourceName()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.GetAsync<Company>().Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [Test]
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

        [Test]
        public void AcceptEndpoint()
        {
            var result = EveClient.GetAsync<Company>(Endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.AreEqual(result.Count, 2);
            ValidateAreEquals(Original, result[0]);
            ValidateAreEquals(Original2, result[1]);
        }

        [Test]
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

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task BaseAddessNullException()
        {
            EveClient.BaseAddress = null;
            await EveClient.GetAsync<Company>(Endpoint);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameNullException()
        {
            await EveClient.GetAsync<Company>(resourceName:null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await EveClient.GetAsync<Company>(string.Empty);
        }
    }
}
