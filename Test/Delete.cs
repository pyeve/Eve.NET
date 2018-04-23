using System;
using System.Threading.Tasks;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eve.Tests
{
	/// <summary>
	/// Test that DELETE requests to document endpoints are properly executed.
	/// </summary>
	[TestClass]
	public class Delete : MethodsBase
	{
		[TestInitialize]
		public void DerivedInit ()
		{
			Init ();

			// POST in order to get a valid ETag
			Original = EveClient.PostAsync<Company> (Endpoint, new Company { Name = "Name" }).Result;
			Assert.AreEqual (HttpStatusCode.Created, EveClient.HttpResponse.StatusCode);
		}

		[TestMethod]
		public void AcceptEndpointAndObject ()
		{
			var message = EveClient.DeleteAsync (Endpoint, Original).Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);

			// confirm that item has been deleted on remote
			message = EveClient.GetAsync (string.Format ("{0}/{1}", Endpoint, Original.UniqueId)).Result;
			Assert.AreEqual (HttpStatusCode.NotFound, message.StatusCode);
		}

		[TestMethod]
		public void AcceptObject ()
		{
			EveClient.ResourceName = Endpoint;
			var message = EveClient.DeleteAsync (Original).Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);

			// confirm that item has been deleted on remote
			message = EveClient.GetAsync (string.Format ("{0}/{1}", Endpoint, Original.UniqueId)).Result;
			Assert.AreEqual (HttpStatusCode.NotFound, message.StatusCode);
		}

		[TestMethod]
		public void DeleteEndpointContent ()
		{
			var message = EveClient.DeleteAsync (Endpoint).Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);

			// confirm that item has been deleted on remote
			var companies = EveClient.GetAsync<Company>(Endpoint).Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);
			Assert.AreEqual (0, companies.Count);
		}
		[TestMethod]
		public void DeleteEndpointContentUsingResourceName ()
		{
			EveClient.ResourceName = Endpoint;
			var message = EveClient.DeleteAsync ().Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);

			// confirm that item has been deleted on remote
			var companies = EveClient.GetAsync<Company>(Endpoint).Result;
			Assert.AreEqual (HttpStatusCode.NoContent, message.StatusCode);
			Assert.AreEqual (0, companies.Count);
		}

		[TestMethod]
		public async Task BaseAddressPropertyNullException ()
		{
			EveClient.BaseAddress = null;

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async ()=>
            {
                await EveClient.DeleteAsync("resource", Original);
            });
            Assert.IsTrue(ex.Message.Contains("BaseAddress"));
		}

		[TestMethod]
		public async Task BaseAddressPropertyNullExceptionAlt ()
		{
			EveClient.BaseAddress = null;
			EveClient.ResourceName = "resource";

            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
               await EveClient.DeleteAsync(Original);
            });
            Assert.IsTrue(ex.Message.Contains("BaseAddress"));
		}

		[TestMethod]
		public async Task ResourceArgumentNameNullException ()
		{
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.DeleteAsync(null, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
		}

		[TestMethod]
		public async Task ResourceNameArgumentException ()
		{
            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await EveClient.DeleteAsync(string.Empty, Original);
            });
            Assert.IsTrue(ex.Message.Contains("resourceName"));
		}

		[TestMethod]
		public async Task ObjArgumentNullException ()
		{
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.DeleteAsync("resource", null);
            });
            Assert.IsTrue(ex.Message.Contains("obj"));
		}

		[TestMethod]
		public async Task ResourceNamePropertyNullException ()
		{
            var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await EveClient.DeleteAsync(Original);
            });
            Assert.IsTrue(ex.Message.Contains("ResourceName"));
		}
	}
}
