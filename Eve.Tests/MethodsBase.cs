using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Eve.Tests
{
	public class MethodsBase
	{

		internal const string Endpoint = "companies";
		internal EveClient EveClient;
		internal Company Original;
        internal string Service;

		[SetUp]
		public void Init ()
		{
            // In order to run the test suite you need an instance of Eve.NET-testbed running.
            // This is a python application, and you have a few options to run it.

            // 1) Clone the repo at: https://github.com/nicolaiarocci/Eve.NET-testbed, then
            // run the webservice. By default it will run on "http://localhost:5000", so you can 
            // set the EveTestServer environment variable accordingly.

            // In my case, I am running Windows in a VirtualBox VM so in order to access the 
            // OSX Host 'localhost', where a local instance of the REST API is running, I use 
            // the standard "http://10.0.2.2:5000" address. This is the default when no envvar 
            // has been set.

            // 2) If you don't have or don't want to run a local Eve.NET-testbed instance, then 
            // you can run the tests against a remote testbed instance, which is available online. 
            // Tests will be  slower and please, don't overuse this feature: the webservice runs on 
            // very limited resources (for free), and we want everyone to keep enjoying it. To hit
            // the remote test server simply set 'EveTestServer' envvar to "http://evenet-testbed.herokuapp.com";

            Service = Environment.GetEnvironmentVariable("EveTestServer") ?? "http://10.0.2.2:5000";

			// Make sure remote remote endpoint is completely empty.
			// We use the standard HttpClient for this (we aren't testing anything yet).
			var hc = new HttpClient { BaseAddress = new Uri (Service) };
			Assert.IsTrue (hc.DeleteAsync (Endpoint).Result.StatusCode == HttpStatusCode.NoContent);

			// Ok let's roll now.
			EveClient = new EveClient (Service);
		}

		/// <summary>
		/// Validate that two Company instances are equal (properties have same values)
		/// </summary>
		/// <param name="original">First instance.</param>
		/// <param name="result">Second instance.</param>
		public void ValidateAreEquals (Company original, Company result)
		{
			Assert.AreEqual (original.UniqueId, result.UniqueId);
			Assert.AreEqual (original.ETag, result.ETag);
			Assert.AreEqual (original.Created, result.Created);
			Assert.AreEqual (original.Updated, result.Updated);
			Assert.AreEqual (original.Name, result.Name);
			Assert.AreEqual (original.Password, result.Password);
		}

		/// <summary>
		/// Validate that the HttpResponseMessage can be casted to a Company and that its values match the original instance.
		/// </summary>
		/// <param name="responseMessage">HttoResponseMessage.</param>
		/// <param name="original">Original Company instance.</param>
		public void ValidateReturnedHttpResponse (HttpResponseMessage responseMessage, Company original)
		{
			var s = responseMessage.Content.ReadAsStringAsync ().Result;
			var c = JsonConvert.DeserializeObject<Company> (s);
			ValidateReturnedObject (c, original);
		}

		/// <summary>
		/// Validate that a Company instance is valid (has all remote service meta field values) and similar to original instance.
		/// </summary>
		/// <param name="obj">Company instance to be tested.</param>
		/// <param name="original">Similar instance.</param>
		/// <remarks>Since we are only changing Name property values, we only make sure that Name values match.</remarks>
		public void ValidateReturnedObject (Company obj, Company original)
		{
			Assert.IsNotNull (obj);
			Assert.IsInstanceOf<DateTime> (obj.Created);
			Assert.IsInstanceOf<DateTime> (obj.Updated);
			Assert.IsNotNullOrEmpty (obj.UniqueId);
			Assert.IsNotNullOrEmpty (obj.ETag);
			Assert.AreEqual (obj.Name, original.Name);
		}
	}
}
