using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Eve.Tests
{
    /// <summary>
    /// Test that GET requests to document endpoints are properly executed.
    /// </summary>
    [TestClass]
    public class GetSingle : MethodsBase
    {

        [TestInitialize]
        public void DerivedInit()
        {
            Init();

            EveClient = new EveClient(Service);

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
        }

        [TestMethod]
        public void AcceptIdReturnHttpResponse()
        {
            var result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void AcceptIdReturnHttpResponseConsiderETag()
        {
            // Get returns NotModified as the etag matches the one on the service.
            var result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, result.StatusCode);

            // GET returns OK since the etag does not match the one on the service and all object is retrieved.
            result = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId), "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void AcceptIdReturnObject()
        {
            var result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId).Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            ValidateAreEquals(Original, result);
        }

        [TestMethod]
        public void AcceptIdReturnObjectConsiderETag()
        {
            // GET will return NotModified and null object since etag still matches the one on the service.
            var result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId, Original.ETag).Result;
            Assert.AreEqual(HttpStatusCode.NotModified, EveClient.HttpResponse.StatusCode);
            Assert.IsNull(result);

            // GET will return OK and equal object since etag does not match the one on the service.
            result = EveClient.GetAsync<Company>(Endpoint, Original.UniqueId, "not really").Result;
            Assert.AreEqual(HttpStatusCode.OK, EveClient.HttpResponse.StatusCode);
            Assert.IsNotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [TestMethod]
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
            Assert.IsNotNull(result);
            Assert.AreEqual(Original.UniqueId, result.UniqueId);
        }

        [TestMethod]
        public async Task DocumentIdNameNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>(null, new Company());
            });
            Assert.IsTrue(ex.Message.Contains("DocumentId"));
        }

        [TestMethod]
        public async Task BaseAddressNullException()
        {
            EveClient.BaseAddress = null;

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>("123", "123");
            });
            Assert.IsTrue(ex.Message.Contains("BaseAddress"));
        }

        [TestMethod]
        public async Task ResourceNameNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>(null, "123");
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task DocumentIdNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>("123", documentId: null);
            });
            Assert.IsTrue(ex.Message.Contains("documentId"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.GetAsync<Company>(null, new Company { UniqueId = "unique" });
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ResourceNameArgumentException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await EveClient.GetAsync<Company>(string.Empty, new Company { UniqueId = "unique" });
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
        }

        [TestMethod]
        public async Task ObjArgumentNullException()
        {
            var rc = new EveClient(Service);

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await rc.GetAsync<Company>("123", obj: null);
            });
            Assert.IsTrue(ex.Message.Contains("obj"));
        }
        [TestMethod]
        public async Task ETagNullException()
        {
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.PutAsync<Company>("123", new Company());
            });
            Assert.IsTrue(ex.Message.Contains("ETag"));
        }
    }
}
