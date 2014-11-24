using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Eve.Tests
{
    [TestFixture]
    class GetSingle : MethodsBase
    {

        [SetUp]
        public void DerivedInit()
        {
            Init();

            EveClient = new EveClient(Service);

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
        }

        [Test]
        public void AcceptIdReturnHttpResponse()
        {
            var result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void AcceptIdReturnHttpResponseConsiderETag()
        {
            // Get returns NotModified as the etag matches the one on the service.
            var result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode);

            // GET returns OK since the etag does not match the one on the service and all object is retrieved.
            result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [Test]
        public void AcceptIdReturnObject()
        {
            var result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateAreEquals(Original, result);
        }

        [Test]
        public void AcceptIdReturnObjectConsiderETag()
        {
            // GET will return NotModified and null object since etag still matches the one on the service.
            var result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId, Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, EveClient.HttpResponse.StatusCode);
            Assert.IsNull(result);

            // GET will return OK and equal object since etag does not match the one on the service.
            result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId, "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [Test]
        public void AcceptObjectReturnObjectConsiderETag()
        {
            // GET will return NotModified and identical object since etag still matches the one on the service.
            var result = EveClient.GetAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, EveClient.HttpResponse.StatusCode);
            ValidateAreEquals(Original, result);

            // GET will return OK and different object since etag does not match the one on the service.
            Original.ETag = "not really";
            result = EveClient.GetAsync<Company>(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.NotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressNullException()
        {
            EveClient.BaseAddress = null;
            await EveClient.GetAsync<Company>("123", "123");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameNullException()
        {
            await EveClient.GetAsync<Company>(null, "123");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="documentId", MatchType = MessageMatch.Contains)]
        public async Task DocumentIdNullException()
        {
            await EveClient.GetAsync<Company>("123", documentId: null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentNullException()
        {
            await EveClient.GetAsync<Company>(null, new Company());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName", MatchType = MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await EveClient.GetAsync<Company>(string.Empty, new Company());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj", MatchType = MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            var rc = new EveClient(Service);
            await rc.GetAsync<Company>("123", obj: null);
        }
    }
}
