using System;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net;

namespace Eve.Tests
{
    /// <summary>
    /// Test that DELETE requests to document endpoints are properly executed.
    /// </summary>
    [TestFixture]
    class Delete : MethodsBase
    {
         [SetUp]
        public void DerivedInit()
        {
            Init();

            // POST in order to get a valid ETag
            Original = EveClient.PostAsync<Company>(Endpoint, new Company { Name = "Name" }).Result;
            Assert.AreEqual(HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
        }

        [Test]
        public void AcceptEndpointAndObject()
        {
            var message = EveClient.DeleteAsync(Endpoint, Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        public void AcceptObject()
        {
            EveClient.ResourceName = Endpoint;
            var message = EveClient.DeleteAsync(Original).Result;
            Assert.AreEqual(HttpStatusCode.OK, message.StatusCode);

            // confirm that item has been deleted on remote
            message = EveClient.GetAsync(string.Format("{0}/{1}", Endpoint, Original.UniqueId)).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullException()
        {
            EveClient.BaseAddress = null;
            await EveClient.DeleteAsync("resource", Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="BaseAddress",MatchType= MessageMatch.Contains)]
        public async Task BaseAddressPropertyNullExceptionAlt()
        {
            EveClient.BaseAddress = null;
            EveClient.ResourceName = "resource";
            await EveClient.DeleteAsync(Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceArgumentNameNullException()
        {
            await EveClient.DeleteAsync(null, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage="resourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNameArgumentException()
        {
            await EveClient.DeleteAsync(string.Empty, Original);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullException()
        {
            await EveClient.DeleteAsync("resource", null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="ResourceName",MatchType= MessageMatch.Contains)]
        public async Task ResourceNamePropertyNullException()
        {
            await EveClient.DeleteAsync(Original);
        }


        [Test]
        [ExpectedException(typeof(ArgumentNullException), ExpectedMessage="obj",MatchType= MessageMatch.Contains)]
        public async Task ObjArgumentNullExceptionAlt()
        {
            EveClient.ResourceName = "resource";
            await EveClient.DeleteAsync(null);
        }

    }
}
