using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eve
{
    public partial class EveClient : IDisposable
    {
        /// <summary>
        /// Performs an asynchronous PUT request on a document endpoint.
        /// </summary>
        /// <returns>The raw response returned by the the servce.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="obj">Object to be stored on the service.</param>
        public async Task<HttpResponseMessage> PutAsync(string resourceName, object obj)
        {

            ValidateBaseAddress();
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            if (resourceName == string.Empty)
            {
                throw new ArgumentException("resourceName");
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (var client = new HttpClient())
            {
                SettingsForEditing(client, obj);
                HttpResponse = await client.PutAsync(string.Format("{0}/{1}", resourceName, GetDocumentId(obj)), SerializeObject(obj)).ConfigureAwait(false);
                return HttpResponse;
            }
        }

        /// <summary>
        /// Performs an asynchronous PUT request on a document endpoint.
        /// </summary>
        /// <returns>The raw response returned by the service.</returns>
        /// <param name="obj">Object to be stored on the service.</param>
        public async Task<HttpResponseMessage> PutAsync(object obj)
        {
            return await PutAsync(ResourceName, obj);
        }

        /// <summary>
        /// Performs an asynchronous PUT request on a document endpoint.
        /// </summary>
        /// <returns>The instance of the document.</returns>
        /// <param name="resourceName">Resource name.</param>
        /// <param name="obj">Object to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<T> PutAsync<T>(string resourceName, object obj)
        {
            HttpResponse = await PutAsync(resourceName, obj);

            switch (HttpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await DeserializeObject<T>(HttpResponse);
                default:
                    return default(T);
            }
        }

        /// <summary>
        /// Performs an asynchronous PUT request on a document endpoint.
        /// </summary>
        /// <returns>An instance of the document.</returns>
        /// <param name="obj">Object to be stored on the service.</param>
        /// <typeparam name="T">Type of the document.</typeparam>
        public async Task<T> PutAsync<T>(object obj)
        {
            ValidateResourceName();
            return await PutAsync<T>(ResourceName, obj);
        }
    }
}
