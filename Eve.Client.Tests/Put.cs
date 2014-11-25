using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;

namespace Eve.Tests
{
    /// <summary>
    /// Test that PUT requests to resource endpoints are properly executed.
    /// </summary>
    [TestFixture]
    class Put : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);

            Original.Name = "Another Name";
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var result = EveClient.PutAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptObject()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.PutAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = EveClient.PutAsync(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        public void AcceptObjectReturnHttpResponse()
        {
            EveClient.ResourceName = Endpoint;
            var message = EveClient.PutAsync(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            EveClient.BaseAddress = null;
            await EveClient.PutAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await EveClient.PutAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await EveClient.PutAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await EveClient.PutAsync("resource", null);
        }
    }
}
