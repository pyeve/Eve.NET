using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;

namespace Eve.Tests
{
    /// <summary>
    /// Test that POST requests to resource endpoints are properly executed.
    /// </summary>
    [TestFixture]
    class Post : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();
            Original = new Company {Name = "Name"};
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var result = EveClient.PostAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptObject()
        {
            EveClient.ResourceName = Endpoint;
            var result = EveClient.PostAsync<Company>(Original).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
            ValidateReturnedObject(result, Original);
        }

        [Test]
        public void AcceptEndpointAndObjectReturnHttpResponse()
        {
            var message = EveClient.PostAsync(Endpoint, Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        public void AcceptObjectReturnHttpResponse()
        {
            EveClient.ResourceName = Endpoint;
            var message = EveClient.PostAsync(Original).Result;
            ValidateReturnedHttpResponse(message, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            EveClient.BaseAddress = null;
            await EveClient.PostAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await EveClient.PostAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await EveClient.PostAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await EveClient.PostAsync("resource", null);
        }
    }
}
