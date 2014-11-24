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
        // We are running Windows in a VirtualBox VM so in order to access the OSX Host 'localhost'
        // where a local instance of the REST API is running, we use standard 10.0.2.2:5000
        internal const string Service = "http://10.0.2.2:5000/";
        internal const string Endpoint = "companies";
        internal EveClient EveClient;
        internal Company Original;

        [SetUp]
        public void Init()
        {
            // make sure remote remote endpoint is completely empty
            var hc = new HttpClient { BaseAddress = new Uri(Service) };
            Assert.IsTrue(hc.DeleteAsync(string.Format("/{0}", Endpoint)).Result.StatusCode == HttpStatusCode.OK);

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
