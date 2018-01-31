using System;
using System.Collections.Generic;
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
            Original = new Company {Name = "Name", Password="pw", StateOrProvince="state"};
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
        [Test]
        public async Task BulkPost()
        {
			var c1 = new Company {Name = "c1", Password ="pw1", StateOrProvince="state1"};
			var c2 = new Company {Name = "c2", Password="pw2", StateOrProvince="state2"};
            var objs = new List<Company> { c1, c2 };

            var retObjs = await EveClient.PostAsync(Endpoint, objs);

            Assert.That(retObjs.Count, Is.EqualTo(2));
            ValidateReturnedObject(retObjs[0], c1);
            ValidateReturnedObject(retObjs[1], c2);
        }
        [Test]
        public async Task BulkPostValidationException()
        {
			var c1 = new Company();
			var c2 = new Company {Name = "c2"};
            var objs = new List<Company> { c1, c2 };

            var retObjs = await EveClient.PostAsync(Endpoint, objs);

            Assert.That(retObjs, Is.Null);
        }
    }
}
