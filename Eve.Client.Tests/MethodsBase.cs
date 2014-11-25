using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;

// TODO (in progress) test that exceptions are raised when arguments or properties are null or empty.

namespace Eve.Tests
{
    public class MethodsBase
    {
        // In order to run the test suite you should clone the evenet-testbed repo:
        // https://github.com/nicolaiarocci/Eve.NET-testbed

        // Run the webservice and update the Service const below accordingly. Alternatively 
        // you can run the tests againsts a free test instance which is running on the web 
        // (see below).

        // LOCAL INSTANCE
        // In my case I am running Windows in a VirtualBox VM so in order to access the 
        // OSX Host 'localhost' where a local instance of the REST API is running, I use 
        // the standard 10.0.2.2:5000 uri.
        internal const string Service = "http://10.0.2.2:5000/";

        // REMOTE INSTANCE
        // If you don't have a local Eve.NET-testbed instance then you can opt to run the 
        // tests against the free instance which is available online. Tests will run much 
        // slower though and please, don't overuse it: the webservice runs on very limited 
        // resources and we want everyone to keep enjoying it.
        //internal const string Service = "http://evenet-testbed.herokuapp.com";

        internal const string Endpoint = "companies";
        internal EveClient EveClient;
        internal Company Original;

        [SetUp]
        public void Init()
        {
            // Make sure remote remote endpoint is completely empty.
            // We use the standard HttpClient for this (we aren't testing anything yet).
            var hc = new HttpClient { BaseAddress = new Uri(Service) };
            Assert.IsTrue(hc.DeleteAsync(string.Format("/{0}", Endpoint)).Result.StatusCode == HttpStatusCode.OK);

            // Ok let's roll now.
            EveClient = new EveClient(Service);
        }

        /// <summary>
        /// Validate that two Company instances are equal (properties have same values)
        /// </summary>
        /// <param name="original">First instance.</param>
        /// <param name="result">Second instance.</param>
        public void ValidateAreEquals(Company original, Company result)
        {
            Assert.AreEqual(original.UniqueId, result.UniqueId);
            Assert.AreEqual(original.ETag, result.ETag);
            Assert.AreEqual(original.Created, result.Created);
            Assert.AreEqual(original.Updated, result.Updated);
            Assert.AreEqual(original.Name, result.Name);
            Assert.AreEqual(original.Password, result.Password);
        }

        /// <summary>
        /// Validate that the HttpResponseMessage can be casted to a Company and that its values match the original instance.
        /// </summary>
        /// <param name="responseMessage">HttoResponseMessage.</param>
        /// <param name="original">Original Company instance.</param>
        public void ValidateReturnedHttpResponse(HttpResponseMessage responseMessage, Company original)
        {
            var s = responseMessage.Content.ReadAsStringAsync ().Result;
            var c = JsonConvert.DeserializeObject<Company> (s);
            ValidateReturnedObject(c, original);
        }

        /// <summary>
        /// Validate that a Company instance is valid (has all remote service meta field values) and similar to original instance.
        /// </summary>
        /// <param name="obj">Company instance to be tested.</param>
        /// <param name="original">Similar instance.</param>
        /// <remarks>Since we are only changing Name property values, we only make sure that Name values match.</remarks>
        public void ValidateReturnedObject(Company obj, Company original)
        {
            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<DateTime>(obj.Created);
            Assert.IsInstanceOf<DateTime>(obj.Updated);
            Assert.IsNotNullOrEmpty(obj.UniqueId);
            Assert.IsNotNullOrEmpty(obj.ETag);
            Assert.AreEqual(obj.Name, original.Name);
        }
    }
}
